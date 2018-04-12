using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    class InputObjectXl : InputObject
    {
        private static void Zap(ref Object o)
        {
            if (o != null)
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(o);
                o = null;
            }
            return;
        }
        /****************************************************************************************
         * Application
        ****************************************************************************************/
        class Application
        {
            private Object _oApplication;  // "System.__ComObject"
            public Object ComObject
            {
                get
                {
                    return _oApplication;
                }
                private set
                {
                    Zap(ref _oApplication);
                    _oApplication = value;
                    return;
                }
            } // ComObject
            private int _Id = 0;

            public Application()
            {
                Environment.SetEnvironmentVariable("DataLoader", System.Diagnostics.Process.GetCurrentProcess().Id.ToString(), EnvironmentVariableTarget.Process);
                Type typeExcel = System.Type.GetTypeFromProgID("Excel.Application");
                if (typeExcel != null)
                {
                    List<int> listIds = new List<int>();
                    {
                        System.Diagnostics.Process[] pExcels = System.Diagnostics.Process.GetProcessesByName("Excel");
                        foreach (System.Diagnostics.Process pExcel in pExcels)
                        {
                            listIds.Add(pExcel.Id);
                        }
                    }
                    //For CSBR-117824 bug solved
                    // to solve this bug we skipping "Excel apllcation Dll/Instance/"progID" to load while runtime. 
                    // This code skip the loading of Excel Dll(Microsoft.Office.Interop.Excel.Dll)in Dataloader.
                    // Client may not have the MSExcel in their system.
                    // Here we adding some code to avoid loading of Excel and providing NULL to it.


                    if (typeExcel == System.Type.GetTypeFromProgID("Excel.Application"))
                    {
                        typeExcel = null;
                        //Here we can provide some Message to Client, if need. 
                    }
                    //else
                    //{

                    //ComObject = Activator.CreateInstance(typeExcel);
                    //{
                    //    System.Diagnostics.Process[] pExcels = System.Diagnostics.Process.GetProcessesByName("Excel");
                    //    foreach (System.Diagnostics.Process pExcel in pExcels)
                    //    {
                    //        if (listIds.Contains(pExcel.Id) == false)
                    //        {
                    //            _Id = pExcel.Id;
                    //            break;  // Assuming that exactly one Excel process started since we built the list
                    //        }
                    //    }
                    //}
                    //}
                }
                return;
            } // Application()
            ~Application()
            {
                ComObject = null;
                if (_Id != 0)
                {
                    try
                    {
                        System.Diagnostics.Process pExcel = System.Diagnostics.Process.GetProcessById(_Id);
                        if (pExcel != null)
                        {
                            pExcel.Kill();
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
                return;
            }
            public Workbooks Workbooks {
                get{
                    return new Workbooks(this);
                }
            } // Workbooks()
        } // class Application

        /****************************************************************************************
         * Workbooks
        ****************************************************************************************/
        class Workbooks
        {
            private Object _oWorkbooks;  // "System.__ComObject"
            public Object ComObject
            {
                get
                {
                    return _oWorkbooks;
                }
                private set
                {
                    Zap(ref _oWorkbooks);
                    _oWorkbooks = value;
                    return;
                }
            } // ComObject
            public int Count
            {
                get
                {
                    return (int)ComObject.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, ComObject, null);
                }
            }
            private List<Workbook> _listItem;
            public List<Workbook> Item
            {
                get
                {
                    return _listItem;
                }
                private set
                {
                    _listItem = value;
                    return;
                }
            } // Item
            private string _name;
            public string Name
            {
                get
                {
                    return _name;
                }
                private set
                {
                    _name = value;
                    return;
                }
            } // Name
            public Workbooks(Application oApplication)
            {
                ComObject = oApplication.ComObject.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, oApplication.ComObject, null);
                Item = new List<Workbook>();
                return;
            } // Workbooks()
            ~Workbooks()
            {
                if (Item != null)
                {
                    for (int n = 0; n < Item.Count; n++)
                    {
                        Item[n] = null;
                    }
                    Item.Clear();
                    Item = null;
                }
                ComObject = null;
                return;
            }
            public void Close()
            {
                foreach (Workbook oWorkbook in Item)
                {
                    oWorkbook.Close();
                }
                Item.Clear();
                ComObject.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, ComObject, null);
                return;
            } // Close()
            public void Open(string strFilename)
            {
                Name = strFilename;
                // Workbook Open(Filename As String, [UpdateLinks], [ReadOnly], [Format], [Password], [WriteResPassword], [IgnoreReadOnlyRecommended], [Origin], [Delimiter], [Editable], [Notify], [Converter], [AddToMru], [Local], [CorruptLoad])
                ComObject.GetType().InvokeMember("Open", BindingFlags.InvokeMethod, null, ComObject, new Object[] { Name, false, true });
                Item.Add(new Workbook(this, Count));
                return;
            } // Open()
        } // class Workbooks

        /****************************************************************************************
         * Workbook
        ****************************************************************************************/
        class Workbook
        {
            private Object _oWorkbook;   // "System.__ComObject"
            public Object ComObject
            {
                get
                {
                    return _oWorkbook;
                }
                private set
                {
                    Zap(ref _oWorkbook);
                    _oWorkbook = value;
                    return;
                }
            } // ComObject
            private Worksheets _oWorksheets;
            public Worksheets Worksheets
            {
                get
                {
                    return _oWorksheets;
                }
                private set
                {
                    _oWorksheets = value;
                    return;
                }
            } // Worksheets
            public void Close()
            {
                ComObject.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, ComObject, null);
                return;
            }
            public Workbook(Workbooks oWorkbooks, int nBook)
            {
                ComObject = oWorkbooks.ComObject.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, oWorkbooks.ComObject, new Object[] { nBook });
                Worksheets = new Worksheets(this);
                return;
            }
            ~Workbook()
            {
                if (Worksheets != null)
                {
                    Worksheets = null;
                }
                ComObject = null;
                return;
            }
        } // class Workbook

        /****************************************************************************************
         * Worksheets
        ****************************************************************************************/
        class Worksheets
        {
            private Object _oWorksheets; // "System.__ComObject"
            public Object ComObject {
                get {
                    return _oWorksheets;
                }
                private set
                {
                    Zap(ref _oWorksheets);
                    _oWorksheets = value;
                    return;
                }
            } // ComObject
            public int Count
            {
                get
                {
                    return (int)ComObject.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, ComObject, null);
                }
            }
            private List<Worksheet> _listItem;
            public List<Worksheet> Item
            {
                get
                {
                    return _listItem;
                }
                private set
                {
                    _listItem = value;
                    return;
                }
            } // Item
            private Dictionary<string, int> _dictItem;
            public int KeyToIndex(string strKey)
            {
                return _dictItem[strKey];
            }
            public Worksheets(Workbook oWorkbook)
            {
                ComObject = oWorkbook.ComObject.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, oWorkbook.ComObject, null);
                Item = new List<Worksheet>();
                _dictItem = new Dictionary<string, int>();
                for (int nWorksheet = 1; nWorksheet <= Count; nWorksheet++)
                {
                    Item.Add(new Worksheet(this, nWorksheet));
                    _dictItem.Add(Item[Item.Count - 1].Name, Item.Count - 1);
                }
                return;
            }
            ~Worksheets()
            {
                if (Item != null)
                {
                    for (int n = 0; n < Item.Count; n++)
                    {
                        Item[n] = null;
                    }
                    Item.Clear();
                    Item = null;
                }
                ComObject = null;
                return;
            }
        } // class Worksheets

        /****************************************************************************************
         * Worksheet
        ****************************************************************************************/
        class Worksheet
        {
            private Object _oWorksheet;  // "System.__ComObject"
            public Object ComObject
            {
                get
                {
                    return _oWorksheet;
                }
                private set
                {
                    Zap(ref _oWorksheet);
                    _oWorksheet = value;
                    return;
                }
            } // ComObject
            private Range _oDataRange;
            public Range DataRange
            {
                get
                {
                    return _oDataRange;
                }
                private set
                {
                    _oDataRange = value;
                    return;
                }
            } // DataRange
            private bool _bHeader;
            public bool Header
            {
                get
                {
                    return _bHeader;
                }
                set
                {
                    _bHeader = value;
                    if (Header && (UsedRange.Rows > 0))
                    {
                        DataRange = UsedRange.Resize(UsedRange.Rows - 1, UsedRange.Columns).Offset(1, 0);
                    }
                    else
                    {
                        DataRange = UsedRange;
                    }
                    return;
                }
            } // Header
            private Range _oUsedRange;
            public Range UsedRange
            {
                get
                {
                    return _oUsedRange;
                }
                private set
                {
                    _oUsedRange = value;
                    return;
                }
            } // UsedRange
            public string Name
            {
                get
                {
                    return (string)ComObject.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, ComObject, null);
                }
            }
            public Worksheet(Worksheets oWorksheets, int nWorksheet)
            {
                ComObject = oWorksheets.ComObject.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, oWorksheets.ComObject, new Object[] { nWorksheet });
                UsedRange = new Range(ComObject.GetType().InvokeMember("UsedRange", BindingFlags.GetProperty, null, ComObject, null));
                Header = false;
                return;
            }
            ~Worksheet()
            {
                if (UsedRange != null)
                {
                    UsedRange = null;
                }
                if (DataRange != null)
                {
                    DataRange = null;
                }
                ComObject = null;
                return;
            }
        } // class Worksheet

        /****************************************************************************************
         * Range
        ****************************************************************************************/
        class Range
        {
            private Object _oRange;  // "System.__ComObject"
            public Object ComObject
            {
                get
                {
                    return _oRange;
                }
                private set
                {
                    Zap(ref _oRange);
                    _oRange = value;
                    return;
                }
            } // ComObject
            public string CommentText(int RowIndex, int ColumnIndex)
            {
                string strRet = null;
                Object oCell = ComObject.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, ComObject, new Object[] { RowIndex, ColumnIndex });
                Object oComment = oCell.GetType().InvokeMember("Comment", BindingFlags.GetProperty, null, oCell, null);
                if (oComment != null)
                {
                    strRet = (string)oComment.GetType().InvokeMember("Text", System.Reflection.BindingFlags.InvokeMethod, null, oComment, null); ;
                    Zap(ref oComment);
                }
                Zap(ref oCell);
                return strRet;
            } // CommentText()
            private Object[,] _oValue;
            private Object[,] Value
            {
                get
                {
                    if (_oValue == null)
                    {
                        try
                        {
                            _oValue = (Object[,])ComObject.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, ComObject, null);
                        }
                        catch
                        {
                            ;
                        }
                    }
                    return _oValue;
                }
                set
                {
                    _oValue = value;
                    return;
                }
            } // Value
            public int Columns
            {
                get
                {
                    return (Value != null) ? Value.GetUpperBound(1) : 0;
                }
            }
            public string Item(int nRow, int nCol)
            {
                return (Value[nRow, nCol] != null) ? Value[nRow, nCol].ToString() : string.Empty;
            }
            public int Rows
            {
                get
                {
                    return (Value != null) ? Value.GetUpperBound(0) : 0;
                }
            }
            public Range Offset(int RowOffset, int ColumnOffset)
            {
                Object[] oArg = { RowOffset, ColumnOffset };
                return new Range((Object)ComObject.GetType().InvokeMember("Offset", BindingFlags.GetProperty, null, ComObject, oArg));
            }
            public Range Resize(int RowSize, int ColumnSize)
            {
                Object[] oArg = { RowSize, ColumnSize };
                return new Range((Object)ComObject.GetType().InvokeMember("Resize", BindingFlags.GetProperty, null, ComObject, oArg));
            }
