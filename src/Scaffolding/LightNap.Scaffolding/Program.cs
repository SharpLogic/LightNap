using LightNap.Scaffolding.AssemblyManager;
using LightNap.Scaffolding.ProjectManager;
using LightNap.Scaffolding.ServiceRunner;
using System.CommandLine;

Argument<string> classNameArgument =
    new("className")
    {
        Description = "The name of the entity to scaffold for"
    };
Option<string> namespaceOption =
    new("--namespace")
    {
        Description = "The namespace of the entity",
        DefaultValueFactory = _ => "LightNap.Core.Data.Entities",
    };
Option<string> srcPathOption =
    new("--src-path")
    {
        Description = "The path to the /src folder of the repo",
        DefaultValueFactory = _ => "./",
    };
Option<string> coreProjectNameOption =
    new("--core-project")
    {
        Description = "The name of the core project",
        DefaultValueFactory = _ => "LightNap.Core",
    };
Option<string> webApiProjectNameOption =
    new("--web-api-project")
    {
        Description = "The name of the web API project",
        DefaultValueFactory = _ => "LightNap.WebApi",
    };
Option<string> angularProjectNameOption =
    new("--angular-project")
    {
        Description = "The name of the Angular project",
        DefaultValueFactory = _ => "lightnap-ng",
    };
Option<bool> skipComponentsOption =
    new("--skip-components")
    {
        Description = "Skip [re]generating Angular components",
        DefaultValueFactory = _ => false,
    };
Option<bool> overwriteOption =
    new("--overwrite")
    {
        Description = "Automatically overwrite files if they exist",
        DefaultValueFactory = _ => false,
    };

var rootCommand = new RootCommand()
{
    classNameArgument,
    namespaceOption,
    srcPathOption,
    coreProjectNameOption,
    webApiProjectNameOption,
    angularProjectNameOption,
    skipComponentsOption,
    overwriteOption
};

ParseResult parseResult = rootCommand.Parse(args);

ServiceRunner runner = new(new ProjectManager(), new AssemblyManager());
runner.Run(new ServiceParameters($"{parseResult.GetRequiredValue(namespaceOption)}.{parseResult.GetRequiredValue(classNameArgument)}",
    parseResult.GetRequiredValue(srcPathOption),
    parseResult.GetRequiredValue(namespaceOption), 
    parseResult.GetRequiredValue(coreProjectNameOption),
    parseResult.GetRequiredValue(webApiProjectNameOption), 
    parseResult.GetRequiredValue(angularProjectNameOption), 
    parseResult.GetRequiredValue(skipComponentsOption), 
    parseResult.GetRequiredValue(overwriteOption)));