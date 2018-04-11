using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using System.Data;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for COERoleBOTest
    /// </summary>
    [TestFixture]
    public class COERoleBOTest
    {

        #region Variables

        //int roleID = 0;
        //string roleName = string.Empty, privilegeTableName = string.Empty;
        COEPrivilegeBOList privileges;
        COERoleBOList roleRoles;
        COEUserBOList roleUsers;
        //string appName = string.Empty;
      
        #endregion

        public COERoleBOTest()
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
        public void COERoleBO_GetRoleIdProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId,SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                int intRoleId = theCOERoleBO.RoleID;
                Assert.AreEqual(SecurityHelper.intRoleId, intRoleId);
            }
        }

        [Test]
        public void COERoleBO_GetRoleNameProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                string strRoleName = theCOERoleBO.RoleName;
                Assert.AreEqual(SecurityHelper.strRoleName, strRoleName);
            }
        }

        [Test]
        public void COERoleBO_GetPrivilegeTableNameProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOERoleBO);                
                object theResult = thePrivateObject.GetProperty("PrivilegeTableName", null);
                Assert.AreEqual(SecurityHelper.strPrivilegeTableName, Convert.ToString(theResult));
            }
        }

        [Test]
        public void COERoleBO_GetPrivilegesProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                COEPrivilegeBOList lstprivileges = theCOERoleBO.Privileges;
                Assert.AreEqual(privileges, lstprivileges);
            }
        }

        [Test]
        public void COERoleBO_GetRoleRolesProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                COERoleBOList lstRoles = theCOERoleBO.RoleRoles;
                Assert.AreEqual(roleRoles, lstRoles);
            }
        }

        [Test]
        public void COERoleBO_GetRoleUsersProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                COEUserBOList lstUsers = theCOERoleBO.RoleUsers;
                Assert.AreEqual(roleUsers, lstUsers);
            }
        }

        [Test]
        public void COERoleBO_GetCOEIdentifierProperty()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName, SecurityHelper.strPrivilegeTableName, privileges, roleRoles, roleUsers, SecurityHelper.strAppName };
                COERoleBO theCOERoleBO = (COERoleBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                string strCOEIdentifier = theCOERoleBO.COEIdentifier;
                Assert.AreEqual(SecurityHelper.strAppName, strCOEIdentifier);
            }
        }

        [Test]
        public void Get_ByRoleName()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                COERoleBO theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);
                Assert.IsNotNull(theCOERoleBO);
            }
        }

        [Test]
        public void Get()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COERoleBO theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName,true,true,true);
                
                Assert.IsNotNull(theCOERoleBO);
            }
        }

        [Test]
        public void Save_UsingNewMethod()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);
                                
                COERoleBO theCOERoleBO = COERoleBO.New();
                theCOERoleBO.RoleName = "NEW_TEST" + roleRoles.Count;
                theCOERoleBO.Privileges = privileges;
                theCOERoleBO.RoleRoles = roleRoles;
                theCOERoleBO.RoleUsers = roleUsers;
                theCOERoleBO.COEIdentifier = SecurityHelper.strAppName;

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOERoleBO);
                thePrivateObject.SetField("_privilegeTableName", SecurityHelper.strPrivilegeTableName);
                
                COERoleBO theResult = theCOERoleBO.Save();
                Assert.IsNotNull(theResult);
            }

        }

        [Test]
        public void Save_UsingNewMethod_ByApplicationName()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                COERoleBO theCOERoleBO = COERoleBO.New(SecurityHelper.strAppName);
                theCOERoleBO.RoleName = "NEW_TEST" + roleRoles.Count;
                theCOERoleBO.Privileges = privileges;
                theCOERoleBO.RoleRoles = roleRoles;
                theCOERoleBO.RoleUsers = roleUsers;
                theCOERoleBO.COEIdentifier = SecurityHelper.strAppName;

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOERoleBO);
                thePrivateObject.SetField("_privilegeTableName", SecurityHelper.strPrivilegeTableName);
               
                COERoleBO theResult = theCOERoleBO.Save();
                Assert.IsNotNull(theResult);
            }
        }

        [Test]
        public void Save_UsingNewMethod_ForceUpdate()
        {
            if (SecurityHelper.GetRolePrivDetails())
            {
                Type type = typeof(COERoleBO);
                privileges = COEPrivilegeBOList.GetList(SecurityHelper.strRoleName);
                roleRoles = COERoleBOList.GetList();
                roleUsers = COEUserBOList.GetListByRole(SecurityHelper.strRoleName);

                COERoleBO theCOERoleBO = COERoleBO.New();
                theCOERoleBO.RoleName = "NEW_TEST" + roleRoles.Count;
                theCOERoleBO.Privileges = privileges;
                theCOERoleBO.RoleRoles = roleRoles;
                theCOERoleBO.RoleUsers = roleUsers;
                theCOERoleBO.COEIdentifier = SecurityHelper.strAppName;

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOERoleBO);
                thePrivateObject.SetField("_privilegeTableName", SecurityHelper.strPrivilegeTableName);

                COERoleBO theResult = theCOERoleBO.Save(true);
                Assert.IsNotNull(theResult);
            }
        }       
       
        [Test]
        public void Delete()
        {
            if (SecurityHelper.GetRoleDetails())
            {
                COERoleBO theCOERoleBO = COERoleBO.Get(SecurityHelper.strRoleName);                
                theCOERoleBO.Delete();                
            }
        }

        #endregion Test Methods
    }
}
