import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@core";
import { BrandedCardComponent } from "@core";

@Component({
  standalone: true,
  templateUrl: "./email-verification-required.component.html",
  imports: [RouterModule, RoutePipe, BrandedCardComponent],
})
export class EmailVerificationRequiredComponent {}
