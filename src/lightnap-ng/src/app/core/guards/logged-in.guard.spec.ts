import { TestBed } from "@angular/core/testing";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { loggedInGuard } from "./logged-in.guard";
import { MockIdentityService, MockRouteAliasService, createMockActivatedRouteSnapshot, createMockRouterStateSnapshot } from "@testing";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { firstValueFrom } from "rxjs";
import { describe, it, beforeEach, vi } from "vitest";

describe("loggedInGuard", () => {
  let mockIdentity: MockIdentityService;
  let mockRouteAlias: MockRouteAliasService;
  let route: ActivatedRouteSnapshot;
  let state: RouterStateSnapshot;

  beforeEach(() => {
    mockIdentity = new MockIdentityService();
    mockRouteAlias = new MockRouteAliasService();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: IdentityService, useValue: mockIdentity },
        { provide: RouteAliasService, useValue: mockRouteAlias },
        { provide: Router, useValue: { createUrlTree: vi.fn() } },
      ],
    });

    route = createMockActivatedRouteSnapshot();
    state = createMockRouterStateSnapshot("/protected");
  });

  it("should allow access when user is logged in", async () => {
    mockIdentity.setLoggedIn("token", ["User"]);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(loggedInGuard(route, state));
    });

    expect(result).toBe(true);
  });

  it("should deny access when user is not logged in", async () => {
    mockIdentity.setLoggedOut();
    mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "login"]);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(loggedInGuard(route, state));
    });

    expect(result).not.toBe(true);
    expect(mockRouteAlias.getRoute).toHaveBeenCalledWith("login");
  });

  it("should set redirect URL when denying access", async () => {
    mockIdentity.setLoggedOut();
    mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "login"]);
    const setRedirectUrlSpy = vi.spyOn(mockIdentity as any, "setRedirectUrl");

    await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(loggedInGuard(route, state));
    });

    expect(setRedirectUrlSpy).toHaveBeenCalledWith("/protected");
  });

  it("should handle different URLs", async () => {
    mockIdentity.setLoggedOut();
    mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "login"]);
    const setRedirectUrlSpy = vi.spyOn(mockIdentity as any, "setRedirectUrl");
    const customState = createMockRouterStateSnapshot("/admin/settings");

    await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(loggedInGuard(route, customState));
    });

    expect(setRedirectUrlSpy).toHaveBeenCalledWith("/admin/settings");
  });
});
