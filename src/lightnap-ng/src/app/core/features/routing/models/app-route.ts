import { Route } from "@angular/router";
import { RouteData } from "./route-data";

/**
 * Represents an application route that extends the Angular `Route` interface.
 */
export interface AppRoute extends Route {
  /**
   * An optional array of child routes, each of which is also an `AppRoute`.
   */
  children?: Array<AppRoute>;

  /**
   * Optional custom data associated with the route.
   */
  data?: RouteData;
}
