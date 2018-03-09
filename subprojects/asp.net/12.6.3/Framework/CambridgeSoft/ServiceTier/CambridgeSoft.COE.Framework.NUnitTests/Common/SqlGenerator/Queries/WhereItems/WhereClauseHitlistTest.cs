using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.UnitTests
{
    /// <summary>
    /// Summary description for WhereClauseHitlistTest
    /// </summary>
    [TestFixture]
    public class WhereClauseHitlistTest
    {
        public WhereClauseHitlistTest()
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
        // [TestFixtureSetUp]
        // public static void MyClassInitialize() { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [TestFixtureTearDown]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [SetUp]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TearDown]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void GetDependantStringWhereClauseHitlist_OracleTest()
        {
            WhereClauseHitlist target = new WhereClauseHitlist();
            target.HitlistType = HitListType.TEMP;
            List<Value> values = new List<Value>();
            string expectedValue = "\"hitlist1\".\"HITLISTID\" = :0";
            string actual = target.Execute(DBMSType.ORACLE, values);
            Assert.AreEqual(expectedValue, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseHitlist" +
                ".GetDependantString did not return the expected value.");

        }
    }
}
