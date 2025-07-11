import { CommonModule } from "@angular/common";
import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { AdminUserDto, ClaimDto, ConfirmPopupComponent, PagedResponseDto } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { UsersService } from "@core/services/users.service";
import { RouteAliasService, RoutePipe } from "@routing";
import { ConfirmationService } from "primeng/api";
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
  readonly #adminService = inject(UsersService);

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
