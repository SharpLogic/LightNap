import { inject, Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, NavigationEnd, Router } from "@angular/router";
import { MenuItem } from "primeng/api";
import { BehaviorSubject, Observable } from "rxjs";
import { filter } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class BreadcrumbService {
  readonly #router = inject(Router);
  readonly #breadcrumbsSubject = new BehaviorSubject<MenuItem[]>([]);
  readonly breadcrumbs$: Observable<MenuItem[]> = this.#breadcrumbsSubject.asObservable();

  constructor() {
    this.#router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      const breadcrumbs = this.#createBreadcrumbs(this.#router.routerState.snapshot.root);
      this.#breadcrumbsSubject.next(breadcrumbs);
    });
  }

  #createBreadcrumbs(route: ActivatedRouteSnapshot, url: string = "", breadcrumbs: MenuItem[] = []): MenuItem[] {
    const children: ActivatedRouteSnapshot[] = route.children;

    for (const child of children) {
      const routeURL: string = child.url.map(segment => segment.path).join("/");
      if (routeURL !== "") {
        url += `/${routeURL}`;
      }

      const breadcrumbData = child.data["breadcrumb"];
      if (breadcrumbData) {
        // Support both static strings and dynamic functions
        const label = typeof breadcrumbData === "function" ? breadcrumbData(child) : breadcrumbData;
        const item: MenuItem = { label, routerLink: url };
        breadcrumbs.push(item);
      }

      return this.#createBreadcrumbs(child, url, breadcrumbs);
    }

    // Remove routerLink from the last breadcrumb (current page)
    if (breadcrumbs.length > 0) {
      delete breadcrumbs[breadcrumbs.length - 1].routerLink;
    }

    return breadcrumbs;
  }
}
