using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    /// <summary>
    /// <see cref="InputObject"/> for MDB databases
    /// </summary>
    class InputObjectMdb : InputObject
    {
        #region data

        private OleDbConnection _oOleDbConnection;

        private string _strDb = string.Empty;

        #endregion
        
        #region property overrides

        public override string Db
        {
            get
            {
                return (_strDb == null) ? string.Empty : _strDb;
            }
            set
            {
                _strDb = value;
                InitializeOledbConnection();
            }
        } // Db

        #endregion

        #region constructors

        public InputObjectMdb()
        {
            Filter = "Microsoft Access databases (*.mdb)|*.mdb";
            _oOleDbConnection = new OleDbConnection();
            IsValid = false;
            try
            {
                do
                {
                    string strCLSID = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("Microsoft.Jet.OLEDB.4.0\\CLSID").GetValue(string.Empty).ToString();
                    //Coveri fix- CID 13121
                    if (!string.IsNullOrEmpty(strCLSID))
                    {
                        string strInprocServer32 = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID\\" + strCLSID + "\\InprocServer32").GetValue(string.Empty).ToString();
                        if (System.IO.File.Exists(strInprocServer32) == false)
                        {
                            break;
                        }
                        IsValid = true;
                    }
                } while (false);
            }
            catch
            {
                ;
            }
            if (IsValid == false)
            {
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, -1, "Microsoft.Jet.OLEDB.4.0 not available.");
            }
            return;
        }

        #endregion

        #region methods

        private bool InitializeOledbConnection()
        {
            _oOleDbConnection = new OleDbConnection();
            string strConnectionString;
            strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0";
            strConnectionString += ";";
            strConnectionString += "Data Source=" + Db;
            strConnectionString += ";";
            strConnectionString += "Mode=" + (0 + 8);   // adModeRead + adModeShareDenyWrite
            strConnectionString += ";";
            strConnectionString += "Prompt=" + 4;   // adPromptNever
            _oOleDbConnection.ConnectionString = strConnectionString;

            return true;
        }

        private bool OpenConnection()
        {
            bool connectionOpened = false;

            if (_oOleDbConnection != null && _oOleDbConnection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    _oOleDbConnection.Open();
                    connectionOpened = true;
                }
                catch (Exception ex)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Unable to open the database: " + ex.Message);
                }
            }

            return connectionOpened;
        }

        private bool CloseConnection()
        {
            bool connectionClosed = false;

            if (_oOleDbConnection != null && _oOleDbConnection.State != System.Data.ConnectionState.Closed)
            {
                _oOleDbConnection.Close();
                connectionClosed = true;

            }
            return connectionClosed;
        }

        public override void CloseDb()
        {
            CloseConnection();
        } // CloseDb()

        public override bool OpenDb()
        {
            ClearMessages();
            do
            {
                if (OpenConnection() == false)
                {
                    break;
                }

                // Build TableList
                {
                    ClearTableList();
                    System.Data.DataTable dataTable;
#if DEBUG
                    { // GetSchema
                        dataTable = _oOleDbConnection.GetSchema();
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
//                            Console.WriteLine(dataRow.ItemArray[0].ToString());
                        }
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            for (int nCol = 0; nCol < dataTable.Columns.Count; nCol++)
                            {
                                DataColumn dataColumn = dataTable.Columns[nCol];
                                string strColumnName = dataColumn.ColumnName;
                                string strString = dataRow.ItemArray[nCol].ToString();
                                strString += string.Empty;    // Debugging
                            }
                            int nFoo = 0; nFoo++;   // Debugging
                        }
                    }
                    {
                        DataTable oDataTable = _oOleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new Object[] { null, null, null });
// TABLE_CATALOG
// TABLE_SCHEMA
// TABLE_NAME
// COLUMN_NAME
// COLUMN_GUID
// COLUMN_PROPID
// ORDINAL
// PK_NAME
                        foreach (DataColumn dataColumn in oDataTable.Columns)
                        {
//                            Console.WriteLine(dataColumn.ColumnName);
                        }
                        foreach (DataRow dataRow in oDataTable.Rows)
                        {
//                            Console.WriteLine(dataRow.ItemArray[3].ToString());
                        }
                        int nFoo = 0; nFoo++;   // Debugging
                    }
                    { // GetSchema
                        dataTable = _oOleDbConnection.GetSchema("Columns");
                        foreach (DataColumn dataColumn in dataTable.Columns)
                        {
//                            Console.WriteLine(dataColumn.ColumnName);
                        }
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
//                            Console.WriteLine(dataRow.ItemArray[0].ToString());
                        }
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            for (int nCol = 0; nCol < dataTable.Columns.Count; nCol++)
                            {
                                DataColumn dataColumn = dataTable.Columns[nCol];
                                string strColumnName = dataColumn.ColumnName;
                                string strString = dataRow.ItemArray[nCol].ToString();
                                strString += string.Empty;   // Debugging
                            }
                            int nFoo = 0; nFoo++;   // Debugging
                        }
                    }
