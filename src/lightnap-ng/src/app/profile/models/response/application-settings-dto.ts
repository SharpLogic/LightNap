import { ExtendedSettingsDto, FeaturesSettingsDto, PreferencesSettingsDto } from "../settings";
import { LayoutConfigDto } from "../settings/layout-config-dto";

/**
 * Represents the settings configuration for this app.
 */
export interface ApplicationSettingsDto {
    /**
     * Extended settings for the app not covered by other settings.
     */
    extended: ExtendedSettingsDto;

    /**
     * Feature-specific settings for the app.
     */
    features: FeaturesSettingsDto;

    /**
     * User preferences settings for the app.
     */
    preferences: PreferencesSettingsDto;

    /**
     * Style-related settings for the app.
     */
    style: LayoutConfigDto;
}
