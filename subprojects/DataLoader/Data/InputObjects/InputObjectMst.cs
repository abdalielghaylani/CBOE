using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;   // DllImport
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    /// <summary>
    /// <see cref="InputObject"/> for MST databases
    /// </summary>
    class InputObjectMst : InputObject
    {

        [DllImport("USER32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int RegisterClipboardFormat(string format);

#if RESEARCH
        [DllImport("USER32.DLL", EntryPoint = "GetClipboardFormatName")]
        static extern int GetClipboardFormatName(uint format, [Out] StringBuilder lpszFormatName, int cchMaxCount);
#endif

        #region data

        private MolServer.Document _msDocument;
        private int _eChemDraw_InterchangeFormat = RegisterClipboardFormat("ChemDraw Interchange Format");
        //private int _eCS_ChemFinder_Native = RegisterClipboardFormat("CS_ChemFinder_Native");
        private Dictionary<string, Object> _dicFields;
        private int[] _nIDs;
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
            }
        } // Db

        #endregion


        #region constructors

        public InputObjectMst()
        {
            Filter = "CambridgeSoft Molecule databases (*.mst)|*.mst";
            try
            {
                _msDocument = new MolServer.Document();
                IsValid = true;
            }
            catch (Exception)
            {
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, -1, "MolServer not available.");
                IsValid = false;   // Silent failure for now
            }
            if (IsValid)
            {
                _dicFields = new Dictionary<string, Object>();
            }
            return;
        } // InputObjectMst()

        #endregion

        #region methods

        private void ArrayFieldSpecFillDataTable(DataTable voDataTable, int[] vnMol_IDs, int nStart, int vcLimit, string vxmlFieldSpec, bool vbShowProgress)
        {
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(vxmlFieldSpec);
            XmlNode oXmlNodeFieldlist = oXmlDocument.SelectSingleNode("fieldlist");
            foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
            {
                string strColumnName = oXmlNode.Attributes["dbname"].Value.ToString();
                string strColumnType = oXmlNode.Attributes["dbtype"].Value.ToString();
                DataColumn oDataColumn = new DataColumn();
                oDataColumn.DataType = Type.GetType(strColumnType);
                oDataColumn.ColumnName = strColumnName;
                voDataTable.Columns.Add(oDataColumn);
            }
            {
                for (int nRow = 0; nRow < vcLimit; nRow++)
                {
                    if (Ph.IsRunning)
                    {
                        if (Ph.CancellationPending && vbShowProgress) break;    // Note that cancellation is tied to show progress
                        if (vbShowProgress)
                        {
                            Ph.Value = nRow;
                            Ph.StatusText = "Loading mst records. Record " + (1 + nStart + nRow) + " of " + Ph.Maximum;
                        }
                    }
                    Record++;
                    DataRow oDataRow = voDataTable.NewRow();
                    int nMol_ID = vnMol_IDs[nStart + nRow];
                    if (nMol_ID > 0)
                    {
                        GetMol(nMol_ID);
                        foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                        {
                            string strColumnName = oXmlNode.Attributes["dbname"].Value.ToString();
                            oDataRow[strColumnName] = _dicFields[strColumnName];
                        }
                    }
                    voDataTable.Rows.Add(oDataRow);
                }
            }
            return;
        } // ArrayFieldSpecFillDataTable()

        public DataSet ArrayGetDataSetPreview(int[] vnMol_IDs, int vcLimit)
        {
            DataSet oDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            oDataSet.Tables.Add(oDataTable);
            Ph.Minimum = Minimum = Value = 0;
            Ph.Maximum = Maximum = vcLimit;
            Ph.CancelConfirmation = "If you stop this operation then not all records will be available to preview";
            Ph.ProgressSection(delegate() /* InputObjectMst::ArrayFieldSpecFillDataTable (Records, Cancel) */
            {
                ArrayFieldSpecFillDataTable(oDataTable, vnMol_IDs, 0, vcLimit, InputFieldSpec, true);
            });
            return oDataSet;
        } // ArrayGetDataSetPreview()

        public DataSet ArrayReadDataSet(int[] vnMol_IDs, int vcLimit)
        {
            DataSet oDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            oDataSet.Tables.Add(oDataTable);
            // No progress. Used by Cfw to append Mst fields
            ArrayFieldSpecFillDataTable(oDataTable, vnMol_IDs, 0, vcLimit, InputFieldSpecMapped, false);
            return oDataSet;
        } // ArrayReadDataSet()

        public override void CloseDb()
        {
            if (_msDocument.IsOpen)
            {
                _msDocument.Close();
            }
            return;
        } // CloseDb()

        private void FieldValuesReset()
        {
            _dicFields["CanonicalCode"] = (String)"(error)";
            _dicFields["Formula"] = (String)"(error)";
            //_dicFields["Index"] = (Int16)0; // Not useful? Always zero?
            _dicFields["IsReaction"] = (Boolean)false;
            _dicFields["MolWeight"] = (Double)(-1);
            _dicFields["Mol_ID"] = (Int32)(-1); // Was Int16
            _dicFields["Name"] = (String)"(error)";
            _dicFields["Structure"] = new Byte[1];
            _dicFields["Type"] = (String)"(error)";
            return;
        } // FieldValuesReset()

        public void GetMol(int vnMol_ID)
        {
            do
            {
                MolServer.Molecule msMolecule;
                try
                {
                    msMolecule = _msDocument.GetMol(vnMol_ID);
                }
                catch (Exception ex)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, -1, "Exception: _msDocument.GetMol(): " + ex.ToString());
                    break;  // Error
                }
                if (msMolecule == null)
                {
                    FieldValuesReset();
                    break;  // Error
                }
                // Com objects
                //string strAtoms = msMolecule.Atoms.ToString();
                //string strBonds = msMolecule.Bonds.ToString();
                //string strDataObject = msMolecule.DataObject.ToString();
                //string strDisplayOptions = msMolecule.DisplayOptions.ToString();
                //string strMolecules = msMolecule.Molecules.ToString();
                //string strParent = msMolecule.Parent.ToString();  // Must be set, default is null

                // Properties
                _dicFields["CanonicalCode"] = msMolecule.CanonicalCode;
                _dicFields["Formula"] = msMolecule.Formula;
                //_dicFields["Index"] = msMolecule.index;
                _dicFields["IsReaction"] = msMolecule.IsReaction;
                _dicFields["MolWeight"] = msMolecule.MolWeight;
                _dicFields["Name"] = msMolecule.Name;
                {
                    string strType;
                    switch (msMolecule.Type)
                    {
                        case MolServer.CFMoleculeType.kCFIntermediate: { strType = "Intermediate"; break; }
                        case MolServer.CFMoleculeType.kCFMolecule: { strType = "Molecule"; break; }
                        case MolServer.CFMoleculeType.kCFProduct: { strType = "Product"; break; }
                        case MolServer.CFMoleculeType.kCFReactant: { strType = "Reactant"; break; }
                        case MolServer.CFMoleculeType.kCFReaction: { strType = "Reaction"; break; }
                        default: { strType = "(unknown)"; break; }
                    }
                    _dicFields["Type"] = strType;
                }

                //  string strProperty = msMolecule.GetPhysicalProperty("which"); // I don't know if it makes any sense to expose this

