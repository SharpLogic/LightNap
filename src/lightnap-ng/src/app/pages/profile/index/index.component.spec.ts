import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { of, throwError } from "rxjs";
import { IndexComponent } from "./index.component";
import { IdentityService } from "@core/services/identity.service";
import { API_URL_ROOT, APP_NAME, ProfileDto } from "@core";
import { ProfileService } from "@core/services/profile.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { ToastService } from "@core/services/toast.service";

describe("IndexComponent", () => {
  let component: IndexComponent;
  let fixture: ComponentFixture<IndexComponent>;
  let mockIdentityService: jasmine.SpyObj<IdentityService>;
  let mockProfileService: jasmine.SpyObj<ProfileService>;
  let mockRouteAliasService: jasmine.SpyObj<RouteAliasService>;
  let mockBlockUiService: jasmine.SpyObj<BlockUiService>;
  let mockToastService: jasmine.SpyObj<ToastService>;

  const mockProfile: ProfileDto = {
    id: "1",
    userName: "testuser",
    email: "test@example.com",
  };

  beforeEach(async () => {
    mockIdentityService = jasmine.createSpyObj("IdentityService", ["logOut", "watchLoggedIn$"]);
    mockProfileService = jasmine.createSpyObj("ProfileService", ["getProfile", "updateProfile"]);
    mockRouteAliasService = jasmine.createSpyObj("RouteAliasService", ["navigate", "getRoute"]);
    mockBlockUiService = jasmine.createSpyObj("BlockUiService", ["show", "hide"]);
    mockToastService = jasmine.createSpyObj("ToastService", ["success"]);

    mockIdentityService.watchLoggedIn$.and.returnValue(of(true));
    mockProfileService.getProfile.and.returnValue(of(mockProfile));
    mockProfileService.updateProfile.and.returnValue(of(mockProfile));
    mockIdentityService.logOut.and.returnValue(of(true));
    mockRouteAliasService.getRoute.and.returnValue(["/profile", "change-email"]);
    mockRouteAliasService.navigate.and.returnValue(Promise.resolve(true));

    await TestBed.configureTestingModule({
      imports: [IndexComponent, ReactiveFormsModule],
      providers: [
        provideHttpClient(),
        provideNoopAnimations(),
        provideRouter([]),
        provideZonelessChangeDetection(),
        { provide: API_URL_ROOT, useValue: "http://localhost:5000/api/" },
        { provide: APP_NAME, useValue: "TestApp" },
        { provide: IdentityService, useValue: mockIdentityService },
        { provide: ProfileService, useValue: mockProfileService },
        { provide: RouteAliasService, useValue: mockRouteAliasService },
        { provide: BlockUiService, useValue: mockBlockUiService },
        { provide: ToastService, useValue: mockToastService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(IndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // Component Initialization Tests
  describe("Component Initialization", () => {
    it("should create", () => {
      expect(component).toBeTruthy();
    });

    it("should initialize errors signal as empty array", () => {
      expect(component.errors()).toEqual([]);
    });

    it("should initialize form", () => {
      expect(component.form).toBeDefined();
      expect(component.form.getRawValue()).toEqual({});
    });

    it("should load profile on initialization", () => {
      expect(mockProfileService.getProfile).toHaveBeenCalled();
    });

    it("should have type helper function", () => {
      expect(component.asProfile).toBeDefined();
    });
  });

  // Profile Update Tests
  describe("Profile Update", () => {
    it("should update profile successfully", () => {
      const formData = { preferredLanguage: "es" };

      component.updateProfile();

      expect(mockProfileService.updateProfile).toHaveBeenCalledWith(component.form.value);
      expect(mockToastService.success).toHaveBeenCalledWith("Profile updated successfully.");
      expect(component.form.pristine).toBe(true);
    });

    it("should show block UI during profile update", () => {
      component.updateProfile();

      expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Updating profile..." });
      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should set errors on profile update failure", () => {
      const errorResponse = { errorMessages: ["Failed to update profile"] };

      mockProfileService.updateProfile.and.returnValue(throwError(() => errorResponse));

      component.updateProfile();

      expect(component.errors()).toEqual(["Failed to update profile"]);
    });

    it("should hide block UI even on profile update error", () => {
      const errorResponse = { errorMessages: ["Failed to update profile"] };

      mockProfileService.updateProfile.and.returnValue(throwError(() => errorResponse));

      component.updateProfile();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });
  });

  // Logout Tests
  describe("Logout", () => {
    it("should logout successfully", () => {
      component.logOut();

      expect(mockIdentityService.logOut).toHaveBeenCalled();
      expect(mockRouteAliasService.navigate).toHaveBeenCalledWith("landing");
    });

    it("should show block UI during logout", () => {
      component.logOut();

      expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Logging out..." });
      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should set errors on logout failure", () => {
      const errorResponse = { errorMessages: ["Failed to logout"] };

      mockIdentityService.logOut.and.returnValue(throwError(() => errorResponse));

      component.logOut();

      expect(component.errors()).toEqual(["Failed to logout"]);
    });

    it("should hide block UI even on logout error", () => {
      const errorResponse = { errorMessages: ["Failed to logout"] };

      mockIdentityService.logOut.and.returnValue(throwError(() => errorResponse));

      component.logOut();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });
  });

  // DOM Rendering Tests
  describe("DOM Rendering", () => {
    it("should render panel component", () => {
      const panel = fixture.nativeElement.querySelector("p-panel");
      expect(panel).toBeTruthy();
    });

    it("should render error list component", () => {
      const errorList = fixture.nativeElement.querySelector("ln-error-list");
      expect(errorList).toBeTruthy();
    });

    it("should render api-response component", () => {
      const apiResponse = fixture.nativeElement.querySelector("ln-api-response");
      expect(apiResponse).toBeTruthy();
    });

    it("should render preferred language select component", () => {
      const languageSelect = fixture.nativeElement.querySelector("ln-preferred-language-select");
      expect(languageSelect).toBeTruthy();
    });

    it("should render update profile button", () => {
      const buttons = fixture.nativeElement.querySelectorAll("p-button");
      const updateButton = Array.from(buttons as NodeListOf<Element>).find(btn => btn.textContent?.trim().includes("Update Profile"));
      expect(updateButton).toBeTruthy();
    });

    it("should render logout button", () => {
      const buttons = fixture.nativeElement.querySelectorAll("p-button");
      const logoutButton = Array.from(buttons as NodeListOf<Element>).find(btn => btn.textContent?.trim().includes("Log Out"));
      expect(logoutButton).toBeTruthy();
    });
  });

  // Data Display Tests
  describe("Data Display", () => {
    it("should use type helper to cast to profile", () => {
      const result = component.asProfile(mockProfile);
      expect(result).toEqual(mockProfile);
    });
  });

  // Error Handling Tests
  describe("Error Handling", () => {
    it("should display errors when present", () => {
      component.errors.set(["Test error"]);
      fixture.detectChanges();

      expect(component.errors()).toContain("Test error");
    });

    it("should handle multiple error messages", () => {
      const errors = ["Error 1", "Error 2", "Error 3"];
      component.errors.set(errors);
      fixture.detectChanges();

      expect(component.errors().length).toBe(3);
      expect(component.errors()).toEqual(errors);
    });

    it("should clear previous errors", () => {
      component.errors.set(["Previous error"]);
      expect(component.errors().length).toBe(1);

      component.errors.set([]);
      expect(component.errors().length).toBe(0);
    });
  });
});