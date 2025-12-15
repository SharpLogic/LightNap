import { vi } from "vitest";

/**
 * General test helper utilities
 */

/**
 * Wait for async operations to complete
 */
export async function flushPromises(): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, 0));
}

/**
 * Create a spy object with mocked methods (Vitest native replacement for jasmine.createSpyObj)
 */
export function createSpyObj<T>(baseName: string, methodNames: string[]): T {
  const spy: any = {};
  methodNames.forEach(methodName => {
    spy[methodName] = vi.fn();
  });
  return spy as T;
}

/**
 * Create a single spy (Vitest native replacement for jasmine.createSpy)
 */
export function createSpy(name: string, fn?: (...args: any[]) => any) {
  return vi.fn().mockName(name);
}

/**
 * Create a partial spy object (only some methods are spies)
 */
export function createPartialSpy<T>(base: Partial<T>, spyMethods: (keyof T)[]): T {
  const spy: any = base ? { ...base } : {};
  spyMethods.forEach(method => {
    spy[method] = vi.fn();
  });
  return spy as T;
}

/**
 * Wait for an observable to emit and return the value
 */
export function firstValueFrom<T>(observable: any): Promise<T> {
  return new Promise((resolve, reject) => {
    const subscription = observable.subscribe({
      next: (value: T) => {
        resolve(value);
        subscription.unsubscribe();
      },
      error: reject,
    });
  });
}

/**
 * Collect all values emitted by an observable
 */
export function collectValues<T>(observable: any, count: number): Promise<T[]> {
  return new Promise((resolve, reject) => {
    const values: T[] = [];
    const subscription = observable.subscribe({
      next: (value: T) => {
        values.push(value);
        if (values.length === count) {
          resolve(values);
          subscription.unsubscribe();
        }
      },
      error: reject,
    });
  });
}

/**
 * Create a mock HttpErrorResponse
 */
export function createHttpError(status: number, message: string = "Error"): any {
  return {
    status,
    statusText: message,
    error: { message },
    ok: false,
  };
}

/**
 * Create a mock ActivatedRouteSnapshot
 */
export function createMockActivatedRouteSnapshot(overrides?: any): any {
  const mockSnapshot: any = {
    params: {},
    queryParams: {},
    data: {},
    url: [],
    parent: null,
    root: null,
    pathFromRoot: [],
    children: [],
    ...overrides,
  };

  // Set root to self if no parent
  if (!mockSnapshot.root) {
    mockSnapshot.root = mockSnapshot;
  }

  return mockSnapshot;
}

/**
 * Create a mock RouterStateSnapshot
 */
export function createMockRouterStateSnapshot(url: string = "/"): any {
  return {
    url,
    root: createMockActivatedRouteSnapshot(),
  };
}

/**
 * Trigger Angular change detection for a component
 */
export function detectChanges(fixture: any): void {
  fixture.detectChanges();
}

/**
 * Query a DOM element from a component fixture
 */
export function queryByCss<T = HTMLElement>(fixture: any, selector: string): T | null {
  return fixture.nativeElement.querySelector(selector);
}

/**
 * Query all DOM elements matching selector
 */
export function queryAllByCss<T = HTMLElement>(fixture: any, selector: string): T[] {
  return Array.from(fixture.nativeElement.querySelectorAll(selector));
}

/**
 * Get text content from a DOM element
 */
export function getTextContent(fixture: any, selector: string): string {
  const element = queryByCss(fixture, selector);
  return element ? element.textContent?.trim() || "" : "";
}

/**
 * Click a button/element in the fixture
 */
export function clickElement(fixture: any, selector: string): void {
  const element = queryByCss<HTMLElement>(fixture, selector);
  if (element) {
    element.click();
    fixture.detectChanges();
  }
}

/**
 * Set input value and trigger input event
 */
export function setInputValue(fixture: any, selector: string, value: string): void {
  const input = queryByCss<HTMLInputElement>(fixture, selector);
  if (input) {
    input.value = value;
    input.dispatchEvent(new Event("input"));
    fixture.detectChanges();
  }
}

/**
 * Wait for a condition to be true
 */
export async function waitFor(condition: () => boolean, timeout: number = 1000, interval: number = 50): Promise<void> {
  const startTime = Date.now();
  while (!condition()) {
    if (Date.now() - startTime > timeout) {
      throw new Error("Timeout waiting for condition");
    }
    await new Promise(resolve => setTimeout(resolve, interval));
  }
}
