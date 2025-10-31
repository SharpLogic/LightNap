import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, createUrlTreeFromSnapshot, RouterStateSnapshot } from "@angular/router";
import { RouteAliasService } from "@core";
import { map, of, switchMap, take } from "rxjs";
import { IdentityService } from "@core/services/identity.service";
import { ContentService } from "@core/content/services/content.service";

export const editPageGuard = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const routeAliasService = inject(RouteAliasService);
  const identityService = inject(IdentityService);
  const contentService = inject(ContentService);

  const key = next.paramMap.get("key");
  if (!key) {
    console.warn("Edit page guard activated without a key parameter.");
    return createUrlTreeFromSnapshot(next, routeAliasService.getRoute("not-found"));
  }

  return identityService.watchLoggedIn$().pipe(
    take(1),
    switchMap(isLoggedIn => {
      if (!isLoggedIn) {
        identityService.setRedirectUrl(state.url);
        return of(createUrlTreeFromSnapshot(next, routeAliasService.getRoute("login")));
      }

      return contentService.getStaticContent(key).pipe(
        map(content => {
          if (!content) return createUrlTreeFromSnapshot(next, routeAliasService.getRoute("not-found"));

          return true;
        })
      );
    })
  );
};
