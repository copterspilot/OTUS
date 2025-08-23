namespace CleanAspire.Application.Pipeline;

/// <summary>
/// Pipeline behavior for handling requests with FusionCache.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class FusionCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFusionCacheRequest<TResponse>
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<FusionCacheBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FusionCacheBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="fusionCache">The FusionCache instance.</param>
    /// <param name="logger">The logger instance.</param>
    public FusionCacheBehaviour(
        IFusionCache fusionCache,
        ILogger<FusionCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _fusionCache = fusionCache;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request by attempting to retrieve the response from the cache, or invoking the next handler if not found.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="next">The next handler delegate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response instance.</returns>
    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling request of type {RequestType} with cache key {CacheKey}", nameof(request), request.CacheKey);
        var response = await _fusionCache.GetOrSetAsync<TResponse>(
            request.CacheKey,
            async (ctx, token) => await next(request, token),
            tags: request.Tags
            ).ConfigureAwait(false);

        return response;
    }
}
