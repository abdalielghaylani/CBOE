using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class CSBR164590 : BugFixBaseCommand
    {


        /// <summary>
        /// patcher for adding 4013, 4014 formBo's on editing properties at customise forms.
        /// </summary>
        public override List<string> Fix(
            List<XmlDocument> forms
            , List<XmlDocument> dataviews
            , List<XmlDocument> configurations
            , XmlDocument objectConfig
            , XmlDocument frameworkConfig
            )
        {

            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            XmlDocument regConfig = null;

            try
            {
                foreach (XmlDocument config in configurations)
                {
                    if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                    {
                        regConfig = new XmlDocument();
                        regConfig.LoadXml(config.OuterXml);
                        string customFromGroupsIdPath = "//add[@name='REGADMIN']/settings/add[@name='CustomRegFormGroupsIds']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(customFromGroupsIdPath);

                        if (xpathRootNode != null)
                        {
                            List<int> arrDataViewtableId = new List<int>();
                            string FromGroupsIds = xpathRootNode.Attributes["value"].Value;
                            if (!(FromGroupsIds.Contains("4013") && FromGroupsIds.Contains("4014")))
                            {
                                if (!FromGroupsIds.Contains("4013"))
                                    FromGroupsIds = FromGroupsIds + "|" + "4013";
                                if (!FromGroupsIds.Contains("4014"))
                                    FromGroupsIds = FromGroupsIds + "|" + "4014";
                                string[] customFromGroupsIds = FromGroupsIds.Split('|');
                                for (int i = 0; i < customFromGroupsIds.Length; i++)
                                    arrDataViewtableId.Add(Convert.ToInt32(customFromGroupsIds[i].Trim()));
                                arrDataViewtableId.Sort();
                                FromGroupsIds = string.Join("|", arrDataViewtableId);
                                xpathRootNode.Attributes["value"].Value = FromGroupsIds;
                            }
                            configurations[configurations.IndexOf(config)] = regConfig;
                            messages.Add("CustomRegFormGroupsIds node was added succesfully.");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("CustomRegFormGroupsIds node is not available to apply patch.");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                errorsInPatch = true;
                messages.Add(ex.Message);
            }
            finally
            {
                if (!errorsInPatch)
                {
                    messages.Add("CSBR164590 was successfully patched");
                }
                else
                    messages.Add("CSBR164590 patch was aborted due to errors");
            }
            return messages;
        }

        
    }
}
