
import { Component, OnChanges, inject, input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { UserSettingKey } from "@core/backend-api";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { SelectListItemComponent } from "@core/components/select-list-item/select-list-item.component";
import { ListItem } from "@core/models";
import { ProfileService } from "@core/services/profile.service";
import { ToastService } from "@core/services/toast.service";
import { Select } from "primeng/select";
import { Observable } from "rxjs";

@Component({
  selector: "ln-user-setting-select",
  standalone: true,
  templateUrl: "./user-setting-select.component.html",
  imports: [FormsModule, Select, SelectListItemComponent, ApiResponseComponent],
})
export class UserSettingSelectComponent<T> implements OnChanges {
  readonly #profileService = inject(ProfileService);
  readonly #toast = inject(ToastService);

  readonly key = input.required<UserSettingKey>();
  readonly label = input.required<string>();
  readonly options = input.required<Array<ListItem<T>>>();
  readonly setting = signal(new Observable<T>());
  readonly defaultValue = input<T>();

  ngOnChanges() {
    this.setting.set(this.#profileService.getSetting<T>(this.key(), this.defaultValue()));
  }

  change(value: T) {
    this.#profileService.setSetting(this.key(), value).subscribe({
      next: () => this.#toast.success("Setting updated."),
      error: () => this.#toast.error("Failed to update setting."),
    });
  }
}
