namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Used to determine or set the 'approval' status of a Registration's lifecycle.
    /// NOTE: This enumeration is subject to change as it appears to be a mish-mash of
    /// the Registry-level 'approval' and Batch-level 'status' concepts.
    /// </summary>
    public enum RegistryStatus
    {
        /// <summary>
        /// This status is an indicator that the record has not been added to either the 'temporary'
        /// or 'permanent' repositories.
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// Records submitted to the 'temporary' repository have this status.
        /// </summary>
        Submitted = 1,
        /// <summary>
        /// Use this status when Sumitted records have been reviewed and are ready for consideration
        /// by the Registrar. For systems not configured to use the apporvals workflow, this status
        /// should be bypassed.
        /// </summary>
        Approved = 2,
        /// <summary>
        /// This is the final status applied by the act of Registration.
        /// </summary>
        Registered = 3,
        /// <summary>
        /// A registered record might be Locked to disallow edition, but allowing the addition of batches and identifiers.
        /// </summary>
        Locked = 4
    }
}