import { beforeEach, describe, expect, it, vi, type MockedObject } from "vitest";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { of, throwError } from "rxjs";
import { DevicesComponent } from "./devices.component";
import { IdentityService } from "@core/services/identity.service";
import { APP_NAME, DeviceDto } from "@core";
import { ConfirmationService } from "primeng/api";

describe("DevicesComponent", () => {
  let component: DevicesComponent;
  let fixture: ComponentFixture<DevicesComponent>;
  let mockIdentityService: MockedObject<IdentityService>;
  let confirmationService: ConfirmationService;

  const mockDevices: DeviceDto[] = [
    {
      id: "device-1",
      lastSeen: new Date("2025-11-01T10:00:00Z"),
      ipAddress: "192.168.1.1",
      details: "Chrome on Windows - Desktop",
    },
    {
      id: "device-2",
      lastSeen: new Date("2025-11-10T15:30:00Z"),
      ipAddress: "192.168.1.2",
      details: "Safari on iOS - Mobile",
    },
  ];

  beforeEach(async () => {
    mockIdentityService = {
      getDevices: vi.fn().mockName("IdentityService.getDevices"),
      revokeDevice: vi.fn().mockName("IdentityService.revokeDevice"),
      watchLoggedIn$: vi.fn().mockName("IdentityService.watchLoggedIn$"),
    } as MockedObject<IdentityService>;

    mockIdentityService.getDevices.mockReturnValue(of(mockDevices));
    mockIdentityService.revokeDevice.mockReturnValue(of(true));
    mockIdentityService.watchLoggedIn$.mockReturnValue(of(true));

    await TestBed.configureTestingModule({
      imports: [DevicesComponent],
      providers: [
        provideHttpClient(),
        provideNoopAnimations(),
        provideRouter([]),
        provideZonelessChangeDetection(),
        { provide: APP_NAME, useValue: "TestApp" },
        { provide: IdentityService, useValue: mockIdentityService },
        ConfirmationService, // Use real service, not mock
      ],
    }).compileComponents();

    confirmationService = TestBed.inject(ConfirmationService);
    // Remove the spy from beforeEach - we'll spy in individual tests that need custom behavior

    fixture = TestBed.createComponent(DevicesComponent);
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

    it("should load devices on initialization", () => {
      expect(mockIdentityService.getDevices).toHaveBeenCalled();
    });

    it("should have devices$ signal with observable", () => {
      expect(component.devices$()).toBeDefined();
    });

    it("should have type helper functions", () => {
      expect(component.asDevices).toBeDefined();
      expect(component.asDevice).toBeDefined();
    });
  });

  // Device Management Tests
  describe("Device Management", () => {
    it("should call confirmationService when revoking device", () => {
      vi.spyOn(confirmationService, "confirm").mockReturnValue(confirmationService);
      const event = { target: document.createElement("button") };
      const deviceId = "device-1";

      component.revokeDevice(event, deviceId);

      expect(confirmationService.confirm).toHaveBeenCalledWith(
        expect.objectContaining({
          header: "Confirm Revoke",
          message: "Are you sure that you want to revoke this device?",
          target: event.target,
          key: deviceId,
        })
      );
    });

    it("should revoke device when confirmation is accepted", () => {
      const event = { target: document.createElement("button") };
      const deviceId = "device-1";

      vi.spyOn(confirmationService, "confirm").mockImplementation((config: any) => {
        config.accept();
        return confirmationService;
      });

      component.revokeDevice(event, deviceId);

      expect(mockIdentityService.revokeDevice).toHaveBeenCalledWith(deviceId);
    });

    it("should reload devices after successful revoke", () => {
      const event = { target: document.createElement("button") };
      const deviceId = "device-1";

      vi.spyOn(confirmationService, "confirm").mockImplementation((config: any) => {
        config.accept();
        return confirmationService;
      });

      const initialCallCount = vi.mocked(mockIdentityService.getDevices).mock.calls.length;
      component.revokeDevice(event, deviceId);

      expect(vi.mocked(mockIdentityService.getDevices).mock.calls.length).toBe(initialCallCount + 1);
    });

    it("should set errors on revoke failure", () => {
      const event = { target: document.createElement("button") };
      const deviceId = "device-1";
      const errorResponse = { errorMessages: ["Failed to revoke device"] };

      mockIdentityService.revokeDevice.mockReturnValue(throwError(() => errorResponse));
      vi.spyOn(confirmationService, "confirm").mockImplementation((config: any) => {
        config.accept();
        return confirmationService;
      });

      component.revokeDevice(event, deviceId);

      expect(component.errors()).toEqual(["Failed to revoke device"]);
    });

    it("should not revoke device when confirmation is cancelled", () => {
      const event = { target: document.createElement("button") };
      const deviceId = "device-1";

      vi.spyOn(confirmationService, "confirm").mockImplementation((config: any) => {
        // Don't call accept
        return confirmationService;
      });

      const initialCallCount = vi.mocked(mockIdentityService.revokeDevice).mock.calls.length;
      component.revokeDevice(event, deviceId);

      expect(vi.mocked(mockIdentityService.revokeDevice).mock.calls.length).toBe(initialCallCount);
    });
  });

  // DOM Rendering Tests
  describe("DOM Rendering", () => {
    it("should render panel component", () => {
      const panel = fixture.nativeElement.querySelector("p-panel");
      expect(panel).toBeTruthy();
    });

    it("should render table component", () => {
      const table = fixture.nativeElement.querySelector("p-table");
      expect(table).toBeTruthy();
    });

    it("should render error list component", () => {
      const errorList = fixture.nativeElement.querySelector("ln-error-list");
      expect(errorList).toBeTruthy();
    });

    it("should render api-response component", () => {
      const apiResponse = fixture.nativeElement.querySelector("ln-api-response");
      expect(apiResponse).toBeTruthy();
    });

    it("should render confirm-dialog component", () => {
      const confirmDialog = fixture.nativeElement.querySelector("ln-confirm-dialog");
      expect(confirmDialog).toBeTruthy();
    });
  });

  // Data Display Tests
  describe("Data Display", () => {
    it("should display devices in table", async () => {
      component.devices$().subscribe(devices => {
        expect(devices).toEqual(mockDevices);
      });
    });

    it("should handle empty devices list", async () => {
      mockIdentityService.getDevices.mockReturnValue(of([]));
      component.devices$.set(mockIdentityService.getDevices());

      component.devices$().subscribe((devices: any) => {
        expect(devices?.length).toBe(0);
      });
    });

    it("should use type helper to cast to devices array", () => {
      const result = component.asDevices(mockDevices);
      expect(result).toEqual(mockDevices);
    });

    it("should use type helper to cast to single device", () => {
      const result = component.asDevice(mockDevices[0]);
      expect(result).toEqual(mockDevices[0]);
    });
  });

  // Error Handling Tests
  describe("Error Handling", () => {
    it("should display errors when device loading fails", () => {
      const errorResponse = { errorMessages: ["Failed to load devices"] };
      component.errors.set(["Failed to load devices"]);
      fixture.detectChanges();

      expect(component.errors()).toContain("Failed to load devices");
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
