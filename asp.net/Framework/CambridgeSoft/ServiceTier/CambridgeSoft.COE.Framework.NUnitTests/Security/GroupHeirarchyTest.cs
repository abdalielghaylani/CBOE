using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework; 
using System.Xml;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    [TestFixture]
    public class GroupHeirarchyTest
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
        public GroupHeirarchyTest()
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

        // Use TestCleanup to run code after each test has ru
        [TearDown]
        public void MyTestCleanup() { }
        #endregion

        #region Test Methods

        [Test]
        public void GetXML_LoadingXML()
        {
            XmlDocument theXmlDocument = GroupHeirarchy.GetXML();
            Assert.IsNotNull(theXmlDocument, "GroupHeirarchy.GetXML() did not return the expected value.");
        }

        [Test]
        public void GetXML_LoadingXMLWithGroupID()
        {
            if (SecurityHelper.GetGroupDetails())
            {
                XmlDocument theXmlDocument = GroupHeirarchy.GetXML(SecurityHelper.intGroupId);
                bool blnResult = SecurityHelper.GroupExistInXML(theXmlDocument.InnerXml, "GROUP_ID", SecurityHelper.intGroupId.ToString());
                Assert.IsTrue(blnResult, "GroupHeirarchy.GetXML(int groupID) did not return the expected value.");
            }
            else
                Assert.Fail("Group ID does not exists.");
        }

        #endregion
    }
}
