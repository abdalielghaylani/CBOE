using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;


namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    public static class COEDataViewManagerUtilities
    {
        public enum ErrNumbers
        {
            Unknown = 0,
            InvalidField = 1,
            InvalidTable = 2,
            InvalidBaseTable = 3,
            InvalidNameDescription = 4,
        }

        public static string GetErrDescription(ErrNumbers errNumber)
        {
            string retVal = String.Empty;
            switch (errNumber)
            {
                case ErrNumbers.Unknown:
                    retVal = "Unknown";
                    break;
                case ErrNumbers.InvalidBaseTable:
                    retVal = "Invalid Base Table";
                    break;
                case ErrNumbers.InvalidField:
                    retVal = "Invalid Field";
                    break;
                case ErrNumbers.InvalidNameDescription:
                    retVal = "Invalid Name & Description";
                    break;

                case ErrNumbers.InvalidTable:
                    retVal = "Invalid Table";
                    break;
            }
            return retVal;
        }
    }

    [Serializable]
    public class CustomEventArgs : EventArgs
    {
        #region Variables

        private int _newID = int.MinValue;
        private string _newAlias = String.Empty;
        private string _key = String.Empty;

        #endregion

        #region Methods

        public CustomEventArgs(int id)
        {
            _newID = id;
        }

        public CustomEventArgs(int id, string key, string alias) : this(id)
        {
            _newAlias = alias;
            _key = key;
        }

        #endregion

        #region Properties

        public int NewId
        {
            get { return this._newID; }
        }

        public string NewAlias
        {
            get { return this._newAlias; }
        }

        public string Key
        {
            get { return this._key; }
        }

        #endregion
    }

    public class CustomBrokenRule
    {
        #region Variables

        private string _detailedDescription = String.Empty;
        private COEDataViewManagerUtilities.ErrNumbers _errorNumber = COEDataViewManagerUtilities.ErrNumbers.Unknown;
        private string _errorDescription = String.Empty;

        #endregion

        #region Properties

        public string DetailedDescription
        {
            get { return _detailedDescription; }
        }

        public string ErrorDescription
        {
            get { return COEDataViewManagerUtilities.GetErrDescription(_errorNumber); }
        }

        public COEDataViewManagerUtilities.ErrNumbers ErrorNumber
        {
            get { return _errorNumber; }
        }
        
        #endregion

        #region Methods

        public static CustomBrokenRule NewRule(COEDataViewManagerUtilities.ErrNumbers errorNumber, string detailedDescription)
        {
            if (!string.IsNullOrEmpty(detailedDescription))
                return new CustomBrokenRule(errorNumber, detailedDescription);
            else
                throw new Exception("Invalid CustomBrokenRule: It must have a description");
        }

        #endregion

        #region Constructor

        private CustomBrokenRule(COEDataViewManagerUtilities.ErrNumbers errNumber, string detailedDescription)
        {
            _detailedDescription = detailedDescription;
            _errorNumber = errNumber;
        }

        #endregion
    }
}

