using System;
using System.Collections.Generic;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Include a list of changes made to this form group in 11.0.4.
    /// </summary>
	public class MigCOEFormGroup4010: BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPath=false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4010")
                {
                    messages.Add("Processing 4010 formGroup xml.");
                    string originalForm_OuterXml = doc.OuterXml;

                    try
                    {
                        //Changes in coeForm id=0
                        messages.Add("Processing coeform id=0");
                        XmlNode CoeFormId0_addMode_IdentifiersLabel = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:addMode/COE:formElement[@name='Identifiers']/COE:label/text()", manager);
                        XmlNode CoeFormId0_viewMode_IdentifiersLabel = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']/COE:label/text()", manager);

                        if (CoeFormId0_addMode_IdentifiersLabel != null &&
                            !CoeFormId0_addMode_IdentifiersLabel.Value.Equals("Registry Identifiers"))
                        {
                            CoeFormId0_addMode_IdentifiersLabel.Value = "Registry Identifiers";
                            messages.Add("A label name has been changed.");
                        }
                        if (CoeFormId0_viewMode_IdentifiersLabel != null &&
                            !CoeFormId0_viewMode_IdentifiersLabel.Value.Equals("Registry Identifiers"))
                        {
                            CoeFormId0_viewMode_IdentifiersLabel.Value = "Registry Identifiers";
                            messages.Add("A label name has been changed.");
                        }

                        //Changes in coeform id=1
                        messages.Add("Processing coeform id=1");
                        XmlNode CoeFormId1_title = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:title/text()", manager);

                        XmlNode CoeFormId1_addMode_Identifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                        XmlNode CoeFormId1_editMode_Identifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                        XmlNode CoeFormId1_viewMode_Identifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);

                        if (CoeFormId1_title != null &&
                            !CoeFormId1_title.Value.Equals("Structure Information"))
                        {
                            CoeFormId1_title.Value = "Structure Information";
                            messages.Add("Title of coeform id=1 has been changed.");
                        }
                        if (CoeFormId1_addMode_Identifiers != null &&
                            !CoeFormId1_addMode_Identifiers.SelectSingleNode("./COE:bindingExpression/text()", manager).Value.Equals("Compound.BaseFragment.Structure.IdentifierList"))
                        {
                            CoeFormId1_addMode_Identifiers.InnerXml = XmlDepository.AdaptableStructureIdentifers;
                            messages.Add("AddMode Identifiers has been changed to structure level.");
                        }
                        if (CoeFormId1_editMode_Identifiers != null &&
                            !CoeFormId1_editMode_Identifiers.SelectSingleNode("./COE:bindingExpression/text()", manager).Value.Equals("Compound.BaseFragment.Structure.IdentifierList"))
                        {
                            CoeFormId1_editMode_Identifiers.InnerXml = XmlDepository.AdaptableStructureIdentifers;
                            messages.Add("EditMode Identifiers has been changed to structure level.");
                        }
                        if (CoeFormId1_viewMode_Identifiers != null &&
                            !CoeFormId1_viewMode_Identifiers.SelectSingleNode("./COE:bindingExpression/text()", manager).Value.Equals("Compound.BaseFragment.Structure.IdentifierList"))
                        {
                            CoeFormId1_viewMode_Identifiers.InnerXml = XmlDepository.ReadonlyStructureIdentifiers;
                            messages.Add("ViewMode Identifiers has been changed to structure level.");
                        }

                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='STRUCT_COMMENTS']", manager) == null)
                        {
                            XmlNode CoeFormId1_addMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode", manager);
                            XmlNode CoeFormId1_addMode_StructComments = doc.CreateElement("formElement", "COE.FormGroup");
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "STRUCT_COMMENTS";
                            CoeFormId1_addMode_StructComments.Attributes.Append(name);
                            CoeFormId1_addMode.InsertAfter(CoeFormId1_addMode_StructComments, CoeFormId1_addMode.LastChild);
                            CoeFormId1_addMode_StructComments.InnerXml = XmlDepository.StructureComments;
                        }
                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='STRUCT_COMMENTS']", manager) == null)
                        {
                            XmlNode CoeFormId1_editMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode", manager);
                            XmlNode CoeFormId1_editMode_StructComments = doc.CreateElement("formElement", "COE.FormGroup");
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "STRUCT_COMMENTS";
                            CoeFormId1_editMode_StructComments.Attributes.Append(name);
                            CoeFormId1_editMode.InsertAfter(CoeFormId1_editMode_StructComments, CoeFormId1_editMode.LastChild);
                            CoeFormId1_editMode_StructComments.InnerXml = XmlDepository.StructureComments;
                        }
                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='STRUCT_COMMENTS']", manager) == null)
                        {
                            XmlNode CoeFormId1_viewMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode", manager);
                            XmlNode CoeFormId1_viewMode_StructComments = doc.CreateElement("formElement", "COE.FormGroup");
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "STRUCT_COMMENTS";
                            CoeFormId1_viewMode_StructComments.Attributes.Append(name);
                            CoeFormId1_viewMode.InsertAfter(CoeFormId1_viewMode_StructComments, CoeFormId1_viewMode.LastChild);
                            CoeFormId1_viewMode_StructComments.InnerXml = XmlDepository.StructureComments;
                        }


                        //Changes in coeform id=2
                        messages.Add("Processing coeform id=2");
                        XmlNode CoeFormId2_title = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:title/text()", manager);

                        XmlNode CoeFormId2_viewMode_Identifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);

                        if (CoeFormId2_title != null &&
                            !CoeFormId2_title.Value.Equals("Structure Information"))
                        {
                            CoeFormId2_title.Value = "Structure Information";
                            messages.Add("Title of coeform id=2 has been changed.");
                        }
                        if (CoeFormId2_viewMode_Identifiers != null &&
                            !CoeFormId2_viewMode_Identifiers.SelectSingleNode("./COE:bindingExpression/text()", manager).Value.Equals("Compound.BaseFragment.Structure.IdentifierList"))
                        {
                            CoeFormId2_viewMode_Identifiers.InnerXml = XmlDepository.ReadonlyStructureIdentifiers;
                            messages.Add("ViewMode Identifiers has been changed to structure level.");
                        }

                        //Changes in coeform id=1001
                        messages.Add("Processing coeform id=1001");
                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:title", manager) == null)
                        {
                            XmlNode CoeFormId1001 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']", manager);
                            XmlNode CoeFormId1001_title = doc.CreateElement("title", "COE.FormGroup");
                            CoeFormId1001_title.InnerText = "Component Information";
                            XmlNode CoeFormId1001_titleCssClass = doc.CreateElement("titleCssClass", "COE.FormGroup");
                            CoeFormId1001_titleCssClass.InnerText = "COEFormTitle";
                            CoeFormId1001.InsertAfter(CoeFormId1001_titleCssClass, CoeFormId1001.FirstChild);
                            CoeFormId1001.InsertAfter(CoeFormId1001_title, CoeFormId1001.FirstChild);
                            messages.Add("New title added.");
                        }

                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:addMode/COE:formElement[@name='Identifiers']", manager) == null)
                        {
                            XmlNode CoeFormId1001_addMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:addMode", manager);
                            XmlNode CoeFormId1001_addMode_Identifiers = doc.CreateElement("formElement", "COE.FormGroup");
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "Identifiers";
                            CoeFormId1001_addMode_Identifiers.Attributes.Append(name);
                            CoeFormId1001_addMode.InsertBefore(CoeFormId1001_addMode_Identifiers, CoeFormId1001_addMode.FirstChild);
                            CoeFormId1001_addMode_Identifiers.InnerXml = XmlDepository.AdaptableComponentIdentifiers;
                            messages.Add("New Identifiers added.");
                        }

                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:editMode/COE:formElement[@name='Identifiers']", manager) == null)
                        {
                            XmlNode CoeFormId1001_editMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:editMode", manager);
                            XmlNode CoeFormId1001_editMode_Identifiers = doc.CreateElement("formElement", "COE.FormGroup");
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "Identifiers";
                            CoeFormId1001_editMode_Identifiers.Attributes.Append(name);
                            CoeFormId1001_editMode.InsertBefore(CoeFormId1001_editMode_Identifiers, CoeFormId1001_editMode.FirstChild);
                            CoeFormId1001_editMode_Identifiers.InnerXml = XmlDepository.AdaptableComponentIdentifiers;
                            messages.Add("New Identifiers added.");
                        }

                        if (doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:viewMode/COE:formElement[@name='Identifiers']", manager) == null)
                        {
                            XmlNode CoeFormId1001_viewMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:viewMode", manager);
                            XmlNode CoeFormId1001_viewMode_Identifiers = doc.CreateElement("formElement", "COE.FormGroup");
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "Identifiers";
                            CoeFormId1001_viewMode_Identifiers.Attributes.Append(name);
                            CoeFormId1001_viewMode.InsertBefore(CoeFormId1001_viewMode_Identifiers, CoeFormId1001_viewMode.FirstChild);
                            CoeFormId1001_viewMode_Identifiers.InnerXml = XmlDepository.ReadonlyComponentIdentifiers;
                            messages.Add("New Identifiers added.");
                        }

                        XmlNode CoeFormId1001_addMode_CMPCOMMENTS = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:addMode/COE:formElement[@name='CMP_COMMENTS']", manager);
                        XmlNode CoeFormId1001_addMode_STRUCTURECOMMENTSTXT = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:addMode/COE:formElement[@name='STRUCTURE_COMMENTS_TXT']", manager);
                        XmlNode CoeFormId1001_editMode_CMPCOMMENTS = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:editMode/COE:formElement[@name='CMP_COMMENTS']", manager);
                        XmlNode CoeFormId1001_editMode_STRUCTURECOMMENTSTXT = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:editMode/COE:formElement[@name='STRUCTURE_COMMENTS_TXT']", manager);
                        XmlNode CoeFormId1001_viewMode_CMPCOMMENTS = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:viewMode/COE:formElement[@name='CMP_COMMENTS']", manager);
                        XmlNode CoeFormId1001_viewMode_STRUCTURECOMMENTSTXT = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:viewMode/COE:formElement[@name='STRUCTURE_COMMENTS_TXT']", manager);

                        if (CoeFormId1001_addMode_CMPCOMMENTS != null)
                        {
                            Update1001CssClass(CoeFormId1001_addMode_CMPCOMMENTS, doc, manager);
                            messages.Add("New style added.");
                        }
                        if (CoeFormId1001_addMode_STRUCTURECOMMENTSTXT != null)
                        {
                            Update1001CssClass(CoeFormId1001_addMode_STRUCTURECOMMENTSTXT, doc, manager);
                            messages.Add("New style added.");
                        }
                        if (CoeFormId1001_editMode_CMPCOMMENTS != null)
                        {
                            Update1001CssClass(CoeFormId1001_editMode_CMPCOMMENTS, doc, manager);
                            messages.Add("New style added.");
                        }
                        if (CoeFormId1001_editMode_STRUCTURECOMMENTSTXT != null)
                        {
                            Update1001CssClass(CoeFormId1001_editMode_STRUCTURECOMMENTSTXT, doc, manager);
                            messages.Add("New style added.");
                        }
                        if (CoeFormId1001_viewMode_CMPCOMMENTS != null)
                        {
                            Update1001CssClass(CoeFormId1001_viewMode_CMPCOMMENTS, doc, manager);
                            messages.Add("New style added.");
                        }
                        if (CoeFormId1001_viewMode_STRUCTURECOMMENTSTXT != null)
                        {
                            Update1001CssClass(CoeFormId1001_viewMode_STRUCTURECOMMENTSTXT, doc, manager);
                            messages.Add("New style added.");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        errorInPath = true;
                        messages.Add("Processing with error: "+ ex.Message);
                    }

                    if (errorInPath == true)
                    {
                        messages.Add("Rollback changes to formGroup 4010, fali to migrate 4010 formGroup xml.");
                        doc.LoadXml(originalForm_OuterXml);
                    }
                    else
                    {
                        messages.Add("4010 processing finished. Succeed to migrate 4010 formGroup xml.");
                    }
                }
            }
            
            return messages;
        }

        private static void Update1001CssClass(XmlNode node, XmlDocument doc, XmlNamespaceManager manager)
        {
            XmlNode displayInfo = node.SelectSingleNode("./COE:displayInfo", manager);
            XmlNode cssClass = node.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
            if (cssClass != null)
            {
                displayInfo.RemoveChild(cssClass);
            }
            if (node.SelectSingleNode("./COE:displayInfo/COE:style", manager) == null)
            {
                XmlNode style = doc.CreateElement("style", "COE.FormGroup");
                style.InnerText = "width:66%";
                displayInfo.InsertBefore(style, displayInfo.FirstChild);
            }
        }

        class XmlDepository
        {
            #region AdaptableStructureIdentifers
            public const string AdaptableStructureIdentifers = @"
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
                  <ReadOnly xmlns=""COE.FormGroup"">false</ReadOnly>
                  <DefaultEmptyRows xmlns=""COE.FormGroup"">1</DefaultEmptyRows>
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
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion

            #region ReadonlyStructureIdentifiers
            public const string ReadonlyStructureIdentifiers = @"
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
                  <DefaultEmptyRows xmlns=""COE.FormGroup"">0</DefaultEmptyRows>
				  <NoDataMessage xmlns=""COE.FormGroup"">No Identifiers associated</NoDataMessage>
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
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion

            #region StructureComments
            public const string StructureComments = @"
              <label xmlns=""COE.FormGroup"">Structure Comments</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.PropertyList[@Name='STRUCT_COMMENTS'| Value]</bindingExpression>
              <Id xmlns=""COE.FormGroup"">STRUCT_COMMENTSProperty</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std100x80</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup"">
                <validationRule validationRuleName=""textLength"" errorMessage=""Max. 200 characters"" displayPosition=""Top_Left"" xmlns=""COE.FormGroup"">
                  <params xmlns=""COE.FormGroup"">
                    <param name=""min"" value=""0"" xmlns=""COE.FormGroup""/>
                    <param name=""max"" value=""200"" xmlns=""COE.FormGroup""/>
                  </params>
                </validationRule>
              </validationRuleList>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <COE:configInfo xmlns:COE=""COE.FormGroup"">
                <COE:fieldConfig>
                  <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
                  <COE:CSSClass>FETextArea</COE:CSSClass>
                  <COE:TextMode>MultiLine</COE:TextMode>
                </COE:fieldConfig>
              </COE:configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion

            #region AdaptableComponentIdentifiers
            public const string AdaptableComponentIdentifiers = @"
              <label xmlns=""COE.FormGroup"">Component Identifiers</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.IdentifierList</bindingExpression>
              <Id xmlns=""COE.FormGroup"">Compound_IdentifiersUltraGrid</Id>
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
                  <ReadOnly xmlns=""COE.FormGroup"">false</ReadOnly>
                  <DefaultEmptyRows xmlns=""COE.FormGroup"">1</DefaultEmptyRows>
                  <tables xmlns=""COE.FormGroup"">
                    <table xmlns=""COE.FormGroup"">
                      <Columns xmlns=""COE.FormGroup"">
                        <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" xmlns=""COE.FormGroup""/>
                        <!-- Identifier type selection drop down column -->
                        <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""CompoundIdentifiersCslaDataSource"" xmlns=""COE.FormGroup"">
                          <!--dataSourceID=""IdentifiersCslaDataSource"">-->
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">LCompound_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">CompoundIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_LCompound_IdentifiersDropDownList</ID>
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
                            <Id xmlns=""COE.FormGroup"">LCompound_IdentifiersNameTextEdit</Id>
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
                    <Event name=""AfterExitEdit"" xmlns=""COE.FormGroup"">
                      AfterExitEdit();
                    </Event>
                    <Event name=""CustomValidation"" xmlns=""COE.FormGroup"">
                      {CustomJS_CustomRowsValidation}
                      <Params parentColKey=""IdentifierID"" childColKey=""InputText"" xmlns=""COE.FormGroup"">
                        <param parentColValue=""1"" validationMethod=""IsAValidCas"" errorMessage=""Invalid CAS number. Please check the entered value."" xmlns=""COE.FormGroup""/>
                      </Params>
                    </Event>
                  </ClientSideEvents>
                  <DefaultRows xmlns=""COE.FormGroup"">
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""4"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""1"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""2"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                  </DefaultRows>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion

            #region ReadonlyComponentIdentifiers
            public const string ReadonlyComponentIdentifiers = @"
              <label xmlns=""COE.FormGroup"">Component Identifiers</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.IdentifierList</bindingExpression>
              <Id xmlns=""COE.FormGroup"">Compound_IdentifiersUltraGrid</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <style xmlns=""COE.FormGroup"">width:32%;height:160px</style>
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
                  <DefaultEmptyRows xmlns=""COE.FormGroup"">0</DefaultEmptyRows>
                  <NoDataMessage xmlns=""COE.FormGroup"">No Identifiers associated</NoDataMessage>
                  <tables xmlns=""COE.FormGroup"">
                    <table xmlns=""COE.FormGroup"">
                      <Columns xmlns=""COE.FormGroup"">
                        <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" xmlns=""COE.FormGroup""/>
                        <!-- Identifier type selection drop down column -->
                        <Column name=""Type"" dataTextValueField=""Name"" dataSourceID=""CompoundIdentifiersCslaDataSource"" xmlns=""COE.FormGroup"">
                          <!--dataSourceID=""IdentifiersCslaDataSource"">-->
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">LCompound_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">CompoundIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_LCompound_IdentifiersDropDownList</ID>
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
                            <Id xmlns=""COE.FormGroup"">LCompound_IdentifiersNameTextEdit</Id>
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
                    <Event name=""AfterExitEdit"" xmlns=""COE.FormGroup"">
                      AfterExitEdit();
                    </Event>
                    <Event name=""CustomValidation"" xmlns=""COE.FormGroup"">
                      {CustomJS_CustomRowsValidation}
                      <Params parentColKey=""IdentifierID"" childColKey=""InputText"" xmlns=""COE.FormGroup"">
                        <param parentColValue=""1"" validationMethod=""IsAValidCas"" errorMessage=""Invalid CAS number. Please check the entered value."" xmlns=""COE.FormGroup""/>
                      </Params>
                    </Event>
                  </ClientSideEvents>
                  <DefaultRows xmlns=""COE.FormGroup"">
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""4"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""1"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                    <Row xmlns=""COE.FormGroup"">
                      <cell bindingExpression=""IdentifierID"" dataValue=""2"" dataText=""Custom_FindFromEditor"" xmlns=""COE.FormGroup""/>
                    </Row>
                  </DefaultRows>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion
        }
    }
}