#if DEBUGSDF
                {
                    string strFilenameSdf = "C:\\Molecule.sdf";  // WJC need to build temporary path & filename
                    MolServer.Hitlist msHitlist = new MolServer.Hitlist();
                    msHitlist.AddHit(vnMol_ID);
                    _msDocument.WriteSDFile(strFilenameSdf, msHitlist);
                    File.Delete(strFilenameSdf);
                }
#endif
                byte[] structureData = null;
#if RESEARCH
                string strName = new string(' ', 255);
                StringBuilder sb = new StringBuilder(strName, strName.Length);
                int nRet = GetClipboardFormatName(50051, sb, strName.Length);
#endif
                {
                    MolServer.DataObject msDataObject = msMolecule.DataObject;
                    byte[] oData = (byte[])msDataObject.GetData(_eChemDraw_InterchangeFormat);
                    int nDepth = 0; // Object depth
                    int nOffset = 0;
                    nOffset += 8;   // VjCD0101
                    nOffset += 4;   // 04 03 02 01
                    nOffset += 10;  // zeroes
                    while (nOffset < oData.Length)
                    {
                        int nTag = oData[nOffset] + (oData[nOffset + 1] << 8); nOffset += 2;
                        if (nTag == 0)
                        {
                            if (nDepth == 0)
                            {
                                break;
                            }
                            nDepth--;
                            continue;
                        }
                        if ((nTag & 0x8000) == 0x8000)
                        {
                            nDepth++;
                            nOffset += 4;   // Object identifier
                        }
                        else
                        {
                            int nLength = oData[nOffset] + (oData[nOffset + 1] << 8); nOffset += 2;
                            if (nLength == 0xFFFF)
                            {
                                nLength = oData[nOffset] + (oData[nOffset + 1] << 8) + (oData[nOffset + 2] << 16) + (oData[nOffset + 3] << 24); nOffset += 4;
                            }
                            nOffset += nLength;
                        }
                    } // while (nOffset < oData.Length)
                    structureData = new byte[nOffset];
                    System.Array.Copy(oData, structureData, nOffset);
                    //oData = msDataObject.GetData(-15485);
                }
