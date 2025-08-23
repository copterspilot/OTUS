namespace CleanAspire.Application.Common.Interfaces.FusionCache;

/// <summary>
/// Represents a request that supports caching with FusionCache.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IFusionCacheRequest<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Gets the cache key for the request.
    /// </summary>
    string CacheKey => string.Empty;

    /// <summary>
    /// Gets the tags associated with the cache entry.
    /// </summary>
    IEnumerable<string>? Tags { get; }
}
