import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { BehaviorSubject } from "rxjs";
import { BreadcrumbComponent } from "./breadcrumb.component";
import { BreadcrumbService } from "../../../services/breadcrumb.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { MockRouteAliasService } from "@testing/mocks/mock-route-alias.service";
import { MenuItem } from "primeng/api";
import { describe, beforeEach, expect, it } from "vitest";

describe("BreadcrumbComponent", () => {
  let component: BreadcrumbComponent;
  let fixture: ComponentFixture<BreadcrumbComponent>;
  let mockBreadcrumbService: any;
  let breadcrumbsSubject: BehaviorSubject<MenuItem[]>;

  beforeEach(async () => {
    breadcrumbsSubject = new BehaviorSubject<MenuItem[]>([]);

    mockBreadcrumbService = {
      breadcrumbs$: breadcrumbsSubject.asObservable(),
    };

    await TestBed.configureTestingModule({
      imports: [BreadcrumbComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideRouter([]),
        { provide: BreadcrumbService, useValue: mockBreadcrumbService },
        { provide: RouteAliasService, useClass: MockRouteAliasService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(BreadcrumbComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should have home item with icon", () => {
    expect(component.homeItem).toBeTruthy();
    expect(component.homeItem.icon).toBe("pi pi-home");
    expect(component.homeItem.routerLink).toEqual(["/", "user-home"]);
  });

  it("should render p-breadcrumb component", () => {
    const breadcrumb = fixture.nativeElement.querySelector("p-breadcrumb");
    expect(breadcrumb).toBeTruthy();
  });

  it("should start with empty breadcrumb items", () => {
    expect(component.breadcrumbItems()).toEqual([]);
  });

  it("should update breadcrumb items when service emits", () => {
    const newItems: MenuItem[] = [
      { label: "Home", routerLink: ["/"] },
      { label: "Dashboard", routerLink: ["/dashboard"] },
    ];

    breadcrumbsSubject.next(newItems);
    fixture.detectChanges();

    expect(component.breadcrumbItems()).toEqual(newItems);
  });

  it("should render breadcrumb items with labels", () => {
    const items: MenuItem[] = [{ label: "Admin" }, { label: "Users" }];

    breadcrumbsSubject.next(items);
    fixture.detectChanges();

    // PrimeNG breadcrumb renders items - check that component received them
    expect(component.breadcrumbItems()).toEqual(items);
  });

  it("should handle breadcrumb items with icons", () => {
    const items: MenuItem[] = [{ label: "Settings", icon: "pi pi-cog" }];

    breadcrumbsSubject.next(items);
    fixture.detectChanges();

    expect(component.breadcrumbItems()?.[0]?.icon).toBe("pi pi-cog");
  });

  it("should handle breadcrumb items with routerLink", () => {
    const items: MenuItem[] = [{ label: "Profile", routerLink: ["/profile"] }];

    breadcrumbsSubject.next(items);
    fixture.detectChanges();

    expect(component.breadcrumbItems()?.[0]?.routerLink).toEqual(["/profile"]);
  });

  it("should handle multiple breadcrumb items", () => {
    const items: MenuItem[] = [{ label: "Admin", routerLink: ["/admin"] }, { label: "Users", routerLink: ["/admin", "users"] }, { label: "Edit" }];

    breadcrumbsSubject.next(items);
    fixture.detectChanges();

    expect(component.breadcrumbItems()?.length).toBe(3);
  });

  it("should have asMenuItem helper", () => {
    expect(component.asMenuItem).toBeDefined();
    const item: MenuItem = { label: "Test" };
    expect(component.asMenuItem(item)).toBe(item);
  });

  it("should apply CSS classes to breadcrumb", () => {
    const breadcrumb = fixture.nativeElement.querySelector("p-breadcrumb");
    expect(breadcrumb.classList.contains("max-w-full")).toBe(true);
    expect(breadcrumb.classList.contains("mb-2")).toBe(true);
    expect(breadcrumb.classList.contains("block")).toBe(true);
  });
});
