using LightNap.Scaffolding.AssemblyManager;

namespace LightNap.Scaffolding.Tests
{
    [TestClass]
    public class TypeHelperTests
    {
        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } = "Test";
            public DateTime CreatedDate { get; set; }
            public Guid UniqueId { get; set; }
            public decimal Amount { get; set; }
            public int? NullableInt { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public Guid? NullableGuid { get; set; }
            public decimal? NullableDecimal { get; set; }
            public List<string> IgnoredProperty { get; set; } = [];
        }

        [TestMethod]
        public void GetPropertyDetails_ShouldReturnCorrectProperties()
        {
            // Arrange
            Type type = typeof(TestClass);

            // Act
            List<TypePropertyDetails> result = TypeHelper.GetPropertyDetails(type);

            // Assert
            Assert.HasCount(9, result);
            Assert.IsTrue(result.Exists(p => p.Name == "Id" && p.Type == typeof(int)));
            Assert.IsTrue(result.Exists(p => p.Name == "Name" && p.Type == typeof(string)));
            Assert.IsTrue(result.Exists(p => p.Name == "CreatedDate" && p.Type == typeof(DateTime)));
            Assert.IsTrue(result.Exists(p => p.Name == "UniqueId" && p.Type == typeof(Guid)));
            Assert.IsTrue(result.Exists(p => p.Name == "Amount" && p.Type == typeof(decimal)));
            Assert.IsTrue(result.Exists(p => p.Name == "NullableInt" && p.Type == typeof(int?)));
            Assert.IsTrue(result.Exists(p => p.Name == "NullableDateTime" && p.Type == typeof(DateTime?)));
            Assert.IsTrue(result.Exists(p => p.Name == "NullableGuid" && p.Type == typeof(Guid?)));
            Assert.IsTrue(result.Exists(p => p.Name == "NullableDecimal" && p.Type == typeof(decimal?)));
        }
    }
}