using Microsoft.Extensions.Caching.Hybrid;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HybridCache"/>.
    /// </summary>
    /// <remarks>
    /// Originally from From https://github.com/dotnet/aspnetcore/discussions/57191#discussioncomment-11898121.
    /// </remarks>
    public static class HybridCacheExtensions
    {
        private static readonly HybridCacheEntryOptions _doNotWriteOptions = new()
        {
            Flags = HybridCacheEntryFlags.DisableLocalCacheWrite | HybridCacheEntryFlags.DisableDistributedCacheWrite
        };

        /// <summary>
        /// Returns true if the cache contains an item with a matching key.
        /// </summary>
        /// <param name="cache">An instance of <see cref="HybridCache"/></param>
        /// <param name="key">The name (key) of the item to search for in the cache.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>True if the item exists already. False if it doesn't.</returns>
        /// <remarks>Will never add or alter the state of any items in the cache.</remarks>
        public async static Task<bool> ExistsAsync(this HybridCache cache, string key, CancellationToken cancellationToken = default)
        {
            (var exists, _) = await TryGetValueAsync<object>(cache, key, cancellationToken);
            return exists;
        }

        /// <summary>
        /// Returns true if the cache contains an item with a matching key, along with the value of the matching cache entry.
        /// </summary>
        /// <typeparam name="T">The type of the value of the item in the cache.</typeparam>
        /// <param name="cache">An instance of <see cref="HybridCache"/></param>
        /// <param name="key">The name (key) of the item to search for in the cache.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A tuple of <see cref="bool"/> and the object (if found) retrieved from the cache.</returns>
        /// <remarks>Will never add or alter the state of any items in the cache.</remarks>
        public async static Task<(bool, T?)> TryGetValueAsync<T>(this HybridCache cache, string key, CancellationToken cancellationToken = default)
        {
            var exists = true;

            var result = await cache.GetOrCreateAsync<object, T>(
                key,
                null!,
                (_, _) =>
                {
                    exists = false;
                    return new ValueTask<T>(default(T)!);
                },
                HybridCacheExtensions._doNotWriteOptions,
                null,
                cancellationToken);

            return (exists, result);
        }
    }
}