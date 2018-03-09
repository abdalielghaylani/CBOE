using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COERoleReadOnlyBOTest
    /// </summary>
    [TestClass]
    public class COERoleReadOnlyBOTest
    {
        #region Variables

        private static bool blnRoleDetails = false;

        #endregion

        public COERoleReadOnlyBOTest()
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
        
         //Use ClassCleanup to run code after all tests in a class have run
         [ClassCleanup()]
         public static void MyClassCleanup() 
         {
             Authentication.Logoff();
         }

         //Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() { }

         //Use TestCleanup to run code after each test has run
         [TestCleanup()]
         public void MyTestCleanup() { }       

        #endregion

         #region Test Methods

         [TestMethod]
         public void COERoleReadOnlyBO_GetRoleId()
         {             
             Type type = typeof(COERoleReadOnlyBO);

             if (blnRoleDetails)
             {
                 object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName };

                 COERoleReadOnlyBO theTestCOERoleReadOnlyBO = (COERoleReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                 int intNewRoleId = theTestCOERoleReadOnlyBO.RoleID;
                 Assert.AreEqual(SecurityHelper.intRoleId, intNewRoleId);
             }         
             else
                 Assert.Fail("Role does not exist");
         }

         [TestMethod]
         public void COERoleReadOnlyBO_GetRoleName()
         {
             Type type = typeof(COERoleReadOnlyBO);

             if (blnRoleDetails)
             {
                 object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName };

                 COERoleReadOnlyBO theTestCOERoleReadOnlyBO = (COERoleReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                 string strNewRoleName = theTestCOERoleReadOnlyBO.RoleName;
                 Assert.AreEqual(SecurityHelper.strRoleName, strNewRoleName);
             }
             else             
                 Assert.Fail("Role does not exist");
             
         }

         [TestMethod]
         public void GetIdValue_RoleId()
         {
             Type type = typeof(COERoleReadOnlyBO);
             if (blnRoleDetails)
             {
                 object[] argList = new object[] { SecurityHelper.intRoleId, SecurityHelper.strRoleName };

                 COERoleReadOnlyBO theCOERoleReadOnlyBO = (COERoleReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, argList, null, null);
                 PrivateObject thePrivateObject = new PrivateObject(theCOERoleReadOnlyBO);
                 object theResult = thePrivateObject.Invoke("GetIdValue", null);
                 Assert.AreEqual(SecurityHelper.intRoleId, Convert.ToInt32(theResult));
             }
             else             
                 Assert.Fail("Role does not exist");             
         }        

        [TestMethod]
        public void Get_RoleBusinessObject()
        {    
            StringBuilder query = new StringBuilder();
            query.Append("SELECT USER_ID FROM PEOPLE ORDER BY USER_ID");
            object theResult = DALHelper.ExecuteScalar(query.ToString());

            if (theResult != null)
            {
                string strUserName = Convert.ToString(theResult);

                COERoleReadOnlyBO theCOERoleReadOnlyBO = COERoleReadOnlyBO.Get(strUserName);
                Assert.IsNotNull(theCOERoleReadOnlyBO, "It returns null value");
            }
            else            
                Assert.Fail("User does not exist");            
        }

        #endregion

    }
}
