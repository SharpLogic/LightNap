import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { PanelModule } from "primeng/panel";

@Component({
  standalone: true,
  templateUrl: "./page.component.html",
  imports: [CommonModule, PanelModule],
})
export class PageComponent {
  readonly #sanitizer = inject(DomSanitizer);

  content = `              <user-id-control userName="admin"></user-id-control>
              <user-id-control userName="user1"></user-id-control>
             `;

  get safeContent() {
    const data = this.#sanitizer.bypassSecurityTrustHtml(this.content);
    return data;
  }
}
