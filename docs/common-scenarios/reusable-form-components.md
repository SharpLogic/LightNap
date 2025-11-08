---
title: Reusable Form Components
layout: home
parent: Common Scenarios
nav_order: 950
---

# {{ page.title }}

LightNap demonstrates a powerful pattern for creating reusable form components that combine presentation with data management. This article explores the architectural pattern used throughout the application, showing how to build composable, type-safe components that reduce boilerplate and maintain consistency.

- TOC
{:toc}

## Understanding the Component Pattern

LightNap uses a layered approach to form components:

1. **Presentation Layer** (`SelectListItemComponent`): Renders individual items in consistent ways
2. **Control Layer** (`UserSettingSelectComponent`): Manages data binding and persistence
3. **Feature Layer** (`PreferredLanguageSelectComponent`): Combines control with domain logic

This separation creates highly reusable components that can be composed together to build complex forms with minimal code duplication.

## The ListItem Model

At the foundation is the `ListItem<T>` class, which provides a consistent interface for working with PrimeNG collection controls:

```typescript
/**
 * Represents an item in a list to make it easier to work with PrimeNG collection controls.
 *
 * @template T - The type of the value.
 */
export class ListItem<T> {
    constructor(
        public value: T,
        public label: string,
        public description?: string
    ) {}
}
```

This simple model provides:

- **Type Safety**: Generic `T` ensures value type consistency
- **Display Text**: `label` for the primary text shown to users
- **Optional Details**: `description` for additional context
- **PrimeNG Integration**: Works seamlessly with PrimeNG's `optionLabel` and `optionValue` bindings

## Presentation Layer: SelectListItemComponent

The `SelectListItemComponent` is a presentation component that renders `ListItem` data in a consistent format across all dropdowns and list controls.

### Component Implementation

```typescript
@Component({
    selector: 'ln-select-list-item',
    templateUrl: './select-list-item.component.html',
    imports: [],
    standalone: true,
})
export class SelectListItemComponent {
    @Input() label = signal("");
    @Input() description = signal<string | undefined>("");

    @Input() set listItem(value: ListItem<any>) {
        this.label.set(value.label);
        this.description.set(value.description);
    }
}
```

### Template

```html
<div class="inline-block mx-1 w-full">
  <span class="m-0">{{ label() }}</span>
  @if (description(); as description) {
    <p class="m-0 whitespace-nowrap text-ellipsis overflow-hidden">
      {{ description }}
    </p>
  }
</div>
```

### Key Features

- **Signal-Based Reactivity**: Uses Angular signals for efficient change detection
- **Optional Description**: Shows additional context when available
- **Consistent Styling**: Provides uniform appearance across the application
- **Flexible Input**: Accepts either individual properties or a `ListItem` object

### Usage in PrimeNG Controls

```html
<p-select
  [options]="items()"
  optionLabel="label"
  optionValue="value"
>
  <ng-template let-option #item>
    <ln-select-list-item [listItem]="option" />
  </ng-template>
</p-select>
```

## Control Layer: UserSettingSelectComponent

The `UserSettingSelectComponent` is a smart component that combines a select dropdown with automatic persistence to user settings.

### TypeScript Implementation

```typescript
@Component({
  selector: "ln-user-setting-select",
  standalone: true,
  templateUrl: "./user-setting-select.component.html",
  imports: [CommonModule, FormsModule, Select, SelectListItemComponent, ApiResponseComponent],
})
export class UserSettingSelectComponent<T> implements OnChanges {
  readonly #profileService = inject(ProfileService);
  readonly #toast = inject(ToastService);

  readonly key = input.required<UserSettingKey>();
  readonly label = input.required<string>();
  readonly options = input.required<Array<ListItem<T>>>();
  readonly setting = signal(new Observable<T>());
  readonly defaultValue = input<T>();

  ngOnChanges() {
    this.setting.set(this.#profileService.getSetting<T>(this.key(), this.defaultValue()));
  }

  onChange(value: T) {
    this.#profileService.setSetting(this.key(), value).subscribe({
      next: () => this.#toast.success("Setting updated."),
      error: () => this.#toast.error("Failed to update setting."),
    });
  }
}
```

