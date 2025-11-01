import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectModule } from "primeng/select";
import { DropdownListItemComponent } from "../dropdown-list-item/dropdown-list-item.component";
import { StaticContentType, StaticContentTypes } from "@core/backend-api/static-content-types";
import { ListItem } from "@core/models/list-item";

@Component({
  selector: "content-type-dropdown",
  templateUrl: "./content-type-dropdown.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentTypeDropdownComponent),
      multi: true,
    },
  ],
})
export class ContentTypeDropdownComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentTypes | null = StaticContentType.Page;
  disabled = false;

  get options() {
    const baseOptions = [
      new ListItem<StaticContentTypes>(StaticContentType.Page, "Page", "Full page content with a URL."),
      new ListItem<StaticContentTypes>(StaticContentType.Zone, "Zone", "Content for a zone within another page."),
    ];

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
