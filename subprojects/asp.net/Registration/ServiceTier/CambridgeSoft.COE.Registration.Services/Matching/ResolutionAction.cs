using System.Collections.Generic;
using System.Text;

namespace RegistrationWebApp.Code
{
    /// <summary>
    /// The possible list of match-resolution mechanisms. These should be considered instructional
    /// for either user-interactive resolution or an automated process.
    /// </summary>
    public enum ResolutionAction
    {
        /// <summary>
        /// No action is specified.
        /// </summary>
        UnspecifiedAction = 0,
        /// <summary>
        /// Add a new batch to the target single-compound registration.
        /// </summary>
        AddCompoundBatch = 1,
        /// <summary>
        /// Create a new single-compound registration.
        /// </summary>
        DuplicateCompound = 2,
        /// <summary>
        /// Use the Structure entity from the target single-compound registration.
        /// </summary>
        UseStructure = 3,
        /// <summary>
        /// Add a new batch to the target multi-compound registration.
        /// </summary>
        AddMixturebatch = 4,
        /// <summary>
        /// Create a new 
        /// </summary>
        DuplicateMixture = 5,
        /// <summary>
        /// 
        /// </summary>
        UseComponent = 6,
    }
}
