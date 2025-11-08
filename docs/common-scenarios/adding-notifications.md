---
title: Adding In-App Notification Types
layout: home
parent: Common Scenarios
nav_order: 800
---

# {{ page.title }}

LightNap includes an in-app notification system that allows the application to deliver real-time notifications to logged-in users. These notifications appear in the user's notification panel and provide a way to keep users informed about important events, updates, and activities within the application. This article explains how to add a new type of notification to the application, following the established patterns used throughout the system.

- TOC
{:toc}

## Understanding the Notification System

The in-app notification system in LightNap provides persistent, user-specific messages that are stored in the database and displayed in the UI. Unlike transient toast messages, these notifications remain available until the user reads or dismisses them.

The system follows the standard LightNap architecture pattern:

- **Backend**: Notifications are stored as entities in the database and managed through a `NotificationService` in `LightNap.Core`.
- **API Layer**: The `MeController` exposes endpoints under `/api/users/me/notifications` for managing notifications.
- **Frontend**: An Angular service (`NotificationService`) and components handle displaying and managing notifications in the UI.

By default, LightNap includes core notification infrastructure. Adding a new notification type involves creating the trigger logic in your application services and ensuring the notification data includes the appropriate type identifier.

## Understanding the Notification Entity

The notification system is built on a `Notification` entity stored in the database. Each notification contains:

- **Id**: Unique identifier for the notification
- **UserId**: The ID of the user who should receive the notification
- **Type**: A notification type identifier (e.g., "NewComment", "SystemAlert", "RoleChanged")
- **Status**: The read status of the notification (Unread, Read, Archived)
- **Timestamp**: When the notification was created
- **Data**: A flexible dictionary containing notification-specific metadata

The `Data` property is key to the notification system's flexibility. Instead of hardcoding paths or URLs, store metadata that allows the frontend to determine the appropriate routing and presentation. For example, a "NewComment" notification should include a `commentId` rather than a fixed URL path to the comment.

## Backend Implementation

### Step 1: Understand the Notification Service

The `INotificationService` in `LightNap.Core/Notifications/Interfaces` provides methods for creating and managing notifications. Key methods include:

```csharp
public interface INotificationService
{
    // Create notification for a specific user
    Task CreateSystemNotificationForUserAsync(string userId, CreateNotificationRequestDto requestDto);

    // Create notifications for all users in a role
    Task CreateSystemNotificationForRoleAsync(string role, CreateNotificationRequestDto requestDto);

    // Create notifications for all users with a specific claim
    Task CreateSystemNotificationForClaimAsync(ClaimDto claim, CreateNotificationRequestDto requestDto);

    // Search and retrieve notifications
    Task<NotificationSearchResultsDto> SearchNotificationsAsync(string userId, SearchNotificationsRequestDto requestDto);

    // Mark notifications as read
    Task MarkAsReadAsync(int id);
    Task MarkAllAsReadAsync(string userId);
}
```

### Step 2: Define Notification Type Constants

To maintain consistency and avoid typos, create constants for your notification types:

Create or update a constants file in `LightNap.Core/Configuration/Constants.cs`:

```csharp
public static class NotificationTypes
{
    public const string WelcomeMessage = "WelcomeMessage";
    public const string NewComment = "NewComment";
    public const string SystemAlert = "SystemAlert";
    public const string ProfileUpdated = "ProfileUpdated";
    public const string RoleChanged = "RoleChanged";
    // Add more notification types as needed
}
```

### Step 3: Create a Notification in Your Service

To add a new notification type, inject `INotificationService` into your application service and call it when the triggering event occurs. For example, let's create a "Welcome" notification when a user completes registration:

1. Open the service where you want to trigger the notification (e.g., `IdentityService.cs` in `LightNap.Core/Identity/Services`).

2. Inject `INotificationService`. LightNap's built-in services use primary constructors, so an example looks like:

    ```csharp
    public class IdentityService(
        INotificationService notificationService,
        UserManager<ApplicationUser> userManager
        // ... other dependencies
    ) : IIdentityService
    {
        // Services are automatically available as fields via the primary constructor
        // You can reference them as notificationService, userManager, etc.
    }
    ```

