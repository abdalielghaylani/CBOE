using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Domain object used to hold information about an 'instance' of a compound.
    /// </summary>
    [Serializable()]
    public class Batch : RegistrationBusinessBase<Batch>
    {

        #region [ Factory Methods ]


        public static Batch NewBatch(string xml, bool isNew, bool isClean, bool sbi, string regNum)
        {
            return new Batch(xml, isNew, isClean, true, sbi, regNum);
        }

        public static Batch NewBatch(string xml, bool isNew, bool isClean)
        {
            return new Batch(xml, isNew, isClean, true);
        }

        public static Batch NewBatch(string xml, bool isNew, bool isClean, bool sbi, string regNum, RLSStatus rlsStatus)
        {
            return new Batch(xml, isNew, isClean, true, sbi, regNum, rlsStatus);
        }

        private Batch()
        {
            return;
        }

        private Batch(string xml, bool isNew, bool isClean, bool isChild)
            : this()
        {
            if (isChild)
            {
                MarkAsChild();
            }

            this.InitializeFromXml(xml, isNew, isClean);

        }

        private Batch(string xml, bool isNew, bool isClean, bool isChild, bool sbi, string regNum)
            : this(xml, isNew, isClean, isChild)
        {
            this._sbi = sbi;
            //this._regNumber = regNum;
        }

        private Batch(string xml, bool isNew, bool isClean, bool isChild, bool sbi, string regNum, RLSStatus rlsStatus)
            : this(xml, isNew, isClean, isChild, sbi, regNum)
        {
            _rlsStatus = rlsStatus;
        }

        public static Batch GetBatch(int id)
        {
            return GetBatch(id, false);
        }

        public static Batch GetBatch(int id, bool bTemporal)
        {
            if (!CanGetObject())
            {
                throw new System.Security.SecurityException("User not authorized to view a Batch");
            }
            Batch result = null;
            try
            {
                if (bTemporal)
                {
                    result = DataPortal.Fetch<Batch>(new TemporalCriteria(id));
                }
                else
                {
                    result = DataPortal.Fetch<Batch>(new Criteria(id));
                }
                result.IsTemporal = bTemporal;
            }
            catch (DataPortalException)
            {
                result = null;
            }
            return result;
        }

        public static void DeleteBatch(int id)
        {
            if (!CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Batch");
            }
            DataPortal.Delete(new Criteria(id));
        }

        public override Batch Save()
        {
            if (IsDeleted && !CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Batch");
            }
            else if (IsNew && !CanAddObject())
            {
                throw new System.Security.SecurityException("User not authorized to add a Batch");
            }
            else if (!CanEditObject())
            {
                throw new System.Security.SecurityException("User not authorized to update a Batch");
            }
            return base.Save();
        }

        #endregion

        #region [ Authorization and Validation Rules ]

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "Batch", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("batch");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("batch");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("batch");
        }

        protected override void AddBusinessRules()
        {
            //ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            //ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
        }

        protected override void AddInstanceBusinessRules()
        {
            ValidationRules.AddInstanceRule(this.ValidateProjectListBasedOnRLSStatus, "ProjectList");
        }

        private bool ValidateProjectListBasedOnRLSStatus(object sender, RuleArgs args)
        {
            bool isValid = RLSStatus == RLSStatus.BatchLevelProjects ? (_projectList != null && _projectList.Count > 0) : true;

            if (!isValid)
            {
                throw new ValidationException("There has to be at least 1 Batch project when RLS is set to Batch-level");
            }

            return isValid;
        }

        #endregion

        #region [ Properties and members ]

        private string _xml;
        private List<string> _changedProperties = new List<string>();
        private bool _sbi;

        private BatchComponentList _batchComponentList;
        public BatchComponentList BatchComponentList
        {
            get
            {
                CanReadProperty(true);
                if (_batchComponentList == null)
                    _batchComponentList = BatchComponentList.NewBatchComponentList();

                return _batchComponentList;
            }
            set
            {
                CanWriteProperty(true);
                if (_batchComponentList != value)
                {
                    _batchComponentList = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _batchNumber;
        [System.ComponentModel.DataObjectField(true, true)]
        public int BatchNumber
        {
            get
            {
                CanReadProperty(true);
                return _batchNumber;
            }
            set
            {
                _batchNumber = value;
            }
        }

        private DateTime _dateCreated;
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

        private DateTime _dateLastModified;
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

        public string FullRegNumber
        {
            get
            {
                if (_sbi)
                    return this._regNumber /*+ "/" + this.OrderIndex*/;
                else
                    return this._regNumber;
            }
        }

        private IdentifierList _identifierList;
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

                if (_identifierList != value)
                {
                    _identifierList = value;
                    PropertyHasChanged();
                }
            }
        }

        private bool _isTemporal;
        public bool IsTemporal
        {
            get
            {
                CanReadProperty();
                return _isTemporal;
            }
            private set
            {
                _isTemporal = value;
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
                return ((BatchList)base.Parent).GetIndex(this);
            }
        }

        private int _personCreated;
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

        private int _personRegistered;
        public int PersonRegistered
        {
            get
            {
                CanReadProperty(true);
                return _personRegistered;
            }
            set
            {
                CanWriteProperty(true);
                if (_personRegistered != value)
                {
                    _personRegistered = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _personApproved;
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

        private ProjectList _projectList;
        public ProjectList ProjectList
        {
            get
            {
                CanReadProperty(true);
                if (_projectList == null)
                    _projectList = ProjectList.NewProjectList();

                return _projectList;
            }
        }

        private PropertyList _propertyList;
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

        private string _regNumber;
        public string RegNumber 
        {
            get { return _regNumber; }
        }

        private RegistryStatus _status = RegistryStatus.NotSet;
        public RegistryStatus Status
        {
            get
            {
                CanReadProperty(true);
                return _status;
            }
            set
            {
                CanWriteProperty(true);
                if (_status != value)
                {
                    _status = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _tempBatchID;
        public int TempBatchID
        {
            get
            {
                CanReadProperty(true);
                return _tempBatchID;
            }
            set
            {
                CanWriteProperty(true);
                if (_tempBatchID != value)
                    _tempBatchID = value;
            }
        }

        /// <summary>
        /// Identifier for a batch object. 
        /// The ID is not enough when you are creating a new component and the ID is null or 0 (temp).
        /// </summary>
        [Browsable(false)]
        public string UniqueID
        {
            get
            {
                return this.ID.ToString() + "|" + _batchNumber;
            }
        }

        /// <summary>
        /// Returns true if any of this object's properties have been changed, or any of the child objects'
        /// properties have been changed.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty
                    || (_propertyList == null ? false : _propertyList.IsDirty)
                    || (_batchComponentList == null ? false : _batchComponentList.IsDirty)
                    || (_projectList == null ? false : _projectList.IsDirty);
            }
        }

        /// <summary>
        /// Returns false if any of this object is not valid, or any of the child objects are not valid.
        /// </summary>
        /// <remarks>
        /// The object must have a non-null BatchComponentList, but the remaining child lists can either be empty
        /// or null.
        /// </remarks>
        public override bool IsValid
        {
            get
            {
                ValidationRules.CheckRules();
                return base.IsValid
                    && (_propertyList == null ? true : _propertyList.IsValid)
                    && (_batchComponentList == null ? false : _batchComponentList.IsValid)
                    && (_projectList == null ? true : _projectList.IsValid)
                ;
            }
        }

        private RLSStatus _rlsStatus = RLSStatus.Off;
        internal RLSStatus RLSStatus
        {
            get { return _rlsStatus; }
        }

        private bool _isBatchEditable = true;
        public bool IsBatchEditable
        {
            get { return _isBatchEditable; }
        }

        #endregion

        #region [ Data Access ]

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

        [Serializable()]
        private class TemporalCriteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }

            public TemporalCriteria(int id)
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
            string xml = this.RegDal.GetBatch(criteria.Id);
            this.InitializeFromXml(xml, false, true);
            return;
        }

        private void DataPortal_Fetch(TemporalCriteria criteria)
        {
            string xml = this.RegDal.GetBatchTemporary(criteria.Id);
            this.InitializeFromXml(xml, false, true);
            return;
        }

        public string Xml
        {
            get
            {
                if (IsDirty)
                {
                    UpdateXml();
                }
                return _xml;
            }
        }

        public void InitializeFromXml(string xml, bool isNew, bool isClean)
        {
            _xml = xml;

            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Batch/PropertyList");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                {
                    _propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, isClean);
                }
            }
            else
                _propertyList = PropertyList.NewPropertyList();

            xIterator = xNavigator.Select("Batch/ProjectList");
            if (xIterator.MoveNext())
                _projectList = ProjectList.NewProjectList(xIterator.Current.OuterXml, isClean,isNew);
            else
                this._projectList = ProjectList.NewProjectList();

            xIterator = xNavigator.Select("Batch/IdentifierList");
            if (xIterator.MoveNext())
                this._identifierList = IdentifierList.NewIdentifierList(xIterator.Current.OuterXml, isNew, isClean);
            else
                this._identifierList = IdentifierList.NewIdentifierList();
            
            xIterator = xNavigator.Select("Batch/BatchComponentList");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                    _batchComponentList = BatchComponentList.NewBatchComponentList(xIterator.Current.OuterXml, isNew, isClean);

            xIterator = xNavigator.Select("Batch/BatchID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this.ID = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Batch/TempBatchID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _tempBatchID = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Batch/BatchNumber");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _batchNumber = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Batch/DateCreated");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _dateCreated = DateTime.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Batch/StatusID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._status = (RegistryStatus)Enum.Parse(typeof(RegistryStatus), xIterator.Current.Value);
                else
                    this._status = RegistryStatus.NotSet;

            xIterator = xNavigator.Select("Batch/PersonCreated");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _personCreated = int.Parse(xIterator.Current.Value);
                else
                    _personCreated = COEUser.ID;

            xIterator = xNavigator.Select("Batch/PersonRegistered");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _personRegistered = int.Parse(xIterator.Current.Value);
                else
                    _personRegistered = COEUser.ID;
            }
            else
            {
                _personRegistered = COEUser.ID;
            }

            xIterator = xNavigator.Select("Batch/PersonApproved");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _personApproved = int.Parse(xIterator.Current.Value);
                else
                    _personApproved = COEUser.ID;

            xIterator = xNavigator.Select("Batch/DateLastModified");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _dateLastModified = DateTime.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Batch/FullRegNumber");
            if(xIterator.MoveNext())
                _regNumber = xIterator.Current.Value;

            xIterator = xNavigator.Select("Batch/IsBatchEditable");
            if (xIterator.MoveNext())
            {
                try { _isBatchEditable = Boolean.Parse(xIterator.Current.Value); }
                catch { _isBatchEditable = true; }
            }

            if (!isNew)
                MarkOld();
            else 
            {
                MarkNew();
                PersonCreated = COEUser.ID;
            }

            if (isClean)
                MarkClean();
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Update()
        {
            if (IsTemporal)
            {
                this.RegDal.UpdateBatchTemporary(Xml);
            }
            else
            {
                this.RegDal.UpdateBatch(Xml);
            }
            // WJC Fetch ? !
            return;
        }

        private void DataPortal_Delete(MoveCriteria criteria)
        {
            string fromRegNum = RegistryRecord.GetRegNumberByBatchId(criteria.Id.ToString());
            RegistryRecord fromReg = RegistryRecord.GetRegistryRecord(fromRegNum);
            RegistryRecord toReg = RegistryRecord.GetRegistryRecord(criteria.ToRegNum);

            if (toReg.Equals(fromReg))
                throw new ArgumentException("Source_Destination_Match", "ToRegNum");

            try
            {
                this.RegDal.MoveBatch(criteria.Id, criteria.ToRegNum);
            }
            finally
            {
                // The following is for triggering addins and saving the changes they may generate, and must happen even if a batch was moved
                // from a record with 1 component to a record with 2 components. That case would throw an exeption we dont want to catch, but
                // we still want to trigger the addins.
                // ------------------------------------
                // Get the updated record
                toReg = RegistryRecord.GetRegistryRecord(criteria.ToRegNum);
                // Force it dirty
                toReg.Xml = toReg.Xml;
                // Trigger addins and save changes. (IE: Batch formula and formula weight needs to be recalculated)
                toReg.CheckOtherMixtures = false;
                toReg.Save();
            }
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            this.RegDal.DeleteBatch(criteria.Id);
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(this.ID));
        }

        [Serializable()]
        private class MoveCriteria
        {
            private int _id;
            private string _toRegNum = String.Empty;

            public int Id
            {
                get { return _id; }
            }

            public string ToRegNum
            {
                get { return _toRegNum; }
            }

            private MoveCriteria(int id)
            { _id = id; }

            public MoveCriteria(int id, string toRegNum)
                : this(id)
            { _toRegNum = toRegNum; }
        }

        #endregion

        /// <summary>
        /// Given a Project name, adds a new Project object to the Registration's ProjectList.
        /// </summary>
        /// <param name="projectName">the internal name of the project</param>
        public void AddProject(string projectName)
        {
            RegSvcUtilities.AddProject(this.ProjectList, projectName, ProjectList.ProjectTypeEnum.B);
        }

        /// <summary>
        /// Given a Project ID, adds a new Project object to the Registration's ProjectList.
        /// </summary>
        /// <param name="projectId">the internal ID of the project</param>
        public void AddProject(int projectId)
        {
            RegSvcUtilities.AddProject(this.ProjectList, projectId, ProjectList.ProjectTypeEnum.B);
        }

        /// <summary>
        /// Given the Identifier Id and a value, adds a new Identifier object to the Batch's
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
                    , IdentifierTypeEnum.B);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given an Identifier name and a value, adds a new Identifier object to the Batch's
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
                    , IdentifierTypeEnum.B);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given an Identifier instance, adds the Identifier to the Batch's IdentifierList.
        /// </summary>
        /// <param name="identifier">an Identifier instance</param>
        public void AddIdentifier(Identifier identifier)
        {
            try
            {
                RegSvcUtilities.CreateNewIdentifier(
                    this.IdentifierList
                    , identifier
                    , IdentifierTypeEnum.B);
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

        public new void MarkClean()
        {
            base.MarkClean();
        }
        public new void MarkDirty()
        {
            base.MarkDirty();
        }

        internal new void MarkNew()
        {
            this.ID = 0;
            this._regNumber = string.Empty;
            this._batchNumber = 0;
            foreach (BatchComponent currentBatchComponent in this.BatchComponentList)
            {
                currentBatchComponent.MarkNew();
            }

            base.MarkNew();
        }

        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
            {
                brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
            }

            this._batchComponentList.GetBrokenRulesDescription(brokenRules);
            this._identifierList.GetBrokenRulesDescription(brokenRules);
            this._projectList.GetBrokenRulesDescription(brokenRules);
            this._propertyList.GetBrokenRulesDescription(brokenRules);
        }

        public static void MoveBatch(int batchid, string toRegNum)
        {
            if (!CanEditObject())
                throw new System.Security.SecurityException("User not authorized to remove a Batch");
            DataPortal.Delete(new MoveCriteria(batchid, toRegNum));
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<Batch");
            if (addCRUDattributes && IsNew)
                builder.Append(" insert=\"yes\"");
            builder.Append(">");

            builder.Append("<BatchID");
            if (addCRUDattributes && _changedProperties.Contains("BatchID"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + (this.ID < 0 ? "0" : this.ID.ToString()) + "</BatchID>");

            if (_tempBatchID > 0)
                builder.Append("<TempBatchID>" + _tempBatchID + "</TempBatchID>");

            builder.Append("<BatchNumber");
            if (addCRUDattributes && _changedProperties.Contains("BatchNumber"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._batchNumber + "</BatchNumber>");

            builder.Append("<FullRegNumber");
            if (addCRUDattributes && _changedProperties.Contains("FullRegNumber"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._regNumber + "</FullRegNumber>");

            builder.Append("<DateCreated");
            if (addCRUDattributes && _changedProperties.Contains("DateCreated"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._dateCreated.ToString(Constants.DATE_FORMAT) + "</DateCreated>");

            builder.Append("<PersonCreated");
            if (addCRUDattributes && _changedProperties.Contains("PersonCreated"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonCreated>", _personCreated > 0 ? _personCreated.ToString() : string.Empty);

            builder.Append("<PersonRegistered");
            if (addCRUDattributes && _changedProperties.Contains("PersonRegistered"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonRegistered>", _personRegistered > 0 ? _personRegistered.ToString() : string.Empty);

            builder.Append("<PersonApproved");
            if (addCRUDattributes && _changedProperties.Contains("PersonApproved"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonApproved>", _personApproved > 0 ? _personApproved.ToString() : string.Empty);

            builder.Append("<DateLastModified");
            if (addCRUDattributes && _changedProperties.Contains("DateLastModified"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._dateLastModified.ToString(Constants.DATE_FORMAT) + "</DateLastModified>");

            builder.Append("<StatusID");
            if (addCRUDattributes && _changedProperties.Contains("Status"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</StatusID>", ((int)_status).ToString());

            if (_projectList != null && _projectList.Count > 0)
                builder.Append(this._projectList.UpdateSelf(addCRUDattributes));

            if (_propertyList != null && _propertyList.Count > 0)
                builder.Append(this._propertyList.UpdateSelf(addCRUDattributes));

            if (_identifierList != null)
                builder.Append(this._identifierList.UpdateSelf(addCRUDattributes));

            if (_batchComponentList != null && _batchComponentList.Count > 0)
                builder.Append(this._batchComponentList.UpdateSelf(addCRUDattributes));

            builder.Append("</Batch>");

            return builder.ToString();
        }
        
        public void UpdateXml()
        {
            _xml = UpdateSelf(true);
            return;
        }

        public void UpdateFromXml(XmlNode incomingNode)
        {
            XmlNode projectListNode = incomingNode.SelectSingleNode("ProjectList");
            if (projectListNode != null)
                this.ProjectList.UpdateFromXml(projectListNode);

            XmlNode identifierListNode = incomingNode.SelectSingleNode("IdentifierList");
            this.IdentifierList.UpdateFromXml(identifierListNode);

            XmlNode propertyListNode = incomingNode.SelectSingleNode("PropertyList");
            this.PropertyList.UpdateFromXml(propertyListNode);

            XmlNode batchComponentListNode = incomingNode.SelectSingleNode("BatchComponentList");
            this.BatchComponentList.UpdateFromXml(batchComponentListNode);
        }
        public void UpdateUserPreference(XmlNode incomingNode)
        {
            XmlNode projectListNode = incomingNode.SelectSingleNode("ProjectList");
            if (projectListNode != null)
                this.ProjectList.UpdateUserPreference(projectListNode, ProjectList.ProjectTypeEnum.B);

            XmlNode identifierListNode = incomingNode.SelectSingleNode("IdentifierList");
            this.IdentifierList.UpdateUserPreference(identifierListNode);

            XmlNode propertyListNode = incomingNode.SelectSingleNode("PropertyList");
            this.PropertyList.UpdateUserPreference(propertyListNode);
            
        }
    }
}

