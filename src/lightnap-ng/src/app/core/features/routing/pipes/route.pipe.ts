import { Pipe, PipeTransform, inject } from "@angular/core";
import { RouteAlias } from "@core";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";

@Pipe({
  name: "route",
  standalone: true,
})
/**
 * A pipe that transforms a given `RouteAlias` into route data that can be bound to [routerLink] in templates.
 */
export class RoutePipe implements PipeTransform {
  #routeAlias = inject(RouteAliasService);

  /**
   * Transforms the provided `RouteAlias` into a route string.
   *
   * @param alias - The `RouteAlias` to be transformed.
   * @param value - Optional additional value to be used in the transformation.
   * @returns The route string corresponding to the provided `RouteAlias`.
   */
  transform(alias: RouteAlias, value?: any) {
    return this.#routeAlias.getRoute(alias, value);
  }
}
