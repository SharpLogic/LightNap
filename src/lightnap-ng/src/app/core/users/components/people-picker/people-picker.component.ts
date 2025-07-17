import { CommonModule } from "@angular/common";
import { Component, forwardRef, inject, Input, Output, EventEmitter, signal } from "@angular/core";
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from "@angular/forms";
import { AdminUsersService } from "@core";
import { AdminUserDto, PublicSearchUsersRequestDto } from "@core/backend-api";
import { AutoCompleteModule, AutoCompleteSelectEvent } from "primeng/autocomplete";
import { finalize } from "rxjs";

@Component({
  selector: "people-picker",
  templateUrl: "./people-picker.component.html",
  imports: [CommonModule, FormsModule, AutoCompleteModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PeoplePickerComponent),
      multi: true,
    },
  ],
})
export class PeoplePickerComponent implements ControlValueAccessor {
  #usersService = inject(AdminUsersService);
  @Input() selectedUserId: string | null = null;
  @Output() selectedUserIdChange = new EventEmitter<string | null>();

  users = signal<AdminUserDto[]>([]);
  loading = false;
  selectedUser: AdminUserDto | null = null;

  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  searchUsers(event: { query: string }) {
    this.loading = true;

    const request: PublicSearchUsersRequestDto = {
      userName: event.query,
      sortBy: "userName",
      reverseSort: false,
      pageNumber: 1,
      pageSize: 10,
    };

    this.#usersService
      .searchUsers(request)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response: any) => {
          this.users.set(response.data || []);
        },
        error: () => {
          this.users.set([]);
        },
      });
  }

  selectUser(event: AutoCompleteSelectEvent) {
    const userId = event.value ? event.value.id : null;
    this.selectedUserId = userId;
    this.selectedUserIdChange.emit(userId);
    this.onChange(userId);
    this.onTouched();
  }

  // ControlValueAccessor methods
  writeValue(value: string | null): void {
    this.selectedUserId = value;
    this.selectedUser = this.users().find(u => u.id === value) || null;
  }
  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    // Optionally implement if you want to support disabling the picker
  }
}
