---
title: Adding Backend Email Scenarios
layout: home
parent: Features
nav_order: 30
---

# {{ page.title }}

LightNap includes a flexible email system for sending transactional emails to users. The system uses T4 templates to generate HTML email content and supports multiple email providers (SMTP, console logging, etc.). This article explains how to add a new email scenario to the application, following the established patterns used throughout the system.

- TOC
{:toc}

## Understanding the Email System

LightNap's email system is designed around two key interfaces that separate concerns:

- **`IEmailService`**: Determines _what_ emails to send and manages email templates and content. This is where you define methods for different email scenarios.
- **`IEmailSender`**: Handles _how_ emails are sent (SMTP, console logging, etc.). You typically don't need to modify this for new email scenarios.

The email system follows the standard LightNap architecture pattern:

- **Backend**: Email templates and service methods in `LightNap.Core/Email`
- **Templates**: T4 (Text Template Transformation Toolkit) templates in `LightNap.Core/Email/Templates` that generate HTML email content
- **Configuration**: Email provider settings in `appsettings.json`

By default, LightNap includes several email scenarios for user authentication and account management:

- **Email Verification**: Sent when users register with email verification enabled
- **Password Reset**: Sent when users request a password reset
- **Two-Factor Authentication**: Sent when users log in with 2FA enabled
- **Registration Welcome**: Sent to welcome new users after registration
- **Email Change**: Sent when users change their email address
- **Magic Link**: Sent for passwordless authentication

## When to Add a New Email Scenario

Consider adding a new email scenario when you need to send transactional or important notifications to users via email. Common examples include:

- **Account Activity**: Suspicious login attempts, account changes, security alerts
- **Order/Transaction Notifications**: Purchase confirmations, shipping updates, receipts
- **Subscription Management**: Renewal reminders, expiration notices, plan changes
- **Content Updates**: New comments on user posts, mentions, content approvals
- **Administrative Alerts**: Account suspension, policy changes, scheduled maintenance

{: .note }
For less critical notifications that should appear within the application UI, consider using [in-app notifications](./adding-notifications) instead. Many scenarios benefit from both email and in-app notifications.

## Backend Implementation

Adding a new email scenario involves four main steps:

1. Create an email template using T4
2. Add a method to `IEmailService`
3. Implement the method in `DefaultEmailService`
4. Call the email method from your application service

Let's walk through adding a "Subscription Expiring" email as an example.

### Step 1: Create the Email Template

Email templates in LightNap use T4 (Text Template Transformation Toolkit) to generate HTML content. Each template consists of two files: a `.tt` template file and the generated `.cs` file.

1. **Navigate to Templates Folder**: Open `LightNap.Core/Email/Templates`

2. **Create the Template File**: Add a new file named `SubscriptionExpiringTemplate.tt`

    ```plaintext
    <#@ template language="C#" inherits="BaseTemplate" #>
    <#@ assembly name="System.Core" #>
    <#@ import namespace="System.Linq" #>
    <#@ import namespace="System.Text" #>
    <#@ import namespace="System.Collections.Generic" #>
    <#@ import namespace="LightNap.Core.Data.Entities" #>

    <# base.TransformText(); #>

    <#+
        public DateTime ExpirationDate { get; set; }
        public string SubscriptionPlan { get; set; }
        public string RenewalUrl { get; set; }

        protected override void RenderBody()
        {
    #>
    <p>Your <strong><#= SubscriptionPlan #></strong> subscription is expiring soon.</p>
    <p>Your subscription will expire on <strong><#= ExpirationDate.ToString("MMMM d, yyyy") #></strong>.</p>
    <p>To continue enjoying uninterrupted service, please renew your subscription:</p>
    <p style="margin: 20px 0;">
      <a href="<#= SiteUrlRoot #><#= RenewalUrl #>"
         style="background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;">
        Renew Subscription
      </a>
    </p>
    <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
    <#+
        }
    #>
    ```

3. **Understanding the Template Structure**:
   - **Inherits from BaseTemplate**: All email templates extend `BaseTemplate`, which provides the HTML structure, header, and footer
   - **Properties**: Define properties for data that will be passed to the template (e.g., `ExpirationDate`, `SubscriptionPlan`)
   - **RenderBody Method**: Override this method to define the main content of your email
   - **Base Properties Available**: `User` (ApplicationUser), `FromDisplayName`, `SiteUrlRoot` are available from `BaseTemplate`

