using System.Text.Json;
using LightNap.Core.Data.Conversion;

namespace LightNap.Core.Tests.Data
{
    [TestClass]
    public class JsonValueConverterTests
    {
        private sealed class SamplePayload
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
            public List<string> Tags { get; set; } = [];
            public NestedPayload? Nested { get; set; }
        }

        private sealed class NestedPayload
        {
            public string? Value { get; set; }
        }

        [TestMethod]
        public void RoundTrip_PreservesEquality()
        {
            var converter = new JsonValueConverter<SamplePayload>();
            var original = new SamplePayload
            {
                Name = "lightnap",
                Count = 42,
                Tags = ["alpha", "beta"],
                Nested = new NestedPayload { Value = "nested-value" }
            };

            var serialized = (string)converter.ConvertToProvider(original)!;
            var roundTripped = (SamplePayload)converter.ConvertFromProvider(serialized)!;

            Assert.AreEqual(original.Name, roundTripped.Name);
            Assert.AreEqual(original.Count, roundTripped.Count);
            CollectionAssert.AreEqual(original.Tags, roundTripped.Tags);
            Assert.IsNotNull(roundTripped.Nested);
            Assert.AreEqual(original.Nested.Value, roundTripped.Nested.Value);
        }

        [TestMethod]
        public void Default_UsesCamelCaseNaming()
        {
            var converter = new JsonValueConverter<SamplePayload>();
            var serialized = (string)converter.ConvertToProvider(new SamplePayload { Name = "x", Count = 1 })!;

            Assert.IsTrue(serialized.Contains("\"name\""), $"Expected camelCase property names, got: {serialized}");
            Assert.IsTrue(serialized.Contains("\"count\""), $"Expected camelCase property names, got: {serialized}");
        }

        [TestMethod]
        public void Default_OmitsNullProperties()
        {
            var converter = new JsonValueConverter<SamplePayload>();
            var serialized = (string)converter.ConvertToProvider(new SamplePayload { Name = "x", Count = 1, Nested = null })!;

            Assert.IsFalse(serialized.Contains("\"nested\""), $"Expected null property to be omitted, got: {serialized}");
        }

        [TestMethod]
        public void CustomOptions_AreRespected()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
            var converter = new JsonValueConverter<SamplePayload>(options);

            var serialized = (string)converter.ConvertToProvider(new SamplePayload { Name = "x", Count = 1 })!;

            Assert.IsTrue(serialized.Contains("\"Name\""), $"Expected PascalCase, got: {serialized}");
        }
    }
}
