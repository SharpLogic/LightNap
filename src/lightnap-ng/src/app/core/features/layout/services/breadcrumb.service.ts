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

  #createBreadcrumbs(route: ActivatedRouteSnapshot, url: string = "", breadcrumbs: MenuItem[] = [], isLast = true): MenuItem[] {
    const children: ActivatedRouteSnapshot[] = route.children;

    if (children.length === 0) {
      return breadcrumbs;
    }

    for (const child of children) {
      const routeURL: string = child.url.map(segment => segment.path).join("/");
      if (routeURL !== "") {
        url += `/${routeURL}`;
      }

      const label = child.data["breadcrumb"];
      if (label) {
        const item: MenuItem = { label };

        // Mark the previous breadcrumb as not last (so it gets a link)
        if (breadcrumbs.length > 0 && isLast) {
          breadcrumbs[breadcrumbs.length - 1].routerLink = breadcrumbs[breadcrumbs.length - 1]["_url"];
        }

        // Store the URL temporarily to use if this isn't the last item
        item["_url"] = url;
        breadcrumbs.push(item);
      }

      return this.#createBreadcrumbs(child, url, breadcrumbs, true);
    }

    return breadcrumbs;
  }
}
