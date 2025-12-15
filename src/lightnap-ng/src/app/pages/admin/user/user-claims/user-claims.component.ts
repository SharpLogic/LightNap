import { Component, computed, inject, input } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ClaimDto, EmptyPagedResponse, PagedResponseDto, RoutePipe, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmPopupComponent } from "@core/components/confirm-popup/confirm-popup.component";
import { AdminUsersService } from "@core/features/users/services/admin-users.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { startWith, Subject, switchMap } from "rxjs";

@Component({
  selector: "user-claims",
  imports: [ReactiveFormsModule, RouterLink, RoutePipe, TableModule, InputTextModule, ButtonModule, ConfirmPopupComponent, ApiResponseComponent],
  templateUrl: "./user-claims.component.html",
})
export class UserClaimsComponent {
  readonly pageSize = 10;

  readonly #adminService = inject(AdminUsersService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #fb = inject(FormBuilder);

  readonly userId = input.required<string>();

  readonly addUserClaimForm = this.#fb.group({
    type: this.#fb.control<string>("", [Validators.required]),
    value: this.#fb.control<string>("", [Validators.required]),
  });

  readonly asUserClaims = TypeHelpers.cast<PagedResponseDto<ClaimDto>>;
  readonly asClaim = TypeHelpers.cast<ClaimDto>;

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly userClaims$ = computed(() =>
    this.#lazyLoadEventSubject.pipe(
      switchMap(event =>
        this.#adminService.getUserClaims({
          userId: this.userId(),
          pageSize: this.pageSize,
          pageNumber: (event.first ?? 0) / this.pageSize + 1,
        })
      ),
      // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
      // so we can avoid a redundant call to the API.
      startWith(new EmptyPagedResponse<ClaimDto>())
    )
  );

  loadUserClaimsLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }

  addClaimClicked() {
    const { type, value } = this.addUserClaimForm.value;
    if (!type || !value) return;

    this.#adminService.addUserClaim(this.userId(), { type, value }).subscribe({
      next: () => {
        this.addUserClaimForm.reset();
        this.#lazyLoadEventSubject.next({ first: 0, rows: this.pageSize });
      },
    });
  }

  removeClaimClicked(event: any, claim: ClaimDto) {
    this.#confirmationService.confirm({
      header: "Confirm Claim Removal",
      message: `Are you sure you want to remove this claim?`,
      target: event.target,
      key: claim.type + "=" + claim.value,
      accept: () => {
        this.#adminService.removeUserClaim(this.userId(), claim).subscribe({
          next: () => this.#lazyLoadEventSubject.next({ first: 0, rows: this.pageSize }),
        });
      },
    });
  }
}
