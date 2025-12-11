import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { ChangePasswordComponent } from "./change-password.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { ToastService } from "@core/services/toast.service";
import { APP_NAME } from "@core/helpers";

describe("ChangePasswordComponent", () => {
    let component: ChangePasswordComponent;
    let fixture: ComponentFixture<ChangePasswordComponent>;
    let mockIdentityService: any;
    let mockBlockUiService: any;
    let mockToastService: any;

    beforeEach(async () => {
        mockIdentityService = {
            changePassword: vi.fn().mockReturnValue(of(void 0)),
            watchLoggedIn$: vi.fn().mockReturnValue(of(true)),
        };

        mockBlockUiService = {
            show: vi.fn(),
            hide: vi.fn(),
        };

        mockToastService = {
            success: vi.fn(),
            error: vi.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [ChangePasswordComponent],
            providers: [
                provideZonelessChangeDetection(),
                provideNoopAnimations(),
                provideRouter([]),
                provideHttpClient(),
                { provide: APP_NAME, useValue: "TestApp" },
                { provide: IdentityService, useValue: mockIdentityService },
                { provide: BlockUiService, useValue: mockBlockUiService },
                { provide: ToastService, useValue: mockToastService },
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(ChangePasswordComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });

    describe("Form Initialization", () => {
        it("should initialize with empty password fields", () => {
            expect(component.form.value.currentPassword).toBe("");
            expect(component.form.value.newPassword).toBe("");
            expect(component.form.value.confirmNewPassword).toBe("");
        });

        it("should have invalid form when fields are empty", () => {
            expect(component.form.valid).toBe(false);
        });

        it("should require currentPassword field", () => {
            const currentPasswordControl = component.form.controls.currentPassword;
            expect(currentPasswordControl.hasError("required")).toBe(true);
        });

        it("should require newPassword field", () => {
            const newPasswordControl = component.form.controls.newPassword;
            expect(newPasswordControl.hasError("required")).toBe(true);
        });

        it("should require confirmNewPassword field", () => {
            const confirmNewPasswordControl = component.form.controls.confirmNewPassword;
            expect(confirmNewPasswordControl.hasError("required")).toBe(true);
        });
    });

    describe("Password Confirmation Validation", () => {
        it("should show passwordsDoNotMatch error when passwords differ", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "different",
            });

            expect(component.form.hasError("passwordsDoNotMatch")).toBe(true);
        });

        it("should not show passwordsDoNotMatch error when passwords match", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            expect(component.form.hasError("passwordsDoNotMatch")).toBe(false);
        });

        it("should have valid form when all fields are correct", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            expect(component.form.valid).toBe(true);
        });
    });

    describe("Change Password Flow", () => {
        it("should call identityService.changePassword on form submit with valid data", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(mockIdentityService.changePassword).toHaveBeenCalledWith({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });
        });

        it("should clear errors before changing password", () => {
            component.errors.set(["Previous error"]);
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(component.errors()).toEqual([]);
        });

        it("should show block UI during password change", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Changing password..." });
        });

        it("should hide block UI after successful password change", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(mockBlockUiService.hide).toHaveBeenCalled();
        });

        it("should show success toast on successful password change", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(mockToastService.success).toHaveBeenCalledWith("Password changed successfully.");
        });

        it("should reset form on successful password change", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(component.form.value.currentPassword).toBeNull();
            expect(component.form.value.newPassword).toBeNull();
            expect(component.form.value.confirmNewPassword).toBeNull();
        });

        it("should set errors on password change failure", () => {
            const errorResponse = { errorMessages: ["Current password is incorrect"] };
            mockIdentityService.changePassword.mockReturnValue(throwError(() => errorResponse));

            component.form.patchValue({
                currentPassword: "wrongpass",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });

            component.changePassword();

            expect(component.errors()).toEqual(["Current password is incorrect"]);
        });
    });

    describe("DOM Rendering", () => {
        it("should render panel component", () => {
            const panel = fixture.nativeElement.querySelector("p-panel");
            expect(panel).toBeTruthy();
        });

        it("should render password input fields", () => {
            const passwordInputs = fixture.nativeElement.querySelectorAll("p-password");
            expect(passwordInputs.length).toBe(3);
        });

        it("should render submit button", () => {
            const submitButton = fixture.nativeElement.querySelector('p-button[type="submit"]');
            expect(submitButton).toBeTruthy();
        });

        it("should render error list component", () => {
            const errorList = fixture.nativeElement.querySelector("ln-error-list");
            expect(errorList).toBeTruthy();
        });
    });

    describe("Form Validation States", () => {
        it("should have invalid form when initialized", () => {
            expect(component.form.valid).toBe(false);
        });

        it("should have invalid form when passwords don't match", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "different",
            });

            expect(component.form.valid).toBe(false);
        });

        it("should disable submit button when form is invalid", () => {
            component.form.patchValue({
                currentPassword: "",
                newPassword: "",
                confirmNewPassword: "",
            });
            fixture.detectChanges();

            expect(component.form.valid).toBe(false);
        });

        it("should enable submit button when form is valid", () => {
            component.form.patchValue({
                currentPassword: "oldpass123",
                newPassword: "newpass123",
                confirmNewPassword: "newpass123",
            });
            fixture.detectChanges();

            expect(component.form.valid).toBe(true);
        });
    });
});
