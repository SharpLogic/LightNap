using LightNap.Core.Configuration;
using LightNap.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace LightNap.WebApi.Services
{
    /// <summary>
    /// Manages web cookies.
    /// </summary>
    public class WebCookieManager(IHttpContextAccessor httpContextAccessor) : ICookieManager
    {
        /// <summary>
        /// Sets a cookie with the specified key and value.
        /// </summary>
        /// <param name="key">The key of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="isPersistent">Indicates whether the cookie is persistent.</param>
        /// <param name="expires">The expiration date of the cookie.</param>
        public void SetCookie(string key, string value, bool isPersistent, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            };

            if (isPersistent)
            {
                cookieOptions.Expires = expires;
            }

            httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
        }

        /// <summary>
        /// Gets the value of the cookie with the specified key.
        /// </summary>
        /// <param name="key">The key of the cookie.</param>
        /// <returns>The value of the cookie, or null if the cookie does not exist.</returns>
        public string? GetCookie(string key)
        {
            return httpContextAccessor.HttpContext?.Request.Cookies[key];
        }

        /// <summary>
        /// Removes the cookie with the specified key.
        /// </summary>
        /// <param name="key">The key of the cookie.</param>
        public void RemoveCookie(string key)
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }

}