3. Create the notification after the triggering event:

    ```csharp
    public async Task<ApiResponseDto<LoginResponseDto>> RegisterAsync(RegisterRequestDto dto)
    {
        // Existing registration logic...

        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            // Existing post-registration logic...

            // Create welcome notification with metadata
            await notificationService.CreateSystemNotificationForUserAsync(
                user.Id,
                new CreateNotificationRequestDto
                {
                    Type = NotificationTypes.WelcomeMessage,
                    Data = new Dictionary<string, object>
                    {
                        { "registrationDate", DateTime.UtcNow }
                    }
                });

            // Continue with remaining logic...
        }

        // Return response...
    }
    ```

## Using Metadata Effectively

The `Data` dictionary in `CreateNotificationRequestDto` should contain metadata that allows the frontend to construct appropriate routing and display logic, rather than hardcoded paths or complete messages.

### Best Practices for Notification Data

**Use Entity IDs, Not Paths:**

```csharp
// Good - Provides metadata for flexible routing and client can load related data if/when needed
await notificationService.CreateSystemNotificationForUserAsync(
    userId,
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.NewComment,
        Data = new Dictionary<string, object>
        {
            { "commentId", comment.Id },
        }
    });

// Avoid - Hardcoded data and paths that can't adapt to routing changes
await notificationService.CreateSystemNotificationForUserAsync(
    userId,
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.NewComment,
        Data = new Dictionary<string, object>
        {
            { "title", "New Comment" },
            { "message", "John commented on your post" },
            { "link", "/posts/123/comments/456" } // Hardcoded path
        }
    });
```

**Include Contextual Information:**

```csharp
// Role change notification with full context since it cannot be easily loaded on-demand later on
await notificationService.CreateSystemNotificationForUserAsync(
    userId,
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.RoleChanged,
        Data = new Dictionary<string, object>
        {
            { "oldRole", "User" },
            { "newRole", "Moderator" },
            { "changedBy", adminUserName },
            { "changedAt", DateTime.UtcNow }
        }
    });
```

The frontend can then use this metadata to:

- Construct localized messages based on the user's language preference
- Route to the appropriate page using route aliases or dynamic routing
- Display rich notification UI with avatars, previews, and action buttons
- Apply different styling based on notification type and context

## Targeting Notifications by Role or Claim

Instead of sending notifications to a specific user, you can target groups of users based on their roles or claims. The `INotificationService` provides methods to send notifications to all users in a role or with a specific claim:

```csharp
// Notify all users in a role
await notificationService.CreateSystemNotificationForRoleAsync(
    Constants.Roles.Administrator,
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.SystemMaintenance,
        Data = new Dictionary<string, object>
        {
            { "scheduledTime", "2024-03-15T22:00:00Z" },
            { "estimatedDuration", "2 hours" }
        }
    });

// Notify all users with a specific claim
await notificationService.CreateSystemNotificationForClaimAsync(
    new ClaimDto { Type = Constants.Claims.ContentEditor, Value = contentId.ToString() },
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.ModerationRequested,
        Data = new Dictionary<string, object>
        {
            { "contentId", contentId }
        }
    });
```

For more information on working with roles and claims, see [Working with Roles](working-with-roles.md) and [Custom Claims](custom-claims.md)

## Frontend Implementation

The frontend notification system follows LightNap's standard data flow pattern: DTOs → Data Services → Application Services → Components.

### Understanding the Frontend Architecture

The notification functionality on the frontend is organized as follows:

- **DTOs**: Located in `app/core/backend-api/notifications/dtos`, these TypeScript interfaces map to backend DTOs
- **Data Service**: Located in `app/core/backend-api/notifications/services`, handles HTTP requests to `/api/users/me/notifications`
- **Application Service**: Located in `app/core/notifications/services`, provides a higher-level API and manages notification state
- **Components**: Various UI components that display and interact with notifications

### Step 1: Understand the Frontend DTOs

The frontend DTOs mirror the backend DTOs and are typically located in `app/core/backend-api/notifications/dtos`. The notification interface includes properties that map to the backend entity:

