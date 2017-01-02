using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList and is intended
    ///to contain all CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList Unit Tests
    ///</summary>
    [TestClass()]
    public class PickListNameValueListTest
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

        /// <summary>
        ///A test for GetAllPickListDomains ()
        ///</summary>
        [TestMethod()]
        public void GetAllPickListDomainsTest()
        {
            PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetAllPickListDomains();            
            Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetAllPickListDomains did not return the expected value.");
            this.DisplayResultsInConsole(actual);
        }

        /// <summary>
        /// A test for GetAllPickListDomains if database is passed as a parameter
        ///</summary>
        [TestMethod()]
        public void GetAllPickListDomainsTest_ByDatabaseName()
        {
            string databaseName = "REGDB";
            PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetAllPickListDomains(databaseName);            
            Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetAllPickListDomains did not return the expected value.");

            this.DisplayResultsInConsole(actual);
        }

        /// <summary>
        ///A test for GetPickListNameValueList if query is passed as a parameter
        ///</summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_BySql()
        {
            string sql = "SELECT USER_ID, USER_CODE FROM COEDB.PEOPLE";
            PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(sql);
            Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

            this.DisplayResultsInConsole(actual);
        }

        /// <summary>
        ///A test for GetPickListNameValueList if database and domain Id are passed as a parameters
        ///</summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseNameAndDomainId()
        {
            string databaseName = "REGDB";
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {
                int domainId = PickListPickerHelper.intDomainId;
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(databaseName, domainId);
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");
                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        ///A test for GetPickListNameValueList if database and domain name are passed as a parameters
        ///</summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseNameAndDomainName()
        {
            string databaseName = "REGDB";
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {                
                string domainName = PickListPickerHelper.strDomainName;
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(databaseName, domainName);                
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// This method is used to check outputs from test methods
        /// </summary>
        /// <param name="actual"></param>
        private void DisplayResultsInConsole(PickListNameValueList actual)
        {
            foreach (PickListNameValueList.NameValuePair pair in actual)
            {
                Console.WriteLine("DomainID: {0}, Key: {1}, Value: {2}", actual.DomainID, pair.Key, pair.Value);
            }
        }

        [TestMethod()]
        public void InvalidateCache()
        {
            PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetAllPickListDomains();
            PrivateObject thePrivateObject = new PrivateObject(actual);

            PickListNameValueList.InvalidateCache();
            object theField = thePrivateObject.GetField("_list", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNull(theField);

        }

        /// <summary>
        /// Test for GetPickListNameValueList, Parameter (PickListStatus - Empty)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_Empty()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {                
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, "", PickListPickerHelper.intUnitId);                
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// Test for GetPickListNameValueList, Parameter(PickListStatus - InActive)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_InActive()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {                
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, PickListStatus.InActive, PickListPickerHelper.intUnitId);             
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// Test for GetPickListNameValueList, Parameter(PickListStatus - Active)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_Active()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, PickListStatus.Active, PickListPickerHelper.intUnitId);
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// Test for GetPickListNameValueList, Parameter(PickListStatus - All)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_All()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, PickListStatus.All, PickListPickerHelper.intUnitId);
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// Test for GetPickListNameValueList, Parameter(PickListStatus - None)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_None()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, PickListStatus.None, PickListPickerHelper.intUnitId);
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// Test for GetPickListNameValueList, Parameter(PickListStatus - boolean - true)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_Boolean()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, true, PickListPickerHelper.intUnitId);
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

        /// <summary>
        /// Test for GetPickListNameValueList, ParameterPickListStatus - integer - 1)
        /// </summary>
        [TestMethod()]
        public void GetPickListNameValueListTest_ByDatabaseIdDomainId_Integer()
        {
            if (PickListPickerHelper.GetPickListDoamainDetails())
            {
                PickListPickerHelper.GetVW_UnitDetails();
                PickListNameValueList actual = CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList(PickListPickerHelper.intDomainId, 1, PickListPickerHelper.intUnitId);
                Assert.IsTrue(actual.Count > 0, "CambridgeSoft.COE.Framework.COEPickListPickerService.PickListNameValueList.GetPickListNameValueList did not return the expected value.");

                this.DisplayResultsInConsole(actual);
            }
            else
            {
                Assert.Fail("Domain does not exist");
            }
        }

    }
}