4. **Generate the C# Class**: In Visual Studio, save the `.tt` file. The IDE will automatically generate a corresponding `.cs` file. If using another editor, you may need to manually run the T4 transformation or generate the class.

    {: .note }
    The generated `.cs` file contains the `TransformText()` method that produces the final HTML. You should not manually edit this file as it will be regenerated when the template changes.

### Step 2: Add Method to IEmailService

Define the contract for your new email scenario in the `IEmailService` interface.

1. Open `LightNap.Core/Email/Interfaces/IEmailService.cs`

2. Add a method for your new email scenario:

    ```csharp
    /// <summary>
    /// Sends a subscription expiring notification email to the specified user.
    /// </summary>
    /// <param name="user">The user to send the email to.</param>
    /// <param name="expirationDate">The date when the subscription expires.</param>
    /// <param name="subscriptionPlan">The name of the subscription plan.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendSubscriptionExpiringAsync(ApplicationUser user, DateTime expirationDate, string subscriptionPlan);
    ```

3. **Method Naming Convention**: Follow the pattern `Send{ScenarioName}Async` for consistency with existing methods

4. **Parameters**: Include the user and any scenario-specific data needed for the template

5. **Documentation**: Add XML documentation comments to describe the method's purpose and parameters

### Step 3: Implement the Method in DefaultEmailService

Implement your new method in the `DefaultEmailService` class.

1. Open `LightNap.Core/Email/Services/DefaultEmailService.cs`

2. Add the implementation:

    ```csharp
    /// <summary>
    /// Sends a subscription expiring notification email to the specified user.
    /// </summary>
    /// <param name="user">The user to send the email to.</param>
    /// <param name="expirationDate">The date when the subscription expires.</param>
    /// <param name="subscriptionPlan">The name of the subscription plan.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendSubscriptionExpiringAsync(ApplicationUser user, DateTime expirationDate, string subscriptionPlan)
    {
        await this.SendMailAsync(user, "Your subscription is expiring soon",
            new SubscriptionExpiringTemplate()
            {
                FromDisplayName = this._fromDisplayName,
                SiteUrlRoot = this._siteUrlRoot,
                User = user,
                ExpirationDate = expirationDate,
                SubscriptionPlan = subscriptionPlan,
                RenewalUrl = "/account/subscription/renew"
            }.TransformText());
    }
    ```

3. **Understanding the Implementation**:
   - **Subject Line**: The second parameter to `SendMailAsync` is the email subject
   - **Template Instantiation**: Create a new instance of your template class
   - **Base Properties**: Set `FromDisplayName`, `SiteUrlRoot`, and `User` (inherited from `BaseTemplate`)
   - **Custom Properties**: Set any additional properties defined in your template
   - **TransformText()**: This method generates the final HTML from the template
   - **SendMailAsync**: This helper method handles creating the `MailMessage` and sending via `emailSender`

### Step 4: Call the Email Method from Your Service

Now that the email functionality is in place, call it from your application service when the triggering event occurs.

1. **Inject IEmailService**: Ensure your service has access to `IEmailService`. LightNap services typically use primary constructors:

    ```csharp
    public class SubscriptionService(
        IEmailService emailService,
        ApplicationDbContext dbContext,
        ILogger<SubscriptionService> logger
    ) : ISubscriptionService
    {
        // Services are automatically available as fields
    }
    ```

2. **Call the Email Method**: Trigger the email when appropriate:

    ```csharp
    public async Task CheckExpiringSubscriptionsAsync()
    {
        // Get subscriptions expiring in the next 7 days
        var expirationThreshold = DateTime.UtcNow.AddDays(7);
        var expiringSubscriptions = await dbContext.Subscriptions
            .Where(s => s.ExpiresAt <= expirationThreshold && s.ExpiresAt > DateTime.UtcNow)
            .Where(s => !s.ExpirationReminderSent)
            .Include(s => s.User)
            .ToListAsync();

        foreach (var subscription in expiringSubscriptions)
        {
            try
            {
                await emailService.SendSubscriptionExpiringAsync(
                    subscription.User,
                    subscription.ExpiresAt,
                    subscription.PlanName
                );

                // Mark as sent to avoid duplicate emails
                subscription.ExpirationReminderSent = true;
            }
            catch (Exception ex)
            {
                // Log the error but continue processing other subscriptions
                logger.LogError(ex,
                    "Failed to send expiration email for subscription {SubscriptionId}",
                    subscription.Id);
            }
        }

        await dbContext.SaveChangesAsync();
    }
    ```

