import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { VerifyCodeComponent } from "./verify-code.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { APP_NAME } from "@core/helpers";

describe("VerifyCodeComponent", () => {
  let component: VerifyCodeComponent;
  let fixture: ComponentFixture<VerifyCodeComponent>;
  let mockIdentityService: any;
  let mockBlockUiService: any;
  let mockRouteAliasService: MockRouteAliasService;

  beforeEach(async () => {
    mockIdentityService = {
      verifyCode: jasmine.createSpy("verifyCode").and.returnValue(of(void 0)),
      watchLoggedIn$: jasmine.createSpy("watchLoggedIn$").and.returnValue(of(false)),
    };

    mockBlockUiService = {
      show: jasmine.createSpy("show"),
      hide: jasmine.createSpy("hide"),
    };

    await TestBed.configureTestingModule({
      imports: [VerifyCodeComponent],
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

    fixture = TestBed.createComponent(VerifyCodeComponent);
    component = fixture.componentInstance;

    // Set required input
    fixture.componentRef.setInput("login", "test@example.com");

    mockRouteAliasService = TestBed.inject(RouteAliasService) as unknown as MockRouteAliasService;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  describe("Form Initialization", () => {
    it("should initialize with empty code", () => {
      expect(component.form.value.code).toBe("");
    });

    it("should initialize rememberMe as false", () => {
      expect(component.form.value.rememberMe).toBe(false);
    });

    it("should have invalid form when code is empty", () => {
      expect(component.form.valid).toBe(false);
    });

    it("should require code field", () => {
      const codeControl = component.form.controls.code;
      expect(codeControl?.hasError("required")).toBe(true);
    });

    it("should have valid form when code is provided", () => {
      component.form.patchValue({
        code: "123456",
      });

      expect(component.form.valid).toBe(true);
    });
  });

  describe("Verify Code Flow", () => {
    it("should call identityService.verifyCode on verify button click", () => {
      component.form.patchValue({
        code: "123456",
        rememberMe: true,
      });

      component.verify();

      expect(mockIdentityService.verifyCode).toHaveBeenCalledWith(
        jasmine.objectContaining({
          code: "123456",
          login: "test@example.com",
          rememberMe: true,
          deviceDetails: jasmine.any(String),
        })
      );
    });

    it("should show block UI during verification", () => {
      component.form.patchValue({
        code: "123456",
      });

      component.verify();

      expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Verifying code..." });
    });

    it("should hide block UI after successful verification", () => {
      component.form.patchValue({
        code: "123456",
      });

      component.verify();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should navigate to user-home on success", () => {
      spyOn(mockRouteAliasService, "navigate");
      component.form.patchValue({
        code: "123456",
      });

      component.verify();

      expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("user-home");
    });

    it("should set errors on verification failure", () => {
      const errorResponse = { errorMessages: ["Invalid verification code"] };
      mockIdentityService.verifyCode.and.returnValue(throwError(() => errorResponse));

      component.form.patchValue({
        code: "000000",
      });

      component.verify();

      expect(component.errors()).toEqual(["Invalid verification code"]);
    });

    it("should include rememberMe value in verification request", () => {
      component.form.patchValue({
        code: "123456",
        rememberMe: false,
      });

      component.verify();

      expect(mockIdentityService.verifyCode).toHaveBeenCalledWith(
        jasmine.objectContaining({
          rememberMe: false,
        })
      );
    });
  });

  describe("DOM Rendering", () => {
    it("should render branded card component", () => {
      const brandedCard = fixture.nativeElement.querySelector("ln-branded-card");
      expect(brandedCard).toBeTruthy();
    });

    it("should render OTP input field", () => {
      const otpInput = fixture.nativeElement.querySelector("p-inputotp");
      expect(otpInput).toBeTruthy();
    });

    it("should render remember me checkbox", () => {
      const rememberMeCheckbox = fixture.nativeElement.querySelector("#rememberMe");
      expect(rememberMeCheckbox).toBeTruthy();
    });

    it("should render verify button", () => {
      const verifyButton = fixture.nativeElement.querySelector('p-button');
      expect(verifyButton).toBeTruthy();
    });

    it("should render error list component", () => {
      const errorList = fixture.nativeElement.querySelector("ln-error-list");
      expect(errorList).toBeTruthy();
    });
  });

  describe("Form Validation States", () => {
    it("should have invalid form when code is empty", () => {
      component.form.patchValue({
        code: "",
      });

      expect(component.form.valid).toBe(false);
    });

    it("should have valid form when code is provided", () => {
      component.form.patchValue({
        code: "123456",
      });

      expect(component.form.valid).toBe(true);
    });
  });

  describe("Input Properties", () => {
    it("should accept login input", () => {
      expect(component.login()).toBe("test@example.com");
    });

    it("should use login value in verification request", () => {
      component.form.patchValue({
        code: "123456",
      });

      component.verify();

      expect(mockIdentityService.verifyCode).toHaveBeenCalledWith(
        jasmine.objectContaining({
          login: "test@example.com",
        })
      );
    });
  });
});
