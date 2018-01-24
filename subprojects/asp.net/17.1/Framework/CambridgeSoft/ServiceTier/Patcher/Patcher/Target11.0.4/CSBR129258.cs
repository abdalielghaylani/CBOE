using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CSBR129258 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                string xmlns = "COE.FormGroup";
                string prefix = "COE";
                manager.AddNamespace(prefix, xmlns);

                if (id == "4014")
                {
                    if (doc.DocumentElement.Attributes["dataViewId"].Value == "8")
                        doc.DocumentElement.Attributes["dataViewId"].Value = "0";
                    XmlNode detailsForms = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms", manager);
                    #region mixture form
                    XmlNode oldComponentForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']", manager);
                    if (oldComponentForm != null)
                    {
                        if (oldComponentForm.Attributes["dataSourceId"].Value == "ComponentListCslaDataSource")
                        {
                            oldComponentForm.Attributes["dataSourceId"].Value = "mixtureCslaDataSource";

                            if (oldComponentForm.SelectSingleNode("COE:titleCssClass", manager) == null)
                                oldComponentForm.InsertBefore(doc.CreateNode(XmlNodeType.Element, "titleCssClass", xmlns), oldComponentForm.FirstChild);
                            oldComponentForm.SelectSingleNode("COE:titleCssClass", manager).InnerText = "COEFormTitle";

                            if (oldComponentForm.SelectSingleNode("COE:title", manager) == null)
                                oldComponentForm.InsertBefore(doc.CreateNode(XmlNodeType.Element, "title", xmlns), oldComponentForm.FirstChild);
                            oldComponentForm.SelectSingleNode("COE:title", manager).InnerText = "Duplicate Registry Information";

                            XmlNode oldComponentFormViewMode = oldComponentForm.SelectSingleNode("COE:viewMode", manager);
                            if (oldComponentFormViewMode != null)
                            {
                                #region mixtureForm formElements

                                oldComponentFormViewMode.InnerXml = @"<formElement name=""Reg Number"">
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>RegNumber.RegNum</bindingExpression>
		<Id>REGNUMBER_REGVIEWMODE</Id>
		<displayInfo>
			<style>height:30px;</style>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSClass>FETableTextBoxViewMode</CSSClass>
				<Style> border-color:#BCBCBC;color:#E4005C;font-family:Verdana;font-size:12px; font-weight: bold;background-color:#CFD8E6;height:20px;</Style>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""APPROVED"">
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>Approved</bindingExpression>
		<Id>APPROVEDProperty</Id>
		<displayInfo>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList>
			<validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters"" displayPosition=""Top_Left"">
				<params>
					<param name=""min"" value=""0""/>
					<param name=""max"" value=""200""/>
				</params>
			</validationRule>
		</validationRuleList>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSClass>state_control</CSSClass>
				<ItemCSSClass>ImageButton</ItemCSSClass>
				<DisplayType>ImageButton</DisplayType>
				<ReadOnly>true</ReadOnly>
				<States>
					<State text=""Approved"" value=""NotSet"" url=""~/App_Themes/Common/Images/UnknownStructure.png""/>
					<State text=""Approved"" value=""Approved"" url=""~/App_Themes/Common/Images/ThumbsUp.png""/>
					<State text=""Approved"" value=""Rejected"" url=""~/App_Themes/Common/Images/ThumbsDown.png""/>
				</States>
				<DefaultSelectedValue>0</DefaultSelectedValue>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""StructureAggregation"">
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>StructureAggregation</bindingExpression>
		<Id>BaseFragmentStructure</Id>
		<displayInfo>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSClass>FEStructureWide</CSSClass>
				<Height>285px</Height>
				<Width>700px</Width>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""AggregateStructure Details Lbl"">
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue>Shown structures have been normalized by ChemScript. For details select each component from the tree</defaultValue>
		<Id>AggregateStructureDetails</Id>
		<displayInfo>
			<height>30</height>
			<width>700</width>
			<cssClass/>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSClass>FELabel</CSSClass>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Projects"">
		<label>Projects</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>ProjectList</bindingExpression>
		<Id>Mix_ProjectsUltraGrid</Id>
		<displayInfo>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
			<HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
			<AddButtonCSS>AddButtonCSS</AddButtonCSS>
			<RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
			<RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
			<RowStyleCSS>RowStyleCSS</RowStyleCSS>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<AddRowTitle>Add Project</AddRowTitle>
				<RemoveRowTitle>Remove Project</RemoveRowTitle>
				<ReadOnly>true</ReadOnly>
				<DefaultEmptyRows>0</DefaultEmptyRows>
				<NoDataMessage>No Projects associated</NoDataMessage>
				<tables>
					<table>
						<Columns>
							<Column name=""Project"" dataTextValueField=""Name"" dataSourceID=""RegistryProjectsCslaDataSource"">
								<formElement>
									<Id>Mix_ProjectDropDownList</Id>
									<label>Project</label>
									<bindingExpression>ProjectID</bindingExpression>
									<configInfo>
										<fieldConfig>
											<CSSClass>FEDropDownListGrid</CSSClass>
											<CSSLabelClass>COERequiredField</CSSLabelClass>
											<DataSourceID>RegistryProjectsCslaDataSource</DataSourceID>
											<DataTextField>Name</DataTextField>
											<DataValueField>ProjectID</DataValueField>
											<ID>Mix_ProjectDropDownList</ID>
											<Columns>
												<Column key=""ProjectID"" title=""Project ID"" visible=""false""/>
												<Column key=""Name"" title=""Name""/>
												<Column key=""Description"" title=""Description""/>
											</Columns>
										</fieldConfig>
									</configInfo>
									<displayInfo>
										<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
									</displayInfo>
								</formElement>
							</Column>
							<Column name=""Name""/>
						</Columns>
					</table>
				</tables>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Identifiers"">
		<label>Identifiers</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>IdentifierList</bindingExpression>
		<Id>Mix_IdentifiersUltraGrid</Id>
		<displayInfo>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
			<HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
			<AddButtonCSS>AddButtonCSS</AddButtonCSS>
			<RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
			<RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
			<RowStyleCSS>RowStyleCSS</RowStyleCSS>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<AddRowTitle>Add Identifier</AddRowTitle>
				<RemoveRowTitle>Remove Identifier</RemoveRowTitle>
				<ReadOnly>true</ReadOnly>
				<DefaultEmptyRows>0</DefaultEmptyRows>
				<NoDataMessage>No Identifiers associated</NoDataMessage>
				<tables>
					<table>
						<Columns>
							<Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
							<!-- Identifier type selection drop down column -->
							<Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""RegistryIdentifiersCslaDataSource"">
								<formElement>
									<Id>Mix_IdentifiersDropDownList</Id>
									<label>Type</label>
									<bindingExpression>IdentifierID</bindingExpression>
									<configInfo>
										<fieldConfig>
											<CSSClass>FEDropDownListGrid</CSSClass>
											<CSSLabelClass>COERequiredField</CSSLabelClass>
											<DataSourceID>RegistryIdentifiersCslaDataSource</DataSourceID>
											<DataTextField>Name</DataTextField>
											<DataValueField>IdentifierID</DataValueField>
											<ID>Inner_Mix_IdentifiersDropDownList</ID>
											<Columns>
												<Column key=""IdentifierID"" title=""Identifier ID"" visible=""false""/>
												<Column key=""Name"" title=""Name""/>
												<Column key=""Description"" title=""Description""/>
											</Columns>
										</fieldConfig>
									</configInfo>
									<displayInfo>
										<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
									</displayInfo>
								</formElement>
							</Column>
							<!-- Identifier Name -->
							<Column name=""Value"">
								<formElement>
									<Id>Mix_IdentifiersNameTextEdit</Id>
									<bindingExpression>InputText</bindingExpression>
									<configInfo>
										<fieldConfig/>
										<MaxLength>50</MaxLength>
									</configInfo>
									<displayInfo>
										<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
									</displayInfo>
								</formElement>
							</Column>
							<Column name=""Name""/>
						</Columns>
					</table>
				</tables>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>";
                                #endregion

                                messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=0 was successfully updated");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=0 viewMode node was not found");
                            }
                        }
                        else
                        {
                            messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=0 dataSourceId was not the expected");
                            errorsInPatch = true;
                        }
                    }
                    else
                    {
                        messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=0 was not found");
                        errorsInPatch = true;
                    }

                    #endregion

                    #region custom mixture form
                    XmlNode oldCustomMixtureForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1000']", manager);
                    if (oldCustomMixtureForm != null)
                    {
                        if (oldCustomMixtureForm.SelectSingleNode("COE:title", manager) == null)
                            oldCustomMixtureForm.InsertBefore(doc.CreateNode(XmlNodeType.Element, "title", xmlns), oldCustomMixtureForm.FirstChild);

                        oldCustomMixtureForm.SelectSingleNode("COE:title", manager).InnerText = "Duplicate Registry Custom Properties";

                        XmlNode oldCustomMixtureFormViewMode = oldCustomMixtureForm.SelectSingleNode("COE:viewMode", manager);
                        List<XmlNode> oldCustomMixtureFElements = new List<XmlNode>();
                        if (oldCustomMixtureFormViewMode == null)
                        {
                            oldCustomMixtureFormViewMode = doc.CreateNode(XmlNodeType.Element, "viewMode", xmlns);
                            oldCustomMixtureForm.AppendChild(oldCustomMixtureFormViewMode);
                        }
                        else
                        {
                            foreach (XmlNode formElement in oldCustomMixtureFormViewMode.ChildNodes)
                            {
                                if (formElement.SelectSingleNode("COE:Id", manager).InnerText != "REG_COMMENTS")
                                    oldCustomMixtureFElements.Add(formElement);
                            }
                        }

                        #region custom mixture formElements

                        oldCustomMixtureFormViewMode.InnerXml = @"<formElement name=""REG_COMMENTS"">
	<label>Registry Comments</label>
	<showHelp>false</showHelp>
	<isFileUpload>false</isFileUpload>
	<pageComunicationProvider/>
	<fileUploadBindingExpression/>
	<helpText/>
	<defaultValue/>
	<bindingExpression>PropertyList[@Name='REG_COMMENTS'| Value]</bindingExpression>
	<Id>REG_COMMENTSProperty</Id>
	<displayInfo>
		<cssClass>Std100x80</cssClass>
		<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly</type>
		<visible>true</visible>
	</displayInfo>
	<validationRuleList/>
	<serverEvents/>
	<clientEvents/>
	<COE:configInfo xmlns:COE=""COE.FormGroup"">
		<COE:fieldConfig>
			<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
			<COE:CSSClass>FETextAreaViewMode</COE:CSSClass>
			<COE:TextMode>MultiLine</COE:TextMode>
		</COE:fieldConfig>
	</COE:configInfo>
	<dataSource/>
	<dataSourceId/>
	<requiredStyle/>
	<displayData/>
</formElement>
";
                        #endregion

                        foreach (XmlNode formElement in oldCustomMixtureFElements)
                            oldCustomMixtureFormViewMode.InsertBefore(formElement, oldCustomMixtureFormViewMode.FirstChild);

                        messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=1000 was successfully updated");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=1000 was not found");
                    }
                    #endregion

                    #region component form
                    XmlNode oldBatchForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']", manager);

                    if (oldBatchForm != null)
                    {
                        if (oldBatchForm.Attributes["dataSourceId"].Value == "BatchCslaDataSource")
                        {
                            oldBatchForm.Attributes["dataSourceId"].Value = "ComponentListCslaDataSource";
                            if (oldBatchForm.SelectSingleNode("COE:title", manager) == null)
                                oldBatchForm.InsertBefore(doc.CreateNode(XmlNodeType.Element, "title", xmlns), oldComponentForm.FirstChild);

                            oldBatchForm.SelectSingleNode("COE:title", manager).InnerText = "Component Information";

                            XmlNode oldBatchFormViewMode = oldBatchForm.SelectSingleNode("COE:viewMode", manager);
                            if (oldBatchFormViewMode != null)
                            {
                                #region component form formElements
                                oldBatchFormViewMode.InnerXml = @"<formElement name=""BaseFragmentStructure"">
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<bindingExpression>Compound.BaseFragment.Structure.Value</bindingExpression>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<Id>BaseFragmentStructure</Id>
		<displayInfo>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSClass>FEStructureViewMode</CSSClass>
				<Height>200px</Height>
				<Width>200px</Width>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Component ID"">
		<label>Component ID</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>Compound.RegNumber.RegNum</bindingExpression>
		<Id>CID</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList>
			<validationRule validationRuleName=""textLength"" errorMessage=""The length must be between 0 and 120"" displayPosition=""Top_Left"">
				<params>
					<param name=""min"" value=""0""/>
					<param name=""max"" value=""120""/>
				</params>
			</validationRule>
		</validationRuleList>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""MF"">
		<label>MF</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>Compound.BaseFragment.Structure.Formula</bindingExpression>
		<Id>Formula</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""MW"">
		<label>MW</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>Compound.BaseFragment.Structure.MolWeight</bindingExpression>
		<Id>MolWeight</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
				<Mask>####.##</Mask>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Identifiers"">
		<label>Identifiers</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>Compound.IdentifierList</bindingExpression>
		<Id>Compound_IdentifiersUltraGrid</Id>
		<displayInfo>
			<style>width:40%</style>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
			<HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
			<AddButtonCSS>AddButtonCSS</AddButtonCSS>
			<RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
			<RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
			<RowStyleCSS>RowStyleCSS</RowStyleCSS>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<AddRowTitle>Add Identifier</AddRowTitle>
				<RemoveRowTitle>Remove Identifier</RemoveRowTitle>
				<ReadOnly>true</ReadOnly>
				<DefaultEmptyRows>1</DefaultEmptyRows>
				<NoDataMessage>No Identifiers associated</NoDataMessage>
				<tables>
					<table>
						<Columns>
							<Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
							<!-- Identifier type selection drop down column -->
							<Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""CompoundIdentifiersCslaDataSource"">
								<formElement>
									<Id>VCompound_IdentifiersDropDownList</Id>
									<label>Type</label>
									<bindingExpression>IdentifierID</bindingExpression>
									<configInfo>
										<fieldConfig>
											<CSSClass>FEDropDownListGrid</CSSClass>
											<CSSLabelClass>COERequiredField</CSSLabelClass>
											<DataSourceID>CompoundIdentifiersCslaDataSource</DataSourceID>
											<DataTextField>Name</DataTextField>
											<DataValueField>IdentifierID</DataValueField>
											<ID>Inner_VCompound_IdentifiersDropDownList</ID>
											<Columns>
												<Column key=""IdentifierID"" title=""Identifier ID"" visible=""false""/>
												<Column key=""Name"" title=""Name""/>
												<Column key=""Description"" title=""Description""/>
											</Columns>
										</fieldConfig>
									</configInfo>
									<displayInfo>
										<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
									</displayInfo>
								</formElement>
							</Column>
							<!-- Identifier Name -->
							<Column name=""Value"">
								<formElement>
									<Id>VCompound_IdentifiersNameTextEdit</Id>
									<bindingExpression>InputText</bindingExpression>
									<configInfo>
										<fieldConfig/>
										<MaxLength>50</MaxLength>
									</configInfo>
									<displayInfo>
										<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
									</displayInfo>
								</formElement>
							</Column>
							<Column name=""Name""/>
						</Columns>
					</table>
				</tables>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>";
                                #endregion

                                messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=1 was successfully updated");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=1 viewMode node was not found");
                            }

                        }
                        else
                        {
                            messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=1 dataSourceId was not the expected");
                            errorsInPatch = true;
                        }
                    }
                    else
                    {
                        messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=1 was not found");
                        errorsInPatch = true;
                    }
                    #endregion

                    #region custom component form
                    XmlNode customComponentForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']", manager);
                    if (customComponentForm != null)
                    {
                        XmlNode customComponentFormViewMode = customComponentForm.SelectSingleNode("COE:viewMode", manager);
                        List<XmlNode> customComponentFormFElements = new List<XmlNode>();
                        if (customComponentFormViewMode == null)
                        {
                            customComponentFormViewMode = doc.CreateNode(XmlNodeType.Element, "viewMode", xmlns);
                            customComponentForm.AppendChild(customComponentFormViewMode);
                        }
                        else
                        {
                            foreach (XmlNode formElement in customComponentFormViewMode.ChildNodes)
                            {
                                if (formElement.SelectSingleNode("COE:Id", manager).InnerText != "CHEM_NAME_AUTOGENProperty")
                                    customComponentFormFElements.Add(formElement);
                            }
                        }

                        #region custom component formElements
                        customComponentFormViewMode.InnerXml = @"	<formElement name=""CHEM_NAME_AUTOGEN"">
		<label>Structure Name</label>
		<showHelp>false</showHelp>
		<helpText/>
		<defaultValue/>
		<bindingExpression>Compound.PropertyList[@Name='CHEM_NAME_AUTOGEN'| Value]</bindingExpression>
		<Id>CHEM_NAME_AUTOGENProperty</Id>
		<displayInfo>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
		</displayInfo>
		<validationRuleList>
			<validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters"" displayPosition=""Top_Left"">
				<params>
					<param name=""min"" value=""0""/>
					<param name=""max"" value=""200""/>
				</params>
			</validationRule>
		</validationRuleList>
		<serverEvents/>
		<clientEvents/>
		<configInfo/>
		<dataSource/>
		<dataSourceId/>
		<displayData/>
	</formElement>";
                        #endregion

                        foreach (XmlNode formElement in customComponentFormFElements)
                            customComponentFormViewMode.InsertBefore(formElement, customComponentFormViewMode.FirstChild);
                        if (oldBatchForm != null)
                            detailsForms.InsertAfter(customComponentForm, oldBatchForm);
                        else
                            messages.Add("WARNING:COEFormGroup id=4014 detailsForm id=0 coeForm id=1001 could be located in a wrong position");
                        messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=1001 was successfully updated");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=1001 was not found");
                    }
                    #endregion

                    #region batch form

                    XmlNode batchForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']", manager);
                    if (batchForm != null)
                    {
                        XmlNode batchFormViewMode = batchForm.SelectSingleNode("COE:viewMode", manager);
                        if (batchFormViewMode != null)
                        {
                            #region batch form formElements
                            batchFormViewMode.InnerXml = @"	<formElement name=""Batch ID"">
		<label>Batch ID</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>ID</bindingExpression>
		<Id>BatchIDTextBox</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Full Reg Number"">
		<label>Full Registry Number</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>FullRegNumber</bindingExpression>
		<Id>FullRegNumber</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Date Created"">
		<label>Date Created</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>DateCreated</bindingExpression>
		<Id>DateCreatedTextBox</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
				<NullDateLabel/>
				<NullValueRepresentation>NotSet</NullValueRepresentation>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""Last Modification Date"">
		<label>Last Modification Date</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>DateLastModified</bindingExpression>
		<Id>ModificationDateTextBox</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<configInfo>
			<fieldConfig>
				<CSSLabelClass>FELabel</CSSLabelClass>
				<CSSClass>FETextBoxViewMode</CSSClass>
				<NullDateLabel/>
				<NullValueRepresentation>NotSet</NullValueRepresentation>
			</fieldConfig>
		</configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>";
                            #endregion

                            messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=2 was successfully updated");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("ERROR:COEFormGroup id=4014 detailsForm id =0 coeForm id=2 was not found");
                        }
                    }

                    #endregion

                    #region batch custom form

                    XmlNode batchCustomForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']", manager);
                    if (batchCustomForm != null)
                    {
                        XmlNode batchCustomFormViewMode = batchCustomForm.SelectSingleNode("COE:viewMode", manager);
                        List<XmlNode> batchCustomFormFElements = new List<XmlNode>();
                        if (batchCustomFormViewMode == null)
                        {
                            batchCustomFormViewMode = doc.CreateNode(XmlNodeType.Element, "viewMode", xmlns);
                            batchCustomForm.AppendChild(batchCustomFormViewMode);
                        }
                        else
                        {
                            string[] ids = { "SCIENTIST_IDProperty", "CREATION_DATEProperty", "NOTEBOOK_TEXTProperty", "AMOUNTProperty", "AMOUNT_UNITSProperty", "APPEARANCEProperty", "PURITYProperty", "PURITY_COMMENTSProperty", "SAMPLEIDProperty", "SOLUBILITYProperty", "BATCH_COMMENTProperty", "STORAGE_REQ_AND_WARNINGSProperty", "FORMULA_WEIGHTProperty", "BATCH_FORMULAProperty", "PERCENT_ACTIVEProperty", "FORMULAWEIGHTProperty", "MOLECULARFORMULAProperty", "PERCENTACTIVEProperty" };
                            List<string> formElementsIds = new List<string>(ids);
                            foreach (XmlNode formElement in batchCustomFormViewMode.ChildNodes)
                            {
                                if (!formElementsIds.Contains(formElement.SelectSingleNode("COE:Id", manager).InnerText))
                                    batchCustomFormFElements.Add(formElement);
                            }
                        }

                        #region batch custom formElements
                        batchCustomFormViewMode.InnerXml = @"	<formElement name=""SCIENTIST_ID"">
		<label>Scientist</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='SCIENTIST_ID'| Value]</bindingExpression>
		<Id>SCIENTIST_IDProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FEDropDownListViewMode</COE:CSSClass>
				<COE:dropDownItemsSelect>SELECT PERSON_ID as key,USER_ID as value FROM COEDB.PEOPLE</COE:dropDownItemsSelect>
				<COE:Enable>False</COE:Enable>
				<COE:ID>SCIENTIST_IDProperty</COE:ID>
				<COE:AutoPostBack>False</COE:AutoPostBack>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""CREATION_DATE"">
		<label>Synthesis Date</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='CREATION_DATE'| Value]</bindingExpression>
		<Id>CREATION_DATEProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:MaxDate>1/1/0001 12:00:00 AM</COE:MaxDate>
				<COE:DateFormat>Short</COE:DateFormat>
				<COE:Editable>False</COE:Editable>
				<COE:AllowNull>True</COE:AllowNull>
				<COE:AutoCloseUp>True</COE:AutoCloseUp>
				<COE:FirstDayOfWeek>Default</COE:FirstDayOfWeek>
				<COE:GridLines>None</COE:GridLines>
				<COE:NullDateLabel>Null</COE:NullDateLabel>
				<COE:NullValueRepresentation>NotSet</COE:NullValueRepresentation>
				<COE:Section508Compliant>False</COE:Section508Compliant>
				<COE:CSSClass/>
				<COE:FontSize/>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""NOTEBOOK_TEXT"">
		<label>Notebook Reference</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='NOTEBOOK_TEXT'| Value]</bindingExpression>
		<Id>NOTEBOOK_TEXTProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""AMOUNT"">
		<label>Amount</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='AMOUNT'| Value]</bindingExpression>
		<Id>AMOUNTProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""AMOUNT_UNITS"">
		<label>Units</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='AMOUNT_UNITS'| Value]</bindingExpression>
		<Id>AMOUNT_UNITSProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FEDropDownListViewMode</COE:CSSClass>
				<COE:dropDownItemsSelect>SELECT ID as key,UNIT as value FROM REGDB.VW_Unit ORDER BY UNIT ASC</COE:dropDownItemsSelect>
				<COE:Enable>False</COE:Enable>
				<COE:ID>AMOUNT_UNITSProperty</COE:ID>
				<COE:AutoPostBack>False</COE:AutoPostBack>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""APPEARANCE"">
		<label>Appearance</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='APPEARANCE'| Value]</bindingExpression>
		<Id>APPEARANCEProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""PURITY"">
		<label>Purity</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='PURITY'| Value]</bindingExpression>
		<Id>PURITYProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""PURITY_COMMENTS"">
		<label>Purity Comments</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='PURITY_COMMENTS'| Value]</bindingExpression>
		<Id>PURITY_COMMENTSProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""SAMPLEID"">
		<label>Sample ID</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='SAMPLEID'| Value]</bindingExpression>
		<Id>SAMPLEIDProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""SOLUBILITY"">
		<label>Solubility</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='SOLUBILITY'| Value]</bindingExpression>
		<Id>SOLUBILITYProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""BATCH_COMMENT"">
		<label>Batch Comments</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='BATCH_COMMENT'| Value]</bindingExpression>
		<Id>BATCH_COMMENTProperty</Id>
		<displayInfo>
			<cssClass>Std50x80</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextAreaViewMode</COE:CSSClass>
				<COE:TextMode>MultiLine</COE:TextMode>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""STORAGE_REQ_AND_WARNINGS"">
		<label>Storage Requirements Warnings</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='STORAGE_REQ_AND_WARNINGS'| Value]</bindingExpression>
		<Id>STORAGE_REQ_AND_WARNINGSProperty</Id>
		<displayInfo>
			<cssClass>Std50x80</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextAreaViewMode</COE:CSSClass>
				<COE:TextMode>MultiLine</COE:TextMode>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""FORMULA_WEIGHT"">
		<label>Formula Weight</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='FORMULA_WEIGHT'| Value]</bindingExpression>
		<Id>FORMULA_WEIGHTProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
				<COE:Mask>####.##</COE:Mask>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""BATCH_FORMULA"">
		<label>Molecular Formula</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='BATCH_FORMULA'| Value]</bindingExpression>
		<Id>BATCH_FORMULAProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>
	<formElement name=""PERCENT_ACTIVE"">
		<label>Percent Active</label>
		<showHelp>false</showHelp>
		<isFileUpload>false</isFileUpload>
		<pageComunicationProvider/>
		<fileUploadBindingExpression/>
		<helpText/>
		<defaultValue/>
		<bindingExpression>PropertyList[@Name='PERCENT_ACTIVE'| Value]</bindingExpression>
		<Id>PERCENT_ACTIVEProperty</Id>
		<displayInfo>
			<cssClass>Std20x40</cssClass>
			<type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
			<visible>true</visible>
		</displayInfo>
		<validationRuleList/>
		<serverEvents/>
		<clientEvents/>
		<COE:configInfo xmlns:COE=""COE.FormGroup"">
			<COE:fieldConfig>
				<COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
				<COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
				<COE:PassWord>false</COE:PassWord>
				<COE:ReadOnly>true</COE:ReadOnly>
			</COE:fieldConfig>
		</COE:configInfo>
		<dataSource/>
		<dataSourceId/>
		<requiredStyle/>
		<displayData/>
	</formElement>";
                        #endregion

                        foreach (XmlNode formElement in batchCustomFormFElements)
                            batchCustomFormViewMode.InsertBefore(formElement, batchCustomFormViewMode.FirstChild);

                        messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=1002 was successfully updated");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("ERROR:COEFormGroup id=4014 detailsForm id=0 coeForm id=1002 was not found");
                    }

                    #endregion

                    #region batch component custom form

                    XmlNode batchCompCustomForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1003']", manager);

                    if (batchCompCustomForm != null && batchCustomForm != null)
                    {
                        if (batchCompCustomForm.SelectSingleNode("COE:title", manager) == null)
                            batchCompCustomForm.InsertBefore(doc.CreateNode(XmlNodeType.Element, "title", xmlns), batchCompCustomForm.FirstChild);

                        batchCompCustomForm.SelectSingleNode("COE:title", manager).InnerText = "Batch Component Information";

                        detailsForms.InsertAfter(batchCompCustomForm, batchCustomForm);
                        messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeFrom id=1003 was successfully relocated");
                    }
                    else
                        messages.Add("WARNING:COEFormGroup id=4014 detailsForm id=0 coeFrom id=1003 could be located in a wrong position");

                    XmlNode fragmentsForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='6']", manager);
                    if (fragmentsForm != null)
                    {
                        detailsForms.RemoveChild(fragmentsForm);
                        messages.Add("SUCCESS:COEFormGroup id=4014 detailsForm id=0 coeForm id=6 was successfully removed");
                    }
                    else
                        messages.Add("WARNING:COEFormGroup id=4014 detailsForm id=0 coeForm id=6 was not found");

                    XmlNodeList allAddModeNodes = doc.DocumentElement.SelectNodes("//COE:addMode", manager);
                    XmlNodeList allEditModeNodes = doc.DocumentElement.SelectNodes("//COE:editMode", manager);

                    foreach (XmlNode node in allAddModeNodes)
                        node.RemoveAll();
                    foreach (XmlNode node in allEditModeNodes)
                        node.RemoveAll();
                    messages.Add("SUCCESS:All add and edit nodes were cleaned successfully");

                    #endregion
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR129258 was successfully patched");
            else
                messages.Add("CSBR129258 was patched with errors");
            return messages;
        }
    }
}
