---
title: CAPTCHA Verification
layout: home
parent: Concepts
nav_order: 700
---

# {{ page.title }}

Public-write surfaces — signup, password reset, contact forms, anonymous submissions — need bot mitigation. LightNap provides an `ICaptchaService` abstraction with four implementations and a `[ValidateCaptcha]` action filter for declarative per-endpoint protection.

## Providers

| Provider           | When to pick it                                                                   |
|--------------------|-----------------------------------------------------------------------------------|
| `None` (default)   | Local development, automated tests. NoOp implementation always succeeds.          |
| `Turnstile`        | Privacy-conscious deployments. Free tier, no CAPTCHA puzzle.                      |
| `RecaptchaV2`      | Existing Google Cloud accounts; binary success/failure with a visible challenge.  |
| `RecaptchaV3`      | Score-based; no UI; you set a minimum score threshold.                            |

Selection is via `Captcha:Provider` in configuration. Switching providers does not require code changes — only the matching settings sub-section.

## Configuration

```jsonc
"Captcha": {
  "Provider": "Turnstile",
  "RejectOnProviderError": true,
  "Turnstile": {
    "SiteKey": "1x00000000000000000000AA",
    "SecretKey": "1x0000000000000000000000000000000AA"
  }
}
```

For `RecaptchaV2` / `RecaptchaV3`, supply the matching sub-section and (for v3) a `MinScore`.

### Fail-closed vs fail-open

`RejectOnProviderError` controls behavior when the upstream provider is unreachable:

- `true` (default, fail-closed): the request is rejected. Safer; prioritizes bot mitigation over availability.
- `false` (fail-open): the request is accepted. Prioritizes availability over strict bot mitigation. Pick this when an outage of the CAPTCHA provider would unacceptably break user signup.

The underlying `HttpClient` for each provider is wired through [LightNap's resilience helper](./http-resilience), so the standard retry / circuit-breaker / timeout policies apply before this flag ever kicks in.

## Protecting an endpoint

Decorate any controller action with `[ValidateCaptcha]`:

```csharp
[HttpPost("contact")]
[ValidateCaptcha]
public async Task<IActionResult> SubmitContactForm([FromBody] ContactDto body)
{
    // Reach here only if the X-Captcha-Token header validated.
}
```

The filter:

1. If the configured provider is `None`, calls the next handler immediately. No header is required — this is what makes the filter a no-op in dev/test environments without forcing callers to synthesize a fake token.
2. Otherwise reads the token from the `X-Captcha-Token` request header.
3. If the header is missing, returns `400` with `{ "error": "captcha_required" }`.
4. Calls `ICaptchaService.ValidateAsync`. If the result is unsuccessful, returns `400` with `{ "error": "captcha_invalid", "errorCodes": [...] }`.
5. Otherwise calls the next handler.

## Client-side discovery

The widget render and token submission happen in the SPA. The backend exposes a small public endpoint so the SPA can discover the active provider and obtain the (browser-safe) site key at runtime, without baking either into the SPA build:

```http
GET /api/public/captcha-config
```

```json
{
  "provider": "Turnstile",
  "siteKey": "1x00000000000000000000AA"
}
```

`provider` is one of `None`, `Turnstile`, `RecaptchaV2`, `RecaptchaV3`. When `provider` is `None`, `siteKey` is `null` and the SPA should skip both the widget render and the `X-Captcha-Token` header on subsequent requests. The endpoint never exposes `SecretKey` — that lives server-side and the response DTO has no field for it.

A typical SPA pattern is to call this endpoint once on app bootstrap, cache the result, and use it both to render the right widget and to conditionally attach the token header.

## Client-side wiring

LightNap ships the backend half: the configuration, the four `ICaptchaService` implementations, the `[ValidateCaptcha]` filter, and the `/api/public/captcha-config` discovery endpoint. The SPA side is consumer-specific because each provider has a different widget shape:

- **Turnstile**: load `https://challenges.cloudflare.com/turnstile/v0/api.js` and render a `<div data-sitekey="...">` placeholder; the widget exposes the token via a callback.
- **reCAPTCHA v2**: load `https://www.google.com/recaptcha/api.js` and render `<div class="g-recaptcha" data-sitekey="...">`; same callback shape.
- **reCAPTCHA v3**: load the same script with `?render=<siteKey>`, then invoke `grecaptcha.execute(siteKey, { action: 'submit' })` programmatically right before submitting the protected form. No visible widget.

Once the token is captured, send it in the `X-Captcha-Token` request header on the protected call.
