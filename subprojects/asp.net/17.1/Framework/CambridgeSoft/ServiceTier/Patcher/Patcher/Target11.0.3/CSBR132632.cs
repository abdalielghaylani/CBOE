using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 132632: Date range based search on Registration 11.0.2 is not possible
    /// </summary>
    class CSBR132632 : BugFixBaseCommand
    {
        /// <summary>
        /// Steps to fix manually:
        /// 1 - Open COEFromGroups 4002 and 4003.
        /// 2 - Search all formElements called DATECREATED and CREATION_DATE on both COEFormGroups.
        /// 3 - Add to all selected formElements names, ids and labels the prefix 'START_'.
        /// 3 - Replace in all selected formElements the searchCriteriaItem operator with 'GTE'.
        /// 4 - Copy each selected formElement and paste it below the original.
        /// 5 - Replace on the formElements copies the previous added prefix 'START_' with the prefix 'END_' and the operator 'GTE' with 'LTE' on its searchCriteriasItems.
        /// 6 - Add in all formElements fieldConfigs a new node called 'Width' and put into its inner text the value '100%'. 
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;

                #region adding dates ranges to search temp form
                if (id == "4002")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    XmlNodeList searchCriteriasNode = doc.SelectNodes("//COE:searchCriteriaItem", manager);
                    int nextId = 0;
                    foreach (XmlNode searchCriteria in searchCriteriasNode)
                    {
                        int searchCriteriaId;
                        int.TryParse(searchCriteria.Attributes["id"].Value, out searchCriteriaId);
                        if (searchCriteriaId > nextId)
                            nextId = searchCriteriaId;
                    }
                    nextId++;

                    XmlNode coeForm = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']", manager);
                    if (coeForm == null)
                    {
                        errorsInPatch = true;
                        messages.Add("The expected coeForm id='0' was not found on COEFormGoup id='4002'");
                    }
                    else
                    {
                        XmlNode layoutInfoNode = coeForm.SelectSingleNode("./COE:layoutInfo", manager);
                        if (layoutInfoNode == null)
                        {
                            errorsInPatch = true;
                            messages.Add("The layoutInfo node was not found on coeForm id='0' COEFormGroup id='4002'");
                        }
                        else
                        {

                            XmlNode dateCreatedFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='DATECREATED']", manager);

                            if (dateCreatedFormElement == null)
                            {
                                errorsInPatch = true;
                                messages.Add("The formElement name='DATECREATED' was not found on coeForm id='0' COEFormGroup id='4002'");
                            }
                            else
                            {
                                XmlNode startDateCreatedFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                startDateCreatedFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                startDateCreatedFormElement.Attributes["name"].Value = "STARTDATECREATED";
                                startDateCreatedFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Start Date Created</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">StartRegDateCreatedTextBox</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""105"" id=""" + nextId.ToString() + @""" tableid=""1"" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""GTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData/>";
                                nextId++;
                                layoutInfoNode.ReplaceChild(startDateCreatedFormElement, dateCreatedFormElement);
                                messages.Add("The formElement name='DATECREATED' was replaced with the formElement name='STARTDATECREATED' on coeForm id='0' COEFormGroup id='4002'");

                                XmlNode endDateCreatedFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                endDateCreatedFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                endDateCreatedFormElement.Attributes["name"].Value = "ENDDATECREATED";
                                endDateCreatedFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">End Date Created</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">EndRegDateCreatedTextBox</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""105"" id=""" + nextId.ToString() + @""" tableid=""1"" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""LTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";
                                nextId++;
                                layoutInfoNode.InsertAfter(endDateCreatedFormElement, startDateCreatedFormElement);
                                messages.Add("The formElement name='ENDDATECREATED' was successfully added on coeForm id='0' COEFormGroup id='4002'");
                                XmlNode personCreatedFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='PERSONCREATED']", manager);
                                if (personCreatedFormElement != null)
                                {
                                    layoutInfoNode.InsertAfter(personCreatedFormElement, endDateCreatedFormElement);
                                    messages.Add("The formElement name='PERSONCREATED' was relocated below the formElement name='ENDDATECREATED' on coeForm id='0' COEFormGroup id='4002'");
                                }
                                else
                                {
                                    errorsInPatch = true;
                                    messages.Add("The formElement name='PERSONCREATED' was not found on coeForm id='0' COEFormGroup id='4002'");
                                }
                            }
                            XmlNode creationDateFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='CREATION_DATE']", manager);
                            if (creationDateFormElement == null)
                            {
                                errorsInPatch = true;
                                messages.Add("The formElement name='CREATION_DATE' was not found on coeForm id='0' COEFormGroup id='4002' ");
                            }
                            else
                            {
                                XmlNode startCreationDateFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                startCreationDateFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                startCreationDateFormElement.Attributes["name"].Value = "START_CREATION_DATE";
                                startCreationDateFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Start Synthesis Date</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">START_CREATION_DATEProperty</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""152"" id=""" + nextId.ToString() + @""" tableid=""1"" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""GTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";
                                nextId++;
                                layoutInfoNode.ReplaceChild(startCreationDateFormElement, creationDateFormElement);
                                messages.Add("The formElement name='CREATION_DATE' was replaced with the formElement name='START_CREATION_DATE' on coeForm id='0' COEFormGroup id='4002' ");

                                XmlNode endCreationDateFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                endCreationDateFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                endCreationDateFormElement.Attributes["name"].Value = "END_CREATION_DATE";
                                endCreationDateFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">End Synthesis Date</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">END_CREATION_DATEProperty</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""152"" id=""" + nextId.ToString() + @""" tableid=""1"" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""LTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";

                                layoutInfoNode.InsertAfter(endCreationDateFormElement, startCreationDateFormElement);
                                messages.Add("The formElement name='END_CREATION_DATE' was successfully added on coeFrom id='0' COEFormGroup id='4002'");
                                XmlNode notebookFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='NOTEBOOK_TEXT']", manager);
                                if (notebookFormElement != null)
                                {
                                    layoutInfoNode.InsertBefore(notebookFormElement, startCreationDateFormElement);
                                    messages.Add("The formElement name='NOTEBOOK_TEXT' was relocated above the formElement name='START_CREATION_DATE' on coeFrom id='0' COEFormGroup id='4002'");
                                }
                                else
                                {
                                    errorsInPatch = true;
                                    messages.Add("The formElement name='NOTEBOOK_TEXT' was not found on coeFrom id='0' COEFormGroup id='4002'");
                                }

                            }
                        }
                    }
                }
                #endregion

                #region adding dates ranges to perm forms
                else if (id == "4003")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    XmlNodeList searchCriteriasNode = doc.SelectNodes("//COE:searchCriteriaItem", manager);
                    int nextId = 0;
                    foreach (XmlNode searchCriteria in searchCriteriasNode)
                    {
                        int searchCriteriaId;
                        int.TryParse(searchCriteria.Attributes["id"].Value, out searchCriteriaId);
                        if (searchCriteriaId > nextId)
                            nextId = searchCriteriaId;
                    }
                    nextId++;

                    string[] coeFormsIds = { "0", "2" };
                    foreach (string coeFormId in coeFormsIds)
                    {
                        XmlNode coeForm = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + coeFormId + "']", manager);
                        if (coeForm == null)
                        {
                            errorsInPatch = true;
                            messages.Add("The expected coeForm id=" + coeFormId + " was not found on COEFormGoup 4003");
                        }
                        else
                        {
                            XmlNode layoutInfoNode = coeForm.SelectSingleNode("./COE:layoutInfo", manager);
                            if (layoutInfoNode == null)
                            {
                                errorsInPatch = true;
                                messages.Add("The layoutInfo node was not found on coeForm id ='" + coeFormId + "' COEFormGroup id ='4003'");
                            }
                            else
                            {
                                XmlNode dateCreatedFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='DATECREATED']", manager);
                                if (dateCreatedFormElement == null)
                                {
                                    errorsInPatch = true;
                                    messages.Add("The formElement name='DATECREATED' was not found on coeForm id='" + coeFormId + "' COEFormGroup id='4002'");
                                }
                                else
                                {
                                    XmlNode startDateCreatedFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                    string startFormElementId = coeFormId == "0" ? "RegStartDateCreatedTextBox" : "BatchStartDateCreatedTextBox";
                                    string fieldId = coeFormId == "0" ? "104" : "903";
                                    string tableId = coeFormId == "0" ? "1" : "9";
                                    startDateCreatedFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                    startDateCreatedFormElement.Attributes["name"].Value = "STARTDATECREATED";
                                    startDateCreatedFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Start Date Created</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">" + startFormElementId + @"</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""" + fieldId + @""" id=""" + nextId.ToString() + @""" tableid=""" + tableId + @""" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""GTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";
                                    nextId++;

                                    layoutInfoNode.ReplaceChild(startDateCreatedFormElement, dateCreatedFormElement);
                                    messages.Add("The formElement name='DATECREATED' was replaced with the formElement name='STARTDATECREATED' on coeForm id ='" + coeFormId + "' COEFormGroup id ='4003'");

                                    XmlNode endDateCreatedFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                    string endFormElementId = coeFormId == "0" ? "EndRegDateCreatedTextBox" : "EndBatchDateCreatedTextBox";
                                    endDateCreatedFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                    endDateCreatedFormElement.Attributes["name"].Value = "ENDDATECREATED";
                                    endDateCreatedFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">End Date Created</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">" + endFormElementId + @"</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""" + fieldId + @""" id=""" + nextId.ToString() + @""" tableid=""" + tableId + @""" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""LTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";
                                    nextId++;
                                    layoutInfoNode.InsertAfter(endDateCreatedFormElement, startDateCreatedFormElement);
                                    messages.Add("The formElement name='ENDDATECREATED' was added on coeForm id='" + coeFormId + "' COEFormGroup id='4003'");
                                    if (coeFormId == "0")
                                    {
                                        XmlNode regNumberFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='RegNumber']", manager);
                                        if (regNumberFormElement != null)
                                        {
                                            layoutInfoNode.InsertBefore(regNumberFormElement, startDateCreatedFormElement);
                                            messages.Add("The formElement name='RegNumber' was relocated above the formElement name='STARTDATECREATED' on coeForm id='" + coeFormId + "' COEFormGroup id='4003'");
                                        }
                                        else
                                        {
                                            errorsInPatch = true;
                                            messages.Add("The formElement name='RegNumber' was not found on coeFrom id='" + coeFormId + "' COEFormGroup id='4003'");
                                        }
                                    }
                                    else
                                    {
                                        XmlNode personCreatedFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='PERSONCREATED']", manager);
                                        if (personCreatedFormElement != null)
                                        {
                                            layoutInfoNode.InsertAfter(personCreatedFormElement, endDateCreatedFormElement);
                                            messages.Add("The formElement name='PERSONCREATED' was relocated bolow the formElement name='ENDDATECREATED' on coeform id='" + coeFormId + "' COEFormGroup id='4003'");
                                        }
                                        else
                                        {
                                            errorsInPatch = true;
                                            messages.Add("The formElement name='PERSONCREATED' was not found on coeform id='" + coeFormId + "' COEFormGroup id='4003'");
                                        }
                                    }

                                }
                                if (coeFormId == "2")
                                {
                                    XmlNode creationDateFormElement = layoutInfoNode.SelectSingleNode("./COE:formElement[@name='CREATION_DATE']", manager);

                                    if (creationDateFormElement == null)
                                    {
                                        errorsInPatch = true;
                                        messages.Add("The formElement name='CREATION_DATE' was not found on  coeForm id='2' COEFormGroup id='4003'");
                                    }
                                    else
                                    {
                                        XmlNode startCreationDateFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                        startCreationDateFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                        startCreationDateFormElement.Attributes["name"].Value = "START_CREATION_DATE";
                                        startCreationDateFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Start Synthesis Date</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">START_CREATION_DATEProperty</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""951"" id=""" + nextId.ToString() + @""" tableid=""9"" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""GTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";
                                        nextId++;
                                        layoutInfoNode.ReplaceChild(startCreationDateFormElement, creationDateFormElement);
                                        messages.Add("The formElement name='CREATION_DATE' was replaced with the formElement name='START_CREATION_DATE' on coeForm id='2' COEFormGroup id='4003'");

                                        XmlNode endCreationDateFormElement = doc.CreateNode(XmlNodeType.Element, "formElement", layoutInfoNode.NamespaceURI);
                                        endCreationDateFormElement.Attributes.Append(doc.CreateAttribute("name"));
                                        endCreationDateFormElement.Attributes["name"].Value = "END_CREATION_DATE";
                                        endCreationDateFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">End Synthesis Date</label>
	<showHelp xmlns=""COE.FormGroup"">false</showHelp>
	<helpText xmlns=""COE.FormGroup""/>
	<defaultValue xmlns=""COE.FormGroup""/>
	<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[" + nextId.ToString() + @"].Criterium.Value</bindingExpression>
	<Id xmlns=""COE.FormGroup"">END_CREATION_DATEProperty</Id>
	<displayInfo xmlns=""COE.FormGroup"">
		<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
		<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker</type>
		<visible xmlns=""COE.FormGroup"">true</visible>
	</displayInfo>
	<validationRuleList xmlns=""COE.FormGroup""/>
	<serverEvents xmlns=""COE.FormGroup""/>
	<clientEvents xmlns=""COE.FormGroup""/>
	<configInfo xmlns=""COE.FormGroup"">
		<fieldConfig xmlns=""COE.FormGroup"">
			<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			<CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
            <Width xmlns=""COE.FormGroup"">100%</Width>
			<MaxDate xmlns=""COE.FormGroup"">1/1/0001 12:00:00 AM</MaxDate>
			<DateFormat xmlns=""COE.FormGroup"">Short</DateFormat>
			<Editable xmlns=""COE.FormGroup"">True</Editable>
			<AllowNull xmlns=""COE.FormGroup"">True</AllowNull>
			<AutoCloseUp xmlns=""COE.FormGroup"">True</AutoCloseUp>
			<FirstDayOfWeek xmlns=""COE.FormGroup"">Default</FirstDayOfWeek>
			<GridLines xmlns=""COE.FormGroup"">None</GridLines>
			<NullDateLabel xmlns=""COE.FormGroup""/>
			<NullValueRepresentation xmlns=""COE.FormGroup"">NotSet</NullValueRepresentation>
			<Section508Compliant xmlns=""COE.FormGroup"">False</Section508Compliant>
			<FontSize xmlns=""COE.FormGroup""/>
		</fieldConfig>
	</configInfo>
	<dataSource xmlns=""COE.FormGroup""/>
	<dataSourceId xmlns=""COE.FormGroup""/>
	<searchCriteriaItem fieldid=""951"" id=""" + nextId.ToString() + @""" tableid=""9"" xmlns=""COE.FormGroup"">
		<dateCriteria negate=""NO"" operator=""LTE"" xmlns=""COE.FormGroup""/>
	</searchCriteriaItem>
	<displayData xmlns=""COE.FormGroup""/>";

                                        layoutInfoNode.InsertAfter(endCreationDateFormElement, startCreationDateFormElement);
                                        messages.Add("The formElement name='END_CREATION_DATE' was added on coeFrom id='2' COEFormGroup id='4003'");

                                    }
                                }
                            }

                        }
                    }
                #endregion
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("CSBR132632 was successfully patched");
            }
            else
                messages.Add("CSBR132632 was patched with errors");
            return messages;
        }
    }
}     
    
