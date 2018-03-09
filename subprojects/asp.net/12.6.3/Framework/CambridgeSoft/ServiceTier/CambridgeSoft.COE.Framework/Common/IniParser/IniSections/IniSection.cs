using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    /// <summary>
    /// The object-oriented representation of an old INI section.
    /// </summary>
    [Serializable]
    public abstract class IniSection
    {
        public const string GLOBALS_SECTION_NAME = "GLOBALS";
        public const string DUPLICATE_CHECKING_SECTION_NAME = "DUPLICATE_CHECKING";
        public const string CS_SECURITY_SECTION_NAME = "CS_SECURITY";
        public const string REGISTRATION_SECTION_NAME = "REGISTRATION";
        public const string DOC_MANAGER_SECTION_NAME = "DOCMANAGER";
        private const string SETTING_DELIMITER = "=";
        public const string DUPLICATE_SETTINGS_MESSAGE = "Settings '{0}' has {1} duplicates found under Section '{2}'";

        protected string _sectionName = string.Empty;
        protected IList<string> _rawLines = null;
        protected Dictionary<string, IniSettingItem> _iniSettings = null;
        protected SortedList<string, IniSettingItem> _iniAppSettingItems = null;
        protected IList<string> _validationErrorMessages = null;

        internal IniSection(string sectionName, IList<string> rawLines)
        {
            _sectionName = sectionName;
            _rawLines = rawLines;
            _validationErrorMessages = new List<string>();
            _iniSettings = new Dictionary<string, IniSettingItem>();
        }

        /// <summary>
        /// The name of the Ini section
        /// </summary>
        public string SectionName
        {
            get { return _sectionName; }
        }

        public IList<string> RawLines
        {
            get { return _rawLines; }
        }

        internal IList<string> ValidationErrorMessage
        {
            get { return _validationErrorMessages; }
        }

        /// <summary>
        /// Applies any section-specific parsing logics.
        /// </summary>
        internal virtual void Parse()
        {
            _iniSettings.Clear();

            string line = string.Empty;
            string key = string.Empty;
            string value = string.Empty;
            IniSettingItem iniSettingItem = null;

            // Skip the first line, which is the section name line
            for (int i = 1; i < _rawLines.Count; i++)
            {
                if ((line = _rawLines[i]) != null)
                {
                    int indexOfDelimiter = line.IndexOf(SETTING_DELIMITER);
                    if (indexOfDelimiter != -1)
                    {
                        key = line.Substring(0, indexOfDelimiter).Trim();
                        value = line.Substring(indexOfDelimiter + 1).Trim();

                        iniSettingItem = new IniSettingItem(_sectionName, key, value);
                        _iniSettings.Add(key, iniSettingItem);
                    }
                }
            }
        }

        /// <summary>
        /// The app settings collection parsed out from the Ini content
        /// </summary>
        [Obsolete("Obsolete, due to the understanding that this class should be generic and Registration-agnostic. Use IniRegistrationSection if needed.")]
        public virtual SortedList<string, IniSettingItem> AppSettingItems
        {
            // TODO: Update all references to this property to have them use IniRegistrationSection instead.
            get { return null; }
        }

        /// <summary>
        /// All the INI setting items contained inside this section.
        /// </summary>
        public IDictionary<string, IniSettingItem> IniSettingItems
        {
            get { return _iniSettings; }
        }

        internal static IniSection CreateIniSection(string sectionName, IList<string> rawLines)
        {
            switch (sectionName.ToUpper())
            {
                case IniSection.REGISTRATION_SECTION_NAME: 
                case IniSection.DUPLICATE_CHECKING_SECTION_NAME:
                    return new IniRegistrationSection(sectionName, rawLines);
                default:
                    return new IniRegularSection(sectionName, rawLines);
            }
        }

        internal void Validate()
        {
            _validationErrorMessages.Clear();

            Dictionary<string, int> settingNames = ExtractSettingNames();
            foreach (KeyValuePair<string, int> item in settingNames)
            {
                if (item.Value > 1)
                {
                    _validationErrorMessages.Add(string.Format(DUPLICATE_SETTINGS_MESSAGE, item.Key, item.Value, SectionName));
                }
            }
        }

        private Dictionary<string, int> ExtractSettingNames()
        {
            Dictionary<string, int> SettingNames = new Dictionary<string, int>();

            string line = string.Empty;
            string key = string.Empty;

            // Skip the first line, which is the section name line
            for (int i = 1; i < _rawLines.Count; i++)
            {
                if ((line = _rawLines[i]) != null)
                {
                    int indexOfDelimiter = line.IndexOf(SETTING_DELIMITER);
                    if (indexOfDelimiter != -1)
                    {
                        key = line.Substring(0, indexOfDelimiter).Trim();

                        if (!SettingNames.ContainsKey(key))
                        {
                            SettingNames.Add(key, 1);
                        }
                        else
                        {
                            SettingNames[key]++;
                        }
                    }
                }
            }

            return SettingNames;
        }
    }
}

