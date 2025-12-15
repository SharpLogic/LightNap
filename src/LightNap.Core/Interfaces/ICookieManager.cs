namespace LightNap.Core.Interfaces
{
    /// <summary>  
    /// Provides methods to manage cookies.  
    /// </summary>  
    public interface ICookieManager
    {
        /// <summary>
        /// Sets a cookie.
        /// </summary>
        /// <param name="key">The cookie key.</param>
        /// <param name="value">The cookei value.</param>
        /// <param name="isPersistent">Whether the ookie should persist across sessions.</param>
        /// <param name="expires">When the cookie expires.</param>
        void SetCookie(string key, string value, bool isPersistent, DateTime expires);

        /// <summary>
        /// Gets the cookie, if available.
        /// </summary>
        /// <param name="key">The keyu of the cookie.</param>
        /// <returns>The string value of the cookie, if available.</returns>
        string? GetCookie(string key);

        /// <summary>
        /// Clears the specified cookie.
        /// </summary>
        /// <param name="key">he key of the cookie.</param>
        void RemoveCookie(string key);
    }

}