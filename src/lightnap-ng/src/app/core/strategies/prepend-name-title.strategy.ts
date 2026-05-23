import { inject, Injectable } from "@angular/core";
import { RouterStateSnapshot, TitleStrategy } from "@angular/router";
import { APP_NAME } from "@core";

@Injectable({
  providedIn: "root",
})
export class PrependNameTitleStrategy extends TitleStrategy {
  #appName = inject(APP_NAME);

  override updateTitle(routerState: RouterStateSnapshot) {
    const title = this.buildTitle(routerState);
    if (title) {
      if (this.#appName?.length && !title.includes("|")) {
        document.title = `${this.#appName} | ${title}`;
      } else {
        document.title = title;
      }
    } else if (this.#appName?.length) {
      document.title = this.#appName;
    }
  }
}
