---
title: SMTP Provider
layout: home
parent: Email Providers
nav_order: 10
---

# {{ page.title }}

The SMTP email provider sends emails through any SMTP-compatible server, including popular services like SendGrid, Mailgun, Gmail, and Office 365.

## Configuration

Update the `Email` section in `appsettings.json`:

```json
{
  "Email": {
    "Provider": "Smtp",
    "FromEmail": "noreply@example.com",
    "FromDisplayName": "LightNap App",
    "Smtp": {
      "Host": "smtp.sendgrid.net",
      "Port": 587,
      "EnableSsl": true,
      "User": "apikey",
      "Password": "your-sendgrid-api-key"
    }
  }
}
```

### Common SMTP Configurations

#### SendGrid
```json
{
  "Smtp": {
    "Host": "smtp.sendgrid.net",
    "Port": 587,
    "EnableSsl": true,
    "User": "apikey",
    "Password": "your-api-key"
  }
}
```

#### Gmail
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "User": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

#### Office 365
```json
{
  "Smtp": {
    "Host": "smtp.office365.com",
    "Port": 587,
    "EnableSsl": true,
    "User": "your-email@company.com",
    "Password": "your-password"
  }
}
```

## Best Practices

- Use app-specific passwords for Gmail
- Enable 2FA on email accounts
- Use dedicated SMTP services for production
- Monitor delivery rates and bounce rates
- Configure SPF/DKIM records for better deliverability
