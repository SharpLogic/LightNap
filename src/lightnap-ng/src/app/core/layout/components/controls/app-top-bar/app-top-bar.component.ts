import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterModule } from "@angular/router";
import { LayoutService } from "@core/layout/services/layout.service";
import { NotificationsButtonComponent, RoutePipe } from "@core";
import { MenuItem } from "primeng/api";
import { StyleClassModule } from "primeng/styleclass";
import { AppConfiguratorComponent } from "../app-configurator/app-configurator.component";

@Component({
  selector: "app-top-bar",
  standalone: true,
  templateUrl: "./app-top-bar.component.html",
  imports: [RouterModule, CommonModule, StyleClassModule, AppConfiguratorComponent, NotificationsButtonComponent, RoutePipe],
})
export class AppTopBarComponent {
  layoutService = inject(LayoutService);
  items!: MenuItem[];

  toggleDarkMode() {
    this.layoutService.layoutConfig.update(state => ({ ...state, darkTheme: !state.darkTheme }));
  }
}
