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
    class OutputObjectConfigImp : OutputObject
    {
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

        public OutputObjectConfigImp()
        {
            OutputType = "Configuration Import";
            IsValid = true;
            return;
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
                    string subDirectory = oDataRow[0].ToString();
                    string XML = oDataRow[1].ToString();
                    try
                    {
                        XmlDocument oXmlDocument = new XmlDocument();
                        oXmlDocument.LoadXml(XML);
                        XmlNode oXmlNode = oXmlDocument.DocumentElement;
                        if (oXmlNode.Name != "configuration")
                        {
                            AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Root element is not configuration tag");
                            continue;
                        }
                        string DatabaseName = oXmlNode.Attributes["databasename"].Value;
                        string Description = oXmlNode.Attributes["description"].Value;
                        int FormGroup = Convert.ToInt32(oXmlNode.Attributes["formgroup"].Value);
                        int ID = Convert.ToInt32(oXmlNode.Attributes["id"].Value);
                        bool IsPublic = Convert.ToBoolean(oXmlNode.Attributes["ispublic"].Value);
                        string Name = oXmlNode.Attributes["name"].Value;
                        string UserName = oXmlNode.Attributes["username"].Value;
                        switch (subDirectory)
                        {
                            case "DataViews":
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(oXmlNode.SelectSingleNode("xml").InnerText);
                                    COEDataView oCOEDataView = new COEDataView(doc);
                                    CambridgeSoft.COE.Framework.COESecurityService.COEAccessRightsBO oCOEAccessRightsBO = null;
                                    {
                                        COEUserReadOnlyBOList usersList = COEUserReadOnlyBOList.GetList();
                                        COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
                                        oCOEAccessRightsBO = new COEAccessRightsBO(usersList, rolesList);
                                    }
                                    COEDataViewBO oCOEDataViewBO = COEDataViewBO.New(Name, Description, oCOEDataView, oCOEAccessRightsBO);
                                    oCOEDataViewBO.DatabaseName = DatabaseName;
                                    oCOEDataViewBO.FormGroup = FormGroup;
                                    oCOEDataViewBO.ID = ID;
                                    oCOEDataViewBO.IsPublic = IsPublic;
                                    oCOEDataViewBO.UserName = UserName;
                                    if (ID <= 0)
                                    {
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "{0:G} configuration {1:G} not stored because the DataView ID must be > 0", subDirectory, oCOEDataViewBO.ID.ToString());
                                        break;
                                    }
                                    oCOEDataViewBO = oCOEDataViewBO.Save(false); // true matters iif ID != -1 and exists
                                    if (oCOEDataViewBO.ID != -1)
                                    {
                                        AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Output, nTransaction, "{0:G} configuration {1:G} stored", subDirectory, oCOEDataViewBO.ID.ToString());
                                    }
                                    else
                                    {
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Failed to store {0:G} configuration {1:G}", subDirectory, oCOEDataViewBO.ID.ToString());
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "XML:{0:G}", XML);
                                    }
                                    break;
                                }
                            case @"Forms\Web":
                                {
                                    // Build COEFormBO
                                    COEFormBO oCOEFormBO = COEFormBO.New(DatabaseName);
                                    oCOEFormBO.ID = ID;
                                    oCOEFormBO.Name = Name;
                                    oCOEFormBO.Description = Description;
                                    oCOEFormBO.UserName = UserName;
                                    oCOEFormBO.IsPublic = IsPublic;
                                    oCOEFormBO.FormGroupId = FormGroup;
                                    oCOEFormBO.Application = oXmlNode.Attributes["application"] != null ? oXmlNode.Attributes["application"].Value : string.Empty;
                                    oCOEFormBO.FormTypeId = oXmlNode.Attributes["formtypeid"] != null ? int.Parse(oXmlNode.Attributes["formtypeid"].Value) : -1;
                                    oCOEFormBO.COEFormGroup = CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.GetFormGroup(oXmlNode.SelectSingleNode("xml").InnerText);

                                    // Save
                                    oCOEFormBO.Save();
                                    if (oCOEFormBO.ID != 0)
                                    {
                                        AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Output, nTransaction, "{0:G} configuration {1:G} stored", subDirectory, oCOEFormBO.ID.ToString());
                                    }
                                    else
                                    {
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Failed to store {0:G} configuration {1:G}", subDirectory, oCOEFormBO.ID.ToString());
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "XML:{0:G}", XML);
                                    }
                                    break;
                                }
                            case @"Forms\Client":
                            case "GenericObjects":
                                {
                                    COEGenericObjectStorageBO oCOEGenericObjectStorageBO = COEGenericObjectStorageBO.New("DatabaseName");
                                    oCOEGenericObjectStorageBO.COEGenericObject = oXmlNode.SelectSingleNode("xml").InnerText;
                                    oCOEGenericObjectStorageBO.Description = Description;
                                    oCOEGenericObjectStorageBO.FormGroup = FormGroup;
                                    oCOEGenericObjectStorageBO.ID = ID;
                                    oCOEGenericObjectStorageBO.IsPublic = IsPublic;
                                    oCOEGenericObjectStorageBO.Name = Name;
                                    oCOEGenericObjectStorageBO.UserName = UserName;
                                    oCOEGenericObjectStorageBO.Save();
                                    if (oCOEGenericObjectStorageBO.ID != -1)
                                    {
                                        AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Output, nTransaction, "{0:G} configuration {1:G} stored", subDirectory, oCOEGenericObjectStorageBO.ID.ToString());
                                    }
                                    else
                                    {
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Failed to store {0:G} configuration {1:G}", subDirectory, oCOEGenericObjectStorageBO.ID.ToString());
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "XML:{0:G}", XML);
                                    }
                                    break;
                                }
                            default:
                                {
                                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Unknown subdirectory {0:G}", subDirectory);
                                    break;
                                }
                        } // switch (subDirectory)
                    }
                    catch (Exception ex)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "Exception for subdirectory {0:G}: {1:G}", subDirectory, ex.Message);
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, "XML:{0:G}", XML);
                    }
                } // foreach (DataRow oDataRow in oDataSet.Tables[0].Rows)
            } while (false);
            return HasMessages;
        }

        public override bool EndWrite()
        {
            ClearMessages();
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
    }
}
