import { afterEach, beforeEach, describe, expect, it, vi, type MockedObject } from "vitest";
import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from "@angular/common/http";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { HttpErrorApiResponse, ApiResponseDto, ApiResponseType } from "@core/backend-api";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { IdentityService } from "@core/services/identity.service";
import { of, throwError } from "rxjs";
import { environment } from "src/environments/environment";
import { apiResponseInterceptor } from "./api-response-interceptor";

describe("apiResponseInterceptor", () => {
  let identityService: MockedObject<IdentityService>;
  let routeAliasService: MockedObject<RouteAliasService>;
  let next: HttpHandlerFn;

  beforeEach(() => {
    const identityServiceSpy = { logOut: vi.fn().mockName("IdentityService.logOut") };
    const routeAliasServiceSpy = { navigate: vi.fn().mockName("RouteAliasService.navigate") };

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: IdentityService, useValue: identityServiceSpy },
        { provide: RouteAliasService, useValue: routeAliasServiceSpy },
      ],
    });

    identityService = TestBed.inject(IdentityService) as MockedObject<IdentityService>;
    routeAliasService = TestBed.inject(RouteAliasService) as MockedObject<RouteAliasService>;
    next = vi.fn().mockReturnValue(of(new HttpResponse({ status: 200 })));
  });

  it("should handle 401 error by logging out and navigating to login", async () => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 401 });
    next = vi.fn().mockReturnValue(throwError(() => errorResponse));

    await new Promise<void>(resolve => {
      TestBed.runInInjectionContext(() => {
        apiResponseInterceptor(request, next).subscribe({
          error: () => {
            expect(identityService.logOut).toHaveBeenCalled();
            expect(routeAliasService.navigate).toHaveBeenCalledWith("login");
            resolve();
          },
        });
      });
    });
  });

  it("should log error in non-production environment", async () => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 500 });
    vi.spyOn(console, "error");
    environment.production = false;
    next = vi.fn().mockReturnValue(throwError(() => errorResponse));

    await new Promise<void>(resolve => {
      TestBed.runInInjectionContext(() => {
        apiResponseInterceptor(request, next).subscribe({
          error: () => {
            expect(console.error).toHaveBeenCalledWith(errorResponse);
            resolve();
          },
        });
      });
    });
  });

  it("should not log error in production environment", async () => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 500 });
    const consoleSpy = vi.spyOn(console, "error");
    environment.production = true;
    next = vi.fn().mockReturnValue(throwError(() => errorResponse));

    await new Promise<void>(resolve => {
      TestBed.runInInjectionContext(() => {
        apiResponseInterceptor(request, next).subscribe({
          error: () => {
            expect(consoleSpy).not.toHaveBeenCalled();
            resolve();
          },
        });
      });
    });
  });

  it("should return HttpErrorApiResponse on error", async () => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 500 });
    next = vi.fn().mockReturnValue(throwError(() => errorResponse));

    await new Promise<void>(resolve => {
      TestBed.runInInjectionContext(() => {
        apiResponseInterceptor(request, next).subscribe({
          error: (event: HttpErrorApiResponse<any>) => {
            expect(event).toBeInstanceOf(HttpErrorApiResponse);
            resolve();
          },
        });
      });
    });
  });

  it("should unwrap successful ApiResponse", async () => {
    const request = new HttpRequest("GET", "/test");
    const successResponse = new HttpResponse({
      status: 200,
      body: { type: ApiResponseType.Success, result: "test" } as ApiResponseDto<string>,
    });
    next = vi.fn().mockReturnValue(of(successResponse));

    await new Promise<void>(resolve => {
      TestBed.runInInjectionContext(() => {
        apiResponseInterceptor(request, next).subscribe({
          next: (event: any) => {
            expect(event.body).toEqual("test");
            resolve();
          },
        });
      });
    });
  });

  it("should handle ApiResponseType.Error", async () => {
    const request = new HttpRequest("GET", "/test");
    const apiError = { type: ApiResponseType.Error, message: "API Error" } as ApiResponseDto<string>;
    const errorResponse = new HttpResponse({ status: 200, body: apiError });
    next = vi.fn().mockReturnValue(of(errorResponse));

    await new Promise<void>(resolve => {
      TestBed.runInInjectionContext(() => {
        apiResponseInterceptor(request, next).subscribe({
          error: event => {
            expect(event).toEqual(apiError);
            resolve();
          },
        });
      });
    });
  });

  afterEach(() => {
    vi.clearAllMocks();
    vi.restoreAllMocks();
  });
});
