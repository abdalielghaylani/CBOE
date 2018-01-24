using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  Updating the difference Between 1210 to 1250 DEV line 
    /// </summary>
    class EN3309 : BugFixBaseCommand
    {

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coePropertyPath = string.Empty;
            string _coeAddInPath = string.Empty;
            _coePropertyPath = "MultiCompoundRegistryRecord/BatchList/Batch/PropertyList"; // Path to check the Rootnode before patcher update.

            XmlNode rootPropertlistNode;

            #region AddIns
            rootPropertlistNode = objectConfig.SelectSingleNode(_coePropertyPath);

            if (_coePropertyPath != null)
            {
                XmlNode amountNode = rootPropertlistNode.SelectSingleNode("Property[@name='AMOUNT_UNITS']");
               
                if (amountNode != null)
                {
                    if (amountNode.Attributes["pickListDisplayValue"] == null)
                    {
                        createNewAttribute("pickListDisplayValue", "", ref amountNode);
                        messages.Add("pickListDisplayValue attribute added to AMOUNT_UNITS property successfully.");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("pickListDisplayValue attribute is already present in AMOUNT_UNITS property.");
                    }
                }
                else
                {
                    errorsInPatch =true;
                }
            }

            #endregion

            if (!errorsInPatch)
            {
                messages.Add("COEObjectConfig was updated succesfully");
            }
            else
                messages.Add("COEObjectConfig  was patched with errors");

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
