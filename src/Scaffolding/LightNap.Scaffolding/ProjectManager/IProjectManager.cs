namespace LightNap.Scaffolding.ProjectManager
{
    /// <summary>
    /// Interface for managing project operations.
    /// </summary>
    public interface IProjectManager
    {
        /// <summary>
        /// Builds the project at the specified path.
        /// </summary>
        /// <param name="projectPath">The path to the project to build.</param>
        /// <returns>A <see cref="ProjectBuildResult"/> containing the result of the build operation.</returns>
        ProjectBuildResult BuildProject(string projectPath);
    }
}
