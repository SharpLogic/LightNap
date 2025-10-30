import { CommonModule } from "@angular/common";
import { Component, inject, input } from "@angular/core";
import { IdentityService, ZoneComponent } from "@core";

@Component({
  templateUrl: "./page.component.html",
  imports: [CommonModule, ZoneComponent],
})
export class PageComponent {
  #identityService = inject(IdentityService);
  key = input.required<string>();
}
