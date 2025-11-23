import { HttpEvent, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { Observable } from "rxjs";
import { HttpResponse } from "@angular/common/http";
import { map } from "rxjs/operators";

/**
 * Intercepts HTTP responses to convert anything matching an ISO string date format into a Date object.
 *
 * @param request - The outgoing HTTP request.
 * @param next - The next handler in the HTTP request chain.
 * @returns An observable of the HTTP event.
 */
export function dateInterceptor(request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  function isIsoDate(str: string): boolean {
    const isoDateRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?Z?$/;
    return isoDateRegex.test(str);
  }

  function convertDates(obj: any): any {
    if (obj === null || typeof obj !== "object") {
      if (typeof obj === "string" && isIsoDate(obj)) {
        return new Date(obj);
      }
      return obj;
    }
    if (Array.isArray(obj)) {
      return obj.map(convertDates);
    }
    const result: any = {};
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        result[key] = convertDates(obj[key]);
      }
    }
    return result;
  }

  return next(request).pipe(
    map(event => {
      if (event instanceof HttpResponse) {
        return event.clone({ body: convertDates(event.body) });
      }
      return event;
    })
  );
}
