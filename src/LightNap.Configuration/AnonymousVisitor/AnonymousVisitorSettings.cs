namespace LightNap.Configuration.AnonymousVisitor
{
    /// <summary>
    /// Settings for the anonymous visitor tracking middleware.
    /// </summary>
    public sealed class AnonymousVisitorSettings
    {
        /// <summary>
        /// The name of the first-party cookie that stores the visitor identifier.
        /// </summary>
        public string CookieName { get; set; } = "lna_visitor_id";

        /// <summary>
        /// How long the visitor cookie persists. Defaults to one year.
        /// </summary>
        public TimeSpan Lifetime { get; set; } = TimeSpan.FromDays(365);

        /// <summary>
        /// When <c>true</c>, the cookie is set with <c>Secure</c> (HTTPS only). Default <c>true</c>;
        /// set <c>false</c> only for local development over HTTP.
        /// </summary>
        public bool SecureOnly { get; set; } = true;
    }
}
