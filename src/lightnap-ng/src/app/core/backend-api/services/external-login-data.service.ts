import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import {
  ExternalLoginRegisterRequestDto,
  ExternalLoginRequestDto,
  ExternalLoginSuccessResultDto,
  LoginSuccessResultDto,
  SupportedExternalLoginDto,
} from "../dtos";

@Injectable({
  providedIn: "root",
})
export class ExternalLoginDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = "/api/ExternalLogin/";

  getSupportedLogins() {
    return this.#http.get<Array<SupportedExternalLoginDto>>(`${this.#apiUrlRoot}supported`);
  }

  getExternalLoginResult(confirmationToken: string) {
    return this.#http.get<ExternalLoginSuccessResultDto>(`${this.#apiUrlRoot}result/${confirmationToken}`);
  }

  completeExternalLogin(confirmationToken: string, loginRequest: ExternalLoginRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}complete/${confirmationToken}`, loginRequest);
  }

  completeExternalLoginRegistration(confirmationToken: string, registerRequest: ExternalLoginRegisterRequestDto) {
    return this.#http.post<LoginSuccessResultDto>(`${this.#apiUrlRoot}register/${confirmationToken}`, registerRequest);
  }
}
