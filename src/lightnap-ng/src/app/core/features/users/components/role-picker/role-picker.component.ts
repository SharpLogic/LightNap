import { CommonModule } from "@angular/common";
import { Component, forwardRef, inject } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectListItemComponent } from "@core/components/select-list-item/select-list-item.component";
import { ListItem } from "@core/models/list-item";
import { PrivilegedUsersService } from "@core/features/users/services/privileged-users.service";
import { SelectModule } from "primeng/select";
import { map } from "rxjs";

@Component({
  selector: "ln-role-picker",
  templateUrl: "./role-picker.component.html",
  imports: [CommonModule, SelectModule, FormsModule, SelectListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RolePickerComponent),
      multi: true,
    },
  ],
})
export class RolePickerComponent implements ControlValueAccessor {
  #userManager = inject(PrivilegedUsersService);
  value: string | null = null;
  disabled = false;

  options = this.#userManager.getRoles().pipe(map(roles => roles.map(role => new ListItem<string>(role.name, role.displayName, role.description))));

  onChange: (value: string | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
