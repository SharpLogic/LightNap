import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { PublishedStaticContentDto } from "@core/backend-api";
import { ContentService } from "@core/content/services/content.service";
import { TypeHelpers } from "@core/helpers";
import { ApiResponseComponent } from "../api-response/api-response.component";
import { StaticContentComponent } from "../static-content/static-content.component";

@Component({
  selector: "zone",
  templateUrl: "./zone.component.html",
  imports: [CommonModule, ApiResponseComponent, StaticContentComponent],
})
export class ZoneComponent {
  readonly #contentService = inject(ContentService);
  readonly key = input.required<string>();
  readonly languageCode = input("en");
  readonly bypassSanitization = input(false);
  readonly content = computed(() => this.#contentService.getPublishedStaticContent(this.key(), this.languageCode()));
  readonly asContent = TypeHelpers.cast<PublishedStaticContentDto>;
}
