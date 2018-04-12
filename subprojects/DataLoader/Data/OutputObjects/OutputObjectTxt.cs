using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

// strOutputType
// StartWrite()
// DataSetWrite()

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for writing to TXT files
    /// </summary>
    class OutputObjectTxt : OutputObject
    {
        #region enums and types
        /// <summary>
        /// Indicates how a delimited TXT file is delimited
        /// </summary>
        public enum DelimiterType {
            /// <summary>
            /// Comma delimited
            /// </summary>
            Comma,
            /// <summary>
            /// Tab delimited
            /// </summary>
            Tab
        };

        /// <summary>
        /// Indicates whether a TXT file has a header
        /// </summary>
        public enum HeaderType {
            /// <summary>
            /// Does not have a header
            /// </summary>
            No,
            /// <summary>
            /// Has a header
            /// </summary>
            Yes
        };
        #endregion

        #region data
        private DelimiterType _eDelimiter;
        private HeaderType _eHeader;
        private StreamWriter _oStreamWriter;
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

        public override string Filter
        {
            get
            {
                string strExtension = (Delimiter == DelimiterType.Comma) ? "csv" : "txt";
                strExtension = "*." + strExtension;
                base.Filter = "Text files (" + strExtension + ")|" + strExtension;
                return base.Filter;
            }
        } // Filter

        public HeaderType Header
        {
            get
            {
                return _eHeader;
            }
            private set
            {
                _eHeader = value;
            }
        } // Header

        /// <summary>
        /// Build the output field specification based on an input field specification
        /// </summary>
        public override string InputFieldSpec
        {
            set
            {
                string xmlInputFieldSpec = value;
                if (xmlInputFieldSpec != string.Empty)
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlists");
                    {
                        oCOEXmlTextWriter.WriteStartElement("fieldlist");
                        {
                            oCOEXmlTextWriter.WriteStartAttribute("name");
                            oCOEXmlTextWriter.WriteString(string.Empty);
                            oCOEXmlTextWriter.WriteEndAttribute(); // name
                        }
                        XmlDocument oXmlDocument = new XmlDocument();
                        oXmlDocument.LoadXml(xmlInputFieldSpec);
                        XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                        foreach (XmlNode XmlNodeField in oXmlNodeRoot)
                        {
                            string strName = XmlNodeField.Attributes["name"].Value;
                            string strFormatPrefix = XmlNodeField.Attributes["type"].Value;
                            string strFormatSuffix = string.Empty;
                            {
                                int indexOf = strFormatPrefix.IndexOf('[');
                                if ((indexOf > 0) && (strFormatPrefix.Substring(indexOf, 2) != "[]"))
                                {
                                    strFormatSuffix = strFormatPrefix.Substring(indexOf);
                                    strFormatPrefix = strFormatPrefix.Remove(indexOf);
                                }
                            }
                            oCOEXmlTextWriter.WriteStartElement("field");
                            {
                                oCOEXmlTextWriter.WriteStartAttribute("name");
                                oCOEXmlTextWriter.WriteString(strName);
                                oCOEXmlTextWriter.WriteEndAttribute(); // name
                            }
                            {
                                oCOEXmlTextWriter.WriteStartAttribute("format");
                                oCOEXmlTextWriter.WriteString(strFormatPrefix + strFormatSuffix);
                                oCOEXmlTextWriter.WriteEndAttribute(); // format
                            }
                            {
                                oCOEXmlTextWriter.WriteStartAttribute("specification");
                                oCOEXmlTextWriter.WriteString(string.Empty);
                                oCOEXmlTextWriter.WriteEndAttribute(); // specification
                            }
                            {
                                oCOEXmlTextWriter.WriteStartElement("sources");
                                {
                                    oCOEXmlTextWriter.WriteStartAttribute("default");
                                    oCOEXmlTextWriter.WriteString("map");
                                    oCOEXmlTextWriter.WriteEndAttribute(); // default
                                }
                                oCOEXmlTextWriter.WriteStartElement("map"); oCOEXmlTextWriter.WriteEndElement();
                                oCOEXmlTextWriter.WriteStartElement("none"); oCOEXmlTextWriter.WriteEndElement();
                                if ((strFormatPrefix != "Binary") && (strFormatSuffix == string.Empty))
                                {
                                    oCOEXmlTextWriter.WriteStartElement("constant"); oCOEXmlTextWriter.WriteEndElement();
                                    oCOEXmlTextWriter.WriteStartElement("calculation"); oCOEXmlTextWriter.WriteEndElement();
                                }
                                oCOEXmlTextWriter.WriteEndElement();    // sources
                            }
                            oCOEXmlTextWriter.WriteEndElement();    // field
                        }
                        oCOEXmlTextWriter.WriteEndElement();    // fieldlist
                    }
                    oCOEXmlTextWriter.WriteEndElement();    // fieldlists
                    OutputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
                return;
            }
        } // InputFieldSpec

        #endregion

        #region constructors
        public OutputObjectTxt()
        {
            Header = HeaderType.Yes;
            Delimiter = DelimiterType.Tab;
            OutputType = "Output to a text file";
            IsValid = true;
            {
                COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                oCOEXmlTextWriter.WriteStartElement("OutputConfiguration");
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
                oCOEXmlTextWriter.WriteEndElement();

                UnboundConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                oCOEXmlTextWriter.Close();
            }
            return;
        } // OutputObjectTxt()
        #endregion

        #region methods
        protected override bool DataSetWrite(int vnRecord)
        {
            ClearMessages();
            do
            {
                string strSeparator = (Delimiter == DelimiterType.Comma) ? "," : "\t";
                DataTable oDataTable = OutputDataSet.Tables[0];
                int cRows = oDataTable.Rows.Count;
                int cCols = oDataTable.Columns.Count;
                foreach (DataRow oDataRow in oDataTable.Rows)
                {
                    for (int nCol = 0; nCol < cCols; nCol++)
                    {
                        if (nCol > 0) _oStreamWriter.Write(strSeparator);
                        string strValue;

                        if (oDataTable.Columns[nCol].DataType.FullName == "System.Byte[]")    // WJC make more effecient!
                        {
                            if (oDataRow[nCol].GetType().FullName != "System.DBNull")
                            {
                                byte[] byteStructure = (byte[])oDataRow[nCol];
                                strValue = Convert.ToBase64String(byteStructure);
                            }
                            else
                            {
                                strValue = string.Empty;
                            }
                        }
                        else
                        {
                            strValue = oDataRow[nCol].ToString();
                        }

                        bool bQuote = (Delimiter == DelimiterType.Comma);
                        if (strValue.IndexOfAny(new char[] { '\r', '\n' }) >= 0)
                        {
                            bQuote = true;
                            string[] strLines = strValue.Split(new char[] { '\r', '\n' });
                            strValue = String.Join(";", strLines);
                        }
                        if (bQuote)
                        {
                            strValue = "\"" + strValue.Replace("\"", "\\\"") + "\"";
                        }
                        _oStreamWriter.Write(strValue);
                    } // for (int nCol = 0; nCol < cCols; nCol++)
                    _oStreamWriter.WriteLine();
                } // foreach (DataRow oDataRow in oDataSet.Tables[0].Rows)
            } while (false);
            return HasMessages;
        } // DataSetWrite()

        public override bool EndWrite()
        {
            ClearMessages();
            if (_oStreamWriter != null)
            {
                _oStreamWriter.Close();
                _oStreamWriter = null;
            }
            return HasMessages;
        } // EndWrite()

        protected override bool StartWrite()
        {
            ClearMessages();
            do
            {
                try
                {
                    _oStreamWriter = File.CreateText(Db);
                }
                catch
                {
                    ;
                }
                if (_oStreamWriter == null)
                {
                    AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Output, 0, "Unable to open the output file");
                    break;
                }
                if (Header == HeaderType.Yes)
                {
                    DataTable oDataTable = OutputDataSet.Tables[0];
                    string strSeparator = (Delimiter == DelimiterType.Comma) ? "," : "\t";
                    int cCols = oDataTable.Columns.Count;
                    for (int nCol = 0; nCol < cCols; nCol++)
                    {
                        if (nCol > 0) _oStreamWriter.Write(strSeparator);
                        _oStreamWriter.Write(oDataTable.Columns[nCol].ColumnName);
                    }
                    _oStreamWriter.WriteLine();
                }
            } while (false);
            return HasMessages;
        } // StartWrite()
        #endregion
    } // class OutputObjectTxt
}
