import { ActivatedRouteSnapshot } from "@angular/router";
import { RouteAlias } from "./route-alias";

/**
 * Represents the data associated with a route.
 */
export interface RouteData {
    /**
     * An optional alias for the route. This alias is a shorthand that can be used to generate routes
     * and for the RoutePipe for cleaner binding in templates.
     */
    alias?: RouteAlias;

    /**
     * An optional breadcrumb label for the route.
     * Can be a static string or a function that receives the route snapshot and returns a string.
     */
    breadcrumb?: string | ((route: ActivatedRouteSnapshot) => string);
}
