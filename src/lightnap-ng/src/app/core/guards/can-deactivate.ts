import { Observable } from "rxjs";

export interface OnCanDeactivate {
  canDeactivate(): Observable<boolean> | boolean;
}
