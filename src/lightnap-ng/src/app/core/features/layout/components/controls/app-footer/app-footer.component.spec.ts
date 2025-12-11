import { ComponentFixture, TestBed } from "@angular/core/testing";
import { beforeEach, describe, expect, it } from "vitest";
import { AppFooterComponent } from "./app-footer.component";

describe("AppFooterComponent", () => {
    let fixture: ComponentFixture<AppFooterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AppFooterComponent],
        }).compileComponents();
        fixture = TestBed.createComponent(AppFooterComponent);
    });

    it("should create", () => {
        const fixture = TestBed.createComponent(AppFooterComponent);
        const component = fixture.componentInstance;
        expect(component).toBeTruthy();
    });

    it("should render footer container", () => {
        const fixture = TestBed.createComponent(AppFooterComponent);
        fixture.detectChanges();
        const compiled = fixture.nativeElement as HTMLElement;
        const footer = compiled.querySelector(".layout-footer");
        expect(footer).toBeTruthy();
    });

    it("should render GitHub link", () => {
        const fixture = TestBed.createComponent(AppFooterComponent);
        fixture.detectChanges();
        const compiled = fixture.nativeElement as HTMLElement;
        const link = compiled.querySelector("a");
        expect(link).toBeTruthy();
        expect(link?.href).toContain("github.com/sharplogic/lightnap");
    });

    it("should have correct link attributes", () => {
        const fixture = TestBed.createComponent(AppFooterComponent);
        fixture.detectChanges();
        const compiled = fixture.nativeElement as HTMLElement;
        const link = compiled.querySelector("a");
        expect(link?.rel).toBe("noopener noreferrer");
        expect(link?.target).toBe("_blank");
    });

    it("should display link text", () => {
        const fixture = TestBed.createComponent(AppFooterComponent);
        fixture.detectChanges();
        const compiled = fixture.nativeElement as HTMLElement;
        const link = compiled.querySelector("a");
        expect(link?.textContent?.trim()).toBe("LightNap on GitHub");
    });
});
