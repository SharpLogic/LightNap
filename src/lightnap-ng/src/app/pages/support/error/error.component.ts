import { Component, inject } from "@angular/core";
import { RouterLink } from "@angular/router";
import { RoutePipe } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { LayoutService } from "@core/features/layout/services/layout.service";

@Component({
  templateUrl: "./error.component.html",
  imports: [RouterLink, RoutePipe, BrandedCardComponent],
})
export class ErrorComponent {
  layoutService = inject(LayoutService);
}
