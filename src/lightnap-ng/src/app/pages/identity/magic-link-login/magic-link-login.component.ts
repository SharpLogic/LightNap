import { Component, inject, input, OnInit, signal } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent } from "@core";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService } from "@core";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";
import { IdentityCardComponent } from "@core";

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

  errors = signal(new Array<string>());

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
        next: () => this.#routeAlias.navigateWithExtras("user-home", undefined, { replaceUrl: true }),
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
