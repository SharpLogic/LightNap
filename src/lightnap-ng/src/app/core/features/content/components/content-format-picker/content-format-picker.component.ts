import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentFormat, StaticContentFormatListItems, StaticContentFormats } from "@core/backend-api";
import { DropdownListItemComponent } from "@core/components/dropdown-list-item/dropdown-list-item.component";
import { ListItem } from "@core/models/list-item";
import { SelectModule } from "primeng/select";

@Component({
  selector: "ln-content-format-picker",
  templateUrl: "./content-format-picker.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentFormatPickerComponent),
      multi: true,
    },
  ],
})
export class ContentFormatPickerComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentFormats | null = StaticContentFormat.Html;
  disabled = false;

  get options() {
    const baseOptions = StaticContentFormatListItems;

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentFormats | null>(null, "Any", "Don't filter by format."), ...baseOptions];
    }

    return baseOptions;
  }

  onChange: (value: StaticContentFormats | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentFormats | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentFormats | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
