import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { ApiResponseComponent } from "./api-response.component";
import { of, throwError } from "rxjs";
import { ErrorApiResponse, SuccessApiResponse } from "@core/backend-api";
import { describe, beforeEach, it, expect } from "vitest";

describe("ApiResponseComponent", () => {
  let component: ApiResponseComponent<string>;
  let fixture: ComponentFixture<ApiResponseComponent<string>>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ApiResponseComponent],
      providers: [provideZonelessChangeDetection(), provideNoopAnimations()],
    });

    fixture = TestBed.createComponent(ApiResponseComponent<string>);
    component = fixture.componentInstance;
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should wrap successful response in SuccessApiResponse", async () => {
    const testData = "Test data";
    fixture.componentRef.setInput("apiResponse", of(testData));
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response).toBeInstanceOf(SuccessApiResponse);
      expect(response.result).toBe(testData);
      expect(response.type).toBe("Success");
    });
  });

  it("should handle error response with errorMessages", async () => {
    const errorResponse = new ErrorApiResponse(["Error 1", "Error 2"]);
    fixture.componentRef.setInput(
      "apiResponse",
      throwError(() => errorResponse)
    );
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response.type).toBe("Error");
      expect(response.errorMessages).toEqual(["Error 1", "Error 2"]);
    });
  });

  it("should handle error without type property", async () => {
    const rawError = new Error("Raw error message");
    fixture.componentRef.setInput(
      "apiResponse",
      throwError(() => rawError)
    );
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response.type).toBe("Error");
      expect(response.errorMessages?.length).toBeGreaterThan(0);
      expect(response.errorMessages?.[0]).toContain("Raw error");
    });
  });

  it("should handle error with type but no errorMessages", async () => {
    const errorWithoutMessages = { type: "Error", errorMessages: [] };
    fixture.componentRef.setInput(
      "apiResponse",
      throwError(() => errorWithoutMessages)
    );
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response.type).toBe("Error");
      expect(response.errorMessages).toEqual(["No error message provided"]);
    });
  });

  it("should handle null result in success response", async () => {
    fixture.componentRef.setInput("apiResponse", of(null));
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response).toBeInstanceOf(SuccessApiResponse);
      expect(response.result).toBeNull();
    });
  });

  it("should handle undefined result in success response", async () => {
    fixture.componentRef.setInput("apiResponse", of(undefined));
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response).toBeInstanceOf(SuccessApiResponse);
      expect(response.result).toBeUndefined();
    });
  });

  it("should update when apiResponse input changes", () => {
    fixture.componentRef.setInput("apiResponse", of("First value"));
    fixture.detectChanges();
    const firstResponse = component.internalApiResponse$();
    expect(firstResponse).toBeDefined();

    fixture.componentRef.setInput("apiResponse", of("Second value"));
    fixture.detectChanges();
    const secondResponse = component.internalApiResponse$();
    expect(secondResponse).toBeDefined();
    expect(secondResponse).not.toBe(firstResponse);
  });

  it("should use default nullMessage", () => {
    fixture.componentRef.setInput("apiResponse", of("test"));
    fixture.detectChanges();
    expect(component.nullMessage()).toBe("This item was not found");
  });

  it("should use custom nullMessage", () => {
    fixture.componentRef.setInput("apiResponse", of("test"));
    fixture.componentRef.setInput("nullMessage", "Custom not found message");
    fixture.detectChanges();
    expect(component.nullMessage()).toBe("Custom not found message");
  });

  it("should use default errorMessage", () => {
    fixture.componentRef.setInput("apiResponse", of("test"));
    fixture.detectChanges();
    expect(component.errorMessage()).toBe("An error occurred");
  });

  it("should use custom errorMessage", () => {
    fixture.componentRef.setInput("apiResponse", of("test"));
    fixture.componentRef.setInput("errorMessage", "Custom error message");
    fixture.detectChanges();
    expect(component.errorMessage()).toBe("Custom error message");
  });

  it("should use default loadingMessage", () => {
    fixture.componentRef.setInput("apiResponse", of("test"));
    fixture.detectChanges();
    expect(component.loadingMessage()).toBe("Loading...");
  });

  it("should use custom loadingMessage", () => {
    fixture.componentRef.setInput("apiResponse", of("test"));
    fixture.componentRef.setInput("loadingMessage", "Please wait...");
    fixture.detectChanges();
    expect(component.loadingMessage()).toBe("Please wait...");
  });

  it("should handle string data in success response", async () => {
    const stringData = "Test string data";
    fixture.componentRef.setInput("apiResponse", of(stringData));
    fixture.detectChanges();

    component.internalApiResponse$()?.subscribe(response => {
      expect(response.result).toBe(stringData);
    });
  });
});
