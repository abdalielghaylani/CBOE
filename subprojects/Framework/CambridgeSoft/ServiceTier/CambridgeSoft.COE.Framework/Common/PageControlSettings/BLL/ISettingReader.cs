using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    /// <summary>
    /// Public interface that needs to be implemented for each AppSettingReader.
    /// </summary>
    interface ISettingReader
    {

        #region Properties

        /// <summary>
        /// Priority of the reader
        /// </summary>
        int Priority
        {
            get;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Retrieves a value from the proper application setting location (text file, database, session or other).
        /// </summary>
        /// <param name="variableName">The name of the variable to be read</param>
        /// <returns>returns a string with the value of the read variable</returns>
        string getData(string variableName);
        #endregion

    }
}
