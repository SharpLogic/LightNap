import { Component, inject, input, OnInit } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent } from "@core";
import { IdentityCardComponent } from "@identity/components/controls/identity-card/identity-card.component";
import { RouteAliasService, RoutePipe } from "@routing";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";
import { IdentityService } from "@identity/services/identity.service";

@Component({
  standalone: true,
  templateUrl: './magic-link-login.component.html',
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, InputTextModule, CheckboxModule, IdentityCardComponent, ErrorListComponent],
})
export class MagicLinkLoginComponent implements OnInit {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #routeAlias = inject(RouteAliasService);

  readonly email = input("");
  readonly code = input("");

  errors: Array<string> = [];

  ngOnInit() {
    this.#blockUi.show({ message: "Verifying login..." });
    this.#identityService
      .logIn({
        type: "MagicLink",
        password: this.code(),
        login: this.email(),
        deviceDetails: navigator.userAgent,
        rememberMe: false
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigateWithReplace("user-home"),
        error: response => (this.errors = response.errorMessages),
      });
  }
}
