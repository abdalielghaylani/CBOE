using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.DataLoader.Calculation.Parser;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.Registration;

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for adding substances to registration
    /// </summary>
    class OutputObjectReg : OutputObjectForm
    {
        private const string _fmtRawResponse = "{0:G}";
        private const string _fmtAddedToPendingReview_1 = "Added to pending review as ID {0:G}";
        private const string _fmtAddedToRegistration_2 = "Added to registration as {0:G}";
        //Fix for CSBR 151262
        //Loading legacy data using Dataloader does not work.
        //Fix for CSBR-156046-The batch number available as constant (1) when resolving duplicates via 'Add Batch' during import to Registry.
        private string _fmtAddedToRegistration_3 = "Batch added to registration as {0:G}";
        private string _fmtAddedToRegistration_4 = "Added to registration as Temperary Record Due to RegNumber(Mapped Field) Conflict";

        //JED: Refers to web reference (which I had named "COERegWebSvc") to the Reg Web Svcs WDSL
        //private COERegWebSvc.COERegistrationServices regSvc;

        static COELog _coeLog = COELog.GetSingleton(_serviceName);
        private const string _serviceName = "COERegistration";
        private RegistryRecord _oRegistryRecord;

        private RegistryRecord _nascentRegistryRecord;
        private RegistrationLoadList _regLoadList = RegistrationLoadList.NewRegistrationLoadList();
        private RegistryRecordList _regList = RegistryRecordList.NewList();
        private int _currentRowsetCount = 0;
        private int _defaultStructureId = -2;
        private string _defaultStructureValue = string.Empty;

        #region > Properties <

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OutputObjectReg()
        {
            OutputType = "Load substances to registration";

            //defer to configuration setting if available
            if (System.Configuration.ConfigurationManager.AppSettings["DefaultStructureId"] != null)
                _defaultStructureId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DefaultStructureId"]);

            //Generate a credentials object based on this ticket
            //regSvc = new CambridgeSoft.COE.DataLoader.COERegWebSvc.COERegistrationServices();
            //COERegWebSvc.COECredentials creds = new COERegWebSvc.COECredentials();
            //creds.AuthenticationTicket = Csla.ApplicationContext.GlobalContext["AuthTicket"].ToString();
            //regSvc.COECredentialsValue = creds;

            bool hasRegTempPermission = false;
            bool hasRegPermPermission = false;

            if (Csla.ApplicationContext.User.Identity.IsAuthenticated == true)
            {
                Structure defaultStructure = Structure.GetStructure(_defaultStructureId);
                _defaultStructureValue = defaultStructure.Value;

                hasRegTempPermission =
                    Csla.ApplicationContext.User.IsInRole("ADD_COMPOUND_TEMP");

                hasRegPermPermission = (
                    Csla.ApplicationContext.User.IsInRole("ADD_COMPONENT")
                    || Csla.ApplicationContext.User.IsInRole("EDIT_COMPOUND_REG")
                );

                if ((hasRegTempPermission == false) && (hasRegPermPermission == false))
                {
                    string message = "Not logged in or user has insufficient privileges.";
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, -1, message);
                }
                else
                {
                    IsValid = true;
                }
            }
            else
            {
                string message = "Not valid or no user is logged in.";
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, -1, message);
            }

            int choices = 0;
            if (hasRegTempPermission)
            {
                choices++;
                RegistrySaveAction = RegistryAction.Save;
            }
            if (hasRegPermPermission == true)
            {
                choices++;
                RegistrySaveAction = RegistryAction.Legacy;
                RegistrySaveAction = RegistryAction.Register;
            }

            WritePermittedActionsConfiguration(hasRegTempPermission, hasRegPermPermission);
        }

        private Nullable<int> _jobChunkSize;
        /// <summary>
        /// The number of records to collect for batching the job activity.
        /// </summary>
        public Nullable<int> JobChunkSize
        {
            get
            {
                if (_jobChunkSize == null)
                {
                    string chunkSize = System.Configuration.ConfigurationManager.AppSettings["JobChunkSize"];
                    if (chunkSize == null)
                        _jobChunkSize = 5;
                    else
                        _jobChunkSize = Convert.ToInt32(chunkSize);
                }
                return _jobChunkSize;
            }
        }

        private Nullable<int> _submissionFormId;
        /// <summary>
        /// The COE Form to be used by the Loader as both a means of defining the loading parameters
        /// as well as the picklists, etc.
        /// </summary>
        public Nullable<int> SubmissionFormId
        {
            get
            {
                if (_submissionFormId == null)
                {
                    string formId = System.Configuration.ConfigurationManager.AppSettings["COESubmissionFormId"];
                    if (formId == null)
                        _submissionFormId = 4015;
                    else
                        try
                        {
                            _submissionFormId = Convert.ToInt32(formId);
                        }
                        catch (Exception exception)
                        {
                            COEExceptionDispatcher.HandleBLLException(exception);
                            _submissionFormId = 4015;
                        }
                }
                return _submissionFormId;
            }
        }

        /// <summary>
        /// This property's setter extracts the instructional information for the
        /// registration process.
        /// </summary>
        public override string Configuration
        {
            set
            {
                //Get the specific output 'action' to take
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(value);
                XmlNode actionNode = doc.SelectSingleNode("//GroupBox[@member='RegistrySaveAction']/@value");
                if (actionNode.Value != null)
                {
                    _eRegistryAction = (RegistryAction)Enum.ToObject(typeof(RegistryAction), Convert.ToInt32(actionNode.Value));
                    if (_eRegistryAction == RegistryAction.Register)
                    {
                        XmlNode dupResolutionNode = doc.SelectSingleNode("//GroupBox[@member='RegDupResolution']/@value");
                        _registerDuplicateResolutionAction = (AutomatedAction)Enum.ToObject(typeof(AutomatedAction), Convert.ToInt32(dupResolutionNode.Value));
                    }
                    else if (_eRegistryAction == RegistryAction.Legacy)
                    {
                        XmlNode dupResolutionNode = doc.SelectSingleNode("//GroupBox[@member='LegacyRegDupResolution']/@value");
                        switch (Convert.ToInt32(dupResolutionNode.Value))
                        {
                            case 0: _legacyDuplicateResolutionAction = AutomatedAction.Batch; break;
                            case 1: _legacyDuplicateResolutionAction = AutomatedAction.Temporary; break;
                        }
                    }
                }

                int usingForm = (int)this.SubmissionFormId;
                if (RegistrySaveAction == RegistryAction.Legacy)
                    ConfigurationSet(value, usingForm, CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode.Add, "RegNum", "String;map;map|calculation", "RegNumber.RegNum");
                else
                    ConfigurationSet(value, usingForm, CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode.Add, string.Empty, string.Empty, string.Empty);

            }
        }

        private AutomatedAction _registerDuplicateResolutionAction = AutomatedAction.Batch;
        /// <summary>
        /// The default diplicate-resolution action for 'Register' action
        /// </summary>
        private AutomatedAction RegisterDuplicateResolutionAction
        {
            get { return _registerDuplicateResolutionAction; }
            set { _registerDuplicateResolutionAction = value; }
        }

        private AutomatedAction _legacyDuplicateResolutionAction = AutomatedAction.Batch;
        /// <summary>
        /// The default diplicate-resolution action for 'Register' action
        /// </summary>
        private AutomatedAction LegacyDuplicateResolutionAction
        {
            get { return _legacyDuplicateResolutionAction; }
            set { _legacyDuplicateResolutionAction = value; }
        }

        private RegistryAction _eRegistryAction = RegistryAction.Save;
        private RegistryAction RegistrySaveAction
        {
            get { return _eRegistryAction; }
            set { _eRegistryAction = value; }
        }

        #endregion

        #region > Methods <

        protected override bool BindStart(int vnTransaction, DataRow voOutputDataRow, int totalRows)
        {
            _coeLog.LogStart("BindStart");
            _currentRowsetCount = totalRows;

            if (_nascentRegistryRecord == null)
            {
                _oRegistryRecord = RegistryRecord.NewRegistryRecord();
                _oRegistryRecord.DataStrategy = RegistryRecord.DataAccessStrategy.BulkLoader;

                _nascentRegistryRecord = _oRegistryRecord.Clone();
            }
            if (_oRegistryRecord != null)
                _oRegistryRecord.Dispose();
            _oRegistryRecord = _nascentRegistryRecord.Clone();

            _coeLog.LogEnd("BindStart");

            return false;
        }

        protected override bool BindAble(string vKey)
        {
            bool isBindable = false;
            switch (vKey.ToLower())
            {
                case MIXTURE:
                case COMPONENT:
                case BATCH:
                case FRAGMENT:
                    {
                        isBindable = true; break;
                    }
                default:
                    break;
            }
            return isBindable;
        }

        protected override Object BindObject(string vKey)
        {
            Object obj = null;
            if (vKey == MIXTURE + "List")
                obj = _oRegistryRecord;
            if (vKey == COMPONENT + "List")
                obj = _oRegistryRecord.ComponentList[0];
            if (vKey == BATCH + "List")
                obj = _oRegistryRecord.BatchList[0];
            if (vKey == MIXTURE + "List" + "@ProjectList")
                obj = _oRegistryRecord.ProjectList;
            if (vKey == BATCH + "List" + "@ProjectList")
                obj = _oRegistryRecord.BatchList[0].ProjectList;
            if (vKey == FRAGMENT)
                obj = _oRegistryRecord.BatchList[0].BatchComponentList[0].BatchComponentFragmentList;
            return obj;
        }

        protected override Object BindObjectByPath(object vParent, string vPath)
        {
            object target = null;
            COEDataBinder binder = new COEDataBinder(vParent);
            if (binder.ContainsProperty(vPath))
                target = binder.RetrieveProperty(vPath);
            return target;
        }

        protected override void BindWrite(int vnTransaction)
        {
            //Default to "No Structure indicator" if the structure is NULL
            foreach (Component c in _oRegistryRecord.ComponentList)
            {
                string value = c.Compound.BaseFragment.Structure.Value;
                bool isNoStructure = false;

                //typically this will simply be NULL but apparently in some cases
                // it may also be "AA==" (appears NOT to happen in an SDFile)
                if (string.IsNullOrEmpty(value) || value == "AA==")
                    isNoStructure = true;

                if (isNoStructure)
                {
                    c.Compound.BaseFragment.Structure.ID = _defaultStructureId;
                    c.Compound.BaseFragment.Structure.Value = _defaultStructureValue;
                }
            }

            //back-fill fragments
            List<BatchComponentFragment> batCompFrags = new List<BatchComponentFragment>();
            foreach (BatchComponentFragment bcf in _oRegistryRecord.BatchList[0].BatchComponentList[0].BatchComponentFragmentList)
            {
                float equivalents = bcf.Equivalents;
                int fragId = bcf.FragmentID;
                if (equivalents > 0 && fragId > 0)
                {
                    batCompFrags.Add(bcf);
                }
            }
            _oRegistryRecord.BatchList[0].BatchComponentList[0].BatchComponentFragmentList.Clear(true);
            foreach (BatchComponentFragment bcf in batCompFrags)
            {
                Fragment frag = FragmentCollection.GetByID(bcf.FragmentID);
                _oRegistryRecord.AddFragment(0, frag, bcf.Equivalents);
            }

            //Jira ID: CBOE-1410
            _oRegistryRecord.ModuleName = CambridgeSoft.COE.Registration.Services.Types.ChemDrawWarningChecker.ModuleName.DATALOADER;
            _regLoadList.AddRegistration(_oRegistryRecord);
        }

        protected override void BindCommit(int vnTransaction)
        {
            // JED: I left this here for debugging (to see the pre-load errors)
            //DataRow[] errantRows = this.OutputDataSet.Tables[0].GetErrors();

            if (RegistrySaveAction == RegistryAction.Save)
                this.DoBulkSubmissionCsla(vnTransaction);

            if (RegistrySaveAction == RegistryAction.Register)
                this.DoBulkRegistrationsCsla(vnTransaction);

            if (RegistrySaveAction == RegistryAction.Legacy)
                this.DoBulkRegistrationsCsla(vnTransaction);

            if (RegistrySaveAction == RegistryAction.FindRegisteredDuplicates)
                this.DoBulkDuplicationCheckCsla(vnTransaction);

        }

        /// <summary>
        /// Packages up N RegistryRecord instances for processing (submitting to temp.)
        /// on the server, reducing network traffic/overhead.
        /// </summary>
        /// <param name="transactionIndex">The overall transaction index</param>
        private void DoBulkSubmissionCsla(int transactionIndex)
        {
            //instruct the BOs to persist themselves
            List<RegRecordSummaryInfo> results = _regLoadList.Submit();
            //log the results
            DoLogResults(results, transactionIndex);
            //prepare for the next batch of results!
            _regLoadList.Clear();
        }

        ///// <summary>
        ///// Performs an individual submission using the Registration web service.
        ///// </summary>
        ///// <param name="vnTransaction">The job's index for the transaction</param>
        //private void DoSubmissionWebSvc(int vnTransaction)
        //{
        //    try
        //    {
        //        DateTime dt0 = DateTime.Now;
        //        string regResponseXml = regSvc.CreateTemporaryRegistryRecord(_oRegistryRecord.Xml);
        //        TimeSpan ts = DateTime.Now - dt0;

        //        AddMessage(
        //            LogMessage.LogSeverity.Information
        //            , LogMessage.LogSource.Output
        //            , vnTransaction
        //            , _fmtRawResponse
        //            , regResponseXml
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        AddMessage(
        //            LogMessage.LogSeverity.Error
        //            , LogMessage.LogSource.Output
        //            , vnTransaction
        //            , _fmtException_2
        //            , new string[] { "RegistryRecord.Save", ex.ToString(), _oRegistryRecord.Xml }
        //        );
        //    }
        //}

        /// <summary>
        /// Packages up N RegistryRecord instances for processing (resgistering to perm,)
        /// on the server, reducing network traffic/overhead.
        /// </summary>
        /// <param name="transactionIndex">The overall transaction index</param>
        private void DoBulkRegistrationsCsla(int transactionIndex)
        {
            int maxQueueSize = (int)this.JobChunkSize;
            int thisQueueSize = (int)_regLoadList.Count;
            int invalidQueueCount = this.OutputDataSet.Tables[0].GetErrors().Length;
            int validQueueCount = (this.OutputDataSet.Tables[0].Rows.Count - invalidQueueCount);

            if (
                //Example: 10 records queued up, and job chunk size is 10
                thisQueueSize == maxQueueSize
                //Example: 4 queued up, but there are only 4 valid ones to begin with
                || thisQueueSize == validQueueCount
                )
            {
                //Determine the automated duplciate-resolution action
                DuplicateAction resolutionAction = DuplicateAction.None;

                if (this.RegistrySaveAction == RegistryAction.Legacy)
                {
                    switch (this.LegacyDuplicateResolutionAction)
                    {
                        case AutomatedAction.Temporary: { resolutionAction = DuplicateAction.Temporary; break; }
                        case AutomatedAction.Duplicate: { resolutionAction = DuplicateAction.Duplicate; break; }
                        case AutomatedAction.Batch: { resolutionAction = DuplicateAction.Batch; break; }
                        case AutomatedAction.None: { resolutionAction = DuplicateAction.None; break; }
                    }
                }
                else if (this.RegistrySaveAction == RegistryAction.Register)
                {
                    switch (this.RegisterDuplicateResolutionAction)
                    {
                        case AutomatedAction.Temporary: { resolutionAction = DuplicateAction.Temporary; break; }
                        case AutomatedAction.Duplicate: { resolutionAction = DuplicateAction.Duplicate; break; }
                        case AutomatedAction.Batch: { resolutionAction = DuplicateAction.Batch; break; }
                        case AutomatedAction.None: { resolutionAction = DuplicateAction.None; break; }
                    }
                }

                /* perform save and fetch results */
                List<RegRecordSummaryInfo> results = _regLoadList.Register(resolutionAction);

                //prepare for the next batch of results!
                _regLoadList.Clear();
                this.OutputDataSet.Tables[0].Rows.Clear();

                //log the results
                DoLogResults(results, transactionIndex);
            }
        }

        private void DoBulkDuplicationCheckCsla(int transactionIndex)
        {
            int maxQueueSize = (int)this.JobChunkSize;
            int thisQueueSize = (int)_regLoadList.Count;
            int invalidQueueCount = this.OutputDataSet.Tables[0].GetErrors().Length;
            int validQueueCount = (this.OutputDataSet.Tables[0].Rows.Count - invalidQueueCount);

            if (
                //Example: 10 records queued up, and job chunk size is 10
                thisQueueSize == maxQueueSize
                //Example: 4 queued up, but there are only 4 valid ones to begin with
                || thisQueueSize == validQueueCount
                )
            {
                /* perform save and fetch results */
                List<DuplicateCheckResponse> rawResults = _regLoadList.CheckDuplicates();

                //TODO: Convert results to RegRecordSummaryInfo objects
                List<RegRecordSummaryInfo> results = RegistrationLoadList.ConvertFromDupCheckResponses(rawResults);

                //prepare for the next batch of results!
                _regLoadList.Clear();
                this.OutputDataSet.Tables[0].Rows.Clear();

                //log the results
                DoLogResults(results, transactionIndex);
            }
        }

        ///// <summary>
        ///// Performs an individual registration using the Registration web service.
        ///// </summary>
        ///// <param name="vnTransaction">The job's index for the transaction</param>
        //private void DoRegistrationWebSvc(int vnTransaction)
        //{
        //    /* <ReturnList>
        //     *  <ActionDuplicateTaken>N</ActionDuplicateTaken>
        //     *  <RegID>1027</RegID>
        //     *  <RegNum>AB-000500</RegNum>
        //     *  <BatchNumber>1</BatchNumber>
        //     *  <BatchID>555</BatchID>
        //     * </ReturnList> */

        //    string duplicateResolution = DuplicateResolutionAction.ToString().Substring(0, 1);
        //    try
        //    {
        //        DateTime dt0 = DateTime.Now;
        //        string regResponseXml = regSvc.CreateRegistryRecord(_oRegistryRecord.Xml, duplicateResolution);
        //        TimeSpan ts = DateTime.Now - dt0;

        //        AddMessage(
        //            LogMessage.LogSeverity.Information
        //            , LogMessage.LogSource.Output
        //            , vnTransaction
        //            , _fmtRawResponse
        //            , regResponseXml
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        AddMessage(
        //            LogMessage.LogSeverity.Error
        //            , LogMessage.LogSource.Output
        //            , vnTransaction
        //            , _fmtException_2
        //            , new string[] { "RegistryRecord.Register", ex.ToString(), _oRegistryRecord.Xml }
        //        );
        //    }
        //}

        /// <summary>
        /// Extracts transaction information from each of the RegRecordSummaryInfo objects
        /// and generates log entries.
        /// </summary>
        /// <param name="results">A list of RegRecordSummaryInfo objects</param>
        /// <param name="transactionIndex">The overall transactional index for the entire job</param>
        private void DoLogResults(List<RegRecordSummaryInfo> results, int transactionIndex)
        {
            string strChemicalWarning = string.Empty;
            foreach (RegRecordSummaryInfo info in results)
            {
                int index = results.IndexOf(info);
                int trueIndex = LoadableRecordIndices[index];

                if (string.IsNullOrEmpty(info.Message))
                {
                    //Fix for CSBR-161150- Message, "Added to pending review as ID 0" needs to be corrected
                    string strTemp = Convert.ToString(info.TempNum);
                    if (!string.IsNullOrEmpty(strTemp))
                    {
                        strChemicalWarning = string.Empty;
                        if (info.IsRedBoxWarningExists)
                            strChemicalWarning = Environment.NewLine + info.RedBoxWarningMessage;        //Jira ID: CBOE-1410
                        AddMessage(
                            LogMessage.LogSeverity.Information
                            , LogMessage.LogSource.Output
                            , trueIndex
                            , _fmtAddedToPendingReview_1 + "\t" + strChemicalWarning
                            , new string[] { info.TempNum }
                        );
                    }
                    else
                    {
                        //Fix for CSBR 151262
                        //Loading legacy data using Dataloader does not work.
                        //Fix for CSBR-156046-The batch number available as constant (1) when resolving duplicates via 'Add Batch' during import to Registry.
                        //Fix for CSBR-161150- Message, "Added to pending review as ID 0" needs to be corrected
                        string strReg = Convert.ToString(info.RegNum);
                        if (strReg != "null" && strReg != "0")
                        {
                            _fmtAddedToRegistration_3 = "Batch added to registration {0:G}";
                        }
                        else
                        {
                            _fmtAddedToRegistration_3 = "Duplicate found. Batch was not added.";
                        }
                        strChemicalWarning = string.Empty;
                        if (info.IsRedBoxWarningExists)
                            strChemicalWarning = Environment.NewLine + info.RedBoxWarningMessage;        //Jira ID: CBOE-1640
                        AddMessage(
                            LogMessage.LogSeverity.Information
                            , LogMessage.LogSource.Output
                            , trueIndex
                            , _fmtAddedToRegistration_3 + "\t" + strChemicalWarning
                            , new string[] { info.RegNum }
                        );
                    }

                }
                else
                {
                    //Fix for CSBR 151262
                    //Loading legacy data using Dataloader does not work.
                    //this code changes displays geniric message to UI for RegNumber(Mapped Field) Conflict.
                    //Fix for CSBR-156046-The batch number available as constant (1) when resolving duplicates via 'Add Batch' during import to Registry.
                    if (info.Message.Contains("ORA-00001:") && info.Message.ToUpper().Contains("UNIQUE CONSTRAINT") && info.Message.ToUpper().Contains("REGDB.UNQ_REGNUM_REG_NUMBER"))
                    {
                        AddMessage(
                                  LogMessage.LogSeverity.Information
                                  , LogMessage.LogSource.Output
                                  , trueIndex
                                  , _fmtAddedToRegistration_4
                                  , new string[] { info.RegNum, info.BatchCount.ToString() }
                              );
                    }

                    else if (info.DiagnosticLevel == System.Diagnostics.EventLogEntryType.Information)
                    {
                        AddMessage(
                            LogMessage.LogSeverity.Information
                            , LogMessage.LogSource.Output
                            , trueIndex
                            , _fmtRawResponse
                            , new string[] { info.Message }
                        );
                    }
                    else if (info.DiagnosticLevel == System.Diagnostics.EventLogEntryType.Warning)
                    {
                        AddMessage(
                            LogMessage.LogSeverity.Warning
                            , LogMessage.LogSource.Output
                            , trueIndex
                            , _fmtRawResponse
                            , new string[] { info.Message }
                        );
                    }
                    else
                    {
                        AddMessage(
                            LogMessage.LogSeverity.Error
                            , LogMessage.LogSource.Output
                            , trueIndex
                            , _fmtException_2
                            , new string[] { "Registration", info.Message, info.RegistryXml })
                        ;
                    }
                }
            }

            LoadableRecordIndices.Clear();
        }

        private void WritePermittedActionsConfiguration(bool hasRegTempPermission, bool hasRegPermPermission)
        {
            if (hasRegTempPermission == true || hasRegPermPermission == true)
            {
                using (COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter())     // Coverity Fix- CBOE-1941
                {
                    oCOEXmlTextWriter.WriteStartElement("OutputConfiguration");
                    oCOEXmlTextWriter.WriteAttributeString("text", "Configuration");
                    oCOEXmlTextWriter.WriteStartElement("GroupBox");
                    oCOEXmlTextWriter.WriteAttributeString("text", "Add substance to");
                    oCOEXmlTextWriter.WriteAttributeString("member", "RegistrySaveAction");

                    oCOEXmlTextWriter.WriteStartElement("RadioButton");
                    oCOEXmlTextWriter.WriteAttributeString("text", "Pending review");
                    oCOEXmlTextWriter.WriteEndElement();

                    if (hasRegPermPermission == true)
                    {
                        oCOEXmlTextWriter.WriteStartElement("RadioButton");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Registered");
                        oCOEXmlTextWriter.WriteEndElement();

                        oCOEXmlTextWriter.WriteStartElement("GroupBox");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Duplicate action");
                        oCOEXmlTextWriter.WriteAttributeString("member", "RegDupResolution");
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Add as a new batch of the registered substance");
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Add as a duplicate registered substance");
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Add to substances pending review");
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "No action - do not import if duplciates exist");
                            oCOEXmlTextWriter.WriteEndElement();
                            /*
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "ONLY perform duplicate-checking; this option does NOT load data");
                            oCOEXmlTextWriter.WriteEndElement();
                            */
                        }
                        oCOEXmlTextWriter.WriteEndElement();//GroupBox

                        oCOEXmlTextWriter.WriteStartElement("RadioButton");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Register with known RegNum values");
                        oCOEXmlTextWriter.WriteEndElement();//RadioButton

                        oCOEXmlTextWriter.WriteStartElement("GroupBox");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Duplicate action");
                        oCOEXmlTextWriter.WriteAttributeString("member", "LegacyRegDupResolution");
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Add as a new batch of the registered substance");
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Add to substances pending review");
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();//GroupBox

                        oCOEXmlTextWriter.WriteStartElement("RadioButton");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Check for Registered duplicates");
                        oCOEXmlTextWriter.WriteEndElement();

                        oCOEXmlTextWriter.WriteEndElement();
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    else if (hasRegPermPermission == false)
                    {
                        oCOEXmlTextWriter.WriteEndElement();
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    UnboundConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    //oCOEXmlTextWriter.Close();        // Coverity Fix- CBOE-1941
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Parallels RegistryRecord.DuplicateCheck
    /// </summary>
    enum AutomatedAction
    {
        /// <summary>
        /// Batch
        /// </summary>
        Batch,
        /// <summary>
        /// Duplicate
        /// </summary>
        Duplicate,
        /// <summary>
        /// Temporary
        /// </summary>
        Temporary,
        /// <summary>
        /// Take no action if a duplciate is found
        /// </summary>
        None,
        /// <summary>
        /// Check for duplicates, do NOT upload any data
        /// </summary>
        DisplayOnly,
    };

    /// <summary>
    /// Requested registry action
    /// </summary>
    enum RegistryAction
    {
        /// <summary>
        /// Save to temporary pending review
        /// </summary>
        Save,
        /// <summary>
        /// Register
        /// </summary>
        Register,
        /// <summary>
        /// Register with known RegNum values
        /// </summary>
        Legacy,
        /// <summary>
        /// Check for duplciates; does NOT import to repository
        /// </summary>
        FindRegisteredDuplicates,
    };

}
