using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Migrate COEFormGroup 4006 ("ELN - Search Perm form")
    /// </summary>
    public class MigCOEFormGroup4006 : BugFixBaseCommand
    {
        /// <summary>
        /// 1. add a new coeForm (<coeForm id="4" name="VW_STRUCTURE">)with a STRUCT_COMMENTS formElement to queryForm
        /// </summary>
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

                if (id == "4006")
                {
                    string originalForm_OuterXml = doc.OuterXml;
                    try
                    {
                        XmlNode VW_STRUCTURE_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:queryForms/COE:queryForm/COE:coeForms/COE:coeForm[@name='VW_STRUCTURE']", manager);
                        if (VW_STRUCTURE_COEForm != null)
                        {
                            messages.Add("coeForm VW_STRUCTURE already exists in queryForm of COEFormGroup 4006");
                            break; //exit if the coeForm to be added already exists
                        }

                        VW_STRUCTURE_COEForm = doc.CreateElement("coeForm", "COE.FormGroup");
                        XmlAttribute id_Attribute = doc.CreateAttribute("id");
                        XmlAttribute name_Attribute = doc.CreateAttribute("name");
                        id_Attribute.Value = "4";
                        name_Attribute.Value = "VW_STRUCTURE";
                        VW_STRUCTURE_COEForm.Attributes.Append(id_Attribute);
                        VW_STRUCTURE_COEForm.Attributes.Append(name_Attribute);

                        XmlNode queryForm_COEForms = doc.SelectSingleNode("/COE:formGroup/COE:queryForms/COE:queryForm/COE:coeForms", manager);
                        XmlNode VW_MIXTURE_REGNUMBER_COEForm = doc.SelectSingleNode("/COE:formGroup/COE:queryForms/COE:queryForm/COE:coeForms/COE:coeForm[@name='VW_MIXTURE_REGNUMBER']", manager);
                        if (VW_MIXTURE_REGNUMBER_COEForm != null)
                        {
                            queryForm_COEForms.InsertAfter(VW_STRUCTURE_COEForm, VW_MIXTURE_REGNUMBER_COEForm);
                        }
                        else
                        {
                            queryForm_COEForms.AppendChild(VW_STRUCTURE_COEForm);
                        }
                        VW_STRUCTURE_COEForm.InnerXml = @"<validationRuleList xmlns=""COE.FormGroup""/>
			                  <title xmlns=""COE.FormGroup"">Structure Information</title>
			                  <titleCssClass xmlns=""COE.FormGroup"">COEFormTitle</titleCssClass>
			                  <layoutInfo xmlns=""COE.FormGroup"">
			                    <formElement name=""STRUCT_COMMENTS"" xmlns=""COE.FormGroup"">
			                      <label xmlns=""COE.FormGroup"">Structure Comments</label>
			                      <showHelp xmlns=""COE.FormGroup"">false</showHelp>
			                      <helpText xmlns=""COE.FormGroup""/>
			                      <defaultValue xmlns=""COE.FormGroup""/>
			                      <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[38].Criterium.Value</bindingExpression>
			                      <Id xmlns=""COE.FormGroup"">StructureCommentsTextBox</Id>
			                      <displayInfo xmlns=""COE.FormGroup"">
			                        <cssClass xmlns=""COE.FormGroup"">Std100x80</cssClass>
			                        <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea</type>
			                        <visible xmlns=""COE.FormGroup"">true</visible>
			                      </displayInfo>
			                      <validationRuleList xmlns=""COE.FormGroup""/>
			                      <serverEvents xmlns=""COE.FormGroup""/>
			                      <clientEvents xmlns=""COE.FormGroup""/>
			                      <configInfo xmlns=""COE.FormGroup"">
			                        <fieldConfig xmlns=""COE.FormGroup"">
			                          <TextMode xmlns=""COE.FormGroup"">MultiLine</TextMode>
			                          <CSSClass xmlns=""COE.FormGroup"">FETextArea</CSSClass>
			                          <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
			                        </fieldConfig>
			                      </configInfo>
			                      <dataSource xmlns=""COE.FormGroup""/>
			                      <dataSourceId xmlns=""COE.FormGroup""/>
			                      <!--havn't figured out how is the ""field"" works, just keep this element like this for display currently-->
			                      <searchCriteriaItem fieldid=""106"" id=""38"" tableid=""1"" xmlns=""COE.FormGroup"">
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
                        messages.Add("add a new coeForm VW_STRUCTURE to queryForm of COEFormGroup 4006");

                    }
                    catch (Exception e)
                    {
                        errorsInPatch = true;
                        messages.Add(string.Format("Exception occurs during patching: {0}",e.Message));
                    }

                    if (errorsInPatch)//restore form
                    {
                        messages.Add("Rollback changes to COEFormGroup 4006");
                        doc.LoadXml(originalForm_OuterXml);
                    }
                }
            }

            if (errorsInPatch)
                messages.Add("Fail to migrate COEFormGroup 4006");
            else
                messages.Add("Succeed to migrate COEFormGroup 4006");

            return messages;
        }
    }
}
