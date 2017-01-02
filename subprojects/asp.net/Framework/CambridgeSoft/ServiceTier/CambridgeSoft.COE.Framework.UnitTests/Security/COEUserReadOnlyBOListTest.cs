using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEUserReadOnlyBOListTest
    /// </summary>
    [TestClass]
    public class COEUserReadOnlyBOListTest
    {

        #region Variables

        List<int> lstPersonId = new List<int>();

        #endregion Variables

        public COEUserReadOnlyBOListTest()
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

        [TestMethod()]
        public void GetUserByID_IfPersonIdExistsInList()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();

            if (SecurityHelper.GetUserDetails())
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByID(SecurityHelper.intPersonId);

                Assert.IsTrue(theCOEUserReadOnlyBO != null, "GetUserByID did not" +
                        " return the expected value.");
            }
        }

        [TestMethod()]
        public void GetUserByID_IfPersonIdDoesNotExistInList()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();

            COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByID(9666);

            Assert.IsTrue(theCOEUserReadOnlyBO == null, "GetUserByID did not" +
                    " return null value.");
        }

        [TestMethod()]
        public void GetUserByUserID_IfUserIdExistsInList()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();
            if (SecurityHelper.GetUserDetails())
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByUserID(SecurityHelper.strUserID);

                Assert.IsTrue(theCOEUserReadOnlyBO != null, "GetUserByUserID did not" +
                        " return the expected value.");
            }
        }

        [TestMethod()]
        public void GetUserByUserID_IfUserIdDoesNotExistInList()        
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();

            COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByUserID("CSSADMIN_test");

            Assert.IsTrue(theCOEUserReadOnlyBO == null, "GetUserByUserID did not" +
                    " return null value.");
        }

        [TestMethod()]
        public void GetListByUserID()
        {
            lstPersonId = SecurityHelper.GetPersonIdList();                  
            COEUserReadOnlyBOList theCOEUserReadOnlyBOList = COEUserReadOnlyBOList.GetListByUserID(lstPersonId);
            Assert.AreEqual(theCOEUserReadOnlyBOList.Count, lstPersonId.Count);
        }

        [TestMethod()]
        public void AddUser()
        {
            COEUserReadOnlyBOList users = null;            
            users = COEUserReadOnlyBOList.GetList();

            if (SecurityHelper.GetUserDetails())
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByUserID(SecurityHelper.strUserID);
                users.RemoveUser(theCOEUserReadOnlyBO);
                int intUsersCount = users.Count;

                users.AddUser(theCOEUserReadOnlyBO);

                Assert.AreEqual(intUsersCount + 1, users.Count);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))] 
        public void AddUser_IfUserAlreadyExist()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();
            if (SecurityHelper.GetUserDetails())
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByUserID(SecurityHelper.strUserID);
                users.AddUser(theCOEUserReadOnlyBO);
            }
        }

        [TestMethod()]
        public void RemoveUser()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();
            int intUsersCount = users.Count;
            if (SecurityHelper.GetUserDetails())
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByUserID(SecurityHelper.strUserID);
                users.RemoveUser(theCOEUserReadOnlyBO);

                Assert.AreEqual(intUsersCount - 1, users.Count);
            }
        }
     
        [TestMethod()]
        public void Contains_IfPersonIdExistsInList()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();
            if (SecurityHelper.GetUserDetails())
            {
                bool blnFlag = users.Contains(SecurityHelper.intPersonId);
                Assert.IsTrue(blnFlag == true);
            }
        }

        [TestMethod()]
        public void Contains_IfPersonIdDoesNotExistInList()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();
            
            bool blnFlag = users.Contains(966);
            Assert.IsTrue(blnFlag == false);
        }

        #endregion

    }
}
