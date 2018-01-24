﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseIn and is intended
    ///to contain all CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseIn Unit Tests
    ///</summary>
    [TestFixture]
    public class WhereClauseInTest
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
        //[SetUp]
        //public void MyTestInitialize()
        //{
        //}
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
        ///A test for GetDependantString (DBMSType, ref List&lt;Value&gt;)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringWhereClauseInTest()
        {
            WhereClauseIn target = new WhereClauseIn();
            target.TrimPosition = SearchCriteria.Positions.None;
            target.DataField = new Field();
            target.DataField.FieldName = "MolId";
            target.DataField.FieldType = DbType.Int32;
            target.Values = new Value[3] { new Value("12", DbType.Int32), new Value("13", DbType.Int32), new Value("24", DbType.Int32) };

            List<Value> queryValues = new List<Value>();
            List<Value> queryValues_expected = new List<Value>();
            queryValues_expected.Add(new Value("12,13,24", DbType.String));

            string expected = "\"MolId\" IN(SELECT /*+ cardinality(t 10)*/ * FROM TABLE(CAST(COEDB.COEDBLibrary.ClobToTable(:0) as COEDB.MYTABLETYPE)) t WHERE ROWNUM >= 0)";
            string actual = target.Execute(DBMSType.ORACLE, queryValues);

            List<Value> queryValuesParalel = new List<Value>();
            System.Diagnostics.Debug.WriteLine(target.Execute(DBMSType.MSACCESS, queryValuesParalel));
            System.Diagnostics.Debug.WriteLine(target.Execute(DBMSType.ORACLE, queryValuesParalel));
            System.Diagnostics.Debug.WriteLine(target.Execute(DBMSType.SQLSERVER, queryValuesParalel));


            Assert.IsTrue(CompareElements(queryValues_expected, queryValues), "queryValues_GetDependantString_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseIn." +
                    "GetDependantString did not return the expected value.");
        }
        private bool CompareElements(List<Value> values_expected, List<Value> values)
        {
            if (values_expected.Count != values.Count)
                return false;
            for (int i = 0; i < values.Count; i++)
            {

                if (values_expected[i].Val != values[i].Val)
                    return false;
            }
            return true;
        }
    }
}
