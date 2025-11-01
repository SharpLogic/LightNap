import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import {
  ApiResponseComponent,
  ContentReadAccessPickerComponent,
  ContentStatusPickerComponent,
  ContentTypePickerComponent,
  ErrorListComponent,
  RouteAliasService,
  RoutePipe,
  setApiErrors,
  StaticContentDto,
  StaticContentReadAccess,
  StaticContentReadAccesses,
  StaticContentStatus,
  StaticContentStatuses,
  StaticContentSupportedLanguageDto,
  StaticContentType,
  StaticContentTypes,
  ToastService,
  TypeHelpers,
  UserLinkComponent,
} from "@core";
import { ContentService } from "@core/content/services/content.service";
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
    RouterLink,
    RoutePipe,
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
    viewerRoles: this.#fb.nonNullable.control(""),
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
          viewerRoles: content.viewerRoles,
        });
      })
    )
  );

  languages = computed(() => this.#contentService.getSupportedLanguages());

  asContent = TypeHelpers.cast<StaticContentDto>;
  asLanguages = TypeHelpers.cast<Array<StaticContentSupportedLanguageDto>>;

  onUpdate() {
    this.#contentService.updateStaticContent(this.key(), this.form.getRawValue()).subscribe({
      next: sc => {
        this.#toast.success("Content updated successfully.");
        this.#routeAlias.navigate("edit-content", sc.key);
      },
      error: setApiErrors(this.errors),
    });
  }
}
