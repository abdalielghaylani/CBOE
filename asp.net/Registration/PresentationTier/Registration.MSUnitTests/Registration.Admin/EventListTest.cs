using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using RegistrationAdmin.Services.MSUnitTests.Helpers;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for EventListTest and is intended
    ///to contain all EventListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EventListTest
    {
        XmlDocument eventListXmlDoc;

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
        public void EventListTestInitialize()
        {
            eventListXmlDoc = XMLDataLoader.LoadXmlDocument("EventList.xml");
            if (eventListXmlDoc == null)
            {
                Assert.Fail("xml document can not be loaded");
            }
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void EventListTestCleanup()
        {

        }
        
        #endregion

        /// <summary>
        ///A test for NewEventList - create event list from xml document
        ///</summary>
        [TestMethod()]
        public void NewEventList_CreateEventListTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(eventListXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");

            //EventList expected = null; // TODO: Initialize to an appropriate value
            EventList actual;
            int expected = xmlNodeList.Count; 
            actual = EventList.NewEventList(xmlNodeList);
            Assert.IsNotNull(actual, "Error creating event list");
            Assert.AreEqual<int>(expected, actual.Count, "Event list failed");
        }

        /// <summary>
        ///A test for RemoveItem - remove item from event list
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CambridgeSoft.COE.RegistrationAdmin.Services.dll")]
        public void RemoveItemTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(eventListXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");
            EventList target = EventList.NewEventList(xmlNodeList); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            target.RemoveAt(index);
            Assert.AreEqual<int>(xmlNodeList.Count - 1, target.Count, "remove item form event list failed");
        }

        /// <summary>
        ///A test for UpdateSelf
        ///</summary>
        [TestMethod()]
        public void UpdateSelf_CheckUpdatedXMLStringTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(eventListXmlDoc); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(xmlNodeList, "xml node list null");
            EventList target = EventList.NewEventList(xmlNodeList); // TODO: Initialize to an appropriate value
            bool addInIsNew = true; // TODO: Initialize to an appropriate value
            string expected = "yes"; // TODO: Initialize to an appropriate value
            string actual;
            target.RemoveAt(0);
            actual = target.UpdateSelf(addInIsNew);
            XmlDocument updatedDoc = new XmlDocument();
            XmlNode rootNode = updatedDoc.CreateNode(XmlNodeType.Element, "UpdatedNodes", "");
            rootNode.InnerXml = actual;
            updatedDoc.AppendChild(rootNode);
            string eventName = xmlNodeList[0].Attributes["eventName"].Value;
            XmlNode nodeOfInterest = updatedDoc.DocumentElement.SelectSingleNode("/UpdatedNodes/Event[@eventName='" + eventName + "']");
            actual = nodeOfInterest.Attributes["delete"].Value;
            Assert.AreEqual(expected, actual,"node deletion failed");
        }

        /// <summary>
        ///A test for ExistEventCheck
        ///</summary>
        [TestMethod()]
        public void ExistEventCheck_EventExistsTest()
        {
            XmlNodeList xmlNodeList = XMLDataLoader.PrepareXMLNodeList(eventListXmlDoc);
            Assert.IsNotNull(xmlNodeList, "xml node list null");
            EventList target = EventList.NewEventList(xmlNodeList); // TODO: Initialize to an appropriate value
            Event evtToCheck = target[0];
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ExistEventCheck(evtToCheck);
            Assert.AreEqual(expected, actual, "event not exists in collection");
        }
    }
}
