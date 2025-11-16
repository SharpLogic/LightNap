import { TypeHelpers } from './type-helpers';

describe('TypeHelpers', () => {
  describe('cast', () => {
    it('should cast value to specified type', () => {
      const value: any = { id: 1, name: 'John' };
      interface User {
        id: number;
        name: string;
      }

      const result = TypeHelpers.cast<User>(value);

      expect(result.id).toBe(1);
      expect(result.name).toBe('John');
    });

    it('should cast number to any type', () => {
      const value: any = 42;

      const result = TypeHelpers.cast<number>(value);

      expect(result).toBe(42);
    });

    it('should cast string to any type', () => {
      const value: any = 'test string';

      const result = TypeHelpers.cast<string>(value);

      expect(result).toBe('test string');
    });

    it('should cast array to typed array', () => {
      const value: any = [1, 2, 3];

      const result = TypeHelpers.cast<number[]>(value);

      expect(result).toEqual([1, 2, 3]);
      expect(Array.isArray(result)).toBe(true);
    });

    it('should cast null to any type', () => {
      const value: any = null;

      const result = TypeHelpers.cast<string | null>(value);

      expect(result).toBeNull();
    });

    it('should cast undefined to any type', () => {
      const value: any = undefined;

      const result = TypeHelpers.cast<string | undefined>(value);

      expect(result).toBeUndefined();
    });

    it('should not perform runtime type checking', () => {
      const value: any = 'not a number';

      // Type system says it's a number, but runtime value is string
      const result = TypeHelpers.cast<number>(value);

      // This demonstrates it's just a type assertion, not actual conversion
      expect(typeof result).toBe('string');
    });
  });
});
