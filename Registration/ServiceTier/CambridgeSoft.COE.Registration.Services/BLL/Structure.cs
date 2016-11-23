using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.BLL;

using Csla;
using Csla.Data;
using Csla.Validation;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using CambridgeSoft.COE.Registration.Validation;
using CambridgeSoft.COE.Registration.Access;



namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class Structure : RegistrationBusinessBase<Structure>
    {
        private const string STRUCTURE_CACHEKEY = "Structure.{0}";
        private const string STRUCTURE_MANAGERNAME = "Default Cache Manager";
        private const CacheItemPriority CACHE_PRIORITY = CacheItemPriority.Normal;
        private static SlidingTime CacheExpiry = new SlidingTime(TimeSpan.FromSeconds(60));

        private List<string> _changedProperties = new List<string>();
        private string _validationRulesXml;
        private bool _isLinked = false;

        public bool IsWildcard
        {
            get
            {
                CanReadProperty(true);
                return this.ID < 0;
            }
        }

        private string _format;
        public string Format
        {
            get
            {
                CanReadProperty(true);
                return _format;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if (_format != value)
                {
                    _format = value;
                    PropertyHasChanged();
                }
            }
        }

        private string _value;
        public string Value
        {
            get
            {
                CanReadProperty(true);
                return _value;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = string.Empty;
                if (value != "null" && _value != value)
                {
                    if (this.IsEncoded(value) && !this.IsAllowedEncodedValues)
                        throw new ValidationException("The application doesn't support encoded values. Please check your ChemDraw version.");
                    else
                    {
                        _value = value;
                        //JED: Why was the following line being done?!?!? 
                        //FRG: To keep the normalized structure with the original value, if no addin work is triggered
                        //NormalizedStructure = _value;

                        NormalizedStructure = _value;
                        PropertyHasChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Flag that indicates if encoded values are allowed.
        /// </summary>
        /// <remarks>Encoded values come usually from chemdraw controls version .net</remarks>
        private bool IsAllowedEncodedValues
        {
            //TODO: get this values from a configuration file.
            get
            {
                return true;
                //TODO: Make this conditional.
                //return false;
            }
        }

        private double _molWeight;
        public double MolWeight
        {
            get
            {
                CanReadProperty(true);
                return this._molWeight;
            }
            set 
            {
                this._molWeight = value;
                PropertyHasChanged();
            }
        }

        private string _formula;
        public string Formula
        {
            get
            {
                CanReadProperty(true);
                return this._formula;
            }
            set
            {
                this._formula = value;
                PropertyHasChanged();
            }
        }

        private DrawingType _drawingType = DrawingType.Chemical;
        /// <summary>
        /// Types of structure
        /// </summary>
        public DrawingType DrawingType
        {
            get
            {
                return _drawingType;
            }
            set
            {
               
                if (_drawingType != value)
                {
                   
                    _drawingType = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _id = 0;
        /// <summary>
        /// Rewrite ID property, update DrawingType and Value when updating the ID.
        /// </summary>
        /// <remarks>
        /// For DrawingType <> 0, the 'Value' can be anything except null or an empty string;
        /// it's there simply to bypass the 'required field' BO rule since the actual DB structure
        /// value will be NULL.
        /// </remarks>
        public override int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value <= 0)
                {
                    switch (value)
                    {
                        case 0:
                            DrawingType = DrawingType.Chemical;
                            this.NormalizedStructure = this.Value;
                            _id = 0;
                            break;
                        case -1:
                            DrawingType = DrawingType.Unknown;
                            this.Value = DrawingType.Unknown.ToString();
                            this.NormalizedStructure = null;
                            break;
                        case -2:
                            DrawingType = DrawingType.NoStructure;
                            this.Value = DrawingType.NoStructure.ToString();
                            this.NormalizedStructure = null;
                            break;
                        case -3:
                            DrawingType = DrawingType.NonChemicalContent;
                            this.Value = DrawingType.NonChemicalContent.ToString();
                            this.NormalizedStructure = null;
                            break;
                    }
                }
                else
                {
                    if (_id != value)
                        _id = value;
                }
            }
        }

        private IdentifierList _identifierList;

        /// <summary>
        /// Structure level Identifiers
        /// </summary>
        public IdentifierList IdentifierList
        {
            get
            {
                if (_identifierList == null)
                    _identifierList = IdentifierList.NewIdentifierList();
                return _identifierList;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if (_identifierList != value)
                {
                    _identifierList = value;
                    PropertyHasChanged();
                }
            }
        }

        private string _normalizedStructure;
        public string NormalizedStructure
        {
            get
            {
                CanReadProperty(true);
                return _normalizedStructure;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = string.Empty;
                if (value != "null" && _normalizedStructure != value)
                {
                    _normalizedStructure = value;
                    PropertyHasChanged();
                }
            }
        }

        private string _normalizationLog;
        public string NormalizationLog
        {
            get
            {
                CanReadProperty(true);
                return _normalizationLog;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null)
                    value = string.Empty;

                if (_normalizationLog != value)
                {
                    _normalizationLog = value;
                    PropertyHasChanged();
                }
            }
        }

        private bool _useNormalizedStructure = true;
        public bool UseNormalizedStructure
        {
            get
            {
                CanReadProperty(true);
                return _useNormalizedStructure;
            }
            set
            {
                CanWriteProperty(true);
                if (_useNormalizedStructure != value)
                    _useNormalizedStructure = value;
            }
        }

        private bool _isRegistering = false;
        public bool IsBeingRegistered
        {
            get { return _isRegistering;  }
            set { _isRegistering = value; }
        }

        private bool _canPropogateStructureEdits = true;
        public bool CanPropogateStructureEdits
        {
            get { return _canPropogateStructureEdits; }
        }

        private bool _isTemporary = true;
        public bool IsTemporary
        {
            get
            {
                CanReadProperty(true);
                return _isTemporary;
            }
            set
            {
                CanWriteProperty(true);
                if (_isTemporary != value)
                    _isTemporary = value;
            }
        }

        private PropertyList _propertyList;
        /// <summary>
        /// Custom properties for this structure
        /// </summary>
        public PropertyList PropertyList
        {
            get
            {
                CanReadProperty(true);
                if (_propertyList == null)
                    _propertyList = PropertyList.NewPropertyList();

                return _propertyList;
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
            get { return base.IsDirty; }
        }

        internal new void MarkClean()
        {
            base.MarkClean();
            this._changedProperties.Clear();
        }

        internal void MarkLinked() 
        {
            this._isLinked = true;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "Value")
            {
                if (this.IsWildcard)
                    this.MarkNew();
            }

            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

        /// <summary>
        /// Given an Identifier name and a value, adds a new Identifier object to the Structure's
        /// IdentifierList.
        /// </summary>
        /// <param name="identifierName">the internal name of the identifier</param>
        /// <param name="identifierValue">the value to apply to the new Identifier instance</param>
        public void AddIdentifier(string identifierName, string identifierValue)
        {
            try
            {
                RegSvcUtilities.CreateNewIdentifier(
                    identifierName
                    , identifierValue
                    , this.IdentifierList
                    , IdentifierTypeEnum.S);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        #region Validation Rules

        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();

            if (!string.IsNullOrEmpty(_validationRulesXml))
                ValidationRulesFactory.GetInstance().AddInstanceRules(this.ValidationRules, "Value", _validationRulesXml);
        }

        [COEUserActionDescription("GetStructureBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                if(this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
                {
                    brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        #endregion

        #region Authorization Rules

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "Structure", "ADD_STRUCTURE");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("structure");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("structure");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("structure");
        }

        #endregion

        #region Factory Methods

        [COEUserActionDescription("CreateStructure")]
        public static Structure NewStructure()
        {
            try
            {
                Structure structure = new Structure();
                return structure;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateStructure")]
        public static Structure NewStructure(string xml, bool isNew, bool isClean)
        {
            try
            {
                return new Structure(xml, isNew, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateStructure")]
        public static Structure NewStructure(string value, double mw, string formula)
        {
            try
            {
                return new Structure(value, mw, formula);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private Structure()
        {
            MarkAsChild();
        }

        private Structure(bool isClean)
        {
            if (isClean)
                MarkClean();
        }

        private Structure(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Structure/StructureFormat");
            if (xIterator.MoveNext())
                _format = xIterator.Current.Value;

            xIterator = xNavigator.Select("Structure/StructureID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this.ID = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Structure/DrawingType");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._drawingType = (DrawingType)Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Structure/Structure");
            if (xIterator.MoveNext())
            {
                _value = xIterator.Current.Value;
                
                _formula = xIterator.Current.GetAttribute("formula", xIterator.Current.NamespaceURI);

                if (xIterator.Current.GetAttribute("molWeight", xIterator.Current.NamespaceURI) != string.Empty)
                    _molWeight = double.Parse(xIterator.Current.GetAttribute("molWeight", xIterator.Current.NamespaceURI), CultureInfo.InvariantCulture);

                xIterator = xNavigator.Select("Structure/Structure/validationRuleList");
                if (xIterator.MoveNext())
                    _validationRulesXml = xIterator.Current.OuterXml;

                xIterator = xNavigator.Select("Structure/NormalizedStructure");
                if (xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.Value))
                    _normalizedStructure = xIterator.Current.Value;
                //if _value is null put the normalized structure here. This issue is occuring when a new component is added
                //to a tempoery record
                if (string.IsNullOrEmpty(_value))
                {
                    _value = _normalizedStructure;
                }

                xIterator = xNavigator.Select("Structure/IdentifierList");
                if (xIterator.MoveNext())
                    this._identifierList = IdentifierList.NewIdentifierList(xIterator.Current.OuterXml, isNew, isClean);
                else
                    this._identifierList = IdentifierList.NewIdentifierList();

                xIterator = xNavigator.Select("Structure/UseNormalization");
                if (xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.Value))
                    _useNormalizedStructure = ((COEDALBoolean)Enum.Parse(typeof(COEDALBoolean), xIterator.Current.Value)) == COEDALBoolean.T ? true : false;

                // 11.0.4: Add custom property
                xIterator = xNavigator.Select("Structure/PropertyList");
                if (xIterator.MoveNext())
                {
                    if (!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                        this._propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, isClean);
            }
                else
                {
                    _propertyList = PropertyList.NewPropertyList();
                }
                xIterator = xNavigator.Select("Structure/CanPropogateStructureEdits");
                if (xIterator.MoveNext())
                {
                    try { _canPropogateStructureEdits = Boolean.Parse(xIterator.Current.Value); }
                    catch { _canPropogateStructureEdits = true; }
                }
            }

            AddInstanceBusinessRules();

            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();
        }

        private Structure(string value, double mw, string formula)
        {
            _value = value;
            _molWeight = mw;
            _formula = formula;
        }

        [COEUserActionDescription("GetStruture")]
        public static Structure GetStructure(int id)
        {
            try
            {
                if(!CanGetObject())
                {
                    throw new System.Security.SecurityException("User not authorized to view a Structure");
                }
                return DataPortal.Fetch<Structure>(new Criteria(id));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetStruture")]
        public static Structure GetStructure(int id, bool useCache)
        {
            Structure structure = null;

            try
            {
                string cacheKey = string.Format(STRUCTURE_CACHEKEY, id);
                if (useCache)
                {
                    CacheManager manager = CacheFactory.GetCacheManager(STRUCTURE_MANAGERNAME);
                    structure = (Structure)manager.GetData(cacheKey);
                    if (structure == null)
                    {
                        structure = Structure.GetStructure(id);
                        manager.Add(cacheKey, structure, CACHE_PRIORITY, null, CacheExpiry);
                    }
                }
                else
                    structure = Structure.GetStructure(id);
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
                // caching is not configured by the hosting application
                structure = Structure.GetStructure(id);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }

            return structure;
        }

        protected static void DeleteStructure(int id)
        {
            if (!CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Structure");
            }
            DataPortal.Delete(new Criteria(id));
        }

        [COEUserActionDescription("SaveStructure")]
        public override Structure Save()
        {
            try
            {
                if(IsDeleted && !CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a Structure");
                }
                else if(IsNew && !CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a Structure");
                }
                else if(!CanEditObject())
                {
                    throw new System.Security.SecurityException("User not authorized to update a Structure");
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

        #region Data Access

        [Serializable()]
        private class Criteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }

            public Criteria(int id)
            { _id = id; }
        }

        [RunLocal()]
        private void DataPortal_Create(Criteria criteria)
        {
            this.ID = 1;
            ValidationRules.CheckRules();
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            SafeDataReader reader = this.RegDal.GetStructure(criteria.Id);
            Fetch(reader);
        }

        private void Fetch(SafeDataReader reader)
        {
            if (reader.Read())
            {
                //for backwards compatibility with structure ID -1, -2 and -3 for non-structural entities
                object val = reader.GetValue("ID");
                int structureId = 0;
                Int32.TryParse(val.ToString(), out structureId); 

                if (structureId > 0)
                    this.ID = structureId;
                else
                    this.DrawingType = (DrawingType)Math.Abs(structureId);
                
                this._value = reader.GetString("VALUE");
                this._formula = reader.GetString("FORMULA");

                int typeOrdinal = reader.GetOrdinal("TYPE");
                if (!reader.IsDBNull(typeOrdinal))
                    this._format = reader.GetString("TYPE");

                int molWeightOrdinal = reader.GetOrdinal("MOLWEIGHT");
                if (!reader.IsDBNull(molWeightOrdinal))
                    this._molWeight = double.Parse(reader.GetValue("MOLWEIGHT").ToString());
            }
        }

        #endregion

        #region Xml

        public string Xml
        {
            [COEUserActionDescription("GetStructureXml")]
            get 
            {
                try
                {
                    return this.UpdateSelf(false); 
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return null;
                }
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            string attributes = string.Empty;
            string buf = string.Empty;

            if (addCRUDattributes)
            {
                if (this.IsNew)
                    attributes = " insert=\"yes\"";
                else if (this.IsDirty || _isLinked)
                    attributes = " update=\"yes\"";
            }

            buf = string.Format("<Structure{0}>", attributes);
            builder.Append(buf);

            buf = createTag("StructureID", string.Empty, this.ID.ToString(), addCRUDattributes);
            builder.Append(buf);

            buf = createTag(
                "StructureFormat", string.Empty, this._format == null ? string.Empty : this._format, addCRUDattributes);
            builder.Append(buf);

            buf = createTag(
                "DrawingType", string.Empty, ((int)DrawingType).ToString(), addCRUDattributes);
            builder.Append(buf);

            string structureAttributes = string.Empty;
            if (!string.IsNullOrEmpty(this.Formula))
                structureAttributes += string.Format("formula=\"{0}\" ", this.Formula);

            if (this.MolWeight >= 0.0)
                structureAttributes += string.Format("molWeight=\"{0}\" ", this.MolWeight);

            //Patch for storing the original structure in case the user selected "use normalized structure"
            /*CSBR-117483 
             *This bug was caused because the method below does not check to see if the normalizedStructure
             *is null before setting it = to temp.  The result in this case is that the structure is stripped
             *out of the registry record.
             * JB 28-JAN-2010
             */
            //if (_isTemporary && _isRegistering && _drawingType == DrawingType.Chemical)
            //    if (_useNormalizedStructure)
            //    {
            //        string temp = _normalizedStructure;
            //        _normalizedStructure = _value;

            //        if (!string.IsNullOrEmpty(temp))
            //        {
            //            _value = temp;
            //        }
            //    }


            buf = createTag(
                "Structure"
                , structureAttributes
                , (DrawingType==DrawingType.Chemical) ? this._value: string.Empty
                , addCRUDattributes && this.IsNew
                , addCRUDattributes && (
                    this._changedProperties.Contains("Value") ||
                    this._changedProperties.Contains("Formula") ||
                    this._changedProperties.Contains("MolWeight")
                )
            );
            builder.Append(buf);

            buf = string.Format("<UseNormalization>{0}</UseNormalization>", _useNormalizedStructure ? COEDALBoolean.T : COEDALBoolean.F);
            builder.Append(buf);

            buf = createTag("NormalizedStructure", null, this._normalizedStructure, addCRUDattributes && this._changedProperties.Contains("NormalizedStructure"));
            builder.Append(buf);

            // 11.0.4: Add custom property list
            if (_propertyList != null)
            {
                buf = this.PropertyList.UpdateSelf(addCRUDattributes);
                builder.Append(buf);
            }

            // 11.0.4: Add Structure IdentifierList
            if (_identifierList != null)
            {
                buf = this.IdentifierList.UpdateSelf(addCRUDattributes);
                builder.Append(buf);
            }

            builder.Append("</Structure>");

            buf = builder.ToString();
            return buf;
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            //validation 
            //1. check id
            XmlNode idNode = parentNode.SelectSingleNode("//Structure/StructureID");
            if ((!string.IsNullOrEmpty(idNode.InnerText) && this._id == int.Parse(idNode.InnerText)))
            {

                XmlNode propertyList = parentNode.SelectSingleNode("//Structure/PropertyList");
                this.PropertyList.UpdateFromXml(propertyList);

                XmlNode IdentifierList = parentNode.SelectSingleNode("//Structure/IdentifierList");
                this.IdentifierList.UpdateFromXml(IdentifierList);
              
            }
        }

        /// <summary>
        /// Userpreference update method for structure properties
        /// </summary>
        /// <param name="parentNode"></param>
        internal void UpdateUserPreference(XmlNode parentNode)
        {
            XmlNode idNode = parentNode.SelectSingleNode("//Structure/StructureID");
            if (!string.IsNullOrEmpty(idNode.InnerText))
            {

                XmlNode propertyList = parentNode.SelectSingleNode("//Structure/PropertyList");
                if (this.PropertyList .Count!=0)
                    this.PropertyList.UpdateUserPreference(propertyList);
                else
                    this._propertyList = PropertyList.NewPropertyList(propertyList.OuterXml, true);
                // Only considering the Properties under structure now.
                //XmlNode IdentifierList = parentNode.SelectSingleNode("//Structure/IdentifierList");
                //this.IdentifierList.UpdateFromXml(IdentifierList);

            }
        }

        internal string createTag(string tagName, string attributes, string value, bool updateAttribute)
        {
            if (attributes == null)
                attributes = string.Empty;

            string updateAttrib = string.Empty;
            if (updateAttribute)
                //please note the space preceeding update="yes"
                updateAttrib = " update=\"yes\"";

            string spacer = string.Empty;
            if (!string.IsNullOrEmpty(attributes))
                spacer = " ";

            //string buf = string.Format("<{0} {3}{1}>{2}</{0}>", tagName, updateAttrib, value, attributes);
            string buf =  string.Format("<{0}{1}{2}{3}>{4}</{0}>", tagName, spacer, attributes, updateAttrib, value);
            return buf;
        }

        internal string createTag(
            string tagName, string attributes, string value, bool isNew, bool updateAttribute)
        {
            if (attributes == null)
                attributes = string.Empty;

            if (isNew)
                return string.Format("<{0} {3}{1}>{2}</{0}>", tagName, " insert=\"yes\"", value, attributes);
            else
                return string.Format("<{0} {3}{1}>{2}</{0}>", tagName, (updateAttribute ? " update=\"yes\"" : string.Empty), value, attributes);
        }

        #endregion

        #region Misc Methods

        /// <summary>
        /// Checks if the given value is a encoded string or not 
        /// </summary>
        /// <param name="value">Value to evaluate</param>
        /// <returns>Boolean indicating the result of the comparison</returns>
        private bool IsEncoded(string value)
        {
            bool retVal = false;
            string notEncodedKey = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAA";
            if (!string.IsNullOrEmpty(value) && value.Length >= notEncodedKey.Length)
            {
                if (!value.Substring(0, notEncodedKey.Length).Equals(notEncodedKey))
                    retVal = true;
            }

            return retVal;
        }
        #endregion
    }
}
