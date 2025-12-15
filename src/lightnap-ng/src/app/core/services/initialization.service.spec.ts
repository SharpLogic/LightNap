import { TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { InitializationService } from "./initialization.service";
import { firstValueFrom } from "rxjs";

describe("InitializationService", () => {
  let service: InitializationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), InitializationService],
    });

    service = TestBed.inject(InitializationService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should emit true when initialized", async () => {
    service.initialized$.subscribe(initialized => {
      expect(initialized).toBe(true);
    });

    service.initialize();
  });

  it("should complete the observable after initialization", async () => {
    let emissionCount = 0;

    service.initialized$.subscribe({
      next: () => {
        emissionCount++;
      },
      complete: () => {
        expect(emissionCount).toBe(1);
      },
    });

    service.initialize();
  });

  it("should allow subscription before initialization", async () => {
    const promise = firstValueFrom(service.initialized$);

    // Initialize after subscription
    service.initialize();

    const result = await promise;
    expect(result).toBe(true);
  });

  it("should replay initialization status to late subscribers", async () => {
    // Initialize first
    service.initialize();

    // Subscribe after initialization
    service.initialized$.subscribe(initialized => {
      expect(initialized).toBe(true);
    });
  });
});
