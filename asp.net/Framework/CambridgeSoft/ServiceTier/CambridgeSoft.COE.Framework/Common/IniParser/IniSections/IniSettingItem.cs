using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    [Serializable]
    /// <summary>
    /// Represents an individual INI setting key/value pair. This class is intended to be used
    /// during INI file parsing in order to provide a constructor for splitting the key from the
    /// value. In addition, the setting retain its memory of the INI [SECTION] it orginated from.
    /// </summary>
    public class IniSettingItem
    {
        #region >Properties<

        private string _sectionName = string.Empty;
        /// <summary>
        /// The INI [SECTION] which houses the setting
        /// </summary>
        public string SectionName
        {
            get { return this._sectionName; }
        }

        private string _settingName = string.Empty;
        /// <summary>
        /// The Key for the setting, given the context of an INI [SECTION]
        /// </summary>
        public string SettingName
        {
            get { return this._settingName; }
        }

        private string _settingRawValue = string.Empty;
        /// <summary>
        /// The setting's Value. If the value is manipulated (parsed, etc.) in any way,
        /// this field retains the original, 'raw' Value.
        /// </summary>
        public string RawValue
        {
            get { return this._settingRawValue; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sectionName">Initializes the 'SectionName'</param>
        /// <param name="settingName"></param>
        /// <param name="settingRawValue"></param>
        public IniSettingItem(string sectionName, string settingName, string settingRawValue)
        {
            this._sectionName = sectionName;
            this._settingName = settingName;
            this._settingRawValue = settingRawValue;
        }
    }
}
