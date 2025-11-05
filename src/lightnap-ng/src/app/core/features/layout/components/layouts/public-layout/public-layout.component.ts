import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { RoutePipe } from "@core";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { IdentityService } from "@core/services/identity.service";
import { SharedModule } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { RippleModule } from "primeng/ripple";
import { StyleClassModule } from "primeng/styleclass";

@Component({
  standalone: true,
  templateUrl: "./public-layout.component.html",
  imports: [CommonModule, SharedModule, StyleClassModule, RouterOutlet, RouterLink, ButtonModule, RippleModule, RoutePipe],
})
export class PublicLayoutComponent {
  readonly layoutService = inject(LayoutService);
  readonly router = inject(Router);
  readonly identityService = inject(IdentityService);

  readonly loggedIn$ = this.identityService.watchLoggedIn$().pipe(takeUntilDestroyed());

  logOutClicked() {
    this.identityService.logOut().subscribe();
  }
}
