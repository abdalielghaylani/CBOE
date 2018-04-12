using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    class InputObjectTxtBlock : InputObject
    {
        #region data
        private COETextReader _oCOETextReader = new COETextReader();
        private DelimiterType _eDelimiter;
        private XmlDocument _xmlLayout = new XmlDocument();
        #endregion

        #region properties
        public DelimiterType Delimiter
        {
            get
            {
                return _eDelimiter;
            }
            private set
            {
                _eDelimiter = value;
                return;
            }
        } // Delimiter
        private XmlDocument Layout
        {
            get
            {
                return _xmlLayout;
            }
        }
        #endregion

        #region constructors
        public InputObjectTxtBlock()
        {
            Filter = "Block Text files (*.txt)|*.txt";
            Delimiter = DelimiterType.Tab;
            IsValid = true;
            IsValid = ((Environment.MachineName == "WJCDUO") || (Environment.MachineName == "BCLAFF745"));  // Until ready
            Layout.LoadXml(
"<LAYOUT>" + "\r" +
" <LINE type=\"field\" cols=\"6\"/>" + "\r" +
" <LINE type=\"value\"/>" + "\r" +
" <LINE type=\"empty\"/>" + "\r" +
" <BLOCK repeat=\"3\">" + "\r" +
"  <LINE type=\"field\" cols=\"1\"/>" + "\r" +
"  <LINE type=\"value\"/>" + "\r" +
"  <LINE type=\"empty\"/>" + "\r" +
"  <LINE type=\"array\" cols=\"24\" rows=\"16\"/>" + "\r" +
"  <LINE type=\"empty\"/>" + "\r" +
" </BLOCK>" + "\r" +
"</LAYOUT>" + "\r" +
""
            );
            return;
        } // InputObjectTxtBlock()
        #endregion

        #region methods
        public override void CloseDb()
        {
            if (_oCOETextReader.IsOpen)
            {
                _oCOETextReader.Close();
            }
            return;
        } // CloseDb()

        private void FillDataTable(DataTable voDataTable, int vcLimit, bool vbShowProgress)
        {
            int nLimit = vcLimit;
            // if (nLimit > Records) nLimit = Records; // WJC record unknown?
            {
                for (int nRow = 0; nRow < nLimit; nRow++)
                {
                    if (Ph.IsRunning) {
                        if (Ph.CancellationPending) break;
                        if (vbShowProgress)
                        {
                            Ph.Value = nRow;
                            Ph.StatusText = "Loading text block records. Record " + (1 + nRow) + " of " + nLimit;
                        }
                    }
                    DataRow oDataRow = voDataTable.NewRow();
                    _WalkTableLayoutInfo.eGatherType = WalkTableLayoutInfo.GatherType.FieldDbValues;
                    listFieldDbValues.Clear();
                    if (_oCOETextReader.Position < _oCOETextReader.Length)
                    {
                        WalkTableLayout(_WalkTableLayoutInfo, Layout.DocumentElement.ChildNodes);
                    }
                    if (listFieldDbValues.Count != listFieldDbNames.Count) {
                        break;  // Probably reached EOF
                    }
                    Record++;
                    for (int nCol = 0; nCol < voDataTable.Columns.Count; nCol++)
                    {
                        oDataRow[nCol] = listFieldDbValues[(int)voDataTable.Columns[nCol].ExtendedProperties["Ordinal"]];
                    }
                    voDataTable.Rows.Add(oDataRow);
                }
            }
            return;
        } // FillDataTable()

        //TODO: don't re-open unnecessarily
        public override bool OpenDb()
        {
            ClearMessages();
            do
            {
                if (_oCOETextReader.IsOpen != true)
                {
                    try
                    {
                        _oCOETextReader.Open(Db);
                    }
                    catch (Exception ex)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Unable to open the database: " + ex.Message);
                        break;
                    }
                }
                // Build TableList
                {
                    ClearTableList();
                    AddTableToTableList("TxtTable");  // Just to indicate there there is a single table
                }
            } while (false);
            return HasMessages;
        } // OpenDb()

        public override bool CloseTable()
        {
            ClearMessages();
            do
            {
                if (_oCOETextReader.IsOpen == false)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
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
                if (_oCOETextReader.IsOpen == false)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
                    break;  // Error
                }
                if (InputFieldSpec.Length == 0)
                {
                    WalkTableClear();
                    // Gather FieldDbNames
                    {
                        Rewind();
                        _WalkTableLayoutInfo.eGatherType = WalkTableLayoutInfo.GatherType.FieldDbNames;
                        WalkTableLayout(_WalkTableLayoutInfo, Layout.DocumentElement.ChildNodes);
                    }
                    // Gather FieldDbTypes
                    Ph.Maximum = (int)_oCOETextReader.Length;
                    Ph.SupportsCancellation = false;
                    Ph.ProgressSection(delegate() /* InputObjectTxtBlock::OpenTable (_oCOETextReader.Length, no Cancel) */
                    {
                        Records = 0; RecordsApproximate = true;
                        Rewind();
                        _WalkTableLayoutInfo.eGatherType = WalkTableLayoutInfo.GatherType.FieldDbTypes;
                        while (_oCOETextReader.Position < _oCOETextReader.Length)
                        {
                            Ph.Value = (int)_oCOETextReader.Position;
                            Ph.StatusText = "Scanning block text file.";
                            long lngPosition = _oCOETextReader.Position;
                            //Console.WriteLine("Record " + (1 + Records) + " @ " + lngPosition.ToString("X"));
                            _WalkTableLayoutInfo.nField = 0;
                            WalkTableLayout(_WalkTableLayoutInfo, Layout.DocumentElement.ChildNodes);
                            Records++;
                        } // while (_oCOETextReader.Position < _oCOETextReader.Length)
                        RecordsApproximate = (_oCOETextReader.Position < _oCOETextReader.Length);
                        Rewind();
                    }
                    );
                    // Create XML
                    {
                        COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                        oCOEXmlTextWriter.WriteStartElement("fieldlist");
                        for (int nCol = 0; nCol < listFieldDbNames.Count; nCol++)
                        {
                            oCOEXmlTextWriter.WriteStartElement("field");
                            oCOEXmlTextWriter.WriteAttributeString("dbname", listFieldDbNames[nCol]);
                            oCOEXmlTextWriter.WriteAttributeString("dbtype", listFieldDbTypes[nCol] + listFieldDbBounds[nCol]);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                        InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                        oCOEXmlTextWriter.Close();
                    }
                } // if (InputFieldSpec.Length == 0)
            } while (false);
            return HasMessages;
        } // OpenTable()

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            OpenDataSet();
            Rewind();
            Minimum = Value = (Int32)_oCOETextReader.Position;
            Maximum = (Int32)_oCOETextReader.Length;
            return HasMessages;
        } // OpenDataSet()

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = DataSetForJob;
            FillDataTable(DataSetForJob.Tables[0], vcLimit, false);
            Value = (Int32)_oCOETextReader.Position;
            return HasMessages;
        } // ReadDataSet()

        protected override DataSet ReadDataSetForPreview()
        {
            DataSet oDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            oDataSet.Tables.Add(oDataTable);
            DataTableAddColumns(oDataTable, InputFieldSpec);
            Rewind();
            Ph.Maximum = Records;
            Ph.CancelConfirmation = "If you stop this operation not all records will be available to preview";
            Ph.ProgressSection(delegate() /* InputObjectTxtBlock::ReadDataSetForPreview (Records, Cancel) */
            {
                FillDataTable(oDataTable, Records, true);
            });
            return oDataSet;
        } // ReadDataSetForPreview()

        private bool ReadRecord(List<string> rlistFieldValues)
        {
            ClearMessages();
            rlistFieldValues.Clear();
            string strLine = _oCOETextReader.ReadLine();
            if (strLine.Length > 0)
            {
                if (Delimiter == DelimiterType.Tab)
                {
                    string[] strFields = strLine.Split('\t');
                    foreach (string strField in strFields)
                    {
                        rlistFieldValues.Add(strField);
                    }
                }
                else
                {
                    int nLft = 0;
                    int nRht = 0;
                    bool bQuote = false;
                    char ch;
                    do
                    {
                        do
                        {
                            if (nRht >= strLine.Length)
                            {
                                ch = '\0';
                                nRht = strLine.Length;
                                break;  // Unexpected EOL
                            }
                            nRht = strLine.IndexOfAny(new char[] { '"', ',', '\\' }, nRht);
                            if (nRht == -1)
                            {
                                ch = '\0';
                                nRht = strLine.Length;
                                break;  // EOL
                            }
                            ch = strLine[nRht];
                            if (ch != '\\')
                            {
                                break;  // not a backslash
                            }
                            nRht++; // Backslash
                        } while (true); // Finding character of interest
                        if (ch == '"')
                        {
                            bQuote = !bQuote;
                            nRht++;
                            continue;   // toggle quote state
                        }
                        if (bQuote && (ch == ','))
                        {
                            nRht++;
                            continue;   // comma inside a quote
                        }
                        string strField = strLine.Substring(nLft, nRht - nLft);
                        if (strField.StartsWith("\"")) strField = strField.Substring(1, strField.Length - 2);
                        rlistFieldValues.Add(strField);
                        nLft = nRht + 1;
                        nRht = nLft;
                        if (ch == '\0')
                        {
                            break;  // EOL
                        }
                    } while (true); // processing the line
                }
            } // if (strLine.Length > 0)
            return HasMessages;
        } // ReadRecord()

        protected void Rewind()
        {
            _oCOETextReader.Position = 0;
            Record = 0;
            return;
        } // Rewind()

        private string CleanFieldDbName(string vstrFieldDbName)
        {
            vstrFieldDbName = vstrFieldDbName.TrimEnd(new char[] { '.' });
            return vstrFieldDbName;
        } // CleanFieldDbName()

        private List<string> listFieldDbNames = new List<string>();
        private List<string> listFieldDbTypes = new List<string>();
        private List<string> listFieldDbBounds = new List<string>();    // Will be appended to Types
        private List<string> listFieldDbValues = new List<string>();
        private void WalkTableClear()
        {
            listFieldDbNames.Clear();
            listFieldDbTypes.Clear();
            listFieldDbBounds.Clear();
            listFieldDbValues.Clear();
            return;
        } // WalkTableClear()

        private class WalkTableLayoutInfo
        {
            #region enums and types
            /// <summary>
            /// Indicates what to gather while walking the file
            /// </summary>
            public enum GatherType
            {
                /// <summary>
                /// Gather fields
                /// </summary>
                FieldDbNames,
                /// <summary>
                /// Gather types
                /// </summary>
                FieldDbTypes,
                /// <summary>
                /// Gather values
                /// </summary>
                FieldDbValues
            };
            #endregion

            public GatherType eGatherType;
            public int cCols;   // Number of columns in the previous element with name="field"
            public int nField;    // Which field is currently being processed (Types and Values)
        } // class WalkTableLayoutInfo
        private WalkTableLayoutInfo _WalkTableLayoutInfo = new WalkTableLayoutInfo();
        private void WalkTableLayout(WalkTableLayoutInfo voWalkTableLayoutInfo, XmlNodeList voXmlNodeListLayout)
        {
            foreach (XmlNode oXmlNodeLayout in voXmlNodeListLayout)
            {
                switch (oXmlNodeLayout.Name)
                {
                    case "LINE":
                        {
                            string strType = oXmlNodeLayout.Attributes["type"].Value.ToString();
                            switch (strType)
                            {
                                case "field":
                                    {
                                        voWalkTableLayoutInfo.cCols = Int32.Parse(oXmlNodeLayout.Attributes["cols"].Value.ToString());
                                        switch (voWalkTableLayoutInfo.eGatherType)
                                        {
                                            case WalkTableLayoutInfo.GatherType.FieldDbNames:
                                                {
                                                    List<string> listFieldValues = new List<string>();
                                                    ReadRecord(listFieldValues);
                                                    for (int nCol = 0; nCol < voWalkTableLayoutInfo.cCols; nCol++)
                                                    {
                                                        listFieldDbNames.Add(CleanFieldDbName(listFieldValues[nCol]));
                                                        listFieldDbTypes.Add("null");
                                                        listFieldDbBounds.Add(string.Empty);  // empty means scalar
                                                    }
                                                    break;
                                                }
                                            case WalkTableLayoutInfo.GatherType.FieldDbTypes:
                                            case WalkTableLayoutInfo.GatherType.FieldDbValues:
                                                {
                                                    _oCOETextReader.ReadLine();
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        } // switch (voWalkTableLayoutInfo.eGatherType)
                                        break;
                                    }
                                case "value":
                                    {
                                        switch (voWalkTableLayoutInfo.eGatherType)
                                        {
                                            case WalkTableLayoutInfo.GatherType.FieldDbNames:
                                                {
                                                    _oCOETextReader.ReadLine();
                                                    break;
                                                }
                                            case WalkTableLayoutInfo.GatherType.FieldDbTypes:
                                                {
                                                    List<string> listFieldValues = new List<string>();
                                                    ReadRecord(listFieldValues);
                                                    for (int nCol = 0; nCol < voWalkTableLayoutInfo.cCols; nCol++)
                                                    {
                                                        string strDbType = listFieldDbTypes[voWalkTableLayoutInfo.nField];
                                                        string strDbTypeSpecifier = string.Empty;
                                                        strDbType = DetermineType(strDbType, listFieldValues[nCol]);
                                                        listFieldDbTypes[voWalkTableLayoutInfo.nField] = strDbType;
                                                        voWalkTableLayoutInfo.nField++;
                                                    }
                                                    voWalkTableLayoutInfo.cCols = 0;
                                                    break;
                                                }
                                            case WalkTableLayoutInfo.GatherType.FieldDbValues:
                                                {
                                                    List<string> listFieldValues = new List<string>();
                                                    ReadRecord(listFieldValues);
                                                    for (int nCol = 0; nCol < voWalkTableLayoutInfo.cCols; nCol++)
                                                    {
                                                        listFieldDbValues.Add(listFieldValues[nCol]);
                                                        voWalkTableLayoutInfo.nField++;
                                                    }
                                                    voWalkTableLayoutInfo.cCols = 0;
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        } // switch (voWalkTableLayoutInfo.eGatherType)
                                        break;
                                    }
                                case "empty":
                                    {
                                        List<string> listFieldValues = new List<string>();
                                        _oCOETextReader.ReadLine();
                                        break;
                                    }
                                case "array":
                                    {
                                        int cRows = Int32.Parse(oXmlNodeLayout.Attributes["rows"].Value.ToString());
                                        int cCols = Int32.Parse(oXmlNodeLayout.Attributes["cols"].Value.ToString());
                                        switch (voWalkTableLayoutInfo.eGatherType)
                                        {
                                            case WalkTableLayoutInfo.GatherType.FieldDbNames:
                                                {
                                                    listFieldDbNames.Add(CleanFieldDbName(listFieldDbNames[listFieldDbNames.Count - 1] + " array"));
                                                    listFieldDbTypes.Add("null");
                                                    listFieldDbBounds.Add("[" + cRows + "," + cCols + "]");  // array bounds
                                                    for (int nRow = 0; nRow < cRows; nRow++)
                                                    {
                                                        _oCOETextReader.ReadLine();
                                                    }
                                                    break;
                                                }
                                            case WalkTableLayoutInfo.GatherType.FieldDbTypes:
                                                {
                                                    for (int nRow = 0; nRow < cRows; nRow++)
                                                    {
                                                        List<string> listFieldValues = new List<string>();
                                                        ReadRecord(listFieldValues);
                                                        for (int nCol = 0; nCol < cCols; nCol++)
                                                        {
                                                            string strDbType = listFieldDbTypes[voWalkTableLayoutInfo.nField];
                                                            string strDbTypeSpecifier = string.Empty;
                                                            strDbType = DetermineType(strDbType, listFieldValues[nCol]);
                                                            listFieldDbTypes[voWalkTableLayoutInfo.nField] = strDbType;
                                                        }
                                                    }
                                                    voWalkTableLayoutInfo.nField++;
                                                    break;
                                                }
                                            case WalkTableLayoutInfo.GatherType.FieldDbValues:
                                                {
                                                    string strArray = string.Empty;
                                                    strArray += "{";
                                                    for (int nRow = 0; nRow < cRows; nRow++)
                                                    {
                                                        List<string> listFieldValues = new List<string>();
                                                        ReadRecord(listFieldValues);
                                                        if (nRow > 0) strArray += ",";
                                                        strArray += "{";
                                                        for (int nCol = 0; nCol < cCols; nCol++)
                                                        {
                                                            if (nCol > 0) strArray += ",";
                                                            strArray += listFieldValues[nCol];
                                                        }
                                                        strArray += "}";
                                                    }
                                                    strArray += "}";
                                                    listFieldDbValues.Add(strArray);
                                                    voWalkTableLayoutInfo.nField++;
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        } // switch (voWalkTableLayoutInfo.eGatherType)
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            } // switch (fieldType)
                            break;
                        } // case "LINE"
                    case "BLOCK":
                        {
                            int cRepeat = Int32.Parse(oXmlNodeLayout.Attributes["repeat"].Value.ToString());
                            for (int nRepeat = 0; nRepeat < cRepeat; nRepeat++)
                            {
                                WalkTableLayout(voWalkTableLayoutInfo, oXmlNodeLayout.ChildNodes);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                } // switch (oXmlNodeLayout.Name)
            } // foreach (XmlNode oXmlNodeLayout in oXmlNodeListLayout)
            return;
        } // WalkTableLayout()

        #endregion
    } // class InputObjectTxtBlock
}
