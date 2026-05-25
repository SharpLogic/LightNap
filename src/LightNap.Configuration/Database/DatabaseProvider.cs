namespace LightNap.Configuration.Database
{
    /// <summary>
    /// Represents the database provider options.
    /// </summary>
    public enum DatabaseProvider
    {
        /// <summary>
        /// No database provider has been configured. This is the default value used to detect missing configuration.
        /// </summary>
        Unconfigured,
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
