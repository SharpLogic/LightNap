using System.Diagnostics.CodeAnalysis;

namespace LightNap.Scaffolding.TemplateManager
{
    /// <summary>
    /// Represents a template item with a template file, output file, and an optional project path.
    /// </summary>
    [method: SetsRequiredMembers]
    public class TemplateItem(string templateFile, string outputFile)
    {
        /// <summary>
        /// Gets or sets the template file path.
        /// </summary>
        public required string TemplateFile { get; set; } = templateFile;

        /// <summary>
        /// Gets or sets the output file path.
        /// </summary>
        public required string OutputFile { get; set; } = outputFile;

    }
}
