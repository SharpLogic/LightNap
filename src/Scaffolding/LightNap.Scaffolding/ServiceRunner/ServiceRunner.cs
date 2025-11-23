using LightNap.Scaffolding.AssemblyManager;
using LightNap.Scaffolding.ProjectManager;
using LightNap.Scaffolding.TemplateManager;
using LightNap.Scaffolding.Templates;

namespace LightNap.Scaffolding.ServiceRunner
{
    /// <summary>
    /// Runs the service scaffolding process.
    /// </summary>
    public class ServiceRunner(IProjectManager projectManager, IAssemblyManager assemblyManager)
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

            if (!projectManager.CanBuild)
            {
                // Detecting the major version is a bit hacky, so just need to remember to notch this up when the major version changes.
                int majorVersion = 9;
                ServiceRunner.PrintError($"The .NET {majorVersion} SDK must be installed to run the scaffolder. It has to be the version the scaffolder was built to target (in project settings) or else it can't build LightNap.Core.");
                return;
            }

            var projectBuildResult = projectManager.BuildProject(parameters.CoreProjectFilePath);
            if (!projectBuildResult.Success)
            {
                ServiceRunner.PrintError($"{parameters.CoreProjectName} build failed. Please fix the errors and try again.");
                return;
            }

            var type = assemblyManager.LoadType(projectBuildResult.OutputAssemblyPath!, parameters.ClassName);
            if (type == null)
            {
                ServiceRunner.PrintError($"Type '{parameters.ClassName}' not found or could not be loaded from assembly.");
                return;
            }

            Console.WriteLine($"Analyzing {type.Name} ({type.FullName})");

            List<TypePropertyDetails> propertiesDetails = TypeHelper.GetPropertyDetails(type);
            if (!propertiesDetails.Any())
            {
                ServiceRunner.PrintError($"No usable properties found in type '{type.Name}'");
                return;
            }

            Console.WriteLine($"{"Name",-30}{"Back-End",-20}{"Front-End",-10}{"Required?",-10}{"Nullable?",-10}{"Get?",-6}{"Set?",-6}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var property in propertiesDetails)
            {
                Console.WriteLine($"{property.Name,-30}{property.BackEndType,-20}{property.FrontEndType,-10}{property.IsRequired,-10}{property.IsNullable,-10}{property.CanGet,-6}{property.CanSet,-6}");
            }
            Console.ResetColor();

            TemplateParameters templateParameters = new(type.Name, propertiesDetails, parameters);

            string pascalNamePlural = templateParameters.PascalNamePlural;
            string kebabName = templateParameters.KebabName;
            string kebabNamePlural = templateParameters.KebabNamePlural;

            string executingPath = assemblyManager.GetExecutingPath();

            var templateItems = new List<TemplateItem>
                {
                    new(new CreateDto() { Parameters = templateParameters }, $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Request/Create{type.Name}Dto.cs"),
                    new(new Dto() { Parameters = templateParameters }, $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Response/{type.Name}Dto.cs"),
                    new(new Extensions() { Parameters = templateParameters },$"{parameters.CoreProjectPath}/Extensions/{type.Name}Extensions.cs"),
                    new(new Interface() { Parameters = templateParameters }, $"{parameters.CoreProjectPath}/{pascalNamePlural}/Interfaces/I{type.Name}Service.cs"),
                    new(new SearchDto() { Parameters = templateParameters }, $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Request/Search{pascalNamePlural}Dto.cs"),
                    new(new Service() { Parameters = templateParameters }, $"{parameters.CoreProjectPath}/{pascalNamePlural}/Services/{type.Name}Service.cs"),
                    new(new UpdateDto() { Parameters = templateParameters }, $"{parameters.CoreProjectPath}/{pascalNamePlural}/Dto/Request/Update{type.Name}Dto.cs"),
                    new(new Controller() { Parameters = templateParameters }, $"{parameters.WebApiProjectPath}/Controllers/{pascalNamePlural}Controller.cs"),

                    new(new CreateRequestDto() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/models/request/create-{kebabName}-request-dto.ts"),
                    new(new SearchRequestDto() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/models/request/search-{kebabNamePlural}-request-dto.ts"),
                    new(new UpdateRequestDto() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/models/request/update-{kebabName}-request-dto.ts"),
                    new(new ResponseDto() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/models/response/{kebabName}-dto.ts"),
                    new(new DataService() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/services/data.service.ts"),
                    new(new AreaService() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/services/{kebabName}.service.ts"),
                };

            if (!parameters.SkipComponents)
            {
                templateItems.AddRange([
                    new(new Routes() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/routes.ts"),
                    new(new IndexHtml() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/index/index.component.html"),
                    new(new IndexCode() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/index/index.component.ts"),
                    new(new GetHtml() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/get/get.component.html"),
                    new(new GetCode() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/get/get.component.ts"),
                    new(new CreateHtml() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/create/create.component.html"),
                    new(new CreateCode() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/create/create.component.ts"),
                    new(new EditHtml() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/edit/edit.component.html"),
                    new(new EditCode() { Parameters = templateParameters }, $"{parameters.FrontEndAppPath}/{kebabNamePlural}/components/pages/edit/edit.component.ts"),
                    ]);
            }

            if (!parameters.Overwrite)
            {
                foreach (var template in templateItems)
                {
                    if (File.Exists(Path.Combine(parameters.SourcePath, template.OutputFile)))
                    {
                        ServiceRunner.PrintError($"Bailing out: File '{Path.GetRelativePath(parameters.SourcePath, template.OutputFile)}' already exists!");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Use the '--overwrite true' switch to overwrite files that already exist");
                        Console.ResetColor();
                        return;
                    }
                }
            }

            int newFiles = 0;
            int overwrittenFiles = 0;
            foreach (var template in templateItems)
            {
                string generatedCode = template.Template.TransformText();
                if (File.Exists(template.OutputFile))
                {
                    // Ignore files that haven't changed.
                    if (File.ReadAllText(template.OutputFile) == generatedCode) { continue; }
                    overwrittenFiles++;
                }
                else
                {
                    newFiles++;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(template.OutputFile)!);
                File.WriteAllText(template.OutputFile, generatedCode);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Generated '{Path.GetRelativePath(parameters.SourcePath, template.OutputFile)}'");
                Console.ResetColor();
            }

            if (newFiles + overwrittenFiles > 0)
            {
                Console.WriteLine(@$"
Scaffolding completed successfully. {newFiles} new files generated, {overwrittenFiles} files overwritten.

Please see TODO comments in generated code to complete integration.

    {parameters.CoreProjectName}:
    - Update front-end and back-end DTO properties in {pascalNamePlural}/Dto to only those you want included.
    - Update extension method mappers between DTOs and the entity in Extensions/{type.Name}Extensions.cs.

    {parameters.WebApiProjectName}:
    - Update the authorization for methods in Controllers/{pascalNamePlural}Controller.cs based on access preferences.
    - Register Web API controller parameter dependency in Extensions/ApplicationServiceExtensions.cs.

    {parameters.AngularProjectName}:
    - Update the models in {kebabNamePlural}/models to match the updated back-end DTOs.
    - Update authorization for the routes in {kebabNamePlural}/components/pages/routes.ts.
    - Add {kebabNamePlural} routes to the root route collection in routing/routes.ts.");
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No changes detected, so no files were added or changed.");
                Console.ResetColor();
            }
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

            if (!Directory.Exists(parameters.FrontEndAppPath))
            {
                ServiceRunner.PrintError($"Angular project not found at: {parameters.FrontEndAppPath}");
                return false;
            }

            return true;
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}