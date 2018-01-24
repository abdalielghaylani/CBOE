using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for EventTest and is intended
    ///to contain all EventTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EventTest
    {
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for NewEvent creation
        ///</summary>
        [TestMethod()]
        public void NewEvent_CreateNewInsertingEventTest()
        {
            string eventName = "Inserting"; // TODO: Initialize to an appropriate value
            string eventHandler = "OnInsertHandler"; // TODO: Initialize to an appropriate value
            bool IsNew = false; // TODO: Initialize to an appropriate value
            Event actual;
            actual = Event.NewEvent(eventName, eventHandler, IsNew);
            Assert.IsNotNull(actual, "Event creation failed");
        }

        /// <summary>
        ///A test for NewEvent - ID property
        ///</summary>
        [TestMethod()]
        public void ID_GetEventIDTest()
        {
            string eventName = "Inserting"; // TODO: Initialize to an appropriate value
            string eventHandler = "OnInsertHandler"; // TODO: Initialize to an appropriate value
            bool IsNew = false; // TODO: Initialize to an appropriate value
            Event actual;
            actual = Event.NewEvent(eventName, eventHandler, IsNew);
            int id = actual.Id;
            Assert.IsNotNull(actual, "Event creation failed");
            Assert.IsTrue(id == 0, "id return error");
        }

        /// <summary>
        ///A test for NewEvent - EventName property
        ///</summary>
        [TestMethod()]
        public void EventName_GetEventnameTest()
        {
            string eventName = "Inserting"; // TODO: Initialize to an appropriate value
            string eventHandler = "OnInsertHandler"; // TODO: Initialize to an appropriate value
            bool IsNew = false; // TODO: Initialize to an appropriate value
            Event actual;
            string expected = "Inserting";
            actual = Event.NewEvent(eventName, eventHandler, IsNew);
            string eventNameProperty = actual.EventName;
            Assert.IsNotNull(actual, "Event creation failed");
            Assert.AreEqual<string>(eventNameProperty, expected, "event name not match");
        }

        /// <summary>
        ///A test for NewEvent - EventHandlerName property
        ///</summary>
        [TestMethod()]
        public void EventHandler_GetEventHandlerTest()
        {
            string eventName = "Inserting"; // TODO: Initialize to an appropriate value
            string eventHandler = "OnInsertHandler"; // TODO: Initialize to an appropriate value
            bool IsNew = false; // TODO: Initialize to an appropriate value
            Event actual;
            string expected = "OnInsertHandler";
            actual = Event.NewEvent(eventName, eventHandler, IsNew);
            string eventHandlerProperty = actual.EventHandler;
            Assert.IsNotNull(actual, "Event creation failed");
            Assert.AreEqual<string>(eventHandlerProperty, expected, "event handler name not match");
        }

        /// <summary>
        ///A test for NewEvent creation
        ///</summary>
        [TestMethod()]
        public void NewEvent_CreateNewInsertingEventWithPropertiesTest()
        {
            string eventName = "Inserting"; // TODO: Initialize to an appropriate value
            string eventHandler = "OnInsertHandler"; // TODO: Initialize to an appropriate value
            bool IsNew = true; // TODO: Initialize to an appropriate value
            Event actual;
            actual = Event.NewEvent(IsNew);
            actual.EventName = eventName;
            Assert.AreEqual<string>(eventName, actual.EventName, "event name set failed");
            actual.EventHandler = eventHandler;
            Assert.AreEqual<string>(eventHandler, actual.EventHandler, "event handler name set failed");
        }
    }
}
