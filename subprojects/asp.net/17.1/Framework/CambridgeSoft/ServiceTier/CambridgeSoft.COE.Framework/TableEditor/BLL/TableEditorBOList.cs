using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Collections.Generic;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Xml;


namespace CambridgeSoft.COE.Framework.COETableEditorService
{
    #region Class - COETableEditorBOList
    /// <summary>
    /// This object is implemented as a CSLA list object to hold a list of COETableEditorBO objects.
    /// </summary>
    [Serializable()]
    public class COETableEditorBOList : Csla.BusinessListBase<COETableEditorBOList, COETableEditorBO>
    {
        #region Variables

        private static DAL _coeDAL = null;
        private static DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COETableEditor";
        private DataTable _returnDataTable;
        private static bool FlgLookupCall;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COETableEditor");

        #endregion

        #region Properties

        /// <summary>
        /// The DataTable for business object to get data.
        /// </summary>
        public DataTable ReturnDataTable
        {
            get
            {
                return _returnDataTable;
            }
            set
            {
                _returnDataTable = value;
            }
        }

        /// <summary>
        /// Set the TableName.
        /// </summary>
        /// <param name="tableName">TableName need to be set after selection by the user</param>
        public string TableName
        {
            set
            {
                if (_coeDAL == null) { LoadDAL(); }  //child table ,move to other place
                _coeDAL.SetServiceSpecificVariables(value);
            }

            get
            {
                return _coeDAL._COETableEditorTableName;
            }

        }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructor
        /// </summary>
        private COETableEditorBOList()
        { /* require use of factory method */ }

        #endregion

