import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import {
    AdminUserDto,
    AdminUsersService,
    ClaimDto,
    EmptyPagedResponse,
    PagedResponseDto,
    PeoplePickerComponent,
    RoutePipe,
    setApiErrors,
    TypeHelpers,
} from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmPopupComponent } from "@core/components/confirm-popup/confirm-popup.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { startWith, Subject, switchMap } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./claim.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PanelModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    RouterLink,
    RoutePipe,
    ErrorListComponent,
    ApiResponseComponent,
    ConfirmPopupComponent,
    PeoplePickerComponent,
  ],
})
export class ClaimComponent {
  readonly pageSize = 10;

  readonly #adminService = inject(AdminUsersService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly type = input.required<string>();
  readonly value = input.required<string>();

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    userId: this.#fb.control<string | null>(null, [Validators.required]),
  });

  readonly errors = signal(new Array<string>());

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly usersForClaim$ = computed(() =>
    this.#lazyLoadEventSubject.pipe(
      switchMap(event =>
        this.#adminService.getUsersWithClaim({
          type: this.type(),
          value: this.value(),
          pageSize: this.pageSize,
          pageNumber: (event.first ?? 0) / this.pageSize + 1,
        })
      ),
      // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
      // so we can avoid a redundant call to the API.
      startWith(new EmptyPagedResponse<ClaimDto>())
    )
  );

  readonly asUsersForClaimResults = TypeHelpers.cast<PagedResponseDto<AdminUserDto>>;
  readonly asUser = TypeHelpers.cast<AdminUserDto>;

  addUserClaim() {
    this.errors.set([]);

    this.#adminService.addUserClaim(this.form.value.userId!, { type: this.type(), value: this.value() }).subscribe({
      next: () => {
        this.form.reset();
        this.#lazyLoadEventSubject.next({ first: 0, rows: this.pageSize });
      },
      error: setApiErrors(this.errors),
    });
  }

  removeUserClaim(event: any, userId: string) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm User Claim Removal",
      message: `Are you sure that you want to remove this user claim?`,
      target: event.target,
      key: userId,
      accept: () => {
        this.#adminService.removeUserClaim(userId, { type: this.type(), value: this.value() }).subscribe({
          next: () => this.#lazyLoadEventSubject.next({ first: 0, rows: this.pageSize }),
          error: setApiErrors(this.errors),
        });
      },
    });
  }

  loadUsersLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }
}
