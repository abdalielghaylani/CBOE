using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COERoleReadOnlyBOListTest
    /// </summary>
    [TestClass]
    public class COERoleReadOnlyBOListTest
    {

        #region Member Variables
                
        private static bool blnRoleDetails = false;

        COERoleReadOnlyBOList theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
        COERoleReadOnlyBO theCOERoleReadOnlyBO = null;
        
        List<string> lstRoleName = new List<string>();
        List<int> lstRoleId = new List<int>();
        List<string> lstApp = new List<string>();

        #endregion

        public COERoleReadOnlyBOListTest()
        {          
        }

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
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {           
            Authentication.Logon();
            blnRoleDetails = SecurityHelper.GetRoleDetails();
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Authentication.Logoff();
        }
        
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        public void MyTestCleanup() { }                         
               
        #endregion

        #region Test Methods     
     
        [TestMethod]
        public void GetRoleByID_RoleIdExistInList()
        {
            theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();
            if (blnRoleDetails)
            {
                COERoleReadOnlyBO theResult = theCOERoleReadOnlyBOList.GetRoleByID(SecurityHelper.intRoleId);
                Assert.IsNotNull(theResult);
            }
        }

        [TestMethod]
        public void GetRoleByID_RoleIdDoesNotExistInList()
        {
            theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();
            COERoleReadOnlyBO theResult = theCOERoleReadOnlyBOList.GetRoleByID(0);
            Assert.IsNull(theResult);
        }

        [TestMethod]
        public void GetRoleByRoleName_RoleNameExistInList()
        {
            theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();
            if (blnRoleDetails)
            {
                COERoleReadOnlyBO theResult = theCOERoleReadOnlyBOList.GetRoleByRoleName(SecurityHelper.strRoleName);
                Assert.IsNotNull(theResult);
            }
        }

        [TestMethod]
        public void GetRoleByRoleName_RoleNameDoesNotExistInList()
        {
            theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();
            COERoleReadOnlyBO theResult = theCOERoleReadOnlyBOList.GetRoleByRoleName("-aaa-test");
            Assert.IsNull(theResult);

        }

        [TestMethod]
        public void AddRole_IfRoleDoesNotExist()
        {
            if (blnRoleDetails)
            {
                theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
                theCOERoleReadOnlyBO = null;
                theCOERoleReadOnlyBO = COERoleReadOnlyBO.Get(SecurityHelper.strRoleName);
                theCOERoleReadOnlyBOList.AddRole(theCOERoleReadOnlyBO);
                Assert.AreEqual(1, theCOERoleReadOnlyBOList.Count);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddRole_IfRoleExist()
        {
            if (blnRoleDetails)
            {
                theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
                theCOERoleReadOnlyBO = null;
                theCOERoleReadOnlyBO = COERoleReadOnlyBO.Get(SecurityHelper.strRoleName);
                theCOERoleReadOnlyBOList.AddRole(theCOERoleReadOnlyBO);
                theCOERoleReadOnlyBOList.AddRole(theCOERoleReadOnlyBO);
            }
        }
        
        [TestMethod]
        public void RemoveRole()
        {
           theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
           theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();
           int intListCount = theCOERoleReadOnlyBOList.Count;

           if (blnRoleDetails)
           {
               theCOERoleReadOnlyBO = COERoleReadOnlyBO.Get(SecurityHelper.strRoleName);
               theCOERoleReadOnlyBOList.RemoveRole(theCOERoleReadOnlyBO);
               Assert.AreEqual(intListCount - 1, theCOERoleReadOnlyBOList.Count);
           }
        }

        [TestMethod]
        public void Contains_RoleId()
        {
            theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();

            if (blnRoleDetails)
            {
                bool blnFlag = theCOERoleReadOnlyBOList.Contains(SecurityHelper.intRoleId);
                Assert.AreEqual(true, blnFlag);
            }
        }

        [TestMethod]
        public void GetList_Roles()
        {
            theCOERoleReadOnlyBOList = new COERoleReadOnlyBOList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();
            Assert.IsNotNull(theCOERoleReadOnlyBOList);
        }

        [TestMethod]
        public void GetListByApplication()
        {          
           lstApp = SecurityHelper.GetApplicationList();
           theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetListByApplication(lstApp);
           Assert.IsNotNull(theCOERoleReadOnlyBOList);
        }


        [TestMethod]
        public void GetListByUser()
        {           
            lstRoleName = SecurityHelper.GetRoleNameList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetListByUser(lstRoleName);
            Assert.IsNotNull(theCOERoleReadOnlyBOList);
        }

        [TestMethod]
        public void GetListByRoleID()
        {           
            lstRoleId = SecurityHelper.GetRoleIdList();
            theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetListByRoleID(lstRoleId);
            Assert.IsNotNull(theCOERoleReadOnlyBOList);
        }

        #endregion Test Methods

    }
}