#endif
                    {
                        string[] strRestrictions = { null, null, null, null };    // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
                        dataTable = _oOleDbConnection.GetSchema("Tables", strRestrictions);
                        DataColumn dColumnTableName = dataTable.Columns["TABLE_NAME"];
                        DataColumn dColumnTableType = dataTable.Columns["TABLE_TYPE"];
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            if (dataRow[dColumnTableType].ToString() == "TABLE")
                            {
                                string strTableName = dataRow[dColumnTableName].ToString();
                                AddTableToTableList(strTableName);
                            }
                        }
                    }
                    {
                        string[] strRestrictions = { null, null, null };    // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME
                        dataTable = _oOleDbConnection.GetSchema("Views", strRestrictions);
                        DataColumn dColumnTableName = dataTable.Columns["TABLE_NAME"];
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            {
                                string strTableName = dataRow[dColumnTableName].ToString();
                                AddTableToTableList(strTableName);
                            }
                        }
                    }
                    if (Tables == 0)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "There are no tables or views in the database");
                        break;  // Error
                    }
                } // Build TableList

            } while (false);
            return HasMessages;
        } // OpenDb()

        public override bool CloseTable()
        {
            ClearMessages();
            do
            {
                if (_oOleDbConnection.State != System.Data.ConnectionState.Open)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
                    break;  // Error
                }
                if (Table.Length == 0)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "No table is selected");
                    break;  // Error
                }
                base.CloseTable();
            } while (false);
            return HasMessages;
        } // CloseTable()

        public override bool OpenTable()
        {
            ClearMessages();
            do
            {
                // Open?
                if (_oOleDbConnection.State != System.Data.ConnectionState.Open)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
                    break;  // Error
                }
                // Table?
                if (Table.Length == 0)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "No table is selected");
                    break;  // Error
                }
                // Count
                if (RecordsUnknown) {
                    OleDbCommand oOleDbCommand = new OleDbCommand();
                    oOleDbCommand.Connection = _oOleDbConnection;
                    oOleDbCommand.CommandType = CommandType.Text;
                    oOleDbCommand.CommandText = "SELECT count(*) FROM " + Table;
                    Records = Convert.ToInt32(oOleDbCommand.ExecuteScalar().ToString());    // Exact
                    Record = 0;
                }
                // Find Primary key
#if FUTURE
                {
                    string[] strRestrictions = { null, null, Table };    // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME
                    DataTable dataTable = _oOleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, strRestrictions);
                    if (dataTable.Rows.Count == 0)
                    {
                        Console.Write("There is no primary key in the '" + Table + "' table");
                    }
                    else if (dataTable.Rows.Count > 1)
                    {
                        Console.Write("There are multiple primary keys in the '" + Table + "' table");
                    }
                    // TABLE_CATALOG
                    // TABLE_SCHEMA
                    // TABLE_NAME
                    // COLUMN_NAME
                    // COLUMN_GUID
                    // COLUMN_PROPID
                    // ORDINAL
                    // PK_NAME
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        Console.Write(dataRow.ItemArray[3]);
                    }
                }
