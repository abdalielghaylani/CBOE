using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-132990: "Regression: A user with single privilege can perform unassigned roles also"
    /// 
    /// STEPS:
    /// 1. Login as cssadmin
    /// 2. Click on Manage Role
    /// 3. Add a new role having only privilege to edit user under COE
    /// 4. create  a new user with this role(only privilege to edit a user and change password)
    /// 5.login with the new user
    /// 6)go to coe manager
    /// 7)Click on add user
    /// </summary>
	class CSBR132990 : BugFixBaseCommand
	{
        /// <summary>
        /// Along with the code changes it is also required to update the coeframeworkconfig.xml to reflect that the CSS_CREATE_USER and 
        /// CSS_DELETE_USER should also make the home buttons to be shown.
        /// 
        /// Manual Steps to fix:
        /// - Open framework config and look for &lt;add name="ManageUsers"
        /// - Add to the privilege attributes the folowing privileges: CSS_DELETE_USER and CSS_CREATE_USER
        /// - Search for &lt; name="Home" under COE Application and add CSS_DELETE_USER and CSS_CREATE_USER as privileges.
        /// 
        /// IE: If there was only CSS_EDIT_USER, the result would be privilege="CSS_EDIT_USER||CSS_DELETE_USER||CSS_CREATE_USER"
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

            XmlNodeList manageUsersNodeList = frameworkConfig.SelectNodes("//add[@name='ManageUsers']/@privilege");

            if (manageUsersNodeList == null || manageUsersNodeList.Count == 0)
            {
                errorsInPatch = true;
                messages.Add("Manage Users was not present in coeframeworkconfig.xml");
            }
            else
            {
                foreach (XmlNode privs in manageUsersNodeList)
                {
                    if (!string.IsNullOrEmpty(privs.Value))
                    {
                        if (!privs.Value.Contains("CSS_DELETE_USER"))
                        {
                            privs.Value += "||CSS_DELETE_USER";
                            messages.Add("CSS_DELETE_USER privilege successfully added to ManageUsers");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("CSS_DELETE_USER privilege was already present in ManageUsers");
                        }

                        if (!privs.Value.Contains("CSS_CREATE_USER"))
                        {
                            privs.Value += "||CSS_CREATE_USER";
                            messages.Add("CSS_CREATE_USER privilege successfully added to ManageUsers");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("CSS_CREATE_USER privilege was already present in ManageUsers");
                        }
                    }
                    else
                    {
                        privs.Value = "CSS_EDIT_USER||CSS_DELETE_USER||CSS_CREATE_USER";
                        messages.Add("Privilege was empty and was updated to have CSS_EDIT_USER||CSS_DELETE_USER||CSS_CREATE_USER in ManageUsers");
                    }
                }
            }

            XmlNode homeLink = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COE']/links/add[@name='Home']/@privilege");
            if (homeLink == null)
            {
                errorsInPatch = true;
                messages.Add("Home was not present for COE application under coehomesettings in FW Config.");
            }
            else
            {
                if (!string.IsNullOrEmpty(homeLink.Value))
                {
                    if (!homeLink.Value.Contains("CSS_DELETE_USER"))
                    {
                        homeLink.Value += "||CSS_DELETE_USER";
                        messages.Add("CSS_DELETE_USER privilege successfully added to Home");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("CSS_DELETE_USER privilege was already present in Home");
                    }

                    if (!homeLink.Value.Contains("CSS_CREATE_USER"))
                    {
                        homeLink.Value += "||CSS_CREATE_USER";
                        messages.Add("CSS_CREATE_USER privilege successfully added to Home");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("CSS_CREATE_USER privilege was already present in Home");
                    }
                }
                else
                {
                    homeLink.Value = "CSS_EDIT_USER||CSS_DELETE_USER||CSS_CREATE_USER";
                    messages.Add("Privilege was empty and was updated to have CSS_EDIT_USER||CSS_DELETE_USER||CSS_CREATE_USER in Home");
                }
            }
            
            if (!errorsInPatch)
                messages.Add("CSBR132990 was successfully patched");
            else
                messages.Add("CSBR132990 was patched with errors");
            return messages;
        }
    }
}
