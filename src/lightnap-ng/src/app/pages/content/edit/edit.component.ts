import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import {
    ApiResponseComponent,
    ContentReadAccessDropdownComponent,
    ContentStatusDropdownComponent,
    ErrorListComponent,
    RouteAliasService,
    setApiErrors,
    StaticContentDto,
    StaticContentReadAccess,
    StaticContentReadAccesses,
    StaticContentStatus,
    StaticContentStatuses,
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
    ContentStatusDropdownComponent,
    ContentReadAccessDropdownComponent,
    UserLinkComponent,
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
          readAccess: content.readAccess,
          editorRoles: content.editorRoles,
          viewerRoles: content.viewerRoles,
        });
      })
    )
  );

  asContent = TypeHelpers.cast<StaticContentDto>;

  onUpdate() {
    this.#contentService.updateStaticContent(this.key(), this.form.getRawValue()).subscribe({
      next: sc => {
        this.#toast.success("Content updated successfully.");
        if (this.key() !== sc.key) {
          this.#routeAlias.navigate("edit-content", sc.key);
        }
      },
      error: setApiErrors(this.errors),
    });
  }
}
