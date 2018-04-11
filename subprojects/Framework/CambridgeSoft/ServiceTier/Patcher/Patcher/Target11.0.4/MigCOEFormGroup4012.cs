using System;
using System.Collections.Generic;
using System.Xml;

using CambridgeSoft.COE.Patcher.Utilities;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// coeForm 1
    /// 1. Change title of coeForm 1 from 'Component Information' to 'Structure Information';
    /// 2. Remove Structure form element from coeForm1's layoutinfo;
    /// 3. Add bindingExpression node to coeForm1's BaseFragmentStructure form element in addMode, editMode and viewMode;
    /// 4. Replace the whole Identifiers form element in coeForm1's addMode, editMode and viewMode (too many individual changes);
    /// 5. Add STRUCT_COMMENTS form element to coeForm1's addMode, editMode and viewMode;
    /// 6. Add Structure ID form element to coeForm1's editMode and viewMode;
    /// 
    /// coeForm 1001
    /// 7. Add title and titleCssClass nodes to coeForm 1001;
    /// 8. Add Identifiers form element to coeForm1001's addMode, editMode and viewMode;
    /// 9. Change displayInfo value of coeForm1001's CMP_COMMENTS, STRUCTURE_COMMENTS_TXT form element in addMode, editMode and viewMode;
    /// 
    /// coeForm 0
    /// 10. Change label of Identifier form element in editMode and viewMode from 'Identifiers' to 'Registry Identifiers';
    /// 
    /// 11. Add a new coeForm 1004 to the end of coeForms;
    /// </summary>
	public class MigCOEFormGroup4012 : BugFixBaseCommand
    {
        #region COEFORM1_IDENTIFIERS_INNER_XML_ADDMODE
        private const string COEFORM1_IDENTIFIERS_INNER_XML_ADDMODE = @"
              <label xmlns=""COE.FormGroup"">Structure Identifiers</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup"" />
              <fileUploadBindingExpression xmlns=""COE.FormGroup"" />
              <helpText xmlns=""COE.FormGroup"" />
              <defaultValue xmlns=""COE.FormGroup"" />
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.IdentifierList</bindingExpression>
              <Id xmlns=""COE.FormGroup"">Structure_IdentifiersUltraGrid</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
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
                        <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" xmlns=""COE.FormGroup"" />
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
                                  <Column key=""IdentifierID"" title=""Identifier ID"" visible=""false"" xmlns=""COE.FormGroup"" />
                                  <Column key=""Name"" title=""Name"" xmlns=""COE.FormGroup"" />
                                  <Column key=""Description"" title=""Description"" xmlns=""COE.FormGroup"" />
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
                              <fieldConfig xmlns=""COE.FormGroup"" />
                              <MaxLength xmlns=""COE.FormGroup"">50</MaxLength>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                            </displayInfo>
                          </formElement>
                        </Column>
                        <Column name=""Name"" xmlns=""COE.FormGroup"" />
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
              <dataSource xmlns=""COE.FormGroup"" />
              <dataSourceId xmlns=""COE.FormGroup"" />
              <displayData xmlns=""COE.FormGroup"" />
            ";
        #endregion
        #region COEFORM1_IDENTIFIERS_INNER_XML_EDITMODE
        private const string COEFORM1_IDENTIFIERS_INNER_XML_EDITMODE = @"
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
              <displayData xmlns=""COE.FormGroup""/>";
        #endregion
        #region COEFORM1_IDENTIFIERS_INNER_XML_VIEWMODE
        private const string COEFORM1_IDENTIFIERS_INNER_XML_VIEWMODE = @"
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
        #endregion
        #region STRUCT_COMMENTS_INNER_XML_ADDMODE
        private const string STRUCT_COMMENTS_INNER_XML_ADDMODE = @"<label xmlns=""COE.FormGroup"">Structure Comments</label>
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
              <displayData xmlns=""COE.FormGroup""/>";
        #endregion
        #region STRUCT_COMMENTS_INNER_XML_EDITMODE
        private const string STRUCT_COMMENTS_INNER_XML_EDITMODE = @"<label xmlns=""COE.FormGroup"">Structure Comments</label>
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
              <displayData xmlns=""COE.FormGroup""/>";
        #endregion
        #region STRUCT_COMMENTS_INNER_XML_VIEWMODE
        private const string STRUCT_COMMENTS_INNER_XML_VIEWMODE = @"<label xmlns=""COE.FormGroup"">Structure Comments</label>
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
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly</type>
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
              <displayData xmlns=""COE.FormGroup""/>";
        #endregion
        #region STRUCTUREID_INNER_XML_EDITMODE
        private const string STRUCTUREID_INNER_XML_EDITMODE = @"<label xmlns=""COE.FormGroup"">Structure ID</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.ID</bindingExpression>
              <Id xmlns=""COE.FormGroup"">CSID</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSClass xmlns=""COE.FormGroup"">FETextBoxViewMode</CSSClass>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>";
        #endregion
        #region STRUCTUREID_INNER_XML_VIEWMODE
        private const string STRUCTUREID_INNER_XML_VIEWMODE = @"<label xmlns=""COE.FormGroup"">Structure ID</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup""/>
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.ID</bindingExpression>
              <Id xmlns=""COE.FormGroup"">CSID</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std20x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
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
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>";
        #endregion
        #region COEFORM1001_IDENTIFIERS_INNER_XML_ADDMODE
        private const string COEFORM1001_IDENTIFIERS_INNER_XML_ADDMODE = @"
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
              <validationRuleListv/>
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
                          <!--dataSourceID=""IdentifiersCslaDataSource""-->
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">ACompound_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">CompoundIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_ACompound_IdentifiersDropDownList</ID>
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
                            <Id xmlns=""COE.FormGroup"">ACompound_IdentifiersNameTextEdit</Id>
                            <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup""/>
                              <MaxLength xmlns=""COE.FormGroup"">50</MaxLength>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <left xmlns=""COE.FormGroup"">110px</left>
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
                    <!-- Comment this line to remove the uniqueness restriction, for CSBR 125310 -->
                    <!-- <Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(IdentifierID)}</Event> -->
                    <Event name=""CustomValidation"" xmlns=""COE.FormGroup"">
                      {CustomJS_CustomRowsValidation}
                      <Params parentColKey=""IdentifierID"" childColKey=""InputText"" xmlns=""COE.FormGroup"">
                        <param parentColValue=""1"" validationMethod=""IsAValidCas"" errorMessage=""Invalid CAS number. Please check the entered value."" xmlns=""COE.FormGroup""/>
                      </Params>
                    </Event>
                  </ClientSideEvents>
                  <DefaultRows xmlns=""COE.FormGroup""/>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
            ";
                #endregion
        #region COEFORM1001_IDENTIFIERS_INNER_XML_EDITMODE
        private const string COEFORM1001_IDENTIFIERS_INNER_XML_EDITMODE = @"
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
                          <!--dataSourceID=""IdentifiersCslaDataSource""-->
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">ACompound_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">CompoundIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_ACompound_IdentifiersDropDownList</ID>
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
                            <Id xmlns=""COE.FormGroup"">ACompound_IdentifiersNameTextEdit</Id>
                            <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup""/>
                              <MaxLength xmlns=""COE.FormGroup"">50</MaxLength>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <left xmlns=""COE.FormGroup"">110px</left>
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
                    <!-- Comment this line to remove the uniqueness restriction, for CSBR 125310 -->
                    <!-- <Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(IdentifierID)}</Event> -->
                    <Event name=""CustomValidation"" xmlns=""COE.FormGroup"">
                      {CustomJS_CustomRowsValidation}
                      <Params parentColKey=""IdentifierID"" childColKey=""InputText"" xmlns=""COE.FormGroup"">
                        <param parentColValue=""1"" validationMethod=""IsAValidCas"" errorMessage=""Invalid CAS number. Please check the entered value."" xmlns=""COE.FormGroup""/>
                      </Params>
                    </Event>
                  </ClientSideEvents>
                  <DefaultRows xmlns=""COE.FormGroup""/>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
              ";
        #endregion
        #region COEFORM1001_IDENTIFIERS_INNER_XML_VIEWMODE
        private const string COEFORM1001_IDENTIFIERS_INNER_XML_VIEWMODE = @"
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
                          <formElement xmlns=""COE.FormGroup"">
                            <Id xmlns=""COE.FormGroup"">ACompound_IdentifiersDropDownList</Id>
                            <label xmlns=""COE.FormGroup"">Type</label>
                            <bindingExpression xmlns=""COE.FormGroup"">IdentifierID</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup"">
                                <CSSClass xmlns=""COE.FormGroup"">FEDropDownListGrid</CSSClass>
                                <CSSLabelClass xmlns=""COE.FormGroup"">COERequiredField</CSSLabelClass>
                                <DataSourceID xmlns=""COE.FormGroup"">CompoundIdentifiersCslaDataSource</DataSourceID>
                                <DataTextField xmlns=""COE.FormGroup"">Name</DataTextField>
                                <DataValueField xmlns=""COE.FormGroup"">IdentifierID</DataValueField>
                                <ID xmlns=""COE.FormGroup"">Inner_ACompound_IdentifiersDropDownList</ID>
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
                            <Id xmlns=""COE.FormGroup"">ACompound_IdentifiersNameTextEdit</Id>
                            <bindingExpression xmlns=""COE.FormGroup"">InputText</bindingExpression>
                            <configInfo xmlns=""COE.FormGroup"">
                              <fieldConfig xmlns=""COE.FormGroup""/>
                              <MaxLength xmlns=""COE.FormGroup"">50</MaxLength>
                            </configInfo>
                            <displayInfo xmlns=""COE.FormGroup"">
                              <left xmlns=""COE.FormGroup"">110px</left>
                              <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit</type>
                            </displayInfo>
                          </formElement>
                        </Column>
                        <Column name=""Name"" xmlns=""COE.FormGroup""/>
                      </Columns>
                    </table>
                  </tables>
                  <DefaultRows xmlns=""COE.FormGroup""/>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
        ";
        #endregion
        #region COEFORM1004_INNER_XML
        private const string COEFORM1004_INNER_XML = @"
          <validationRuleList xmlns=""COE.FormGroup""/>
          <layoutInfo xmlns=""COE.FormGroup""/>
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
          </viewMode>
          <clientScripts xmlns=""COE.FormGroup""/>
