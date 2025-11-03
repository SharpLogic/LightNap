import { CommonModule } from "@angular/common";
import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { PagedResponseDto } from "@core/backend-api/dtos/paged-response-dto";
import { PrivilegedUserDto } from "@core/backend-api/dtos/users/response/privileged-user-dto";
import { EmptyPagedResponse } from "@core/backend-api/empty-paged-response";
import { PeoplePickerComponent } from "@core/features/users/components/people-picker/people-picker.component";
import { PrivilegedUsersService } from "@core/features/users/services/privileged-users.service";
import { setApiErrors } from "@core/helpers/rxjs-helpers";
import { TypeHelpers } from "@core/helpers/type-helpers";
import { ToastService } from "@core/services/toast.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { startWith, Subject, switchMap } from "rxjs";
import { ApiResponseComponent } from "../api-response/api-response.component";
import { ConfirmPopupComponent } from "../confirm-popup/confirm-popup.component";
import { ErrorListComponent } from "../error-list/error-list.component";
import { UserLinkComponent } from "../user-link/user-link.component";

@Component({
  selector: "ln-claim-users-manager",
  templateUrl: "./claim-users-manager.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    TableModule,
    PanelModule,
    ApiResponseComponent,
    ErrorListComponent,
    PeoplePickerComponent,
    UserLinkComponent,
    ConfirmPopupComponent,
  ],
})
export class ClaimUsersManagerComponent {
  #usersService = inject(PrivilegedUsersService);
  #toast = inject(ToastService);
  #confirmationService = inject(ConfirmationService);
  #fb = inject(FormBuilder);

  type = input.required<string>();
  value = input.required<string>();

  addUserForm = this.#fb.group({
    userId: this.#fb.nonNullable.control("", [Validators.required]),
  });

  errors = signal(new Array<string>());

  pageSize = input(10);

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly claims$ = this.#lazyLoadEventSubject.pipe(
    switchMap(event =>
      this.#usersService.getUsersWithClaim({
        type: this.type(),
        value: this.value(),
        pageSize: this.pageSize(),
        pageNumber: (event.first ?? 0) / this.pageSize() + 1,
      })
    ),
    // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
    // so we can avoid a redundant call to the API.
    startWith(new EmptyPagedResponse<PrivilegedUserDto>())
  );

  readonly asUserResults = TypeHelpers.cast<PagedResponseDto<PrivilegedUserDto>>;
  readonly asUser = TypeHelpers.cast<PrivilegedUserDto>;

  loadClaimsLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }

  addUser() {
    const userId = this.addUserForm.value.userId;
    if (!userId) return;

    this.errors.set([]);
    this.#usersService
      .addUserClaim(userId, {
        type: this.type(),
        value: this.value(),
      })
      .subscribe({
        next: () => {
          this.#toast.success("User claim added successfully.");
          this.addUserForm.reset();
          this.#lazyLoadEventSubject.next({ first: 0, rows: this.pageSize() });
        },
        error: setApiErrors(this.errors),
      });
  }

  removeUser(event: any, userId: string) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm User Claim Removal",
      message: `Are you sure that you want to remove this user claim?`,
      target: event.target,
      key: userId,
      accept: () => {
        this.#usersService
          .removeUserClaim(userId, {
            type: this.type(),
            value: this.value(),
          })
          .subscribe({
            next: () => {
              this.#toast.success("User claim removed successfully.");
              this.#lazyLoadEventSubject.next({ first: 0, rows: this.pageSize() });
            },
            error: setApiErrors(this.errors),
          });
      },
    });
  }
}
