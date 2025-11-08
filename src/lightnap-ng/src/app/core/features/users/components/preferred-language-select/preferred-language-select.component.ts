import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ContentService } from "@core/features/content/services/content.service";
import { ListItem } from "@core/models";
import { map } from "rxjs";
import { UserSettingSelectComponent } from "../user-setting-select/user-setting-select.component";

@Component({
  selector: "ln-preferred-language-select",
  standalone: true,
  templateUrl: "./preferred-language-select.component.html",
  imports: [CommonModule, UserSettingSelectComponent, ApiResponseComponent],
})
export class PreferredLanguageSelectComponent {
  readonly #contentService = inject(ContentService);
  readonly supportedLanguages = signal(
    this.#contentService
      .getSupportedLanguages()
      .pipe(map(languages => [new ListItem("", "Auto-detect"), ...languages.map(lang => new ListItem(lang.languageCode, lang.languageName))]))
  );
}
