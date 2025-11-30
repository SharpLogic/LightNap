import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import {
    ChangeEmailRequestDto,
    ChangePasswordRequestDto,
    ConfirmChangeEmailRequestDto,
    DeviceDto,
    ExternalLoginRegisterRequestDto,
    ExternalLoginRequestDto,
    ExternalLoginSuccessResultDto,
    LoginRequestDto,
    LoginSuccessResultDto,
    NewPasswordRequestDto,
    RegisterRequestDto,
    ResetPasswordRequestDto,
    SendMagicLinkEmailRequestDto,
    SendVerificationEmailRequestDto,
    VerifyCodeRequestDto,
    VerifyEmailRequestDto,
} from "../dtos";

@Injectable({
  providedIn: "root",
})
export class IdentityDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = "/api/identity/";

  getAccessToken() {
    return this.#http.get<string>(`${this.#apiUrlRoot}access-token`);
  }

  logIn(loginRequest: LoginRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}login`, loginRequest);
  }

  register(registerRequest: RegisterRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}register`, registerRequest);
  }

  getExternalLoginResult(confirmationToken: string) {
    return this.#http.get<ExternalLoginSuccessResultDto>(`${this.#apiUrlRoot}external-login-result/${confirmationToken}`);
  }

  completeExternalLogin(confirmationToken: string, loginRequest: ExternalLoginRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}external-login-complete/${confirmationToken}`, loginRequest);
  }

  completeExternalLoginRegistration(confirmationToken: string, registerRequest: ExternalLoginRegisterRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}external-login-registration/${confirmationToken}`, registerRequest);
  }

  logOut() {
    return this.#http.get<boolean>(`${this.#apiUrlRoot}logout`);
  }

  resetPassword(resetPasswordRequest: ResetPasswordRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}reset-password`, resetPasswordRequest);
  }

  newPassword(newPasswordRequest: NewPasswordRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}new-password`, newPasswordRequest);
  }

  verifyCode(verifyCodeRequest: VerifyCodeRequestDto) {
    return this.#http.post<string>(`${this.#apiUrlRoot}verify-code`, verifyCodeRequest);
  }

  requestVerificationEmail(sendVerificationEmailRequest: SendVerificationEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}request-verification-email`, sendVerificationEmailRequest);
  }

  verifyEmail(verifyEmailRequest: VerifyEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}verify-email`, verifyEmailRequest);
  }

  requestMagicLinkEmail(sendMagicLinkEmailRequest: SendMagicLinkEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}request-magic-link`, sendMagicLinkEmailRequest);
  }

  changePassword(changePasswordRequest: ChangePasswordRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}change-password`, changePasswordRequest);
  }

  changeEmail(changeEmailRequest: ChangeEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}change-email`, changeEmailRequest);
  }

  confirmEmailChange(confirmChangeEmailRequest: ConfirmChangeEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}confirm-email-change`, confirmChangeEmailRequest);
  }

  getDevices() {
    return this.#http.get<Array<DeviceDto>>(`${this.#apiUrlRoot}devices`);
  }

  revokeDevice(deviceId: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}devices/${deviceId}`);
  }
}
