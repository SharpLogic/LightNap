using Microsoft.Extensions.Caching.Hybrid;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HybridCache"/>.
    /// </summary>
    public static class HybridCacheExtensions
    {
        private static readonly HybridCacheEntryOptions _doNotWriteOptions = new()
        {
            Flags = HybridCacheEntryFlags.DisableLocalCacheWrite | HybridCacheEntryFlags.DisableDistributedCacheWrite
        };

        extension(HybridCache cache)
        {
            /// <summary>
            /// Retrieves the cached value associated with the specified key, or creates and stores a new value using
            /// the provided factory function if the key does not exist.
            /// </summary>
            /// <remarks>If the key is not found and no factory is provided, the method returns null.
            /// When a factory is supplied, the cache entry is created using the specified options. The factory function
            /// is invoked only if the value is not already cached.</remarks>
            /// <typeparam name="T">The type of the value to retrieve or create in the cache.</typeparam>
            /// <param name="key">The unique key used to identify the cached value.</param>
            /// <param name="factory">An optional asynchronous factory function that generates a new value if the key is not found in the
            /// cache. The function receives a cancellation token to support cancellation.</param>
            /// <param name="options">The options that configure the cache entry, such as expiration and priority. This parameter is required
            /// if a factory function is provided.</param>
            /// <returns>A task that represents the asynchronous operation. The task result contains the cached value associated
            /// with the specified key, or null if the key does not exist and no factory is provided.</returns>
            /// <exception cref="ArgumentException">Thrown if the 'options' parameter is null when a factory function is provided.</exception>
            public async Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>>? factory = null, HybridCacheEntryOptions? options = null)
            {
                if (factory is null)
                {
                    var (_, value) = await cache.TryGetValueAsync<T>(key);
                    return value;
                }

                if (options is null) { throw new ArgumentException("Cache entry options is required if a factory is provided"); }

                return await cache.GetOrCreateAsync(key,
                    (ct) =>
                    {
                        var task = factory(ct);
                        return new ValueTask<T>(task);
                    },
                    options);
            }

            /// <summary>
            /// Returns true if the cache contains an item with a matching key, along with the value of the matching cache entry.
            /// </summary>
            /// <typeparam name="T">The type of the value of the item in the cache.</typeparam>
            /// <param name="key">The name (key) of the item to search for in the cache.</param>
            /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
            /// <returns>A tuple of <see cref="bool"/> and the object (if found) retrieved from the cache.</returns>
            /// <remarks>Will never add or alter the state of any items in the cache.</remarks>
            public async Task<(bool, T?)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
            {
                bool exists = true;

                var result = await cache.GetOrCreateAsync<T>(
                    key,
                    async _ =>
                    {
                        exists = false;
                        return await ValueTask.FromResult(default(T)!);
                    },
                    HybridCacheExtensions._doNotWriteOptions,
                    null,
                    cancellationToken
                );

                return (exists, result);
            }
        }
    }
}
