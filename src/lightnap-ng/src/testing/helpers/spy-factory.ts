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
 *   const spy = jasmine.createSpyObj<any>("Service", [
 *     "method1",
 *     "method2",
 *     // ❌ No type checking, must list methods manually
 *   ]);
 *
 * Use a strongly-typed spy helper:
 *
 *   const spy = createLightNapWebApiServiceSpy(jasmine);
 *   // ✅ Full TypeScript checking
 *   // ✅ Autocomplete for all methods
 *   // ✅ Fails compile-time if service changes
 *   // ✅ No manual maintenance
 */

import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";

/**
 * Creates a strongly-typed spy object for LightNapWebApiService with all methods
 * configured to throw by default if called without being overridden.
 *
 * @param jasmineInstance - The Jasmine instance
 * @returns A fully typed SpyObj with all methods throwing by default
 *
 * @example
 * const spy = createLightNapWebApiServiceSpy(jasmine);
 * spy.getUser.and.returnValue(of(mockUser)); // Override for this test
 * expect(spy.getUser).toHaveBeenCalledWith("123");
 */
export function createLightNapWebApiServiceSpy(
  jasmineInstance: any
): jasmine.SpyObj<LightNapWebApiService> {
  const methodNames = Object.getOwnPropertyNames(LightNapWebApiService.prototype)
    .filter(
      name =>
        name !== "constructor" &&
        !name.startsWith("_") &&
        !name.startsWith("#") &&
        typeof (LightNapWebApiService.prototype as any)[name] === "function"
    )
    .sort();

  const spy = jasmineInstance.createSpyObj(
    "LightNapWebApiService",
    methodNames
  ) as jasmine.SpyObj<LightNapWebApiService>;

  // Configure all methods to throw by default
  methodNames.forEach(methodName => {
    spy[methodName as keyof LightNapWebApiService].and.throwError(
      `${methodName} was called but not configured in test`
    );
  });

  return spy;
}
