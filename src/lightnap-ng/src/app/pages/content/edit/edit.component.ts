import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
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
import { ClaimUsersManagerComponent } from "@core/components/claim-users-manager/claim-users-manager.component";
import { ContentReadAccessPickerComponent } from "@core/components/content-read-access-picker/content-read-access-picker.component";
import { ContentStatusPickerComponent } from "@core/components/content-status-picker/content-status-picker.component";
import { ContentTypePickerComponent } from "@core/components/content-type-picker/content-type-picker.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RolesPickerComponent } from "@core/components/roles-picker/roles-picker.component";
import { UserLinkComponent } from "@core/components/user-link/user-link.component";
import { ContentService } from "@core/features/content/services/content.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { ToastService } from "@core/services/toast.service";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
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

  content = computed(() =>
    this.#contentService.getStaticContent(this.key()).pipe(
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
      })
    )
  );

  languages = computed(() => this.#contentService.getSupportedLanguages());

  asContent = TypeHelpers.cast<StaticContentDto>;
  asLanguages = TypeHelpers.cast<Array<StaticContentSupportedLanguageDto>>;

  onUpdate() {
    const value = {
      ...this.form.getRawValue(),
      readerRoles: this.form.value.readAccess === StaticContentReadAccess.Explicit ? this.form.value.readerRoles : undefined,
    };

    this.#contentService.updateStaticContent(this.key(), value).subscribe({
      next: sc => {
        this.#toast.success("Content updated successfully.");
        this.#routeAlias.navigate("edit-content", sc.key);
      },
      error: setApiErrors(this.errors),
    });
  }
}
