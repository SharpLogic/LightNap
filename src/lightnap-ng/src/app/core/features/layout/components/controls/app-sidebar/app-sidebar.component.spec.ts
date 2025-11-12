import { provideZonelessChangeDetection, signal } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { MenuService } from "@core/features/layout/services/menu.service";
import { MenuItem } from "primeng/api";
import { AppSidebarComponent } from "./app-sidebar.component";

describe("AppSidebarComponent", () => {
  let mockMenuService: jasmine.SpyObj<MenuService>;
  let menuItemsSignal: ReturnType<typeof signal<MenuItem[]>>;

  beforeEach(async () => {
    menuItemsSignal = signal<MenuItem[]>([
      { label: "Home", icon: "pi pi-home", routerLink: "/home" },
      { label: "Profile", icon: "pi pi-user", routerLink: "/profile" },
    ]);

    mockMenuService = jasmine.createSpyObj("MenuService", [], {
      menuItems: menuItemsSignal,
    });

    await TestBed.configureTestingModule({
      imports: [AppSidebarComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideRouter([]),
        { provide: MenuService, useValue: mockMenuService },
      ],
    }).compileComponents();
  });

  it("should create", () => {
    const fixture = TestBed.createComponent(AppSidebarComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it("should have menuItems from MenuService", () => {
    const fixture = TestBed.createComponent(AppSidebarComponent);
    const component = fixture.componentInstance;
    expect(component.menuItems()).toEqual([
      { label: "Home", icon: "pi pi-home", routerLink: "/home" },
      { label: "Profile", icon: "pi pi-user", routerLink: "/profile" },
    ]);
  });

  it("should render sidebar container", () => {
    const fixture = TestBed.createComponent(AppSidebarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const sidebar = compiled.querySelector(".layout-sidebar");
    expect(sidebar).toBeTruthy();
  });

  it("should have menu-container class", () => {
    const fixture = TestBed.createComponent(AppSidebarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const sidebar = compiled.querySelector(".menu-container");
    expect(sidebar).toBeTruthy();
  });

  it("should render PrimeNG PanelMenu", () => {
    const fixture = TestBed.createComponent(AppSidebarComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const panelMenu = compiled.querySelector("p-panelmenu");
    expect(panelMenu).toBeTruthy();
  });

  it("should update when menuItems signal changes", () => {
    const fixture = TestBed.createComponent(AppSidebarComponent);
    const component = fixture.componentInstance;

    const newItems: MenuItem[] = [
      { label: "Dashboard", icon: "pi pi-chart-line", routerLink: "/dashboard" },
      { label: "Settings", icon: "pi pi-cog", routerLink: "/settings" },
      { label: "Logout", icon: "pi pi-sign-out", routerLink: "/logout" },
    ];

    menuItemsSignal.set(newItems);

    expect(component.menuItems()).toEqual(newItems);
    expect(component.menuItems().length).toBe(3);
  });

  it("should handle empty menu items", () => {
    menuItemsSignal.set([]);

    const fixture = TestBed.createComponent(AppSidebarComponent);
    const component = fixture.componentInstance;

    expect(component.menuItems()).toEqual([]);
    expect(component.menuItems().length).toBe(0);
  });
});
