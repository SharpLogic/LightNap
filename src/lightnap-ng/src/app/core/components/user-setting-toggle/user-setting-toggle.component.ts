import { CommonModule } from "@angular/common";
import { Component, OnChanges, inject, input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ApiResponseComponent, ProfileService, ToastService, UserSettingKeys } from "@core";
import { ToggleSwitchModule } from "primeng/toggleswitch";
import { Observable } from "rxjs";

@Component({
  selector: "user-setting-toggle",
  standalone: true,
  templateUrl: "./user-setting-toggle.component.html",
  imports: [CommonModule, FormsModule, ToggleSwitchModule, ApiResponseComponent],
})
export class UserSettingToggleComponent implements OnChanges {
  readonly #profileService = inject(ProfileService);
  readonly #toast = inject(ToastService);

  readonly key = input.required<UserSettingKeys>();
  readonly label = input.required<string>();
  readonly setting = signal(new Observable<boolean>());

  ngOnChanges() {
    this.setting.set(this.#profileService.getSetting<boolean>(this.key()));
  }

  onToggle($event: { originalEvent: Event; checked: boolean }) {
    this.#profileService.setSetting(this.key(), $event.checked).subscribe({
        next: () => this.#toast.success("Setting updated."),
        error: () => this.#toast.error("Failed to update setting."),
    });
  }
}
