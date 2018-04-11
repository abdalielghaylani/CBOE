using System;
using System.Collections.Generic;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR134630 - Enhancement: Search by Prefix is not possible for Temp table
    /// </summary>
	public class CSBR134630 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument form in forms)
            {
                string id = form.DocumentElement.Attributes["id"] == null ? string.Empty : form.DocumentElement.Attributes["id"].Value;
                
                if (id == "4002")
                {
                    XmlNamespaceManager nm = new XmlNamespaceManager(form.NameTable);
                    nm.AddNamespace("COE", "COE.FormGroup");

                    XmlNode layoutNode = form.SelectSingleNode(@"COE:formGroup/COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", nm);

                    XmlNode prefixNode = layoutNode.SelectSingleNode("COE:formElement[@name='PREFIX']", nm);
                    if (prefixNode != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Prefix element has already been added");
                    }
                    else
                    {
                        XmlNode registryprojectNode = layoutNode.SelectSingleNode("COE:formElement[@name='REGISTRY_PROJECT']", nm);
                        XmlNode newPrefixNode = form.CreateNode(XmlNodeType.Element, "formElement", form.DocumentElement.NamespaceURI);
                        XmlAttribute nameRP = form.CreateAttribute("name");
                        nameRP.Value = "PREFIX";
                        newPrefixNode.Attributes.Append(nameRP);

                        int searchCriteriaId = int.Parse(form.SelectNodes("//COE:searchCriteriaItem[not(@id <= preceding::COE:searchCriteriaItem/@id) and not(@id <=following::COE:searchCriteriaItem/@id)]", nm)[0].Attributes["id"].Value) + 1;

                        newPrefixNode.InnerXml = string.Format(@"
              <label xmlns=""COE.FormGroup"">Prefix</label><showHelp xmlns=""COE.FormGroup"">false</showHelp><isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload><pageComunicationProvider xmlns=""COE.FormGroup"" /><fileUploadBindingExpression xmlns=""COE.FormGroup"" /><helpText xmlns=""COE.FormGroup"" /><defaultValue xmlns=""COE.FormGroup"" /><bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[{0}].Criterium.Value</bindingExpression><Id xmlns=""COE.FormGroup"">PREFIXTextBox</Id><displayInfo xmlns=""COE.FormGroup""><cssClass>Std25x40</cssClass><type>
                CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type><visible>true</visible></displayInfo><validationRuleList xmlns=""COE.FormGroup"" /><serverEvents xmlns=""COE.FormGroup"" /><clientEvents xmlns=""COE.FormGroup"" /><configInfo xmlns=""COE.FormGroup""><fieldConfig><dropDownItemsSelect>SELECT S.SEQUENCEID AS KEY, S.PREFIX AS VALUE FROM REGDB.VW_SEQUENCE S WHERE (TYPE = 'R' OR TYPE = 'A') ORDER BY S.PREFIX ASC</dropDownItemsSelect><CSSLabelClass>FELabel</CSSLabelClass><CSSClass>FEDropDownList</CSSClass><Enable>True</Enable><ID>PrefixTextBox</ID><AutoPostBack>False</AutoPostBack></fieldConfig></configInfo><dataSource xmlns=""COE.FormGroup"" /><dataSourceId xmlns=""COE.FormGroup"" /><requiredStyle xmlns=""COE.FormGroup"" /><searchCriteriaItem fieldid=""102"" id=""{0}"" tableid=""1"" searchLookupByID=""true"" xmlns=""COE.FormGroup""><numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" /></searchCriteriaItem><displayData xmlns=""COE.FormGroup"" />
            ", searchCriteriaId);

                        layoutNode.InsertBefore(newPrefixNode, registryprojectNode);
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR134630 was successfully patched");
            }
            else
                messages.Add("CSBR134630 was patched with errors");

            return messages;
        }
    }
}
