import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { UserIdComponent } from "@core/components/user-id/user-id.component";
import { PanelModule } from 'primeng/panel';

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [CommonModule, PanelModule, UserIdComponent],
})
export class IndexComponent {

}
