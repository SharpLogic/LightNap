/**
 * Custom Jasmine matchers for LightNap testing
 * 
 * Provides domain-specific matchers for clearer test assertions
 */

export interface CustomMatchers<T> extends jasmine.Matchers<T> {
  toHaveBeenCalledWithPartial(expected: any): boolean;
  toBeValidToken(): boolean;
  toBeExpiredToken(): boolean;
  toHaveRole(role: string): boolean;
  toHaveClaim(type: string, value: string): boolean;
}

/**
 * Custom Jasmine matcher implementations
 */
export const customMatchers: jasmine.CustomMatcherFactories = {
  /**
   * Matches if spy was called with object containing expected properties
   */
  toHaveBeenCalledWithPartial: (): jasmine.CustomMatcher => ({
    compare: (actual: jasmine.Spy, expected: any) => {
      if (!actual.calls) {
        return {
          pass: false,
          message: 'Expected a Jasmine spy',
        };
      }

      const calls = actual.calls.all();
      const found = calls.some(call => {
        const arg = call.args[0];
        return Object.keys(expected).every(key => arg[key] === expected[key]);
      });

      return {
        pass: found,
        message: found
          ? `Expected spy not to have been called with partial match`
          : `Expected spy to have been called with partial match of ${JSON.stringify(expected)}`,
      };
    },
  }),

  /**
   * Matches if value is a valid (non-expired) JWT token
   */
  toBeValidToken: (): jasmine.CustomMatcher => ({
    compare: (actual: string) => {
      if (typeof actual !== 'string') {
        return { pass: false, message: 'Expected a string token' };
      }

      const parts = actual.split('.');
      if (parts.length !== 3) {
        return { pass: false, message: 'Expected token with 3 parts' };
      }

      try {
        const payload = JSON.parse(atob(parts[1]));
        const now = Math.floor(Date.now() / 1000);
        const isValid = !payload.exp || payload.exp > now;

        return {
          pass: isValid,
          message: isValid
            ? 'Expected token to be expired'
            : 'Expected token to be valid (not expired)',
        };
      } catch {
        return { pass: false, message: 'Expected valid JWT token' };
      }
    },
  }),

  /**
   * Matches if value is an expired JWT token
   */
  toBeExpiredToken: (): jasmine.CustomMatcher => ({
    compare: (actual: string) => {
      if (typeof actual !== 'string') {
        return { pass: false, message: 'Expected a string token' };
      }

      const parts = actual.split('.');
      if (parts.length !== 3) {
        return { pass: false, message: 'Expected token with 3 parts' };
      }

      try {
        const payload = JSON.parse(atob(parts[1]));
        const now = Math.floor(Date.now() / 1000);
        const isExpired = payload.exp && payload.exp <= now;

        return {
          pass: isExpired,
          message: isExpired
            ? 'Expected token not to be expired'
            : 'Expected token to be expired',
        };
      } catch {
        return { pass: false, message: 'Expected valid JWT token' };
      }
    },
  }),

  /**
   * Matches if user object has the specified role
   */
  toHaveRole: (): jasmine.CustomMatcher => ({
    compare: (actual: any, expected: string) => {
      if (!actual || !Array.isArray(actual.roles)) {
        return { pass: false, message: 'Expected object with roles array' };
      }

      const hasRole = actual.roles.includes(expected);
      return {
        pass: hasRole,
        message: hasRole
          ? `Expected user not to have role "${expected}"`
          : `Expected user to have role "${expected}"`,
      };
    },
  }),

  /**
   * Matches if user object has the specified claim
   */
  toHaveClaim: (): jasmine.CustomMatcher => ({
    compare: (actual: any, expectedType: string, expectedValue: string) => {
      if (!actual || !Array.isArray(actual.claims)) {
        return { pass: false, message: 'Expected object with claims array' };
      }

      const hasClaim = actual.claims.some(
        (c: any) => c.type === expectedType && c.value === expectedValue
      );

      return {
        pass: hasClaim,
        message: hasClaim
          ? `Expected user not to have claim ${expectedType}:${expectedValue}`
          : `Expected user to have claim ${expectedType}:${expectedValue}`,
      };
    },
  }),
};

/**
 * Add custom matchers to the current test suite
 * 
 * Usage in beforeEach:
 * ```typescript
 * beforeEach(() => {
 *   addCustomMatchers();
 * });
 * ```
 */
export function addCustomMatchers(): void {
  jasmine.addMatchers(customMatchers);
}
