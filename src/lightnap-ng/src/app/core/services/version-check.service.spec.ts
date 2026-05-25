import { beforeEach, describe, expect, it, vi, type MockedObject } from "vitest";
import { TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { SwUpdate, VersionReadyEvent } from "@angular/service-worker";
import { VersionCheckService } from "./version-check.service";
import { Subject } from "rxjs";

describe("VersionCheckService", () => {
  let service: VersionCheckService;
  let swUpdateSpy: MockedObject<SwUpdate>;
  let versionUpdatesSubject: Subject<VersionReadyEvent>;

  beforeEach(() => {
    versionUpdatesSubject = new Subject<VersionReadyEvent>();

    swUpdateSpy = {
      checkForUpdate: vi.fn().mockName("SwUpdate.checkForUpdate"),
      activateUpdate: vi.fn().mockName("SwUpdate.activateUpdate"),
    } as MockedObject<SwUpdate>;

    Object.defineProperty(swUpdateSpy, "isEnabled", {
      get: () => true,
      configurable: true,
    });

    Object.defineProperty(swUpdateSpy, "versionUpdates", {
      get: () => versionUpdatesSubject.asObservable(),
      configurable: true,
    });

    swUpdateSpy.checkForUpdate.mockResolvedValue(false);
    swUpdateSpy.activateUpdate.mockResolvedValue(true);

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), VersionCheckService, { provide: SwUpdate, useValue: swUpdateSpy }],
    });

    service = TestBed.inject(VersionCheckService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  describe("update detection", () => {
    it("should start update check when service worker is enabled", () => {
      service.startUpdateCheck();
      expect(swUpdateSpy.checkForUpdate).toHaveBeenCalled();
      expect(swUpdateSpy.versionUpdates).toBeDefined();
    });

    it("should activate update when a VERSION_READY event is received", async () => {
      service.startUpdateCheck();

      versionUpdatesSubject.next({
        type: "VERSION_READY",
        currentVersion: { hash: "old" },
        latestVersion: { hash: "new" },
      });

      // Wait a microtask so the subscription handler runs.
      await Promise.resolve();
      expect(swUpdateSpy.activateUpdate).toHaveBeenCalled();
    });

    it("should ignore non-VERSION_READY events", async () => {
      service.startUpdateCheck();
      swUpdateSpy.activateUpdate.mockClear();

      versionUpdatesSubject.next({
        type: "VERSION_DETECTED",
        version: { hash: "new" },
      } as any);

      await Promise.resolve();
      expect(swUpdateSpy.activateUpdate).not.toHaveBeenCalled();
    });
  });

  describe("service worker handling", () => {
    it("should handle service worker disabled", () => {
      Object.defineProperty(swUpdateSpy, "isEnabled", {
        get: () => false,
        configurable: true,
      });

      expect(() => service.startUpdateCheck()).not.toThrow();
      expect(swUpdateSpy.checkForUpdate).not.toHaveBeenCalled();
    });
  });
});
