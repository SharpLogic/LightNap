import { HttpClient, provideHttpClient, withInterceptors } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { API_URL_ROOT, IdentityService } from "@core";
import { tokenInterceptor } from "./token-interceptor";

describe("TokenInterceptor", () => {
  let httpMock: HttpTestingController;
  let httpClient: HttpClient;
  let identityService: jasmine.SpyObj<IdentityService>;

  beforeEach(() => {
    const identityServiceSpy = jasmine.createSpyObj("IdentityService", ["getBearerToken"]);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideHttpClient(withInterceptors([tokenInterceptor])),
        provideHttpClientTesting(),
        { provide: IdentityService, useValue: identityServiceSpy },
        { provide: API_URL_ROOT, useValue: "http://api.example.com" },
      ],
    });

    httpMock = TestBed.inject(HttpTestingController);
    httpClient = TestBed.inject(HttpClient);
    identityService = TestBed.inject(IdentityService) as jasmine.SpyObj<IdentityService>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it("should add Authorization header for API requests", () => {
    const token = "Bearer test-token";
    identityService.getBearerToken.and.returnValue(token);

    httpClient.get("http://api.example.com/data").subscribe();

    const req = httpMock.expectOne("http://api.example.com/data");
    expect(req.request.headers.has("Authorization")).toBeTruthy();
    expect(req.request.headers.get("Authorization")).toBe(token);
    expect(req.request.withCredentials).toBeTrue();
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
    identityService.getBearerToken.and.returnValue(undefined);

    httpClient.get("http://api.example.com/data").subscribe();

    const req = httpMock.expectOne("http://api.example.com/data");
    expect(req.request.headers.has("Authorization")).toBeFalsy();
    expect(req.request.withCredentials).toBeTrue();
    req.flush({});
  });
});
