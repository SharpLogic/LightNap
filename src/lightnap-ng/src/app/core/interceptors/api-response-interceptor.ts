import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { ApiResponseDto, HttpErrorApiResponse } from "@core";
import { RouteAliasService } from "@routing";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { IdentityService } from "src/app/identity/services/identity.service";
import { environment } from "src/environments/environment";

/**
 * Intercepts HTTP responses to unwrap successful results and throw errors via the RxJS pipeline.
 *
 * @param request - The outgoing HTTP request.
 * @param next - The next handler in the HTTP request pipeline.
 * @returns An observable of the HTTP event.
 *
 * @remarks
 * - If the response status is 401 (Unauthorized), the user is logged out and navigated to the login page.
 * - In non-production environments, the error is logged to the console.
 * - The function returns an observable of an `HttpResponse` containing an `HttpErrorApiResponse` object.
 */
export function apiResponseInterceptor(request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  const identityService = inject(IdentityService);
  const routeAliasService = inject(RouteAliasService);

  return next(request).pipe(
    switchMap(httpEvent => {
      const apiHttpResponse = httpEvent as HttpResponse<ApiResponseDto<string>>;
      switch (apiHttpResponse.body?.type) {
        case "Error":
        case "UnexpectedError":
          return throwError(() => apiHttpResponse.body);
        case "Success":
          return of(apiHttpResponse.clone({ body: apiHttpResponse.body.result ?? null }));
        default:
          break;
      }

      return of(httpEvent);
    }),
    catchError(error => {
      if (error.status === 401) {
        identityService.logOut();
        routeAliasService.navigate("login");
        return throwError(() => error);
      }

      switch (error?.type) {
        case "Error":
        case "UnexpectedError":
          return throwError(() => error);
      }

      if (!environment.production) {
        console.error(error);
      }

      return throwError(() => new HttpErrorApiResponse(error as HttpErrorResponse));
    })
  );
}
