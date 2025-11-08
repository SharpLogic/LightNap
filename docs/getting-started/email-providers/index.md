---
title: Email Providers
parent: Getting Started
layout: home
nav_order: 500
---

# {{ page.title }}

LightNap includes a flexible email system for sending transactional emails such as password resets, email verification codes, and two-factor authentication tokens. The email functionality is split into two concerns:

- **`IEmailService`** - Determines _what_ emails to send and manages email templates and content
- **`IEmailSender`** - Handles _how_ emails are sent (SMTP, console logging, etc.)

This separation allows you to easily switch email providers without changing your application logic.

## Available Providers

### [SMTP Provider](./smtp-provider)

Send emails through any SMTP-compatible email server, including popular services like SendGrid, Mailgun, Gmail, and Office 365.

**Best For**: Production deployments, real email delivery

**Configuration**: Requires SMTP server details, credentials, and sender information

### [Log To Console Provider](./log-to-console-provider)

Logs email content to the console instead of actually sending emails. Perfect for development and testing when you need to verify email content without setting up an SMTP server.

**Best For**: Local development, testing, demos

**Configuration**: Minimal; only requires sender display name

## Configuration

Email providers are configured in `appsettings.json` under the `Email` node:

```json
{
  "Email": {
    "Provider": "Smtp",  // or "LogToConsole"
    "FromEmail": "noreply@example.com",
    "FromDisplayName": "LightNap App",
    "Smtp": {
      "Host": "smtp.sendgrid.net",
      "Port": 587,
      "EnableSsl": true,
      "User": "apikey",
      "Password": "your-api-key"
    }
  }
}
```

## Common Email Scenarios

LightNap sends emails for:

- **Email Verification** - When users register with `RequireEmailVerification` enabled
- **Password Reset** - When users request a password reset
- **Two-Factor Authentication** - When users log in with 2FA enabled
- **Custom Notifications** - You can extend the email service for your own scenarios

For a comprehensive guide on adding custom email scenarios, see [Adding Backend Email Scenarios](../../common-scenarios/adding-email-scenarios)

## Choosing a Provider

| Scenario | Recommended Provider |
|----------|---------------------|
| Production deployment | SMTP |
| Local development | Log To Console |
| Testing/QA | Log To Console |
| Demo environments | Log To Console |

## Switching Providers

To switch between providers:

1. Update the `Provider` setting in `appsettings.json`
2. Configure provider-specific settings (like SMTP credentials)
3. Restart the application

No code changes are required; the dependency injection container automatically resolves the correct provider.

## Best Practices

- **Use environment variables** for sensitive SMTP credentials in production
- **Test with Log To Console** provider first to verify email content
- **Configure SPF/DKIM records** when using SMTP to improve deliverability
- **Monitor email delivery** in production to catch issues early
- **Consider transactional email services** like SendGrid or Mailgun for better deliverability and analytics

## See Also

- [Application Configuration](../application-configuration) - Overview of all configuration options
- [Configuring Application Settings](../configuring-application-settings) - Email-related application settings
