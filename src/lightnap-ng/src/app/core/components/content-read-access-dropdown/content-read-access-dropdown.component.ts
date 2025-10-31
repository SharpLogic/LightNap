import { Component, forwardRef } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { StaticContentReadAccess, StaticContentReadAccesses } from "@core";
import { SelectModule } from "primeng/select";

@Component({
  selector: "content-read-access-dropdown",
  templateUrl: "./content-read-access-dropdown.component.html",
  imports: [SelectModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentReadAccessDropdownComponent),
      multi: true,
    },
  ],
})
export class ContentReadAccessDropdownComponent implements ControlValueAccessor {
  value: StaticContentReadAccesses = StaticContentReadAccess.Explicit;
  disabled = false;

  options = [
    { label: "Public", value: StaticContentReadAccess.Public },
    { label: "Authenticated", value: StaticContentReadAccess.Authenticated },
    { label: "Explicit", value: StaticContentReadAccess.Explicit },
  ];

  onChange: (value: StaticContentReadAccesses) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentReadAccesses): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentReadAccesses) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
