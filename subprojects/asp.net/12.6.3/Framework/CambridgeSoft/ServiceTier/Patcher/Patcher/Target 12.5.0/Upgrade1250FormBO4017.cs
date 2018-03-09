using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class Upgrade1250FormBO4017 : BugFixBaseCommand
	{
        #region Variables
        XmlNode _parentNode;
        XmlNode _newNode;
        string _newElementAttributes = string.Empty;
        string _nameSpaceURI = string.Empty;
        const string PREFIX = "COE:";
      
        #endregion


        #region Property
        

        #endregion



        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeFormPath = string.Empty;

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                #region 4017.xml
                if (id == "4017")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath ="//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']";
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);

                    if (rootSelectedNode != null)
                    {
                        XmlNode cssClassNode = rootSelectedNode.SelectSingleNode("COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:columnStyle", manager);

                        #region cssClassNode
                        if (cssClassNode != null)
                        {
                            cssClassNode.InnerText = "color: #000000; border-color: #CFD8E6; border-style:solid; border-width:1px; font-size:10px;font-family: Verdana;vertical-align:middle;text-align:center;";
                            messages.Add("Form[" + id + "] columnStyle updated for table");
                        }
                        else
                        {
                            messages.Add("Form[" + id + "]:columnStyle node was not found.");
                        }
                        #endregion

                        #region RegNumHRef
                        XmlNodeList RegNumhref = rootSelectedNode.SelectNodes("COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns/COE:Column[@name='REG_NUMBER']", manager);
                        if (RegNumhref != null)
                        {

                            foreach (XmlNode RegnumNode in RegNumhref)
                            {
                                XmlNode headerText = RegnumNode.SelectSingleNode("COE:headerText", manager);
                                if (!string.IsNullOrEmpty(headerText.InnerText) && headerText.InnerText.Trim() == "Add Component")
                                {
                                    XmlNode HRef = RegnumNode.SelectSingleNode("COE:formElement/COE:configInfo/COE:fieldConfig/COE:HRef", manager);
                                    if (HRef !=null)
                                    {
                                        HRef.InnerText = "/COERegistration/Forms/ReviewRegister/ContentArea/ReviewRegisterMixture.aspx?RegisteredRegId={0}&amp;RegisteredCompoundId={0}"; 
                                    }
                                }

                            }
                        }

                        #endregion

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:The formElement tables-COE:table-COE:columnStyle does not exist");
                    }
                    break;
                }
                #endregion

            }
            #endregion
            if (!errorsInPatch)
                messages.Add("Upgrade1250FormBO4017 was successfully fixed.");
            else
                messages.Add("Upgrade1250FormBO4017 was fixed with errors.");
            return messages;
        }

	}
}
