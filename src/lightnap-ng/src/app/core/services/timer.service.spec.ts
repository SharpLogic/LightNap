import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { take, skip } from "rxjs/operators";
import { firstValueFrom } from "rxjs";
import { TimerService } from "./timer.service";
import { describe, beforeEach, it, expect, vi } from "vitest";

describe("TimerService", () => {
    let service: TimerService;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideZonelessChangeDetection(), TimerService] });
        service = TestBed.inject(TimerService);
    });

    it("should be created", () => {
        expect(service).toBeTruthy();
    });

    describe("timer validation", () => {
        it("should throw an error if interval is negative", () => {
            expect(() => service.watchTimer$(-1000)).toThrowError("Intervals must be positive: '-1000' not valid");
        });
    });

    describe("timer emissions", () => {
        it("should return an observable that emits at the specified interval", async () => {
            const milliseconds = 1000;
            const emittedValue = await firstValueFrom(service.watchTimer$(milliseconds).pipe(take(1)));
            expect(emittedValue).toBe(milliseconds);
        });

        it("should emit correct values multiple times", async () => {
            const milliseconds = 500;
            const values = await firstValueFrom(service.watchTimer$(milliseconds).pipe(take(3)));
            expect(values).toBe(milliseconds);
        });
    });

    describe("timer caching", () => {
        it("should return the same observable for the same interval", () => {
            const milliseconds = 1000;
            const observable1 = service.watchTimer$(milliseconds);
            const observable2 = service.watchTimer$(milliseconds);
            expect(observable1).toBe(observable2);
        });

        it("should create a new observable for a different interval", () => {
            const milliseconds1 = 1000;
            const milliseconds2 = 2000;
            const observable1 = service.watchTimer$(milliseconds1);
            const observable2 = service.watchTimer$(milliseconds2);
            expect(observable1).not.toBe(observable2);
        });
    });

    describe("timer zero interval", () => {
        it("should accept zero as a valid interval", async () => {
            const emittedValue = await firstValueFrom(service.watchTimer$(0).pipe(take(1)));
            expect(emittedValue).toBe(0);
        });
    });
});
