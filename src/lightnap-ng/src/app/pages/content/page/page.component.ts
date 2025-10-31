import { Component, input } from "@angular/core";
import { ZoneComponent } from "@core";

@Component({
  templateUrl: "./page.component.html",
  imports: [ZoneComponent],
})
export class PageComponent {
  key = input.required<string>();
}
