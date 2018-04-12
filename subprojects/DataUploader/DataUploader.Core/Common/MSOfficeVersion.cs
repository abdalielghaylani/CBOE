using System;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Provides version options for the constructor of the ExcelOleDbWrapper class.
    /// </summary>
    /// <remarks>
    /// The caller should provide the version of the file in order to use the correct provider.
    /// </remarks>
    public enum MSOfficeVersion
    {
        /// <summary>
        /// When using this option, expect to have an alternate mechanism for determining
        /// the actual value, such as the file extension or a user-interactive model.
        /// </summary>
        Unknown,
        /// <summary>
        /// MS Office '97
        /// </summary>
        Office97 = 8,
        /// <summary>
        /// MS Office 2000
        /// </summary>
        Office2000 = 9,
        /// <summary>
        /// MS Office XP, came out in 2002
        /// </summary>
        OfficeXp2002 = 10,
        /// <summary>
        /// MS Office 2003
        /// </summary>
        Office2003 = 11,
        /// <summary>
        /// MS Office 2007, came out about the same time as Vista
        /// </summary>
        Offcie2007 = 12
    }
}
