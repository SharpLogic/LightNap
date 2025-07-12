import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot } from "@angular/router";
import { ClaimDto, RoleNames } from "@identity/models";
import { RouteAliasService } from "@core";
import { map, take } from "rxjs";
import { IdentityService } from "@core/services/identity.service";

export function permissionsGuard(roles: RoleNames | Array<RoleNames>, claims: ClaimDto | Array<ClaimDto>, guardOptions?: { redirectTo?: Array<object> }) {
  return (next: ActivatedRouteSnapshot) => {
    const identityService = inject(IdentityService);
    const routeAliasService = inject(RouteAliasService);

    return identityService.watchUserPermission$(Array.isArray(roles) ? roles : [roles], Array.isArray(claims) ? claims : [claims]).pipe(
      take(1),
      map(hasPermission =>
        hasPermission ? true : createUrlTreeFromSnapshot(next, guardOptions?.redirectTo ?? routeAliasService.getRoute("access-denied"))
      )
    );
  };
}
