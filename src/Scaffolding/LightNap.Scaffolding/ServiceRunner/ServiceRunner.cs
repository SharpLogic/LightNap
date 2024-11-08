using LightNap.Scaffolding.AssemblyManager;
using LightNap.Scaffolding.ProjectManager;
using LightNap.Scaffolding.TemplateManager;

namespace LightNap.Scaffolding.ServiceRunner
{
    /// <summary>
    /// Runs the service scaffolding process.
    /// </summary>
    public class ServiceRunner(IProjectManager projectManager, ITemplateManager templateManager, IAssemblyManager assemblyManager)
    {
        /// <summary>
        /// Executes the service scaffolding process with the provided parameters.
        /// </summary>
        /// <param name="parameters">The parameters for the service scaffolding process.</param>
        public void Run(ServiceParameters parameters)
        {
            if (!ValidateParameters(parameters))
            {
                return;
            }

            var projectBuildResult = projectManager.BuildProject(parameters.CoreProjectFilePath);
            if (!projectBuildResult.Success)
            {
                Console.WriteLine("Please fix the build failures and try again.");
                return;
            }

            var type = assemblyManager.LoadType(projectBuildResult.OutputAssemblyPath!, parameters.ClassName);
            if (type == null)
            {
                Console.WriteLine($"Type '{parameters.ClassName}' not found or could not be loaded from assembly.");
                return;
            }

            Console.WriteLine($"Analyzing {type.Name} ({type.FullName})");

            List<TypePropertyDetails> propertiesDetails = TypeHelper.GetPropertyDetails(type);
            TemplateParameters templateParameters = new(type.Name, propertiesDetails, parameters);

            string pascalNamePlural = templateParameters.Replacements["PascalNamePlural"];
            string kebabName = templateParameters.Replacements["KebabName"];
            string kebabNamePlural = templateParameters.Replacements["KebabNamePlural"];

            string executingPath = assemblyManager.GetExecutingPath();

            var templateItems = new List<TemplateItem>
                {
                    new(Path.Combine(executingPath, "Templates/Server/CreateDto.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Request/Create{type.Name}Dto.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/Dto.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Response/{type.Name}Dto.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/Extensions.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Extensions/{type.Name}Extensions.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/Interface.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Interfaces/I{type.Name}Service.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/SearchDto.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Request/Search{type.Name}Dto.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/Service.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Services/{type.Name}Service.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/UpdateDto.cs.txt"), $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Request/Update{type.Name}Dto.cs"),
                    new(Path.Combine(executingPath, "Templates/Server/Controller.cs.txt"), $"{parameters.WebApiProjectPath}/Controllers/{pascalNamePlural}Controller.cs"),

                    new(Path.Combine(executingPath, "Templates/Client/routes.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/routes.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/index.component.html.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/index/index.component.html"),
                    new(Path.Combine(executingPath, "Templates/Client/index.component.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/index/index.component.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/get.component.html.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/get/get.component.html"),
                    new(Path.Combine(executingPath, "Templates/Client/get.component.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/get/get.component.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/create.component.html.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/create/create.component.html"),
                    new(Path.Combine(executingPath, "Templates/Client/create.component.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/create/create.component.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/edit.component.html.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/edit/edit.component.html"),
                    new(Path.Combine(executingPath, "Templates/Client/edit.component.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/components/pages/edit/edit.component.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/create-request.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/models/request/create-{kebabName}-request.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/search-request.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/models/request/search-{kebabNamePlural}-request.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/update-request.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/models/request/update-{kebabName}-request.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/response.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/models/response/{kebabName}.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/data.service.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/services/data.service.ts"),
                    new(Path.Combine(executingPath, "Templates/Client/service.ts.txt"), $"{parameters.ClientAppPath}/{kebabNamePlural}/services/{kebabName}.service.ts"),
                };

            foreach (var template in templateItems)
            {
                if (File.Exists(Path.Combine(parameters.SourcePath, template.OutputFile)))
                {
                    Console.WriteLine($"Bailing out: File '{template.OutputFile}' already exists!");
                    return;
                }
            }

            foreach (var template in templateItems)
            {
                templateManager.ProcessTemplate(template, templateParameters);

                Console.WriteLine($"Generated {template.OutputFile}");
            }

            Console.WriteLine(
@$"Scaffolding completed successfully. Please see TODO comments in generated code to complete integration.

    {parameters.CoreProjectName}:
    - Update client and server DTO properties in {pascalNamePlural}/Dto to only those you want included.
    - Update extension method mappers between DTOs and the entity in Extensions/{type.Name}Extensions.cs.

    {parameters.WebApiProjectName}:
    - Update the authorization for methods in Controllers/{pascalNamePlural}Controller.cs based on access preferences.
    - Register Web API controller parameter dependency in Extensions/ApplicationServiceExtensions.cs.

    {parameters.AngularProjectName}:
    - Update the models in {kebabNamePlural}/models to match the updated server DTOs.
    - Update authorization for the routes in {kebabNamePlural}/components/pages/routes.ts.
    - Add {kebabNamePlural} routes to the root route collection in routing/routes.ts.");
        }

        /// <summary>
        /// Validates the provided service parameters.
        /// </summary>
        /// <param name="parameters">The parameters to validate.</param>
        /// <returns>True if the parameters are valid; otherwise, false.</returns>
        public static bool ValidateParameters(ServiceParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.SourcePath))
            {
                Console.WriteLine("Path to /src is required.");
                return false;
            }

            if (!File.Exists(parameters.WebApiProjectFilePath))
            {
                Console.WriteLine($"Web API project not found at: {parameters.WebApiProjectFilePath}");
                return false;
            }

            if (!File.Exists(parameters.CoreProjectFilePath))
            {
                Console.WriteLine($"Core project not found at: {parameters.CoreProjectFilePath}");
                return false;
            }

            if (!Directory.Exists(parameters.ClientAppPath))
            {
                Console.WriteLine($"Angular project not found at: {parameters.ClientAppPath}");
                return false;
            }

            return true;
        }
    }
}