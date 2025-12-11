import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { RegisterComponent } from "./register.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { LoginSuccessType } from "@core/backend-api";
import { APP_NAME } from "@core/helpers";

describe("RegisterComponent", () => {
    let component: RegisterComponent;
    let fixture: ComponentFixture<RegisterComponent>;
    let mockIdentityService: any;
    let mockBlockUiService: any;
    let mockRouteAliasService: MockRouteAliasService;

    beforeEach(async () => {
        mockIdentityService = {
            register: vi.fn().mockReturnValue(of({
                type: LoginSuccessType.AccessToken,
                accessToken: "test-token",
            })),
            redirectLoggedInUser: vi.fn(),
            watchLoggedIn$: vi.fn().mockReturnValue(of(false)),
        };

        mockBlockUiService = {
            show: vi.fn(),
            hide: vi.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [RegisterComponent],
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

        fixture = TestBed.createComponent(RegisterComponent);
        component = fixture.componentInstance;
        mockRouteAliasService = TestBed.inject(RouteAliasService) as unknown as MockRouteAliasService;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });

    it("should initialize form with correct default values", () => {
        expect(component.form.value).toEqual({
            email: "",
            password: "",
            confirmPassword: "",
            userName: "",
            agreedToTerms: false,
            rememberMe: true,
        });
    });

    it("should have email control with required and email validators", () => {
        const emailControl = component.form.controls.email;

        emailControl.setValue("");
        expect(emailControl.hasError("required")).toBe(true);

        emailControl.setValue("invalid-email");
        expect(emailControl.hasError("email")).toBe(true);

        emailControl.setValue("valid@email.com");
        expect(emailControl.valid).toBe(true);
    });

    it("should have password control with required validator", () => {
        const passwordControl = component.form.controls.password;

        passwordControl.setValue("");
        expect(passwordControl.hasError("required")).toBe(true);

        passwordControl.setValue("password123");
        expect(passwordControl.valid).toBe(true);
    });

    it("should have confirmPassword control with required validator", () => {
        const confirmPasswordControl = component.form.controls.confirmPassword;

        confirmPasswordControl.setValue("");
        expect(confirmPasswordControl.hasError("required")).toBe(true);

        confirmPasswordControl.setValue("password123");
        expect(confirmPasswordControl.valid).toBe(true);
    });

    it("should have userName control with required validator", () => {
        const userNameControl = component.form.controls.userName;

        userNameControl.setValue("");
        expect(userNameControl.hasError("required")).toBe(true);

        userNameControl.setValue("testuser");
        expect(userNameControl.valid).toBe(true);
    });

    it("should have agreedToTerms control with requiredTrue validator", () => {
        const agreedToTermsControl = component.form.controls.agreedToTerms;

        agreedToTermsControl.setValue(false);
        expect(agreedToTermsControl.hasError("required")).toBe(true);

        agreedToTermsControl.setValue(true);
        expect(agreedToTermsControl.valid).toBe(true);
    });

    it("should validate that passwords match", () => {
        component.form.patchValue({
            password: "password123",
            confirmPassword: "password456",
        });

        expect(component.form.hasError("passwordsDoNotMatch")).toBe(true);

        component.form.patchValue({
            password: "password123",
            confirmPassword: "password123",
        });

        expect(component.form.hasError("passwordsDoNotMatch")).toBe(false);
    });

    it("should have invalid form when required fields are empty", () => {
        expect(component.form.valid).toBe(false);
    });

    it("should have valid form when all fields are correctly filled", () => {
        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
            rememberMe: true,
        });

        expect(component.form.valid).toBe(true);
    });

    it("should call identityService.register on form submit with valid data", () => {
        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
            rememberMe: false,
        });

        component.register();

        expect(mockIdentityService.register).toHaveBeenCalledWith({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            rememberMe: false,
            deviceDetails: expect.any(String),
        });
    });

    it("should show block UI during registration", () => {
        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });

        component.register();

        expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Registering..." });
    });

    it("should hide block UI after registration", () => {
        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });

        component.register();

        expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should redirect logged in user on successful registration with AccessToken", () => {
        mockIdentityService.register.mockReturnValue(of({
            type: LoginSuccessType.AccessToken,
            accessToken: "test-token",
        }));

        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });

        component.register();

        expect(mockIdentityService.redirectLoggedInUser).toHaveBeenCalled();
    });

    it("should navigate to verify-code on TwoFactorRequired", () => {
        mockIdentityService.register.mockReturnValue(of({
            type: LoginSuccessType.TwoFactorRequired,
        }));

        vi.spyOn(mockRouteAliasService, "navigate");

        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });

        component.register();

        expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("verify-code", "test@example.com");
    });

    it("should navigate to verify-email on EmailVerificationRequired", () => {
        mockIdentityService.register.mockReturnValue(of({
            type: LoginSuccessType.EmailVerificationRequired,
        }));

        vi.spyOn(mockRouteAliasService, "navigate");

        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });

        component.register();

        expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("email-verification-required");
    });

    it("should set errors on registration failure", () => {
        const errorResponse = { errorMessages: ["Email already exists"] };
        mockIdentityService.register.mockReturnValue(throwError(() => errorResponse));

        component.form.patchValue({
            email: "existing@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });

        component.register();

        expect(component.errors()).toEqual(["Email already exists"]);
    });

    it("should render branded card component", () => {
        const brandedCard = fixture.nativeElement.querySelector("ln-branded-card");
        expect(brandedCard).toBeTruthy();
    });

    it("should render userName input field", () => {
        const userNameInput = fixture.nativeElement.querySelector("#userName");
        expect(userNameInput).toBeTruthy();
    });

    it("should render email input field", () => {
        const emailInput = fixture.nativeElement.querySelector("#email");
        expect(emailInput).toBeTruthy();
    });

    it("should render password input field", () => {
        const passwordInputs = fixture.nativeElement.querySelectorAll("p-password");
        expect(passwordInputs.length).toBe(2);
    });

    it("should render confirm password input field", () => {
        const confirmPasswordInput = fixture.nativeElement.querySelector('p-password[formControlName="confirmPassword"]');
        expect(confirmPasswordInput).toBeTruthy();
    });

    it("should render remember me checkbox", () => {
        const checkbox = fixture.nativeElement.querySelector("#rememberMe");
        expect(checkbox).toBeTruthy();
    });

    it("should render agreed to terms checkbox", () => {
        const checkbox = fixture.nativeElement.querySelector("#agreedToTerms");
        expect(checkbox).toBeTruthy();
    });

    it("should render terms and conditions link", () => {
        const links = fixture.nativeElement.querySelectorAll("a");
        const termsLink = Array.from(links).find((link: any) => link.textContent.includes("Terms and Conditions"));
        expect(termsLink).toBeTruthy();
    });

    it("should render privacy policy link", () => {
        const links = fixture.nativeElement.querySelectorAll("a");
        const privacyLink = Array.from(links).find((link: any) => link.textContent.includes("Privacy Policy"));
        expect(privacyLink).toBeTruthy();
    });

    it("should render login link", () => {
        const links = fixture.nativeElement.querySelectorAll("a");
        const loginLink = Array.from(links).find((link: any) => link.textContent.includes("I already have an account"));
        expect(loginLink).toBeTruthy();
    });

    it("should render error list component", () => {
        const errorList = fixture.nativeElement.querySelector("ln-error-list");
        expect(errorList).toBeTruthy();
    });

    it("should disable submit button when form is invalid", () => {
        component.form.patchValue({
            email: "",
            password: "",
        });
        fixture.detectChanges();

        expect(component.form.valid).toBe(false);
    });

    it("should enable submit button when form is valid", () => {
        component.form.patchValue({
            email: "test@example.com",
            password: "password123",
            confirmPassword: "password123",
            userName: "testuser",
            agreedToTerms: true,
        });
        fixture.detectChanges();

        expect(component.form.valid).toBe(true);
    });
});
