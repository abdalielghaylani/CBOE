using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace CambridgeSoft.COE.Patcher
{
    class CBOE1493 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeAddInPath = string.Empty;
            _coeAddInPath = "MultiCompoundRegistryRecord/AddIns"; // Path to check the Rootnode before patcher update.
            XmlNode rootNode;
            #region AddIns
            rootNode = objectConfig.SelectSingleNode(_coeAddInPath);
            XmlNode chemDrawAddInNode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.ChemDrawChemicalWarningCheckerAddIn'][@friendlyName='ChemDraw Chemical Warning Checker']");
            if (chemDrawAddInNode == null)
            {

                chemDrawAddInNode = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AddIn", null);

                createNewAttribute("assembly", "CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc", ref chemDrawAddInNode);
                createNewAttribute("class", "CambridgeSoft.COE.Registration.Services.RegistrationAddins.ChemDrawChemicalWarningCheckerAddIn", ref chemDrawAddInNode);
                createNewAttribute("friendlyName", "ChemDraw Chemical Warning Checker", ref chemDrawAddInNode);
                createNewAttribute("required", "no", ref chemDrawAddInNode);
                createNewAttribute("enabled", "no", ref chemDrawAddInNode);

                XmlNode eventNode = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Event", null);
                createNewAttribute("eventName", "Inserting", ref eventNode);
                createNewAttribute("eventHandler", "OnSubmitHandler", ref eventNode);
                chemDrawAddInNode.AppendChild(eventNode);

                eventNode = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Event", null);
                createNewAttribute("eventName", "Updating", ref eventNode);
                createNewAttribute("eventHandler", "OnSubmitHandler", ref eventNode);
                chemDrawAddInNode.AppendChild(eventNode);

                chemDrawAddInNode.AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AddInConfiguration", null));

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "SubmitButton", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/SubmitButton").InnerText = "true";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "RegisterButton", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/RegisterButton").InnerText = "true";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ValenceChargeErrors", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/ValenceChargeErrors").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "UnbalancedParantheses", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/UnbalancedParantheses").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "InvalidIsotopes", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/InvalidIsotopes").InnerText = "true";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "IsolatedBonds", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/IsolatedBonds").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AtomsNearOtherBonds", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/AtomsNearOtherBonds").InnerText = "true";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "UnspecifiedStereochemistry", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/UnspecifiedStereochemistry").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AmbiguousStereochemistry", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/AmbiguousStereochemistry").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "StereobondChiralAtoms", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/StereobondChiralAtoms").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "LinearAtoms", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/LinearAtoms").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "MiscWarning", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/MiscWarning").InnerText = "true";


                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "DataLoader_ChemDrawWarningChecker", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/DataLoader_ChemDrawWarningChecker").InnerText = "false";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "DataLoader2_ChemDrawWarningChecker", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/DataLoader2_ChemDrawWarningChecker").InnerText = "false";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "InvLoader_ChemDrawWarningChecker", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/InvLoader_ChemDrawWarningChecker").InnerText = "false";

                chemDrawAddInNode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ENoteBook_ChemDrawWarningChecker", null));
                chemDrawAddInNode.SelectSingleNode("AddInConfiguration/ENoteBook_ChemDrawWarningChecker").InnerText = "false";


                rootNode.AppendChild(chemDrawAddInNode);
               
                messages.Add("ChemDraw Chemical Warning Checker AddIn node was added succesfully.");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("ChemDraw Chemical Warning Checker AddIn already exist.");
            }


            #endregion

            if (!errorsInPatch)
            {
                messages.Add("COEObjectConfig was added successfully.");
            }
            else
                messages.Add("COEObjectConfig was not patched due to errors.");

            return messages;
        }
        #region Private Method

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

        #endregion
    }
}
