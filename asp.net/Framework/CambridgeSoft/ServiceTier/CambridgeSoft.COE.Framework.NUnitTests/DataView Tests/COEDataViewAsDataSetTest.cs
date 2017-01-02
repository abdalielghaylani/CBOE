using CambridgeSoft.COE.Framework.COEDataViewService;
using NUnit.Framework;
using System;
using System.Data;

namespace CambridgeSoft.COE.Framework.COEDataViewService.UnitTests
{
    /// <summary>
    ///This is a test class for COEDataViewAsDataSetTest and is intended
    ///to contain all COEDataViewAsDataSetTest Unit Tests
    ///</summary>
    [TestFixture]
    public class COEDataViewAsDataSetTest
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
        ///A test for GetMasterTables
        ///</summary>
        [Test]
        public void GetMasterTablesTest()
        {
            DataSet expected = new DataSet(); // TODO: Initialize to an appropriate value
            DataSet actual;
            actual = COEDataViewAsDataSet.GetMasterTables();
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Method : COEDataViewAsDataSet.GetMasterTables() not implemented.");
        }

        /// <summary>
        ///A test for GetPublishedTables
        ///</summary>
        [Test]
        public void GetPublishedTablesTest()
        {
            DataSet expected = new DataSet(); // TODO: Initialize to an appropriate value
            DataSet actual;
            actual = COEDataViewAsDataSet.GetPublishedTables();
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Method : COEDataViewAsDataSet.GetPublishedTables() not implemented.");
        }
    }
}
