import { Component, computed, inject, input, signal } from "@angular/core";
import { ExternalLoginSuccessType } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { ExternalLoginService } from "@core/services/external-login.service";
import { IdentityService } from "@core/services/identity.service";
import { finalize, switchMap, tap } from "rxjs";

@Component({
  templateUrl: "./callback.component.html",
  imports: [ApiResponseComponent, ErrorListComponent],
})
export class CallbackComponent {
  readonly #identityService = inject(IdentityService);
  readonly #externalLoginService = inject(ExternalLoginService);
  readonly #blockUi = inject(BlockUiService);
  readonly #routeAlias = inject(RouteAliasService);

  readonly token = input.required<string>();

  readonly externalLoginResult = computed(() => {
    this.#blockUi.show({ message: "Processing external login..." });

    return this.#identityService.getLoggedIn$().pipe(
      finalize(() => this.#blockUi.hide()),
      switchMap(isLoggedIn =>
        this.#externalLoginService.getExternalLoginResult(this.token()).pipe(
          tap(loginResult => {
            switch (loginResult.type) {
              case ExternalLoginSuccessType.AlreadyLinkedToDifferentAccount:
                this.errors.set([
                  "This external account is already linked to a different user account. Please use a different external account or log in with your existing account.",
                ]);
                break;
              case ExternalLoginSuccessType.AlreadyLinked:
                if (isLoggedIn) {
                  this.#routeAlias.navigateWithExtras("my-external-logins", null, { replaceUrl: true });
                } else {
                  this.#routeAlias.navigateWithExtras("external-login-complete", this.token(), { replaceUrl: true });
                }
                break;
              case ExternalLoginSuccessType.NewAccountLink:
                this.#routeAlias.navigateWithExtras("my-external-logins", null, { replaceUrl: true });
                break;
              case ExternalLoginSuccessType.RequiresRegistration:
                this.#routeAlias.navigateWithExtras("external-login-register", this.token(), { replaceUrl: true });
                break;
              default:
                throw new Error(`Unexpected ExternalLoginSuccessResult.type: '${loginResult.type}'`);
            }
          })
        )
      )
    );
  });

  readonly errors = signal(new Array<string>());
}
