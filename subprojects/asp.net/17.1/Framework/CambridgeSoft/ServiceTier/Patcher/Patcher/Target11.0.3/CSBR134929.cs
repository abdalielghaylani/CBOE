using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-134929: Salt Suffix Type “GetSaltDescription” is still available in Sequence Table
    /// (no version): Daniel Sahayam notes that 
    /// Steps to Reproduce:
    ///1. Go to Customize Registration -> Manage Customizable Table
    ///2. Select Sequences table from drop down
    ///3. Click Add and verify the availability of “GetSaltDescription” in Salt Suffix Type drop down
    ///
    ///Expected Result:
    ///As per CSBR-132532 “GetSaltDescription” should not be available
    ///
    ///Actual Result:
    ///“GetSaltDescription” is available in Salt Suffix Type drop down
    ///
    ///Note #1: This issue is not reproducible in COE11VMSC1, COE1103VM2, COE1103VM3 (all are clean installation)
    ///Note #2: Screen shot 134929.GIF attached.
    /// </summary>
    public class CSBR134929 : BugFixBaseCommand
    {
        /// <summary>
        /// - Manual steps to fix:
        /// - Open COEFrameworkConfig.xml file. 
        /// - Search coeConfiguration section -> applications -> REGISTRATION entry -> innerXml section -> SaltSuffixGenerators entry -> innerXmlData section. 
        /// - Remove entry for 'GetSaltDescription'.
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

            XmlNode getSaltDescriptionNode = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/innerXml/add[@name='SALTSUFFIXGENERATORS']/innerXmlData/add[@name='GetSaltDescription']");

            if (getSaltDescriptionNode != null)
            {
                getSaltDescriptionNode.ParentNode.RemoveChild(getSaltDescriptionNode);
                messages.Add("GetSaltDescription option removed.");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("GetSaltDescription option was not present - skipping.");
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR-133313 was successfully patched");
            }
            else
                messages.Add("CSBR-133313 was patched with errors");

            return messages;
        }
    }
}