- **id**: Unique identifier for the notification
- **userId**: The ID of the user who received the notification
- **type**: Notification type identifier (e.g., "WelcomeMessage", "NewComment")
- **status**: Read status (Unread, Read, Archived)
- **timestamp**: When the notification was created
- **data**: Dictionary containing notification-specific metadata

The frontend application interprets the `type` and `data` properties to construct appropriate UI presentation, routing, and user messages.

### Step 2: Using the Notification Service

The `NotificationService` is typically available as a singleton service. To display notifications in your component:

```typescript
import { Component, OnInit } from '@angular/core';
import { NotificationService } from '@core/notifications/services/notification.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-my-component',
  templateUrl: './my-component.component.html'
})
export class MyComponent implements OnInit {
  unreadCount$!: Observable<number>;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    // Watch for unread notification count
    this.unreadCount$ = this.notificationService.watchUnreadCount$();

    // Refresh notifications when component loads
    this.notificationService.refreshNotifications();
  }
}
```

### Step 3: Displaying Notifications in the UI

Notifications are typically displayed in a bell icon menu in the application header. The notification service exposes observables that components can subscribe to:

```typescript
// In your header or notification panel component
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '@core/notifications/services/notification.service';
import { Notification } from '@core/backend-api/notifications/dtos/response/notification';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-notification-panel',
  template: `
    <div class="notification-panel">
      <div class="notification-header">
        <h3>Notifications</h3>
        <span class="badge" *ngIf="(unreadCount$ | async) as count">{{ count }}</span>
      </div>

      <div class="notification-list">
        <div
          *ngFor="let notification of (notifications$ | async)"
          class="notification-item"
          [class.unread]="notification.status === 'Unread'"
          (click)="handleNotificationClick(notification)">

          <div class="notification-icon" [attr.data-type]="notification.type">
            <i [class]="getNotificationIcon(notification.type)"></i>
          </div>

          <div class="notification-content">
            <h4>{{ getNotificationTitle(notification) }}</h4>
            <p>{{ getNotificationMessage(notification) }}</p>
            <span class="notification-time">{{ notification.timestamp | date: 'short' }}</span>
          </div>
        </div>
      </div>
    </div>
  `
})
export class NotificationPanelComponent implements OnInit {
  notifications$!: Observable<Notification[]>;
  unreadCount$!: Observable<number>;

  constructor(
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.notifications$ = this.notificationService.watchNotifications$();
    this.unreadCount$ = this.notificationService.watchUnreadCount$();
  }

  handleNotificationClick(notification: Notification): void {
    // Mark as read if unread
    if (notification.status === 'Unread') {
      this.notificationService.markAsRead(notification.id).subscribe();
    }

    // Navigate based on notification type and data
    const route = this.getNotificationRoute(notification);
    if (route) {
      this.router.navigate([route]);
    }
  }

  getNotificationIcon(type: string): string {
    const iconMap: { [key: string]: string } = {
      'WelcomeMessage': 'pi pi-user',
      'NewComment': 'pi pi-comment',
      'SystemAlert': 'pi pi-exclamation-triangle',
      'ProfileUpdated': 'pi pi-user-edit',
      'RoleChanged': 'pi pi-shield'
    };
    return iconMap[type] || 'pi pi-bell';
  }

  getNotificationTitle(notification: Notification): string {
    // Construct title based on notification type
    switch (notification.type) {
      case 'WelcomeMessage':
        return 'Welcome!';
      case 'NewComment':
        return 'New Comment';
      case 'RoleChanged':
        return 'Role Updated';
      default:
        return 'Notification';
    }
  }

  getNotificationMessage(notification: Notification): string {
    // Construct message from notification type and data
    switch (notification.type) {
      case 'WelcomeMessage':
        return 'Welcome to the application!';
      case 'NewComment':
        return `You have a new comment`;
      case 'RoleChanged':
        const oldRole = notification.data['oldRole'] || 'Unknown';
        const newRole = notification.data['newRole'] || 'Unknown';
        return `Your role changed from ${oldRole} to ${newRole}`;
      default:
        return 'You have a new notification';
    }
  }

  getNotificationRoute(notification: Notification): string | null {
    // Construct route based on notification type and data
    switch (notification.type) {
      case 'NewComment':
        const commentId = notification.data['commentId'];
        return commentId ? `/comments/${commentId}` : null;
      case 'ProfileUpdated':
        return '/profile';
      default:
        return null;
    }
  }
}
```

