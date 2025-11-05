import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import {
  RoutePipe,
  setApiErrors,
  ShowByPermissionsDirective,
  StaticContentDto,
  StaticContentReadAccess,
  StaticContentReadAccesses,
  StaticContentStatus,
  StaticContentStatuses,
  StaticContentSupportedLanguageDto,
  StaticContentType,
  StaticContentTypes,
  ToStringPipe,
  TypeHelpers,
} from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ContentReadAccessPickerComponent } from "@core/features/content/components/content-read-access-picker/content-read-access-picker.component";
import { ContentStatusPickerComponent } from "@core/features/content/components/content-status-picker/content-status-picker.component";
import { ContentTypePickerComponent } from "@core/features/content/components/content-type-picker/content-type-picker.component";
import { ContentService } from "@core/features/content/services/content.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { ClaimUsersManagerComponent } from "@core/features/users/components/claim-users-manager/claim-users-manager.component";
import { RolesPickerComponent } from "@core/features/users/components/roles-picker/roles-picker.component";
import { UserLinkComponent } from "@core/features/users/components/user-link/user-link.component";
import { ToastService } from "@core/services/toast.service";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TabsModule } from "primeng/tabs";
import { tap } from "rxjs";

@Component({
  templateUrl: "./edit.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    PanelModule,
    ApiResponseComponent,
    ErrorListComponent,
    ContentReadAccessPickerComponent,
    ContentStatusPickerComponent,
    ContentTypePickerComponent,
    UserLinkComponent,
    RolesPickerComponent,
    RouterLink,
    RoutePipe,
    ClaimUsersManagerComponent,
    ShowByPermissionsDirective,
    ToStringPipe,
    TabsModule,
  ],
})
export class EditComponent {
  #contentService = inject(ContentService);
  #toast = inject(ToastService);
  #routeAlias = inject(RouteAliasService);
  #fb = inject(FormBuilder);

  key = input.required<string>();

  form = this.#fb.group({
    key: this.#fb.nonNullable.control("", [Validators.required]),
    status: this.#fb.nonNullable.control<StaticContentStatuses>(StaticContentStatus.Draft, [Validators.required]),
    type: this.#fb.nonNullable.control<StaticContentTypes>(StaticContentType.Page, [Validators.required]),
    readAccess: this.#fb.nonNullable.control<StaticContentReadAccesses>(StaticContentReadAccess.Explicit, [Validators.required]),
    editorRoles: this.#fb.nonNullable.control(""),
    readerRoles: this.#fb.nonNullable.control(""),
  });

  errors = signal(new Array<string>());

  #updateSignal = signal(true);

  pageUrl = signal("");

  content = computed(() => {
    this.#updateSignal();
    return this.#contentService.getStaticContent(this.key()).pipe(
      tap(content => {
        if (!content) return;
        this.form.patchValue({
          key: content.key,
          status: content.status,
          type: content.type,
          readAccess: content.readAccess,
          editorRoles: content.editorRoles,
          readerRoles: content.readerRoles,
        });
        this.pageUrl.set(window.location.origin + "/content/" + content.key);
      })
    );
  });

  languages = computed(() => this.#contentService.getSupportedLanguages());

  asContent = TypeHelpers.cast<StaticContentDto>;
  asLanguages = TypeHelpers.cast<Array<StaticContentSupportedLanguageDto>>;

  #previousTabName = "settings";

  constructor() {
    this.form.controls.key.valueChanges.pipe(takeUntilDestroyed()).subscribe({
      next: key => this.pageUrl.set(window.location.origin + "/content/" + key),
    });
  }

  onUpdate() {
    const value = {
      ...this.form.getRawValue(),
      readerRoles: this.form.value.readAccess === StaticContentReadAccess.Explicit ? this.form.value.readerRoles : undefined,
    };

    this.#contentService.updateStaticContent(this.key(), value).subscribe({
      next: sc => {
        this.form.reset();
        this.#toast.success("Content updated successfully.");
        if (sc.key !== this.key()) {
          this.#routeAlias.navigate("edit-content", sc.key);
        } else {
          this.#updateSignal.set(!this.#updateSignal());
        }
      },
      error: setApiErrors(this.errors),
    });
  }

  onTabChanged(tabName: any) {
    if (this.#previousTabName === "settings" && this.form.dirty) {
      this.#toast.info("You have unsaved changes in the Settings tab.");
    }
    this.#previousTabName = tabName;
  }

  onCopyPageUrl() {
    navigator.clipboard
      .writeText(this.pageUrl())
      .then(() => {
        this.#toast.success("Page URL copied to clipboard.");
      })
      .catch(() => {
        this.#toast.error("Failed to copy URL to clipboard.");
      });
  }
}
