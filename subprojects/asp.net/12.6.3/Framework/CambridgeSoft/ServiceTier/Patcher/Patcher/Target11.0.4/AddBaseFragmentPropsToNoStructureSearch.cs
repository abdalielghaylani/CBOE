using System.Xml;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// To test from Visual Studio:
    /// (1) In PatchLists/11.0.4.txt, prepend // to each line EXCEPT AddBaseFragmentPropsToNoStructureSearch
    /// (2) Provide this as the command-line arguments:
    /// -i ORCL -u system -p manager2 -l .\PatchLists\11.0.4.txt
    /// (3) Run the patch in debug mode
    /// (4) REMOVE the // prefix from each line in PatchLists/11.0.4.txt
    /// </summary>
    class AddBaseFragmentPropsToNoStructureSearch : BugFixBaseCommand
	{
        public override List<string> Fix(
            List<System.Xml.XmlDocument> forms
            , List<System.Xml.XmlDocument> dataviews
            , List<System.Xml.XmlDocument> configurations
            , System.Xml.XmlDocument objectConfig
            , System.Xml.XmlDocument frameworkConfig
            )
        {
            List<string> messages = new List<string>();
            bool patchApplied = false;

            //get the Registration configuration
            XmlDocument regConfig = null;
            foreach (XmlDocument config in configurations)
            {
                if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                {
                    regConfig = config;
                    break;
                }
            }

            if (regConfig != null)
            {
                //pull the 'enhanced' duplicate-checking node
                string xpath = "//add[@name='ENHANCED_DUPLICATE_SCAN']/settings/add[@name='Level']";
                XmlNode addNode = regConfig.SelectSingleNode(xpath);
                if (addNode != null)
                {
                    XmlAttribute allowedVals = addNode.Attributes["allowedValues"];
                    //apply the new value to the setting
                    if (allowedVals != null)
                    {
                        allowedVals.Value = "Component|Structure";
                        patchApplied = true;
                    }
                    else
                        messages.Add("Unable to find configuration attribute matching 'allowedValues'");
                }
                else
                {
                    messages.Add("Unable to find configuration node matching 'ENHANCED_DUPLICATE_SCAN'");
                }
            }
            else
            {
                messages.Add("Unable to find configuration matching 'Reg App Settings'");
            }

            if (patchApplied)
                messages.Add("AddBaseFragmentPropsToNoStructureSearch was successfully patched.");
            else
                messages.Add("AddBaseFragmentPropsToNoStructureSearch was not patched.");

            return messages;

        }
	}
}