### Step 4: Customizing Notification Display by Type

You can customize how different notification types appear in your UI:

```typescript
// Create a helper service for notification presentation
@Injectable({
  providedIn: 'root'
})
export class NotificationPresentationService {
  getNotificationStyle(type: string): { icon: string; color: string; severity: string } {
    const styles: { [key: string]: { icon: string; color: string; severity: string } } = {
      'WelcomeMessage': {
        icon: 'pi pi-user',
        color: '#4CAF50',
        severity: 'success'
      },
      'NewComment': {
        icon: 'pi pi-comment',
        color: '#2196F3',
        severity: 'info'
      },
      'SystemAlert': {
        icon: 'pi pi-exclamation-triangle',
        color: '#FF9800',
        severity: 'warn'
      },
      'ProfileUpdated': {
        icon: 'pi pi-user-edit',
        color: '#9C27B0',
        severity: 'info'
      },
      'RoleChanged': {
        icon: 'pi pi-shield',
        color: '#F44336',
        severity: 'warn'
      }
    };

    return styles[type] || {
      icon: 'pi pi-bell',
      color: '#607D8B',
      severity: 'info'
    };
  }
}
```

## Real-Time Notifications (Optional)

For a more responsive user experience, consider implementing real-time notification delivery using SignalR:

1. **Backend Setup**: Add SignalR hub for notification broadcasting
2. **Frontend Integration**: Connect to the SignalR hub and listen for new notifications
3. **Update UI**: Automatically display new notifications without page refresh

```typescript
// Example of SignalR integration in NotificationService
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private hubConnection?: HubConnection;

  constructor(private notificationDataService: NotificationDataService) {
    this.initializeSignalR();
  }

  private initializeSignalR(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('/notificationHub')
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
      // Add notification to the local observable stream
      this.addNotification(notification);
    });

    this.hubConnection.start();
  }
}
```

## Testing Your Notification

### 1. Backend Testing

Create unit tests for your notification creation logic:

```csharp
[TestMethod]
public async Task CreateWelcomeNotification_ShouldSucceed()
{
    // Arrange
    var userId = "test-user-id";
    var requestDto = new CreateNotificationRequestDto
    {
        Type = NotificationTypes.WelcomeMessage,
        Data = new Dictionary<string, object>
        {
            { "userName", "testuser" }
        }
    };

    // Act
    await _notificationService.CreateSystemNotificationForUserAsync(userId, requestDto);

    // Assert
    var notifications = await _dbContext.Notifications
        .Where(n => n.UserId == userId)
        .ToListAsync();
    Assert.AreEqual(1, notifications.Count);
    Assert.AreEqual(NotificationTypes.WelcomeMessage, notifications[0].Type);
}
```

### 2. Integration Testing

Test the complete flow from trigger to notification creation:

```csharp
[TestMethod]
public async Task UserRegistration_ShouldCreateWelcomeNotification()
{
    // Arrange
    var registerDto = new RegisterRequestDto
    {
        UserName = "testuser",
        Email = "test@example.com",
        Password = "TestPassword123!"
    };

    // Act
    var result = await _identityService.RegisterAsync(registerDto);

    // Assert
    var notifications = await _notificationService.SearchNotificationsAsync(
        result.Result.UserId,
        new SearchNotificationsRequestDto { PageSize = 10 }
    );

    Assert.IsTrue(notifications.Items.Any(n => n.Type == NotificationTypes.WelcomeMessage));
}
```

### 3. Frontend Testing

Test the notification service and components:

