namespace CleanAspire.Application.Pipeline;


/// <summary>
/// Pipeline behavior for handling FusionCache refresh requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class FusionCacheRefreshBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFusionCacheRefreshRequest<TResponse>
{
    private readonly IFusionCache _cache;
    private readonly ILogger<FusionCacheRefreshBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FusionCacheRefreshBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The FusionCache instance.</param>
    /// <param name="logger">The logger instance.</param>
    public FusionCacheRefreshBehaviour(
        IFusionCache cache,
        ILogger<FusionCacheRefreshBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request and refreshes the cache if necessary.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next delegate in the pipeline.</returns>
    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType} with details {@Request}", nameof(request), request);
        var response = await next(request, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            await _cache.RemoveAsync(request.CacheKey);
            _logger.LogTrace("Cache key {CacheKey} removed from cache", request.CacheKey);
        }
        if (request.Tags != null && request.Tags.Any())
        {
            foreach (var tag in request.Tags)
            {
                await _cache.RemoveByTagAsync(tag);
                _logger.LogTrace("Cache tag {CacheTag} removed from cache", tag);
            }
        }
        return response;
    }
}
