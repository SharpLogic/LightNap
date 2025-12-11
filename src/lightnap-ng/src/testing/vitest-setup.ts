import { getTestBed } from '@angular/core/testing';
import {
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting,
} from '@angular/platform-browser-dynamic/testing';
import { expect } from 'vitest';

// Initialize the Angular testing environment.
// Note: BrowserDynamicTestingModule is marked as deprecated but is still the best option
// for module-based testing in Angular 21. This will remain necessary until all components
// are converted to standalone, at which point you can remove this entire initialization.
//
// Migration path for future versions:
// 1. Convert components to standalone: @Component({ standalone: true, ... })
// 2. Remove this initTestEnvironment call
// 3. Tests will use standalone components directly in configureTestingModule
getTestBed().initTestEnvironment(
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting(),
);

// Add custom matchers that Vitest doesn't have by default
const customMatchers = {
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
};

expect.extend(customMatchers as any);



