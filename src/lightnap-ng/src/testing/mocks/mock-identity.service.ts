import { Injectable } from '@angular/core';
import { ClaimDto } from '@core/backend-api';
import { Observable, of, ReplaySubject } from 'rxjs';

/**
 * Mock IdentityService for testing
 * 
 * Provides a simplified implementation of IdentityService with configurable behavior
 */
@Injectable()
export class MockIdentityService {
  private loggedInSubject$ = new ReplaySubject<boolean>(1);
  private loggedInRolesSubject$ = new ReplaySubject<string[]>(1);
  private loggedInClaimsSubject$ = new ReplaySubject<Map<string, string[]>>(1);
  
  private mockToken?: string;
  private mockClaims = new Map<string, string[]>();
  private mockRoles: string[] = [];

  constructor() {
    this.loggedInSubject$.next(false);
    this.loggedInRolesSubject$.next([]);
    this.loggedInClaimsSubject$.next(new Map());
  }

  /**
   * Configure the mock to simulate a logged-in user
   */
  setLoggedIn(token: string, roles: string[] = [], claims: Map<string, string[]> = new Map()): void {
    this.mockToken = token;
    this.mockRoles = roles;
    this.mockClaims = claims;
    this.loggedInSubject$.next(true);
    this.loggedInRolesSubject$.next(roles);
    this.loggedInClaimsSubject$.next(claims);
  }

  /**
   * Configure the mock to simulate a logged-out user
   */
  setLoggedOut(): void {
    this.mockToken = undefined;
    this.mockRoles = [];
    this.mockClaims = new Map();
    this.loggedInSubject$.next(false);
    this.loggedInRolesSubject$.next([]);
    this.loggedInClaimsSubject$.next(new Map());
  }

  logIn(loginRequest: any): Observable<any> {
    return of({ accessToken: 'mock-token', type: 'AccessToken' });
  }

  logOut(): Observable<boolean> {
    this.setLoggedOut();
    return of(true);
  }

  register(registerRequest: any): Observable<any> {
    return of({ accessToken: 'mock-token', type: 'AccessToken' });
  }

  verifyCode(verifyCodeRequest: any): Observable<string> {
    return of('mock-token');
  }

  resetPassword(resetPasswordRequest: any): Observable<any> {
    return of({});
  }

  newPassword(newPasswordRequest: any): Observable<any> {
    return of({ accessToken: 'mock-token', type: 'AccessToken' });
  }

  requestMagicLinkEmail(request: any): Observable<boolean> {
    return of(true);
  }

  requestVerificationEmail(request: any): Observable<boolean> {
    return of(true);
  }

  verifyEmail(request: any): Observable<boolean> {
    return of(true);
  }

  getDevices(): Observable<any> {
    return of([]);
  }

  revokeDevice(deviceId: string): Observable<any> {
    return of({});
  }

  changePassword(request: any): Observable<any> {
    return of({});
  }

  changeEmail(request: any): Observable<any> {
    return of(true);
  }

  confirmEmailChange(request: any): Observable<any> {
    return of(true);
  }

  getBearerToken(): string | undefined {
    return this.mockToken ? `Bearer ${this.mockToken}` : undefined;
  }

  isTokenExpired(): boolean {
    return !this.mockToken;
  }

  hasUserClaim(claim: ClaimDto): boolean {
    const values = this.mockClaims.get(claim.type);
    return values ? values.includes(claim.value) : false;
  }

  hasAnyUserClaim(claims: ClaimDto[]): boolean {
    return claims.some(claim => this.hasUserClaim(claim));
  }

  hasUserRole(roleName: string): boolean {
    return this.mockRoles.includes(roleName);
  }

  hasAnyUserRole(roleNames: string[]): boolean {
    return roleNames.some(role => this.hasUserRole(role));
  }

  get watchLoggedIn$(): Observable<boolean> {
    return this.loggedInSubject$.asObservable();
  }

  get watchLoggedInRoles$(): Observable<string[]> {
    return this.loggedInRolesSubject$.asObservable();
  }

  get watchLoggedInClaims$(): Observable<Map<string, string[]>> {
    return this.loggedInClaimsSubject$.asObservable();
  }

  watchAnyUserClaim$(claims: ClaimDto[]): Observable<boolean> {
    return of(this.hasAnyUserClaim(claims));
  }

  watchAnyUserRole$(roleNames: string[]): Observable<boolean> {
    return of(this.hasAnyUserRole(roleNames));
  }
}
