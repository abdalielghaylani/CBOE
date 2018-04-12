using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Describes the set of actions that can be performed from the command-line.
    /// </summary>
    public enum TargetActionType
    {
        /// <summary>
        /// directs the record-count task
        /// </summary>
        CountRecords = 1,
        /// <summary>
        /// directs the table-list task
        /// </summary>
        ListTables = 2,
        /// <summary>
        /// directs the field-definitions task
        /// </summary>
        ListFields = 3,
        /// <summary>
        /// splits the source file into several ones
        /// </summary>
        SplitFile = 4,
        /// <summary>
        /// validates whether the mapping file is mappable, providing the data file
        /// </summary>
        ValidateMapping = 5,
        /// <summary>
        /// directs the data-mapping and validation task
        /// </summary>
        ValidateData = 6,
        /// <summary>
        /// directs the duplicate-check task
        /// </summary>
        FindDuplicates = 7,
        /// <summary>
        /// imports the source file to backend storage, which typically is database, as 'temp'
        /// </summary>
        ImportTemp = 8,
        /// <summary>
        /// imports the source file to backend storage, which typically is database, as 'perm'
        /// ignoring duplicates
        /// </summary>
        ImportRegDupAsCreateNew = 9,
        /// <summary>
        /// imports the source file to backend storage, which typically is database, as 'perm'
        /// but adding a new batch to the first existing duplicate found
        /// </summary>
        ImportRegDupAsNewBatch = 10,
        /// <summary>
        /// imports the duplicated records to 'temp', even if the original attempt was
        /// to import to 'perm'
        /// </summary>
        ImportRegDupAsTemp = 11,
        /// <summary>
        /// attempts to save source file records to the repository, but will skip the import
        /// of the particular record if any duplicate is found
        /// </summary>
        ImportRegDupNone = 12,
        /// <summary>
        /// default dummy member
        /// </summary>
        Unknown = 99
    }
}
