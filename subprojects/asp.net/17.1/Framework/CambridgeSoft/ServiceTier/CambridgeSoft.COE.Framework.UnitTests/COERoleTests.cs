﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEFormService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
namespace CambridgeSoft.COE.Framework.COESecurityService.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.COEFormService.COEFormBOList and is intended
    ///to contain all CambridgeSoft.COE.Framework.COEFormService.COEFormBOList Unit Tests
    ///</summary>
    [TestClass()]
    public class COERoleTests
    {
        private DALFactory _dalFactory = new DALFactory();
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;

            string userName = "cssadmin";
            string password = "cssadmin";
            bool result = COEPrincipal.Login(userName, password);

        }


        #endregion


        [TestMethod()]
        public void GetRole()
        {

            COERoleReadOnlyBO role = null;
            string roleName = "CSS_ADMIN";
            role = COERoleReadOnlyBO.Get(roleName);

            Assert.IsTrue(role != null, "GetAllRoles did not" +
                    " return the expected value.");
        }

        [TestMethod()]
        public void GetAllRoles()
        {

            COERoleReadOnlyBOList roles = null;

            roles = COERoleReadOnlyBOList.GetList();

            Assert.IsTrue(roles != null, "GetAllRoles did not" +
                    " return the expected value.");
        }
        [TestMethod()]
        public void GetAllRolesByApplicationSingle()
        {

            COERoleReadOnlyBOList roles = null;

            List<string> appNames = new List<string>();

            appNames.Add("CHEMINVDB2");

            roles = COERoleReadOnlyBOList.GetListByApplication(appNames);

            Assert.IsTrue(roles != null, "GetAllRoles did not" +
                    " return the expected value.");
        }

        [TestMethod()]
        public void GetAllRolesByApplicationMulti()
        {

            COERoleReadOnlyBOList roles = null;
            List<string> appNames = new List<string>();

            appNames.Add("CHEMINVDB2");
            appNames.Add("SAMPLE");

            roles = COERoleReadOnlyBOList.GetListByApplication(appNames);
            Assert.IsTrue(roles != null, "GetAllRoles did not" +
                    " return the expected value.");
        }

        [TestMethod()]
        public void GetAllRolesByUserSingle()
        {


            COEUserReadOnlyBOList roles = null;
            List<int> userIDS = new List<int>();

            userIDS.Add(1);



            roles = COEUserReadOnlyBOList.GetListByUserID(userIDS);

            Assert.IsTrue(roles != null, "GetAllRoles did not" +
                    " return the expected value.");
        }

        [TestMethod()]
        public void GetAllRolesByUserMulti()
        {

            COEUserReadOnlyBOList roles = null;
            List<int> userIDS = new List<int>();

            userIDS.Add(1);
            userIDS.Add(2);
            userIDS.Add(3);



            roles = COEUserReadOnlyBOList.GetListByUserID(userIDS);
            Assert.IsTrue(roles != null, "GetAllRoles did not" +
                    " return the expected value.");
        }


    }
}
