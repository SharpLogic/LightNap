export class TypeHelpers {
  static cast<T>(value: any): T {
    return value as T;
  }

  static objectsEqual(obj1: any, obj2: any): boolean {
    return JSON.stringify(obj1) === JSON.stringify(obj2);
  }

  static stripUndefined<T>(obj: T): T {
    return JSON.parse(JSON.stringify(obj));
  }

  static rowTrackBy<T>(selector: (item: T) => string) {
    return (_: number, item: T): string => {
      return selector(item);
    };
  }
}
