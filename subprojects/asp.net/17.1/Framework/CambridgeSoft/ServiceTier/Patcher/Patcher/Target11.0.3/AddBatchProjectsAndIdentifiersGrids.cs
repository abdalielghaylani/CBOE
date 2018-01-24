using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Projects and Identifiers need to be added for supporting them at batch level in forms 4011, 4012 and 4013
    /// </summary>
    public class AddBatchProjectsAndIdentifiersGrids : BugFixBaseCommand
	{
        /// <summary>
        /// No manual steps are provided
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
                #region Form 4011
                if (id == "4011")
                {
                    XmlNode editModeF10024011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode", manager);
                    if (editModeF10024011.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager) == null)
                    {
                        XmlNode identifiersFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Identifiers";
                        identifiersFormElement.Attributes.Append(name);
                        identifiersFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
                            <showHelp xmlns=""COE.FormGroup"">false</showHelp>
                            <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                            <pageComunicationProvider xmlns=""COE.FormGroup""/>
                            <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                            <helpText xmlns=""COE.FormGroup""/>
                            <defaultValue xmlns=""COE.FormGroup""/>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierList</bindingExpression>
                            <Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
                            <displayInfo xmlns=""COE.FormGroup"">
                                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                                <visible xmlns=""COE.FormGroup"">true</visible>
                            </displayInfo>
                            <validationRuleList xmlns=""COE.FormGroup""/>
                            <serverEvents xmlns=""COE.FormGroup""/>
                            <clientEvents xmlns=""COE.FormGroup""/>
                            <configInfo xmlns=""COE.FormGroup"">
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
                                    <ReadOnly>false</ReadOnly>
                                    <DefaultEmptyRows>0</DefaultEmptyRows>
                                    <NoDataMessage>No Identifiers associated</NoDataMessage>
                                    <tables>
                                        <table>
                                            <Columns>
                                                <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
                                                <!-- Identifier type selection drop down column -->
                                                <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""BatchIdentifiersCslaDataSource"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_IdentifiersDropDownList</Id>
                                                        <label xmlns=""COE.FormGroup"">Type</label>
                                                        <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig>
                                                                <CSSClass>FEDropDownListGrid</CSSClass>
                                                                <CSSLabelClass>COERequiredField</CSSLabelClass>
                                                                <DataSourceID>BatchIdentifiersCslaDataSource</DataSourceID>
                                                                <DataTextField>Name</DataTextField>
                                                                <DataValueField>IdentifierID</DataValueField>
                                                                <ID>Inner_Batch_IdentifiersDropDownList</ID>
                                                                <Columns>
                                                                    <Column key=""IdentifierID"" title=""Identifier ID"" visible=""false""/>
                                                                    <Column key=""Name"" title=""Name""/>
                                                                    <Column key=""Description"" title=""Description""/>
                                                                </Columns>
                                                            </fieldConfig>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <!-- Identifier Name -->
                                                <Column name=""Value"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_IdentifiersNameTextEdit</Id>
                                                        <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig/>
                                                            <MaxLength>50</MaxLength>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <Column name=""Name""/>
                                            </Columns>
                                        </table>
                                    </tables>
                                </fieldConfig>
                            </configInfo>
                            <dataSource xmlns=""COE.FormGroup""/>
                            <dataSourceId xmlns=""COE.FormGroup""/>
                            <requiredStyle xmlns=""COE.FormGroup""/>
                            <displayData xmlns=""COE.FormGroup""/>";
                        editModeF10024011.InsertBefore(identifiersFormElement, editModeF10024011.FirstChild);
                        messages.Add("Identifiers form element was successfully added in form 1002 of coeform 4011 in editMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was already present in form 1002 of coeform 4011 in editMode");
                    }

                    if (editModeF10024011.SelectSingleNode("./COE:formElement[@name='Projects']", manager) == null)
                    {
                        XmlNode projectsFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Projects";
                        projectsFormElement.Attributes.Append(name);
                        projectsFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Projects</label>
                            <showHelp xmlns=""COE.FormGroup"">false</showHelp>
                            <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                            <pageComunicationProvider xmlns=""COE.FormGroup""/>
                            <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                            <helpText xmlns=""COE.FormGroup""/>
                            <defaultValue xmlns=""COE.FormGroup""/>
                            <bindingExpression xmlns=""COE.FormGroup"">ProjectList</bindingExpression>
                            <Id xmlns=""COE.FormGroup"">Batch_ProjectsUltraGrid</Id>
                            <displayInfo xmlns=""COE.FormGroup"">                                
                                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                                <visible xmlns=""COE.FormGroup"">true</visible>
                            </displayInfo>
                            <validationRuleList xmlns=""COE.FormGroup""/>
                            <serverEvents xmlns=""COE.FormGroup""/>
                            <clientEvents xmlns=""COE.FormGroup""/>
                            <configInfo xmlns=""COE.FormGroup"">
                                <HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
                                <HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
                                <AddButtonCSS>AddButtonCSS</AddButtonCSS>
                                <RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
                                <RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
                                <RowStyleCSS>RowStyleCSS</RowStyleCSS>
                                <SelectedRowStyleCSS>RowSelectedStyleCSS</SelectedRowStyleCSS>
                                <fieldConfig>
                                    <AddRowTitle>Add Project</AddRowTitle>
                                    <RemoveRowTitle>Remove Project</RemoveRowTitle>
                                    <ReadOnly>false</ReadOnly>
                                    <DefaultEmptyRows>1</DefaultEmptyRows>
                                    <tables>
                                        <table>
                                            <Columns>
                                                <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
                                                <Column name=""Project"" dataTextValueField=""Name"" dataSourceID=""BatchProjectsCslaDataSource"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_ProjectDropDownList</Id>
                                                        <label xmlns=""COE.FormGroup"">Project</label>
                                                        <bindingExpression xmlns=""COE.FormGroup"">ProjectID</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig>
                                                                <CSSClass>FEDropDownListGrid</CSSClass>
                                                                <CSSLabelClass>FELabel</CSSLabelClass>
                                                                <DataSourceID>BatchProjectsCslaDataSource</DataSourceID>
                                                                <DataTextField>Name</DataTextField>
                                                                <DataValueField>ProjectID</DataValueField>
                                                                <ID>Inner_B_ProjectDropDownList</ID>
                                                                <Columns>
                                                                    <Column key=""ProjectID"" title=""Project ID"" visible=""false""/>
                                                                    <Column key=""Name"" title=""Name""/>
                                                                    <Column key=""Description"" title=""Description""/>
                                                                </Columns>
                                                            </fieldConfig>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <Column name=""Name""/>
                                            </Columns>
                                        </table>
                                    </tables>
                                    <ClientSideEvents>
                                        <Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(ProjectID)}</Event>
                                    </ClientSideEvents>
                                </fieldConfig>
                            </configInfo>
                            <dataSource xmlns=""COE.FormGroup""/>
                            <dataSourceId xmlns=""COE.FormGroup""/>
                            <requiredStyle xmlns=""COE.FormGroup""/>
                            <displayData xmlns=""COE.FormGroup""/>";
                        editModeF10024011.InsertBefore(projectsFormElement, editModeF10024011.FirstChild);
                        messages.Add("Projects form element was successfully added in form 1002 of coeform 4011 in editMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Projects form element was already present in form 1002 of coeform 4011 in editMode");
                    }
                    //-----------------
                    XmlNode viewModeF10024011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:viewMode", manager);
                    if (viewModeF10024011.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager) == null)
                    {
                        XmlNode identifiersFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Identifiers";
                        identifiersFormElement.Attributes.Append(name);
                        identifiersFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
                            <showHelp xmlns=""COE.FormGroup"">false</showHelp>
                            <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                            <pageComunicationProvider xmlns=""COE.FormGroup""/>
                            <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                            <helpText xmlns=""COE.FormGroup""/>
                            <defaultValue xmlns=""COE.FormGroup""/>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierList</bindingExpression>
                            <Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
                            <displayInfo xmlns=""COE.FormGroup"">
                                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                                <visible xmlns=""COE.FormGroup"">true</visible>
                            </displayInfo>
                            <validationRuleList xmlns=""COE.FormGroup""/>
                            <serverEvents xmlns=""COE.FormGroup""/>
                            <clientEvents xmlns=""COE.FormGroup""/>
                            <configInfo xmlns=""COE.FormGroup"">
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
                                                <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""BatchIdentifiersCslaDataSource"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_IdentifiersDropDownList</Id>
                                                        <label xmlns=""COE.FormGroup"">Type</label>
                                                        <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig>
                                                                <CSSClass>FEDropDownListGrid</CSSClass>
                                                                <CSSLabelClass>COERequiredField</CSSLabelClass>
                                                                <DataSourceID>BatchIdentifiersCslaDataSource</DataSourceID>
                                                                <DataTextField>Name</DataTextField>
                                                                <DataValueField>IdentifierID</DataValueField>
                                                                <ID>Inner_Batch_IdentifiersDropDownList</ID>
                                                                <Columns>
                                                                    <Column key=""IdentifierID"" title=""Identifier ID"" visible=""false""/>
                                                                    <Column key=""Name"" title=""Name""/>
                                                                    <Column key=""Description"" title=""Description""/>
                                                                </Columns>
                                                            </fieldConfig>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <!-- Identifier Name -->
                                                <Column name=""Value"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_IdentifiersNameTextEdit</Id>
                                                        <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig/>
                                                            <MaxLength>50</MaxLength>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <Column name=""Name""/>
                                            </Columns>
                                        </table>
                                    </tables>
                                </fieldConfig>
                            </configInfo>
                            <dataSource xmlns=""COE.FormGroup""/>
                            <dataSourceId xmlns=""COE.FormGroup""/>
                            <requiredStyle xmlns=""COE.FormGroup""/>
                            <displayData xmlns=""COE.FormGroup""/>";
                        viewModeF10024011.InsertAfter(identifiersFormElement, viewModeF10024011.FirstChild);
                        messages.Add("Identifiers form element was successfully added in form 1002 of coeform 4011 in viewMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was already present in form 1002 of coeform 4011 in viewMode");
                    }

                    if (viewModeF10024011.SelectSingleNode("./COE:formElement[@name='Projects']", manager) == null)
                    {
                        XmlNode projectsFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Projects";
                        projectsFormElement.Attributes.Append(name);
                        projectsFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Projects</label>
                            <showHelp xmlns=""COE.FormGroup"">false</showHelp>
                            <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                            <pageComunicationProvider xmlns=""COE.FormGroup""/>
                            <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                            <helpText xmlns=""COE.FormGroup""/>
                            <defaultValue xmlns=""COE.FormGroup""/>
                            <bindingExpression xmlns=""COE.FormGroup"">ProjectList</bindingExpression>
                            <Id xmlns=""COE.FormGroup"">Batch_ProjectsUltraGrid</Id>
                            <displayInfo xmlns=""COE.FormGroup"">                                
                                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                                <visible xmlns=""COE.FormGroup"">true</visible>
                            </displayInfo>
                            <validationRuleList xmlns=""COE.FormGroup""/>
                            <serverEvents xmlns=""COE.FormGroup""/>
                            <clientEvents xmlns=""COE.FormGroup""/>
                            <configInfo xmlns=""COE.FormGroup"">
                                <HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
                                <HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
                                <AddButtonCSS>AddButtonCSS</AddButtonCSS>
                                <RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
                                <RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
                                <RowStyleCSS>RowStyleCSS</RowStyleCSS>
                                <SelectedRowStyleCSS>RowSelectedStyleCSS</SelectedRowStyleCSS>
                                <fieldConfig>
                                    <AddRowTitle>Add Project</AddRowTitle>
                                    <RemoveRowTitle>Remove Project</RemoveRowTitle>
                                    <ReadOnly>true</ReadOnly>
                                    <DefaultEmptyRows>1</DefaultEmptyRows>
                                    <tables>
                                        <table>
                                            <Columns>
                                                <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
                                                <Column name=""Project"" dataTextValueField=""Name"" dataSourceID=""BatchProjectsCslaDataSource"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_ProjectDropDownList</Id>
                                                        <label xmlns=""COE.FormGroup"">Project</label>
                                                        <bindingExpression xmlns=""COE.FormGroup"">ProjectID</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig>
                                                                <CSSClass>FEDropDownListGrid</CSSClass>
                                                                <CSSLabelClass>FELabel</CSSLabelClass>
                                                                <DataSourceID>BatchProjectsCslaDataSource</DataSourceID>
                                                                <DataTextField>Name</DataTextField>
                                                                <DataValueField>ProjectID</DataValueField>
                                                                <ID>Inner_B_ProjectDropDownList</ID>
                                                                <Columns>
                                                                    <Column key=""ProjectID"" title=""Project ID"" visible=""false""/>
                                                                    <Column key=""Name"" title=""Name""/>
                                                                    <Column key=""Description"" title=""Description""/>
                                                                </Columns>
                                                            </fieldConfig>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <Column name=""Name""/>
                                            </Columns>
                                        </table>
                                    </tables>
                                    <ClientSideEvents>
                                        <Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(ProjectID)}</Event>
                                    </ClientSideEvents>
                                </fieldConfig>
                            </configInfo>
                            <dataSource xmlns=""COE.FormGroup""/>
                            <dataSourceId xmlns=""COE.FormGroup""/>
                            <requiredStyle xmlns=""COE.FormGroup""/>
                            <displayData xmlns=""COE.FormGroup""/>";
                        viewModeF10024011.InsertAfter(projectsFormElement, viewModeF10024011.FirstChild);
                        messages.Add("Projects form element was successfully added in form 1002 of coeform 4011 in viewMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Projects form element was already present in form 1002 of coeform 4011 in viewMode");
                    }
                }
                #endregion
                #region Form 4012
                else if (id == "4012")
                {
                    XmlNode editModeF44012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:editMode", manager);

                    if (editModeF44012.SelectSingleNode("./COE:formElement[@name='Projects']", manager) == null)
                    {
                        XmlNode ProjectsFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Projects";
                        ProjectsFormElement.Attributes.Append(name);
                        ProjectsFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Projects</label>
							<showHelp xmlns=""COE.FormGroup"">false</showHelp>
							<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
							<pageComunicationProvider xmlns=""COE.FormGroup""/>
							<fileUploadBindingExpression xmlns=""COE.FormGroup""/>
							<helpText xmlns=""COE.FormGroup""/>
							<defaultValue xmlns=""COE.FormGroup""/>
							<bindingExpression xmlns=""COE.FormGroup"">ProjectList</bindingExpression>
							<Id xmlns=""COE.FormGroup"">Batch_ProjectsUltraGrid</Id>
							<displayInfo xmlns=""COE.FormGroup"">
								<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
								<visible xmlns=""COE.FormGroup"">true</visible>
							</displayInfo>
							<validationRuleList xmlns=""COE.FormGroup""/>
							<serverEvents xmlns=""COE.FormGroup""/>
							<clientEvents xmlns=""COE.FormGroup""/>
							<configInfo xmlns=""COE.FormGroup"">
								<HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
								<HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
								<AddButtonCSS>AddButtonCSS</AddButtonCSS>
								<RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
								<RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
								<RowStyleCSS>RowStyleCSS</RowStyleCSS>
								<SelectedRowStyleCSS>RowSelectedStyleCSS</SelectedRowStyleCSS>
								<fieldConfig>
									<CSSLabelClass>FELabel</CSSLabelClass>
									<AddRowTitle>Add Project</AddRowTitle>
									<RemoveRowTitle>Remove Project</RemoveRowTitle>
									<ReadOnly>false</ReadOnly>
									<DefaultEmptyRows>1</DefaultEmptyRows>
									<tables>
										<table>
											<Columns>
												<Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
												<Column name=""Project"" dataTextValueField=""Name"" dataSourceID=""BatchProjectsCslaDataSource"">
													<formElement xmlns=""COE.FormGroup"">
														<Id xmlns=""COE.FormGroup"">Batch_ProjectDropDownList</Id>
														<label xmlns=""COE.FormGroup"">Project</label>
														<bindingExpression xmlns=""COE.FormGroup"">ProjectID</bindingExpression>
														<configInfo xmlns=""COE.FormGroup"">
															<fieldConfig>
																<CSSClass>FEDropDownListGrid</CSSClass>
																<CSSLabelClass>FELabel</CSSLabelClass>
																<DataSourceID>BatchProjectsCslaDataSource</DataSourceID>
																<DataTextField>Name</DataTextField>
																<DataValueField>ProjectID</DataValueField>
																<ID>Inner_B_ProjectDropDownList</ID>
																<Columns>
																	<Column key=""ProjectID"" title=""Project ID"" visible=""false""/>
																	<Column key=""Name"" title=""Name""/>
																	<Column key=""Description"" title=""Description""/>
																</Columns>
															</fieldConfig>
														</configInfo>
														<displayInfo xmlns=""COE.FormGroup"">
															<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
														</displayInfo>
													</formElement>
												</Column>
												<Column name=""Name""/>
											</Columns>
										</table>
									</tables>
									<ClientSideEvents>
										<Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(ProjectID)}</Event>
									</ClientSideEvents>
								</fieldConfig>
							</configInfo>
							<dataSource xmlns=""COE.FormGroup""/>
							<dataSourceId xmlns=""COE.FormGroup""/>
							<requiredStyle xmlns=""COE.FormGroup""/>
							<displayData xmlns=""COE.FormGroup""/>";
                        editModeF44012.AppendChild(ProjectsFormElement);
                        messages.Add("Projects form element was successfully added in form 4 of coeform 4012 in editMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Projects form element was already present in form 4 of coeform 4012 in editMode");
                    }

                    if (editModeF44012.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager) == null)
                    {
                        XmlNode identifiersFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Identifiers";
                        identifiersFormElement.Attributes.Append(name);
                        identifiersFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
							<showHelp xmlns=""COE.FormGroup"">false</showHelp>
							<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
							<pageComunicationProvider xmlns=""COE.FormGroup"" />
							<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
							<helpText xmlns=""COE.FormGroup"" />
							<defaultValue xmlns=""COE.FormGroup"" />
							<bindingExpression xmlns=""COE.FormGroup"">IdentifierList</bindingExpression>
							<Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
							<displayInfo xmlns=""COE.FormGroup"">
								<style xmlns=""COE.FormGroup"">width:40%</style>
								<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
								<visible xmlns=""COE.FormGroup"">true</visible>
							</displayInfo>
							<validationRuleList xmlns=""COE.FormGroup"" />
							<serverEvents xmlns=""COE.FormGroup"" />
							<clientEvents xmlns=""COE.FormGroup"" />
							<configInfo xmlns=""COE.FormGroup"">
								<HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
								<HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
								<AddButtonCSS>AddButtonCSS</AddButtonCSS>
								<RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
								<RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
								<RowStyleCSS>RowStyleCSS</RowStyleCSS>
								<SelectedRowStyleCSS>RowSelectedStyleCSS</SelectedRowStyleCSS>
								<fieldConfig>
									<CSSLabelClass>FELabel</CSSLabelClass>
									<AddRowTitle>Add Identifier</AddRowTitle>
									<RemoveRowTitle>Remove Identifier</RemoveRowTitle>
									<ReadOnly>false</ReadOnly>
									<DefaultEmptyRows>1</DefaultEmptyRows>
									<tables>
										<table>
											<Columns>
												<Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" />
												<!-- Identifier type selection drop down column -->
												<Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""BatchIdentifiersCslaDataSource"">
													<formElement xmlns=""COE.FormGroup"">
														<Id xmlns=""COE.FormGroup"">Batch_IdentifiersDropDownList</Id>
														<label xmlns=""COE.FormGroup"">Type</label>
														<bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
														<configInfo xmlns=""COE.FormGroup"">
															<fieldConfig>
																<CSSClass>FEDropDownListGrid</CSSClass>
																<CSSLabelClass>COELabel</CSSLabelClass>
																<DataSourceID>BatchIdentifiersCslaDataSource</DataSourceID>
																<DataTextField>Name</DataTextField>
																<DataValueField>IdentifierID</DataValueField>
																<ID>Inner_Batch_IdentifiersDropDownList</ID>
																<Columns>
																	<Column key=""IdentifierID"" title=""Identifier ID"" visible=""false"" />
																	<Column key=""Name"" title=""Name"" />
																	<Column key=""Description"" title=""Description"" />
																</Columns>
															</fieldConfig>
														</configInfo>
														<displayInfo xmlns=""COE.FormGroup"">
															<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
														</displayInfo>
													</formElement>
												</Column>
												<!-- Identifier Name -->
												<Column name=""Value"">
													<formElement xmlns=""COE.FormGroup"">
														<Id xmlns=""COE.FormGroup"">Batch_IdentifiersNameTextEdit</Id>
														<bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
														<configInfo xmlns=""COE.FormGroup"">
															<fieldConfig />
															<MaxLength>50</MaxLength>
														</configInfo>
														<displayInfo xmlns=""COE.FormGroup"">
															<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
														</displayInfo>
													</formElement>
												</Column>
												<Column name=""Name"" />
											</Columns>
										</table>
									</tables>
									<ClientSideEvents>
										<Event name=""AfterExitEdit"">
							AfterExitEdit();
										</Event>
									</ClientSideEvents>
									<DefaultRows>
										<Row>
											<cell bindingExpression=""IdentifierID"" dataValue=""4"" dataText=""Custom_FindFromEditor"" />
										</Row>
									</DefaultRows>
								</fieldConfig>
							</configInfo>
							<dataSource xmlns=""COE.FormGroup"" />
							<dataSourceId xmlns=""COE.FormGroup"" />
							<displayData xmlns=""COE.FormGroup"" />";
                        editModeF44012.AppendChild(identifiersFormElement);
                        messages.Add("Identifiers form element was successfully added in form 4 of coeform 4012 in editMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was already present in form 4 of coeform 4012 in editMode");
                    }

                    //-----------------
                    XmlNode viewModeF44012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:viewMode", manager);
                    if (viewModeF44012.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager) == null)
                    {
                        XmlNode identifiersFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Identifiers";
                        identifiersFormElement.Attributes.Append(name);
                        identifiersFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
                            <showHelp xmlns=""COE.FormGroup"">false</showHelp>
                            <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                            <pageComunicationProvider xmlns=""COE.FormGroup""/>
                            <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                            <helpText xmlns=""COE.FormGroup""/>
                            <defaultValue xmlns=""COE.FormGroup""/>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierList</bindingExpression>
                            <Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
                            <displayInfo xmlns=""COE.FormGroup"">
                                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                                <visible xmlns=""COE.FormGroup"">true</visible>
                            </displayInfo>
                            <validationRuleList xmlns=""COE.FormGroup""/>
                            <serverEvents xmlns=""COE.FormGroup""/>
                            <clientEvents xmlns=""COE.FormGroup""/>
                            <configInfo xmlns=""COE.FormGroup"">
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
                                                <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""BatchIdentifiersCslaDataSource"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_IdentifiersDropDownList</Id>
                                                        <label xmlns=""COE.FormGroup"">Type</label>
                                                        <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig>
                                                                <CSSClass>FEDropDownListGrid</CSSClass>
                                                                <CSSLabelClass>COERequiredField</CSSLabelClass>
                                                                <DataSourceID>BatchIdentifiersCslaDataSource</DataSourceID>
                                                                <DataTextField>Name</DataTextField>
                                                                <DataValueField>IdentifierID</DataValueField>
                                                                <ID>Inner_Batch_IdentifiersDropDownList</ID>
                                                                <Columns>
                                                                    <Column key=""IdentifierID"" title=""Identifier ID"" visible=""false""/>
                                                                    <Column key=""Name"" title=""Name""/>
                                                                    <Column key=""Description"" title=""Description""/>
                                                                </Columns>
                                                            </fieldConfig>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <!-- Identifier Name -->
                                                <Column name=""Value"">
                                                    <formElement xmlns=""COE.FormGroup"">
                                                        <Id xmlns=""COE.FormGroup"">Batch_IdentifiersNameTextEdit</Id>
                                                        <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                                                        <configInfo xmlns=""COE.FormGroup"">
                                                            <fieldConfig/>
                                                            <MaxLength>50</MaxLength>
                                                        </configInfo>
                                                        <displayInfo xmlns=""COE.FormGroup"">
                                                            <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                                                        </displayInfo>
                                                    </formElement>
                                                </Column>
                                                <Column name=""Name""/>
                                            </Columns>
                                        </table>
                                    </tables>
                                </fieldConfig>
                            </configInfo>
                            <dataSource xmlns=""COE.FormGroup""/>
                            <dataSourceId xmlns=""COE.FormGroup""/>
                            <requiredStyle xmlns=""COE.FormGroup""/>
                            <displayData xmlns=""COE.FormGroup""/>";
                        viewModeF44012.AppendChild(identifiersFormElement);
                        messages.Add("Identifiers form element was successfully added in form 4 of coeform 4012 in viewMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was already present in form 4 of coeform 4012 in viewMode");
                    }
                }
                #endregion
                #region Form 4013
                else if (id == "4013")
                {
                    XmlNode viewModeF24013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode", manager);
                    if (viewModeF24013.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager) == null)
                    {
                        XmlNode identifiersFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "Identifiers";
                        identifiersFormElement.Attributes.Append(name);
                        identifiersFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
							<showHelp xmlns=""COE.FormGroup"">false</showHelp>
							<helpText xmlns=""COE.FormGroup""/>
							<defaultValue xmlns=""COE.FormGroup""/>
							<bindingExpression xmlns=""COE.FormGroup"">this.Duplicates.Current.IdentifierList</bindingExpression>
							<Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
							<displayInfo xmlns=""COE.FormGroup"">
								<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
								<visible xmlns=""COE.FormGroup"">true</visible>
							</displayInfo>
							<validationRuleList xmlns=""COE.FormGroup""/>
							<serverEvents xmlns=""COE.FormGroup""/>
							<clientEvents xmlns=""COE.FormGroup""/>
							<configInfo xmlns=""COE.FormGroup"">
								<HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
								<HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
								<AddButtonCSS>AddButtonCSS</AddButtonCSS>
								<RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
								<RowStyleCSS>RowStyleCSS</RowStyleCSS>
								<fieldConfig>
									<CSSLabelClass>FERegistryLabelVM</CSSLabelClass>
									<AddRowTitle>Add Identifier</AddRowTitle>
									<ReadOnly>true</ReadOnly>
									<DefaultEmptyRows>0</DefaultEmptyRows>
									<NoDataMessage>No Identifiers associated</NoDataMessage>
									<tables>
										<table>
											<Columns>
												<Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0""/>
												<!-- Identifier type selection drop down column -->
												<Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""IdentifiersCslaDataSource"">
													<formElement xmlns=""COE.FormGroup"">
														<Id xmlns=""COE.FormGroup"">Batch_IdentifiersDropDownList</Id>
														<label xmlns=""COE.FormGroup"">Type</label>
														<bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
														<configInfo xmlns=""COE.FormGroup"">
															<fieldConfig>
																<CSSClass>FEDropDownListGrid</CSSClass>
																<CSSLabelClass>COERequiredField</CSSLabelClass>
																<DataSourceID>IdentifiersCslaDataSource</DataSourceID>
																<DataTextField>Name</DataTextField>
																<DataValueField>IdentifierID</DataValueField>
																<ID>Inner_Batch_IdentifiersDropDownList</ID>
																<Columns>
																	<Column key=""IdentifierID"" title=""Identifier ID"" visible=""false""/>
																	<Column key=""Name"" title=""Name""/>
																	<Column key=""Description"" title=""Description""/>
																</Columns>
															</fieldConfig>
														</configInfo>
														<displayInfo xmlns=""COE.FormGroup"">
															<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
														</displayInfo>
													</formElement>
												</Column>
												<!-- Identifier Name -->
												<Column name=""Value"">
													<formElement xmlns=""COE.FormGroup"">
														<Id xmlns=""COE.FormGroup"">Batch_IdentifiersNameTextEdit</Id>
														<bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
														<configInfo xmlns=""COE.FormGroup"">
															<fieldConfig/>
															<MaxLength>50</MaxLength>
														</configInfo>
														<displayInfo xmlns=""COE.FormGroup"">
															<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
														</displayInfo>
													</formElement>
												</Column>
												<Column name=""Name""/>
											</Columns>
										</table>
									</tables>
								</fieldConfig>
							</configInfo>
							<dataSource xmlns=""COE.FormGroup""/>
							<dataSourceId xmlns=""COE.FormGroup""/>
							<displayData xmlns=""COE.FormGroup""/>";
                        viewModeF24013.AppendChild(identifiersFormElement);
                        messages.Add("Identifiers form element was successfully added in form 2 of coeform 4013 in viewMode");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Identifiers form element was already present in form 2 of coeform 4013 in viewMode");

                    }
                }
                #endregion
            }
            if (!errorsInPatch)
            {
                messages.Add("AddBatchProjectsAndIdentifiersGrids was successfully patched");
            }
            else
                messages.Add("AddBatchProjectsAndIdentifiersGrids was patched with errors");
            return messages;
        }
    }
}
