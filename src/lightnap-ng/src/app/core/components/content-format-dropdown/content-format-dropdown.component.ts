import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectModule } from "primeng/select";
import { DropdownListItemComponent } from "../dropdown-list-item/dropdown-list-item.component";
import { StaticContentFormat, StaticContentFormats } from "@core/backend-api";
import { ListItem } from "@core/models/list-item";

@Component({
  selector: "content-format-dropdown",
  templateUrl: "./content-format-dropdown.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentFormatDropdownComponent),
      multi: true,
    },
  ],
})
export class ContentFormatDropdownComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentFormats | null = StaticContentFormat.Html;
  disabled = false;

  get options() {
    const baseOptions = [
      new ListItem<StaticContentFormats>(StaticContentFormat.Html, "HTML", "Render as HTML."),
      new ListItem<StaticContentFormats>(StaticContentFormat.Markdown, "Markdown", "Render as Markdown."),
      new ListItem<StaticContentFormats>(StaticContentFormat.PlainText, "Plain Text", "Render as plain text."),
    ];

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
