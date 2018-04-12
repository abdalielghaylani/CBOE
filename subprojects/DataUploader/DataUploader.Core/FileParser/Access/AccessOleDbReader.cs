using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.Properties;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.Access
{
    /// <summary>
    /// Reads table or view data from Access files (MS Office 97 through MS Office 2007),
    /// using the OleDb model.
    /// </summary>
    public class AccessOleDbReader : FileReaderBase
    {
        private string _extendedPropFileVersion = "Access {0}.0";
        private string _provider;
        private string _sqlSelect = null;
        private bool _leaveConnectionOpen = false;
        private int _readAheadSize = 250;
        private int _readChunkStartIndex = 0;
        private OleDbConnectionStringBuilder _connectionBuilder;
        private DataTable _queueTable = null;
        private DataTable[] _queueTables = null;

        #region > Constructors <

        /// <summary>
        /// Provides initialization values for the System.Data.OleDb.OleDbConnection object
        /// that will read the given file.
        /// </summary>
        /// <param name="accessFilePath">the complete path to the file to be read</param>
        /// <param name="tableOrViewName">name of the table or view to read data from</param>
        /// <param name="accessVersion">the version of the Access file</param>
        public AccessOleDbReader(
            string accessFilePath
            , string tableOrViewName
            , MSOfficeVersion accessVersion
            )
        {
            this._accessFileInfo = new FileInfo(accessFilePath);
            this._currentTableOrViewName = tableOrViewName;

            this.Initialize(accessVersion);
        }

        private void Initialize(MSOfficeVersion accessVersion)
        {
            //verify the file exists
            if (!this._accessFileInfo.Exists)
            {
                string err = string.Format("Could not find file: {0}", this._accessFileInfo.FullName);
                throw new FileNotFoundException(err);
            }

            //determine the Access file version
            if (accessVersion == MSOfficeVersion.Unknown)
            {
                if (this._accessFileInfo.Extension.ToLower().Equals(MSOfficeConstants.ACCESS_2007_EXTENSION))
                    accessVersion = MSOfficeVersion.Offcie2007;
            }

            string version = ((int)accessVersion).ToString();
            switch (accessVersion)
            {
                case MSOfficeVersion.Offcie2007:
                    this._provider = MSOfficeConstants.ACE_PROVIDER;
                    this._extendedPropFileVersion = string.Format(this._extendedPropFileVersion, version);
                    break;
                default:
                    this._provider = MSOfficeConstants.JET_PROVIDER;
                    this._extendedPropFileVersion = string.Format(this._extendedPropFileVersion, "8");
                    break;
            }

            _leaveConnectionOpen = true;

            //ensure the table or view actually exists
            List<string> tableAndViewNames = this.GetTableAndViewList();
            if (tableAndViewNames.Contains(this._currentTableOrViewName))
            {
                //ensure the table/view contains at least one column
                List<DataColumn> columns = GetTableOrViewColumns(this._currentTableOrViewName, true);
                if (columns.Count > 0)
                {
                    this._fieldNames = ExtractFieldNames(columns);
                    this._queueTable = CreateTemplateTable(this._currentTableOrViewName, columns);
                    this._queueTables = new DataTable[] { this._queueTable };

                    //generate the default select statement to be used by the reader
                    string columnsToFetch = GetDelimitedColumnNames(columns, ", ");
                    string selectTemplate = "select {0} from [{1}]";
                    _sqlSelect = string.Format(selectTemplate, columnsToFetch, this._queueTable.TableName);

                    //create all the ADO.Net entities that will leverage this connection
                    _command = this.Connection.CreateCommand();
                    _command.CommandText = _sqlSelect;
                    _dataAdapter = new OleDbDataAdapter((OleDbCommand)_command);

                    this._hasValidTable = true;
                }
            }

            _leaveConnectionOpen = false;
            this.Connection.Close();
        }

        #endregion

        #region > Properties <

        private List<string> _fieldNames = new List<string>();
        /// <summary>
        /// The field names, either discovered or artifically created.
        /// </summary>
        public List<string> FieldNames
        {
            get { return _fieldNames; }
        }

        private MappingType _xmlMapping = MappingType.Element;
        /// <summary>
        /// Determines the serialization mechanism for the Datatable objects that will hold the data.
        /// This must be set prior to extracting the metadata during the data-extraction.
        /// </summary>
        public MappingType XmlMapping
        {
            get { return _xmlMapping; }
            set { _xmlMapping = value; }
        }

        private DbCommand _command = null;
        private OleDbDataAdapter _dataAdapter = null;

        private OleDbConnection _connection;
        /// <summary>
        /// Read-only
        /// </summary>
        public OleDbConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = this.InitializeConnection();
                return this._connection;
            }
        }

        private FileInfo _accessFileInfo;
        /// <summary>
        /// Read only
        /// </summary>
        public FileInfo AccessFileInfo
        {
            get { return this._accessFileInfo; }
        }

        // instance information
        private bool _hasValidTable;
        /// <summary>
        /// Read only. Set by the instance constructor if the Access table/view is readable.
        /// </summary>
        public bool HasValidTable
        {
            get { return _hasValidTable; }
        }

        private string _currentTableOrViewName;
        /// <summary>
        /// The active table/view to read from.
        /// </summary>
        public string CurrentTableOrViewName
        {
            get { return _currentTableOrViewName; }
            set { _currentTableOrViewName = value; }
        }

        #endregion

        #region > Utilities <

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInformation"></param>
        /// <returns></returns>
        public static List<string> FetchTableNames(SourceFileInfo fileInformation)
        {
            if (fileInformation.FileType != SourceFileType.MSAccess) return null;
            AccessOleDbReader reader = null;
            try
            {
                reader = new AccessOleDbReader(
                               fileInformation.FullFilePath
                               , null
                               , MSOfficeVersion.Unknown
                           );

                return reader.GetTableAndViewList();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Opens the file and reads its metadata to discover the table and view names.
        /// </summary>
        /// <returns>a string list of table/view names found in the file</returns>
        public List<string> GetTableAndViewList()
        {
            List<string> tableAndViewNames = new List<string>();
            try
            {
                if (this.Connection.State != ConnectionState.Open)
                    this.Connection.Open();

                string[] tableRestrictions = new string[4];
                tableRestrictions[3] = "TABLE";//restrict TABLE_TYPE to retrieve only the user created tables.
                DataTable metaTable = this.Connection.GetSchema("Tables", tableRestrictions); 
                DataTable metaViews = this.Connection.GetSchema("Views");
                metaTable.Merge(metaViews, true, MissingSchemaAction.AddWithKey);

                for (int rowIndex = 0; rowIndex < metaTable.Rows.Count; rowIndex++)
                {
                    object[] vals = metaTable.Rows[rowIndex].ItemArray;
                    string tableOrViewName = metaTable.Rows[rowIndex].ItemArray[MSOfficeConstants.TABLE_NAME_METAINDEX].ToString();
                    tableAndViewNames.Add(tableOrViewName);
                }

                return tableAndViewNames;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!_leaveConnectionOpen)
                    if (this.Connection.State != ConnectionState.Closed)
                        this.Connection.Close();
            }
        }

        /// <summary>
        /// For a given table or view, gets the count of rows matching the criteria.
        /// </summary>
        /// <param name="tableOrViewName">a table or view name</param>
        /// <param name="filterClause">analog of the System.Data.Datatable's 'Select' property</param>
        /// <returns>the number of rows in the table or view</returns>
        public int GetTableOrViewRowCount(string tableOrViewName, string filterClause)
        {
            int rowCount;
            try
            {
                if (this.Connection.State != ConnectionState.Open)
                    this.Connection.Open();

                OleDbCommand cmd = this.Connection.CreateCommand();
                string select = "select count(*) from [{0}]";
                if (!string.IsNullOrEmpty(filterClause))
                    select += String.Format(" where {0}", filterClause);

                cmd.CommandText = string.Format(select, tableOrViewName);
                rowCount = (int)cmd.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!_leaveConnectionOpen)
                    if (this.Connection.State != ConnectionState.Closed)
                        this.Connection.Close();
            }

            return rowCount;
        }

        /// <summary>
        /// Extract the table/view column metadata.
        /// </summary>
        /// <param name="tableOrViewName">a table or view name</param>
        /// <param name="inferDataTypes">if true, all types default to 'System.String'</param>
        /// <returns>a list of DataColumn objects</returns>
        public List<DataColumn> GetTableOrViewColumns(string tableOrViewName, bool inferDataTypes)
        {
            List<DataColumn> tableColumns = new List<DataColumn>();
            try
            {
                if (this.Connection.State != ConnectionState.Open)
                    this.Connection.Open();

                //string[] restrictions = { null, null, tableName, null };
                //DataTable columns = this.Connection.GetSchema("Columns", restrictions);
                //foreach (DataRow row in columns.Rows)
                //{
                //    object[] items = row.ItemArray;
                //}

                OleDbCommand cmd = this.Connection.CreateCommand();
                cmd.CommandText = string.Format("select * from [{0}]", tableOrViewName);
                OleDbDataReader columnReader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);

                DataTable schemaTable = columnReader.GetSchemaTable();
                //Coverity fix - CID 19190
                if (schemaTable != null)
                {
                foreach (DataRow schemaRow in schemaTable.Rows)
                {
                    string colName = schemaRow[MSOfficeConstants.COLUMN_NAME_METAINDEX].ToString();
                    string colType = string.Empty;
                    if (inferDataTypes == true)
                        colType = schemaRow[MSOfficeConstants.COLUMN_TYPE_METAINDEX].ToString();
                    else
                        colType = (typeof(System.String)).ToString();

                    DataColumn column = new DataColumn(
                        colName
                        , System.Type.GetType(colType)
                        , null
                        , MappingType.Element
                    );

                    tableColumns.Add(column);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!_leaveConnectionOpen)
                    if (this.Connection.State != ConnectionState.Closed)
                        this.Connection.Close();
            }

            return tableColumns;
        }

        /// <summary>
        /// Utilizes a pre-configured IDataAdapter instance and template table to extract
        /// a set number of table/view rows, starting at a given row index.
        /// </summary>
        /// <param name="templateTable">a DataTable object with the correct columns</param>
        /// <param name="da">an OleDbDataAdapter instance</param>
        /// <param name="startingIndex">the starting row index for the selection range</param>
        /// <param name="recordsToRetrieve">the number of rows to retrieve</param>
        public void GetTableOrViewData(
            ref DataTable[] templateTable
            , OleDbDataAdapter da
            , int startingIndex
            , int recordsToRetrieve
            )
        {
            da.Fill(startingIndex, recordsToRetrieve, templateTable);
        }

        /// <summary>
        /// Generates a System.Data.DataTable template based on a table or view name and the derived list
        /// of columns.
        /// </summary>
        /// <param name="tableOrViewName">a table or view name</param>
        /// <param name="columnsToInclude">the column list to use in the output</param>
        /// <returns></returns>
        public DataTable CreateTemplateTable(string tableOrViewName, List<DataColumn> columnsToInclude)
        {
            DataTable template = new DataTable(tableOrViewName);
            List<DataColumn> myCols = new List<DataColumn>();

            foreach (DataColumn dc in columnsToInclude)
            {
                DataColumn clonedColumn = new DataColumn(
                    dc.ColumnName
                    , dc.DataType
                    , dc.Expression
                    , dc.ColumnMapping
                );
                clonedColumn.Caption = dc.Caption;
                clonedColumn.ColumnMapping = this._xmlMapping;
                myCols.Add(clonedColumn);
            }

            template.Columns.AddRange(myCols.ToArray());
            return template;
        }

        /// <summary>
        /// Creates a delimitd list for use in a 'select'
        /// </summary>
        /// <param name="dataColumns">typed collection object, DataColumnCollection</param>
        /// <param name="delimiter">Defaults to ',' if null or empty</param>
        /// <returns>Delimited string of bracket-wrapped column names (ex. '[Service Date]')</returns>
        public string GetDelimitedColumnNames(DataColumnCollection dataColumns, string delimiter)
        {
            DataColumn[] cols = new DataColumn[dataColumns.Count];
            dataColumns.CopyTo(cols, 0);
            List<DataColumn> columns = new List<DataColumn>(cols);

            return GetDelimitedColumnNames(columns, delimiter);
        }

        /// <summary>
        /// Creates a delimitd list for use in a 'select'
        /// </summary>
        /// <param name="dataColumns">List of System.Data.DataColumn objects</param>
        /// <param name="delimiter">Defaults to ',' if null or empty</param>
        /// <returns>Delimited string of bracket-wrapped column names (ex. '[Service Date]')</returns>
        public string GetDelimitedColumnNames(List<DataColumn> dataColumns, string delimiter)
        {
            string columnsToFetch = string.Empty;

            if (string.IsNullOrEmpty(delimiter))
                delimiter = ",";

            foreach (DataColumn col in dataColumns)
            {
                if (string.IsNullOrEmpty(columnsToFetch))
                    columnsToFetch = "[" + col.ColumnName + "]";
                else
                    columnsToFetch += delimiter + "[" + col.ColumnName + "]";
            }
            return columnsToFetch;
        }

        #endregion

        #region > IFileReader<CSVSourceRecord> Members <

        public override int CountAll()
        {
            this._totalRecordCount = this.GetTableOrViewRowCount(this._currentTableOrViewName, null);
            return _totalRecordCount;
        }

        public override void ReadNext()
        {
            this.OnRecordParsing(new RecordParsingEventArgs(_currentRecordIndex));

            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            //Create a queue of records to improve the performance of OldDb calls
            int chunkSize = _readAheadSize;
            object[] rowObjectValues = null;

            if (_currentRecordIndex < RecordCount)
            {
                //If the required row has not been read,read new chunk starting from the given index
                if (_queueTable.Rows.Count==0  ||
                    _currentRecordIndex < _readChunkStartIndex || _currentRecordIndex > _readChunkStartIndex + _readAheadSize - 1)
                {
                    this._queueTable.Rows.Clear();
                    this.GetTableOrViewData(ref _queueTables, _dataAdapter, _currentRecordIndex, chunkSize);
                    _readChunkStartIndex = _currentRecordIndex;
                }

                rowObjectValues = this._queueTable.Rows[_currentRecordIndex - _readChunkStartIndex].ItemArray;

                _currentRecordIndex++;

                this.ExtractRecord(rowObjectValues);

                _parsedRecordCount++;

                this.OnRecordParsed(new RecordParsedEventArgs(_current));
            }
            else
            {
                _current = null;
            }
        }

        public override ISourceRecord GetNext()
        {
            ReadNext();
            return Current; ;
        }

        public override void Seek(int recordIndex)
        {
            if (recordIndex < this.RecordCount)
                _currentRecordIndex = recordIndex;
            else
            {
                _currentRecordIndex = 0;
                throw new IndexOutOfRangeException(string.Format(Resources.AccessReaderIndexOutOfRng , this.RecordCount));
            }
        }

        public override void Rewind()
        {
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            this._current = null;
        }

        public override void Close()
        {
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            if (this.Connection.State != ConnectionState.Closed)
                this.Connection.Close();
        }

        #endregion

        /// <summary>
        /// The Connection property uses a lazy-loading mechanism to instantiate the OleDb
        /// connection to the Access file.
        /// </summary>
        /// <returns>a new OleDbConnection instance</returns>
        private OleDbConnection InitializeConnection()
        {
            string connStr = string.Empty;
            string extendedPropsRaw = "{0};";
            string extendedProps = string.Format(extendedPropsRaw, _extendedPropFileVersion);

            _connectionBuilder = new OleDbConnectionStringBuilder();
            _connectionBuilder.Provider = _provider;
            _connectionBuilder.DataSource = _accessFileInfo.FullName;
            //_connectionBuilder.Add("extended properties", extendedProps);
            _connectionBuilder.Add("mode", 8);
            _connectionBuilder.Add("prompt", 4);
            //_connectionBuilder.Add("exclusive", 1);

            //finalize the connection string
            connStr = _connectionBuilder.ConnectionString;
            OleDbConnection conn = new OleDbConnection(connStr);

            //return the connection
            return conn;
        }

        /// <summary>
        /// Generates a list of strings representing the DataColumns.
        /// </summary>
        /// <param name="columns">the list of DataColumn objects from the table's schema</param>
        /// <returns>a list of strings to be used as column names</returns>
        private List<string> ExtractFieldNames(List<DataColumn> columns)
        {
            List<string> columnNames = new List<string>();
            foreach (DataColumn dc in columns)
            {
                columnNames.Add(dc.ColumnName);
            }
            return columnNames;
        }

        private void ExtractRecord(object[] rowValues)
        {
            string[] rowStringValues = new string[rowValues.Length];

            for (int i = 0; i < rowValues.Length; i++)
            {
                string buf = null;
                if (rowValues[i] != DBNull.Value)
                    buf = rowValues[i].ToString();
                rowStringValues[i] = buf;
            }

            ExtractRecord(rowStringValues);
        }

        private void ExtractRecord(string[] rowValues)
        {
            _current = new CSVSourceRecord(this.CurrentRecordIndex, rowValues);

            if (rowValues != null && rowValues.Length > 0)
            {
                for (int fieldIndex = 0; fieldIndex < rowValues.Length; fieldIndex++)
                {
                    string key = _fieldNames[fieldIndex];
                    SourceFieldTypes.SetValue(key, rowValues[fieldIndex], _current);
                }
            }
        }
    }
}
