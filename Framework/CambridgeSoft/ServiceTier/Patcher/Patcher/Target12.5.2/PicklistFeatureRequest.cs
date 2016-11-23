using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Data;


namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// New Feature request for PICKLISTDOMAIN.
    /// Chages to formBO's, Frameworkconfig.xml
    /// </summary>
    public class PicklistFeatureRequest : BugFixBaseCommand
    {
        

        List<string> _messages = new List<string>();
        Dictionary<string, string> PicklistDomain = new Dictionary<string, string>();

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
           
            bool errorsInPatch = false;
            string coeDetailsFormsPath = string.Empty;
            string coeQueryFormsPath = string.Empty;

           
            UpdateDB();
            UpdatePicklistDomainSqlRows();

            coeDetailsFormsPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm"; // Path to check the Rootnode before patcher update.
            coeQueryFormsPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm";

            string[] formModes = new string[] { "COE:addMode", "COE:editMode", "COE:viewMode" };
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");


                #region Details Forms
                XmlNodeList xnlCoeForm = doc.SelectNodes(coeDetailsFormsPath, manager);
                foreach (XmlNode xnCoeForm in xnlCoeForm)
                {
                    foreach (string mode in formModes)
                    {
                        XmlNodeList xnlFormElements = xnCoeForm.SelectNodes(mode + "/COE:formElement", manager);
                        ManipulateFormElements(ref xnlFormElements, manager);
                    }
                }
                #endregion

                #region Query Forms
                xnlCoeForm = doc.SelectNodes(coeQueryFormsPath, manager);
                foreach (XmlNode xnCoeForm in xnlCoeForm)
                {
                    XmlNodeList xnlFormElements = xnCoeForm.SelectNodes("COE:layoutInfo/COE:formElement", manager);
                    ManipulateFormElements(ref xnlFormElements, manager);
                }
                #endregion
            }

            #region Framework change
            UpdateFrameworkConfig(frameworkConfig);
            #endregion

            if (!errorsInPatch)
                _messages.Add("PicklistFeatureRequest was successfully patched");
            else
                _messages.Add("PicklistFeatureRequest was patched with errors");
            return _messages;
        }// Method


        private void ManipulateFormElements(ref XmlNodeList xnlFormElements, XmlNamespaceManager manager)
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList";
            string sqlQuery = string.Empty;

            foreach (XmlNode xnFormElement in xnlFormElements)
            {
                if (xnFormElement.SelectSingleNode("COE:displayInfo", manager) != null && xnFormElement.SelectSingleNode("COE:displayInfo/COE:type", manager) != null)
                {
                    if (xnFormElement.SelectSingleNode("COE:displayInfo/COE:type", manager).InnerText.ToUpper() == controlType.ToUpper())
                    {
                        XmlNode xnfieldConfig = xnFormElement.SelectSingleNode("COE:configInfo/COE:fieldConfig", manager);
                        if (xnfieldConfig == null)
                            continue;

                        XmlNode xnDropDownItemsSelect = xnfieldConfig.SelectSingleNode("COE:dropDownItemsSelect", manager);
                        XmlNode xnPickListDomain = xnfieldConfig.SelectSingleNode("COE:PickListDomain", manager);
                        XmlNode xndefaultValue = xnFormElement.SelectSingleNode("COE:defaultValue", manager);

                        if (xndefaultValue != null && xndefaultValue.InnerText != string.Empty && xndefaultValue.InnerText.ToUpper().Contains("SELECT"))
                            xndefaultValue.InnerText = "&&useSortOrderTop";

                        if (xnPickListDomain == null)
                        {
                            xnPickListDomain = xnfieldConfig.OwnerDocument.CreateNode(XmlNodeType.Element, "PickListDomain", "COE.FormGroup");
                            if (xnDropDownItemsSelect == null)
                                xnfieldConfig.AppendChild(xnPickListDomain);
                            else
                                xnfieldConfig.InsertAfter(xnPickListDomain, xnDropDownItemsSelect);
                        }

                        if (xnDropDownItemsSelect != null)
                        {
                            if (xnDropDownItemsSelect.InnerText.Contains("COEDB.PEOPLE") || xnDropDownItemsSelect.InnerText.Contains("REGDB.VW_Unit") || xnDropDownItemsSelect.InnerText.Contains("REGDB.VW_SITE_PREFIX") || xnDropDownItemsSelect.InnerText.Contains("REGDB.VW_SEQUENCE") || xnDropDownItemsSelect.InnerText.Contains("REGDB.VW_NOTEBOOKS"))
                            {
                                sqlQuery = xnDropDownItemsSelect.InnerText;
                                string tableName = (xnDropDownItemsSelect.InnerText);
                                xnPickListDomain.InnerText = ConvertToken(xnDropDownItemsSelect.InnerText);
                                xnDropDownItemsSelect.InnerText = "";
                            }
                            else if ((!string.IsNullOrEmpty(xnDropDownItemsSelect.InnerText)) && (xnDropDownItemsSelect.InnerText.Contains("REGDB.VW_PROJECT") || xnDropDownItemsSelect.InnerText.Contains("VW_PROJECT")) && (xnDropDownItemsSelect.InnerText.Contains("SELECT") && xnDropDownItemsSelect.InnerText.Contains("ACTIVE")))
                            {
                                xnDropDownItemsSelect.InnerText = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A') AND (ACTIVE = 'T' OR ACTIVE = 'F')";
                            }

                        }
                        if (xnDropDownItemsSelect.InnerText == "" && xnPickListDomain.InnerText == "")
                        {
                            xnDropDownItemsSelect.InnerText = sqlQuery;
                            _messages.Add("Patch was not seccesfully for query " + sqlQuery);
                        }

                    }
                }
            }
        }

        private void UpdateDB()
        {
            try
            {
                ExecuteSql.ExecuteNonQuery("UPDATE REGDB.PICKLISTDOMAIN SET EXT_TABLE = 'REGDB.VW_PEOPLE' WHERE  (UPPER(EXT_TABLE) = 'REGDB.COEDB.PEOPLE' OR UPPER(EXT_TABLE) = 'COEDB.PEOPLE')");
                ExecuteSql.ExecuteNonQuery("UPDATE REGDB.PICKLISTDOMAIN SET DESCRIPTION = 'UNITS' WHERE  (UPPER(DESCRIPTION) = 'UNITS')");
            }
            catch (Exception ex)
            {
                _messages.Add(ex.Message);
            }
            
        }

        /// <summary>
        /// Converts the given token to a known converted value.
        /// </summary>
        /// <param name="token">The token to convert.</param>
        /// <returns>The real needed value</returns>
        private string ConvertToken(string token)
        {
            token = token.ToUpper();
            string retVal = string.Empty;
            retVal = GetTableName(token, "FROM", (token.Contains("WHERE")) ? "WHERE" : (token.Contains("ORDER")) ? "ORDER" : "");

            if (retVal.Contains("COEDB.PEOPLE"))
            {
                retVal = "REGDB.VW_PEOPLE";
            }
            else if (retVal.Contains("REGDB.VW_SEQUENCE S"))
            {
                retVal = "REGDB.VW_SITE_PREFIX";
            }
            if (!PicklistDomain.ContainsKey(retVal.Trim()))
            {
                string pickListId = ExecuteSql.ExecuteScaler("SELECT ID as key FROM REGDB.VW_PICKLISTDOMAIN WHERE Upper(Ext_Table) ='" + retVal.Trim() + "'");
                PicklistDomain.Add(retVal.Trim(), pickListId);
            } 
            retVal = PicklistDomain[retVal.Trim()];
            return retVal;
        }

        private string GetTableName(string token, string start, string end)
        {
            string retVal = string.Empty;
            token = token + (end = (end == string.Empty) ? " $$SQLEND$$" : end);
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(token);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                foreach (Group group in groups)
                    retVal = group.Value;
            }
            return retVal;
        }

        private void  UpdateFrameworkConfig(XmlDocument frameworkConfig)
        {
            bool errorsInPatch = false;
            string viewPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='{0}']";
            string parentPath = viewPath + "/tableEditorData";
            string columnToAddPath = parentPath + "/add[@name='{1}']";
            string columnToAddAfterPath = parentPath + "/add[@name='{1}']";
            string currentView = string.Empty;
            string columnToAdd = string.Empty;
            string columnToAddAfter = string.Empty;

            XmlNode viewContentNode;
            XmlNode parentContentNode;
            XmlNode columnContentNode;
            XmlNode insertAfterNode;

            #region VW_PICKLISTDOMAIN
            currentView = "VW_PICKLISTDOMAIN";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {

                #region Update delete privilege
                if (viewContentNode.Attributes["deletePriv"] != null)
                {
                    viewContentNode.Attributes["deletePriv"].Value = "HIDEME";
                }
                #endregion


                #region Insert node EXT_SQL_SORTORDER
                //  <add name="EXT_SQL_SORTORDER" dataType="string" alias="Aditional SQL SortOrder">
                //  <validationRule>
                //    <add name="textLength" errorMessage="The length must be between 1 and 200. ">
                //      <parameter>
                //        <add name="min" value="1" />
                //        <add name="max" value="200" />
                //      </parameter>
                //    </add>
                //  </validationRule>
                //</add>
                columnToAdd = "EXT_SQL_SORTORDER";
                columnToAddAfter = "EXT_SQL_FILTER";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "string", ref columnContentNode);
                    createNewAttribute("alias", "Aditional SQL SortOrder", ref columnContentNode);
                    insertAfterNode.InnerXml = @"<validationRule>
                                                    <add name=""textLength"" errorMessage=""The length must be between 1 and 200. "">
                                                        <parameter>
                                                           <add name=""min"" value=""1"" />
                                                           <add name=""max"" value=""200"" />
                                                        </parameter>
                                                    </add>
                                                </validationRule>";
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in " + currentView + " .");
                }
                #endregion

                #region Insert node LOCKED
                //    <add name="LOCKED" dataType="string" lookupLocation="innerXml_ACTIVECASES" defaultValue="F" alias="Is Locked?" hidden="TRUE">
                //</add>
                columnToAdd = "LOCKED";
                columnToAddAfter = "EXT_SQL_SORTORDER";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "string", ref columnContentNode);
                    createNewAttribute("lookupLocation", "innerXml_ACTIVECASES", ref columnContentNode);
                    createNewAttribute("defaultValue", "F", ref columnContentNode);
                    createNewAttribute("alias", "Is Locked?", ref columnContentNode);
                    createNewAttribute("hidden", "TRUE", ref columnContentNode);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in " + currentView + " .");
                }
                #endregion
            }
            else
            {
                errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }

            #endregion

            #region "VW_PICKLIST
            currentView = "VW_PICKLIST";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {

                #region Update delete privilege
                if (viewContentNode.Attributes["deletePriv"] != null)
                {
                    viewContentNode.Attributes["deletePriv"].Value = "HIDEME";
                }
                #endregion

                #region Insert node ACTIVE
                //<add name="ACTIVE" dataType="string" lookupLocation="innerXml_ACTIVECASES" alias="Is Active?">
              //  <validationRule>
              //    <add name="requiredField" errorMessage="This field is required" />
              //  </validationRule>
              //</add>
                columnToAdd = "ACTIVE";
                columnToAddAfter = "PICKLISTVALUE";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "string", ref columnContentNode);
                    createNewAttribute("lookupLocation", "innerXml_ACTIVECASES", ref columnContentNode);
                    createNewAttribute("alias", "Is Active?", ref columnContentNode);

                    XmlNode validationRule = columnContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", null);
                    validationRule.InnerXml = @"<add name=""requiredField"" errorMessage=""This field is required""/>";
                    columnContentNode.AppendChild(validationRule);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in VW_Sequence.");
                }
                #endregion

                #region Insert node SORTORDER
                //<add name="SORTORDER" dataType="NUMBER" alias="Sort Order" />
                columnToAdd = "SORTORDER";
                columnToAddAfter = "ACTIVE";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "NUMBER", ref columnContentNode);
                    createNewAttribute("alias", "Sort Order", ref columnContentNode);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in " + currentView + " .");
                }
                #endregion
            }
            else
            {
                errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }

            #endregion

            #region VW_NOTEBOOKS
            currentView = "VW_NOTEBOOKS";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {

                #region Update delete privilege
                if (viewContentNode.Attributes["deletePriv"] != null)
                {
                    viewContentNode.Attributes["deletePriv"].Value = "HIDEME";
                }
                #endregion                                        

                #region Insert node ACTIVE
                //   <add name="ACTIVE" dataType="string" lookupLocation="innerXml_ACTIVECASES" alias="Is Active?">
                //  <validationRule>
                //    <add name="requiredField" errorMessage="This field is required" />
                //  </validationRule>
                //</add>
                columnToAdd = "ACTIVE";
                columnToAddAfter = "DESCRIPTION";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "string", ref columnContentNode);
                    createNewAttribute("lookupLocation", "innerXml_ACTIVECASES", ref columnContentNode);
                    createNewAttribute("alias", "Is Active?", ref columnContentNode);

                    XmlNode validationRule = columnContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", null);
                    validationRule.InnerXml = @"<add name=""requiredField"" errorMessage=""This field is required""/>";
                    columnContentNode.AppendChild(validationRule);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in VW_Sequence.");
                }
                #endregion

                #region Insert node PERSONID
                //<add name="PERSONID" dataType="number" lookupLocation="database" lookupID="COEDB.PEOPLE.PERSON_ID" lookupField="COEDB.PEOPLE.USER_CODE" isStructureLookupField="false" alias="User Code">
                //  <validationRule>
                //    <add name="requiredField" errorMessage="This field is required" />
                //  </validationRule>
                //</add>
                columnToAdd = "PERSONID";
                columnToAddAfter = "ACTIVE";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);

                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "NUMBER", ref columnContentNode);
                    createNewAttribute("lookupLocation", "database", ref columnContentNode);
                    createNewAttribute("lookupID", "COEDB.PEOPLE.PERSON_ID", ref columnContentNode);
                    createNewAttribute("lookupField", "COEDB.PEOPLE.USER_CODE", ref columnContentNode);
                    createNewAttribute("isStructureLookupField", "false", ref columnContentNode);
                    createNewAttribute("alias", "User Code", ref columnContentNode);
                    XmlNode validationRule = columnContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", null);
                    validationRule.InnerXml = @"<add name=""requiredField"" errorMessage=""This field is required""/>";
                    columnContentNode.AppendChild(validationRule);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in " + currentView + " .");
                }
                #endregion

                #region Insert node SORTORDER
                //<add name="SORTORDER" dataType="NUMBER" alias="Sort Order" />
                columnToAdd = "SORTORDER";
                columnToAddAfter = "PERSONID";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "NUMBER", ref columnContentNode);
                    createNewAttribute("alias", "Sort Order", ref columnContentNode);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in " + currentView + " .");
                }
                #endregion
            }
            else
            {
                errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }

            #endregion

            #region VW_SEQUENCE
            currentView = "VW_SEQUENCE";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {
                                                
                #region Insert node SORTORDER
                //<add name="SORTORDER" dataType="NUMBER" alias="Sort Order" />
                columnToAdd = "SORTORDER";
                columnToAddAfter = "SITEID";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAdd));
                if (columnContentNode == null)
                {
                    #region ADD
                    insertAfterNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToAddAfter));
                    columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("name", columnToAdd, ref columnContentNode);
                    createNewAttribute("dataType", "NUMBER", ref columnContentNode);
                    createNewAttribute("alias", "Sort Order", ref columnContentNode);
                    #endregion

                    #region INSERT NODE
                    if (insertAfterNode != null)
                        parentContentNode.InsertAfter(columnContentNode, insertAfterNode);
                    else
                        parentContentNode.AppendChild(columnContentNode);
                    #endregion

                    _messages.Add(columnToAdd + " node was added succesfully.");
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add(columnToAdd + "is already available in " + currentView + " .");
                }
                #endregion
            }
            else
            {
                errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }

            #endregion
        }

        private void UpdatePicklistDomainSqlRows()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(PickListDomainXml.PDXML);
                DataSet ds;
                XmlNodeList xNodes = xDoc.SelectNodes("//VW_PICKLISTDOMAIN");
                foreach (XmlNode xNode in xNodes)
                {
                    PD pd = new PD(xNode);
                    ds = new DataSet();
                    ds = ExecuteSql.ExecuteDataset("SELECT * FROM REGDB.VW_PICKLISTDOMAIN WHERE DESCRIPTION = '" + pd.DESCRIPTION + "'");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        pd.InsertPD();
                        _messages.AddRange(pd.Messages);
                    }
                    else
                    {
                        pd.UpdatePD();
                        _messages.AddRange(pd.Messages);
                    }
                }
            }
            catch (Exception ex)
            {
                _messages.Add(ex.Message);
            }
        }

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

        private class PD 
        {
            string _id = string.Empty;
            string _description = string.Empty;
            string _extTable = string.Empty;
            string _extIDCol = string.Empty;
            string _extDisplayCol = string.Empty;
            string _extSqlFilter = string.Empty;
            string _extSortOrder = string.Empty;
            string _locked = string.Empty;
            string _table = string.Empty;
            List<string> _messages = new List<string>();
            Dictionary<string, string> _columns;

            public PD(XmlNode xNode)
            {
                Columns.Clear();
                Table = xNode.Name;
                ID = xNode.SelectSingleNode("ID").InnerText;
                _columns.Add("DESCRIPTION", xNode.SelectSingleNode("DESCRIPTION").InnerText.Replace("'", "&#39;"));
                _columns.Add("EXT_TABLE", xNode.SelectSingleNode("EXT_TABLE").InnerText.Replace("'", "&#39;"));
                _columns.Add("EXT_ID_COL", xNode.SelectSingleNode("EXT_ID_COL").InnerText.Replace("'", "&#39;"));
                _columns.Add("EXT_DISPLAY_COL", xNode.SelectSingleNode("EXT_DISPLAY_COL").InnerText.Replace("'", "&#39;"));
                _columns.Add("EXT_SQL_FILTER", xNode.SelectSingleNode("EXT_SQL_FILTER").InnerText.Replace("'", "&#39;"));
                _columns.Add("EXT_SQL_SORTORDER", xNode.SelectSingleNode("EXT_SQL_SORTORDER").InnerText.Replace("'", "&#39;"));
                _columns.Add("LOCKED", xNode.SelectSingleNode("LOCKED").InnerText.Replace("'", "&#39;"));
            }

            public List<string> Messages
            {
                get
                {
                    return _messages;
                }

            }

            public Dictionary<string, string> Columns
            {
                get
                {
                    if (_columns == null)
                        _columns = new Dictionary<string, string>();
                    return _columns;
                }

            }

            public string ID
            {
                get
                {
                    return _id;
                }
                set
                {
                    _id = value;

                }
            }

            public string Table
            {
                get
                {
                    return _table;
                }
                set
                {
                    _table = value;
                    if (!_table.Equals("REGDB." + value, StringComparison.OrdinalIgnoreCase))
                        _table = "REGDB." + _table;
                }
            }


            public string DESCRIPTION
            {
                get
                {
                    return _columns["DESCRIPTION"];
                }
                set
                {
                    _columns["DESCRIPTION"] = value;

                }
            }

            public string EXT_TABLE
            {
                get
                {
                    return _columns["EXT_TABLE"];
                }
                set
                {
                    _columns["EXT_TABLE"] = value;

                }
            }

            public string EXT_ID_COL
            {
                get
                {
                    return _columns["EXT_ID_COL"];
                }
                set
                {
                    _columns["EXT_ID_COL"] = value;

                }
            }

            public string EXT_DISPLAY_COL
            {
                get
                {
                    return _columns["EXT_DISPLAY_COL"];
                }
                set
                {
                    _columns["EXT_DISPLAY_COL"] = value;

                }
            }
            public string EXT_SQL_FILTER
            {
                get
                {
                    return _columns["EXT_SQL_FILTER"];
                }
                set
                {
                    _columns["EXT_SQL_FILTER"] = value;

                }
            }

            public string EXT_SQL_SORTORDER
            {
                get
                {
                    return _columns["EXT_SQL_SORTORDER"];
                }
                set
                {
                    _columns["EXT_SQL_SORTORDER"] = value;

                }
            }

            public string LOCKED
            {
                get
                {
                    return _columns["LOCKED"];
                }
                set
                {
                    _columns["LOCKED"] = value;

                }
            }

            public void InsertPD()
            {
                int rowCount = Convert.ToInt32(ExecuteSql.ExecuteScaler("SELECT MAX(ID) FROM  " + this.Table));
                rowCount += 1;
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("INSERT INTO " + this.Table + " (ID, ");
                IDictionaryEnumerator iPDColumns = this.Columns.GetEnumerator();
                while (iPDColumns.MoveNext())
                {
                    sqlBuilder.Append(iPDColumns.Key + ", ");
                }
                sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                sqlBuilder.Append(") values (" + rowCount + ", ");
                iPDColumns = this.Columns.GetEnumerator();
                while (iPDColumns.MoveNext())
                {
                    sqlBuilder.Append("'" + iPDColumns.Value + "', ");
                }
                sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                sqlBuilder.Append(")");
                ExecuteSql.ExecuteNonQuery(sqlBuilder.ToString());
                _messages.Add(this.DESCRIPTION + " row inserted succesfully");
            }
            public void UpdatePD()
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("UPDATE " + this.Table + " SET ");
                IDictionaryEnumerator iPDColumns = this.Columns.GetEnumerator();
                while (iPDColumns.MoveNext())
                {
                    sqlBuilder.Append(iPDColumns.Key + " ='");
                    sqlBuilder.Append(iPDColumns.Value + "', ");
                }
                sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                sqlBuilder.Append(" WHERE ID = " + this.ID + " AND DESCRIPTION = '" + this.DESCRIPTION + "'");
                ExecuteSql.ExecuteNonQuery(sqlBuilder.ToString());
               _messages.Add(this.DESCRIPTION + " row updated succesfully");
            }
        }

        private class PickListDomainXml
        {
            public const string PDXML = @"<VW_PICKLISTDOMAINS>
    <VW_PICKLISTDOMAIN>
		<ID>2</ID>
		<DESCRIPTION>UNITS</DESCRIPTION>
		<EXT_TABLE>REGDB.VW_Unit</EXT_TABLE>
		<EXT_ID_COL>ID</EXT_ID_COL>
		<EXT_DISPLAY_COL>UNIT</EXT_DISPLAY_COL>
		<EXT_SQL_FILTER>Where active='T'</EXT_SQL_FILTER>
		<EXT_SQL_SORTORDER>ORDER BY SORTORDER ASC</EXT_SQL_SORTORDER>
        <LOCKED>F</LOCKED>		
	</VW_PICKLISTDOMAIN>
	<VW_PICKLISTDOMAIN>
		<ID>3</ID>
		<DESCRIPTION>Scientists</DESCRIPTION>		
		<EXT_TABLE>REGDB.VW_PEOPLE</EXT_TABLE>
		<EXT_ID_COL>PERSONID</EXT_ID_COL>
		<EXT_DISPLAY_COL>USERID</EXT_DISPLAY_COL>
		<EXT_SQL_FILTER>where active='T'</EXT_SQL_FILTER>
		<EXT_SQL_SORTORDER>ORDER BY USERID ASC</EXT_SQL_SORTORDER>
        <LOCKED>F</LOCKED>		
	</VW_PICKLISTDOMAIN>
     <VW_PICKLISTDOMAIN>
		<ID>4</ID>
		<DESCRIPTION>Notebooks</DESCRIPTION>		
		<EXT_TABLE>REGDB.VW_NOTEBOOKS</EXT_TABLE>
		<EXT_ID_COL>NOTEBOOKID</EXT_ID_COL>
		<EXT_DISPLAY_COL>NAME</EXT_DISPLAY_COL>
		<EXT_SQL_FILTER>where active='T'</EXT_SQL_FILTER>
		<EXT_SQL_SORTORDER>ORDER BY SORTORDER ASC</EXT_SQL_SORTORDER>
        <LOCKED>F</LOCKED>		
	</VW_PICKLISTDOMAIN>
    <VW_PICKLISTDOMAIN>
		<ID>5</ID>
		<DESCRIPTION>Prefixes</DESCRIPTION>		
		<EXT_TABLE>REGDB.VW_SITE_PREFIX</EXT_TABLE>
		<EXT_ID_COL>SEQUENCEID</EXT_ID_COL>
		<EXT_DISPLAY_COL>PREFIX</EXT_DISPLAY_COL>
		<EXT_SQL_FILTER>where active='T' AND (personid = &amp;&amp;loggedInUser or personid is null) and (upper(type) = 'R' or upper(type) = 'A')</EXT_SQL_FILTER>
		<EXT_SQL_SORTORDER>ORDER BY SORTORDER ASC</EXT_SQL_SORTORDER>
        <LOCKED>T</LOCKED>		
	</VW_PICKLISTDOMAIN>
	<VW_PICKLISTDOMAIN>
		<ID>6</ID>
		<DESCRIPTION>Sequences</DESCRIPTION>		
		<EXT_TABLE>REGDB.VW_SEQUENCE</EXT_TABLE>
		<EXT_ID_COL>SEQUENCEID</EXT_ID_COL>
		<EXT_DISPLAY_COL>PREFIX</EXT_DISPLAY_COL>
		<EXT_SQL_FILTER>WHERE (TYPE = 'R' OR TYPE = 'A') AND ACTIVE = 'T'</EXT_SQL_FILTER>
		<EXT_SQL_SORTORDER>ORDER BY SORTORDER ASC</EXT_SQL_SORTORDER>
        <LOCKED>F</LOCKED>		
	</VW_PICKLISTDOMAIN>
   <VW_PICKLISTDOMAIN>
		<ID>7</ID>
		<DESCRIPTION>NotebooksFilteredByUser</DESCRIPTION>		
		<EXT_TABLE>REGDB.VW_NOTEBOOKS</EXT_TABLE>
		<EXT_ID_COL>NOTEBOOKID</EXT_ID_COL>
		<EXT_DISPLAY_COL>NAME</EXT_DISPLAY_COL>
		<EXT_SQL_FILTER>where active='T' AND personid =&amp;&amp;loggedInUser</EXT_SQL_FILTER>
		<EXT_SQL_SORTORDER>ORDER BY SORTORDER ASC</EXT_SQL_SORTORDER>
        <LOCKED>F</LOCKED>		
	</VW_PICKLISTDOMAIN>
   </VW_PICKLISTDOMAINS>";
        }


    }// Class

}// Name Space

