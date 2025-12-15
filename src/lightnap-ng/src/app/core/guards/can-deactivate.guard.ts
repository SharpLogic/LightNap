import { CanDeactivateFn } from "@angular/router";
import { OnCanDeactivate } from "./can-deactivate";

export const canDeactivateGuard: CanDeactivateFn<OnCanDeactivate> = (component: OnCanDeactivate) => {
  if (!component.canDeactivate) {
    return true;
  }

  return component.canDeactivate();
};
