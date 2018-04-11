using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using NUnit.Framework;

namespace NUnitTest
{
    [TestFixture]
    class InsertLDAPUser_Test : LoginBase
    {

        #region private method
        /// <summary>
        /// this test mehtod will test the following cases
        /// 1) New user creation for LDAP  -----Excepted result= Sucess
        /// 2) If LDAP user already exits  -----Expected Result= Throws error
        /// </summary>
        [Test]
        public void CreateUserTest()
        {
            COEUserBO user = COEUserBO.New();
            COEUserBO obj = null;
            try
            {

                List<string> roles = new List<string>();
                roles.Add("CSS_USER");
                roles.Add("CSS_ADMIN");

                user.UserID = "kganta";
                user.IsDBMSUser = false;
                user.Password = "7ATNAGK11C";
                user.Roles = COERoleBOList.NewList(roles);
                user.FirstName = "Krishna";
                user.LastName = "Ganta";
                user.MiddleName = "";
                user.Telephone = "";
                user.Email = "KGanta@cambridgesoft.com";
                user.Address = "";
                user.UserCode = "kganta";
                user.SupervisorID = 0;
                user.SiteID = 0;
                user.Active = true;
                obj = user.Save();
                Assert.AreNotEqual(null, obj, "Did not return the expected result.");

            }
            catch (Exception ex)
            {
                
                if (((Csla.DataPortalException)(ex)).BusinessException.Message.Contains("conflicts with another user"))
                {
                    try
                    {
                        //deleting the user
                        user.Delete();
                        //create user again after deleting the user, so that we can see the new user creation status here.
                        obj = user.Save();
                    }
                    catch (Exception ex1)
                    {
                        Assert.Fail(((Csla.DataPortalException)(ex1)).BusinessException.Message);
                    }
                }
            }

        }
        /// <summary>
        /// this test mehtod will test the following cases
        /// 1) Checks for LDAP user, here manually we have Alter the existing user with other then magic password
        /// -----Excepted result= throws error. 
        /// </summary>
        [Test]
        public void CreateUserConfilctsTest()
        {
            try
            {

                List<string> roles = new List<string>();
                roles.Add("CSS_USER");
                roles.Add("CSS_ADMIN");
                COEUserBO user = COEUserBO.New();
                user.UserID = "kganta";
                user.IsDBMSUser = false;
                user.Password = "7ATNAGK11C";
                user.Roles = COERoleBOList.NewList(roles);
                user.FirstName = "Krishna";
                user.LastName = "Ganta";
                user.MiddleName = "";
                user.Telephone = "";
                user.Email = "KGanta@cambridgesoft.com";
                user.Address = "";
                user.UserCode = "kganta";
                user.SupervisorID = 0;
                user.SiteID = 0;
                user.Active = true;
                COEUserBO obj = user.Save();
                if (obj != null)
                    user.Save();
               
            }
            catch (Exception ex)
            {
                string Expected = "user name KGANTA conflicts with another user";
                string Actual = ((Csla.DataPortalException)(ex)).BusinessException.Message.Trim();
                Assert.AreEqual(Expected, Actual,"Did not return the expected result.");                
            }

        }


        #endregion
    }
}
