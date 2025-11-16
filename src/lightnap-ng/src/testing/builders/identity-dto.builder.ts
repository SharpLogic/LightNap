/**
 * Builder for identity-related DTOs
 *
 * Provides fluent API for creating test identity data
 */

export class IdentityDtoBuilder {
  /**
   * Create a valid JWT token for testing
   * @param expiresInSeconds Optional expiration time in seconds from now
   */
  static createTestToken(expiresInSeconds?: number): string {
    const now = Math.floor(Date.now() / 1000);
    const exp = expiresInSeconds ? now + expiresInSeconds : now + 3600; // Default 1 hour

    // Header and payload
    const header = { alg: 'HS256', typ: 'JWT' };
    const payload = {
      sub: '12345678-1234-1234-1234-123456789012',
      name: 'Test User',
      email: 'test@example.com',
      iat: now,
      exp: exp,
    };

    // Base64 encode (simplified for testing)
    const base64Header = btoa(JSON.stringify(header));
    const base64Payload = btoa(JSON.stringify(payload));
    const signature = 'test-signature';

    return `${base64Header}.${base64Payload}.${signature}`;
  }

  /**
   * Create an expired JWT token for testing
   */
  static createExpiredToken(): string {
    return this.createTestToken(-3600); // Expired 1 hour ago
  }

  /**
   * Create a token with custom claims
   */
  static createTokenWithClaims(claims: Record<string, any>): string {
    const now = Math.floor(Date.now() / 1000);
    const header = { alg: 'HS256', typ: 'JWT' };
    const payload = {
      sub: '12345678-1234-1234-1234-123456789012',
      name: 'Test User',
      email: 'test@example.com',
      iat: now,
      exp: now + 3600,
      ...claims,
    };

    const base64Header = btoa(JSON.stringify(header));
    const base64Payload = btoa(JSON.stringify(payload));
    const signature = 'test-signature';

    return `${base64Header}.${base64Payload}.${signature}`;
  }

  /**
   * Create a LoginRequestDto
   */
  static createLoginRequest(overrides?: Partial<any>): any {
    return {
      userName: 'testuser',
      password: 'Test123!',
      rememberMe: false,
      ...overrides,
    };
  }

  /**
   * Create a RegisterRequestDto
   */
  static createRegisterRequest(overrides?: Partial<any>): any {
    return {
      userName: 'newuser',
      email: 'newuser@example.com',
      password: 'Test123!',
      confirmPassword: 'Test123!',
      ...overrides,
    };
  }

  /**
   * Create a ResetPasswordRequestDto
   */
  static createResetPasswordRequest(overrides?: Partial<any>): any {
    return {
      email: 'user@example.com',
      ...overrides,
    };
  }

  /**
   * Create a NewPasswordRequestDto
   */
  static createNewPasswordRequest(overrides?: Partial<any>): any {
    return {
      token: 'reset-token',
      password: 'NewPass123!',
      confirmPassword: 'NewPass123!',
      ...overrides,
    };
  }

  /**
   * Create a VerifyCodeRequestDto
   */
  static createVerifyCodeRequest(overrides?: Partial<any>): any {
    return {
      code: '123456',
      rememberMe: false,
      ...overrides,
    };
  }

  /**
   * Create a ChangePasswordRequestDto
   */
  static createChangePasswordRequest(overrides?: Partial<any>): any {
    return {
      currentPassword: 'OldPass123!',
      newPassword: 'NewPass123!',
      confirmPassword: 'NewPass123!',
      ...overrides,
    };
  }

  /**
   * Create a LoginSuccessResultDto
   */
  static createLoginSuccessResult(overrides?: Partial<any>): any {
    return {
      accessToken: this.createTestToken(),
      type: 'AccessToken',
      ...overrides,
    };
  }

  /**
   * Create a ClaimDto
   */
  static createClaim(type: string, value: string): any {
    return { type, value };
  }

  /**
   * Create multiple ClaimDtos
   */
  static createClaims(claims: Record<string, string>): any[] {
    return Object.entries(claims).map(([type, value]) => ({ type, value }));
  }
}