#endif
                // Build FieldList and TypeList
                if (InputFieldSpec.Length == 0)
                {
                    System.Data.DataTable dataTable;
                    string[] strRestrictions = { null, null, Table, null };    // PROCEDURE_CATALOG, PROCEDURE_SCHEMA, PROCEDURE_NAME, COLUMN_NAME
                    dataTable = _oOleDbConnection.GetSchema("Columns", strRestrictions);
                    if (dataTable.Rows.Count == 0)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "There are no fields in the '" + Table + "' table");
                        break;  // Error
                    }
                    DataColumn dColumnColumnName = dataTable.Columns["COLUMN_NAME"];
                    DataColumn dColumnDataType = dataTable.Columns["DATA_TYPE"];
                    {
                        COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                        oCOEXmlTextWriter.WriteStartElement("fieldlist");
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            string strColumnName = dataRow[dColumnColumnName].ToString();
                            int nDataType = Convert.ToInt32(dataRow[dColumnDataType].ToString());
                            OleDbType oleDbType = (OleDbType)nDataType;
                            string strOleDbType = oleDbType.ToString();
#if RESEARCH_OLEDBTYPE
                        switch (oleDbType)
                        {
                            case OleDbType.Empty: //	No value (DBTYPE_EMPTY).
                                break;  // (none)
                            case OleDbType.Boolean: //	A Boolean value (DBTYPE_BOOL). This maps to Boolean.
                                break;  // Boolean
                            case OleDbType.Binary: //	A stream of binary data (DBTYPE_BYTES). This maps to an Array of type Byte.
                            case OleDbType.UnsignedTinyInt: //	A 8-bit unsigned integer (DBTYPE_UI1). This maps to Byte.
                                break;  // Byte
                            case OleDbType.LongVarBinary: //	A long binary value (OleDbParameter only). This maps to an Array of type Byte.
                                break;  // Byte[]
                            case OleDbType.Date: //	Date data, stored as a double (DBTYPE_DATE). The whole portion is the number of days since December 30, 1899, and the fractional portion is a fraction of a day. This maps to DateTime.
                            case OleDbType.DBDate: //	Date data in the format yyyymmdd (DBTYPE_DBDATE). This maps to DateTime.
                            case OleDbType.DBTimeStamp: //	Data and time data in the format yyyymmddhhmmss (DBTYPE_DBTIMESTAMP). This maps to DateTime.
                            case OleDbType.Filetime: //	A 64-bit unsigned integer representing the number of 100-nanosecond intervals since January 1, 1601 (DBTYPE_FILETIME). This maps to DateTime.
                                break;  // DateTime
                            case OleDbType.Currency: //	A currency value ranging from -2 63 (or -922,337,203,685,477.5808) to 2 63 -1 (or +922,337,203,685,477.5807) with an accuracy to a ten-thousandth of a currency unit (DBTYPE_CY). This maps to Decimal.
                            case OleDbType.Decimal: //	A fixed precision and scale numeric value between -10 38 -1 and 10 38 -1 (DBTYPE_DECIMAL). This maps to Decimal.
                            case OleDbType.Numeric: //	An exact numeric value with a fixed precision and scale (DBTYPE_NUMERIC). This maps to Decimal.
                            case OleDbType.VarNumeric: //	A variable-length numeric value (OleDbParameter only). This maps to Decimal.
                                break;  // Decimal
                            case OleDbType.Double: //	A floating-point number within the range of -1.79E +308 through 1.79E +308 (DBTYPE_R8). This maps to Double.
                                break;  // Double
                            case OleDbType.Error: //	A 32-bit error code (DBTYPE_ERROR). This maps to Exception.
                                break;  // Exception
                            case OleDbType.Guid: //	A globally unique identifier (or GUID) (DBTYPE_GUID). This maps to Guid.
                                break;  // Guid
                            case OleDbType.SmallInt: //	A 16-bit signed integer (DBTYPE_I2). This maps to Int16.
                                break;  // Int16
                            case OleDbType.Integer: //	A 32-bit signed integer (DBTYPE_I4). This maps to Int32.
                                break;  // Int32
                            case OleDbType.BigInt: //	A 64-bit signed integer (DBTYPE_I8). This maps to Int64.
                                break;  // Int64
                            case OleDbType.IDispatch: //	A pointer to an IDispatch interface (DBTYPE_IDISPATCH). This maps to Object.
                            case OleDbType.IUnknown: //	A pointer to an IUnknown interface (DBTYPE_UNKNOWN). This maps to Object.
                            case OleDbType.PropVariant: //	An automation PROPVARIANT (DBTYPE_PROP_VARIANT). This maps to Object.
                            case OleDbType.VarBinary: //	A variable-length stream of binary data (OleDbParameter only). This maps to an Array of type Byte.
                            case OleDbType.Variant: //	A special data type that can contain numeric, string, binary, or date data, and also the special values Empty and Null (DBTYPE_VARIANT). This type is assumed if no other is specified. This maps to Object.
                                break;  // Object
                            case OleDbType.TinyInt: //	A 8-bit signed integer (DBTYPE_I1). This maps to SByte.
                                break;  // SByte
                            case OleDbType.Single: //	A floating-point number within the range of -3.40E +38 through 3.40E +38 (DBTYPE_R4). This maps to Single.
                                break;  // Single
                            case OleDbType.BSTR: //	A null-terminated character string of Unicode characters (DBTYPE_BSTR). This maps to String.
                            case OleDbType.Char: //	A character string (DBTYPE_STR). This maps to String.
                            case OleDbType.LongVarChar: //	A long string value (OleDbParameter only). This maps to String.
                            case OleDbType.LongVarWChar: //	A long null-terminated Unicode string value (OleDbParameter only). This maps to String.
                            case OleDbType.VarChar: //	A variable-length stream of non-Unicode characters (OleDbParameter only). This maps to String.
                            case OleDbType.VarWChar: //	A variable-length, null-terminated stream of Unicode characters (OleDbParameter only). This maps to String.
                            case OleDbType.WChar: //	A null-terminated stream of Unicode characters (DBTYPE_WSTR). This maps to String.
                                break;  // String
                            case OleDbType.DBTime: //	Time data in the format hhmmss (DBTYPE_DBTIME). This maps to TimeSpan.
                                break;  // TimeSpan
                            case OleDbType.UnsignedBigInt: //	A 64-bit unsigned integer (DBTYPE_UI8). This maps to UInt64.
                                break;  // UInt64
                            case OleDbType.UnsignedInt: //	A 32-bit unsigned integer (DBTYPE_UI4). This maps to UInt32.
                                break;  // UInt32
                            case OleDbType.UnsignedSmallInt: //	A 16-bit unsigned integer (DBTYPE_UI2). This maps to UInt16.
                                break;  // Uint16
                            default:
                                break;  // OOPS
                        } // switch (oleDbType)
#endif
                            oCOEXmlTextWriter.WriteStartElement("field");
                            oCOEXmlTextWriter.WriteAttributeString("dbname", strColumnName);
                            oCOEXmlTextWriter.WriteAttributeString("dbtype", strOleDbType);
                            oCOEXmlTextWriter.WriteAttributeString("dbtypereadonly", "true");
                            oCOEXmlTextWriter.WriteEndElement();
                        } // foreach (DataRow dataRow in dataTable.Rows)
                        oCOEXmlTextWriter.WriteEndElement();
                        InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                        oCOEXmlTextWriter.Close();
                    }
                } // if (InputFieldSpec.Length == 0)
            } while (false);
            return HasMessages;
        } // OpenTable()

        private OleDbDataReader _oOleDbDataReader;
        public override bool CloseDataSet()
        {
            if (_oOleDbDataReader != null)
            {
                _oOleDbDataReader.Close();
                _oOleDbDataReader = null;
            }
            base.CloseDataSet();
            return HasMessages;
        } // CloseDataSet()

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            DataSetForJob = new DataSet(Table + "List");
            DataSetForJob.Tables.Add(Table);
            DataTable oDataTable = DataSetForJob.Tables[0];
            Value = 0;
            // Set up the command
            OleDbCommand oOleDbCommand = new OleDbCommand();
            oOleDbCommand.Connection = _oOleDbConnection;
            oOleDbCommand.CommandType = CommandType.Text;
            string strFields = string.Empty;
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(InputFieldSpecMapped); // oXmlDocument.LoadXml(InputFieldSpec)
                XmlNode oXmlNodeFieldlist = oXmlDocument.DocumentElement;
                foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                {
                    if (strFields.Length > 0) strFields += ",";
                    string strDbName = oXmlNodeField.Attributes["dbname"].Value;
                    string strName = (oXmlNodeField.Attributes["name"] != null) ? oXmlNodeField.Attributes["name"].Value.ToString() : strDbName;
                    strFields += "[" + strDbName + "]";
                    if (strName != strDbName)
                    {
                        strFields += " AS " + "[" + oXmlNodeField.Attributes["name"].Value + "]";
                    }
                    // Add DataColumns
                    DataColumn oDataColumn = new DataColumn();
                    oDataColumn.Caption = strName;
                    oDataColumn.ColumnName = strDbName;
                    oDataTable.Columns.Add(oDataColumn);
                }
            }
            oOleDbCommand.CommandText = "SELECT " + strFields + " FROM " + Table;   // WJC parametric!
            if (InputFieldSort.Length > 0)
            {
                string strOrderBy = InputFieldSortSql;
                if (strOrderBy.Length > 0)
                {
                    oOleDbCommand.CommandText = oOleDbCommand.CommandText + " ORDER BY " + strOrderBy;
                }
            }
            // Get the reader
            _oOleDbDataReader = oOleDbCommand.ExecuteReader();
            // Set DataColumn types
            if (_oOleDbDataReader != null)
            {
                for (int nColumn = 0; nColumn < _oOleDbDataReader.FieldCount; nColumn++)
                {
                    oDataTable.Columns[NameGetDbName(_oOleDbDataReader.GetName(nColumn))].DataType = _oOleDbDataReader.GetFieldType(nColumn);
                }
            }
            Ph.Minimum = Minimum = Value = 0;
            Ph.Maximum = Maximum = vnStart;
            Ph.CancelConfirmation = "If you stop this operation then no records will be processed";
            Ph.ProgressSection(delegate() /* InputObjectMdb::OpenDataSet(Record, Cancel) */
            {
                for (Record = 0; Record < vnStart; Record++)
                {
                    if (Ph.CancellationPending)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "The user stopped the operation before reaching the start record");
                        break;
                    }
                    Ph.Value = Value = Record;
                    Ph.StatusText = "Skipping mdb record " + (1 + Record) + " of " + vnStart;
                    if (_oOleDbDataReader.Read() == false)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "End of file occurred before reaching the start record");
                        break;  // End of dataset
                    }
                }
            });
            Minimum = Value = vnStart;
            Maximum = ((vcLimit != int.MaxValue) ? Minimum : 0) + vcLimit;
            if (Maximum > Records) Maximum = Records;
            return HasMessages;
        } // OpenDataSet()

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = DataSetForJob;
            DataTable oDataTable = DataSetForJob.Tables[0];
