Update the example in the 'Adding User Settings' guide to use PreferredLanguage instead of BrowserSettings. This provides a better demonstration of:

1. How to add a user setting with a meaningful default value (empty string for auto-detect)
2. How to create a reusable select component for user settings (UserSettingSelectComponent)
3. How to integrate user settings with other services (ContentService for language detection)
4. How to implement fallback logic (browser language detection when setting is empty)

The updated example should show:
- Adding the constant in Constants.cs
- Registering in UserSettingsConfig.cs with proper JSON default value
- Creating the Angular UserSettingKey
- Building a reusable UserSettingSelectComponent that can be used for any select-based setting
- Creating a domain-specific component (PreferredLanguageSelectComponent) that uses the reusable component
- Integrating the setting into the ContentService for automatic language selection

This demonstrates a more complete pattern that developers can follow for their own settings.