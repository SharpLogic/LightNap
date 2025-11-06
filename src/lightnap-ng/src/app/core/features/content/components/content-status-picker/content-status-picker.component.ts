import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentStatus, StaticContentStatuses, StaticContentStatusListItems } from "@core/backend-api";
import { DropdownListItemComponent } from "@core/components/dropdown-list-item/dropdown-list-item.component";
import { ListItem } from "@core/models/list-item";
import { SelectModule } from "primeng/select";

@Component({
  selector: "ln-content-status-picker",
  templateUrl: "./content-status-picker.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentStatusPickerComponent),
      multi: true,
    },
  ],
})
export class ContentStatusPickerComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentStatus | null = StaticContentStatuses.Draft;
  disabled = false;

  get options() {
    const baseOptions = StaticContentStatusListItems;

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentStatus | null>(null, "Any", "Don't filter by status."), ...baseOptions];
    }

    return baseOptions;
  }

  onChange: (value: StaticContentStatus | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentStatus | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentStatus | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
