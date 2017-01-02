using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Bug Description:
    /// 1.	Login as cssadmin
    /// 2.	Click on Manage Roles 
    /// 3.	Add a new role "TestRole" for COE (make sure "CSS_CHANGE_PASSWORD" should not be checked).
    /// 4.	Save the Role and then Click on “Done”. 
    /// 5.	Click on Manage Users.
    /// 6.	Add user “TestUser” and assign Role only "TestRole".
    /// 7.	Save the Role and then Click on “Done”. 
    /// 8.	Logoff.
    /// 9.	Login as TestUser.
    /// 
    /// BUG: Under the security section "Change Password" option is available even though the "CSS_CHANGE_PASSWORD" option is not selected for the “TestRole”
    /// 
    /// NOTE: If try to change the password able to change successfully.
    /// 
    /// Expected Result: When the option "CSS_CHANGE_PASSWORD” is not checked for the TestRole, the TestUser should not have privilege to Change Password.
    /// </summary>
	public class CSBR124363 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Open coeframeworkconfig.xml look for &lt;add name="ChangePassword" and set the privilege attribute to CSS_CHANGE_PASSWORD as in privilege="CSS_CHANGE_PASSWORD"
        /// Two entries should've been found.
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
            XmlNodeList changePasswordNodes = frameworkConfig.SelectNodes("//add[@name='ChangePassword']");
            if (changePasswordNodes.Count == 0)
            {
                messages.Add("COEFrameworkConfig.xml did not have a change password entry");
                errorsInPatch = true;
            }
            else
            {
                foreach (XmlNode changePasswordEntry in changePasswordNodes)
                {
                    XmlAttribute privilege = changePasswordEntry.Attributes["privilege"];
                    if (privilege == null)
                    {
                        privilege = frameworkConfig.CreateAttribute("privilege");
                        changePasswordEntry.Attributes.Append(privilege);
                    }
                    if (privilege.Value.Contains("CSS_CHANGE_PASSWORD"))
                    {
                        errorsInPatch = true;
                        messages.Add("ChangePassword privilege was already added");
                    }
                    else
                    {
                        privilege.Value = "CSS_CHANGE_PASSWORD";
                        messages.Add("ChangePassword privilege added successfully");
                    }
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR124363 was successfully patched");
            else
                messages.Add("CSBR124363 was patched with errors");
            return messages;
        }
    }
}
