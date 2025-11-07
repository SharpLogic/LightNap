namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Contains constant values used in the configuration of the core library.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Contains constant values related to the Identity service.
        /// </summary>
        internal static class Identity
        {
            /// <summary>
            /// The purpose string used to create and validate magic link tokens.
            /// </summary>
            public const string MagicLinkTokenPurpose = "MagicLink";

            /// <summary>
            /// The hardcoded user ID for system-generated actions. This will not map to a user in the database,
            /// so it needs to be accounted for wherever user IDs are used that may be the system, such as content creation.
            /// </summary>
            public const string SystemUserId = "system";
        }

        /// <summary>
        /// Role names used in the application.
        /// </summary>
        public static class Roles
        {
            /// <summary>
            /// The name of the administrator role.
            /// </summary>
            public const string Administrator = "Administrator";

            /// <summary>
            /// The name of the content editor role.
            /// </summary>
            public const string ContentEditor = "ContentEditor";
        }

        /// <summary>
        /// Claim types used in the application.
        /// </summary>
        public static class Claims
        {
            /// <summary>
            /// The name of the static content editor claim.
            /// </summary>
            public const string ContentEditor = "Content:Editor";

            /// <summary>
            /// The name of the static content reader claim.
            /// </summary>
            public const string ContentReader = "Content:Reader";
        }

        /// <summary>
        /// Contains constant values related to DTO lengths.
        /// </summary>
        internal static class Dto
        {
            public const int MaxLoginLength = 256;
            public const int MaxPasswordLength = 256;
            public const int MaxDeviceDetailsLength = 512;
            public const int MaxPasswordResetTokenLength = 512;
            public const int MaxStaticContentKeyLength = 64;
            public const int MaxUserNameLength = 32;
            public const int MaxVerificationCodeLength = 512;
        }

        /// <summary>
        /// Contains constant values related to cookies.
        /// </summary>
        internal class Cookies
        {
            /// <summary>
            /// The name of the refresh token cookie.
            /// </summary>
            public const string RefreshToken = "refreshToken";
        }

        /// <summary>
        /// Contains constant values related to refresh tokens.
        /// </summary>
        internal class RefreshTokens
        {
            public const string NoIpProvided = "No IP Provided";
        }

        /// <summary>
        /// Contains keys used by user settings.
        /// </summary>
        public class UserSettingKeys
        {
            /// <summary>
            /// The JSON data set by the browser for its layout and styling preferences. This is all handled by the browser app
            /// and we just store and return the data it sends.
            /// </summary>
            public const string BrowserSettings = "BrowserSettings";

            /// <summary>
            /// The user's preferred language code for content. Can be empty for auto-detection from browser.
            /// </summary>
            public const string PreferredLanguage = "PreferredLanguage";
        }

    }
}