### HTML Template

```html
<ln-api-response [apiResponse]="setting()">
  <ng-template #success let-value>
    <p-select
      [ngModel]="value"
      (ngModelChange)="onChange($event)"
      [options]="options()"
      optionLabel="label"
      optionValue="value"
      class="w-full"
    >
      <ng-template let-option #item>
        <ln-select-list-item [listItem]="option" />
      </ng-template>
    </p-select>
  </ng-template>
</ln-api-response>
```

### Component Features

- **Generic Type Support**: Type parameter `T` ensures type safety for different value types
- **Automatic Loading**: Fetches current setting value on initialization
- **Automatic Persistence**: Saves changes immediately when user selects an option
- **Error Handling**: Shows toast notifications for success/failure
- **Loading States**: Uses `ApiResponseComponent` to handle loading and error states
- **Default Values**: Supports default values when no setting exists

### Usage Example

```html
<ln-user-setting-select
  key="PreferredLanguage"
  label="Preferred Language"
  [options]="languageOptions()"
  [defaultValue]="''" />
```

## Feature Layer: Domain-Specific Components

Feature-layer components combine the control layer with domain-specific logic and data sources.

### PreferredLanguageSelectComponent Example

This component demonstrates how to create a specialized setting component:

```typescript
@Component({
  selector: "ln-preferred-language-select",
  standalone: true,
  templateUrl: "./preferred-language-select.component.html",
  imports: [CommonModule, UserSettingSelectComponent, ApiResponseComponent],
})
export class PreferredLanguageSelectComponent {
  readonly #contentService = inject(ContentService);

  readonly supportedLanguages = signal(
    this.#contentService
      .getSupportedLanguages()
      .pipe(map(languages => [
        new ListItem("", "Auto-detect"),
        ...languages.map(lang => new ListItem(lang.languageCode, lang.languageName))
      ]))
  );
}
```

### Component Template

```html
<ln-api-response [apiResponse]="supportedLanguages()">
  <ng-template #success let-supportedLanguages>
    <ln-user-setting-select
      key="PreferredLanguage"
      label="Preferred Language"
      [options]="supportedLanguages"
      [defaultValue]="null" />
  </ng-template>
</ln-api-response>
```

### What This Demonstrates

- **Service Integration**: Fetches available languages from `ContentService`
- **Data Transformation**: Converts API response to `ListItem` array
- **Special Values**: Includes "Auto-detect" option with empty string value
- **Single Responsibility**: Component only handles data preparation, delegates persistence to `UserSettingSelectComponent`
- **Composability**: Can be dropped into any form without additional configuration

### Usage

```html
<!-- In a profile form -->
<div class="setting-group">
  <h3>Language Preferences</h3>
  <ln-preferred-language-select />
</div>
```

## Similar Patterns in LightNap

LightNap uses this pattern consistently throughout the codebase. Here are other examples you can reference:

### RolePickerComponent

Single role selection for user management:

```typescript
@Component({
  selector: "ln-role-picker",
  template: `
    <p-select
      [ngModel]="value()"
      (ngModelChange)="valueChange.emit($event)"
      [options]="roles()"
      optionLabel="label"
      optionValue="value"
      placeholder="Select a role...">
      <ng-template let-role #item>
        <ln-select-list-item [listItem]="role" />
      </ng-template>
    </p-select>
  `,
  imports: [CommonModule, SelectModule, FormsModule, SelectListItemComponent],
})
export class RolePickerComponent {
  readonly value = input<string>();
  readonly valueChange = output<string>();

  readonly roles = input<Array<ListItem<string>>>([]);
}
```

### ContentFormatPickerComponent

Selecting content format (Markdown, HTML, etc.):

