import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection, signal } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { of } from "rxjs";
import { PublicLayoutComponent } from "./public-layout.component";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { LayoutConfigDto } from "@core/backend-api";

describe("PublicLayoutComponent", () => {
  let component: PublicLayoutComponent;
  let fixture: ComponentFixture<PublicLayoutComponent>;
  let mockLayoutService: any;
  let mockIdentityService: any;

  beforeEach(async () => {
    const layoutConfigSignal = signal<LayoutConfigDto>({
      preset: "Aura",
      primary: "blue",
      surface: "slate",
      darkTheme: false,
      menuMode: "static",
    });

    mockLayoutService = {
      layoutConfig: layoutConfigSignal,
      isDarkTheme: signal(false),
      appName: "LightNap Test",
    };

    mockIdentityService = {
      watchLoggedIn$: jasmine.createSpy("watchLoggedIn$").and.returnValue(of(false)),
      logOut: jasmine.createSpy("logOut").and.returnValue(of(void 0)),
    };

    await TestBed.configureTestingModule({
      imports: [PublicLayoutComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideRouter([]),
        { provide: LayoutService, useValue: mockLayoutService },
        { provide: IdentityService, useValue: mockIdentityService },
        { provide: RouteAliasService, useClass: MockRouteAliasService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PublicLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should inject LayoutService", () => {
    expect(component.layoutService).toBe(mockLayoutService);
  });

  it("should inject IdentityService", () => {
    expect(component.identityService).toBe(mockIdentityService);
  });

  it("should watch logged in status on init", () => {
    expect(mockIdentityService.watchLoggedIn$).toHaveBeenCalled();
  });

  it("should render container with min-h-screen", () => {
    const container = fixture.nativeElement.querySelector(".min-h-screen");
    expect(container).toBeTruthy();
  });

  it("should render header with background and shadow", () => {
    const header = fixture.nativeElement.querySelector(".bg-surface-0.dark\\:bg-surface-900.mb-4.shadow.h-20");
    expect(header).toBeTruthy();
  });

  it("should render logo link", () => {
    const logoLink = fixture.nativeElement.querySelector("a.flex.items-center");
    expect(logoLink).toBeTruthy();
  });

  it("should render logo image with light theme", () => {
    mockLayoutService.isDarkTheme.set(false);
    fixture.detectChanges();

    const logo = fixture.nativeElement.querySelector("img[alt*='Logo']");
    expect(logo).toBeTruthy();
    expect(logo.src).toContain("logo-light.png");
  });

  it("should render logo image with dark theme", () => {
    mockLayoutService.isDarkTheme.set(true);
    fixture.detectChanges();

    const logo = fixture.nativeElement.querySelector("img[alt*='Logo']");
    expect(logo).toBeTruthy();
    expect(logo.src).toContain("logo-dark.png");
  });

  it("should render app name", () => {
    const appName = fixture.nativeElement.querySelector(".text-2xl");
    expect(appName).toBeTruthy();
    expect(appName.textContent).toContain("LightNap Test");
  });

  it("should render router-outlet", () => {
    const routerOutlet = fixture.nativeElement.querySelector("router-outlet");
    expect(routerOutlet).toBeTruthy();
  });

  it("should render login and register buttons when not logged in", () => {
    mockIdentityService.watchLoggedIn$.and.returnValue(of(false));
    fixture = TestBed.createComponent(PublicLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    const buttons = fixture.nativeElement.querySelectorAll("p-button");
    const buttonLabels = Array.from(buttons).map((btn: any) => btn.getAttribute("label"));

    expect(buttonLabels).toContain("Login");
    expect(buttonLabels).toContain("Register");
  });

  it("should render app and logout buttons when logged in", () => {
    mockIdentityService.watchLoggedIn$.and.returnValue(of(true));
    fixture = TestBed.createComponent(PublicLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    const buttons = fixture.nativeElement.querySelectorAll("p-button");
    const buttonLabels = Array.from(buttons).map((btn: any) => btn.getAttribute("label"));

    expect(buttonLabels).toContain("App");
    expect(buttonLabels).toContain("Logout");
  });

  it("should call logOut when logout button clicked", () => {
    mockIdentityService.watchLoggedIn$.and.returnValue(of(true));
    fixture = TestBed.createComponent(PublicLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    component.logOutClicked();

    expect(mockIdentityService.logOut).toHaveBeenCalled();
  });

  it("should have logOutClicked method", () => {
    expect(component.logOutClicked).toBeDefined();
  });

  it("should render main content container", () => {
    const contentContainer = fixture.nativeElement.querySelector(".flex.flex-column.justify-center > .container");
    expect(contentContainer).toBeTruthy();
  });

  it("should render header container with flex layout", () => {
    const headerContainer = fixture.nativeElement.querySelector(".container.h-full.px-4.flex.items-center.justify-between");
    expect(headerContainer).toBeTruthy();
  });

  it("should have logo with hover effect", () => {
    const logoLink = fixture.nativeElement.querySelector("a.hover\\:no-underline");
    expect(logoLink).toBeTruthy();
  });

  it("should render buttons with gap", () => {
    const buttonContainer = fixture.nativeElement.querySelector(".flex.items-center.justify-between.gap-2");
    expect(buttonContainer).toBeTruthy();
  });
});
