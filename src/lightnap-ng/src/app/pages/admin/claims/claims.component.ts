import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ClaimDto, EmptyPagedResponse, PagedResponseDto, RoutePipe, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { AdminUsersService } from "@core/features/users/services/admin-users.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { debounceTime, startWith, Subject, switchMap } from "rxjs";

@Component({
  templateUrl: "./claims.component.html",
  imports: [
    ReactiveFormsModule,
    PanelModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    RouterLink,
    RoutePipe,
    ErrorListComponent,
    ApiResponseComponent,
    CheckboxModule,
  ],
})
export class ClaimsComponent {
  readonly pageSize = 10;

  readonly #adminService = inject(AdminUsersService);

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    type: this.#fb.nonNullable.control(""),
    typeExact: this.#fb.control(false),
    value: this.#fb.nonNullable.control(""),
    valueExact: this.#fb.control(false),
  });

  readonly errors = signal(new Array<string>());

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly claims$ = this.#lazyLoadEventSubject.pipe(
    switchMap(event =>
      this.#adminService.searchClaims({
        type: this.form.value.typeExact ? this.form.value.type : undefined,
        typeContains: !this.form.value.typeExact ? this.form.value.type : undefined,
        value: this.form.value.valueExact ? this.form.value.value : undefined,
        valueContains: !this.form.value.valueExact ? this.form.value.value : undefined,
        pageSize: this.pageSize,
        pageNumber: (event.first ?? 0) / this.pageSize + 1,
      })
    ),
    // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
    // so we can avoid a redundant call to the API.
    startWith(new EmptyPagedResponse<ClaimDto>())
  );

  readonly asClaimResults = TypeHelpers.cast<PagedResponseDto<ClaimDto>>;
  readonly asClaim = TypeHelpers.cast<ClaimDto>;

  constructor() {
    this.form.valueChanges.pipe(debounceTime(300)).subscribe({ next: () => this.#lazyLoadEventSubject.next({ first: 0 }) });
  }

  loadClaimsLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }
}
