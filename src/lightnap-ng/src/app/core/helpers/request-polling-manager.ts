import { Observable, Subject, timer } from "rxjs";
import { finalize, take, takeUntil } from "rxjs/operators";

/**
 * @class RequestPollingManager
 * @description
 * The RequestPollingManager class provides methods to manage polling requests.
 */
export class RequestPollingManager<T> {
  #stopPolling$ = new Subject<void>();
  #isPaused = false;

  constructor(
    private pollingFn: () => Observable<T>,
    private intervalMillis: number
  ) {}

  /**
   * Starts the polling process with an optional initial delay.
   *
   * @param due - The initial delay before starting the polling, in milliseconds. Defaults to 0, which means the method will be called immediately.
   *
   * This method sets up a timer that triggers the polling function at regular intervals. Note that the interval timer is reset after each polling
   * function call completes once. If the observable emits more than one value, only the first is taken.
   *
   * The polling process will stop when the `stopPolling()` or `pausePolling()` are called.
   */
  startPolling(due = 0) {
    this.#isPaused = false;

    timer(due, this.intervalMillis)
      .pipe(takeUntil(this.#stopPolling$), take(1))
      .subscribe({
        next: () => {
          this.pollingFn()
            .pipe(
              take(1),
              finalize(() => {
                if (!this.#isPaused) {
                  this.resumePolling();
                }
              })
            )
            .subscribe();
        },
      });
  }

  /**
   * Stops the polling process. Note that this has the same behavior as `pausePolling()`.
   */
  stopPolling() {
    this.pausePolling();
  }

  /**
   * Pauses the polling process. Note that this has the same behavior as `stopPolling()`.
   */
  pausePolling() {
    this.#isPaused = true;
    this.#stopPolling$.next();
  }

  /**
   * Resumes the polling process. This has the same effect as calling `startPolling()` with the timer interval.
   */
  resumePolling() {
    // Make sure we stop any pending timers.
    this.pausePolling();
    this.startPolling(this.intervalMillis);
  }
}
