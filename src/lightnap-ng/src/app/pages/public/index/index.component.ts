import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { PanelModule } from 'primeng/panel';

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [CommonModule, PanelModule],
})
export class IndexComponent {

}
