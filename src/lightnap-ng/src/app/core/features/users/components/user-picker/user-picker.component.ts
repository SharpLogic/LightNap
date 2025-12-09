
import { Component, EventEmitter, forwardRef, inject, Input, Output, signal } from "@angular/core";
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from "@angular/forms";
import { AdminSearchUsersRequestDto, AdminUserDto, ApplicationUserSortBy } from "@core/backend-api";
import { AdminUsersService } from "@core/features/users/services/admin-users.service";
import { AutoCompleteModule, AutoCompleteSelectEvent } from "primeng/autocomplete";
import { finalize } from "rxjs";

@Component({
  selector: 'ln-user-picker',
  templateUrl: './user-picker.component.html',
  imports: [FormsModule, AutoCompleteModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UserPickerComponent),
      multi: true,
    },
  ],
})
export class UserPickerComponent implements ControlValueAccessor {
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

    const request: AdminSearchUsersRequestDto = {
      userName: event.query,
      sortBy: ApplicationUserSortBy.UserName,
      reverseSort: false,
      pageNumber: 1,
      pageSize: 10,
    };

    this.#usersService
      .searchUsers(request)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => {
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
  setDisabledState?(_isDisabled: boolean): void {
    // Optionally implement if you want to support disabling the picker
  }
}
