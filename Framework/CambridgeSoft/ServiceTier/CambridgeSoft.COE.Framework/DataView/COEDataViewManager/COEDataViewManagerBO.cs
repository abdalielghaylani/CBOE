using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable()]
    public class COEDataViewManagerBO : Csla.BusinessBase<COEDataViewManagerBO>
    {
        #region Variables

        private int _basetableId = int.MinValue;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _database = String.Empty;
        private int _dataviewId = int.MinValue;
        private string _application = string.Empty;
        private string _xmlNs = String.Empty;
        private string _xml = String.Empty;
        private string _baseTableName = String.Empty;
        private int _highestID = -1;
        private const string _className = " COEDataViewManager";
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("COEDataViewService");
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDataViewManager";
        private TableListBO _tables;
        private RelationshipListBO _relationships;
        private bool _saveFromDataViewManager = false;
        private string _dataviewHandling = COEDataView.DataViewHandlingOptions.USE_SERVER_DATAVIEW.ToString();
        private bool _isDefaultFieldInTable = false;
        #endregion

        #region Enums
        /// <summary>
        /// Objects property names. Used to avoid string hardcoding.
        /// </summary>
        private enum Properties
        {
            DataViewId,
            BaseTable,
            BaseTableId,
            DataBase,
            Xml,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Id of the dataview. 0 for master dataview, -1 for new unsaved dataviews.
        /// </summary>
        [System.ComponentModel.DataObjectField(true)]
        public int DataViewId
        {
            get
            {
                return _dataviewId;
            }
            set
            {
                CanWriteProperty(Properties.DataViewId.ToString(), true);
                if (value == null)
                {
                    _dataviewId = int.MinValue;
                    PropertyHasChanged(Properties.DataViewId.ToString());
                }
                if (value != _dataviewId)
                {
                    _dataviewId = value;
                    PropertyHasChanged(Properties.DataViewId.ToString());
                }
            }
        }

        /// <summary>
        /// Name of the dataview
        /// </summary>
        internal string Name
        {
            set
            {
                CanWriteProperty("Name");
                if (value != _name)
                {
                    _name = value;
                    PropertyHasChanged("Name");
                }
            }
        }

        /// <summary>
        /// Description of the dataview
        /// </summary>
        internal string Description
        {
            set
            {
                CanWriteProperty("Description");
                if (value != _description)
                {
                    _description = value;
                    PropertyHasChanged("Description");
                }
            }
        }

        internal string Application
        {
            set
            {
                CanWriteProperty("Application");
                if (value != _application)
                {
                    _application = value;
                    PropertyHasChanged("Application");
                }
            }
        }

        /// <summary>
        /// Base table name/alias
        /// </summary>
        public string BaseTable
        {
            get
            {
                TableBO baseTable = _tables.GetTable(this.BaseTableId);
                string retVal = baseTable != null ? baseTable.Name : String.Empty;
                if (baseTable != null)
                    retVal += !string.IsNullOrEmpty(baseTable.Alias) ? " (" + baseTable.Alias + ")" : String.Empty;
                return retVal;
            }
        }

        /// <summary>
        /// Base table id
        /// </summary>
        public int BaseTableId
        {
            get
            {
                return _basetableId;
            }
            set
            {
                CanWriteProperty(Properties.BaseTableId.ToString(), true);
                if (value == null)
                {
                    _basetableId = int.MinValue;
                    PropertyHasChanged(Properties.BaseTableId.ToString());
                }
                if (value != _basetableId)
                {
                    _basetableId = value;
                    PropertyHasChanged(Properties.BaseTableId.ToString());
                }
            }
        }

        /// <summary>
        /// Database name
        /// </summary>
        public string DataBase
        {
            get
            {
                return _database;
            }
            set
            {
                CanWriteProperty(Properties.DataBase.ToString());
                if (value != _database)
                {
                    _database = value;
                    PropertyHasChanged("DataBase");
                }
            }
        }

        /// <summary>
        /// Xml representation of the object
        /// </summary>
        public string Xml
        {
            get
            {
                return _xml;
            }
        }

        /// <summary>
        /// List of tables
        /// </summary>
        public TableListBO Tables
        {
            get
            {
                return _tables;
            }
        }

        /// <summary>
        /// List of relationships
        /// </summary>
        public RelationshipListBO Relationships
        {
            get
            {
                return _relationships;
            }
        }

        /// <summary>
        /// Indicates if the object is new.
        /// </summary>
        public bool IsNew
        {
            get
            {
                return base.IsNew;
            }
        }

        /// <summary>
        /// Indicates if the object is in a valid state
        /// </summary>
        public override bool IsValid
        {
            get
            {
                this.ValidationRules.CheckRules();
                return base.IsValid && _tables.IsValid && _relationships.IsValid;
            }
        }

        /// <summary>
        /// Indicates if the object is dirty. (modified and unsaved)
        /// </summary>
        public override bool IsDirty
        {
            get { return (base.IsDirty || this.Tables.IsDirty || this.Relationships.IsDirty); }
        }
              
        /// <summary>
        /// Returns true if all tables in dataview contains at least one default/display field
        /// </summary>
        public bool IsDefaultFieldInTable
        {
            get
            {
                return _isDefaultFieldInTable;
            }
            set
            {
                _isDefaultFieldInTable = value;
            }
        }

        #endregion

        #region Overrided Methods
        /// <summary>
        /// Unique identifier of the object. This value is used by CSLA's implementation of System.Object overrides, such us Equals()
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return _dataviewId;
        }

        /// <summary>
        /// Business rules should be added here
        /// </summary>
        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }

        /// <summary>
        /// Instance business rules should be added here.
        /// </summary>
        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();
            ValidationRules.AddInstanceRule(this.EnforceRelationships, new RelationshipRuleArgs("Relationships"));
            ValidationRules.AddInstanceRule(this.ValidBaseTable, new RuleArgs("BaseTable"));
            ValidationRules.AddInstanceRule(this.ValidDefaultFieldsInTable, new RuleArgs("TablesAndFields"));
        }

        /// <summary>
        /// This method is fired after deserialization. Its main purpose is to populate [NonSerialized] fields back again.
        /// </summary>
        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            _coeLog = COELog.GetSingleton("COEDataViewService");
            base.OnDeserialized(context);
        }

        /// <summary>
        /// Enforce relationships are well formed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool EnforceRelationships(object target, RuleArgs e)
        {

            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            string errors = string.Empty;
            RelationshipRuleArgs ruleArgs = e as RelationshipRuleArgs;
            if (!this.CheckDataviewsRelationships(ref errors, ruleArgs))
            {
                e.Description = errors;
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
                return false;
            }
            else
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
                return true;
            }

        }

        /// <summary>
        /// Checks if the base table is valid one
        /// </summary>
        /// <param name="tartet"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool ValidBaseTable(object tartet, RuleArgs e)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            string errors = string.Empty;
            if (!this.IsValidBaseTable(ref errors))
            {
                e.Description = errors;
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
                return false;
            }
            else
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
                return true;
            }
        }

        /// <summary>
        /// Checks if at least one default field is present in the table
        /// </summary>
        /// <param name="tartet"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool ValidDefaultFieldsInTable(object tartet, RuleArgs e)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            string errors = string.Empty;
            if (!this.CheckDefaultFields(ref errors))
            {
                e.Description = errors;
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
                IsDefaultFieldInTable = false;
                return false;
            }
            else
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
                IsDefaultFieldInTable = true;
                return true;
            }
        }

        #endregion

        #region Constructors
        internal COEDataViewManagerBO(COEDataView dataView)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            _name = dataView.Name;
            _description = dataView.Description;
            _basetableId = dataView.Basetable;
            _database = dataView.Database;
            _dataviewId = dataView.DataViewID;
            _application = dataView.Application;
            _xmlNs = dataView.XmlNs;
            //Create List of tables
            _tables = TableListBO.NewTableListBO(dataView.Tables);
            this.SetHighestID();
            //Create list of Relationships.
            _relationships = RelationshipListBO.NewRelationShipListBO(dataView.Relationships);
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        internal COEDataViewManagerBO()
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            _tables = TableListBO.NewTableListBO();
            this.SetHighestID();
            _relationships = RelationshipListBO.NewRelationShipListBO();
            this.MarkClean();
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        internal COEDataViewManagerBO(COEDataView dataView, int dataviewId)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            _name = dataView.Name;
            _description = dataView.Description;
            _basetableId = dataView.Basetable;
            _database = dataView.Database;
            _application = dataView.Application;
            _dataviewId = dataviewId;
            _xmlNs = dataView.XmlNs;
            //Create List of tables
            _tables = TableListBO.NewTableListBO(dataView.Tables);
            this.SetHighestID();
            //Create list of Relationships.
            _relationships = RelationshipListBO.NewRelationShipListBO(dataView.Relationships);
            _dataviewHandling = dataView.DataViewHandling.ToString();
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        internal COEDataViewManagerBO(COEDataView dataView, int dataviewId, bool isClean)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            _name = dataView.Name;
            _description = dataView.Description;
            _basetableId = dataView.Basetable;
            _database = dataView.Database;
            _application = dataView.Application;
            _dataviewId = dataviewId;
            _xmlNs = dataView.XmlNs;
            //Create List of tables
            _tables = TableListBO.NewTableListBO(dataView.Tables, isClean);
            this.SetHighestID();
            //Create list of Relationships.
            _relationships = RelationshipListBO.NewRelationShipListBO(dataView.Relationships, isClean);
            if (isClean)
                this.MarkClean();
            else
                this.MarkDirty();
            _dataviewHandling = dataView.DataViewHandling.ToString();
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        private void SetHighestID()
        {
            if (_tables.HighestID > _highestID)
            {
                _highestID = _tables.HighestID;
            }
        }

        #endregion

        #region Validaton Rules

        private void AddCommonRules()
        {
            //Examples from other class
            //ValidationRules.AddRule(CommonRules.StringRequired, "BaseTable");
            //ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            //ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 50));
            //ValidationRules.AddRule(CommonRules.MinValue<SmartDate>, new CommonRules.MinValueRuleArgs<SmartDate>("DateCreated", new SmartDate("1/1/2005")));
            //ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
        }

        //public List<BrokenRuleDescription> GetBrokenRulesDescription()
        //{
        //    List<BrokenRuleDescription> brokenRules = new List<BrokenRuleDescription>();

        //    if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
        //    {
        //        brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
        //    }
        //    return brokenRules;
        //}

        #endregion

        #region Factory Methods

        public static COEDataViewManagerBO NewManager(COEDataView dataView)
        {
            try
            {
                return new COEDataViewManagerBO(dataView);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static COEDataViewManagerBO NewManager()
        {
            try
            {
                return new COEDataViewManagerBO();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static COEDataViewManagerBO NewManager(COEDataView dataView, int dataviewId)
        {
            try
            {
                return new COEDataViewManagerBO(dataView, dataviewId);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static COEDataViewManagerBO Get(int dataviewId)
        {
            try
            {
                if (!CanGetObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + _className);
                return DataPortal.Fetch<COEDataViewManagerBO>(new Criteria(dataviewId));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static void Delete(int dataviewId)
        {
            try
            {
                if (!CanDeleteObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + _className);
                DataPortal.Delete(new Criteria(dataviewId));
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public override COEDataViewManagerBO Save()
        {
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
                if (IsDeleted && !CanDeleteObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + _className);
                else if (IsNew && !CanAddObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + _className);
                else if (!CanEditObject())
                    throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + _className);
                COEDataViewManagerBO result = base.Save();
                return result;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);// LogEnd is return in finally block, in order to reduce redundency of code.
            }
            return null;
        }

        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanDeleteObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        #endregion

        #region Data Access
        protected override void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
        {
            COEExceptionDispatcher.HandleBLLException(ex);
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            if (_coeDAL == null)
                LoadDAL();
            if (criteria._dataview != null)
                this.Initialize(criteria._dataview, true, true);
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Criterias

        [Serializable()]
        private class Criteria
        {
            internal int _dataviewId;
            internal COEDataView _dataview;

            public Criteria(int dataviewId)
            {
                _dataviewId = dataviewId;
            }

            public Criteria(COEDataView dataview)
            {
                _dataview = dataview;
                _dataviewId = dataview.DataViewID;
            }

            public Criteria(COEDataView dataview, int dataviewId)
            {
                _dataview = dataview;
                _dataviewId = dataviewId;
            }
        }
        #endregion

        #region DALLoader

        private void LoadDAL()
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks that the base tables is valid (checking also if it's a master dataview)
        /// </summary>
        /// <returns>Boolean indicating if the current dataview is valid or not</returns>
        public bool IsValidBaseTable(ref string errors)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            bool retVal = _dataviewId == 0 ? true : false;
            try
            {
                if (_basetableId >= 0 && _dataviewId != 0) // Master dataview (id=0) has no basetable.
                {
                    TableBO baseTable = _tables.GetTable(_basetableId);
                    if (baseTable == null)
                    {
                        retVal = false;
                        errors += "Selected base table is not present in dataview";
                    }
                    else
                    {
                        FieldBO pkField = baseTable.Fields.GetField(baseTable.PrimaryKey);
                        if (pkField == null)
                        {
                            retVal = false;
                            errors += "Base table has no primary key selected";
                        }
                        else
                            retVal = true;
                    }
                }
                else
                {
                    errors += "There's no base table selected";
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
            return retVal;// I have removed this in order to maintain style and to avoid redundency of code.
        }

        /// <summary>
        /// Method to check that all the relationships are valid.
        /// If a table is not base table, it has to have at least one relationship to some of the fields of the baseTable.
        /// </summary>
        /// <returns>Boolean indicating if the list of relationships is valid or not</returns>
        /// <exception cref="ValidationException">Catch it at show it in the GUI.</exception>
        public bool CheckDataviewsRelationships(ref string errors, RelationshipRuleArgs ruleArgs)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            bool relationshipsOK = true;
            StringBuilder sb = new StringBuilder();
            List<int> cascadeTableIDList = new List<int>();
            List<int> recursiveTableIDList = new List<int>();

            AnalyzeCascadeAndRecursiveLookup(ref cascadeTableIDList,ref recursiveTableIDList);

            try
            {
                string instanceName = ConfigurationUtilities.GetInstanceNameByDatabaseName(_tables[0].DataBase);
                bool IsSingleDataSource = _tables.All(table => ConfigurationUtilities.GetInstanceNameByDatabaseName(table.DataBase) == instanceName);

                if (this.DataViewId != 0) //Master Dataview doesn't have basetable, so we can't check relationships.
                {
                    foreach (TableBO table in _tables)
                    {
                        if (!table.FromMasterSchema)
                        {
                            sb.Clear();
                            //if we have cascade lookup in dataview, will pop up an error
                            if (cascadeTableIDList.Contains(table.ID))
                            {
                                sb.Append(Resources.ErrorRelationship_6);
                            }

                            if (recursiveTableIDList.Contains(table.ID))
                            {
                                sb.Append(Resources.ErrorRelationship_7);
                            }

                            if (table.ID != _basetableId) //if it's not base table, there must be a relationship for it.
                            {
                                //Find relationships with the current tableID
                                List<RelationshipBO> foundRelations = _relationships.GetByParentOrChildId(table.ID);
                                if (foundRelations.Count == 0 && !HasTableInvolvedInLookup(table)) //If it is an orphan table.                                
                                    sb.Append(Resources.ErrorRelationship_1);

                                //MN 27-AUG-2013 :: CBOE-1735 :For CBV client needs the primary key selected for table.
                                //Previously it was not throwing error as relationship did not have primary key and forms were not generated in CBV.
                                if (table.PrimaryKey <= 0)
                                {
                                    if (sb.ToString().Length > 0)
                                        sb.Append(Resources.ErrorRelationship_3);
                                    else
                                        sb.Append(Resources.ErrorRelationship_2);
                                }
                                
                            }
                            
                            //end :CBOE-1735 
                            if (sb.ToString().Length > 0)
                            {
                                relationshipsOK = false;
                                errors += sb.ToString() + " " + Resources.ErrorRelationship_4 + table.OrderIndex + " (" + table.Alias + ").\n";
                                ruleArgs.TableID = table.ID;
                            }
                        }
                    }
                }
                return relationshipsOK;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
            return relationshipsOK;
        }

        /// <summary>
        /// Checks that at least one default field is present in the table
        /// </summary>
        /// <returns>Boolean indicating if the current dataview is valid or not</returns>
        public bool CheckDefaultFields(ref string errors)
        {
            bool isDefaultPresent = true;
            try
            {             
                if (this.DataViewId != 0) //Master Dataview will not require default fields
                {
                    foreach (TableBO table in _tables)
                    {
                        bool isDefault = false;
                        foreach (FieldBO field in table.Fields)
                        {
                            if (!isDefault && field.IsDefault)
                            {
                                isDefault = true;
                                break;
                            }
                        }
                        if (!isDefault)
                        {
                            isDefaultPresent = false;
                            errors += "Please select at least one display field for tableId " + table.OrderIndex + " (" + table.Alias + ").\n";
                        }
                    }                    
                }            
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return isDefaultPresent;
        }

        private List<int> HasRecursiveRelationLookup()
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            List<int> tableIDList = new List<int>();

            foreach (TableBO table in _tables)
            {
                var lookupFields = from x in table.Fields
                                   where x.LookupFieldId > 0
                                   select x;

                foreach (FieldBO field in lookupFields)
                {
                   
                    
                }
            }
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            return tableIDList;
        }

        private void AnalyzeCascadeAndRecursiveLookup(ref List<int> cascadeTableIDList, ref List<int> recursiveTableIDList)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            
            foreach (TableBO table in _tables)
            {
                var lookupFields = from x in table.Fields
                                   where x.LookupFieldId > 0
                                   select x;

                foreach (FieldBO field in lookupFields)
                {
                    // if Relationship columns and lookup columns have same value, add them into list
                    foreach (RelationshipBO relationShip in _relationships)
                    {
                        if ((relationShip.ChildKey == field.LookupFieldId) && (relationShip.ParentKey == field.ID))
                        {
                            recursiveTableIDList.Add(table.ID);
                        }
                    }

                    // if find cascade lookup here, add them into list.
                    foreach (TableBO lookupTable in _tables)
                    {
                        var lookupField = lookupTable.Fields.Where(z => z.ID == field.LookupDisplayFieldId).FirstOrDefault();
                        if (lookupField != null && lookupField.LookupFieldId > 0)
                        {
                            cascadeTableIDList.Add(table.ID);
                        }
                    }
                }
            }
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        private bool IsLookupTable(TableBO table)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            foreach (FieldBO field in table.Fields)
            {
                if (field.LookupDisplayFieldId >= 0)
                    return true;
            }
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            return false;
        }

        private bool HasTableInvolvedInLookup(TableBO table)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            foreach (FieldBO field in table.Fields)
            {
                if (field.LookupDisplayFieldId >= 0)
                    return true;

                List<TableBO> tables = this.Tables.GetTablesByLookup(field.ID);
                if (tables != null && tables.Count > 0)
                    return true;
            }
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            return false;
        }

        private void Initialize(COEDataView dataview, bool isNew, bool isClean)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            _basetableId = dataview.Basetable;
            _dataviewId = dataview.DataViewID;
            _xmlNs = dataview.XmlNs;
            _database = dataview.Database;
            //if (dataview.Tables != null)
            //    COEDataViewTableList(dataview.Tables)
            if (!isNew)
                MarkOld();
            if (isClean)
                MarkClean();
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Sets the base table.
        /// </summary>
        /// <param name="tableId"></param>
        public void SetBaseTable(int tableId)
        {
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
                TableBO table = null;
                //This is done to check that the table really exists. 
                table = this._tables.GetTable(tableId);
                if (table != null)
                {
                    if (_basetableId != table.ID)
                    {
                        _basetableId = table.ID;
                        _database = table.DataBase;
                    }
                    table.FromMasterSchema = false;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Merges the dataview with the given tables and relationships
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="relationships"></param>
        public void Merge(TableListBO tables, RelationshipListBO relationships)
        {
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
                //Merge Tables
                foreach (TableBO table in tables)
                {
                    TableBO currentTable = _tables.GetTable(table.Name, table.Alias, table.DataBase);
                    if (currentTable == null)
                    {
                        TableBO tbl = _tables.GetTable(table.ID);
                        if (tbl != null && tbl.DataBase == table.DataBase && tbl.Name == table.Name)
                        {
                            break;
                        }
                        TableBO modifiedTable = this.RegenerateIds(table, relationships);
                        //TableBO modifiedTable = table;
                        modifiedTable.FromMasterSchema = false;
                        _tables.Add(modifiedTable);
                    }
                    else
                    {
                        //The table already exists but we have different ids, so we need to change those.
                        int oldTableID = table.ID;
                        int newTableID = currentTable.ID;
                        currentTable.HitListDataType = table.HitListDataType;

                        foreach (FieldBO field in table.Fields)
                        {
                            if (!currentTable.Fields.ExistsFieldKey(field.Database, field.Name)) //Don't need of alias cause we're comparing against master schema.
                            {
                                this.RegenerateIds(field);
                                field.FromMasterSchema = true;
                                field.Visible = false;
                                currentTable.Fields.AddField(field);
                            }
                            else
                            {
                                //The field already exists but we have different ids, so we need to change those.
                                FieldBO currentField = currentTable.Fields.GetField(field.Database, field.Name, field.Alias);
                                if (currentField == null)
                                    currentField = currentTable.Fields.GetField(field.Database, field.Name);

                                int oldFieldID = field.ID;
                                int newFieldID = currentField.ID;
                                foreach (RelationshipBO relationship in relationships.GetByParentKeyOrChildKeyId(oldFieldID))
                                {
                                    if (relationship.ParentKey == oldFieldID)
                                    {
                                        relationship.Parent = newTableID;
                                        relationship.ParentKey = newFieldID;
                                    }
                                    if (relationship.ChildKey == oldFieldID)
                                    {
                                        relationship.Child = newTableID;
                                        relationship.ChildKey = newFieldID;
                                    }
                                    relationship.FromMasterSchema = _relationships.GetByParentKeyOrChildKeyId(oldFieldID).Count == 0;
                                }
                            }
                        }
                    }
                }
                //Merge Relationships. The ids have been changed in the previous loop.
                foreach (RelationshipBO relationship in relationships)
                {
                    RelationshipBO foundRelation = null;
                    foundRelation = _relationships.Get(relationship.ParentKey, relationship.ChildKey);
                    if (foundRelation == null)
                    {
                        _relationships.Add(relationship);
                    }
                }

                int i = 0;

                List<int> tableIdsRemoved = new List<int>();
                while (i < this.Tables.Count)
                {
                    TableBO table = this.Tables[i];
                    if (table.DataBase == tables[0].DataBase)
                    {
                        if (!tables.Exists(table.Name, table.DataBase))
                        {
                            tableIdsRemoved.Add(table.ID);
                            this.Tables.RemoveAt(i);
                        }
                        else
                        {
                            TableBO tbl = tables.GetTable(table.Name, table.Alias, table.DataBase);
                            if (tbl != null)
                            {
                                int j = 0;
                                while (j < table.Fields.Count)
                                {
                                    FieldBO currentField = table.Fields[j];
                                    if (!tbl.Fields.ExistsFieldKey(currentField.Database, currentField.Name))
                                    {
                                        List<RelationshipBO> rels = this.Relationships.GetByParentKeyOrChildKeyId(currentField.ID);
                                        int k = rels.Count - 1;
                                        while (k >= 0)
                                        {
                                            RelationshipBO currentRel = rels[k];
                                            this.Relationships.Remove(currentRel.ParentKey, currentRel.ChildKey);
                                            k--;
                                        }
                                        table.Fields.RemoveAt(j);
                                    }
                                    else
                                    {
                                        j++;
                                    }
                                }
                            }
                            i++;
                        }
                    }
                    else
                        i++;
                }
                this.Relationships.Remove(tableIdsRemoved);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Merge master dataview table with given table. 
        /// </summary>
        /// <param name="table"></param>
        public void MergeTable(TableListBO tables, TableBO table, RelationshipListBO relationships)
        {
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);

                TableBO currentTable = _tables.GetTable(table.Name, table.Alias, table.DataBase);
                TableBO currentTableFromMaster = tables.GetTable(table.Name, table.Alias, table.DataBase);

                //get the index field information from database to update the IsIndexed property of the fields those are not included in currently selected view - PP on 06Feb2013
                DataTable indexInformationDataTable = GetFieldIndexes(DataBase, table.Name);

                //Add Duplicate Table to master schema
                if (currentTableFromMaster == null)
                {
                    TableBO tbl = tables.GetTable(table.Name, table.Alias, table.DataBase);
                    if (tbl != null && tbl.DataBase == table.DataBase && tbl.Name == table.Name)
                    {
                        foreach (FieldBO field in tbl.Fields)
                        {
                            if (currentTable != null && !currentTable.Fields.ExistsFieldKey(field.Database, field.Name))
                            {
                                field.FromMasterSchema = true;
                                field.Visible = false;
                                //check if field has indexing applied then set IsIndexed property to true before adding to collection - PP on 06Feb013
                                DataRow[] indexDataRows = indexInformationDataTable.Select("column_name = '" + field.Name + "'");
                                if (indexDataRows != null && indexDataRows.Length > 0)
                                {
                                    field.IsIndexed = true;
                                    field.IndexName = indexDataRows[0]["indexname"].ToString();  //CBOE-529 Set indexname proerty  ASV 27032013
                                }
                                else
                                {
                                    field.IsIndexed = false;
                                }
                                currentTable.Fields.AddField(field);
                            }
                        }
                    }
                    else
                    {
                        if (tbl != null)
                        {

                            TableBO modifiedTable = this.RegenerateIds(table, relationships);
                            modifiedTable.FromMasterSchema = true;
                            _tables.Add(modifiedTable);
                        }
                    }
                }
                else
                {
                    if (currentTable == null)
                        currentTable = _tables.GetTable(table.ID);
                    foreach (FieldBO field in currentTableFromMaster.Fields)
                    {
                        if (!currentTable.Fields.ExistsFieldKey(field.Database, field.Name)) //Don't need the alias because we are comparing against the master dataview.
                        {
                            field.FromMasterSchema = true;
                            field.Visible = false;
                            //check if field has indexing applied then set IsIndexed property to true before adding to collection - PP on 06Feb013
                            DataRow[] indexDataRows = indexInformationDataTable.Select("column_name = '" + field.Name + "'");
                            if (indexDataRows != null && indexDataRows.Length > 0)
                            {
                                field.IsIndexed = true;
                                field.IndexName = indexDataRows[0]["indexname"].ToString();   //CBOE-529 Set indexname proerty  ASV 27032013
                            }
                            else
                            {
                                field.IsIndexed = false;
                            }
                            currentTable.Fields.AddField(field);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Obsoleted method
        /// This method is specially for refreshing tables in ENBIOASSAYVIEWS schema
        /// </summary>
        /// <param name="strDatabase"></param>
        void RefreshSchema(string strDatabase)
        {           
            COEDatabaseBO bo = COEDatabaseBO.Get(strDatabase).RefreshPublish();
            COEDataViewManagerBO man = COEDataViewManagerBO.NewManager(bo.COEDataView);
            COEDataViewBO masterdataViewBO = COEDataViewBO.GetMasterSchema();           
            // Merge BioAssay tables into master dataview
            masterdataViewBO.DataViewManager.Merge(man.Tables, man.Relationships);            
            masterdataViewBO.SaveFromDataViewManager();           
            
        }

        /// <summary>
        /// It will check whether schema is present in Master or not.
        /// </summary>
        /// <param name="strDatabase"></param>
        /// <returns></returns>
        public bool IsDatabasePresentInMater(string strDatabase)
        {
            try
            {
                COEDataViewBO masterDataViewBO = COEDataViewBO.GetMasterSchema();
                foreach (TableBO table in masterDataViewBO.DataViewManager.Tables)
                {
                    if (table.DataBase == strDatabase)
                        return true;
                }
            }
            catch
            {
                throw;
            }
            return false;
        }

        private TableListBO MergeWithMasterTables(TableListBO masterDVTables)
        {
            TableListBO tempTables = null;
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
                tempTables = TableListBO.NewTableListBO(masterDVTables);
                foreach (TableBO table in _tables)
                {
                    if (!tempTables.Exists(table))
                        tempTables.Add(table);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
            return tempTables;
        }

        /// <summary>
        /// Gets the status of the master dataview tables
        /// </summary>
        /// <param name="dataViewTablesBO"></param>
        /// <param name="masterDVTables"></param>
        /// <returns></returns>
        public List<TableStatus> GetMasterDataViewTablesStatus(TableListBO dataViewTablesBO, TableListBO masterDVTables)
        {
            List<TableStatus> tablesStatusList = new List<TableStatus>();
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
                //Just supported master data view for this method (for now).
                if (_dataviewId == 0)
                {
                    TableListBO mergedTables = this.MergeWithMasterTables(masterDVTables);
                    foreach (TableBO table in mergedTables)
                    {
                        TableStatus tableStatus = new TableStatus(table);
                        tableStatus.Status = dataViewTablesBO.Exists(table) ? TableStatus.TableStatusOpt.Added : TableStatus.TableStatusOpt.Removed;
                        tablesStatusList.Add(tableStatus);
                    }
                }
                return tablesStatusList;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            }
            return tablesStatusList;
        }

        private void RegenerateIds(FieldBO field)
        {
            _coeLog.LogStart("RegenerateIds(FieldBO field)");
            if (field.ID < _highestID)
                field.ID = ++_highestID;
            else
                _highestID = field.ID;
            _coeLog.LogEnd("RegenerateIds(FieldBO field)");
        }

        private TableBO RegenerateIds(TableBO table, RelationshipListBO relationships)
        {
            try
            {
                _coeLog.LogStart("RegenerateIds(TableBO table, RelationshipListBO relationships)");
                int oldTableID = table.ID;
                int newTableID = oldTableID;
                //Tables Stuff
                if (table.ID < _highestID)
                {
                    newTableID = ++_highestID;
                    table.ID = newTableID;
                }
                else
                    _highestID = newTableID;


                foreach (FieldBO field in table.Fields)
                {
                    int oldFieldID = field.ID;
                    if (field.ID < _highestID)
                    {
                        int newFieldID = ++_highestID;
                        List<RelationshipBO> foundRelations = relationships.GetByParentKeyOrChildKeyId(oldFieldID);
                        foreach (RelationshipBO relationship in foundRelations)
                        {
                            if (relationship.ChildKey == oldFieldID)
                            {
                                relationship.Child = newTableID;
                                relationship.ChildKey = newFieldID;
                            }
                            if (relationship.ParentKey == oldFieldID)
                            {
                                relationship.Parent = newTableID;
                                relationship.ParentKey = newFieldID;
                            }
                        }

                        if (table.PrimaryKey == oldFieldID)
                            table.PrimaryKey = newFieldID;
                        field.ID = newFieldID;
                        //CSBR-137641
                        //set field visible property as true while selecting the tables for dataview
                        field.Visible = true;
                    }
                    else
                        _highestID = oldFieldID;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            finally
            {
                _coeLog.LogEnd("RegenerateIds(TableBO table, RelationshipListBO relationships)");
            }
            return table;
        }

        /// <summary>
        /// Builds a string representation of the object in an xml format
        /// </summary>
        /// <param name="noMasterSchemaFields"></param>
        /// <returns></returns>
        internal string ToString(bool noMasterSchemaFields)
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);

            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<COEDataView  xmlns=\"" + _xmlNs + "\" basetable=\"" + _basetableId + "\" database=\"" + _database + "\" name=\"" + _name + "\" description=\"" + _description + "\" application=\"" + _application + "\" dataviewHandling=\"" + _dataviewHandling + "\" dataviewid=\"" + _dataviewId.ToString() + "\">");

            builder.Append("<tables>");
            for (int i = 0; i < _tables.Count; i++)
            {
                if (noMasterSchemaFields)
                {
                    if (!_tables[i].FromMasterSchema)
                        builder.Append(_tables[i].ToString(noMasterSchemaFields));
                }
                else
                {
                    builder.Append(_tables[i].ToString(noMasterSchemaFields));
                }
            }
            builder.Append("</tables>");
            builder.Append("<relationships>");
            for (int i = 0; i < _relationships.Count; i++)
            {
                if (noMasterSchemaFields)
                {
                    if (!_relationships[i].FromMasterSchema)
                    {
                        //Before getting the relationship, you have to get the parent and child values. 
                        //This is until we fully support relationships with just childKey and parentKey
                        if (_relationships[i].Child <= 0)
                            _relationships[i].Child = _tables.GetTableIdByFieldId(_relationships[i].ChildKey);
                        if (_relationships[i].Parent <= 0)
                            _relationships[i].Parent = _tables.GetTableIdByFieldId(_relationships[i].ParentKey);
                        //End Temp code.
                        builder.Append(_relationships[i].ToString());
                    }
                }
                else
                {
                    //Before getting the relationship, you have to get the parent and child values. 
                    //This is until we fully support relationships with just childKey and parentKey
                    if (_relationships[i].Child <= 0)
                        _relationships[i].Child = _tables.GetTableIdByFieldId(_relationships[i].ChildKey);
                    if (_relationships[i].Parent <= 0)
                        _relationships[i].Parent = _tables.GetTableIdByFieldId(_relationships[i].ParentKey);
                    //End Temp code.
                    builder.Append(_relationships[i].ToString());
                }
            }
            builder.Append("</relationships>");
            builder.Append("</COEDataView>");
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            return builder.ToString();
        }

        /// <summary>
        /// Bulds a string representation of the object in an xml format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name);
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<COEDataView  xmlns=\"" + _xmlNs + "\" basetable=\"" + _basetableId + "\" database=\"" + _database + "\" name=\"" + _name + "\" description=\"" + _description + "\" application=\"" + _application + "\" dataviewid=\"" + _dataviewId.ToString() + "\">");

            builder.Append("<tables>");
            for (int i = 0; i < _tables.Count; i++)
                builder.Append(_tables[i].ToString());
            builder.Append("</tables>");
            builder.Append("<relationships>");
            for (int i = 0; i < _relationships.Count; i++)
                builder.Append(_relationships[i].ToString());
            builder.Append("</relationships>");
            builder.Append("</COEDataView>");
            _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name);
            return builder.ToString();
        }

        #endregion

        /// <summary>
        /// Removes orphan relationships by marking them as being fromMaster (inherited and not choosen by the user)
        /// </summary>
        public void RemoveOrphanRelationships()
        {
            try
            {
                int i = this.Relationships.Count - 1;
                while (i >= 0)
                {
                    RelationshipBO relationship = this.Relationships[i];
                    TableBO childTbl = this.Tables.GetTable(relationship.Child);
                    TableBO parentTbl = this.Tables.GetTable(relationship.Parent);
                    if (childTbl == null || childTbl.FromMasterSchema || parentTbl == null || parentTbl.FromMasterSchema)
                    {
                        this.Relationships.Remove(relationship.Parent, relationship.ParentKey, relationship.Child, relationship.ChildKey);
                    }
                    i--;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Retrieves the index information from the selected database and table.
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Returns datatable containing information about the index fields and columns on the selected database.</returns>
        public DataTable GetFieldIndexes(string database, string tableName)
        {
            return CambridgeSoft.COE.Framework.COEDatabasePublishingService.COEDatabaseBO.GetFieldIndexes(database, tableName);
        }

        /// <summary>
        /// Returns the datatable with primary & unique key contraint column information
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public DataTable GetPrimaryUniqueKeyConstraintInfo(string database, string tableName)
        {
            //Bug Fixing : CBOE-242
            DataTable returnDataTable = null;
            try
            {
                COEDataViewBO theCOEDataViewBO = COEDataViewBO.New();
                if (theCOEDataViewBO != null)
                {
                    returnDataTable= theCOEDataViewBO.GetUniqueFields(database, tableName);
                }
                    
                theCOEDataViewBO = null;
            }
            catch
            {
                return null;
            }
            return returnDataTable;
        }

        /// <summary>
        /// Returns the datatable which contains the NOTNULL string/number columns
        /// </summary>
        /// <param name="database">Name of the database, used as database owner.</param>
        /// <param name="table">Name of the table to find the index information</param>
        /// <returns>Datatable</returns>
        public DataTable GetPrimaryKeyFieldNotNullCols(string database, string tableName)
        {
            //Bug Fixing : CBOE-242
            DataTable returnDataTable = null;
            try
            {
                COEDataViewBO theCOEDataViewBO = COEDataViewBO.New();
                if (theCOEDataViewBO != null)
                    returnDataTable = theCOEDataViewBO.GetPrimaryKeyFieldNotNullCols(database, tableName);
                theCOEDataViewBO = null;
            }
            catch
            {
                return null;
            }
            return returnDataTable;
        }
    }

    public class TableStatus
    {
        public enum TableStatusOpt
        {
            Added,
            Removed,
        }

        private TableBO _table;
        private TableStatusOpt _status = TableStatusOpt.Added;

        public TableBO Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public TableStatusOpt Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public TableStatus(TableBO table)
        {
            _table = table;
        }
    }

    public class RelationshipRuleArgs : RuleArgs
    {
        public int TableID;

        public RelationshipRuleArgs(string propertyName)
            : base(propertyName)
        {
            this.TableID = -1;
        }

        public RelationshipRuleArgs(string propertyName, int tableID)
            : this(propertyName)
        {
            this.TableID = tableID;
        }
    }
}
