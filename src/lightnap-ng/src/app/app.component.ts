import { Component, inject, OnInit, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { RouterOutlet } from "@angular/router";
import { BlockUiService } from "@core/services/block-ui.service";
import { VersionCheckService } from "@core/services/version-check.service";
import { BlockUIModule } from "primeng/blockui";
import { PrimeNG } from "primeng/config";
import { DrawerModule } from "primeng/drawer";
import { ToastModule } from "primeng/toast";

@Component({
  standalone: true,
  selector: "app-root",
  templateUrl: "./app.component.html",
  imports: [RouterOutlet, BlockUIModule, DrawerModule, ToastModule],
})
export class AppComponent implements OnInit {
  readonly #primengConfig = inject(PrimeNG);
  readonly #blockUiService = inject(BlockUiService);
  readonly #versionCheckService = inject(VersionCheckService);

  readonly showBlockUi = signal(false);
  readonly blockUiIconClass = signal("pi pi-spin pi-spinner text-4xl");
  readonly blockUiMessage = signal("Processing...");
  readonly showUpdateAvailable = signal(false);

  constructor() {
    this.#blockUiService.onShow$.pipe(takeUntilDestroyed()).subscribe(blockUiParams => {
      this.showBlockUi.set(true);
      this.blockUiMessage.set(blockUiParams.message ?? "Processing...");
      this.blockUiIconClass.set(blockUiParams.iconClass ?? "pi pi-spin pi-spinner text-4xl");
    });

    this.#blockUiService.onHide$.subscribe({
      next: () => this.showBlockUi.set(false),
    });

    this.#versionCheckService.versionUpdated$.pipe(takeUntilDestroyed()).subscribe({
      next: () => this.showUpdateAvailable.set(true),
    });

    this.#versionCheckService.startUpdateCheck();
  }

  ngOnInit() {
    this.#primengConfig.ripple.set(true);
  }

  reloadPage() {
    window.location.reload();
  }
}
