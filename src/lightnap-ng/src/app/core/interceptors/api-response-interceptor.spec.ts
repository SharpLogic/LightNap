import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from "@angular/common/http";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { HttpErrorApiResponse } from "@core";
import { RouteAliasService } from "@routing";
import { of, throwError } from "rxjs";
import { IdentityService } from "src/app/identity/services/identity.service";
import { environment } from "src/environments/environment";
import { apiResponseInterceptor } from "./api-response-interceptor";

describe("apiResponseInterceptor", () => {
  let identityService: jasmine.SpyObj<IdentityService>;
  let routeAliasService: jasmine.SpyObj<RouteAliasService>;
  let next: HttpHandlerFn;

  beforeEach(() => {
    const identityServiceSpy = jasmine.createSpyObj("IdentityService", ["logOut"]);
    const routeAliasServiceSpy = jasmine.createSpyObj("RouteAliasService", ["navigate"]);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: IdentityService, useValue: identityServiceSpy },
        { provide: RouteAliasService, useValue: routeAliasServiceSpy },
      ],
    });

    identityService = TestBed.inject(IdentityService) as jasmine.SpyObj<IdentityService>;
    routeAliasService = TestBed.inject(RouteAliasService) as jasmine.SpyObj<RouteAliasService>;

    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200 })));
  });

  it("should handle 401 error by logging out and navigating to login", done => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 401 });

    next = jasmine.createSpy().and.returnValue(throwError(() => errorResponse));

    TestBed.runInInjectionContext(() => {
      apiResponseInterceptor(request, next).subscribe({
        error: (event: HttpEvent<unknown>) => {
          expect(identityService.logOut).toHaveBeenCalled();
          expect(routeAliasService.navigate).toHaveBeenCalledWith("login");
          done();
        },
      });
    });
  });

  it("should log error in non-production environment", done => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 500 });

    spyOn(console, "error");
    environment.production = false;

    next = jasmine.createSpy().and.returnValue(throwError(() => errorResponse));

    TestBed.runInInjectionContext(() => {
      apiResponseInterceptor(request, next).subscribe({
        error: (event: HttpEvent<unknown>) => {
          expect(console.error).toHaveBeenCalledWith(errorResponse);
          done();
        },
      });
    });
  });

  it("should not log error in production environment", done => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 500 });

    spyOn(console, "error");
    environment.production = true;

    next = jasmine.createSpy().and.returnValue(throwError(() => errorResponse));

    TestBed.runInInjectionContext(() => {
      apiResponseInterceptor(request, next).subscribe({
        error: (event: HttpEvent<unknown>) => {
          expect(console.error).not.toHaveBeenCalled();
          done();
        },
      });
    });
  });

  it("should return HttpErrorApiResponse on error", done => {
    const request = new HttpRequest("GET", "/test");
    const errorResponse = new HttpErrorResponse({ status: 500 });

    next = jasmine.createSpy().and.returnValue(throwError(() => errorResponse));

    TestBed.runInInjectionContext(() => {
      apiResponseInterceptor(request, next).subscribe({
        error: (event: HttpErrorApiResponse<any>) => {
          done();
        },
      });
    });
  });
});
