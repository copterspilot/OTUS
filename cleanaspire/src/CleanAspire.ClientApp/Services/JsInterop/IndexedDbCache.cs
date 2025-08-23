using System.Text.Json;
using Microsoft.JSInterop;

namespace CleanAspire.ClientApp.Services.JsInterop;

/// <summary>
/// Provides methods to interact with IndexedDB for caching purposes.
/// </summary>
public sealed class IndexedDbCache
{
    /// <summary>
    /// The name of the IndexedDB database.
    /// </summary>
    public const string DATABASENAME = "CleanAspire.IndexedDB";
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedDbCache"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public IndexedDbCache(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Saves data to IndexedDB with optional tags and expiration.
    /// </summary>
    /// <typeparam name="T">The type of the data to save.</typeparam>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="key">The key to identify the data.</param>
    /// <param name="value">The data to save.</param>
    /// <param name="tags">Optional tags to associate with the data.</param>
    /// <param name="expiration">Optional expiration time for the data.</param>
    public async Task SaveDataAsync<T>(string dbName, string key, T value, string[]? tags = null, TimeSpan? expiration = null)
    {
        var expirationMs = expiration.HasValue ? (int)expiration.Value.TotalMilliseconds : (int?)null;
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.saveData", dbName, key, value, tags ?? Array.Empty<string>(), expirationMs);
    }

    /// <summary>
    /// Gets data from IndexedDB or sets it if it does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="key">The key to identify the data.</param>
    /// <param name="factory">The factory function to create the data if it does not exist.</param>
    /// <param name="tags">Optional tags to associate with the data.</param>
    /// <param name="expiration">Optional expiration time for the data.</param>
    /// <returns>The data from the cache or the newly created data.</returns>
    public async Task<T> GetOrSetAsync<T>(string dbName, string key, Func<Task<T>> factory, string[]? tags = null, TimeSpan? expiration = null)
    {
        var existingData = await GetDataAsync<T>(dbName, key);
        if (existingData != null)
        {
            return existingData;
        }

        var newData = await factory();
        await SaveDataAsync(dbName, key, newData, tags, expiration);
        return newData;
    }

    /// <summary>
    /// Gets data from IndexedDB by key.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="key">The key to identify the data.</param>
    /// <returns>The data from the cache.</returns>
    public async Task<T> GetDataAsync<T>(string dbName, string key)
    {
        return await _jsRuntime.InvokeAsync<T>("indexedDbStorage.getData", dbName, key);
    }

    /// <summary>
    /// Gets all data from IndexedDB by tags.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="tags">The tags to filter the data.</param>
    /// <returns>A dictionary of key-value pairs of the data.</returns>
    public async Task<Dictionary<string, T>> GetDataByTagsAsync<T>(string dbName, string[] tags)
    {
        var results = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("indexedDbStorage.getDataByTags", dbName, tags);

        return results.ToDictionary(
            result => result["key"].ToString(),
            result =>
            {
                var jsonElement = result["value"];
                return JsonSerializer.Deserialize<T>(jsonElement.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        );
    }

    /// <summary>
    /// Deletes specific data from IndexedDB by key.
    /// </summary>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="key">The key to identify the data.</param>
    public async Task DeleteDataAsync(string dbName, string key)
    {
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.deleteData", dbName, key);
    }

    /// <summary>
    /// Deletes all data from IndexedDB by tags.
    /// </summary>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="tags">The tags to filter the data.</param>
    public async Task DeleteDataByTagsAsync(string dbName, string[] tags)
    {
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.deleteDataByTags", dbName, tags);
    }

    /// <summary>
    /// Clears all data from the IndexedDB store.
    /// </summary>
    /// <param name="dbName">The name of the database.</param>
    public async Task ClearDataAsync(string dbName)
    {
        await _jsRuntime.InvokeVoidAsync("indexedDbStorage.clearData", dbName);
    }
}
