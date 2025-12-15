import { TestBed } from "@angular/core/testing";
import { ActivatedRouteSnapshot } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { roleGuard } from "./role.guard";
import { MockIdentityService, MockRouteAliasService, createMockActivatedRouteSnapshot } from "@testing";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { RoleName } from "@core/backend-api";
import { firstValueFrom, of } from "rxjs";

describe("roleGuard", () => {
  let mockIdentity: MockIdentityService;
  let mockRouteAlias: MockRouteAliasService;
  let route: ActivatedRouteSnapshot;

  beforeEach(() => {
    mockIdentity = new MockIdentityService();
    mockRouteAlias = new MockRouteAliasService();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: IdentityService, useValue: mockIdentity },
        { provide: RouteAliasService, useValue: mockRouteAlias },
      ],
    });

    route = createMockActivatedRouteSnapshot();
  });

  it("should allow access when user has single required role", async () => {
    mockIdentity.setLoggedIn("token", ["Administrator"]);
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

    const guard = roleGuard("Administrator" as RoleName);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });

  it("should allow access when user has any of multiple required roles", async () => {
    mockIdentity.setLoggedIn("token", ["Editor"]);
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator", "Editor"]));

    const guard = roleGuard(["Administrator", "Editor"] as RoleName[]);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });

  it("should deny access when user lacks required role", async () => {
    mockIdentity.setLoggedIn("token", ["User"]);
    mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "access-denied"]);
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

    const guard = roleGuard("Administrator" as RoleName);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).not.toBe(true);
  });

  it("should support custom redirect option", async () => {
    mockIdentity.setLoggedIn("token", ["User"]);
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

    const customRedirect = ["/", "custom-page"] as any;
    const guard = roleGuard("Administrator" as RoleName, { redirectTo: customRedirect });

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).not.toBe(true);
  });

  it("should handle empty role array", async () => {
    mockIdentity.setLoggedIn("token", ["User"]);
    const watchPermSpy = vi.spyOn(mockIdentity, "watchUserPermission$");
    watchPermSpy.mockReturnValue(of(true));

    const guard = roleGuard([] as RoleName[]);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });
});
