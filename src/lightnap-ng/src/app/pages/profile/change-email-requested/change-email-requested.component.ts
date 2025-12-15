import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";
import { RoutePipe } from "@core";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";

@Component({
  templateUrl: "./change-email-requested.component.html",
  imports: [PanelModule, ButtonModule, RouterLink, RoutePipe],
})
export class ChangeEmailRequestedComponent {}
