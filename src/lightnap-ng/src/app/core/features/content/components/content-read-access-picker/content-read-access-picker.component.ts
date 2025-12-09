import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentReadAccess, StaticContentReadAccessListItems } from "@core/backend-api";
import { SelectListItemComponent } from "@core/components/select-list-item/select-list-item.component";
import { ListItem } from "@core/models/list-item";
import { SelectModule } from "primeng/select";

@Component({
  selector: "ln-content-read-access-picker",
  templateUrl: "./content-read-access-picker.component.html",
  imports: [SelectModule, FormsModule, SelectListItemComponent],
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

  value: StaticContentReadAccess | null = StaticContentReadAccess.Explicit;
  disabled = false;

  get options() {
    const baseOptions = StaticContentReadAccessListItems;

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentReadAccess | null>(null, "Any", "Don't filter by read access."), ...baseOptions];
    }

    return baseOptions;
  }

  change: (value: StaticContentReadAccess | null) => void = () => {};
  markTouched: () => void = () => {};

  writeValue(value: StaticContentReadAccess | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentReadAccess | null) => void): void {
    this.change = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.markTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
