using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class Compound : BusinessBase<Compound>
    {
        #region Business Methods

        [COEUserActionDescription("GetCompoundBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
                {
                    brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
                }

                this._baseFragment.GetBrokenRulesDescription(brokenRules);
                this._identifierList.GetBrokenRulesDescription(brokenRules);
                this._propertyList.GetBrokenRulesDescription(brokenRules);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

        /// <summary>
        /// Given the Identifier Id and a value, adds a new Identifier object to the Compound's
        /// IdentifierList.
        /// </summary>
        /// <param name="identifierId">the internal code for the identifier</param>
        /// <param name="identifierValue">the value to apply to the new Identifier instance</param>
        public void AddIdentifier(int identifierId, string identifierValue)
        {
            try
            {
                RegSvcUtilities.CreateNewIdentifier(
                    identifierId
                    , identifierValue
                    , this.IdentifierList
                    , IdentifierTypeEnum.C);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given an Identifier name and a value, adds a new Identifier object to the Compound's
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
                    , IdentifierTypeEnum.C);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given an Identifier instance, adds the Identifier to the Compound's IdentifierList.
        /// </summary>
        /// <param name="identifier">an Identifier instance</param>
        public void AddIdentifier(Identifier identifier)
        {
            try
            {
                RegSvcUtilities.CreateNewIdentifier(
                    this.IdentifierList
                    , identifier
                    , IdentifierTypeEnum.C);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.RegNumber.RegNum))
                return "Compound " + this.RegNumber.RegNum;
            else if (this.ID > 0)
                return "Compound " + this.ID;

            return "New Compound";
        }

        #endregion

        #region Business Properties

        private CompoundFragmentList _compoundFragmentList;
        public CompoundFragmentList CompoundFragmentList
        {
            get
            {
                CanReadProperty(true);
                return _compoundFragmentList;
            }
            set
            {
                CanWriteProperty(true);
                if (_compoundFragmentList != value)
                {
                    _compoundFragmentList = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _id;
        private DateTime _dateCreated;
        private DateTime _dateLastModified;
        private int _personCreated;
        private int _personRegistered;
        private int _personApproved;
        private IdentifierList _identifierList;
        private BaseFragment _baseFragment;
        private FragmentList _fragmentList;
        private PropertyList _propertyList;
        private ProjectList _projectList;
        private RegNumber _regNumber;
        private List<string> _changedProperties = new List<string>();
        private int _registeredCompoundID;
        private int _singleCompoundCount;
        private String _tag;
        private bool _canPropogateComponentEdits = true;
        
        public int SingleCompoundCount
        {
            get
            {
                CanReadProperty(true);
                return _singleCompoundCount;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if (_singleCompoundCount != value)
                {
                    _singleCompoundCount = value;
                    PropertyHasChanged();
                }
            }
        }

        public FragmentList FragmentList
        {
            get
            {
                CanReadProperty(true);
                return _fragmentList;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if(_fragmentList != value)
                {
                    _fragmentList = value;
                    PropertyHasChanged();
                }
            }
        }
        public PropertyList PropertyList
        {
            get
            {
                CanReadProperty(true);
                if(_propertyList == null)
                    _propertyList = PropertyList.NewPropertyList();

                return _propertyList;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if(_propertyList != value)
                {
                    _propertyList = value;
                    PropertyHasChanged();
                }
            }
        }

        public ProjectList ProjectsList
        {
            get
            {
                CanReadProperty(true);
                return _projectList;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if(_projectList != value)
                {
                    _projectList = value;
                    PropertyHasChanged();
                }
            }
        }

        public RegNumber RegNumber
        {
            get
            {
                CanReadProperty(true);
                return _regNumber;
            }
            set {
                CanWriteProperty(true);
                if (!this.RegNumber.Equals(value))
                {
                    this._regNumber = value;
                    PropertyHasChanged();
                }
            }
        }

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
                CanWriteProperty(true);
                if (_id != value)
                {
                    _id = value;
                    //PropertyHasChanged();
                }
            }
        }

        public DateTime DateCreated
        {
            get
            {
                CanReadProperty(true);
                return _dateCreated;
            }
            set
            {
                CanWriteProperty(true);
                if (_dateCreated != value)
                {
                    _dateCreated = value;
                    PropertyHasChanged();
                }
            }
        }
        
        public DateTime DateLastModified
        {
            get
            {
                CanReadProperty(true);
                return _dateLastModified;
            }
            set
            {
                CanWriteProperty(true);
                if (_dateLastModified != value)
                {
                    _dateLastModified = value;
                    PropertyHasChanged();
                }
            }
        }

        public int PersonCreated
        {
            get
            {
                CanReadProperty(true);
                return _personCreated;
            }
            set
            {
                CanWriteProperty(true);
                if (_personCreated != value)
                {
                    _personCreated = value;
                    PropertyHasChanged();
                }
            }
        }

        public int PersonRegistered {
            get
            {
                CanReadProperty(true);
                return _personRegistered;
            }
            set
            {
                CanWriteProperty(true);
                if(_personRegistered != value)
                {
                    _personRegistered = value;
                    PropertyHasChanged();
                }
            }
        }
        public int PersonApproved
        {
            get
            {
                CanReadProperty(true);
                return _personApproved;
            }
            set
            {
                CanWriteProperty(true);
                if (_personApproved != value)
                {
                    _personApproved = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool CanPropogateComponentEdits
        {
            get { return _canPropogateComponentEdits; }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

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
                if(_identifierList != value)
                {
                    _identifierList = value;
                    PropertyHasChanged();
                }
            }
        }

        public BaseFragment BaseFragment
        {
            get
            {
                return _baseFragment;
            }
            set
            {
                CanWriteProperty(true);
                //if (value == null) value = string.Empty;
                if (_baseFragment != value)
                {
                    _baseFragment = value;
                    PropertyHasChanged();
                }
            }
        }

        public override bool IsValid
        {
            get { return
                base.IsValid 
                && _baseFragment.IsValid
                && _propertyList.IsValid;
            }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty || _propertyList.IsDirty || (_projectList == null ? false : _projectList.IsDirty) || _baseFragment.IsDirty; }
            //get { return base.IsDirty || _resources.IsDirty; }
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
                return ((CompoundList)base.Parent).GetIndex(this);
            }
        }

        /// <summary>
        /// Identifier for a component object. 
        /// The ID is not enough when you are creating a new component and the ID is null.
        /// </summary>
        [Browsable(false)]
        public string UniqueID
        {
            get
            {
                return _id.ToString() + "|" + _regNumber.ToString();
            }
        }

        [Browsable(true)]
        public String Tag
        {
            get
            {
                CanReadProperty(true);
                return _tag;
            }
            set
            {
                CanWriteProperty(true);
                if (_tag != value)
                {
                    _tag = value;
                    PropertyHasChanged();
                }
            }
        }

        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
        }

        #endregion

        #region Authorization Rules

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "BaseFragement", "xxx");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("xxx");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return Csla.ApplicationContext.User.IsInRole("xxx");
        }

        public static bool CanEditObject()
        {
            return Csla.ApplicationContext.User.IsInRole("xxx");
        }

        #endregion

        #region Factory Methods

        private Compound() {
            _id = -1;
            _registeredCompoundID = -1;
        }
 
        private Compound(string xml, bool isNew, bool isClean) {

            //XmlTextReader xReader = new XmlTextReader(xml);
            //XPathDocument xDocument = new XPathDocument(xReader);
            //XPathNavigator xNavigator = xDocument.CreateNavigator();

            //if (this._dateCreated.CompareTo(DateTime.MinValue) == 0)
            //    this._dateCreated = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            //if (this._dateLastModified.CompareTo(DateTime.MinValue) == 0)
            //    this._dateLastModified = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();


            XPathNodeIterator xIterator = xNavigator.Select("Compound/CompoundID");
            if(xIterator.MoveNext())
                if(!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._id = int.Parse(xIterator.Current.Value);
                else
                    _id = -1;

            /*xIterator = xNavigator.Select("Compound/CompoundIndex");
            if(xIterator.MoveNext())
                if(!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._compoundIndex = int.Parse(xIterator.Current.Value);
                else
                    _compoundIndex = -1;
            */
            xIterator = xNavigator.Select("Compound/RegisteredCompoundID");
            if(xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.Value))
                this._registeredCompoundID = int.Parse(xIterator.Current.Value);
            else
                _registeredCompoundID = -1;

            xIterator = xNavigator.Select("Compound/DateCreated");
            if(xIterator.MoveNext())
                if(!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._dateCreated = DateTime.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Compound/PersonCreated");
            if(xIterator.MoveNext())
                if(!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._personCreated = int.Parse(xIterator.Current.Value);
                else
                    this._personCreated = CambridgeSoft.COE.Framework.Common.COEUser.ID;

            xIterator = xNavigator.Select("Compound/PersonRegistered");
            if(xIterator.MoveNext()) {
                if(!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._personRegistered = int.Parse(xIterator.Current.Value);
                else
                    this._personRegistered = CambridgeSoft.COE.Framework.Common.COEUser.ID;
            } else {
                _personRegistered = CambridgeSoft.COE.Framework.Common.COEUser.ID;
            }

            xIterator = xNavigator.Select("Compound/PersonApproved");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._personApproved = int.Parse(xIterator.Current.Value);
                else
                    this._personApproved = CambridgeSoft.COE.Framework.Common.COEUser.ID;
            }
           
            

            xIterator = xNavigator.Select("Compound/DateLastModified");
            if(xIterator.MoveNext())
                if(!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._dateLastModified = DateTime.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Compound/Tag");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _tag = xIterator.Current.Value.ToString();

            xIterator = xNavigator.Select("Compound/IdentifierList");
            if (xIterator.MoveNext())
                this._identifierList = IdentifierList.NewIdentifierList(xIterator.Current.OuterXml, isNew, isClean);
            else
                this._identifierList = IdentifierList.NewIdentifierList();

            xIterator = xNavigator.Select("Compound/BaseFragment");
            xIterator.MoveNext();
            this._baseFragment = BaseFragment.NewBaseFragment(xIterator.Current.OuterXml, isNew, isClean);

            xIterator = xNavigator.Select("Compound/FragmentList");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                {
                    this._fragmentList = FragmentList.NewFragmentList(xIterator.Current.OuterXml, isNew, isClean);
                    this._compoundFragmentList = CompoundFragmentList.NewCompoundFragmentList(xIterator.Current.OuterXml, isNew, isClean);
                }
                else
                {
                    this._fragmentList = FragmentList.NewFragmentList();
                    this._compoundFragmentList = CompoundFragmentList.NewCompoundFragmentList();
                }
            }
            else
            {
                this._fragmentList = FragmentList.NewFragmentList();
                this._compoundFragmentList = CompoundFragmentList.NewCompoundFragmentList();
            }

            xIterator = xNavigator.Select("Compound/PropertyList");
            if(xIterator.MoveNext()) {
                if(!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                    this._propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, isClean);
            } else {
                _propertyList = PropertyList.NewPropertyList();
            }

                xIterator = xNavigator.Select("Compound/ProjectList");
            if(xIterator.MoveNext())
                this._projectList = ProjectList.NewProjectList(xIterator.Current.OuterXml, isClean,isNew);

            xIterator = xNavigator.Select("Compound/RegNumber");
            if(xIterator.MoveNext())
                this._regNumber = RegNumber.NewRegNumber(xIterator.Current.OuterXml, isClean);
            else
                _regNumber = RegNumber.NewRegNumber();

            xIterator = xNavigator.Select("Compound/CanPropogateComponentEdits");
            if(xIterator.MoveNext())
            {
                try { _canPropogateComponentEdits = Boolean.Parse(xIterator.Current.Value); }
                catch { _canPropogateComponentEdits = true; }
            }

            if (!isNew)
                MarkOld();
            else {
                MarkNew();
                this.PersonCreated = CambridgeSoft.COE.Framework.Common.COEUser.ID;
            }

            if (isClean)
                MarkClean();
        }

        [COEUserActionDescription("CreateCompound")]
        public static Compound NewCompound()
        {
            try
            {
                if (!CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a Compound");
                }
                return DataPortal.Create<Compound>();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateCompound")]
        public static Compound NewCompound(string xml, bool isNew, bool isClean)
        {
            try
            {
                return new Compound(xml, isNew, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetCompound")]
        public static Compound GetCompound(int id)
        {
            try
            {
                //if (!CanGetObject())
                //{
                //    throw new System.Security.SecurityException("User not authorized to view a Compound");
                //}
                return DataPortal.Fetch<Compound>(new Criteria(id));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        protected static void DeleteCompound(int id)
        {
            if (!CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Compound");
            }
            DataPortal.Delete(new Criteria(id));
        }

        [COEUserActionDescription("SaveCompound")]
        public override Compound Save()
        {
            try
            {
                if (IsDeleted && !CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a Compound");
                }
                else if (IsNew && !CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a Compound");
                }
                else if (!CanEditObject())
                {
                    throw new System.Security.SecurityException("User not authorized to update a Compound");
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
            ValidationRules.CheckRules();
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
        }
        
        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        private void DataPortal_Delete(Criteria criteria)
        {
        }

        #endregion

        #region XML

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<Compound");
            if (addCRUDattributes && this.IsNew)
                builder.Append(" insert=\"yes\"");
            builder.Append(">");

            builder.Append("<CompoundID");
            if (addCRUDattributes && _changedProperties.Contains("CompoundID") && !this.IsNew)
            {
                builder.Append(" update=\"yes\"");
            }
            else{ 
                if (addCRUDattributes && this.IsNew)
                {            
                    builder.Append(" insert=\"yes\"");
                }
             }
            builder.Append(">" + this._id + "</CompoundID>");

            if(_registeredCompoundID >= 0) {
                builder.Append("<RegisteredCompoundID");
                if (addCRUDattributes && _changedProperties.Contains("RegisteredCompoundID"))
                    builder.Append(" update=\"yes\"");
                builder.Append(">" + this._registeredCompoundID + "</RegisteredCompoundID>");
            }

           /* if(_compoundIndex >= 0) {
                builder.Append("<CompoundIndex");
                if(_changedProperties.Contains("CompoundIndex"))
                    builder.Append(" update=\"yes\"");
                builder.Append(">" + this._compoundIndex + "</CompoundIndex>");
            }*/

            builder.Append("<DateCreated");
            if (addCRUDattributes && _changedProperties.Contains("DateCreated"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</DateCreated>", this._dateCreated.ToString(Constants.DATE_FORMAT));

            builder.Append("<DateLastModified");
            if (addCRUDattributes && _changedProperties.Contains("DateLastModified"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</DateLastModified>", this._dateLastModified.ToString(Constants.DATE_FORMAT));

            builder.Append("<PersonCreated");
            if (addCRUDattributes && _changedProperties.Contains("PersonCreated"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonCreated>", _personCreated > 0 ? _personCreated.ToString() : string.Empty);

            builder.Append("<Tag");
            if (addCRUDattributes && _changedProperties.Contains("Tag"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</Tag>", _tag);

            builder.Append("<PersonRegistered");
            if (addCRUDattributes && _changedProperties.Contains("PersonRegistered"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonRegistered>", _personRegistered > 0 ? _personRegistered.ToString() : string.Empty);

            builder.Append("<PersonApproved");
            if (addCRUDattributes && _changedProperties.Contains("PersonApproved"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonApproved>", _personApproved > 0 ? _personApproved.ToString() : string.Empty);

          
            if (_regNumber != null)
                builder.Append(this._regNumber.UpdateSelf(addCRUDattributes));

            if (_baseFragment != null)
                builder.Append(this._baseFragment.UpdateSelf(addCRUDattributes));

            if (_fragmentList != null)
                builder.Append(this._fragmentList.UpdateSelf(addCRUDattributes));

            if (_propertyList != null)
                builder.Append(this._propertyList.UpdateSelf(addCRUDattributes));

            if (_projectList != null && _projectList.Count > 0)
                builder.Append(this._projectList.UpdateSelf(addCRUDattributes));

            if (_identifierList != null)
                builder.Append(this._identifierList.UpdateSelf(addCRUDattributes));

            builder.Append("</Compound>");

            //MarkOld();

            return builder.ToString();
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            //Validation 
            //1.check RegNumber.
            XmlNode regNumberNode = parentNode.SelectSingleNode("//Compound/RegNumber/RegNumber");
            if (!(string.Compare(regNumberNode.InnerText, this.RegNumber.RegNum, true) == 0))
                throw new Exception("Invalid compoundID.");

            //Update

            //1.propertylist
            XmlNode propertyListNode = parentNode.SelectSingleNode("PropertyList");
            this.PropertyList.UpdateFromXml(propertyListNode);

            //2.BaseFragment
            XmlNode baseFragmentNode = parentNode.SelectSingleNode("BaseFragment");
            this.BaseFragment.UpdateFromXml(baseFragmentNode);

            //3.IdentifierList
            XmlNode identifierListNode = parentNode.SelectSingleNode("IdentifierList");
            this.IdentifierList.UpdateFromXml(identifierListNode);

            //4.FragmentList: haven't been implemented.
        }

        internal void UpdateUserPreference(XmlNode parentNode)
        {
            //Validation 
            //1.check RegNumber.
            //XmlNode regNumberNode = parentNode.SelectSingleNode("//Compound/RegNumber/RegNumber");
            //if (!(string.Compare(regNumberNode.InnerText, this.RegNumber.RegNum, true) == 0))
            //    throw new Exception("Invalid compoundID.");

            //Update

            //1.propertylist
            XmlNode propertyListNode = parentNode.SelectSingleNode("PropertyList");
            this.PropertyList.UpdateUserPreference(propertyListNode);

            ////2.BaseFragment
            XmlNode baseFragmentNode = parentNode.SelectSingleNode("BaseFragment");
            this.BaseFragment.UpdateUserPreference(baseFragmentNode);

            //3.IdentifierList
            XmlNode identifierListNode = parentNode.SelectSingleNode("IdentifierList");
            this.IdentifierList.UpdateUserPreference(identifierListNode);

            //4.FragmentList: haven't been implemented.
        }
        #endregion

     }
}
