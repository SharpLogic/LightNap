import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot } from "@angular/router";
import { Claim } from "@identity/models";
import { RouteAliasService } from "@routing";
import { map, take } from "rxjs";
import { IdentityService } from "src/app/identity/services/identity.service";

export function claimGuard(claims: Claim | Array<Claim>, guardOptions?: { redirectTo?: Array<object> }) {
  return (next: ActivatedRouteSnapshot) => {
    const identityService = inject(IdentityService);
    const routeAliasService = inject(RouteAliasService);

    return identityService.watchAnyUserClaim$(Array.isArray(claims) ? claims : [claims]).pipe(
      take(1),
      map(isInRole => (isInRole ? true : createUrlTreeFromSnapshot(next, guardOptions?.redirectTo ?? routeAliasService.getRoute("access-denied"))))
    );
  };
}
