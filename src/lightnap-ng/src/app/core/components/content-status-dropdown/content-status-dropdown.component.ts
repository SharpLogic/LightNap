import { Component, forwardRef } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentStatus, StaticContentStatuses } from "@core";
import { SelectModule } from "primeng/select";

@Component({
  selector: "content-status-dropdown",
  templateUrl: "./content-status-dropdown.component.html",
  imports: [SelectModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentStatusDropdownComponent),
      multi: true,
    },
  ],
})
export class ContentStatusDropdownComponent implements ControlValueAccessor {
  value: StaticContentStatuses = StaticContentStatus.Draft;
  disabled = false;

  options = [
    { label: "Draft", value: StaticContentStatus.Draft },
    { label: "Published", value: StaticContentStatus.Published },
    { label: "Archived", value: StaticContentStatus.Archived },
  ];

  onChange: (value: StaticContentStatuses) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentStatuses): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentStatuses) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
