import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ProfileDto, RoutePipe, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { PreferredLanguageSelectComponent } from "@core/features/users/components/preferred-language-select/preferred-language-select.component";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ProfileService } from "@core/services/profile.service";
import { ToastService } from "@core/services/toast.service";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { SelectModule } from "primeng/select";
import { finalize, tap } from "rxjs";

@Component({
  templateUrl: "./index.component.html",
  imports: [
    ErrorListComponent,
    ReactiveFormsModule,
    ButtonModule,
    SelectModule,
    PanelModule,
    RouterLink,
    RoutePipe,
    ApiResponseComponent,
    PreferredLanguageSelectComponent,
  ],
})
export class IndexComponent {
  readonly #identityService = inject(IdentityService);
  readonly #profileService = inject(ProfileService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly #blockUi = inject(BlockUiService);
  readonly #toast = inject(ToastService);
  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({});
  readonly errors = signal(new Array<string>());

  readonly profile$ = this.#profileService.getProfile().pipe(tap(profile => {}));

  asProfile = TypeHelpers.cast<ProfileDto>;

  updateProfile() {
    this.#blockUi.show({ message: "Updating profile..." });

    this.#profileService
      .updateProfile(this.form.value)
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => {
          this.#toast.success("Profile updated successfully.");
          this.form.markAsPristine();
        },
        error: setApiErrors(this.errors),
      });
  }

  logOut() {
    this.#blockUi.show({ message: "Logging out..." });
    this.#identityService
      .logOut()
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("landing"),
        error: setApiErrors(this.errors),
      });
  }
}
