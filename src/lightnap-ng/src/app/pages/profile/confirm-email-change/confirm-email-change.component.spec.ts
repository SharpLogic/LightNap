import { beforeEach, describe, expect, it, vi, type MockedObject } from "vitest";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { of, throwError } from "rxjs";
import { ConfirmEmailChangeComponent } from "./confirm-email-change.component";
import { IdentityService } from "@core/services/identity.service";
import { BlockUiService } from "@core/services/block-ui.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { APP_NAME } from "@core";

describe("ConfirmEmailChangeComponent", () => {
  let component: ConfirmEmailChangeComponent;
  let fixture: ComponentFixture<ConfirmEmailChangeComponent>;
  let mockIdentityService: MockedObject<IdentityService>;
  let mockBlockUiService: MockedObject<BlockUiService>;
  let mockRouteAliasService: MockedObject<RouteAliasService>;

  class MockRouteAliasService {
    navigateWithExtras = vi.fn();
  }

  beforeEach(async () => {
    mockIdentityService = {
      confirmEmailChange: vi.fn().mockName("IdentityService.confirmEmailChange"),
      watchLoggedIn$: vi.fn().mockName("IdentityService.watchLoggedIn$"),
    } as MockedObject<IdentityService>;
    mockBlockUiService = {
      show: vi.fn().mockName("BlockUiService.show"),
      hide: vi.fn().mockName("BlockUiService.hide"),
    } as MockedObject<BlockUiService>;
    mockRouteAliasService = new MockRouteAliasService() as any;

    mockIdentityService.confirmEmailChange.mockReturnValue(of(true));
    mockIdentityService.watchLoggedIn$.mockReturnValue(of(true));

    await TestBed.configureTestingModule({
      imports: [ConfirmEmailChangeComponent],
      providers: [
        provideHttpClient(),
        provideNoopAnimations(),
        provideRouter([]),
        provideZonelessChangeDetection(),
        { provide: APP_NAME, useValue: "TestApp" },
        { provide: IdentityService, useValue: mockIdentityService },
        { provide: BlockUiService, useValue: mockBlockUiService },
        { provide: RouteAliasService, useValue: mockRouteAliasService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmEmailChangeComponent);
    component = fixture.componentInstance;
    // Set required inputs before any detectChanges to avoid NG0950 error
    fixture.componentRef.setInput("newEmail", "newemail@example.com");
    fixture.componentRef.setInput("code", "test-code-123");
  });

  // Component Initialization Tests
  describe("Component Initialization", () => {
    it("should create", () => {
      fixture.detectChanges();
      expect(component).toBeTruthy();
    });

    it("should initialize errors signal as empty array", () => {
      expect(component.errors()).toEqual([]);
    });

    it("should have required input properties", () => {
      expect(component.newEmail).toBeDefined();
      expect(component.code).toBeDefined();
    });
  });

  // Email Confirmation Flow Tests
  describe("Email Confirmation Flow", () => {
    beforeEach(() => {
      fixture.componentRef.setInput("newEmail", "newemail@example.com");
      fixture.componentRef.setInput("code", "confirmation-code-123");
    });

    it("should call confirmEmailChange on ngOnInit with correct parameters", () => {
      fixture.detectChanges(); // Triggers ngOnInit

      expect(mockIdentityService.confirmEmailChange).toHaveBeenCalledWith({
        newEmail: "newemail@example.com",
        code: "confirmation-code-123",
      });
    });

    it("should show block UI with message on init", () => {
      fixture.detectChanges();

      expect(mockBlockUiService.show).toHaveBeenCalledWith({ message: "Confirming email change..." });
    });

    it("should hide block UI after successful confirmation", () => {
      fixture.detectChanges();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should navigate to profile on successful confirmation", () => {
      fixture.detectChanges();

      expect(mockRouteAliasService.navigateWithExtras).toHaveBeenCalledWith("profile", undefined, { replaceUrl: true });
    });

    it("should hide block UI even on error", () => {
      const errorResponse = { errorMessages: ["Invalid confirmation code"] };
      mockIdentityService.confirmEmailChange.mockReturnValue(throwError(() => errorResponse));

      fixture.detectChanges();

      expect(mockBlockUiService.hide).toHaveBeenCalled();
    });

    it("should set errors on confirmation failure", () => {
      const errorResponse = { errorMessages: ["Invalid confirmation code", "Code expired"] };
      mockIdentityService.confirmEmailChange.mockReturnValue(throwError(() => errorResponse));

      fixture.detectChanges();

      expect(component.errors()).toEqual(["Invalid confirmation code", "Code expired"]);
    });

    it("should not navigate on error", () => {
      const errorResponse = { errorMessages: ["Invalid confirmation code"] };
      mockIdentityService.confirmEmailChange.mockReturnValue(throwError(() => errorResponse));

      fixture.detectChanges();

      expect(mockRouteAliasService.navigateWithExtras).not.toHaveBeenCalled();
    });
  });

  // DOM Rendering Tests
  describe("DOM Rendering", () => {
    beforeEach(() => {
      fixture.componentRef.setInput("newEmail", "newemail@example.com");
      fixture.componentRef.setInput("code", "confirmation-code-123");
    });

    it("should render panel with correct header", () => {
      fixture.detectChanges();
      const panel = fixture.nativeElement.querySelector("p-panel");

      expect(panel).toBeTruthy();
      expect(panel.getAttribute("header")).toBe("Confirm Email Change");
    });

    it("should render error list component", () => {
      fixture.detectChanges();
      const errorList = fixture.nativeElement.querySelector("ln-error-list");

      expect(errorList).toBeTruthy();
    });

    it("should display errors when present", () => {
      const errorResponse = { errorMessages: ["Invalid code"] };
      mockIdentityService.confirmEmailChange.mockReturnValue(throwError(() => errorResponse));

      fixture.detectChanges();

      expect(component.errors()).toContain("Invalid code");
    });
  });

  // Input Properties Tests
  describe("Input Properties", () => {
    it("should accept newEmail input", () => {
      fixture.componentRef.setInput("newEmail", "test@example.com");
      fixture.detectChanges();

      expect(component.newEmail()).toBe("test@example.com");
    });

    it("should accept code input", () => {
      fixture.componentRef.setInput("code", "test-code-456");
      fixture.detectChanges();

      expect(component.code()).toBe("test-code-456");
    });

    it("should use different email values correctly", () => {
      fixture.componentRef.setInput("newEmail", "different@example.com");
      fixture.componentRef.setInput("code", "different-code");
      fixture.detectChanges();

      expect(mockIdentityService.confirmEmailChange).toHaveBeenCalledWith({
        newEmail: "different@example.com",
        code: "different-code",
      });
    });
  });

  // Error Handling Tests
  describe("Error Handling", () => {
    beforeEach(() => {
      fixture.componentRef.setInput("newEmail", "newemail@example.com");
      fixture.componentRef.setInput("code", "confirmation-code-123");
    });

    it("should handle network errors", () => {
      const errorResponse = { errorMessages: ["Network error"] };
      mockIdentityService.confirmEmailChange.mockReturnValue(throwError(() => errorResponse));

      fixture.detectChanges();

      expect(component.errors()).toContain("Network error");
    });

    it("should handle multiple error messages", () => {
      const errorResponse = { errorMessages: ["Error 1", "Error 2", "Error 3"] };
      mockIdentityService.confirmEmailChange.mockReturnValue(throwError(() => errorResponse));

      fixture.detectChanges();

      expect(component.errors().length).toBe(3);
      expect(component.errors()).toEqual(["Error 1", "Error 2", "Error 3"]);
    });
  });
});
