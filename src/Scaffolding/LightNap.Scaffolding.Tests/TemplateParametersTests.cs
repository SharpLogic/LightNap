using LightNap.Scaffolding.AssemblyManager;
using LightNap.Scaffolding.ServiceRunner;
using LightNap.Scaffolding.TemplateManager;

namespace LightNap.Scaffolding.Tests
{
    [TestClass]
    public class TemplateParametersTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeReplacementsCorrectly()
        {
            // Arrange
            string pascalName = "TestClass";
            var propertiesDetails = new List<TypePropertyDetails>
            {
                new(typeof(int), "Id", false, true, true),
                new(typeof(int), "TestInt", false, true, true),
                new(typeof(string), "TestString", false, true, true)
            };
            var serviceParameters = new ServiceParameters("TestClass", "./", "LightNap.Core", "", "", "", false, false);

            // Act
            var templateParameters = new TemplateParameters(pascalName, propertiesDetails, serviceParameters);

            // Assert
            Assert.AreEqual("TestClass", templateParameters.PascalName);
            Assert.AreEqual("TestClasses", templateParameters.PascalNamePlural);
            Assert.AreEqual("TestClasses", templateParameters.NameForNamespace);
            Assert.AreEqual("testClass", templateParameters.CamelName);
            Assert.AreEqual("testClasses", templateParameters.CamelNamePlural);
            Assert.AreEqual("test-class", templateParameters.KebabName);
            Assert.AreEqual("test-classes", templateParameters.KebabNamePlural);
            Assert.AreEqual("number", templateParameters.IdProperty!.FrontEndType);
            Assert.AreEqual("int", templateParameters.IdProperty!.BackEndType);
        }
    }
}