## Template Best Practices

### 1. Use Semantic HTML

Structure your email content with proper HTML elements for better rendering across email clients:

```plaintext
<#+
    protected override void RenderBody()
    {
#>
<h2>Important Update</h2>
<p>This is the main content of your email.</p>
<ul>
  <li>First point</li>
  <li>Second point</li>
</ul>
<p style="margin: 20px 0;">
  <a href="<#= SiteUrlRoot #>/action"
     style="background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; display: inline-block;">
    Take Action
  </a>
</p>
<#+
    }
#>
```

### 2. Inline CSS Styles

Many email clients strip `<style>` tags, so use inline styles for formatting:

```plaintext
<p style="color: #333; font-size: 16px; line-height: 1.5;">
  Your styled content here.
</p>
```

### 3. Provide Plain Text Fallbacks

For critical information, avoid relying solely on images or complex formatting:

```plaintext
<!-- Good: Includes text alternative -->
<p>Click here to verify: <a href="<#= VerificationUrl #>"><#= VerificationUrl #></a></p>

<!-- Avoid: Link without visible URL -->
<p><a href="<#= VerificationUrl #>">Click here</a> to verify.</p>
```

### 4. Use Absolute URLs

Always use absolute URLs for links and images in emails:

```plaintext
<!-- Good -->
<a href="<#= SiteUrlRoot #>/account/settings">Account Settings</a>
<img src="<#= SiteUrlRoot #>/images/logo.png" alt="Logo" />

<!-- Bad: Relative URLs don't work in emails -->
<a href="/account/settings">Account Settings</a>
<img src="/images/logo.png" alt="Logo" />
```

### 5. Test Across Email Clients

Email clients render HTML differently. Test your templates in multiple clients:

- **Desktop**: Outlook, Thunderbird, Apple Mail
- **Web**: Gmail, Outlook.com, Yahoo Mail
- **Mobile**: iOS Mail, Gmail app, Outlook mobile

Consider using email testing tools like Litmus or Email on Acid for comprehensive testing.

### 6. Keep It Simple

Complex layouts may break in email clients. Stick to simple, single-column layouts:

```plaintext
<#+
    protected override void RenderBody()
    {
#>
<div style="max-width: 600px; margin: 0 auto;">
  <h2>Email Title</h2>
  <p>Simple, single-column content works best across all email clients.</p>
  <p>Avoid complex multi-column layouts, absolute positioning, and JavaScript.</p>
</div>
<#+
    }
#>
```

### 7. Include Unsubscribe Options

