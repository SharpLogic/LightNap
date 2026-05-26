namespace LightNap.Core.Interfaces
{
    /// <summary>
    /// Discriminates the kind of actor represented by an <see cref="IUserContext"/>.
    /// </summary>
    public enum UserContextKind
    {
        /// <summary>
        /// No identifiable actor. <see cref="IUserContext.GetActorId"/> throws.
        /// </summary>
        Anonymous,

        /// <summary>
        /// An anonymous visitor tracked by a stable per-browser identifier (e.g., a first-party cookie).
        /// <see cref="IUserContext.GetActorId"/> returns the visitor identifier.
        /// </summary>
        AnonymousVisitor,

        /// <summary>
        /// An authenticated user with an ASP.NET Identity record. <see cref="IUserContext.GetActorId"/>
        /// returns the user ID.
        /// </summary>
        Authenticated,

        /// <summary>
        /// A system context used by background jobs, seeders, and maintenance tasks.
        /// <see cref="IUserContext.GetActorId"/> returns a stable "system" sentinel.
        /// </summary>
        System
    }
}
