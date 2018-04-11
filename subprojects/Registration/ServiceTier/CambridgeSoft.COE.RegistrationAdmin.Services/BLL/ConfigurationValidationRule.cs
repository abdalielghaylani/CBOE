using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.RegistrationAdmin.Services.BLL
{
    public class ConfigurationValidationRule : ValidationRule
    {
        #region Variables

        private string _type;
        private string _error;
        private string _parameterName;
        private string _parameterValue;
        private string _parameter;

        #endregion

        #region Constructors

        private ConfigurationValidationRule(string Type, string Error, string ParameterName, string ParameterValue)
        {
            this.TypeValue = Type;
            this.Error = Error;
            this.Parameter = this.CreateParameter(_parameterName, _parameterValue);
        }
        

        #endregion

        #region Properties

        public string TypeValue
        {
            get
            {
                CanReadProperty(true);
                if (_type == null)
                    _type = string.Empty;
                return _type;
            }
            set
            {
                CanWriteProperty(true);
                _type = value;
                PropertyHasChanged();
            }
        }

        public string Error
        {
            get
            {
                CanReadProperty(true);
                if (_error == null)
                    _error = string.Empty;
                return _error;
            }
            set
            {
                CanWriteProperty(true);
                _error = value;
                PropertyHasChanged();
            }
        }
        public string ParameterName
        {
            get
            {
                CanReadProperty(true);
                if (_parameterName == null)
                    _parameterName = string.Empty;
                return _parameterName;
            }
            set
            {
                CanWriteProperty(true);
                _parameterName = value;
                PropertyHasChanged();
            }
        }

        public string ParameterValue
        {
            get
            {
                CanReadProperty(true);
                if (_parameterValue == null)
                    _parameterValue = string.Empty;
                return _parameterValue;
            }
            set
            {
                CanWriteProperty(true);
                _parameterValue = value;
                PropertyHasChanged();
            }
        }
        public string Parameter
        {
            get 
            {
                CanReadProperty(true);
                return CreateParameter(_parameterValue, _parameterName);
            }            
        }

        #endregion
        
        #region Factory Methods

        public static ConfigurationValidationRule NewCofigurationValidationRule(string Type, string Error, string ParameterName, string ParameterValue)
        {
            return new ConfigurationValidationRule(Type, Error, ParameterName, ParameterValue);

        }
        #endregion

        #region Methods

        private string CreateParameter(string ParameterName, string ParameterValue)
        {
            string param = "name=" + ParameterName + " - " + "value = " + ParameterValue;
            return param;
        }

        #endregion

    }
}
