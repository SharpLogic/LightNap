import { Injectable, inject } from "@angular/core";
import { ExternalLoginRegisterRequestDto, ExternalLoginRequestDto } from "@core/backend-api";
import { ExternalLoginDataService } from "@core/backend-api/services/external-login-data.service";
import { switchMap, take, tap, shareReplay } from "rxjs";
import { IdentityService } from "./identity.service";

/**
 * Service responsible for managing user identity, including authentication and token management.
 *
 * @remarks
 * This service handles the acquisition, storage, and refreshing of authentication tokens. It also provides
 * methods for logging in, registering, logging out, verifying codes, and resetting passwords.
 */
@Injectable({
  providedIn: "root",
})
export class ExternalLoginService {
  #dataService = inject(ExternalLoginDataService);
  #identityService = inject(IdentityService);
  #supportedLogins$ = this.#dataService.getSupportedLogins().pipe(shareReplay(1));

  /**
   * @method getSupportedLogins
   * @description Retrieves the list of supported external login providers.
   * @returns {Observable<SupportedExternalLoginDto[]>} An observable containing the list of supported external login providers.
   */
  getSupportedLogins() {
    return this.#supportedLogins$;
  }

  /**
   * @method getExternalLoginResult
   * @description Gets the result of an external login.
   * @param {string} confirmationToken - The confirmation token for external login.
   * @returns {Observable<LoginSuccessResult>} An observable containing the result of the operation.
   */
  getExternalLoginResult(confirmationToken: string) {
    return this.#identityService.watchLoggedIn$().pipe(
      take(1),
      switchMap(_ => this.#dataService.getExternalLoginResult(confirmationToken))
    );
  }

  /**
   * @method completeExternalLogin
   * @description Completes an external login for a returning user.
   * @param {string} confirmationToken - The confirmation token for external login.
   * @param {ExternalLoginRequestDto} loginRequest - The request object containing login information.
   * @returns {Observable<LoginSuccessResult>} An observable containing the result of the operation.
   */
  completeExternalLogin(confirmationToken: string, loginRequest: ExternalLoginRequestDto) {
    return this.#dataService
      .completeExternalLogin(confirmationToken, loginRequest)
      .pipe(tap(result => this.#identityService.setToken(result?.accessToken)));
  }

  /**
   * @method completeExternalLoginRegistration
   * @description Completes an external login by registering a new user.
   * @param {string} confirmationToken - The confirmation token for external login.
   * @param {ExternalLoginRegisterRequestDto} registerRequest - The request object containing registration information.
   * @returns {Observable<LoginSuccessResult>} An observable containing the result of the operation.
   */
  completeExternalLoginRegistration(confirmationToken: string, registerRequest: ExternalLoginRegisterRequestDto) {
    return this.#dataService
      .completeExternalLoginRegistration(confirmationToken, registerRequest)
      .pipe(tap(result => this.#identityService.setToken(result?.accessToken)));
  }
}
