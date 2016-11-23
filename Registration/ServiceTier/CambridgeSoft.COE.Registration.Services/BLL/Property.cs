using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class Property : RegistrationBusinessBase<Property>
    {

        #region Factory Methods

        [COEUserActionDescription("CreateProperty")]
        public static Property NewProperty()
        {
            try
            {
                Property property = new Property();

                return property;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateProperty")]
        public static Property NewProperty(string xml, bool isClean)
        {
            try
            {
                return new Property(xml, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        protected Property()
            : base()
        {
            this._type = this._value = string.Empty;
        }

        private Property(string xml, bool isClean)
            : this()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode node = doc.SelectSingleNode("Property");
            _validationRulesXml = string.Empty;
            XmlNode validationRuleListNode = node.SelectSingleNode("validationRuleList");
            if (node.ChildNodes.Count > 0 && validationRuleListNode != null)
            {
                _validationRulesXml = validationRuleListNode.OuterXml;
                this._validationRuleList = ValidationRuleList.NewValidationRuleList(validationRuleListNode);

                node.RemoveChild(validationRuleListNode);
            }

            foreach (XmlAttribute attrib in node.Attributes)
            {
                switch (attrib.Name)
                {
                    case "id":
                        if (!string.IsNullOrEmpty(attrib.Value))
                            _id = int.Parse(attrib.Value); break;
                    case NAME: _name = attrib.Value; break;
                    case TYPE: _type = attrib.Value; break;
                    case PICKLIST_DOMAIN_ID: _pickListDomainId = attrib.Value; break;
                    case PICKLIST_DISPLAY_VALUE: _pickListDisplayValue = attrib.Value; break;
                    case SUBTYPE: _subType = attrib.Value; break;
                    case FRIENDLY_NAME: _friendlyName = attrib.Value; break;
                    case PRECISION:
                        if (!string.IsNullOrEmpty(attrib.Value))
                            _precision = attrib.Value; break;
                    case SORT_ORDER:
                        if (!string.IsNullOrEmpty(attrib.Value))
                            _sortOrder = int.Parse(attrib.Value);
                        else
                            _sortOrder = -1;
                        break;
                    default: break;
                }
            }

            //Ensures that the _type has already been assigned

            //Remove trailing new line and white-space character, which were generated during retreive process (xsl transform)
            //Without removing, these characters would cause problem in Oracle 10g
            _value = node.InnerText.TrimEnd('\r', '\n', ' ');

            if (isClean)
                MarkClean();
            else
                MarkDirty();
            AddInstanceBusinessRules();
        }

        [COEUserActionDescription("SaveProperty")]
        public override Property Save()
        {
            try
            {
                if (IsDeleted && !CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a Property");
                }
                else if (IsNew && !CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a Property");
                }
                else if (!CanEditObject())
                {
                    throw new System.Security.SecurityException("User not authorized to update a Property");
                }
                return base.Save();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        #region Authorization and Validation Rules

        protected override void AddAuthorizationRules()
        {
            //TODO: Determine if this is correct or not. Is this is a dual-purpose permission?
            AuthorizationRules.AllowWrite("Property", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("property");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("property");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("property");
        }

        /// <summary>
        /// Provides validation rules dynamically based on the metadata from xml-based initialization.
        /// </summary>
        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();

            if (!string.IsNullOrEmpty(_name) && _name.ToUpper() == "BATCH_PREFIX")
            {
                if (RegSvcUtilities.GetEnableBatchPrefix())
                    ValidationRulesFactory.GetInstance().AddInstanceRules(this.ValidationRules, "Value", _validationRulesXml);
            }
            else if (!string.IsNullOrEmpty(_validationRulesXml))
                ValidationRulesFactory.GetInstance().AddInstanceRules(this.ValidationRules, "Value", _validationRulesXml);

            if (!string.IsNullOrEmpty(_pickListDomainId))
                this.ValidationRules.AddInstanceRule(
                    ValidationRulesFactory.IsPicklistValue
                    , new IsPicklistValueRuleArgs("Value", PickListDomainId));

            if (!string.IsNullOrEmpty(_type) && _type.ToUpper() == "DATE")
            {
                RuleHandler formatHandler = new RuleHandler(ValidationRulesFactory.IsDateFormat);
                RuleArgs ruleArgs = new IsDateFormatRuleArgs(
                    "Value", Constants.DATE_FORMAT, new System.Globalization.CultureInfo(Constants.DEFAULT_CULTURE));
                this.ValidationRules.AddInstanceRule(formatHandler, ruleArgs);
            }
        }

        #endregion

        #region Properties and members

        protected string _validationRulesXml;

        protected int _id;
        /// <summary>
        /// The ID of the instance
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        protected string _name;
        /// <summary>
        /// The internal name of this Property; a database column name
        /// </summary>
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                CanWriteProperty(true);
                if (_name != value)
                {
                    _name = value;
                    PropertyHasChanged();
                }
            }
        }

        protected string _type;
        /// <summary>
        /// An agnostic data-type indicator for use by consumers
        /// </summary>
        public virtual string Type
        {
            get
            {
                return _type;
            }
            set
            {
                CanWriteProperty(true);
                if (_type != value)
                {
                    _type = value;
                    PropertyHasChanged();
                }
            }
        }

        protected string _pickListDisplayValue;
        /// <summary>
        /// The read-only 'name' meber of the natural 'name/value' pair represented by the Value property.
        /// </summary>
        public virtual string PickListDisplayValue
        {
            get { return _pickListDisplayValue; }
            set { _pickListDisplayValue = value; }
        }

        protected string _pickListDomainId;
        /// <summary>
        /// The read-only identifier of the picklist from which the instance's Value was derived.
        /// This is useful if the caller needs to determine all available/possible values. This
        /// should be empty (or null) if the value is not derived from a proscribed picklist.
        /// </summary>
        public virtual string PickListDomainId
        {
            get { return _pickListDomainId; }
        }

        protected string _value;
        /// <summary>
        /// The 'value' of the Property itself
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                CanWriteProperty(true);
                if (_value != value)
                {
                    //Blank out non-matching display data
                    _pickListDisplayValue = null;

                    //These methods provide support for culture-invariant dates/numbers, and also
                    // for automatic conversion of display- to value-members for picklists
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (_type.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                            _value = FormatBoolString(value);

                        else if (_type.Equals("date", StringComparison.OrdinalIgnoreCase))
                            _value = FormatDateString(value);

                        else if (_type.Equals("number", StringComparison.OrdinalIgnoreCase))
                            _value = FormatNumberString(value);

                        else if (!string.IsNullOrEmpty(_pickListDomainId))
                            _value = GetPicklistMemberValue(value);
                        else
                            _value = value;
                    }
                    else
                        _value = value;

                    PropertyHasChanged();
                }
            }
        }

        protected string _defaultValue = string.Empty;
        /// <summary>
        /// The internal name of this Property; a default value for column
        /// </summary>
        public virtual string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                CanWriteProperty(true);
                if (_defaultValue != value)
                {
                    _defaultValue = value;
                    PropertyHasChanged();
                }
            }
        }

        protected string _precision;
        /// <summary>
        /// For numeric values, describes the maximum number of decimal digits available.
        /// For string values, it signifies the maximum length of the value.
        /// </summary>
        public virtual string Precision
        {
            get { return _precision; }
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _precision = value;
                    PrecisionIsUpdate = true;
                    PropertyHasChanged("Precision");
                }
            }
        }
        
        protected ValidationRuleList _validationRuleList;
        /// <summary>
        /// As derived from the instantiating xml string, the set of validation rules
        /// to be applied to the BO instance.
        /// </summary>
        public ValidationRuleList ValRuleList
        {
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _validationRuleList = value;
                    PropertyHasChanged();
                }

            }
            get
            {
                CanReadProperty(true);
                return _validationRuleList;
            }
        }

        protected int _sortOrder;
        /// <summary>
        /// For presentation tier and xml-gen. mechanisms, the order in which the property
        /// should be in its parent PropertyList instance.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
                _sortOrderUpdate = true;
            }
        }

        private bool _sortOrderUpdate = false;
        public bool SortOrderIsUpdate
        {
            get
            {
                return _sortOrderUpdate;
            }
        }

        protected string _subType = string.Empty;
        /// <summary>
        /// May not be in use at this time?
        /// </summary>
        public virtual string SubType
        {
            get { return _subType; }
            set { _subType = value; }
        }
        
        protected string _friendlyName = string.Empty;
        /// <summary>
        /// The outward-facing name describing the Property; can be used as a UI label
        /// </summary>
        public virtual string FriendlyName
        {
            get
            {
                if (_friendlyName == string.Empty)
                    return _name;
                else
                    return _friendlyName;
            }
        }

        protected bool _valRuleListIsUdate;
        public bool ValRuleListIsUpdate
        {
            get
            {
                if (ValRuleList == null) return false;
                else if(ValRuleList.IsDirty)//if there are any non-new deletions, or any child objects are dirty
                    return true;
                else//if there are any new child objects
                {
                    foreach (ValidationRule rule in ValRuleList)
                        if (rule.IsNew) return true;
                }
                return false;
            }
        }

        private bool _precisionIsUpdate = false;
        public bool PrecisionIsUpdate
        {
            get { return _precisionIsUpdate; }
            set
            {
                CanWriteProperty(true);
                _precisionIsUpdate = true;
                PropertyHasChanged("PrecisionIsUpdate");
            }
        }

        public override bool IsValid
        {
            get
            {
                this.ValidationRules.CheckRules();

                return base.IsValid;
            }
        }

        public override bool IsDirty
        {

            get
            {
                if (this._validationRuleList != null)
                    return base.IsDirty || this._validationRuleList.IsDirty;
                else
                    return base.IsDirty;
            }
        }

        [NonSerialized()]
        public string InstanceXml;

        #endregion

        #region Constants

        private const string PICKLIST_DOMAIN_ID = "pickListDomainID";
        private const string PICKLIST_DISPLAY_VALUE = "pickListDisplayValue";
        private const string NAME = "name";
        private const string TYPE = "type";
        private const string SUBTYPE = "subtype";
        private const string FRIENDLY_NAME = "friendlyName";
        private const string PRECISION = "precision";
        private const string SORT_ORDER = "sortOrder";

        #endregion

        [COEUserActionDescription("GetPropertyBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                if (!(this.IsValid) && this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
                {
                    brokenRules.Add(new BrokenRuleDescription(this, this.GetBrokenRuleDescriptions()));
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Returns a string array containing all broken rule descriptions using the Property.Name in
        /// place of the property name.
        /// </summary>
        /// <returns>The text of all broken rule descriptions.</returns>
        private string[] GetBrokenRuleDescriptions()
        {
            List<string> result = new List<string>();
            foreach (BrokenRule item in this.BrokenRulesCollection)
            {
                string description = string.Format("Property '{0}' - {1}", this.FriendlyName, item.Description);
                result.Add(description);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Csla requirement for the current version of that framework.
        /// </summary>
        /// <returns>the local ID value</returns>
        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// Utility mechanism to ensure a wide variety of acceptable boolean strings.
        /// </summary>
        /// <remarks>
        /// Callers to this method must avoid passing nulls.
        /// </remarks>
        /// <param name="rawValue">the pending Property.Value string</param>
        /// <returns>as required, the reformatted value</returns>
        private string FormatBoolString(string rawValue)
        {
            string formattedValue = rawValue.Trim();
            if (rawValue != "T" && rawValue != "F")
            {
                bool boolValue;
                if (Boolean.TryParse(rawValue, out boolValue))
                {
                    formattedValue =
                        boolValue.ToString(new System.Globalization.CultureInfo(Constants.DEFAULT_CULTURE));
                    formattedValue = formattedValue.Substring(0, 1).ToUpper();
                }
                else
                {
                    //try a custom conversion of widely-acceptable boolean equivalents
                    switch (rawValue.ToLower())
                    {
                        case "y":
                        case "yes":
                        case "t":
                        case "true":
                        case "1":
                            {
                                formattedValue = "T";
                                break;
                            }
                        case "n":
                        case "no":
                        case "f":
                        case "false":
                        case "0":
                            {
                                formattedValue = "F";
                                break;
                            }
                    }
                }
            }
            return formattedValue;
        }

        /// <summary>
        /// Utility mechanism to ensure the format of the number string is the culture-invariant
        /// one required by the data-repository.
        /// </summary>
        /// <param name="rawValue">the pending Property.Value string</param>
        /// <returns>as required, the reformatted value</returns>
        private string FormatNumberString(string rawValue)
        {
            string formattedValue = rawValue;
            decimal decValue;
            if (Decimal.TryParse(rawValue, out decValue))
                formattedValue = decValue.ToString(new System.Globalization.CultureInfo(Constants.DEFAULT_CULTURE));
            return formattedValue;
        }

        /// <summary>
        /// Utility mechanism to ensure the format of the date string is the culture-invariant
        /// one required by the data-repository.
        /// </summary>
        /// <param name="rawValue">the pending Property.Value string</param>
        /// <returns>as required, the reformatted value</returns>
        private string FormatDateString(string rawValue)
        {
            string formattedValue = rawValue;
            DateTime dtValue;
            if (DateTime.TryParse(rawValue, out dtValue))
                formattedValue = dtValue.ToString(
                    Constants.DATE_FORMAT, new System.Globalization.CultureInfo(Constants.DEFAULT_CULTURE));
            return formattedValue;
        }

        /// <summary>
        /// Provides a built-in converter from the a display-member (KeyValuePair.Value)
        /// to a value-member (KeyValuePair.Key).
        /// </summary>
        /// <param name="rawValue">the pending Property.Value string</param>
        /// <returns>the ID string of the picklist item, or -1 if no match is found</returns>
        private string GetPicklistMemberValue(string rawValue)
        {
            int id = -1;
            if (Int32.TryParse(rawValue, out id) == false)
            {
                Picklist list = DalUtils.GetPicklistByCode(this.PickListDomainId);
                if (list != null)
                {
                    id = list.GetListItemIdByValue(rawValue);
                    if (id != -1)
                        this.PickListDisplayValue = list.GetListItemValueById(id);
                }
            }
            return id.ToString();
        }

        /// <summary>
        /// OVerridden ToString includes only Type and Value data points.
        /// </summary>
        /// <returns>a string representation of the object's data</returns>
        public override string ToString()
        {
            return this._id + " - "
                + (string.IsNullOrEmpty(this._type) ? string.Empty : this._type + " ")
                + this._name + (!string.IsNullOrEmpty(this._value) ? " = " + this._value : string.Empty);
        }

        /// <summary>
        /// Allows updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the Property node</param>
        public void UpdateFromXml(XmlNode parentNode)
        {   //LJB: only update when the  not  is not blank and the values do not match
            if (!string.IsNullOrEmpty(parentNode.InnerText.Trim()) && this.Value.Trim() != parentNode.InnerText.ToString().Trim())
            {
                this.Value = parentNode.InnerText;
            }
        }
        /// <summary>
        /// Allows updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the Property node</param>
        public void UpdateUserPreference(XmlNode parentNode)
        {   //only update when the value is null and user preference value is not null
            if (!string.IsNullOrEmpty(parentNode.InnerText.Trim()) && string.IsNullOrEmpty(this.Value.Trim()))
            {
                this.Value = parentNode.InnerText;
            }
        }

        /// <summary>
        /// Lock Picklistdomain
        /// </summary>
        /// <param name="domainId">the picklistdomain id</param>
         public string LockPickListDomain(string domainId)
        {
            int ID = -1;
            string retVal = string.Empty;
            if(int.TryParse(domainId, out ID))
                retVal = this.RegDal.UpdatekPickListDomainLock('T', ID);
            return retVal;
        }

         /// <summary>
         /// Unlock Picklistdomain
         /// </summary>
         /// <param name="domainId">the picklistdomain id</param>
         public string UnLockPickListDomain(string domainId)
         {
             int ID = -1;
             string retVal = string.Empty;
             if (int.TryParse(domainId, out ID))
                 retVal = this.RegDal.UpdatekPickListDomainLock('F', ID);
             return retVal;
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addCRUDattributes"></param>
        /// <returns></returns>
        public virtual string UpdateSelf(bool addCRUDattributes)
        {
            //TODO: Determine if subType should be here; also if id, friendlyName and sortOrder should be added.
            StringBuilder builder = new StringBuilder("");
            builder.Append("<Property");
            if (!string.IsNullOrEmpty(_name))
            {
                builder.Append(" ");
                builder.Append(NAME + "=\"" + _name + "\"");
            }
            if (!string.IsNullOrEmpty(_type))
            {
                builder.Append(" ");
                builder.Append(TYPE + "=\"" + _type + "\"");
            }
            if (!string.IsNullOrEmpty(_pickListDomainId))
            {
                builder.Append(" ");
                builder.Append(PICKLIST_DOMAIN_ID + "=\"" + _pickListDomainId + "\"");
                if (!string.IsNullOrEmpty(_value))
                    LockPickListDomain(_pickListDomainId); // Lock picklist domain.
            }
            if (!string.IsNullOrEmpty(_pickListDisplayValue))
            {
                builder.Append(" ");
                builder.Append(PICKLIST_DISPLAY_VALUE + "=\"" + _pickListDisplayValue + "\"");
            }
            if (!string.IsNullOrEmpty(_subType))
            {
                builder.Append(" ");
                builder.Append(SUBTYPE + "=\"" + _subType + "\"");
            }
            if (addCRUDattributes && this.IsNew)
                builder.Append(" insert=\"yes\"");
            if (addCRUDattributes && this.IsDirty)
                builder.Append(" update=\"yes\"");
            if (addCRUDattributes && IsDeleted)
                builder.Append(" deleted=\"yes\"");
            builder.Append(">");
            if (_value == "True" || _value == "False")
                builder.Append(System.Web.HttpUtility.HtmlEncode(_value.Substring(0, 1)));
            else
                builder.Append(System.Web.HttpUtility.HtmlEncode(_value));
            builder.Append("</Property>");

            string sxml = builder.ToString();
            this.InstanceXml = sxml;
            return sxml;
        }

    }
}
