using LightNap.Core.Captcha.Dto.Response;
using LightNap.Core.Public.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for handling publicly accessible data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PublicController(IPublicService publicService) : ControllerBase
    {
        /// <summary>
        /// Returns the browser-safe CAPTCHA configuration. SPAs call this once on bootstrap
        /// (or lazily before rendering a protected form) to learn which provider is active and
        /// to obtain the public site key for widget rendering.
        /// </summary>
        /// <returns>The current CAPTCHA client configuration.</returns>
        [HttpGet("captcha-config")]
        public ActionResult<CaptchaClientConfigDto> GetCaptchaConfig()
            => this.Ok(publicService.GetCaptchaConfig());
    }
}
