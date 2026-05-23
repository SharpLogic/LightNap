namespace LightNap.MaintenanceService
{
    /// <summary>
    /// Represents a maintenance task that can be run.
    /// </summary>
    internal interface IMaintenanceTask
    {
        /// <summary>
        /// Gets the name of the maintenance task.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Runs the maintenance task asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RunAsync();
    }
}
