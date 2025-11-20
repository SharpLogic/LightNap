import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection, signal } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { LoginComponent } from "./login.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { LoginSuccessTypes } from "@core/backend-api";
import { APP_NAME } from "@core/helpers";

describe("LoginComponent", () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockIdentityService: any;
  let mockBlockUiService: any;
  let mockRouteAliasService: MockRouteAliasService;

  beforeEach(async () => {
    mockIdentityService = {
      logIn: jasmine.createSpy("logIn").and.returnValue(
        of({
          type: LoginSuccessTypes.AccessToken,
          accessToken: "test-token",
        })
      ),
      requestMagicLinkEmail: jasmine.createSpy("requestMagicLinkEmail").and.returnValue(of(void 0)),
      redirectLoggedInUser: jasmine.createSpy("redirectLoggedInUser"),
      watchLoggedIn$: jasmine.createSpy("watchLoggedIn$").and.returnValue(of(false)),
    };

    mockBlockUiService = {
      show: jasmine.createSpy("show"),
      hide: jasmine.createSpy("hide"),
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideRouter([]),
        provideHttpClient(),
        { provide: APP_NAME, useValue: "TestApp" },
        { provide: IdentityService, useValue: mockIdentityService },
        { provide: BlockUiService, useValue: mockBlockUiService },
        { provide: RouteAliasService, useClass: MockRouteAliasService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    mockRouteAliasService = TestBed.inject(RouteAliasService) as unknown as MockRouteAliasService;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should initialize form with empty values except rememberMe", () => {
    expect(component.form.value).toEqual({
      login: "",
      password: "",
      rememberMe: true,
    });
  });

  it("should mark form as touched on init", () => {
    expect(component.form.touched).toBe(true);
  });

  it("should have login control with required validator", () => {
    const loginControl = component.form.controls.login;
    loginControl.setValue("");
    expect(loginControl.hasError("required")).toBe(true);

    loginControl.setValue("user@example.com");
    expect(loginControl.hasError("required")).toBe(false);
  });

  it("should have password control with required validator", () => {
    const passwordControl = component.form.controls.password;
    passwordControl.setValue("");
    expect(passwordControl.hasError("required")).toBe(true);

    passwordControl.setValue("password123");
    expect(passwordControl.hasError("required")).toBe(false);
  });

  it("should sync login value to showMagicLink control", () => {
    component.form.controls.login.setValue("test@example.com");
    expect(component.showMagicLink.value).toBe("test@example.com");
  });

  it("should validate email format for magic link", () => {
    component.showMagicLink.setValue("invalid-email");
    expect(component.showMagicLink.hasError("email")).toBe(true);

    component.showMagicLink.setValue("valid@email.com");
    expect(component.showMagicLink.hasError("email")).toBe(false);
  });

  it("should call identityService.logIn on form submit with valid data", () => {
    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
      rememberMe: false,
    });

    component.logIn();

    expect(mockIdentityService.logIn).toHaveBeenCalledWith({
      login: "user@example.com",
      password: "password123",
      rememberMe: false,
      deviceDetails: jasmine.any(String),
    });
  });

  it("should show block UI during login", () => {
    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
    });

    component.logIn();

    expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Logging in..." });
  });

  it("should hide block UI after successful login", () => {
    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
    });

    component.logIn();

    expect(mockBlockUiService.hide).toHaveBeenCalled();
  });

  it("should redirect logged in user on successful login with AccessToken", () => {
    mockIdentityService.logIn.and.returnValue(
      of({
        type: LoginSuccessTypes.AccessToken,
        accessToken: "test-token",
      })
    );

    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
    });

    component.logIn();

    expect(mockIdentityService.redirectLoggedInUser).toHaveBeenCalled();
  });

  it("should navigate to verify-code on TwoFactorRequired", () => {
    mockIdentityService.logIn.and.returnValue(
      of({
        type: LoginSuccessTypes.TwoFactorRequired,
      })
    );

    spyOn(mockRouteAliasService, "navigate");

    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
    });

    component.logIn();

    expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("verify-code", "user@example.com");
  });

  it("should navigate to email-verification-required on EmailVerificationRequired", () => {
    mockIdentityService.logIn.and.returnValue(
      of({
        type: LoginSuccessTypes.EmailVerificationRequired,
      })
    );

    spyOn(mockRouteAliasService, "navigate");

    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
    });

    component.logIn();

    expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("email-verification-required");
  });

  it("should set errors on login failure", () => {
    const errorResponse = { errorMessages: ["Invalid credentials"] };
    mockIdentityService.logIn.and.returnValue(throwError(() => errorResponse));

    component.form.patchValue({
      login: "user@example.com",
      password: "wrongpassword",
    });

    component.logIn();

    expect(component.errors()).toEqual(["Invalid credentials"]);
  });

  it("should call requestMagicLinkEmail when sendMagicLink is called", () => {
    component.form.controls.login.setValue("user@example.com");

    component.sendMagicLink();

    expect(mockIdentityService.requestMagicLinkEmail).toHaveBeenCalledWith({
      email: "user@example.com",
    });
  });

  it("should show block UI during magic link send", () => {
    component.form.controls.login.setValue("user@example.com");

    component.sendMagicLink();

    expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Sending magic link..." });
  });

  it("should navigate to magic-link-sent after successful magic link send", () => {
    spyOn(mockRouteAliasService, "navigate");

    component.form.controls.login.setValue("user@example.com");
    component.sendMagicLink();

    expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("magic-link-sent");
  });

  it("should set errors on magic link send failure", () => {
    const errorResponse = { errorMessages: ["Email not found"] };
    mockIdentityService.requestMagicLinkEmail.and.returnValue(throwError(() => errorResponse));

    component.form.controls.login.setValue("nonexistent@example.com");
    component.sendMagicLink();

    expect(component.errors()).toEqual(["Email not found"]);
  });

  it("should disable submit button when form is invalid", () => {
    component.form.patchValue({
      login: "",
      password: "",
    });
    fixture.detectChanges();

    expect(component.form.valid).toBe(false);
  });

  it("should enable submit button when form is valid", () => {
    component.form.patchValue({
      login: "user@example.com",
      password: "password123",
    });
    fixture.detectChanges();

    expect(component.form.valid).toBe(true);
  });

  it("should render branded card component", () => {
    const brandedCard = fixture.nativeElement.querySelector("ln-branded-card");
    expect(brandedCard).toBeTruthy();
  });

  it("should render login input field", () => {
    const loginInput = fixture.nativeElement.querySelector("#login");
    expect(loginInput).toBeTruthy();
  });

  it("should render password input field", () => {
    const passwordInput = fixture.nativeElement.querySelector("p-password");
    expect(passwordInput).toBeTruthy();
  });

  it("should render remember me checkbox", () => {
    const checkbox = fixture.nativeElement.querySelector("#rememberMe");
    expect(checkbox).toBeTruthy();
  });

  it("should render forgot password link", () => {
    const links = fixture.nativeElement.querySelectorAll("a");
    const forgotPasswordLink = Array.from(links).find((link: any) =>
      link.textContent.includes("Forgot password")
    );
    expect(forgotPasswordLink).toBeTruthy();
  });

  it("should render create account link", () => {
    const links = fixture.nativeElement.querySelectorAll("a");
    const createAccountLink = Array.from(links).find((link: any) =>
      link.textContent.includes("I need to create an account")
    );
    expect(createAccountLink).toBeTruthy();
  });

  it("should render error list component", () => {
    const errorList = fixture.nativeElement.querySelector("ln-error-list");
    expect(errorList).toBeTruthy();
  });

  it("should disable magic link button when email is invalid", () => {
    component.form.controls.login.setValue("invalid-email");
    fixture.detectChanges();

    expect(component.showMagicLink.valid).toBe(false);
  });

  it("should enable magic link button when email is valid", () => {
    component.form.controls.login.setValue("valid@email.com");
    fixture.detectChanges();

    expect(component.showMagicLink.valid).toBe(true);
  });
});
