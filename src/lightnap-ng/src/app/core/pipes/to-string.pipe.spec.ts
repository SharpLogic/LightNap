import { ToStringPipe } from './to-string.pipe';

describe('ToStringPipe', () => {
  let pipe: ToStringPipe;

  beforeEach(() => {
    pipe = new ToStringPipe();
  });

  it('should create', () => {
    expect(pipe).toBeTruthy();
  });

  it('should convert string to string', () => {
    expect(pipe.transform('test')).toBe('test');
  });

  it('should convert number to string', () => {
    expect(pipe.transform(123)).toBe('123');
    expect(pipe.transform(0)).toBe('0');
    expect(pipe.transform(-456)).toBe('-456');
  });

  it('should convert boolean to string', () => {
    expect(pipe.transform(true)).toBe('true');
    expect(pipe.transform(false)).toBe('false');
  });

  it('should handle null and return empty string', () => {
    expect(pipe.transform(null)).toBe('');
  });

  it('should handle undefined and return empty string', () => {
    expect(pipe.transform(undefined)).toBe('');
  });

  it('should convert object with toString to string', () => {
    const obj = { toString: () => 'custom string' };
    expect(pipe.transform(obj)).toBe('custom string');
  });

  it('should convert array to string', () => {
    expect(pipe.transform([1, 2, 3])).toBe('1,2,3');
    expect(pipe.transform([])).toBe('');
  });

  it('should handle Date object', () => {
    const date = new Date('2025-01-01T00:00:00Z');
    const result = pipe.transform(date);
    expect(result).toBeTruthy();
    expect(typeof result).toBe('string');
  });

  it('should convert object to [object Object]', () => {
    const obj = { key: 'value' };
    expect(pipe.transform(obj)).toBe('[object Object]');
  });
});
