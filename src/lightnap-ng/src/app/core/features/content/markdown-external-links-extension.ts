import type { MarkedExtension } from "marked";

const ABSOLUTE_URL = /^(?:[a-z][a-z0-9+.-]*:|\/\/)/i;
const ANCHOR_TAG = /<a\b([^>]*)>/gi;
const HREF_ATTR = /\bhref\s*=\s*(?:"([^"]*)"|'([^']*)')/i;
const TARGET_ATTR = /\btarget\s*=/i;

/**
 * marked extension that rewrites anchor tags whose href is an absolute URL to open in a new tab
 * with rel="noopener noreferrer". Anchors that already declare a target attribute are left alone.
 *
 * Wire it through provideMarkdown:
 *   providers: [
 *     provideMarkdown({ markedExtensions: [{ provide: MARKED_EXTENSIONS, useValue: externalLinksMarkedExtension, multi: true }] }),
 *   ]
 */
export const externalLinksMarkedExtension: MarkedExtension = {
  hooks: {
    postprocess(html: string): string {
      return html.replace(ANCHOR_TAG, (match, attrs: string) => {
        const hrefMatch = HREF_ATTR.exec(attrs);
        const href = hrefMatch?.[1] ?? hrefMatch?.[2];
        if (!href || !ABSOLUTE_URL.test(href) || TARGET_ATTR.test(attrs)) {
          return match;
        }
        return `<a${attrs} target="_blank" rel="noopener noreferrer">`;
      });
    },
  },
};
