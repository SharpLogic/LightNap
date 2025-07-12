import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@core";
import { IdentityCardComponent } from "@core";

@Component({
  standalone: true,
  templateUrl: './magic-link-sent.component.html',
  imports: [RouterModule, RoutePipe, IdentityCardComponent],
})
export class MagicLinkSentComponent {}
