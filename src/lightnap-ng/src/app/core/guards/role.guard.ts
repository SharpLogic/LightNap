import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot } from "@angular/router";
import { RouteAliasService } from "@routing";
import { map, take } from "rxjs";
import { IdentityService } from "@core/services/identity.service";
import { RoleNames } from "@core/api/role-names";

export function roleGuard(roles: RoleNames | Array<RoleNames>, guardOptions?: { redirectTo?: Array<object> }) {
  return (next: ActivatedRouteSnapshot) => {
    const identityService = inject(IdentityService);
    const routeAliasService = inject(RouteAliasService);

    return identityService.watchAnyUserRole$(Array.isArray(roles) ? roles : [roles]).pipe(
      take(1),
      map(isInRole => (isInRole ? true : createUrlTreeFromSnapshot(next, guardOptions?.redirectTo ?? routeAliasService.getRoute("access-denied"))))
    );
  };
}
