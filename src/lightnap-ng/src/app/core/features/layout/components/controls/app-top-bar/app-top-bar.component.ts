import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterModule } from "@angular/router";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { NotificationsButtonComponent } from "@core/features/notifications/components/notifications-button/notifications-button.component";
import { RoutePipe } from "@core/features/routing";
import { MenuItem } from "primeng/api";
import { StyleClassModule } from "primeng/styleclass";
import { AppConfiguratorComponent } from "../app-configurator/app-configurator.component";

@Component({
  selector: "ln-app-top-bar",
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
