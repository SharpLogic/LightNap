import { HttpEvent, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { IdentityService } from "@core/services/identity.service";
import { Observable } from "rxjs";

/**
 * Intercepts HTTP requests to add an Authorization header with a bearer token if the request URL starts with the API root URL.
 *
 * @param request - The outgoing HTTP request.
 * @param next - The next handler in the HTTP request chain.
 * @returns An observable of the HTTP event, with the Authorization header added if applicable.
 */
export function tokenInterceptor(request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  const apiUrlRoot = "/api/";
  const identityService = inject(IdentityService);

  if (request.url.startsWith(apiUrlRoot)) {
    const token = identityService.getBearerToken();
    if (token) {
      return next(
        request.clone({
          headers: request.headers.set("Authorization", token),
          withCredentials: true,
        })
      );
    }

    return next(
      request.clone({
        withCredentials: true,
      })
    );
  }

  return next(request);
}
