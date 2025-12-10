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
  let initializationServiceSpy: jasmine.SpyObj<InitializationService>;
  let webApiServiceSpy: jasmine.SpyObj<LightNapWebApiService>;
  let timerServiceSpy: jasmine.SpyObj<TimerService>;

  beforeEach(() => {
    const timerSpy = jasmine.createSpyObj<TimerService>("TimerService", ["watchTimer$"]);
    const initializationSpy = jasmine.createSpyObj<InitializationService>("InitializationService", ["initialized$"]);
    const webApiSpy = createLightNapWebApiServiceSpy(jasmine);

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

    initializationServiceSpy = TestBed.inject(InitializationService) as jasmine.SpyObj<InitializationService>;
    Object.defineProperty(initializationServiceSpy, "initialized$", { get: () => of(true) });

    timerServiceSpy = TestBed.inject(TimerService) as jasmine.SpyObj<TimerService>;
    timerServiceSpy.watchTimer$.and.returnValue(of(0));

    webApiServiceSpy = TestBed.inject(LightNapWebApiService) as jasmine.SpyObj<LightNapWebApiService>;
    // Configure default return values that throw to catch unconfigured mocks
    webApiServiceSpy.getAccessToken.and.returnValue(of(null as any));

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
    it("should log in and set token", done => {
      const token = IdentityDtoBuilder.createTestToken();
      const loginRequest = IdentityDtoBuilder.createLoginRequest();
      const loginResponse = getLogInResponseMock({ accessToken: token });
      webApiServiceSpy.logIn.and.returnValue(of(loginResponse) as any);

      service.logIn(loginRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.logIn).toHaveBeenCalledWith(loginRequest);
          done();
        },
      });
    });

    it("should log out and clear token", done => {
      webApiServiceSpy.logOut.and.returnValue(of(true) as any);
      service.logOut().subscribe({
        next: () => {
          expect(service.getBearerToken()).toBeUndefined();
          expect(webApiServiceSpy.logOut).toHaveBeenCalled();
          done();
        },
      });
    });

    it("should handle login errors gracefully", done => {
      const loginRequest = IdentityDtoBuilder.createLoginRequest();
      const errorMessage = "Invalid credentials";

      webApiServiceSpy.logIn.and.returnValue(throwError(() => new Error(errorMessage)));

      service.logIn(loginRequest).subscribe({
        next: () => fail("Expected error but got success"),
        error: error => {
          expect(error.message).toBe(errorMessage);
          expect(service.getBearerToken()).toBeUndefined();
          done();
        },
      });
    });
  });

  describe("registration", () => {
    it("should register and set token", done => {
      const token = IdentityDtoBuilder.createTestToken();
      const registerRequest: RegisterRequestDto = IdentityDtoBuilder.createRegisterRequest() as any;
      const registerResponse = getRegisterResponseMock({ accessToken: token });
      webApiServiceSpy.register.and.returnValue(of(registerResponse) as any);

      service.register(registerRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.register).toHaveBeenCalledWith(registerRequest);
          done();
        },
      });
    });
  });

  describe("verification and codes", () => {
    it("should verify code and set token", done => {
      const token = IdentityDtoBuilder.createTestToken();
      const verifyCodeRequest: VerifyCodeRequestDto = {} as any;
      webApiServiceSpy.verifyCode.and.returnValue(of(token) as any);

      service.verifyCode(verifyCodeRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.verifyCode).toHaveBeenCalledWith(verifyCodeRequest);
          done();
        },
      });
    });

    it("should detect when a token is expired", done => {
      const expiredToken = IdentityDtoBuilder.createExpiredToken();
      const loginResponse = getLogInResponseMock({ accessToken: expiredToken });
      webApiServiceSpy.logIn.and.returnValue(of(loginResponse) as any);
      service.logIn({} as any).subscribe({
        next: () => {
          expect(service.isTokenExpired()).toBeTrue();
          done();
        },
      });
    });

    it("should request magic link email", done => {
      const sendMagicLinkEmailRequest = {} as any;
      webApiServiceSpy.requestMagicLinkEmail.and.returnValue(of(true) as any);

      service.requestMagicLinkEmail(sendMagicLinkEmailRequest).subscribe(result => {
        expect(result).toBe(true);
        expect(webApiServiceSpy.requestMagicLinkEmail).toHaveBeenCalledWith(sendMagicLinkEmailRequest);
        done();
      });
    });

    it("should request verification email", done => {
      const sendVerificationEmailRequest = {} as any;
      webApiServiceSpy.requestVerificationEmail.and.returnValue(of(true) as any);

      service.requestVerificationEmail(sendVerificationEmailRequest).subscribe(result => {
        expect(result).toBe(true);
        expect(webApiServiceSpy.requestVerificationEmail).toHaveBeenCalledWith(sendVerificationEmailRequest);
        done();
      });
    });

    it("should verify email", done => {
      const verifyEmailRequest = {} as any;
      webApiServiceSpy.verifyEmail.and.returnValue(of(true) as any);

      service.verifyEmail(verifyEmailRequest).subscribe(result => {
        expect(result).toBe(true);
        expect(webApiServiceSpy.verifyEmail).toHaveBeenCalledWith(verifyEmailRequest);
        done();
      });
    });
  });

  describe("password management", () => {
    it("should reset password", done => {
      const resetPasswordRequest: ResetPasswordRequestDto = IdentityDtoBuilder.createResetPasswordRequest() as any;
      webApiServiceSpy.resetPassword.and.returnValue(of({} as any));

      service.resetPassword(resetPasswordRequest).subscribe({
        next: () => {
          expect(webApiServiceSpy.resetPassword).toHaveBeenCalledWith(resetPasswordRequest);
          done();
        },
      });
    });

    it("should set new password and set token", done => {
      const token = IdentityDtoBuilder.createTestToken();
      const newPasswordRequest: NewPasswordRequestDto = IdentityDtoBuilder.createNewPasswordRequest() as any;
      const newPasswordResponse = getNewPasswordResponseMock({ accessToken: token });
      webApiServiceSpy.newPassword.and.returnValue(of(newPasswordResponse) as any);

      service.newPassword(newPasswordRequest).subscribe({
        next: () => {
          expect(service.getBearerToken()).toBe(`Bearer ${token}`);
          expect(webApiServiceSpy.newPassword).toHaveBeenCalledWith(newPasswordRequest);
          done();
        },
      });
    });

    it("should change password", done => {
      const changePasswordRequest: ChangePasswordRequestDto = {} as any;
      webApiServiceSpy.changePassword.and.returnValue(of({} as any));

      service.changePassword(changePasswordRequest).subscribe({
        next: () => {
          expect(webApiServiceSpy.changePassword).toHaveBeenCalledWith(changePasswordRequest);
          done();
        },
      });
    });
  });

  describe("device management", () => {
    it("should get devices", done => {
      const devicesResponse = getGetDevicesResponseMock();
      webApiServiceSpy.getDevices.and.returnValue(of(devicesResponse) as any);

      service.getDevices().subscribe({
        next: () => {
          expect(webApiServiceSpy.getDevices).toHaveBeenCalled();
          done();
        },
      });
    });

    it("should revoke device", done => {
      const deviceId = "device123";
      webApiServiceSpy.revokeDevice.and.returnValue(of({} as any));

      service.revokeDevice(deviceId).subscribe({
        next: () => {
          expect(webApiServiceSpy.revokeDevice).toHaveBeenCalledWith(deviceId);
          done();
        },
      });
    });
  });

  describe("claims and user information", () => {
    it("should check if user has a specific claim", done => {
      const tokenWithClaims = IdentityDtoBuilder.createTokenWithClaims({
        claimType: "claimValue",
      });

      const loginResponse = getLogInResponseMock({ accessToken: tokenWithClaims });
      webApiServiceSpy.logIn.and.returnValue(of(loginResponse) as any);
      service.logIn({} as any).subscribe({
        next: () => {
          const hasClaim = service.hasUserClaim({ type: "claimType", value: "claimValue" });
          expect(hasClaim).toBeTrue();
          done();
        },
      });
    });

    it("should emit correct values from watchAnyUserClaim$", done => {
      const tokenWithClaims = IdentityDtoBuilder.createTokenWithClaims({
        claimType: "claimValue",
      });

      const emittedValues = new Array<boolean>();
      service.watchAnyUserClaim$([{ type: "claimType", value: "claimValue" }]).subscribe(hasAnyClaim => {
        emittedValues.push(hasAnyClaim);
        if (emittedValues.length === 2) {
          expect(emittedValues).toEqual([false, true]);
          done();
        }
      });

      const loginResponse = getLogInResponseMock({ accessToken: tokenWithClaims });
      webApiServiceSpy.logIn.and.returnValue(of(loginResponse) as any);
      service.logIn({} as any).subscribe();
    });
  });
});
