/**
 * Strongly-typed spy factory for services.
 *
 * This generates spy objects with full TypeScript type safety, ensuring
 * that spies are always in sync with their service interface.
 *
 * USAGE:
 * ------
 *
 * Instead of using untyped spies:
 *
 *   const spy = {
 *     method1: vi.fn(),
 *     method2: vi.fn(),
 *     // ❌ No type checking, must list methods manually
 *   };
 *
 * Use a strongly-typed spy helper:
 *
 *   const spy = createLightNapWebApiServiceSpy();
 *   // ✅ Full TypeScript checking
 *   // ✅ Autocomplete for all methods
 *   // ✅ Fails compile-time if service changes
 *   // ✅ No manual maintenance
 */

import { vi } from "vitest";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";

/**
 * Creates a strongly-typed spy object for LightNapWebApiService with all methods
 * configured to throw by default if called without being overridden.
 *
 * @returns A fully typed spy object with all methods throwing by default
 *
 * @example
 * const spy = createLightNapWebApiServiceSpy();
 * spy.getUser.mockReturnValue(of(mockUser)); // Override for this test
 * expect(spy.getUser).toHaveBeenCalledWith("123");
 */
export function createLightNapWebApiServiceSpy(): Partial<LightNapWebApiService> {
  const methodNames = Object.getOwnPropertyNames(LightNapWebApiService.prototype)
    .filter(
      name =>
        name !== "constructor" &&
        !name.startsWith("_") &&
        !name.startsWith("#") &&
        typeof (LightNapWebApiService.prototype as any)[name] === "function"
    )
    .sort();

  const spy: any = {};

  // Configure all methods to throw by default
  methodNames.forEach(methodName => {
    spy[methodName] = vi.fn(() => {
      throw new Error(`${methodName} was called but not configured in test`);
    });
  });

  return spy as Partial<LightNapWebApiService>;
}