```typescript
@Component({
  selector: "ln-content-format-picker",
  template: `
    <p-select
      [ngModel]="contentFormat()"
      (ngModelChange)="contentFormatChange.emit($event)"
      [options]="contentFormatOptions"
      optionLabel="label"
      optionValue="value">
      <ng-template let-option #item>
        <ln-select-list-item [listItem]="option" />
      </ng-template>
    </p-select>
  `,
  imports: [SelectModule, FormsModule, SelectListItemComponent],
})
export class ContentFormatPickerComponent {
  readonly contentFormat = input<ContentFormat>(ContentFormat.Markdown);
  readonly contentFormatChange = output<ContentFormat>();

  readonly contentFormatOptions = [
    new ListItem(ContentFormat.Markdown, "Markdown"),
    new ListItem(ContentFormat.Html, "HTML"),
    new ListItem(ContentFormat.PlainText, "Plain Text"),
  ];
}
```

## Creating Your Own Reusable Components

Follow this pattern when building your own form components:

### 1. Identify Reusable Patterns

Look for forms controls that:

- Appear in multiple places
- Have complex logic (validation, formatting, etc.)
- Integrate with services
- Need consistent styling

### 2. Choose the Right Layer

- **Presentation Layer**: Pure display components with no business logic
- **Control Layer**: Generic controls that manage state and persistence
- **Feature Layer**: Domain-specific components that combine data and controls

### 3. Use Generic Types When Appropriate

```typescript
export class MySelectComponent<T> {
  readonly options = input.required<Array<ListItem<T>>>();
  readonly value = input<T>();
  readonly valueChange = output<T>();
}
```

### 4. Leverage Dependency Injection

Inject services at the appropriate layer:

```typescript
readonly #myService = inject(MyService);
readonly #toast = inject(ToastService);
```

### 5. Handle Loading and Error States

Use `ApiResponseComponent` for observable data:

```html
<ln-api-response [apiResponse]="data()">
  <ng-template #success let-data>
    <!-- Your control here -->
  </ng-template>
</ln-api-response>
```

### 6. Keep Components Focused

Each component should have a single, clear purpose:

- ✅ `LanguageSelectComponent`: Selects a language
- ❌ `LanguageAndTimeZoneComponent`: Too broad, split into two components

### 7. Make Components Self-Contained

Include all necessary imports in the component's `imports` array:

```typescript
@Component({
  selector: "ln-my-component",
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Select,
    SelectListItemComponent,
    ApiResponseComponent
  ],
})
```

## Benefits of This Pattern

### Consistency

All form controls across the application look and behave the same way, creating a cohesive user experience.

### Reduced Boilerplate

Complex functionality is encapsulated in reusable components:

```html
<!-- Without pattern: ~50 lines of template + component code -->
<!-- With pattern: -->
<ln-preferred-language-select />
```

### Type Safety

Generic types ensure compile-time type checking:

```typescript
// This would cause a TypeScript error:
const items: Array<ListItem<string>> = [...];
const component: UserSettingSelectComponent<number> = ...;
component.options = items; // Error: Type mismatch
```

### Testability

Each layer can be tested independently:

- Test `SelectListItemComponent` for proper rendering
- Test `UserSettingSelectComponent` for persistence logic
- Test `PreferredLanguageSelectComponent` for data transformation

### Maintainability

Changes to common functionality propagate automatically:

- Update `SelectListItemComponent` styling → affects all dropdowns
- Improve `UserSettingSelectComponent` error handling → affects all settings
- Add new language → automatically appears in all language selectors

## Additional Resources

### Related Documentation

- [Adding User Settings](./adding-user-settings) - Implementing user preferences
- [Adding Entities](./adding-entities) - Creating database entities
- [Solution & Project Structure](../concepts/project-structure) - Understanding the architecture

### PrimeNG Documentation

- [PrimeNG Select Component](https://primeng.org/select)
- [PrimeNG MultiSelect Component](https://primeng.org/multiselect)

### Angular Patterns

- [Angular Signals](https://angular.dev/guide/signals)
- [Standalone Components](https://angular.dev/guide/components/importing)
- [Dependency Injection](https://angular.dev/guide/di)

## See Also

- [Adding User Settings](./adding-user-settings) - User preferences and settings
- [Adding Profile Fields](./adding-profile-fields) - User-specific data
- [Solution & Project Structure](../concepts/project-structure) - Architecture overview
