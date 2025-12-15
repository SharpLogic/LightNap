import { afterEach, beforeEach, describe, expect, it, vi, type MockedObject } from "vitest";
import { HttpClient, provideHttpClient, withInterceptors } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { IdentityService } from "@core/services/identity.service";
import { tokenInterceptor } from "./token-interceptor";

describe("tokenInterceptor", () => {
  let httpMock: HttpTestingController;
  let httpClient: HttpClient;
  let identityService: MockedObject<IdentityService>;

  beforeEach(() => {
    const identityServiceSpy = {
      getBearerToken: vi.fn().mockName("IdentityService.getBearerToken"),
    };

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideHttpClient(withInterceptors([tokenInterceptor])),
        provideHttpClientTesting(),
        { provide: IdentityService, useValue: identityServiceSpy },
      ],
    });

    httpMock = TestBed.inject(HttpTestingController);
    httpClient = TestBed.inject(HttpClient);
    identityService = TestBed.inject(IdentityService) as MockedObject<IdentityService>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it("should add Authorization header for API requests", () => {
    const token = "Bearer test-token";
    identityService.getBearerToken.mockReturnValue(token);

    httpClient.get("/api/data").subscribe();

    const req = httpMock.expectOne("/api/data");
    expect(req.request.headers.has("Authorization")).toBeTruthy();
    expect(req.request.headers.get("Authorization")).toBe(token);
    expect(req.request.withCredentials).toBe(true);
    req.flush({});
  });

  it("should not add Authorization header for non-API requests", () => {
    httpClient.get("http://otherapi.example.com/data").subscribe();

    const req = httpMock.expectOne("http://otherapi.example.com/data");
    expect(req.request.headers.has("Authorization")).toBeFalsy();
    expect(req.request.withCredentials).toBeFalsy();
    req.flush({});
  });

  it("should not add Authorization header if token is not available", () => {
    identityService.getBearerToken.mockReturnValue(undefined);

    httpClient.get("/api/data").subscribe();

    const req = httpMock.expectOne("/api/data");
    expect(req.request.headers.has("Authorization")).toBeFalsy();
    expect(req.request.withCredentials).toBe(true);
    req.flush({});
  });
});
