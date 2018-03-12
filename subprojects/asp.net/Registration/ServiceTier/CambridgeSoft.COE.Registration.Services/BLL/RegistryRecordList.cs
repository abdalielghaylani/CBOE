using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Access;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.Properties;

using Csla;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Container class for multiple RegistryRecord instances.
    /// </summary>
    [Serializable()]
    public class RegistryRecordList : ReadOnlyListBase<RegistryRecordList, RegistryRecord>
    {
        [NonSerialized]
        private string _registerXmlIds = string.Empty;

        private DataTable _dtLogInfo;
        /// <summary>
        /// The DataTable for business object to get data from Log table.
        /// </summary>
        public DataTable DTLogInfo
        {
            get
            {
                return _dtLogInfo;
            }
            set
            {
                _dtLogInfo = value;
            }
        }

        private static ChemDrawWarningChecker.ModuleName _moduleName = ChemDrawWarningChecker.ModuleName.NOTSET;
        /// <summary>
        /// The Module name
        /// </summary>
        public static ChemDrawWarningChecker.ModuleName ModuleName
        {
            get
            {
                return _moduleName;
            }
            set
            {
                _moduleName = value;
            }
        }

        public static bool CanGetObject()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        #region Factory Methods

        [COEUserActionDescription("GetTempRecordCountInRegistryRecordList")]
        public static int GetTempRecordCount(int SavedHitlistID)
        {

            try
            {
                GetTempRecordCount result = DataPortal.Execute<GetTempRecordCount>(new GetTempRecordCount(SavedHitlistID));
                return result.RecordCount;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return 0;
            }
        }

        [COEUserActionDescription("GetRegistryRecordList")]
        public static RegistryRecordList GetList(string filter)
        {
            try
            {
                Criteria criteria = new Criteria(filter);

                return DataPortal.Fetch<RegistryRecordList>(criteria);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetTempRegistryRecordList")]
        public static RegistryRecordList GetTemporaryList(string filter)
        {
            try
            {
                Criteria criteria = new Criteria(filter, true);

                return DataPortal.Fetch<RegistryRecordList>(criteria);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetRegistryRecordList")]
        public static RegistryRecordList GetListFomXml(string xml)
        {
            try
            {
                return DataPortal.Fetch<RegistryRecordList>(new XmlCriteria(xml));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Factory method to generate a RegistryRecordList using a (potentially) shared component.
        /// </summary>
        /// <param name="mixtureComponent"></param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("GetListFromSharableComponent")]
        public static RegistryRecordList GetListFromSharableComponent(Component mixtureComponent)
        {
            try
            {
                return DataPortal.Fetch<RegistryRecordList>(CriteriaSharedComponent.NewCriteriaSharedComponent(mixtureComponent));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
        

        /// <summary>
        /// Factory method to generate a RegistryRecordList using the registry ID of a (potentially) shared component.
        /// </summary>
        /// <param name="mixtureComponentId"></param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("GetListFromSharableComponent")]
        public static RegistryRecordList GetListFromSharableComponent(int mixtureComponentId)
        {
            try
            {
                return DataPortal.Fetch<RegistryRecordList>(CriteriaSharedComponent.NewCriteriaSharedComponent(mixtureComponentId));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [RunLocal()]
        [COEUserActionDescription("CreateRegistryRecordList")]
        public static RegistryRecordList NewList()
        {
            try
            {
                return new RegistryRecordList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        public new bool Remove(RegistryRecord registry)
        {
            return this.Items.Remove(registry);
        }

        private void LoadFromXml(string xml)
        {
            //added on 2008-12-02 for the question:"Insert is an invalid operation"
            IsReadOnly = false;

            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("MultiCompoundRegistryRecordList/MultiCompoundRegistryRecord");

            bool more = xIterator.MoveNext();
            while (more)
            {
                //Get registry 0 or null (template) instead of newregistryrecrod so that it's marked as old.
                RegistryRecord record = RegistryRecord.GetRegistryRecord(null);//RegistryRecord.NewRegistryRecord();
                record.InitializeFromXml(xIterator.Current.OuterXml, false, true);

                this.Add(record);
                more = xIterator.Current.MoveToNext();
            }

            //added on 2008-12-02 for the question:"Insert is an invalid operation"
            IsReadOnly = true;
        }

        /// <summary>
        /// Service method that accepts a HitList identifier
        /// </summary>
        /// <param name="actionDuplicates"></param>
        /// <param name="hitListId"></param>
        /// <param name="logMessage"></param>
        /// <param name="actor">The user performing the action.</param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("LoadRegistryRecords")]
        public static RegRecordListInfo LoadRegistryRecordsByHitList(
            DuplicateAction actionDuplicates
            , int hitListId
            , string actor
            , string logMessage
            )
        {
            RegRecordListInfo _reglistInfo = null;

            try
            {
                //security-check
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException("User not authorized to create a RegistryRecord");
                }
                
                //perform action
                string message = string.Empty;
                CriteriaPromote oPro = CriteriaPromote.PerformPromotion(actionDuplicates, hitListId, actor, logMessage);
                _reglistInfo = new RegRecordListInfo();
                _reglistInfo.IntLogId = oPro.LogId;
            }
            catch(Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
            finally { }

            return _reglistInfo;

        }

        /// <summary>
        /// DataLoader use this method to bulk register xml. 
        /// </summary>
        /// <param name="actionDuplicates">register rocords's Duplicate action</param>
        /// <param name="strxml">The register records's information is placed in strxml</param>
        /// <returns>the logged id</returns> 
        /// 
        [COEUserActionDescription("LoadRegistryRecords")]
        public static RegRecordListInfo LoadRegistryRecordList(DuplicateAction actionDuplicates, string strxml)
        {
            Int32 intLogID = 0;
            RegRecordListInfo _reglistInfo = null;

            try
            {
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException("User not authorized to view a RegistryRecord");
                }
                intLogID = CriteriaLog.InsertCriteriaLog(actionDuplicates, string.Empty, string.Empty);
                CriteriaLoad.setCriteriaLoad(strxml, actionDuplicates, "", "DataLoader", intLogID);
                _reglistInfo = new RegRecordListInfo();
                _reglistInfo.IntLogId = intLogID;

                return _reglistInfo;
            }
            catch (Exception e)
            {
                COEExceptionDispatcher.HandleBLLException(e);
                return null;
            }
        }

        /// <summary>
        /// ChemBioViz use this method to bulk register xml.
        /// </summary>
        /// <param name="actionDuplicates">register rocords's Duplicate action</param>
        /// <param name="hitListId">User hits the marked id saved in coesaved hitlist table's id</param>
        /// <param name="dateStamp">User hits the marked id's date saved in coesaved hitlist table's id</param>
        /// <returns>the logged id</returns> 
        //public static RegRecordListInfo LoadRegistryRecordList(RegistryRecord.DuplicateAction actionDuplicates, int hitListId, string dateStamp)
        [COEUserActionDescription("LoadRegistryRecords")]
        public static RegRecordListInfo LoadRegistryRecordList(DuplicateAction actionDuplicates, int hitListId)
        {
            RegistryRecordList _tempRecordList = null;
            string strxml = "";
            string[] strID;
            Int32 intLogID;
            RegRecordListInfo _reglistInfo = null;
            try
            {
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException("User not authorized to view a RegistryRecord");
                }

                //_tempRecordList = GetRegistryRecordList(hitListId, dateStamp);
                _tempRecordList = GetRegistryRecordList(hitListId);

                //RegistryRecordList.InsertLogInfoTable(duplicateAction, string.Empty);

                if (_tempRecordList._registerXmlIds != string.Empty)
                {
                    intLogID = CriteriaLog.InsertCriteriaLog(actionDuplicates, string.Empty, string.Empty);

                    StringBuilder builder = new StringBuilder("");
                    builder.Append("<MultiCompoundRegistryRecordList>");

                    strID = _tempRecordList._registerXmlIds.Split(char.Parse(","));
                    for (int i = 0; i < strID.Length; i++)
                    {
                        RegistryRecord record = RegistryRecord.GetRegistryRecord(int.Parse(strID[i]));

                        if(record != null)
                        {
                            record.UpdateXml();
                            builder.Append(record.Xml);

                            //if (i > 0 && (i + 1) % 2 == 0)
                            if(i > 0 && (i + 1) % 100 == 0)
                            {
                                if(i != strID.Length - 1)
                                {
                                    builder.Append("</MultiCompoundRegistryRecordList>");
                                    strxml = builder.ToString();
                                    //DataPortal.Update(new CriteriaLoad(strxml, actionDuplicates, ""));
                                    CriteriaLoad.setCriteriaLoad(strxml, actionDuplicates, "", "Registration", intLogID);

                                    builder.Remove(0, builder.Length);
                                    builder.Append("<MultiCompoundRegistryRecordList>");
                                }
                            }
                        }
                    }

                    builder.Append("</MultiCompoundRegistryRecordList>");
                    strxml = builder.ToString();

                    // Use LoadList to bulk load Data.
                    CriteriaLoad.setCriteriaLoad(strxml, actionDuplicates, "", "Registration", intLogID);

                    // Delete the hitListId from coeSaveHitList table
                    COEHitListBO marked = COEHitListBO.Get(HitListType.MARKED, hitListId);
                    for (int i = 0; i < strID.Length; i++)
                    {
                        marked.UnMarkHit(int.Parse(strID[i]));
                    }

                    _reglistInfo = new RegRecordListInfo();
                    _reglistInfo.IntLogId = intLogID;
                }
            }
            catch (Exception e)
            {
                //DataPortal.Fetch<RegistryRecord>(new CriteriaTemp(xml));
                COEExceptionDispatcher.HandleBLLException(e);
                return null;
            }
            finally
            {

            }

            return _reglistInfo;
        }

        /// <summary>
        /// ChemBioViz use this method to bulk register xml.
        /// </summary>
        /// <param name="actionDuplicates">register rocords's Duplicate action</param>
        /// <param name="hitListId">User hits the marked id saved in coesaved hitlist table's id</param>
        /// <param name="bApprovalsEnabled">Status of Approval Workflow Enabled/Disabled</param>
        /// <returns>RegRecordListInfo</returns> 
        [COEUserActionDescription("LoadRegistryRecords")]
        public static RegRecordListInfo LoadRegistryRecordList(DuplicateAction actionDuplicates, int hitListId, bool bApprovalsEnabled, string actor, string logMessage)
        {
            RegistryRecordList _tempRecordList = null;
            string[] strID;
            Int32 intLogID;
            RegRecordListInfo _reglistInfo = null;
            try
            {
                if (!CanGetObject())
                {
                    throw new System.Security.SecurityException(Resources.NotAuthorizedViewRegistryRecord);
                }

                _tempRecordList = GetRegistryRecordList(hitListId);

                if (_tempRecordList._registerXmlIds != string.Empty)
                {
                    intLogID = CriteriaLog.InsertCriteriaLog(actionDuplicates, actor, logMessage);
                    strID = _tempRecordList._registerXmlIds.Split(char.Parse(","));
                    for (int i = 0; i < strID.Length; i++)
                    {
                        RegistryRecord record = _tempRecordList[i];
                        if (record != null)
                        {
                            if (!record.IsEditable || !record.CanEditRegistry())
                                CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, strID[i], actionDuplicates, string.Empty, 0, Resources.NotAuthorizedToRegisterTempRecords);
                            else if (bApprovalsEnabled && record.Status != RegistryStatus.Approved)
                                CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, strID[i], actionDuplicates, string.Empty, 0, Resources.NotApprovedCannotRegister);
                            else
                            {
                                record.DataStrategy = RegistryRecord.DataAccessStrategy.BulkLoader;
                                record.ModuleName = _moduleName;
                                
                                RegistryRecord result = record.Register(actionDuplicates, record.DataStrategy);
                             
                                if (result != null && !string.IsNullOrEmpty(result.RegNumber.RegNum) && result.RegNumber.RegNum.ToLower() != "null")
                                {
                                    if (actionDuplicates != DuplicateAction.Temporary && actionDuplicates != DuplicateAction.None)
                                    {
                                        //Next two lines are to make the record deletable
                                        record.Status = RegistryStatus.Submitted;
                                        record.SetApprovalStatus();
                                        RegistryRecord.DeleteRegistryRecord(record.ID);
                                        CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, string.Empty, (string.IsNullOrEmpty(record.FoundDuplicates) ? result.ActionDuplicates : actionDuplicates), result.RegNumber.RegNum, 0, result.DalResponseMessage);
                                    }
                                    else if (string.IsNullOrEmpty(record.FoundDuplicates) && (actionDuplicates == DuplicateAction.Temporary || actionDuplicates == DuplicateAction.None))
                                    {
                                        //Next two lines are to make the record deletable
                                        record.Status = RegistryStatus.Submitted;
                                        record.SetApprovalStatus();
                                        RegistryRecord.DeleteRegistryRecord(record.ID);
                                        CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, string.Empty, DuplicateAction.Compound, result.RegNumber.RegNum, 0, result.DalResponseMessage);
                                    }
                                    else
                                    {
                                        string message = result.DalResponseMessage;
                                        if ((actionDuplicates != DuplicateAction.Temporary || actionDuplicates != DuplicateAction.None) && !string.IsNullOrEmpty(record.FoundDuplicates) && string.IsNullOrEmpty(message))
                                            message = "Duplicate found";
                                        CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, strID[i], actionDuplicates, string.Empty, 0, message);
                                    }
                                }
                                else
                                {
                                    //Coverity Fix 19155
                                    if (result != null)
                                    {
                                        if ((result.IsTemporal && result.SubmitCheckRedBoxWarning) || (!result.IsTemporal && result.RegisterCheckRedBoxWarning))
                                        {
                                            //string  message = "Chem Draw warnings found";
                                            string message = Resources.RedBoxWarningMessage_BulkReg;
                                            CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, strID[i], (actionDuplicates == DuplicateAction.None ? DuplicateAction.None : DuplicateAction.Temporary), string.Empty, 0, message);
                                        }
                                        else
                                        {
                                            string message = result.DalResponseMessage;
                                            if (!string.IsNullOrEmpty(record.FoundDuplicates) && string.IsNullOrEmpty(message))
                                                message = "Duplicate found";
                                            CriteriaBulkLog.InsertCriteriaBulkLog(intLogID, strID[i], (actionDuplicates == DuplicateAction.None ? DuplicateAction.None : DuplicateAction.Temporary), string.Empty, 0, message);
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                    // Delete the hitListId from coeSaveHitList table
                    COEHitListBO marked = COEHitListBO.Get(HitListType.MARKED, hitListId);
                    for (int i = 0; i < strID.Length; i++)
                    {
                        marked.UnMarkHit(int.Parse(strID[i]));
                    }

                    _reglistInfo = new RegRecordListInfo();
                    _reglistInfo.IntLogId = intLogID;
                }
            }
            catch (Exception e)
            {
                COEExceptionDispatcher.HandleBLLException(e);
                return null;
            }
            finally
            {

            }

            return _reglistInfo;
        }

        private RegistryRecordList()
        { /* require use of factory methods */ }

        /// <summary>
        /// we can get register records's id use this method in chemBioViz. 
        /// </summary>
        /// <param name="hitListId">User hits the marked id saved in coesaved hitlist table's id</param>
        /// <param name="dateStamp">User hits the marked id's date saved in coesaved hitlist table's id</param>
        /// <returns>RegistryRecordList</returns> 
        //private static RegistryRecordList GetRegistryRecordList(int hitListId, string dateStamp)
        [COEUserActionDescription("GetRegistryRecordList")]
        public static RegistryRecordList GetRegistryRecordList(int hitListId)
        {
            try
            {
                //return new RegistryRecordList();
                RegistryRecordList result = null;
                //result = DataPortal.Fetch<RegistryRecordList>(new CriteriaTemp(hitListId, dateStamp));
                result = DataPortal.Fetch<RegistryRecordList>(new CriteriaTemp(hitListId));
                return result;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("GetRegistryRecordList")]
        public static RegistryRecordList GetRegistryRecordList(int hitListId, bool isTemporary)
        {
            try
            {
                //return new RegistryRecordList();
                RegistryRecordList result = null;
                //result = DataPortal.Fetch<RegistryRecordList>(new CriteriaTemp(hitListId, dateStamp));
                result = DataPortal.Fetch<RegistryRecordList>(new HitListCriteria(hitListId, isTemporary));
                return result;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        /// <summary>
        /// added on 2008-11-25 for get and update the register Log info. 
        /// </summary>
        /// 
        [COEUserActionDescription("GetRegisterLogInfo")]
        public static DataTable GetLogInfoTable(int logId)
        {
            try
            {
                RegistryRecordList lst = DataPortal.Fetch<RegistryRecordList>(new CriteriaLog(logId));

                return lst.DTLogInfo;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// update the register Log info. 
        /// </summary>
        /// 
        [COEUserActionDescription("UpdateRegisterLogInfo")]
        public static void UpdateLogInfo(int logId, string strLogDescription)
        {
            try
            {
                CriteriaLog.SetCriteriaLog(logId, strLogDescription);            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }
        //end added

        static COEConfigurationManager configurationManager;

        private static COEConfigurationManager GetConfigurationManager()
        {
            if (configurationManager == null)
            {
                configurationManager = new COEConfigurationManager();
            }

            return configurationManager;
        }

        /// <summary>
        /// JED: ???
        /// </summary>
        /// <param name="linkName"></param>
        /// <returns></returns> 
        /// 
        [COEUserActionDescription("GetChemVizUrl")]
        public static string chemVizUrl(string linkName)
        {
            try
            {
                COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
                Microsoft.Practices.EnterpriseLibrary.Common.Configuration.NamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
                ApplicationData appName = myApps.Get(COEAppName.Get());
                linkParameters lp = appName.LinkParameters.Get(linkName);
                return lp.FormGroupId + "," + lp.SearchCriteriaId;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        #region Data Access

        [NonSerialized, NotUndoable]
        private RegistrationOracleDAL _regDal = null;
        /// <summary>
        /// DAL implementation.
        /// </summary>
        private RegistrationOracleDAL RegDal
        {
            get
            {
                if (_regDal == null)
                    DalUtils.GetRegistrationDAL(ref _regDal, Constants.SERVICENAME);
                return _regDal;
            }
        }

        /// <summary>
        /// Clears the StructureAggregation property to force re-aggregation via the add-in.
        /// This method is intended to be run only if the List is derived from the
        /// "CriteriaSharedComponent" criterion.
        /// </summary>
        public void UpdateAggregateStructures()
        {
            foreach (RegistryRecord r in this)
            {
                r.StructureAggregation = string.Empty;
                r.CheckOtherMixtures = false; //this will be unnecessary at this point
                r.AggregateOtherMixtures = false; //avoid an infinite loop!
                foreach (Component c in r.ComponentList)
                {
                    //JED: This bypasses strcuture normalization, since it is reverting to the OLD structure!
                    c.Compound.BaseFragment.Structure.IsTemporary = false;
                    c.Compound.BaseFragment.Structure.NormalizedStructure =
                        c.Compound.BaseFragment.Structure.Value;
                }
                //JED: For SOME reason, the records thinks it's a TEMP one, so override this
                r.Save(DuplicateCheck.None, RegistryRecord.DataAccessStrategy.Bulk);
                r.Dispose();
            }
        }

        /// <summary>
        /// Factory method to generate a RegistryRecordList using the structure ID of a (potentially) linked structureId
        /// </summary>
        /// <param name="structureId">the id of the potentially linked structure</param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("GetRegistryRecordListFromLinkedStructure")]
        public static RegistryRecordList GetListFromLinkedStructure(int structureId)
        {
            try
            {
                return DataPortal.Fetch<RegistryRecordList>(CriteriaLinkedStructure.NewCriteriaLinkedStructure(structureId));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// "Fetch" criteria used when the RegistryRecordList should be populated by all RegistryRecords
        /// that share the component specified.
        /// </summary>
        [Serializable()]
        private class CriteriaSharedComponent
        {
            private int _compoundId;
            public int CompoundId
            {
                get { return _compoundId; }
            }

            /// <summary>
            /// Factory method to create the new fetch criterion.
            /// </summary>
            /// <param name="sharableComponent"></param>
            /// <returns></returns>
            public static CriteriaSharedComponent NewCriteriaSharedComponent(Component sharableComponent)
            {
                return new CriteriaSharedComponent(sharableComponent.Compound.ID);
            }

            /// <summary>
            /// Factory method to create the new fetch criterion.
            /// </summary>
            /// <param name="sharableComponentId"></param>
            /// <returns></returns>
            public static CriteriaSharedComponent NewCriteriaSharedComponent(int sharableCompoundId)
            {
                return new CriteriaSharedComponent(sharableCompoundId);
            }

            /// <summary>
            /// Private constructor
            /// </summary>
            /// <param name="componentId"></param>
            private CriteriaSharedComponent(int compoundId)
            {
                _compoundId = compoundId;
            }
        }

        [Serializable()]
        private class CriteriaPromote
        {
            [NonSerialized, NotUndoable]
            private RegistrationOracleDAL _coeRegistrationDAL = null;

            //private members
            private DuplicateAction _duplicateResolutionAction;
            private int _hitlistId;
            private string _actor;
            private string _logMessage;
            
            private Int32 _logId = 0;

            /// <summary>
            /// The action to take when duplicate records are found.
            /// </summary>
            public DuplicateAction DuplicationResolution
            {
                get { return _duplicateResolutionAction; }
            }

            /// <summary>
            /// the HitList to act on.
            /// </summary>
            public int HitlistId
            {
                get { return _hitlistId; }
            }

            /// <summary>
            /// The user/entity prompting the event (the 'actor')
            /// </summary>
            public string Actor
            {
                get { return _actor; }
            }

            /// <summary>
            /// The log message to apply to the batch-processing log
            /// </summary>
            public string LogMessage
            {
                get { return _logMessage; }
            }

            /// <summary>
            /// The batch-processing identifier assigned by the server process.
            /// </summary>
            public Int32 LogId
            {
                get { return _logId; }
            }

            /// <summary>
            /// Private constructor
            /// </summary>
            /// <param name="actionDuplicates"></param>
            /// <param name="hitListId"></param>
            /// <param name="logMessage"></param>
            private CriteriaPromote(
                DuplicateAction actionDuplicates
                , int hitListId
                , string actor
                , string logMessage
                )
            {
                _duplicateResolutionAction = actionDuplicates;
                _hitlistId = hitListId;
                _actor = actor;
                _logMessage = logMessage;
                LoadDAL();
            }

            /// <summary>
            /// Public action taken
            /// </summary>
            /// <param name="actionDuplicates"></param>
            /// <param name="hitListId"></param>
            /// <param name="logMessage"></param>
            public static CriteriaPromote PerformPromotion(
                DuplicateAction actionDuplicates
                , int hitListId
                , string actor
                , string logMessage
                )
            {
                CriteriaPromote cPro = new CriteriaPromote(actionDuplicates, hitListId, actor, logMessage);
                DataPortal.Update(cPro);
                return cPro;
            }

            private void LoadDAL()
            {
                if (_coeRegistrationDAL == null)
                    DalUtils.GetRegistrationDAL(ref _coeRegistrationDAL, Constants.SERVICENAME);
            }

            private void DataPortal_Update()
            {
                LoadDAL();
                this._logId = _coeRegistrationDAL.PromoteTempRegistryRecordList(
                    this._duplicateResolutionAction
                    , this._hitlistId
                    , this._actor
                    , this._logMessage
                );
            }

        }

        /// <summary>
        /// bulk register the records to database.
        /// </summary>
        [Serializable()]
        private class CriteriaLoad
        {
            [NonSerialized, NotUndoable]
            private RegistrationOracleDAL _coeRegistrationDAL = null;

            private string _strXml;
            public string StrXml
            {
                get { return _strXml; }
                set { _strXml = value; }
            }

            private DuplicateAction _duplication;
            public DuplicateAction Duplication
            {
                get { return _duplication; }
                set { _duplication = value; }
            }

            private string _strTempId;
            public string StrTempId
            {
                get { return _strTempId; }
                set { _strTempId = value; }
            }

            private Int32 _intLogId;
            public Int32 IntLogId
            {
                get { return _intLogId; }
                set { _intLogId = value; }
            }

            //added for DataLoader
            private string _registerName;
            public string RegisterName
            {
                get { return _registerName; }
                set { _registerName = value; }
            }

            private CriteriaLoad(
                string strXml
                , DuplicateAction duplication
                , string strTempId
                , string registerName
                , Int32 intLogID
                )
            {
                _strXml = strXml;
                _duplication = duplication;
                //_strTempId = strTempId;
                _registerName = registerName;
                _intLogId = intLogID;
                LoadDAL();
            }
            //end added

            public static void setCriteriaLoad(string strXml, DuplicateAction duplication, string strTempId, string registerName, Int32 intLogID)
            {
                DataPortal.Update(new CriteriaLoad(strXml, duplication, strTempId, registerName, intLogID));
            }

            private void LoadDAL()
            {
                if (_coeRegistrationDAL == null)
                    DalUtils.GetRegistrationDAL(ref _coeRegistrationDAL, Constants.SERVICENAME);
            }

            private void DataPortal_Update()
            {
                if (_registerName == "DataLoader")
                {
                    _coeRegistrationDAL.InsertRegistryRecordList(_strXml, _duplication, _intLogId);
                }
                else
                    _coeRegistrationDAL.InsertRegistryRecordList(_strXml, _duplication, _intLogId);

            }
        }

        /// <summary>
        /// Get the Log id info to update the log id table description
        /// </summary>
        [Serializable()]
        private class CriteriaTemp
        {

            private int _hitListId;
            public int HitListId
            {
                get { return _hitListId; }
                set { _hitListId = value; }
            }

            private string _dateStamp;
            public string DateStamp
            {
                get { return _dateStamp; }
                set { _dateStamp = value; }
            }

            //public CriteriaTemp(int hitListId, string dateStamp)
            //{
            //    _hitListId = hitListId;
            //    _dateStamp = dateStamp;
            //}
            public CriteriaTemp(int hitListId)
            {
                _hitListId = hitListId;
            }
        }

        /// <summary>
        /// Fetch criterion object utilizing a hit-list identifier. Upon execution, the RegistryRecordList
        /// should contain only RegistryRecords currently on that hit-list.
        /// </summary>
        [Serializable()]
        private class HitListCriteria
        {

            private int _hitListId;
            public int HitListId
            {
                get { return _hitListId; }
                set { _hitListId = value; }
            }

            private string _dateStamp;
            public string DateStamp
            {
                get { return _dateStamp; }
                set { _dateStamp = value; }
            }

            private bool _isTemporal;
            public bool IsTemporal
            {
                get {
                    return _isTemporal;
                }
                set {
                    _isTemporal = value;
                }
            }

            //public CriteriaTemp(int hitListId, string dateStamp)
            //{
            //    _hitListId = hitListId;
            //    _dateStamp = dateStamp;
            //}
            public HitListCriteria(int hitListId, bool isTemporal)
            {
                _hitListId = hitListId;
                _isTemporal = isTemporal;
            }
        }

        [Serializable()]
        private class Criteria
        {
            private bool _isTemporary;
            private string _filter;
            public string Filter
            {
                get { return _filter; }
                set { _filter = value; }
            }

            public bool IsTemporary
            {
                get { return _isTemporary; }
                set { _isTemporary = value; }
            }
            public Criteria(string filter)
            {
                _filter = filter;
                _isTemporary = false;
            }
            public Criteria(string filter, bool isTemporary)
                : this(filter)
            {
                _isTemporary = isTemporary;
            }
        }

        [Serializable()]
        private class LocalOperationCriteria
        {
            private RegistryRecord _registry;
            public RegistryRecord RegistryRecord
            {
                get 
                {
                    return _registry;
                }
                set
                {
                    _registry = value;
                }
            }
            public LocalOperationCriteria(RegistryRecord registry)
            {
                _registry = registry;
            }
        }

        [Serializable()]
        private class XmlCriteria
        {
            private string _xml;
            public string Xml
            {
                get { return _xml; }
            }
            public XmlCriteria(string xml)
            {
                _xml = xml;
            }
        }

        /// <summary>
        /// added on 2008-11-25 for insert,get and update the register Log info
        /// </summary>
        [Serializable()]
        private class CriteriaLog
        {
            [NonSerialized, NotUndoable]
            private RegistrationOracleDAL _coeRegistrationDAL = null;

            private bool isUpdateDescription = false;

            [NonSerialized]
            private int _logId;
            public int LogId
            {
                get { return _logId; }
                set { _logId = value; }
            }

            private DuplicateAction _strDuplicateAction;
            public DuplicateAction StrDuplicateAction
            {
                get { return _strDuplicateAction; }
                set { _strDuplicateAction = value; }
            }

            private string _strUserId;
            public string StrUserId
            {
                get { return _strUserId; }
                set { _strUserId = value; }
            }

            private string _strLogDescription;
            public string StrLogDescription
            {
                get { return _strLogDescription; }
                set { _strLogDescription = value; }
            }

            public CriteriaLog(int logId)
            {
                _logId = logId;
            }

            public CriteriaLog(int logId, string strLogDescription)
            {
                isUpdateDescription = true;
                _logId = logId;
                _strLogDescription = strLogDescription;
                LoadDAL();
            }

            public CriteriaLog(DuplicateAction strDuplicateAction, string strUserId, string strLogDescription)
            {
                isUpdateDescription = false;
                _strDuplicateAction = strDuplicateAction;
                _strUserId = strUserId;
                _strLogDescription = strLogDescription;
                LoadDAL();
            }

            public static void SetCriteriaLog(int logId, string strLogDescription)
            {

                DataPortal.Update(new CriteriaLog(logId, strLogDescription));
            }

            public static Int32 InsertCriteriaLog(DuplicateAction strDuplicateAction, string strUserId, string strLogDescription)
            {
                CriteriaLog tmpCriteriaLog = new CriteriaLog(strDuplicateAction, strUserId, strLogDescription);
                DataPortal.Update(tmpCriteriaLog);
                return tmpCriteriaLog.LogId;
            }

            private void LoadDAL()
            {
                if (_coeRegistrationDAL == null)
                    DalUtils.GetRegistrationDAL(ref _coeRegistrationDAL, Constants.SERVICENAME);
            }

            private void DataPortal_Update()
            {
                if (isUpdateDescription)
                    _coeRegistrationDAL.UpdateLogInfo(_logId, _strLogDescription);
                else
                    _logId = _coeRegistrationDAL.InsertLogInfo(_strDuplicateAction, _strUserId, _strLogDescription);
            }
        }

        /// <summary>
        /// added on 2012-10-17 for updating the bulk register Log info
        /// </summary>
        [Serializable()]
        private class CriteriaBulkLog
        {
            [NonSerialized, NotUndoable]
            private RegistrationOracleDAL _coeRegistrationDAL = null;

            [NonSerialized]
            private int _logId;
            public int LogId
            {
                get { return _logId; }
                set { _logId = value; }
            }

            private DuplicateAction _strDuplicateAction;
            public DuplicateAction StrDuplicateAction
            {
                get { return _strDuplicateAction; }
                set { _strDuplicateAction = value; }
            }

            private string _strLogDescription;
            public string StrLogDescription
            {
                get { return _strLogDescription; }
                set { _strLogDescription = value; }
            }

            private string _strtempID;
            public string StrTempID
            {
                get { return _strtempID; }
                set { _strtempID = value; }
            }

            private string _strregNumber;
            public string StrRegNumber
            {
                get { return _strregNumber; }
                set { _strregNumber = value; }
            }

            private int _batchID;
            public int BatchID
            {
                get { return _batchID; }
                set { _batchID = value; }
            }

            public CriteriaBulkLog()
            {
            }

            public CriteriaBulkLog(int logId, string tempID, DuplicateAction strDuplicateAction, string regNumber, int batchID, string strLogDescription)
            {
                _logId = logId;
                _strDuplicateAction = strDuplicateAction;
                _strLogDescription = strLogDescription;
                _strtempID = tempID;
                _strregNumber = regNumber;
                _batchID = batchID;
                LoadDAL();
            }

            public static void InsertCriteriaBulkLog(int logId, string tempId, DuplicateAction strDuplicateAction, string regNumber, int batchId, string strLogDescription)
            {
                CriteriaBulkLog tmpCriteriaBulkLog = new CriteriaBulkLog(logId, tempId, strDuplicateAction, regNumber, batchId, strLogDescription);
                DataPortal.Update(tmpCriteriaBulkLog);
            }

            private void LoadDAL()
            {
                if (_coeRegistrationDAL == null)
                    DalUtils.GetRegistrationDAL(ref _coeRegistrationDAL, Constants.SERVICENAME);
            }

            private void DataPortal_Update()
            {
                _coeRegistrationDAL.LogBulkRegistration(_logId, _strtempID, _strDuplicateAction, _strregNumber, _batchID, _strLogDescription);
            }
        }

        /// <summary>
        /// "Fetch" criteria used when the RegistryRecordList should be populated by all RegistryRecords
        /// that share the structure id specified.
        /// </summary>
        [Serializable]
        private class CriteriaLinkedStructure
        {
            #region Properties
            [NonSerialized]
            private int _structureId;

            public int StructureId
            {
                get { return this._structureId; }
            }
            #endregion

            #region Factory Methods
            private CriteriaLinkedStructure(int structureId) 
            {
                this._structureId = structureId;
            }

            public static CriteriaLinkedStructure NewCriteriaLinkedStructure(int structureId) 
            {
                return new CriteriaLinkedStructure(structureId);
            }
            #endregion
        }

        /// <summary>
        /// Given a specific component ID, fetch all the MCRRs that share this component.
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(CriteriaSharedComponent criteria)
        {
            //get the list of RegIDs for the mixtures that contain this component
            string[] regNums = this.RegDal.GetRecordsThatContainsCompound(criteria.CompoundId);
            if (regNums.Length > 1)
            {
                string xmlFilter = this.RegDal.CreateReglistFilterFromRegNums(regNums);

                //call the standard fetch method using the regid list
                Criteria stdCriteria = new Criteria(xmlFilter, false);
                DataPortal_Fetch(stdCriteria);
            }
        }

        /// <summary>
        /// Get the Log id's list 
        /// </summary>
        private void DataPortal_Fetch(CriteriaTemp criteriaTemp)
        {
            try
            {
                IsReadOnly = RaiseListChangedEvents = false;
                string tempIds = this.RegDal.GetRegistryRecordTemporaryIdList(criteriaTemp.HitListId);
                string[] temporaryIds = !string.IsNullOrEmpty(tempIds) ? tempIds.Split(',') : new string[0];

                foreach (string temporaryId in temporaryIds)
                {
                    RegistryRecord record = null;
                    if ((record = RegistryRecord.GetRegistryRecord(int.Parse(temporaryId))) != null)
                        this.Add(record);
                }
                _registerXmlIds = tempIds;

                IsReadOnly = RaiseListChangedEvents = true;

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        
        /// <summary>
        /// Get the Log id's list 
        /// </summary>
        private void DataPortal_Fetch(HitListCriteria hitListCriteria)
        {
            try
            {
                IsReadOnly = RaiseListChangedEvents = false;
                string xml = string.Empty;
                if (hitListCriteria.IsTemporal)
                {
                    string tempIds = this.RegDal.GetRegistryRecordTemporaryIdList(hitListCriteria.HitListId);
                    xml = this.GetTemporaryRegistriesXmlList(tempIds);
                }
                else
                {
                    string tempRegNums = this.RegDal.GetRegistryRecordsFromHitList(hitListCriteria.HitListId);
                    xml = this.RegDal.GetRegistryRecordList(tempRegNums);
                }
                this.LoadFromXml(xml);
                IsReadOnly = RaiseListChangedEvents = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            string xml = string.Empty;
            if (criteria.IsTemporary)
                xml = this.GetTemporaryRegistriesXmlList(criteria.Filter);
            else
                xml = this.RegDal.GetRegistryRecordList(criteria.Filter);

            this.LoadFromXml(xml);
        }

        private string GetTemporaryRegistriesXmlList(string idsList)
        {
            StringBuilder builder = new StringBuilder(string.Empty);

            string[] temporaryIds = idsList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            builder.Append("<MultiCompoundRegistryRecordList>");
            foreach (string temporaryId in temporaryIds)
            {
                RegistryRecord record = null;
                if ((record = RegistryRecord.GetRegistryRecord(int.Parse(temporaryId))) != null)
                    builder.Append(record.Xml);
            }
            builder.Append("</MultiCompoundRegistryRecordList>");

            return builder.ToString();
        }

        /// <summary>
        /// JED: This isn't really a CSLA "fetch", and doesn't need to happen on the server.
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(XmlCriteria criteria)
        {
            RaiseListChangedEvents = false;
            //update by david zhang 2008-10-31: base class change to BusinessListBase
            //IsReadOnly = false;

            this.LoadFromXml(criteria.Xml);

            //update by david zhang 2008-10-31: base class change to BusinessListBase
            //IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        /// <summary>
        /// Given a specific structure ID, fetch all the MCRRs that share this structureId.
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(CriteriaLinkedStructure criteria) 
        {
            //get the list of RegIDs for the mixtures that contain this 
            string[] regNums = new string[] { };
            if (criteria.StructureId > -1)
                regNums = this.RegDal.GetRecordsThatContainsStructure(criteria.StructureId, true);
            if (regNums.Length > 1)
            {
                string xmlFilter = this.RegDal.CreateReglistFilterFromRegNums(regNums);

                //call the standard fetch method using the regid list
                Criteria stdCriteria = new Criteria(xmlFilter, false);
                DataPortal_Fetch(stdCriteria);
            }
        }

        /// <summary>
        /// Get the Log id info to update the log id table description
        /// </summary>
        private void DataPortal_Fetch(CriteriaLog criteriaLog)
        {
            //RaiseListChangedEvents = false;
            _dtLogInfo = this.RegDal.GetLogInfo(criteriaLog.LogId);
            //RaiseListChangedEvents = true;
        }

        #endregion

    }

    /// <summary>
    /// The class hold the Log id info
    /// </summary>
    [Serializable()]
    public class RegRecordListInfo
    {
        #region Business Methods

        private Int32 _intLogId;
        public Int32 IntLogId
        {
            get
            {
                return _intLogId;
            }
            set
            {
                _intLogId = value;
            }
        }
        #endregion
    }

    [Serializable()]
    public class GetTempRecordCount : RegistrationCommandBase
    {
        int SavedHitlistID = 0;
        public int RecordCount;

        public GetTempRecordCount(int savedHitListID)
        {
            this.SavedHitlistID = savedHitListID;
        }

        protected override void DataPortal_Execute()
        {
            this.RecordCount = this.RegDal.GetTempRecordCount(this.SavedHitlistID);
        }

    }
}
