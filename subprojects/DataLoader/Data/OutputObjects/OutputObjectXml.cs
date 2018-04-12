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
    class OutputObjectXml : OutputObject
    {
        private StreamWriter _oStreamWriter;
        DataSet _oDataSet;

        public override string Filter
        {
            get
            {
                string strExtension = "xml";
                strExtension = "*." + strExtension;
                base.Filter = "XML files (" + strExtension + ")|" + strExtension;
                return base.Filter;
            }
        }

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
        }

        public OutputObjectXml()
        {
            OutputType = "Output to an XML file";
            IsValid = true;
            return;
        }

        protected override bool DataSetWrite(int vnRecord)
        {
            ClearMessages();
            do
            {
                DataTable oDataTable = OutputDataSet.Tables[0];
                _oDataSet.Merge(oDataTable);
            } while (false);
            return HasMessages;
        }

        public override bool EndWrite()
        {
            ClearMessages();
            if (_oStreamWriter != null)
            {
                _oDataSet.WriteXml(_oStreamWriter, XmlWriteMode.IgnoreSchema);
                _oStreamWriter.Close();
                _oStreamWriter = null;
            }
            return HasMessages;
        }

        protected override bool StartWrite()
        {
            ClearMessages();
            do
            {
                try
                {
                    _oDataSet = OutputDataSet.Clone();
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
            } while (false);
            return HasMessages;
        }

    }
}
