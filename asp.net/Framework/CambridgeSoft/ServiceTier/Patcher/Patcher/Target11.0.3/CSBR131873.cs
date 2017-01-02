using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-131873: The identifiers dropdown was disappeared in the submit registry form
    /// 
    /// Steps to Reproduce:
    /// 
    /// 1. Login to CBOE application with reg privilliges 
    /// 2. Submit new record
    /// 3. Click on Identifiers type
    /// 
    /// Bug: The identifiers dropdown was disappeared. not able to acess the list of identifiers available. and the user was able to enter the identifier value directly.
    /// 
    /// Expected result: It should show a dropdown with list of values available.
    /// 
    /// </summary>
    public class CSBR131873 : BugFixBaseCommand
    {
        /// <summary>
        /// No manual fix is provided
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                #region Form 4010
                if (id == "4010")
                {
                    XmlNode addModeIdentifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                    if (addModeIdentifiers != null)
                    {
                        XmlNode typeColumn = addModeIdentifiers.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "RegistryIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) add mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) add mode for form 0 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "RegistryIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) add mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) add mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on add mode for form 0 in coeform 4010");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on add mode for form 0 in coeform 4010");
                    }

                    XmlNode editModeIdentifierList = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='IdentifierList']", manager);
                    if (editModeIdentifierList != null)
                    {
                        XmlNode typeColumn = editModeIdentifierList.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "RegistryIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) edit mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) edit mode for form 0 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "RegistryIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) edit mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) edit mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on edit mode for form 0 in coeform 4010");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4010");
                    }

                    XmlNode viewModeIdentifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiers != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiers.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] == null)
                            {
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "RegistryIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully added attribute datasourceid for column (type) view mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to add attribute datasourceid for column (type) view mode for form 0 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4010");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4010");
                    }

                    //----------
                    XmlNode addModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                    if (addModeIdentifiersF1 != null)
                    {
                        XmlNode typeColumn = addModeIdentifiersF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "CompoundIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) add mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) add mode for form 1 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "CompoundIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) add mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) add mode for form 1 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on add mode for form 1 in coeform 4010");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4010");
                    }

                    XmlNode editModeIdentifierListF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                    if (editModeIdentifierListF1 != null)
                    {
                        XmlNode typeColumn = editModeIdentifierListF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "CompoundIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) edit mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) edit mode for form 1 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "CompoundIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) edit mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) edit mode for form 1 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on edit mode for form 1 in coeform 4010");
                        }

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on edit mode for form 1 in coeform 4010");
                    }

                    XmlNode viewModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifier']", manager);
                    if (viewModeIdentifiersF1 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully Modified attribute datasourceid for column (type) view mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 1 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 1 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 1 in coeform 4010");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 1 in coeform 4010");
                    }

                    //-------------
                    XmlNode viewModeIdentifiersF2 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF2 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF2.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified attribute datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified element datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify element datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 2 in coeform 2");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 2 in coeform 4010");
                    }

                    //--------------
                    XmlNode addModeIdentifiersF4 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                    if (addModeIdentifiersF4 != null)
                    {
                        XmlNode typeColumn = addModeIdentifiersF4.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified attribute datasourceid for column (type) add mode for form 4 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) add mode for form 4 in coeform 4010");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified element datasourceid for column (type) add mode for form 4 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify element datasourceid for column (type) add mode for form 4 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on add mode for form 4 in coeform 4010");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on add mode for form 4 in coeform 4010");
                    }
                }
                #endregion
                #region Form 4011
                else if (id == "4011")
                {
                    XmlNode editModeIdentifierList = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                    if (editModeIdentifierList != null)
                    {
                        XmlNode typeColumn = editModeIdentifierList.SelectSingleNode(".//COE:Column[@name='IdentifierType']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "RegistryIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) edit mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) edit mode for form 0 in coeform 4011");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "RegistryIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) edit mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) edit mode for form 0 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on edit mode for form 0 in coeform 4011");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4011");
                    }

                    XmlNode viewModeIdentifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiers != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiers.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified attribute datasourceid for column (type) view mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to add attribute datasourceid for column (type) view mode for form 0 in coeform 4011");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 0 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4011");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4011");
                    }

                    //----------
                    XmlNode addModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                    if (addModeIdentifiersF1 != null)
                    {
                        XmlNode typeColumn = addModeIdentifiersF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "CompoundIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) add mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) add mode for form 1 in coeform 4011");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "CompoundIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) add mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) add mode for form 1 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on add mode for form 1 in coeform 4011");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4011");
                    }

                    XmlNode editModeIdentifierListF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                    if (editModeIdentifierListF1 != null)
                    {
                        XmlNode typeColumn = editModeIdentifierListF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "CompoundIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) edit mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) edit mode for form 1 in coeform 4011");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "CompoundIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) edit mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) edit mode for form 1 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on edit mode for form 1 in coeform 4011");
                        }

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on edit mode for form 1 in coeform 4011");
                    }

                    XmlNode viewModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF1 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully Modified attribute datasourceid for column (type) view mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 1 in coeform 4011");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 1 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 1 in coeform 4011");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 1 in coeform 4011");
                    }

                    //-------------
                    XmlNode viewModeIdentifiersF2 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF2 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF2.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified attribute datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified element datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify element datasourceid for column (type) view mode for form 2 in coeform 2");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 2 in coeform 2");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 2 in coeform 4011");
                    }
                }
                #endregion
                #region Form 4012
                else if (id == "4012")
                {
                    XmlNode editModeIdentifierList = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Identifier']", manager);
                    if (editModeIdentifierList != null)
                    {
                        XmlNode typeColumn = editModeIdentifierList.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "RegistryIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) edit mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) edit mode for form 0 in coeform 4012");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "RegistryIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) edit mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) edit mode for form 0 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on edit mode for form 0 in coeform 4012");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4012");
                    }

                    XmlNode viewModeIdentifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiers != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiers.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully modified attribute datasourceid for column (type) view mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to add attribute datasourceid for column (type) view mode for form 0 in coeform 4012");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 0 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4012");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4012");
                    }

                    //----------
                    XmlNode addModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                    if (addModeIdentifiersF1 != null)
                    {
                        XmlNode typeColumn = addModeIdentifiersF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "CompoundIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) add mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) add mode for form 1 in coeform 4012");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "CompoundIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) add mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) add mode for form 1 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on add mode for form 1 in coeform 4012");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4012");
                    }

                    XmlNode editModeIdentifierListF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                    if (editModeIdentifierListF1 != null)
                    {
                        XmlNode typeColumn = editModeIdentifierListF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSource"] != null)
                            {
                                typeColumn.Attributes.Remove(typeColumn.Attributes["dataSource"]);
                                XmlAttribute dataSourceIDAtrr = doc.CreateAttribute("dataSourceID");
                                dataSourceIDAtrr.Value = "CompoundIdentifiersCslaDataSource";
                                typeColumn.Attributes.Append(dataSourceIDAtrr);
                                messages.Add("Succesfully replaced attribute datasource with dataSourceID for column (type) edit mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to remove attribute datasource for column (type) edit mode for form 1 in coeform 4012");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSource", manager);
                            if (fcDataSource != null)
                            {
                                XmlNode fcDataSourceID = doc.CreateElement("DataSourceID", doc.DocumentElement.NamespaceURI);
                                fcDataSourceID.InnerText = "CompoundIdentifiersCslaDataSource";
                                fcDataSource.ParentNode.ReplaceChild(fcDataSourceID, fcDataSource);
                                messages.Add("Succesfully replaced element datasource with dataSourceID for column (type) edit mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to replace element datasource for column (type) edit mode for form 1 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on edit mode for form 1 in coeform 4012");
                        }

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on edit mode for form 1 in coeform 4012");
                    }

                    XmlNode viewModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF1 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF1.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully Modified attribute datasourceid for column (type) view mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 1 in coeform 4012");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 1 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 1 in coeform 4012");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 1 in coeform 4012");
                    }
                }
                #endregion
                #region Form 4013
                else if (id == "4013")
                {
                    XmlNode viewModeIdentifiersF94013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='9']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF94013 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF94013.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully Modified attribute datasourceid for column (type) view mode for form 9 in coeform 4013");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 9 in coeform 4013");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "RegistryIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 9 in coeform 4013");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 9 in coeform 4013");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 9 in coeform 4013");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 9 in coeform 4013");
                    }

                    XmlNode viewModeIdentifiersF04013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF04013 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF04013.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully Modified attribute datasourceid for column (type) view mode for form 0 in coeform 4013");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 0 in coeform 4013");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./COE:formElement/COE:configInfo/COE:fieldConfig/COE:DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "CompoundIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 0 in coeform 4013");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 0 in coeform 4013");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 0 in coeform 4013");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4013");
                    }

                    XmlNode viewModeIdentifiersF24013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                    if (viewModeIdentifiersF24013 != null)
                    {
                        XmlNode typeColumn = viewModeIdentifiersF24013.SelectSingleNode(".//COE:Column[@name='Type']", manager);
                        if (typeColumn != null)
                        {
                            if (typeColumn.Attributes["dataSourceID"] != null && typeColumn.Attributes["dataSourceID"].Value == "IdentifiersCslaDataSource")
                            {
                                typeColumn.Attributes["dataSourceID"].Value = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully Modified attribute datasourceid for column (type) view mode for form 2 in coeform 4013");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("Failed to modify attribute datasourceid for column (type) view mode for form 2 in coeform 4013");
                            }
                            XmlNode fcDataSource = typeColumn.SelectSingleNode("./formElement/configInfo/fieldConfig/DataSourceID", manager);
                            if (fcDataSource != null && fcDataSource.InnerText == "IdentifiersCslaDataSource")
                            {
                                fcDataSource.InnerText = "BatchIdentifiersCslaDataSource";
                                messages.Add("Succesfully updated element datasourceid for column (type) view mode for form 2 in coeform 4013");
                            }
                            else
                            {
                                messages.Add("Failed to update element datasourceid for column (type) view mode for form 2 in coeform 4013");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Column name='Type' was not found on view mode for form 2 in coeform 4013");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was not found on view mode for form 2 in coeform 4013");
                    }


                    XmlNode listForm04013 = doc.SelectSingleNode("//COE:listForm[@id='0']", manager);
                    if (listForm04013 != null)
                    {
                        if (listForm04013.InnerXml.Contains("\"IdentifiersCslaDataSource\"") || listForm04013.InnerXml.Contains("<DataSourceID>IdentifiersCslaDataSource</DataSourceID>"))
                        {
                            listForm04013.InnerXml = listForm04013.InnerXml.Replace("\"IdentifiersCslaDataSource\"", "\"CompoundIdentifiersCslaDataSource\"").Replace("<DataSourceID>IdentifiersCslaDataSource</DataSourceID>", "<DataSourceID>CompoundIdentifiersCslaDataSource</DataSourceID>");
                            messages.Add("Sucessfully updated datasources in identifiers form element on list mode for coeform 4013");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("failed to update datasources in identifiers form element on list mode for coeform 4013");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("There was no list form for coeform 4013");
                    }
                }
                #endregion
                #region Form 4014
                if (id == "4014")
                {
                    XmlNode viewModeF04014 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode", manager);
                    if (viewModeF04014 != null)
                    {
                        if (viewModeF04014.InnerXml.Contains("\"IdentifiersCslaDataSource\"") || viewModeF04014.InnerXml.Contains("<DataSourceID>IdentifiersCslaDataSource</DataSourceID>"))
                        {
                            viewModeF04014.InnerXml = viewModeF04014.InnerXml.Replace("\"IdentifiersCslaDataSource\"", "\"CompoundIdentifiersCslaDataSource\"").Replace("<DataSourceID>IdentifiersCslaDataSource</DataSourceID>", "<DataSourceID>CompoundIdentifiersCslaDataSource</DataSourceID>");
                            messages.Add("Sucessfully updated datasources in identifiers form element on view mode for form 0 in coeform 4014");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Failed to update datasources in identifiers form element on view mode for form 0 in coeform 4014");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("There was on view mode for form 0 in coeform 4014");
                    }
                }
                #endregion
            }
            if (!errorsInPatch)
            {
                messages.Add("CSBR131873 was successfully patched");
            }
            else
                messages.Add("CSBR131873 was patched with errors");
            return messages;
        }
    }
}
