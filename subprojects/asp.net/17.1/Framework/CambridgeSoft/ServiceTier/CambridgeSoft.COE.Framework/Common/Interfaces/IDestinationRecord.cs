using System.Collections.Generic;
using System;

namespace CambridgeSoft.COE.Framework.Services
{
    /// <summary>
    /// Defines contracts for all types of destination object.
    /// </summary>
    public interface IDestinationRecord : IDisposable
    {
        /// <summary>
        /// Determines whether or not this record is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// return the broken rules description when destination record is invalid
        /// </summary>
        string ValidationErrorMessage { get;}

        /// <summary>
        /// Uploads this record to its storage.
        /// </summary>
        /// <returns>A boolean value indicating whether the import action succeeds.</returns>
        bool Import();

    }
}
