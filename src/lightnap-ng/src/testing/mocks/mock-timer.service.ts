import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

/**
 * Mock TimerService for testing
 *
 * Allows manual control of timer emissions
 */
@Injectable()
export class MockTimerService {
  private timerSubject$ = new Subject<number>();
  private tickCount = 0;

  watchTimer$(intervalMillis: number): Observable<number> {
    return this.timerSubject$.asObservable();
  }

  /**
   * Manually emit a timer tick
   */
  tick(): void {
    this.tickCount++;
    this.timerSubject$.next(this.tickCount);
  }

  /**
   * Reset the tick count
   */
  reset(): void {
    this.tickCount = 0;
  }
}
