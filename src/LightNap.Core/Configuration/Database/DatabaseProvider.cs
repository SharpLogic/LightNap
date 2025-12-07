namespace LightNap.Core.Configuration.Database
{
    /// <summary>
    /// Represents the database provider options.
    /// </summary>
    public enum DatabaseProvider
    {
        /// <summary>
        /// In-memory database provider.
        /// </summary>
        InMemory,
        /// <summary>
        /// SQLite database provider.
        /// </summary>
        Sqlite,
        /// <summary>
        /// SQL Server database provider.
        /// </summary>
        SqlServer
    }
}