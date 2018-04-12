using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Windows.Common;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    /// <summary>
    /// <see cref="InputObject"/> for CFW databases
    /// </summary>
    class InputObjectCfw : InputObject
    {
        private InputObjectMdb _oInputObjectMdb;
        private InputObjectMst _oInputObjectMst;
        private bool _bJoinedToMst; // true iif mdb + mst
        private string _strJoinedToMst; // column used to join mdb to mst

        /// <summary>
        /// Constructor
        /// </summary>
        public InputObjectCfw()
        {
            Filter = "CambridgeSoft ChemFinder databases (*.cfx;*.cfw)|*.cfx;*.cfw";
            _oInputObjectMdb = new InputObjectMdb();
            _oInputObjectMst = new InputObjectMst();
            IsValid = _oInputObjectMdb.IsValid && _oInputObjectMst.IsValid;
            if (IsValid == false)
            {
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, -1, "ChemFinder databases not available.");
                AddMessages(_oInputObjectMdb.MessageList);
                AddMessages(_oInputObjectMst.MessageList);
            }
        }

        #region property overrides

        public override bool ExternalSortRequired
        {
            get
            {
                return _oInputObjectMst.ExternalSortRequired;
            }
        }

        public override string InputFieldSort
        {
            get
            {
                return base.InputFieldSort;
            }
            set
            {
                base.InputFieldSort = value;
                if (value != string.Empty)
                {
                    XmlDataDocument oXmlDataDocumentCfw = new XmlDataDocument();
                    XmlDataDocument oXmlDataDocumentMdb = new XmlDataDocument();
                    XmlDataDocument oXmlDataDocumentMst = new XmlDataDocument();
                    oXmlDataDocumentCfw.LoadXml(value);
                    XmlNode oXmlNodeRootCfw = oXmlDataDocumentCfw.DocumentElement;
                    oXmlDataDocumentMdb.AppendChild(oXmlDataDocumentMdb.ImportNode(oXmlNodeRootCfw, false));
                    XmlNode oXmlNodeRootMdb = oXmlDataDocumentMdb.DocumentElement;
                    oXmlDataDocumentMst.AppendChild(oXmlDataDocumentMst.ImportNode(oXmlNodeRootCfw, false));
                    XmlNode oXmlNodeRootMst = oXmlDataDocumentMst.DocumentElement;
                    foreach (XmlNode oXmlNodeCfw in oXmlNodeRootCfw)
                    {
                        string dbname = oXmlNodeCfw.Attributes["dbname"].Value.ToString();
                        if (dbname.StartsWith("ms_"))
                        {
                            XmlNode oXmlNodeMst = oXmlDataDocumentMst.ImportNode(oXmlNodeCfw, true);
                            oXmlNodeMst.Attributes["dbname"].Value = oXmlNodeMst.Attributes["dbname"].Value.ToString().Substring(3);
                            oXmlNodeRootMst.AppendChild(oXmlNodeMst);
                        }
                        else
                        {
                            oXmlNodeRootMdb.AppendChild(oXmlDataDocumentMdb.ImportNode(oXmlNodeCfw, true));
                        }
                    } // foreach (XmlNode oXmlNodeCfw in oXmlNodeRootCfw)
                    _oInputObjectMdb.InputFieldSort = (oXmlDataDocumentMdb.ChildNodes[0].ChildNodes.Count > 0) ? oXmlDataDocumentMdb.OuterXml : string.Empty;
                    _oInputObjectMst.InputFieldSort = (oXmlDataDocumentMst.ChildNodes[0].ChildNodes.Count > 0) ? oXmlDataDocumentMst.OuterXml : string.Empty;
                }
                else
                {
                    _oInputObjectMdb.InputFieldSort = value;
                    _oInputObjectMst.InputFieldSort = value;
                }
                return;
            }
        }

        public override string InputFieldSpec
        {
            get
            {
                return base.InputFieldSpec;
            }
            set
            {
                base.InputFieldSpec = value;
                if (value != string.Empty)
                {
                    XmlDataDocument oXmlDataDocumentCfw = new XmlDataDocument();
                    XmlDataDocument oXmlDataDocumentMdb = new XmlDataDocument();
                    XmlDataDocument oXmlDataDocumentMst = new XmlDataDocument();
                    oXmlDataDocumentCfw.LoadXml(value);
                    XmlNode oXmlNodeRootCfw = oXmlDataDocumentCfw.DocumentElement;
                    oXmlDataDocumentMdb.AppendChild(oXmlDataDocumentMdb.ImportNode(oXmlNodeRootCfw, false));
                    XmlNode oXmlNodeRootMdb = oXmlDataDocumentMdb.DocumentElement;
                    oXmlDataDocumentMst.AppendChild(oXmlDataDocumentMst.ImportNode(oXmlNodeRootCfw, false));
                    XmlNode oXmlNodeRootMst = oXmlDataDocumentMst.DocumentElement;
                    foreach (XmlNode oXmlNodeCfw in oXmlNodeRootCfw)
                    {
                        string dbname = oXmlNodeCfw.Attributes["dbname"].Value.ToString();
                        if (dbname.StartsWith("ms_"))
                        {
                            XmlNode oXmlNodeMst = oXmlDataDocumentMst.ImportNode(oXmlNodeCfw, true);
                            oXmlNodeMst.Attributes["dbname"].Value = oXmlNodeMst.Attributes["dbname"].Value.ToString().Substring(3);
                            oXmlNodeRootMst.AppendChild(oXmlNodeMst);
                        }
                        else
                        {
                            oXmlNodeRootMdb.AppendChild(oXmlDataDocumentMdb.ImportNode(oXmlNodeCfw, true));
                        }
                    } // foreach (XmlNode oXmlNodeCfw in oXmlNodeRootCfw)
                    _oInputObjectMdb.InputFieldSpec = oXmlDataDocumentMdb.OuterXml;
                    _oInputObjectMst.InputFieldSpec = oXmlDataDocumentMst.OuterXml;
                }
                else
                {
                    _oInputObjectMdb.InputFieldSpec = value;
                    _oInputObjectMst.InputFieldSpec = value;
                }
                return;
            }
        }

        protected string JoinedToMstDbName
        {
            get
            {
                return (_strJoinedToMst == null) ? "Mol_ID" : _strJoinedToMst;
            }
            set
            {
                _strJoinedToMst = value;
            }
        }

        public override string Mappings
        {
            set
            {
                string xmlMappings = value;
                string xmlMappingsMdb;
                string xmlMappingsMst;
                if (xmlMappings != string.Empty)
                {
                    XmlDocument oXmlDocumentMappings = new XmlDocument();
                    oXmlDocumentMappings.LoadXml(xmlMappings);
                    XmlNode oXmlNodeFieldlists = oXmlDocumentMappings.DocumentElement;
                    XmlDocument oXmlDocumentMappingsMdb = new XmlDocument();
                    XmlNode oXmlNodeFieldlistsMdb = oXmlDocumentMappingsMdb.CreateElement("fieldlists");
                    oXmlDocumentMappingsMdb.AppendChild(oXmlNodeFieldlistsMdb);
                    XmlDocument oXmlDocumentMappingsMst = new XmlDocument();
                    XmlNode oXmlNodeFieldlistsMst = oXmlDocumentMappingsMst.CreateElement("fieldlists");
                    oXmlDocumentMappingsMst.AppendChild(oXmlNodeFieldlistsMst);
                    XmlNode oXmlNodeFieldlistMdb = null;
                    XmlNode oXmlNodeFieldlistMst = null;
                    bool bMol_ID = false;
                    bool bMst = false;
                    {
                        XmlNodeList oXmlNodeListMaplist = oXmlNodeFieldlists.SelectNodes("descendant::field[attribute::fields]");
                        foreach (XmlNode oXmlNodeField in oXmlNodeListMaplist)
                        {
                            string strNameList = oXmlNodeField.Attributes["fields"].Value;
                            string strNameListMdb = string.Empty;
                            string strNameListMst = string.Empty;
                            string[] strNames = strNameList.Split(';');
                            for (int n = 0; n < strNames.Length; n++)
                            {
                                string strDbName = NameGetDbName(strNames[n]);
                                if (strDbName.StartsWith("ms_")) // Mst
                                {
                                    if (strNameListMst != string.Empty) strNameListMst += ';';
                                    strNameListMst += strNames[n];
                                    bMst = true;
                                }
                                else // Mdb
                                {
                                    if (strNameListMdb != string.Empty) strNameListMdb += ';';
                                    strNameListMdb += strNames[n];
                                    bMol_ID |= (strDbName == JoinedToMstDbName);
                                }
                            } // for (int n = 0; n < strDbNames.Length; n++ )
                            string strFormName = oXmlNodeField.ParentNode.Attributes["name"].Value.ToString();
                            if (strNameListMst != string.Empty)
                            {
                                XmlNode oXmlNodeCopy = oXmlDocumentMappingsMst.ImportNode(oXmlNodeField, false);
                                oXmlNodeCopy.Attributes["fields"].Value = strNameListMst;
                                oXmlNodeFieldlistMst = oXmlNodeFieldlistsMst.SelectSingleNode("child::fieldlist[attribute::name=" + "'" + strFormName + "'" + "]");
                                if (oXmlNodeFieldlistMst == null)
                                {
                                    oXmlNodeFieldlistMst = oXmlDocumentMappingsMst.CreateElement("fieldlist");
                                    XmlAttribute oXmlAttributeMst = oXmlDocumentMappingsMst.CreateAttribute("name");
                                    oXmlAttributeMst.Value = strFormName;
                                    oXmlNodeFieldlistMst.Attributes.Append(oXmlAttributeMst);
                                    oXmlNodeFieldlistsMst.AppendChild(oXmlNodeFieldlistMst);
                                }
                                oXmlNodeFieldlistMst.AppendChild(oXmlNodeCopy);
                            }
                            if (strNameListMdb != string.Empty)
                            {
                                XmlNode oXmlNodeCopy = oXmlDocumentMappingsMdb.ImportNode(oXmlNodeField, false);
                                oXmlNodeCopy.Attributes["fields"].Value = strNameListMdb;
                                oXmlNodeFieldlistMdb = oXmlNodeFieldlistsMdb.SelectSingleNode("child::fieldlist[attribute::name=" + "'" + strFormName + "'" + "]");
                                if (oXmlNodeFieldlistMdb == null)
                                {
                                    oXmlNodeFieldlistMdb = oXmlDocumentMappingsMdb.CreateElement("fieldlist");
                                    XmlAttribute oXmlAttributeMdb = oXmlDocumentMappingsMdb.CreateAttribute("name");
                                    oXmlAttributeMdb.Value = strFormName;
                                    oXmlNodeFieldlistMdb.Attributes.Append(oXmlAttributeMdb);
                                    oXmlNodeFieldlistsMdb.AppendChild(oXmlNodeFieldlistMdb);
                                }
                                oXmlNodeFieldlistMdb.AppendChild(oXmlNodeCopy);
                            }

                        } // foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                    }
                    if (bMst && !bMol_ID)
                    {
                        XmlNode oXmlNode = oXmlDocumentMappingsMdb.CreateElement("field");
                        {
                            XmlAttribute oXmlAttribute = oXmlDocumentMappingsMdb.CreateAttribute("type");
                            oXmlAttribute.Value = "Binary";
                            oXmlNode.Attributes.Append(oXmlAttribute);
                        }
                        {
                            XmlAttribute oXmlAttribute = oXmlDocumentMappingsMdb.CreateAttribute("source");
                            oXmlAttribute.Value = "map";
                            oXmlNode.Attributes.Append(oXmlAttribute);
                        }
                        {
                            XmlAttribute oXmlAttribute = oXmlDocumentMappingsMdb.CreateAttribute("value");
                            oXmlAttribute.Value = JoinedToMstDbName;  // Mdb
                            oXmlNode.Attributes.Append(oXmlAttribute);
                        }
                        {
                            XmlAttribute oXmlAttribute = oXmlDocumentMappingsMdb.CreateAttribute("fields");
                            oXmlAttribute.Value = JoinedToMstDbName;  // Mdb
                            oXmlNode.Attributes.Append(oXmlAttribute);
                        }
                        if (oXmlNodeFieldlistMdb == null)
                        {
                            oXmlNodeFieldlistMdb = oXmlDocumentMappingsMdb.CreateElement("fieldlist");
                            XmlAttribute oXmlAttributeMdb = oXmlDocumentMappingsMdb.CreateAttribute("name");
                            oXmlAttributeMdb.Value = string.Empty;    // ???
                            oXmlNodeFieldlistMdb.Attributes.Append(oXmlAttributeMdb);
                            oXmlNodeFieldlistsMdb.AppendChild(oXmlNodeFieldlistMdb);
                        }
                        oXmlNodeFieldlistMdb.AppendChild(oXmlNode); // Note: Most recent Fieldlist is OK
                    }
                    xmlMappingsMdb = oXmlDocumentMappingsMdb.OuterXml;
                    xmlMappingsMst = oXmlDocumentMappingsMst.OuterXml;
                }
                else
                {
                    xmlMappingsMdb = xmlMappings;
                    xmlMappingsMst = xmlMappings;
                }
                _oInputObjectMdb.Mappings = xmlMappingsMdb;
                _oInputObjectMst.Mappings = xmlMappingsMst;
            }
        }

        public override COEProgressHelper Ph
        {
            set
            {
                base.Ph = value;
                _oInputObjectMdb.Ph = Ph;
                _oInputObjectMst.Ph = Ph;
                return;
            }
        }

        #endregion

        public override void CloseDb()
        {
            _oInputObjectMdb.CloseDb();
            _oInputObjectMst.CloseDb();
            return;
        }

        public override bool OpenDb()
        {
            ClearMessages();
            do
            {
                // WJC filenames really ought to come from the .cfw file
                string strFilenameMdb = Path.ChangeExtension(Db, ".mdb");
                string strFilenameMst = Path.ChangeExtension(Db, ".mst");
                // Exists?
                if (File.Exists(strFilenameMdb) == false)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Unable to open the database because the .mdb file does not exist: " + strFilenameMdb);
                    break;  // ERROR
                }
                if (File.Exists(strFilenameMst) == false)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Input, 0, "Unable to open the database because the .mst file does not exist: " + strFilenameMst);
                    break;  // ERROR
                }
                // OpenDb
                _oInputObjectMdb.Db = strFilenameMdb;
                if (_oInputObjectMdb.OpenDb())
                {
                    AddMessages(_oInputObjectMdb.MessageList);
                    break;  // ERROR
                }
                _oInputObjectMst.Db = strFilenameMst;
                if (_oInputObjectMst.OpenDb())
                {
                    AddMessages(_oInputObjectMst.MessageList);
                    break;  // ERROR
                }
                // TableList
                ClearTableList();
                foreach (string strTable in _oInputObjectMdb.TableList)
                {
                    AddTableToTableList(strTable);
                }
            } while (false);
            // WJC TODO on error close iff open
            return HasMessages;
        }

        public override bool CloseTable()
        {
            ClearMessages();
            do
            {
                if (_oInputObjectMdb.CloseTable())
                {
                    AddMessages(_oInputObjectMdb.MessageList);
                    break;  // ERROR
                }
                if (_oInputObjectMst.CloseTable())
                {
                    AddMessages(_oInputObjectMst.MessageList);
                    break;  // ERROR
                }
                _bJoinedToMst = false;
                JoinedToMstDbName = string.Empty;
                base.CloseTable();
            } while (false);
            return HasMessages;
        }

        public override bool OpenTable()
        {
            ClearMessages();
            do
            {
                // OpenTable
                _oInputObjectMdb.Table = Table;
                if (_oInputObjectMdb.OpenTable())
                {
                    AddMessages(_oInputObjectMdb.MessageList);
                    break;  // ERROR
                }
                if (_oInputObjectMst.OpenTable())
                {
                    AddMessages(_oInputObjectMst.MessageList);
                    break;  // ERROR
                }
                Record = 0;
                Records = _oInputObjectMdb.Records; // Exact
                // if (InputFieldSpec.Length == 0) unconditional, for reexecution
                {
                    // Field list
                    // WJC treat Mol_ID and Structure specially.
                    // WJC Append ms Field list. Keep separate counts.
                    XmlDocument oXmlDocumentCfw = new XmlDocument();
                    oXmlDocumentCfw.LoadXml(_oInputObjectMdb.InputFieldSpec);
                    XmlNode oXmlNodeFieldlistCfw = oXmlDocumentCfw.DocumentElement;
                    foreach (XmlNode oXmlNodeField in oXmlNodeFieldlistCfw)
                    {
                        XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["dbname"];
                        if ((oXmlAttribute != null) && (oXmlAttribute.Value.ToLower() == "mol_id"))
                        {
                            _bJoinedToMst = true;
                            JoinedToMstDbName = oXmlAttribute.Value;
                        }
                    }
                    if (_bJoinedToMst)
                    {
                        _oInputObjectMst.Table = _oInputObjectMst.TableList[0];
                        XmlDocument oXmlDocumentMst = new XmlDocument();
                        oXmlDocumentMst.LoadXml(_oInputObjectMst.InputFieldSpec);
                        XmlNode oXmlNodeFieldlistMst = oXmlDocumentMst.DocumentElement;
                        foreach (XmlNode oXmlNode in oXmlNodeFieldlistMst)
                        {
                            if (oXmlNode.Attributes["dbname"].Value == "Mol_ID")    // Mst
                            {
                                continue;
                            }
                            XmlNode oXmlNodeCopy = oXmlDocumentCfw.ImportNode(oXmlNode, false);
                            oXmlNodeCopy.Attributes["dbname"].Value = "ms_" + oXmlNodeCopy.Attributes["dbname"].Value;
                            oXmlNodeFieldlistCfw.AppendChild(oXmlNodeCopy);
                        }
                        InputFieldSpec = oXmlDocumentCfw.OuterXml;
                    }
                    else
                    {
                        InputFieldSpec = _oInputObjectMdb.InputFieldSpec;
                    }
                } // if (InputFieldSpec.Length == 0)
            } while (false);
            return HasMessages;
        }

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            if (_oInputObjectMdb.OpenDataSet(vnStart, vcLimit))
            {
                AddMessages(_oInputObjectMdb.MessageList);
            }
            if (_oInputObjectMst.OpenDataSet(vnStart, vcLimit))
            {
                AddMessages(_oInputObjectMst.MessageList);
            }
            DataTable oDataTableMst = _oInputObjectMst.DataSetForJob.Tables[0];
            int cJoinedToMst = oDataTableMst.Columns.Count;
            if (cJoinedToMst > 0)
            {
                DataTable oDataTableMdb = _oInputObjectMdb.DataSetForJob.Tables[0];
                foreach (DataColumn oDataColumnMst in oDataTableMst.Columns)
                {
                    DataColumn oDataColumnMdb = new DataColumn();
                    oDataColumnMdb.ColumnName = "ms_" + oDataColumnMst.ColumnName;
                    oDataColumnMdb.DataType = oDataColumnMst.DataType;
                    oDataTableMdb.Columns.Add(oDataColumnMdb);
                }
            }
            Minimum = Value = vnStart;
            Maximum = ((vcLimit != int.MaxValue) ? Minimum : 0) + vcLimit;
            if (Maximum > Records) Maximum = Records;
            return HasMessages;
        }

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            if (_oInputObjectMdb.ReadDataSet(vcLimit, ref riDataSet))
            {
                AddMessages(_oInputObjectMdb.MessageList);
            }
            // WJC TODO test for failure
            if (_bJoinedToMst && riDataSet.Tables[0].Columns.Contains(JoinedToMstDbName))    // Mdb
            {
                DataTable oDataTable = riDataSet.Tables[0];
                int[] nMol_IDs = new int[oDataTable.Rows.Count];
                // Build list of Mol_IDs
                {
                    DataColumn oDataColumnMol_ID = oDataTable.Columns[JoinedToMstDbName]; // Mdb
                    for (int nRow = 0; nRow < oDataTable.Rows.Count; nRow++)    // Progress?
                    {
                        DataRow oDataRow = oDataTable.Rows[nRow];
                        string strMol_ID = oDataRow[oDataColumnMol_ID].ToString();
                        int nMol_ID = (strMol_ID.Length > 0) ? Convert.ToInt32(strMol_ID) : 0;
                        nMol_IDs[nRow] = nMol_ID;
                    }
                }
                // Fetch Mol_IDs
                DataSet DataSetMst = _oInputObjectMst.ArrayReadDataSet(nMol_IDs, oDataTable.Rows.Count);
                DataTable oDataTableMst = DataSetMst.Tables[0];

                int cColsMdb = oDataTable.Columns.Count - oDataTableMst.Columns.Count;
                // Append data
                {
                    for (int nRow = 0; nRow < oDataTableMst.Rows.Count; nRow++) // Progress?
                    {
                        DataRow oDataRow = oDataTable.Rows[nRow];
                        DataRow oDataRowMst = oDataTableMst.Rows[nRow];
                        for (int nCol = 0; nCol < oDataTableMst.Columns.Count; nCol++)
                        {
                            oDataRow[cColsMdb + nCol] = oDataRowMst[nCol];
                        }
                    }
                }
            }
            Value += riDataSet.Tables[0].Rows.Count;
            return HasMessages;
        }

        protected override DataSet ReadDataSetForPreview()
        {
            DataSet DataSetRet = _oInputObjectMdb.DataSetForPreview;
            Boolean Check=false;
            // this will add new columns
            if (_bJoinedToMst)
            {
                DataTable oDataTable = DataSetRet.Tables[0];
                int[] nMol_IDs = new int[oDataTable.Rows.Count];
                // Build list of Mol_IDs
                {
                    DataColumn oDataColumnMol_ID = oDataTable.Columns[JoinedToMstDbName]; // Mdb
                    for (int nRow = 0; nRow < oDataTable.Rows.Count; nRow++)    // Progress?
                    {
                        DataRow oDataRow = oDataTable.Rows[nRow];
                        string strMol_ID = oDataRow[oDataColumnMol_ID].ToString();
                        int nMol_ID = (strMol_ID.Length > 0) ? Convert.ToInt32(strMol_ID) : 0;
                        nMol_IDs[nRow] = nMol_ID;
                    }
                }
                // Fetch Mol_IDs
                DataSet DataSetMst = _oInputObjectMst.ArrayGetDataSetPreview(nMol_IDs, oDataTable.Rows.Count);
                DataTable oDataTableMst = DataSetMst.Tables[0];

                // Append DataColumns
                int cColsMdb = oDataTable.Columns.Count;
                int cColumnsMst = oDataTableMst.Columns.Count;
                for (int nCol = 0; nCol < cColumnsMst; nCol++)
                {
                    DataColumn oDataColumn = oDataTableMst.Columns[nCol];
                    oDataColumn.ColumnName = "ms_" + oDataColumn.ColumnName;
                    Check = CheckExistingColumns(oDataColumn, oDataTable);
                    if(Check == false)
                    oDataTable.Columns.Add(oDataColumn.ColumnName, oDataColumn.DataType, oDataColumn.Expression);
                }
                
                // Append data
                if (Check == false)
                {
                    for (int nRow = 0; nRow < oDataTableMst.Rows.Count; nRow++) // Progress?
                    {
                        DataRow oDataRow = oDataTable.Rows[nRow];
                        DataRow oDataRowMst = oDataTableMst.Rows[nRow];
                        for (int nCol = 0; nCol < cColumnsMst; nCol++)
                        {
                            oDataRow[cColsMdb + nCol] = oDataRowMst[nCol];
                        }
                    }
                }
            }
            return DataSetRet;
        }
        public Boolean CheckExistingColumns(DataColumn dc, DataTable dt)
        {
            DataColumnCollection columns = dt.Columns;
            if (columns.Contains(dc.ColumnName))
                return true;
            else
                return false;
        }

    }
}
