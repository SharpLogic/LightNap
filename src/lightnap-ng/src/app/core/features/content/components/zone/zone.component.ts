import { Component, computed, inject, input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { PublishedContent } from "@core/features/content/entities";
import { ContentService } from "@core/features/content/services/content.service";
import { RoutePipe } from "@core/features/routing";
import { TypeHelpers } from "@core/helpers";
import { StaticContentDirective } from "../../directives/static-content.directive";

@Component({
  selector: "ln-zone",
  templateUrl: "./zone.component.html",
  imports: [RouterModule, RoutePipe, ApiResponseComponent, StaticContentDirective],
})
export class ZoneComponent {
  readonly #contentService = inject(ContentService);
  readonly key = input.required<string>();
  readonly languageCode = input("");
  readonly sanitize = input(false);
  readonly showContentStripWarning = input(false);
  readonly showAccessWarnings = input(false);
  readonly content = computed(() => this.#contentService.getPublishedStaticContent(this.key(), this.languageCode()));
  readonly asContent = TypeHelpers.cast<PublishedContent>;
}
