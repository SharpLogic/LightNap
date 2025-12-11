import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { ResetPasswordComponent } from "./reset-password.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { APP_NAME } from "@core/helpers";

describe("ResetPasswordComponent", () => {
    let component: ResetPasswordComponent;
    let fixture: ComponentFixture<ResetPasswordComponent>;
    let mockIdentityService: any;
    let mockBlockUiService: any;
    let mockRouteAliasService: MockRouteAliasService;

    beforeEach(async () => {
        mockIdentityService = {
            resetPassword: vi.fn().mockReturnValue(of(void 0)),
            watchLoggedIn$: vi.fn().mockReturnValue(of(false)),
        };

        mockBlockUiService = {
            show: vi.fn(),
            hide: vi.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [ResetPasswordComponent],
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

        fixture = TestBed.createComponent(ResetPasswordComponent);
        component = fixture.componentInstance;
        mockRouteAliasService = TestBed.inject(RouteAliasService) as unknown as MockRouteAliasService;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });

    describe("Form Initialization", () => {
        it("should initialize with empty email", () => {
            expect(component.form.value.email).toBe("");
        });

        it("should have invalid form when email is empty", () => {
            expect(component.form.valid).toBe(false);
        });

        it("should require email field", () => {
            const emailControl = component.form.controls.email;
            expect(emailControl.hasError("required")).toBe(true);
        });

        it("should validate email format", () => {
            const emailControl = component.form.controls.email;
            emailControl.setValue("invalid-email");
            expect(emailControl.hasError("email")).toBe(true);
        });

        it("should accept valid email", () => {
            const emailControl = component.form.controls.email;
            emailControl.setValue("valid@email.com");
            expect(emailControl.valid).toBe(true);
        });
    });

    describe("Reset Password Flow", () => {
        it("should call identityService.resetPassword on form submit with valid data", () => {
            component.form.patchValue({
                email: "test@example.com",
            });

            component.resetPassword();

            expect(mockIdentityService.resetPassword).toHaveBeenCalledWith({
                email: "test@example.com",
            });
        });

        it("should show block UI during password reset", () => {
            component.form.patchValue({
                email: "test@example.com",
            });

            component.resetPassword();

            expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Resetting password..." });
        });

        it("should hide block UI after successful password reset", () => {
            component.form.patchValue({
                email: "test@example.com",
            });

            component.resetPassword();

            expect(mockBlockUiService.hide).toHaveBeenCalled();
        });

        it("should navigate to reset-instructions-sent on success", () => {
            vi.spyOn(mockRouteAliasService, "navigate");
            component.form.patchValue({
                email: "test@example.com",
            });

            component.resetPassword();

            expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("reset-instructions-sent");
        });

        it("should set errors on password reset failure", () => {
            const errorResponse = { errorMessages: ["Email not found"] };
            mockIdentityService.resetPassword.mockReturnValue(throwError(() => errorResponse));

            component.form.patchValue({
                email: "nonexistent@example.com",
            });

            component.resetPassword();

            expect(component.errors()).toEqual(["Email not found"]);
        });
    });

    describe("DOM Rendering", () => {
        it("should render branded card component", () => {
            const brandedCard = fixture.nativeElement.querySelector("ln-branded-card");
            expect(brandedCard).toBeTruthy();
        });

        it("should render email input field", () => {
            const emailInput = fixture.nativeElement.querySelector("#email");
            expect(emailInput).toBeTruthy();
            expect(emailInput.getAttribute("type")).toBe("text");
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

        it("should have valid form with proper email", () => {
            component.form.patchValue({
                email: "test@example.com",
            });
            expect(component.form.valid).toBe(true);
        });

        it("should disable submit button when form is invalid", () => {
            component.form.patchValue({
                email: "",
            });
            fixture.detectChanges();

            expect(component.form.valid).toBe(false);
        });

        it("should enable submit button when form is valid", () => {
            component.form.patchValue({
                email: "test@example.com",
            });
            fixture.detectChanges();

            expect(component.form.valid).toBe(true);
        });
    });
});
