using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Differentiate different kinds of Structure
    /// </summary>
    public enum DrawingType
    {
        /// <summary>
        /// Nomal structure drawn by user
        /// </summary>
        Chemical=0,
        /// <summary>
        /// "No Structure"
        /// </summary>
        NoStructure = 2,
        /// <summary>
        /// "Unknown structure"
        /// </summary>
        Unknown=1,
        /// <summary>
        /// "Non Chemical Content"
        /// </summary>
        NonChemicalContent=3
    }
}
