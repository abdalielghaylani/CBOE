using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    /// <summary>
    /// <see cref="InputObject"/> for SDF databases
    /// </summary>
    /// 
    //CSBR-132944 bug Solved code.
    //By solving this bug we providing "Prevoew" for all recordsafter selecting any .SDF file like .CFX,.CFW...
    class InputObjectSdf : InputObject
    {
        private COETextReader _oCOETextReader = new COETextReader();
        private StringBuilder structureBuilder;
        //For CSBR-132944 Bug changed code 
        //commenting this line, Which no more in use.
        //private int _maxPreviewRecords = 500;
        private int _maxRecordsTotal = 0;

        public InputObjectSdf()
        {
            Filter = "SD files (*.sdf)|*.sdf";
            IsValid = true;
            //defer to configuration setting if available
            //For CSBR-132944 Bug changed code 
            //commenting this lines to avoid getttnig the value "1000" provided in the config files,Which is no more in use.
            //if (System.Configuration.ConfigurationManager.AppSettings["PrescanChunkSize"] != null)
            //    _maxPreviewRecords = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PrescanChunkSize"]);
        }

        /// <summary>
        /// Opens the SDF file for reading.
        /// </summary>
        /// <returns></returns>
        public override bool OpenDb()
        {
            ClearMessages();
            try
            {
                //close any previously-open DBs!
                CloseDb();
                _oCOETextReader.Open(Db);
            }
            catch (Exception ex)
            {
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Unable to open the database: " + ex.Message);
            }

            if (!HasMessages)
            {
                ClearTableList();
                AddTableToTableList("SdfTable");  // Just to indicate there there is a single table
            }
            return HasMessages;
        }

        /// <summary>
        /// Closes the SDF file's reader
        /// </summary>
        public override void CloseDb()
        {
            if (_oCOETextReader.IsOpen)
                _oCOETextReader.Close();
        }

        /// <summary>
        /// Reads SDFile records from a file and packs the data into the mapped table columns.
        /// </summary>
        /// <remarks>
        /// By this point, the field metadata (columns - name and type) has been defined.
        /// </remarks>
        /// <param name="voDataTable">An empty table containing the mapped columns</param>
        /// <param name="vcLimit">The maximum number of records to read at a time</param>
        /// <param name="vbShowProgress">Indicator to update or ignore the progress meter</param>
        private void FillDataTable(DataTable voDataTable, int vcLimit, bool vbShowProgress)
        {
            for (int nRow = 0; nRow < vcLimit; nRow++)
            {
                //Manage the progress-helper's status, including any 'break' instruction
                if (Ph.IsRunning)
                {
                    if (Ph.CancellationPending) break;
                    if (vbShowProgress)
                    {
                        Ph.Value = (int)_oCOETextReader.Position;
                        string strStatusText = "Loading sdf records.";
                        strStatusText += " Reading record " + (1 + nRow) + " of ";
                        if (RecordsApproximate)
                            strStatusText += " approximately " + Records;
                        else
                            strStatusText += vcLimit;

                        int nPercent = (int)((100 * _oCOETextReader.Position) / _oCOETextReader.Length);
                        strStatusText += ". (" + nPercent + "% complete)";
                        Ph.StatusText = strStatusText;
                    }
                }

                //Generate the intermediate data containers
                DataRow oDataRow = voDataTable.NewRow();
                List<string> listHeader = new List<string>();
                List<string> listMolecule = new List<string>();
                Dictionary<string, List<string>> dictFieldValues = new Dictionary<string, List<string>>();

                if (ReadRecord(ref listHeader, ref listMolecule, ref dictFieldValues))
                    break;  // Probably reached EOF

                if (listHeader.Count == 0)
                    break;  // Probably reached EOF

                foreach (DataColumn oDataColumn in voDataTable.Columns)
                {
                    string colName = oDataColumn.ColumnName;

                    if (colName == "sdf_molecule")
                    {
                        //Pack the SDF structure data carefully
                        if (listMolecule.Count >= 3) //CBOE-301 SJ To show the preview of atoms in the sdf file
                        {
                            structureBuilder = new StringBuilder();

                            //Was structureBuilder.AppendLine, but line breaks might be a problem(?)
                            foreach (string hdr in listHeader)
                                structureBuilder.AppendLine(hdr);
                            foreach (string det in listMolecule)
                                structureBuilder.AppendLine(det);

                            string sdfStructureString = structureBuilder.ToString();
                            byte[] sdfStructureBytes = ConvertStringToBytes(sdfStructureString);
                            oDataRow[colName] = sdfStructureBytes;
                        }
                    }
                    else
                    {
                        if (dictFieldValues.ContainsKey(colName))
                        {
                            int cLines = dictFieldValues[colName].Count;
                            if (cLines == 0)
                                oDataRow[colName] = System.DBNull.Value;
                            else if (cLines == 1)
                                oDataRow[colName] = dictFieldValues[colName][0];
                            else
                                oDataRow[colName] = String.Join(";", dictFieldValues[colName].ToArray());
                        }
                    }
                }

                //Finally, add the data to the table
                voDataTable.Rows.Add(oDataRow);
            }
        }

        /// <summary>
        /// Closes the SDFile reader.
        /// </summary>
        /// <returns></returns>
        public override bool CloseTable()
        {
            ClearMessages();
            if (_oCOETextReader.IsOpen == false)
            {
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
            }
            if (!HasMessages)
                base.CloseTable();

            return HasMessages;
        }

        /// <summary>
        /// Opens the SDFile reader, and attempts to create metadata based on its contents.
        /// </summary>
        /// <returns></returns>
        public override bool OpenTable()
        {
            ClearMessages();
            if (_oCOETextReader.IsOpen == false)
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");

            if (!HasMessages && InputFieldSpec.Length == 0)
            {
                Ph.Maximum = (int)_oCOETextReader.Length;
                _maxRecordsTotal = GetRecordCount();

                PreReadTable();

                //overwrite the value from the pre-read since we're only pre-reading a subset of the total
                Records = _maxRecordsTotal;
                RecordsApproximate = true;
            }

            return HasMessages;
        }

        /// <summary>
        /// Perform a pre-read of the file in order to get a 'maximum' record count.
        /// (Considered a 'maximum' count because some records may be blank or invalid.)
        /// </summary>
        /// <returns></returns>
        private int GetRecordCount()
        {
            int recordCount = 0;
            Ph.ProgressSection(delegate()
            {
                ClearMessages();
                Rewind();
                string lineOfData = string.Empty;
                string status = "Scanning sdf file...reading record {0}. ({1}% complete)";
                //For CSBR-132944 Bug changed code 
                //Here skipping the Default vaue(1000) getting from config file and comparing
                //while (_oCOETextReader.Position < _oCOETextReader.Length && recordCount < _maxPreviewRecords)
                while (_oCOETextReader.Position < _oCOETextReader.Length)
                {
                    lineOfData = _oCOETextReader.ReadLine();
                    if (lineOfData.StartsWith("$$$$"))
                        recordCount++;

                    int nPercent = (int)((100 * _oCOETextReader.Position) / _oCOETextReader.Length);
                    Ph.Value = (int)_oCOETextReader.Position;
                    Ph.StatusText = string.Format(status, recordCount.ToString(), nPercent.ToString());
                }

                if (_oCOETextReader.Position < _oCOETextReader.Length)
                {
                    recordCount = Convert.ToInt32((recordCount) * (_oCOETextReader.Length / _oCOETextReader.Position));
                }

                _oCOETextReader.Position = 0;
            });
            return recordCount;
        }

        /// <summary>
        /// Discovery mechanism for generating file format metadata.
        /// Builds 'FieldList' and 'TypeList'; Count / cache.
        /// </summary>
        /// <remarks>
        /// This method is a helper for "OpenTable"
        /// </remarks>
        private void PreReadTable()
        {
            Ph.ProgressSection(delegate() /* InputObjectSdf::OpenTable (_oCOETextReader.Length, no Cancel) */
            {
                SortedDictionary<string, string> dictFieldNameTypes = new SortedDictionary<string, string>();
                bool bMolecule = false;
                Records = 0;
                RecordsApproximate = true;
                Rewind();
                Record = 0;
                string status = "Scanning sdf file...reading record {0} of approximately {1}. ({2}% complete)";
                //For CSBR-132944 Bug changed code 
                //Here skipping the Default vaue getting from config file and comparing with total records available
                //while (_oCOETextReader.Position < _oCOETextReader.Length && Records < _maxPreviewRecords)
                while (_oCOETextReader.Position < _oCOETextReader.Length)
                {
                    int nApproximateRecords = (_oCOETextReader.Position > 0) ? (int)(Records * ((double)_oCOETextReader.Length / _oCOETextReader.Position)) : 0;
                    if (Ph.CancellationPending)
                    {
                        if (_oCOETextReader.Position > 0)
                            Records = nApproximateRecords;
                        break;
                    }

                    if (Records % 10 == 0)
                    {
                        int nPercent = (int)((100 * _oCOETextReader.Position) / _oCOETextReader.Length);
                        Ph.Value = (int)_oCOETextReader.Position;
                        Ph.StatusText = string.Format(
                            status, Records.ToString(), nApproximateRecords.ToString(), nPercent.ToString()
                        );
                    }

                    long lngPosition = _oCOETextReader.Position;
                    List<string> listHeader = new List<string>();
                    List<string> listMolecule = new List<string>();
                    Dictionary<string, List<string>> dictFieldValues = new Dictionary<string, List<string>>();

                    if (ReadRecord(ref listHeader, ref listMolecule, ref dictFieldValues))
                        break;  // FAILED

                    if (listHeader.Count != 3)
                        break;  // EOF

                    if (listMolecule.Count > 0)
                        bMolecule = true;

                    foreach (KeyValuePair<string, List<string>> kvp in dictFieldValues)
                    {
                        string strField = kvp.Key;
                        string strDbType;
                        string strDbTypeSpecifier;
                        if (dictFieldNameTypes.ContainsKey(strField) == false)
                        {
                            strDbType = "null";
                            dictFieldNameTypes.Add(strField, strDbType);
                        }

                        strDbType = dictFieldNameTypes[strField];
                        strDbTypeSpecifier = string.Empty;

                        if (strDbType == "System.String") continue;   // Nothing is more generic than a string
                        if (kvp.Value.Count > 1)
                            strDbType = "System.String";  // Multiple lines must be a string
                        else
                        {
                            string strValue = (kvp.Value.Count == 0) ? string.Empty : kvp.Value[0];
                            strDbType = DetermineType(strDbType, strValue);
                        }
                        dictFieldNameTypes[strField] = strDbType;
                    }

                    // position[cRecord] = lngPosition
                    Records++;
                }

                //RecordsApproximate = (_oCOETextReader.Position < _oCOETextReader.Length);
                Rewind();
                Record = 0;
                COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                oCOEXmlTextWriter.WriteStartElement("fieldlist");

                if (bMolecule)
                {
                    oCOEXmlTextWriter.WriteStartElement("field");
                    oCOEXmlTextWriter.WriteAttributeString("dbname", "sdf_molecule");
                    oCOEXmlTextWriter.WriteAttributeString("dbtype", "System.Byte[]");
                    oCOEXmlTextWriter.WriteEndElement();
                }
                foreach (KeyValuePair<string, string> kvp in dictFieldNameTypes)
                {
                    //Exclude mapping options for fields with no discovered values
                    if (kvp.Value != "null")    // WJC exclude fields with no values
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("dbname", kvp.Key);
                        oCOEXmlTextWriter.WriteAttributeString("dbtype", kvp.Value);
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                } //
                oCOEXmlTextWriter.WriteEndElement();
                InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                oCOEXmlTextWriter.Close();
            });
        }

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            OpenDataSet();
            Rewind(); Record = 0;
            Value = 0;
            Ph.Maximum = vnStart;
            Ph.CancelConfirmation = "If you stop this operation then no records will be processed";
            Ph.ProgressSection(delegate() /* InputObjectSdf::OpenDataSet(Record, Cancel) */
            {
                for (Record = 0; Record < vnStart; Record++)
                {
                    if (Ph.CancellationPending)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "The user stopped the operation before reaching the start record");
                        break;
                    }
                    Ph.Value = Record;
                    Ph.StatusText = "Skipping sdf record " + (1 + Record) + " of " + vnStart;
                    List<string> listHeader = new List<string>();
                    List<string> listMolecule = new List<string>();
                    Dictionary<string, List<string>> dictFieldValues = new Dictionary<string, List<string>>();
                    if (ReadRecord(ref listHeader, ref listMolecule, ref dictFieldValues))
                    {
                        break;  // OK, probably reached EOF
                    }
                    if (listHeader.Count == 0)
                    {
                        break;  // OK, probably reached EOF
                    }
                }
            });
            Maximum = (Int32)_oCOETextReader.Length;
            return HasMessages;
        }

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = DataSetForJob;
            FillDataTable(DataSetForJob.Tables[0], vcLimit, false);
            Value = (Int32)_oCOETextReader.Position;
            return HasMessages;
        }

        protected override DataSet ReadDataSetForPreview()
        {
            DataSet oDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            oDataSet.Tables.Add(oDataTable);
            DataTableAddColumns(oDataTable, InputFieldSpec);
            Rewind();
            Record = 0;
            Ph.Maximum = (Int32)_oCOETextReader.Length;
            Ph.CancelConfirmation = "If you stop this operation not all records will be available to preview";
            Ph.ProgressSection(delegate() /* InputObjectSdf::ReadDataSetForPreview (Record, Cancel) */
            {
                //For CSBR-132944 Bug changed code 
                //Here skipping the Default vaue passing into dataset and passing total records available in .Sdf file. 
                //FillDataTable(oDataTable, _maxPreviewRecords, true);
                FillDataTable(oDataTable, _maxRecordsTotal, true);
            });
            return oDataSet;
        }

        /// <summary>
        /// Extracts an SDFile record via the reader using industry-standard SDFile formatting definitions.
        /// </summary>
        /// <remarks>
        /// This is the core parsing mechanism for SDFiles.
        /// </remarks>
        /// <param name="rlistHeader">Will be populated with record header information</param>
        /// <param name="rlistMolecule">Will be populated with the string of the molecule itself</param>
        /// <param name="rdictFieldValues">Will house additional (and optional) field information</param>
        /// <returns></returns>
        private bool ReadRecord(
            ref List<string> rlistHeader
            , ref List<string> rlistMolecule
            , ref Dictionary<string, List<string>> rdictFieldValues
        )
        {
            string FIELD_NAME_PREFIX = "DT";
            ClearMessages();
            rlistHeader.Clear();
            rlistMolecule.Clear();
            rdictFieldValues.Clear();

            List<string> listField = null;  // Current field
            while ((rlistHeader.Count < 3) && (_oCOETextReader.Position < _oCOETextReader.Length))
            {
                rlistHeader.Add(_oCOETextReader.ReadLine());
            }

            List<string> listFieldnames = new List<string>();
            int cBlankLines = 0;
            while (_oCOETextReader.Position < _oCOETextReader.Length)
            {
                string strLine = _oCOETextReader.ReadLine();
                if (!strLine.StartsWith("$$$$"))
                {
		/* CSBR# 140363
		Purpose of Change: To make the logic of how data field headers are read, compatible with more valid formats */
                    if (strLine.StartsWith(">") && ((cBlankLines > 0) || listField == null))
                    {
                        string strField = "";
                        if(strLine.Contains("<"))
                        {
                            strField = strLine.Substring(strLine.IndexOf('<') + 1);
                            int nEOF = strField.IndexOf('>');
                            if (nEOF >= 0)
                            {
                                strField = strField.Substring(0, nEOF);
                            }
                        }
                        else if (strLine.Contains(FIELD_NAME_PREFIX))
                        {
                            string[] values = strLine.Substring(1).Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                            if (values != null)
                            {
                                foreach (string value in values)
                                {
                                    if (value.StartsWith(FIELD_NAME_PREFIX))
                                    {
                                        strField = value;
                                        if (strField.Contains("("))
                                            strField = strField.Substring(0, value.IndexOf("("));
                                        break;
                                    }
                                }
                            }
                        }
                            strField = strField.Replace('.', '_').Trim();  // WJC Need a more general procedure
                            {
                                if (listField != null) cBlankLines--;   // Except for the 1st field there is one blank separator line
                                while (cBlankLines > 0)
                                {
                                    cBlankLines--;
                                }
                                listField = new List<string>();
                                if (rdictFieldValues.ContainsKey(strField))
                                {
                                    int n;
                                    for (n = 1; rdictFieldValues.ContainsKey(strField + "(" + n + ")"); n++) ;
                                    strField = strField + "(" + n + ")";
                                }
                            if (strField.Trim() != String.Empty)
                                rdictFieldValues.Add(strField, listField);
                                continue;   // Field boundary
                            }
                        //End of Change for CSBR 140363
                    }
                    if (listField == null)
                    {
                        // Assuming that blank lines are after 'M END'
                        // Accumulate molecule
                        if (strLine.Length > 0) rlistMolecule.Add(strLine);
                        continue;
                    }
                    if (strLine.Length == 0)
                    {
                        cBlankLines++;
                        continue;
                    }
                    while (cBlankLines > 0)
                    {
                        listField.Add(string.Empty);  // Blank lines embedded in the field
                        cBlankLines--;
                    }
                    if ((listField.Count > 0) && ((listField[listField.Count - 1].Length % 80) == 0))
                    {
                        listField[listField.Count - 1] += strLine;
                    }
                    else
                    {
                        listField.Add(strLine);
                    }
                }
                else
                    //end of record
                    return HasMessages;
            }
            return HasMessages;
        }

        /// <summary>
        /// Moves the file reader back to the beginning of the file.
        /// </summary>
        protected void Rewind()
        {
            _oCOETextReader.Position = 0;
        }

        /// <summary>
        /// From: 
        /// http://weblogs.asp.net/erwingriekspoor/archive/2009/05/01/convert-string-to-byte-array-and-byte-array-to-string.aspx
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToBytes(string input)
        {
            byte[] inputAsBytes = System.Text.Encoding.UTF8.GetBytes(input);
            string base64Input = Convert.ToBase64String(inputAsBytes);

            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(input);
                writer.Flush();
            }
            return stream.ToArray();
        }
    }
}
