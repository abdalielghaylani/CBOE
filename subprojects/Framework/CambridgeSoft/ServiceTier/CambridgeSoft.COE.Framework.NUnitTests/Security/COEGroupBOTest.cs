using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using System.Data;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for COEGroupBOTest
    /// </summary>
    [TestFixture]
    public class COEGroupBOTest
    {
              
        public COEGroupBOTest()
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

        #region Test Methods

        [Test]
        public void Get_GroupBOByGroupId_IfExist()
        {
            if (SecurityHelper.GetGroupDetails())
            {
                COEGroupBO theCOEGroupBO = COEGroupBO.Get(SecurityHelper.intGroupId);
                Assert.IsNotNull(theCOEGroupBO);
            }
        }

        [Test]
        public void Get_GroupBOByGroupId_IfDoesNotExist()
        {
            COEGroupBO theCOEGroupBO = COEGroupBO.Get(-1);
            Assert.IsNotNull(theCOEGroupBO);
        }

        [Test]
        public void New_CreateNewGroupBO()
        {
            COEGroupBO theCOEGroupBO = COEGroupBO.New();
            Assert.IsNotNull(theCOEGroupBO);
        }

        [Test]
        public void Delete()
        {
            if (SecurityHelper.GetChildGroupDetails())
            {
                COEGroupBO.Delete(SecurityHelper.intGroupId);

                StringBuilder query = new StringBuilder();
                query.Append("SELECT GROUP_ID FROM COEGROUP WHERE GROUP_ID = " + SecurityHelper.intGroupId);
                DataTable dtGroup = DALHelper.ExecuteQuery(query.ToString());                             
                Assert.AreEqual(0, dtGroup.Rows.Count);             
            }
            else
                Assert.Fail("Group does not exist");
        }


        [Test]
        public void Save_IfNotAlreadyExist()
        {
            Type type = typeof(COEGroupBO);

            if (SecurityHelper.GetChildGroupDetails())
            {
                try
                {
                    GroupUserList theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupOrgId);
                    RoleList theRoleList = RoleList.GetGroupRoleList(1);
                    RoleList theGroupRoleAvailList = RoleList.GetGroupRoleAvailableList(1);

                    object[] argList = new object[] { 0, SecurityHelper.intGroupOrgId, "Inv_Group" + SecurityHelper.GenerateRandomNumber(), SecurityHelper.intGroupId, SecurityHelper.intLeaderPersonId, theGroupUserList, theRoleList, theGroupRoleAvailList };
                    COEGroupBO theCOEGroupBO = (COEGroupBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                                       
                    COEGroupBO theResult = theCOEGroupBO.Save();
                    Assert.IsNotNull(theResult);
                }
                catch
                {
                    Assert.Fail("Group name already exist!");
                }
            }
        }
        [Test]
        public void Save_ForceUpdate()
        {
            Type type = typeof(COEGroupBO);

            if (SecurityHelper.GetChildGroupDetails())
            {
                try
                {
                    GroupUserList theGroupUserList = GroupUserList.Get(SecurityHelper.intGroupOrgId);
                    RoleList theRoleList = RoleList.GetGroupRoleList(1);
                    RoleList theGroupRoleAvailList = RoleList.GetGroupRoleAvailableList(1);

                    object[] argList = new object[] { 0, SecurityHelper.intGroupOrgId, "Inv_Group" + SecurityHelper.GenerateRandomNumber(), SecurityHelper.intGroupId, SecurityHelper.intLeaderPersonId, theGroupUserList, theRoleList, theGroupRoleAvailList };
                    COEGroupBO theCOEGroupBO = (COEGroupBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                    
                    COEGroupBO theResult = theCOEGroupBO.Save(true);
                    Assert.IsNotNull(theResult);
                }
                catch
                {
                    Assert.Fail("Group name already exist!");
                }
            }
        }

        [Test]
        public void SetGroupUsers()
        {
            if (SecurityHelper.GetGroupDetails())
            {
                List<int> lstUsers = SecurityHelper.GetPersonIdList();
                string strUserString = string.Empty;

                for (int i = 0; i < lstUsers.Count; i++)
                {
                    strUserString += Convert.ToString(lstUsers[i]) + ",";
                }
                strUserString = strUserString.Substring(0, strUserString.Length - 1);
                COEGroupBO theCOEGroupBO = COEGroupBO.SetGroupUsers(SecurityHelper.intGroupId, strUserString);
                Assert.IsNotNull(theCOEGroupBO);
            }
            else            
                Assert.Fail("Group does not exist");
            
        }

        [Test]
        public void GetIdValue()
        {
            if (SecurityHelper.GetGroupDetails())
            {
                COEGroupBO theCOEGroupBO = COEGroupBO.Get(SecurityHelper.intGroupId);

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEGroupBO);
                object theId = thePrivateObject.Invoke("GetIdValue");

                Assert.AreEqual(theCOEGroupBO.GroupID, Convert.ToInt32(theId));
            }
            else
                Assert.Fail("Group does not exist");

        }

        
        #endregion Test Methods
    }
}
