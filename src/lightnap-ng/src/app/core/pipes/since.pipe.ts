import { DestroyRef, Pipe, PipeTransform, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { TimerService } from "@core/services/timer.service";
import { map, of, startWith } from "rxjs";

@Pipe({
  name: "since",
})
export class SincePipe implements PipeTransform {
  readonly timerInterval = 15 * 1000;
  #destroyRef = inject(DestroyRef);
  #timer = inject(TimerService);

  transform(date: Date | number) {
    if (!date) {
      return of("--");
    }

    return this.#timer.watchTimer$(this.timerInterval).pipe(
      takeUntilDestroyed(this.#destroyRef),
      startWith(() => this.#getText(date)),
      map(() => this.#getText(date))
    );
  }

  #getText(date: Date | number) {
    if (typeof date === "number") {
      date = new Date(date);
    }

    const now = new Date();

    const millis = now.getTime() - date.getTime();

    const minuteMillis: number = 1000 * 60;
    const hourMillis: number = minuteMillis * 60;
    const dayMillis: number = hourMillis * 24;
    const weekMillis: number = dayMillis * 7;

    if (millis < minuteMillis) {
      return `just now`;
    }
    if (millis < hourMillis) {
      return `${Math.floor(millis / minuteMillis)}m ago`;
    }
    if (millis < dayMillis) {
      return `${Math.floor(millis / hourMillis)}h ago`;
    }
    if (millis < weekMillis) {
      return `${Math.floor(millis / dayMillis)}d ago`;
    } else return `${Math.floor(millis / weekMillis)}w ago`;
  }
}
