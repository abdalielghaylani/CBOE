using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 132365: Unable to add batch project after getting validation message in the submit registry form.
    /// </summary>
    class CSBR132365 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                XmlNode coeForm = null;
                if (id == "4010")
                {
                    coeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']", manager);

                    if (coeForm != null)
                    {
                        XmlNode editMode = coeForm.SelectSingleNode("//COE:coeForm[@id='4']/COE:editMode", manager);

                        XmlNode batchProjectElement = editMode.SelectSingleNode("./COE:formElement[@name='Projects']", manager);

                        if (batchProjectElement == null)
                        {
                            batchProjectElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                            XmlAttribute nameP = doc.CreateAttribute("name");
                            nameP.Value = "Projects";
                            batchProjectElement.Attributes.Append(nameP);
                            batchProjectElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Projects</label>
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
																<ID>Inner_Batch_ProjectDropDownList</ID>
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

                            editMode.AppendChild(batchProjectElement);
                            messages.Add("Successfully added Projects form Element at batch level in edit mode in form id: " + id);
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects item was already present in form id: " + id);
                        }

                        XmlNode batchIdentifierElement = editMode.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager);

                        if (batchIdentifierElement == null)
                        {
                            batchIdentifierElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "Identifiers";
                            batchIdentifierElement.Attributes.Append(name);
                            batchIdentifierElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
							<showHelp xmlns=""COE.FormGroup"">false</showHelp>
							<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
							<pageComunicationProvider xmlns=""COE.FormGroup""/>
							<fileUploadBindingExpression xmlns=""COE.FormGroup""/>
							<helpText xmlns=""COE.FormGroup""/>
							<defaultValue xmlns=""COE.FormGroup""/>
							<bindingExpression xmlns=""COE.FormGroup"">IdentifierList</bindingExpression>
							<Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
							<displayInfo xmlns=""COE.FormGroup"">
								<style xmlns=""COE.FormGroup"">width:40%</style>
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
									<AddRowTitle>Add Identifier</AddRowTitle>
									<RemoveRowTitle>Remove Identifier</RemoveRowTitle>
									<ReadOnly>false</ReadOnly>
									<DefaultEmptyRows>1</DefaultEmptyRows>
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
																<CSSLabelClass>COELabel</CSSLabelClass>
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
														<Id xmlns=""COE.FormGroup"">Mix_IdentifiersNameTextEdit</Id>
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
									<ClientSideEvents>
										<Event name=""AfterExitEdit"">
                          AfterExitEdit();
                        </Event>
									</ClientSideEvents>
									<DefaultRows>
										<Row>
											<cell bindingExpression=""IdentifierID"" dataValue=""4"" dataText=""Custom_FindFromEditor""/>
										</Row>
									</DefaultRows>
								</fieldConfig>
							</configInfo>
							<dataSource xmlns=""COE.FormGroup""/>
							<dataSourceId xmlns=""COE.FormGroup""/>
							<requiredStyle xmlns=""COE.FormGroup""/>
							<displayData xmlns=""COE.FormGroup""/>";

                            editMode.AppendChild(batchIdentifierElement);
                            messages.Add("Successfully added Identifiers form Element at batch level in edit mode in form id: " + id);
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers item was already present in form id: " + id);
                        }


                        XmlNode viewMode = coeForm.SelectSingleNode("//COE:coeForm[@id='4']/COE:viewMode", manager);

                        XmlNode batchProjectElementVM = viewMode.SelectSingleNode("./COE:formElement[@name='Projects']", manager);

                        if (batchProjectElementVM == null)
                        {
                            batchProjectElementVM = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                            XmlAttribute nameP = doc.CreateAttribute("name");
                            nameP.Value = "Projects";
                            batchProjectElementVM.Attributes.Append(nameP);
                            batchProjectElementVM.InnerXml = @"<label xmlns=""COE.FormGroup"">Projects</label>
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
																<ID>Inner_Batch_ProjectDropDownList</ID>
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

                            viewMode.AppendChild(batchProjectElementVM);
                            messages.Add("Successfully added Projects form Element at batch level in view mode in form id: " + id);
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects item was already present in form id: " + id);
                        }

                        XmlNode batchIdentifierElementVM = viewMode.SelectSingleNode("./COE:formElement[@name='Identifiers']", manager);

                        if (batchIdentifierElementVM == null)
                        {
                            batchIdentifierElementVM = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "Identifiers";
                            batchIdentifierElementVM.Attributes.Append(name);
                            batchIdentifierElementVM.InnerXml = @"<label xmlns=""COE.FormGroup"">Identifiers</label>
							<showHelp xmlns=""COE.FormGroup"">false</showHelp>
							<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
							<pageComunicationProvider xmlns=""COE.FormGroup""/>
							<fileUploadBindingExpression xmlns=""COE.FormGroup""/>
							<helpText xmlns=""COE.FormGroup""/>
							<defaultValue xmlns=""COE.FormGroup""/>
							<bindingExpression xmlns=""COE.FormGroup"">IdentifierList</bindingExpression>
							<Id xmlns=""COE.FormGroup"">Batch_IdentifiersUltraGrid</Id>
							<displayInfo xmlns=""COE.FormGroup"">
								<style xmlns=""COE.FormGroup"">width:40%</style>
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
									<AddRowTitle>Add Identifier</AddRowTitle>
									<RemoveRowTitle>Remove Identifier</RemoveRowTitle>
									<ReadOnly>true</ReadOnly>
									<DefaultEmptyRows>1</DefaultEmptyRows>
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
																<CSSLabelClass>COELabel</CSSLabelClass>
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
														<Id xmlns=""COE.FormGroup"">Mix_IdentifiersNameTextEdit</Id>
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
									<ClientSideEvents>
										<Event name=""AfterExitEdit"">
                          AfterExitEdit();
                        </Event>
									</ClientSideEvents>
									<DefaultRows>
										<Row>
											<cell bindingExpression=""IdentifierID"" dataValue=""4"" dataText=""Custom_FindFromEditor""/>
										</Row>
									</DefaultRows>
								</fieldConfig>
							</configInfo>
							<dataSource xmlns=""COE.FormGroup""/>
							<dataSourceId xmlns=""COE.FormGroup""/>
							<requiredStyle xmlns=""COE.FormGroup""/>
							<displayData xmlns=""COE.FormGroup""/>";

                            viewMode.AppendChild(batchIdentifierElementVM);
                            messages.Add("Successfully added Identifiers form Element at batch level in view mode in form id: " + id);
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers item was already present in form id: " + id);
                        }
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR132365 was successfully patched");
            }
            else
                messages.Add("CSBR132365 was patched with errors");

            return messages;
        }
    }
}

