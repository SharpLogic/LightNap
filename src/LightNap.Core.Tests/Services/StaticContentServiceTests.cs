using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Enums;
using LightNap.Core.StaticContents.Models;
using LightNap.Core.StaticContents.Services;
using LightNap.Core.Tests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class StaticContentServiceTests
    {
#pragma warning disable CS8618
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private TestUserContext _userContext;
        private StaticContentService _staticContentService;
#pragma warning restore CS8618

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase()
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            this._userContext = new TestUserContext();
            services.AddScoped<IUserContext>(sp => this._userContext);
            services.AddScoped<StaticContentService>();

            var serviceProvider = services.BuildServiceProvider();
            this._dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this._userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._staticContentService = serviceProvider.GetRequiredService<StaticContentService>();

            // Set up admin context for content creation
            this._userContext.Roles.Add(Constants.Roles.Administrator);
            this._userContext.UserId = "admin-user";
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._dbContext.Database.EnsureDeleted();
            this._dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithValidKey_ReturnsContent()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            var staticContent = await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Test content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("test-key", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("Test content", result.Content.Content);
            Assert.AreEqual(StaticContentFormat.Markdown, result.Content.Format);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithNonExistentKey_ReturnsNull()
        {
            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("non-existent", "en");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithMissingLanguage_FallsBackToDefault()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Default content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("test-key", "fr");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("Default content", result.Content.Content);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithDraftStatus_ReturnsNullForUnauthorizedUser()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "draft-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Draft content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("draft-key", "en", languageDto);

            this._userContext.UserId = null; // Unauthenticated user
            this._userContext.Roles.Clear();

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("draft-key", "en");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetStaticContentAsync_WithValidKey_ReturnsContent()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            // Act
            var result = await this._staticContentService.GetStaticContentAsync("test-key");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentType.Page, result.Type);
            Assert.AreEqual(StaticContentStatus.Published, result.Status);
        }

        [TestMethod]
        public async Task SearchStaticContentAsync_ReturnsPagedResults()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                var createDto = new CreateStaticContentDto
                {
                    Key = $"key-{i}",
                    Type = StaticContentType.Page,
                    Status = StaticContentStatus.Published,
                    ReadAccess = StaticContentReadAccess.Public,
                };
                await this._staticContentService.CreateStaticContentAsync(createDto);
            }

            var searchDto = new SearchStaticContentRequestDto
            {
                PageNumber = 1,
                PageSize = 3,
                SortBy = StaticContentSortBy.Key
            };

            // Act
            var result = await this._staticContentService.SearchStaticContentAsync(searchDto);

            // Assert
            Assert.HasCount(3, result.Data);
            Assert.AreEqual(5, result.TotalCount);
        }

        [TestMethod]
        public async Task CreateStaticContentAsync_WithAuthorization_CreatesContent()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "new-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Act
            var result = await this._staticContentService.CreateStaticContentAsync(createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentType.Page, result.Type);
            Assert.AreEqual(StaticContentStatus.Draft, result.Status);

            var dbContent = await this._dbContext.StaticContents.FindAsync(result.Id);
            Assert.IsNotNull(dbContent);
        }

        [TestMethod]
        public async Task UpdateStaticContentAsync_WithValidKey_UpdatesContent()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var updateDto = new UpdateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Act
            var result = await this._staticContentService.UpdateStaticContentAsync("test-key", updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentStatus.Published, result.Status);
        }

        [TestMethod]
        public async Task DeleteStaticContentAsync_WithValidKey_DeletesContent()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            // Act
            await this._staticContentService.DeleteStaticContentAsync("test-key");

            // Assert
            var dbContent = await this._dbContext.StaticContents
                .FirstOrDefaultAsync(c => c.Key == "test-key");
            Assert.IsNull(dbContent);
        }

        [TestMethod]
        public async Task GetStaticContentLanguageAsync_WithValidKeyAndLanguage_ReturnsLanguage()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "English content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            // Act
            var result = await this._staticContentService.GetStaticContentLanguageAsync("test-key", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("English content", result.Content);
            Assert.AreEqual(StaticContentFormat.Markdown, result.Format);
        }

        [TestMethod]
        public async Task CreateStaticContentLanguageAsync_CreatesLanguageVariant()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "French content",
                Format = StaticContentFormat.Html
            };

            // Act
            var result = await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "fr", languageDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("French content", result.Content);
            Assert.AreEqual(StaticContentFormat.Html, result.Format);
        }

        [TestMethod]
        public async Task UpdateStaticContentLanguageAsync_UpdatesLanguageVariant()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Old content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            var updateDto = new UpdateStaticContentLanguageDto
            {
                Content = "Updated content",
                Format = StaticContentFormat.Html
            };

            // Act
            var result = await this._staticContentService.UpdateStaticContentLanguageAsync("test-key", "en", updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated content", result.Content);
            Assert.AreEqual(StaticContentFormat.Html, result.Format);
        }

        [TestMethod]
        public async Task DeleteStaticContentLanguageAsync_DeletesLanguageVariant()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            var staticContent = await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "English content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            // Act
            await this._staticContentService.DeleteStaticContentLanguageAsync("test-key", "en");

            // Assert
            var dbLanguage = await this._dbContext.StaticContentLanguages
                .FirstOrDefaultAsync(l => l.StaticContentId == staticContent.Id && l.LanguageCode == "en");
            Assert.IsNull(dbLanguage);
        }

        [TestMethod]
        public async Task CreateStaticContentAsync_WithDuplicateKey_ThrowsException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "duplicate-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            // Act
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.CreateStaticContentAsync(createDto);
            });
        }

        [TestMethod]
        public async Task CreateStaticContentAsync_WithInvalidKey_ThrowsException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "", // Invalid key
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Act
            await Assert.ThrowsExactlyAsync<ValidationException>(async () =>
            {
                await this._staticContentService.CreateStaticContentAsync(createDto);
            });
        }

        [TestMethod]
        public async Task UpdateStaticContentAsync_WithNonExistentKey_ThrowsOrReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateStaticContentDto
            {
                Key = "non-existent-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Act
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                var result = await this._staticContentService.UpdateStaticContentAsync("non-existent-key", updateDto);
            });
        }

        [TestMethod]
        public async Task CreateStaticContentLanguageAsync_WithNonExistentContentKey_ThrowsException()
        {
            // Arrange
            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Some content",
                Format = StaticContentFormat.Markdown
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.CreateStaticContentLanguageAsync("non-existent-key", "en", languageDto);
            });
        }

        [TestMethod]
        public async Task UpdateStaticContentLanguageAsync_WithNonExistentLanguage_ThrowsException()
        {
            // Arrange
            var updateDto = new UpdateStaticContentLanguageDto
            {
                Content = "Updated content",
                Format = StaticContentFormat.Html
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.UpdateStaticContentLanguageAsync("test-key", "non-existent-lang", updateDto);
            });
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithRequiresAuthenticationTrue_DeniesAnonymousAccess()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "auth-required",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Authenticated,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Protected content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("auth-required", "en", languageDto);

            // Remove authentication
            this._userContext.UserId = null;
            this._userContext.Roles.Clear();

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("auth-required", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentUserVisibility.RequiresAuthentication, result.Visibility);
            Assert.IsNull(result.Content);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithRequiresAuthenticationTrue_AllowsAuthenticatedUser()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "auth-required",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Protected content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("auth-required", "en", languageDto);

            // Set authenticated user without admin role
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Add viewer claim for this specific content
            var content = await this._dbContext.StaticContents.FirstAsync(c => c.Key == "auth-required");
            this._userContext.Claims.Add((Constants.Claims.ContentReader, new List<string> { content.Id.ToString() }));

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("auth-required", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("Protected content", result.Content.Content);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithContentEditorClaim_AllowsAccess()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "editor-content",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Editor content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("editor-content", "en", languageDto);

            // Set authenticated user with editor claim
            this._userContext.UserId = "editor-user";
            this._userContext.Roles.Clear();

            var content = await this._dbContext.StaticContents.FirstAsync(c => c.Key == "editor-content");
            this._userContext.Claims.Add((Constants.Claims.ContentEditor, new List<string> { content.Id.ToString() }));

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("editor-content", "en");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithoutRequiredClaims_DeniesAccess()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "protected-content",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Explicit,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Protected content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("protected-content", "en", languageDto);

            // Set authenticated user without claims
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("protected-content", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentUserVisibility.Restricted, result.Visibility);
            Assert.IsNull(result.Content);
        }

        [TestMethod]
        public async Task GetStaticContentAsync_WithoutEditPermission_ThrowsUnauthorizedException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            // Set non-admin user
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.GetStaticContentAsync("test-key");
            });
        }

        [TestMethod]
        public async Task CreateStaticContentAsync_WithoutPermission_ThrowsUnauthorizedException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "new-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Remove admin role
            this._userContext.Roles.Clear();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.CreateStaticContentAsync(createDto);
            });
        }

        [TestMethod]
        public async Task UpdateStaticContentAsync_WithContentEditorClaim_AllowsUpdate()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            var created = await this._staticContentService.CreateStaticContentAsync(createDto);

            // Set user with editor claim for this content
            this._userContext.UserId = "editor-user";
            this._userContext.Roles.Clear();
            this._userContext.Claims.Add((Constants.Claims.ContentEditor, new List<string> { created.Id.ToString() }));

            var updateDto = new UpdateStaticContentDto
            {
                Key = "test-key-updated",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Act
            var result = await this._staticContentService.UpdateStaticContentAsync("test-key", updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test-key-updated", result.Key);
        }

        [TestMethod]
        public async Task UpdateStaticContentAsync_WhenStatusChanges_UpdatesStatusChangedDate()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            var created = await this._staticContentService.CreateStaticContentAsync(createDto);

            var originalStatusDate = (await this._dbContext.StaticContents.FindAsync(created.Id))?.StatusChangedDate;

            await Task.Delay(100); // Ensure time difference

            var updateDto = new UpdateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };

            // Act
            await this._staticContentService.UpdateStaticContentAsync("test-key", updateDto);

            // Assert
            var dbContent = await this._dbContext.StaticContents.FindAsync(created.Id);
            Assert.IsNotNull(dbContent);
            Assert.IsNotNull(dbContent.StatusChangedDate);
            Assert.AreNotEqual(originalStatusDate, dbContent.StatusChangedDate);
        }

        [TestMethod]
        public async Task GetStaticContentLanguagesAsync_WithMultipleLanguages_ReturnsAll()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "multi-lang",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            await this._staticContentService.CreateStaticContentLanguageAsync("multi-lang", "en", new CreateStaticContentLanguageDto
            {
                Content = "English",
                Format = StaticContentFormat.Markdown
            });

            await this._staticContentService.CreateStaticContentLanguageAsync("multi-lang", "fr", new CreateStaticContentLanguageDto
            {
                Content = "French",
                Format = StaticContentFormat.Markdown
            });

            await this._staticContentService.CreateStaticContentLanguageAsync("multi-lang", "es", new CreateStaticContentLanguageDto
            {
                Content = "Spanish",
                Format = StaticContentFormat.Markdown
            });

            // Act
            var result = await this._staticContentService.GetStaticContentLanguagesAsync("multi-lang");

            // Assert
            Assert.HasCount(3, result);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithNoLanguageVariants_ReturnsNull()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "no-languages",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            // Act
            this._userContext.UserId = null;
            this._userContext.Roles.Clear();
            var result = await this._staticContentService.GetPublishedStaticContentAsync("no-languages", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentUserVisibility.Reader, result.Visibility);
            Assert.IsNull(result.Content);
        }

        [TestMethod]
        public async Task CreateStaticContentLanguageAsync_WithUnsupportedLanguage_ThrowsException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Content",
                Format = StaticContentFormat.Markdown
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "invalid-lang", languageDto);
            });
        }

        [TestMethod]
        public async Task SearchStaticContentAsync_WithStatusFilter_ReturnsFilteredResults()
        {
            // Arrange
            await this._staticContentService.CreateStaticContentAsync(new CreateStaticContentDto
            {
                Key = "draft-1",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            });

            await this._staticContentService.CreateStaticContentAsync(new CreateStaticContentDto
            {
                Key = "published-1",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            });

            var searchDto = new SearchStaticContentRequestDto
            {
                PageNumber = 1,
                PageSize = 10,
                Status = StaticContentStatus.Published,
                SortBy = StaticContentSortBy.Key
            };

            // Act
            var result = await this._staticContentService.SearchStaticContentAsync(searchDto);

            // Assert
            Assert.HasCount(1, result.Data);
            Assert.AreEqual(StaticContentStatus.Published, result.Data[0].Status);
        }

        [TestMethod]
        public async Task SearchStaticContentAsync_WithTypeFilter_ReturnsFilteredResults()
        {
            // Arrange
            await this._staticContentService.CreateStaticContentAsync(new CreateStaticContentDto
            {
                Key = "page-1",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            });

            await this._staticContentService.CreateStaticContentAsync(new CreateStaticContentDto
            {
                Key = "zone-1",
                Type = StaticContentType.Zone,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            });

            var searchDto = new SearchStaticContentRequestDto
            {
                PageNumber = 1,
                PageSize = 10,
                Type = StaticContentType.Zone,
                SortBy = StaticContentSortBy.Key
            };

            // Act
            var result = await this._staticContentService.SearchStaticContentAsync(searchDto);

            // Assert
            Assert.HasCount(1, result.Data);
            Assert.AreEqual(StaticContentType.Zone, result.Data[0].Type);
        }

        [TestMethod]
        public async Task SearchStaticContentAsync_WithKeyContainsFilter_ReturnsMatchingResults()
        {
            // Arrange
            await this._staticContentService.CreateStaticContentAsync(new CreateStaticContentDto
            {
                Key = "about-us",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            });

            await this._staticContentService.CreateStaticContentAsync(new CreateStaticContentDto
            {
                Key = "contact-us",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            });

            var searchDto = new SearchStaticContentRequestDto
            {
                PageNumber = 1,
                PageSize = 10,
                KeyContains = "about",
                SortBy = StaticContentSortBy.Key
            };

            // Act
            var result = await this._staticContentService.SearchStaticContentAsync(searchDto);

            // Assert
            Assert.HasCount(1, result.Data);
            Assert.AreEqual("about-us", result.Data[0].Key);
        }

        [TestMethod]
        public async Task UpdateStaticContentAsync_WithRequiredRoles_AutoSetsRequiresAuthentication()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Draft,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var updateDto = new UpdateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
                EditorRoles = "Editor,Moderator"
            };

            // Act
            var result = await this._staticContentService.UpdateStaticContentAsync("test-key", updateDto);

            // Assert
            var dbContent = await this._dbContext.StaticContents.FindAsync(result.Id);
            Assert.IsNotNull(dbContent);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithRequiredRoles_DeniesUserWithoutRole()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "role-content",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Explicit
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Role-protected content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("role-content", "en", languageDto);

            // Set user without required role
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("role-content", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StaticContentUserVisibility.Restricted, result.Visibility);
            Assert.IsNull(result.Content);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithRequiredReaderRoles_AllowsUserWithRole()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "role-content",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Explicit,
                ReaderRoles = "Editor,Moderator"
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Role-protected content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("role-content", "en", languageDto);

            // Set user with one of the required viewer roles
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();
            this._userContext.Roles.Add("Editor");

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("role-content", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("Role-protected content", result.Content.Content);
        }

        [TestMethod]
        public async Task UpdateStaticContentLanguageAsync_WithoutPermission_ThrowsUnauthorizedException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Original content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            // Remove admin role
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            var updateDto = new UpdateStaticContentLanguageDto
            {
                Content = "Updated content",
                Format = StaticContentFormat.Html
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.UpdateStaticContentLanguageAsync("test-key", "en", updateDto);
            });
        }

        [TestMethod]
        public async Task DeleteStaticContentLanguageAsync_WithoutPermission_ThrowsUnauthorizedException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "English content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", languageDto);

            // Remove admin role
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.DeleteStaticContentLanguageAsync("test-key", "en");
            });
        }

        [TestMethod]
        public void GetSupportedLanguages_ReturnsExpectedLanguages()
        {
            // Act
            var result = this._staticContentService.GetSupportedLanguages();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Any(lang => lang.LanguageCode == "en"), "Expected 'en' language to be in supported languages");
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithAuthenticatedAccess_AllowsAuthenticatedUser()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "authenticated-content",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Authenticated,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Authenticated content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("authenticated-content", "en", languageDto);

            // Set authenticated user (without any special roles or claims)
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("authenticated-content", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("Authenticated content", result.Content.Content);
        }

        [TestMethod]
        public async Task GetPublishedStaticContentAsync_WithExplicitAccessAndViewerRole_AllowsAccess()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "explicit-content",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Explicit,
                ReaderRoles = "Moderator,Editor"
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            var languageDto = new CreateStaticContentLanguageDto
            {
                Content = "Explicit access content",
                Format = StaticContentFormat.Markdown
            };
            await this._staticContentService.CreateStaticContentLanguageAsync("explicit-content", "en", languageDto);

            // Set authenticated user with one of the viewer roles
            this._userContext.UserId = "moderator-user";
            this._userContext.Roles.Clear();
            this._userContext.Roles.Add("Moderator");

            // Act
            var result = await this._staticContentService.GetPublishedStaticContentAsync("explicit-content", "en");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("Explicit access content", result.Content.Content);
        }

        [TestMethod]
        public async Task GetStaticContentLanguagesAsync_WithoutPermission_ThrowsUnauthorizedException()
        {
            // Arrange
            var createDto = new CreateStaticContentDto
            {
                Key = "test-key",
                Type = StaticContentType.Page,
                Status = StaticContentStatus.Published,
                ReadAccess = StaticContentReadAccess.Public,
            };
            await this._staticContentService.CreateStaticContentAsync(createDto);

            await this._staticContentService.CreateStaticContentLanguageAsync("test-key", "en", new CreateStaticContentLanguageDto
            {
                Content = "English",
                Format = StaticContentFormat.Markdown
            });

            // Remove admin role
            this._userContext.UserId = "regular-user";
            this._userContext.Roles.Clear();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<UserFriendlyApiException>(async () =>
            {
                await this._staticContentService.GetStaticContentLanguagesAsync("test-key");
            });
        }
    }
}