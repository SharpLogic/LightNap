import { WritableSignal } from "@angular/core";
import { ErrorApiResponse } from "@core/backend-api";
import { distinctUntilChanged, Observable } from "rxjs";

/**
 * RxJS operator that filters out items emitted by the source Observable that are the same as the previous item when stringified.
 *
 * @template T The type of items emitted by the source Observable.
 * @returns A function that returns an Observable that emits items from the source Observable only when the serialized JSON representation of the current item is different from the serialized JSON representation of the previous item.
 */
export function distinctUntilJsonChanged<T>() {
  return (source$: Observable<T>) =>
    source$.pipe(distinctUntilChanged((original, incoming) => JSON.stringify(original) === JSON.stringify(incoming)));
}

/**
 * Sets API error messages into a writable signal.
 * @param errorSignal The writable signal to set the error messages into.
 * @returns A function that takes an ErrorApiResponse and sets its error messages into the provided signal.
 */
export function setApiErrors(errorSignal: WritableSignal<string[]>) {
  return (response: ErrorApiResponse<string>) => {
    errorSignal.set(response.errorMessages || []);
  };
}
