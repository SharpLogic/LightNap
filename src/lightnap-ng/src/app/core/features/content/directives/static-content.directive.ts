import { Directive, effect, ElementRef, inject, input, Renderer2, SecurityContext } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { StaticContentFormats } from "@core/backend-api";
import { Marked } from "marked";

@Directive({
  selector: "[staticContent]",
})
export class StaticContentDirective {
  readonly #sanitizer = inject(DomSanitizer);
  readonly #elementRef = inject(ElementRef);
  readonly #renderer = inject(Renderer2);

  readonly content = input.required<string>();
  readonly format = input.required<StaticContentFormats>();
  readonly sanitize = input<boolean>(false);
  readonly showContentStripWarning = input<boolean>(false);

  readonly #marked = new Marked();

  constructor() {
    effect(() => {
      const untrusted = this.content();
      if (untrusted?.trim().length === 0) {
        this.#renderer.setProperty(this.#elementRef.nativeElement, "innerHTML", "");
        return;
      }

      const untrustedHtml = this.#getUntrustedHtml(untrusted);
      const trustedHtml = this.#getTrustedHtml(untrustedHtml);

      switch (this.format()) {
        case "Html":
        case "Markdown":
          this.#renderer.setProperty(this.#elementRef.nativeElement, "innerHTML", trustedHtml);
          break;
        case "PlainText":
          const textNode = this.#renderer.createText(untrustedHtml);
          this.#renderer.setProperty(this.#elementRef.nativeElement, "innerHTML", "");
          this.#renderer.appendChild(this.#elementRef.nativeElement, textNode);
          break;
        default:
          throw new Error(`Unsupported content format: '${this.format()}'`);
      }

      if (this.showContentStripWarning() && this.#wasContentStripped(untrustedHtml, trustedHtml)) {
        const warning = this.#renderer.createElement("p");
        this.#renderer.addClass(warning, "text-red-500");
        this.#renderer.addClass(warning, "italic");
        this.#renderer.setProperty(warning, "textContent", "*Content was stripped during sanitization.");
        this.#renderer.appendChild(this.#elementRef.nativeElement, warning);
      }
    });
  }

  #getUntrustedHtml(untrusted: string): string {
    switch (this.format()) {
      case "Html":
      case "PlainText":
        return untrusted;
      case "Markdown":
        return this.#marked.parse(untrusted) as string;
      default:
        throw new Error(`Unsupported content format: '${this.format()}'`);
    }
  }

  #getTrustedHtml(untrusted: string): string {
    if (this.sanitize()) {
      return this.#sanitizer.sanitize(SecurityContext.HTML, untrusted) ?? "The provided content could not be sanitized.";
    }
    return untrusted;
  }

  #wasContentStripped(untrustedHtml: string, trustedHtml: string): boolean {
    const untrustedNormalized = untrustedHtml.replace(/\s+/g, " ").trim();
    const trustedNormalized = trustedHtml.replace(/&#10;/g, " ").replace(/\s+/g, " ").trim();
    return untrustedNormalized !== trustedNormalized;
  }
}
