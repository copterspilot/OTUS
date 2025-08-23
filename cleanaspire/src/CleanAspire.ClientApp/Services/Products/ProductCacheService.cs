// This class provides caching services for product data, enabling efficient retrieval, storage, and offline mode support using IndexedDbCache.

// Purpose:
// 1. **Caching Product Data**:
//    - Saves or retrieves individual products and paginated product lists.
//    - Supports offline operations by caching product-related commands (create, update, delete).

// 2. **Offline Mode**:
//    - Stores commands (create, update, delete) in cache when offline, ensuring data consistency when reconnecting to the server.
//    - Enables synchronization of cached commands with the server upon re-establishing connectivity.

// 3. **Data Integrity**:
//    - Updates cached data to reflect deletions and changes, maintaining consistency between cache and server state.

using CleanAspire.Api.Client.Models;
using CleanAspire.ClientApp.Services.JsInterop;


namespace CleanAspire.ClientApp.Services.Products;

/// <summary>
/// This class provides caching services for product data, enabling efficient retrieval, storage, and offline mode support using IndexedDbCache.
/// </summary>
public class ProductCacheService
{
    private readonly IndexedDbCache _cache;
    public const string DATABASENAME = IndexedDbCache.DATABASENAME;
    private const string OFFLINE_CREATE_COMMAND_CACHE_KEY = "OfflineCreateCommand:product";
    private const string OFFLINE_UPDATE_COMMAND_CACHE_KEY = "OfflineUpdateCommand:product";
    private const string OFFLINE_DELETE_COMMAND_CACHE_KEY = "OfflineDeleteCommand:product";
    private const string OFFLINE_PAGINATION_CACHE_TAG = "OfflinePagination:";
    private const string PAGINATION_TAG = "products_pagination";
    private const string PAGINATION_CACHE_TAG = "products_pagination_cache";
    private const string COMMANDS_TAG = "product_commands";
    private const string OBJECT_TAG = "product";

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductCacheService"/> class.
    /// </summary>
    /// <param name="cache">The IndexedDbCache instance to use for caching operations.</param>
    public ProductCacheService(IndexedDbCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Saves or updates a product in the cache.
    /// </summary>
    /// <param name="product">The product to save or update.</param>
    public async Task SaveOrUpdateProductAsync(ProductDto product)
    {
        var productCacheKey = GenerateProductCacheKey(product.Id);
        await _cache.SaveDataAsync(DATABASENAME, productCacheKey, product, new[] { OBJECT_TAG });
    }

    /// <summary>
    /// Retrieves a product from the cache.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <returns>The cached product, or null if not found.</returns>
    public async Task<ProductDto?> GetProductAsync(string productId)
    {
        var productCacheKey = GenerateProductCacheKey(productId);
        return await _cache.GetDataAsync<ProductDto>(DATABASENAME, productCacheKey);
    }

    /// <summary>
    /// Caches paginated product data.
    /// </summary>
    /// <param name="cacheKey">The cache key for the paginated data.</param>
    /// <param name="data">The paginated product data to cache.</param>
    public async Task SaveOrUpdatePaginatedProductsAsync(string cacheKey, PaginatedResultOfProductDto data)
    {
        await _cache.SaveDataAsync(DATABASENAME, cacheKey, data, new[] { PAGINATION_TAG });
    }

    /// <summary>
    /// Retrieves paginated product data from the cache.
    /// </summary>
    /// <param name="cacheKey">The cache key for the paginated data.</param>
    /// <returns>The cached paginated product data, or null if not found.</returns>
    public async Task<PaginatedResultOfProductDto?> GetPaginatedProductsAsync(string cacheKey)
    {
        return await _cache.GetDataAsync<PaginatedResultOfProductDto>(DATABASENAME, cacheKey);
    }

    /// <summary>
    /// Retrieves all cached paginated product results.
    /// </summary>
    /// <returns>A dictionary of cached paginated product results.</returns>
    public async Task<Dictionary<string, PaginatedResultOfProductDto>> GetAllCachedPaginatedResultsAsync()
    {
        var cachedData = await _cache.GetDataByTagsAsync<PaginatedResultOfProductDto>(
                DATABASENAME, new[] { PAGINATION_TAG }
            );
        return cachedData ?? new Dictionary<string, PaginatedResultOfProductDto>();
    }

    /// <summary>
    /// Deletes a product from the cache.
    /// </summary>
    /// <param name="productId">The ID of the product to delete.</param>
    public async Task DeleteProductAsync(string productId)
    {
        var productCacheKey = GenerateProductCacheKey(productId);
        await _cache.DeleteDataAsync(DATABASENAME, productCacheKey);
    }

    /// <summary>
    /// Updates cached data after deleting products.
    /// </summary>
    /// <param name="productIds">The list of product IDs to delete.</param>
    public async Task UpdateDeletedProductsAsync(List<string> productIds)
    {
        foreach (var pid in productIds)
        {
            await DeleteProductAsync(pid);
        }
        var cachedPaginatedProducts = await _cache
            .GetDataByTagsAsync<PaginatedResultOfProductDto>(DATABASENAME, new[] { PAGINATION_TAG });

        if (cachedPaginatedProducts != null && cachedPaginatedProducts.Any())
        {
            foreach (var kvp in cachedPaginatedProducts)
            {
                var key = kvp.Key;
                var paginatedProducts = kvp.Value;
                paginatedProducts.Items = paginatedProducts.Items
                    .Where(p => !productIds.Contains(p.Id))
                    .ToList();

                paginatedProducts.TotalItems = paginatedProducts.Items.Count;
                await _cache.SaveDataAsync(
                    DATABASENAME,
                    key,
                    paginatedProducts,
                    new[] { PAGINATION_TAG }
                );
            }
        }
    }

    /// <summary>
    /// Caches an offline create command for later synchronization.
    /// </summary>
    /// <param name="command">The create command to cache.</param>
    public async Task StoreOfflineCreateCommandAsync(CreateProductCommand command)
    {
        var cached = await _cache.GetDataAsync<List<CreateProductCommand>>(
            DATABASENAME, OFFLINE_CREATE_COMMAND_CACHE_KEY
        ) ?? new List<CreateProductCommand>();

        cached.Add(command);
        await _cache.SaveDataAsync(DATABASENAME, OFFLINE_CREATE_COMMAND_CACHE_KEY, cached, new[] { COMMANDS_TAG });
    }

    /// <summary>
    /// Caches an offline update command for later synchronization.
    /// </summary>
    /// <param name="command">The update command to cache.</param>
    public async Task StoreOfflineUpdateCommandAsync(UpdateProductCommand command)
    {
        var cached = await _cache.GetDataAsync<List<UpdateProductCommand>>(
            DATABASENAME, OFFLINE_UPDATE_COMMAND_CACHE_KEY
        ) ?? new List<UpdateProductCommand>();

        cached.Add(command);
        await _cache.SaveDataAsync(DATABASENAME, OFFLINE_UPDATE_COMMAND_CACHE_KEY, cached, new[] { COMMANDS_TAG });
    }

    /// <summary>
    /// Caches an offline delete command for later synchronization.
    /// </summary>
    /// <param name="command">The delete command to cache.</param>
    public async Task StoreOfflineDeleteCommandAsync(DeleteProductCommand command)
    {
        var cached = await _cache.GetDataAsync<List<DeleteProductCommand>>(
            DATABASENAME, OFFLINE_DELETE_COMMAND_CACHE_KEY
        ) ?? new List<DeleteProductCommand>();

        cached.Add(command);
        await _cache.SaveDataAsync(DATABASENAME, OFFLINE_DELETE_COMMAND_CACHE_KEY, cached, new[] { COMMANDS_TAG });
    }

    /// <summary>
    /// Retrieves all pending offline commands.
    /// </summary>
    /// <returns>A tuple containing the count of all pending commands and lists of create, update, and delete commands.</returns>
    public async Task<Tuple<int, List<CreateProductCommand>, List<UpdateProductCommand>, List<DeleteProductCommand>>> GetAllPendingCommandsAsync()
    {
        var createCommands = await _cache.GetDataAsync<List<CreateProductCommand>>(
            IndexedDbCache.DATABASENAME, OFFLINE_CREATE_COMMAND_CACHE_KEY
        ) ?? new List<CreateProductCommand>();

        var updateCommands = await _cache.GetDataAsync<List<UpdateProductCommand>>(
            IndexedDbCache.DATABASENAME, OFFLINE_UPDATE_COMMAND_CACHE_KEY
        ) ?? new List<UpdateProductCommand>();

        var deleteCommands = await _cache.GetDataAsync<List<DeleteProductCommand>>(
            IndexedDbCache.DATABASENAME, OFFLINE_DELETE_COMMAND_CACHE_KEY
        ) ?? new List<DeleteProductCommand>();

        return new Tuple<int, List<CreateProductCommand>, List<UpdateProductCommand>, List<DeleteProductCommand>>(
            createCommands.Count + updateCommands.Count + deleteCommands.Count,
            createCommands,
            updateCommands,
            deleteCommands
        );
    }

    /// <summary>
    /// Clears all cached offline commands.
    /// </summary>
    public async Task ClearCommands()
    {
        await _cache.DeleteDataAsync(DATABASENAME, OFFLINE_CREATE_COMMAND_CACHE_KEY);
        await _cache.DeleteDataAsync(DATABASENAME, OFFLINE_UPDATE_COMMAND_CACHE_KEY);
        await _cache.DeleteDataAsync(DATABASENAME, OFFLINE_DELETE_COMMAND_CACHE_KEY);
    }

    /// <summary>
    /// Generates a unique cache key for a product.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>The generated cache key.</returns>
    private string GenerateProductCacheKey(string productId)
    {
        return $"{nameof(ProductDto)}:{productId}";
    }

    /// <summary>
    /// Generates a unique cache key for paginated product data.
    /// </summary>
    /// <param name="query">The query parameters for pagination.</param>
    /// <returns>The generated cache key.</returns>
    public string GeneratePaginationCacheKey(ProductsWithPaginationQuery query)
    {
        return $"{nameof(ProductsWithPaginationQuery)}:{query.PageNumber}_{query.PageSize}_{query.Keywords}_{query.OrderBy}_{query.SortDirection}";
    }
}
