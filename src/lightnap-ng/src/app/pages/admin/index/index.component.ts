import { Component } from "@angular/core";
import { ZoneComponent } from "@core/features/content/components/zone/zone.component";

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [ZoneComponent],
})
export class IndexComponent {}
