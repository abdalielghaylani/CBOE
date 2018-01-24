using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	/// <summary>
    /// Upgrade the CBVExcel link for 1230
    /// </summary>
    public class UpgradeCBVExcelLinkTo1230 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            XmlNode excelLinkNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Utilities']/links/add[@name='CBVExcel']");

            if (excelLinkNode == null)
            {
                errorInPatch = true;
                messages.Add("CBVExcel application is not configured in framework config!");
            }
            else
            {
                if (excelLinkNode.Attributes["url"].Value.ToLower() != "/cfserverasp/clients/chembiovizexcel addin/chembiovizexceladdin1230.exe")
                {
                    messages.Add("CBVExcel url was successfully updated");
                    excelLinkNode.Attributes["url"].Value = "/cfserverasp/Clients/ChemBioVizExcel Addin/ChemBioVizExcelAddin1230.exe";
                }
                else
                    messages.Add("CBVExcel url was already upgraded");
            }

            if (!errorInPatch)
            {
                messages.Add("CBVExcel link was successfully upgraded");
            }
            else
                messages.Add("CBVExcel link was not upgraded due to errors");

            return messages;
        }
    }
}
