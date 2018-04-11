using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Data;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for COEUserReadOnlyBOListTest
    /// </summary>
    [TestFixture]
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

        [Test]
        public void GetUserByID_IfPersonIdDoesNotExistInList()
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();

            COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByID(9666);

            Assert.IsTrue(theCOEUserReadOnlyBO == null, "GetUserByID did not" +
                    " return null value.");
        }

        [Test]
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

        [Test]
        public void GetUserByUserID_IfUserIdDoesNotExistInList()        
        {
            COEUserReadOnlyBOList users = null;
            users = COEUserReadOnlyBOList.GetList();

            COEUserReadOnlyBO theCOEUserReadOnlyBO = users.GetUserByUserID("CSSADMIN_test");

            Assert.IsTrue(theCOEUserReadOnlyBO == null, "GetUserByUserID did not" +
                    " return null value.");
        }

        [Test]
        public void GetListByUserID()
        {
            lstPersonId = SecurityHelper.GetPersonIdList();                  
            COEUserReadOnlyBOList theCOEUserReadOnlyBOList = COEUserReadOnlyBOList.GetListByUserID(lstPersonId);
            Assert.AreEqual(theCOEUserReadOnlyBOList.Count, lstPersonId.Count);
        }

        [Test]
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

        [Test]
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

        [Test]
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
     
        [Test]
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

        [Test]
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
