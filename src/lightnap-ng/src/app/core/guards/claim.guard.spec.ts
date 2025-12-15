import { TestBed } from "@angular/core/testing";
import { ActivatedRouteSnapshot } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { claimGuard } from "./claim.guard";
import { MockIdentityService, MockRouteAliasService, createMockActivatedRouteSnapshot } from "@testing";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { ClaimDto } from "@core/backend-api";
import { firstValueFrom, of } from "rxjs";

describe("claimGuard", () => {
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

  it("should allow access when user has single required claim", async () => {
    const claims = new Map([["permission", ["read"]]]);
    mockIdentity.setLoggedIn("token", [], claims);

    const claim: ClaimDto = { type: "permission", value: "read" };
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$([claim]));

    const guard = claimGuard(claim);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });

  it("should allow access when user has any of multiple required claims", async () => {
    const claims = new Map([["permission", ["write"]]]);
    mockIdentity.setLoggedIn("token", [], claims);

    const requiredClaims: ClaimDto[] = [
      { type: "permission", value: "read" },
      { type: "permission", value: "write" },
    ];
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$(requiredClaims));

    const guard = claimGuard(requiredClaims);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });

  it("should deny access when user lacks required claim", async () => {
    mockIdentity.setLoggedIn("token", []);
    mockRouteAlias.getRoute = vi.fn().mockReturnValue(["/", "access-denied"]);

    const claim: ClaimDto = { type: "permission", value: "admin" };
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$([claim]));

    const guard = claimGuard(claim);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).not.toBe(true);
  });

  it("should support custom redirect option", async () => {
    mockIdentity.setLoggedIn("token", []);
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$([{ type: "test", value: "test" }]));

    const claim: ClaimDto = { type: "permission", value: "special" };
    const customRedirect = ["/", "custom-denied"] as any;
    const guard = claimGuard(claim, { redirectTo: customRedirect });

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).not.toBe(true);
  });

  it("should handle empty claim array", async () => {
    mockIdentity.setLoggedIn("token", []);
    const watchPermSpy = vi.spyOn(mockIdentity, "watchUserPermission$");
    watchPermSpy.mockReturnValue(of(true));

    const guard = claimGuard([]);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });

  it("should handle different claim types", async () => {
    const claims = new Map([["resource", ["document-123"]]]);
    mockIdentity.setLoggedIn("token", [], claims);

    const claim: ClaimDto = { type: "resource", value: "document-123" };
    vi.spyOn(mockIdentity, "watchUserPermission$").mockReturnValue(mockIdentity.watchAnyUserClaim$([claim]));

    const guard = claimGuard(claim);

    const result = await TestBed.runInInjectionContext(async () => {
      return firstValueFrom(guard(route));
    });

    expect(result).toBe(true);
  });
});
