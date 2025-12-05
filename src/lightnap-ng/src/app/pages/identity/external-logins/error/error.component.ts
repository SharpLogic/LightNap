import { Component, input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";

@Component({
  templateUrl: "./error.component.html",
  imports: [RouterModule, RoutePipe, BrandedCardComponent],
})
export class ErrorComponent {
    readonly error = input.required<string>();
}
