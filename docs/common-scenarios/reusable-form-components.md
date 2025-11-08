Create a new guide explaining how to build reusable form components in LightNap:

# Creating Reusable Form Components

LightNap uses a pattern of creating reusable form components that can be composed together. This guide demonstrates the pattern using examples from the codebase.

## The SelectListItem Pattern

For dropdown/select components, LightNap uses a consistent pattern:

1. **SelectListItemComponent**: A presentation component that displays list items with optional descriptions
2. **Domain-specific picker components**: Components that implement ControlValueAccessor for specific use cases
3. **Generic setting components**: Reusable components for common patterns like user settings

### Example: SelectListItemComponent

[Show the component code and explain how it's used in templates]

### Example: Creating a Domain-Specific Picker

[Show ContentFormatPickerComponent as an example of implementing ControlValueAccessor]

### Example: Creating a Generic Setting Component

[Show UserSettingSelectComponent as an example of a reusable component that works with any user setting]

## Best Practices

1. Use SelectListItemComponent for consistent dropdown item rendering
2. Implement ControlValueAccessor for components that need to work with Angular forms
3. Create generic components for common patterns (like user settings)
4. Create domain-specific components that compose the generic ones
5. Use the ListItem<T> model for type-safe option lists