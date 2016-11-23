using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using RegistrationAdmin.Services.MSUnitTests.Helpers;
using System.Text;
using System.Reflection;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for AddInTest and is intended
    ///to contain all AddInTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddInTest
    {
        XmlDocument addInXmlDoc;
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void AddInTestInitialize()
        {
            addInXmlDoc = XMLDataLoader.LoadXmlDocument("AddIn.xml");
            if (addInXmlDoc == null)
                Assert.Fail("Xml document reading error");
           
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void AddInTestCleanup()
        {
        }
        
        #endregion

        /// <summary>
        ///A test for creating a new NewAddIn and test for not null condition
        ///</summary>
        [TestMethod()]
        public void NewAddIn_ShouldCreateAddInWithSpecifiedParametersTest()
        {
            XmlNodeList nodeList = XMLDataLoader.PrepareXMLNodeList(addInXmlDoc);
            if (nodeList == null)
                Assert.Fail("node list null");
            if (nodeList.Count == 0)
                Assert.Fail("Add in not present");

            string assembly = AddInTestHelpers.GetAttributeValue(nodeList[0], "assembly"); // TODO: Initialize to an appropriate value
            string className = AddInTestHelpers.GetAttributeValue(nodeList[0], "class"); // TODO: Initialize to an appropriate value
            string friendlyName = AddInTestHelpers.GetAttributeValue(nodeList[0], "friendlyName"); // TODO: Initialize to an appropriate value
            EventList eventList = null; // TODO: Initialize to an appropriate value
            string configuration =string.Empty; // TODO: Initialize to an appropriate value
            string nameSpace = string.Empty; // TODO: Initialize to an appropriate value
            bool enable = false; // TODO: Initialize to an appropriate value
            bool required = false; // TODO: Initialize to an appropriate value
            AddIn actual;
            actual = AddIn.NewAddIn(assembly, className, friendlyName, eventList, configuration, nameSpace, enable, required);
            Assert.IsNotNull(actual, string.Format("{0}.NewAddIn(assembly, className, friendlyName, eventList, configuration, nameSpace, enable, required) failed to return expected value", Helpers.Helpers.GetFullTypeNameMessage<AddIn>(actual)));
        }

        /// <summary>
        ///A test for MarkAsNew
        ///</summary>
        [TestMethod()]
        public void MarkAsNewTest()
        {
            XmlNodeList nodeList = XMLDataLoader.PrepareXMLNodeList(addInXmlDoc);
            if (nodeList == null)
                Assert.Fail("node list null");
            if (nodeList.Count == 0)
                Assert.Fail("Add in not present");
            XmlNode xmlNode = nodeList[0]; // TODO: Initialize to an appropriate value
            bool isNew = false; // TODO: Initialize to an appropriate value
            AddIn addInObject;
            addInObject = AddIn.NewAddIn(xmlNode, isNew);
            addInObject.MarkAsNew();
            bool actual = addInObject.IsNew;
            Assert.IsTrue(actual, string.Format("{0}.IsNew did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<AddIn>(addInObject)));
        }

        /// <summary>
        ///A test for NewAddIn with empty addin constructor
        ///</summary>
        [TestMethod()]
        public void NewAddIn_ShouldCreateObjectWithEmptyConstructorTest()
        {
            AddIn actual;
            actual = AddIn.NewAddIn();
            Assert.IsNotNull(actual, string.Format("{0}.AddIn.NewAddIn() did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<AddIn>(actual)));
        }

        /// <summary>
        ///A test for NewAddIn
        ///</summary>
        [TestMethod()]
        public void NewAddIn_ShouldCreateAddinWithXMLNodeListTest()
        {
            XmlNodeList nodeList = XMLDataLoader.PrepareXMLNodeList(addInXmlDoc);
            if (nodeList == null)
                Assert.Fail("node list null");
            if (nodeList.Count == 0)
                Assert.Fail("Add in not present");
            XmlNode xmlNode = nodeList[0]; // TODO: Initialize to an appropriate value
            bool isNew = false; // TODO: Initialize to an appropriate value
            AddIn actual;
            actual = AddIn.NewAddIn(xmlNode, isNew);
            Assert.IsNotNull(actual, string.Format("{0}.NewAddIn(xmlNode, isNew) did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<AddIn>(actual)));
        }

        /// <summary>
        ///A test for UpdateSelf
        ///</summary>
        [TestMethod()]
        public void UpdateSelf_ShouldUpdateSelfTest()
        {
            XmlNodeList nodeList = XMLDataLoader.PrepareXMLNodeList(addInXmlDoc);
            if (nodeList == null)
                Assert.Fail("node list null");
            if (nodeList.Count == 0)
                Assert.Fail("Add in not present");
            XmlNode xmlNode = nodeList[0]; // TODO: Initialize to an appropriate value
            bool isNew = false; // TODO: Initialize to an appropriate value
            AddIn target;
            target = AddIn.NewAddIn(xmlNode, isNew); // TODO: Initialize to an appropriate value
            string actual;
            //change some attributes of AddIn
            target.MarkAsNew();
            target.IsEnable = true;
            string addinUpdatedString = target.UpdateSelf();

            XmlDocument updatedDoc = new XmlDocument();
            XmlNode rootNode = updatedDoc.CreateNode(XmlNodeType.Element, "UpdatedNodes", "");
            rootNode.InnerXml = addinUpdatedString;
            updatedDoc.AppendChild(rootNode);
            string expected = "yes";
            XmlNode nodeOfInterest = updatedDoc.DocumentElement.SelectSingleNode("/UpdatedNodes/AddIn");//[@enabled='" + eventName + "']");
            actual = nodeOfInterest.Attributes["enabled"].Value;
            Assert.AreEqual<string>(expected, actual, string.Format("{0}.UpdateSelf() did not return expected value", Helpers.Helpers.GetFullTypeNameMessage<AddIn>(target)));
        }
    }
}
