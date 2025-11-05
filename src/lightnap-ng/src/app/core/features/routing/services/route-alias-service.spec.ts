import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { Router } from "@angular/router";
import { AppRoute, RouteAlias } from "../models";
import { RouteAliasService } from "./route-alias-service";

describe("RouteAliasService", () => {
  let service: RouteAliasService;
  let router: Router;

  const mockRoutes: AppRoute[] = [
    { path: "home", data: { alias: "user-home" } },
    { path: "about", data: { alias: "about" } },
    {
      path: "admin",
      data: { alias: "admin-home" },
      children: [
        { path: "user/:userId", data: { alias: "admin-user" } },
        { path: "role/:roleId", data: { alias: "admin-role" } },
      ],
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        RouteAliasService,
        { provide: Router, useValue: { config: mockRoutes, navigate: jasmine.createSpy("navigate") } },
      ],
    });

    service = TestBed.inject(RouteAliasService);
    router = TestBed.inject(Router);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should build route map correctly", () => {
    expect(service.getRoute("user-home")).toEqual(["/", "home"]);
    expect(service.getRoute("about")).toEqual(["/", "about"]);
    expect(service.getRoute("admin-home")).toEqual(["/", "admin"]);
    expect(service.getRoute("admin-user", "user-id")).toEqual(["/", "admin", "user", "user-id"]);
    expect(service.getRoute("admin-role", "Administrator")).toEqual(["/", "admin", "role", "Administrator"]);
  });

  it("should navigate to the correct route", () => {
    service.navigate("user-home");
    expect(router.navigate).toHaveBeenCalledWith(["/", "home"]);
  });

  it("should navigate with replaceUrl option", () => {
    service.navigateWithExtras("about", undefined, { replaceUrl: true });
    expect(router.navigate).toHaveBeenCalledWith(["/", "about"], { replaceUrl: true });
  });

  it("should throw error for duplicate alias", () => {
    const duplicateRoutes: AppRoute[] = [
      { path: "duplicate", data: { alias: "user-home" } },
      { path: "duplicate2", data: { alias: "user-home" } },
    ];

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: Router, useValue: { config: duplicateRoutes, navigate: jasmine.createSpy("navigate") } },
        RouteAliasService,
      ],
    });

    expect(() => {
      TestBed.inject(RouteAliasService);
    }).toThrowError(/Duplicate route for 'user-home'/);
  });

  it("should throw error for unexpected route alias", () => {
    expect(() => service.getRoute("non-existent-alias" as RouteAlias)).toThrowError(/Unexpected route alias 'non-existent-alias'/);
  });

  it("should append value to dynamic routes", () => {
    expect(service.getRoute("admin-user", "123")).toEqual(["/", "admin", "user", "123"]);
    expect(service.getRoute("admin-role", "456")).toEqual(["/", "admin", "role", "456"]);
  });
});
