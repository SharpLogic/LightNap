import { Component, input } from "@angular/core";
import { ZoneComponent } from "@core/features/content/components/zone/zone.component";

@Component({
  templateUrl: "./page.component.html",
  imports: [ZoneComponent],
})
export class PageComponent {
  key = input.required<string>();
}
