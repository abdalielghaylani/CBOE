using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-125166: 
    /// Export SDF: Add configuration option to dictate the version of mol file format used (V2000 vs V3000)
    /// Option for Setting the File Format to be exported 
    /// 
    ///Expected Result:
    ///As per CSBR-125166 molFileFormat attribute should be added to CSORACLECARTRIDGE under sqlGeneratorData
    ///
    ///Actual Result:
    ///There was no attribute to retrive the molFileFormat for Exporting
    ///
    /// </summary>
	public class CSBR125166 : BugFixBaseCommand
	{
        /// <summary>
        /// - Manual steps to fix:
        /// - Open COEFrameworkConfig.xml file. 
        /// - Search coeConfiguration section -> dbmsTypes -> ORACLE entry -> sqlGeneratorData -> CSORACLECARTRIDGE entry. 
        /// - Add entry molFileFormat="V3000".
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode molFileFormatNode = frameworkConfig.SelectSingleNode("//coeConfiguration/dbmsTypes/add[@name='ORACLE']/sqlGeneratorData/add[@name='CSORACLECARTRIDGE']");            
            if (molFileFormatNode.Attributes["molFileFormat"] == null)
            {
                XmlAttribute molFileFormatAttribute = molFileFormatNode.OwnerDocument.CreateAttribute("molFileFormat");
                molFileFormatAttribute.Value = "V2000";
                molFileFormatNode.Attributes.Append(molFileFormatAttribute);
                messages.Add("molFileFormat option is Added.");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("molFileFormat option is already present - skipping.");
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR-125166 was successfully patched");
            }
            else
                messages.Add("CSBR-125166 was patched with errors");

            return messages;
        }
	}
}
