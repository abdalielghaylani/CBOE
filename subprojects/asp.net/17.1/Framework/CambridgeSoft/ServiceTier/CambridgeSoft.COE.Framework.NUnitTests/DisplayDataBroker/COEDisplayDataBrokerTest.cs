﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
namespace CambridgeSoft.COE.Framework.NUnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker and is intended
    ///to contain all CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker Unit Tests
    ///</summary>
    [TestFixture]
    public class COEDisplayDataBrokerTest
    {

        string assemblyName = "CambridgeSoft.COE.Framework";
        string className = "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList";
        string methodName = "GetAllPickListDomains";
        private string pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\DisplayData XML\");
        
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
        //
        //[TestFixtureSetUp]
        //public static void MyClassInitialize()
        //{
            
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[TestFixtureTearDown]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        [SetUp]
        public void MyTestInitialize()
        {
            CambridgeSoft.COE.Framework.Common.COEDatabaseName.Set("REGDB");
        }
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TearDown]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetDisplayData (string, string)
        ///</summary>
        ///
        [Test]
        public void GetDisplayDataTestWithoutAssembly()
        {
            IKeyValueListHolder actual = (IKeyValueListHolder) COEDisplayDataBroker.GetDisplayData(className, methodName);
            Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithoutAssembly did not return the expected value.");
        }

        [Test]
        public void GetDisplayDataTestWithEmptyMethodName()
        {
            IKeyValueListHolder actual = (IKeyValueListHolder)COEDisplayDataBroker.GetDisplayData(className, String.Empty);
            Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithoutAssembly did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDisplayData (string, string, string)
        ///</summary>
        [Test]
        public void GetDisplayDataTestWithAssembly()
        {
            IKeyValueListHolder actual = (IKeyValueListHolder) COEDisplayDataBroker.GetDisplayData(assemblyName, className, methodName);
            Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithAssembly did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDisplayData (string, string, string, object[])
        ///</summary>
        [Test]
        public void GetDisplayDataTestWithParamValues()
        {
            object[] paramValues = new object[2] { "REGDB", "Units" };
            methodName = "GetPickListNameValueList";
            IKeyValueListHolder actual = (IKeyValueListHolder) COEDisplayDataBroker.GetDisplayData(assemblyName, className, methodName, paramValues);
          //  Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithParamValues did not return the expected value.");
            Assert.IsNotNull(actual.KeyValueList, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithParamValues did not return the expected value.");
        }

     

        /// <summary>
        ///A test for GetDisplayData (string, string, string, object[], string[])
        ///</summary>
        [Test]
        public void GetDisplayDataTestWithParamValuesAndTypes()
        {
            object[] paramValues = new object[2] { "REGDB", "1" };
            string[] paramTypes = new string[2] { "System.String", "System.Int32" };
            methodName = "GetPickListNameValueList";
            IKeyValueListHolder actual = (IKeyValueListHolder) COEDisplayDataBroker.GetDisplayData(assemblyName, className, methodName, paramValues, paramTypes);
            //Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithParamValuesAndTypes did not return the expected value.");
            Assert.IsNotNull(actual.KeyValueList, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithParamValues did not return the expected value.");
        }


        [Test]
        [ExpectedException(typeof(Exception))]
        public void GetDisplayDataTestWithmismatchParamValuesAndTypes()
        {
            object[] paramValues = new object[2] { "REGDB", "1" };
            string[] paramTypes = new string[3] { "System.String", "System.Int32","" };
            methodName = "GetPickListNameValueList";
            IKeyValueListHolder actual = (IKeyValueListHolder)COEDisplayDataBroker.GetDisplayData(assemblyName, className, methodName, paramValues, paramTypes);
           // Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithParamValuesAndTypes did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDisplayData (string)
        ///</summary>
        [Test]
        public void GetDisplayDataTestWithDisplayDataSTRING()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + "DisplayDataFormGroupTest.xml");
            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("COE", "COE.FormGroup");
            string displayData = doc.SelectSingleNode("//COE:displayData", manager).OuterXml;
            IKeyValueListHolder actual = (IKeyValueListHolder) COEDisplayDataBroker.GetDisplayData(displayData);
            Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithDisplayDataSTRING did not return the expected value.");
        }


        /// <summary>
        ///A test for GetDisplayData (string) Method Name is empty
        ///</summary>
        [Test]
        [ExpectedException(typeof(Exception))]
        public void GetDisplayDataTestWithEmptyMethod()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + "DisplayMethodTest.xml");
            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("COE", "COE.FormGroup");
            string displayData = doc.SelectSingleNode("//COE:displayData", manager).OuterXml;
            IKeyValueListHolder actual = (IKeyValueListHolder)COEDisplayDataBroker.GetDisplayData(displayData);
            Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithDisplayDataSTRING did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDisplayData (FormGroup.DisplayData)
        ///</summary>
        [Test]
        public void GetDisplayDataTestWithDisplayDataTYPE()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + "DisplayDataFormGroupTest.xml");
            FormGroup form = FormGroup.GetFormGroup(doc.InnerXml);

            IKeyValueListHolder actual = (IKeyValueListHolder) COEDisplayDataBroker.GetDisplayData(form.QueryForms[0].Forms[0].LayoutInfo[0].DisplayData);
            Assert.IsTrue(actual.KeyValueList.Count > 0, "CambridgeSoft.COE.Framework.COEListBrokerService.COEListBroker.GetDisplayDataTestWithDisplayDataTYPE did not return the expected value.");
        }
    }


}
