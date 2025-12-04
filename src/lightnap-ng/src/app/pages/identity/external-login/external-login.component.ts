import { Component, inject, signal } from "@angular/core";
import { RouterModule } from "@angular/router";
import { SupportedExternalLoginDto, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ExternalLoginService } from "@core/services/external-login.service";

@Component({
  templateUrl: "./external-login.component.html",
  imports: [ApiResponseComponent, RouterModule, BrandedCardComponent],
})
export class ExternalLoginComponent {
  readonly #externalLoginService = inject(ExternalLoginService);
  readonly externalLogins = signal(this.#externalLoginService.getSupportedLogins());

  readonly toSupportedExternalLogins = TypeHelpers.cast<Array<SupportedExternalLoginDto>>;
}
