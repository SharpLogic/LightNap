import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

/**
 * Mock ProfileService for testing
 */
@Injectable()
export class MockProfileService {
  getProfile(): Observable<any> {
    return of({
      id: 'mock-user-id',
      userName: 'mockuser',
      email: 'mock@example.com',
    });
  }

  updateProfile(profile: any): Observable<any> {
    return of(profile);
  }

  deleteAccount(): Observable<boolean> {
    return of(true);
  }
}
