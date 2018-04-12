using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.ConfigLoader.Windows.Controls;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.COESecurityService;

// strOutputType
// StartWrite()
// DataSetWrite()

namespace CambridgeSoft.COE.ConfigLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for writing to TXT files
    /// </summary>
    class OutputObjectConfigExp : OutputObject
    {

        //Constructor
        public OutputObjectConfigExp()
        {
            OutputType = "Configuration Export";
            IsValid = true;
            return;
        }

        //Properties
        public override string Filter
        {
            get
            {
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
                if (xmlInputFieldSpec != "")
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlists");
                    {
                        oCOEXmlTextWriter.WriteStartElement("fieldlist");
                        {
                            oCOEXmlTextWriter.WriteStartAttribute("name");
                            oCOEXmlTextWriter.WriteString("");
                            oCOEXmlTextWriter.WriteEndAttribute(); // name
                        }
                        XmlDocument oXmlDocument = new XmlDocument();
                        oXmlDocument.LoadXml(xmlInputFieldSpec);
                        XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                        foreach (XmlNode XmlNodeField in oXmlNodeRoot)
                        {
                            string strName = XmlNodeField.Attributes["name"].Value;
                            string strFormatPrefix = XmlNodeField.Attributes["type"].Value;
                            string strFormatSuffix = "";
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
                                oCOEXmlTextWriter.WriteString("");
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
                                if ((strFormatPrefix != "Binary") && (strFormatSuffix == ""))
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
                    OutputFieldSpec = UIBase.FormatXmlString(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
                return;
            }
        }

        protected override bool DataSetWrite(int vnRecord)
        {
            ClearMessages();
            do
            {
                DataTable oDataTable = OutputDataSet.Tables[0];
                int nTransaction = vnRecord;
                foreach (DataRow oDataRow in oDataTable.Rows)
                {
                    nTransaction++;
                    string Xml = oDataRow[0].ToString();
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(Xml);
                    XmlNode oXmlNode = oXmlDocument.DocumentElement;
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["subdirectory"];
                    string subDirectory = oXmlAttribute.Value.ToString();
                    {
                        string strFolder = Path.Combine(Db, subDirectory);
                        if (Directory.Exists(strFolder) == false)
                        {
                            Directory.CreateDirectory(strFolder);
                            if (Directory.Exists(strFolder) == false)
                            {
                                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Unable to create the {0:G} folder", strFolder);
                                continue;
                            }
                        }
                        // oXmlAttribute = oXmlNode.Attributes["databasename", DatabaseName);
                        // oXmlAttribute = oXmlNode.Attributes["description", Description);
                        // oXmlAttribute = oXmlNode.Attributes["formgroup", FormGroup.ToString());
                        oXmlAttribute = oXmlNode.Attributes["id"];
                        string ID = oXmlAttribute.Value.ToString();
                        // oXmlAttribute = oXmlNode.Attributes["ispublic", IsPublic.ToString());
                        // oXmlAttribute = oXmlNode.Attributes["name", Name);
                        // oXmlAttribute = oXmlNode.Attributes["username", UserName);
                        string strFullname = Path.Combine(strFolder, ID + ".xml");
                        StreamWriter sw = File.CreateText(strFullname);
                        sw.Write(Xml);  // Will be using oXmlNode.SelectSingleNode("xml").InnerText
                        sw.Close();
                    }
                } // foreach (DataRow oDataRow in oDataSet.Tables[0].Rows)
            } while (false);
            return HasMessages;
        }

        protected override bool StartWrite()
        {
            ClearMessages();
            do
            {
                ;
            } while (false);
            return HasMessages;
        }

        public override bool EndWrite()
        {
            ClearMessages();
            return HasMessages;
        }

    }
}
