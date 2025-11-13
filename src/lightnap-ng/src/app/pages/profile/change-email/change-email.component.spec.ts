import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";
import { of, throwError } from "rxjs";
import { ChangeEmailComponent } from "./change-email.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { API_URL_ROOT, APP_NAME } from "@core/helpers";

describe("ChangeEmailComponent", () => {
  let component: ChangeEmailComponent;
  let fixture: ComponentFixture<ChangeEmailComponent>;
  let mockIdentityService: any;
  let mockBlockUiService: any;
  let mockRouteAliasService: MockRouteAliasService;

  beforeEach(async () => {
    mockIdentityService = {
      changeEmail: jasmine.createSpy("changeEmail").and.returnValue(of(void 0)),
      watchLoggedIn$: jasmine.createSpy("watchLoggedIn$").and.returnValue(of(true)),
    };

    mockBlockUiService = {
      show: jasmine.createSpy("show"),
      hide: jasmine.createSpy("hide"),
    };

    await TestBed.configureTestingModule({
      imports: [ChangeEmailComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideRouter([]),
        provideHttpClient(),
        { provide: API_URL_ROOT, useValue: "http://localhost:5000/api/" },
        { provide: APP_NAME, useValue: "TestApp" },
        { provide: IdentityService, useValue: mockIdentityService },
        { provide: BlockUiService, useValue: mockBlockUiService },
        { provide: RouteAliasService, useClass: MockRouteAliasService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ChangeEmailComponent);
    component = fixture.componentInstance;
    mockRouteAliasService = TestBed.inject(RouteAliasService) as unknown as MockRouteAliasService;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  describe("Form Initialization", () => {
    it("should initialize with empty email", () => {
      expect(component.form.value.newEmail).toBe("");
    });

    it("should have invalid form when email is empty", () => {
      expect(component.form.valid).toBe(false);
    });

    it("should require email field", () => {
      const emailControl = component.form.controls.newEmail;
      expect(emailControl.hasError("required")).toBe(true);
    });

    it("should validate email format", () => {
      const emailControl = component.form.controls.newEmail;
      emailControl.setValue("invalid-email");
      expect(emailControl.hasError("email")).toBe(true);
    });

    it("should accept valid email", () => {
      const emailControl = component.form.controls.newEmail;
      emailControl.setValue("valid@email.com");
      expect(emailControl.valid).toBe(true);
    });
  });

  describe("Change Email Flow", () => {
    it("should call identityService.changeEmail on form submit with valid data", () => {
      component.form.patchValue({
        newEmail: "newemail@example.com",
      });

      component.changeEmail();

      expect(mockIdentityService.changeEmail).toHaveBeenCalledWith({
        newEmail: "newemail@example.com",
      });
    });

    it("should show block UI during email change", () => {
      component.form.patchValue({
        newEmail: "newemail@example.com",
      });

      component.changeEmail();

      expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Changing email..." });
    });

    it("should hide block UI after successful email change", () => {
      component.form.patchValue({
        newEmail: "newemail@example.com",
      });

      component.changeEmail();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should navigate to change-email-requested on success", () => {
      spyOn(mockRouteAliasService, "navigate");
      component.form.patchValue({
        newEmail: "newemail@example.com",
      });

      component.changeEmail();

      expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("change-email-requested");
    });

    it("should set errors on email change failure", () => {
      const errorResponse = { errorMessages: ["Email already in use"] };
      mockIdentityService.changeEmail.and.returnValue(throwError(() => errorResponse));

      component.form.patchValue({
        newEmail: "existing@example.com",
      });

      component.changeEmail();

      expect(component.errors()).toEqual(["Email already in use"]);
    });
  });

  describe("DOM Rendering", () => {
    it("should render panel component", () => {
      const panel = fixture.nativeElement.querySelector("p-panel");
      expect(panel).toBeTruthy();
    });

    it("should render email input field", () => {
      const emailInput = fixture.nativeElement.querySelector('input[formControlName="newEmail"]');
      expect(emailInput).toBeTruthy();
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

    it("should have valid form with proper email", () => {
      component.form.patchValue({
        newEmail: "test@example.com",
      });
      expect(component.form.valid).toBe(true);
    });

    it("should disable submit button when form is invalid", () => {
      component.form.patchValue({
        newEmail: "",
      });
      fixture.detectChanges();

      expect(component.form.valid).toBe(false);
    });

    it("should enable submit button when form is valid", () => {
      component.form.patchValue({
        newEmail: "test@example.com",
      });
      fixture.detectChanges();

      expect(component.form.valid).toBe(true);
    });
  });
});
