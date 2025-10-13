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

  content = `
  <p-card-control class="w-96">
    <p>Card Content</p>
  <ng-template #title> Card Title </ng-template>
  <ng-template #subtitle> Card Subtitle </ng-template>
    <p>Card Content</p>
    </p-card-control>

  <user-id-control userName="admin"></user-id-control>
              <user-id-control userName="user1"></user-id-control>
             `;

  get safeContent() {
    const data = this.#sanitizer.bypassSecurityTrustHtml(this.content);
    return data;
  }
}
