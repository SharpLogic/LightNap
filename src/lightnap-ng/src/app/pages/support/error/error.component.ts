import { Component, inject } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BrandedCardComponent, RoutePipe } from "@core";
import { LayoutService } from "@core/layout/services/layout.service";

@Component({
  standalone: true,
  templateUrl: "./error.component.html",
  imports: [RouterLink, RoutePipe, BrandedCardComponent],
})
export class ErrorComponent {
  layoutService = inject(LayoutService);
}
