using System;
using System.Xml;
using System.Collections.Generic;

using CambridgeSoft.COE.Patcher.Utilities;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// 1. In coeForm 0, change label of IDENTIFIERTYPE form element from 'Identifier' to 'Registry Identifier';
    /// 2. Add a new coeForm 4 named VW_STRUCTURE between coeForm 0 and coeForm 1;
    /// 3. In coeForm 1, change label of IDENTIFIERTYPE form element from 'Identifier' to 'Component Identifier';
    /// 4. In queryForms/coeForm 2, add IDENTIFIERTYPE and IDENTIFIERVALUE form elements;
    /// </summary>
    public class MigCOEFormGroup4003 : BugFixBaseCommand
    {
        #region VW_STRUCTURE_NODE_INNER_XML
        private const string VW_STRUCTURE_NODE_INNER_XML = @"
          <validationRuleList xmlns=""COE.FormGroup""/>
          <title xmlns=""COE.FormGroup"">Structure Information</title>
          <titleCssClass xmlns=""COE.FormGroup"">COEFormTitle</titleCssClass>
          <layoutInfo xmlns=""COE.FormGroup"">
            <formElement name=""STRUCT_COMMENTS"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">Structure Comments</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[45].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">STRUCT_COMMENTSProperty</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std100x80</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <COE:configInfo xmlns:COE=""COE.FormGroup"">
                <COE:fieldConfig>
                  <COE:Height>50px</COE:Height>
                  <COE:TextMode>MultiLine</COE:TextMode>
                  <COE:CSSClass>FETextArea</COE:CSSClass>
                  <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
                </COE:fieldConfig>
              </COE:configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem fieldid=""452"" id=""45"" tableid=""4"" xmlns=""COE.FormGroup"">
                <textCriteria negate=""NO"" normalizedChemicalName=""NO"" hillFormula=""NO"" fullWordSearch=""NO"" caseSensitive=""NO"" trim=""NONE"" operator=""IN"" hint="""" defaultWildCardPosition=""NONE"" xmlns=""COE.FormGroup""/>
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
            <formElement name=""IDENTIFIERTYPE"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">Structure Identifier</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[46].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">IDENTIFIERTYPETextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std50x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <dropDownItemsSelect xmlns=""COE.FormGroup"">SELECT ID as key, NAME as value FROM REGDB.VW_IDENTIFIERTYPE WHERE (TYPE ='S' OR TYPE = 'A') AND ACTIVE = 'T'</dropDownItemsSelect>
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
                  <Enable xmlns=""COE.FormGroup"">True</Enable>
                  <ID xmlns=""COE.FormGroup"">IDENTIFIERTYPETextBox</ID>
                  <AutoPostBack xmlns=""COE.FormGroup"">False</AutoPostBack>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem fieldid=""1503"" id=""46"" tableid=""15"" xmlns=""COE.FormGroup"">
                <numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" xmlns=""COE.FormGroup""/>
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
            <formElement name=""IdentifierValue"" xmlns=""COE.FormGroup"">
              <label xmlns=""COE.FormGroup"">Value</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[47].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">IdentifierValueTextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std50x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem fieldid=""1502"" id=""47"" tableid=""15"" xmlns=""COE.FormGroup"">
                <textCriteria negate=""NO"" normalizedChemicalName=""NO"" hillFormula=""NO"" fullWordSearch=""NO"" caseSensitive=""NO"" trim=""NONE"" operator=""IN"" hint="""" defaultWildCardPosition=""NONE"" xmlns=""COE.FormGroup""/>
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
            </formElement>
          </layoutInfo>
          <formDisplay xmlns=""COE.FormGroup"">
            <cssClass xmlns=""COE.FormGroup"">COEFormDisplay</cssClass>
            <layoutStyle xmlns=""COE.FormGroup"">flowLayout</layoutStyle>
            <visible xmlns=""COE.FormGroup"">true</visible>
          </formDisplay>
          <addMode xmlns=""COE.FormGroup""/>
          <editMode xmlns=""COE.FormGroup""/>
          <viewMode xmlns=""COE.FormGroup""/>
          <clientScripts xmlns=""COE.FormGroup""/>";
        #endregion
        #region IDENTIFIERTYPE_FORM_ELEMENT
        private const string IDENTIFIERTYPE_FORM_ELEMENT = @"
<label xmlns=""COE.FormGroup"">Batch Identifier</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[48].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">IDENTIFIERTYPETextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <dropDownItemsSelect xmlns=""COE.FormGroup"">SELECT ID as key, NAME as value FROM REGDB.VW_IDENTIFIERTYPE WHERE (TYPE ='B' OR TYPE='A') AND ACTIVE = 'T'</dropDownItemsSelect>
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                  <CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
                  <Enable xmlns=""COE.FormGroup"">True</Enable>
                  <ID xmlns=""COE.FormGroup"">IDENTIFIERTYPETextBox</ID>
                  <AutoPostBack xmlns=""COE.FormGroup"">False</AutoPostBack>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem fieldid=""1403"" id=""48"" tableid=""14"" xmlns=""COE.FormGroup"">
                <numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" xmlns=""COE.FormGroup""/>
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
";
        #endregion
        #region IDENTIFIERVALUE_FORM_ELEMENT
        private const string IDENTIFIERVALUE_FORM_ELEMENT = @"
<label xmlns=""COE.FormGroup"">Value</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[49].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">IdentifierValueTextBox</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig xmlns=""COE.FormGroup"">
                  <CSSClass xmlns=""COE.FormGroup"">FETextBox</CSSClass>
                  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem fieldid=""1402"" id=""49"" tableid=""14"" xmlns=""COE.FormGroup"">
                <textCriteria negate=""NO"" normalizedChemicalName=""NO"" hillFormula=""NO"" fullWordSearch=""NO"" caseSensitive=""NO"" trim=""NONE"" operator=""IN"" hint="""" defaultWildCardPosition=""NONE"" xmlns=""COE.FormGroup""/>
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
";
        #endregion

        private XmlDocument coeFormGroup4003 = null;

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            coeFormGroup4003 = PatcherUtility.GetCoeFormGroupById(forms, "4003");
            if (coeFormGroup4003 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeFormGroup 4003");
            }
            else
            {
                PatcherUtility.SetXmlNamespaceManager(coeFormGroup4003);
             
                UpgradeCoeForm0(ref errorsInPatch, messages);
                AddCoeForm4(ref errorsInPatch, messages);
                UpgradeCoeForm1(ref errorsInPatch, messages);
                UpgradeCoeForm2(ref errorsInPatch, messages);
            }

            if (!errorsInPatch)
                messages.Add("COE form group 4003 was successfully migrated");
            else
                messages.Add("Failed to migrate COE form group 4003");

            return messages;
        }

        private void UpgradeCoeForm0(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm0 = coeFormGroup4003.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']", PatcherUtility.XmlNamespaceManager);
            if (coeForm0 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 0");
            }
            else
            {
                XmlNode identifierTypeLabel = coeForm0.SelectSingleNode("./COE:layoutInfo/COE:formElement[@name='IDENTIFIERTYPE']/COE:label", PatcherUtility.XmlNamespaceManager);
                if (identifierTypeLabel == null)
                {
                    errorsInPatch = true;
                    messages.Add("ERROR: Couldn't find label node in IDENTIFIERTYPE element in coeForm 0");
                }
                else
                {
                    identifierTypeLabel.InnerXml = "Registry Identifier";
                }
            }
        }

        private void UpgradeCoeForm1(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm1 = coeFormGroup4003.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']", PatcherUtility.XmlNamespaceManager);
            if (coeForm1 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 1");
            }
            else //Coverity fix - CID 19423
            {
                XmlNode identifierTypeLabel = coeForm1.SelectSingleNode("./COE:layoutInfo/COE:formElement[@name='IDENTIFIERTYPE']/COE:label", PatcherUtility.XmlNamespaceManager);
                if (identifierTypeLabel == null)
                {
                    errorsInPatch = true;
                    messages.Add("ERROR: Couldn't find label node in IDENTIFIERTYPE element in coeForm 1");
                }
                else
                {
                    identifierTypeLabel.InnerXml = "Component Identifier";
                }
            }
        }

        private void AddCoeForm4(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm4 = coeFormGroup4003.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']", PatcherUtility.XmlNamespaceManager);
            if (coeForm4 != null)
            {
                messages.Add("coeForm 4 was already added");
            }
            else
            {
                XmlNode coeForm0 = coeFormGroup4003.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']", PatcherUtility.XmlNamespaceManager);
                if (coeForm0 == null)
                {
                    errorsInPatch = true;
                    messages.Add("ERROR: Couldn't find coeForm 0");
                }
                else
                {
                    coeForm4 = coeFormGroup4003.CreateElement("coeForm", "COE.FormGroup");
                    XmlAttribute idAttr = coeFormGroup4003.CreateAttribute("id");
                    idAttr.Value = "4";
                    coeForm4.Attributes.Append(idAttr);
                    XmlAttribute nameAttr = coeFormGroup4003.CreateAttribute("name");
                    nameAttr.Value = "VW_STRUCTURE";
                    coeForm4.Attributes.Append(nameAttr);

                    coeForm0.ParentNode.InsertAfter(coeForm4, coeForm0);

                    coeForm4.InnerXml = VW_STRUCTURE_NODE_INNER_XML;
                }
            }
        }

        private void UpgradeCoeForm2(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm2 = coeFormGroup4003.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']", PatcherUtility.XmlNamespaceManager);
            if (coeForm2 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 2");
            }
            else
            {
                XmlNode layoutInfoNode = coeForm2.SelectSingleNode("COE:layoutInfo", PatcherUtility.XmlNamespaceManager);

                PatcherUtility.InsertFormElementAfter(coeFormGroup4003,
                    layoutInfoNode,
                    "IDENTIFIERTYPE",
                    IDENTIFIERTYPE_FORM_ELEMENT,
                    null);
                PatcherUtility.InsertFormElementAfter(coeFormGroup4003,
                    layoutInfoNode,
                    "IdentifierValue",
                    IDENTIFIERVALUE_FORM_ELEMENT,
                    null);
            }
        }
    }
}
