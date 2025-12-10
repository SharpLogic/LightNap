import { getTestBed } from '@angular/core/testing';
import {
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting,
} from '@angular/platform-browser-dynamic/testing';
import { vi, expect, beforeEach } from 'vitest';

// First, initialize the Angular testing environment.
getTestBed().initTestEnvironment(
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting(),
);

// Add Jasmine-compatible matchers to expect
const jasmineMatcher = {
  toBeTrue: (actual: any) => ({
    message: () => `Expected ${actual} to be true`,
    pass: actual === true,
  }),
  toBeFalse: (actual: any) => ({
    message: () => `Expected ${actual} to be false`,
    pass: actual === false,
  }),
  toBeTruthy: (actual: any) => ({
    message: () => `Expected ${actual} to be truthy`,
    pass: !!actual,
  }),
  toBeFalsy: (actual: any) => ({
    message: () => `Expected ${actual} to be falsy`,
    pass: !actual,
  }),
  toBeUndefined: (actual: any) => ({
    message: () => `Expected ${actual} to be undefined`,
    pass: actual === undefined,
  }),
  toBeNull: (actual: any) => ({
    message: () => `Expected ${actual} to be null`,
    pass: actual === null,
  }),
  toHaveBeenCalled: (actual: any) => ({
    message: () => `Expected spy to have been called`,
    pass: actual.mock?.calls?.length > 0,
  }),
  toHaveBeenCalledWith: (actual: any, ...args: any[]) => ({
    message: () => `Expected spy to have been called with ${JSON.stringify(args)}`,
    pass: actual.mock?.calls?.some((callArgs: any) =>
      JSON.stringify(callArgs) === JSON.stringify(args)
    ) || false,
  }),
};

expect.extend(jasmineMatcher as any);

// Make jasmine available globally for compatibility with existing tests
// This allows tests using jasmine.createSpyObj to work with Vitest
declare global {
  namespace jasmine {
    interface Matchers<T> {
      toBeTrue(): boolean;
      toBeFalse(): boolean;
      toHaveBeenCalled(): boolean;
      toHaveBeenCalledWith(...args: any[]): boolean;
    }
    
    interface SpyObj<T> {
      [K in keyof T]: T[K] extends (...args: any[]) => any
        ? ReturnType<T[K]> extends Promise<infer U>
          ? jasmine.Spy<(...args: any[]) => Promise<U>>
          : jasmine.Spy<T[K]>
        : T[K];
    }
  }
  
  const jasmine: {
    createSpyObj: <T>(baseName: string, methodNames: string[]) => jasmine.SpyObj<T>;
    createSpy: (name: string, originalFn?: any) => jasmine.Spy;
    clock: () => { install: () => void; uninstall: () => void; tick: (ms: number) => void };
  };
}

// Simple Jasmine-compatible spy factory using Vitest's vi module
(globalThis as any).jasmine = {
  createSpyObj: <T>(baseName: string, methodNames: string[]): jasmine.SpyObj<T> => {
    const spy: any = {};
    
    methodNames.forEach(methodName => {
      const viSpy = vi.fn();
      viSpy.and = {
        returnValue: (val: any) => {
          viSpy.mockReturnValue(val);
          return viSpy;
        },
        returnValues: (vals: any[]) => {
          vals.forEach(val => viSpy.mockReturnValueOnce(val));
          return viSpy;
        },
        throwError: (err: any) => {
          viSpy.mockImplementation(() => {
            throw typeof err === 'string' ? new Error(err) : err;
          });
          return viSpy;
        },
        callFake: (fn: any) => {
          viSpy.mockImplementation(fn);
          return viSpy;
        },
        stub: () => {
          viSpy.mockClear();
          return viSpy;
        },
      };
      
      viSpy.toHaveBeenCalled = () => viSpy.mock.calls.length > 0;
      viSpy.toHaveBeenCalledWith = (...args: any[]) =>
        viSpy.mock.calls.some(callArgs =>
          JSON.stringify(callArgs) === JSON.stringify(args)
        );
      
      spy[methodName] = viSpy;
    });
    
    return spy as jasmine.SpyObj<T>;
  },

  createSpy: (name: string, originalFn?: any): jasmine.Spy => {
    const spy = vi.fn(originalFn);
    spy.and = {
      returnValue: (val: any) => {
        spy.mockReturnValue(val);
        return spy;
      },
      returnValues: (vals: any[]) => {
        vals.forEach(val => spy.mockReturnValueOnce(val));
        return spy;
      },
      throwError: (err: any) => {
        spy.mockImplementation(() => {
          throw typeof err === 'string' ? new Error(err) : err;
        });
        return spy;
      },
      callFake: (fn: any) => {
        spy.mockImplementation(fn);
        return spy;
      },
      stub: () => {
        spy.mockClear();
        return spy;
      },
    };
    spy.calls = {
      all: () => spy.mock.calls.map(args => ({ args })),
      count: () => spy.mock.calls.length,
      mostRecent: () => ({
        args: spy.mock.calls[spy.mock.calls.length - 1] || [],
      }),
      reset: () => spy.mockClear(),
      argsFor: (index: number) => spy.mock.calls[index] || [],
      first: () => ({
        args: spy.mock.calls[0] || [],
      }),
    };
    return spy as any;
  },

  clock: () => ({
    install: () => vi.useFakeTimers(),
    uninstall: () => vi.useRealTimers(),
    tick: (ms: number) => vi.advanceTimersByTime(ms),
  }),
};



