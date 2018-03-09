using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Properties;
using Csla;
using Csla.Validation;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    /// <summary>
    /// Business List for TableBO's
    /// </summary>
    [Serializable()]
    public class TableListBO : BusinessListBase<TableListBO, TableBO>
    {
        #region Variables
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("COEDataViewTableListBO");
        [NonSerialized]
        private SortedBindingList<TableBO> _sortedList;
        private int _highestID = -1;
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDataViewTableListBO";
        [NonSerialized]
        List<CustomBrokenRule> _customBrokenRules;
        private NameValueCollection _fullTableKeys;
        private NameValueCollection _simpleTableKeys;
        private Dictionary<int, TableBO> _idToTable;
        private readonly string _tableSeparator = "|";

        #endregion

        #region Events

        //public event EventHandler<CustomEventArgs> IDChanged = null;

        //protected void IDHasChanged(object sender, CustomEventArgs e)
        //{
        //    if (IDChanged != null)
        //    {
        //        EventHandler<CustomEventArgs> currentEventRaised = IDChanged;
        //        currentEventRaised(sender, e);
        //    }
        //}

        #endregion

        #region Overrided Properties

        /// <summary>
        /// This list contains the error ocurred in object validations
        /// </summary>
        /// <remarks>We cannot create BrokenRules because that class has no Constructor/Public method. It works with ValidationRules</remarks>
        public List<CustomBrokenRule> CustomBrokenRules
        {
            get
            {
                _customBrokenRules = new List<CustomBrokenRule>();
                foreach (TableBO table in this)
                    _customBrokenRules.AddRange(table.Fields.CustomBrokenRules);
                return _customBrokenRules;
            }
        }

        /// <summary>
        /// Indicates if the object is valid
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.CheckTables();
            }
        }

        /// <summary>
        /// The highest id used
        /// </summary>
        public int HighestID
        {
            get
            {
                return _highestID;
            }
            set
            {
                if (value > _highestID)
                    _highestID = value;
                foreach (TableBO table in this)
                    table.HighestID = value;
            }
        }

        /// <summary>
        /// A list of broken rules
        /// </summary>
        public List<BrokenRule> BrokenRules
        {
            get
            {
                List<BrokenRule> brokenRules = new List<BrokenRule>();
                foreach (TableBO table in this)
                {
                    foreach (BrokenRule rule in table.BrokenRulesCollection)
                        brokenRules.Add(rule);
                }
                return brokenRules;
            }
        }

        /// <summary>
        /// Indicates if the object was changed
        /// </summary>
        public bool IsDirty
        {
            get { if (DeletedList.Count > 0) return true; else return base.IsDirty; }
        }
        #endregion

        #region Factory Methods

        public static TableListBO NewTableListBO(List<COEDataView.DataViewTable> tables)
        {
            try
            {
                return new TableListBO(tables);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static TableListBO NewTableListBO(List<COEDataView.DataViewTable> tables, bool isClean)
        {
            try
            {
                return new TableListBO(tables, isClean);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static TableListBO NewTableListBO()
        {
            try
            {
                return new TableListBO();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static TableListBO NewTableListBO(TableListBO tables)
        {
            try
            {
                return new TableListBO(tables);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        #endregion

        #region DALLoader

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Unfortunately, we cannot handle BrokenRulesCollection for a BusinessListBase.
        /// That's why I throw a ValidationException.
        /// </summary>
        /// <returns>Boolean indicating if the TableList is valid</returns>
        /// <see cref="http://forums.lhotka.net/forums/thread/14653.aspx"/>
        private bool CheckTables()
        {
            bool valid = true;
            try
            {
                //Check at least there is one table
                if (this.Count == 0)
                    throw new ValidationException("TableListBO: No tables present in this DataView");
                //Check that each PK exists in each table.
                //this.CheckPKs();
                //Check unique valid ids for tables, check that lookups tables are present in dataview.
                this.ValidateTablesAndField();
                //Check each table is unique about name, alias and database.
                this.CheckPropertiesAreUnique();
                return valid;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return valid;
        }

        /// <summary>
        /// Method to check if the set of Name, Alias and DB properties are unique for the rest of Tables.
        /// </summary>
        /// <remarks>TODO: Change harcoded messages for Resources entries</remarks>
        private void CheckPropertiesAreUnique()
        {
            try
            {
                foreach (TableBO tableA in this)
                {
                    string[] foundKeys = _fullTableKeys.GetValues(this.BuildFullTblKey(tableA.Name, tableA.Alias, tableA.DataBase));
                    if (foundKeys != null && foundKeys.Length > 1)
                        throw new ValidationException("TableListBO: The tableId " + tableA.ID + " is not unique in the DataView (Check other tables Name, Alias and Database)");
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        internal bool Exists(TableBO tableA)
        {
            return ExistsKey(tableA.Name, tableA.DataBase, tableA.Alias);
        }

        internal bool Exists(string name, string database)
        {
            return ExistsKey(name, database);
        }

        private bool ExistsKey(string name, string database)
        {
            bool retVal = false;
            string[] foundKeys = _simpleTableKeys.GetValues(this.BuildSimpleTblKey(name, database));
            if (foundKeys != null)
                retVal = foundKeys.Length > 0 ? true : false;
            return retVal;
        }

        private bool ExistsKey(string name, string database, string alias)
        {
            bool retVal = false;
            string[] foundKeys = _fullTableKeys.GetValues(this.BuildFullTblKey(name, alias, database));
            if (foundKeys != null)
                retVal = foundKeys.Length > 0 ? true : false;
            return retVal;
        }

        private void AddTableToLists(TableBO table)
        {
            //if (!ExistsKey(table.Name, table.DataBase))
            //    _simpleTableKeys.Add(this.BuildSimpleTblKey(table.Name, table.DataBase), table.ID.ToString());

            var simpleKey = this.BuildSimpleTblKey(table.Name, table.DataBase);

            if (!ExistsKey(table.Name, table.DataBase))
            {
                _simpleTableKeys.Add(simpleKey, table.ID.ToString());
            }
            else
            {
                var oldValue = _simpleTableKeys[simpleKey];
                _simpleTableKeys[simpleKey] = oldValue + "," + table.ID.ToString();
            }
            if (!ExistsKey(table.Name, table.DataBase, table.Alias))
                _fullTableKeys.Add(this.BuildFullTblKey(table.Name, table.Alias, table.DataBase), table.ID.ToString());
            if (!_idToTable.ContainsKey(table.ID))
                _idToTable.Add(table.ID, table);
        }

        private void InitializeArrays()
        {
            if (_simpleTableKeys == null)
                _simpleTableKeys = new NameValueCollection();
            if (_fullTableKeys == null)
                _fullTableKeys = new NameValueCollection();
            if (_idToTable == null)
                _idToTable = new Dictionary<int, TableBO>();
            if (_sortedList == null)
                _sortedList = new SortedBindingList<TableBO>(this);
        }

        private void RemoveTableFromLists(TableBO table)
        {
            try
            {
                _idToTable.Remove(table.ID);

                string simplekey = this.BuildSimpleTblKey(table.Name, table.DataBase);

                //if (_simpleTableKeys.GetValues(simplekey).Length < 2)
                //    _simpleTableKeys.Remove(simplekey);
                //else
                //{
                //    string[] oldValues = _simpleTableKeys.GetValues(simplekey);
                //    List<string> newValues = new List<string>();
                //    for (int i = 0; i < oldValues.Length; i++)
                //    {
                //        if (oldValues[i] != table.ID.ToString())
                //            newValues.Add(oldValues[i]);
                //    }
                //    _simpleTableKeys[simplekey] = string.Join(",", newValues.ToArray());
                //}

                if (this.ExistsKey(table.Name, table.DataBase))
                {
                    var idStr = _simpleTableKeys[simplekey];
                    var ids = idStr.Split(',').ToList();

                    if (ids.Contains(table.ID.ToString()))
                    {
                        ids.Remove(table.ID.ToString());
                    }

                    if (ids.Count > 0)
                    {
                        _simpleTableKeys[simplekey] = string.Join(",", ids);
                    }
                    else
                    {
                        _simpleTableKeys.Remove(simplekey);
                    }
                }

                string fullkey = this.BuildFullTblKey(table.Name, table.Alias, table.DataBase);
                if (_fullTableKeys.GetValues(fullkey) != null)
                {
                    if (_fullTableKeys.GetValues(fullkey).Length < 2)
                        _fullTableKeys.Remove(fullkey);
                    else
                    {
                        string[] oldValues = _fullTableKeys.GetValues(fullkey);
                        List<string> newValues = new List<string>();
                        for (int i = 0; i < oldValues.Length; i++)
                        {
                            if (oldValues[i] != table.ID.ToString())
                                newValues.Add(oldValues[i]);
                        }
                        _fullTableKeys[fullkey] = string.Join(",", newValues.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private void UpdateTableFromLists(TableBO oldTable, TableBO newTable)
        {
            RemoveTableFromLists(oldTable);
            AddTableToLists(newTable);
        }

        private string BuildFullTblKey(string name, string alias, string database)
        {
            return database + _tableSeparator + name + _tableSeparator + alias;
        }

        private string BuildSimpleTblKey(string name, string database)
        {
            return database + _tableSeparator + name;
        }

        /// <summary>
        /// Method to check that all the ids are unique for the entire DataView.
        /// All the exceptions thrown must be catched by the UI.
        /// </summary>
        private void ValidateTablesAndField()
        {
            try
            {
                var fieldIds = new Dictionary<string, List<int>>();
                var tableIds = new Dictionary<string, List<int>>();

                foreach (TableBO tableA in this)
                {
                    if (tableA.FromMasterSchema)
                    {
                        continue;
                    }

                    int tableid = tableA.ID;

                    // If database is not exists, create new keyvalue pair with database and tableId.
                    if (!tableIds.ContainsKey(tableA.DataBase))
                    {
                        tableIds.Add(tableA.DataBase, new List<int> { tableid });
                    }
                    // If database exists but tableId not exists, add it to list.
                    else if (!tableIds[tableA.DataBase].Contains(tableid))
                    {
                        tableIds[tableA.DataBase].Add(tableid);
                    }
                    // Else throw the error. the tableId in database level should unique.
                    // NOTE: the database property already include the instance name.
                    else
                    {
                        var errorFormat = "TableListBO: The table ({0}:{1}) is not unique in the DataView";
                        throw new ValidationException(string.Format(errorFormat, tableA.DataBase, tableA.ID));
                    }

                    foreach (FieldBO field in tableA.Fields)
                    {
                        if (!field.FromMasterSchema)
                        {
                            int fieldid = field.ID;
                            if (!fieldIds.ContainsKey(tableA.DataBase))
                            {
                                fieldIds.Add(tableA.DataBase, new List<int> { fieldid });
                            }
                            else if (!fieldIds[tableA.DataBase].Contains(fieldid))
                            {
                                fieldIds[tableA.DataBase].Add(fieldid);
                            }
                            else
                            {
                                throw new ValidationException(string.Format(Resources.NotUniqueFieldID_Error_Text, tableA.Name + " (" + tableid + ")"));
                            }
                        }
                        if (field.LookupFieldId >= 0)
                        {
                            if (field.LookupFieldId == field.LookupDisplayFieldId)
                                throw new ValidationException(string.Format("Field {0} in tableId {1} is an invalid lookup. Join and display fields cannot be the same.", field.Alias, tableid));

                            TableBO lookupTable = this.GetTableByFieldId(field.LookupFieldId);
                            if (lookupTable == null)
                                throw new ValidationException(string.Format("Field {0} in tableId {1} is a lookup, but its lookup table is not present in dataview.", field.Alias, tableid));
                        }
                    }
                }

                foreach (TableBO tableA in this)
                {
                    string[] foundKeys = _fullTableKeys.GetValues(this.BuildFullTblKey(tableA.Name, tableA.Alias, tableA.DataBase));
                    if (foundKeys != null && foundKeys.Length > 1)
                        throw new ValidationException("TableListBO: The tableId " + tableA.ID + " is not unique in the DataView (Check other tables Name, Alias and Database)");
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Method to check that all pks (one per table) exist in the list of fields.
        /// </summary>
        /// <remarks>If there are more than one field with the same id, it will be found with the UniqueTablesAndFields method</remarks>
        private void CheckPKs()
        {
            foreach (TableBO table in this)
            {
                if (table.Fields.GetField(table.PrimaryKey) == null)
                    throw new ValidationException(string.Format(Resources.InvalidTablePK_Error_Text, table.ID));
            }
        }

        /// <summary>
        /// Gets a table given its name
        /// </summary>
        /// <param name="tableName">The name</param>
        /// <returns>The TableBO</returns>
        public TableBO this[string tableName]
        {
            get
            {
                foreach (TableBO currentTable in this)
                {
                    if (currentTable.Name == tableName)
                        return currentTable;
                }

                return null;
            }
        }

        /// <summary>
        /// Apply a sort to a list of tables
        /// </summary>
        /// <param name="property">Property name to sort by</param>
        /// <param name="direction">Asc or Desc direction</param>
        /// <returns>A sorted list of TableBO</returns>
        public SortedBindingList<TableBO> ApplySort(string property, ListSortDirection direction)
        {
            if (_sortedList == null)
                _sortedList = new SortedBindingList<TableBO>(this);
            if (IsAValidProperty(property))
                _sortedList.ApplySort(property, direction);
            return _sortedList;
        }

        /// <summary>
        /// Checks if the property name is correct in the current TableListBO
        /// </summary>
        /// <param name="property">Name of the property</param>
        /// <returns>A bool indicating if the property exists</returns>
        private bool IsAValidProperty(string property)
        {
            bool retVal = true;
            //TODO: Check that is a valid Property.
            return retVal;
        }

        /// <summary>
        /// Get a table given a TableId
        /// </summary>
        /// <param name="Id">Id of the table to look for</param>
        /// <returns>The found table or a null</returns>
        public TableBO GetTable(int Id)
        {
            TableBO result = null;
            _idToTable.TryGetValue(Id, out result);
            return result;
        }

        internal TableBO GetTable(string name, string alias, string database)
        {
            string foundTableId = _fullTableKeys[this.BuildFullTblKey(name, alias, database)];
            TableBO result = null;
            if (!string.IsNullOrEmpty(foundTableId))
            {
                _idToTable.TryGetValue(int.Parse(foundTableId), out result);
            }

            return result;
        }

        /// <summary>
        /// Get the first TableBO by using the table name and database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        internal TableBO GetTable(string name, string database)
        {
            string foundTableId = _simpleTableKeys[this.BuildSimpleTblKey(name, database)];
            TableBO result = null;
            if (!string.IsNullOrEmpty(foundTableId))
            {
                // Return the first table.
                var id = foundTableId.Split(',')[0];
                _idToTable.TryGetValue(int.Parse(id), out result);
            }

            return result;
        }

        /// <summary>
        /// Gets all the Tables with the given DataBase param
        /// </summary>
        /// <param name="database">Database name to look for</param>
        /// <returns>A list of the found tables</returns>
        public List<TableBO> GetTablesByDB(string database)
        {
            List<TableBO> list = new List<TableBO>();
            try
            {
                foreach (TableBO table in this)
                {
                    if (table.DataBase == database)
                        list.Add(table);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Gets all the Tables with the given DataBase param that has FromMasterSchema property as the given
        /// </summary>
        /// <param name="database">Database name to look for</param>
        /// <param name="fromMasterSchema">FromMasterSchema value</param>
        /// <returns>A list of the found tables</returns>
        public List<TableBO> GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        {
            List<TableBO> list = new List<TableBO>();
            try
            {
                foreach (TableBO table in this)
                {
                    if (table.DataBase == database && table.FromMasterSchema == fromMasterSchema)
                        list.Add(table);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Gets all the Tables with the given DataBase param that has FromMasterSchema property as the given, and exluding specified table ids.
        /// </summary>
        /// <param name="database">Database name to look for</param>
        /// <param name="fromMasterSchema">FromMasterSchema value</param>
        /// <param name="tableIdsToExclude">Table ids to be excluded, separated by commas</param>
        /// <returns>A list of the found tables</returns>
        public List<TableBO> GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        {
            List<TableBO> list = new List<TableBO>();
            try
            {
                foreach (TableBO table in this)
                {
                    if (table.DataBase == database && table.FromMasterSchema == fromMasterSchema && !tableIdsToExclude.Contains(table.ID.ToString()))
                        list.Add(table);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Gets all the Tables that has FromMasterSchema property as the given.
        /// </summary>
        /// <param name="fromMasterSchema">FromMasterSchema value</param>
        /// <returns>A list of the found tables</returns>
        public List<TableBO> GetTablesFromMasterSchema(bool fromMasterSchema)
        {
            List<TableBO> list = new List<TableBO>();
            try
            {
                foreach (TableBO table in this)
                {
                    if (table.FromMasterSchema == fromMasterSchema)
                        list.Add(table);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }
        /// <summary>
        /// Gets a FieldBO given a fieldID 
        /// This methods looks in all the tables for the fiven fieldId
        /// </summary>
        /// <param name="fieldId">Id of the field to look for</param>
        /// <returns>The found fieldId or null</returns>
        /// <remarks>Call this method in case you don't know to wich table this fieldId belongs</remarks>
        public FieldBO GetField(int fieldId)
        {
            foreach (TableBO table in this)
            {
                FieldBO field = table.Fields.GetField(fieldId);
                if (field != null)
                    return field;
            }
            return null;
        }

        /// <summary>
        /// Gets a tableID given a fieldId
        /// This method looks for the table in which this fieldId belongs.
        /// </summary>
        /// <param name="fieldId">Id of the field to look for</param>
        /// <returns>The id of the found Table or int.MinValue</returns>
        public int GetTableIdByFieldId(int fieldId)
        {
            try
            {
                foreach (TableBO table in this)
                {
                    FieldBO field = table.Fields.GetField(fieldId);
                    if (field != null)
                        return table.ID;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return int.MinValue;//I have moved this in order to maintain consistency.
        }

        /// <summary>
        /// Gets all the tables by a lookup field id
        /// </summary>
        /// <param name="lookupField">Lookup field</param>
        /// <returns></returns>
        public List<TableBO> GetTablesByLookup(int lookupField)
        {
            List<TableBO> list = new List<TableBO>();
            try
            {
                foreach (TableBO table in this)
                {
                    List<FieldBO> fields = table.Fields.GetFieldsByLookup(lookupField);
                    if (fields != null && fields.Count > 0)
                        list.Add(table);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Gets a table given a fieldId
        /// This method looks for the table in which this fieldId belongs.
        /// </summary>
        /// <param name="fieldId">Id of the field to look for</param>
        /// <returns>The found table or null</returns>
        public TableBO GetTableByFieldId(int fieldId)
        {
            try
            {
                foreach (TableBO table in this)
                {
                    FieldBO field = table.Fields.GetField(fieldId);
                    if (field != null)
                        return table;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        /// <summary>
        /// Gets all databases involved in this table list.
        /// </summary>
        /// <returns>All databases involved in this table list.</returns>
        public string[] GetDatabasesInTables()
        {
            List<string> databases = new List<string>();
            try
            {
                foreach (TableBO table in this)
                {
                    if (!databases.Contains(table.DataBase))
                        databases.Add(table.DataBase);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return databases.ToArray();
        }

        /// <summary>
        /// Gets all the Tables with the given Instance param
        /// </summary>
        /// <param name="instanceName">Database name to look for</param>
        /// <returns>A list of the found tables</returns>
        public List<string> GetTablesByInstance(string instanceName)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (TableBO table in this)
                {
                    string tableIntanceName = CambridgeSoft.COE.Framework.Common.Utilities.GetInstanceName(table.DataBase);

                    if (tableIntanceName.Equals(instanceName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        list.Add(table.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Checks the Id of table against the highestID
        /// </summary>
        /// <param name="table">Table to check</param>
        private void CheckHighestID(TableBO table)
        {
            int maxTempId = Math.Max(table.ID, table.Fields.HighestID);
            if (maxTempId > _highestID)
                _highestID = maxTempId;
        }

        /// <summary>
        /// if id is is highest than the current highest id, it updates the highest id.
        /// </summary>
        /// <param name="id"></param>
        private void SetHighestID(int id)
        {
            if (id > _highestID)
            {
                _highestID = id;
            }
        }

        /// <summary>
        /// Get the next available Id for a table.
        /// </summary>
        /// <returns>A valid Id to use</returns>
        private int GetId()
        {
            return ++_highestID;
        }

        /// <summary>
        /// Removes a List of table of the current List
        /// </summary>
        /// <param name="IdList">List of table ids to remove from the current list</param>
        public void Remove(List<int> IdList)
        {
            foreach (int id in IdList)
                this.Remove(id);
        }

        /// <summary>
        /// Removes a single table from the list
        /// </summary>
        /// <param name="id">Table's id to be removed</param>
        public void Remove(int id)
        {
            try
            {
                if (id >= 0)
                {
                    TableBO table = this.GetTable(id);
                    if (table != null)
                    {
                        this.RemoveTableFromLists(table);
                        base.Remove(table);
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Method to clone a table (and fields inside) and add it to the current TableListBO.
        /// This is to create Table Alias
        /// </summary>
        /// <param name="tableID">Id of the table that the clone will be based on</param>
        /// <param name="wordToAdd">A word to add to the cloned table</param>
        public int CloneAndAddTable(int tableID, string wordToAdd)
        {
            TableBO newTable = this.GetTable(tableID).Clone();
            this.RegenerateIds(newTable);
            newTable = TableBO.NewTable(newTable);
            //We call this method in order to make sure each table/field is unique. See more details in method's comments
            if (string.IsNullOrEmpty(wordToAdd))
                wordToAdd = "_Alias"; //TODO: Move to Resources file.
            newTable.Alias += wordToAdd;
            this.Add(newTable);
            return newTable.ID;
        }

        #endregion

        #region Constructor

        private TableListBO()
        {
            InitializeArrays();
        }

        private TableListBO(List<COEDataView.DataViewTable> tables)
            : this()
        {
            TableBO currentTable;
            foreach (COEDataView.DataViewTable table in tables)
            {
                currentTable = TableBO.NewTable(table);
                this.Add(currentTable);
            }
        }

        private TableListBO(List<COEDataView.DataViewTable> tables, bool isClean)
            : this()
        {
            TableBO currentTable;
            foreach (COEDataView.DataViewTable table in tables)
            {
                currentTable = TableBO.NewTable(table, isClean);
                this.Add(currentTable);
            }
        }

        private TableListBO(TableListBO tables)
            : this()
        {
            foreach (TableBO table in tables)
            {
                if (!this.Exists(table))
                {
                    this.Add(table);
                }
            }
        }

        protected override void InsertItem(int index, TableBO item)
        {
            this.AddTableToLists(item);
            this.CheckHighestID(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, TableBO item)
        {
            this.UpdateTableFromLists(this[index], item);
            base.SetItem(index, item);
        }
        #endregion

        /// <summary>
        /// Method to re generate the ids of a table (and innner fields) according the HighestID used in the current TableListBO.
        /// </summary>
        /// <param name="table">Table to check ids</param>
        /// <remarks>It's also checked fieldIds and PrimaryKey value</remarks>
        public void RegenerateIds(TableBO table)
        {
            if (table.ID < _highestID)
                table.ID = this.GetId();
            else
                _highestID = table.ID;

            foreach (FieldBO field in table.Fields)
            {
                int oldFieldId = field.ID;
                if (field.ID < _highestID)
                {
                    field.ID = this.GetId();
                    if (oldFieldId == table.PrimaryKey)
                        table.PrimaryKey = field.ID;
                }
                else
                    _highestID = field.ID;
            }
        }
    }
}
