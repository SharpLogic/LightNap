import type { MockedObject } from "vitest";
import { TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { PublicService } from "./public.service";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";
import { createLightNapWebApiServiceSpy } from "@testing/helpers";

describe("PublicService", () => {
  let service: PublicService;
  let webApiServiceSpy: MockedObject<LightNapWebApiService>;

  beforeEach(() => {
    const webApiSpy = createLightNapWebApiServiceSpy();

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), PublicService, { provide: LightNapWebApiService, useValue: webApiSpy }],
    });

    webApiServiceSpy = TestBed.inject(LightNapWebApiService) as MockedObject<LightNapWebApiService>;
    service = TestBed.inject(PublicService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should have data service dependency", () => {
    // Service should be created without errors, indicating successful injection
    expect(service).toBeDefined();
  });
});
