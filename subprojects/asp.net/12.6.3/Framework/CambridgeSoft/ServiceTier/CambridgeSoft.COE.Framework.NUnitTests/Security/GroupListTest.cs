using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    [TestFixture]
    public class GroupListTest
    {
        #region Variables
        private TestContext testContextInstance;

        #endregion

        #region Properties

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
        #endregion

        #region Constructor
        public GroupListTest()
        {
        }
        #endregion

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
        public void GetList_LoadingGroupListWithGroupOrgIDAndGroupID()
        {
            if (SecurityHelper.GetGroupOrgID() && SecurityHelper.GetGroupDetails())
            {
                GroupList theGroupList = GroupList.GetList(SecurityHelper.intGroupOrgId, SecurityHelper.intGroupId);
                Assert.IsNotNull(theGroupList, "GroupList.GetList(int groupOrgID, int groupID) did not return the expected value.");
            }
        }

        [Test]      
        public void InvalidateCache()
        {
            GroupList.InvalidateCache();
            Type theType = typeof(GroupList);
            FieldInfo theFieldInfo = theType.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            object theResult = theFieldInfo.GetValue(null);
            Assert.IsNull(theResult, "GroupList.InvalidateCache() did not return the expected value.");
        }

        #endregion
    }
}
