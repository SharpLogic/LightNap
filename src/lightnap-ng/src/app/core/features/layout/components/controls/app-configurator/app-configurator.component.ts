import { CommonModule } from "@angular/common";
import { Component, inject, PLATFORM_ID, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { SelectButtonModule } from "primeng/selectbutton";

@Component({
  selector: "ln-app-configurator",
  templateUrl: "./app-configurator.component.html",
  imports: [CommonModule, FormsModule, SelectButtonModule],
  host: {
    class:
      "hidden absolute top-[3.25rem] right-0 w-72 p-4 bg-surface-0 dark:bg-surface-900 border border-surface rounded-border origin-top shadow-[0px_3px_5px_rgba(0,0,0,0.02),0px_0px_2px_rgba(0,0,0,0.05),0px_1px_4px_rgba(0,0,0,0.08)]",
  },
})
export class AppConfiguratorComponent {
  readonly router = inject(Router);
  readonly layoutService = inject(LayoutService);
  readonly platformId = inject(PLATFORM_ID);
  readonly presets = Object.keys(this.layoutService.presets);
  readonly showMenuModeButton = signal(!this.router.url.includes("identity"));

  onPrimaryColorChange(primary: string) {
    this.layoutService.layoutConfig.update(state => ({ ...state, primary }));
  }

  onSurfaceColorChange(surface: string) {
    this.layoutService.layoutConfig.update(state => ({ ...state, surface }));
  }

  onPresetChange(preset: string) {
    this.layoutService.layoutConfig.update(state => ({ ...state, preset }));
  }

  onMenuModeChange(menuMode: string) {
    this.layoutService.layoutConfig.update(state => ({ ...state, menuMode }));
  }
}
