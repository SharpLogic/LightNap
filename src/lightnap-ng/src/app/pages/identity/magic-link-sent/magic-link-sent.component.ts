import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";

@Component({
  standalone: true,
  templateUrl: './magic-link-sent.component.html',
  imports: [RouterModule, RoutePipe, BrandedCardComponent],
})
export class MagicLinkSentComponent {}
