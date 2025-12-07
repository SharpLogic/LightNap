import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentFormat, StaticContentFormatListItems, StaticContentFormats } from "@core/backend-api";
import { SelectListItemComponent } from "@core/components/select-list-item/select-list-item.component";
import { ListItem } from "@core/models/list-item";
import { SelectModule } from "primeng/select";

@Component({
  selector: "ln-content-format-picker",
  templateUrl: "./content-format-picker.component.html",
  imports: [SelectModule, FormsModule, SelectListItemComponent],
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

  value: StaticContentFormat | null = StaticContentFormats.Html;
  disabled = false;

  get options() {
    const baseOptions = StaticContentFormatListItems;

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentFormat | null>(null, "Any", "Don't filter by format."), ...baseOptions];
    }

    return baseOptions;
  }

  change: (value: StaticContentFormat | null) => void = () => {};
  markTouched: () => void = () => {};

  writeValue(value: StaticContentFormat | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentFormat | null) => void): void {
    this.change = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.markTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
