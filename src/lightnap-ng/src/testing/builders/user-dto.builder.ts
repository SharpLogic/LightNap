/**
 * Builder for user-related DTOs
 * 
 * Provides fluent API for creating test user data
 */

let userIdCounter = 1;

export class UserDtoBuilder {
  /**
   * Create a UserDto with default values
   */
  static createUser(overrides?: Partial<any>): any {
    const id = `user-${userIdCounter++}`;
    return {
      id,
      userName: `testuser${userIdCounter}`,
      email: `user${userIdCounter}@example.com`,
      emailVerified: true,
      roles: [],
      claims: [],
      createdOn: new Date().toISOString(),
      ...overrides,
    };
  }

  /**
   * Create multiple users
   */
  static createUsers(count: number, overrides?: Partial<any>): any[] {
    return Array.from({ length: count }, (_, i) => this.createUser(overrides));
  }

  /**
   * Create a user with specific roles
   */
  static createUserWithRoles(roles: string[], overrides?: Partial<any>): any {
    return this.createUser({ roles, ...overrides });
  }

  /**
   * Create a user with specific claims
   */
  static createUserWithClaims(claims: any[], overrides?: Partial<any>): any {
    return this.createUser({ claims, ...overrides });
  }

  /**
   * Create an admin user
   */
  static createAdminUser(overrides?: Partial<any>): any {
    return this.createUser({
      userName: 'adminuser',
      email: 'admin@example.com',
      roles: ['Administrator'],
      ...overrides,
    });
  }

  /**
   * Create a DeviceDto
   */
  static createDevice(overrides?: Partial<any>): any {
    return {
      id: `device-${Date.now()}`,
      name: 'Test Device',
      os: 'Windows',
      browser: 'Chrome',
      lastUsed: new Date().toISOString(),
      isCurrent: false,
      ...overrides,
    };
  }

  /**
   * Create multiple devices
   */
  static createDevices(count: number, overrides?: Partial<any>): any[] {
    return Array.from({ length: count }, () => this.createDevice(overrides));
  }

  /**
   * Reset the user ID counter (useful for test isolation)
   */
  static resetCounter(): void {
    userIdCounter = 1;
  }
}
