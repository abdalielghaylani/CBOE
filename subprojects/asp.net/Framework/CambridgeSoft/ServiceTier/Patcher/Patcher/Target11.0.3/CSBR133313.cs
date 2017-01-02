using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR133313: “Full Registry Number” search field is not available under Batch information frame in Search Registry page
    /// 
    /// STEPS:
    /// 1. Login to Registration enterprise
    /// 2. Go to Search Registry page and verify he availability of ‘Full Registry Number’ field under Batch information frame
    /// 
    /// Bug: ‘Full Registry Number’ field is not available under Batch information frame (Find attached 133313.GIF file)
    /// 
    /// Expected result: Full Registry Number’ search field should be available under Batch information frame
    /// 
    /// Ref: Requirement ID:SSM4 from Salt Suffix Management Requirements
    /// </summary>
    public class CSBR133313 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {

            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            int fullRegNumberFieldId = 0;

            #region COEDataview - 4003
            foreach (XmlDocument currentDataViewDocument in dataviews)
            {
                string id = currentDataViewDocument.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataViewDocument.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4003"))
                {
                    messages.Add("Proceeding to patch DATAVIEW 4003");

                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataViewDocument.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");

                    XmlNode batchTableNode = currentDataViewDocument.SelectSingleNode("//COE:tables/COE:table[@id='9']", manager);

                    if (batchTableNode == null)
                    {
                        messages.Add("  ERROR - DATAVIEW 4003: Table 9 not found!");
                        errorsInPatch = true;
                    }
                    else
                    {
                        XmlNode fullRegNumberNode = batchTableNode.SelectNodes("COE:fields[@name='FULLREGNUMBER']", manager)[0];
                        if (fullRegNumberNode != null)
                        {
                            fullRegNumberFieldId = int.Parse(fullRegNumberNode.Attributes["id"].Value);
                            messages.Add(string.Format("    DATAVIEW 4003: 'FULLREGNUMBER' (id = {0}) field already present on table 9", fullRegNumberFieldId));
                        }
                        else
                        {
                            XmlNode previousField = batchTableNode.SelectNodes("COE:fields[@id < 9000 and not(@id <= preceding-sibling::COE:fields/@id) and not(following-sibling::COE:fields/@id < 9000 and @id <=following-sibling::COE:fields/@id)]", manager)[0];

                            fullRegNumberFieldId = int.Parse(previousField.Attributes["id"].Value) + 1;

                            XmlDocumentFragment fullRegNumberFragment = currentDataViewDocument.CreateDocumentFragment();
                            fullRegNumberFragment.InnerXml = string.Format("<fields id='{0}' name='FULLREGNUMBER' alias='FULLREGNUMBER' dataType='TEXT' indexType='NONE' mimeType='NONE' visible='1' isUniqueKey='0' xmlns='COE.COEDataView' />", fullRegNumberFieldId); ;

                            batchTableNode.InsertAfter(fullRegNumberFragment, previousField);

                            messages.Add(string.Format("    DATAVIEW 4003: 'FULLREGNUMBER' (id = {0}) field added to table 9", fullRegNumberFieldId));
                        }
                    }
                    break;
                }
            }
            #endregion

            #region COEForm - 4003
            if (fullRegNumberFieldId > 0)
            {
                foreach (XmlDocument doc in forms)
                {
                    string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;

                    if (id.Equals("4003"))
                    {
                        messages.Add("Proceeding to patch FORM 4003");

                        XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                        manager.AddNamespace("COE", "COE.FormGroup");

                        #region Query Forms
                        XmlNode layoutInfo = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo", manager);

                        if (layoutInfo == null)
                        {
                            messages.Add("  ERROR - QUERY MODE: Could not find layoutInfo from queryForm id='0' -> coeForm id='2'");
                            errorsInPatch = true;
                        }
                        else
                        {
                            if (layoutInfo.SelectSingleNode("COE:formElement[@name='FULLREGNUMBER']", manager) != null)
                            {
                                messages.Add("  QUERY MODE: formElement 'FULLREGNUMBER' already present in query form.");
                            }
                            else
                            {

                                int searchCriteriaId = int.Parse(doc.SelectNodes("//COE:searchCriteriaItem[not(@id <= preceding::COE:searchCriteriaItem/@id) and not(@id <=following::COE:searchCriteriaItem/@id)]", manager)[0].Attributes["id"].Value) + 1;

                                messages.Add(string.Format("    Next searchCriteriaId to use = {0}", searchCriteriaId));

                                XmlDocumentFragment newFullRegNumber = doc.CreateDocumentFragment();
                                newFullRegNumber.InnerXml = string.Format(@"<formElement name='FULLREGNUMBER' xmlns='COE.FormGroup'><label>Full Registry Number</label><showHelp>false</showHelp><isFileUpload>false</isFileUpload><pageComunicationProvider /><fileUploadBindingExpression /><helpText /><defaultValue /><bindingExpression>SearchCriteria[{1}].Criterium.Value</bindingExpression><Id>FullRegNumber</Id><displayInfo><cssClass>Std25x40</cssClass><type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox</type><visible>true</visible></displayInfo><serverEvents /><clientEvents /><configInfo><fieldConfig><CSSClass>FETextBox</CSSClass><CSSLabelClass>FELabel</CSSLabelClass></fieldConfig></configInfo><dataSource /><dataSourceId /><requiredStyle /><searchCriteriaItem fieldid='{0}' id='{1}' tableid='9' searchLookupByID='true' xmlns='COE.FormGroup'><textCriteria negate='NO' normalizedChemicalName='NO' hillFormula='NO' fullWordSearch='NO' caseSensitive='NO' trim='NONE' operator='IN' hint='' defaultWildCardPosition='NONE' /></searchCriteriaItem><displayData /></formElement>",
                                    fullRegNumberFieldId, searchCriteriaId);

                                layoutInfo.InsertAfter(newFullRegNumber, layoutInfo.FirstChild);

                                messages.Add("  QUERY MODE: formElement 'FULLREGNUMBER' added to query form.");

                                XmlNode NotebookNode = layoutInfo.SelectSingleNode("COE:formElement[@name='NOTEBOOK_TEXT']", manager);
                                if (NotebookNode != null)
                                    layoutInfo.AppendChild(NotebookNode);
                            }

                        }
                        #endregion

                        #region ListsForms
                        #region Column
                        XmlNode ColumnsNode = doc.SelectSingleNode("//COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[1]/COE:configInfo/COE:fieldConfig/COE:tables/COE:table[@name='Table_9']/COE:Columns", manager);

                        if (ColumnsNode == null)
                        {
                            messages.Add("  ERROR - LIST MODE: Could not find table 'Table_9' columns in listForm id='0' form id='0'");
                            errorsInPatch = true;
                        }
                        else
                        {
                            if (ColumnsNode.SelectSingleNode("COE:Column[@name='FULLREGNUMBER']", manager) != null)
                            {
                                messages.Add("  LIST MODE:'FULLREGNUMBER' column already present in table 'Table_9' in listForm id='0' form id='0'");
                            }
                            else
                            {
                                XmlDocumentFragment fullRegNumberColumnFragment = doc.CreateDocumentFragment();
                                fullRegNumberColumnFragment.InnerXml = "<Column xmlns='COE.FormGroup' name='FULLREGNUMBER' allowSorting='yes'><headerText>Full Reg Number</headerText><width>150px</width><formElement name='FULLREGNUMBER'><configInfo><fieldConfig><CSSClass>FETableItem</CSSClass></fieldConfig></configInfo></formElement></Column>";
                                ColumnsNode.InsertAfter(fullRegNumberColumnFragment, ColumnsNode.FirstChild);

                                messages.Add("  LIST MODE:'FULLREGNUMBER' column added to table 'Table_9' listForm id='0' form id='0'");
                            }
                        }
                        #endregion

                        #region ResultsCriteria
                        manager.AddNamespace("RC", "COE.ResultsCriteria");
                        XmlNode batchResultsCriteriaTableNode = doc.SelectSingleNode("//RC:resultsCriteria/RC:tables/RC:table[@id='9']", manager);
                        if (batchResultsCriteriaTableNode == null)
                        {
                            messages.Add("  ERROR - RESULTSCRITERIA: Could not find resultsCriteria table id='9'.");
                            errorsInPatch = true;
                        }
                        else
                        {

                            if (batchResultsCriteriaTableNode.SelectSingleNode("RC:field[@alias='FULLREGNUMBER']", manager) != null)
                            {
                                messages.Add("  RESULTSCRITERIA: 'FULLREGNUMBER' already present on resultsCriteria table id='9'");
                            }
                            else
                            {

                                XmlDocumentFragment fullRegNumberRCFragment = doc.CreateDocumentFragment();
                                fullRegNumberRCFragment.InnerXml = string.Format("<field visible='true' alias='FULLREGNUMBER' orderById='0' direction='asc' fieldId='{0}' xmlns='COE.ResultsCriteria'/>", fullRegNumberFieldId);

                                batchResultsCriteriaTableNode.AppendChild(fullRegNumberRCFragment);
                                messages.Add("  RESULTSCRITERIA: Added 'FULLREGNUMBER' resultsCriteria field to resultsCriteria table id='9'");
                            }
                        }
                        #endregion
                        #endregion

                        break;
                    }
                }
            }
            #endregion

            if (!errorsInPatch)
            {
                messages.Add("CSBR-133313 was successfully patched");
            }
            else
                messages.Add("CSBR-133313 was patched with errors");

            return messages;
        }
    }
}
