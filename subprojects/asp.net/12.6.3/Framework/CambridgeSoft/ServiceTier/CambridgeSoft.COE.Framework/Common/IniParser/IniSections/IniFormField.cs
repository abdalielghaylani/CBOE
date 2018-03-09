using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    [Serializable]
    /// <summary>
    /// Instances of this class are intended to be derived from INI files where individual
    /// elements of interest may have properties spread across multiple INI settings. In this
    /// case, the 'element of interest' is a field of data in the CambridgeSoft COE v.10 system.
    /// </summary>
    public class IniFormField : IniRegistrationSettingItem, IComparable<IniFormField>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IniFormField(string sectionName, string settingName, string settingRawValue)
            :base(sectionName,settingName,settingRawValue)
        {
            _fieldkey = settingName;
            IniSettingType = IniSettingType.FormField;
        }

        #region >Properties<

        private bool _isVisible = true;
        /// <summary>
        /// Determines in the associated field will be rendered in the GUI
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        private string _fieldkey = string.Empty;
        /// <summary>
        /// The official name of the field, generally corresponding to the database field from which
        /// its data is derived.
        /// </summary>
        public string FieldKey
        {
            get { return _fieldkey; }
            set { _fieldkey = value; }
        }

        private string _fieldLabel = string.Empty;
        /// <summary>
        /// The caption, or label, ofthe form field when itis rendered in the GUI.
        /// </summary>
        public string FieldLabel
        {
            get { return _fieldLabel; }
            set { _fieldLabel = value; }
        }

        private bool _isRequired = false;
        /// <summary>
        /// Indidates whether or not this form field is a mandatory one on UI
        /// </summary>
        public bool IsRequired
        {
            get { return _isRequired; }
            set { _isRequired = value; }
        }

        private bool _isSkipped = true;
        /// <summary>
        /// Indicates whether this form field should be skipped. If all properties between the INI and 
        /// the existing XML configuration have the same values, this form field should be skipped.
        /// </summary>
        public bool IsSkipped
        {
            get { return _isSkipped; }
            set { _isSkipped = value; }
        }

        private IniFormFieldUIInfo _fieldUIInfo = null;
        /// <summary>
        /// The raw 
        /// </summary>
        public IniFormFieldUIInfo FieldUIInfo
        {
            get { return _fieldUIInfo; }
            set { _fieldUIInfo = value; }
        }

        #endregion

        #region IComparable<IniFormField> Members

        public int CompareTo(IniFormField otherField)
        {
            return this.FieldKey.CompareTo(otherField.FieldKey);
        }

        #endregion
    }
}
