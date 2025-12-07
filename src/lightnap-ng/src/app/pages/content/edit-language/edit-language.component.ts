import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { StaticContentFormat, StaticContentFormats, StaticContentLanguageDto, TypeHelpers, setApiErrors } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ContentFormatPickerComponent } from "@core/features/content/components/content-format-picker/content-format-picker.component";
import { StaticContentDirective } from "@core/features/content/directives/static-content.directive";
import { ContentService } from "@core/features/content/services/content.service";
import { UserLinkComponent } from "@core/features/users/components/user-link/user-link.component";
import { ToastService } from "@core/services/toast.service";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TextareaModule } from "primeng/textarea";
import { tap } from "rxjs";

@Component({
  templateUrl: "./edit-language.component.html",
  imports: [
    ApiResponseComponent,
    ErrorListComponent,
    PanelModule,
    CommonModule,
    UserLinkComponent,
    ReactiveFormsModule,
    ButtonModule,
    TextareaModule,
    ContentFormatPickerComponent,
    StaticContentDirective,
  ],
})
export class EditLanguageComponent {
  #contentService = inject(ContentService);
  #toast = inject(ToastService);
  #fb = inject(FormBuilder);

  key = input.required<string>();
  languageCode = input.required<string>();

  form = this.#fb.group({
    content: this.#fb.nonNullable.control(""),
    format: this.#fb.nonNullable.control<StaticContentFormat>(StaticContentFormats.Html, [Validators.required]),
  });

  errors = signal(new Array<string>());

  triggerUpdate = signal(false);

  language = computed(() => {
    this.triggerUpdate();

    return this.#contentService.getStaticContentLanguage(this.key(), this.languageCode()).pipe(
      tap(content => {
        if (!content) return;
        this.form.patchValue({
          content: content.content,
          format: content.format,
        });
      })
    );
  });

  asLanguage = TypeHelpers.cast<StaticContentLanguageDto>;

  update() {
    this.#contentService.updateStaticContentLanguage(this.key(), this.languageCode(), this.form.getRawValue()).subscribe({
      next: _ => this.#toast.success("Language updated successfully."),
      error: setApiErrors(this.errors),
    });
  }

  create() {
    this.#contentService.createStaticContentLanguage(this.key(), this.languageCode(), this.form.getRawValue()).subscribe({
      next: _ => {
        this.#toast.success("Language created successfully.");
        this.triggerUpdate.set(!this.triggerUpdate());
      },
      error: setApiErrors(this.errors),
    });
  }
}
