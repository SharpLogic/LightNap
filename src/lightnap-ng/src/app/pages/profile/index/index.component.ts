import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ProfileDto, RoutePipe, setApiErrors, TypeHelpers } from "@core";
import { UserSettingKeys } from "@core/backend-api/user-setting-key";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ContentService } from "@core/features/content/services/content.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ProfileService } from "@core/services/profile.service";
import { ToastService } from "@core/services/toast.service";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { SelectModule } from "primeng/select";
import { finalize, tap } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [CommonModule, ErrorListComponent, ReactiveFormsModule, ButtonModule, SelectModule, PanelModule, RouterLink, RoutePipe, ApiResponseComponent],
})
export class IndexComponent {
  readonly #identityService = inject(IdentityService);
  readonly #profileService = inject(ProfileService);
  readonly #contentService = inject(ContentService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly #blockUi = inject(BlockUiService);
  readonly #toast = inject(ToastService);
  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    preferredLanguage: ['']
  });
  readonly errors = signal(new Array<string>());
  readonly supportedLanguages = signal<Array<{ label: string; value: string }>>([]);

  readonly profile$ = this.#profileService.getProfile().pipe(
    tap(profile => {
      // Load supported languages
      this.#contentService.getSupportedLanguages().subscribe(languages => {
        this.supportedLanguages.set([
          { label: 'Auto-detect', value: '' },
          ...languages.map(lang => ({ label: lang.languageName, value: lang.languageCode }))
        ]);
      });

      // Load current language preference
      this.#profileService.getSetting<string>(UserSettingKeys.PreferredLanguage, '').subscribe(preferredLanguage => {
        this.form.patchValue({ preferredLanguage: preferredLanguage || '' });
      });
    })
  );

  asProfile = TypeHelpers.cast<ProfileDto>;

  updateProfile() {
    this.#blockUi.show({ message: "Updating profile..." });
    
    const preferredLanguage = this.form.value.preferredLanguage || '';
    
    this.#profileService
      .setSetting(UserSettingKeys.PreferredLanguage, preferredLanguage)
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
