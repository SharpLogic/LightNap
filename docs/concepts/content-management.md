---
title: Content Management System
layout: default
parent: Concepts
nav_order: 301
---

# Content Management System (CMS)

LightNap includes a basic built-in Content Management System (CMS) that allows administrators and content editors to create, manage, and publish static content. This enables content changes within the app that do not require changing source code. The CMS also supports multilingual content, role-based access control, and multiple content formats.

## Overview

The CMS is built around **static content** that can be of two types:

- **Pages** - Full page content that can be displayed independently
- **Zones** - Content blocks that can be embedded within other pages or components

All content supports multiple languages and can be published, drafted, or archived with fine-grained access control.

## Content Types

### Pages vs Zones

```typescript
enum StaticContentType {
  Zone = 0,    // Content blocks for embedding
  Page = 1     // Full page content
}
```

**Pages** are standalone content pieces that typically represent full pages in your application. They can be accessed via routes and displayed as complete pages.

**Zones** are smaller content blocks designed to be embedded within other pages or components. They're perfect for:

- Dynamic text blocks within pages
- Localized content sections
- Configurable UI elements
- Marketing banners or announcements

## Content Lifecycle

### Content Status

```typescript
enum StaticContentStatus {
  Draft = 0,       // Work in progress, not publicly visible
  Published = 1,   // Live and accessible to users
  Archived = 2     // Retired content, no longer active
}
```

Content moves through a clear lifecycle:

1. **Draft** - Content is being created or edited, not visible to end users
2. **Published** - Content is live and accessible based on access permissions
3. **Archived** - Content is no longer active but preserved for historical purposes

Only **Published** content is returned by the public API endpoints for non-editors.

## Access Control

### Read Access Levels

```typescript
enum StaticContentReadAccess {
  Public = 0,        // Anyone can read
  Authenticated = 1, // Any logged-in user can read
  Explicit = 2       // Only users with specific permissions
}
```

The CMS provides three levels of read access:

- **Public** - Content is accessible to all users, including anonymous visitors
- **Authenticated** - Content requires users to be logged in
- **Explicit** - Content requires specific claims or roles to access

### Editorial Permissions

Content creation and editing is restricted to users with appropriate roles:

- **Administrator** - Full access to all CMS functions
- **ContentEditor** - Can create, edit, and publish content
- **Custom Claims** - Fine-grained permissions using `Content:Reader` and `Content:Editor` claims

## Content Formats

```typescript
enum StaticContentFormat {
  Html = 0,        // Rich HTML content
  Markdown = 1,    // Markdown that gets converted to HTML
  PlainText = 2    // Plain text content
}
```

The CMS supports multiple content formats:

- **HTML** - Rich formatting with full HTML support
- **Markdown** - Easy-to-write format that converts to HTML
- **PlainText** - Simple text without formatting

## Multilingual Support

The CMS has built-in support for multilingual content:

- Each content item has a unique **key** identifier
- Multiple **language versions** can be created for each content item
- Each language version has its own content, format, and status
- Fallback mechanisms can be implemented for missing translations

### Language Structure

```typescript
interface StaticContentLanguage {
  key: string;           // Content identifier
  languageCode: string;  // ISO language code (e.g., "en", "es", "fr")
  content: string;       // The actual content
  format: StaticContentFormat;
}
```

## API Endpoints

The CMS provides a comprehensive REST API through the `ContentController`:

### Public Endpoints

```http
GET /api/content/published/{key}/{languageCode}
GET /api/content/supported-languages
```

These endpoints are accessible without authentication and return only published content based on access permissions.

### Administrative Endpoints

```http
POST /api/content                           # Create content
GET /api/content/{key}                      # Get content details
POST /api/content/search                    # Search content
PUT /api/content/{key}                      # Update content
DELETE /api/content/{key}                   # Delete content (Admin only)

GET /api/content/{key}/languages            # Get all language versions
GET /api/content/{key}/languages/{lang}     # Get specific language version
POST /api/content/{key}/languages/{lang}    # Create language version
PUT /api/content/{key}/languages/{lang}     # Update language version
DELETE /api/content/{key}/languages/{lang}  # Delete language version (Admin only)
```

Administrative endpoints require authentication and appropriate permissions.

## Seeding content

The seeding system uses a directory structure within the `LightNap.WebApi` project to automatically determine default content properties:

### Directory Structure Conventions

```bash
StaticContent/
├── pages/
│   ├── public/
│   │   ├── privacy-policy/
│   │   │   ├── en.html
│   │   │   ├── es.md
│   │   │   └── fr.txt
│   │   └── terms-of-service/
│   │       └── en.html
│   ├── authenticated/
│   │   └── user-guide/
│   │       └── en.md
│   └── explicit/
│       └── admin-dashboard-help/
│           └── en.html
└── zones/
  ├── public/
  │   ├── welcome-banner/
  │   │   ├── en.html
  │   │   └── es.html
  │   └── footer-notice/
  │       └── en.txt
  ├── authenticated/
  │   └── member-benefits/
  │       └── en.md
  └── explicit/
      └── admin-alert/
          └── en.html
```

### Path Component Mapping

Each part of the file path maps to specific content properties:

- **First Level** (`pages`/`zones`) → **Type**
  - `pages/` creates content with `StaticContentType.Page`
  - `zones/` creates content with `StaticContentType.Zone`

- **Second Level** (`public`/`authenticated`/`explicit`) → **Access Level**
  - `public/` sets `StaticContentReadAccess.Public`
  - `authenticated/` sets `StaticContentReadAccess.Authenticated`
  - `explicit/` sets `StaticContentReadAccess.Explicit`

