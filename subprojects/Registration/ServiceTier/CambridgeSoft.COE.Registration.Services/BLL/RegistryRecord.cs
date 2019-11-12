using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.Common;

using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Services;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Registration.Services.BLL.Command;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Properties;
using CambridgeSoft.COE.Registration.Validation;

using Csla;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Registration.Access;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using ChemDrawControl19;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// This is the core domain class for Registration. It acts as both data-container and service.
    /// When possible, utility functions have been moved to RegSvcUtilities to reduce coe in this class.
    /// The duplicate-checking workflow and auto-resolution logic are pretty messy, but are functional
    /// and should represent the correct business logic.
    /// </summary>

    [Serializable()]
    public class RegistryRecord :
        RegistrationBusinessBase<RegistryRecord>, IRegistryRecord, IDisposable, IDestinationRecord
    {
        #region >Private<

        const string PROPERTYLIST = "PropertyList";
        const string PROPERTY = "Property";
        const int _maxConnectionTimerInSeconds = 90;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton(Constants.SERVICENAME);

        private string _propertyNameAttribute = "name";
        private string _addInsXpath = "/MultiCompoundRegistryRecord/AddIns";
        private bool _noUpdateXml = false;
        private List<int> _potentiallyCopiedCompoundsIds = new List<int>();     
      
        #endregion

        #region >Events and event-handlers<

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

        public event EventHandler Loaded;
        public event EventHandler Inserting;
        public event EventHandler Inserted;
        public event EventHandler Updating;
        public event EventHandler Updated;
        public event EventHandler Registering;
        public event EventHandler UpdatingPerm;
        public event EventHandler Saving;       

        protected void OnLoaded(object sender, EventArgs args)
        {
            if (Loaded != null && !this.IsNew)
                Loaded(sender, args);
        }

        protected void OnInserting(object sender, EventArgs args)
        {
            if (Inserting != null)
                Inserting(sender, args);
        }

        protected void OnInserted(object sender, EventArgs args)
        {
            if (Inserted != null)
                Inserted(sender, args);
        }

        protected void OnUpdating(object sender, EventArgs args)
        {
            if (Updating != null)
                Updating(sender, args);
        }

        protected void OnUpdated(object sender, EventArgs args)
        {
            if (Updated != null)
                Updated(sender, args);
        }

        protected void OnRegistering(object sender, EventArgs args)
        {
            if (Registering != null)
                Registering(sender, args);
        }

        protected void OnUpdatingPerm(object sender, EventArgs args)
        {
            if (UpdatingPerm != null)
                UpdatingPerm(sender, args);
        }

        public void OnSaving(object sender, EventArgs args)
        {
            if (Saving != null)
                Saving(sender, args);
        }

        #endregion

        #region Business Methods

        private int _mixtureId;
        private DateTime _dateLastModified;
        private DateTime _dateCreated;
        private string _xml;
        private bool _isTemporal;
        private DataAccessStrategy _dataStrategy = DataAccessStrategy.Atomic;
        private DuplicateCheck _checkDuplicates = DuplicateCheck.CompoundCheck;
        private DuplicateAction _actionDuplicates = DuplicateAction.None;
        private DuplicateAction _actionDuplicateTaken = DuplicateAction.None;
        private bool _checkOtherMixtures = true;
        private bool _aggregateOtherMixtures = true;
        private List<string> _changedProperties = new List<string>();
        private int _personCreated;
        private string _submissionComments;
        private Dictionary<string, string[]> _regsAffectedByCompoundUpdate = new Dictionary<string, string[]>();
        private Dictionary<string, string[]> _regsAffectedByStructureUpdate = new Dictionary<string, string[]>();
        private RLSStatus _rlsStatus;
        private bool _isEditable;
        private bool _isRegistryDeleteable;
        private bool _isDirectReg = false;
        //JED: CRUD methods always have an XML output parameter which we can extract information from
        [NotUndoable]
        private string _foundDuplicates = string.Empty;
        [NotUndoable]
        private string _dalResponseXml = string.Empty;
        [NotUndoable]
        private string _dalResultXml = string.Empty;
        [NotUndoable]
        private string _dalResponseMessage = string.Empty;
        [NotUndoable]
        private string _customDuplicateXml = string.Empty;  //custom dulicate check response
        [NotUndoable]
        private string _customFieldsResponseXml=string.Empty; //Purpose to fetch any custom field of Registry record value to this on and need to parse it later.

        private List<DuplicateCheckResponse> _registeredMatches = new List<DuplicateCheckResponse>();

        private string _prototypeXml = null;
        private bool _submitCheckRedBoxWarning = false;
        private bool _registerCheckRedBoxWarning = false;
        private string _redBoxWarning = string.Empty;
        private ChemDrawWarningChecker _theChemDrawChecker = new ChemDrawWarningChecker();

        /// <summary>
        /// 
        /// </summary>
        public ChemDrawWarningChecker TheChemDrawChecker
        {
            get { return _theChemDrawChecker; }
            set { _theChemDrawChecker = value; }
        }

        /// <summary>
        /// Lazy-fetches the 
        /// </summary>
        public string PrototypeXml
        {
            [COEUserActionDescription("GetPrototypeRegistryXml")]
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_prototypeXml))
                    {
                        string msg = "Retrieving prototype Registration object XML";
                        _coeLog.LogStart(msg);

                        PrototypeRegistryXml proto = PrototypeRegistryXml.GetPrototypeRegistryXml(this);
                        _prototypeXml = proto.Prototype;

                        _coeLog.LogEnd(msg);
                    }
                    return _prototypeXml;
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return null;
                }
            }
        }

        private RegistryStatus _status;
        private ChemDrawWarningChecker.ModuleName _moduleName = ChemDrawWarningChecker.ModuleName.NOTSET; 		//CBOE-1159 added a variable for storing module name : ASV 10JUL13

        public ChemDrawWarningChecker.ModuleName ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; PropertyHasChanged(); }
        }
        /// <summary>
        /// To Retrive Xml When Any Exception Occurs In Xml That Failed To Insert.
        /// </summary>
        public string ExceptionXml
        {
           get 
           {
                return _xml;
           }
    
        }
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
                    foreach(Batch batch in this.BatchList)
                    {
                        batch.Status = value;
                    }
                    PropertyHasChanged();
                }
            }
        }

        private BatchList _batchList;
        public BatchList BatchList
        {
            get { return _batchList; }
        }

        public bool IsSingleCompound
        {
            get
            {
                return this.ComponentList.Count == 1;
            }
        }

        private RegNumber _regNumber;
        public RegNumber RegNumber
        {
            get
            {
                CanReadProperty(true);
                return _regNumber;
            }
        }
        
        /// <summary>
        /// Must be set-able for legacy data loading.
        /// </summary>
        public string RegNum
        {
            get { return _regNumber.RegNum; }
            set { _regNumber.RegNum = value; }
        }

        private ComponentList _componentList;
        public ComponentList ComponentList
        {
            get { return _componentList; }
        }

        private string _structureAggregation;
        public string StructureAggregation
        {
            get
            {
                CanReadProperty(true);
                return _structureAggregation;
            }
            set
            {
                CanWriteProperty(true);
                if (_structureAggregation != value && value != "null")
                {
                    _structureAggregation = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// Person who created the current registry object
        /// </summary>
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

        /// <summary>
        /// Comments from the submitter
        /// </summary>
        public string SubmissionComments
        {
            get
            {
                CanReadProperty(true);
                return _submissionComments;
            }
            set
            {
                CanWriteProperty(true);
                if (_submissionComments != value)
                {
                    _submissionComments = value;
                    PropertyHasChanged();
                }
            }
        }

        /// <summary>
        /// Flag for indicating no Temp table  for UI workflow
        /// </summary>
        public bool IsDirectReg
        {
            get
            {
                CanReadProperty(true);
                return _isDirectReg;
            }
            set
            {
                CanWriteProperty(true);
                if (_isDirectReg != value)
                {
                    _isDirectReg = value;
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
                return _projectList;
            }
            set
            {
                CanWriteProperty(true);
                if (_projectList != value)
                {
                    _projectList = value;
                    PropertyHasChanged();
                }
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
            set
            {
                CanWriteProperty(true);
                if (_propertyList != value)
                {
                    _propertyList = value;
                    PropertyHasChanged();
                }
            }
        }

        private IdentifierList _identifierList;
        public IdentifierList IdentifierList
        {
            get
            {
                CanReadProperty(true);
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

        //JED: A property setter for this is ill-advised: we are willing to assume that the caller is
        // providing 'good' xml despite no validation rules being in place?
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
            set
            {
                _xml = value;
                PropertyHasChanged();
            }
        }

        public string XmlWithAddIns
        {
            [COEUserActionDescription("GetRegistryRecordXmlWithAddIns")]
            get
            {
                try
                {
                    if (IsDirty)
                    {
                        UpdateXml();
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_xml);

                    if (doc.SelectSingleNode(_addInsXpath) == null && this.PrototypeXml != null)
                    {
                        doc.SelectSingleNode("//MultiCompoundRegistryRecord").AppendChild(
                            doc.ImportNode(this.GetAddInsXmlNode(), true)
                        );
                    }
                    return doc.OuterXml;
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return null;
                }
            }
        }

        public string XmlWithAddInsWithoutUpdate
        {
            [COEUserActionDescription("GetRegistryRecordXmlWithAddInsWithoutUpdate")]
            get
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_xml);

                    if (doc.SelectSingleNode(_addInsXpath) == null && this.PrototypeXml != null)
                    {
                        doc.SelectSingleNode("//MultiCompoundRegistryRecord").AppendChild(
                            doc.ImportNode(this.GetAddInsXmlNode(), true)
                        );
                    }
                    return doc.OuterXml;
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return null;
                }
            }
        }

        private object _allowUnregisteredComponents = null;
        public bool AllowUnregisteredComponents
        {
            get
            {
                if (_allowUnregisteredComponents == null)
                {
                    _allowUnregisteredComponents = Convert.ToBoolean(RegSvcUtilities.GetAllowUnregisteredComponents());
                }
                return (bool)_allowUnregisteredComponents;
            }
        }

        private object _EnableBatchPrefix = null;
        public bool EnableBatchPrefix
        {
            get
            {
                if (_EnableBatchPrefix == null)
                {
                    _EnableBatchPrefix = Convert.ToBoolean(RegSvcUtilities.GetEnableBatchPrefix());
                }

                return (bool)_EnableBatchPrefix;
            }
        }

        private object _defaultBatchPrefix = null;
        public int DefaultBatchPrefix
        {
            get
            {
                if (_EnableBatchPrefix == null)
                {
                    _defaultBatchPrefix = Convert.ToInt32(RegSvcUtilities.GetDefaultBatchPrefix());
                }
                
                return (int)_defaultBatchPrefix;
            }
        }

        private bool _samesBatchIdentity;
        public bool SameBatchesIdentity
        {
            get
            {
                return _samesBatchIdentity;
            }
        }

        public RLSStatus RLSStatus
        {
            get { return _rlsStatus; }
        }

        public Dictionary<string, string[]> RegsAffectedByCompoundUpdate
        {
            get
            {
                CanReadProperty();
                return _regsAffectedByCompoundUpdate;
            }
        }
        public new bool IsNew
        {
            get
            {
                return base.IsNew;
            }
        }
        public override bool IsValid
        {
            [COEUserActionDescription("ValidateRegistryRecord")]
            get
            {
                try
                {
                    if (this._componentList.Count <= 0)
                        throw new ValidationException(Resources.ComponentCount);

                    ValidationRules.CheckRules();

                    bool totallyValid = base.IsValid && _batchList.IsValid && _componentList.IsValid && PropertyList.IsValid;
                    return totallyValid;
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return false;
                }
            }
        }        

        public override bool IsDirty
        {
            get { return base.IsDirty || ProjectList.IsDirty || IdentifierList.IsDirty || ComponentList.IsDirty || BatchList.IsDirty || PropertyList.IsDirty; }
            //get { return base.IsDirty || _resources.IsDirty; }
        }

        public string ValidationErrorMessage
        {
            get
            {
                List<string> message = new List<string>();
                List<BrokenRuleDescription> brokenRuleList = GetBrokenRulesDescription();
                foreach (BrokenRuleDescription brokenRule in brokenRuleList)
                {
                    message.AddRange(brokenRule.BrokenRulesMessages);
                }
                return string.Join(" __________ ", message.ToArray());
            }
        }

        public string DalResponseMessage
        {
            get
            {
                CanReadProperty();
                return _dalResponseMessage;
            }
        }

        private string DalResponseResult
        {
            get
            {
                CanReadProperty();
                return _dalResultXml;
            }
        }

        public string CustomFieldsResponse
        {
            get
            {
                CanReadProperty();
                return _customFieldsResponseXml;
            }
        }

        private bool HasDuplicatesFound
        {
            get { return !string.IsNullOrEmpty(_foundDuplicates); }
        }

        /// <summary>
        /// Found Duplicates
        /// </summary>
        public string FoundDuplicates
        {
            get
            {
                CanReadProperty();
                return _foundDuplicates;
            }
            set
            {
                CanWriteProperty();
                if (_foundDuplicates != value)
                    _foundDuplicates = value;
            }
        }

        /// <summary>
        /// ActionDuplicateTaken
        /// Duplicate action that was taken, if any, by DataPortal_Insert
        /// </summary>
        public string ActionDuplicateTaken
        {
            get
            {
                return _actionDuplicateTaken.ToString().Substring(0, 1);
            }
        }

        public DuplicateAction ActionDuplicates
        {
            get
            {
                return _actionDuplicates;
            }
        }
        
        public bool IsTemporal
        {
            get
            {
                CanReadProperty();
                return _isTemporal;
            }
        }

        public DataAccessStrategy DataStrategy
        {
            get
            {
                CanReadProperty();
                return _dataStrategy;
            }
            set
            {
                CanWriteProperty();
                if (_dataStrategy != value)
                    _dataStrategy = value;
            }
        }

        public bool CheckOtherMixtures
        {
            get
            {
                CanReadProperty();
                return _checkOtherMixtures;
            }
            set
            {
                CanWriteProperty();
                if (_checkOtherMixtures != value)
                    _checkOtherMixtures = value;
            }
        }

        /// <summary>
        /// If in the process of updating other mixtures' aggregate structures becuase a shared compound has
        /// been edited, set this to FALSE for those other mixtures to avoid an infinite loop!
        /// </summary>
        public bool AggregateOtherMixtures
        {
            get
            {
                CanReadProperty();
                return _aggregateOtherMixtures;
            }
            set
            {
                CanWriteProperty();
                if (_aggregateOtherMixtures != value)
                    _aggregateOtherMixtures = value;
            }
        }

        public bool CanInsert
        {
            get { return this.IsValid && this.IsDirty && HasDuplicatesFound == false; }
        }

        /// <summary>
        /// Directive for what kind of duplicate checking should occurr during registration
        /// </summary>
        public DuplicateCheck CheckDuplicates
        {
            get
            {
                return _checkDuplicates;
            }
            /*
              set
              {
                  CanWriteProperty();
                  if (_checkDuplicates != value)
                      _checkDuplicates = value;
              }*/
        }

        /// <summary>
        /// Special property similar to the keyword this, but not read only. Needed for being used as binding expression for replacing the keyword this.
        /// </summary>
        public RegistryRecord Self
        {
            get
            {
                return this;
            }
            set
            {
                this.InitializeFromXml(value.XmlWithAddIns, value.IsNew, !value.IsDirty);
            }
        }

        /// <summary>
        /// Returns true if the user is allowed to edit the record based on RLS restrictions.
        /// </summary>
        public bool IsEditable
        {
            get { return _isEditable; }
        }

        /// <summary>
        /// Returns true if the user is allowed to delete the registry record based on RLS restrictions.
        /// </summary>
        public bool IsRegistryDeleteable
        {
            get { return _isRegistryDeleteable; }
        }

        /// <summary>
        /// Returns true if Red Box warnings are found and should be checked on Submit button
        /// </summary>
        public bool SubmitCheckRedBoxWarning
        {
            get { return _submitCheckRedBoxWarning; }
            set { _submitCheckRedBoxWarning = value; }
        }

        /// <summary>
        ///  Returns true if Red Box warnings are found and should be checked on Register button
        /// </summary>
        public bool RegisterCheckRedBoxWarning
        {
            get { return _registerCheckRedBoxWarning; }
            set { _registerCheckRedBoxWarning = value; }
        }

        /// <summary>
        /// Red box warning message
        /// </summary>
        public string RedBoxWarning
        {
            get { return _redBoxWarning; }
            set { _redBoxWarning = value; }
       }

       
        /// <summary>
        /// Allows a mechanism for the 'FindDuplicates' add-in to provide its input to RegistryRecord.
        /// </summary>
        /// <param name="duplicateResponseXml">the xml string containing the add-in's findings</param>
        public void SetDuplicateResponse(string duplicateResponseXml)
        {
            _dalResponseXml = duplicateResponseXml;
            ExtractDalResponse();
        }
        /// <summary>
        /// Allows a mechanism for the 'FindDuplicates' add-in to set the list of duplicates to insert into duplicates table.
        /// </summary>
        /// <param name="customDuplicateResponseXml"></param>
        public void SetCustomDuplicateResponse(string customDuplicateResponseXml)
        {
            _customDuplicateXml = customDuplicateResponseXml;
        }

        /// <summary>
        /// Extracts the new object XML, any error xml/message and a user-friendly message attribute
        /// from the "Response" xml returned from the DAL's CRUD methods
        /// </summary>
        /// 
        [COEUserActionDescription("ExtractDalResponse")]
        public void ExtractDalResponse()
        {
            try
            {
                if (!string.IsNullOrEmpty(_dalResponseXml))
                {
                    try
                    {
                        XmlDocument msgDoc = new XmlDocument();
                        msgDoc.LoadXml(_dalResponseXml);

                        /* The expected format is THIS.
                         * HOWEVER, we really need an OO-representation so we can do business processing!
                         * <Response message="1 duplicated mixture.">
                         *   <Error>
                         *     <REGISTRYLIST>
                         *       <REGNUMBER SAMEFRAGMENT="True" SAMEEQUIVALENT="True">AB-000001</REGNUMBER>
                         *     </REGISTRYLIST>
                         *   </Error>
                         *   <Result></Result>
                         * </Response>
                         * */
                        XmlNode responseNode = msgDoc.SelectSingleNode("/Response");
                        string xml = string.Empty;
                        if (responseNode != null)
                        {
                            if (responseNode.Attributes.Count > 0) // Coverity fix - CID 11791 
                            {
                                XmlNode theMessageNode = responseNode.Attributes.GetNamedItem("message");// Coverity fix - CID 11791
                                if(theMessageNode !=null) // Coverity fix - CID 11791
                                    _dalResponseMessage = theMessageNode.Value.ToString();// Coverity fix - CID 11791
                            }
                            XmlNode errorNode = responseNode.FirstChild;
                            //Coverity fix JIRA ID:  CBOE-356
                            if (errorNode != null)
                            {
                                if (_potentiallyCopiedCompoundsIds.Count > 0)
                                    this.RemovePotentiallyCopiedCompounds(ref errorNode);

                                if (!string.IsNullOrEmpty(errorNode.InnerXml))
                                {
                                    this.RemoveResolvedDuplicates(ref errorNode);
                                    _foundDuplicates = errorNode.InnerXml;
                                }
                                XmlNode resultNode = errorNode.NextSibling;
                                if (resultNode != null && !string.IsNullOrEmpty(resultNode.InnerXml)) //Coverity fix JIRA ID:  CBOE-356
                                {
                                    _dalResultXml = resultNode.InnerXml;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _dalResultXml = null;
                        _foundDuplicates = null;
                        _dalResponseMessage = null;

                        _coeLog.Log("Error in 'ExtractDalResponse':/n/n" + ex.ToString(), 0, System.Diagnostics.SourceLevels.Error);
                    }
                }
                else
                {
                    _dalResultXml = null;
                    _foundDuplicates = null;
                    _dalResponseMessage = null;
                    _registeredMatches = new List<DuplicateCheckResponse>();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given a list of registered compounds, creates a mixture out of the current record
        /// using these compounds and their percentages of the overall mixture.
        /// </summary>
        /// <param name="compoundRegNumList">
        /// the delimited list of pre-registered compound registry numbers
        /// </param>
        /// <param name="compoundPercentagesList">
        /// the delimited list of each compound's percentage of the mixture
        /// </param>
        /// <param name="delimiterCharacter">
        /// the delimiter used to split the compound- and percentage- lists
        /// </param>
        [COEUserActionDescription("CreateMixtureFromCompoundList")]
        public void CreateMixture(
            string compoundRegNumList, string compoundPercentagesList, string delimiterCharacter)
        {
            try
            {
                RegSvcUtilities.CreateMixtureFromRegisteredComponents(
                    this, compoundRegNumList, compoundPercentagesList, delimiterCharacter);
                this.MarkDirty();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("AddComponentToRegistryRecord")]
        public void AddComponent(int index, Component component)
        {
            try
            {
                foreach (Component componentTemp in this.ComponentList)
                {
                    if (componentTemp.ComponentIndex >= component.ComponentIndex)
                        component.ComponentIndex = componentTemp.ComponentIndex + 1;
                }

                _componentList.Insert(index, component);

                foreach (Batch batch in _batchList)
                {
                    BatchComponent currentBatchComponent = BatchComponent.NewBatchComponent();
                    currentBatchComponent.CompoundID = component.Compound.ID;
                    currentBatchComponent.ComponentIndex = component.ComponentIndex;
                    currentBatchComponent.BatchID = batch.ID;
                    batch.BatchComponentList.Add(currentBatchComponent);
                }

                OnLoaded(this, new EventArgs());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }

        }

        [COEUserActionDescription("AddComponentToRegistryRecord")]
        public void AddComponent(Component component)
        {
            try
            {
                this.AddComponent(this.ComponentList.Count, component);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("AddBatchToRegistryRecord")]
        public void AddBatch()
        {
            try
            {
                using (RegistryRecord record = RegistryRecord.NewRegistryRecord())
                {
                    Batch batch = record.BatchList[0];
                    BatchComponent firstBatchComponent = BatchComponent.NewBatchComponent();
                    firstBatchComponent.CompoundID = this.ComponentList[0].Compound.ID;
                    firstBatchComponent.ComponentIndex = this.ComponentList[0].ComponentIndex;
                    firstBatchComponent.BatchID = batch.ID;
                    batch.BatchComponentList[0] = firstBatchComponent;
                    batch.Status = this.Status;

                    for (int i = 1; i < this.ComponentList.Count; i++)
                    {
                        BatchComponent batchComponent = BatchComponent.NewBatchComponent();
                        batchComponent.CompoundID = this.ComponentList[i].Compound.ID;
                        batchComponent.ComponentIndex = this.ComponentList[i].ComponentIndex;

                        batchComponent.BatchID = batch.ID;
                        batch.BatchComponentList.Add(batchComponent);
                    }
                    //Add Fragments info
                    if (this.SameBatchesIdentity)
                    {
                        for (int j = 0; j < batch.BatchComponentList.Count; j++)
                        {
                            //Take first batch and copy its values to the new object (by default, same fragmenttype and equivalent value
                            foreach (BatchComponentFragment fragment in this.BatchList[0].BatchComponentList[j].BatchComponentFragmentList)
                            {
                                //Created with the same fragmentid and equivalent value that the first batch.
                                BatchComponentFragment batchComponentFragment = BatchComponentFragment.NewBatchComponentFragment(fragment.FragmentID, fragment.Equivalents);
                                //The COEObjectConfig provides us an object with one BatchComponentFragmentList on it.
                                if (batch.BatchComponentList[j].BatchComponentFragmentList.Count == 1)
                                {
                                    if (batch.BatchComponentList[j].BatchComponentFragmentList[0].FragmentID == 0)
                                        batch.BatchComponentList[j].BatchComponentFragmentList.RemoveAt(0);
                                }
                                batch.BatchComponentList[j].BatchComponentFragmentList.Add(batchComponentFragment);
                            }
                        }
                    }

                    batch.MarkClean();
                    this.BatchList.Add(batch);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("AddBatchToRegistryRecord")]
        public void AddBatch(Batch batch)
        {
            try
            {
                batch.ID = 0;
                this.InsertBatch(batch);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        public void InsertBatch(Batch batch)
        {
            batch.MarkNew();
            this.BatchList.Add(batch);

            if (batch.BatchComponentList.Count != this.ComponentList.Count)
                throw new ValidationException("Component and BatchComponent Lists element doesn't match");


            if (this.ComponentList.Count == 1)
            {
                foreach (BatchComponent currentBatchComponent in batch.BatchComponentList)
                {
                    foreach (Component currentComponent in this.ComponentList)
                    {
                        if (currentComponent.ComponentIndex == currentBatchComponent.ComponentIndex)
                            currentBatchComponent.CompoundID = currentComponent.Compound.ID;
                    }
                }
            }
            else
            {
                //RegistrationOracleDAL regDal = null;
                //DalUtils.GetRegistrationDAL(ref regDal, CambridgeSoft.COE.Registration.Constants.SERVICENAME);
                foreach (BatchComponent batchComponent in batch.BatchComponentList)
                {
                    if (string.IsNullOrEmpty(batchComponent.RegNum) && this.IsTemporal == true)
                    {
                        int batchComponentRegId = this.RegDal.GetTempComponentRegId(batchComponent.CompoundID);
                        foreach (Component duplicateComponent in this.ComponentList)
                        {
                            if (batchComponentRegId == duplicateComponent.Compound.RegNumber.ID)
                            {
                                batchComponent.ComponentIndex = duplicateComponent.ComponentIndex;
                                batchComponent.CompoundID = duplicateComponent.Compound.ID;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Component currentComponent in this.ComponentList)
                        {
                            if ((currentComponent.ComponentIndex == batchComponent.ComponentIndex) || (currentComponent.Compound.RegNumber.RegNum == batchComponent.RegNum)) //add additional check for duplicate mixture add batch scenario.
                            {
                                batchComponent.CompoundID = currentComponent.Compound.ID;
                                batchComponent.ComponentIndex = currentComponent.ComponentIndex;
                            }
                        }
                    }
                }
            }
        }

        //this might be removed. it doesn't seem necessary and causes some issues
        public void FixMixtureBatchesFragments()
        {
            if (this.ComponentList.Count > 1 && AllowUnregisteredComponents == false && this.SameBatchesIdentity==false)
            {
                foreach (Batch currentBatch in _batchList)
                {
                    BatchComponentFragmentList fl = currentBatch.BatchComponentList[currentBatch.BatchComponentList.Count - 1].BatchComponentFragmentList;

                    foreach (BatchComponent currentBatchComponent in currentBatch.BatchComponentList)
                    {
                        
                            currentBatchComponent.BatchComponentFragmentList = fl;
                       
                        
                    }
                }
            }
            FixMixtureBatchesFragmentsRegNum();

        }


        private void FixMixtureBatchesFragmentsRegNum()
        {
            //fix the regnum for display
            if(this.ComponentList.Count>1 && AllowUnregisteredComponents==false){
            
                foreach (Batch currentBatch in _batchList)
                {
                    foreach (BatchComponent batchComponent in currentBatch.BatchComponentList)
                    {   
                            batchComponent.RegNum = this.RegDal.GetRegNumFromCompoundID(batchComponent.CompoundID);
                    }
                }
            }
            

        }

        /// <summary>
        /// Methods that will loop and overwrite the fragments values for each batch based on the values of Batch 0.
        /// </summary>
        /// 
        [COEUserActionDescription("FixBatchFragmentsInRegistryRecord")]
        public void FixBatchesFragments()
        {
            try
            {
                //In case fragments has been updated, we need to modify the other batches if the flag sameIdentity = true.
                if (this.SameBatchesIdentity)
                {
                    if (this.BatchList.Count > 0)
                    {
                        //We assume the first batch is the one that has the correct values for the fragment sections. Notice that in RegGUI only values of batch 0 are editable (if SBI = true).
                        foreach (BatchComponent batchComp in this.BatchList[0].BatchComponentList)
                        {
                            if (batchComp.BatchComponentFragmentList.IsDirty)
                            {
                                for (int i = 1; i < this.BatchList.Count; i++) //Overwrite values of other batches.
                                {
                                    this.BatchList[i].BatchComponentList[batchComp.OrderIndex - 1].BatchComponentFragmentList = batchComp.BatchComponentFragmentList;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("AddGroupOfBatches")]
        public void MoveBatches(RegistryRecord originRegistry)
        {
            try
            {
                foreach (Batch currentBatch in originRegistry.BatchList)
                {  
                    this.InsertBatch(currentBatch);
                }

            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("UpdateRegistryRecordFragments")]
        public void UpdateFragments()
        {
            try
            {
                //TODO: Remove those unnecesaries fragments.
                // Nico: Surprisingly fragment deletion was not implemented at all.
                RegSvcUtilities.UpdateFragments(this);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

         public  void UpdateNormalizedPriorToSave()
            {  //this is to address the issue where a registry record structure is editted and normalization is turned off.  Becasue of this, the normalization property
                //always contains the uneditted structure.  StructureAggregation uses the normalized field, so you get incorrect aggregation.
                //if normalization is turned on, it will overwrite this field when done.
                
                ComponentList cl = this.ComponentList;
                if((AllowUnregisteredComponents== false && cl.Count ==1) || AllowUnregisteredComponents == true){
                    for (int i=0;i<cl.Count;i++){
                        if (cl[i].Compound.BaseFragment.Structure.IsDirty)
                        {
                            
                                if (string.IsNullOrEmpty(cl[i].Compound.BaseFragment.Structure.NormalizedStructure))
                                {
                                    cl[i].Compound.BaseFragment.Structure.NormalizedStructure = cl[i].Compound.BaseFragment.Structure.Value;
                                }
                            
                            
                        }

                    }
                }
            }


         public void UpdateDrawingType()
         {
             ComponentList cl = this.ComponentList;
             if ((AllowUnregisteredComponents == false && cl.Count == 1) || AllowUnregisteredComponents == true)
             {
                 for (int i = 0; i < cl.Count; i++)
                 {
                    
                        //update to correct xml when drawingtype <>0
                        switch (cl[i].Compound.BaseFragment.Structure.DrawingType)
                        {
                            case DrawingType.Chemical:
                                break;
                            case DrawingType.NoStructure:
                                cl[i].Compound.BaseFragment.Structure.Value = cl[i].Compound.BaseFragment.Structure.NormalizedStructure = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBkNjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAIBsAABAYgAzc3oAM7O8AAEJCAAAAAAAAAAAAAIJCAAAAOEAAAAsAQ0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsICAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAAHgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgBgAAAAEAhAAAAAAAAAAAAAAANACAAAcAhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoAWAAAAAAIIAABAdwAAQGIABAIQAACAbAAAQGIAM3N6ADOzvAAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4ndCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAABxgAAQAAAAQAAADwAAMATk8gU1RSVUNUVVJFAAAAAAAAAAA=";                          
                                break;
                            case DrawingType.Unknown:
                                cl[i].Compound.BaseFragment.Structure.Value = cl[i].Compound.BaseFragment.Structure.NormalizedStructure = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAIAAsAAABz bWFsbC5jZHgEAhAAAEBTAAAA3gDMDIgAzMwSAQEJCAAAQBEAAAADAAIJCAAAALkBAEAzAg0I AQABCAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABE BAEAAAoICAADAGAAyAADAAsICAADAAAAyAADAAkIBAAAgAIACAgEAJmZAQAHCAQAmZkAAAYI BAAAAAIABQgEAGZmDgAECAIAtAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAEC CBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAA AP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABDwAAAAEAAwDkBAUAQXJpYWwACHgA AAMAAAEgASAAAAAAC2YIoP+E/4gL4wkYA2cFJwP8AAIAAAEgASAAAAAAC2YIoAABAGQAZAAA AAEAAQEBAAAAAScPAAEAAQAAAAAAAAAAAAAAAAACABkBkAAAAAAAYAAAAAAAAAAAAAEAAAAA AAAAAAAAAAAAAAAAAYAIAAAABAIQAAAAAAAAAAAAAMDPAgAAHAIWCAQAAAAkABgIBAAAACQA GQgAABAIAgABAA8IAgABAAmAAgAAAAQCEAAAQFMAAADeAMwMiADMzBIBCgACAAEAcQp+CP/Y /+AAEEpGSUYAAQEBAEgASAAA/9sAQwAKBwcIBwYKCAgICwoKCw4YEA4NDQ4dFRYRGCMfJSQi HyIhJis3LyYpNCkhIjBBMTQ5Oz4+PiUuRElDPEg3PT47/9sAQwEKCwsODQ4cEBAcOygiKDs7 Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7/8AAEQgA gACAAwEiAAIRAQMRAf/EABsAAAEFAQEAAAAAAAAAAAAAAAUAAQMEBgIH/8QAOBAAAgIBAgME CAMHBQAAAAAAAQIAAwQFEQYSISIxQVETMkJhcYGxwQcU0RVScpGSoeEjM0NTVP/EABoBAAID AQEAAAAAAAAAAAAAAAACAwQFAQb/xAAeEQADAAIDAQEBAAAAAAAAAAAAAQIDERIhMQRRQf/a AAwDAQACEQMRAD8A9miiigAooooAKKNOLLq6lLO4UDzMAO48AZvGeh4LFXzUdx7NfaP9oLs/ EnS1OyUZLjzFe31kk4sleIV3K9Zs40x1f4k6Wx2soyUHma9/pCmFxlomcwWvNRXPsv2T/eFY sk+o4rl+MPRSOu6u1QUcMD5GdyMceKKKACiiigAooooAKcswUbk7CJmCjcnYTz7iziy7LyW0 nSXIAPLbcp67+IB+pkmLFWWuMiXahbYU4g46x9PsbEwE/NZQ6HY9lD7z9hMvZia5xA3pdSy2 qqPX0fUD+kfePpen04ShuUNZ4sfD4Qwt2wmtHzzjXS2/0o1md+vSKNHDGnUIOfntPvPKP5Cd 2aXgINlxax8pba/pK9lm/jLEzT9ZA6kHXaXiv3VBf4TBeXpgq7u0p8/CH2aVbxzKZZhfpDT/ AAGYGqapo7g4eU4Uf8bnmU/Lw+U3fD/HONqTri5qjGyT0AJ7L/A/aYp8feU8jE3HQSv9HxY8 i2umTYvqqen2j21WDDcHcTqedcI8X20XJpup2Fg3Sq5vof1noaOHUFTuDMDLirHXGjVi1a2j qKKKRjijR5HbYKqmsY7BRvADKcdcQtpuGuDivtk5II3HsL4mYrSKFrQuR1PSRannNrGtZGax JVm5a/cg7v1+ctYnYXlPdPRfJ86jHt+syPozcr0vEElcbTr0khSPY4qqZ27gJYaSK+2zt7lQ czsFHvlZtQoB6Fm+Ag6yx7nLuev0nBKjx3PkJVrM99FhY1/Qn+0Mc9CWX4idB0uXetgy+YgZ iW6d3uj0u9Fges7H6zs/Q0+xaxJ+Bc1yN6dxLVJF9K2KOjD+Uc1y4r2V+OgHlYu4PSbngniF szHODlPvkUdNz7S+Bmaup3BlHFyH0rVacxDsFbZ/ep75S+zEskb/AKi382ThWn4z2QHePKuD kDIx0cHfcbyzPPmsPAHGeacHhrLdDs7JyL8T0+8PzG/iRYRo1NY7nvXf6/aSYp5WkLb1LZgs KoAADuHSFq06SnhJ2RCdaT1SekYD9HRZHm0X3UBKuva3PXaWkSTqkht7Whp6ezPNp+WO+lz8 OsJ6Xheio9I6bWMT3jqBCapKWoZ35femn/d8T+7/AJlXUx2T7q+ilrAq561Xb0g35tvLw3g3 lkxBYkkkk95Mt4OnWZj77Fah6zfYSu65V0TJcUW9LrP5BSe4sSJaNctrStaBFGyqNgJya5ci tLRWpbeyjZX0gjPo3U9JoHSC86vsmO62hUuzVcGZxyNJqDHdlHKfiOk1Q6iYDgiwolqeAtM3 qHdRPO5Fq2jbl7lM7mN/Easto9L+CXrv9PvNlAHGWGczh3JVRuyrzr8R1+07irjaYWty0efY Q3UQpWsFaewZQR49YZqG4npFXRhtdkiLJ0SMiywiyGqGSGVOm/lMs5NljOx3LEkzX8vLWzbb 7AnaCV1mr/wJ/MfpKeWk/wCljGvxFDTsNczMWtj2AOZtvKaZaVRAiKFUdAB4Srp2oJl5QqXF WslSeYH/ABC3o/dFikvDtb2UzXI2SXWrkbJJVZG0UXSCs8dkw3auwgPUnCq2/cJJy6OKey3w cuxsbztM9Aq9QTE8J0FMarcbFu0fn1m3rHZExcj3TZrStSkdyHIqFtLIRuGG0mjGIMeRX4ra Xq1+Gw2CNunvU936fKFsZgwEL8aaK2RWufjJvdT3ge0viJm9PyQyjY982fnzc40/UZmfFxra DdYlmtZWoYMJcrEktkKR06/6Fn8B+kyagbCbRFk9dNf/AFr/AEiU8nZPD0Znh9QdUX+BpqfR yVKUHUIoPuEkNchVaHfZSZJC6y667SnewUGSzQjQPym5VMzmYDlZSY6+2e18PGFdTy1RGJOw Eg0bCey05Ninns7gfZE7lycZJMOPdbNLoePygdNgBNEo6Sjp2P6KkdJemcXh40eKAEV1QtQq w3Bnn2v6FbpuU+ZiIWqY72VqO4+Y/Seiyvk4yXoVYA7x4tw9oWpVLTPPMHOV1BDAgwzRcpAl bV+GWqubIwj6Jydyu3Zb4j7wUmbfhNyZdbVEe13qfnL855pFK8Lnw1tTgy3WRM3jamrAEMCP MGX69RXb1otCpB6sjxM7d1A74EGpqB60hv1dVUkuAPMmV2uyRBPIyFUHrAeoaglaMSwAHjKO RrDZB5MZWuPmPVHznGPp1uRaLMpudh1CD1VneakecbZBRj2ajeLrVIqB3VT3t7zNdpWBtsxH ScadphOxZekP01CtQAJBVOntliZUrSOkUKNhO40eKMKKKKACjR4oARWUrYNiIJzNGrt37IO/ uhuNtADDZPC9QYslZQ+aEr9JTbRL09XKvHxIP2noTVK3eJE2HW3siNypHOKZghpGQfWyrz8C B9pLXoSE8zI1h83Jb6zbfkav3Z2uJWvsiHJhpIzWNo7dNkAEMYulJXsSISWtV7hOtop04SsI NgJ3HigAooooAf/ZAAAAAAAAAAA=";                              
                                break;
                            case DrawingType.NonChemicalContent:
                                cl[i].Compound.BaseFragment.Structure.Value = cl[i].Compound.BaseFragment.Structure.NormalizedStructure = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBk NjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAEBbAADAYwAzM2kAzAz6AAEJ CAAAAPT/AAD0/wIJCAAAAOEAAADhAA0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8 BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsI CAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAA HgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAk AAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAA AP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJp YWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAQAAAAEAhAAAAAAAAAAAAAAANACAAAc AhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoACAAAAAAIIAAAAZgAA wGMABAIQAABAWwAAwGMAMzNpAMwM+gAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4n dCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAAByAAAQAAAAQAAADwAAMATk9O IENIRU1JQ0FMIENPTlRFTlQAAAAAAAAAAA==";                              
                                break;
                            default:
                                break;
                        }
                      
                     }

                 }
             }
         
        public void UpdateNormalizedPriorToInsert()
        {  //this is to address the issue where normalization is turned off and the normalizedstructure field might be blank. this should not be allowed to occur
            //because there are so many dependencies on that field.

            ComponentList cl = this.ComponentList;
            if ((AllowUnregisteredComponents == false && cl.Count == 1) || AllowUnregisteredComponents == true)
            {
                for (int i = 0; i < cl.Count; i++)
                {
                    if (string.IsNullOrEmpty(cl[i].Compound.BaseFragment.Structure.NormalizedStructure))
                    {
                        if (cl[i].Compound.BaseFragment.Structure.DrawingType == 0)
                        {
                            cl[i].Compound.BaseFragment.Structure.NormalizedStructure = cl[i].Compound.BaseFragment.Structure.Value;
                            cl[i].Compound.BaseFragment.Structure.UseNormalizedStructure = false;
                        }
                        else
                        {
                            //update to correct xml when drawingtype <>0
                            switch (cl[i].Compound.BaseFragment.Structure.DrawingType)
                            {
                                
                                case DrawingType.NoStructure:
                                    cl[i].Compound.BaseFragment.Structure.Value = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBkNjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAIBsAABAYgAzc3oAM7O8AAEJCAAAAAAAAAAAAAIJCAAAAOEAAAAsAQ0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsICAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAAHgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgBgAAAAEAhAAAAAAAAAAAAAAANACAAAcAhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoAWAAAAAAIIAABAdwAAQGIABAIQAACAbAAAQGIAM3N6ADOzvAAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4ndCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAABxgAAQAAAAQAAADwAAMATk8gU1RSVUNUVVJFAAAAAAAAAAA=";
                                    cl[i].Compound.BaseFragment.Structure.NormalizedStructure = cl[i].Compound.BaseFragment.Structure.Value;
                                    break;
                                case DrawingType.Unknown:
                                    cl[i].Compound.BaseFragment.Structure.Value = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAIAAsAAABz bWFsbC5jZHgEAhAAAEBTAAAA3gDMDIgAzMwSAQEJCAAAQBEAAAADAAIJCAAAALkBAEAzAg0I AQABCAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABE BAEAAAoICAADAGAAyAADAAsICAADAAAAyAADAAkIBAAAgAIACAgEAJmZAQAHCAQAmZkAAAYI BAAAAAIABQgEAGZmDgAECAIAtAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAEC CBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAA AP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABDwAAAAEAAwDkBAUAQXJpYWwACHgA AAMAAAEgASAAAAAAC2YIoP+E/4gL4wkYA2cFJwP8AAIAAAEgASAAAAAAC2YIoAABAGQAZAAA AAEAAQEBAAAAAScPAAEAAQAAAAAAAAAAAAAAAAACABkBkAAAAAAAYAAAAAAAAAAAAAEAAAAA AAAAAAAAAAAAAAAAAYAIAAAABAIQAAAAAAAAAAAAAMDPAgAAHAIWCAQAAAAkABgIBAAAACQA GQgAABAIAgABAA8IAgABAAmAAgAAAAQCEAAAQFMAAADeAMwMiADMzBIBCgACAAEAcQp+CP/Y /+AAEEpGSUYAAQEBAEgASAAA/9sAQwAKBwcIBwYKCAgICwoKCw4YEA4NDQ4dFRYRGCMfJSQi HyIhJis3LyYpNCkhIjBBMTQ5Oz4+PiUuRElDPEg3PT47/9sAQwEKCwsODQ4cEBAcOygiKDs7 Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7/8AAEQgA gACAAwEiAAIRAQMRAf/EABsAAAEFAQEAAAAAAAAAAAAAAAUAAQMEBgIH/8QAOBAAAgIBAgME CAMHBQAAAAAAAQIAAwQFEQYSISIxQVETMkJhcYGxwQcU0RVScpGSoeEjM0NTVP/EABoBAAID AQEAAAAAAAAAAAAAAAACAwQFAQb/xAAeEQADAAIDAQEBAAAAAAAAAAAAAQIDERIhMQRRQf/a AAwDAQACEQMRAD8A9miiigAooooAKKNOLLq6lLO4UDzMAO48AZvGeh4LFXzUdx7NfaP9oLs/ EnS1OyUZLjzFe31kk4sleIV3K9Zs40x1f4k6Wx2soyUHma9/pCmFxlomcwWvNRXPsv2T/eFY sk+o4rl+MPRSOu6u1QUcMD5GdyMceKKKACiiigAooooAKcswUbk7CJmCjcnYTz7iziy7LyW0 nSXIAPLbcp67+IB+pkmLFWWuMiXahbYU4g46x9PsbEwE/NZQ6HY9lD7z9hMvZia5xA3pdSy2 qqPX0fUD+kfePpen04ShuUNZ4sfD4Qwt2wmtHzzjXS2/0o1md+vSKNHDGnUIOfntPvPKP5Cd 2aXgINlxax8pba/pK9lm/jLEzT9ZA6kHXaXiv3VBf4TBeXpgq7u0p8/CH2aVbxzKZZhfpDT/ AAGYGqapo7g4eU4Uf8bnmU/Lw+U3fD/HONqTri5qjGyT0AJ7L/A/aYp8feU8jE3HQSv9HxY8 i2umTYvqqen2j21WDDcHcTqedcI8X20XJpup2Fg3Sq5vof1noaOHUFTuDMDLirHXGjVi1a2j qKKKRjijR5HbYKqmsY7BRvADKcdcQtpuGuDivtk5II3HsL4mYrSKFrQuR1PSRannNrGtZGax JVm5a/cg7v1+ctYnYXlPdPRfJ86jHt+syPozcr0vEElcbTr0khSPY4qqZ27gJYaSK+2zt7lQ czsFHvlZtQoB6Fm+Ag6yx7nLuev0nBKjx3PkJVrM99FhY1/Qn+0Mc9CWX4idB0uXetgy+YgZ iW6d3uj0u9Fges7H6zs/Q0+xaxJ+Bc1yN6dxLVJF9K2KOjD+Uc1y4r2V+OgHlYu4PSbngniF szHODlPvkUdNz7S+Bmaup3BlHFyH0rVacxDsFbZ/ep75S+zEskb/AKi382ThWn4z2QHePKuD kDIx0cHfcbyzPPmsPAHGeacHhrLdDs7JyL8T0+8PzG/iRYRo1NY7nvXf6/aSYp5WkLb1LZgs KoAADuHSFq06SnhJ2RCdaT1SekYD9HRZHm0X3UBKuva3PXaWkSTqkht7Whp6ezPNp+WO+lz8 OsJ6Xheio9I6bWMT3jqBCapKWoZ35femn/d8T+7/AJlXUx2T7q+ilrAq561Xb0g35tvLw3g3 lkxBYkkkk95Mt4OnWZj77Fah6zfYSu65V0TJcUW9LrP5BSe4sSJaNctrStaBFGyqNgJya5ci tLRWpbeyjZX0gjPo3U9JoHSC86vsmO62hUuzVcGZxyNJqDHdlHKfiOk1Q6iYDgiwolqeAtM3 qHdRPO5Fq2jbl7lM7mN/Easto9L+CXrv9PvNlAHGWGczh3JVRuyrzr8R1+07irjaYWty0efY Q3UQpWsFaewZQR49YZqG4npFXRhtdkiLJ0SMiywiyGqGSGVOm/lMs5NljOx3LEkzX8vLWzbb 7AnaCV1mr/wJ/MfpKeWk/wCljGvxFDTsNczMWtj2AOZtvKaZaVRAiKFUdAB4Srp2oJl5QqXF WslSeYH/ABC3o/dFikvDtb2UzXI2SXWrkbJJVZG0UXSCs8dkw3auwgPUnCq2/cJJy6OKey3w cuxsbztM9Aq9QTE8J0FMarcbFu0fn1m3rHZExcj3TZrStSkdyHIqFtLIRuGG0mjGIMeRX4ra Xq1+Gw2CNunvU936fKFsZgwEL8aaK2RWufjJvdT3ge0viJm9PyQyjY982fnzc40/UZmfFxra DdYlmtZWoYMJcrEktkKR06/6Fn8B+kyagbCbRFk9dNf/AFr/AEiU8nZPD0Znh9QdUX+BpqfR yVKUHUIoPuEkNchVaHfZSZJC6y667SnewUGSzQjQPym5VMzmYDlZSY6+2e18PGFdTy1RGJOw Eg0bCey05Ninns7gfZE7lycZJMOPdbNLoePygdNgBNEo6Sjp2P6KkdJemcXh40eKAEV1QtQq w3Bnn2v6FbpuU+ZiIWqY72VqO4+Y/Seiyvk4yXoVYA7x4tw9oWpVLTPPMHOV1BDAgwzRcpAl bV+GWqubIwj6Jydyu3Zb4j7wUmbfhNyZdbVEe13qfnL855pFK8Lnw1tTgy3WRM3jamrAEMCP MGX69RXb1otCpB6sjxM7d1A74EGpqB60hv1dVUkuAPMmV2uyRBPIyFUHrAeoaglaMSwAHjKO RrDZB5MZWuPmPVHznGPp1uRaLMpudh1CD1VneakecbZBRj2ajeLrVIqB3VT3t7zNdpWBtsxH ScadphOxZekP01CtQAJBVOntliZUrSOkUKNhO40eKMKKKKACjR4oARWUrYNiIJzNGrt37IO/ uhuNtADDZPC9QYslZQ+aEr9JTbRL09XKvHxIP2noTVK3eJE2HW3siNypHOKZghpGQfWyrz8C B9pLXoSE8zI1h83Jb6zbfkav3Z2uJWvsiHJhpIzWNo7dNkAEMYulJXsSISWtV7hOtop04SsI NgJ3HigAooooAf/ZAAAAAAAAAAA=";
                                    cl[i].Compound.BaseFragment.Structure.NormalizedStructure = cl[i].Compound.BaseFragment.Structure.Value;
                                    break;
                                case DrawingType.NonChemicalContent:
                                    cl[i].Compound.BaseFragment.Structure.Value = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBk NjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAEBbAADAYwAzM2kAzAz6AAEJ CAAAAPT/AAD0/wIJCAAAAOEAAADhAA0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8 BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsI CAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAA HgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAk AAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAA AP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJp YWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAQAAAAEAhAAAAAAAAAAAAAAANACAAAc AhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoACAAAAAAIIAAAAZgAA wGMABAIQAABAWwAAwGMAMzNpAMwM+gAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4n dCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAAByAAAQAAAAQAAADwAAMATk9O IENIRU1JQ0FMIENPTlRFTlQAAAAAAAAAAA==";
                                    cl[i].Compound.BaseFragment.Structure.NormalizedStructure = cl[i].Compound.BaseFragment.Structure.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
            }
        }
        

        public  void  CheckForChangeInFragments()
            {
                foreach (Batch currentBatch in _batchList)
                {
                    foreach (BatchComponent currentBatchComponent in currentBatch.BatchComponentList)
                    {
                        if (currentBatchComponent.BatchComponentFragmentList.IsDirty ){
                            currentBatch.MarkDirty();
                            break;
                        }
                    }
                }
                
            }
      
        public void SetApprovalStatus()
        {
            SetApprovalStatusCommand.Execute(this.ID, this.Status, COEUser.ID);
        }
        public void SetLockStatus()
        {
            SetLockedStatusCommand.Execute(this.ID, this.Status, COEUser.ID);
        }

        [COEUserActionDescription("ReplaceCompoundInRegistryRecord")]
        public void ReplaceCompound(int index, Compound newCompound)
        {
            try
            {
                Component newComponent = Component.NewComponent();
                foreach (Batch batch in _batchList)
                {
                    foreach (BatchComponent currentBatchComponent in batch.BatchComponentList)
                    {
                        if (currentBatchComponent.ComponentIndex == ComponentList[index].ComponentIndex)
                        {
                            currentBatchComponent.RegNum = !string.IsNullOrEmpty(newCompound.RegNumber.RegNum) ? newCompound.RegNumber.RegNum : String.Empty;
                            //currentBatchComponent.ComponentIndex = newCompound.ID;
                            newComponent.ComponentIndex = currentBatchComponent.ComponentIndex;
                            currentBatchComponent.CompoundID = newCompound.ID;
                            break;
                        }
                    }
                }

                this.ComponentList[index].Compound.BaseFragment.Structure.MarkClean();
                newCompound.FragmentList = this.ComponentList[index].Compound.FragmentList;
                this.ComponentList.RemoveAt(index);
                newComponent.Compound = newCompound;
                this.ComponentList.Insert(index, newComponent);

                OnLoaded(this, new EventArgs());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("ReplaceCompoundInRegistryRecord")]
        public void ReplaceCompound(int index, Compound newCompound, BatchComponentFragmentList newBCFragmentList)
        {
            try
            {
                Component newComponent = Component.NewComponent();
                foreach (Batch batch in _batchList)
                {
                    foreach (BatchComponent currentBatchComponent in batch.BatchComponentList)
                    {
                        if (currentBatchComponent.ComponentIndex == ComponentList[index].ComponentIndex)
                        {
                            currentBatchComponent.RegNum = !string.IsNullOrEmpty(newCompound.RegNumber.RegNum) ? newCompound.RegNumber.RegNum : String.Empty;
                            //currentBatchComponent.ComponentIndex = newCompound.ID;
                            newComponent.ComponentIndex = currentBatchComponent.ComponentIndex;
                            currentBatchComponent.CompoundID = newCompound.ID;
                            currentBatchComponent.BatchComponentFragmentList = newBCFragmentList;
                            break;
                        }
                    }
                }

                this.ComponentList[index].Compound.BaseFragment.Structure.MarkClean();
                //newCompound.FragmentList = this.ComponentList[index].Compound.FragmentList;
                this.ComponentList.RemoveAt(index);
                newComponent.Compound = newCompound;
                this.ComponentList.Insert(index, newComponent);

                OnLoaded(this, new EventArgs());

            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("ReplaceCompoundInRegistryRecord")]
        public void ReplaceComponent(int index, Component newComponent)
        {
            try
            {
                Component componentToDelete = this.ComponentList[index];

                //This is a workaround to a problem with SP
                this.ComponentList[index].Compound.BaseFragment.Structure.MarkClean();

                foreach (Batch currentBatch in this.BatchList)
                {
                    foreach (BatchComponent currentBatchComponent in currentBatch.BatchComponentList)
                    {
                        if (currentBatchComponent.ComponentIndex == componentToDelete.ComponentIndex)
                        {
                            currentBatch.BatchComponentList.Remove(currentBatchComponent);

                            BatchComponent newBatchComponent = BatchComponent.NewBatchComponent();
                            newBatchComponent.PropertyList = currentBatchComponent.PropertyList;
                            newBatchComponent.RegNum = !string.IsNullOrEmpty(newComponent.Compound.RegNumber.RegNum) ? newComponent.Compound.RegNumber.RegNum : String.Empty;
                            newBatchComponent.ComponentIndex = newComponent.ComponentIndex;
                            newBatchComponent.CompoundID = newComponent.Compound.ID;
                            newBatchComponent.BatchID = currentBatchComponent.BatchID;
                            newBatchComponent.BatchComponentFragmentList = currentBatchComponent.BatchComponentFragmentList;
                            newBatchComponent.BatchComponentFragmentList.MarkNew();
                            foreach (BatchComponentFragment bcFragments in newBatchComponent.BatchComponentFragmentList)
                            {
                                bcFragments.ID = 0;
                            }
                            // Undo the changes for original BatchComponent.and update for new BatchComponent
                            // Note : You dont require to carry deleted BatchComponentFragment list in BatchComponent when it is NEW.
                            BatchComponentFragmentList newFrgList = newBatchComponent.BatchComponentFragmentList;
                            newFrgList.ClearDeletedList();
                            currentBatchComponent.BatchComponentFragmentList.ClearDeletedList();
                            currentBatch.BatchComponentList.Add(newBatchComponent);
                            break;
                        }
                    }
                }
               
                this.ComponentList[index].Compound.BaseFragment.Structure.MarkClean();

                this.ComponentList.RemoveAt(index);
                newComponent.Compound.BaseFragment.Structure.ID = 0;
                this.ComponentList.Insert(index, newComponent);

                OnLoaded(this, new EventArgs());

            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        private void CheckOtherMixturesAffected()
        {
            foreach (Component component in _componentList)
            {
                if (component.Compound.IsDirty && !component.Compound.IsNew)
                {
                    string[] regNumbersAffected = this.RegDal.GetRecordsThatContainsCompound(component.Compound.ID);
                    string[] regNumberAffectedByLinkedStructure = new string[] { };
                    int currentStructureId = component.Compound.BaseFragment.Structure.ID;
                    //We can skip this check if it entails a non-user-editable structure
                    if (currentStructureId > -1)
                    {
                        bool anyDirtyStructure = false;
                        foreach (Component comp in this.ComponentList)
                        {
                            if (comp.Compound.BaseFragment.Structure.IsDirty)
                            {
                                anyDirtyStructure = true;
                                break;
                            }
                        }
                        if (anyDirtyStructure)
                            regNumberAffectedByLinkedStructure = this.RegDal.GetRecordsThatContainsStructure(currentStructureId, true);
                    }
                    if (regNumbersAffected.Length > 1)
                        _regsAffectedByCompoundUpdate.Add(component.Compound.RegNumber.RegNum, regNumbersAffected);

                    if (regNumberAffectedByLinkedStructure.Length > 1)
                        _regsAffectedByStructureUpdate.Add(component.Compound.RegNumber.RegNum, regNumberAffectedByLinkedStructure);
                }
            }
        }

        [COEUserActionDescription("DeleteCompoundFromRegistryRecord")]
        public void DeleteComponent(int componentIndex)
        {
            try
            {
                Component componentToDelete = this.ComponentList[componentIndex];

                //This is a workaround to a problem with SP
                this.ComponentList[componentIndex].Compound.BaseFragment.Structure.MarkClean();

                this.ComponentList.RemoveAt(componentIndex);
                foreach (Batch currentBatch in this.BatchList)
                {
                    foreach (BatchComponent currentBatchComponent in currentBatch.BatchComponentList)
                    {
                        if (currentBatchComponent.ComponentIndex == componentToDelete.ComponentIndex)
                        {
                            currentBatch.BatchComponentList.Remove(currentBatchComponent);
                            break;
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("UpdateEditedComponents")]
        public void CopyEditedComponents()
        {
            try
            {
                for (int index = 0; index < this.ComponentList.Count; index++)
                {
                    Component currentComponent = this.ComponentList[index];
                    
                    _potentiallyCopiedCompoundsIds.Add(currentComponent.Compound.ID);
                    if (currentComponent.IsDirty)
                    {
                        Component newComponent = Component.NewComponent(currentComponent.UpdateSelf(false), true, true);
                        newComponent.Compound.RegNumber = RegNumber.NewRegNumber();
                        newComponent.Compound.RegNumber.Sequence = currentComponent.Compound.RegNumber.Sequence;
                        


                        newComponent.Compound.FragmentList = FragmentList.NewFragmentList();
                        newComponent.ComponentIndex = currentComponent.ComponentIndex - 1;

                        foreach (Identifier currentIdentifier in currentComponent.Compound.IdentifierList)
                            currentIdentifier.ID = 0;
                        if (newComponent.Compound != null)
                            newComponent.Compound.ID = 0; //Setting the new compound id to 0 becuse we creating the copy of the component.
                        this.ReplaceComponent(index, newComponent);

                        this.UpdateFragments();

                        // Undo the changes for original component.and update for new component
                        List<Fragment> deletedFragments = currentComponent.Compound.FragmentList.GetDeletedList();
                        FragmentList newFrgList = newComponent.Compound.FragmentList;
                        newFrgList.ClearDeletedList(); 
                        foreach (Fragment fg in deletedFragments)
                        {
                            Fragment f = newFrgList.GetByID(fg.FragmentID);
                            if (f != null)
                                newFrgList.Remove(f);
                        }
                        currentComponent.Compound.FragmentList.ClearDeletedList();
                    }
                }
                
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }

        }

        [COEUserActionDescription("UpdateEditedStructures")]
        public void CopyEditedStructures()
        {
            try
            {
                //CSBR:160456-Loop through the whole list to find which component is modified
                for (int index = 0; index < this.ComponentList.Count; index++)
                {
                    Component currentComponent = this.ComponentList[index];
                    //It is not possible to link structures when EnabledMixtures = true, that's why the only component we checked is the component index = 0; 
                    if (currentComponent.Compound.BaseFragment.Structure.IsDirty)
                    {
                        Structure newStructure = Structure.NewStructure(currentComponent.Compound.BaseFragment.Structure.UpdateSelf(false), true, true);
                        //force new structure
                        newStructure.ID = 0;
                        this.ComponentList[index].Compound.BaseFragment.ReplaceStructure(newStructure, true);
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        public void BatchPrefixDefaultOverride(bool isWebserviceCall)
        {
            if (isWebserviceCall || _dataStrategy == DataAccessStrategy.BulkLoader)
            {
                if (RegSvcUtilities.GetEnableBatchPrefix() && RegSvcUtilities.GetDefaultBatchPrefix() > 0)
                {
                    foreach (Batch batch in _batchList)
                    {
                        if (string.IsNullOrEmpty(batch.PropertyList["BATCH_PREFIX"].Value))
                        {
                            batch.PropertyList["BATCH_PREFIX"].Value = RegSvcUtilities.GetDefaultBatchPrefix().ToString();
                        }
                    }
                }
            }
 
        }
        public void ApproverForBulkLoading(bool isWebserviceCall)
        {
            if (isWebserviceCall || _dataStrategy == DataAccessStrategy.BulkLoader)
            {
                //setApprover
                this.ComponentList[0].Compound.PersonApproved = COEUser.ID;
                this.BatchList[0].PersonApproved = COEUser.ID;
            }
        }

        [COEUserActionDescription("GetRegisteredInfoFromTempBatchID")]
        public string GetRegisteredInfoFromTempBatchID(int tempBatchID)
        {
            try
            {
                string xml = this.RegDal.FetchRegisteredInfoFromTempBatchID(tempBatchID);
                return xml;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetRegNumberByBatchId")]
        public static string GetRegNumberByBatchId(string batchId)
        {
            string result = null;
            try
            {
                RegistrationOracleDAL regDal = null;
                DalUtils.GetRegistrationDAL(ref regDal, Constants.SERVICENAME);
                result = regDal.GetRegNumberByBatchId(batchId);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        [COEUserActionDescription("GetCompoundLockedStatus")]
        public static int GetCompoundLockedStatus(int  compoundid)
        {
            int result =0;
            try
            {
                RegistrationOracleDAL regDal = null;
                DalUtils.GetRegistrationDAL(ref regDal, Constants.SERVICENAME);
                result = regDal.GetCompoundLockStatus(compoundid);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        [COEUserActionDescription("GetRegisteryLockedStatus")]
        public static string GetLockedRegistryRecords(string regNumberList)
        {
            string result = string.Empty;
            try
            {
                RegistrationOracleDAL regDal = null;
                DalUtils.GetRegistrationDAL(ref regDal, Constants.SERVICENAME);
                result = regDal.GetLockedRegistryList(regNumberList);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        public static void EndBulkLoadingStrategy()
        {
            try
            {
                if ((System.Diagnostics.Process.GetCurrentProcess().ProcessName == "DataLoader.exe" && System.Diagnostics.Process.GetProcessesByName("DataLoader.exe").Length > 0) || (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "DataLoaderGUI.exe" && System.Diagnostics.Process.GetProcessesByName("DataLoaderGUI.exe").Length > 0))
                {
                    string msg = "EndBulkLoadingStrategy";
                    _coeLog.LogStart(msg);
                    RegistrationOracleDAL regDal = null;
                    DalUtils.GetRegistrationDAL(ref regDal, Constants.SERVICENAME);
                    regDal.EndBulkLoadingStrategy();
                    _coeLog.LogEnd(msg);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        public void CleanFoundDuplicates()
        {
            this._foundDuplicates = string.Empty;
            _actionDuplicates = DuplicateAction.None;
            _actionDuplicateTaken = DuplicateAction.None;
        }

        /// <summary>
        /// Replace the compound structure in component index = <paramref name="componentIndex"/>
        /// </summary>
        /// <param name="componentIndex">the index of the component to modify</param>
        /// <param name="newStrucrure">the structure to replace for</param>
        public void ReplaceStructure(int componentIndex, Structure structure)
        {
            if (this.IsTemporal)
                this.ComponentList[componentIndex].Compound.BaseFragment.ReplaceStructure(structure, true);
            else
                this.ComponentList[componentIndex].Compound.BaseFragment.ReplaceStructure(structure, false);
        }
        /// <summary>
        /// Removes from database log those componds ids that have already been copied
        /// </summary>
        /// <param name="errorNode">database log xml</param>
        private void RemovePotentiallyCopiedCompounds(ref XmlNode errorNode)
        {
            if (errorNode.SelectSingleNode("//COMPOUND//REGISTRYLIST") != null)
            {
                int i = errorNode.SelectSingleNode("//COMPOUND//REGISTRYLIST").ChildNodes.Count - 1;
                while (i >= 0)
                {
                    int compoundId = 0;
                    int.TryParse(errorNode.SelectSingleNode("//COMPOUND//REGISTRYLIST").ChildNodes[i].Attributes["CompoundID"].Value, out compoundId);
                    if (_potentiallyCopiedCompoundsIds.Contains(compoundId))
                        errorNode.SelectSingleNode("//COMPOUND//REGISTRYLIST").RemoveChild(errorNode.SelectSingleNode("//COMPOUND//REGISTRYLIST").ChildNodes[i]);
                    i--;
                }
            }
            this._potentiallyCopiedCompoundsIds.Clear();
        }
        /// <summary>
        /// Removes from database log those componds ids that have already been duplicated
        /// </summary>
        /// <param name="errorNode">database log xml</param>
        private void RemoveResolvedDuplicates(ref XmlNode errorNode)
        {
            List<int> alreadyDuplicateCompoundIdList = new List<int>();
            int[] compoundIdList;
            foreach (Component component in this.ComponentList)
            {
                if (!string.IsNullOrEmpty(component.Compound.RegNumber.RegNum) && component.Compound.RegNumber.RegNum != "N/A")
                {
                    compoundIdList = RegDal.GetAlreadyDuplicate(component.Compound.RegNumber.RegNum);
                    foreach (int compoundId in compoundIdList)
                    {
                        alreadyDuplicateCompoundIdList.Add(compoundId);
                    }
                }
            }
            foreach (XmlNode compoundNode in errorNode.ChildNodes[0].ChildNodes)
            {
                if (compoundNode.SelectSingleNode("./REGISTRYLIST") != null)
                {
                    int i = compoundNode.SelectSingleNode("./REGISTRYLIST").ChildNodes.Count - 1;
                    while (i >= 0)
                    {
                        int compoundId = 0;
                        int.TryParse(compoundNode.SelectSingleNode("./REGISTRYLIST").ChildNodes[i].Attributes["CompoundID"].Value, out compoundId);
                        if (alreadyDuplicateCompoundIdList.Contains(compoundId))
                            compoundNode.SelectSingleNode("./REGISTRYLIST").RemoveChild(compoundNode.SelectSingleNode("./REGISTRYLIST").ChildNodes[i]);
                        i--;
                    }
                    if (compoundNode.SelectSingleNode("./REGISTRYLIST").ChildNodes.Count == 0)
                        compoundNode.RemoveChild(compoundNode.SelectSingleNode("./TEMPCOMPOUNDID"));
                }

            }




        }

        /// <summary>
        /// Validates project list have one project at lease when RLSStatus != Off
        /// </summary>
        private bool CheckRLSRequirements()
        {
            switch (RLSStatus)
            {
                case RLSStatus.RegistryLevelProjects:
                    if (this.ProjectList.Count < 1)
                        throw new ValidationException(Resources.RLS_RegistryLevelProjects_ErrorMessage);
                    else
                        return true;
                    break;
                case RLSStatus.BatchLevelProjects:
                    foreach (Batch batch in this.BatchList)
                    {
                        if (batch.ProjectList.Count < 1)
                            throw new ValidationException(Resources.RLS_BatchLevelProjects_ErrorMessage);
                    }
                    return true;
                    break;
                default:
                    return true;
                    break;
            }
        }

        /// <summary>
        /// Public utility method invoking the internal overload via a Csla command object.
        /// </summary>
        /// <param name="registration">any registry record</param>
        /// <param name="strategy">the data-access strategy to use</param>
        /// <returns></returns>
        [COEUserActionDescription("RegistrationFindDuplicates")]
        public static List<DuplicateCheckResponse> FindDuplicates(
            RegistryRecord registration, DataAccessStrategy strategy)
        {
            List<RegistryRecord> regList = new List<RegistryRecord>();
            regList.Add(registration);

            try
            {
                List<DuplicateCheckResponse> results =
                    DuplicateCheckRecordsCommand.CheckAll(regList, registration._dataStrategy);
                return results;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Intended initially for bulk-loading scenarios where the RegistryRecord is expected
        /// to have just one component to use for the duplicate-checking itself.
        /// <para>
        /// NOTE: This MUST be executed from the server-side.
        /// </para>
        /// </summary>
        /// <param name="strategy">affects the initialization of the data-access layer class</param>
        /// <returns>
        /// a 'property-bag' object representing matched registrations, components and structures.
        /// </returns>
        public List<DuplicateCheckResponse> FindDuplicates(DataAccessStrategy strategy)
        {
            this._dataStrategy = strategy;
            List<DuplicateCheckResponse> responses = new List<DuplicateCheckResponse>();

            try
            {
                responses = RegSvcUtilities.FindDuplicates(this);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }

            _registeredMatches = responses;
            return responses;
        }

        /// <summary>
        /// Adds the fragment to the appropriate Component's fragment list, as well as the associated
        /// BatchComponentFragment list.
        /// </summary>
        /// <param name="componentIndex">the component to add the fragment will be added</param>
        /// <param name="fragmentCode">the look-up code for the fragment</param>
        /// <param name="equivalents">the number of molecular equivalents to add</param>
        public void AddFragment(int componentIndex, string fragmentCode, float equivalents)
        {
            if (string.IsNullOrEmpty(fragmentCode))
                return;

            try
            {
                FragmentList allFragments = FragmentList.GetFragmentList(this.DataStrategy != DataAccessStrategy.Atomic);
                Fragment matchedFragment = allFragments.GetByCode(fragmentCode);
                if (matchedFragment == null)
                    matchedFragment = allFragments.GetByDesc(fragmentCode);
                if (matchedFragment != null)
                    AddFragment(componentIndex, matchedFragment, equivalents);
                else
                    throw new BusinessObjectNotFoundException(fragmentCode, typeof(Fragment));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Adds the fragment to the appropriate Component's fragment list, as well as the associated
        /// BatchComponentFragment list.
        /// </summary>
        /// <param name="componentIndex">the component to add the fragment will be added</param>
        /// <param name="fragmentToAdd">the actual fragment instance to add</param>
        /// <param name="equivalents">the number of molecular equivalents to add</param>
        public void AddFragment(int componentIndex, Fragment fragmentToAdd, float equivalents)
        {
            try
            {
                RegSvcUtilities.AddFragment(this, componentIndex, fragmentToAdd, equivalents);
                this.MarkDirty();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given the Identifier Id and a value, adds a new Identifier object to the Registration's
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
                    , IdentifierTypeEnum.R);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given an Identifier name and a value, adds a new Identifier object to the Registration's
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
                    , IdentifierTypeEnum.R);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given an Identifier instance, adds the Identifier to the Registration's IdentifierList.
        /// </summary>
        /// <param name="identifier">an Identifier instance</param>
        public void AddIdentifier(Identifier identifier)
        {
            try
            {
                RegSvcUtilities.CreateNewIdentifier(
                    this.IdentifierList
                    , identifier
                    , IdentifierTypeEnum.R);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Given a Project name, adds a new Project object to the Registration's ProjectList.
        /// </summary>
        /// <param name="projectName">the internal name of the project</param>
        public void AddProject(string projectName)
        {
            RegSvcUtilities.AddProject(this.ProjectList, projectName, ProjectList.ProjectTypeEnum.R);
        }

        /// <summary>
        /// Given a Project ID, adds a new Project object to the Registration's ProjectList.
        /// </summary>
        /// <param name="projectId">the internal ID of the project</param>
        public void AddProject(int projectId)
        {
            RegSvcUtilities.AddProject(this.ProjectList, projectId, ProjectList.ProjectTypeEnum.R);
        }

        /// <summary>
        /// Provides a 'default' structure value (and id) for a given Component when no structure has
        /// already been assigned.
        /// </summary>
        /// <param name="componentIndex">the component to add the fragment will be added</param>
        /// <param name="structureId">the structure's ID value</param>
        public void AssignDefaultStructureById(int componentIndex, int structureId)
        {
            try
            {
                RegSvcUtilities.AssignDefaultStructureById(this, componentIndex, structureId);
                this.MarkDirty();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Checks the component edition scope of the current user
        /// </summary>
        /// <param name="componentIndex">the index of the component to check</param>
        /// <returns></returns>
        public bool CanEditComponent(int componentIndex)
        {
            return RegSvcUtilities.CanUserEditRegistration(this.ComponentList[componentIndex].Compound.PersonCreated, this.IsTemporal);
        }

        /// <summary>
        /// Checks the registry edition scope of the current user
        /// </summary>
        /// <param name="componentIndex">the index of the component to check</param>
        /// <returns></returns>
        public bool CanEditRegistry()
        {
            return RegSvcUtilities.CanUserEditRegistration(this.PersonCreated, this.IsTemporal);
        }

        /// <summary>
        /// Checks the batch edition scope of the current user
        /// </summary>
        /// <param name="batchIndex">the index of the batch to check</param>
        /// <returns></returns>
        public bool CanEditBatch(int batchIndex)
        {
            return RegSvcUtilities.CanUserEditRegistration(this.BatchList[batchIndex].PersonCreated, this.IsTemporal);
        }
        /// <summary>
        /// Checks the whether current user can propogate structrue edits
        /// </summary>
        /// <returns></returns>
        public bool CanPropogateStructureEdits()
        {
            return RegSvcUtilities.CanUserPropogateStructureEdits();
        }

        /// <summary>
        /// Checks the whether current user can propogate component edits
        /// </summary>
        /// <returns></returns>
        public bool CanPropogateComponentEdits()
        {
            return RegSvcUtilities.CanUserPropogateComponentEdits();
        }


        /// <summary>
        /// Checks the whether current user can create duplicate component 
        /// </summary>
        /// <returns></returns>
        public bool CanCreateDuplicateComponent()
        {
            return RegSvcUtilities.CanUserCreateDuplicateComponent();
        }

        /// <summary>
        /// Checks the whether current user can create duplicate registry 
        /// </summary>
        /// <returns></returns>
        public bool CanCreateDuplicateRegistry()
        {
            return RegSvcUtilities.CanUserCreateDuplicateRegistry();
        }

        /// <summary>
        /// Checks the whether current prefix calls for auto selecting component during duplicate checking of mixtures 
        /// </summary>
        /// <returns></returns>
        public bool CanAutoSelectComponentForDupChk()
        {
            return RegSvcUtilities.CanAutoSelectComponentForDupChk(this.RegNumber.Sequence);
        }

        /// <summary>
        /// This method is required for Chemical warnings addin. It will create and add a node with module name.
        /// This will help to identify calling module and addin can take proper action as per module configuration.
        /// </summary>
        /// <param name="xml">Registration xml</param>
        /// <param name="moduleName">modulename as string </param>
        /// <returns></returns>
        public string AddNewNode(string xml, string moduleName)		//CBOE-1159 added method adding new module name : ASV 10JUL13
        {
            try
            {
                if (!string.IsNullOrEmpty(xml) && !string.IsNullOrEmpty(moduleName))
                {
                    XmlDocument temp = new XmlDocument();
                    temp.LoadXml(xml);
                    XmlNode newElem;
                    newElem = temp.CreateNode(XmlNodeType.Element, "ModuleName", "");
                    newElem.InnerText = moduleName;
                    XmlElement root = temp.DocumentElement;
                    root.AppendChild(newElem);
                    return temp.InnerXml;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return xml;
        }
        #endregion

        #region Validation Rules

        [COEUserActionDescription("GetRegistryRecordBrokenRules")]
        public List<BrokenRuleDescription> GetBrokenRulesDescription()
        {
            try
            {
                List<BrokenRuleDescription> brokenRules = new List<BrokenRuleDescription>();

                if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
                {
                    brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
                }                

                this._componentList.GetBrokenRulesDescription(brokenRules);
                this._batchList.GetBrokenRulesDescription(brokenRules);
                this._identifierList.GetBrokenRulesDescription(brokenRules);
                this._projectList.GetBrokenRulesDescription(brokenRules);
                this._propertyList.GetBrokenRulesDescription(brokenRules);

                return brokenRules;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        protected override void AddInstanceBusinessRules()
        {
            ValidationRules.AddInstanceRule(this.ValidateProjectListBasedOnRLSStatus, "ProjectList");
            ValidationRules.AddInstanceRule(this.ValidateBatchPrefixStatus, "BatchList");
            ValidationRules.AddInstanceRule(this.ValidateLegacySaltAndBatchSuffix, "LegacySaltAndBatchSuffix");
            ValidationRules.AddInstanceRule(this.ValidateRedBoxWarnings, "RedBoxWarning");
        }

        private bool ValidateProjectListBasedOnRLSStatus(object sender, RuleArgs args)
        {
            bool isValid = RLSStatus == RLSStatus.RegistryLevelProjects ? (_projectList != null && _projectList.Count > 0) : true;

            if (!isValid)
            {
                args.Description = "There has to be at least 1 Registry project when RLS is set to Registry-level";
            }

            return isValid;
        }

        /// <summary>
        /// Validating bacth prefix on Registration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool ValidateBatchPrefixStatus(object sender, RuleArgs args)
        {
            bool isValid = true;
            
            if (!(this.IsTemporal) && EnableBatchPrefix && this.RegNumber.Sequence.Prefix == "" && this._dataStrategy != DataAccessStrategy.BulkLoader)
            {
                foreach (Batch currentbatch in this.BatchList)
                {
                    if (currentbatch.PropertyList["BATCH_PREFIX"] != null && string.IsNullOrEmpty(currentbatch.PropertyList["BATCH_PREFIX"].Value))
                    {
                        isValid = false;
                        args.Description = "Batch Prefix is required";
                        break;
                    }
                }
            }
            return isValid;
        }
        
        private bool ValidateRedBoxWarnings(object sender, RuleArgs args)
        {
            bool isValid = true;
            
            if (_submitCheckRedBoxWarning || _registerCheckRedBoxWarning)
            {
                isValid = false;
                args.Description = _redBoxWarning;             
            }            
            
            return isValid;
        }
                
        /// <summary>
        /// Validating LegacySaltAndBatchSuffix text filed aganist previous batches.  The combination of batch prefix + salt and batch prefix must be unique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool ValidateLegacySaltAndBatchSuffix(object sender, RuleArgs args)
        {
            bool isValid = true;
            Hashtable LegacySaltList = new Hashtable();
            if (!(this.IsTemporal) && EnableBatchPrefix && this.RegNumber.Sequence.Prefix == "" && this._dataStrategy != DataAccessStrategy.BulkLoader)
            {
                //first go through and check if any of the batches have saltandbatchsuffix populated. this means it is legacy so all batches need to be checked for 
                //for uniqueness
                bool isLegacy = false;
               foreach (Batch currentbatch in this.BatchList)
                {
                   if (currentbatch.PropertyList["SALTANDBATCHSUFFIX"].Value.Length>0){
                       isLegacy = true;
                       break;
                   }
                }

                if(isLegacy){
                foreach (Batch currentbatch in this.BatchList)
                {
                  string saltbatchsuffix = currentbatch.PropertyList["SALTANDBATCHSUFFIX"].Value.ToUpper();
                  string batchprefix = currentbatch.PropertyList["BATCH_PREFIX"].Value.ToUpper();
                  string fullValueToCheck = batchprefix + saltbatchsuffix;
                  if (saltbatchsuffix != null && !string.IsNullOrEmpty(saltbatchsuffix))
                  {
                      if (LegacySaltList[fullValueToCheck] != null)
                      {
                          isValid = false;
                          args.Description = "Batch Prefix + Salt and Batch Suffix must be unique";
                          break;
                      }
                      else
                          LegacySaltList.Add(fullValueToCheck, fullValueToCheck);
                  }
                  else
                  {
                      isValid = false;
                      args.Description = "Salt and Batch Suffix is required";
                      break;
                  }
                }
                }
            }
            return isValid;
        }  


        #endregion


        #region Authorization Rules

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "RegistryRecord", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("multiCompoundRegistryRecord");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("multiCompoundRegistryRecord");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("multiCompoundRegistryRecord");
        }

        #endregion

        #region Factory Methods

        [COEUserActionDescription("CreateRegistryRecord")]
        public static RegistryRecord NewRegistryRecord()
        {
            try
            {
                string msg = "Retrieving base Registration object";
                _coeLog.LogStart(msg);

                RegistryRecord registryRecord = DataPortal.Create<RegistryRecord>();

                _coeLog.LogEnd(msg);

                registryRecord.MarkClean();
                return registryRecord;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private RegistryRecord()
        {
            //MarkAsChild(); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Supporting this from the web applciation directly means exposing ApplyAddIns that use OnSaving event.
        /// </remarks>
        /// <param name="name">the template's user-derived name</param>
        /// <param name="description">the template's user-derived description</param>
        /// <param name="isPublic">the scope of visibility of this template</param>
        /// <param name="formGroup">what form group to recover the XML with</param>
        /// <returns>the repository-stored COEGenericObjectStorageBO</returns>
        public COEGenericObjectStorageBO SaveTemplate(
            string name, string description, bool isPublic, int formGroup)
        {
            //JED: This 'save as template' mechanism 
            COEGenericObjectStorageBO template = null;

            try
            {
                this._status = RegistryStatus.NotSet;
                UpdateDrawingType();
                this.ApplyAddIns();
                this.OnSaving(this, new EventArgs());

                string repositoryName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                template = COEGenericObjectStorageBO.New(repositoryName);
                template.Name = name;
                template.Description = description;
                template.COEGenericObject = this.XmlWithAddIns;
                template.FormGroup = formGroup;
                template.UserName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ?
                    "unknown" : COEUser.Get();
                template.IsPublic = isPublic;
                template.Save();
            }
            catch(Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return template;
        }

        [COEUserActionDescription("GetRegistryRecord")]
        public static RegistryRecord GetRegistryRecordByBatch(int batchId)
        {
            try
            {
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException(Resources.NotAuthorizedViewRegistryRecord);
                }

                RegistryRecord result = null;
                string msg = string.Format("Retrieving Registration by Batch ID '{0}'", batchId);
                _coeLog.LogStart(msg);

                result = DataPortal.Fetch<RegistryRecord>(new BatchCriteria(batchId));

                _coeLog.LogEnd(msg);

                result.MarkOld();

                return result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetRegistryRecord")]
        public static RegistryRecord GetRegistryRecordByBatch(string batchRegNum)
        {
            try
            {
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException(Resources.NotAuthorizedViewRegistryRecord);
                }

                RegistryRecord result = null;
                string msg = string.Format("Retrieving Registration by Batch Registry Number '{0}'", batchRegNum);
                _coeLog.LogStart(msg);

                result = DataPortal.Fetch<RegistryRecord>(new BatchCriteria(batchRegNum));

                _coeLog.LogEnd(msg);

                result.MarkOld();

                return result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetRegistryRecord")]
        public static RegistryRecord GetRegistryRecord(string regNum)
        {
            try
            {
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException(Resources.NotAuthorizedViewRegistryRecord);
                }

                RegistryRecord result = null;
                string msg = string.Format("Retrieving Registration '{0}'", regNum);
                _coeLog.LogStart(msg);

                result = DataPortal.Fetch<RegistryRecord>(new Criteria(regNum));

                _coeLog.LogEnd(msg);

                result.MarkOld();

                return result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }

        }

        [COEUserActionDescription("GetRegistryRecord")]
        public static RegistryRecord GetRegistryRecord(int id)
        {
            try
            {
                RegistryRecord result = null;
                try
                {
                    string msg = string.Format("Retrieving Registration '{0}'", id.ToString());
                    _coeLog.LogStart(msg);

                    result = DataPortal.Fetch<RegistryRecord>(new TemporalCriteria(id));

                    _coeLog.LogEnd(msg);

                    result.MarkOld();
                }
                catch (Exception ex)
                {
                    _coeLog.Log("Error in 'GetRegistryRecord':/n/n" + ex.ToString(), 0, System.Diagnostics.SourceLevels.Error);
                    throw;
                }
                return result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("DeleteRegistryRecord")]
        public static void DeleteRegistryRecord(int id)
        {
            try
            {
                if (!CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a RegistryRecord");
                }
                string msg = string.Format("Deleting Registration '{0}'", id.ToString());
                _coeLog.LogStart(msg);

                DataPortal.Delete(new TemporalCriteria(id));

                _coeLog.LogEnd(msg);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("DeleteRegistryRecord")]
        public static void DeleteRegistryRecord(string regNum)
        {
            try
            {
                if (!CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a RegistryRecord");
                }
                string msg = string.Format("Deleting Registration '{0}'", regNum);
                _coeLog.LogStart(msg);

                DataPortal.Delete(new Criteria(regNum));

                _coeLog.LogEnd(msg);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("SaveRegistryRecord")]
        public bool Import()
        {
            return Register() != null;
        }

        [COEUserActionDescription("SaveRegistryRecord")]
        public override RegistryRecord Save()
        {
            try
            {
                if (IsDeleted && !CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a RegistryRecord");
                }

                else if (IsNew && !CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a RegistryRecord");
                }
                else if (!CanEditObject())
                {
                    throw new System.Security.SecurityException("User not authorized to update a RegistryRecord");
                }

                if (this.IsValid)
                {
                    // Check red box warnings
                    if ((_isTemporal && _submitCheckRedBoxWarning) || (!_isTemporal && _registerCheckRedBoxWarning))
                    {
                        RegistryRecord recordClone = this.Clone();
                        base.Save();
                        recordClone.SubmitCheckRedBoxWarning = _submitCheckRedBoxWarning;
                        recordClone.RegisterCheckRedBoxWarning = _registerCheckRedBoxWarning;
                        recordClone.RedBoxWarning = _redBoxWarning;

                        ValidationRules.CheckRules("RedBoxWarning");
                       
                        return recordClone;
                    }
                    else
                        return base.Save();
                }
                else
                    return this;
                    //return base.Save();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Checks validation rule for red box warnings
        /// </summary>
        public void CheckValidationRedBoxWarningRule()
        {
            this.ValidationRules.CheckRules("RedBoxWarning");
        }

        /// <summary>
        /// Clears red box warning properties
        /// </summary>
        public void ClearRedBoxWarningSetting()
        {
            this.SubmitCheckRedBoxWarning = false;
            this.RegisterCheckRedBoxWarning = false;
            this.RedBoxWarning = string.Empty;
            this.ValidationRules.CheckRules("RedBoxWarning");            
        }
        
        [COEUserActionDescription("SaveRegistryRecord")]
        public RegistryRecord Save(DuplicateCheck duplicateCheck)
        {
            try
            {
                return this.Save(duplicateCheck,DataAccessStrategy.Atomic);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("SaveRegistryRecord")]
        public RegistryRecord Save(DataAccessStrategy strategy)
        {
            try
            {
                this._dataStrategy = strategy;
                return Save(DuplicateCheck.None,strategy);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("SaveRegistryRecord")]
        public RegistryRecord Save(DuplicateCheck duplicateCheck, DataAccessStrategy strategy)
        {
            try
            {
                if (duplicateCheck == DuplicateCheck.PreReg)
                {
                     IMatchResponse _matchResponse = RegistryDuplicateChecker.DupCheck(this);
                        if (_matchResponse.MatchedItems.Count > 0)
                        {
                            _foundDuplicates = _matchResponse.DuplicateXML;
                            return this;
                        }
                        else{

                            this._checkDuplicates = DuplicateCheck.None;
                            this._dataStrategy = strategy;
                            BatchPrefixDefaultOverride(false);
                            return Save();
                        }
                   
                    
                }
                else
                {
                    
                    this._checkDuplicates = duplicateCheck;
                    this._dataStrategy = strategy;
                    BatchPrefixDefaultOverride(false);
                    //LJB:  when the only edit made is adding a salt to a batch, the batch is not correctly marked dirty, so the salts are not added or deleted
                    CheckForChangeInFragments();
                    return Save();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("SaveRegistryRecord")]
        public RegistryRecord SaveFromCurrentXml()
        {
            try
            {
                this._noUpdateXml = true;
                return this.Save(DuplicateCheck.CompoundCheck);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("RegisteringRegiserRecord")]
        public RegistryRecord Register()
        {
            try
            {
                return this.Register(DuplicateCheck.CompoundCheck);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("RegisteringRegiserRecord")]
        public RegistryRecord Register(DuplicateAction duplicateAction)
        {
            try
            {
                return Register(DuplicateCheck.CompoundCheck, duplicateAction);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("RegisteringRegiserRecord")]
        public RegistryRecord Register(DuplicateAction duplicateAction, DataAccessStrategy strategy)
        {
            try
            {
                this._dataStrategy = strategy;
                BatchPrefixDefaultOverride(false);
                ApproverForBulkLoading(false);
                return Register(DuplicateCheck.CompoundCheck, duplicateAction);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }   

        [COEUserActionDescription("RegisteringRegiserRecord")]
        public RegistryRecord Register(DuplicateCheck duplicateCheck)
        {
            try
            {
                return Register(duplicateCheck, DuplicateAction.None);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("RegisteringRegiserRecord")]
        private RegistryRecord Register(DuplicateCheck duplicateCheck, DuplicateAction duplicateAction)
        {
            RegistryRecord response = null;

            this.BeginEdit();
            bool regRecordStatus = _isTemporal;
            try
            {
                if (_isTemporal)
                {
                    this.MarkNew();
                    this.SetTemporary(false, true);

                    this._checkDuplicates = duplicateCheck;
                    this._actionDuplicates = duplicateAction;


                    if (this._batchList != null && this._batchList.Count > 0)
                    {
                        _batchList[0].TempBatchID = _batchList[0].ID;
                    }
                    if (this.IsValid)
                    {
                        string msg = "Registering Registration";
                        _coeLog.LogStart(msg);
                        response = DataPortal.Update<RegistryRecord>(this);

                        _coeLog.LogEnd(msg);
                    }
                    //Added code to throw the validation for batch prefix on registration.
                    else if (this.BrokenRulesCollection.Count > 0)
                    {
                        throw new ValidationException(this.BrokenRulesCollection[0].Description);
                    }
                    //Added code to throw the validation for webservices and registration for all properities
                    else if (GetBrokenRulesDescription().Count > 0)
                    {
                        throw new ValidationException();
                    }

                }
                else
                    response = this;

                if (!_registerCheckRedBoxWarning)
                {

                    if (this.HasDuplicatesFound)
                    {
                        if (regRecordStatus && this.BatchList[0] != null)
                            this.BatchList[0].ApplyEdit();
                        this.CancelEdit();

                        this.RegisterCheckRedBoxWarning = false;
                        this.RedBoxWarning = string.Empty;
                    }
                    else
                    {
                        this.ApplyEdit();
                    }
                }
                else
                {
                    if(regRecordStatus)
                    this.SetTemporary(true, false);

                    if (this._dataStrategy != DataAccessStrategy.BulkLoader)
                    {
                        if (this.BrokenRulesCollection.Count > 0)
                        {
                            throw new ValidationException(this.BrokenRulesCollection[0].Description);
                        }
                    }
                }

                return response;
            }
            catch (Exception ex) //Restore object state.
            {
                CancelEdit();

                _coeLog.Log("Error in 'Register':/n/n" + ex.ToString(), 0, System.Diagnostics.SourceLevels.Error);

                throw;
            }
        }


        private void SetTemporary(bool isTemporary, bool isBeingRegistered)
        {
            _isTemporal = isTemporary;
            foreach (Component component in _componentList)
            {
                component.Compound.BaseFragment.Structure.IsTemporary =
                    string.IsNullOrEmpty(component.Compound.RegNumber.RegNum);
                component.Compound.BaseFragment.Structure.IsBeingRegistered = isBeingRegistered;
            }
            foreach (Batch currentBatch in _batchList)
            {
                foreach (BatchComponent currentBatchComponent in currentBatch.BatchComponentList)
                {
                    currentBatchComponent.BatchComponentFragmentList.IsTemporary = isTemporary;

                    currentBatchComponent.IsBeingRegistered = isBeingRegistered;
                }
            }
        }

        #endregion

        #region Data Access

        [Serializable()]
        private class BatchCriteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }

            private string _batchRegNum;
            public string BatchRegNum
            {
                get { return _batchRegNum; }
            }

            public BatchCriteria(int id)
            {
                _id = id;
            }

            public BatchCriteria(string batchRegNum)
            {
                _batchRegNum = batchRegNum;
            }
        }

        [Serializable()]
        private class Criteria
        {
            private string _regNum;
            public string RegNum
            {
                get { return _regNum.Trim(); }
            }

            public Criteria(string regNum)
            {
                if (regNum == null)
                    regNum = string.Empty;

                _regNum = regNum;
            }
        }

        [Serializable()]
        private class TemporalCriteria
        {
            private int _id;

            public int Id
            {
                get
                {
                    return _id;
                }
            }

            public TemporalCriteria(int id)
            {
                _id = id;
            }
        }

        /// <summary>
        /// Make sure not all components ids are -1 or we won't be able to distingish them in case of duplicates.
        /// </summary>
        public void EnsureComponentUniqueness()
        {
            if (this.IsNew && string.IsNullOrEmpty(this.RegNumber.RegNum))


                foreach (Component currentComponent in this.ComponentList)
                {
                    // Add user created if not there
                    if (currentComponent.Compound.PersonCreated < 1)
                        currentComponent.Compound.PersonCreated = COEUser.ID;

                    // Make sure not all components ids are -1 or we won't be able to distingish them in case of duplicates.
                    if (currentComponent.Compound.ID == -1)
                        currentComponent.Compound.ID = -Math.Abs(currentComponent.ComponentIndex) - 1;
                }

        }

        private void ResetDalResponse()
        {
            _dalResponseXml = string.Empty;
            ExtractDalResponse();
        }

        /// <summary>
        /// Clears the list of deleted Components and BatchComponents.
        /// </summary>
        /// <remarks>
        /// There is no indication anywhere why this [is / should be / needs to be] done.
        /// </remarks>
        private void ForgetDeleted()
        {
            this.ComponentList.ForgetDeleted();

            foreach (Batch currentBatch in this.BatchList)
                currentBatch.BatchComponentList.ForgetDeleted();
        }

        /// <summary>
        /// Hook the add-ins into this record; execute only on the applciation server tier.
        /// </summary>
        public void ApplyAddIns()
        {
            string regRecordAsXml = this.XmlWithAddIns;

            if (!string.IsNullOrEmpty(regRecordAsXml))
            {
                XPathDocument xDocument = new XPathDocument(new StringReader(regRecordAsXml));
                XPathNavigator xNavigator = xDocument.CreateNavigator();
                try
                {
                    XPathNodeIterator xIterator = xNavigator.Select("MultiCompoundRegistryRecord/AddIns");

                    if (xIterator.MoveNext())
                    {
                        AddInsManager addInsManager = AddInsManager.GetManager(this);
                        addInsManager.Add(this, xIterator.Current.OuterXml);
                    }
                    else
                    {
                        xDocument = new XPathDocument(new StringReader(this.PrototypeXml));
                        xNavigator = xDocument.CreateNavigator();
                        xIterator = xNavigator.Select("MultiCompoundRegistryRecord/AddIns");
                        if (xIterator.MoveNext())
                        {
                            AddInsManager addInsManager = AddInsManager.GetManager(this);
                            addInsManager.Add(this, xIterator.Current.OuterXml);
                        }
                    }
                }
                catch (Exception)
                {
                    //JED: Hmmm...why is this in a try/catch if we're not even going to log the error?
                    throw;
                }
            }
        }

        private void UpdateStructureToUse(){
            for (int i = 0; i < ComponentList.Count; i++)
            {
                if (ComponentList[i].Compound.BaseFragment.Structure.UseNormalizedStructure)
                {
                    string temp = ComponentList[i].Compound.BaseFragment.Structure.NormalizedStructure;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        ComponentList[i].Compound.BaseFragment.Structure.NormalizedStructure = ComponentList[i].Compound.BaseFragment.Structure.Value;
                        ComponentList[i].Compound.BaseFragment.Structure.Value = temp;
                        ComponentList[i].Compound.BaseFragment.Structure.UseNormalizedStructure = false;
                        
                    }
                }

            }
            
        }
        /// <summary>
        /// Conditionally reformat the base fragments' structures; execute only on the applciation server tier.
        /// </summary>
        private void ConvertStructuresToCDX()
        {
            foreach (Component component in this.ComponentList)
            {
                RegSvcUtilities.ConvertStructureFormatToCdx(component.Compound.BaseFragment.Structure);
                if (!(string.IsNullOrEmpty(component.Compound.BaseFragment.Structure.Value)) && (string.IsNullOrEmpty(component.Compound.BaseFragment.Structure.NormalizedStructure)))
                {
                    component.Compound.BaseFragment.Structure.NormalizedStructure =component.Compound.BaseFragment.Structure.Value;
                }
            }
        }

        /// <summary>
        /// Conditionally keep track of tempbatchId as enotebook requires this for retrieval of records.
        /// </summary>
        private void RememberTempBatchIDForENotebook()
        {
            if (this.ID > 0 && this.BatchList.Count >= 1 && this.BatchList[0].ID == 0)
                this.BatchList[0].ID = this.BatchList[0].TempBatchID = this.ID;
        }

        protected override void DataPortal_Create()
        {
            DataPortal_Fetch(new TemporalCriteria(0));
            SetTemporary(true, false);

            //ValidationRules.CheckRules();
            //OnLoaded(this, new EventArgs());
        }

        /// <summary>
        /// Initialize the instance using its XML. The XML will either be retrieved from the
        /// repository or is already in-hand due to a 'Save' call immediately prior to fetching.
        /// </summary>
        /// <param name="criteria">Contains the retrieval criterion, the batch ID</param>
        private void DataPortal_Fetch(BatchCriteria criteria)
        {
            string initializationXml = this.DalResponseResult;

            if (string.IsNullOrEmpty(initializationXml))
            {
                //bulk-loading process should never fetch permanent records
                if (this.DataStrategy != DataAccessStrategy.BulkLoader)
                {
                    if (!string.IsNullOrEmpty(criteria.BatchRegNum))
                        initializationXml = this.RegDal.GetRegistryRecordByBatch(criteria.BatchRegNum);
                    else if (criteria.Id != 0)
                        initializationXml = this.RegDal.GetRegistryRecordByBatch(criteria.Id);
                }
                //re-set with prototype xml by default
                else
                    initializationXml = this.PrototypeXml;
            }

            this.InitializeFromXml(initializationXml, false, true);

            SetTemporary(false, false);
            OnLoaded(this, new EventArgs());
        }

        /// <summary>
        /// Initialize the instance using its XML. The XML will either be retrieved from the
        /// repository or is already in-hand due to a 'Save' call immediately prior to fetching.
        /// </summary>
        /// <param name="criteria">Contains the retrieval criterion, the registry number</param>
        private void DataPortal_Fetch(Criteria criteria)
        {
            string initializationXml = this.DalResponseResult;

            if (string.IsNullOrEmpty(initializationXml))
            {
                //bulk-loading process should never fetch permanent records
                if (this.DataStrategy != DataAccessStrategy.BulkLoader)
                {
                    initializationXml = this.RegDal.GetRegistryRecord(criteria.RegNum);
                }
                //re-set with prototype xml by default
                else
                    initializationXml = this.PrototypeXml;
            }

            this.InitializeFromXml(initializationXml, criteria.RegNum == null, true);

            //fake-out the DataLoader
            if (this.DataStrategy == DataAccessStrategy.BulkLoader) { this.RegNum = criteria.RegNum; }

            SetTemporary(false, false);
            OnLoaded(this, new EventArgs());
        }

        /// <summary>
        /// Initialize the instance using its XML. The XML will either be retrieved from the
        /// repository or is already in-hand due to a 'Save' call immediately prior to fetching.
        /// </summary>
        /// <remarks>
        /// The special case, 'id = 0', will load the object prototype XML from the repository.
        /// </remarks>
        /// <param name="criteria">Contains the retrieval criterion, the temporary registry id</param>
        private void DataPortal_Fetch(TemporalCriteria criteria)
        {
            string initializationXml = string.Empty;

            try
            {
                //ALWAYS fetch the prototype RegistryRecord XML when requested
                if (criteria.Id == 0)
                {
                    initializationXml = this.RegDal.GetRegistryRecordTemporary(criteria.Id);
                    _prototypeXml = initializationXml;
                }
                else
                {
                    initializationXml = this.DalResponseResult;
                    //We'll need to get the xml from the database unless we're fetching immediately
                    // following a Save event (which puts the NEW xml in the DalResponseResult property) 
                    if (string.IsNullOrEmpty(initializationXml))
                    {
                        //bulk-loading process should not fetch any non-prototype records
                        if (this.DataStrategy != DataAccessStrategy.BulkLoader)
                        {
                            initializationXml = this.RegDal.GetRegistryRecordTemporary(criteria.Id);
                        }
                        //re-set with prototype xml by default
                        else
                        {
                            initializationXml = this.PrototypeXml;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _coeLog.Log(ex.ToString(), 0, System.Diagnostics.SourceLevels.Error);
                throw new BusinessObjectNotFoundException(
                    criteria.Id.ToString(), typeof(RegistryRecord), ex
                );
            }

            //apply the xml
            this.InitializeFromXml(initializationXml, criteria.Id == 0, true);

            //fake-out the DataLoader
            if (this.DataStrategy == DataAccessStrategy.BulkLoader) { this.ID = criteria.Id; }

            SetTemporary(true, false);
            OnLoaded(this, new EventArgs());

            this.ID = criteria.Id;
        }

        /// <summary>
        /// This method covers three distinct scenarios:
        /// (1) New temporary records are being inserted directly into the queue; the IDs returned are numeric.
        /// (2) Temporary records that are being converted into permanent records; the value returned is
        ///     a registration 'number' which is actually a combination of:
        ///     (1) an indicator of the DuplicateAction taken and
        ///     (2) a record identifier
        /// (3) Records that are being bulk-loaded directly as permanent records.
        /// </summary>
        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Insert()
        {
            // lmarzetti: whe need to check if project list have one project at least
            //   when RLS is enabled before insert the registry record

            // Nico: This should be done as part of the object validation process,
            //   not in the form of throwing an exception.

            if (this.DataStrategy == DataAccessStrategy.Atomic ||this.DataStrategy == DataAccessStrategy.BulkLoader)
                RegSvcUtilities.UpdateFragments(this);

            DateTime before;
            ForgetDeleted();

            before = DateTime.Now;
            ConvertStructuresToCDX();
            //LJB:  when normalization is turned off  the normalized structure field needs to populated
            UpdateDrawingType();
            UpdateNormalizedPriorToInsert();
            Trace.WriteLine(string.Format("{0} seconds for ConvertStructuresToCDX()", DateTime.Now.Subtract(before).TotalSeconds));

            before = DateTime.Now;
            ApplyAddIns();
            Trace.WriteLine(string.Format("{0} seconds for ApplyAddIns()", DateTime.Now.Subtract(before).TotalSeconds));
          
            OnInserting(this, new EventArgs());
           
        
            if ((_isTemporal && !_submitCheckRedBoxWarning) || (!_isTemporal && !_registerCheckRedBoxWarning))
            {

                before = DateTime.Now;
                //InsertDataMethod();

                RememberTempBatchIDForENotebook();

                before = DateTime.Now;
                Trace.Indent();
                if (_isTemporal)
                    InsertTemp();
                else
                {
                    InsertPerm();
                    UpdateDuplicates(); // fix for CSBR-161146: CustomFieldDuplicates Addin: Duplicates table is not updating.
                }

                OnInserted(this, new EventArgs());
                Trace.Unindent();
            }
            else
            {                
                if (ModuleName == ChemDrawWarningChecker.ModuleName.DATALOADER || ModuleName == ChemDrawWarningChecker.ModuleName.DATALOADER2)// || ModuleName == ChemDrawWarningChecker.ModuleName.INVLOADER)
                {
                    RememberTempBatchIDForENotebook();
                    before = DateTime.Now;

                    Trace.Indent();
                    //this._isTemporal = true;

                    // This check is done for adding bulk records to permanent with duplicate action as NONE.
                    //Jira ID: CBOE-1546
                    if (this._actionDuplicates == DuplicateAction.None && _registerCheckRedBoxWarning && !_isTemporal)      ////Jira ID: CBOE-1640 Added new check _isTemporal
                    {                        
                        IMatchResponse _matchResponse = RegistryDuplicateChecker.DupCheck(this);
                        if (_matchResponse.MatchedItems.Count > 0 && this.HasDuplicatesFound)
                        {
                            _foundDuplicates = _matchResponse.DuplicateXML;                        
                            //No Action if duplicate found, but for getting the dalResponseMessage calling the InsertPerm() method.
                            //It will useful for set the log information with matched records.
                            InsertPerm();
                            UpdateDuplicates();
                        }
                        else
                            InsertTemp();
                    }
                    else
                    {
                        if (this.IsTemporal)
                        {
                            this.SubmitCheckRedBoxWarning = false;
                            //this.RedBoxWarning = string.Empty;    
                            this.ValidationRules.CheckRules("ValidateRedBoxWarnings");
                        }

                        InsertTemp();
                    }

                    OnInserted(this, new EventArgs());
                    Trace.Unindent();
                }
            }
           
            Trace.WriteLine(string.Format("{0} seconds for inserting ( InsertPerm or InsertTemp ) record", DateTime.Now.Subtract(before).TotalSeconds));
        }        

        /// <summary>
        /// Inserts a temporary RegistryRecord, then retrieves it.
        /// </summary>
        private void InsertTemp()
        {
            if (this.DataStrategy != DataAccessStrategy.Atomic)
            {
                this.RegDal.UseBulkLoadStrategy(30);
            }

            //Set the status to Submitted if it is not set and has not already been set
            if (this.Status == RegistryStatus.NotSet)
            {
                this.Status = RegistryStatus.Submitted;
                foreach (Batch batch in this.BatchList)
                {
                    batch.Status = RegistryStatus.Submitted;
                }
            }
            
            UpdateXml(false);
            RegistrationCrudResponse response = this.RegDal.InsertRegistryRecordTemporary(_xml);
            this.ID = response.RegistrationId;
            this._dalResultXml = string.Empty;
            this._dalResponseMessage = response.Message;
            this._foundDuplicates = response.Error;
            this._dalResultXml = response.Result;
            DataPortal_Fetch(new TemporalCriteria(this.ID));
        }

        /// <summary>
        /// Conditionally inserts a permanent RegistryRecord, then retrieves it. In some cases, the
        /// DB will direct the creation of a temporary record instead.
        /// </summary>
        private void InsertPerm()
        {
            if (this.DataStrategy != DataAccessStrategy.Atomic)

            {
                this.RegDal.UseBulkLoadStrategy(30);
            }

            DateTime before;            

            EnsureComponentUniqueness();
            
            //JED: Remember, if the Go-To-Temp resolution mechanism is invoked, this may be inaccurate!
            this.Status = RegistryStatus.Registered;
            foreach (Batch b in this.BatchList)
                b.Status = RegistryStatus.Registered;

            before = DateTime.Now;

            UpdateStructureToUse();
            UpdateXml(false);
            
            Trace.WriteLine(string.Format("{0} seconds for serialization ( RegistryRecord.UpdateXml )", DateTime.Now.Subtract(before).TotalSeconds));

            int id = ID;
            RegistrationCrudResponse response = null;

            this.ResetDalResponse();
            OnRegistering(this, new EventArgs());

            //Set the status to Registered
            this.Status = RegistryStatus.Registered;

            //if there are duplicates, get out
            if (HasDuplicatesFound)
            {
                if (_actionDuplicates != DuplicateAction.None)
                {
                    if (_dataStrategy == DataAccessStrategy.BulkLoader
                        && _registeredMatches.Count > 0 && _registeredMatches[0].MatchedRegistrations.Count > 0
                        && (_registeredMatches[0].Mechanism == PreloadDupCheckMechanism.ComponentIdentifier
                            || _registeredMatches[0].Mechanism == PreloadDupCheckMechanism.StructureIdentifier
                            || _registeredMatches[0].Mechanism == PreloadDupCheckMechanism.ComponentProperty
                            || _registeredMatches[0].Mechanism == PreloadDupCheckMechanism.StructureProperty))
                    {
                        if (_actionDuplicates == DuplicateAction.Batch)
                        {
                            XPathDocument xDocument = new XPathDocument(new StringReader(this.FoundDuplicates));
                            XPathNavigator xNavigator = xDocument.CreateNavigator();
                            XPathNodeIterator xIterator = xNavigator.Select("COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER");
                            if (xIterator.Count >= 2) return;//return if multiple matches found

                            before = DateTime.Now;
                            Trace.Indent();
                            RegistryRecord record = ComponentDuplicateResolver.ResolveUsingAction(this, _actionDuplicates, _registeredMatches);
                            this.InitializeFromXml(record.Xml, false, true);
                            Trace.Unindent();
                            Trace.WriteLine(string.Format("{0} seconds for ComponentDuplicateResolver.ResolveUsingAction(,{1},)", DateTime.Now.Subtract(before).TotalSeconds, _actionDuplicates.ToString()));
                            //DataPortal_Fetch(new Criteria(record.RegNum));
                            return;
                        }
                        else if (_actionDuplicates == DuplicateAction.Temporary)
                        {
                            if (ID == 0) InsertTemp();

                            return;
                        }
                    }
                }
                else
                    return;
            }

            before = DateTime.Now;
            Trace.Indent();

            // duplicate-resolution is the prime directive
            if (_actionDuplicates != DuplicateAction.None)
            {
                //In case of bulk-style inserts going directly to permanent using DuplicateAction member
                if (_dataStrategy == DataAccessStrategy.BulkLoader){
                    try
                    {						
                        string strategy = "BulkLoading";																											
                        response = this.RegDal.InsertRegistryRecord(_xml, _actionDuplicates, 1, strategy);																													
                    }
                    catch (Exception ex)
                    {
                        if (_actionDuplicates == DuplicateAction.Temporary && !string.IsNullOrEmpty(ex.Message))
                        {
                            if (ex.Message.Contains("ORA-00001:") && ex.Message.ToUpper().Contains("UNIQUE CONSTRAINT"))
                            { // ORA-00001 => unique key violation; meaning temp record already existing 
                                // while trying to add record to temp in case of duplicate.
                                // This is specially ofr bulk register from temp registration.
                                this._foundDuplicates = ex.Message;
                                return;
                            }
                        }
                        else
                            throw ex;
                    }
                }
                else{
                    response = this.RegDal.RegisterRegistryRecord(ID, _xml, _checkDuplicates, _actionDuplicates);
                }
                this._customDuplicateXml = string.Empty;
            }
            else
            {
                if (_checkDuplicates == DuplicateCheck.None && _actionDuplicates == DuplicateAction.None && _dataStrategy == DataAccessStrategy.BulkLoader)
                    response = this.RegDal.InsertRegistryRecord(_xml, _checkDuplicates);
                else if (_actionDuplicates == DuplicateAction.None && _dataStrategy == DataAccessStrategy.BulkLoader)
                    response = this.RegDal.InsertRegistryRecord(_xml, _actionDuplicates, 0, "None");
                else
                    response = this.RegDal.RegisterRegistryRecord(ID, _xml, _checkDuplicates);
            }
            Trace.Unindent();
            Trace.WriteLine(string.Format("{0} seconds in OracleDataAccessClientDAL", DateTime.Now.Subtract(before).TotalSeconds));

            this.RegNum = response.RegistrationKey;
            this._dalResultXml = string.Empty;
            this._dalResponseMessage = response.Message;
            this._foundDuplicates = response.Error;
            this._dalResultXml = response.Result;
            this._customFieldsResponseXml = response.CustomFieldsResponse;

            //This HAS to be conditional based on the 'mode'
            if (HasDuplicatesFound && this.DataStrategy == DataAccessStrategy.Atomic)
                return;

            _actionDuplicates = response.DuplicateActionTaken;
            if (response.DuplicateActionTaken == DuplicateAction.Temporary)
                DataPortal_Fetch(new TemporalCriteria(response.RegistrationId));
            else
                DataPortal_Fetch(new Criteria(response.RegistrationKey.Trim()));
        }
       
        /// <summary>
        /// This method will update the duplicate table wiht component duplicates found in custom duplicate addin
        /// </summary>
        private void UpdateDuplicates()
        {
            if (!string.IsNullOrEmpty(this._customDuplicateXml))
            {
                string strCompoundRegnum=string.Empty;
                foreach (Component compt in this.ComponentList)
                {
                    if (compt.Compound != null && compt.Compound.BaseFragment.Structure.DrawingType!=DrawingType.Chemical)
                    {
                        strCompoundRegnum = compt.Compound.RegNumber.RegNum;
                    }
                }
                if (!string.IsNullOrEmpty(strCompoundRegnum))
                {
                    string customDupXml=this._customDuplicateXml.Replace("NEW_COMPOUNDID",strCompoundRegnum);
                    this.RegDal.InsertDuplicateEntries(customDupXml, this.PersonCreated.ToString());
                }
            }
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Update()
        {
            /*lmarzetti: whe need to check if project list have one project at least
             when RLS is enabled before update the registry record*/
            if (CheckRLSRequirements())
            {
                ConvertStructuresToCDX();

                //as necessary, back-fill the fragmentlist for each component
                //NOTE: This wilhl slow down bulk-loading substantially until we
                //      enable a caching mechanism for this. -- Jeff D. 18-Feb-11
                if (this.DataStrategy == DataAccessStrategy.Atomic || this.DataStrategy == DataAccessStrategy.BulkLoader)
                    RegSvcUtilities.UpdateFragments(this);

                //LJB:  when normalization is turned off and a record is edited, the normalized structure field needs to be updated
                UpdateDrawingType();
                
                UpdateNormalizedPriorToSave();
            
                ApplyAddIns();
                OnUpdating(this, new EventArgs());

                if ((_isTemporal && !_submitCheckRedBoxWarning) || (!_isTemporal && !_registerCheckRedBoxWarning))
                {
                    if (_isTemporal && !_noUpdateXml)
                    {
                        //JED: If the add-ins changed anything, we have to incorporate it into the object's XML
                        //if (!_noUpdateXml)
                            UpdateXml();

                        this.RegDal.UpdateRegistryRecordTemporary(_xml, out _dalResponseXml);
                        DataPortal_Fetch(new TemporalCriteria(this.ID));
                    }
                    else
                    {
                        _regsAffectedByCompoundUpdate = new Dictionary<string, string[]>();
                        _regsAffectedByStructureUpdate = new Dictionary<string, string[]>();
                        if (_componentList.IsDirty && CheckOtherMixtures)
                        {
                            CheckOtherMixturesAffected();
                            if (_regsAffectedByCompoundUpdate.Count > 0)
                                throw new EditAffectsOtherMixturesException(Resources.UpdateAffectOtherRecords, _regsAffectedByCompoundUpdate);
                            if (_regsAffectedByStructureUpdate.Count > 0)
                                throw new EditAffectsOtherMixturesException(Resources.UpdateAffectOtherComponents, _regsAffectedByStructureUpdate);
                        }

                        this.ResetDalResponse();
                        OnUpdatingPerm(this, new EventArgs());

                        // any add-in that fired from 'OnUpdatingPerm' might inform us of duplicates
                        if (HasDuplicatesFound)
                            return;
                        else
                        {
                            //if SBI=true we need to synchronize all BatchComponentFragmentList on all Batches in case the ComponentFragmentList                         has changed.
                            if (this.SameBatchesIdentity == true)
                                this.FixBatchesFragments();

                            // if the add-ins changed anything, we have to incorporate it into the object's XML
                            if (!_noUpdateXml)
                                UpdateXml();

                            DuplicateCheck checkDupState = _checkDuplicates;
                            if (!ComponentList.IsDirty)
                                this._checkDuplicates = DuplicateCheck.None;

                            try
                            {
                                this.RegDal.UpdateRegistryRecord(_xml, _checkDuplicates, out _dalResponseXml);
                            }
                            catch (Exception ex)
                            {
                                if (this.DataStrategy == DataAccessStrategy.BulkLoader
                                    && (_actionDuplicates == DuplicateAction.Batch || _actionDuplicates == DuplicateAction.Temporary || _actionDuplicates == DuplicateAction.None)
                                    && !string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("ORA-20027:"))
                                {
                                    // ORA-20027 => Error validating the compound <xxxx>.
                                    // The compound should have the same identity of fragments between batches.
                                    // (The "SameBatchesIdentity" flag is set in "true")
                                    // In case of bulk loading catch this error and don't throw 
                                }
                                else
                                    throw ex;
                            }

                            this._checkDuplicates = checkDupState;
                            this.ExtractDalResponse();
                        }

                        if (HasDuplicatesFound /*&& this.DataStrategy != DataAccessStrategy.BulkLoader*/)
                            return;
                        else
                        {
                            //JED: If the structure was modified and the component is shared by other mixtures,
                            //     we need to transactionally update those mixtures' aggregate structures
                            //LJB: with the new mixture workflow, this should not be bothered with when componentlist>1. components in
                            //mixtures cannot be modified. When componentlist = 1 and it has been modified, aggregate structures
                            //with this shared component are still updated.
                            if ((AllowUnregisteredComponents == false && _componentList.Count == 1) || (AllowUnregisteredComponents == true))
                            {
                                if (_componentList.IsDirty && AggregateOtherMixtures)
                                {
                                    foreach (Component c in _componentList)
                                    {
                                        if (c.Compound.BaseFragment.Structure.IsDirty)
                                        {
                                            RegistryRecordList listByCommonComponent = RegistryRecordList.GetListFromSharableComponent(c);
                                            listByCommonComponent.Remove(this);
                                            if (listByCommonComponent.Count > 0)
                                                listByCommonComponent.UpdateAggregateStructures();

                                            /*lmarzetti: If the structure was modified, and this is shared by other mixtures,
                                            we need to transactionally update those mixtures' aggregate structures*/
                                            RegistryRecordList listByCommonStructure = RegistryRecordList.GetListFromLinkedStructure(c.Compound.BaseFragment.Structure.ID);
                                            listByCommonStructure.Remove(this);
                                            if (listByCommonStructure.Count > 0)
                                                listByCommonStructure.UpdateAggregateStructures();

                                        }
                                    }
                                }
                            }
                            DataPortal_Fetch(new Criteria(this.RegNumber.RegNum));
                        }
                    }

                    OnUnknownPropertyChanged();
                    OnUpdated(this, new EventArgs());
                    this._noUpdateXml = false;                 
                }
            }
        }

        private void UpdateDataMethod()
        {
            if (_isTemporal)
            {
                //JED: If the add-ins changed anything, we have to incorporate it into the object's XML
                if (!_noUpdateXml)
                    UpdateXml();

                this.RegDal.UpdateRegistryRecordTemporary(_xml, out _dalResponseXml);
                DataPortal_Fetch(new TemporalCriteria(this.ID));
            }
            else
            {
                _regsAffectedByCompoundUpdate = new Dictionary<string, string[]>();
                _regsAffectedByStructureUpdate = new Dictionary<string, string[]>();
                if (_componentList.IsDirty && CheckOtherMixtures)
                {
                    CheckOtherMixturesAffected();
                    if (_regsAffectedByCompoundUpdate.Count > 0)
                        throw new EditAffectsOtherMixturesException(Resources.UpdateAffectOtherRecords, _regsAffectedByCompoundUpdate);
                    if (_regsAffectedByStructureUpdate.Count > 0)
                        throw new EditAffectsOtherMixturesException(Resources.UpdateAffectOtherComponents, _regsAffectedByStructureUpdate);
                }

                this.ResetDalResponse();
                OnUpdatingPerm(this, new EventArgs());

                // any add-in that fired from 'OnUpdatingPerm' might inform us of duplicates
                if (HasDuplicatesFound)
                    return;
                else
                {
                    //if SBI=true we need to synchronize all BatchComponentFragmentList on all Batches in case the ComponentFragmentList                         has changed.
                    if (this.SameBatchesIdentity == true)
                        this.FixBatchesFragments();

                    // if the add-ins changed anything, we have to incorporate it into the object's XML
                    if (!_noUpdateXml)
                        UpdateXml();

                    DuplicateCheck checkDupState = _checkDuplicates;
                    if (!ComponentList.IsDirty)
                        this._checkDuplicates = DuplicateCheck.None;

                    try
                    {
                        this.RegDal.UpdateRegistryRecord(_xml, _checkDuplicates, out _dalResponseXml);
                    }
                    catch (Exception ex)
                    {
                        if (this.DataStrategy == DataAccessStrategy.BulkLoader
                            && (_actionDuplicates == DuplicateAction.Batch || _actionDuplicates == DuplicateAction.Temporary || _actionDuplicates == DuplicateAction.None)
                            && !string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("ORA-20027:"))
                        {
                            // ORA-20027 => Error validating the compound <xxxx>.
                            // The compound should have the same identity of fragments between batches.
                            // (The "SameBatchesIdentity" flag is set in "true")
                            // In case of bulk loading catch this error and don't throw 
                        }
                        else
                            throw ex;
                    }

                    this._checkDuplicates = checkDupState;
                    this.ExtractDalResponse();
                }

                if (HasDuplicatesFound /*&& this.DataStrategy != DataAccessStrategy.BulkLoader*/)
                    return;
                else
                {
                    //JED: If the structure was modified and the component is shared by other mixtures,
                    //     we need to transactionally update those mixtures' aggregate structures
                    //LJB: with the new mixture workflow, this should not be bothered with when componentlist>1. components in
                    //mixtures cannot be modified. When componentlist = 1 and it has been modified, aggregate structures
                    //with this shared component are still updated.
                    if ((AllowUnregisteredComponents == false && _componentList.Count == 1) || (AllowUnregisteredComponents == true))
                    {
                        if (_componentList.IsDirty && AggregateOtherMixtures)
                        {
                            foreach (Component c in _componentList)
                            {
                                if (c.Compound.BaseFragment.Structure.IsDirty)
                                {
                                    RegistryRecordList listByCommonComponent = RegistryRecordList.GetListFromSharableComponent(c);
                                    listByCommonComponent.Remove(this);
                                    if (listByCommonComponent.Count > 0)
                                        listByCommonComponent.UpdateAggregateStructures();

                                    /*lmarzetti: If the structure was modified, and this is shared by other mixtures,
                                    we need to transactionally update those mixtures' aggregate structures*/
                                    RegistryRecordList listByCommonStructure = RegistryRecordList.GetListFromLinkedStructure(c.Compound.BaseFragment.Structure.ID);
                                    listByCommonStructure.Remove(this);
                                    if (listByCommonStructure.Count > 0)
                                        listByCommonStructure.UpdateAggregateStructures();

                                }
                            }
                        }
                    }
                    DataPortal_Fetch(new Criteria(this.RegNumber.RegNum));
                }
            }

            OnUnknownPropertyChanged();
            OnUpdated(this, new EventArgs());
            this._noUpdateXml = false;

        }
        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_DeleteSelf()
        {
            if (_isTemporal)
                DataPortal_Delete(new TemporalCriteria(this.ID));
            else
                DataPortal_Delete(new Criteria(RegNumber.RegNum));
        }

        [Transactional(TransactionalTypes.Manual)]
        private void DataPortal_Delete(Criteria criteria)
        {
            this.RegDal.DeleteRegistryRecord(criteria.RegNum);
        }

        [Transactional(TransactionalTypes.Manual)]
        private void DataPortal_Delete(TemporalCriteria criteria)
        {
            this.RegDal.DeleteRegistryRecordTemporary(criteria.Id);
        }

        #endregion

        #region Xml

        internal string UpdateSelf()
        {
            return UpdateSelf(true);
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            string msg = "Serializing via 'UpdateSelf'";
            _coeLog.LogStart(msg);

            StringBuilder builder = new StringBuilder("");
            //ljb adding missing attributes
            builder.Append("<MultiCompoundRegistryRecord ");
            builder.Append("SameBatchesIdentity=\"" + this.SameBatchesIdentity + "\" ");
            builder.Append("ModuleName=\"" + this.ModuleName.ToString() + "\" ");           
            builder.Append("IsEditable=\"" + this.IsEditable + "\" ");
            builder.Append("ActiveRLS=\"" + this._rlsStatus + "\">");
            builder.Append("<ID>" + this._mixtureId + "</ID>");
            builder.Append("<DateCreated>" + this._dateCreated.ToString(Constants.DATE_FORMAT) + "</DateCreated>");
            builder.Append("<DateLastModified>" + this._dateLastModified.ToString(Constants.DATE_FORMAT) + "</DateLastModified>");

            builder.Append("<PersonCreated");
            if (addCRUDattributes && _changedProperties.Contains("PersonCreated"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</PersonCreated>", _personCreated > 0 ? _personCreated.ToString() : string.Empty);

            builder.Append("<SubmissionComments");
            if (addCRUDattributes && _changedProperties.Contains("SubmissionComments"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</SubmissionComments>", string.IsNullOrEmpty(_submissionComments) ? string.Empty : _submissionComments.ToString());
            
            builder.Append("<StructureAggregation");
            if (addCRUDattributes && this._changedProperties.Contains("StructureAggregation"))
                builder.Append(" update=\"yes\"");
            builder.Append(">" + this._structureAggregation + "</StructureAggregation>");
            builder.Append(this._regNumber.UpdateSelf(addCRUDattributes));

            builder.Append("<StatusID");
            if (addCRUDattributes && _changedProperties.Contains("Status"))
                builder.Append(" update=\"yes\"");
            builder.AppendFormat(">{0}</StatusID>", ((int)_status));

            string buf = string.Empty;

            buf = this._identifierList.UpdateSelf(addCRUDattributes);
            builder.Append(buf);

            buf = this._projectList.UpdateSelf(addCRUDattributes);
            builder.Append(buf);

            buf = this._propertyList.UpdateSelf(addCRUDattributes);
            builder.Append(buf);

            buf = this._componentList.UpdateSelf(addCRUDattributes);
            builder.Append(buf);

            buf = this._batchList.UpdateSelf(addCRUDattributes);
            builder.Append(buf);

            buf = this.GetAddInsXmlNode().OuterXml;
            builder.Append(buf);

            builder.Append("</MultiCompoundRegistryRecord>");

            _coeLog.LogEnd(msg);

            return builder.ToString();
        }

        /// <summary>
        /// Performs custom 'serialization' of the Registry Record instance into an xml string.
        /// </summary>
        /// 
        [COEUserActionDescription("SerializeRegistryRecord")]
        public void UpdateXml()
        {
            try
            {
                this._xml = UpdateSelf();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("SerializeRegistryRecord")]
        public void UpdateXml(bool addInsertUpdateAttribute)
        {
            try
            {
                this._xml = UpdateSelf(addInsertUpdateAttribute);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Extracts the AddIns node as an xml string from the Registration prototype xml document.
        /// </summary>
        /// <returns></returns>
        private XmlNode GetAddInsXmlNode()
        {
            XmlNode addIns = null;
            if (this.PrototypeXml != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(this.PrototypeXml);
                addIns = doc.SelectSingleNode(_addInsXpath);
            }
            return addIns;
        }

        /// <summary>
        /// Performs custom 'de-serialization' (hydration of the current instance) of a Registry Record
        /// instance from an xml string.
        /// </summary>
        /// <remarks>
        /// Essentially a reverssal of the UpdateXml() method.
        /// </remarks>
        /// <param name="xml">A string representation of a 'serialized' Registration Record</param>
        /// <param name="isNew"></param>
        /// <param name="isClean"></param>
        /// 
        [COEUserActionDescription("DeserializeRegistryRecord")]
        public void InitializeFromXml(string xml, bool isNew, bool isClean)
        {
            try
            {
                this.Updated = null;
                this.Updating = null;
                this.Inserted = null;
                this.Inserting = null;
                this.Loaded = null;
                this.Registering = null;
                this.UpdatingPerm = null;
                this.Saving = null;

                //Provides some necessary clean-up of the incoming xml, especially when derived from external sources
                // such as the web-service (from ELN, Inventory, etc.)
                string cleanedXml = CleanIncomingXml(xml);
                _xml = cleanedXml;

                XPathDocument xDocument = new XPathDocument(new StringReader(cleanedXml));
                XPathNavigator xNavigator = xDocument.CreateNavigator();

                XPathNodeIterator xIterator = xNavigator.Select("MultiCompoundRegistryRecord/DateCreated");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        _dateCreated = DateTime.Parse(xIterator.Current.Value);

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/DateLastModified");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        _dateLastModified = DateTime.Parse(xIterator.Current.Value);

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/RegNumber");
                if (xIterator.MoveNext())
                    _regNumber = RegNumber.NewRegNumber(xIterator.Current.OuterXml, isClean);
                else
                    _regNumber = RegNumber.NewRegNumber();

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/PersonCreated");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        this._personCreated = int.Parse(xIterator.Current.Value);
                    else
                        this._personCreated = COEUser.ID;

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/SubmissionComments");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        this._submissionComments = xIterator.Current.Value;
                    else
                        this._submissionComments = string.Empty;
                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/StatusID");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        this._status = (RegistryStatus)Enum.Parse(typeof(RegistryStatus), xIterator.Current.Value);
                    else
                        this._status = RegistryStatus.NotSet;

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/IdentifierList");
                if (xIterator.MoveNext())
                    _identifierList = IdentifierList.NewIdentifierList(xIterator.Current.OuterXml, isNew, isClean);

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/ProjectList");
                if (xIterator.MoveNext())
                    _projectList = ProjectList.NewProjectList(xIterator.Current.OuterXml, isClean, isNew);

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/PropertyList");
                if (xIterator.MoveNext())
                {
                    if (!string.IsNullOrEmpty(xIterator.Current.OuterXml))
                    {
                        _propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, isClean);
                    }
                }
                else
                    _propertyList = PropertyList.NewPropertyList();

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/ComponentList");
                if (xIterator.MoveNext())
                    _componentList = ComponentList.NewComponentList(xIterator.Current.OuterXml, isNew, isClean);

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("SameBatchesIdentity", string.Empty)))
                        _samesBatchIdentity = xIterator.Current.GetAttribute("SameBatchesIdentity", string.Empty).ToUpper() == "TRUE" ? true : false;

                _moduleName = ChemDrawWarningChecker.ModuleName.NOTSET;
                xIterator = xNavigator.Select("MultiCompoundRegistryRecord");
                if (xIterator.MoveNext())
                {
                    if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("ActiveRLS", string.Empty)))
                    {
                        try
                        { _rlsStatus = (RLSStatus)Enum.Parse(typeof(RLSStatus), xIterator.Current.GetAttribute("ActiveRLS", string.Empty).Replace(" ", "")); }
                        catch { _rlsStatus = RLSStatus.Off; }
                    }

                    if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("ModuleName", string.Empty)))
                    {
                        try
                        {
                            string strCurrentModuleName = xIterator.Current.GetAttribute("ModuleName", string.Empty).Replace(" ", "").ToUpperInvariant();

                            if (strCurrentModuleName == ChemDrawWarningChecker.ModuleName.DATALOADER.ToString())
                                _moduleName = ChemDrawWarningChecker.ModuleName.DATALOADER;
                            else if (strCurrentModuleName == ChemDrawWarningChecker.ModuleName.DATALOADER2.ToString())
                                _moduleName = ChemDrawWarningChecker.ModuleName.DATALOADER2;
                            else if (strCurrentModuleName == ChemDrawWarningChecker.ModuleName.INVLOADER.ToString())
                                _moduleName = ChemDrawWarningChecker.ModuleName.INVLOADER;
                            else if (strCurrentModuleName == ChemDrawWarningChecker.ModuleName.ELN.ToString())
                                _moduleName = ChemDrawWarningChecker.ModuleName.ELN;
                            else
                                _moduleName = ChemDrawWarningChecker.ModuleName.NOTSET;

                        }
                        catch { _moduleName = ChemDrawWarningChecker.ModuleName.NOTSET; }
                    }
                }               

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/StructureAggregation");
                if (xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.Value))
                    _structureAggregation = xIterator.Current.Value;

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/BatchList");
                if (xIterator.MoveNext())
                    _batchList = BatchList.NewBatchList(xIterator.Current.OuterXml, isNew, isClean, this.SameBatchesIdentity, this._regNumber.RegNum, this.RLSStatus);

                //this.CompletePercentageList(this._batchList, this._componentList);

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord/ID");
                if (xIterator.MoveNext() && xIterator.Current.Value.Trim() != string.Empty)
                    _mixtureId = int.Parse(xIterator.Current.Value);

                if (_regNumber.ID != 0)
                    this.ID = _regNumber.ID;
                else if (_mixtureId != 0)
                    this.ID = _mixtureId;

                if (isClean)
                    MarkClean();

                if (!isNew)
                    MarkOld();
                else
                {
                    MarkNew();  // Added by WJC otherwise InitializeFromXml can never be used to create a new record
                    this.PersonCreated = COEUser.ID;
                }

                xIterator = xNavigator.Select("MultiCompoundRegistryRecord");
                if (xIterator.MoveNext())
                {
                    if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("IsEditable", string.Empty)))
                    {
                        try { _isEditable = Boolean.Parse(xIterator.Current.GetAttribute("IsEditable", string.Empty)); }
                        catch { _isEditable = true; }
                    }
                    else
                        _isEditable = true;
                    if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("IsRegistryDeleteable", string.Empty)))
                    {
                        try { _isRegistryDeleteable = Boolean.Parse(xIterator.Current.GetAttribute("IsRegistryDeleteable", string.Empty)); }
                        catch { _isRegistryDeleteable = true; }
                    }
                    else
                        _isRegistryDeleteable = true;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Allows a RegistryRecord in memory to be updated from an xml string representation.
        /// </summary>
        /// <param name="xml">the XML string containing updated information</param>
        [COEUserActionDescription("DeserializeRegistryRecord")]
        public void UpdateFromXml(string xml)
        {
            string cleanXml = CleanIncomingXml(xml);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(cleanXml);
            XmlNode matchingChild;

            // RegistryRecord itself only update properties that are allowed to be updated not autogenerated fields like person created

            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/SubmissionComments");
            if (matchingChild != null && !string.IsNullOrEmpty(matchingChild.InnerText) && (this.SubmissionComments != matchingChild.InnerText))
                this.SubmissionComments = matchingChild.InnerText;

            // properties
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/PropertyList");
            if (matchingChild != null)
                this.PropertyList.UpdateFromXml(matchingChild);

            // projects
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/ProjectList");
            if (matchingChild != null)
                this.ProjectList.UpdateFromXml(matchingChild);

            //identifiers
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/IdentifierList");
            if (matchingChild != null)
                this.IdentifierList.UpdateFromXml(matchingChild);

            // batches
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/BatchList");
            if (matchingChild != null)
                this.BatchList.UpdateFromXml(matchingChild);

            //components
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/ComponentList");
            if (matchingChild != null)
                this.ComponentList.UpdateFromXml(matchingChild);
            // addins won't be updated at all.
        }

        /// <summary>
        /// Allows a RegistryRecord in memory to be updated from user preference xml string.
        /// </summary>
        /// <param name="xml">the XML string containing default/user preference information</param>
        [COEUserActionDescription("DeserializeRegistryRecord")]
        public void UpdateUserPreference(string userPreferenceXml)
        {

            
            string cleanXml = CleanIncomingXml(userPreferenceXml);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(cleanXml);
            XmlNode matchingChild;

            // RegistryRecord itself only update properties that are allowed to be updated 
            // To take care of the prefix with respect to user preference
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/RegNumber/SequenceID");
            if (matchingChild != null && !string.IsNullOrEmpty(matchingChild.InnerText))
            {
                int sequenceId = int.Parse(matchingChild.InnerText);
                //only update when the current value is 0 and user preference value is not 0 or not equal to current value
                if (sequenceId > 0 && this.RegNumber.Sequence.ID <= 0 && this.RegNumber.Sequence.ID != sequenceId)
                    this.RegNumber.Sequence.ID = int.Parse(matchingChild.InnerText);
            }

            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/SubmissionComments");
            if (matchingChild != null && !string.IsNullOrEmpty(matchingChild.InnerText) && string.IsNullOrEmpty(this.SubmissionComments))
                this.SubmissionComments = matchingChild.InnerText;

            // properties
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/PropertyList");
            if (matchingChild != null)
                this.PropertyList.UpdateUserPreference(matchingChild);

            // projects
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/ProjectList");
            if (matchingChild != null)
                this.ProjectList.UpdateUserPreference(matchingChild,ProjectList.ProjectTypeEnum.R);

            ////identifiers
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/IdentifierList");
            if (matchingChild != null)
                this.IdentifierList.UpdateUserPreference(matchingChild);

            //// batches
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/BatchList");
            if (matchingChild != null)
                this.BatchList.UpdateUserPreference(matchingChild);

            ////components
            matchingChild = doc.SelectSingleNode("MultiCompoundRegistryRecord/ComponentList");
            if (matchingChild != null)
                this.ComponentList.UpdateUserPreference(matchingChild);
            // addins won't be updated at all.

        }

        /// <summary>
        /// Force-fit certain aspects of the incoming XML into what is expected of the services that will utilize it.
        /// </summary>
        /// <remarks>
        /// The prototypeXml parameter should have, at a minimum:
        /// (1) default values for all possible "PropertyList/Property" elements, such that it contains all elements.
        /// (2) a complete "AddIns section"
        /// </remarks>
        /// <param name="prototypeXml">Template xml for a nascent Registryrecord object.</param>
        /// <param name="sourceXml">Incoming xml string from which the RegistryRecord instance is being initialized</param>
        /// <returns></returns>
        private string CleanIncomingXml(string sourceXml)
        {
            if (this.PrototypeXml == null)
                return sourceXml;

            //bypass if we're initializing with prototype xml and the strings match
            if (!string.IsNullOrEmpty(sourceXml)
                && sourceXml.Equals(this.PrototypeXml)
            )
                return sourceXml;

            //load the raw document provided as well as the prototype xml
            XmlDocument prototypeDoc = new XmlDocument();
            prototypeDoc.LoadXml(this.PrototypeXml);
            XmlDocument rawDoc = new XmlDocument();
            rawDoc.LoadXml(sourceXml);

            //bypass if we're initializing with the prototype xml but the strings didn't match for some reason
            if (rawDoc.OuterXml.Equals(prototypeDoc.OuterXml))
                return sourceXml;

            //provide the prototype AddIns section as necessary
            ProvideAddInsNode(ref rawDoc, prototypeDoc);

            //validate incoming propertylist data against prototype-xml propertylist properties
            NormalizePropertyLists(ref rawDoc, prototypeDoc);

            string outputXml = rawDoc.OuterXml;
            return outputXml;
        }

        /// <summary>
        /// Ensures that the initialization xml's AddIns match those of the prototype xml
        /// </summary>
        /// <param name="rawDoc">Representation of the xml being used to initialize the Registry Record instance.</param>
        /// <param name="nascentDoc">Representation of the prototype xml for the Registry Record instance.</param>
        private void ProvideAddInsNode(ref XmlDocument rawDoc, XmlDocument nascentDoc)
        {
            //replace the AddIns section (or create one) as necessary
            XmlNode rawAddInsNode = rawDoc.SelectSingleNode(_addInsXpath);
            XmlNode nascentAddInsNode = nascentDoc.SelectSingleNode(_addInsXpath);
            if (rawAddInsNode == null)
            {
                //create one
                rawDoc.SelectSingleNode("//MultiCompoundRegistryRecord").AppendChild(
                    rawDoc.ImportNode(nascentAddInsNode, true)
                );
            }
            else if (rawAddInsNode.OuterXml != nascentAddInsNode.OuterXml)
            {
                //replace whatever one is there
                rawAddInsNode.ParentNode.ReplaceChild(
                    rawDoc.ImportNode(nascentAddInsNode, true)
                    , rawAddInsNode
                );
            }
        }

        /// <summary>
        /// Eliminates invalid property elements from, and provides required elements for,
        /// an xml string representing a Registry Record instance.
        /// </summary>
        /// <param name="rawDoc">Representation of the xml being used to initialize the Registry Record instance.</param>
        /// <param name="prototypeDoc">Representation of the prototype xml for the Registry Record instance.</param>
        private void NormalizePropertyLists(ref XmlDocument rawDoc, XmlDocument prototypeDoc)
        {
            //derive the keyed lists of PropertyList properties
            Dictionary<string, XmlNode> propertyListsBySection = this.GetPropertyListsBySection(prototypeDoc);

            foreach (string sectionName in propertyListsBySection.Keys)
            {
                //fetch the corresponding PropertyList template node
                XmlNode templatePropList = propertyListsBySection[sectionName];

                //fetch the PARENT element for the corresponding PropertyList from the raw document
                string containerSections = string.Format("//{0}/{1}/..", sectionName, PROPERTYLIST);
                XmlNodeList rawPropListContainers = rawDoc.SelectNodes(containerSections);

                //there may be multiple sections requiring normalization (eg. BatchList  1-->n  Batch)
                foreach (XmlNode container in rawPropListContainers)
                {
                    //create a clone on the template
                    XmlNode clonedTemplatePropList = templatePropList.Clone();

                    //get the PropertyList node in this specific section
                    string sectionList = string.Format("./{0}", PROPERTYLIST);
                    XmlNode sectionPropList = container.SelectSingleNode(sectionList);

                    //migrate the data from the section-specific PropertyList to a cloned template node
                    XmlNode fixedNode = FillPropertyTemplate(rawDoc, prototypeDoc, clonedTemplatePropList, sectionPropList);

                    //and replace the existing PropertyList node with the 'data-migrated' node
                    if (fixedNode != null)
                    {
                        XmlNode replacementNode = rawDoc.ImportNode(fixedNode, true);
                        container.ReplaceChild(replacementNode, sectionPropList);
                    }
                }
            }
        }

        /// <summary>
        /// Migrate the attributes and value data from matching xml child nodes to a template node.
        /// </summary>
        /// <param name="rawDoc">An incoming XML document representing the RegistryRecord instance</param>
        /// <param name="prototypeDoc">The native, prototype XML for new RegistryRecord objects</param>
        /// <param name="templatePropList">For a specific RegistryRecord section, the prototype PropertyList xml node</param>
        /// <param name="sectionPropList">An xml node derived from a specific section's PropertyList (from raw xml)</param>
        /// <returns></returns>
        private XmlNode FillPropertyTemplate(XmlDocument rawDoc, XmlDocument prototypeDoc, XmlNode templatePropList, XmlNode sectionPropList)
        {
            //migrate the attributes to the template
            templatePropList.Attributes.RemoveAll();
            foreach (XmlAttribute rawAttrib in sectionPropList.Attributes)
            {
                XmlAttribute newAttrib = rawDoc.ImportNode(rawAttrib, false) as XmlAttribute;
                if (newAttrib != null) // coverity fix - CID 11700 
                    templatePropList.Attributes.Append(newAttrib);
            }

            //migrate the child data to the template
            XmlNodeList rawNodes = sectionPropList.SelectNodes(".//" + PROPERTY);
            XmlNodeList templateNodes = templatePropList.SelectNodes(".//" + PROPERTY);
            foreach (XmlNode templateNode in templateNodes)
            {
                foreach (XmlNode rawNode in rawNodes)
                {
                    //if the attribute names match, assign the incoming value to the 'clean' template node
                    if (templateNode.Attributes[_propertyNameAttribute].Value == rawNode.Attributes[_propertyNameAttribute].Value)
                    {
                        //search only the 'text' nodes of the immediate children for the node's 'value'
                        foreach (XmlNode n in rawNode.ChildNodes)
                        {
                            if (n.NodeType == XmlNodeType.Text && !string.IsNullOrEmpty(n.Value) && n.NodeType != XmlNodeType.Comment)
                            {
                                //modify the node's value 
                                string value = n.Value;
                                XmlNode valueNode = prototypeDoc.CreateTextNode(value);
                                templateNode.AppendChild(valueNode);

                                //modify the node's attribute values as necessary
                                foreach (XmlAttribute templateAttrib in templateNode.Attributes)
                                {
                                    XmlAttribute rawAttrib = rawNode.Attributes[templateAttrib.Name];
                                    if (rawAttrib != null
                                        && rawAttrib.Value != templateAttrib.Value
                                        && !string.IsNullOrEmpty(rawAttrib.Value)
                                    )
                                        templateAttrib.Value = rawAttrib.Value;
                                }

                                //correct node type found and value- / attribute-value migration accomplished
                                break;
                            }
                        }
                        //matching (by name) PropertyList Property found and handled
                        break;
                    }
                }
            }

            return templatePropList;
        }

        /// <summary>
        /// Returns all 'valid' properties, keyed by section name, from a prototype registration record.
        /// </summary>
        /// <param name="prototypeXml">The xml from which to derive the prototypical PropertyList elements</param>
        /// <returns>List of Property element names based on the existing sections that contain PropertyList elements</returns>
        private Dictionary<string, List<string>> GetPropertiesBySection(string prototypeXml)
        {
            Dictionary<string, List<string>> validSectionProperties = new Dictionary<string, List<string>>();

            XmlDocument nativeDoc = new XmlDocument();
            nativeDoc.LoadXml(prototypeXml);
            string allPropertyNodeXpath = string.Format("//{0}/{1}", PROPERTYLIST, PROPERTY);
            XmlNodeList propertyNodes = nativeDoc.SelectNodes(allPropertyNodeXpath);

            foreach (XmlNode propertyNode in propertyNodes)
            {
                if (propertyNode.ParentNode.ParentNode != null)
                {
                    string sectionName = propertyNode.ParentNode.ParentNode.Name;
                    if (!validSectionProperties.ContainsKey(sectionName))
                    {
                        validSectionProperties.Add(sectionName, new List<string>());
                    }
                    string propertyName = propertyNode.Attributes[_propertyNameAttribute].Value.ToUpper();
                    validSectionProperties[sectionName].Add(propertyName);
                }
            }

            return validSectionProperties;
        }

        /// <summary>
        /// Returns all 'valid' PropertyList XmlNode objects, keyed by section name, from a prototype registration record.
        /// </summary>
        /// <param name="prototypeDoc">The native, prototype XML for new RegistryRecord objects</param>
        /// <returns>A dictionary of template PropertyList XmlNode objects, keyed by containing section name.</returns>
        private Dictionary<string, XmlNode> GetPropertyListsBySection(XmlDocument prototypeDoc)
        {
            Dictionary<string, XmlNode> validPropertyLists = new Dictionary<string, XmlNode>();

            string allPropertyNodeXpath = string.Format("//{0}/{1}", PROPERTYLIST, PROPERTY);
            XmlNodeList propertyNodes = prototypeDoc.SelectNodes(allPropertyNodeXpath);

            foreach (XmlNode propertyNode in propertyNodes)
            {
                if (propertyNode.ParentNode.ParentNode != null)
                {
                    string sectionName = propertyNode.ParentNode.ParentNode.Name;
                    if (!validPropertyLists.ContainsKey(sectionName))
                    {
                        validPropertyLists[sectionName] = propertyNode.ParentNode;
                    }
                }
            }

            return validPropertyLists;
        }

        private Compound findCompound(int compoundId)
        {
            foreach (Component component in _componentList)
            {
                if (component.ComponentIndex == compoundId)
                    return component.Compound;
            }

            return null;
        }

        #endregion

        #region >Private classes, enums<

        [Serializable()]
        private class RegistryStructure : CommandBase
        {
            public static string CheckUnique(RegistryRecord vRegistryRecord)
            {
                RegistryStructure oRegistryStructure = DataPortal.Execute(new RegistryStructure(vRegistryRecord));
                return oRegistryStructure._DuplicateInformation;
            }
            private RegistryRecord _RegistryRecord; // in to Execute
            private string _DuplicateInformation; // out from Execute
            private RegistryStructure(RegistryRecord vRegistryRecord)
            {
                _RegistryRecord = vRegistryRecord;
                return;
            }
            protected override void DataPortal_Execute()
            {
                _RegistryRecord.UpdateXml();
                _DuplicateInformation = _RegistryRecord.RegDal.CanInsertRegistryRecord(
                    _RegistryRecord._xml, _RegistryRecord._checkDuplicates
                );
                return;
            }
        } // class RegistryStructure

        public string CheckUniqueRegistryRecord(DuplicateCheck veDuplicateCheck)
        {
            this._checkDuplicates = veDuplicateCheck;
            return RegistryStructure.CheckUnique(this);
        }

        //public enum DuplicateCheck
        //{
        //    CompoundCheck = 1,  // "C"
        //    MixCheck = 2,       // "M"
        //    None = 3            // "N"
        //}

        //public enum DuplicateAction
        //{
        //    Duplicate = 1,  // "D" Create duplicate
        //    Batch,          // "B" Add batch to existing
        //    Temporary,      // "T" Put into temporary
        //    None,           // "N" Do not store, duplicate is ignored
        //}

        public enum DataAccessStrategy
        {
            /// <summary>
            /// no special care taken to control the creation/opening/closing of any database
            /// connection by the DAL
            /// </summary>
            Atomic = 0,
            /// <summary>
            /// keeps the DAL connection alive between calls until that connection expires from the
            /// cache (via a custom timeout mechanism)
            /// </summary>
            Bulk = 1,
            /// <summary>
            /// keeps the DAL connection alive between calls until that connection expires from the
            /// cache (via a custom timeout mechanism)
            /// </summary>
            BulkLoader = 2,
        }

        [Serializable()]
        private class RegistryDuplicateChecker : CommandBase
        {
            public static IMatchResponse DupCheck(RegistryRecord vRegistryRecord)
            {
                RegistryDuplicateChecker oRegistryStructure = DataPortal.Execute(new RegistryDuplicateChecker(vRegistryRecord));
                return oRegistryStructure._matchResponse;
            }
            private RegistryRecord _RegistryRecord; // in to Execute
            private IMatchResponse _matchResponse; // out from Execute
            private RegistryDuplicateChecker(RegistryRecord vRegistryRecord)
            {
                _RegistryRecord = vRegistryRecord;
                return;
            }
            protected override void DataPortal_Execute()
            {
                _matchResponse = RegistrationMatcher.GetMatches(_RegistryRecord);
                _matchResponse.DuplicateXML = String.Format(_matchResponse.DuplicateXML ,this._RegistryRecord.ID);
                return;
            }
        } // class RegistryDuplicateChecker
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Inserted = null;
            this.Inserting = null;
            this.Loaded = null;
            this.Registering = null;
            this.Updating = null;
            this.UpdatingPerm = null;
            this.Saving = null;
        }

        ~RegistryRecord()
        {
            //garbage collection
        }

        #endregion

    }
}
