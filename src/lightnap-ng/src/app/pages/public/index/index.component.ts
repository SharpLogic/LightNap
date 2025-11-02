import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ZoneComponent } from "@core/components/zone/zone.component";
import { PanelModule } from "primeng/panel";

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [CommonModule, PanelModule, ZoneComponent],
})
export class IndexComponent {}
