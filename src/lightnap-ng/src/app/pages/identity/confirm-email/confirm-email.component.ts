import { Component, inject, input, OnInit, signal } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";

@Component({
  templateUrl: "./confirm-email.component.html",
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, InputTextModule, CheckboxModule, BrandedCardComponent, ErrorListComponent],
})
export class ConfirmEmailComponent implements OnInit {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #routeAlias = inject(RouteAliasService);

  readonly email = input("");
  readonly code = input("");

  errors = signal(new Array<string>());

  ngOnInit() {
    this.#blockUi.show({ message: "Verifying email..." });
    this.#identityService
      .verifyEmail({
        code: this.code(),
        email: this.email(),
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigateWithExtras("user-home", undefined, { replaceUrl: true }),
        error: setApiErrors(this.errors),
      });
  }
}