```typescript
describe('NotificationService', () => {
  let service: NotificationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [NotificationService]
    });

    service = TestBed.inject(NotificationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should fetch notifications', () => {
    const mockNotifications: Notification[] = [
      {
        id: 1,
        userId: 'user1',
        type: 'WelcomeMessage',
        status: 'Unread',
        timestamp: new Date(),
        data: {
          registrationDate: new Date().toISOString()
        }
      }
    ];

    service.watchNotifications$().subscribe(notifications => {
      expect(notifications.length).toBe(1);
      expect(notifications[0].type).toBe('WelcomeMessage');
      expect(notifications[0].status).toBe('Unread');
    });

    const req = httpMock.expectOne('/api/users/me/notifications');
    req.flush({ result: mockNotifications });
  });
});
```

### 4. End-to-End Testing

Test the complete user experience:

1. Register a new user account
2. Verify the welcome notification appears in the notification panel
3. Click the notification to mark it as read
4. Verify the unread count decreases
5. Navigate to the linked page if applicable

## Best Practices

### 1. Use Type Constants

Always use constants for notification types to maintain consistency and avoid typos:

```csharp
// Good
await _notificationService.CreateSystemNotificationForUserAsync(
    userId,
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.WelcomeMessage,
        Data = new Dictionary<string, object>()
    });

// Bad - prone to typos
await _notificationService.CreateSystemNotificationForUserAsync(
    userId,
    new CreateNotificationRequestDto
    {
        Type = "WelcomeMessage",
        Data = new Dictionary<string, object>()
    });
```

### 2. Implement Error Handling

Notification creation should not break the main application flow. Wrap notification calls in try-catch blocks:

```csharp
public async Task<ApiResponseDto<LoginResponseDto>> RegisterAsync(RegisterRequestDto dto)
{
    // Main registration logic...

    try
    {
        await notificationService.CreateSystemNotificationForUserAsync(
            user.Id,
            new CreateNotificationRequestDto
            {
                Type = NotificationTypes.WelcomeMessage,
                Data = new Dictionary<string, object>
                {
                    { "registrationDate", DateTime.UtcNow }
                }
            });
    }
    catch (Exception ex)
    {
        // Log the error but don't fail the registration
        this._logger.LogError(ex, "Failed to create welcome notification for user {UserId}", user.Id);
    }

    // Continue with response...
}
```

### 3. Use Metadata, Not Hardcoded Content

Store entity IDs and contextual data rather than fully-formed messages or paths:

```csharp
// Good - Metadata allows frontend flexibility
Data = new Dictionary<string, object>
{
    { "commentId", 456 },
}

// Avoid - Everything else can be easily loaded at runtime based on the comment ID
Data = new Dictionary<string, object>
{
    { "commentId", 456 },
    { "postId", 123 },
    { "authorName", "John Doe" },
    { "commentPreview", "Great article! I especially..." },
    { "title", "New Comment" },
    { "message", "John Doe replied to your post." },
    { "link", "/posts/123/comments" }
}
```

### 4. Implement Notification Expiration

Consider adding expiration logic to automatically clean up old notifications:

```csharp
// In your maintenance service or background job
public async Task CleanupExpiredNotificationsAsync()
{
    var cutoffDate = DateTime.UtcNow.AddDays(-30);
    var expiredNotifications = await _dbContext.Notifications
        .Where(n => n.CreatedAt < cutoffDate && n.IsRead)
        .ToListAsync();

    _dbContext.Notifications.RemoveRange(expiredNotifications);
    await _dbContext.SaveChangesAsync();
}
```

### 5. Batch Notifications Wisely

For high-frequency events, consider batching notifications to avoid overwhelming users:

```csharp
// Instead of creating a notification for each like
// Create a single notification with aggregated information
if (newLikeCount >= 10)
{
    await _notificationService.CreateSystemNotificationForUserAsync(
        userId,
        new CreateNotificationRequestDto
        {
            Type = NotificationTypes.PostEngagement,
            Data = new Dictionary<string, object>
            {
                { "postId", postId },
                { "likeCount", newLikeCount },
                { "threshold", 10 }
            }
        });
}
```

### 6. Localization Support

For multi-language applications, let the frontend handle localization by providing raw data:

