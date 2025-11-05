import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { Card } from "primeng/card";
import { Panel } from "primeng/panel";
import { CmsElementDefinition } from "./cms-element-definition";
import { ZoneComponent } from "../components/zone/zone.component";

/**
 * Definitions for CMS elements available in the system. Add more items here to make them available for
 * use in static content pages and zones.
 */
export const CMS_ELEMENTS: CmsElementDefinition[] = [
  {
    tagName: "cms-branded-card",
    component: BrandedCardComponent,
    displayName: "Branded Card",
    description: "A card component with this site's branding and styling",
    inputs: [
      {
        name: "title",
        type: "string",
        description: "Card title text",
        required: false,
      },
      {
        name: "subtitle",
        type: "string",
        description: "Card subtitle text",
        required: false,
      },
    ],
    example: `<cms-branded-card>
  <span title>Card Title</span>
  <span subtitle>Card Subtitle</span>
  <p>Your content here</p>
</cms-branded-card>`,
  },
  {
    tagName: "cms-panel",
    component: Panel,
    displayName: "Panel",
    description: "Collapsible panel container from PrimeNG",
    inputs: [
      {
        name: "header",
        type: "string",
        description: "Panel header text",
        required: false,
      },
      {
        name: "toggleable",
        type: "boolean",
        description: "Whether the panel can be collapsed",
        required: false,
        default: "false",
      },
      {
        name: "collapsed",
        type: "boolean",
        description: "Initial collapsed state",
        required: false,
        default: "false",
      },
    ],
    example: `<cms-panel header="Section Title" toggleable="true">
  <p>Panel content</p>
</cms-panel>`,
  },
  {
    tagName: "cms-card",
    component: Card,
    displayName: "Card",
    description: "Basic card container from PrimeNG",
    inputs: [
      {
        name: "header",
        type: "string",
        description: "Card header text",
        required: false,
      },
      {
        name: "subheader",
        type: "string",
        description: "Card subheader text",
        required: false,
      },
    ],
    example: `<cms-card header="Title" subheader="Subtitle">
  <p>Card content</p>
</cms-card>`,
  },
  {
    tagName: "cms-zone",
    component: ZoneComponent,
    displayName: "Content Zone",
    description: "Dynamic content zone that loads content from the CMS",
    inputs: [
      {
        name: "key",
        type: "string",
        description: "Zone name identifier",
        required: true,
      },
      {
        name: "language-code",
        type: "string",
        description: "Language code for content localization",
        required: false,
        default: "en",
      },
      {
        name: "sanitize",
        type: "boolean",
        description: "Whether to sanitize the content to strip out potentially unsafe HTML",
        required: false,
        default: "false",
      },
      {
        name: "show-content-strip-warning",
        type: "boolean",
        description: "Whether to show a warning when content has been stripped during sanitization",
        required: false,
        default: "false",
      },
    ],
    example: `<ln-zone key="sidebar"></ln-zone>`,
  },
];
