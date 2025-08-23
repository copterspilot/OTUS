namespace CleanAspire.Application.Common.Interfaces.FusionCache;

/// <summary>
/// Represents a request to refresh a cache entry in FusionCache.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IFusionCacheRefreshRequest<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Gets the cache key associated with the request.
    /// </summary>
    string CacheKey => string.Empty;

    /// <summary>
    /// Gets the tags associated with the cache entry.
    /// </summary>
    IEnumerable<string>? Tags { get; }
}
