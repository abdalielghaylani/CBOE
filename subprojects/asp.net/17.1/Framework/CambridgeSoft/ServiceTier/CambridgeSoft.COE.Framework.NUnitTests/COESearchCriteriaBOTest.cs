﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using NUnit.Framework;
using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
namespace CambridgeSoft.COE.Framework.COESearchCriteriaService.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.COEGenericObjectStorageService.COEGenericObjectStorageBO and is intended
    ///to contain all CambridgeSoft.COE.Framework.COEGenericObjectStorageService.COEGenericObjectStorageBO Unit Tests
    ///</summary>
    [TestFixture]
    public class COESearchCriteriaBOTest
    {
        private string pathToXmls = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("CambridgeSoft.COE.Framework.NUnitTests")) + @"CambridgeSoft.COE.Framework.NUnitTests" + @"\TestXML";
        private string databaseName = "SAMPLE";
        private DALFactory _dalFactory = new DALFactory();

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


        [SetUp]
        public void MyTestInitialize()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;

            string userName = "cssadmin";
            string password = "cssadmin";
            bool result = COEPrincipal.Login(userName, password);

        }

        /// <summary>
        ///A test for New ()
        ///</summary>
        [Test]
        public void StoreSearchCriteriaTest()
        {
            int id = 0;
            try
            {
                //this should create a new object
                COESearchCriteriaBO searchCriteriaBOObject = StoreSearchCriteria();
                id = searchCriteriaBOObject.ID;

            }
            catch(Exception) { }

            Assert.IsTrue(id > 0, "CambridgeSoft.COE.Framework.COEGenericObjectStorageService.COEGenericObjectStorag" +
                    "eBO.New did not return the expected value.");
        }


        private COESearchCriteriaBO StoreSearchCriteria()
        {
            //this should create a new object
            COESearchCriteriaBO myObject = COESearchCriteriaBO.New(databaseName);
            myObject.Name = "temp";
            myObject.Description = "temp";
            //this really should come from the logged in user..
            myObject.UserName = "lbialic";
            myObject.DatabaseName = "MAIN.SAMPLE";
            myObject.SearchCriteria = BuildSearchCriteriaFromXML();
            //this is where it get's persisted
            myObject = myObject.Update();
            return myObject;
        }

        /// <summary>
        ///A test for Get ()
        ///</summary>
        [Test]
        public void GetSearchCriteriaTest()
        {

            ////First make sure their is an object to get
            COESearchCriteriaBO searchCriteriaBO = StoreSearchCriteria();
            int id = searchCriteriaBO.ID;
            //nullify
            searchCriteriaBO = null;
            try
            {
                //this should create a new object
                searchCriteriaBO = COESearchCriteriaBO.Get(SearchCriteriaType.TEMP, id);

            }
            catch(Exception) { }
            Assert.IsTrue(searchCriteriaBO.ID == id, "GetsearchCriteriaBOTest did not return the expected value.");
        }



        public SearchCriteria BuildSearchCriteriaFromXML()
        {
            //Load SearchCriteriaSerialized.XML
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + "\\SearchCriteriaForTests.xml");
            SearchCriteria coeSearchCriteria = new SearchCriteria();
            coeSearchCriteria.GetFromXML(doc);
            return coeSearchCriteria;
        }
    }


}