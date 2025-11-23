import { HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from "@angular/common/http";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { dateInterceptor } from "./date-interceptor";

describe("dateInterceptor", () => {
  let next: HttpHandlerFn;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection()],
    });

    next = jasmine.createSpy().and.callFake(() => of(new HttpResponse({ status: 200 })));
  });

  it("should convert ISO date strings to Date objects in response body", done => {
    const request = new HttpRequest("GET", "/test");
    const responseBody = {
      createdAt: "2023-10-01T12:00:00Z",
      updatedAt: "2023-10-02T13:30:45.123Z",
      name: "test",
    };
    const expectedBody = {
      createdAt: new Date("2023-10-01T12:00:00Z"),
      updatedAt: new Date("2023-10-02T13:30:45.123Z"),
      name: "test",
    };

    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200, body: responseBody })));

    TestBed.runInInjectionContext(() => {
      dateInterceptor(request, next).subscribe((event: HttpEvent<unknown>) => {
        expect(event instanceof HttpResponse).toBe(true);
        const httpResponse = event as HttpResponse<unknown>;
        expect(httpResponse.body).toEqual(expectedBody);
        expect((httpResponse.body as any).createdAt instanceof Date).toBe(true);
        expect((httpResponse.body as any).updatedAt instanceof Date).toBe(true);
        done();
      });
    });
  });

  it("should handle nested objects with dates", done => {
    const request = new HttpRequest("GET", "/test");
    const responseBody = {
      user: {
        profile: {
          birthDate: "1990-01-01T00:00:00Z",
        },
      },
    };
    const expectedBody = {
      user: {
        profile: {
          birthDate: new Date("1990-01-01T00:00:00Z"),
        },
      },
    };

    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200, body: responseBody })));

    TestBed.runInInjectionContext(() => {
      dateInterceptor(request, next).subscribe((event: HttpEvent<unknown>) => {
        expect(event instanceof HttpResponse).toBe(true);
        const httpResponse = event as HttpResponse<unknown>;
        expect(httpResponse.body).toEqual(expectedBody);
        expect(((httpResponse.body as any).user.profile.birthDate) instanceof Date).toBe(true);
        done();
      });
    });
  });

  it("should handle arrays with dates", done => {
    const request = new HttpRequest("GET", "/test");
    const responseBody = {
      dates: ["2023-01-01T00:00:00Z", "2023-02-01T00:00:00Z"],
    };
    const expectedBody = {
      dates: [new Date("2023-01-01T00:00:00Z"), new Date("2023-02-01T00:00:00Z")],
    };

    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200, body: responseBody })));

    TestBed.runInInjectionContext(() => {
      dateInterceptor(request, next).subscribe((event: HttpEvent<unknown>) => {
        expect(event instanceof HttpResponse).toBe(true);
        const httpResponse = event as HttpResponse<unknown>;
        expect(httpResponse.body).toEqual(expectedBody);
        expect((httpResponse.body as any).dates[0] instanceof Date).toBe(true);
        expect((httpResponse.body as any).dates[1] instanceof Date).toBe(true);
        done();
      });
    });
  });

  it("should not convert non-ISO date strings", done => {
    const request = new HttpRequest("GET", "/test");
    const responseBody = {
      date: "not-a-date",
      anotherDate: "2023/10/01T12:00:00Z", // not matching ISO format
    };

    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200, body: responseBody })));

    TestBed.runInInjectionContext(() => {
      dateInterceptor(request, next).subscribe((event: HttpEvent<unknown>) => {
        expect(event instanceof HttpResponse).toBe(true);
        const httpResponse = event as HttpResponse<unknown>;
        expect(httpResponse.body).toEqual(responseBody);
        expect(typeof (httpResponse.body as any).date).toBe("string");
        expect(typeof (httpResponse.body as any).anotherDate).toBe("string");
        done();
      });
    });
  });

  it("should not modify non-HttpResponse events", done => {
    const request = new HttpRequest("GET", "/test");
    const nonHttpEvent = { type: 0 }; // HttpSentEvent or similar

    next = jasmine.createSpy().and.returnValue(of(nonHttpEvent));

    TestBed.runInInjectionContext(() => {
      dateInterceptor(request, next).subscribe((event: HttpEvent<unknown>) => {
        expect(event).toEqual(nonHttpEvent);
        done();
      });
    });
  });

  it("should handle null and primitive values", done => {
    const request = new HttpRequest("GET", "/test");
    const responseBody = {
      nullValue: null,
      stringValue: "string",
      numberValue: 123,
      booleanValue: true,
    };

    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200, body: responseBody })));

    TestBed.runInInjectionContext(() => {
      dateInterceptor(request, next).subscribe((event: HttpEvent<unknown>) => {
        expect(event instanceof HttpResponse).toBe(true);
        const httpResponse = event as HttpResponse<unknown>;
        expect(httpResponse.body).toEqual(responseBody);
        done();
      });
    });
  });
});