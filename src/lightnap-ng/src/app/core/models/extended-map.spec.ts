import { ExtendedMap } from "./extended-map";

describe("ExtendedMap", () => {
  let map: ExtendedMap<string, number>;

  beforeEach(() => {
    map = new ExtendedMap<string, number>();
  });

  describe("getOrSetDefault", () => {
    it("should return existing value when key exists", () => {
      map.set("key1", 42);

      const result = map.getOrSetDefault("key1", () => 100);

      expect(result).toBe(42);
      expect(map.get("key1")).toBe(42);
    });

    it("should set and return default value when key does not exist", () => {
      const result = map.getOrSetDefault("key1", () => 100);

      expect(result).toBe(100);
      expect(map.get("key1")).toBe(100);
    });

    it("should call defaultFactory only when key does not exist", () => {
      const factory = vi.fn().mockReturnValue(200);

      map.getOrSetDefault("key1", factory);
      map.getOrSetDefault("key1", factory);

      expect(factory).toHaveBeenCalledTimes(1);
    });
  });

  describe("filter", () => {
    beforeEach(() => {
      map.set("a", 1);
      map.set("b", 2);
      map.set("c", 3);
      map.set("d", 4);
    });

    it("should return new ExtendedMap with filtered entries", () => {
      const result = map.filter(value => value > 2);

      expect(result).toBeInstanceOf(ExtendedMap);
      expect(result.size).toBe(2);
      expect(result.get("c")).toBe(3);
      expect(result.get("d")).toBe(4);
      expect(result.has("a")).toBe(false);
      expect(result.has("b")).toBe(false);
    });

    it("should return empty map when no entries match", () => {
      const result = map.filter(() => false);

      expect(result.size).toBe(0);
    });

    it("should return map with all entries when all match", () => {
      const result = map.filter(() => true);

      expect(result.size).toBe(4);
      expect(result.get("a")).toBe(1);
      expect(result.get("b")).toBe(2);
      expect(result.get("c")).toBe(3);
      expect(result.get("d")).toBe(4);
    });

    it("should pass value and key to predicate", () => {
      const predicate = vi.fn().mockImplementation((value: number, key: string) => key === "b");

      const result = map.filter(predicate);

      expect(predicate).toHaveBeenCalledWith(1, "a");
      expect(predicate).toHaveBeenCalledWith(2, "b");
      expect(predicate).toHaveBeenCalledWith(3, "c");
      expect(predicate).toHaveBeenCalledWith(4, "d");
      expect(result.size).toBe(1);
      expect(result.get("b")).toBe(2);
    });

    it("should not modify original map", () => {
      map.filter(value => value > 2);

      expect(map.size).toBe(4);
      expect(map.get("a")).toBe(1);
    });
  });
});
