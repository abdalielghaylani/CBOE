using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Migrate COEFormGroup 4014 (Registry Duplicates)
    /// 
    /// Changes in 4014 form can be divided into two parts:
    /// a. Fix for CSBR-129258
    /// b. Others
    /// Fix for CSBR-129258 have been patched in CSBR129258.cs, this class only patch those changes other than CSBR-129258. 
    /// </summary>
	public class MigCOEFormGroup4014 : BugFixBaseCommand
	{
        /// <summary>
        /// 1. Change label of Identifiers formElement in "Duplicate Registry Information" coeForm (id=0)
        /// 2. Change a coeForm (id=1, originally used for Component Information) to be used as Structure Information
        ///    a. Change title from "Component Information" to "Structure Information"
        ///    b. Replace Identifiers formElement so that it can be used as Structure Identifier
        /// 3. Change a coeForm (id=1001, originally used for Component custom properties) to include component information removed from coeForm(id=1)
        ///    a. Change tilte from "Component custom properties" to "Component Information"
        ///    b. add a Identifiers formElement
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
                if (id == "4014")
                {
                    string originalForm_OuterXml = doc.OuterXml;//save original content
                    try
                    {

                        XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                        string xmlns = "COE.FormGroup";
                        string prefix = "COE";
                        manager.AddNamespace(prefix, xmlns);

                        XmlNode detailsForms = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms", manager);

                        //change 1
                        XmlNode identifiers_Label=detailsForms.SelectSingleNode("COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']/COE:label",manager);
                        if (identifiers_Label != null)
                        {
                            identifiers_Label.InnerText = "Registry Identifiers";
                            messages.Add("Change label of Identifiers formElement in \"Duplicate Registry Information\" coeForm (id=0)");
                        }

                        //change 2
                        XmlNode old_ComponentInformation_COEForm = detailsForms.SelectSingleNode("COE:coeForm[@id='1']", manager);
                        if (old_ComponentInformation_COEForm != null)
                        {
                            XmlNode old_ComponentInformation_COEForm_FormTitle = old_ComponentInformation_COEForm.SelectSingleNode("COE:title", manager);
                            if (old_ComponentInformation_COEForm_FormTitle != null)
                            {
                                old_ComponentInformation_COEForm_FormTitle.InnerText = "Structure Information";
                                messages.Add("Change title of original Component Information form");
                            }

                            XmlNode identifiers_FormElement = old_ComponentInformation_COEForm.SelectSingleNode("COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                            if (identifiers_FormElement != null)
                            {
                                identifiers_FormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Structure Identifiers</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.IdentifierList</bindingExpression>
              <Id xmlns=""COE.FormGroup"">Structure_IdentifiersUltraGrid</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <HeaderStyleCSS xmlns=""COE.FormGroup"">HeaderStyleCSS</HeaderStyleCSS>
                <HeaderHorizontalAlign xmlns=""COE.FormGroup"">Center</HeaderHorizontalAlign>
                <AddButtonCSS xmlns=""COE.FormGroup"">AddButtonCSS</AddButtonCSS>
                <RemoveButtonCSS xmlns=""COE.FormGroup"">RemoveButtonCSS</RemoveButtonCSS>
                <RowAlternateStyleCSS xmlns=""COE.FormGroup"">RowAlternateStyleCSS</RowAlternateStyleCSS>
                <RowStyleCSS xmlns=""COE.FormGroup"">RowStyleCSS</RowStyleCSS>
                <SelectedRowStyleCSS xmlns=""COE.FormGroup"">RowSelectedStyleCSS</SelectedRowStyleCSS>
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <AddRowTitle xmlns=""COE.FormGroup"">Add Identifier</AddRowTitle>
                  <RemoveRowTitle xmlns=""COE.FormGroup"">Remove Identifier</RemoveRowTitle>
                  <ReadOnly xmlns=""COE.FormGroup"">true</ReadOnly>
                  <NoDataMessage xmlns=""COE.FormGroup"">No Identifiers associated</NoDataMessage>
                  <DefaultEmptyRows xmlns=""COE.FormGroup"">0</DefaultEmptyRows>
                  <tables xmlns=""COE.FormGroup"">
                    <table xmlns=""COE.FormGroup"">
                      <Columns xmlns=""COE.FormGroup"">
                        <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" xmlns=""COE.FormGroup""/>
                        <!-- Identifier type selection drop down column -->
                        <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""StructureIdentifiersCslaDataSource"" xmlns=""COE.FormGroup"">
                          <!--dataSourceID=""IdentifiersCslaDataSource"">-->
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">LStructure_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">StructureIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_LStructure_IdentifiersDropDownList</ID>
                                <Columns xmlns=""COE.FormGroup"">
                                  <Column key=""IdentifierID"" title=""Identifier ID"" visible=""false"" xmlns=""COE.FormGroup""/>
                                  <Column key=""Name"" title=""Name"" xmlns=""COE.FormGroup""/>
                                  <Column key=""Description"" title=""Description"" xmlns=""COE.FormGroup""/>
                                </Columns>
                              </fieldConfig>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                            </displayInfo>
                          </formElement>
                        </Column>
                        <!-- Identifier Name -->
                        <Column name=""Value"" xmlns=""COE.FormGroup"">
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">LStructure_IdentifiersNameTextEdit</Id>
                            <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup""/>
                              <MaxLength xmlns=""COE.FormGroup"">50</MaxLength>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                            </displayInfo>
                          </formElement>
                        </Column>
                        <Column name=""Name"" xmlns=""COE.FormGroup""/>
                      </Columns>
                    </table>
                  </tables>
                  <ClientSideEvents xmlns=""COE.FormGroup"">
                  </ClientSideEvents>
                  <DefaultRows xmlns=""COE.FormGroup"">
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""5"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                  </DefaultRows>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>";
                                messages.Add("Add Structure Identifier");
                            }
                        }

                        //change 3
                        XmlNode old_ComponentCustomProperties_COEForm = detailsForms.SelectSingleNode("COE:coeForm[@id='1001']", manager);
                        if (old_ComponentInformation_COEForm != null)
                        {
                            XmlNode old_ComponentCustomProperties_COEForm_FormTitle = old_ComponentCustomProperties_COEForm.SelectSingleNode("COE:title", manager);
                            if (old_ComponentCustomProperties_COEForm_FormTitle != null)
                                old_ComponentCustomProperties_COEForm_FormTitle.InnerText = "Component Information";

                            XmlNode old_ComponentCustomProperties_COEForm_ViewMode = old_ComponentCustomProperties_COEForm.SelectSingleNode("COE:viewMode",manager);
                            if (old_ComponentCustomProperties_COEForm_ViewMode != null)
                            {
                                XmlNode componentCustomProperties_Identifiers = old_ComponentCustomProperties_COEForm_ViewMode.SelectSingleNode("COE:formElement[@name='Identifiers']", manager);
                                if (componentCustomProperties_Identifiers == null)
                                {
                                    componentCustomProperties_Identifiers = doc.CreateElement("formElement", xmlns);
                                    XmlAttribute identifiers_Name = doc.CreateAttribute("name");
                                    identifiers_Name.InnerText = "Identifiers";
                                    componentCustomProperties_Identifiers.Attributes.Append(identifiers_Name);
                                    old_ComponentCustomProperties_COEForm_ViewMode.InsertBefore(componentCustomProperties_Identifiers, old_ComponentCustomProperties_COEForm_ViewMode.FirstChild);
                                    componentCustomProperties_Identifiers.InnerXml = @"<label xmlns=""COE.FormGroup"">Component Identifiers</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.IdentifierList</bindingExpression>
              <Id xmlns=""COE.FormGroup"">Compound_IdentifiersUltraGrid</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <style xmlns=""COE.FormGroup"">width:40%</style>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <HeaderStyleCSS xmlns=""COE.FormGroup"">HeaderStyleCSS</HeaderStyleCSS>
                <HeaderHorizontalAlign xmlns=""COE.FormGroup"">Center</HeaderHorizontalAlign>
                <AddButtonCSS xmlns=""COE.FormGroup"">AddButtonCSS</AddButtonCSS>
                <RemoveButtonCSS xmlns=""COE.FormGroup"">RemoveButtonCSS</RemoveButtonCSS>
                <RowAlternateStyleCSS xmlns=""COE.FormGroup"">RowAlternateStyleCSS</RowAlternateStyleCSS>
                <RowStyleCSS xmlns=""COE.FormGroup"">RowStyleCSS</RowStyleCSS>
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <AddRowTitle xmlns=""COE.FormGroup"">Add Identifier</AddRowTitle>
                  <RemoveRowTitle xmlns=""COE.FormGroup"">Remove Identifier</RemoveRowTitle>
                  <ReadOnly xmlns=""COE.FormGroup"">true</ReadOnly>
                  <DefaultEmptyRows xmlns=""COE.FormGroup"">1</DefaultEmptyRows>
                  <NoDataMessage xmlns=""COE.FormGroup"">No Identifiers associated</NoDataMessage>
                  <tables xmlns=""COE.FormGroup"">
                    <table xmlns=""COE.FormGroup"">
                      <Columns xmlns=""COE.FormGroup"">
                        <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" xmlns=""COE.FormGroup""/>
                        <!-- Identifier type selection drop down column -->
                        <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""CompoundIdentifiersCslaDataSource"" xmlns=""COE.FormGroup"">
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">VCompound_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">CompoundIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_VCompound_IdentifiersDropDownList</ID>
                                <Columns xmlns=""COE.FormGroup"">
                                  <Column key=""IdentifierID"" title=""Identifier ID"" visible=""false"" xmlns=""COE.FormGroup""/>
                                  <Column key=""Name"" title=""Name"" xmlns=""COE.FormGroup""/>
                                  <Column key=""Description"" title=""Description"" xmlns=""COE.FormGroup""/>
                                </Columns>
                              </fieldConfig>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                            </displayInfo>
                          </formElement>
                        </Column>
                        <!-- Identifier Name -->
                        <Column name=""Value"" xmlns=""COE.FormGroup"">
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">VCompound_IdentifiersNameTextEdit</Id>
                            <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup""/>
                              <MaxLength xmlns=""COE.FormGroup"">50</MaxLength>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                            </displayInfo>
                          </formElement>
                        </Column>
                        <Column name=""Name"" xmlns=""COE.FormGroup""/>
                      </Columns>
                    </table>
                  </tables>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>";
                                    messages.Add("Move Component Identifiers from form 1 to 1001");
                                }
                            }
                        }


                        
                    }
                    catch (Exception e)
                    {
                        errorsInPatch = true;
                        messages.Add(string.Format("Exception occurs during patching: {0}", e.Message));
                    }

                    if (errorsInPatch)//restore form
                    {
                        doc.LoadXml(originalForm_OuterXml);
                    }
                }
            }

            if (errorsInPatch)
                messages.Add("Fail to migrate COEFormGroup 4014");
            else
                messages.Add("Succeed to migrate COEFormGroup 4014");

            return messages;
        }
	}
}
