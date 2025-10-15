import { Injectable } from "@angular/core";
import { ReplaySubject } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class InitializationService {
  #initializedSubject$ = new ReplaySubject<boolean>(1);
  initialized$ = this.#initializedSubject$.asObservable();

  initialize() {
    this.#initializedSubject$.next(true);
    this.#initializedSubject$.complete();
  }
}
