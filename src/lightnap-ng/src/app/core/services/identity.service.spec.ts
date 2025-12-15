import { beforeEach, describe, expect, it, vi, type MockedObject } from "vitest";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { JwtHelperService } from "@auth0/angular-jwt";
import { ChangePasswordRequestDto, NewPasswordRequestDto, RegisterRequestDto, ResetPasswordRequestDto, VerifyCodeRequestDto } from "@core";
import {
  getLogInResponseMock,
  getRegisterResponseMock,
  getNewPasswordResponseMock,
  getVerifyCodeResponseMock,
  getGetDevicesResponseMock,
  LightNapWebApiService,
} from "@core/backend-api/services/lightnap-api";
import { createLightNapWebApiServiceSpy } from "@testing/helpers";
import { IdentityDtoBuilder } from "@testing/builders";
import { of, throwError } from "rxjs";
import { IdentityService } from "./identity.service";
import { InitializationService } from "./initialization.service";
import { TimerService } from "./timer.service";

describe("IdentityService", () => {
  let service: IdentityService;
  let initializationServiceSpy: MockedObject<InitializationService>;
  let webApiServiceSpy: MockedObject<LightNapWebApiService>;
  let timerServiceSpy: MockedObject<TimerService>;

  beforeEach(() => {
    const timerSpy = {
      watchTimer$: vi.fn().mockName("TimerService.watchTimer$"),
    };
    const initializationSpy = {
      initialized$: vi.fn().mockName("InitializationService.initialized$"),
    };
    const webApiSpy = createLightNapWebApiServiceSpy();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        IdentityService,
        { provide: InitializationService, useValue: initializationSpy },
        { provide: LightNapWebApiService, useValue: webApiSpy },
        { provide: TimerService, useValue: timerSpy },
        JwtHelperService,
      ],
    });

    initializationServiceSpy = TestBed.inject(InitializationService) as MockedObject<InitializationService>;
    Object.defineProperty(initializationServiceSpy, "initialized$", { get: () => of(true) });

    timerServiceSpy = TestBed.inject(TimerService) as MockedObject<TimerService>;
    timerServiceSpy.watchTimer$.mockReturnValue(of(0));

    webApiServiceSpy = TestBed.inject(LightNapWebApiService) as MockedObject<LightNapWebApiService>;
    // Configure default return values that throw to catch unconfigured mocks
    webApiServiceSpy.getAccessToken.mockReturnValue(of(null as any));

    service = TestBed.inject(IdentityService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should initialize and try to refresh token", () => {
    service = TestBed.inject(IdentityService);
    expect(timerServiceSpy.watchTimer$).toHaveBeenCalled();
    expect(webApiServiceSpy.getAccessToken).toHaveBeenCalled();
  });

  describe("login and authentication", () => {
    it("should log in and set token", async () => {
      const token = IdentityDtoBuilder.createTestToken();
      const loginRequest = IdentityDtoBuilder.createLoginRequest();
      const loginResponse = getLogInResponseMock({ accessToken: token });
      webApiServiceSpy.logIn.mockReturnValue(of(loginResponse) as any);

      service.logIn(loginRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.logIn).toHaveBeenCalledWith(loginRequest);
        },
      });
    });

    it("should log out and clear token", async () => {
      webApiServiceSpy.logOut.mockReturnValue(of(true) as any);
      service.logOut().subscribe({
        next: () => {
          expect(service.getBearerToken()).toBeUndefined();
          expect(webApiServiceSpy.logOut).toHaveBeenCalled();
        },
      });
    });

    it("should handle login errors gracefully", async () => {
      const loginRequest = IdentityDtoBuilder.createLoginRequest();
      const errorMessage = "Invalid credentials";

      webApiServiceSpy.logIn.mockReturnValue(throwError(() => new Error(errorMessage)));

      service.logIn(loginRequest).subscribe({
        next: () => expect.fail("Expected error but got success"),
        error: error => {
          expect(error.message).toBe(errorMessage);
          expect(service.getBearerToken()).toBeUndefined();
        },
      });
    });
  });

  describe("registration", () => {
    it("should register and set token", async () => {
      const token = IdentityDtoBuilder.createTestToken();
      const registerRequest: RegisterRequestDto = IdentityDtoBuilder.createRegisterRequest() as any;
      const registerResponse = getRegisterResponseMock({ accessToken: token });
      webApiServiceSpy.register.mockReturnValue(of(registerResponse) as any);

      service.register(registerRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.register).toHaveBeenCalledWith(registerRequest);
        },
      });
    });
  });

  describe("verification and codes", () => {
    it("should verify code and set token", async () => {
      const token = IdentityDtoBuilder.createTestToken();
      const verifyCodeRequest: VerifyCodeRequestDto = {} as any;
      webApiServiceSpy.verifyCode.mockReturnValue(of(token) as any);

      service.verifyCode(verifyCodeRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.verifyCode).toHaveBeenCalledWith(verifyCodeRequest);
        },
      });
    });

    it("should detect when a token is expired", async () => {
      const expiredToken = IdentityDtoBuilder.createExpiredToken();
      const loginResponse = getLogInResponseMock({ accessToken: expiredToken });
      webApiServiceSpy.logIn.mockReturnValue(of(loginResponse) as any);
      service.logIn({} as any).subscribe({
        next: () => {
          expect(service.isTokenExpired()).toBe(true);
        },
      });
    });

    it("should request magic link email", async () => {
      const sendMagicLinkEmailRequest = {} as any;
      webApiServiceSpy.requestMagicLinkEmail.mockReturnValue(of(true) as any);

      service.requestMagicLinkEmail(sendMagicLinkEmailRequest).subscribe(result => {
        expect(result).toBe(true);
        expect(webApiServiceSpy.requestMagicLinkEmail).toHaveBeenCalledWith(sendMagicLinkEmailRequest);
      });
    });

    it("should request verification email", async () => {
      const sendVerificationEmailRequest = {} as any;
      webApiServiceSpy.requestVerificationEmail.mockReturnValue(of(true) as any);

      service.requestVerificationEmail(sendVerificationEmailRequest).subscribe(result => {
        expect(result).toBe(true);
        expect(webApiServiceSpy.requestVerificationEmail).toHaveBeenCalledWith(sendVerificationEmailRequest);
      });
    });

    it("should verify email", async () => {
      const verifyEmailRequest = {} as any;
      webApiServiceSpy.verifyEmail.mockReturnValue(of(true) as any);

      service.verifyEmail(verifyEmailRequest).subscribe(result => {
        expect(result).toBe(true);
        expect(webApiServiceSpy.verifyEmail).toHaveBeenCalledWith(verifyEmailRequest);
      });
    });
  });

  describe("password management", () => {
    it("should reset password", async () => {
      const resetPasswordRequest: ResetPasswordRequestDto = IdentityDtoBuilder.createResetPasswordRequest() as any;
      webApiServiceSpy.resetPassword.mockReturnValue(of({} as any));

      service.resetPassword(resetPasswordRequest).subscribe({
        next: () => {
          expect(webApiServiceSpy.resetPassword).toHaveBeenCalledWith(resetPasswordRequest);
        },
      });
    });

    it("should set new password and set token", async () => {
      const token = IdentityDtoBuilder.createTestToken();
      const newPasswordRequest: NewPasswordRequestDto = IdentityDtoBuilder.createNewPasswordRequest() as any;
      const newPasswordResponse = getNewPasswordResponseMock({ accessToken: token });
      webApiServiceSpy.newPassword.mockReturnValue(of(newPasswordResponse) as any);

      service.newPassword(newPasswordRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.newPassword).toHaveBeenCalledWith(newPasswordRequest);
        },
      });
    });

    it("should change password", async () => {
      const changePasswordRequest: ChangePasswordRequestDto = {} as any;
      webApiServiceSpy.changePassword.mockReturnValue(of({} as any));

      service.changePassword(changePasswordRequest).subscribe({
        next: () => {
          expect(webApiServiceSpy.changePassword).toHaveBeenCalledWith(changePasswordRequest);
        },
      });
    });
  });

  describe("device management", () => {
    it("should get devices", async () => {
      const devicesResponse = getGetDevicesResponseMock();
      webApiServiceSpy.getDevices.mockReturnValue(of(devicesResponse) as any);

      service.getDevices().subscribe({
        next: () => {
          expect(webApiServiceSpy.getDevices).toHaveBeenCalled();
        },
      });
    });

    it("should revoke device", async () => {
      const deviceId = "device123";
      webApiServiceSpy.revokeDevice.mockReturnValue(of({} as any));

      service.revokeDevice(deviceId).subscribe({
        next: () => {
          expect(webApiServiceSpy.revokeDevice).toHaveBeenCalledWith(deviceId);
        },
      });
    });
  });

  describe("claims and user information", () => {
    it("should check if user has a specific claim", async () => {
      const tokenWithClaims = IdentityDtoBuilder.createTokenWithClaims({
        claimType: "claimValue",
      });

      const loginResponse = getLogInResponseMock({ accessToken: tokenWithClaims });
      webApiServiceSpy.logIn.mockReturnValue(of(loginResponse) as any);
      service.logIn({} as any).subscribe({
        next: () => {
          const hasClaim = service.hasUserClaim({ type: "claimType", value: "claimValue" });
          expect(hasClaim).toBe(true);
        },
      });
    });

    it("should emit correct values from watchAnyUserClaim$", async () => {
      const tokenWithClaims = IdentityDtoBuilder.createTokenWithClaims({
        claimType: "claimValue",
      });

      const emittedValues = new Array<boolean>();
      service.watchAnyUserClaim$([{ type: "claimType", value: "claimValue" }]).subscribe(hasAnyClaim => {
        emittedValues.push(hasAnyClaim);
        if (emittedValues.length === 2) {
          expect(emittedValues).toEqual([false, true]);
        }
      });

      const loginResponse = getLogInResponseMock({ accessToken: tokenWithClaims });
      webApiServiceSpy.logIn.mockReturnValue(of(loginResponse) as any);
      service.logIn({} as any).subscribe();
    });
  });
});
