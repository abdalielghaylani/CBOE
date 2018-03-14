﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLogical and is intended
    ///to contain all CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLogical Unit Tests
    ///</summary>
    [TestClass()]
    public class WhereClauseLogicalTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetDependantString (DBMSType, ref List&lt;Value&gt;)
        ///</summary>
        [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [TestMethod()]
        public void GetDependantStringTest() {
            WhereClauseLogical target = new WhereClauseLogical();
            target.Items = new List<WhereClauseBase>();

            WhereClauseEqual substance_name = new WhereClauseEqual();
            substance_name.DataField = new Field(19, "SUBSTANCE_NAME", System.Data.DbType.String);
            substance_name.Val = new Value("null", System.Data.DbType.String);
            target.Items.Add(substance_name);

            WhereClauseEqual cas = new WhereClauseEqual();
            cas.DataField = new Field(21, "CAS", System.Data.DbType.String);
            cas.Val = new Value("null", System.Data.DbType.String);
            target.Items.Add(cas);

            DBMSType dataBaseType = DBMSType.ORACLE; // TODO: Initialize to an appropriate value

            List<Value> queryValues = null;
            List<Value> queryValues_expected = null;

            // with the And logical operator
            target.LogicalOperator = SearchCriteria.COELogicalOperators.And;
            string expected = "((\"SUBSTANCE_NAME\" IS NULL) And (\"CAS\" IS NULL))";
            string actual = target.Execute(dataBaseType, queryValues);

            Assert.AreEqual(queryValues_expected, queryValues, "queryValues_GetDependantString_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLog" +
                    "ical.GetDependantString did not return the expected value.");

            // with the Or logical operator
            target.LogicalOperator = SearchCriteria.COELogicalOperators.Or;
            expected = "((\"SUBSTANCE_NAME\" IS NULL) Or (\"CAS\" IS NULL))";
            actual = target.Execute(dataBaseType, queryValues);

            Assert.AreEqual(queryValues_expected, queryValues, "queryValues_GetDependantString_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLog" +
                    "ical.GetDependantString did not return the expected value.");
        }

    }


}