#if EXPERIMENT
            public void Sort()
            {
                Range rColumns = new Range((Object)oRange.GetType().InvokeMember("Columns", BindingFlags.GetProperty, null, oRange, null));
                Object[] oArg = {
                    "A",   // Key1	Optional	Variant	Specifies the first sort field, either as a range name (String) or Range object; determines the values to be sorted.
                    1,              // XlSortOrder.xlAscending          // Order1	Optional	XlSortOrder	Determines the sort order for the values specified in Key1.
                    Type.Missing,   // Key2	Optional	Variant	Second sort field; cannot be used when sorting a pivot table.
                    Type.Missing,   // Type	Optional	Variant	Specified which elements are to be sorted.
                    1,              // XlSortOrder.xlAscending          // Order2	Optional	XlSortOrder	Determines the sort order for the values specified in Key2.
                    Type.Missing,   // Key3	Optional	Variant	Third sort field; cannot be used when sorting a pivot table.
                    1,              // XlSortOrder.xlAscending          // Order3	Optional	XlSortOrder	Determines the sort order for the values specified in Key3.
                    2,              // XlYesNoGuess.xlNo                // Header	Optional	XlYesNoGuess	Specifies whether the first row contains header information. xlNo is the default value; specify xlGuess if you want Excel to attempt to determine the header.
                    Type.Missing,   // OrderCustom	Optional	Variant	Specifies a one-based integer offset into the list of custom sort orders.
                    Type.Missing,   // MatchCase	Optional	Variant	Set to True to perform a case-sensitive sort, False to perform non-case sensitive sort; cannot be used with pivot tables.
                    1,              // XlSortOrientation.xlSortColumns  // Orientation	Optional	XlSortOrientation	Specifies if the sort should be in acending or decending order.
                    1,              // XlSortMethod.xlPinYin            // SortMethod	Optional	XlSortMethod	Specifies the sort method.
                    0,              // XlSortDataOption.xlSortNormal    // DataOption1	Optional	XlSortDataOption	Specifies how to sort text in the range specified in Key1; does not apply to pivot table sorting.
                    0,              // XlSortDataOption.xlSortNormal    // DataOption2	Optional	XlSortDataOption	Specifies how to sort text in the range specified in Key2; does not apply to pivot table sorting.
                    0,              // XlSortDataOption.xlSortNormal    // DataOption3	Optional	XlSortDataOption	Specifies how to sort text in the range specified in Key3; does not apply to pivot table sorting.
                };
                Object o = (Object)oRange.GetType().InvokeMember("Sort", BindingFlags.InvokeMethod, null, oRange, oArg);
                return;
            }
