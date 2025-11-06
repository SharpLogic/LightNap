import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentReadAccess, StaticContentReadAccesses, StaticContentReadAccessListItems } from "@core/backend-api";
import { DropdownListItemComponent } from "@core/components/dropdown-list-item/dropdown-list-item.component";
import { ListItem } from "@core/models/list-item";
import { SelectModule } from "primeng/select";

@Component({
  selector: "ln-content-read-access-picker",
  templateUrl: "./content-read-access-picker.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentReadAccessPickerComponent),
      multi: true,
    },
  ],
})
export class ContentReadAccessPickerComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentReadAccess | null = StaticContentReadAccesses.Explicit;
  disabled = false;

  get options() {
    const baseOptions = StaticContentReadAccessListItems;

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentReadAccess | null>(null, "Any", "Don't filter by read access."), ...baseOptions];
    }

    return baseOptions;
  }

  onChange: (value: StaticContentReadAccess | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentReadAccess | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentReadAccess | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
