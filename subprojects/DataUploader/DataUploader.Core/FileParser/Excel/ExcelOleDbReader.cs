using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

//http://www.codeproject.com/KB/database/ReadExcel07.aspx
//http://wlasson.wordpress.com/2008/09/19/excel-and-csharp-easy/

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.Excel
{
    /// <summary>
    /// Reads worksheet data from Excel files (MS Office 97 through MS Office 2007), using the OleDb
    /// model. This model should be used for data NOT embedded in object wrappers, and when it will
    /// be useful to (1) read rows by their index, or (2) provide a SQL 'where' clause by which to filter
    /// the data.
    /// </summary>
    public class ExcelOleDbReader : FileReaderBase
    {
        /* What do the utility methods need to be?
         * string[] GetWorksheetList() - for combobox listing when an excel file is chosen for import
         * int GetWorksheetRowCount(worksheetName) - for CountAll() method
         * List<DataColumn> GetTableOrViewColumns(worksheetName, bool inferDataTypes) - for fields
         * */

        //Constants
        private const string FIELD_AUTO_PREFIX = "COLUMN_";

        //Member data
        private string _extendedPropFileVersion = "Excel {0}.0";
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
        /// <param name="excelFilePath">the complete path to the file to be read.</param>
        /// <param name="worksheetName">name of the worksheet to read data from</param>
        /// <param name="excelVersion">the version of the Excel file.</param>
        /// <param name="hasHeaderRows">
        /// provide true if the table's first row contains column names instead of data
        /// </param>
        public ExcelOleDbReader(
            string excelFilePath
            , string worksheetName
            , MSOfficeVersion excelVersion
            , bool hasHeaderRows
            )
        {
            this._hasHeaderRow = hasHeaderRows;
            this._excelFileInfo = new FileInfo(excelFilePath);
            this._currentWorksheetName = worksheetName;

            this.Initialize(excelVersion);
        }

        private void Initialize(MSOfficeVersion excelVersion)
        {
            //verify the file exists
            if (!this._excelFileInfo.Exists)
            {
                string err = string.Format("Could not find file: {0}", this._excelFileInfo.FullName);
                throw new FileNotFoundException(err);
            }

            //determine the Excel file version
            if (excelVersion == MSOfficeVersion.Unknown)
            {
                if (this._excelFileInfo.Extension.ToLower().Equals(MSOfficeConstants.EXCEL_2007_EXTENSION))
                    excelVersion = MSOfficeVersion.Offcie2007;
            }

            string version = ((int)excelVersion).ToString();
            switch (excelVersion)
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

            //ensure the worksheet actually exists
            List<string> fileWorksheetNames = this.GetWorksheetsList();
            if (fileWorksheetNames.Contains(this._currentWorksheetName))
            {
                //ensure the worksheet also contains at least one column
                List<DataColumn> columns = GetWorksheetColumns(this._currentWorksheetName, true);
                if (columns.Count > 0)
                {
                    this._fieldNames = ExtractFieldNames(columns);
                    this._queueTable = CreateTemplateTable(this._currentWorksheetName, columns);
                    this._queueTables = new DataTable[] { this._queueTable };

                    //generate the default select statement to be used by the reader
                    string columnsToFetch = GetDelimitedColumnNames(columns, ", ");
                    string selectTemplate = "select {0} from [{1}$]";
                    _sqlSelect = string.Format(selectTemplate, columnsToFetch, this._queueTable.TableName);

                    //create all the ADO.Net entities that will leverage this connection
                    _command = this.Connection.CreateCommand();
                    _command.CommandText = _sqlSelect;
                    _dataAdapter = new OleDbDataAdapter((OleDbCommand)_command);

                    this._hasValidWorksheet = true;
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

        private bool _hasHeaderRow;
        /// <summary>
        /// Indicator of presence/absence of a row with explicit field names.
        /// </summary>
        public bool HasHeaderRow
        {
            get { return _hasHeaderRow; }
        }

        private MappingType _xmlMapping = MappingType.Element;
        /// <summary>
        /// Determines the serialization mechanism for the Datatable objects that will hold the worksheet data.
        /// This must be set prior to extracting worksheet metadata during the data-extraction.
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

        private FileInfo _excelFileInfo;
        /// <summary>
        /// Read only
        /// </summary>
        public FileInfo ExcelFileInfo
        {
            get { return this._excelFileInfo; }
        }

        // instance information
        private bool _hasValidWorksheet;
        /// <summary>
        /// Read only. Set by the instance constructor if the Excel worksheet is readable.
        /// </summary>
        public bool HasValidWorksheet
        {
            get { return _hasValidWorksheet; }
        }

        private string _currentWorksheetName;
        /// <summary>
        /// The active worksheet to read from.
        /// </summary>
        public string CurrentWorksheetName
        {
            get { return _currentWorksheetName; }
            set { _currentWorksheetName = value; }
        }

        #endregion

        #region > Utilities <

        public static List<string> FetchTableNames(SourceFileInfo fileInformation)
        {
            if(fileInformation.FileType!=SourceFileType.MSExcel) return null;
            
            ExcelOleDbReader reader = null;

            try
            {
                reader = new ExcelOleDbReader(
                                                    fileInformation.FullFilePath
                                                    , null
                                                    , MSOfficeVersion.Unknown
                                                    , false
                                                );
                return reader.GetWorksheetsList();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Opens the file and reads its metadata to discover the worksheet names.
        /// </summary>
        /// <returns>An string list worksheet names found in the file.</returns>
        /// <remarks>
        /// Worksheet names may terminate in a '$' character which should be masked if used for display.
        /// This method intended only for validation of the file format and readability.
        /// </remarks>
        public List<string> GetWorksheetsList()
        {
            List<string> worksheetNames = new List<string>();
            try
            {
                if (this.Connection.State != ConnectionState.Open)
                    this.Connection.Open();

                //DataTable metaTableA = this.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //foreach (DataRow dr in metaTableA.Rows)
                //{
                //    object[] vals = dr.ItemArray;
                //}    

                //DataTable metaTableB = this.Connection.GetSchema("Tables", new string[] { null, null, null, "Table" });
                //foreach (DataRow dr in metaTableB.Rows)
                //{
                //    object[] vals = dr.ItemArray;
                //}    
                
                DataTable metaTable = this.Connection.GetSchema("Tables");
                for (int rowIndex = 0; rowIndex < metaTable.Rows.Count; rowIndex++)
                {
                    object[] vals = metaTable.Rows[rowIndex].ItemArray;
                    string sheetName = metaTable.Rows[rowIndex].ItemArray[MSOfficeConstants.TABLE_NAME_METAINDEX].ToString();
                    worksheetNames.Add(sheetName);
                }

                //Supposed the Excel file contains a worksheet name MolTable,what the returned list contains may be in one of the following two scenario :
                //1. MolTable, MolTable$
                //2. MolTable$
                // For either case, we should return MolTable always.
                List<string> temp = new List<string>();
                worksheetNames.Sort();
                foreach (string wsName in worksheetNames)
                {
                    string worksheetName = wsName;
                    //removing single quotes
                    if (worksheetName.StartsWith("\'") && worksheetName.EndsWith("\'"))
                        worksheetName = wsName.Substring(1, wsName.Length - 2);
                    if (!worksheetName.EndsWith("$"))
                    {
                        temp.Add(worksheetName);
                    }
                    else
                    {
                        string elimitedTableName = worksheetName.Remove(worksheetName.Length - 1);//remove the letter $
                        if (!temp.Contains(elimitedTableName))
                        {
                            temp.Add(elimitedTableName);
                        }
                    }
                }
                worksheetNames = temp;
                return worksheetNames;
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
        /// For a given worksheet, gets the count of rows matching the criteria.
        /// </summary>
        /// <remarks>
        /// The OleDbCommand takes the header row (or lack thereof) into account as part of the connection
        /// string initialization, so no manual adjustments for row indices are necessary.
        /// </remarks>
        /// <param name="sheetName">An Excel worksheet name</param>
        /// <returns>The number of non-header rows in the worksheet</returns>
        /// <param name="filterClause">Analog of the System.Data.Datatable's 'Select' property </param>
        public int GetWorksheetRowCount(string tableName, string filterClause)
        {
            int rowCount;
            try
            {
                if (this.Connection.State != ConnectionState.Open)
                    this.Connection.Open();

                OleDbCommand cmd = this.Connection.CreateCommand();
                string select = "select count(*) from [{0}$]";
                if (!string.IsNullOrEmpty(filterClause))
                    select += String.Format(" where {0}", filterClause);

                cmd.CommandText = string.Format(select, tableName);
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
        /// Extract the worksheet column metadata.
        /// </summary>
        /// <param name="sheetName">an Excel worksheet name</param>
        /// <param name="inferDataTypes">if true, all types default to 'System.String'</param>
        /// <returns>a list of DataColumn objects</returns>
        private List<DataColumn> GetWorksheetColumns(string worksheetName, bool inferDataTypes)
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
                cmd.CommandText = string.Format("select * from [{0}$]", worksheetName);
                OleDbDataReader columnReader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);

                DataTable schemaTable = columnReader.GetSchemaTable();
                //Coverity fix - CID 19191
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
        /// a set number of worksheet rows, starting at a given row index.
        /// </summary>
        /// <param name="templateTable">a DataTable object with the correct columns</param>
        /// <param name="da">an OleDbDataAdapter instance</param>
        /// <param name="startingIndex">the starting row index for the selection range</param>
        /// <param name="recordsToRetrieve">the number of rown to retrieve</param>
        public void GetWorksheetData(
            ref DataTable[] templateTable
            , OleDbDataAdapter da
            , int startingIndex
            , int recordsToRetrieve
            )
        {
            da.Fill(startingIndex, recordsToRetrieve, templateTable);
        }

        /// <summary>
        /// Generates a System.Data.DataTable template based on an Excel worksheet name and the derived list
        /// of worksheet columns.
        /// </summary>
        /// <param name="sheetName">the name of an Excel worksheet</param>
        /// <param name="columnsToInclude">the column list from an Excel worksheet</param>
        /// <returns></returns>
        public DataTable CreateTemplateTable(string sheetName, List<DataColumn> columnsToInclude)
        {
            DataTable template = new DataTable(sheetName);
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

        #region > Unused <

         /// <summary>
         /// Creates a Dictionary<TKey,TValue>, keyed on worksheet name, containing column information
         /// for each worksheet.
         /// </summary>
         /// <param name="inferDataTypes">
         /// To automatically detect column data-types, use true. False will result in all columns being "System.String".
         /// </param>
         /// <returns></returns>
         public Dictionary<string, List<DataColumn>> GetWorksheetsList(bool inferDataTypes)
         {
             Dictionary<string, List<DataColumn>> tableDetails = new Dictionary<string, List<DataColumn>>();

             try
             {
                 if (this.Connection.State != ConnectionState.Open)
                     this.Connection.Open();

                 DataTable metaTable = this.Connection.GetSchema("Tables", new string[] { null, null, null, "Table" });
                 foreach (DataRow metaRow in metaTable.Rows)
                 {
                     //worksheet name
                     string tableName = metaRow.ItemArray[MSOfficeConstants.TABLE_NAME_METAINDEX].ToString();

                     //skip table 'object' representations
                     //if (!tableName.EndsWith("$"))
                     //{
                     List<DataColumn> tableColumns = GetWorksheetColumns(tableName, inferDataTypes);
                     tableDetails.Add(tableName, tableColumns);
                     //}
                 }

             }
             catch
             {
                 throw;
             }
             finally
             {
                 if (this.Connection.State != ConnectionState.Closed)
                     this.Connection.Close();
             }

             return tableDetails;
         }

         /// <summary>
         /// Uses an instance of System.Data.OleDbDataReader object to retrieve worksheet data.
         /// </summary>
         /// <param name="worksheetTable">A System.Data.DataTable instance with complete metadata</param>
         /// <param name="cmd">The System.Data.OleDbCommand object applied to the source data</param>
         /// <param name="startingIndex">Index of the first record to return, previously corrected</param>
         /// <param name="recordsToRetrieve">Total number of records to return</param>
         public void PopulateTableWithReader(
             DataTable worksheetTable
             , OleDbCommand cmd
             , int startingIndex
             , int recordsToRetrieve
         )
         {
             //Populate table with DatarReader
             OleDbDataReader dataReader = cmd.ExecuteReader();
             int recordIndex = 0;

             while (dataReader.HasRows && dataReader.Read())
             {
                 object[] vals = new object[dataReader.FieldCount];
                 if (recordIndex >= startingIndex && recordIndex < (startingIndex + recordsToRetrieve))
                 {
                     if (dataReader.FieldCount <= worksheetTable.Columns.Count)
                     {
                         DataRow dr = worksheetTable.NewRow();
                         int ret = dataReader.GetValues(vals);
                         dr.ItemArray = vals;
                         worksheetTable.Rows.Add(dr);
                     }
                 }
                 recordIndex++;
             }
             dataReader.Close();
         }

         /// <summary>
         /// Underloaded call; all rows will be returned.
         /// </summary>
         /// <param name="sheetName"></param>
         /// <param name="columnsToInclude"></param>
         /// <returns></returns>
         public DataTable GetWorkSheetData(string tableName, List<DataColumn> columnsToInclude)
         {
             return GetWorksheetData(
                 tableName
                 , columnsToInclude
                 , 0
                 , this.GetWorksheetRowCount(tableName, String.Empty)
                 , String.Empty
             );
         }

         /// <summary>
         /// Fetches some or all of the worksheet's data as a DataTable.
         /// </summary>
         /// <param name="sheetName">An Excel worksheet name</param>
         /// <param name="columnsToInclude">Can be used to narrow or expand the tabular data returned.</param>
         /// <param name="startingIndex"></param>
         /// <param name="recordsToRetrieve"></param>
         /// <param name="filterClause">Analog of the System.Data.Datatable's 'Select' property </param>
         /// <returns></returns>
         /// <remarks>
         /// The System.Data.DataColumn object can be used to provide default data or even to transform a value.
         /// </remarks>
         public DataTable GetWorksheetData(
             string sheetName
             , List<DataColumn> columnsToInclude
             , int startingIndex
             , int recordsToRetrieve
             , string filterClause
             )
         {
             DataTable worksheetTable = new DataTable(sheetName);
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

             worksheetTable.Columns.AddRange(myCols.ToArray());

             string columnsToFetch = GetDelimitedColumnNames(columnsToInclude, null);

             //OleDbCommand cmd = this.Connection.CreateCommand();
             System.Data.Common.DbCommand cmd = this.Connection.CreateCommand();
             string select = "select {0} from [{1}]";
             if (!string.IsNullOrEmpty(filterClause))
                 //Add 'where' clause filter
                 select += String.Format(" where {0}", filterClause);
             cmd.CommandText = string.Format(select, columnsToFetch, sheetName);

             //To populate table with DataAdapter:
             OleDbDataAdapter da = new OleDbDataAdapter((OleDbCommand)cmd);

             //protect against negative starting index
             if (startingIndex <= 0)
                 startingIndex = 1;

             da.Fill(startingIndex - 1, recordsToRetrieve, new DataTable[] { worksheetTable });

             //Provide metadata as table extended properties
             worksheetTable.ExtendedProperties.Add("RowCount", worksheetTable.Rows.Count);
             worksheetTable.ExtendedProperties.Add("FilteredStartIndex", startingIndex);

             if (!string.IsNullOrEmpty(filterClause))
                 worksheetTable.ExtendedProperties.Add("Filter", cmd.CommandText);

             return worksheetTable;
         }

         /// <summary>
         /// Underloaded call; does not require a filter clause.
         /// </summary>
         /// <param name="sheetName"></param>
         /// <param name="columnsToInclude"></param>
         /// <param name="startingIndex"></param>
         /// <param name="recordsToRetrieve"></param>
         /// <returns></returns>
         public DataTable GetWorkSheetData(
             string tableName
             , List<DataColumn> columnsToInclude
             , int startingIndex
             , int recordsToRetrieve
             )
         {
             return GetWorksheetData(
                 tableName, columnsToInclude, startingIndex, recordsToRetrieve, String.Empty
             );
         }

         /// <summary>
         /// Replace space characters from a System.Data.DataTable's TableName proeprty.
         /// </summary>
         /// <param name="table">System.Data.DataTable object</param>
         /// <param name="spaceReplacement">The replacement character(s) that will displace spaces.</param>
         public void FixTableName(DataTable table, string spaceReplacement)
         {
             table.ExtendedProperties.Add("Caption", table.TableName);
             string tabName = table.TableName;
             tabName = tabName.Replace(String.Empty.PadRight(1), spaceReplacement);
             table.TableName = tabName;
         }

         /// <summary>
         /// Replace space characters from a System.Data.CataColumn's ColumnName proeprty.
         /// </summary>
         /// <param name="columns">System.Data.DataColumnCollection object (DataTable.Columns)</param>
         /// <param name="spaceReplacement">The replacement character(s) that will displace spaces.</param>
         public void FixColumnNames(DataColumnCollection columns, string spaceReplacement)
         {
             List<DataColumn> cols = new List<DataColumn>();
             foreach (DataColumn dc in columns)
             {
                 cols.Add(dc);
             }
             FixColumnNames(cols, spaceReplacement);
         }

         /// <summary>
         /// Replace space characters from a System.Data.CataColumn's ColumnName proeprty.
         /// </summary>
         /// <param name="columns">List<DataColumn></param>
         /// <param name="spaceReplacement">The replacement character(s) that will displace spaces.</param>
         public void FixColumnNames(List<DataColumn> columns, string spaceReplacement)
         {
             foreach (DataColumn col in columns)
             {
                 string colName = col.ColumnName;
                 col.Caption = col.ColumnName;
                 colName = colName.Replace(String.Empty.PadRight(1), spaceReplacement);
                 col.ColumnName = colName;
             }
         }

         /// <summary>
         /// Provides a delegate through which the System.Data.OleDbConnection may send error messages.
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="args"></param>
         private void _Message_Event(object sender, OleDbInfoMessageEventArgs args)
         {
             throw new ApplicationException(
                 string.Format(
                     "ExcelReader has encountered an error: {}", args.Message
                 )
             );
         }

        #endregion

        #endregion

        #region > IFileReader<CSVSourceRecord> Members <

        public override int CountAll()
        {
            this._totalRecordCount = this.GetWorksheetRowCount(this._currentWorksheetName, null);
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
                if (_queueTable.Rows.Count == 0 ||
                    _currentRecordIndex < _readChunkStartIndex || _currentRecordIndex > _readChunkStartIndex + _readAheadSize - 1)
                {
                    this._queueTable.Rows.Clear();
                    this.GetWorksheetData(ref _queueTables, _dataAdapter, _currentRecordIndex, chunkSize); 
                    _readChunkStartIndex = _currentRecordIndex;
                }

                rowObjectValues = this._queueTable.Rows[_currentRecordIndex - _readChunkStartIndex].ItemArray;

                _currentRecordIndex++;

                this.ExtractRecord(rowObjectValues);

                _parsedRecordCount++;
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
            if (recordIndex <= this.RecordCount)
                _currentRecordIndex = recordIndex;
            else
            {
                _currentRecordIndex = 0;

                throw new IndexOutOfRangeException();
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
        /// connection to the Excel worksheet.
        /// </summary>
        /// <returns>a new OleDbConnection instance</returns>
        private OleDbConnection InitializeConnection()
        {
            string connStr = string.Empty;
            string extendedPropsRaw = "{0};HDR={1};";
            string headerIndicator = "No";
            if (_hasHeaderRow)
                headerIndicator = "Yes";

            string extendedProps = string.Format(extendedPropsRaw, _extendedPropFileVersion, headerIndicator);

            _connectionBuilder = new OleDbConnectionStringBuilder();
            _connectionBuilder.Provider = _provider;
            _connectionBuilder.DataSource = _excelFileInfo.FullName;
            _connectionBuilder.Add("extended properties", extendedProps);
            _connectionBuilder.Add("mode", 8);
            _connectionBuilder.Add("prompt", 4);
            //_connectionBuilder.Add("exclusive", 1);

            //finalize the connection string
            connStr = _connectionBuilder.ToString();
            OleDbConnection conn = new OleDbConnection(connStr);

            //return the connection
            return conn;
        }

        /// <summary>
        /// Generates a list of strings representing the DataColumns. If the worksheet does not have
        /// a header row, the auto-generated column names ("F1", "F2" and so on) will be replaced
        /// using an integnal algorithm.
        /// </summary>
        /// <param name="columns">the list of DataColumn objects from the worksheet's schema</param>
        /// <returns>a list of strings to be used as column names</returns>
        private List<string> ExtractFieldNames(List<DataColumn> columns)
        {
            //This will always return column names, but in the case of auto-generated names,
            // we want to override the values.
            List<string> columnNames = new List<string>();
            foreach (DataColumn dc in columns)
            {
                if (this._hasHeaderRow)
                    columnNames.Add(dc.ColumnName);
                else
                {
                    char padChar = "0".ToCharArray()[0];
                    columnNames.Add(FIELD_AUTO_PREFIX + (columns.IndexOf(dc) + 1).ToString().PadLeft(3, padChar));
                }
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
