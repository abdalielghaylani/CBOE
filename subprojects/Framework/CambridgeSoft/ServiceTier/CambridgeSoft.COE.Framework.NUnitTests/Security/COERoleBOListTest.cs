using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Data;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for COERoleBOListTest
    /// </summary>
    [TestFixture]
    public class COERoleBOListTest
    {

        #region Variables

        COERoleBOList theCOERoleBOList = null;
        COERoleBO theCOERoleBO = null;
        List<string> lstApp = new List<string>();
        List<string> lstRoleName = new List<string>();
        List<int> lstRoleId = new List<int>();

        #endregion

        public COERoleBOListTest()
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
        [TestFixtureSetUp]
        public static void MyClassInitialize()
        {
            Authentication.Logon();
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void MyClassCleanup() 
        {
            Authentication.Logoff();
        }
        
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup() { }               
       
        #endregion

        #region Test methods
        
        [Test]
        public void GetRoleByID_RoleIdExistInList()
        {           
            theCOERoleBOList = COERoleBOList.GetList();
            if (SecurityHelper.GetRoleDetails())
            {
                COERoleBO theResult = theCOERoleBOList.GetRoleByID(SecurityHelper.intRoleId);
                Assert.IsNotNull(theResult);
            }
        }

        [Test]         
        public void GetRoleByID_RoleIdDoesNotExistInList()        
        {            
            theCOERoleBOList = COERoleBOList.GetList();           
            COERoleBO theResult = theCOERoleBOList.GetRoleByID(0);
            Assert.IsNull(theResult);           
        }

        [Test]
        public void GetRoleByRoleName_RoleNameExistInList()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                theCOERoleBOList = COERoleBOList.GetList();
                COERoleBO theResult = theCOERoleBOList.GetRoleByRoleName(SecurityHelper.strRoleName);
                Assert.IsNotNull(theResult);
            }
        }

        [Test]
        public void GetRoleByRoleName_RoleNameDoesNotExistInList()
        {         
            theCOERoleBOList = COERoleBOList.GetList();
            COERoleBO theResult = theCOERoleBOList.GetRoleByRoleName(SecurityHelper.strRoleName + "-aaa-test");
            Assert.IsNull(theResult);
        }

        [Test]
        public void AddRole_IfRoleDoesNotExist()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                theCOERoleBOList = (COERoleBOList)Activator.CreateInstance(typeof(COERoleBOList), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);
                theCOERoleBOList.AddRole(theCOERoleBO);
                Assert.AreEqual(1, theCOERoleBOList.Count);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddRole_IfRoleExist()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                theCOERoleBOList = (COERoleBOList)Activator.CreateInstance(typeof(COERoleBOList), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);
                theCOERoleBOList.AddRole(theCOERoleBO);
                theCOERoleBOList.AddRole(theCOERoleBO);                
            }
        }

        [Test]
        public void RemoveRole()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                theCOERoleBOList = COERoleBOList.GetList();                                
                int intListCount = theCOERoleBOList.Count;
                theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);
                theCOERoleBOList.RemoveRole(theCOERoleBO);
                bool blnFlag = theCOERoleBOList.ContainsDeleted(theCOERoleBO);
                Assert.IsTrue(blnFlag);
            }
        }

        [Test]
        public void Contains_RoleId()
        {            
            theCOERoleBOList = COERoleBOList.GetList();

            if (SecurityHelper.GetRoleDetails())
            {
                bool blnFlag = theCOERoleBOList.Contains(SecurityHelper.intRoleId);
                Assert.AreEqual(true, blnFlag);
            }
        }

        [Test]
        public void Contains_RoleName()
        {
            theCOERoleBOList = COERoleBOList.GetList();

            if (SecurityHelper.GetRoleDetails())
            {
                bool blnFlag = theCOERoleBOList.Contains(SecurityHelper.strRoleName);
                Assert.AreEqual(true, blnFlag);
            }
        }

        [Test]
        public void GetRoleNameList()
        {
            theCOERoleBOList = COERoleBOList.GetList();
            List<string> lstRoles = COERoleBOList.GetRoleNameList(theCOERoleBOList);
            Assert.AreEqual(theCOERoleBOList.Count, lstRoles.Count);
        }

        [Test]
        public void GetNewUserAvailableRoles()
        {
            theCOERoleBOList = COERoleBOList.GetNewUserAvailableRoles();
            Assert.IsNotNull(theCOERoleBOList);
        }        

        [Test]
        public void GetList_Roles()
        {            
            theCOERoleBOList = COERoleBOList.GetList();
            Assert.IsNotNull(theCOERoleBOList);
        }

        [Test]
        public void GetListByApplication_ByAppList()
        {
            lstApp = SecurityHelper.GetApplicationList();
            theCOERoleBOList = COERoleBOList.GetListByApplication(lstApp);            
            Assert.Fail("Error : Unable to find specified column COEIDENTIFIER in result set");
        }

        [Test]
        public void GetListByApplication()
        {
            lstApp = SecurityHelper.GetApplicationList();
            if (lstApp.Count > 0)
            {
                theCOERoleBOList = COERoleBOList.GetListByApplication(lstApp[0],true,true,true);                
                Assert.Fail("Error : Unable to find specified column COEIDENTIFIER in result set");
            }
        }


        [Test]
        public void GetListByUser_ByUserName()
        {
            if (SecurityHelper.GetUserDetails())
            {                
                theCOERoleBOList = COERoleBOList.GetListByUser(SecurityHelper.strUserID);
                Assert.IsNotNull(theCOERoleBOList);
            }
        }

        [Test]
        public void GetListByUser_ByUserList()
        {
            if (SecurityHelper.GetUserDetails())
            {
                List<string> lstUserNames = new List<string>();
                lstUserNames.Add(SecurityHelper.strUserID);
                theCOERoleBOList = COERoleBOList.GetListByUser(lstUserNames);
                Assert.IsNotNull(theCOERoleBOList);
            }
        }

        [Test]
        public void GetListByUser_ByUserListAndGetPrivileges()
        {
            if (SecurityHelper.GetUserDetails())
            {
                List<string> lstUserNames = new List<string>();
                lstUserNames.Add(SecurityHelper.strUserID);
                theCOERoleBOList = COERoleBOList.GetListByUser(lstUserNames,true);
                Assert.IsNotNull(theCOERoleBOList);
            }
            
        }

        [Test]
        public void GetListByRoleID()
        {            
            lstRoleId = SecurityHelper.GetRoleIdList();
            theCOERoleBOList = COERoleBOList.GetListByRoleID(lstRoleId);
            Assert.IsNotNull(theCOERoleBOList);
        }

        [Test]
        public void BuildFromListItems()
        {
            theCOERoleBOList = COERoleBOList.GetList();
            int intCount = theCOERoleBOList.Count;

            List<string> lstRoles = new List<string>();
            lstRoles.Add("NEW-ROLE1");
            theCOERoleBOList.BuildFromListItems(theCOERoleBOList, lstRoles);
            Assert.AreEqual(intCount + 1, theCOERoleBOList.Count);
        }

        [Test]
        public void NewList()
        {           
            List<string> lstRoles = new List<string>();
            lstRoles.Add("NEW-ROLE1");
            theCOERoleBOList = COERoleBOList.NewList(lstRoles);
            Assert.AreEqual(lstRoles.Count, theCOERoleBOList.Count);
        }

        [Test]
        public void GetRoleAvailableRoles()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                theCOERoleBOList = (COERoleBOList)Activator.CreateInstance(typeof(COERoleBOList), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);
                theCOERoleBOList = COERoleBOList.GetRoleAvailableRoles(theCOERoleBO);
                Assert.IsNotNull(theCOERoleBOList);
            }
        }

        [Test]
        public void GetUserAvailableRoles()
        {
            if (SecurityHelper.GetUserDetails())
            {
                theCOERoleBOList = COERoleBOList.GetUserAvailableRoles(SecurityHelper.strUserID);
                Assert.IsNotNull(theCOERoleBOList);
            }
        }

        [Test]
        public void GetRoleRoles()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                theCOERoleBOList = COERoleBOList.GetRoleRoles(SecurityHelper.strRoleName);
                Assert.IsNotNull(theCOERoleBOList);
            }
        }       

        #endregion
    }
}
