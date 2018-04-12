using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Defines the set of supported file types which the FileParserFactory can instantiate.
    /// </summary>
    public enum SourceFileType
    {
        /// <summary>
        /// placeholder for a required value
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// classic SDFile format
        /// </summary>
        SDFile = 1,
        /// <summary>
        /// character-separated values (involving a delimiter and possibly quoted fields)
        /// </summary>
        CSV = 2,
        /// <summary>
        /// any version of Excel file where a worksheet can be read with an OleDbReader
        /// </summary>
        MSExcel = 3,
        /// <summary>
        /// any version of Access file where a table or view can be read with an OleDbReader
        /// </summary>
        MSAccess = 4,
        /// <summary>
        /// ChemFinder.Net database files, version 12 or later
        /// </summary>
        ChemFinder = 5,
    }
}
