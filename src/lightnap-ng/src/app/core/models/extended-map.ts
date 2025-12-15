/**
 * A class that extends the native Map object with additional utility methods.
 *
 * @template K - The type of keys in the map.
 * @template V - The type of values in the map.
 */
export class ExtendedMap<K, V> extends Map<K, V> {
  /**
   * Retrieves the value associated with the specified key, or sets it to a default value if the key does not exist.
   *
   * @param key - The key whose associated value is to be returned or set.
   * @param defaultFactory - A function that produces the default value if the key does not exist.
   * @returns The value associated with the specified key, or the newly set default value.
   */
  getOrSetDefault(key: K, defaultFactory: () => V) {
    if (!this.has(key)) {
      this.set(key, defaultFactory());
    }
    return this.get(key)!;
  }

  /**
   * Creates a new ExtendedMap containing only the entries that satisfy the provided predicate function.
   *
   * @param predicate - A function that tests each entry for a condition.
   * It receives the value and key as arguments.
   * @returns A new ExtendedMap containing only the entries that satisfy the predicate.
   */
  filter(predicate: (v: V, k: K) => boolean) {
    const newMap = new ExtendedMap<K, V>();
    const entries = Array.from(this.entries());
    for (const [key, value] of entries) {
      if (predicate(value, key)) {
        newMap.set(key, value);
      }
    }
    return newMap;
  }
}
