using LightNap.Core.Data.Conversion;
using LightNap.Core.Data.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Tests.Data
{
    [TestClass]
    public class JsonValueConverterIntegrationTests
    {
        private sealed class JsonPayload
        {
            public string Label { get; set; } = "";
            public int Number { get; set; }
            public List<string> Items { get; set; } = [];
        }

        private sealed class JsonTestEntity
        {
            public int Id { get; set; }

            [StoredAsJson]
            public JsonPayload? Payload { get; set; }
        }

        private sealed class JsonTestDbContext(DbContextOptions<JsonTestDbContext> options) : DbContext(options)
        {
            public DbSet<JsonTestEntity> Entities => this.Set<JsonTestEntity>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.ApplyJsonValueConverters();
            }
        }

        private static JsonTestDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<JsonTestDbContext>()
                .UseInMemoryDatabase($"JsonTestDb_{Guid.NewGuid()}")
                .Options;
            return new JsonTestDbContext(options);
        }

        private static (JsonTestDbContext context, SqliteConnection connection) CreateSqliteContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<JsonTestDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new JsonTestDbContext(options);
            context.Database.EnsureCreated();
            return (context, connection);
        }

        private static async Task AssertRoundTripAsync(JsonTestDbContext context)
        {
            var entity = new JsonTestEntity
            {
                Payload = new JsonPayload
                {
                    Label = "round-trip",
                    Number = 7,
                    Items = ["one", "two", "three"]
                }
            };

            context.Entities.Add(entity);
            await context.SaveChangesAsync();

            // Detach so the next read fetches from the store.
            context.ChangeTracker.Clear();

            var loaded = await context.Entities.SingleAsync();
            Assert.IsNotNull(loaded.Payload);
            Assert.AreEqual("round-trip", loaded.Payload.Label);
            Assert.AreEqual(7, loaded.Payload.Number);
            CollectionAssert.AreEqual(new List<string> { "one", "two", "three" }, loaded.Payload.Items);
        }

        [TestMethod]
        public async Task InMemoryProvider_RoundTripsJsonPayload()
        {
            using var context = CreateInMemoryContext();
            await AssertRoundTripAsync(context);
        }

        [TestMethod]
        public async Task SqliteProvider_RoundTripsJsonPayload()
        {
            var (context, connection) = CreateSqliteContext();
            try
            {
                await AssertRoundTripAsync(context);
            }
            finally
            {
                context.Dispose();
                connection.Dispose();
            }
        }

        [TestMethod]
        public async Task SqliteProvider_StoresPayloadAsString()
        {
            var (context, connection) = CreateSqliteContext();
            try
            {
                context.Entities.Add(new JsonTestEntity
                {
                    Payload = new JsonPayload { Label = "hello", Number = 1, Items = ["a"] }
                });
                await context.SaveChangesAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Payload FROM Entities LIMIT 1";
                var raw = (string?)await command.ExecuteScalarAsync();

                Assert.IsNotNull(raw);
                Assert.IsTrue(raw.Contains("\"label\""), $"Expected camelCase JSON, got: {raw}");
                Assert.IsTrue(raw.Contains("\"hello\""), $"Expected payload value, got: {raw}");
            }
            finally
            {
                context.Dispose();
                connection.Dispose();
            }
        }
    }
}
