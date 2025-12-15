import { DestroyRef, inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { interval, Observable, Subject } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class TimerService {
  #destroyRef = inject(DestroyRef);
  #timers: { [index: number]: { subject: Subject<number>; observable$: Observable<number> } } = {};

  watchTimer$(milliseconds: number) {
    if (milliseconds < 0) {
      throw new Error(`Intervals must be positive: '${milliseconds}' not valid`);
    }

    if (!this.#timers[milliseconds]) {
      const subject = new Subject<number>();
      this.#timers[milliseconds] = { subject, observable$: subject.asObservable() };

      interval(milliseconds)
        .pipe(takeUntilDestroyed(this.#destroyRef))
        .subscribe({
          next: () => this.#timers[milliseconds].subject.next(milliseconds),
        });
    }
    return this.#timers[milliseconds].observable$;
  }
}
