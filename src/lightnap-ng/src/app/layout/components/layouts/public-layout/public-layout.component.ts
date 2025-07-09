import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { IdentityService } from "@core";
import { RoutePipe } from "@routing";
import { SharedModule } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { RippleModule } from "primeng/ripple";
import { StyleClassModule } from "primeng/styleclass";
import { LayoutService } from "src/app/layout/services/layout.service";

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
