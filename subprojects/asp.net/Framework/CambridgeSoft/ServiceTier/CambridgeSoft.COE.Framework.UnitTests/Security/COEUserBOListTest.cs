using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEUserBOListTest
    /// </summary>
    [TestClass]
    public class COEUserBOListTest
    {
        public COEUserBOListTest()
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
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Authentication.Logoff();
        }
        
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
            COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
            Assert.IsNotNull(theCOEUserBOList);                
        }

        [TestMethod]
        public void GetListByApplication_AppName()
        {
            if (SecurityHelper.GetApplicationDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetListByApplication(SecurityHelper.strAppName);
                Assert.IsNotNull(theCOEUserBOList);
            }
            else
                Assert.Fail("Application does not exist");
        }

        [TestMethod]
        public void GetListByApplication_AppNameList()
        {
            List<string> lstApp = SecurityHelper.GetApplicationList();          
            COEUserBOList theCOEUserBOList = COEUserBOList.GetListByApplication(lstApp);
            Assert.IsNotNull(theCOEUserBOList);         
        }

        [TestMethod]
        public void GetListByRole_RoleName()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);
                Assert.IsNotNull(theCOEUserBOList);
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void GetListByRole_RoleName_getUserRoles_True()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
                PrivateObject thePrivateObject = new PrivateObject(theCOEUserBOList);
                thePrivateObject.Invoke("GetListByRole", BindingFlags.NonPublic | BindingFlags.Static, new object[] { SecurityHelper.strRoleName, true });                
                Assert.IsNotNull(theCOEUserBOList);
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void GetListByRole_RoleName_getUserRoles_False()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
                PrivateObject thePrivateObject = new PrivateObject(theCOEUserBOList);
                thePrivateObject.Invoke("GetListByRole", BindingFlags.NonPublic | BindingFlags.Static, new object[] { SecurityHelper.strRoleName, false });
                Assert.IsNotNull(theCOEUserBOList);
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void GetListByRole_RoleNameList()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                //List<string> lstRoleName = SecurityHelper.GetRoleNameList();
                //COEUserBOList theCOEUserBOList = COEUserBOList.GetListByRole(lstRoleName);
                COEUserBOList theCOEUserBOList = COEUserBOList.GetListByRole(new List<string> { SecurityHelper.strRoleName });
                Assert.IsNotNull(theCOEUserBOList);
            }
            else
                Assert.Fail("Role does not exist");
        }      

        [TestMethod]
        public void GetListByUserID()
        {
            List<int> lstPerson = SecurityHelper.GetPersonIdList();
            COEUserBOList theCOEUserBOList = COEUserBOList.GetListByUserID(lstPerson);
            Assert.IsNotNull(theCOEUserBOList);
        }

        [TestMethod]
        public void GetRoleAvailableUsers()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COERoleBO theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);
                COEUserBOList theCOEUserBOList = COEUserBOList.GetRoleAvailableUsers(theCOERoleBO);
                Assert.IsNotNull(theCOEUserBOList);
            }
            else
                Assert.Fail("Role does not exist");
        }

        [TestMethod]
        public void NewList()
        {
            List<string> lstPerson = SecurityHelper.GetPersonNameList();
            COEUserBOList theCOEUserBOList = COEUserBOList.NewList(lstPerson);
            Assert.AreEqual(lstPerson.Count, theCOEUserBOList.Count);
        }

        [TestMethod]
        public void NewList_Null()
        {            
            COEUserBOList theCOEUserBOList = COEUserBOList.NewList(null);
            Assert.IsNull(theCOEUserBOList);
        }

        [TestMethod]
        public void GetUserByID_IfUserExists()
        {
            if(SecurityHelper.GetUserDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
                COEUserBO theCOEUserBO = theCOEUserBOList.GetUserByID(SecurityHelper.intPersonId);
                Assert.IsNotNull(theCOEUserBO);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void GetUserByID_IfUserDoesNotExist()
        {
            COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
            COEUserBO theCOEUserBO = theCOEUserBOList.GetUserByID(-1);
            Assert.IsNull(theCOEUserBO);            
        }

        [TestMethod]
        public void GetUserByUserID_IfUserExists()
        {
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
                COEUserBO theCOEUserBO = theCOEUserBOList.GetUserByUserID(SecurityHelper.strUserID);
                Assert.IsNotNull(theCOEUserBO);
            }
            else
                Assert.Fail("User does not exist");
        }

       

        [TestMethod]
        public void GetUserByUserID_IfUserDoesNotExist()
        {
            COEUserBOList theCOEUserBOList = COEUserBOList.GetList();            
            COEUserBO theCOEUserBO = theCOEUserBOList.GetUserByUserID(string.Empty);
            Assert.IsNull(theCOEUserBO);
        }

        [TestMethod]
        public void AddUser()
        {
            COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
            int intCount = theCOEUserBOList.Count;

            if (theCOEUserBOList.Count > 0)
            {                
                COEUserBO theCOEUserBO = COEUserBO.New();
                theCOEUserBO.UserCode = "new-user";

                theCOEUserBOList.AddUser(theCOEUserBO);
                Assert.AreEqual(intCount + 1, theCOEUserBOList.Count);
            }
            else
                Assert.Fail("User does not exist");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddUser_IfExists()
        {
            COEUserBOList theCOEUserBOList = COEUserBOList.GetList();            
            COEUserBO theCOEUserBO = theCOEUserBOList[0];                
            theCOEUserBOList.AddUser(theCOEUserBO);                            
        }

        [TestMethod]
        public void RemoveUser()
        {           
            COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
            int intCount = theCOEUserBOList.Count;

            if (theCOEUserBOList.Count > 0)
            {              
                COEUserBO theCOEUserBO = theCOEUserBOList[0];                
                theCOEUserBOList.RemoveUser(theCOEUserBO);
                Assert.AreEqual(intCount - 1, theCOEUserBOList.Count);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void Contains_PersonId()
        {
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
                bool blnResult = theCOEUserBOList.Contains(SecurityHelper.intPersonId);
                Assert.AreEqual(true, blnResult);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void Contains_UserId()
        {
            if (SecurityHelper.GetUserDetails())
            {
                COEUserBOList theCOEUserBOList = COEUserBOList.GetList();
                bool blnResult = theCOEUserBOList.Contains(SecurityHelper.strUserID);
                Assert.AreEqual(true,blnResult);
            }     
            else
                Assert.Fail("User does not exist");
        }

        #endregion Test Methods
    }
}
