import { TestBed } from "@angular/core/testing";
import { ActivatedRouteSnapshot, Router } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { permissionsGuard } from "./permissions.guard";
import { MockIdentityService, MockRouteAliasService, createMockActivatedRouteSnapshot } from "@testing";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { ClaimDto, RoleName } from "@core/backend-api";
import { firstValueFrom, of } from "rxjs";

describe("permissionsGuard", () => {
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
        { provide: Router, useValue: { createUrlTree: vi.fn() } },
      ],
    });

    route = createMockActivatedRouteSnapshot();
  });

  describe("role-based permissions", () => {
    it("should allow access when user has required role", async () => {
      mockIdentity.setLoggedIn("token", ["Administrator"]);
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

      const guard = permissionsGuard("Administrator" as RoleName, []);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).toBe(true);
    });

    it("should deny access when user lacks required role", async () => {
      mockIdentity.setLoggedIn("token", ["User"]);
      mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "access-denied"]);
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

      const guard = permissionsGuard("Administrator" as RoleName, []);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).not.toBe(true);
    });

    it("should allow access when user has any of multiple required roles", async () => {
      mockIdentity.setLoggedIn("token", ["Editor"]);
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator", "Editor"]));

      const guard = permissionsGuard(["Administrator", "Editor"] as RoleName[], []);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).toBe(true);
    });
  });

  describe("claim-based permissions", () => {
    it("should allow access when user has required claim", async () => {
      const claims = new Map([["permission", ["read"]]]);
      mockIdentity.setLoggedIn("token", [], claims);

      const claim: ClaimDto = { type: "permission", value: "read" };
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$([claim]));

      const guard = permissionsGuard([], claim);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).toBe(true);
    });

    it("should deny access when user lacks required claim", async () => {
      mockIdentity.setLoggedIn("token", []);
      mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "access-denied"]);

      const claim: ClaimDto = { type: "permission", value: "write" };
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$([claim]));

      const guard = permissionsGuard([], claim);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).not.toBe(true);
    });

    it("should process claim templates with route params", async () => {
      const claims = new Map([["resource:document:123", ["read"]]]);
      mockIdentity.setLoggedIn("token", [], claims);

      route = createMockActivatedRouteSnapshot({ params: { id: "123" } });

      const claim: ClaimDto = { type: "resource:document:{{id}}", value: "read" };
      const watchPermSpy = vi.spyOn(mockIdentity, "watchUserPermission$");
      watchPermSpy.mockReturnValue(of(true));

      const guard = permissionsGuard([], claim);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).toBe(true);
    });
  });

  describe("custom redirect", () => {
    it("should redirect to custom route when specified", async () => {
      mockIdentity.setLoggedIn("token", ["User"]);
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

      const customRedirect = ["/", "custom-denied"] as any;
      const guard = permissionsGuard("Administrator" as RoleName, [], { redirectTo: customRedirect });

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).not.toBe(true);
    });

    it("should use default access-denied route when no custom redirect", async () => {
      mockIdentity.setLoggedIn("token", ["User"]);
      mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "access-denied"]);
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Administrator"]));

      const guard = permissionsGuard("Administrator" as RoleName, []);

      await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(mockRouteAlias.getRoute).toHaveBeenCalledWith("access-denied");
    });
  });

  describe("combined role and claim permissions", () => {
    it("should allow access when user has both role and claim", async () => {
      const claims = new Map([["permission", ["write"]]]);
      mockIdentity.setLoggedIn("token", ["Editor"], claims);

      const claim: ClaimDto = { type: "permission", value: "write" };
      vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserRole$(["Editor"]));

      const guard = permissionsGuard("Editor" as RoleName, claim);

      const result = await TestBed.runInInjectionContext(async () => {
        return firstValueFrom(guard(route));
      });

      expect(result).toBe(true);
    });
  });
});
