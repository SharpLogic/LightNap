import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT } from "@core";
import { LoginRequest, LoginSuccessResultDto, NewPasswordRequestDto, RegisterRequestDto, ResetPasswordRequestDto, SendMagicLinkEmailRequestDto, SendVerificationEmailRequestDto, VerifyCodeRequestDto, VerifyEmailRequestDto } from "@identity";

@Injectable({
  providedIn: "root",
})
export class DataService {
  #http = inject(HttpClient);
  #identityApiUrlRoot = `${inject(API_URL_ROOT)}identity/`;

  getAccessToken() {
    return this.#http.get<string>(`${this.#identityApiUrlRoot}access-token`);
  }

  logIn(loginRequest: LoginRequest) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#identityApiUrlRoot}login`, loginRequest);
  }

  register(registerRequest: RegisterRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#identityApiUrlRoot}register`, registerRequest);
  }

  logOut() {
    return this.#http.get<boolean>(`${this.#identityApiUrlRoot}logout`);
  }

  resetPassword(resetPasswordRequest: ResetPasswordRequestDto) {
    return this.#http.post<boolean>(`${this.#identityApiUrlRoot}reset-password`, resetPasswordRequest);
  }

  newPassword(newPasswordRequest: NewPasswordRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#identityApiUrlRoot}new-password`, newPasswordRequest);
  }

  verifyCode(verifyCodeRequest: VerifyCodeRequestDto) {
    return this.#http.post<string>(`${this.#identityApiUrlRoot}verify-code`, verifyCodeRequest);
  }

  requestVerificationEmail(sendVerificationEmailRequest: SendVerificationEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#identityApiUrlRoot}request-verification-email`, sendVerificationEmailRequest);
  }

  verifyEmail(verifyEmailRequest: VerifyEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#identityApiUrlRoot}verify-email`, verifyEmailRequest);
  }

  requestMagicLinkEmail(sendMagicLinkEmailRequest: SendMagicLinkEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#identityApiUrlRoot}request-magic-link`, sendMagicLinkEmailRequest);
  }


}
