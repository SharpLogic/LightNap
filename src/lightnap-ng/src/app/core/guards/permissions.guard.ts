import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot } from "@angular/router";
import { ClaimDto, RoleNames, RouteTemplateHelpers } from "@core";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { IdentityService } from "@core/services/identity.service";
import { map, take } from "rxjs";

export function permissionsGuard(
  roles: RoleNames | Array<RoleNames>,
  claims: ClaimDto | Array<ClaimDto>,
  guardOptions?: { redirectTo?: Array<object> }
) {
  return (next: ActivatedRouteSnapshot) => {
    const identityService = inject(IdentityService);
    const routeAliasService = inject(RouteAliasService);

    const claimTemplates = Array.isArray(claims) ? claims : [claims];
    const processedClaims = claimTemplates.map(claim => {
      return {
        type: RouteTemplateHelpers.processTemplate(claim.type, next.params),
        value: RouteTemplateHelpers.processTemplate(claim.value, next.params),
      } as ClaimDto;
    });

    return identityService.watchUserPermission$(Array.isArray(roles) ? roles : [roles], processedClaims).pipe(
      take(1),
      map(hasPermission =>
        hasPermission ? true : createUrlTreeFromSnapshot(next, guardOptions?.redirectTo ?? routeAliasService.getRoute("access-denied"))
      )
    );
  };
}
