using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Properties;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    //updated on 2008/03/31 for the section of  BatchComponentFragmentList
    [Serializable()]
    public class BatchComponent : RegistrationBusinessBase<BatchComponent>
    {

        private int _batchID;
        private int _compoundID;
        private PropertyList _propertyList;
        private int _componentIndex;
        private List<string> _changedProperties = new List<string>();
        private DataTable _bindablePropertiesDT;
        private string _regNum;
        private string _mixtureComponentId;
        private BatchComponentFragmentList _batchComponentFragmentList;
        private bool _isRegistering = false;
        
        /// <summary>
        /// Identifier for a batchcomponent object. 
        /// The ID is not enough when you are creating a new component and the ID is null.
        /// </summary>
        [Browsable(false)]
        public string UniqueID
        {
            get
            {
                return this.ID.ToString() + "|" + _componentIndex.ToString() + "|" + _batchID;
            }
        }

        /// <summary>
        /// It is of the utmost importance that "GetIdValue" return a unique Identifier value.
        /// Otherwise the "equals" method doesn't work and thus some BatchComponentList methods
        /// (which require usage of "equals") don't work neither, like Remove(BatchComponent object)
        /// , Contains(BatchComponent object), etc.
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return ID == 0 ? UniqueID : (object)ID;
        }

        public bool IsBeingRegistered
        {
            set
            {
                _isRegistering = value;
            }
            get {
                return _isRegistering;
            }
        }
        
        public int BatchID
        {
            get
            {
                CanReadProperty(true);
                return _batchID;
            }
            set
            {
                CanWriteProperty(true);
                if (_batchID != value)
                    _batchID = value;
            }
        }

        public int CompoundID
        {
            get
            {
                CanReadProperty(true);
                return _compoundID;
            }
            set
            {
                CanWriteProperty(true);
                if (_compoundID != value)
                {
                    _compoundID = value;
                    PropertyHasChanged();
                }
            }
        }

        public string DisplayKey
        {
            get
            {
                CanReadProperty(true);

                if (!string.IsNullOrEmpty(_regNum))
                    return _regNum;

                if (_compoundID > 0)
                    return string.Format("{0:000}", _compoundID);

                return this.OrderIndex.ToString();
            }
        }

        public string RegNum
        {
            get
            {
                CanReadProperty(true);
                return _regNum;
            }
            set
            {
                CanWriteProperty(true);
                if (_regNum != value)
                {
                    _regNum = value;
                }
            }
        }

        public PropertyList PropertyList
        {
            get
            {
                CanReadProperty(true);
                if (_propertyList == null)
                    _propertyList = PropertyList.NewPropertyList();

                return _propertyList;
            }
            set
            {
                CanWriteProperty(true);
                if (_propertyList != value)
                    _propertyList = value;
            }
        }

        public BatchComponentFragmentList BatchComponentFragmentList
        {
            get
            {
                CanReadProperty(true);
                if (_batchComponentFragmentList == null)
                    _batchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList();

                return _batchComponentFragmentList;
            }
            set
            {
                CanWriteProperty(true);
                if (_batchComponentFragmentList != value)
                {
                    _batchComponentFragmentList = value;
                    PropertyHasChanged();
                }
            }
        }

        public int ComponentIndex
        {
            get
            {
                CanReadProperty(true);
                return _componentIndex;
            }
            set
            {
                CanWriteProperty(true);
                if (_componentIndex != value)
                {
                    _componentIndex = value;
                    PropertyHasChanged();
                }
            }
        }

        public DataTable BindeablePropertiesDT
        {
            get
            {
                CanReadProperty(true);
                if (_bindablePropertiesDT == null)
                    SetPropertiesDataTable();
                return _bindablePropertiesDT; 
            }
        }

        /// <summary>
        /// Index of the current object in the collection that belongs.
        /// </summary>
        [Browsable(true)]
        public int OrderIndex
        {
            get
            {
                CanReadProperty(true);
                return ((BatchComponentList)base.Parent).GetIndex(this);
            }
        }

        public override bool IsValid
        {
            get
            {
                if (RegSvcUtilities.GetComponentCount()>1)
                    return base.IsValid && _propertyList.IsValid && this.AreValidPercentages();
                else
                    return true;
            }
        }

        

        public override bool IsDirty
        {
            get { return base.IsDirty || this.PropertyList.IsDirty; }
        }

        internal new void MarkNew()
        {
            this.ID = 0;

            this.BatchComponentFragmentList.MarkNew();

            base.MarkNew();
        }

        /// <summary>
        /// Hardcoded percentage format validation. Should be removed when missing server validations (regexp in this case) are implemented.
        /// </summary>
        /// <returns>wether the percentages is in correct format or not.</returns>
        private bool AreValidPercentages()
        {
            bool retVal = true;
            foreach (Property currentProperty in _propertyList)
            {
                if (currentProperty.Name.ToUpper() == "PERCENTAGE")
                    retVal &= ValidatePercentage(currentProperty.Value);
            }
            return retVal;
        }

        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
            {
                brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
            }
            this._propertyList.GetBrokenRulesDescription(brokenRules);
        }
        
        private BatchComponent()
        { 
            this.MarkAsChild();

            _mixtureComponentId = string.Empty;
        }

        private BatchComponent(string xml, bool isNew, bool isClean) : this() {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            //Bind ID value from xml. 
            XPathNodeIterator xIterator = xNavigator.Select("BatchComponent/ID");
            xIterator.MoveNext();
            int localId;
            int.TryParse(xIterator.Current.Value, out localId);
            this.ID = localId;

            //Bind ComponentIndex value from xml
            xIterator = xNavigator.Select("BatchComponent/ComponentIndex");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._componentIndex = int.Parse(xIterator.Current.Value);

            //Bind CompoundID value from xml.
            xIterator = xNavigator.Select("BatchComponent/CompoundID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._compoundID = int.Parse(xIterator.Current.Value);

            //Bind BatchID value from xml.
            xIterator = xNavigator.Select("BatchComponent/BatchID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._batchID = int.Parse(xIterator.Current.Value);

            //Bind BatchID value from xml.
            xIterator = xNavigator.Select("BatchComponent/MixtureComponentID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value.Trim()))
                    this._mixtureComponentId = xIterator.Current.Value.Trim();
            //Bind ComponentRegNum value from xml.
            xIterator = xNavigator.Select("BatchComponent/ComponentRegNum");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value.Trim()))
                    this.RegNum = xIterator.Current.Value.Trim();

            //Bind PropertyList value from Xml.
            xIterator = xNavigator.Select("BatchComponent/PropertyList");
            if(xIterator.MoveNext()) {
                if(!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                    this._propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, isClean);
            } else {
                _propertyList = PropertyList.NewPropertyList();
            }

            xIterator = xNavigator.Select("BatchComponent/BatchComponentFragmentList");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                    _batchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(xIterator.Current.OuterXml, isNew, isClean);
        }

        public static BatchComponent NewBatchComponent(string xml, bool isNew, bool isClean)
        {
            BatchComponent batchComponent = new BatchComponent(xml, isNew, isClean);

            if (isNew)
                batchComponent.MarkNew();
            else
                batchComponent.MarkOld();

            if (isClean)
                batchComponent.MarkClean();
            else
                batchComponent.MarkDirty();

            return batchComponent;
        }

        public static BatchComponent NewBatchComponent()
        {
            using(RegistryRecord record = RegistryRecord.NewRegistryRecord())
            {
                return record.BatchList[0].BatchComponentList[0];
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            string state = string.Empty;
            if (addCRUDattributes && this.IsDeleted)
                state = "delete=\"yes\"";

            if (addCRUDattributes && this.IsNew)
                state = "insert=\"yes\"";

            builder.AppendFormat("<BatchComponent {0}>", state);
            builder.Append("<ID>" + this.ID + "</ID>");
            builder.Append("<BatchID>" + this._batchID + "</BatchID>");    

            builder.Append(string.Format("<MixtureComponentID>{0}</MixtureComponentID>", 
                !string.IsNullOrEmpty(_mixtureComponentId) ? _mixtureComponentId : "0"));

            builder.Append("<CompoundID");
            if (addCRUDattributes && !this.IsBeingRegistered && !this.IsNew && _changedProperties.Contains("CompoundID"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._compoundID + "</CompoundID>");
            builder.Append("<ComponentIndex");
            if (addCRUDattributes && !this.IsBeingRegistered &&  !this.IsNew && _changedProperties.Contains("ComponentIndex"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._componentIndex + "</ComponentIndex>");
            builder.Append("<OrderIndex>" + this.OrderIndex.ToString() + "</OrderIndex>");
            builder.Append(this.PropertyList.UpdateSelf(addCRUDattributes));

            builder.Append(this.BatchComponentFragmentList.UpdateSelf(addCRUDattributes && !IsDeleted && !IsNew));
            builder.Append("</BatchComponent>");
            return builder.ToString();
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            XmlNode propertyListNode = parentNode.SelectSingleNode("//BatchComponent/PropertyList");
            this.PropertyList.UpdateFromXml(propertyListNode);
            XmlNode batchcomponentFragmentListNode = parentNode.SelectSingleNode("//BatchComponent/BatchComponentFragmentList");
            this.BatchComponentFragmentList.UpdateFromXml(batchcomponentFragmentListNode);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                base.OnPropertyChanged(propertyName);
                if (!_changedProperties.Contains(propertyName))
                    _changedProperties.Add(propertyName);
            }
        }

        /// <summary>
        /// Method to populate the table to display the Property Lists.
        /// </summary>
        private void SetPropertiesDataTable()
        {
            _bindablePropertiesDT = new DataTable();
            foreach (Property currentProperty in _propertyList)
            {
                DataColumn currentDataColumn = new DataColumn(currentProperty.Name);
                _bindablePropertiesDT.Columns.Add(currentDataColumn);
            }
            DataRow currentDataRow = _bindablePropertiesDT.NewRow();
            foreach (Property currentProperty in _propertyList)
            {
                currentDataRow[currentProperty.Name] = currentProperty.Value;
            }
            _bindablePropertiesDT.Rows.Add(currentDataRow);
        }

        private bool ValidatePercentage(string textToTest)
        {
            //double percentage;
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            Regex regEx = new Regex(@"^\d{0,3}([" + decimalSeparator + @"]\d{1,2})?$");

            //if (_value.Length > 5 || !double.TryParse(_value, out percentage))
            if (!regEx.IsMatch(textToTest))
                throw new ValidationException("Invalid Percentage: Format '###" + decimalSeparator + "##'");

            //Check that is greater or equal to 0
            double percentage = 0;
            if(double.TryParse(textToTest,out percentage))
            {
                if(percentage < 0 || percentage > 100)
                    throw new ValidationException(Resources.PercentageInvalidNumericValue);
            }
            return true;
        }
    }
}
