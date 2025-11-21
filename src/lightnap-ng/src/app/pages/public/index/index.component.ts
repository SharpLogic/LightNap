
import { Component } from "@angular/core";
import { ZoneComponent } from "@core/features/content/components/zone/zone.component";
import { PanelModule } from "primeng/panel";

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [PanelModule, ZoneComponent],
})
export class IndexComponent {}
