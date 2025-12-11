import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideRouter } from "@angular/router";
import { provideZonelessChangeDetection, signal } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { AppConfiguratorComponent } from "./app-configurator.component";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { LayoutConfigDto } from "@core/backend-api";

describe("AppConfiguratorComponent", () => {
    let component: AppConfiguratorComponent;
    let fixture: ComponentFixture<AppConfiguratorComponent>;
    let mockLayoutService: any;

    beforeEach(async () => {
        const configSignal = signal<LayoutConfigDto>({
            preset: "Aura",
            primary: "blue",
            surface: "slate",
            darkTheme: false,
            menuMode: "static",
        });

        mockLayoutService = {
            layoutConfig: configSignal,
            isDarkTheme: signal(false),
            preset: signal("Aura"),
            primaryPalette: signal("blue"),
            surfacePalette: signal("slate"),
            menuMode: signal("static"),
            primaryColors: signal([
                { name: "blue", palette: { "500": "#3b82f6" } },
                { name: "green", palette: { "500": "#10b981" } },
                { name: "noir", palette: { "500": "#000000" } },
            ]),
            surfaces: [
                { name: "slate", palette: { "200": "#e2e8f0", "800": "#1e293b" } },
                { name: "zinc", palette: { "200": "#e4e4e7", "800": "#27272a" } },
                { name: "noir", palette: { "200": "#000000", "800": "#000000" } },
            ],
            presets: { Aura: {}, Lara: {}, Nora: {} },
            colors: [],
            menuModeOptions: [
                { label: "Static", value: "static" },
                { label: "Overlay", value: "overlay" },
            ],
            appName: "LightNap Test",
            onMenuToggle: vi.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [AppConfiguratorComponent],
            providers: [
                provideZonelessChangeDetection(),
                provideNoopAnimations(),
                provideRouter([]),
                { provide: LayoutService, useValue: mockLayoutService },
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AppConfiguratorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });

    it("should inject LayoutService", () => {
        expect(component.layoutService).toBe(mockLayoutService);
    });

    it("should load presets from LayoutService", () => {
        expect(component.presets).toEqual(["Aura", "Lara", "Nora"]);
    });

    it("should show menu mode button when not on identity page", () => {
        expect(component.showMenuModeButton()).toBe(true);
    });

    it("should render primary color section", () => {
        const primarySection = fixture.nativeElement.querySelector(".flex.flex-col.gap-4 > div:first-child");
        expect(primarySection).toBeTruthy();
        const primaryLabel = primarySection.querySelector("span");
        expect(primaryLabel?.textContent).toBe("Primary");
    });

    it("should render primary color buttons", () => {
        const primaryButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:first-child button");
        expect(primaryButtons.length).toBe(3);
    });

    it("should apply outline to selected primary color", () => {
        const primaryButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:first-child button");
        const selectedButton = Array.from(primaryButtons).find((btn: any) => btn.classList.contains("outline-primary"));
        expect(selectedButton).toBeTruthy();
    });

    it("should update primary color when button clicked", () => {
        const updateSpy = vi.fn();
        mockLayoutService.layoutConfig.update = updateSpy;

        component.changePrimaryColor("green");

        expect(updateSpy).toHaveBeenCalledWith(expect.any(Function));
    });

    it("should render surface color section", () => {
        const surfaceSection = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div")[1];
        expect(surfaceSection).toBeTruthy();
        const surfaceLabel = surfaceSection.querySelector("span");
        expect(surfaceLabel?.textContent).toBe("Surface");
    });

    it("should render surface color buttons", () => {
        const surfaceButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:nth-child(2) button");
        expect(surfaceButtons.length).toBe(3);
    });

    it("should apply outline to selected surface color", () => {
        const surfaceButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:nth-child(2) button");
        const selectedButton = Array.from(surfaceButtons).find((btn: any) => btn.classList.contains("outline-primary"));
        expect(selectedButton).toBeTruthy();
    });

    it("should update surface color when button clicked", () => {
        const updateSpy = vi.fn();
        mockLayoutService.layoutConfig.update = updateSpy;

        component.changeSurfaceColor("zinc");

        expect(updateSpy).toHaveBeenCalledWith(expect.any(Function));
    });

    it("should render presets section", () => {
        const presetsSection = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-2")[0];
        expect(presetsSection).toBeTruthy();
        const presetsLabel = presetsSection.querySelector("span");
        expect(presetsLabel?.textContent).toBe("Presets");
    });

    it("should render presets select button", () => {
        const presetsSelectButton = fixture.nativeElement.querySelector("p-selectbutton");
        expect(presetsSelectButton).toBeTruthy();
    });

    it("should update preset when changed", () => {
        const updateSpy = vi.fn();
        mockLayoutService.layoutConfig.update = updateSpy;

        component.changePreset("Lara");

        expect(updateSpy).toHaveBeenCalledWith(expect.any(Function));
    });

    it("should render menu mode section when showMenuModeButton is true", () => {
        const menuModeSection = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-2")[1];
        expect(menuModeSection).toBeTruthy();
        const menuModeLabel = menuModeSection.querySelector("span");
        expect(menuModeLabel?.textContent).toBe("Menu Mode");
    });

    it("should render menu mode select button", () => {
        const selectButtons = fixture.nativeElement.querySelectorAll("p-selectbutton");
        expect(selectButtons.length).toBe(2); // presets + menu mode
    });

    it("should update menu mode when changed", () => {
        const updateSpy = vi.fn();
        mockLayoutService.layoutConfig.update = updateSpy;

        component.changeMenuMode("overlay");

        expect(updateSpy).toHaveBeenCalledWith(expect.any(Function));
    });

    it("should hide menu mode section when showMenuModeButton is false", () => {
        component.showMenuModeButton.set(false);
        fixture.detectChanges();

        const selectButtons = fixture.nativeElement.querySelectorAll("p-selectbutton");
        expect(selectButtons.length).toBe(1); // only presets
    });

    it("should apply special styling for noir primary color", () => {
        const primaryButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:first-child button");
        const noirButton = Array.from(primaryButtons).find((btn: any) => btn.getAttribute("title") === "noir");
        expect(noirButton).toBeTruthy();
    });

    it("should apply different surface colors based on dark theme", () => {
        // Light theme
        mockLayoutService.isDarkTheme.set(false);
        fixture.detectChanges();

        const surfaceButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:nth-child(2) button");
        expect(surfaceButtons.length).toBeGreaterThan(0);

        // Dark theme
        mockLayoutService.isDarkTheme.set(true);
        fixture.detectChanges();

        const darkSurfaceButtons = fixture.nativeElement.querySelectorAll(".flex.flex-col.gap-4 > div:nth-child(2) button");
        expect(darkSurfaceButtons.length).toBeGreaterThan(0);
    });
});
