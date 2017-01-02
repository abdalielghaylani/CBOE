using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    /// <summary>
    /// Represents an INI setting item specific to Registration use.
    /// </summary>
    public class IniRegistrationSettingItem : IniSettingItem
    {
        public IniRegistrationSettingItem(string sectionName, string settingName, string settingRawValue)
            : base(sectionName, settingName, settingRawValue)
        { }

        private bool _isActive = false;
        /// <summary>
        /// Indicates whether this setting should be used by the caller.
        /// <para>
        /// In the case of settings migration from classic INI to more modern configuration
        /// options, this property should be used to determine when the setting has been
        /// deprecated and can be ignored by any migration processes.
        /// </para>
        /// </summary>
        public bool IsActive
        {
            get { return this._isActive; }
            set { this._isActive = value; }
        }

        private IniSettingType _iniSettingType = IniSettingType.AppSetting;
        public IniSettingType IniSettingType
        {
            get { return this._iniSettingType; }
            protected set { this._iniSettingType = value; }
        }
    }

    /// <summary>
    /// Specifies whether an Ini setting is app setting or a form field setting
    /// </summary>
    public enum IniSettingType
    {
        AppSetting,
        FormField
    }
}
