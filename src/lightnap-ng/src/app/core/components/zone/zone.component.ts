import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { PublishedContent } from "@core/content/entities";
import { ContentService } from "@core/content/services/content.service";
import { StaticContentDirective } from "@core/directives";
import { TypeHelpers } from "@core/helpers";
import { ApiResponseComponent } from "../api-response/api-response.component";
import { RouterModule } from "@angular/router";
import { RoutePipe } from "@core/routing";

@Component({
  selector: "ln-zone",
  templateUrl: "./zone.component.html",
  imports: [CommonModule, RouterModule, RoutePipe, ApiResponseComponent, StaticContentDirective],
})
export class ZoneComponent {
  readonly #contentService = inject(ContentService);
  readonly key = input.required<string>();
  readonly languageCode = input("en");
  readonly sanitize = input(false);
  readonly showContentStripWarning = input(false);
  readonly content = computed(() => this.#contentService.getPublishedStaticContent(this.key(), this.languageCode()));
  readonly asContent = TypeHelpers.cast<PublishedContent>;
}
