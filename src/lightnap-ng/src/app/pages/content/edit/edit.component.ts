import { Component, input } from "@angular/core";
import { ZoneComponent } from "@core";
import { PanelModule } from "primeng/panel";

@Component({
  templateUrl: "./edit.component.html",
  imports: [PanelModule, ZoneComponent],
})
export class EditComponent {
  key = input.required<string>();
}
