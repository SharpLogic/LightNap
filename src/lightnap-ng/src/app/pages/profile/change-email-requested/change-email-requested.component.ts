import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";
import { RoutePipe } from "@pages";
import { ButtonModule } from "primeng/button";
import { PanelModule } from 'primeng/panel';

@Component({
  standalone: true,
  templateUrl: './change-email-requested.component.html',
  imports: [CommonModule, PanelModule, ButtonModule, RouterLink, RoutePipe],
})
export class ChangeEmailRequestedComponent {
}
