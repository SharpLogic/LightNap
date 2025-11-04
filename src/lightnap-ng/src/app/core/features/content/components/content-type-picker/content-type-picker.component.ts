import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectModule } from "primeng/select";
import { DropdownListItemComponent } from "@core/components/dropdown-list-item/dropdown-list-item.component";
import { StaticContentType, StaticContentTypeListItems, StaticContentTypes } from "@core/backend-api/static-content-types";
import { ListItem } from "@core/models/list-item";

@Component({
  selector: 'ln-content-type-picker',
  templateUrl: './content-type-picker.component.html',
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentTypePickerComponent),
      multi: true,
    },
  ],
})
export class ContentTypePickerComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentTypes | null = StaticContentType.Page;
  disabled = false;

  get options() {
    const baseOptions = StaticContentTypeListItems;

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentTypes | null>(null, "Any", "Don't filter by type."), ...baseOptions];
    }

    return baseOptions;
  }

  onChange: (value: StaticContentTypes | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentTypes | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentTypes | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
