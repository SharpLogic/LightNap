import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { PublishedStaticContentDto } from "@core/backend-api";
import { ContentService } from "@core/content/services/content.service";
import { TypeHelpers } from "@core/helpers";
import { ApiResponseComponent } from "../api-response/api-response.component";
import { StaticContentDirective } from "@core/directives";

@Component({
  selector: "zone",
  templateUrl: "./zone.component.html",
  imports: [CommonModule, ApiResponseComponent, StaticContentDirective],
})
export class ZoneComponent {
  readonly #contentService = inject(ContentService);
  readonly key = input.required<string>();
  readonly languageCode = input("en");
  readonly sanitize = input(false);
  readonly showContentStripWarning = input(false);
  readonly content = computed(() => this.#contentService.getPublishedStaticContent(this.key(), this.languageCode()));
  readonly asContent = TypeHelpers.cast<PublishedStaticContentDto>;
}
