using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;


namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
    public class Structure : BusinessBase<Structure>
    {
        #region Business Methods

        private int _id;
        private string _type;
        private string _value;
        private double _molWeight;
        private string _formula;
        private string _normalizedStructure;
        private string _normalizationLog;
        private bool _isTemporary = true;
        private bool _isRegistering = false;
        private bool _useNormalizedStructure = true;
        private List<string> _changedProperties = new List<string>();
        private string _validationRulesXml;

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

        public bool IsTemplate
        {
            get
            {
                CanReadProperty(true);
                return this.ID < 0;
            }
            /*set {
                CanWriteProperty(true);

                if (value)
                {
                    this.ID = -1;
                    PropertyHasChanged();
                }
                else
                    if (this.ID == -1)
                    {
                        this.ID = 0;
                        PropertyHasChanged();
                    }
            }*/
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        public string Type
        {
            get
            {
                CanReadProperty(true);
                return _type;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if (_type != value)
                {
                    _type = value;
                    PropertyHasChanged();
                }
            }
        }

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
                if (value == null) value = string.Empty;
                if (value != "null" && _value != value)
                {
                    if (this.IsEncoded(value) && !this.IsAllowedEncodedValues)
                        throw new ValidationException("The application doesn't support encoded values. Please check your ChemDraw version.");
                    else
                    {
                        _value = value;
                        _normalizedStructure = _value;
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
            get { return false; }
        }


        public double MolWeight
        {
            get
            {
                CanReadProperty(true);
                return this._molWeight;
            }
        }

        public string Formula
        {
            get
            {
                CanReadProperty(true);
                return this._formula;
            }
        }

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

        public bool IsBeingRegistered
        {
            set
            {
                _isRegistering = value;
            }
        }

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

        public override bool IsValid
        {
            //get { return base.IsValid && _resources.IsValid; }
            get
            {
                this.ValidationRules.CheckRules();
                return base.IsValid;
            }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty; }
            //get { return base.IsDirty || _resources.IsDirty; }
        }

        internal void MarkClean()
        {
            base.MarkClean();
            this._changedProperties.Clear();
        }
        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "Value")
            {
                if (this.IsTemplate)
                    this.MarkNew();
            }

            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

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
        #endregion

        #region Validation Rules

        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();

            if (!string.IsNullOrEmpty(_validationRulesXml))
                ValidationRulesFactory.GetInstance().AddInstanceRules(this.ValidationRules, "Value", _validationRulesXml);
        }

        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
            {
                brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
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

        public static Structure NewStructure()
        {
            Structure structure = new Structure();
            return structure;
        }

        public static Structure NewStructure(string xml, bool isNew, bool isClean)
        {
            return new Structure(xml, isNew, isClean);
        }

		public static Structure NewStructure(string xml)
		{
			return new Structure(xml);
		}

        public static Structure NewStructure(string value, double mw, string formula)
        {
            return new Structure(value, mw, formula);
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

		private Structure(string xml)
		{
			SetStructurePropreties(xml);
		}

        private Structure(string xml, bool isNew, bool isClean)
        {
			SetStructurePropreties(xml);

            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();
        }


		//extracting the basic processing to a new private method to be shared 
		private void SetStructurePropreties (string xml)
		{
			XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Structure/StructureFormat");
            if (xIterator.MoveNext())
                _type = xIterator.Current.Value;

            xIterator = xNavigator.Select("Structure/StructureID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Structure/Structure");
            if (xIterator.MoveNext())
            {
                _value = xIterator.Current.Value;
                _formula = xIterator.Current.GetAttribute("formula", xIterator.Current.NamespaceURI);

                if (xIterator.Current.GetAttribute("molWeight", xIterator.Current.NamespaceURI) != string.Empty)
                    _molWeight = double.Parse(xIterator.Current.GetAttribute("molWeight", xIterator.Current.NamespaceURI));

                xIterator = xNavigator.Select("Structure/Structure/validationRuleList");
                if (xIterator.MoveNext())
                    _validationRulesXml = xIterator.Current.OuterXml;

                xIterator = xNavigator.Select("Structure/NormalizedStructure");
                if (xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.Value))
                    _normalizedStructure = xIterator.Current.Value;
            }

			AddInstanceBusinessRules();
		}



        private Structure(string value, double mw, string formula)
        {
            _value = value;
            _molWeight = mw;
            _formula = formula;
        }

        public static Structure GetStructure(int id)
        {
            if (!CanGetObject())
            {
                throw new System.Security.SecurityException("User not authorized to view a Structure");
            }
            return DataPortal.Fetch<Structure>(new Criteria(id));
        }

        protected static void DeleteStructure(int id)
        {
            if (!CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Structure");
            }
            DataPortal.Delete(new Criteria(id));
        }

        public override Structure Save()
        {
            if (IsDeleted && !CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Structure");
            }
            else if (IsNew && !CanAddObject())
            {
                throw new System.Security.SecurityException("User not authorized to add a Structure");
            }
            else if (!CanEditObject())
            {
                throw new System.Security.SecurityException("User not authorized to update a Structure");
            }
            return base.Save();
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
            _id = 1;
            ValidationRules.CheckRules();
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
        }

        private void Fetch(SafeDataReader reader)
        {
            if (reader.Read())
            {
                this._id = reader.GetInt32("ID");
                this._type = "Base64_cdx";
                this._value = reader.GetString("BASE64_CDX");
            }
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
        }

        //private void Insert(OracleCommand cmd)
        //{

        //}

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {

        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_id));
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        private void DataPortal_Delete(Criteria criteria)
        {

        }

        #endregion

        #region Xml

        internal string UpdateSelf()
        {
            /*if(this._changedProperties.Contains("Value"))
                NormalizeStructure();*/

            StringBuilder builder = new StringBuilder("");

            builder.Append(string.Format("<Structure{0}>", this.IsNew ? " insert=\"yes\"" : string.Empty));

            builder.Append(createTag("StructureID", string.Empty, this._id.ToString(), this._changedProperties.Contains("ID")));
            builder.Append(createTag("StructureFormat", string.Empty, this._type == null ? string.Empty : this._type, this._changedProperties.Contains("Type")));

            string structureAttributes = string.Empty;
            if (!string.IsNullOrEmpty(this.Formula))
                structureAttributes += string.Format("formula=\"{0}\" ", this.Formula);

            if (this.MolWeight >= 0.0)
                structureAttributes += string.Format("molWeight=\"{0}\" ", this.MolWeight);

            if (_isTemporary && !_isRegistering)
            {
                builder.Append(createTag("Structure", structureAttributes, this._value, this.IsNew,
                                                                                    this._changedProperties.Contains("Value") ||
                                                                                    this._changedProperties.Contains("Formula") ||
                                                                                    this._changedProperties.Contains("MolWeight")));



                //builder.Append("<NormalizedStructure>" + this._normalizedStructure + "</NormalizedStructure>");
                builder.Append(createTag("NormalizedStructure", null, this._normalizedStructure, this._changedProperties.Contains("NormalizedStructure")));
                builder.Append("</Structure>");
            }
            else
            {
                if (_useNormalizedStructure && !string.IsNullOrEmpty(_normalizedStructure) && _isRegistering)
                {
                    //this._normalizedStructure = NormalizeStructure();

                    builder.Append(createTag("Structure", structureAttributes, this._normalizedStructure, this.IsNew,
                                                                                    this._changedProperties.Contains("Value") ||
                                                                                    this._changedProperties.Contains("Formula") ||
                                                                                    this._changedProperties.Contains("MolWeight")));

                }
                else
                {
                    builder.Append(createTag("Structure", structureAttributes, this._value, this.IsNew,
                                                                                    this._changedProperties.Contains("Value") ||
                                                                                    this._changedProperties.Contains("Formula") ||
                                                                                    this._changedProperties.Contains("MolWeight")));
                }
                builder.Append("</Structure>");
            }
            return builder.ToString();
        }

        internal string createTag(string tagName, string attributes, string value, bool updateAttribute)
        {
            if (attributes == null)
                attributes = string.Empty;

            //please note the space preceeding update="yes"
            return string.Format("<{0} {3}{1}>{2}</{0}>", tagName, (updateAttribute ? " update=\"yes\"" : string.Empty), value, attributes);
        }

        internal string createTag(string tagName, string attributes, string value, bool isNew, bool updateAttribute)
        {
            if (attributes == null)
                attributes = string.Empty;

            if (IsNew)
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
            if (!value.Substring(0, notEncodedKey.Length).Equals(notEncodedKey))
                retVal = true;
            return retVal;
        }

        #endregion
    }
}
