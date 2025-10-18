import { CommonModule } from "@angular/common";
import { Component, computed, inject, input, SecurityContext, signal } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { StaticContentTypes } from "@core";
import { MarkdownService } from "ngx-markdown";

@Component({
  selector: "static-content",
  templateUrl: "./static-content.component.html",
  imports: [CommonModule],
})
export class StaticContentComponent {
  readonly #sanitizer = inject(DomSanitizer);
  readonly #markdown = inject(MarkdownService);

  readonly content = input.required<string>();
  readonly contentType = input.required<StaticContentTypes>();
  readonly bypassSanitization = input<boolean>(false);
  readonly contentStripped = signal(false);

  html = computed(() => {
    const untrusted = this.content();
    if (untrusted?.trim().length === 0) return "";

    let html: string;

    switch (this.contentType()) {
      case "Html":
        html = untrusted;
        break;
      case "Markdown":
        // NOTE: This assumes there are no async extensions. If one is added, then
        // parse() will return a promise and this whole signal will need to be updated.
        html = this.#markdown.parse(this.content()) as string;
        break;
      default:
        throw new Error(`Unsupported content type: '${this.contentType()}'`);
    }

    if (!this.bypassSanitization()) {
      // TODO: Potentially leverage DOMPurify to further sanitize untrusted content.

      const normalized = (s: string) => s.replace(/\s+/g, " ").trim();
      const expectedHtml = normalized(html);

      html = this.#sanitizer.sanitize(SecurityContext.HTML, html) ?? "The provided content could not be sanitized.";

      // TODO: Can't set a signal inside of a computed signal, so this needs to move out.
//      this.contentStripped.set(normalized(html) !== expectedHtml);
    }

    const safeHtml = this.#sanitizer.bypassSecurityTrustHtml(html);
    return safeHtml;
  });
}