#if OBSOLETE
                byte[] structureDataSlow = null;
                {
                    string strFilenameCdx = "C:\\Molecule.cdx";  // WJC need to build temporary path & filename
                    File.Delete(strFilenameCdx);
                    // If it still exists we're headed for trouble !
                    msMolecule.Write(strFilenameCdx, null, null);
                    structureDataSlow = File.ReadAllBytes(strFilenameCdx);
                    File.Delete(strFilenameCdx);
                }
                if (structureData.Length != structureDataSlow.Length)
                {
                    Console.WriteLine("Arg!");
                }
                else
                {
                    for (int nOffset = 0; nOffset < structureData.Length; nOffset++)
                    {
                        if (structureData[nOffset] != structureDataSlow[nOffset])
                        {
                            Console.WriteLine("Arg!");
                            break;
                        }
                    }
                }
#endif
                // aha! string strBase64 = Convert.ToBase64String(structureDataSlow);
                _dicFields["Structure"] = structureData;
            } while (false);
            _dicFields["Mol_ID"] = vnMol_ID;
            return;
        } // GetMol

        public override bool OpenDb()
        {
            ClearMessages();
            do
            {
                if (_msDocument.IsOpen != true)
                {
                    try
                    {
                        _msDocument.Open(this._strDb, Convert.ToInt32(MolServer.MSOpenModes.kMSReadOnly), string.Empty);
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
                    AddTableToTableList("MstTable");  // Just to indicate there there is a single table
                }
            } while (false);
            return HasMessages;
        } // OpenDb()

        public override bool CloseTable()
        {
            ClearMessages();
            do
            {
                if (_msDocument.IsOpen == false)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
                    break;  // Error
                }
                _dicFields.Clear();
                _nIDs = null;
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
                if (_msDocument.IsOpen == false)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Database is not open");
                    break;  // Error
                }
                if (InputFieldSpec.Length == 0)
                {
                    // Table? (n/a)
                    // Build FieldList and TypeList
                    {
                        _dicFields.Clear();
                        FieldValuesReset();
                        COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                        oCOEXmlTextWriter.WriteStartElement("fieldlist");
                        foreach (KeyValuePair<string, Object> kvp in _dicFields)
                        {
                            oCOEXmlTextWriter.WriteStartElement("field");
                            oCOEXmlTextWriter.WriteAttributeString("dbname", kvp.Key);
                            oCOEXmlTextWriter.WriteAttributeString("dbtype", kvp.Value.GetType().ToString());
                            oCOEXmlTextWriter.WriteAttributeString("dbtypereadonly", "true");
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                        InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                        oCOEXmlTextWriter.Close();
                    }
                } else if (_dicFields.Count == 0) {
                    FieldValuesReset();
                }
                // Count / cache
                if (_nIDs == null)
                {
                    MolServer.searchInfo msSearchInfo = new MolServer.searchInfo();
                    msSearchInfo.FragmentCountRange = 100 << 8 | 1;
                    MolServer.Search msSearch;
                    try
                    {
                        msSearch = _msDocument.CreateSearchObject(msSearchInfo);
                    }
                    catch (Exception ex)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Exception: _msDocument.CreateSearchObject(): " + ex.Message);
                        break;  // Error
                    }
                    try
                    {
                        msSearch.Start();
                        do
                        {
                            msSearch.WaitForCompletion(1000);   // WJC hard-coded
                        } while (msSearch.Status == 1); // Progress?
                    }
                    catch (Exception ex)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Exception: msSearch.WaitForCompletion(): " + ex.Message);
                        break;  // Error
                    }
                    try
                    {
                        MolServer.Hitlist msHitList = msSearch.Hitlist;
                        Records = msHitList.Count;  // Exact
                        Ph.Maximum = Records;
                        Ph.SupportsCancellation = false;
                        Ph.ProgressSection(delegate() /* InputObjectMst::OpenTable (cRecords, no Cancel) */
                        {
                            Record = 0;
                            _nIDs = new int[Records];
                            for (int nItem = 0; nItem < Records; nItem++)
                            {
                                if (Ph.CancellationPending) break;
                                Ph.Value = nItem;
                                Ph.StatusText = "Scanning mst file. Record " + (1 + nItem) + " of " + Records;
                                _nIDs[nItem] = msHitList.get_At(nItem);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Exception: msHitList: " + ex.Message);
                        break;  // Error
                    }
                }
            } while (false);
            return HasMessages;
        } // OpenTable()

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            DataSetForJob = new DataSet(Table + "List");
            DataSetForJob.Tables.Add(Table);
            DataTable oDataTable = DataSetForJob.Tables[0];
            Value = 0;
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(InputFieldSpecMapped);
                XmlNode oXmlNodeFieldlist = oXmlDocument.DocumentElement;
                foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                {
                    string strDbName = oXmlNodeField.Attributes["dbname"].Value;
                    string strName = (oXmlNodeField.Attributes["name"] != null) ? oXmlNodeField.Attributes["name"].Value.ToString() : strDbName;
                    DataColumn oDataColumn = new DataColumn();
                    oDataColumn.Caption = strName;
                    oDataColumn.ColumnName = strDbName;
                    oDataColumn.DataType = _dicFields[strDbName].GetType();
                    oDataTable.Columns.Add(oDataColumn);
                }
            }
            Record = 0;
            Value = 0;
            Maximum = Records;
            return HasMessages;
        } // OpenDataSet()

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = new DataSet(Table + "List");
            DataTable oDataTable = new DataTable(Table);
            riDataSet.Tables.Add(oDataTable);
            int cLimit = _nIDs.Length - Record;
            if (cLimit > vcLimit) cLimit = vcLimit;
            // No progress. Handled outside
            ArrayFieldSpecFillDataTable(oDataTable, _nIDs, Record, cLimit, InputFieldSpecMapped, false);
            Record += cLimit;
            Value += cLimit;
            return HasMessages;
        } // ReadDataSet()

        protected override DataSet ReadDataSetForPreview()
        {
            Record = 0;
            return ArrayGetDataSetPreview(_nIDs, Records);
        } // ReadDataSetForPreview()

        #endregion

    } // class InputObjectMst
}
