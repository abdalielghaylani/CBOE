using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;

namespace CambridgeSoft.COE.Framework.UnitTests.Helpers
{
    public static class SecurityHelper
    {

        #region Variables

        public static int intRoleId = 0;
        public static string strRoleName = string.Empty;

        public static int intAppId = 0;
        public static string strAppName = string.Empty;

        public static object[] argList = null;

        public static int personID = 0, supervisorID = 0;
        public static string userCode = string.Empty, userID = string.Empty;
        public static string title = string.Empty;
        public static string firstName = string.Empty, middleName = string.Empty, lastName = string.Empty;
        public static int siteID = 0;
        public static string department = string.Empty, telephone = string.Empty, email = string.Empty, address = string.Empty;
        public static bool active;

        public static int intPersonId = 0;
        public static string strUserID = string.Empty;

        public static int intGroupId = 0, intGroupOrgId = 0, intParentGroupId = 0, intLeaderPersonId = 0;
        public static string strGroupName = string.Empty;        

        public static int intObjectTypeId = 0;

        public static int intPrivId = 0;
        public static string strPrivName = string.Empty;

        public static string strPrivilegeTableName = string.Empty;

        #endregion Variables

        #region Methods

        public static string GenerateRandomNumber()
        {
            string miliseconds = DateTime.Now.Millisecond.ToString();
            int length = miliseconds.Length;
            while (length < 3)
            {
                miliseconds = miliseconds.Insert(0, "0");
                length++;
            }
            return miliseconds.Substring(length - 3, 3);
        }

