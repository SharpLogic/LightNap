import { ActivatedRouteSnapshot } from "@angular/router";
import { RoleNames } from "@core/backend-api";
import { permissionsGuard } from "./permissions.guard";

export function roleGuard(roles: RoleNames | Array<RoleNames>, guardOptions?: { redirectTo?: Array<object> }) {
  return (next: ActivatedRouteSnapshot) => permissionsGuard(roles, [], guardOptions)(next);
}
