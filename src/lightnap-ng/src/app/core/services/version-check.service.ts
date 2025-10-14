import { DestroyRef, inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { SwUpdate } from "@angular/service-worker";
import { interval, Subject } from "rxjs";

@Injectable({
  providedIn: "root",
})
/**
 * @class VersionCheckService
 * @description
 * The VersionCheckService class is responsible for checking and activating application updates using Angular Service Worker.
 */
export class VersionCheckService {
  readonly #updateCheckInterval = 5 * 1000 * 1000; // 5 minutes
  readonly #swUpdate = inject(SwUpdate);
  readonly #destroyRef = inject(DestroyRef);

  readonly #versionUpdatedSubject$ = new Subject<boolean>();

  /**
   * Emits when a new version of the application is available.
   */
  readonly versionUpdated$ = this.#versionUpdatedSubject$.asObservable();

  /**
   * Starts periodic update checks to see if this app has been updated on the server.
   * Subscribes to version update events and notifies when a new version is ready.
   */
  startUpdateCheck() {
    if (this.#swUpdate.isEnabled) {
      this.#swUpdate.versionUpdates.subscribe(event => {
        if (event.type === "VERSION_READY") {
          this.#versionUpdatedSubject$.next(true);
        }
      });

      interval(this.#updateCheckInterval)
        .pipe(takeUntilDestroyed(this.#destroyRef))
        .subscribe({ next: () => this.#swUpdate.checkForUpdate() });
    }
  }

  /**
   * Activates the available update and reloads the application to apply the new version.
   */
  activateUpdate() {
    this.#swUpdate.activateUpdate().then(() => window.location.reload());
  }
}
