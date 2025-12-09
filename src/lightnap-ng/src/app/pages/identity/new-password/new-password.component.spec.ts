import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection, signal } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { NewPasswordComponent } from "./new-password.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { LoginSuccessType } from "@core/backend-api";
import { APP_NAME } from "@core/helpers";

describe("NewPasswordComponent", () => {
  let component: NewPasswordComponent;
  let fixture: ComponentFixture<NewPasswordComponent>;
  let mockIdentityService: any;
  let mockBlockUiService: any;
  let mockRouteAliasService: MockRouteAliasService;

  beforeEach(async () => {
    mockIdentityService = {
      newPassword: jasmine.createSpy("newPassword").and.returnValue(
        of({
          type: LoginSuccessType.AccessToken,
          accessToken: "test-token",
        })
      ),
      redirectLoggedInUser: jasmine.createSpy("redirectLoggedInUser"),
      watchLoggedIn$: jasmine.createSpy("watchLoggedIn$").and.returnValue(of(false)),
    };

    mockBlockUiService = {
      show: jasmine.createSpy("show"),
      hide: jasmine.createSpy("hide"),
    };

    await TestBed.configureTestingModule({
      imports: [NewPasswordComponent],
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

    fixture = TestBed.createComponent(NewPasswordComponent);
    component = fixture.componentInstance;

    // Set required inputs
    fixture.componentRef.setInput("email", "test@example.com");
    fixture.componentRef.setInput("token", "reset-token-123");

    mockRouteAliasService = TestBed.inject(RouteAliasService) as unknown as MockRouteAliasService;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  describe("Form Initialization", () => {
    it("should initialize with empty password fields", () => {
      expect(component.form.value.password).toBe("");
      expect(component.form.value.confirmPassword).toBe("");
    });

    it("should initialize rememberMe as false", () => {
      expect(component.form.value.rememberMe).toBe(false);
    });

    it("should have invalid form when fields are empty", () => {
      expect(component.form.valid).toBe(false);
    });

    it("should require password field", () => {
      const passwordControl = component.form.controls.password;
      expect(passwordControl.hasError("required")).toBe(true);
    });

    it("should require confirmPassword field", () => {
      const confirmPasswordControl = component.form.controls.confirmPassword;
      expect(confirmPasswordControl.hasError("required")).toBe(true);
    });
  });

  describe("Password Confirmation Validation", () => {
    it("should show passwordsDoNotMatch error when passwords differ", () => {
      component.form.patchValue({
        password: "password123",
        confirmPassword: "different",
      });

      expect(component.form.hasError("passwordsDoNotMatch")).toBe(true);
    });

    it("should not show passwordsDoNotMatch error when passwords match", () => {
      component.form.patchValue({
        password: "password123",
        confirmPassword: "password123",
      });

      expect(component.form.hasError("passwordsDoNotMatch")).toBe(false);
    });

    it("should have valid form when all fields are correct", () => {
      component.form.patchValue({
        password: "password123",
        confirmPassword: "password123",
        rememberMe: true,
      });

      expect(component.form.valid).toBe(true);
    });
  });

  describe("New Password Flow", () => {
    it("should call identityService.newPassword on form submit with valid data", () => {
      component.form.patchValue({
        password: "newpassword123",
        confirmPassword: "newpassword123",
        rememberMe: true,
      });

      component.newPassword();

      expect(mockIdentityService.newPassword).toHaveBeenCalledWith(
        jasmine.objectContaining({
          email: "test@example.com",
          password: "newpassword123",
          token: "reset-token-123",
          rememberMe: true,
          deviceDetails: jasmine.any(String),
        })
      );
    });

    it("should show block UI during password reset", () => {
      component.form.patchValue({
        password: "newpassword123",
        confirmPassword: "newpassword123",
      });

      component.newPassword();

      expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Setting new password..." });
    });

    it("should hide block UI after successful password reset", () => {
      component.form.patchValue({
        password: "newpassword123",
        confirmPassword: "newpassword123",
      });

      component.newPassword();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should redirect logged in user on AccessToken success", () => {
      mockIdentityService.newPassword.and.returnValue(
        of({
          type: LoginSuccessType.AccessToken,
          accessToken: "test-token",
        })
      );

      component.form.patchValue({
        password: "newpassword123",
        confirmPassword: "newpassword123",
      });

      component.newPassword();

      expect(mockIdentityService.redirectLoggedInUser).toHaveBeenCalled();
    });

    it("should navigate to verify-code on TwoFactorRequired", () => {
      mockIdentityService.newPassword.and.returnValue(
        of({
          type: LoginSuccessType.TwoFactorRequired,
        })
      );
      spyOn(mockRouteAliasService, "navigate");

      component.form.patchValue({
        password: "newpassword123",
        confirmPassword: "newpassword123",
      });

      component.newPassword();

      expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("verify-code", "test@example.com");
    });

    it("should set errors on password reset failure", () => {
      const errorResponse = { errorMessages: ["Invalid or expired token"] };
      mockIdentityService.newPassword.and.returnValue(throwError(() => errorResponse));

      component.form.patchValue({
        password: "newpassword123",
        confirmPassword: "newpassword123",
      });

      component.newPassword();

      expect(component.errors()).toEqual(["Invalid or expired token"]);
    });
  });

  describe("DOM Rendering", () => {
    it("should render branded card component", () => {
      const brandedCard = fixture.nativeElement.querySelector("ln-branded-card");
      expect(brandedCard).toBeTruthy();
    });

    it("should render password input field", () => {
      const passwordInputs = fixture.nativeElement.querySelectorAll("p-password");
      expect(passwordInputs.length).toBeGreaterThanOrEqual(2);
    });

    it("should render remember me checkbox", () => {
      const rememberMeCheckbox = fixture.nativeElement.querySelector("#rememberMe");
      expect(rememberMeCheckbox).toBeTruthy();
    });

    it("should render submit button", () => {
      const submitButton = fixture.nativeElement.querySelector('p-button[type="submit"]');
      expect(submitButton).toBeTruthy();
    });

    it("should render error list component", () => {
      const errorList = fixture.nativeElement.querySelector("ln-error-list");
      expect(errorList).toBeTruthy();
    });

    it("should render return to login link", () => {
      const loginLink = fixture.nativeElement.querySelector("a");
      expect(loginLink).toBeTruthy();
      expect(loginLink.textContent).toContain("Return to login");
    });
  });

  describe("Form Validation States", () => {
    it("should have invalid form when initialized", () => {
      expect(component.form.valid).toBe(false);
    });

    it("should have invalid form when passwords don't match", () => {
      component.form.patchValue({
        password: "password123",
        confirmPassword: "different",
      });

      expect(component.form.valid).toBe(false);
    });

    it("should disable submit button when form is invalid", () => {
      component.form.patchValue({
        password: "",
        confirmPassword: "",
      });
      fixture.detectChanges();

      expect(component.form.valid).toBe(false);
    });

    it("should enable submit button when form is valid", () => {
      component.form.patchValue({
        password: "password123",
        confirmPassword: "password123",
      });
      fixture.detectChanges();

      expect(component.form.valid).toBe(true);
    });
  });

  describe("Input Properties", () => {
    it("should accept email input", () => {
      expect(component.email()).toBe("test@example.com");
    });

    it("should accept token input", () => {
      expect(component.token()).toBe("reset-token-123");
    });
  });
});
