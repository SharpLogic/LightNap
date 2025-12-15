import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { AdminExternalLoginDto, EmptyPagedResponse, PagedResponseDto, RoutePipe, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmDialogComponent } from "@core/components/confirm-dialog/confirm-dialog.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ExternalLoginService } from "@core/services/external-login.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { debounceTime, startWith, Subject, switchMap } from "rxjs";

@Component({
  templateUrl: "./external-logins.component.html",
  imports: [
    ReactiveFormsModule,
    PanelModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    ErrorListComponent,
    ApiResponseComponent,
    CheckboxModule,
    ConfirmDialogComponent,
  ],
})
export class ExternalLoginsComponent {
  readonly pageSize = 10;

  readonly #externalLoginService = inject(ExternalLoginService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    loginProvider: this.#fb.nonNullable.control(""),
    userId: this.#fb.nonNullable.control(""),
  });

  readonly errors = signal(new Array<string>());

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly externalLogins$ = this.#lazyLoadEventSubject.pipe(
    switchMap(event =>
      this.#externalLoginService.searchExternalLogins({
        loginProvider: this.form.value.loginProvider,
        userId: this.form.value.userId,
        pageSize: this.pageSize,
        pageNumber: (event.first ?? 0) / this.pageSize + 1,
      })
    ),
    // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
    // so we can avoid a redundant call to the API.
    startWith(new EmptyPagedResponse<AdminExternalLoginDto>())
  );

  readonly asExternalLoginResults = TypeHelpers.cast<PagedResponseDto<AdminExternalLoginDto>>;
  readonly asExternalLogin = TypeHelpers.cast<AdminExternalLoginDto>;

  constructor() {
    this.form.valueChanges.pipe(debounceTime(300)).subscribe({ next: () => this.#lazyLoadEventSubject.next({ first: 0 }) });
  }

  loadClaimsLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }

  removeExternalLogin(event: any, userId: string, loginProvider: string, providerKey: string) {
    this.#confirmationService.confirm({
      header: "Confirm Removal",
      message: `Are you sure that you want to remove this external login?`,
      target: event.target,
      key: `${userId}:${loginProvider}:${providerKey}`,
      accept: () => {
        this.#externalLoginService.removeExternalLogin(userId, loginProvider, providerKey).subscribe({
          next: () => this.#lazyLoadEventSubject.next({ first: 0 }),
          error: setApiErrors(this.errors),
        });
      },
    });
  }
}
