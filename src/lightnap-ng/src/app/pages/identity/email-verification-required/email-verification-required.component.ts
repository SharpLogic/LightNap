import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@core";
import { IdentityCardComponent } from "@core";

@Component({
  standalone: true,
  templateUrl: "./email-verification-required.component.html",
  imports: [RouterModule, RoutePipe, IdentityCardComponent],
})
export class EmailVerificationRequiredComponent {}
