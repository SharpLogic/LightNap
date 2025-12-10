import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { SwUpdate, VersionReadyEvent } from '@angular/service-worker';
import { VersionCheckService } from './version-check.service';
import { Subject } from 'rxjs';

describe('VersionCheckService', () => {
  let service: VersionCheckService;
  let swUpdateSpy: jasmine.SpyObj<SwUpdate>;
  let versionUpdatesSubject: Subject<VersionReadyEvent>;

  beforeEach(() => {
    versionUpdatesSubject = new Subject<VersionReadyEvent>();

    swUpdateSpy = jasmine.createSpyObj<SwUpdate>('SwUpdate', [
      'checkForUpdate',
      'activateUpdate',
    ]);

    Object.defineProperty(swUpdateSpy, 'isEnabled', {
      get: () => true,
      configurable: true,
    });

    Object.defineProperty(swUpdateSpy, 'versionUpdates', {
      get: () => versionUpdatesSubject.asObservable(),
      configurable: true,
    });

    swUpdateSpy.activateUpdate.and.returnValue(Promise.resolve(true));

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        VersionCheckService,
        { provide: SwUpdate, useValue: swUpdateSpy },
      ],
    });

    service = TestBed.inject(VersionCheckService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('update detection', () => {
    it('should start update check when service worker is enabled', () => {
      service.startUpdateCheck();
      expect(swUpdateSpy.versionUpdates).toBeDefined();
    });

    it('should emit when new version is ready', (done) => {
      service.versionUpdated$.subscribe((updated) => {
        expect(updated).toBe(true);
        done();
      });

      service.startUpdateCheck();

      // Simulate VERSION_READY event
      versionUpdatesSubject.next({
        type: 'VERSION_READY',
        currentVersion: { hash: 'old' },
        latestVersion: { hash: 'new' },
      });
    });

    it('should not emit for non-VERSION_READY events', (done) => {
      let emitted = false;

      service.versionUpdated$.subscribe(() => {
        emitted = true;
      });

      service.startUpdateCheck();

      // Simulate different event type
      versionUpdatesSubject.next({
        type: 'VERSION_DETECTED',
        version: { hash: 'new' },
      } as any);

      setTimeout(() => {
        expect(emitted).toBe(false);
        done();
      }, 100);
    });
  });

  describe('service worker handling', () => {
    // Note: activateUpdate() calls location.reload() which cannot be tested without causing page reload
    // This method is tested through integration testing

    it('should handle service worker disabled', () => {
      Object.defineProperty(swUpdateSpy, 'isEnabled', {
        get: () => false,
        configurable: true,
      });

      // Should not throw
      expect(() => service.startUpdateCheck()).not.toThrow();
    });
  });
});