```csharp
// Backend provides raw data
await _notificationService.CreateSystemNotificationForUserAsync(
    userId,
    new CreateNotificationRequestDto
    {
        Type = NotificationTypes.WelcomeMessage,
        Data = new Dictionary<string, object>
        {
            { "userName", userName },
            { "registrationDate", DateTime.UtcNow }
        }
    });

// Frontend uses notification type + data to construct localized message:
// - English: "Welcome, John! You joined on Nov 5, 2025."
// - Spanish: "¡Bienvenido, John! Te uniste el 5 de nov de 2025."
// - French: "Bienvenue, John ! Vous avez rejoint le 5 nov 2025."
```

### 7. User Preferences

Allow users to control which notifications they receive:

```csharp
public async Task<bool> ShouldCreateNotificationAsync(
    string userId,
    string notificationType)
{
    var userPreferences = await _dbContext.NotificationPreferences
        .FirstOrDefaultAsync(p => p.UserId == userId);

    if (userPreferences == null)
    {
        return true; // Default to sending all notifications
    }

    return userPreferences.EnabledTypes.Contains(notificationType);
}

// Use before creating notification
if (await ShouldCreateNotificationAsync(userId, NotificationTypes.NewComment))
{
    await _notificationService.CreateSystemNotificationForUserAsync(
        userId,
        new CreateNotificationRequestDto
        {
            Type = NotificationTypes.NewComment,
            Data = new Dictionary<string, object>
            {
                { "commentId", commentId }
            }
        });
}
```

## Common Notification Types

Here are some common notification types you might want to implement in your application:

### Account & Security

- **WelcomeMessage**: Sent when a user first registers
- **PasswordChanged**: Alert when password is updated
- **EmailChanged**: Confirmation of email address change
- **NewDeviceLogin**: Alert for login from unrecognized device
- **TwoFactorEnabled**: Confirmation of 2FA activation

### User Engagement

- **ProfileViewed**: Someone viewed your profile
- **NewFollower**: Someone started following you
- **MentionReceived**: You were mentioned in a post or comment
- **MessageReceived**: New direct message from another user

### Content Interactions

- **NewComment**: Someone commented on your content
- **NewLike**: Your content received likes
- **ContentShared**: Your content was shared
- **ReplyReceived**: Someone replied to your comment

### Administrative

- **RoleChanged**: User role was modified
- **AccountSuspended**: Account has been suspended
- **SystemMaintenance**: Scheduled maintenance notification
- **PolicyUpdate**: Terms of service or policy changes

### Transactional (if applicable)

- **OrderConfirmed**: Order was successfully placed
- **PaymentProcessed**: Payment was successful
- **SubscriptionExpiring**: Subscription renewal reminder
- **InvoiceGenerated**: New invoice available

## Additional Resources

### Data Flow Pattern

The notification system follows the standard LightNap data flow pattern described in the [Project Structure](../concepts/project-structure) documentation:

```mermaid
graph TD
  subgraph Database
    DB[(Database)]
  end

  subgraph Backend
    NotificationService[NotificationService]
    MeController[MeController]
  end

  subgraph Frontend
    NotificationDataService[NotificationDataService]
    NotificationAppService[NotificationService]
    NotificationComponents[Notification Components]
  end

  NotificationService -.-> |Entity Framework| DB
  MeController --> |NotificationService methods| NotificationService
  NotificationDataService -.-> |GET/POST /api/users/me/notifications| MeController
  NotificationAppService --> NotificationDataService
  NotificationComponents --> NotificationAppService
```

### Related Documentation

- [Solution & Project Structure](../concepts/project-structure) - Understanding the overall architecture
- [Adding Entities](./adding-entities) - Creating database entities for custom notification data
- [API Response Model](../concepts/api-response-model) - Understanding REST API patterns
- [Working With Roles](./working-with-roles) - Implementing role-based notification permissions
- [Working With Custom Claims](./custom-claims) - Using claims for granular notification access

### Email Notifications

While this article focuses on in-app notifications, LightNap also supports email notifications through the [Email Providers](../getting-started/email-providers) system. Consider implementing both types of notifications for important events:

- **In-App Notifications**: For real-time, interactive alerts within the application
- **Email Notifications**: For critical alerts that should reach users even when they're not actively using the application

Many applications send both types for important events like security alerts or critical system notifications.

For a comprehensive guide on implementing email notifications, see [Adding Backend Email Scenarios](./adding-email-scenarios).
