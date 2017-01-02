using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-133565: The Projects dropdown was available in the Registry information in Duplicates Components information screen
    /// 
    /// Steps to Reproduce:
    /// 
    /// 1. Login to CBOE application
    /// 2. Click on Main form of registration
    /// 3. Click on Duplicate records count.
    /// 4. Click on new query link in the Duplicates components information screen.
    /// 5. View the Projects dropdown.
    /// 
    /// Bug: The Projects dropdown was available in the Registry information but here RLS was set to Batch level.
    /// 
    /// Expected Result: The Projects dropdown should be available in the Batch information.
    /// </summary>
    class CSBR133565 : BugFixBaseCommand
    {
        /// <summary>
        /// No manual steps are provided. The issue is there is only one project formelement, which is placed at coeForm='0'.
        /// And there whould be 2 formElements, one at coeForm='0' and another at coeForm='2'. Moreover the one at form 0 must be named
        /// REGISTRY_PROJECT and must use as ID REGISTRY_PROJECTDropDownListPerm. The one at batch level (form 2) must be named BATCH_PROJECT
        /// and must use as ID BATCH_PROJECTDropDownListPerm.
        /// </summary>        
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            int tableId = -1, projectFieldId = -1, batchIdFieldId = -1;

            #region COEDataView - 4019
            foreach (XmlDocument currentDataview in dataviews)
            {
                string id = currentDataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataview.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4019"))
                {
                    messages.Add("Proceeding to patch DATAVIEW 4019");

                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataview.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");

                    XmlNode tables = currentDataview.SelectSingleNode("//COE:tables", manager);
                    if (tables == null)
                    {
                        messages.Add("  ERROR - DATAVIEW 4019: Could not find tables!");
                        errorsInPatch = true;
                    }
                    else
                    {
                        if (tables.SelectSingleNode("//COE:table[@name='VW_BATCH_PROJECT']", manager) == null)
                        {
                            XmlNode previousTable = tables.SelectSingleNode("COE:table[not(@id <= preceding-sibling::COE:table/@id) and not(@id <=following-sibling::COE:table/@id)]", manager);

                            if (previousTable != null)
                            {
                                tableId = int.Parse(previousTable.Attributes["id"].Value) + 1;
                                int fieldIdBase = tableId * 100;

                                XmlDocumentFragment newTable = currentDataview.CreateDocumentFragment();
                                newTable.InnerXml = string.Format(@"<table id='{0}' name='VW_BATCH_PROJECT' alias='VW_BATCH_PROJECT' database='REGDB' primaryKey='{1}' xmlns='COE.COEDataView'>
      <fields id='{1}' name='ID' dataType='INTEGER' alias='ID' indexType='NONE' mimeType='NONE' />
      <fields id='{2}' name='BATCHID' dataType='INTEGER' alias='BATCHID' indexType='NONE' mimeType='NONE' />
      <fields id='{3}' name='PROJECTID' dataType='INTEGER' alias='PROJECTID' indexType='NONE' mimeType='NONE' />
    </table>",
                                    tableId, fieldIdBase, batchIdFieldId = fieldIdBase + 1, projectFieldId = fieldIdBase + 2);

                                tables.AppendChild(newTable);

                                messages.Add("  DATAVIEW 4019 - Table VW_BATCH_PROJECT added.");
                            }
                        }
                        else
                        {
                            XmlNode batchProjectTableNode = tables.SelectSingleNode("COE:table[@name='VW_BATCH_PROJECT']", manager);
                            if (batchProjectTableNode != null)
                            {
                                tableId = batchProjectTableNode.Attributes["id"] == null ? -1 : int.Parse(batchProjectTableNode.Attributes["id"].Value);
                                XmlNode ProjectFieldNode = batchProjectTableNode.SelectSingleNode("COE:fields[@name='PROJECTID']", manager);
                                if (ProjectFieldNode != null)
                                    projectFieldId = ProjectFieldNode.Attributes["id"] == null ? -1 : int.Parse(ProjectFieldNode.Attributes["id"].Value);
                                XmlNode batchIdFieldNode = batchProjectTableNode.SelectSingleNode("COE:fields[@name='BATCHID']", manager);
                                if (batchIdFieldNode != null)
                                    batchIdFieldId = batchIdFieldNode.Attributes["id"] == null ? -1 : int.Parse(batchIdFieldNode.Attributes["id"].Value);

                                messages.Add(string.Format("    DATAVIEW 4019 - Table VW_BATCH_PROJECT already present(tableid={0}).", tableId));
                            }
                        }

                        //Ensure the relationship exists
                        if (tableId > 0 && projectFieldId > 0 && batchIdFieldId > 0)
                        {
                            XmlNode relationships = currentDataview.SelectSingleNode("//COE:relationships", manager);
                            if (relationships != null)
                            {
                                if (relationships.SelectSingleNode(string.Format("COE:relationship[@childkey='{0}' and @parentkey='900']", batchIdFieldId), manager) == null)
                                {
                                    XmlDocumentFragment newRelationship = currentDataview.CreateDocumentFragment();
                                    newRelationship.InnerXml = string.Format(@"<relationship parentkey='900' childkey='{1}' parent='9' child='{0}' jointype='INNER' xmlns='COE.COEDataView'/>", tableId, batchIdFieldId);

                                    relationships.AppendChild(newRelationship);

                                    messages.Add("  DATAVIEW 4019 - relationship from VW_BATCH to VW_BATCH_PROJECT was not present, so it was added.");
                                }
                            }
                        }

                    }
                }
            }
            #endregion

            #region COEForm 4019
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;

                if (id == "4019")
                {
                    messages.Add("Proceeding to patch FORM 4019");

                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    messages.Add("  Form element PROJECT");

                    #region FormElement - PROJECT
                    XmlNode layoutInfo = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", manager);

                    if (layoutInfo == null)
                    {
                        messages.Add("      ERROR - QUERY MODE: Could not find layoutInfo from queryForm id='0' -> coeForm id='0'");
                        errorsInPatch = true;
                    }
                    else
                    {
                        XmlNode regProject = layoutInfo.SelectSingleNode("COE:formElement[@name='PROJECT']", manager);
                        if (regProject == null)
                        {
                            if (layoutInfo.SelectSingleNode("COE:formElement[@name='REGISTRY_PROJECT']", manager) != null)
                                messages.Add("      QUERY MODE: WARNING: formElement 'Project' no longer exist, but REGISTRY_PROJECT is already present in query form - seems to be already patched.");
                            else
                            {
                                messages.Add("      QUERY MODE: formElement 'Project' no longer exist in query form - nothing was made.");
                                errorsInPatch = true;
                            }
                        }
                        else
                        {
                            regProject.Attributes["name"].Value = "REGISTRY_PROJECT";
                            regProject.SelectSingleNode("./COE:label", manager).InnerText = "Registry Project Name";
                            regProject.SelectSingleNode("./COE:Id", manager).InnerText = "REGISTRY_PROJECTDropDownListPerm";
                            regProject.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ID", manager).InnerText = "REGISTRY_PROJECTDropDownListPerm";
                            messages.Add("      Successfully updated formElement 'PROJECT' at registry level in duplicates search form");
                        }
                    }
                    #endregion
                    
                    messages.Add("  Form element BATCH_PROJECT");
                    #region FormElement - BATCH_PROJECT
                    layoutInfo = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo", manager);
                    if (layoutInfo == null)
                    {
                        messages.Add("      ERROR - QUERY MODE: Could not find layoutInfo from queryForm id='0' -> coeForm id='2'");
                        errorsInPatch = true;
                    }
                    else
                    {
                        XmlNode batchProject = layoutInfo.SelectSingleNode("./COE:formElement[@name='BATCH_PROJECT']", manager);
                        if (batchProject != null)
                        {
                            messages.Add("      BATCH_PROJECT is already present in duplicates form group and won't be added");

                            if (tableId > 0 && projectFieldId > 0)
                            {
                                XmlNode searchCriteriaItem = layoutInfo.SelectSingleNode("./COE:formElement[@name='BATCH_PROJECT']/COE:searchCriteriaItem", manager);
                                searchCriteriaItem.Attributes["tableid"].Value = tableId.ToString();
                                searchCriteriaItem.Attributes["fieldid"].Value = projectFieldId.ToString();

                                messages.Add(string.Format("      BATCH_PROJECT searchCriteriaItem updated with tableid={0} fieldid={1}", tableId, projectFieldId));
                            }
                        }
                        else
                        {
                            int searchCriteriaId = int.Parse(doc.SelectSingleNode("//COE:searchCriteriaItem[not(@id <= preceding::COE:searchCriteriaItem/@id) and not(@id <=following::COE:searchCriteriaItem/@id)]", manager).Attributes["id"].Value) + 1;
                            batchProject = doc.CreateDocumentFragment();
                            batchProject.InnerXml = string.Format(@"<formElement name='BATCH_PROJECT' xmlns='COE.FormGroup'>
							<label xmlns='COE.FormGroup'>Batch Project Name</label>
							<showHelp xmlns='COE.FormGroup'>false</showHelp>
							<helpText xmlns='COE.FormGroup'/>
							<defaultValue xmlns='COE.FormGroup'/>
							<bindingExpression xmlns='COE.FormGroup'>SearchCriteria[{0}].Criterium.Value</bindingExpression>
							<Id xmlns='COE.FormGroup'>BATCH_PROJECTDropDownListPerm</Id>
							<displayInfo xmlns='COE.FormGroup'>
								<cssClass xmlns='COE.FormGroup'>Std25x40</cssClass>
								<type xmlns='COE.FormGroup'>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
								<visible xmlns='COE.FormGroup'>true</visible>
							</displayInfo>
							<validationRuleList xmlns='COE.FormGroup'/>
							<serverEvents xmlns='COE.FormGroup'/>
							<clientEvents xmlns='COE.FormGroup'/>
							<configInfo xmlns='COE.FormGroup'>
								<fieldConfig>
									<dropDownItemsSelect>SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A') AND ACTIVE = 'T'</dropDownItemsSelect>
									<CSSLabelClass>FELabel</CSSLabelClass>
									<CSSClass>FEDropDownList</CSSClass>
									<Enable>True</Enable>
									<ID>Inner_BATCH_PROJECTDropDownListPerm</ID>
									<AutoPostBack>False</AutoPostBack>
								</fieldConfig>
							</configInfo>
							<dataSource xmlns='COE.FormGroup'/>
							<dataSourceId xmlns='COE.FormGroup'/>
							<searchCriteriaItem fieldid='{2}' id='{0}' tableid='{1}' xmlns='COE.FormGroup'>
								<numericalCriteria negate='NO' trim='NONE' operator='EQUAL'/>
							</searchCriteriaItem>
							<displayData xmlns='COE.FormGroup'/>
						</formElement>", searchCriteriaId, tableId, projectFieldId);
                            XmlNode creationDate = layoutInfo.SelectSingleNode("./COE:formElement[@name='CREATION_DATE']", manager);
                            if (creationDate != null)
                                layoutInfo.InsertAfter(batchProject, creationDate);
                            else
                                layoutInfo.InsertAfter(batchProject, layoutInfo.FirstChild);

                            messages.Add("      Successfully added BATCH_PROJECT form element to duplicates searches.");
                        }
                    }
                    #endregion
                }
            }
            #endregion

            if (!errorsInPatch)
            {
                messages.Add("CSBR-133565 was successfully patched");
            }
            else
                messages.Add("CSBR-133565 was patched with errors");
            return messages;
        }
    }
}
