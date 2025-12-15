import { signal } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { distinctUntilJsonChanged, setApiErrors } from "./rxjs-helpers";
import { ErrorApiResponse } from "@core/backend-api";

describe("RxJsHelpers", () => {
  describe("distinctUntilJsonChanged", () => {
    it("should emit when JSON representation changes", async () => {
      const values: any[] = [];
      const source$ = of(
        { id: 1, name: "John" },
        { id: 1, name: "John" }, // Duplicate - should be filtered
        { id: 2, name: "Jane" }
      );

      source$.pipe(distinctUntilJsonChanged()).subscribe({
        next: value => values.push(value),
        complete: () => {
          expect(values.length).toBe(2);
          expect(values[0]).toEqual({ id: 1, name: "John" });
          expect(values[1]).toEqual({ id: 2, name: "Jane" });
        },
      });
    });

    it("should emit first value even if null", async () => {
      const values: any[] = [];
      const source$ = of(null, null, { id: 1 });

      source$.pipe(distinctUntilJsonChanged()).subscribe({
        next: value => values.push(value),
        complete: () => {
          expect(values.length).toBe(2);
          expect(values[0]).toBeNull();
          expect(values[1]).toEqual({ id: 1 });
        },
      });
    });

    it("should detect changes in nested objects", async () => {
      const values: any[] = [];
      const source$ = of(
        { user: { id: 1, profile: { name: "John" } } },
        { user: { id: 1, profile: { name: "John" } } }, // Duplicate
        { user: { id: 1, profile: { name: "Jane" } } }
      );

      source$.pipe(distinctUntilJsonChanged()).subscribe({
        next: value => values.push(value),
        complete: () => {
          expect(values.length).toBe(2);
          expect(values[1].user.profile.name).toBe("Jane");
        },
      });
    });

    it("should detect changes in array order", async () => {
      const values: any[] = [];
      const source$ = of([1, 2, 3], [1, 2, 3], [3, 2, 1]);

      source$.pipe(distinctUntilJsonChanged()).subscribe({
        next: value => values.push(value),
        complete: () => {
          expect(values.length).toBe(2);
          expect(values[0]).toEqual([1, 2, 3]);
          expect(values[1]).toEqual([3, 2, 1]);
        },
      });
    });

    it("should work with primitives", async () => {
      const values: any[] = [];
      const source$ = of(1, 1, 2, 2, 3);

      source$.pipe(distinctUntilJsonChanged()).subscribe({
        next: value => values.push(value),
        complete: () => {
          expect(values).toEqual([1, 2, 3]);
        },
      });
    });
  });

  describe("setApiErrors", () => {
    it("should set error messages from ErrorApiResponse", () => {
      const errorSignal = signal<string[]>([]);
      const response = new ErrorApiResponse<string>(["Error 1", "Error 2", "Error 3"]);

      setApiErrors(errorSignal)(response);

      expect(errorSignal()).toEqual(["Error 1", "Error 2", "Error 3"]);
    });

    it("should set empty array when errorMessages is undefined", () => {
      const errorSignal = signal<string[]>(["Previous error"]);
      const response = new ErrorApiResponse<string>(undefined as any);

      setApiErrors(errorSignal)(response);

      expect(errorSignal()).toEqual([]);
    });

    it("should set empty array when errorMessages is null", () => {
      const errorSignal = signal<string[]>(["Previous error"]);
      const response = new ErrorApiResponse<string>(null as any);

      setApiErrors(errorSignal)(response);

      expect(errorSignal()).toEqual([]);
    });

    it("should overwrite previous errors", () => {
      const errorSignal = signal<string[]>(["Old error 1", "Old error 2"]);
      const response = new ErrorApiResponse<string>(["New error"]);

      setApiErrors(errorSignal)(response);

      expect(errorSignal()).toEqual(["New error"]);
    });

    it("should work as RxJS operator", async () => {
      const errorSignal = signal<string[]>([]);
      const errorResponse = new ErrorApiResponse<string>(["API Error"]);

      of(errorResponse).subscribe({
        next: setApiErrors(errorSignal),
        complete: () => {
          expect(errorSignal()).toEqual(["API Error"]);
        },
      });
    });
  });
});
