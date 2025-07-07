import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { Router } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { InitializationService } from "@core/services/initialization.service";
import {
  ClaimDto,
  LoginRequestDto,
  NewPasswordRequestDto,
  RegisterRequestDto,
  ResetPasswordRequestDto,
  SendMagicLinkEmailRequestDto,
  SendVerificationEmailRequestDto,
  VerifyCodeRequestDto,
  VerifyEmailRequestDto,
} from "@identity/models";
import { RouteAliasService } from "@routing";
import { distinctUntilChanged, filter, finalize, map, of, ReplaySubject, switchMap, take, tap } from "rxjs";
import { TimerService } from "../../core/services/timer.service";
import { DataService } from "./data.service";

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
export class IdentityService {
  // How often we check if we need to refresh the token. (Evaluate the expiration every minute.)
  static readonly TokenRefreshCheckMillis = 60 * 1000;
  // How close to expiration we should try to refresh the token. (Refresh if it expires in less than 5 minutes.)
  static readonly TokenExpirationWindowMillis = 5 * 60 * 1000;

  #initializationService = inject(InitializationService);
  #timer = inject(TimerService);
  #dataService = inject(DataService);
  #routeAlias = inject(RouteAliasService);
  #router = inject(Router);

  #loggedInSubject$ = new ReplaySubject<boolean>(1);
  #loggedInRolesSubject$ = new ReplaySubject<Array<string>>(1);
  #loggedInClaimsSubject$ = new ReplaySubject<Map<string, Array<string>>>(1);

  #token?: string;
  #expires = 0;
  #requestingRefreshToken = false;
  #userId?: string;
  #userName?: string;
  #email?: string;
  #roles?: Array<string>;
  #claims?: Map<string, Array<string>>;
  #redirectUrl?: string;

  /**
   * @property loggedIn
   * @description Returns whether the user is currently logged in.
   * @returns {boolean} True if the user is logged in, false otherwise.
   * @readonly
   * @remarks This property should only be used after the initial login status of the user is known to have determined.
   * Prefer using watchLoggedIn$() to observe changes in the login status.
   */
  get loggedIn() {
    return !!this.#token;
  }

  /**
   * @property userId
   * @description Gets the user ID from the decoded token.
   * @returns {string | undefined} The user ID if available, otherwise undefined.
   * @readonly
   * @remarks This property should only be used when the user is known to be logged in.
   */
  get userId() {
    return this.#userId;
  }

  /**
   * @property userName
   * @description Gets the user name from the decoded token.
   * @returns {string | undefined} The user name if available, otherwise undefined.
   * @readonly
   * @remarks This property should only be used when the user is known to be logged in.
   */
  get userName() {
    return this.#userName;
  }

  /**
   * @property email
   * @description Gets the email address from the decoded token.
   * @returns {string | undefined} The email address if available, otherwise undefined.
   * @readonly
   * @remarks This property should only be used when the user is known to be logged in.
   */
  get email() {
    return this.#email;
  }

  /**
   * @property roles
   * @description Gets the roles from the decoded token.
   * @returns {Array<string> | undefined} The roles if available, otherwise undefined.
   * @readonly
   * @remarks This property should only be used when the user is known to be logged in.
   * Prefer using watchAnyRole$() to observe changes in the login status and roles.
   */
  get roles() {
    return this.#roles;
  }

  /**
   * @property redirectUrl
   * @description Sets the redirect URL to navigate to after a successful login.
   * @param {string} value - The originally requested URL.
   */
  setRedirectUrl(value: string) {
    this.#redirectUrl = value;
  }

