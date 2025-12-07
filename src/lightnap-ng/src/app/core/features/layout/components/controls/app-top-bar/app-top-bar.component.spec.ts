import { provideHttpClient } from "@angular/common/http";
import { provideZonelessChangeDetection, signal } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { LayoutConfigDto } from "@core/backend-api";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { AppTopBarComponent } from "./app-top-bar.component";
import { MessageService } from "primeng/api";

describe("AppTopBarComponent", () => {
  let mockLayoutService: any;
  let layoutConfigSignal: ReturnType<typeof signal<LayoutConfigDto>>;

  beforeEach(async () => {
    const defaultConfig: LayoutConfigDto = {
      darkTheme: false,
      menuMode: "static",
      preset: "Aura",
      primary: "blue",
      surface: "slate",
    };

    layoutConfigSignal = signal<LayoutConfigDto>(defaultConfig);

    mockLayoutService = {
      layoutConfig: layoutConfigSignal,
      isDarkTheme: signal(false),
      appName: "LightNap Test",
      toggleMenu: jasmine.createSpy("toggleMenu"),
      presets: { Aura: {}, Lara: {}, Nora: {} },
      surfaces: [],
      colors: [],
      primaryColors: signal([]),
      preset: signal("Aura"),
      primaryPalette: signal("blue"),
      surfacePalette: signal("slate"),
      menuMode: signal("static"),
      menuModeOptions: [],
    };

    await TestBed.configureTestingModule({
      imports: [AppTopBarComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideHttpClient(),
        provideRouter([]),
        { provide: LayoutService, useValue: mockLayoutService },
        { provide: RouteAliasService, useClass: MockRouteAliasService },
        MessageService,
      ],
    }).compileComponents();
  });

  it("should create", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it("should have layoutService injected", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    const component = fixture.componentInstance;
    expect(component.layoutService).toBeTruthy();
  });

  it("should render topbar container", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const topbar = compiled.querySelector(".layout-topbar");
    expect(topbar).toBeTruthy();
  });

  it("should render logo container", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const logoContainer = compiled.querySelector(".layout-topbar-logo-container");
    expect(logoContainer).toBeTruthy();
  });

  it("should render menu toggle button", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const menuButton = compiled.querySelector(".layout-menu-button");
    expect(menuButton).toBeTruthy();
    expect(menuButton?.querySelector("i.pi-bars")).toBeTruthy();
  });

  it("should call toggleMenu when menu button clicked", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const menuButton = compiled.querySelector(".layout-menu-button") as HTMLButtonElement;

    menuButton.click();

    expect(mockLayoutService.toggleMenu).toHaveBeenCalled();
  });

  it("should render app logo with light theme", () => {
    mockLayoutService.isDarkTheme.set(false);
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const logo = compiled.querySelector("img") as HTMLImageElement;

    expect(logo).toBeTruthy();
    expect(logo.src).toContain("logo-light.png");
    expect(logo.alt).toBe("LightNap Logo");
  });

  it("should render app logo with dark theme", () => {
    mockLayoutService.isDarkTheme.set(true);
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const logo = compiled.querySelector("img") as HTMLImageElement;

    expect(logo).toBeTruthy();
    expect(logo.src).toContain("logo-dark.png");
  });

  it("should display app name", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const appName = compiled.querySelector(".layout-topbar-logo-container span");

    expect(appName?.textContent).toBe("LightNap Test");
  });

  it("should render dark mode toggle button", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const darkModeButton = compiled.querySelector(".layout-config-menu button");

    expect(darkModeButton).toBeTruthy();
  });

  it("should show moon icon when dark theme active", () => {
    mockLayoutService.isDarkTheme.set(true);
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const icon = compiled.querySelector(".layout-config-menu button i");

    expect(icon?.classList.contains("pi-moon")).toBe(true);
  });

  it("should show sun icon when light theme active", () => {
    mockLayoutService.isDarkTheme.set(false);
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const icon = compiled.querySelector(".layout-config-menu button i");

    expect(icon?.classList.contains("pi-sun")).toBe(true);
  });

  it("should toggle dark mode when button clicked", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    const initialDarkTheme = layoutConfigSignal().darkTheme;
    component.toggleDarkMode();

    expect(layoutConfigSignal().darkTheme).toBe(!initialDarkTheme);
  });

  it("should render theme configurator", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const configurator = compiled.querySelector("ln-app-configurator");

    expect(configurator).toBeTruthy();
  });

  it("should render palette button", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const paletteButton = compiled.querySelector("i.pi-palette");

    expect(paletteButton).toBeTruthy();
  });

  it("should render topbar actions", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const actions = compiled.querySelector(".layout-topbar-actions");

    expect(actions).toBeTruthy();
  });

  it("should render notifications button", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const notificationsButton = compiled.querySelector("ln-notifications-button");

    expect(notificationsButton).toBeTruthy();
  });

  it("should render profile button in menu", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const topbarMenu = compiled.querySelector(".layout-topbar-menu");

    expect(topbarMenu).toBeTruthy();
    expect(topbarMenu?.textContent).toContain("Profile");
  });

  it("should render profile icon in menu", () => {
    const fixture = TestBed.createComponent(AppTopBarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const profileIcon = compiled.querySelector('.layout-topbar-menu i.pi-user');

    expect(profileIcon).toBeTruthy();
  });
});
