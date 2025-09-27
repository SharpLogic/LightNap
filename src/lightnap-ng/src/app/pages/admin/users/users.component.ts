import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import {
  AdminUserDto,
  ApiResponseComponent,
  ConfirmPopupComponent,
  EmptyPagedResponse,
  ErrorListComponent,
  ListItem,
  PagedResponseDto,
  SearchUsersSortBy,
  ToastService,
  TypeHelpers,
} from "@core";
import { AdminUsersService } from "@core";
import { RoutePipe } from "@core";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { TagModule } from "primeng/tag";
import { debounceTime, startWith, Subject, switchMap } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./users.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PanelModule,
    ApiResponseComponent,
    TableModule,
    ButtonModule,
    RouterModule,
    RoutePipe,
    ErrorListComponent,
    InputTextModule,
    ConfirmPopupComponent,
    TagModule,
  ],
})
export class UsersComponent {
  readonly pageSize = 25;

  readonly #adminService = inject(AdminUsersService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #toast = inject(ToastService);
  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    email: this.#fb.control(""),
    userName: this.#fb.control(""),
  });

  readonly errors = signal(new Array<string>());

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly users$ = this.#lazyLoadEventSubject.pipe(
    switchMap(event =>
      this.#adminService.searchUsers({
        sortBy: (event.sortField as SearchUsersSortBy) ?? "userName",
        reverseSort: event.sortOrder === -1,
        pageSize: this.pageSize,
        pageNumber: (event.first ?? 0) / this.pageSize + 1,
        email: (this.form.value.email?.length ?? 0 > 0) ? this.form.value.email! : undefined,
        userName: (this.form.value.userName?.length ?? 0 > 0) ? this.form.value.userName! : undefined,
      })
    ),
    // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
    // so we can avoid a redundant call to the API.
    startWith(new EmptyPagedResponse<AdminUserDto>() as PagedResponseDto<AdminUserDto>)
  );

  readonly sortBys = [
    new ListItem<SearchUsersSortBy>("userName", "User Name", "Sort by user name."),
    new ListItem<SearchUsersSortBy>("email", "Email", "Sort by email."),
    new ListItem<SearchUsersSortBy>("createdDate", "Created", "Sort by created date."),
    new ListItem<SearchUsersSortBy>("lastModifiedDate", "Last Modified", "Sort by last modified date."),
  ];

  readonly asUserResults = TypeHelpers.cast<PagedResponseDto<AdminUserDto>>;
  readonly asUser = TypeHelpers.cast<AdminUserDto>;

  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(), debounceTime(1000)).subscribe({ next: () => this.#lazyLoadEventSubject.next({ first: 0 }) });
  }

  loadUsersLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }

  deleteUser(event: any, userId: string) {
    this.#confirmationService.confirm({
      header: "Confirm Delete",
      message: `Are you sure that you want to delete this user?`,
      key: userId,
      target: event.target,
      accept: () => {
        this.#adminService.deleteUser(userId).subscribe({
          next: () => {
            this.#toast.success("User deleted successfully.");
            this.#lazyLoadEventSubject.next({ first: 0 });
          },
          error: response => this.errors.set(response.errorMessages),
        });
      },
    });
  }
}
