import { ActivatedRouteSnapshot } from "@angular/router";
import { RoleName } from "@core/backend-api";
import { permissionsGuard } from "./permissions.guard";

export function roleGuard(roles: RoleName | Array<RoleName>, guardOptions?: { redirectTo?: Array<object> }) {
  return (next: ActivatedRouteSnapshot) => permissionsGuard(roles, [], guardOptions)(next);
}
