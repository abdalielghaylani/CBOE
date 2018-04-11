using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Csla;

using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.BLL;
using CambridgeSoft.COE.Registration.Services.Types;

using CambridgeSoft.COE.RegistrationAdmin.Services.Common;
using Csla.Validation;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class ConfigurationProperty : Property
    {
        #region Properties

        public override string Precision
        {
            get
            {
                CanReadProperty(true);
                return _precision;
            }
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _precision = value;
                    PrecisionIsUpdate = true;
                    PropertyHasChanged();
                }
            }
        }

        public ValidationRuleList ValidationRuleList
        {
            get
            {
                CanReadProperty(true);
                return _validationRuleList;
            }
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _validationRuleList = value;
                    PropertyHasChanged();
                }
            }
        }

        public override string Name
        {
            get
            {
                return base.Name.ToUpper();
            }
            set
            {
                base.Name = value;
            }
        }

        public override string Type
        {
            get
            {
                return base.Type.ToUpper();
            }
            set
            {
                base.Type = value;
            }
        }

        public override bool IsDirty
        {
            get
            {
                return this.SortOrderIsUpdate || base.IsDirty;
            }
        }

        public override string SubType
        {
            get
            {
                return base.SubType.ToUpper();
            }
            set
            {
                base.SubType = value;
            }
        }

        #endregion

        #region Factory Methods

        private ConfigurationProperty(string name,string friendlyName,string type, string precision, bool isNew)
        {

            _name = name.Trim();
            _type = type;
            _validationRuleList = ValidationRuleList.NewValidationRuleList();
            _friendlyName = friendlyName;
            _precision = precision;
            if (isNew)
            {
                _sortOrder = -1;
                AddDefaultRule();
            }
            if (type == "NUMBER" && isNew)
            {
                _precision = RegAdminUtils.ConvertPrecision(precision,true);
            }
            else
            {
                _precision = precision;
            }
        }

        private ConfigurationProperty(string name, string friendlyName, string type, string precision, bool isNew, string subType,string pickListDomainId)
            : this(name, friendlyName, type, precision, isNew)
        {
            _pickListDomainId = pickListDomainId;
            _subType = subType;
        }

        private ConfigurationProperty(Property property, bool isClean, bool isNew)
            : this(property.Name,property.FriendlyName,property.Type, property.Precision, isNew)
        {
            if (property.ValRuleList != null)
            {
                ValidationRuleList = property.ValRuleList;
            }
            if (!string.IsNullOrEmpty(property.SubType))
                SubType = property.SubType;
            if (isClean)
                MarkClean();
            else
                MarkDirty();
            if (isNew)
            {
                _sortOrder = -1;
                MarkNew();
            }
            else
            {
                _sortOrder = property.SortOrder;
                MarkOld();
            }
        }

        private ConfigurationProperty(Property property, bool isClean, bool isNew, string pikListDomainId)
            : this(property.Name, property.FriendlyName, property.Type, property.Precision, isNew)
        {
            _pickListDomainId = pikListDomainId;
            if (property.ValRuleList != null)
            {
                ValidationRuleList = property.ValRuleList;
            }

            if (isClean)
                MarkClean();
            else
                MarkDirty();
            if (isNew)
            {
                _sortOrder = -1;
                MarkNew();
            }
            else
            {
                _sortOrder = property.SortOrder;
                MarkOld();
            }
        }

        [COEUserActionDescription("CreateConfigurationProperty")]
        public static ConfigurationProperty NewConfigurationProperty(string name,string friendlyName ,string type, string precision, bool isNew)
        {
            try
            {
                if (name == string.Empty || type == string.Empty)
                {
                    throw new Exception("You must insert Property Name");
                }

                else if (friendlyName == string.Empty )
                {
                    throw new Exception("You must insert Property label");
                }
                else if (float.Parse(precision) < 1)
                {
                    throw new Exception("Property precision can´t be less than one (1)");
                }
                else
                    return new ConfigurationProperty(name, friendlyName, type, precision.Replace(",", "."), isNew);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        /// <summary>
        /// Creates a Config property for a further insert into the DB (when saved)
        /// </summary>
        /// <param name="name">Property Name</param>
        /// <param name="type">Type of property (related with DB type)</param>
        /// <param name="precision">Precision of the property</param>
        /// <param name="isNew"></param>
        /// <param name="subType">Type of control to associated with</param>
        /// <returns>A newly craeted property</returns>
        /// <remarks>With subtype, you have a better control of the control that will be created (e.g URL -> Link and not a textbox)</remarks>
        [COEUserActionDescription("CreateConfigurationProperty")]
        public static ConfigurationProperty NewConfigurationProperty(string name,string friendlyName ,string type, string precision, bool isNew, string subType,string pickListDomainId)
        {
            try
            {
                if (name == string.Empty || type == string.Empty)
                {
                    throw new ValidationException("You must insert Property Name");
                }
                else if (friendlyName == string.Empty)
                {
                    throw new Exception("You must insert Property label");
                }
                else if (float.Parse(precision) < 1)
                {
                    throw new ValidationException("Property precision can´t be less than one (1)");

                }
                else
                    return new ConfigurationProperty(name, friendlyName, type, precision.Replace(",", "."), isNew, subType, pickListDomainId);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("CreateConfigurationProperty")]
        public static ConfigurationProperty NewConfigurationProperty(Property property, bool isClean, bool isNew)
        {
            try
            {
                if (!string.IsNullOrEmpty(property.PickListDomainId))
                    return new ConfigurationProperty(property, isClean, isNew, property.PickListDomainId);
                else
                    return new ConfigurationProperty(property, isClean, isNew);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        #endregion

        #region Business Method

        [COEUserActionDescription("GetConfigurationPropertyXml")]
        public override string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                builder.Append("<Property");
                if (!string.IsNullOrEmpty(_name))
                    builder.Append(" name=\"" + _name + "\"");
                if (!string.IsNullOrEmpty(_friendlyName))
                    builder.Append(" friendlyName=\"" + _friendlyName + "\"");
                if (!string.IsNullOrEmpty(_type))
                    builder.Append(" type=\"" + _type + "\"");
                if (!string.IsNullOrEmpty(_subType))
                    builder.Append(" subtype=\"" + _subType + "\"");
                if (this.Type != FormGroup.ValidationRuleEnum.Date.ToString())
                    builder.Append(" precision=\"" + _precision + "\"");
                if (!string.IsNullOrEmpty(_pickListDomainId))
                {
                    builder.Append(" pickListDomainID=\"" + _pickListDomainId + "\"");
                    builder.Append(" pickListDisplayValue=\"" + (string.IsNullOrEmpty(_pickListDisplayValue) ? "" : _pickListDisplayValue) + "\"");
                }

                if (SortOrderIsUpdate && !IsNew && !IsDeleted)
                {
                    builder.Append(" update=\"sortOrder\"");
                }

                if (PrecisionIsUpdate && !IsNew && !IsDeleted)
                {
                    builder.Append(" update=\"precision\"");
                }

                builder.Append(" sortOrder=\"" + _sortOrder + "\"");

                if (!IsDeleted && IsNew)
                {
                    builder.Append(" insert=\"yes\"");
                }
                else if (IsDeleted)
                {
                    builder.Append(" delete=\"yes\"");
                    builder.Append(">");
                    builder.Append("</Property>");
                    return builder.ToString();
                }
                builder.Append(">");
                builder.Append(_validationRuleList.UpdateSelfConfig(this.IsNew));
                builder.Append("</Property>");
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return builder.ToString();
        }

        [COEUserActionDescription("AddConfigurationPropertyDefaultRule")]
        public void AddDefaultRule()
        {
            long integerPart;
            long decimalPart;
            long maxLength = 0;
            try
            {
                switch (this.Type.ToUpper())
                {
                    case "NUMBER":

                        if (_precision != null && _precision != string.Empty)
                        {
                            char[] delElement ={ '.' };

                            string[] presSplit = _precision.Split(delElement);
                            maxLength = integerPart = Convert.ToInt64(presSplit[0]);
                            decimalPart = Convert.ToInt64(presSplit[1]);
                            maxLength += 1;

                            ParameterList numberParams = ParameterList.NewParameterList();
                            Parameter intPart = Parameter.NewParameter("integerPart", integerPart.ToString(), true);
                            Parameter decPart = Parameter.NewParameter("decimalPart", decimalPart.ToString(), true);
                            numberParams.Add(intPart);
                            numberParams.Add(decPart);
                            string numberError = string.Empty;
                            if (decPart.Value == "0")
                            {
                                numberError = string.Format("This property value must be an integer number", intPart.Value);
                                ValidationRule integerVal = ValidationRule.NewValidationRule("integer", numberError, numberParams, true);
                                _validationRuleList.Add(integerVal);
                                ParameterList parameters = ParameterList.NewParameterList();
                                Parameter min = Parameter.NewParameter("min", "0", true);
                                Parameter max = Parameter.NewParameter("max", intPart.Value, true);
                                parameters.Add(min);
                                parameters.Add(max);
                                string error = string.Format("The property value can have between {0} and {1} characters", min.Value, max.Value);//TODO: FriendlyName for Label text .
                                ValidationRule length = ValidationRule.NewValidationRule("textLength", error, parameters, true);
                                _validationRuleList.Add(length);
                            }
                            else
                            {
                                numberError = string.Format("This property can have at most {0} integer and {1} decimal digits", intPart.Value, decPart.Value);
                                ValidationRule floatVal = ValidationRule.NewValidationRule("float", numberError, numberParams, true);
                                _validationRuleList.Add(floatVal);
                            }

                        }
                        break;
                    case "TEXT":
                        if (_precision != null)
                        {
                            maxLength += Convert.ToInt64(_precision);
                        }
                        break;
                    default:

                        break;
                }

                if ((Type == "TEXT") && this.SubType != "URL")
                {
                    ParameterList parameters = ParameterList.NewParameterList();
                    Parameter min = Parameter.NewParameter("min", "0", true);
                    Parameter max = Parameter.NewParameter("max", maxLength.ToString(), true);
                    parameters.Add(min);
                    parameters.Add(max);
                    string error = string.Format("The property value can have between {0} and {1} characters", min.Value, max.Value);//TODO: FriendlyName for Label text .
                    ValidationRule defVal = ValidationRule.NewValidationRule("textLength", error, parameters, true);
                    _validationRuleList.Add(defVal);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }     

        #endregion
    }
}