For marketing or recurring notifications, include clear unsubscribe options (and ensure they're legally compliant):

```plaintext
<#+
    protected override void RenderBody()
    {
#>
<p>Your notification content here.</p>
<hr style="margin-top: 40px; border: none; border-top: 1px solid #ccc;" />
<p style="font-size: 12px; color: #666;">
  Don't want to receive these emails?
  <a href="<#= SiteUrlRoot #>/account/email-preferences">Update your preferences</a>
</p>
<#+
    }
#>
```

## Error Handling Best Practices

### 1. Don't Break Application Flow

Email sending should never cause your main application logic to fail. Always wrap email calls in try-catch blocks:

```csharp
public async Task ProcessOrderAsync(Order order)
{
    // Critical business logic
    await dbContext.Orders.AddAsync(order);
    await dbContext.SaveChangesAsync();

    // Non-critical: Email confirmation
    try
    {
        var user = await dbContext.Users.FindAsync(order.UserId);
        await emailService.SendOrderConfirmationAsync(user, order);
    }
    catch (Exception ex)
    {
        // Log but don't fail the order
        logger.LogError(ex, "Failed to send order confirmation email for order {OrderId}", order.Id);
    }

    // Continue with other logic...
}
```

### 2. Implement Retry Logic

For critical emails, consider implementing retry logic with exponential backoff:

```csharp
public async Task SendEmailWithRetryAsync(Func<Task> emailOperation, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await emailOperation();
            return; // Success
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            logger.LogWarning(ex, "Email send attempt {Attempt} failed, retrying...", attempt);
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Email send failed after {MaxRetries} attempts", maxRetries);
            throw; // Rethrow on final attempt if needed
        }
    }
}

// Usage
await SendEmailWithRetryAsync(async () =>
    await emailService.SendSubscriptionExpiringAsync(user, expirationDate, planName));
```

### 3. Log Email Failures

Always log email failures with sufficient context for debugging:

```csharp
try
{
    await emailService.SendSubscriptionExpiringAsync(user, expirationDate, planName);
    logger.LogInformation(
        "Sent subscription expiring email to {Email} for subscription {SubscriptionId}",
        user.Email, subscription.Id);
}
catch (Exception ex)
{
    logger.LogError(ex,
        "Failed to send subscription expiring email to {Email} for subscription {SubscriptionId}. " +
        "Plan: {PlanName}, Expiration: {ExpirationDate}",
        user.Email, subscription.Id, planName, expirationDate);
}
```

### 4. Track Email Delivery

Consider adding a database table to track email sends for auditing and troubleshooting:

```csharp
public class EmailLog
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string EmailType { get; set; }
    public string Subject { get; set; }
    public string Recipient { get; set; }
    public DateTime SentAt { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

// In your email service or a wrapper
public async Task SendAndLogEmailAsync(ApplicationUser user, string emailType, Func<Task> sendOperation)
{
    var log = new EmailLog
    {
        UserId = user.Id,
        EmailType = emailType,
        Recipient = user.Email,
        SentAt = DateTime.UtcNow
    };

    try
    {
        await sendOperation();
        log.Success = true;
    }
    catch (Exception ex)
    {
        log.Success = false;
        log.ErrorMessage = ex.Message;
        throw;
    }
    finally
    {
        await dbContext.EmailLogs.AddAsync(log);
        await dbContext.SaveChangesAsync();
    }
}
```

## Testing Email Scenarios

### 1. Use Log To Console Provider for Development

During development, use the `LogToConsole` email provider to view email content without actually sending emails:

```json
{
  "Email": {
    "Provider": "LogToConsole",
    "FromEmail": "noreply@example.com",
    "FromDisplayName": "LightNap Development"
  }
}
```

This logs the complete email HTML to the console, allowing you to verify content without needing an SMTP server.

### 2. Unit Test Email Service Methods

Test that your email service methods correctly generate email content:

```csharp
[TestMethod]
public async Task SendSubscriptionExpiringAsync_ShouldGenerateCorrectEmail()
{
    // Arrange
    var user = new ApplicationUser
    {
        UserName = "testuser",
        Email = "test@example.com"
    };
    var expirationDate = DateTime.UtcNow.AddDays(7);
    var planName = "Premium Plan";

    // Mock the email sender to capture the sent message
    var capturedMessage = null as MailMessage;
    var mockEmailSender = new Mock<IEmailSender>();
    mockEmailSender
        .Setup(s => s.SendMailAsync(It.IsAny<MailMessage>()))
        .Callback<MailMessage>(m => capturedMessage = m)
        .Returns(Task.CompletedTask);

    var emailService = new DefaultEmailService(
        mockConfiguration,
        mockEmailSender.Object,
        mockApplicationSettings);

    // Act
    await emailService.SendSubscriptionExpiringAsync(user, expirationDate, planName);

    // Assert
    Assert.IsNotNull(capturedMessage);
    Assert.AreEqual("test@example.com", capturedMessage.To[0].Address);
    Assert.IsTrue(capturedMessage.Subject.Contains("expiring"));
    Assert.IsTrue(capturedMessage.Body.Contains(planName));
    Assert.IsTrue(capturedMessage.Body.Contains(expirationDate.ToString("MMMM d, yyyy")));
    Assert.IsTrue(capturedMessage.IsBodyHtml);
}
```

### 3. Integration Testing

Test the complete flow from trigger to email generation:

```csharp
[TestMethod]
public async Task CheckExpiringSubscriptions_ShouldSendEmailsForExpiringSubscriptions()
{
    // Arrange
    var user = await CreateTestUserAsync();
    var subscription = new Subscription
    {
        UserId = user.Id,
        User = user,
        PlanName = "Premium",
        ExpiresAt = DateTime.UtcNow.AddDays(5),
        ExpirationReminderSent = false
    };
    await dbContext.Subscriptions.AddAsync(subscription);
    await dbContext.SaveChangesAsync();

    // Act
    await subscriptionService.CheckExpiringSubscriptionsAsync();

    // Assert
    var updatedSubscription = await dbContext.Subscriptions.FindAsync(subscription.Id);
    Assert.IsTrue(updatedSubscription.ExpirationReminderSent);

    // Verify email was sent (using mock or log verification)
    mockEmailService.Verify(
        s => s.SendSubscriptionExpiringAsync(
            It.Is<ApplicationUser>(u => u.Id == user.Id),
            subscription.ExpiresAt,
            "Premium"),
        Times.Once);
}
```

### 4. Manual Testing with Test SMTP Server

For testing actual email sending without spamming real inboxes, use a test SMTP server:

**Popular Options:**
- **[Papercut-SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP)**: Local SMTP server that captures emails
- **[MailHog](https://github.com/mailhog/MailHog)**: Email testing tool with web interface
- **[Mailtrap](https://mailtrap.io/)**: Cloud-based email testing service

Configure your development `appsettings.json` to use the test server:

```json
{
  "Email": {
    "Provider": "Smtp",
    "FromEmail": "noreply@example.com",
    "FromDisplayName": "LightNap Development",
    "Smtp": {
      "Host": "localhost",
      "Port": 25,
      "EnableSsl": false,
      "User": "",
      "Password": ""
    }
  }
}
```

### 5. Preview Templates in Browser

For quick visual testing of your templates, create a temporary endpoint to render the template:

```csharp
// Temporary development-only endpoint
[HttpGet("preview-email")]
public IActionResult PreviewSubscriptionExpiringEmail()
{
    #if DEBUG
    var user = new ApplicationUser
    {
        UserName = "testuser",
        Email = "test@example.com"
    };

    var template = new SubscriptionExpiringTemplate
    {
        FromDisplayName = "LightNap",
        SiteUrlRoot = "https://localhost:4200",
        User = user,
        ExpirationDate = DateTime.UtcNow.AddDays(7),
        SubscriptionPlan = "Premium Plan",
        RenewalUrl = "/account/subscription/renew"
    };

    return Content(template.TransformText(), "text/html");
    #else
    return NotFound();
    #endif
}
```

Access this endpoint in your browser to see the rendered email. Remove this endpoint before deploying to production.

## Common Email Scenarios

Here are common email scenarios you might want to implement:

### Account & Security

- **Password Changed**: Alert when password is successfully updated
- **Email Changed**: Confirmation that email address was changed
- **Account Locked**: Notification when account is locked due to failed login attempts
- **Suspicious Activity**: Alert for login from new location or device
- **Two-Factor Disabled**: Security notification when 2FA is disabled

### User Engagement

- **Welcome Series**: Multi-step onboarding email sequence
- **Inactivity Reminder**: Re-engagement email for dormant accounts
- **Feature Announcement**: New features or important updates
- **Milestone Achievement**: Congratulations on account anniversary, achievements, etc.

### Transactions

- **Order Confirmation**: Receipt and order details
- **Shipping Notification**: Tracking information for shipped orders
- **Payment Receipt**: Payment confirmation and invoice
- **Refund Processed**: Confirmation of refund transaction

### Subscriptions

- **Subscription Activated**: Welcome email for new subscription
- **Subscription Expiring**: Reminder before subscription expires
- **Subscription Renewed**: Confirmation of successful renewal
- **Subscription Cancelled**: Confirmation of cancellation with feedback request
- **Payment Failed**: Alert when subscription payment fails

### Content & Community

- **New Comment Notification**: Someone commented on user's content
- **Mention Notification**: User was mentioned in a post or comment
- **Content Published**: User's submitted content was approved/published
- **Moderation Action**: Content was flagged or removed

### Administrative

- **Account Suspended**: Notification with reason and appeal process
- **Terms of Service Update**: Important policy changes
- **Scheduled Maintenance**: Advance notice of system downtime
- **Data Export Ready**: User-requested data export is available

## Advanced Topics

### Localization and Multi-Language Support

For applications supporting multiple languages, consider:

1. **Create Language-Specific Templates**: Have separate templates for each supported language (e.g., `SubscriptionExpiringTemplate.en.tt`, `SubscriptionExpiringTemplate.es.tt`)

2. **Use Resource Files**: Store text content in resource files and reference them in templates:

    ```plaintext
    <#@ assembly name="System.Resources" #>
    <p><#= Resources.EmailStrings.SubscriptionExpiringMessage #></p>
    ```

3. **Dynamic Template Selection**: Load the appropriate template based on user's language preference:

    ```csharp
    public async Task SendLocalizedEmailAsync(ApplicationUser user, string templateKey)
    {
        var userLanguage = user.PreferredLanguage ?? "en";
        var template = GetTemplateForLanguage(templateKey, userLanguage);
        await SendMailAsync(user, GetSubject(templateKey, userLanguage), template);
    }
    ```

### Scheduled Email Campaigns

For recurring email notifications, consider copying the pattern provided by the `LightNap.MaintenanceService` project that runs daily. Alternatively, you can implement a background service within the `LightNap.WebApi` project:

```csharp
public class EmailCampaignService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailCampaignService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

            try
            {
                await subscriptionService.CheckExpiringSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking expiring subscriptions");
            }

            // Run once per day
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
```

Register in `Program.cs`:

```csharp
builder.Services.AddHostedService<EmailCampaignService>();
```

### Email Analytics

Track email engagement by adding tracking parameters:

```csharp
public string GenerateTrackingUrl(string baseUrl, string userId, string campaign)
{
    var trackingParams = $"?utm_source=email&utm_medium=email&utm_campaign={campaign}&user_id={userId}";
    return $"{baseUrl}{trackingParams}";
}

// In your template
<a href="<#= GenerateTrackingUrl($"{SiteUrlRoot}/renew", User.Id, "subscription_expiring") #>">
  Renew Now
</a>
```

Track link clicks in your application to measure email effectiveness.

### Rich Email Content

For more sophisticated emails, consider:

- **Dynamic Content**: Personalize emails based on user behavior or preferences
- **Embedded Images**: Include logos, product images, or charts (ensure proper hosting)
- **Interactive Elements**: Buttons, accordions (limited support)
- **Email Frameworks**: Use frameworks like Foundation for Emails or MJML for responsive layouts

## Related Documentation

- [Email Providers](../getting-started/email-providers) - Configuring SMTP and email providers
- [Adding In-App Notification Types](./adding-notifications) - Complementary in-app notifications
- [Project Structure](../concepts/project-structure) - Understanding the overall architecture
- [Configuring Application Settings](../getting-started/configuring-application-settings) - Email-related settings

## Additional Resources

### Email Development Tools

- **[T4 Templates Documentation](https://learn.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates)** - Microsoft's T4 template documentation
- **[Can I Email](https://www.caniemail.com/)** - Email client CSS support reference
- **[Email on Acid](https://www.emailonacid.com/)** - Email testing platform
- **[Litmus](https://www.litmus.com/)** - Email preview and testing

### Email Best Practices

- **[Really Good Emails](https://reallygoodemails.com/)** - Email design inspiration
- **[MJML](https://mjml.io/)** - Responsive email framework
- **[Foundation for Emails](https://get.foundation/emails.html)** - Responsive email framework

### Deliverability

When implementing email scenarios in production:

- **Configure SPF Records**: Authorize your domain to send emails
- **Set Up DKIM**: Add email authentication signatures
- **Implement DMARC**: Define email authentication policies
- **Monitor Bounce Rates**: Track delivery failures
- **Maintain Clean Lists**: Remove invalid email addresses
- **Respect Unsubscribes**: Honor opt-out requests promptly

Poor email practices can damage your domain's reputation and affect deliverability for all your emails.
