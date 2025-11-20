import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { LoginRequestDto, LoginSuccessResultDto, RegisterRequestDto, ResetPasswordRequestDto, NewPasswordRequestDto, VerifyCodeRequestDto, SendVerificationEmailRequestDto, VerifyEmailRequestDto, SendMagicLinkEmailRequestDto, ChangePasswordRequestDto, ChangeEmailRequestDto, ConfirmChangeEmailRequestDto, DeviceDto } from "../dtos";
import { DeviceHelper } from "../helpers/device.helper";
import { tap } from "rxjs";

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
    return this.#http
      .get<Array<DeviceDto>>(`${this.#apiUrlRoot}devices`)
      .pipe(tap(devices => devices.forEach(device => DeviceHelper.rehydrate(device))));
  }

  revokeDevice(deviceId: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}devices/${deviceId}`);
  }

}
