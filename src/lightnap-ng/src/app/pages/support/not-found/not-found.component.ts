import { Component, inject } from "@angular/core";
import { RouterModule } from "@angular/router";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { LayoutService } from "@core/features/layout/services/layout.service";

@Component({
  standalone: true,
  templateUrl: "./not-found.component.html",
  imports: [RouterModule, BrandedCardComponent],
})
export class NotFoundComponent {
  layoutService = inject(LayoutService);
}
