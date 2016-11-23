using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.UnitTests.Common.SqlGenerator.NonQueries
{
    /// <summary>
    /// Summary description for TruncateTest
    /// </summary>
    [TestClass]
    public class TruncateTest
    {
        public TruncateTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Returns the Truncate itself as a string, and the list of parameters.
        /// </summary>
        [TestMethod]
        public void GetDependantStringTest()
        {
            try
            {
                Truncate target = new Truncate();
                target.MainTable = new Table("INV_COMPOUNDS");
                target.MainTable.Database = "COETEST";
                string expected = "Truncate table COETEST.INV_COMPOUNDS";
                string actual = target.GetDependantString(DBMSType.ORACLE);
                Assert.AreEqual(expected, actual, "Truncate.GetDependantString did not return the expected value");
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            try
            {
                Truncate target = new Truncate();
                target.MainTable = new Table("INV_COMPOUNDS");
                target.MainTable.Database = "COETEST";
                target.UseParameters = false;
                string expected = "Truncate table COETEST.INV_COMPOUNDS";
                string actual = target.ToString();
                Assert.AreEqual(expected, actual, "Truncate.GetDependantString did not return the expected value");
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SQLGeneratorException))]
        public void GetDependantString_SQLExceptionTest()
        {
            try
            {
                Truncate target = new Truncate();
                target.MainTable = null;
                target.GetDependantString(DBMSType.ORACLE);
            }
            catch
            {
                throw;
            }
        }
    }
}