#if OLDWAY
                OleDbDataAdapter oOleDbDataAdapter = new OleDbDataAdapter();
                oOleDbDataAdapter.TableMappings.Add("Table", Table);
                oOleDbDataAdapter.SelectCommand = oOleDbCommand;
                oOleDbDataAdapter.Fill(oDataSet); // WJC Note oOleDbDataAdapter.Fill(oDataSet, 0, 10, Table); for 1st 10 rows
#endif
            // Perform the read and data transfer
            while ((oDataTable.Rows.Count < vcLimit) && _oOleDbDataReader.Read())
            {
                DataRow oDataRow = oDataTable.NewRow();
                for (int nColumn = 0; nColumn < _oOleDbDataReader.FieldCount; nColumn++)
                {
                    oDataRow[nColumn] = _oOleDbDataReader[nColumn];
                }
                oDataTable.Rows.Add(oDataRow);
            }
            Value += oDataTable.Rows.Count;
            return HasMessages;
        } // ReadDataSet()

        protected override DataSet ReadDataSetForPreview()
        {
            // DataSet
            DataSet oDataSet = new DataSet(Table + "List");
            // DataTable
            oDataSet.Tables.Add(Table);
            DataTable oDataTable = oDataSet.Tables[0];
            // Set up the command
            OleDbCommand oOleDbCommand = new OleDbCommand();
            oOleDbCommand.Connection = _oOleDbConnection;
            oOleDbCommand.CommandType = CommandType.Text;
            oOleDbCommand.CommandText = "SELECT * FROM " + Table;
            if (InputFieldSort.Length > 0)
            {
                string strOrderBy = InputFieldSortSql;
                if (strOrderBy.Length > 0)
                {
                    oOleDbCommand.CommandText = oOleDbCommand.CommandText + " ORDER BY " + strOrderBy;
                }
            }
#if OLDWAY
            OleDbDataAdapter oOleDbDataAdapter = new OleDbDataAdapter();
            oOleDbDataAdapter.TableMappings.Add("Table", Table);
            oOleDbDataAdapter.SelectCommand = oOleDbCommand;
            oOleDbDataAdapter.Fill(oDataSet);
#endif
            // Get the reader
            OleDbDataReader oOleDbDataReader = oOleDbCommand.ExecuteReader();
            // Set up DataColumns
            if (oOleDbDataReader == null)
                throw new System.NullReferenceException();
            if (oDataTable.Columns.Count == 0)
            {
                for (int nColumn = 0; nColumn < oOleDbDataReader.FieldCount; nColumn++)
                {
                    DataColumn oDataColumn = new DataColumn();
                    oDataColumn.ColumnName = oOleDbDataReader.GetName(nColumn);
                    oDataColumn.DataType = oOleDbDataReader.GetFieldType(nColumn);
                    oDataTable.Columns.Add(oDataColumn);
                }
            }
            // Perform the read and data transfer
            while (oOleDbDataReader.Read())
            {
                DataRow oDataRow = oDataSet.Tables[0].NewRow();
                for (int nColumn = 0; nColumn < oOleDbDataReader.FieldCount; nColumn++)
                {
                    oDataRow[nColumn] = oOleDbDataReader[nColumn];
                }
                oDataTable.Rows.Add(oDataRow);
            }
            return oDataSet;
        } // ReadDataSetForPreview()

        #endregion

    } // class InputObjectMdb
}
