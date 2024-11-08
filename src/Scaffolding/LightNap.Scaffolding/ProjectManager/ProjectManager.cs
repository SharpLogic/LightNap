using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging;

namespace LightNap.Scaffolding.ProjectManager
{
    /// <summary>
    /// Manages project operations such as building and adding files to the project.
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManager"/> class.
        /// Registers the default MSBuild instance.
        /// </summary>
        public ProjectManager()
        {
            MSBuildLocator.RegisterDefaults();
        }

        /// <summary>
        /// Builds the project at the specified path.
        /// </summary>
        /// <param name="projectPath">The path to the project file.</param>
        /// <returns>A <see cref="ProjectBuildResult"/> indicating the result of the build.</returns>
        public ProjectBuildResult BuildProject(string projectPath)
        {
            Console.WriteLine($"Attempting to build project at: {projectPath}");

            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(projectPath);

            var buildParameters = new BuildParameters(projectCollection)
            {
                Loggers = [new ConsoleLogger(LoggerVerbosity.Quiet)]
            };

            var buildRequest = new BuildRequestData(project.CreateProjectInstance(), ["Build"]);
            var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

            if (buildResult.OverallResult != BuildResultCode.Success)
            {
                foreach (var item in buildResult.ResultsByTarget)
                {
                    Console.WriteLine($"{item.Key}: {item.Value.ResultCode}");
                }
                return new ProjectBuildResult() { Success = false };
            }

            var outputPath = project.GetPropertyValue("OutputPath");
            var outputFileName = project.GetPropertyValue("AssemblyName") + ".dll";
            return new ProjectBuildResult()
            {
                OutputAssemblyPath = Path.Combine(project.DirectoryPath, outputPath, outputFileName),
                Success = true
            };
        }
    }
}