  constructor() {
    this.#timer
      .watchTimer$(IdentityService.TokenRefreshCheckMillis)
      .pipe(
        takeUntilDestroyed(),
        filter(
          () =>
            !this.#requestingRefreshToken &&
            (this.#token?.length ?? 0) > 0 &&
            this.#expires - IdentityService.TokenExpirationWindowMillis < Date.now()
        )
      )
      .subscribe({
        next: () => this.#tryRefreshToken(),
      });

    this.#timer
      .watchTimer$(100)
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.#initializationService.initialized$.pipe(take(1)).subscribe({
            next: () => this.#tryRefreshToken(),
          });
        },
      });
  }

  #tryRefreshToken() {
    if (this.#requestingRefreshToken) return;
    this.#requestingRefreshToken = true;
    this.#dataService
      .getAccessToken()
      .pipe(finalize(() => (this.#requestingRefreshToken = false)))
      .subscribe({
        next: token => this.#onTokenReceived(token),
        error: () => this.#onTokenReceived(undefined),
      });
  }

  #onTokenReceived(token?: string) {
    this.#token = token;
    this.#claims = new Map<string, Array<string>>();
    this.#loggedInSubject$.next(!!this.#token);

    if (this.#token) {
      const helper = new JwtHelperService();
      const decodedToken = helper.decodeToken(this.#token);
      if (!decodedToken) throw new Error("Invalid token received");

      Object.entries(decodedToken).forEach(([key, value]) => {
        switch (key) {
          case "exp":
            this.#expires = decodedToken.exp * 1000;
            break;
          case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":
            this.#userId = value as string;
            break;
          case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name":
            this.#userName = value as string;
            break;
          case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress":
            this.#email = value as string;
            break;
          case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role":
            this.#roles = this.#ensureArray(value);
            break;
          case "aud":
          case "iat":
          case "iss":
          case "nbf":
            // Ignoring these properties for the time being.
            break;
          default:
            // Assume anything else present is a custom claim.
            this.#claims!.set(key, this.#ensureArray(value));
            break;
        }
      });
    } else {
      this.#expires = 0;
      this.#roles = [];
    }

    this.#loggedInRolesSubject$.next(this.#roles!);
    this.#loggedInClaimsSubject$.next(this.#claims!);
  }

  #ensureArray(value: any): Array<string> {
    return Array.isArray(value) ? value : value ? [value] : [];
  }

  /**
   * @method watchLoggedIn$
   * @description Watches for changes in the login status.
   * @returns {Observable<boolean>} Emits true when the user logs in and false when the user logs out.
   */
  watchLoggedIn$() {
    return this.#loggedInSubject$.pipe(distinctUntilChanged());
  }

  /**
   * @method isUserInRole
   * @description Checks if the user has a specific role.
   * @param {string} role - The role to check for.
   * @returns {boolean} True if the user has the role, false otherwise.
   * @remarks Prefer using watchRole$() to observe changes in the login status and role. This method is
   * suitable only for synchronous scenarios where the user is already known to be logged in (like at a guarded route).
   */
  isUserInRole(role: string) {
    return this.isUserInAnyRole([role]);
  }

  /**
   * @method isUserInAnyRole
   * @description Checks if the user has any of the specified roles.
   * @param {Array<string>} roles - The roles to check for.
   * @returns {boolean} True if the user has any of the roles, false otherwise.
   * @remarks Prefer using watchAnyRole$() to observe changes in the login status and roles. This method is
   * suitable only for synchronous scenarios where the user is already known to be logged in (like at a guarded route).
   */
  isUserInAnyRole(roles: Array<string>) {
    return roles.some(role => this.#roles?.includes(role));
  }

  /**
   * @method watchUserRole$
   * @description Watches for changes in the login status and checks if the user has a specific role.
   * @param {string} allowedRole - The role to check for.
   * @returns {Observable<boolean>} Emits true when the user is logged into the role, otherwise false.
   */
  watchUserRole$(allowedRole: string) {
    return this.watchAnyUserRole$([allowedRole]);
  }

  /**
   * @method watchAnyUserRole$
   * @description Watches for changes in the login status and checks if the user has any of the specified roles.
   * @param {Array<string>} allowedRoles - The roles to check for.
   * @returns {Observable<boolean>} Emits true when the user is logged into any of the roles, otherwise false.
   */
  watchAnyUserRole$(allowedRoles: Array<string>) {
    return this.#loggedInRolesSubject$.pipe(map(roles => this.isUserInAnyRole(allowedRoles)));
  }

  /**
   * @method watchUserClaim$
   * @description Watches for changes in the login status and checks if the user has the specified claim.
   * @param {ClaimDto} allowedClaim - The claim to check for.
   * @returns {Observable<boolean>} Emits true when the user is logged in with the specified claim, otherwise false.
   */
  watchUserClaim$(allowedClaim: ClaimDto) {
    return this.watchAnyUserClaim$([allowedClaim]);
  }
  /**
   * @method watchAnyUserClaim$
   * @description Watches for changes in the login status and checks if the user has any of the specified claims.
   * @param {Array<ClaimDto>} allowedClaims - The claims to check for.
   * @returns {Observable<boolean>} Emits true when the user is logged into any of the claims, otherwise false.
   */
  watchAnyUserClaim$(allowedClaims: Array<ClaimDto>) {
    return this.#loggedInClaimsSubject$.pipe(map(claims => this.doesUserHaveAnyClaim(allowedClaims)));
  }

  /**
   * @method doesUserHaveClaim
   * @description Checks if the user has the specified claim.
   * @param {ClaimDto} allowedClaim - The claim to check for, represented as a tuple of [claimType, claimValue].
   * @returns {boolean} True if the user has the claim, false otherwise.
   * @remarks This method is suitable for synchronous scenarios where the user is already known to be logged in (like at a guarded route).
   */
  doesUserHaveClaim(allowedClaim: ClaimDto) {
    return this.doesUserHaveAnyClaim([allowedClaim]);
  }

  /**
   * @method doesUserHaveAnyClaim
   * @description Checks if the user has any of the specified claims.
   * @param {Array<ClaimDto>} allowedClaims - The claims to check for, each represented as a tuple of [claimType, claimValue].
   * @returns {boolean} True if the user has any of the claims, false otherwise.
   * @remarks This method is suitable for synchronous scenarios where the user is already known to be logged in (like at a guarded route).
   */
  doesUserHaveAnyClaim(allowedClaims: Array<ClaimDto>) {
    return allowedClaims.some(claim => this.#claims?.get(claim.type)?.includes(claim.value));
  }

  /**
   * @method watchPermission$
   * @description Watches for changes in the login status and checks if the user has any of the specified roles or claims.
   * @param {Array<string>} allowedRoles - The roles to check for.
   * @param {Array<ClaimDto>} allowedClaims - The claims to check for.
   * @returns {Observable<boolean>} Emits true when the user is logged in with any of the specified roles or claims, otherwise false.
   * @remarks This is an "any" check. To check for "all" (cumulative permissions), use multiple calls.
   */
  watchUserPermission$(allowedRoles: Array<string>, allowedClaims: Array<ClaimDto>) {
    if (!allowedRoles?.length) {
      if (!allowedClaims?.length) {
        return of(false);
      } else {
        return this.watchAnyUserClaim$(allowedClaims);
      }
    }
    if (!allowedClaims?.length) {
      return this.watchAnyUserRole$(allowedRoles);
    }

    return this.watchAnyUserRole$(allowedRoles).pipe(
      switchMap(isInRole => {
        if (isInRole) return of(true);
        return this.watchAnyUserClaim$(allowedClaims);
      }),
      distinctUntilChanged()
    );
  }

    /**
     * @method doesUserHavePermission
     * @description Checks if the user has any of the specified roles or claims.
     * @param {Array<string>} allowedRoles - The roles to check for.
     * @param {Array<ClaimDto>} allowedClaims - The claims to check for.
     * @returns {boolean} True if the user has any of the specified roles or claims, false otherwise.
     * @remarks This is an "any" check. To check for "all" (cumulative permissions), use multiple calls.
     */
  doesUserHavePermission(allowedRoles: Array<string>, allowedClaims: Array<ClaimDto>) {
    return this.isUserInAnyRole(allowedRoles) || this.doesUserHaveAnyClaim(allowedClaims);
  }

  /**
   * @method getBearerToken
   * @description Returns the current authorization header string.
   * @returns {string | undefined} The bearer token authorization header string if the user is logged in, otherwise undefined.
   */
  getBearerToken() {
    if (!this.#token) return undefined;
    return `Bearer ${this.#token}`;
  }

  /**
   * @method redirectLoggedInUser
   * @description Redirects the user to the originally requested URL after a successful login or to their default landing page.
   */
  redirectLoggedInUser() {
    if (this.#redirectUrl) {
      this.#router.navigateByUrl(this.#redirectUrl);
      this.#redirectUrl = undefined;
    } else {
      this.#routeAlias.navigate("user-home");
    }
  }

  /**
   * @method logIn
   * @description Logs the user in.
   * @param {LoginRequestDto} loginRequest - The request object containing login information.
   * @returns {Observable<LoginSuccessResult>} An observable containing the result of the operation.
   */
  logIn(loginRequest: LoginRequestDto) {
    return this.#dataService.logIn(loginRequest).pipe(tap(result => this.#onTokenReceived(result.accessToken)));
  }

  /**
   * @method register
   * @description Registers a new user.
   * @param {RegisterRequestDto} registerRequest - The request object containing registration information.
   * @returns {Observable<LoginSuccessResult>} An observable containing the result of the operation.
   */
  register(registerRequest: RegisterRequestDto) {
    return this.#dataService.register(registerRequest).pipe(tap(result => this.#onTokenReceived(result?.accessToken)));
  }

  /**
   * @method logOut
   * @description Logs the user out.
   * @returns {Observable<boolean>} An observable containing the result of the operation.
   */
  logOut() {
    return this.#dataService.logOut().pipe(tap(() => this.#onTokenReceived(undefined)));
  }

  /**
   * @method verifyCode
   * @description Verifies a two-factor login code.
   * @param {VerifyCodeRequestDto} verifyCodeRequest - The request object containing the code to verify.
   * @returns {Observable<string>} An observable containing the result of the operation.
   */
  verifyCode(verifyCodeRequest: VerifyCodeRequestDto) {
    return this.#dataService.verifyCode(verifyCodeRequest).pipe(tap(token => this.#onTokenReceived(token)));
  }

  /**
   * @method resetPassword
   * @description Resets the user's password.
   * @param {ResetPasswordRequestDto} resetPasswordRequest - The request object containing password reset information.
   * @returns {Observable<boolean>} An observable containing the result of the operation.
   */
  resetPassword(resetPasswordRequest: ResetPasswordRequestDto) {
    return this.#dataService.resetPassword(resetPasswordRequest);
  }

  /**
   * @method newPassword
   * @description Sets a new password for the user.
   * @param {NewPasswordRequestDto} newPasswordRequest - The request object containing the new password information.
   * @returns {Observable<string>} An observable containing the result of the operation.
   */
  newPassword(newPasswordRequest: NewPasswordRequestDto) {
    return this.#dataService.newPassword(newPasswordRequest).pipe(tap(result => this.#onTokenReceived(result.accessToken)));
  }

  /**
   * @method requestVerificationEmail
   * @description Requests a new verification email.
   * @param {SendVerificationEmailRequestDto} sendVerificationEmailRequest - The email address to send the verification email to.
   * @returns {Observable<boolean>} An observable containing the result of the operation.
   */
  requestVerificationEmail(sendVerificationEmailRequest: SendVerificationEmailRequestDto) {
    return this.#dataService.requestVerificationEmail(sendVerificationEmailRequest);
  }

  /**
   * @method verifyEmail
   * @description Verifies an email address.
   * @param {VerifyEmailRequestDto} verifyEmailRequest - The request object containing the email and verification code.
   * @returns {Observable<boolean>} An observable containing the result of the operation.
   */
  verifyEmail(verifyEmailRequest: VerifyEmailRequestDto) {
    return this.#dataService.verifyEmail(verifyEmailRequest);
  }

  /**
   * @method requestMagicLinkEmail
   * @description Requests a new magic link email.
   * @param {SendMagicLinkEmailRequestDto} sendMagicLinkEmailRequest - The email address to send the magic link email to.
   * @returns {Observable<boolean>} An observable containing the result of the operation.
   */
  requestMagicLinkEmail(sendMagicLinkEmailRequest: SendMagicLinkEmailRequestDto) {
    return this.#dataService.requestMagicLinkEmail(sendMagicLinkEmailRequest);
  }
}
