import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot } from "@angular/router";
import { RoleNames } from "@identity/models";
import { RouteAliasService } from "@routing";
import { map, take } from "rxjs";
import { IdentityService } from "src/app/identity/services/identity.service";

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
