namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Used to determine or set the RLS status of a Registration's lifecycle.
    /// </summary>
    public enum RLSStatus
    {
        /// <summary>
        /// Row-level security is disabled.
        /// </summary>
        Off,
        /// <summary>
        /// Row-level security is based on project association at the registry level.
        /// </summary>
        RegistryLevelProjects,
        /// <summary>
        /// Row-level security is based on project association at the batch level
        /// </summary>
        BatchLevelProjects
    }
}