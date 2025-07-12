import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { AdminUsersService, ClaimDto, PagedResponseDto, RoutePipe } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";
import { Observable } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./claims.component.html",
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
  ],
})
export class ClaimsComponent {
  readonly #adminService = inject(AdminUsersService);

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    type: this.#fb.nonNullable.control(""),
    value: this.#fb.nonNullable.control(""),
  });

  errors = signal(new Array<string>());

  claims$ = signal(new Observable<PagedResponseDto<ClaimDto>>());

  constructor() {
    this.search();
  }

  ngOnChanges() {
    this.search();
  }

  search() {
    this.claims$.set(this.#adminService.searchClaims(this.form.value));
  }
}
