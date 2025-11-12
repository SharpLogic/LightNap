import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

/**
 * Mock IdentityDataService for testing
 * 
 * Returns mock HTTP responses for identity-related operations
 */
@Injectable()
export class MockIdentityDataService {
  getAccessToken(): Observable<string> {
    return of('');
  }

  logIn(loginRequest: any): Observable<any> {
    return of({ accessToken: 'mock-access-token', type: 'AccessToken' });
  }

  logOut(): Observable<boolean> {
    return of(true);
  }

  register(registerRequest: any): Observable<any> {
    return of({ accessToken: 'mock-access-token', type: 'AccessToken' });
  }

  resetPassword(resetPasswordRequest: any): Observable<boolean> {
    return of(true);
  }

  newPassword(newPasswordRequest: any): Observable<any> {
    return of({ accessToken: 'mock-access-token', type: 'AccessToken' });
  }

  verifyCode(verifyCodeRequest: any): Observable<string> {
    return of('mock-access-token');
  }

  requestVerificationEmail(request: any): Observable<boolean> {
    return of(true);
  }

  verifyEmail(request: any): Observable<boolean> {
    return of(true);
  }

  requestMagicLinkEmail(request: any): Observable<boolean> {
    return of(true);
  }

  changePassword(request: any): Observable<boolean> {
    return of(true);
  }

  changeEmail(request: any): Observable<boolean> {
    return of(true);
  }

  confirmEmailChange(request: any): Observable<boolean> {
    return of(true);
  }

  getDevices(): Observable<any[]> {
    return of([]);
  }

  revokeDevice(deviceId: string): Observable<boolean> {
    return of(true);
  }
}
