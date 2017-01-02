using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEPrivilegeBOListTest
    /// </summary>
    [TestClass]
    public class COEPrivilegeBOListTest
    {
        public COEPrivilegeBOListTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public static void MyClassInitialize(TestContext testContext) { }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { }
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }
        
        #endregion

        #region Test Methods

        [TestMethod]
        public void GetList()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                Assert.IsNotNull(theCOEPrivilegeBOList);
            }            
        }

        [TestMethod]
        public void GetList_RetriveGrantPrivileges_True()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName,true);
                Assert.IsNotNull(theCOEPrivilegeBOList);
            }
        }

        [TestMethod]
        public void GetList_RetriveGrantPrivileges_False()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName, false);
                Assert.IsNotNull(theCOEPrivilegeBOList);
            }
        }

        
       [TestMethod]
        public void GetDefaultListForApp()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetDefaultListForApp(SecurityHelper.strAppName);
                Assert.IsNotNull(theCOEPrivilegeBOList);
            }
        }

        
        [TestMethod]
        public void GetPrivilegeByID_IfPrivIdExists()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                if (theCOEPrivilegeBOList.Count > 0)
                {
                    COEPrivilegeBO theCOEPrivilegeBO = theCOEPrivilegeBOList.GetPrivilegeByID(theCOEPrivilegeBOList[0].PrivilegeID);
                    Assert.IsNotNull(theCOEPrivilegeBO);
                }
                else
                    Assert.Fail("Privilege does not exist");
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void GetPrivilegeByID_IfPrivIdDoesNotExist()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
               
                COEPrivilegeBO theCOEPrivilegeBO = theCOEPrivilegeBOList.GetPrivilegeByID(-1);
                Assert.IsNull(theCOEPrivilegeBO);
               
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void GetPrivilegeByName_IfPrivNameExists()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                if (theCOEPrivilegeBOList.Count > 0)
                {
                    COEPrivilegeBO theCOEPrivilegeBO = theCOEPrivilegeBOList.GetPrivilegeByName(theCOEPrivilegeBOList[0].PrivilegeName);
                    Assert.IsNotNull(theCOEPrivilegeBO);
                }
                else            
                    Assert.Fail("Privilege does not exist");            
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void GetPrivilegeByName_IfPrivNameDoesNotExist()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
               
                COEPrivilegeBO theCOEPrivilegeBO = theCOEPrivilegeBOList.GetPrivilegeByName("Pirv-1");
                Assert.IsNull(theCOEPrivilegeBO);               
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void AddPrivilege_IfPrivDoesNotExist()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBO theCOEPrivilegeBO = COEPrivilegeBO.New();
                theCOEPrivilegeBO.PrivilegeName = "NEW_PRIV";
                    
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                int intCount = theCOEPrivilegeBOList.Count;

                COEPrivilegeBOList theNewCOEPrivilegeBOList = theCOEPrivilegeBOList.AddPrivilege(theCOEPrivilegeBO);
                Assert.AreEqual(intCount + 1, theNewCOEPrivilegeBOList.Count);
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddPrivilege_IfPrivExists()
        {
            if (SecurityHelper.GetRoleDetails())
            {                
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);            
                if (theCOEPrivilegeBOList.Count > 0)
                {
                    COEPrivilegeBOList theNewCOEPrivilegeBOList = theCOEPrivilegeBOList.AddPrivilege(theCOEPrivilegeBOList[0]);                    
                }
                else
                    Assert.Fail("Privilege list is empty");
            }
        }

        [TestMethod]
        public void RemovePrivilege()
        {           
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                int intCount = theCOEPrivilegeBOList.Count;

                if (intCount > 0)
                {
                    COEPrivilegeBO theCOEPrivilegeBO = theCOEPrivilegeBOList.GetPrivilegeByID(theCOEPrivilegeBOList[0].PrivilegeID);                    
                    COEPrivilegeBOList theNewCOEPrivilegeBOList = theCOEPrivilegeBOList.RemovePrivilege(theCOEPrivilegeBO);
                    Assert.AreEqual(intCount - 1,theNewCOEPrivilegeBOList.Count);
                }
                else
                    Assert.Fail("Privilege does not exist");  
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void Contains()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEPrivilegeBOList theCOEPrivilegeBOList = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
              
                if (theCOEPrivilegeBOList.Count > 0)
                {                   
                    bool blnFlag = theCOEPrivilegeBOList.Contains(theCOEPrivilegeBOList[0].PrivilegeID);
                    Assert.AreEqual(true, blnFlag);
                }
                else
                    Assert.Fail("Privilege does not exist");
            }
            else
                Assert.Fail("Role does not exist");
        }

        #endregion Test Methods
    }
}
