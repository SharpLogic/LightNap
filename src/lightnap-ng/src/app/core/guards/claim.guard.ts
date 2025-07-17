import { ActivatedRouteSnapshot } from "@angular/router";
import { ClaimDto } from "@core";
import { permissionsGuard } from "./permissions.guard";

export function claimGuard(claims: ClaimDto | Array<ClaimDto>, guardOptions?: { redirectTo?: Array<object> }) {
  return (next: ActivatedRouteSnapshot) => permissionsGuard([], claims, guardOptions)(next);
}
