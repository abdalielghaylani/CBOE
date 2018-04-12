using System;
using System.Collections.Generic;
using System.IO;

namespace CambridgeSoft.COE.DataLoader.Core.Contracts
{
    /// <summary>
    /// This contract describes a logical record from any 'flat' data source, including
    /// the field names and values, as well as the record's index in the original source file.
    /// </summary>
    public interface ISourceRecord
    {
        /// <summary>
        /// The 1-based index of this record from the beginning of its original source.
        /// </summary>
        int SourceIndex { get; }

        FieldValues FieldSet { get; }
    }
}
