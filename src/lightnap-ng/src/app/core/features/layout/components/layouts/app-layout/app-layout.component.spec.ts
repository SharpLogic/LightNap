import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection, Renderer2, signal } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter, Router } from "@angular/router";
import { provideLocationMocks } from "@angular/common/testing";
import { Subject } from "rxjs";
import { AppLayoutComponent } from "./app-layout.component";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { LayoutConfigDto } from "@core/backend-api";
import { provideHttpClient } from "@angular/common/http";
import { MenuService } from "@core/features/layout/services/menu.service";
import { BreadcrumbService } from "@core/features/layout/services/breadcrumb.service";
import { MenuItem, MessageService } from "primeng/api";

describe("AppLayoutComponent", () => {
  let component: AppLayoutComponent;
  let fixture: ComponentFixture<AppLayoutComponent>;
  let mockLayoutService: any;
  let overlayOpenSubject: Subject<void>;
  let mockRouter: any;
  let mockMenuService: any;
  let mockBreadcrumbService: any;

  beforeEach(async () => {
    overlayOpenSubject = new Subject<void>();

    const layoutConfigSignal = signal<LayoutConfigDto>({
      preset: "Aura",
      primary: "blue",
      surface: "slate",
      darkTheme: false,
      menuMode: "static",
    });

    const layoutStateSignal = signal({
      staticMenuDesktopInactive: false,
      overlayMenuActive: false,
      staticMenuMobileActive: false,
      menuHoverActive: false,
    });

    mockLayoutService = {
      layoutConfig: layoutConfigSignal,
      layoutState: layoutStateSignal,
      isDarkTheme: signal(false),
      preset: signal("Aura"),
      primaryPalette: signal("blue"),
      surfacePalette: signal("slate"),
      menuMode: signal("static"),
      primaryColors: signal([]),
      surfaces: [],
      colors: [],
      presets: { Aura: {}, Lara: {}, Nora: {} },
      menuModeOptions: [],
      appName: "LightNap Test",
      onMenuToggle: jasmine.createSpy("onMenuToggle"),
      overlayOpen$: overlayOpenSubject.asObservable(),
    };

    mockRouter = {
      events: new Subject(),
      url: "/dashboard",
    };

    mockMenuService = {
      menuItems: signal<MenuItem[]>([]),
    };

    mockBreadcrumbService = {
      breadcrumbs$: new Subject<MenuItem[]>().asObservable(),
    };

    await TestBed.configureTestingModule({
      imports: [AppLayoutComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideHttpClient(),
        provideRouter([]),
        provideLocationMocks(),
        { provide: LayoutService, useValue: mockLayoutService },
        { provide: RouteAliasService, useClass: MockRouteAliasService },
        { provide: MenuService, useValue: mockMenuService },
        { provide: BreadcrumbService, useValue: mockBreadcrumbService },
        MessageService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AppLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should inject LayoutService", () => {
    expect(component.layoutService).toBe(mockLayoutService);
  });

  it("should inject Renderer2", () => {
    expect(component.renderer).toBeTruthy();
  });

  it("should inject Router", () => {
    expect(component.router).toBeTruthy();
  });

  it("should render layout wrapper", () => {
    const layoutWrapper = fixture.nativeElement.querySelector(".layout-wrapper");
    expect(layoutWrapper).toBeTruthy();
  });

  it("should render app-top-bar", () => {
    const topBar = fixture.nativeElement.querySelector("ln-app-top-bar");
    expect(topBar).toBeTruthy();
  });

  it("should render app-sidebar", () => {
    const sidebar = fixture.nativeElement.querySelector("ln-app-sidebar");
    expect(sidebar).toBeTruthy();
  });

  it("should render breadcrumb", () => {
    const breadcrumb = fixture.nativeElement.querySelector("ln-breadcrumb");
    expect(breadcrumb).toBeTruthy();
  });

  it("should render router-outlet", () => {
    const routerOutlet = fixture.nativeElement.querySelector("router-outlet");
    expect(routerOutlet).toBeTruthy();
  });

  it("should render app-footer", () => {
    const footer = fixture.nativeElement.querySelector("ln-app-footer");
    expect(footer).toBeTruthy();
  });

  it("should render layout-sidebar container", () => {
    const sidebar = fixture.nativeElement.querySelector(".layout-sidebar");
    expect(sidebar).toBeTruthy();
  });

  it("should render layout-main-container", () => {
    const mainContainer = fixture.nativeElement.querySelector(".layout-main-container");
    expect(mainContainer).toBeTruthy();
  });

  it("should render layout-mask", () => {
    const mask = fixture.nativeElement.querySelector(".layout-mask");
    expect(mask).toBeTruthy();
  });

  it("should have containerClass getter", () => {
    const classes = component.containerClass;
    expect(classes).toBeDefined();
  });

  it("should apply layout-static class when menuMode is static", () => {
    const classes = component.containerClass;
    expect(classes["layout-static"]).toBe(true);
  });

  it("should apply layout-overlay class when menuMode is overlay", () => {
    mockLayoutService.layoutConfig.set({
      preset: "Aura",
      primary: "blue",
      surface: "slate",
      darkTheme: false,
      menuMode: "overlay",
    });

    const classes = component.containerClass;
    expect(classes["layout-overlay"]).toBe(true);
  });

  it("should apply layout-overlay-active class when overlay menu is active", () => {
    mockLayoutService.layoutState.set({
      staticMenuDesktopInactive: false,
      overlayMenuActive: true,
      staticMenuMobileActive: false,
      menuHoverActive: false,
    });

    const classes = component.containerClass;
    expect(classes["layout-overlay-active"]).toBe(true);
  });

  it("should apply layout-mobile-active class when mobile menu is active", () => {
    mockLayoutService.layoutState.set({
      staticMenuDesktopInactive: false,
      overlayMenuActive: false,
      staticMenuMobileActive: true,
      menuHoverActive: false,
    });

    const classes = component.containerClass;
    expect(classes["layout-mobile-active"]).toBe(true);
  });

  it("should apply layout-static-inactive class when static menu is inactive on desktop", () => {
    mockLayoutService.layoutState.set({
      staticMenuDesktopInactive: true,
      overlayMenuActive: false,
      staticMenuMobileActive: false,
      menuHoverActive: false,
    });

    const classes = component.containerClass;
    expect(classes["layout-static-inactive"]).toBe(true);
  });

  it("should have hideMenu method", () => {
    expect(component.hideMenu).toBeDefined();
  });

  it("should update layout state when hideMenu is called", () => {
    const updateSpy = jasmine.createSpy("update");
    mockLayoutService.layoutState.update = updateSpy;

    component.hideMenu();

    expect(updateSpy).toHaveBeenCalledWith(jasmine.any(Function));
  });

  it("should have isOutsideClicked method", () => {
    expect(component.isOutsideClicked).toBeDefined();
  });

  it("should have blockBodyScroll method", () => {
    expect(component.blockBodyScroll).toBeDefined();
  });

  it("should add blocked-scroll class to body when blockBodyScroll is called", () => {
    component.blockBodyScroll();
    expect(document.body.classList.contains("blocked-scroll")).toBe(true);

    // Cleanup
    component.unblockBodyScroll();
  });

  it("should have unblockBodyScroll method", () => {
    expect(component.unblockBodyScroll).toBeDefined();
  });

  it("should remove blocked-scroll class from body when unblockBodyScroll is called", () => {
    document.body.classList.add("blocked-scroll");

    component.unblockBodyScroll();

    expect(document.body.classList.contains("blocked-scroll")).toBe(false);
  });

  it("should cleanup outside click listener on destroy", () => {
    component.menuOutsideClickListener = jasmine.createSpy("listener");

    component.ngOnDestroy();

    expect(component.menuOutsideClickListener).toHaveBeenCalled();
  });
});
