import { CommonModule } from "@angular/common";
import { Component, OnChanges, inject, input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { UserSettingKey } from "@core/backend-api";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ProfileService } from "@core/services/profile.service";
import { ToastService } from "@core/services/toast.service";
import { ToggleSwitchModule } from "primeng/toggleswitch";
import { Observable } from "rxjs";

@Component({
  selector: "ln-user-setting-toggle",
  standalone: true,
  templateUrl: "./user-setting-toggle.component.html",
  imports: [CommonModule, FormsModule, ToggleSwitchModule, ApiResponseComponent],
})
export class UserSettingToggleComponent implements OnChanges {
  readonly #profileService = inject(ProfileService);
  readonly #toast = inject(ToastService);

  readonly key = input.required<UserSettingKey>();
  readonly label = input.required<string>();
  readonly setting = signal(new Observable<boolean>());

  ngOnChanges() {
    this.setting.set(this.#profileService.getSetting<boolean>(this.key()));
  }

  toggle($event: { originalEvent: Event; checked: boolean }) {
    this.#profileService.setSetting(this.key(), $event.checked).subscribe({
      next: () => this.#toast.success("Setting updated."),
      error: () => this.#toast.error("Failed to update setting."),
    });
  }
}
