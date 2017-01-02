using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using RegistrationAdmin.Services.MSUnitTests.Helpers;
using System.Collections.Generic;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for AddInListTest and is intended
    ///to contain all AddInListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddInListTest
    {
        XmlDocument addInListXmlDocument;
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
        public void MyTestInitialize()
        {
            addInListXmlDocument = XMLDataLoader.LoadXmlDocument("AddInList.xml");
            if (addInListXmlDocument == null)
            {
                Assert.Fail("xml document can not be loaded");
            }
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        #endregion


        /// <summary>
        ///A test for NewAddInList
        ///</summary>
        [TestMethod()]
        public void NewAddInListTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList actual;
            actual = AddInList.NewAddInList(addInList);
            Assert.IsNotNull(actual, string.Format("{0}.NewAddInList(addInList) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(actual)));
        }

        /// <summary>
        ///A test for ExistAddIndCheck - add in present test
        ///</summary>
        [TestMethod()]
        public void ExistAddIndCheck_AddInPresentTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            XmlNode addInNode = addInList.ChildNodes[0];

            AddIn addInToCheck = AddIn.NewAddIn(addInNode, false); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ExistAddIndCheck(addInToCheck);
            Assert.IsTrue(actual, string.Format("{0}.ExistAddIndCheck(addInToCheck) failed", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }

        /// <summary>
        ///A test for ExistAddIndCheck - add in absent test
        ///</summary>
        [TestMethod()]
        public void ExistAddIndCheck_AddInNotPresentTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            XmlNode addInNode = addInList.ChildNodes[0];

            AddIn addInToCheck = AddIn.NewAddIn(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ExistAddIndCheck(addInToCheck);
            Assert.IsFalse(actual, string.Format("{0}.ExistAddIndCheck(addInToCheck) failed", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }

        /// <summary>
        ///A test for ExistAddInFriendlyName - check from friendly name of add in
        ///</summary>
        [TestMethod()]
        public void ExistAddInFriendlyNameTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            XmlNode addInNode = addInList.ChildNodes[0];
            string friendlyName = addInNode.Attributes["friendlyName"].Value;
            
            bool actual;
            actual = target.ExistAddInFriendlyName(friendlyName);
            Assert.IsTrue(actual, string.Format("{0}.ExistAddInFriendlyName(friendlyName) failed", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }

        /// <summary>
        ///A test for ClearDeletedList
        ///</summary>
        [TestMethod()]
        public void ClearDeletedListTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            //remove first item so that deleted list will contain one add in entry
            target.RemoveAt(0);

            PrivateObject obj = new PrivateObject(target);
            List<AddIn> p = (List<AddIn>)obj.GetProperty("DeletedList");
            if (p != null)
            {
                Assert.IsTrue(p.Count == 1, string.Format("{0} failed to generate the deleted list", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
            }

            target.ClearDeletedList();
            if (p != null)
            {
                Assert.IsTrue(p.Count == 0, string.Format("{0} failed to clear the deleted list", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
            }
        }

        /// <summary>
        ///A test for AddAddin - add existing add in
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Csla.Validation.ValidationException),"failed to raise the expected exception")]
        public void AddAddin_ShouldRaiseValidationExceptionTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            //create existing add in to test
            AddIn addIn = AddIn.NewAddIn();
            addIn.Assembly = string.Empty;
            addIn.ClassName = string.Empty;
            target.AddAddin(addIn);
        }

        /// <summary>
        ///A test for AddAddin - add existing add in
        ///</summary>
        [TestMethod()]
        public void AddAddin_ShouldNotAddExistingAddInTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            XmlNode existingAddInNode = addInList.ChildNodes[0];

            //create existing add in to test
            AddIn addIn = AddIn.NewAddIn(existingAddInNode, false);

            target.AddAddin(addIn);
            Assert.IsFalse(addIn.IsNew, string.Format("{0}.AddAddin(addIn) failed and added already existing add in to list", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }

        /// <summary>
        ///A test for AddAddin - add new add in
        ///</summary>
        [TestMethod()]
        public void AddAddin_ShouldAddNewAddinTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            XmlNode existingAddInNode = addInList.ChildNodes[0];

            //create existing add in to test
            AddIn addIn = AddIn.NewAddIn(existingAddInNode, false);

            //remove existing add in from collection
            target.RemoveAt(0);

            //add same add in again
            target.AddAddin(addIn);

            Assert.IsTrue(addIn.IsNew, string.Format("{0}.AddAddin(addIn) failed to add new add in to list", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }

        /// <summary>
        ///A test for UpdateSelf
        ///</summary>
        [TestMethod()]
        public void UpdateSelf_ShouldUpdatedDeletedAddinTest()
        {
            string expected = "yes";
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(addInListXmlDocument); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            XmlNode addInList = xmlNodeList[0]; // TODO: Initialize to an appropriate value
            AddInList target;
            target = AddInList.NewAddInList(addInList);

            XmlNode existingAddInNode = addInList.ChildNodes[0];

            //create existing add in to test
            AddIn addIn = AddIn.NewAddIn(existingAddInNode, false);

            //remove existing add in from collection
            target.RemoveAt(0);

            string updatedXmlString = target.UpdateSelf();

            XmlDocument updatedDoc = new XmlDocument();
            XmlNode rootNode = updatedDoc.CreateNode(XmlNodeType.Element, "UpdatedNodes", "");
            rootNode.InnerXml = updatedXmlString;
            updatedDoc.AppendChild(rootNode);
            XmlNode nodeOfInterest = updatedDoc.DocumentElement.SelectSingleNode("/UpdatedNodes/AddIn[@friendlyName='Structure Normalization']");
            string actual = nodeOfInterest.Attributes["delete"].Value;

            Assert.AreEqual<string>(expected, actual, string.Format("{0}.UpdateSelf() failed to set delete attribute of deleted addin", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }

        /// <summary>
        ///A test for NewAddInList
        ///</summary>
        [TestMethod()]
        public void NewAddInList_ShouldCreateNewAddInListTest()
        {
            AddInList target;
            target = AddInList.NewAddInList();
            Assert.IsNotNull(target, string.Format("{0}.NewAddInList() failed to create default addin list", Helpers.Helpers.GetFullTypeNameMessage<AddInList>(target)));
        }
    }
}
