import { inject, Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, NavigationEnd, Router } from "@angular/router";
import { MenuItem } from "primeng/api";
import { filter, map, startWith } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class BreadcrumbService {
  readonly #router = inject(Router);

  readonly breadcrumbs$ = this.#router.events.pipe(
    filter(event => event instanceof NavigationEnd),
    map(() => this.#createBreadcrumbs(this.#router.routerState.snapshot.root))
  );

  #createBreadcrumbs(route: ActivatedRouteSnapshot, url: string = "", breadcrumbs: MenuItem[] = []): MenuItem[] {
    const children: ActivatedRouteSnapshot[] = route.children;

    for (const child of children) {
      const routeURL: string = child.url.map(segment => segment.path).join("/");
      if (routeURL !== "") {
        url += `/${routeURL}`;
      }

      const breadcrumbData = child.data["breadcrumb"];
      if (breadcrumbData) {
        // Support both static strings and dynamic functions. If you need to do something more complex,
        // like loading data from an API, consider using a resolver to fetch the data before route activation.
        const label = typeof breadcrumbData === "function" ? breadcrumbData(child) : breadcrumbData;
        const item: MenuItem = { label, routerLink: url };
        breadcrumbs.push(item);
      }

      return this.#createBreadcrumbs(child, url, breadcrumbs);
    }

    // Remove routerLink from the last breadcrumb (current page). This is kind of a hack because it
    // assumes the last breadcrumb is always the current page and it looks better without the link.
    // If you're here wondering why that last breadcrumb isn't rendering as a link, now you know.
    if (breadcrumbs.length > 0) {
      delete breadcrumbs[breadcrumbs.length - 1].routerLink;
    }

    return breadcrumbs;
  }
}
