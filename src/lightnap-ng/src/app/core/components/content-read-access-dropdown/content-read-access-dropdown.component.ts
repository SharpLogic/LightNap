import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectModule } from "primeng/select";
import { DropdownListItemComponent } from "../dropdown-list-item/dropdown-list-item.component";
import { StaticContentReadAccess, StaticContentReadAccesses } from "@core/backend-api/static-content-read-accesses";
import { ListItem } from "@core/models/list-item";

@Component({
  selector: "content-read-access-dropdown",
  templateUrl: "./content-read-access-dropdown.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentReadAccessDropdownComponent),
      multi: true,
    },
  ],
})
export class ContentReadAccessDropdownComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentReadAccesses | null = StaticContentReadAccess.Explicit;
  disabled = false;

  get options() {
    const baseOptions = [
      new ListItem<StaticContentReadAccesses>(StaticContentReadAccess.Public, "Public", "Visible to anyone."),
      new ListItem<StaticContentReadAccesses>(StaticContentReadAccess.Authenticated, "Authenticated", "Visible to authenticated users."),
      new ListItem<StaticContentReadAccesses>(StaticContentReadAccess.Explicit, "Explicit", "Visible to explicitly granted roles and users."),
    ];

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentReadAccesses>(null!, "Any", "Don't filter by read access."), ...baseOptions];
    }

    return baseOptions;
  }

  onChange: (value: StaticContentReadAccesses | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentReadAccesses | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentReadAccesses | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
