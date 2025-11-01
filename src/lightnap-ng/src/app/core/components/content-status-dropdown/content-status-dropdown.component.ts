import { Component, forwardRef, input } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { SelectModule } from "primeng/select";
import { DropdownListItemComponent } from "../dropdown-list-item/dropdown-list-item.component";
import { StaticContentStatus, StaticContentStatuses } from "@core/backend-api/static-content-statuses";
import { ListItem } from "@core/models/list-item";

@Component({
  selector: "content-status-dropdown",
  templateUrl: "./content-status-dropdown.component.html",
  imports: [SelectModule, FormsModule, DropdownListItemComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContentStatusDropdownComponent),
      multi: true,
    },
  ],
})
export class ContentStatusDropdownComponent implements ControlValueAccessor {
  showAnyOption = input<boolean>(false);

  value: StaticContentStatuses | null = StaticContentStatus.Draft;
  disabled = false;

  get options() {
    const baseOptions = [
      new ListItem<StaticContentStatuses>(StaticContentStatus.Draft, "Draft", "Content is not visible to end users."),
      new ListItem<StaticContentStatuses>(StaticContentStatus.Published, "Published", "Published content visible to allowed users."),
      new ListItem<StaticContentStatuses>(StaticContentStatus.Archived, "Archived", "Archived and not visible to end users."),
    ];

    if (this.showAnyOption()) {
      return [new ListItem<StaticContentStatuses | null>(null, "Any", "Don't filter by status."), ...baseOptions];
    }

    return baseOptions;
  }

  onChange: (value: StaticContentStatuses | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: StaticContentStatuses | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: StaticContentStatuses | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
