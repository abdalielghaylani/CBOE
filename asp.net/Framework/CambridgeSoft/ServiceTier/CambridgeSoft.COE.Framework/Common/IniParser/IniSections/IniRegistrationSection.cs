using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    [Serializable]
    public class IniRegistrationSection : IniSection
    {
        private const string FIELD_LABELS_PREFIX = "FIELD_LABELS";
        private const string FIELD_DISPLAY_PREFIX = "DISPLAY_TYPES";
        private const string HIDDEN_FIELDS_PREFIX = "GUI_FIELDS_TO_HIDE";
        private const string DERIVED_FIELDS_PREFIX = "DERIVED_FIELDS";

        private SortedList<string, IniFormField> _formFields = null;
        private IDictionary<string, IniRegistrationSettingItem> _appSettingItems = null;

        public IDictionary<string, IniRegistrationSettingItem> AppSettingItems
        {
            get { return _appSettingItems; }
        }

        public IniRegistrationSection(string sectionName, IList<string> rawLines)
            : base(sectionName, rawLines)
        { }

        /// <summary>
        /// Parses the Registration INI section in an object-oriented way
        /// </summary>
        internal override void Parse()
        {
            base.Parse();

            _appSettingItems = new Dictionary<string, IniRegistrationSettingItem>();
            ParseAppSettings();

            if (SectionName == IniSection.REGISTRATION_SECTION_NAME)
            {
                _formFields = new SortedList<string, IniFormField>();
                ParseFormFields();
                ParseDisplayType();
                ParseHiddenFields();
                ParseDerivedFields();
            }
        }

        /// <summary>
        /// The collection of all form fields parsed from INI settings.
        /// </summary>
        public SortedList<string, IniFormField> FormFields
        {
            get { return _formFields; }
        }

        public string GenerateFormFieldsFormatString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Field Name\tField Label\tDisplay Info\tVisibility");

            foreach (IniFormField formField in FormFields.Values)
            {
                sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}",
                    formField.FieldKey,
                    formField.FieldLabel,
                    formField.FieldUIInfo.RawUIInfo,
                    formField.IsVisible));
            }

            return sb.ToString();
        }

        public string GenerateAppSettingsFormatString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Section Name\tSetting Name\tSetting Value");

            foreach (IniRegistrationSettingItem registrationSettingItem in AppSettingItems.Values)
            {
                sb.AppendLine(string.Format("{0}\t{1}\t{2}",
                            SectionName,
                            registrationSettingItem.SettingName,
                            registrationSettingItem.RawValue));
            }

            return sb.ToString();
        }

        private void ParseAppSettings()
        {
            foreach (string key in _iniSettings.Keys)
            {
                if (!key.StartsWith(FIELD_LABELS_PREFIX) &&
                    !key.StartsWith(FIELD_DISPLAY_PREFIX) &&
                    !key.StartsWith(HIDDEN_FIELDS_PREFIX) &&
                    !key.StartsWith(DERIVED_FIELDS_PREFIX))
                {
                    IniRegistrationSettingItem registrationSettingItem =
                        new IniRegistrationSettingItem(SectionName, key, _iniSettings[key].RawValue);
                    _appSettingItems.Add(key, registrationSettingItem);
                }
            }
        }

        /// <summary>
        /// Gets the name and display label of all form fields.
        /// </summary>
        private void ParseFormFields()
        {
            foreach (KeyValuePair<string, IniSettingItem> iniSettingPair in this._iniSettings)
            {
                //filter by the prefix that descibes field labels
                if (iniSettingPair.Value.SettingName.StartsWith(FIELD_LABELS_PREFIX))
                {
                    string val = iniSettingPair.Value.RawValue;
                    int colonPos = val.IndexOf(":");
                    if (colonPos > -1)
                    {
                        string fldKey = val.Substring(0, colonPos).Trim().ToUpper();
                        string fldLabel = val.Substring(colonPos + 1);
                        IniFormField fld = new IniFormField(SectionName, fldKey, val);
                        fld.FieldKey = fldKey;
                        fld.FieldLabel = fldLabel;
                        // the visibility is determined by settings defined in other parts
                        _formFields.Add(fldKey, fld);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the UI control display information of all form fields
        /// </summary>
        private void ParseDisplayType()
        {
            foreach (KeyValuePair<string, IniSettingItem> iniSettingPair in this._iniSettings)
            {
                //filter by the prefix that describes accessory field information
                if (iniSettingPair.Value.SettingName.StartsWith(FIELD_DISPLAY_PREFIX))
                {
                    string val = iniSettingPair.Value.RawValue;
                    int colonPos = val.IndexOf(";");
                    if (colonPos > -1)
                    {
                        string fldKey = val.Substring(0, colonPos).Trim().ToUpper();
                        string fldValue = val.Substring(colonPos + 1);

                        if (_formFields.ContainsKey(fldKey))
                        {
                            IniFormField field = _formFields[fldKey];
                            field.FieldUIInfo = new IniFormFieldUIInfo(fldValue);
                            field.IsVisible = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates field visibilities based on the list of hidden fields.
        /// </summary>
        private void ParseHiddenFields()
        {
            foreach (KeyValuePair<string, IniSettingItem> iniSettingPair in this._iniSettings)
            {
                //filter by the prefix that describes delimited lists of non-visible fields
                if (iniSettingPair.Value.SettingName.StartsWith(HIDDEN_FIELDS_PREFIX))
                {
                    string[] vals = iniSettingPair.Value.RawValue.ToUpper().Split(",".ToCharArray());
                    foreach (string val in vals)
                    {
                        if (_formFields.ContainsKey(val.Trim()))
                            _formFields[val].IsVisible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Updates field visibilities based on the list of derived fields.
        /// </summary>
        private void ParseDerivedFields()
        {
            foreach (KeyValuePair<string, IniSettingItem> iniSettingPair in this._iniSettings)
            {
                //filter by the prefix that describes delimited lists of derived fields
                if (iniSettingPair.Value.SettingName.StartsWith(DERIVED_FIELDS_PREFIX))
                {
                    string[] vals = iniSettingPair.Value.RawValue.Split(",".ToCharArray());
                    foreach (string val in vals)
                    {
                        if (_formFields.ContainsKey(val.Trim()))
                            _formFields[val].IsVisible = false;
                    }
                }
            }
        }

    }
}
