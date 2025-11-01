import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import {
  ApiResponseComponent,
  ContentFormatDropdownComponent,
  ErrorListComponent,
  setApiErrors,
  StaticContentFormat,
  StaticContentFormats,
  StaticContentLanguageDto,
  ToastService,
  TypeHelpers,
  UserLinkComponent,
} from "@core";
import { ContentService } from "@core/content/services/content.service";
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
    ContentFormatDropdownComponent,
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
    format: this.#fb.nonNullable.control<StaticContentFormats>(StaticContentFormat.Html, [Validators.required]),
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

  onUpdate() {
    this.#contentService.updateStaticContentLanguage(this.key(), this.languageCode(), this.form.getRawValue()).subscribe({
      next: _ => this.#toast.success("Language updated successfully."),
      error: setApiErrors(this.errors),
    });
  }

  onCreate() {
    this.#contentService.createStaticContentLanguage(this.key(), this.languageCode(), this.form.getRawValue()).subscribe({
      next: _ => {
        this.#toast.success("Language created successfully.");
        this.triggerUpdate.set(!this.triggerUpdate());
      },
      error: setApiErrors(this.errors),
    });
  }
}
