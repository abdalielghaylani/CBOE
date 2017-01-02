using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    public class IniFormFieldUIInfo
    {
        public IniFormFieldUIInfo(string rawUIInfo)
        {
            this._rawUIInfo = rawUIInfo;

            //ParseUIInfo();
        }

        private void ParseUIInfo()
        {
            string[] uiInfoPieces = _rawUIInfo.Split(";".ToCharArray());
            if (uiInfoPieces.Length < 2)
            {
                if (string.Compare(uiInfoPieces[0], "NOT_CONFIGURABLE", true) == 0)
                {
                    IsConfigurable = false;
                }
                else
                {
                    throw new ArgumentException("The UI information for form field is invalid");
                }
            }
            else
            {
                ParseInputField(uiInfoPieces[0]);
                ParseDisplayField(uiInfoPieces[1]);
            }
        }

        private void ParseInputField(string inputField)
        {
            string[] inputFieldPieces = inputField.Split(":".ToCharArray());

            if (inputFieldPieces.Length < 2)
            {
                throw new ArgumentException("The UI input field for form field is invalid");
            }
            else
            {
                string inputFieldControlType = inputFieldPieces[0];
                IniFormFieldUIControlType = (IniFormFieldUIControlType)Enum.Parse(typeof(IniFormFieldUIControlType), inputFieldControlType, true);
            }
        }

        private void ParseDisplayField(string displayField)
        {
            string[] displayFieldPieces = displayField.Split(":".ToCharArray());
        }

        private string _rawUIInfo = string.Empty;
        public string RawUIInfo
        {
            get { return _rawUIInfo; }
            set { _rawUIInfo = value; }
        }

        private bool _isConfigurable = true;
        public bool IsConfigurable
        {
            get { return _isConfigurable; }
            set { _isConfigurable = value; }
        }

        private IniFormFieldUIControlType _iniFormFieldUIControlType = IniFormFieldUIControlType.DEFAULT;
        public IniFormFieldUIControlType IniFormFieldUIControlType
        {
            get { return _iniFormFieldUIControlType; }
            set { _iniFormFieldUIControlType = value; }
        }

        private string _validationRule = string.Empty;
        public string ValidationRule
        {
            get { return _validationRule; }
            set { _validationRule = value; }
        }
    }

    public enum IniFormFieldUIControlType
    {
        TEXT,
        TEXTAREA,
        PICKLIST,
        CHECKBOX,
        HYPERLINK,
        DEFAULT
    }
}
