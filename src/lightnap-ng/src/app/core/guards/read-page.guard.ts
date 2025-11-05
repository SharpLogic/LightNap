import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot, RouterStateSnapshot } from "@angular/router";
import { ContentService } from "@core/features/content/services/content.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { IdentityService } from "@core/services/identity.service";
import { map, switchMap, take } from "rxjs";

export const readPageGuard = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const routeAliasService = inject(RouteAliasService);
  const identityService = inject(IdentityService);
  const contentService = inject(ContentService);

  const key = next.paramMap.get("key");
  if (!key) {
    console.warn("Read page guard activated without a key parameter.");
    return createUrlTreeFromSnapshot(next, routeAliasService.getRoute("not-found"));
  }

  return identityService.watchLoggedIn$().pipe(
    take(1),
    switchMap(_ =>
      contentService.getPublishedStaticContent(key, "en").pipe(
        map(content => {
          if (!content) return createUrlTreeFromSnapshot(next, routeAliasService.getRoute("not-found"));
          if (content.canView) return true;

          if (content.requiresAuthentication) {
            identityService.setRedirectUrl(state.url);
            return createUrlTreeFromSnapshot(next, routeAliasService.getRoute("login"));
          }

          return createUrlTreeFromSnapshot(next, routeAliasService.getRoute("access-denied"));
        })
      )
    )
  );
};
