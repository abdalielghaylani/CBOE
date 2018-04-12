using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    class InputObjectTxt : InputObject
    {
        #region data
        private COETextReader _oCOETextReader = new COETextReader();
        private DelimiterType _eDelimiter;
        private HeaderType _eHeader;
        #endregion

        #region properties
        public override string Configuration
        {
            get
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
                    if (Path.GetExtension(Db).ToLower() == ".csv")
                    {
                        Delimiter = DelimiterType.Comma;
                    }
                    else
                    {
                        oCOEXmlTextWriter.WriteStartElement("GroupBox");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Delimiter");
                        oCOEXmlTextWriter.WriteAttributeString("member", "_eDelimiter");
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", DelimiterType.Comma.ToString());
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", DelimiterType.Tab.ToString());
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                }
                oCOEXmlTextWriter.WriteEndElement();

                UnboundConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                oCOEXmlTextWriter.Close();
                return base.Configuration;
            } // get
        } // Configuration

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

        public HeaderType Header
        {
            get
            {
                return _eHeader;
            }
            private set
            {
                _eHeader = value;
                return;
            }
        } // Header
        #endregion

        #region constructors
        public InputObjectTxt()
        {
            Filter = "Text files (*.csv;*.txt)|*.csv;*.txt";
            Delimiter = DelimiterType.Tab;
            Header = HeaderType.Yes;
            IsValid = true;   // Silent failure for now
            return;
        } // InputObjectTxt()
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
                List<string> listFieldValues = new List<string>();
                for (int nRow = 0; nRow < nLimit; nRow++)
                {
                    if (Ph.IsRunning) {
                        if (Ph.CancellationPending) break;
                        if (vbShowProgress)
                        {
                            Ph.Value = nRow;
                            Ph.StatusText = "Loading text records. Record " + (1 + nRow) + " of " + nLimit;
                        }
                    }
                    DataRow oDataRow = voDataTable.NewRow();
                    if (ReadRecord(listFieldValues))
                    {
                        break;  // Probably reached EOF
                    }
                    if (listFieldValues.Count == 0)
                    {
                        break;  // EOF
                    }
                    Record++;
                    for (int nCol = 0; nCol < voDataTable.Columns.Count; nCol++)
                    {
                        if (!string.IsNullOrEmpty(listFieldValues[(int)voDataTable.Columns[nCol].ExtendedProperties["Ordinal"]]))
                            oDataRow[nCol] = listFieldValues[(int)voDataTable.Columns[nCol].ExtendedProperties["Ordinal"]];
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
                    // Build FieldList and TypeList
                    // Count / cache
                    Ph.Maximum = (int)_oCOETextReader.Length;
                    Ph.SupportsCancellation = false;
                    Ph.ProgressSection(delegate() /* InputObjectTxt::OpenTable (_oCOETextReader.Length, no Cancel) */
                    {
                        List<string> listFieldDbNames = new List<string>();
                        List<string> listFieldDbTypes = new List<string>();
                        Records = 0; RecordsApproximate = true;
                        Rewind(); Record = 0;
                        List<string> listFieldValues = new List<string>();
                        ReadRecord(listFieldValues);
                        if (Header == HeaderType.Yes)
                        {
                            foreach (string strDbName in listFieldValues)
                            {
                                // !! duplicates
                                listFieldDbNames.Add(strDbName);
                            }
                        }
                        else
                        {
                            for (int nCol = 1; nCol <= listFieldValues.Count; nCol++)
                            {
                                listFieldDbNames.Add("Field" + nCol);
                            }
                            Rewind();
                        }
                        for (int nCol = 0; nCol < listFieldValues.Count; nCol++)
                        {
                            listFieldDbTypes.Add("null");
                        }
                        while (_oCOETextReader.Position < _oCOETextReader.Length)
                        {
                            Ph.Value = (int)_oCOETextReader.Position;
                            Ph.StatusText = "Scanning text file.";
                            long lngPosition = _oCOETextReader.Position;
                            //Console.WriteLine("Record " + (1 + Records) + " @ " + lngPosition.ToString("X"));
                            if (ReadRecord(listFieldValues))
                            {
                                break;  // FAILED
                            }
                            if (listFieldValues.Count != listFieldDbNames.Count)
                            {
                                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, -1, "Inconsistent number of fields at record " + Records);
                                break;
                            }
                            for (int nCol = 0; nCol < listFieldValues.Count; nCol++)
                            {
                                string strDbType = listFieldDbTypes[nCol];
                                string strDbTypeSpecifier = string.Empty;
                                strDbType = DetermineType(strDbType, listFieldValues[nCol]);
                                listFieldDbTypes[nCol] = strDbType;
                            }
                            Records++;
                        } // while (_oCOETextReader.Position < _oCOETextReader.Length)
                        RecordsApproximate = (_oCOETextReader.Position < _oCOETextReader.Length);
                        {
                            COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                            oCOEXmlTextWriter.WriteStartElement("fieldlist");
                            for (int nCol = 0; nCol < listFieldDbNames.Count; nCol++)
                            {
                                oCOEXmlTextWriter.WriteStartElement("field");
                                oCOEXmlTextWriter.WriteAttributeString("dbname", listFieldDbNames[nCol]);
                                oCOEXmlTextWriter.WriteAttributeString("dbtype", listFieldDbTypes[nCol]);
                                oCOEXmlTextWriter.WriteEndElement();
                            }
                            oCOEXmlTextWriter.WriteEndElement();
                            InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                            oCOEXmlTextWriter.Close();
                        }
                        Rewind(); Record = 0;
                    });
                } // if (InputFieldSpec.Length == 0)
            } while (false);
            return HasMessages;
        } // OpenTable()

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            OpenDataSet();
            Rewind();
            if (Header == HeaderType.Yes) _oCOETextReader.ReadLine();
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
            Rewind(); Record = 0;
            if (Header == HeaderType.Yes) _oCOETextReader.ReadLine();
            Ph.Maximum = Records;
            Ph.CancelConfirmation = "If you stop this operation not all records will be available to preview";
            Ph.ProgressSection(delegate() /* InputObjectSdf::ReadDataSetForPreview (Records, Cancel) */
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
            return;
        } // Rewind()

        #endregion
    } // class InputObjectTxt
}
