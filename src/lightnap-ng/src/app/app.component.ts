import { Component, inject, OnInit, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { RouterOutlet } from "@angular/router";
import { BlockUiService } from "@core";
import { BlockUIModule } from "primeng/blockui";
import { PrimeNG } from "primeng/config";
import { ToastModule } from "primeng/toast";

@Component({
  standalone: true,
  selector: "app-root",
  templateUrl: "./app.component.html",
  imports: [RouterOutlet, BlockUIModule, ToastModule]
})
export class AppComponent implements OnInit {
  #primengConfig = inject(PrimeNG);
  #blockUiService = inject(BlockUiService);

  showBlockUi = signal(false);
  blockUiIconClass = signal("pi pi-spin pi-spinner text-4xl");
  blockUiMessage = signal("Processing...");

  constructor() {
    this.#blockUiService.onShow$.pipe(takeUntilDestroyed()).subscribe(blockUiParams => {
      this.showBlockUi.set(true);
      this.blockUiMessage.set(blockUiParams.message ?? "Processing...");
      this.blockUiIconClass.set(blockUiParams.iconClass ?? "pi pi-spin pi-spinner text-4xl");
    });

    this.#blockUiService.onHide$.subscribe(() => {
      this.showBlockUi.set(false);
    });
  }

  ngOnInit() {
    this.#primengConfig.ripple.set(true);
  }
}
