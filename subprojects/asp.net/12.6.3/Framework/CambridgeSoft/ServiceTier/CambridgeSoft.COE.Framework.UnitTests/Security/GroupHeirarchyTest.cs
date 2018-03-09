using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting; 
using System.Xml;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    [TestClass()]
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

        // Use TestCleanup to run code after each test has ru
        [TestCleanup()]
        public void MyTestCleanup() { }
        #endregion

        #region Test Methods

        [TestMethod]
        public void GetXML_LoadingXML()
        {
            XmlDocument theXmlDocument = GroupHeirarchy.GetXML();
            Assert.IsNotNull(theXmlDocument, "GroupHeirarchy.GetXML() did not return the expected value.");
        }

        [TestMethod]
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
