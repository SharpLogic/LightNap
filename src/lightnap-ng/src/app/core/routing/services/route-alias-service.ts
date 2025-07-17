import { inject, Injectable } from "@angular/core";
import { NavigationExtras, Router } from "@angular/router";
import { RouteAlias } from "@core/models";
import { AppRoute } from "@core";

@Injectable({
  providedIn: "root",
})
/**
 * Service to manage route aliases and their corresponding paths.
 *
 * This service provides functionality to map route aliases to their paths,
 * navigate to routes using aliases, and retrieve paths associated with aliases.
 */
export class RouteAliasService {
  #router = inject(Router);

  /**
   * Map to store route aliases and their corresponding paths.
   * @type {Map<RouteAlias, Array<string>>}
   * @private
   */
  #routeMap = new Map<RouteAlias, Array<string>>();

  /**
   * Constructs the RouteAliasService and initializes the route map.
   */
  constructor() {
    this.#router.config.forEach(route => this.#buildRoutes(route, ["/"]));
  }

  /**
   * Recursively builds the route map from the router configuration.
   * @param {AppRoute} route - The current route being processed.
   * @param {Array<string>} path - The accumulated path segments.
   * @private
   * @throws {Error} If a duplicate route alias is found.
   */
  #buildRoutes(route: AppRoute, path: Array<string>) {
    if (route.path?.length) {
      path.push(...route.path.split("/"));
    }

    const alias = route.data?.alias;
    if (alias) {
      const mappedRoute = this.#routeMap.get(alias);
      if (mappedRoute) {
        const existingPath = mappedRoute.join("/");
        const newPath = path.join("/");
        throw new Error(`Duplicate route for '${alias}': Two route paths have the same alias. See route: '${existingPath}' and '${newPath}'.`);
      }
      this.#routeMap.set(alias, [...path]);
    }

    if (route.children) {
      route.children.forEach(child => this.#buildRoutes(child, [...path]));
    }
  }

  /**
   * Navigates to the route associated with the given alias.
   * @param {RouteAlias} view - The alias of the route to navigate to.
   * @param {any} [value] - Optional value to pass to the route.
   * @returns {Promise<boolean>} A promise that resolves to true if navigation is successful, false otherwise.
   */
  navigate(view: RouteAlias, value?: any) {
    return this.#router.navigate(this.getRoute(view, value));
  }

  /**
   * Navigates to the route associated with the given alias using the replace option.
   * @param {RouteAlias} view - The alias of the route to navigate to.
   * @param {any} [value] - Optional value to pass to the route.
   * @returns {Promise<boolean>} A promise that resolves to true if navigation is successful, false otherwise.
   */
  navigateWithExtras(view: RouteAlias, value?: any, extras?: NavigationExtras) {
    return this.#router.navigate(this.getRoute(view, value), extras);
  }

  /**
   * Retrieves the route path associated with the given alias.
   * @param {RouteAlias} alias - The alias of the route.
   * @param {any} [value] - Optional value/array to pass to the route as ordered params.
   * @returns {Array<string>} The route path segments.
   * @private
   * @throws {Error} If the wrong number of parameters is provided for the route.
   */
  getRoute(alias: RouteAlias, value?: any): Array<string> {
    const path = this.#routeMap.get(alias);
    if (!path) {
      throw new Error(`Unexpected route alias '${alias}'.`);
    }

    if (!value) return path;
    value = Array.isArray(value) ? value : [value];

    const pathList = [...path];
    const paramList = [...value];

    for (let i = 0; i < pathList.length; i++) {
      if (pathList[i].startsWith(":")) {
        if (pathList[i].endsWith("?")) {
          pathList[i] = paramList.length ? paramList.shift() : "";
        } else if (paramList.length) {
          pathList[i] = paramList.shift();
        } else {
          throw new Error(`Not enough parameters provided for route alias '${alias}'  ('${path.join("/")}'). Only received '${value.join("/")}'.`);
        }
      }
    }

    if (paramList.length) {
      throw new Error(`Too many parameters provided for route alias '${alias}' ('${path.join("/")}'). Received '${value.join("/")}'.`);
    }

    return pathList;
  }
}
