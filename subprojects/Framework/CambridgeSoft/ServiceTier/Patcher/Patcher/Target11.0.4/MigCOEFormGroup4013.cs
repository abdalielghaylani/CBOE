using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Migrate COEFormGroup 4013 ("Component Duplicates")
    /// </summary>
    public class MigCOEFormGroup4013 : BugFixBaseCommand
    {
        List<string> _messages = new List<string>();
        bool _errorsInPatch = false;

        /// <summary>
        /// 1. Edit label of registry level identifiers from "Identifiers" to "Registry Identifiers"
        /// 2. Add "Structure Information" section (a coeForm with id="1") after "Registry Custom Properties" section (a coeForm with id="1000"), as child of detailsForms
        ///    Some of its data come from "Component Information" section.
        /// 3. Remove content that has been moved to "Structure Information" section from "Component Information" section (a coeForm with id="0"),including (viewMode/):
        ///    a. Structure
        ///    b. Component ID
        ///    c. MF
        ///    d. MW
        /// 4. Edit Identifiers formElemet of "Component Information" section
        ///    a. Change label from Identifiers to "Component Identifiers"
        ///    b. set DefaultEmptyRows from 1 to 0
        /// 5. Place "Component Information" section after "Component custom properties" section (a coeForm with id="1001")
        /// 6. Assign "Fragment List" section a new id (from 1 to 3)
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4013")
                {
                    string originalForm_OuterXml = doc.OuterXml;//save original content
                    try
                    {
                        XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                        string xmlns = "COE.FormGroup";
                        string prefix = "COE";
                        manager.AddNamespace(prefix, xmlns);

                        Edit_RegistryIdentifiers_Label(doc, manager);
                        Add_StrInf_COEForm(doc, manager);
                        RemoveFrom_CmpInf_COEForm(doc, manager);
                        Edit_CmpInf_COEForm(doc, manager);
                        Move_CmpInf_COEForm(doc, manager);
                        Edit_FrgLst_COEForm(doc, manager);

                    }
                    catch (Exception e)
                    {
                        _errorsInPatch = true;
                        _messages.Add(string.Format("Exception occurs during patching: {0}", e.Message));
                    }

                    if (_errorsInPatch)//restore form
                    {
                        _messages.Add("Rollback changes to COEFormGroup 4013");
                        doc.LoadXml(originalForm_OuterXml);
                    }
                }
            }

            if (_errorsInPatch)
                _messages.Add("Fail to migrate COEFormGroup 4013");
            else
                _messages.Add("Succeed to migrate COEFormGroup 4013");

            return _messages;
        }

        /// <summary>
        /// 1. Edit lable of registry level identifiers from "Identifiers" to "Registry Identifiers"
        /// </summary>
        /// <param name="doc"></param>
        void Edit_RegistryIdentifiers_Label(XmlDocument doc, XmlNamespaceManager manager)
        {
            XmlNode registryIdentifiers_Label = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms/COE:coeForm[@id='9']/COE:viewMode/COE:formElement[@name='Identifiers']/COE:label", manager);
            registryIdentifiers_Label.InnerText = "Registry Identifiers";
            _messages.Add("Edit lable of registry level identifiers from \"Identifiers\" to \"Registry Identifiers\"");
        }

        /// <summary>
        /// 2. Add "Structure Information" section (a coeForm with id="1") after "Registry Custom Properties" section (a coeForm with id="1000"), as child of detailsForms
        ///    Some of its data come from "Component Information" section.
        /// </summary>
        /// <param name="doc"></param>
        void Add_StrInf_COEForm(XmlDocument doc, XmlNamespaceManager manager)
        {
            XmlNode detailsForms_COEForms = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms", manager);
            XmlNode registryCustomProperties_COEForm = detailsForms_COEForms.SelectSingleNode("./COE:coeForm[@id='1000']", manager);
            XmlNode strInf_COEForm = detailsForms_COEForms.SelectSingleNode("./COE:coeForm[COE:title='Structure Information']", manager);
            if (strInf_COEForm != null)
            {
                _messages.Add("Structure Information section already exists");
                return;
            }
            else
            {
                strInf_COEForm = doc.CreateElement("coeForm", "COE.FormGroup");
                XmlAttribute id_Attribute = doc.CreateAttribute("id");
                XmlAttribute dataSourceId_Attribute = doc.CreateAttribute("dataSourceId");
                id_Attribute.Value="1";
                dataSourceId_Attribute.Value="DuplicateCompoundsCslaDataSource";
                strInf_COEForm.Attributes.Append(id_Attribute);
                strInf_COEForm.Attributes.Append(dataSourceId_Attribute);
            }


            if (registryCustomProperties_COEForm != null)//insert after "Registry Custom Properties" section
            {
                detailsForms_COEForms.InsertAfter(strInf_COEForm, registryCustomProperties_COEForm);
            }
            else//insert as first child of detailsForms_COEForms
            {
                detailsForms_COEForms.InsertBefore(strInf_COEForm, detailsForms_COEForms.FirstChild);
            }

            strInf_COEForm.InnerXml = @"<validationRuleList xmlns=""COE.FormGroup""/>
          <title xmlns=""COE.FormGroup"">Structure Information</title>
          <titleCssClass xmlns=""COE.FormGroup"">COEFormTitle</titleCssClass>
          <layoutInfo xmlns=""COE.FormGroup"">
          </layoutInfo>
          <formDisplay xmlns=""COE.FormGroup"">
            <cssClass xmlns=""COE.FormGroup"">COEFormDisplay</cssClass>
            <layoutStyle xmlns=""COE.FormGroup"">flowLayout</layoutStyle>
            <visible xmlns=""COE.FormGroup"">true</visible>
          </formDisplay>
          <addMode xmlns=""COE.FormGroup"">
          </addMode>
          <editMode xmlns=""COE.FormGroup"">
          </editMode>
          <viewMode xmlns=""COE.FormGroup"">
            <formElement name=""Structure"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup""/>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup"">Structure</defaultValue>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">BaseFragmentStructureViewMode</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSClass xmlns=""COE.FormGroup"">FEStructureViewMode</CSSClass>
                  <Height xmlns=""COE.FormGroup"">200px</Height>
                  <Width xmlns=""COE.FormGroup"">200px</Width>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
            <formElement name=""Component ID"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">Component ID</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.RegNumber.RegNum</bindingExpression>
              <Id xmlns=""COE.FormGroup"">CIDTextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std20x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup"">
                <validationRule validationRuleName=""textLength"" errorMessage=""The length must be between 0 and 120"" displayPosition=""Top_Left"" xmlns=""COE.FormGroup"">
                  <params xmlns=""COE.FormGroup"">
                    <param name=""min"" value=""0"" xmlns=""COE.FormGroup""/>
                    <param name=""max"" value=""120"" xmlns=""COE.FormGroup""/>
                  </params>
                </validationRule>
              </validationRuleList>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <CSSClass xmlns=""COE.FormGroup"">FETextBoxViewMode</CSSClass>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
            <formElement name=""MF"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">MF</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.Formula</bindingExpression>
              <Id xmlns=""COE.FormGroup"">FormulaTextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std20x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <CSSClass xmlns=""COE.FormGroup"">FETextBoxViewMode</CSSClass>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
            <formElement name=""MW"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">MW</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.MolWeight</bindingExpression>
              <Id xmlns=""COE.FormGroup"">MolWeightTextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std20x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <CSSClass xmlns=""COE.FormGroup"">FETextBoxViewMode</CSSClass>
                  <Mask xmlns=""COE.FormGroup"">####.##</Mask>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
            <formElement name=""Identifiers"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">Structure Identifiers</label>
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
              <dataSource/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
          </viewMode>
          <clientScripts xmlns=""COE.FormGroup""/>";

            _messages.Add("Add Structure Information section");
        }

        /// <summary>
        /// 3. Remove content that has been moved to "Structure Information" section from "Component Information" section (a coeForm with id="0"),including (viewMode/):
        ///    a. Structure
        ///    b. Component ID
        ///    c. MF
        ///    d. MW
        /// </summary>
        /// <param name="doc"></param>
        void RemoveFrom_CmpInf_COEForm(XmlDocument doc, XmlNamespaceManager manager)
        {
            XmlNode cmpInf_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms/COE:coeForm[@id='0']", manager);
            XmlNode structure_FormElement = cmpInf_COEForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Structure']", manager);
            XmlNode componentID_FormElement = cmpInf_COEForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Component ID']", manager);
            XmlNode MF_FormElement = cmpInf_COEForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='MF']", manager);
            XmlNode MW_FormElement = cmpInf_COEForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='MW']", manager);

            if (structure_FormElement != null)
            {
                structure_FormElement.ParentNode.RemoveChild(structure_FormElement);
                _messages.Add(string.Format("Remove {0} formElement from Component Information section", "Structure"));
            }
            if (componentID_FormElement != null)
            {
                componentID_FormElement.ParentNode.RemoveChild(componentID_FormElement);
                _messages.Add(string.Format("Remove {0} formElement from Component Information section", "Component ID"));
            }
            if (MF_FormElement != null)
            {
                MF_FormElement.ParentNode.RemoveChild(MF_FormElement);
                _messages.Add(string.Format("Remove {0} formElement from Component Information section", "MF"));
            }
            if (MW_FormElement != null)
            {
                MW_FormElement.ParentNode.RemoveChild(MW_FormElement);
                _messages.Add(string.Format("Remove {0} formElement from Component Information section", "MW"));
            }
        }

        /// <summary>
        /// 4. Edit Identifiers formElemet of "Component Information" section
        ///    a. Change label from Identifiers to "Component Identifiers"
        ///    b. set DefaultEmptyRows from 1 to 0
        /// </summary>
        /// <param name="doc"></param>
        void Edit_CmpInf_COEForm(XmlDocument doc, XmlNamespaceManager manager)
        {
            XmlNode cmpInf_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms/COE:coeForm[@id='0']", manager);
            XmlNode identifiers_FormElement = cmpInf_COEForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Identifiers']", manager);

            XmlNode identifiers_Label = identifiers_FormElement.SelectSingleNode("./COE:label", manager);
            identifiers_Label.InnerText = "Component Identifiers";
            _messages.Add("Edit label of component identifiers");

            XmlNode identifiers_DefaultEmptyRows = identifiers_FormElement.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:DefaultEmptyRows", manager);
            identifiers_DefaultEmptyRows.InnerText = "0";
            _messages.Add("Set DefaultEmptyRows of component identifiers to 0");
        }

        /// <summary>
        /// 5. Place "Component Information" section after "Component custom properties" section (a coeForm with id="1001")
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="manager"></param>
        void Move_CmpInf_COEForm(XmlDocument doc, XmlNamespaceManager manager)
        {
            XmlNode cmpInf_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms/COE:coeForm[@id='0']", manager);
            XmlNode cmpCstPrp_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms/COE:coeForm[@id='1001']", manager);
            cmpInf_COEForm.ParentNode.InsertAfter(cmpInf_COEForm, cmpCstPrp_COEForm);
            _messages.Add(@"Place ""Component Information"" section after ""Component custom properties"" section (a coeForm with id=""1001"")");
        }

        /// <summary>
        /// 6. Assign "Fragment List" section a new id (from 1 to 3)
        /// </summary>
        /// <param name="doc"></param>
        void Edit_FrgLst_COEForm(XmlDocument doc, XmlNamespaceManager manager)
        {
            ///formGroup/detailsForms/detailsForm/coeForms/coeForm[5]
            XmlNode frgLst_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:detailsForms/COE:detailsForm/COE:coeForms/COE:coeForm[COE:title='Fragment List']", manager);
            frgLst_COEForm.Attributes["id"].Value = "3";
            _messages.Add("Set form id of Fragment List form to 3");
        }
    }
}
