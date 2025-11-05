using LightNap.Core.Api;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Interfaces;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Enums;
using LightNap.Core.Notifications.Interfaces;
using LightNap.Core.Notifications.Services;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class NotificationServiceTests
    {
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private INotificationService _notificationService;
#pragma warning restore CS8618

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            this._userContext = new TestUserContext();
            services.AddScoped<IUserContext>(sp => this._userContext);
            services.AddScoped<INotificationService, NotificationService>();

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            this._notificationService = serviceProvider.GetRequiredService<INotificationService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        #region CreateSystemNotificationForUserAsync Tests

        [TestMethod]
        public async Task CreateSystemNotificationForUserAsync_ValidUser_CreatesNotification()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            var requestDto = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Test notification" } }
            };

            // Act
            await this._notificationService.CreateSystemNotificationForUserAsync(userId, requestDto);

            // Assert
            var notifications = this._dbContext.Notifications.Where(n => n.UserId == userId).ToList();
            Assert.HasCount(1, notifications);
            Assert.AreEqual(NotificationType.AdministratorNewUserRegistration, notifications[0].Type);
            Assert.AreEqual(NotificationStatus.Unread, notifications[0].Status);
            Assert.IsTrue(notifications[0].Data.ContainsKey("message"));
        }

        [TestMethod]
        public async Task CreateSystemNotificationForUserAsync_MultipleNotifications_CreatesAll()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            var requestDto1 = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "First notification" } }
            };
            var requestDto2 = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Second notification" } }
            };

            // Act
            await this._notificationService.CreateSystemNotificationForUserAsync(userId, requestDto1);
            await this._notificationService.CreateSystemNotificationForUserAsync(userId, requestDto2);

            // Assert
            var notifications = this._dbContext.Notifications.Where(n => n.UserId == userId).ToList();
            Assert.HasCount(2, notifications);
        }

        #endregion

        #region CreateSystemNotificationForRoleAsync Tests

        [TestMethod]
        public async Task CreateSystemNotificationForRoleAsync_UsersInRole_CreatesNotificationsForAll()
        {
            // Arrange
            var roleName = "TestRole";
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "user-2");
            var user3 = await TestHelper.CreateTestUserAsync(this._userManager, "user-3");

            await TestHelper.CreateTestRoleAsync(this._roleManager, roleName);
            await this._userManager.AddToRoleAsync(user1, roleName);
            await this._userManager.AddToRoleAsync(user2, roleName);

            var requestDto = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Role notification" } }
            };

            // Act
            await this._notificationService.CreateSystemNotificationForRoleAsync(roleName, requestDto);

            // Assert
            var user1Notifications = this._dbContext.Notifications.Where(n => n.UserId == user1.Id).ToList();
            var user2Notifications = this._dbContext.Notifications.Where(n => n.UserId == user2.Id).ToList();
            var user3Notifications = this._dbContext.Notifications.Where(n => n.UserId == user3.Id).ToList();

            Assert.HasCount(1, user1Notifications);
            Assert.HasCount(1, user2Notifications);
            Assert.HasCount(0, user3Notifications);
        }

        [TestMethod]
        public async Task CreateSystemNotificationForRoleAsync_NoUsersInRole_CreatesNoNotifications()
        {
            // Arrange
            var roleName = "EmptyRole";
            await TestHelper.CreateTestRoleAsync(this._roleManager, roleName);

            var requestDto = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Role notification" } }
            };

            // Act
            await this._notificationService.CreateSystemNotificationForRoleAsync(roleName, requestDto);

            // Assert
            var notifications = this._dbContext.Notifications.ToList();
            Assert.HasCount(0, notifications);
        }

        #endregion

        #region CreateSystemNotificationForClaimAsync Tests

        [TestMethod]
        public async Task CreateSystemNotificationForClaimAsync_UsersWithClaim_CreatesNotificationsForAll()
        {
            // Arrange
            var claimType = "test-claim-type";
            var claimValue = "test-claim-value";
            var user1 = await TestHelper.CreateTestUserAsync(this._userManager, "user-1");
            var user2 = await TestHelper.CreateTestUserAsync(this._userManager, "user-2");
            var user3 = await TestHelper.CreateTestUserAsync(this._userManager, "user-3");

            await this._userManager.AddClaimAsync(user1, new Claim(claimType, claimValue));
            await this._userManager.AddClaimAsync(user2, new Claim(claimType, claimValue));

            var requestDto = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Claim notification" } }
            };

            // Act
            await this._notificationService.CreateSystemNotificationForClaimAsync(
                new ClaimDto { Type = claimType, Value = claimValue },
                requestDto);

            // Assert
            var user1Notifications = this._dbContext.Notifications.Where(n => n.UserId == user1.Id).ToList();
            var user2Notifications = this._dbContext.Notifications.Where(n => n.UserId == user2.Id).ToList();
            var user3Notifications = this._dbContext.Notifications.Where(n => n.UserId == user3.Id).ToList();

            Assert.HasCount(1, user1Notifications);
            Assert.HasCount(1, user2Notifications);
            Assert.HasCount(0, user3Notifications);
        }

        [TestMethod]
        public async Task CreateSystemNotificationForClaimAsync_NoUsersWithClaim_CreatesNoNotifications()
        {
            // Arrange
            await TestHelper.CreateTestUserAsync(this._userManager, "user-1");

            var requestDto = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Claim notification" } }
            };

            // Act
            await this._notificationService.CreateSystemNotificationForClaimAsync(
                new ClaimDto { Type = "non-existent-claim", Value = "non-existent-value" },
                requestDto);

            // Assert
            var notifications = this._dbContext.Notifications.ToList();
            Assert.HasCount(0, notifications);
        }

        #endregion

        #region GetNotificationAsync Tests

        [TestMethod]
        public async Task GetNotificationAsync_NotificationExists_ReturnsNotification()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            var requestDto = new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Test notification" } }
            };
            await this._notificationService.CreateSystemNotificationForUserAsync(userId, requestDto);
            var notification = this._dbContext.Notifications.First();

            // Act
            var result = await this._notificationService.GetNotificationAsync(notification.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(notification.Id, result.Id);
            Assert.AreEqual(NotificationType.AdministratorNewUserRegistration, result.Type);
            Assert.AreEqual(NotificationStatus.Unread, result.Status);
        }

        [TestMethod]
        public async Task GetNotificationAsync_NotificationDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await this._notificationService.GetNotificationAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region MarkAllAsReadAsync Tests

        [TestMethod]
        public async Task MarkAllAsReadAsync_MultipleUnreadNotifications_MarksAllAsRead()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            for (int i = 0; i < 3; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            // Verify they're unread
            var unreadNotifications = this._dbContext.Notifications.Where(n => n.UserId == userId && n.Status == NotificationStatus.Unread).ToList();
            Assert.HasCount(3, unreadNotifications);

            // Act
            await this._notificationService.MarkAllAsReadAsync(userId);

            // Assert
            var readNotifications = this._dbContext.Notifications.Where(n => n.UserId == userId && n.Status == NotificationStatus.Read).ToList();
            Assert.HasCount(3, readNotifications);
        }

        [TestMethod]
        public async Task MarkAllAsReadAsync_NoNotifications_DoesNotThrow()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            // Act & Assert
            await this._notificationService.MarkAllAsReadAsync(userId);
        }

        [TestMethod]
        public async Task MarkAllAsReadAsync_OnlyMarksSpecifiedUserNotifications()
        {
            // Arrange
            var user1Id = "user-1";
            var user2Id = "user-2";
            await TestHelper.CreateTestUserAsync(this._userManager, user1Id);
            await TestHelper.CreateTestUserAsync(this._userManager, user2Id);

            await this._notificationService.CreateSystemNotificationForUserAsync(user1Id, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "User 1 notification" } }
            });
            await this._notificationService.CreateSystemNotificationForUserAsync(user2Id, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "User 2 notification" } }
            });

            // Act
            await this._notificationService.MarkAllAsReadAsync(user1Id);

            // Assert
            var user1Notifications = this._dbContext.Notifications.Where(n => n.UserId == user1Id).ToList();
            var user2Notifications = this._dbContext.Notifications.Where(n => n.UserId == user2Id).ToList();

            Assert.AreEqual(NotificationStatus.Read, user1Notifications[0].Status);
            Assert.AreEqual(NotificationStatus.Unread, user2Notifications[0].Status);
        }

        #endregion

        #region MarkAsReadAsync Tests

        [TestMethod]
        public async Task MarkAsReadAsync_NotificationExists_MarksAsRead()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Test notification" } }
            });
            var notification = this._dbContext.Notifications.First();
            Assert.AreEqual(NotificationStatus.Unread, notification.Status);

            // Act
            await this._notificationService.MarkAsReadAsync(notification.Id);

            // Assert
            var updatedNotification = this._dbContext.Notifications.First(n => n.Id == notification.Id);
            Assert.AreEqual(NotificationStatus.Read, updatedNotification.Status);
        }

        [TestMethod]
        public async Task MarkAsReadAsync_NotificationDoesNotExist_ThrowsError()
        {
            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._notificationService.MarkAsReadAsync(999));
        }

        #endregion

        #region SearchNotificationsAsync Tests

        [TestMethod]
        public async Task SearchNotificationsAsync_NoFilters_ReturnsAllUserNotifications()
        {
            // Arrange
            var userId = "test-user-id";
            var otherUserId = "other-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            await TestHelper.CreateTestUserAsync(this._userManager, otherUserId);

            for (int i = 0; i < 3; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }
            await this._notificationService.CreateSystemNotificationForUserAsync(otherUserId, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Other user notification" } }
            });

            // Act
            var result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(3, result.Data);
            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(3, result.UnreadCount);
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_WithSinceId_ReturnsNotificationsAfterSpecifiedId()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            for (int i = 0; i < 5; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            var notifications = this._dbContext.Notifications.OrderBy(n => n.Id).ToList();
            var sinceId = notifications[2].Id;

            // Act
            var result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                SinceId = sinceId,
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(2, result.Data);
            Assert.IsTrue(result.Data.All(n => n.Id > sinceId));
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_WithPriorToId_ReturnsNotificationsBeforeSpecifiedId()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            for (int i = 0; i < 5; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            var notifications = this._dbContext.Notifications.OrderBy(n => n.Id).ToList();
            var priorToId = notifications[3].Id;

            // Act
            var result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                PriorToId = priorToId,
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(3, result.Data);
            Assert.IsTrue(result.Data.All(n => n.Id < priorToId));
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_WithStatusFilter_ReturnsMatchingNotifications()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            for (int i = 0; i < 3; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            var notifications = this._dbContext.Notifications.ToList();
            await this._notificationService.MarkAsReadAsync(notifications[0].Id);

            // Act
            var unreadResult = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                Status = NotificationStatus.Unread,
                PageNumber = 1,
                PageSize = 10
            });

            var readResult = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                Status = NotificationStatus.Read,
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(2, unreadResult.Data);
            Assert.HasCount(1, readResult.Data);
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_WithTypeFilter_ReturnsMatchingNotifications()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "General notification" } }
            });
            await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Alert notification" } }
            });

            // Act
            var generalResult = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                PageNumber = 1,
                PageSize = 10
            });

            var alertResult = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(2, generalResult.Data);
            Assert.AreEqual(NotificationType.AdministratorNewUserRegistration, generalResult.Data[0].Type);
            Assert.HasCount(2, alertResult.Data);
            Assert.AreEqual(NotificationType.AdministratorNewUserRegistration, alertResult.Data[0].Type);
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_ExcludesArchivedByDefault()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Active notification" } }
            });

            // Create and archive a notification manually
            var archivedNotification = new Notification
            {
                UserId = userId,
                Type = NotificationType.AdministratorNewUserRegistration,
                Status = NotificationStatus.Archived,
                Data = new Dictionary<string, object> { { "message", "Archived notification" } },
                Timestamp = DateTime.UtcNow
            };
            this._dbContext.Notifications.Add(archivedNotification);
            await this._dbContext.SaveChangesAsync();

            // Act
            var result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(1, result.Data);
            Assert.AreNotEqual(NotificationStatus.Archived, result.Data[0].Status);
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            for (int i = 0; i < 10; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            // Act
            var page1Result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                PageNumber = 1,
                PageSize = 3
            });

            var page2Result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                PageNumber = 2,
                PageSize = 3
            });

            // Assert
            Assert.HasCount(3, page1Result.Data);
            Assert.HasCount(3, page2Result.Data);
            Assert.AreEqual(10, page1Result.TotalCount);
            Assert.AreEqual(10, page2Result.TotalCount);
            Assert.AreNotEqual(page1Result.Data[0].Id, page2Result.Data[0].Id);
        }

        [TestMethod]
        public async Task SearchNotificationsAsync_ReturnsUnreadCount()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);

            for (int i = 0; i < 5; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            var notifications = this._dbContext.Notifications.Take(2).ToList();
            foreach (var notification in notifications)
            {
                await this._notificationService.MarkAsReadAsync(notification.Id);
            }

            // Act
            var result = await this._notificationService.SearchNotificationsAsync(userId, new SearchNotificationsRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.AreEqual(3, result.UnreadCount);
        }

        #endregion

        #region SearchMyNotificationsAsync Tests

        [TestMethod]
        public async Task SearchMyNotificationsAsync_AuthenticatedUser_ReturnsUserNotifications()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this._userContext.LogIn(userId);

            for (int i = 0; i < 3; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            // Act
            var result = await this._notificationService.SearchMyNotificationsAsync(new SearchNotificationsRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Assert
            Assert.HasCount(3, result.Data);
        }

        [TestMethod]
        public async Task SearchMyNotificationsAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange
            this._userContext.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
                await this._notificationService.SearchMyNotificationsAsync(new SearchNotificationsRequestDto
                {
                    PageNumber = 1,
                    PageSize = 10
                }));
        }

        #endregion

        #region MarkAllMyNotificationsAsReadAsync Tests

        [TestMethod]
        public async Task MarkAllMyNotificationsAsReadAsync_AuthenticatedUser_MarksAllAsRead()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this._userContext.LogIn(userId);

            for (int i = 0; i < 3; i++)
            {
                await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
                {
                    Type = NotificationType.AdministratorNewUserRegistration,
                    Data = new Dictionary<string, object> { { "message", $"Notification {i}" } }
                });
            }

            // Act
            await this._notificationService.MarkAllMyNotificationsAsReadAsync();

            // Assert
            var notifications = this._dbContext.Notifications.Where(n => n.UserId == userId).ToList();
            Assert.IsTrue(notifications.All(n => n.Status == NotificationStatus.Read));
        }

        [TestMethod]
        public async Task MarkAllMyNotificationsAsReadAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange
            this._userContext.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
                await this._notificationService.MarkAllMyNotificationsAsReadAsync());
        }

        #endregion

        #region MarkMyNotificationAsReadAsync Tests

        [TestMethod]
        public async Task MarkMyNotificationAsReadAsync_ValidNotification_MarksAsRead()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this._userContext.LogIn(userId);

            await this._notificationService.CreateSystemNotificationForUserAsync(userId, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "Test notification" } }
            });
            var notification = this._dbContext.Notifications.First();

            // Act
            await this._notificationService.MarkMyNotificationAsReadAsync(notification.Id);

            // Assert
            var updatedNotification = this._dbContext.Notifications.First(n => n.Id == notification.Id);
            Assert.AreEqual(NotificationStatus.Read, updatedNotification.Status);
        }

        [TestMethod]
        public async Task MarkMyNotificationAsReadAsync_NotificationDoesNotExist_ThrowsError()
        {
            // Arrange
            var userId = "test-user-id";
            await TestHelper.CreateTestUserAsync(this._userManager, userId);
            this._userContext.LogIn(userId);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._notificationService.MarkMyNotificationAsReadAsync(999));
        }

        [TestMethod]
        public async Task MarkMyNotificationAsReadAsync_NotificationBelongsToOtherUser_ThrowsError()
        {
            // Arrange
            var user1Id = "user-1";
            var user2Id = "user-2";
            await TestHelper.CreateTestUserAsync(this._userManager, user1Id);
            await TestHelper.CreateTestUserAsync(this._userManager, user2Id);

            await this._notificationService.CreateSystemNotificationForUserAsync(user1Id, new CreateNotificationRequestDto
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object> { { "message", "User 1 notification" } }
            });
            var notification = this._dbContext.Notifications.First();

            // User 2 tries to mark User 1's notification as read
            this._userContext.LogIn(user2Id);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
                await this._notificationService.MarkMyNotificationAsReadAsync(notification.Id));
        }

        [TestMethod]
        public async Task MarkMyNotificationAsReadAsync_UserLoggedOut_ThrowsError()
        {
            // Arrange
            this._userContext.LogOut();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
                await this._notificationService.MarkMyNotificationAsReadAsync(1));
        }

        #endregion
    }
}
