import { Component } from "@angular/core";
import { ZoneComponent } from "@core";
import { PanelModule } from "primeng/panel";

@Component({
  templateUrl: "./manage.component.html",
  imports: [PanelModule, ZoneComponent],
})
export class ManageComponent {}
