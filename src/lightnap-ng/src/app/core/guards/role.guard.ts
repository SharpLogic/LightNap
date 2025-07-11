import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot } from "@angular/router";
import { RoleNames } from "@core/backend-api";
import { IdentityService } from "@core/services";
import { RouteAliasService } from "@pages";
import { take, map } from "rxjs";

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