        #region Member Mothods

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            string _databaseName = ConfigurationUtilities.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, _databaseName, true);
        }

        /// <summary>
        /// Overloaded method of the LoadDAL, 
        /// </summary>
        /// <param name="appName">Name of the Application, It should be one from the provided in the Configuration.</param>
        private void LoadDAL(string appName)
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, appName, true);
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Instantialize a COETableEditorBOList
        /// </summary>
        /// <returns></returns>
        public static COETableEditorBOList NewList()
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COETableEditorBOList");
            return new COETableEditorBOList();
        }

        /// <summary>
        /// To set the name of Database.
        /// </summary>
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }

        /// <summary>
        /// init DALFactory
        /// </summary>
        public static void Reset()
        {
            _coeDAL = null;
            _dalFactory = new DALFactory();
        }

        public static bool CanAddObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        public static bool CanGetObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }
        public static bool CanEditObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }
        public static bool CanDeleteObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        #endregion //Factory Methods

        #region Business Methods

        /// <summary>
        /// Get lookup values of the field.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FieldforLookup">Field for lookup</param>
        /// <returns>Lookup value list</returns>
        public static List<ID_Column> getLookupFieldList(int id, string FieldforLookup)
        {
            // updated by Jerry on 2008/07/08 for lookuplocation is xml file 
            SetDatabaseName();
            string lookupField = COETableEditorUtilities.getLookupField(_coeDAL._COETableEditorTableName, FieldforLookup);
            string lookupID = COETableEditorUtilities.getLookupID(_coeDAL._COETableEditorTableName, FieldforLookup);
            string lookupLocation = COETableEditorUtilities.getLookupLocation(_coeDAL._COETableEditorTableName, FieldforLookup);
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + "COETableEditorBOList");

            List<ID_Column> Id_Column_List = new List<ID_Column>();
            if (lookupLocation.ToLower() == "database")
            {
                COETableEditorBOList lst = DataPortal.Fetch<COETableEditorBOList>(new LookupCriteria(FieldforLookup));
                FlgLookupCall = true;
                foreach (COETableEditorBO TEobj in lst)
                {
                    Id_Column_List.Add(new ID_Column(TEobj.ID.ToString(), TEobj.Columns.Find(new GetColumn(lookupField).Match)));
                }
            }
            //lookupfieldlocation property of the lookupfield is xml in coeFramework 
            else if (lookupLocation.ToLower().Contains("innerxml_"))
            {
                Id_Column_List = COETableEditorUtilities.getId_Column_List(_coeDAL._COETableEditorTableName, FieldforLookup);
            }
            //lookupfieldlocation property of the lookupfield is xml file  
            else
            {
                XmlDocument _xmlDocument = new XmlDocument();
                _xmlDocument.Load(lookupLocation);
                XmlNodeList rowXmlNodeList = _xmlDocument.DocumentElement.ChildNodes;
                XmlNode idNode = null;
                XmlNode columnNode = null;
                int idValue = 0;
                foreach (XmlElement rowElement in rowXmlNodeList)
                {
                    idNode = (XmlNode)rowElement.SelectSingleNode(lookupID);
                    columnNode = (XmlNode)rowElement.SelectSingleNode(lookupField);
                    idValue = int.Parse(idNode.InnerText.Trim());
                    Column column = new Column(lookupField, DbType.AnsiString);
                    column.FieldValue = columnNode.InnerText;
                    Id_Column_List.Add(new ID_Column(idValue.ToString(), column));
                }
            }

            return Id_Column_List;
        }

        /// <summary>
        /// Get all data of the current table.
        /// child table for bind webgrid
        /// </summary>
        /// <returns></returns>
        public static DataTable getTableEditorDataTable(string tableName)
        {
            SetDatabaseName();
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + "COETableEditorBOList");

            FlgLookupCall = false;
            COETableEditorBOList lst = DataPortal.Fetch<COETableEditorBOList>(new Criteria(tableName));

            return lst.ReturnDataTable;
        }

        /// <summary>
        /// Get all data of CurrentTable's ChildTable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static DataTable getChildDataTable(string id, bool flag)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + "COETableEditorBOList");

            FlgLookupCall = false;
            COETableEditorBOList boList = DataPortal.Fetch<COETableEditorBOList>(new ChildTableCriteria(id, flag));

            return boList.ReturnDataTable;
        }

        #endregion

        #region Data Access Methods

        #region Filter Criteria

        /// <summary>
        /// TODO: for getting the current table's parameters
        /// </summary>
        [Serializable()]
        private class Criteria
        {
            private string _tableName;
            
            //constructors
            public Criteria()
            {

            }
            public string TableName
            {
                get { return _tableName; }
            }

            public Criteria(string tableName)
            { _tableName = tableName; }
            //end
        }

        /// <summary>
        /// For getting the lookup values's parameters
        /// </summary>
        [Serializable()]
        private class LookupCriteria
        {
            internal string _fieldForLookup;

            public LookupCriteria(string fieldForLookup)
            {
                _fieldForLookup = fieldForLookup;
            }
        }

        /// <summary>
        /// For getting the ChildTable values's parameters
        /// </summary>
        [Serializable()]
        private class ChildTableCriteria
        {
            private string _id;

            private bool _flag;

            public ChildTableCriteria(string id, bool flag)
            {
                _id = id;
                _flag = flag;
            }

            public string ID
            {
                get
                {
                    return _id;
                }
            }

            public bool Flag
            {
                get
                {
                    return _flag;
                }
            }
        }

        #endregion //Filter Criteria


        #region Data Access - Fetch

        /// <summary>
        /// fetch lookup data
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(LookupCriteria criteria)
        {
            string fieldName = string.Empty;
            RaiseListChangedEvents = false;
            SafeDataReader dr = null;
            if (_coeDAL == null) { LoadDAL(); }
            FlgLookupCall = true;
            fieldName = criteria._fieldForLookup;

            dr = _coeDAL.GetLookupData(criteria._fieldForLookup);

            Fetch(COEAppName.Get().ToString(), dr, fieldName);

            RaiseListChangedEvents = true;
        }

        /// <summary>
        /// get parent table data
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(Criteria criteria)
        {
            //added by Jerry on 2008/07/04 for fetch a ReturnTable
            RaiseListChangedEvents = false;
            FlgLookupCall = false;
            if (_coeDAL == null) { LoadDAL(); }
            _returnDataTable = _coeDAL.GetParentTableData(criteria.TableName);
            RaiseListChangedEvents = true;
        }

        /// <summary>
        /// get child table data
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(ChildTableCriteria criteria)
        {
            RaiseListChangedEvents = false;
            FlgLookupCall = false;
            if (_coeDAL == null) { LoadDAL(); }
            _returnDataTable = _coeDAL.GetChildTableData(criteria.ID, criteria.Flag);
            RaiseListChangedEvents = true;
        }

        /// <summary>
        /// fetch data
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="dr"></param>
        /// <param name="FieldName"></param>
        protected void Fetch(string applicationName, SafeDataReader dr, string FieldName)
        {
            while (dr.Read())
            {
                try
                {
                    int _id;
                    List<Column> _cList = null;
                    string _sequenceName;
                    if (FlgLookupCall)
                    {
                        _id = dr.GetInt16(COETableEditorUtilities.getLookupID(_coeDAL._COETableEditorTableName, FieldName));
                        _cList = COETableEditorUtilities.getLookupColumnList(_coeDAL._COETableEditorTableName, FieldName);
                    }
                    else
                    {
                        _id = dr.GetInt16(COETableEditorUtilities.getIdFieldName(_coeDAL._COETableEditorTableName));
                        _cList = COETableEditorUtilities.getColumnList(_coeDAL._COETableEditorTableName);
                    }
                    _sequenceName = COETableEditorUtilities.getSequenceName(_coeDAL._COETableEditorTableName);
                    for (int i = 0; i < _cList.Count; i++)
                    {
                        switch (_cList[i].FieldType)
                        {
                            case DbType.DateTime:
                                _cList[i].FieldValue = dr.GetSmartDate(_cList[i].FieldName);
                                break;
                            case DbType.Boolean:
                                _cList[i].FieldValue = dr.GetBoolean(_cList[i].FieldName);
                                break;
                            case DbType.Int16:
                                _cList[i].FieldValue = dr.GetInt16(_cList[i].FieldName);
                                break;
                            case DbType.Double:
                                _cList[i].FieldValue = dr.GetDouble(_cList[i].FieldName);
                                break;
                            default:
                                _cList[i].FieldValue = dr.GetString(_cList[i].FieldName);
                                break;
                        }
                    }

                    COETableEditorBO tableEditorData = new COETableEditorBO(
                        _id,
                        _cList,
                        applicationName,
                        _sequenceName);
                    this.Add(tableEditorData);
                }
                catch (Exception e)
                {
                    //Loopthrough
                }
            }
            dr.Close();
        }

        #endregion


        #region Data Access - Update
        /// <summary>
        /// update data
        /// </summary>
        protected override void DataPortal_Update()
        {
            RaiseListChangedEvents = false;
            if (_coeDAL == null) { LoadDAL(); }

            // loop through each deleted child object
            foreach (COETableEditorBO deletedChild in DeletedList)
                deletedChild.DeleteSelf(_coeDAL);
            DeletedList.Clear();

            // loop through each non-deleted child object
            foreach (COETableEditorBO child in this)
            {
                if (child.IsNew)
                    child.Insert(_coeDAL);
                else
                    child.Update(_coeDAL);
                RaiseListChangedEvents = true;
            }
        }

        #endregion //Data Access - Update
    
        #endregion

    }

    #endregion

    #region Class - ID_Column
    /// <summary>
    /// Class to represent ID with any other field/Column of the Record. Its object can be 
    /// used inside the combobox of the TableEditor Control.
    /// add by Jerry to Test lookup field validator on 2008/09/10
    /// </summary>
    [Serializable()] 
    public class ID_Column
    {
        #region Properties

        string _id;
        Column _column;

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public object PColumn
        {
            get
            {
                return _column.FieldValue.ToString();
            }
            set
            {
                _column = (Column)value;
            }
        }

        #endregion

        #region Constructor

        public ID_Column(string id, Column col)
        {
            _id = id;
            _column = col;
        }

        #endregion
    }

    #endregion

    #region Class - GetColumn

    /// <summary>
    /// Predicate column so that the Columns Property can be searched for the Particular column.
    /// </summary>
    /// 
    public class GetColumn
    {
        #region Properties

        private string _columnName;

        // Initializes with suffix we want to match.
        public GetColumn(string columnName)
        {
            _columnName = columnName;
        }

        // Sets a different suffix to match.
        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        #endregion

        #region Methods

        // Gets the predicate.  Now it's possible to re-use this predicate with 
        // various suffixes.
        public Predicate<Column> Match
        {
            get { return IsMatch; }
        }

        private bool IsMatch(Column s)
        {
            if (s.FieldName.Equals(_columnName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    #endregion 
}