        public static bool GetRoleDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ROLE_ID, ROLE_NAME FROM SECURITY_ROLES ORDER BY ROLE_ID DESC");
            DataTable dtRoles = DALHelper.ExecuteQuery(query.ToString());
            intRoleId = 0;
            strRoleName = string.Empty;

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                intRoleId = Convert.ToInt32(dtRoles.Rows[0]["ROLE_ID"]);
                strRoleName = Convert.ToString(dtRoles.Rows[0]["ROLE_NAME"]);
                return true;
            }
            return false;
        }   

        public static List<string> GetRoleNameList()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ROLE_NAME FROM SECURITY_ROLES ORDER BY ROLE_NAME"); // WHERE rownum <= 5
            DataTable dtRoles = DALHelper.ExecuteQuery(query.ToString());
            List<string> lstRoleName = new List<string>();
            lstRoleName.Clear();

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                for (int i = 0; i < dtRoles.Rows.Count; i++)
                {
                    lstRoleName.Add(Convert.ToString(dtRoles.Rows[i]["ROLE_NAME"]));
                }
            }
            return lstRoleName;
        }

        public static List<string> GetApplicationList()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT DISTINCT COEIDENTIFIER FROM SECURITY_ROLES ORDER BY COEIDENTIFIER");
            DataTable dtApp = DALHelper.ExecuteQuery(query.ToString());
            List<string> lstApp = new List<string>();
            lstApp.Clear();

            if (dtApp != null && dtApp.Rows.Count > 0)
            {
                for (int i = 0; i < dtApp.Rows.Count; i++)
                {
                    lstApp.Add(Convert.ToString(dtApp.Rows[i]["COEIDENTIFIER"]));
                }
            }
            return lstApp;
        }

        public static List<int> GetRoleIdList()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ROLE_ID FROM SECURITY_ROLES ORDER BY ROLE_ID");
            DataTable dtRoles = DALHelper.ExecuteQuery(query.ToString());
            List<int> lstRoleId = new List<int>();
            lstRoleId.Clear();

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                for (int i = 0; i < dtRoles.Rows.Count; i++)
                {
                    lstRoleId.Add(Convert.ToInt32(dtRoles.Rows[i]["ROLE_ID"]));
                }
            }
            return lstRoleId;
        }

        public static bool GetApplicationDetails()
        {

            StringBuilder query = new StringBuilder();
            query.Append("SELECT RID, COEIDENTIFIER FROM SECURITY_ROLES ORDER BY RID");
            DataTable dtRoles = DALHelper.ExecuteQuery(query.ToString());
            intAppId = 0;
            strAppName = string.Empty;

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                intAppId = Convert.ToInt32(dtRoles.Rows[0]["RID"]);
                strAppName = Convert.ToString(dtRoles.Rows[0]["COEIDENTIFIER"]);

                argList = new object[2];
                argList[0] = intAppId;
                argList[1] = strAppName;

                return true;
            }
            return false;
        }

        public static bool GetPeopleDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT PERSON_ID, USER_CODE, USER_ID, SUPERVISOR_INTERNAL_ID, TITLE, FIRST_NAME, MIDDLE_NAME, LAST_NAME, SITE_ID, DEPARTMENT, ");
            query.Append("TELEPHONE, EMAIL, INT_ADDRESS, ACTIVE FROM PEOPLE ORDER BY PERSON_ID");
            DataTable dtPerson = DALHelper.ExecuteQuery(query.ToString());

            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                if (dtPerson.Rows[0]["PERSON_ID"] != null && dtPerson.Rows[0]["PERSON_ID"] != DBNull.Value)
                    personID = Convert.ToInt32(dtPerson.Rows[0]["PERSON_ID"]);
                if (dtPerson.Rows[0]["USER_CODE"] != null && dtPerson.Rows[0]["USER_CODE"] != DBNull.Value)
                    userCode = Convert.ToString(dtPerson.Rows[0]["USER_CODE"]);
                if (dtPerson.Rows[0]["USER_ID"] != null && dtPerson.Rows[0]["USER_ID"] != DBNull.Value)
                    userID = Convert.ToString(dtPerson.Rows[0]["USER_ID"]);
                if (dtPerson.Rows[0]["SUPERVISOR_INTERNAL_ID"] != null && dtPerson.Rows[0]["SUPERVISOR_INTERNAL_ID"] != DBNull.Value)
                    supervisorID = Convert.ToInt32(dtPerson.Rows[0]["SUPERVISOR_INTERNAL_ID"]);
                if (dtPerson.Rows[0]["TITLE"] != null && dtPerson.Rows[0]["TITLE"] != DBNull.Value)
                    title = Convert.ToString(dtPerson.Rows[0]["TITLE"]);
                if (dtPerson.Rows[0]["FIRST_NAME"] != null && dtPerson.Rows[0]["FIRST_NAME"] != DBNull.Value)
                    firstName = Convert.ToString(dtPerson.Rows[0]["FIRST_NAME"]);
                if (dtPerson.Rows[0]["MIDDLE_NAME"] != null && dtPerson.Rows[0]["MIDDLE_NAME"] != DBNull.Value)
                    middleName = Convert.ToString(dtPerson.Rows[0]["MIDDLE_NAME"]);
                if (dtPerson.Rows[0]["LAST_NAME"] != null && dtPerson.Rows[0]["LAST_NAME"] != DBNull.Value)
                    lastName = Convert.ToString(dtPerson.Rows[0]["LAST_NAME"]);
                if (dtPerson.Rows[0]["SITE_ID"] != null && dtPerson.Rows[0]["SITE_ID"] != DBNull.Value)
                    siteID = Convert.ToInt32(dtPerson.Rows[0]["SITE_ID"]);
                if (dtPerson.Rows[0]["DEPARTMENT"] != null && dtPerson.Rows[0]["DEPARTMENT"] != DBNull.Value)
                    department = Convert.ToString(dtPerson.Rows[0]["DEPARTMENT"]);
                if (dtPerson.Rows[0]["TELEPHONE"] != null && dtPerson.Rows[0]["TELEPHONE"] != DBNull.Value)
                    telephone = Convert.ToString(dtPerson.Rows[0]["TELEPHONE"]);
                if (dtPerson.Rows[0]["EMAIL"] != null && dtPerson.Rows[0]["EMAIL"] != DBNull.Value)
                    email = Convert.ToString(dtPerson.Rows[0]["EMAIL"]);
                if (dtPerson.Rows[0]["INT_ADDRESS"] != null && dtPerson.Rows[0]["INT_ADDRESS"] != DBNull.Value)
                    address = Convert.ToString(dtPerson.Rows[0]["INT_ADDRESS"]);
                if (dtPerson.Rows[0]["ACTIVE"] != null && dtPerson.Rows[0]["ACTIVE"] != DBNull.Value)
                    active = Convert.ToBoolean(dtPerson.Rows[0]["ACTIVE"]);

                argList = new object[13];
                argList[0] = Convert.ToInt32(personID);
                argList[1] = Convert.ToString(userCode);
                argList[2] = Convert.ToString(userID);
                argList[3] = Convert.ToInt32(supervisorID);
                argList[4] = Convert.ToString(title);
                argList[5] = Convert.ToString(firstName);
                argList[6] = Convert.ToString(middleName);
                argList[7] = Convert.ToString(lastName);
                argList[8] = Convert.ToInt32(siteID);
                argList[9] = Convert.ToString(department);
                argList[10] = Convert.ToString(telephone);
                argList[11] = Convert.ToString(email);
                argList[12] = Convert.ToBoolean(active);

                return true;
            }
            return false;
        }

        public static bool GetUserDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT PERSON_ID, USER_ID FROM PEOPLE ORDER BY PERSON_ID DESC");
            DataTable dtPerson = DALHelper.ExecuteQuery(query.ToString());
            intPersonId = 0;
            strUserID = string.Empty;

            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                intPersonId = Convert.ToInt32(dtPerson.Rows[0]["PERSON_ID"]);
                strUserID = Convert.ToString(dtPerson.Rows[0]["USER_ID"]);
                return true;
            }
            return false;
        }

        public static List<int> GetPersonIdList()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT PERSON_ID FROM PEOPLE ORDER BY PERSON_ID");
            DataTable dtPerson = DALHelper.ExecuteQuery(query.ToString());
            List<int> lstPersonId = new List<int>();
            lstPersonId.Clear();

            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                for (int i = 0; i < dtPerson.Rows.Count; i++)
                {
                    lstPersonId.Add(Convert.ToInt32(dtPerson.Rows[i]["PERSON_ID"]));
                }
            }
            return lstPersonId;
        }

        public static List<string> GetPersonNameList()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT USER_ID FROM PEOPLE ORDER BY USER_ID");
            DataTable dtPerson = DALHelper.ExecuteQuery(query.ToString());
            List<string> lstPersonName = new List<string>();
            lstPersonName.Clear();

            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                for (int i = 0; i < dtPerson.Rows.Count; i++)
                {
                    lstPersonName.Add(Convert.ToString(dtPerson.Rows[i]["USER_ID"]));
                }
            }
            return lstPersonName;
        }

        public static bool GetGroupPeopleDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT GROUP_ID, PERSON_ID FROM COEGROUPPEOPLE");
            DataTable dtGroupPeople = DALHelper.ExecuteQuery(query.ToString());
            intPersonId = 0;
            intGroupId  = 0;

            if (dtGroupPeople != null && dtGroupPeople.Rows.Count > 0)
            {
                intPersonId = Convert.ToInt32(dtGroupPeople.Rows[0]["PERSON_ID"]);
                intGroupId = Convert.ToInt32(dtGroupPeople.Rows[0]["GROUP_ID"]);
                return true;
            }
            return false;
        }

        public static bool GetGroupDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT GROUP_ID FROM COEGROUP");
            DataTable dtGroup = DALHelper.ExecuteQuery(query.ToString());          
            intGroupId = 0;

            if (dtGroup != null && dtGroup.Rows.Count > 0)
            {
                intGroupId = Convert.ToInt32(dtGroup.Rows[0]["GROUP_ID"]);
                return true;
            }
            return false;
        }

        public static bool GetChildGroupDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT GROUPORG_ID, GROUP_NAME, PARENT_GROUP_ID,LEADER_PERSON_ID,GROUP_ID FROM COEGROUP WHERE GROUP_ID <> GROUPORG_ID ORDER BY GROUP_ID DESC");
            DataTable dtGroup = DALHelper.ExecuteQuery(query.ToString());

            if (dtGroup != null && dtGroup.Rows.Count > 0)
            {
                if (dtGroup.Rows[0]["GROUP_ID"] != null && dtGroup.Rows[0]["GROUP_ID"] != DBNull.Value)
                    intGroupId = Convert.ToInt32(dtGroup.Rows[0]["GROUP_ID"]);

                if (dtGroup.Rows[0]["GROUPORG_ID"] != null && dtGroup.Rows[0]["GROUPORG_ID"] != DBNull.Value)
                    intGroupOrgId = Convert.ToInt32(dtGroup.Rows[0]["GROUPORG_ID"]);
                if (dtGroup.Rows[0]["PARENT_GROUP_ID"] != null && dtGroup.Rows[0]["PARENT_GROUP_ID"] != DBNull.Value)
                    intParentGroupId = Convert.ToInt32(dtGroup.Rows[0]["PARENT_GROUP_ID"]);
                if (dtGroup.Rows[0]["LEADER_PERSON_ID"] != null && dtGroup.Rows[0]["LEADER_PERSON_ID"] != DBNull.Value)
                    intLeaderPersonId = Convert.ToInt32(dtGroup.Rows[0]["LEADER_PERSON_ID"]);
                if (dtGroup.Rows[0]["GROUP_NAME"] != null && dtGroup.Rows[0]["GROUP_NAME"] != DBNull.Value)
                    strGroupName = Convert.ToString(dtGroup.Rows[0]["GROUP_NAME"]);

                return true;
            }
            return false;
        }

        public static bool GetObjectId(string strObjectType)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ID FROM COEOBJECTTYPE WHERE NAME = '" + strObjectType + "'");
            object theObjectType = DALHelper.ExecuteScalar(query.ToString());
            intObjectTypeId = 0;

            if (theObjectType != null && theObjectType != DBNull.Value)
            {
                intObjectTypeId = Convert.ToInt32(theObjectType);
                return true;
            }
            return false;
        }

        public static bool GetRolePrivDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ROLE_ID, ROLE_NAME, PRIVILEGE_TABLE_NAME, APP_NAME FROM SECURITY_ROLES, PRIVILEGE_TABLES WHERE ");
            query.Append("SECURITY_ROLES.PRIVILEGE_TABLE_INT_ID = PRIVILEGE_TABLES.PRIVILEGE_TABLE_ID ORDER BY ROLE_ID");

            DataTable dtRoles = DALHelper.ExecuteQuery(query.ToString());

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                intRoleId = Convert.ToInt32(dtRoles.Rows[0]["ROLE_ID"]);
                strRoleName = Convert.ToString(dtRoles.Rows[0]["ROLE_NAME"]);
                strPrivilegeTableName = Convert.ToString(dtRoles.Rows[0]["PRIVILEGE_TABLE_NAME"]);
                strAppName = Convert.ToString(dtRoles.Rows[0]["APP_NAME"]);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Method checks for <GROUP /> and its attribute-value
        /// </summary>
        /// <param name="strXMLDoc">XML document</param>
        /// <param name="strAttributeName">Name of attribute </param>
        /// <param name="strAttributeValue">Value of attribute </param>
        /// <returns>bool. True if value found for strAttributeName </returns>
        public static bool GroupExistInXML(string strXMLDoc, string strAttributeName, string strAttributeValue)
        {
            bool isGroupIDExistInXML = false;
            try
            {

                if (!string.IsNullOrEmpty(strXMLDoc) && !string.IsNullOrEmpty(strAttributeName))
                {
                    XmlDocument theXmlDocument = new XmlDocument();
                    theXmlDocument.LoadXml(strXMLDoc);
                    XmlNodeList theXmlNodeList = theXmlDocument.GetElementsByTagName("GROUP");
                    if (theXmlNodeList.Count > 0 && theXmlNodeList[0].Attributes[strAttributeName].Value == strAttributeValue)
                        isGroupIDExistInXML = true;
                    else
                        isGroupIDExistInXML = false;
                }
                else
                    isGroupIDExistInXML = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isGroupIDExistInXML;
        }


        public static bool GetGroupOrgID()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT GROUPORG_ID FROM COEGROUPORG");
            DataTable dtGroup = DALHelper.ExecuteQuery(query.ToString());
            intGroupOrgId = 0;

            if (dtGroup != null && dtGroup.Rows.Count > 0)
            {
                intGroupOrgId = Convert.ToInt32(dtGroup.Rows[0]["GROUPORG_ID"]);
                return true;
            }
            return false;
        }

        
        #endregion Methods
    }
}
