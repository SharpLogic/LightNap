import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, SecurityContext } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { StaticContentTypes } from "@core";
import { Marked } from "marked";

@Component({
  selector: "static-content",
  templateUrl: "./static-content.component.html",
  imports: [CommonModule],
})
export class StaticContentComponent {
  readonly #sanitizer = inject(DomSanitizer);

  readonly content = input.required<string>();
  readonly contentType = input.required<StaticContentTypes>();
  readonly bypassSanitization = input<boolean>(false);

  readonly #marked = new Marked();

  readonly #untrustedHtml = computed(() => {
    const untrusted = this.content();
    if (untrusted?.trim().length === 0) return "";

    switch (this.contentType()) {
      case "Html":
        return untrusted;
      case "Markdown":
        return this.#marked.parse(untrusted) as string;
      default:
        throw new Error(`Unsupported content type: '${this.contentType()}'`);
    }
  });

  readonly #trustedHtml = computed(() => {
    const untrusted = this.#untrustedHtml();
    if (this.bypassSanitization()) {
      return untrusted;
    }
    return this.#sanitizer.sanitize(SecurityContext.HTML, untrusted) ?? "The provided content could not be sanitized.";
  });

  readonly html = computed(() => this.#sanitizer.bypassSecurityTrustHtml(this.#trustedHtml()));

  readonly contentStripped = computed(() => {
    const untrustedHtml = this.#untrustedHtml().replace(/\s+/g, " ").trim();
    const trustedHtml = this.#trustedHtml().replace(/&#10;/g, " ").replace(/\s+/g, " ").trim();
    return untrustedHtml !== trustedHtml;
  });
}
