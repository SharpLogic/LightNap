import { Injectable } from '@angular/core';
import { Observable, of, ReplaySubject } from 'rxjs';

/**
 * Mock InitializationService for testing
 * 
 * Simulates application initialization state
 */
@Injectable()
export class MockInitializationService {
  private initializedSubject$ = new ReplaySubject<boolean>(1);

  constructor() {
    this.initializedSubject$.next(true);
  }

  get initialized$(): Observable<boolean> {
    return this.initializedSubject$.asObservable();
  }

  setInitialized(value: boolean): void {
    this.initializedSubject$.next(value);
  }

  initialize(): Observable<boolean> {
    this.setInitialized(true);
    return of(true);
  }
}