- **Third Level** (folder name) → **Content Key**
  - The folder name becomes the unique identifier for the content

- **File Name** → **Language and Format**
  - Base name (before extension) is the language code
  - File extension determines the format:
  - `.html` → `StaticContentFormat.Html`
  - `.md` → `StaticContentFormat.Markdown`
  - `.txt` → `StaticContentFormat.PlainText`

### Seeding Examples

| File Path | Type | Access | Key | Language | Format |
|-----------|------|--------|-----|----------|---------|
| `zones/public/public-index-welcome/en.html` | Zone | Public | public-index-welcome | en | Html |
| `pages/authenticated/dashboard/es.md` | Page | Authenticated | dashboard | es | Markdown |
| `zones/explicit/admin-notice/fr.txt` | Zone | Explicit | admin-notice | fr | PlainText |

### Seeding Behavior

- All seeded content starts with `StaticContentStatus.Published`
- Content is automatically created during application startup if it doesn't exist
- If a CMS key already exists, its metadata is not changed
- If a language for a key already exists, it is not changed

## Frontend Integration

### Using Zones in Templates

The Angular frontend provides a `ln-zone` component for embedding content zones:

```html
<!-- Basic zone usage -->
<ln-zone key="welcome-message" />

<!-- Zone with specific language -->
<ln-zone key="welcome-message" languageCode="es" />

<!-- Zone with custom options -->
<ln-zone
  key="terms-notice"
  languageCode="en"
  [sanitize]="true"
  [showAccessWarnings]="true" />
```

### Content Service Features

The `ContentService` provides methods for both public and administrative use:

```typescript
// Public methods
getPublishedStaticContent(key: string, languageCode: string)
getSupportedLanguages()

// Administrative methods
createStaticContent(createDto: CreateStaticContentDto)
updateStaticContent(key: string, updateDto: UpdateStaticContentDto)
deleteStaticContent(key: string)

// Language management
createStaticContentLanguage(key: string, languageCode: string, createDto)
updateStaticContentLanguage(key: string, languageCode: string, updateDto)
getStaticContentLanguages(key: string)

// Permission management
addReader(userId: string, key: string)
removeReader(userId: string, key: string)
addEditor(userId: string, key: string)
removeEditor(userId: string, key: string)
```

### Custom Elements

Angular components can be exposed for use in CMS content by registering them in the `app/core/features/content/models/cms-elements.ts` list. This allows them to be used within CMS language content in **HTML** or **Markdown**.

For example:

```html
<!-- HTML -->
<cms-panel header="Welcome To The App">
    <p>cms-panel maps to PrimeNG's p-panel.</p>
</cms-panel>
```

```markdown
<!-- Markdown -->
# Welcome to this Markdown page

<cms-panel header="Welcome To The App">
    <p>Content inside elements still needs to be HTML</p>
</cms-panel>
```

Supported elements are prefixed with `cms-` and include:

- `cms-branded-card`: LightNap's `ln-branded-card` branded card
- `cms-zone`: LightNap's `ln-zone` that allows for nested zones
- `cms-panel`: PrimeNG's `p-panel`
- `cms-card`: PrimeNG's `p-card`

Additional custom elements can easily be supported using the patterns in the above, but test thoroughly due to subtle differences in behavior when used relative to traditional Angular usage, such as camel-cased attributes requiring kebab casing.

```HTML
<!-- When used from Angular -->
<cms-my-control userId="123"></cms-my-control>

<!-- When used from CMS custom element loading -->
<cms-my-control user-id="123"></cms-my-control>
```

### Caching

The `ContentService` implements intelligent caching:

- Published content is cached per key/language combination
- Cache is automatically cleared for editors when they update the content
  - Cached content for other users does not clear until the browser refreshes
- Caching improves performance for frequently accessed content

## Common Use Cases

### Marketing Content

Use zones for dynamic marketing content that can be updated without code changes:

```html
<ln-zone key="hero-banner" />
<ln-zone key="promotional-message" />
```

### Legal Pages

Create pages for terms of service, privacy policies, etc.:

```typescript
// Route to a content page
{ path: 'terms', component: ContentPageComponent,
  data: { contentKey: 'terms-of-service' } }
```

### Multi-Language Content

Provide content in multiple languages:

```html
<ln-zone key="welcome-message" [languageCode]="currentLanguage" />
```

### Role-Based Content

Show different content based on user roles or permissions:

```typescript
// Content with explicit access control
{
  key: 'admin-notice',
  readAccess: StaticContentReadAccess.Explicit,
  readerRoles: 'Administrator,Moderator'
}
```

## Best Practices

### Content Organization

- Use descriptive, consistent naming for content keys
- Group related content with common prefixes (e.g., `legal-privacy`, `legal-terms`)
- Keep zone content focused and concise
- Use pages for substantial, standalone content

### Permission Management

- Start with the most restrictive access level needed
- Use explicit access control for sensitive content
- Regularly audit content permissions
- Document which roles can access specific content

### Multilingual Strategy

- Plan your supported languages early
- Create consistent language codes (ISO standard)
- Implement fallback strategies for missing translations
- Consider right-to-left language support if needed

### Performance

- Leverage the built-in caching for frequently accessed content
- Keep content sizes reasonable for better loading times
- Use appropriate content formats (Markdown for simple content, HTML for rich formatting)

## Security Considerations

- Content creation/editing requires authentication and appropriate roles
- Published content respects access control settings
- Administrative functions are restricted to appropriate roles
- Content is validated and sanitized before storage

The CMS provides a powerful foundation for managing dynamic content while maintaining security and performance standards throughout your LightNap application.
