import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@routing";
import { IdentityCardComponent } from "../../controls/identity-card/identity-card.component";

@Component({
  standalone: true,
  templateUrl: './magic-link-sent.component.html',
  imports: [RouterModule, RoutePipe, IdentityCardComponent],
})
export class MagicLinkSentComponent {}
