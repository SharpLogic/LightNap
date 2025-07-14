import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { JwtHelperService } from "@auth0/angular-jwt";
import { of } from "rxjs";
import { IdentityService } from "./identity.service";
import { InitializationService } from "./initialization.service";
import { TimerService } from "./timer.service";
import {
  RegisterRequestDto,
  VerifyCodeRequestDto,
  ResetPasswordRequestDto,
  NewPasswordRequestDto,
  ChangePasswordRequestDto,
} from "@core/backend-api";
import { IdentityDataService } from "@core/backend-api/services/identity-data.service";

describe("IdentityService", () => {
  let service: IdentityService;
  let initializationServiceSpy: jasmine.SpyObj<InitializationService>;
  let dataServiceSpy: jasmine.SpyObj<IdentityDataService>;
  let timerServiceSpy: jasmine.SpyObj<TimerService>;
  // Using a valid JWT token for testing purposes. IdentityService will try to parse it so it might as well work.
  const token =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

  beforeEach(() => {
    const dataSpy = jasmine.createSpyObj("IdentityDataService", [
      "getAccessToken",
      "logIn",
      "logOut",
      "newPassword",
      "register",
      "requestMagicLinkEmail",
      "requestVerificationEmail",
      "resetPassword",
      "verifyCode",
      "verifyEmail",
      "getDevices",
      "revokeDevice",
      "changePassword",
    ]);
    const timerSpy = jasmine.createSpyObj("TimerService", ["watchTimer$"]);
    const initializationSpy = jasmine.createSpyObj("InitializationService", ["initialized$"]);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        IdentityService,
        { provide: InitializationService, useValue: initializationSpy },
        { provide: IdentityDataService, useValue: dataSpy },
        { provide: TimerService, useValue: timerSpy },
        JwtHelperService,
      ],
    });

    initializationServiceSpy = TestBed.inject(InitializationService) as jasmine.SpyObj<InitializationService>;
    Object.defineProperty(initializationServiceSpy, "initialized$", { get: () => of(true) });

    timerServiceSpy = TestBed.inject(TimerService) as jasmine.SpyObj<TimerService>;
    timerServiceSpy.watchTimer$.and.returnValue(of(0));

    dataServiceSpy = TestBed.inject(IdentityDataService) as jasmine.SpyObj<IdentityDataService>;
    dataServiceSpy.getAccessToken.and.returnValue(of(""));

    service = TestBed.inject(IdentityService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should initialize and try to refresh token", () => {
    service = TestBed.inject(IdentityService);
    expect(timerServiceSpy.watchTimer$).toHaveBeenCalled();
    expect(dataServiceSpy.getAccessToken).toHaveBeenCalled();
  });

  it("should log in and set token", () => {
    const loginRequest = {} as any;
    dataServiceSpy.logIn.and.returnValue(of({ accessToken: token, type: "AccessToken" }));
    service.logIn(loginRequest).subscribe(() => {
      expect(service.getBearerToken()).toBe(`Bearer ${token}`);
    });
    expect(dataServiceSpy.logIn).toHaveBeenCalledWith(loginRequest);
  });

  it("should log out and clear token", () => {
    dataServiceSpy.logOut.and.returnValue(of(true));
    service.logOut().subscribe(() => {
      expect(service.getBearerToken()).toBeUndefined();
    });
    expect(dataServiceSpy.logOut).toHaveBeenCalled();
  });

  it("should register and set token", () => {
    const registerRequest: RegisterRequestDto = {} as any;
    dataServiceSpy.register.and.returnValue(of({ accessToken: token, type: "AccessToken" }));
    service.register(registerRequest).subscribe(() => {
      expect(service.getBearerToken()).toBe(`Bearer ${token}`);
    });
    expect(dataServiceSpy.register).toHaveBeenCalledWith(registerRequest);
  });

  it("should verify code and set token", () => {
    const verifyCodeRequest: VerifyCodeRequestDto = {} as any;
    dataServiceSpy.verifyCode.and.returnValue(of(token));
    service.verifyCode(verifyCodeRequest).subscribe(() => {
      expect(service.getBearerToken()).toBe(`Bearer ${token}`);
    });
    expect(dataServiceSpy.verifyCode).toHaveBeenCalledWith(verifyCodeRequest);
  });

  it("should reset password", () => {
    const resetPasswordRequest: ResetPasswordRequestDto = <any>{};
    dataServiceSpy.resetPassword.and.returnValue(of({} as any));
    service.resetPassword(resetPasswordRequest).subscribe();
    expect(dataServiceSpy.resetPassword).toHaveBeenCalledWith(resetPasswordRequest);
  });

  it("should set new password and set token", () => {
    const newPasswordRequest: NewPasswordRequestDto = {} as any;
    dataServiceSpy.newPassword.and.returnValue(of({ accessToken: token, type: "AccessToken" }));
    service.newPassword(newPasswordRequest).subscribe(() => {
      expect(service.getBearerToken()).toBe(`Bearer ${token}`);
    });
    expect(dataServiceSpy.newPassword).toHaveBeenCalledWith(newPasswordRequest);
  });

  it("should request magic link email", () => {
    const sendMagicLinkEmailRequest = {} as any;
    dataServiceSpy.requestMagicLinkEmail.and.returnValue(of(true));
    service.requestMagicLinkEmail(sendMagicLinkEmailRequest).subscribe(result => {
      expect(result).toBe(true);
    });
    expect(dataServiceSpy.requestMagicLinkEmail).toHaveBeenCalledWith(sendMagicLinkEmailRequest);
  });

  it("should request verification email", () => {
    const sendVerificationEmailRequest = {} as any;
    dataServiceSpy.requestVerificationEmail.and.returnValue(of(true));
    service.requestVerificationEmail(sendVerificationEmailRequest).subscribe(result => {
      expect(result).toBe(true);
    });
    expect(dataServiceSpy.requestVerificationEmail).toHaveBeenCalledWith(sendVerificationEmailRequest);
  });

  it("should verify email", () => {
    const verifyEmailRequest = {} as any;
    dataServiceSpy.verifyEmail.and.returnValue(of(true));
    service.verifyEmail(verifyEmailRequest).subscribe(result => {
      expect(result).toBe(true);
    });
    expect(dataServiceSpy.verifyEmail).toHaveBeenCalledWith(verifyEmailRequest);
  });

  it("should get devices", () => {
    dataServiceSpy.getDevices.and.returnValue(of({} as any));

    service.getDevices().subscribe();

    expect(dataServiceSpy.getDevices).toHaveBeenCalled();
  });

  it("should revoke device", () => {
    const deviceId = "device123";
    dataServiceSpy.revokeDevice.and.returnValue(of({} as any));

    service.revokeDevice(deviceId).subscribe();

    expect(dataServiceSpy.revokeDevice).toHaveBeenCalledWith(deviceId);
  });

  it("should change password", () => {
    const changePasswordRequest: ChangePasswordRequestDto = {} as any;
    dataServiceSpy.changePassword.and.returnValue(of({} as any));

    service.changePassword(changePasswordRequest).subscribe();

    expect(dataServiceSpy.changePassword).toHaveBeenCalledWith(changePasswordRequest);
  });
});