#endif

            public Range(Object oObject)
            {
                ComObject = oObject;
                return;
            }
            ~Range()
            {
                if (_oValue != null)
                {
                    Value = null;
                }
                ComObject = null;
                return;
            }
        } // class Range

        /****************************************************************************************
         *
        ****************************************************************************************/
        private Dictionary<string, int> _dictFieldSpec; // xmlFieldSpec
        private bool[] _bComment;
        private void FillDataTable(DataTable oDataTable, int cLimit, bool vbShowProgress)
        {
            int nLimit = cLimit;
            if (nLimit > (Records - Record)) nLimit = (Records - Record);
            {
                int cHeader = (Header == HeaderType.Yes) ? 1 : 0;
                int nStartRow = Record + 1;
                for (int nRow = nStartRow; nRow < (nStartRow + nLimit); nRow++)
                {
                    if (Ph.IsRunning) {
                        if (Ph.CancellationPending) break;
                        if (vbShowProgress)
                        {
                            Ph.Value = nRow;
                            Ph.StatusText = "Loading xl records. Record " + (nRow - cHeader) + " of " + nLimit;
                        }
                    }
                    Record++;
                    DataRow oDataRow = oDataTable.NewRow();
                    for (int nCol = 0; nCol < oDataTable.Columns.Count; nCol++ )
                    {
                        int nColIndex = _dictFieldSpec[oDataTable.Columns[nCol].ColumnName];
                        string strItem = _oWorksheet.DataRange.Item(nRow, nColIndex);
                        if (_bComment[nColIndex - 1])
                        {
                            string strComment = _oWorksheet.DataRange.CommentText(nRow, nColIndex);
                            if (strComment != null)
                            {
                                byte[] byteBuffer = Convert.FromBase64String(strComment);
                                string strBuffer = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byteBuffer));
                                string[] strItems = strBuffer.Split('|');
                                if (strItems.Length == 3)
                                {
                                    if (strItems[0] == strItem)
                                    {
                                        strItem = strItems[2];
                                    }
                                }
                            }
                        }
                        oDataRow[nCol] = strItem;
                    }
                    oDataTable.Rows.Add(oDataRow);
                } // for (int nRow = nStartRow; nRow < (nStartRow + nLimit); nRow++)
            }
            return;
        } // FillDataTable()
        public override void CloseDb()
        {
            {
                _oWorkbooks.Close();
                ClearTableList();
            }
            return;
        } // CloseDb()
        public override bool OpenDb()
        {
            ClearMessages();
            do
            {
                if (_oWorkbooks.Name != null)
                {
                    if (_oWorkbooks.Name == Db)
                    {
                        break;  // Already open (OK)
                    }
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Different Database already open");
                    break;
                }
                // Open
                try
                {
                    _oWorkbooks.Open(Db);
                    _oWorkbook = _oWorkbooks.Item[0];
                    ClearTableList();
                    for (int nWorksheet = 0; nWorksheet < _oWorkbook.Worksheets.Count; nWorksheet++)
                    {
                        string strName = _oWorkbook.Worksheets.Item[nWorksheet].Name;
                        AddTableToTableList(strName);
                    }
                }
                catch (Exception ex)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Exception: oWorkbooks.Open(): " + ex.Message);
                    break;  // Error
                }
            } while (false);
            return HasMessages;
        }// OpenDb()

        public override bool CloseTable()
        {
            ClearMessages();
            do
            {
                if (_oWorkbook == null)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Workbook not open");
                    break;
                }
                if (_oWorksheet == null)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Table not open");
                    break;
                }
                _oWorksheet = null;
                base.CloseTable();
            } while (false);
            return HasMessages;
        } // CloseTable()

        public override bool OpenTable()
        {
            ClearMessages();
            do
            {
                if (_oWorkbook == null)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Workbook not open");
                    break;
                }
                if (_oWorksheet != null)
                {
                    if (_oWorksheet.Name == Table)
                    {
                        break;  // Already open (OK)
                    }
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Different Table already open");
                    break;
                }
                _oWorksheet = _oWorkbook.Worksheets.Item[_oWorkbook.Worksheets.KeyToIndex(Table)];
                _oWorksheet.Header = (Header == HeaderType.Yes);
#if EXPERIMENT
                oWorksheet.DataRange.Sort();
#endif
                Record = 0;
                if (RecordsUnknown)
                {
                    Records = _oWorksheet.DataRange.Rows;  // Exact
                }
                if (InputFieldSpec.Length == 0)
                {
                    Dictionary<string, int> dictFieldCount = new Dictionary<string, int>();
                    _dictFieldSpec = new Dictionary<string, int>();
                    _bComment = new bool[_oWorksheet.DataRange.Columns];
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlist");
                    for (int nCol = 1; nCol <= _oWorksheet.DataRange.Columns; nCol++)
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        {
                            string strDbName;
                            if (Header == HeaderType.Yes)
                            {
                                strDbName = _oWorksheet.UsedRange.Item(1, nCol);
                                int nDbName = (dictFieldCount.ContainsKey(strDbName)) ? dictFieldCount[strDbName] : 0;
                                if (nDbName > 0)
                                {
                                    dictFieldCount[strDbName] = nDbName + 1;
                                }
                                else
                                {
                                    dictFieldCount.Add(strDbName, nDbName + 1);
                                }
                                if ((nDbName != 0) || (strDbName == string.Empty)) strDbName = strDbName + "(" + nDbName + ")";
                            }
                            else
                            {
                                strDbName = "Field" + nCol.ToString();
                            }
                            _dictFieldSpec.Add(strDbName, nCol);
                            oCOEXmlTextWriter.WriteAttributeString("dbname", strDbName);
                        }
                        {
                            string strDbType = "null";
                            string strDbTypeSpecifier = string.Empty;
                            for (int nRow = 1; nRow <= Records; nRow++)
                            {
                                string strItem = _oWorksheet.DataRange.Item(nRow, nCol);
                                string strComment = _oWorksheet.DataRange.CommentText(nRow, nCol);
                                if (strComment != null)
                                {
                                    byte[] byteBuffer = Convert.FromBase64String(strComment);
                                    string strBuffer = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byteBuffer));
                                    string[] strItems = strBuffer.Split('|');
                                    if (strItems.Length == 3)
                                    {
                                        if (strItems[0] == strItem)
                                        {
                                            strItem = strItems[2];
                                            _bComment[nCol - 1] = true;
                                        }
                                    }
                                }
                                strDbType = DetermineType(strDbType, strItem);
                            }
                            oCOEXmlTextWriter.WriteAttributeString("dbtype", strDbType);
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                    } // for (int nCol = 1; nCol <= oWorksheet.DataRange.Columns; nCol++)
                    oCOEXmlTextWriter.WriteEndElement();
                    InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                } // if (InputFieldSpec.Length == 0)
            } while (false);
            return HasMessages;
        } // OpenTable()
        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            OpenDataSet();
            // Initialize as if we are reading the entire dataset
            Record = vnStart;
            Minimum = Value = Record;
            Maximum = ((vcLimit < int.MaxValue) ? Record : 0) + vcLimit;
            if (Maximum > Records) Maximum = Records;
            return HasMessages;
        } // OpenDataSet()
        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            riDataSet.Tables.Add(oDataTable);
            DataTableAddColumns(oDataTable, InputFieldSpecMapped);
            FillDataTable(oDataTable, vcLimit, false);  // No read progress. Job::Execute will show write progress.
            Value += oDataTable.Rows.Count;
            return HasMessages;
        } // ReadDataSet()
        protected override System.Data.DataSet ReadDataSetForPreview()
        {
            DataSet oDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            oDataSet.Tables.Add(oDataTable);
            DataTableAddColumns(oDataTable, InputFieldSpec);
            Record = 0;
            Minimum = 0;
            Maximum = Records;
            Value = Record;
            Ph.CancelConfirmation = "If you stop this operation not all records will be available to preview";
            Ph.ProgressSection(delegate() /* InputObjectXl::FillDataTable (nLimit, Cancel) */
            {
                FillDataTable(oDataTable, Records, true);
            });
            return oDataSet;
        } // ReadDataSetForPreview()

        private HeaderType _eHeader;
        private HeaderType Header
        {
            get
            {
                return _eHeader;
            }
            set
            {
                _eHeader = value;
                return;
            }
        }
        private Application _oExcel; // Excel
        private Workbooks _oWorkbooks;   // All workbooks
        private Workbook _oWorkbook; // The (0th) workbook
        private Worksheet _oWorksheet;
        public InputObjectXl()
        {
            Filter = "Microsoft Excel Files (*.xls)|*.xls";
            do
            { // while (false)
                Header = HeaderType.Yes;
                _oExcel = new Application();
                if (_oExcel.ComObject == null)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, -1, "Excel.Application not available.");
                    break;   // Not valid, unable to create Excel object
                }
                _oWorkbooks = _oExcel.Workbooks;
                IsValid = true;
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("InputConfiguration");
                    oCOEXmlTextWriter.WriteAttributeString("text", "Configuration");
                    {
                        oCOEXmlTextWriter.WriteStartElement("GroupBox");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Header");
                        oCOEXmlTextWriter.WriteAttributeString("member", "_eHeader");
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", HeaderType.No.ToString());
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", HeaderType.Yes.ToString());
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    oCOEXmlTextWriter.WriteEndElement();

                    UnboundConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(_oExcel.ComObject);
                    _oExcel = null;
                }
            } while (false);
            return;
        } // InputObjectXl()
        ~InputObjectXl()
        {
            _oWorksheet = null;
            _oWorkbook = null;
            _oWorkbooks = null;
            _oExcel = null;
#if OBSOLETE // and dangerous
            {
                System.Diagnostics.Process[] pExcels = System.Diagnostics.Process.GetProcessesByName("Excel");
                foreach (System.Diagnostics.Process pExcel in pExcels)
                {
                    // ? if (pExcel.StartInfo.EnvironmentVariables.ContainsKey("DataLoader"))
                    if (pExcel.MainWindowHandle.ToInt32() == 0)
                    {
                        try
                        {
                            pExcel.Kill();
                        }
                        catch
                        {
                            ;
                        }
                    }
                }
            }
#endif
            return;
        } // ~InputObjectXl()
    } // class InputObjectXl
}
