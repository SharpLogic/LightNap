import { CommonModule } from "@angular/common";
import { Component, forwardRef, inject } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectListItemComponent } from "@core/components/select-list-item/select-list-item.component";
import { PrivilegedUsersService } from "@core/features/users/services/privileged-users.service";
import { ListItem } from "@core/models/list-item";
import { MultiSelectModule } from "primeng/multiselect";
import { map } from "rxjs";

@Component({
  selector: "ln-roles-picker",
  templateUrl: "./roles-picker.component.html",
  imports: [CommonModule, MultiSelectModule, FormsModule, SelectListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RolesPickerComponent),
      multi: true,
    },
  ],
})
export class RolesPickerComponent implements ControlValueAccessor {
  #userManager = inject(PrivilegedUsersService);
  value: string[] = [];
  disabled = false;

  options = this.#userManager.getRoles().pipe(map(roles => roles.map(role => new ListItem<string>(role.name, role.displayName, role.description))));

  onChange: (value: string) => void = () => {};
  markTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value = value ? value.split(",").map(v => v.trim()) : [];
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.markTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  change(selectedRoles: string[]): void {
    this.value = selectedRoles;
    this.onChange(selectedRoles.join(","));
  }
}
