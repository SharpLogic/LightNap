import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { BlockUiService, ErrorListComponent, ToastService } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { ProfileService } from "@core/services/profile.service";
import { RouteAliasService, RoutePipe } from "@routing";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { finalize, tap } from "rxjs";
import { IdentityService } from "@core/services/identity.service";

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [CommonModule, ErrorListComponent, ReactiveFormsModule, ButtonModule, PanelModule, RouterLink, RoutePipe, ApiResponseComponent],
})
export class IndexComponent {
  readonly #identityService = inject(IdentityService);
  readonly #profileService = inject(ProfileService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly #blockUi = inject(BlockUiService);
  readonly #toast = inject(ToastService);
  readonly #fb = inject(FormBuilder);

  form = this.#fb.group({});
  errors = signal(new Array<string>());

  readonly profile$ = this.#profileService.getProfile().pipe(
    tap(profile => {
      // Set form values.
    })
  );

  updateProfile() {
    this.#blockUi.show({ message: "Updating profile..." });
    this.#profileService
      .updateProfile({})
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#toast.success("Profile updated successfully."),
        error: response => this.errors.set(response.errorMessages),
      });
  }

  logOut() {
    this.#blockUi.show({ message: "Logging out..." });
    this.#identityService
      .logOut()
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("landing"),
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
