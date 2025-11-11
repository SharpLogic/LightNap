import { Component, inject } from "@angular/core";
import { PanelMenuModule } from "primeng/panelmenu";
import { MenuService } from "@core/features/layout/services/menu.service";

@Component({
  selector: "ln-app-sidebar",
  templateUrl: "./app-sidebar.component.html",
  imports: [PanelMenuModule],
})
export class AppSidebarComponent {
  readonly #menuService = inject(MenuService);
  readonly menuItems = this.#menuService.menuItems;
}
