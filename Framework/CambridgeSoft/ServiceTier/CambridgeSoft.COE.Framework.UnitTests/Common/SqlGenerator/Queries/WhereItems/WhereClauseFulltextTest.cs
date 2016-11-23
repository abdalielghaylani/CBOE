using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using System.Data;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.UnitTests
{
    /// <summary>
    /// Summary description for WhereClauseFulltextTest
    /// </summary>
    [TestClass]
    public class WhereClauseFulltextTest
    {
        public WhereClauseFulltextTest()
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
        /// Unit Test For GetDependantString :DBMSType.ORACLE
        /// </summary>
        [TestMethod]
        public void GetDependantStringWhereClauseFulltext_OracleTest()
        {
            WhereClauseFullText target = new WhereClauseFullText();
            target.DataField = new Field();
            target.DataField.FieldName = "Name";
            target.DataField.FieldType = DbType.String;
            target.Val.Type = DbType.String;
            target.Val.Val = "CBOE";
            List<Value> values = new List<Value>();
            string expected = "CONTAINS(\"Name\",:0)>0";
            string actual = target.Execute(DBMSType.ORACLE, values);
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseGre" +
                        "aterThan.GetDependantString did not return the expected value.");

        }

        /// <summary>
        /// Unit Test For GetDependantString : DB Type :SQLSERVER
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetDependantStringWhereClauseFulltext_SqlServerTest()
        {
            WhereClauseFullText target = new WhereClauseFullText();
            target.DataField = new Field();
            target.DataField.FieldName = "Name";
            target.DataField.FieldType = DbType.String;
            target.Val.Type = DbType.String;
            target.Val.Val = "CBOE";
            List<Value> values = new List<Value>();
            string actual = target.Execute(DBMSType.SQLSERVER, values);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetDependantStringWhereClauseFulltext_AccessServerTest()
        {
            WhereClauseFullText target = new WhereClauseFullText();
            target.DataField = new Field();
            target.DataField.FieldName = "Name";
            target.DataField.FieldType = DbType.String;
            target.Val.Type = DbType.String;
            target.Val.Val = "CBOE";
            List<Value> values = new List<Value>();
            string actual = target.Execute(DBMSType.MSACCESS, values);
        }
    }
}
