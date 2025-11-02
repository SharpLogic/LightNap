import { CommonModule } from "@angular/common";
import { Component, forwardRef, inject } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { DropdownListItemComponent } from "@core/components/dropdown-list-item/dropdown-list-item.component";
import { ListItem } from "@core/models/list-item";
import { PrivilegedUsersService } from "@core/users";
import { MultiSelectModule } from "primeng/multiselect";
import { map } from "rxjs";

@Component({
  selector: "ln-roles-picker",
  templateUrl: "./roles-picker.component.html",
  imports: [CommonModule, MultiSelectModule, FormsModule, DropdownListItemComponent],
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

  options = this.#userManager.getRoles().pipe(map(roles => roles.map(role => new ListItem<string>(role.name, role.name, role.description))));

  onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value = value ? value.split(",").map(v => v.trim()) : [];
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onValueChange(selectedRoles: string[]): void {
    this.value = selectedRoles;
    this.onChange(selectedRoles.join(","));
  }
}
