import { DestroyRef, inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { SwUpdate } from "@angular/service-worker";
import { interval } from "rxjs";

@Injectable({
  providedIn: "root",
})
/**
 * @class VersionCheckService
 * @description
 * The VersionCheckService class is responsible for checking and activating application updates using Angular Service Worker.
 */
export class VersionCheckService {
  readonly #updateCheckInterval = 5 * 60 * 1000; // 5 minutes
  readonly #swUpdate = inject(SwUpdate);
  readonly #destroyRef = inject(DestroyRef);

  /**
   * Starts immediate-then-periodic update checks. When a new version is ready, silently
   * activates it and reloads the page so users always run the latest build.
   */
  startUpdateCheck() {
    if (this.#swUpdate.isEnabled) {
      // Check for updates immediately on app startup
      this.#swUpdate
        .checkForUpdate()
        .then(updateAvailable => {
          if (updateAvailable) {
            this.#activateAndReload();
          }
        })
        .catch(err => console.error("Error checking for updates on startup:", err));

      // Silently reload whenever a new version is ready
      this.#swUpdate.versionUpdates.subscribe(event => {
        if (event.type === "VERSION_READY") {
          this.#activateAndReload();
        }
      });

      // Check periodically every 5 minutes
      interval(this.#updateCheckInterval)
        .pipe(takeUntilDestroyed(this.#destroyRef))
        .subscribe(() => {
          this.#swUpdate.checkForUpdate().catch(err =>
            console.error("Error checking for periodic updates:", err)
          );
        });
    }
  }

  #activateAndReload() {
    this.#swUpdate.activateUpdate().then(() => window.location.reload());
  }
}