";
        #endregion

        private XmlDocument coeFormGroup4012 = null;

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            coeFormGroup4012 = PatcherUtility.GetCoeFormGroupById(forms, "4012");
            if (coeFormGroup4012 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeFormGroup 4012");
            }
            else
            {
                PatcherUtility.SetXmlNamespaceManager(coeFormGroup4012);

                UpgradeCoeForm1(ref errorsInPatch, messages);
                UpgradeCOEForm1001(ref errorsInPatch, messages);
                UpgradeCoeForm0(ref errorsInPatch, messages);
                AddCoeForm1004(ref errorsInPatch, messages);
            }

            if (!errorsInPatch)
                messages.Add("COE form group 4012 was successfully migrated");
            else
                messages.Add("Failed to migrate COE form group 4012");

            return messages;
        }

        private void UpgradeCoeForm1(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm1 = coeFormGroup4012.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']", PatcherUtility.XmlNamespaceManager);
            if (coeForm1 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 1");
            }
            else
            {
                // 1
                PatcherUtility.ChangeCOEFormTitle(coeForm1, "Structure Information");
                // 2
                PatcherUtility.RemoveFormElement(coeForm1.SelectSingleNode("./COE:layoutInfo", PatcherUtility.XmlNamespaceManager), "Structure");
                // 3
                AddBindingExpressionNodeToBaseFragmentStructure(coeForm1, "addMode");
                AddBindingExpressionNodeToBaseFragmentStructure(coeForm1, "editMode");
                AddBindingExpressionNodeToBaseFragmentStructure(coeForm1, "viewMode");
                // 4
                ReplaceIDENTIFIERSINNERXML(coeForm1, "addMode", COEFORM1_IDENTIFIERS_INNER_XML_ADDMODE);
                ReplaceIDENTIFIERSINNERXML(coeForm1, "editMode", COEFORM1_IDENTIFIERS_INNER_XML_EDITMODE);
                ReplaceIDENTIFIERSINNERXML(coeForm1, "viewMode", COEFORM1_IDENTIFIERS_INNER_XML_VIEWMODE);
                // 5
                PatcherUtility.InsertFormElementAfter(coeFormGroup4012,
                    null,
                    "STRUCT_COMMENTS",
                    STRUCT_COMMENTS_INNER_XML_ADDMODE,
                    coeForm1.SelectSingleNode("./COE:addMode/COE:formElement[@name='StructureID']", PatcherUtility.XmlNamespaceManager)
                );
                PatcherUtility.InsertFormElementAfter(coeFormGroup4012,
                    null,
                    "STRUCT_COMMENTS",
                    STRUCT_COMMENTS_INNER_XML_EDITMODE,
                    coeForm1.SelectSingleNode("./COE:editMode/COE:formElement[@name='StructureID']", PatcherUtility.XmlNamespaceManager)
                );
                PatcherUtility.InsertFormElementAfter(coeFormGroup4012,
                    null,
                    "STRUCT_COMMENTS",
                    STRUCT_COMMENTS_INNER_XML_VIEWMODE,
                    coeForm1.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Identifiers']", PatcherUtility.XmlNamespaceManager)
                );
                // 6
                PatcherUtility.InsertFormElementAfter(coeFormGroup4012,
                    null,
                    "Structure ID",
                    STRUCTUREID_INNER_XML_EDITMODE,
                    coeForm1.SelectSingleNode("./COE:editMode/COE:formElement[@name='Component ID']", PatcherUtility.XmlNamespaceManager)
                );
                PatcherUtility.InsertFormElementAfter(coeFormGroup4012,
                    null,
                    "Structure ID",
                    STRUCTUREID_INNER_XML_VIEWMODE,
                    coeForm1.SelectSingleNode("./COE:viewMode/COE:formElement[@name='MW']", PatcherUtility.XmlNamespaceManager)
                );
            }
        }

        private void AddBindingExpressionNodeToBaseFragmentStructure(XmlNode coeForm, string mode)
        {
            XmlNode bindingExpressionNode = coeFormGroup4012.CreateElement("bindingExpression", PatcherUtility.NAMESPACE_URI);
            bindingExpressionNode.InnerXml = "Compound.BaseFragment.Structure.Value";

            XmlNode formElemementInMode = coeForm.SelectSingleNode(
                string.Format("./COE:{0}/COE:formElement[@name='BaseFragmentStructure']", mode),
                PatcherUtility.XmlNamespaceManager);
            XmlNode defaultValueNode = formElemementInMode.SelectSingleNode("./COE:defaultValue", PatcherUtility.XmlNamespaceManager);

            formElemementInMode.InsertAfter(bindingExpressionNode, defaultValueNode);
        }

        private void ReplaceIDENTIFIERSINNERXML(XmlNode coeForm, string mode, string innerXml)
        {
            XmlNode formElemementInMode = coeForm.SelectSingleNode(
                string.Format("./COE:{0}/COE:formElement[@name='Identifiers']", mode),
                PatcherUtility.XmlNamespaceManager);

            formElemementInMode.InnerXml = innerXml;
        }

        private void UpgradeCOEForm1001(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm1001 = coeFormGroup4012.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']", PatcherUtility.XmlNamespaceManager);
            if (coeForm1001 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 1001");
            }
            else
            {
                // 7
                AddTitleCssClass(coeForm1001);
                AddTitle(coeForm1001);
                // 8

                // AddMode
                if (coeForm1001.SelectSingleNode("COE:addMode/COE:formElement[@name='Identifiers']", PatcherUtility.XmlNamespaceManager) == null)
                    PatcherUtility.PrependFormElement(coeFormGroup4012, "Identifiers",
                    COEFORM1001_IDENTIFIERS_INNER_XML_ADDMODE,
                    coeForm1001.SelectSingleNode("./COE:addMode", PatcherUtility.XmlNamespaceManager)
                );
                else
                    ReplaceIDENTIFIERSINNERXML(coeForm1001, "addMode", COEFORM1001_IDENTIFIERS_INNER_XML_ADDMODE);

                // EditMode
                if (coeForm1001.SelectSingleNode("COE:editMode/COE:formElement[@name='Identifiers']", PatcherUtility.XmlNamespaceManager) == null)
                    PatcherUtility.PrependFormElement(coeFormGroup4012, "Identifiers",
                     COEFORM1001_IDENTIFIERS_INNER_XML_EDITMODE,
                     coeForm1001.SelectSingleNode("./COE:editMode", PatcherUtility.XmlNamespaceManager)
                 );
                else
                    ReplaceIDENTIFIERSINNERXML(coeForm1001, "editMode", COEFORM1001_IDENTIFIERS_INNER_XML_EDITMODE);

                // ViewMode
                if (coeForm1001.SelectSingleNode("COE:viewMode/COE:formElement[@name='Identifiers']", PatcherUtility.XmlNamespaceManager) == null)
                    PatcherUtility.PrependFormElement(coeFormGroup4012, "Identifiers",
                     COEFORM1001_IDENTIFIERS_INNER_XML_VIEWMODE,
                     coeForm1001.SelectSingleNode("./COE:viewMode", PatcherUtility.XmlNamespaceManager)
                 );
                else
                    ReplaceIDENTIFIERSINNERXML(coeForm1001, "viewMode", COEFORM1001_IDENTIFIERS_INNER_XML_VIEWMODE);
                
                
                // 9
                ReplaceCssClassWithStyleInDisplayInfo(coeForm1001, "./COE:addMode/COE:formElement[@name='CMP_COMMENTS']/COE:displayInfo");
                ReplaceCssClassWithStyleInDisplayInfo(coeForm1001, "./COE:editMode/COE:formElement[@name='CMP_COMMENTS']/COE:displayInfo");
                ReplaceCssClassWithStyleInDisplayInfo(coeForm1001, "./COE:viewMode/COE:formElement[@name='CMP_COMMENTS']/COE:displayInfo");
                ReplaceCssClassWithStyleInDisplayInfo(coeForm1001, "./COE:addMode/COE:formElement[@name='STRUCTURE_COMMENTS_TXT']/COE:displayInfo");
                ReplaceCssClassWithStyleInDisplayInfo(coeForm1001, "./COE:editMode/COE:formElement[@name='STRUCTURE_COMMENTS_TXT']/COE:displayInfo");
                ReplaceCssClassWithStyleInDisplayInfo(coeForm1001, "./COE:viewMode/COE:formElement[@name='STRUCTURE_COMMENTS_TXT']/COE:displayInfo");
            }
        }

        private void AddTitleCssClass(XmlNode coeForm1001)
        {
            XmlNode titleCssClassNode = coeFormGroup4012.CreateElement("titleCssClass", PatcherUtility.NAMESPACE_URI);
            titleCssClassNode.InnerXml = "COEFormTitle";

            XmlNode validationRuleListNode = coeForm1001.SelectSingleNode("./COE:validationRuleList", PatcherUtility.XmlNamespaceManager);

            validationRuleListNode.ParentNode.InsertAfter(titleCssClassNode, validationRuleListNode);
        }

        private void AddTitle(XmlNode coeForm)
        {
            XmlNode titleNode = coeFormGroup4012.CreateElement("title", PatcherUtility.NAMESPACE_URI);
            titleNode.InnerXml = "Component Information";

            XmlNode validationRuleListNode = coeForm.SelectSingleNode("./COE:validationRuleList", PatcherUtility.XmlNamespaceManager);

            validationRuleListNode.ParentNode.InsertAfter(titleNode, validationRuleListNode);
        }

        private void ReplaceCssClassWithStyleInDisplayInfo(XmlNode coeForm, string displayInfoNodeXPath)
        {
            XmlNode styleNode = coeFormGroup4012.CreateElement("style", PatcherUtility.NAMESPACE_URI);
            styleNode.InnerXml = "width:66%";

            XmlNode displayInfoNode = coeForm.SelectSingleNode(displayInfoNodeXPath, PatcherUtility.XmlNamespaceManager);
            //Coverity fix - CID 19445
            if (displayInfoNode != null)
            {
                XmlNode cssClassNode = displayInfoNode.SelectSingleNode("./COE:cssClass", PatcherUtility.XmlNamespaceManager);
                if (cssClassNode != null)
                    displayInfoNode.ReplaceChild(styleNode, cssClassNode);
            }
        }

        private void UpgradeCoeForm0(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm0 = coeFormGroup4012.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']", PatcherUtility.XmlNamespaceManager);
            if (coeForm0 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 0");
            }
            else
            {
                // 9
                coeForm0.SelectSingleNode("./COE:editMode/COE:formElement[@name='Identifier']/COE:label", PatcherUtility.XmlNamespaceManager)
                    .InnerXml = "Registry Identifiers";
                coeForm0.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Identifiers']/COE:label", PatcherUtility.XmlNamespaceManager)
                    .InnerXml = "Registry Identifiers";
            }
        }

        private void AddCoeForm1004(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm8 = coeFormGroup4012.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='8']", PatcherUtility.XmlNamespaceManager);
            if (coeForm8 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 8");
            }
            else
            {
                if (coeFormGroup4012.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1004']", PatcherUtility.XmlNamespaceManager) == null)
                {
                    XmlNode coeForm1004 = coeFormGroup4012.CreateElement("coeForm", "COE.FormGroup");
                    XmlAttribute idAttr = coeFormGroup4012.CreateAttribute("id");
                    idAttr.Value = "1004";
                    coeForm1004.Attributes.Append(idAttr);
                    XmlAttribute nameAttr = coeFormGroup4012.CreateAttribute("dataSourceId");
                    nameAttr.Value = "ComponentListCslaDataSource";
                    coeForm1004.Attributes.Append(nameAttr);

                    coeForm8.ParentNode.InsertAfter(coeForm1004, coeForm8);

                    coeForm1004.InnerXml = COEFORM1004_INNER_XML;
                }
            }
        }
    }
}
