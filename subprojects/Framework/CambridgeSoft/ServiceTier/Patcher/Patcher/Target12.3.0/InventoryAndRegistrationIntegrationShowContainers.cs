using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// In Chembioviz list form a new icon should be shown right before the batch number to create a container.
    /// In Registration, while vieweing a permanent record, a new grid is to be shown with the container list.
    /// </summary>
    public class InventoryAndRegistrationIntegrationShowContainers : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            string coeFormInnerXml = @"<validationRuleList xmlns=""COE.FormGroup"" />
			<title xmlns=""COE.FormGroup"">Inventory Containers</title>
			<titleCssClass xmlns=""COE.FormGroup"">COEFormTitle</titleCssClass>
			<layoutInfo xmlns=""COE.FormGroup"" />
			<formDisplay xmlns=""COE.FormGroup"">
				<cssClass xmlns=""COE.FormGroup"">COEFormDisplay</cssClass>
				<layoutStyle xmlns=""COE.FormGroup"">flowLayout</layoutStyle>
				<visible xmlns=""COE.FormGroup"">true</visible>
			</formDisplay>
			<addMode xmlns=""COE.FormGroup"" />
			<editMode xmlns=""COE.FormGroup"" />
			<viewMode xmlns=""COE.FormGroup"">
				<formElement name=""ContainersGrid"" xmlns=""COE.FormGroup"">
					<showHelp xmlns=""COE.FormGroup"">false</showHelp>
					<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
					<pageComunicationProvider xmlns=""COE.FormGroup"" />
					<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
					<helpText xmlns=""COE.FormGroup"" />
					<defaultValue xmlns=""COE.FormGroup"" />
					<bindingExpression xmlns=""COE.FormGroup"">this</bindingExpression>
					<Id xmlns=""COE.FormGroup"">VContainersUltraGrid</Id>
					<displayInfo xmlns=""COE.FormGroup"">
						<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                        <style xmlns=""COE.FormGroup"">width:700px</style>
						<visible xmlns=""COE.FormGroup"">true</visible>
					</displayInfo>
					<validationRuleList xmlns=""COE.FormGroup"" />
					<serverEvents xmlns=""COE.FormGroup"" />
					<clientEvents xmlns=""COE.FormGroup"" />
					<configInfo xmlns=""COE.FormGroup"">
						<HeaderStyleCSS xmlns=""COE.FormGroup"">HeaderStyleCSS</HeaderStyleCSS>
						<HeaderHorizontalAlign xmlns=""COE.FormGroup"">Center</HeaderHorizontalAlign>
						<AddButtonCSS xmlns=""COE.FormGroup"">AddButtonCSS</AddButtonCSS>
						<RemoveButtonCSS xmlns=""COE.FormGroup"">RemoveButtonCSS</RemoveButtonCSS>
						<RowAlternateStyleCSS xmlns=""COE.FormGroup"">RowAlternateStyleCSS</RowAlternateStyleCSS>
						<RowStyleCSS xmlns=""COE.FormGroup"">RowStyleCSS</RowStyleCSS>
						<EnableClientSideNumbering xmlns=""COE.FormGroup"">false</EnableClientSideNumbering>
						<Id xmlns=""COE.FormGroup"">VContainersUltraGrid</Id>
						<fieldConfig xmlns=""COE.FormGroup"">
							<AddRowTitle xmlns=""COE.FormGroup"">New Container</AddRowTitle>
							<ReadOnly xmlns=""COE.FormGroup"">true</ReadOnly>
							<NoDataMessage xmlns=""COE.FormGroup"">No container associated</NoDataMessage>
							<tables xmlns=""COE.FormGroup"">
								<table xmlns=""COE.FormGroup"">
									<Columns xmlns=""COE.FormGroup"">
										<Column name=""Barcode"" headerText=""ID"" width=""70px"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_Barcode</Id>
												<bindingExpression xmlns=""COE.FormGroup"">Barcode</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""QtyAvailable"" headerText=""Qty Available"" width=""70px"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_QtyAvailable</Id>
												<bindingExpression xmlns=""COE.FormGroup"">QtyAvailable</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""ContainerSize"" headerText=""Container Size"" width=""70px"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_ContainerSize</Id>
												<bindingExpression xmlns=""COE.FormGroup"">ContainerSize</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""Location"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_Location</Id>
												<bindingExpression xmlns=""COE.FormGroup"">Location</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""ContainerType"" headerText=""Container Type"" width=""70px"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_ContainerType</Id>
												<bindingExpression xmlns=""COE.FormGroup"">ContainerType</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""RegBatchID"" headerText=""Reg Number"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_RegNumber</Id>
												<bindingExpression xmlns=""COE.FormGroup"">RegBatchID</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""RequestFromBatchURL"" headerText=""Request from Batch"" formatText=""&lt;a href='{0}' class='LinkButton' titleText='Get batch sample' onclick='ShowModalFrame(this.href, this.titleText, true);return false;'&gt;&lt;div style=&quot;background: url('/COECommonResources/ChemInv/flask_open_icon_16.gif') no-repeat center;height:17px&quot;&gt;&amp;nbsp;&lt;/div&gt;&lt;/a&gt;"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_RequestFromBatchURL</Id>
                                                <bindingExpression xmlns=""COE.FormGroup"">RequestFromBatchURL</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
										<Column name=""RequestFromContainerURL"" headerText=""Request from Container"" formatText=""&lt;a href='{0}' class='LinkButton' titleText='Get container sample' onclick='ShowModalFrame(this.href, this.titleText, true);return false;'&gt;&lt;div style=&quot;background: url('/COECommonResources/ChemInv/flask_open_icon_16.gif') no-repeat center;height:17px&quot;&gt;&amp;nbsp;&lt;/div&gt;&lt;/a&gt;"" xmlns=""COE.FormGroup"">
											<formElement xmlns=""COE.FormGroup"">
												<Id xmlns=""COE.FormGroup"">VContainersUltraGrid_RequestFromContainerURL</Id>
                                                <bindingExpression xmlns=""COE.FormGroup"">RequestFromContainerURL</bindingExpression>
												<configInfo xmlns=""COE.FormGroup"">
													<fieldConfig />
												</configInfo>
												<displayInfo xmlns=""COE.FormGroup"">
													<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
												</displayInfo>
											</formElement>
										</Column>
									</Columns>
								</table>
							</tables>
							<ClientSideEvents xmlns=""COE.FormGroup"" />
						</fieldConfig>
					</configInfo>
					<dataSource xmlns=""COE.FormGroup"" />
					<dataSourceId xmlns=""COE.FormGroup"" />
					<requiredStyle xmlns=""COE.FormGroup"" />
					<displayData xmlns=""COE.FormGroup"" />
				</formElement>
				<formElement name=""ContainersCountTitle"" xmlns=""COE.FormGroup"">
					<showHelp xmlns=""COE.FormGroup"">false</showHelp>
					<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
					<pageComunicationProvider xmlns=""COE.FormGroup"" />
					<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
					<helpText xmlns=""COE.FormGroup"" />
					<defaultValue xmlns=""COE.FormGroup"">Number of Containers:</defaultValue>
					<bindingExpression xmlns=""COE.FormGroup""></bindingExpression>
					<Id xmlns=""COE.FormGroup"">ContainersCountTitle</Id>
					<displayInfo xmlns=""COE.FormGroup"">
						<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel</type>
						<style xmlns=""COE.FormGroup"">margin-top:10px;margin-left:50px;</style>
						<visible xmlns=""COE.FormGroup"">true</visible>
					</displayInfo>
					<validationRuleList xmlns=""COE.FormGroup"" />
					<serverEvents xmlns=""COE.FormGroup"" />
					<clientEvents xmlns=""COE.FormGroup"" />
					<configInfo xmlns=""COE.FormGroup"">
						<fieldConfig xmlns=""COE.FormGroup"">
							<CSSClass xmlns=""COE.FormGroup"">FELabel</CSSClass>
						</fieldConfig>
					</configInfo>
					<dataSource xmlns=""COE.FormGroup"" />
					<dataSourceId xmlns=""COE.FormGroup"" />
					<requiredStyle xmlns=""COE.FormGroup"" />
					<displayData xmlns=""COE.FormGroup"" />
				</formElement>
				<formElement name=""ContainersCount"" xmlns=""COE.FormGroup"">
					<showHelp xmlns=""COE.FormGroup"">false</showHelp>
					<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
					<pageComunicationProvider xmlns=""COE.FormGroup"" />
					<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
					<helpText xmlns=""COE.FormGroup"" />
					<defaultValue xmlns=""COE.FormGroup"" />
					<bindingExpression xmlns=""COE.FormGroup"">this.Count</bindingExpression>
					<Id xmlns=""COE.FormGroup"">ContainersCount</Id>
					<displayInfo xmlns=""COE.FormGroup"">
						<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel</type>
						<style xmlns=""COE.FormGroup"">margin-top:10px;margin-left:10px;</style>
						<visible xmlns=""COE.FormGroup"">true</visible>
					</displayInfo>
					<validationRuleList xmlns=""COE.FormGroup"" />
					<serverEvents xmlns=""COE.FormGroup"" />
					<clientEvents xmlns=""COE.FormGroup"" />
					<configInfo xmlns=""COE.FormGroup"">
						<fieldConfig xmlns=""COE.FormGroup"">
							<CSSClass xmlns=""COE.FormGroup"">FELabel</CSSClass>
						</fieldConfig>
					</configInfo>
					<dataSource xmlns=""COE.FormGroup"" />
					<dataSourceId xmlns=""COE.FormGroup"" />
					<requiredStyle xmlns=""COE.FormGroup"" />
					<displayData xmlns=""COE.FormGroup"" />
				</formElement>
				<formElement name=""TotalQtyAvailableTitle"" xmlns=""COE.FormGroup"">
					<showHelp xmlns=""COE.FormGroup"">false</showHelp>
					<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
					<pageComunicationProvider xmlns=""COE.FormGroup"" />
					<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
					<helpText xmlns=""COE.FormGroup"" />
					<defaultValue xmlns=""COE.FormGroup"">Total Quantity Available:</defaultValue>
					<bindingExpression xmlns=""COE.FormGroup""></bindingExpression>
					<Id xmlns=""COE.FormGroup"">TotalQtyAvailableTitle</Id>
					<displayInfo xmlns=""COE.FormGroup"">
						<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel</type>
						<style xmlns=""COE.FormGroup"">margin-top:10px;margin-left:50px;</style>
						<visible xmlns=""COE.FormGroup"">true</visible>
					</displayInfo>
					<validationRuleList xmlns=""COE.FormGroup"" />
					<serverEvents xmlns=""COE.FormGroup"" />
					<clientEvents xmlns=""COE.FormGroup"" />
					<configInfo xmlns=""COE.FormGroup"">
						<fieldConfig xmlns=""COE.FormGroup"">
							<CSSClass xmlns=""COE.FormGroup"">FELabel</CSSClass>
						</fieldConfig>
					</configInfo>
					<dataSource xmlns=""COE.FormGroup"" />
					<dataSourceId xmlns=""COE.FormGroup"" />
					<requiredStyle xmlns=""COE.FormGroup"" />
					<displayData xmlns=""COE.FormGroup"" />
				</formElement>
				<formElement name=""TotalQtyAvailable"" xmlns=""COE.FormGroup"">
					<showHelp xmlns=""COE.FormGroup"">false</showHelp>
					<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
					<pageComunicationProvider xmlns=""COE.FormGroup"" />
					<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
					<helpText xmlns=""COE.FormGroup"" />
					<defaultValue xmlns=""COE.FormGroup"" />
					<bindingExpression xmlns=""COE.FormGroup"">this.TotalQtyAvailable</bindingExpression>
					<Id xmlns=""COE.FormGroup"">TotalQtyAvailable</Id>
					<displayInfo xmlns=""COE.FormGroup"">
						<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel</type>
						<style xmlns=""COE.FormGroup"">margin-top:10px;margin-left:10px;</style>
						<visible xmlns=""COE.FormGroup"">true</visible>
					</displayInfo>
					<validationRuleList xmlns=""COE.FormGroup"" />
					<serverEvents xmlns=""COE.FormGroup"" />
					<clientEvents xmlns=""COE.FormGroup"" />
					<configInfo xmlns=""COE.FormGroup"">
						<fieldConfig xmlns=""COE.FormGroup"">
							<CSSClass xmlns=""COE.FormGroup"">FELabel</CSSClass>
						</fieldConfig>
					</configInfo>
					<dataSource xmlns=""COE.FormGroup"" />
					<dataSourceId xmlns=""COE.FormGroup"" />
					<requiredStyle xmlns=""COE.FormGroup"" />
					<displayData xmlns=""COE.FormGroup"" />
				</formElement>
				<formElement name=""LastContainerRequestURL"" xmlns=""COE.FormGroup"">
					<showHelp xmlns=""COE.FormGroup"">false</showHelp>
					<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
					<pageComunicationProvider xmlns=""COE.FormGroup"" />
					<fileUploadBindingExpression  xmlns=""COE.FormGroup""/>
					<helpText xmlns=""COE.FormGroup"" />
					<defaultValue xmlns=""COE.FormGroup"" />
					<bindingExpression xmlns=""COE.FormGroup"">this.LastContainerRequestURL</bindingExpression>
					<Id xmlns=""COE.FormGroup"">ReqMaterial</Id>
					<displayInfo xmlns=""COE.FormGroup"">
						<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink</type>
						<style xmlns=""COE.FormGroup"">margin-top:10px;margin-left:50px;</style>
						<visible xmlns=""COE.FormGroup"">true</visible>
					</displayInfo>
					<validationRuleList xmlns=""COE.FormGroup"" />
					<serverEvents xmlns=""COE.FormGroup"" />
					<clientEvents xmlns=""COE.FormGroup"" />
					<configInfo xmlns=""COE.FormGroup"">
						<fieldConfig xmlns=""COE.FormGroup"">
							<CSSClass xmlns=""COE.FormGroup"">LinkButton</CSSClass>
							<Text xmlns=""COE.FormGroup"">&lt;img src=&quot;/COECommonResources/ChemInv/flask_open_icon_16.gif&quot; alt=&quot;Request Material&quot; style=&quot;width:17px;height:17px&quot;&gt;Request Material&lt;/div&gt;</Text>
							<HRef xmlns=""COE.FormGroup"">{0}&amp;RequestType=R</HRef>
							<ClientClick xmlns=""COE.FormGroup"">ShowModalFrame(this.href, 'Request sample', true);return false;</ClientClick>
						</fieldConfig>
					</configInfo>
					<dataSource xmlns=""COE.FormGroup"" />
					<dataSourceId xmlns=""COE.FormGroup"" />
					<requiredStyle xmlns=""COE.FormGroup"" />
					<displayData xmlns=""COE.FormGroup"" />
				</formElement>
			</viewMode>
			<clientScripts xmlns=""COE.FormGroup"" />";

            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4012")
                {
                    XmlNode coeForms = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms", manager);
                    XmlNode coeForm9 = coeForms.SelectSingleNode("./COE:coeForm[@id='9']", manager);
                    
                    if (coeForm9 == null)
                    {
                        //<coeForm id="9" dataSourceId="InvContainersDataSource">
                        coeForm9 = doc.CreateElement("COE:coeForm", "COE.FormGroup");
                        XmlAttribute idAttr = doc.CreateAttribute("id");
                        idAttr.Value = "9";
                        XmlAttribute datasourceidAttr = doc.CreateAttribute("dataSourceId");
                        datasourceidAttr.Value = "InvContainersDataSource";
                        coeForm9.Attributes.Append(idAttr);
                        coeForm9.Attributes.Append(datasourceidAttr);
                        coeForm9.InnerXml = coeFormInnerXml;

                        XmlNode coeForm8 = coeForms.SelectSingleNode("./COE:coeForm[@id='8']", manager);

                        if (coeForm8 == null)
                        {
                            errorsInPatch = true;
                            messages.Add("coeForm with id 8 was not found on view mixture xml, and thus from 9 was not added");
                        }
                        else
                        {
                            coeForms.InsertAfter(coeForm9, coeForm8);
                            messages.Add("Successfully added coeForm with id 9");
                        }
                    }
                    else
                    {
                        messages.Add("coeForm with id 9 is already present; Is the patcher already run?");
                    }
                }
                else if (id == "4003")
                {
                    XmlNode table9Columns = doc.SelectSingleNode("//COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo//COE:table[@name='Table_9']/COE:Columns", manager);

                    if (table9Columns == null)
                    {
                        errorsInPatch = true;
                        messages.Add("Table_9 was not found in the list of tables for permanent search.");
                    }
                    else
                    {
                        manager.AddNamespace("COE1", "COE.ResultsCriteria");

                        bool batchIdPresentInRC = false;
                        //<field visible="true" alias="BATCHID" orderById="0" direction="asc" fieldId="900"/>
                        XmlNode rcTable9 = doc.SelectSingleNode("//COE:listForm[@id='0']/COE1:resultsCriteria/COE1:tables/COE1:table[@id='9']", manager);

                        if (rcTable9 == null)
                        {
                            errorsInPatch = true;
                            messages.Add("Results Criteria do not hold table_id 9");
                        }
                        else
                        {
                            XmlNode batchIDRC = rcTable9.SelectSingleNode("./COE1:field[@alias='BATCHID']", manager);
                            if (batchIDRC != null)
                            {
                                batchIdPresentInRC = true;
                                messages.Add("BatchID already present in results criteria. Have you already run the patcher?");
                            }
                            else
                            {
                                batchIDRC = rcTable9.OwnerDocument.CreateElement("field", "COE.ResultsCriteria");
                                batchIDRC.Attributes.Append(doc.CreateAttribute("visible")).Value = "true";
                                batchIDRC.Attributes.Append(doc.CreateAttribute("alias")).Value = "BATCHID";
                                batchIDRC.Attributes.Append(doc.CreateAttribute("orderById")).Value = "0";
                                batchIDRC.Attributes.Append(doc.CreateAttribute("direction")).Value = "asc";
                                batchIDRC.Attributes.Append(doc.CreateAttribute("fieldId")).Value = "900";
                                rcTable9.InsertBefore(batchIDRC, rcTable9.FirstChild);

                                messages.Add("BatchID result criteria was successfully added");
                                batchIdPresentInRC = true;
                            }
                        }
                        
                        if(batchIdPresentInRC)
                        {
                            XmlNode batchIDColumn = table9Columns.SelectSingleNode("./COE:Column[@name='BATCHID']", manager);
                            if (batchIDColumn == null)
                            {
                                batchIDColumn = doc.CreateElement("COE:Column", "COE.FormGroup");
                                XmlAttribute nameAttr = doc.CreateAttribute("name");
                                nameAttr.Value = "BATCHID";
                                batchIDColumn.Attributes.Append(nameAttr);
                                batchIDColumn.InnerXml = @"<headerText xmlns=""COE.FormGroup""> </headerText>
                          <width xmlns=""COE.FormGroup"">17px</width>
                          <formElement name=""BATCHID"" xmlns=""COE.FormGroup"">
                            <displayInfo>
                              <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink</type>
                            </displayInfo>
							<Id>CreateContainerLink</Id>
                            <configInfo>
                              <fieldConfig>
                                <CSSClass>FELinkButton</CSSClass>
                                <ImageURL>/COECommonResources/ChemInv/flask_closed_icon_16.gif</ImageURL>
                                <HRef>/COERegistration/Forms/SendToInventory/ContentArea/SendToInventory.aspx?BatchID={0}</HRef>
								<ClientClick>ShowModalFrame(this.href, 'Create new container', true);return false;</ClientClick>
                              </fieldConfig>
                            </configInfo>
                          </formElement>";

                                XmlNode batchNumber = table9Columns.SelectSingleNode("./COE:Column[@name='BATCHNUMBER']", manager);

                                if (batchNumber == null)
                                {
                                    errorsInPatch = true;
                                    messages.Add("Batch number was not found on permanent search form, there is no suitable place to add the new container flask link.");
                                }
                                else
                                {
                                    table9Columns.InsertBefore(batchIDColumn, batchNumber);
                                    messages.Add("Successfully added the column BATCHID for creating new containers (flask icon link).");
                                }
                            }
                        }
                    }
                }
            }
            

            if (!errorsInPatch)
                messages.Add("Inventory/Registration Integration was successfully patched");
            else
                messages.Add("Inventory/Registration Integration was patched with errors");

            return messages;
        }
    }
}
