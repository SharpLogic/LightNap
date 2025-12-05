import { Injectable, inject } from "@angular/core";
import { ExternalLoginRegisterRequestDto, ExternalLoginRequestDto, SearchExternalLoginsRequestDto } from "@core/backend-api";
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
   * @method searchExternalLogins
   * @description Searches for external logins based on the provided search criteria.
   * @param {SearchExternalLoginsRequestDto} searchRequestDto - The search criteria for external logins.
   * @returns {Observable<PagedResponseDto<AdminExternalLoginDto>>} An observable containing the paged response of external logins.
   */
  searchExternalLogins(searchRequestDto: SearchExternalLoginsRequestDto) {
    return this.#dataService.searchExternalLogins(searchRequestDto);
  }

  /**
   * @method removeExternalLogin
   * @description Removes an external login for a user.
   * @param {string} userId - The ID of the user.
   * @param {string} loginProvider - The login provider of the external login.
   * @param {string} providerKey - The provider key of the external login.
   * @returns {Observable<void>} An observable indicating the completion of the operation.
   */
  removeExternalLogin(userId: string, loginProvider: string, providerKey: string){
    return this.#dataService.removeExternalLogin(userId, loginProvider, providerKey);
  }

  /**
   * @method getExternalLoginResult
   * @description Gets the result of an external login.
   * @param {string} confirmationToken - The confirmation token for external login.
   * @returns {Observable<LoginSuccessResult>} An observable containing the result of the operation.
   */
  getExternalLoginResult(confirmationToken: string) {
    return this.#identityService.getLoggedIn$().pipe(
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
