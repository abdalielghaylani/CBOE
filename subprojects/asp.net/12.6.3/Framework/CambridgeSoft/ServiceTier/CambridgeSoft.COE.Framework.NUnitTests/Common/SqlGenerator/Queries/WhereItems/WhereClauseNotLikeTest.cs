using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.UnitTests
{
    /// <summary>
    /// Summary description for WhereClauseNotLikeTest
    /// </summary>
    [TestFixture]
    public class WhereClauseNotLikeTest
    {
        public WhereClauseNotLikeTest()
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

        /// <summary>
        ///A test for GetDependantString (DBMSType, ref List&lt;Value&gt;)
        ///</summary>
       // [DeploymentItem("CambridgeSoft.COE.Framework.dll")]
        [Test]
        public void GetDependantStringWhereClauseLikeTest()
        {
            WhereClauseNotLike target = new WhereClauseNotLike();
            target.DataField = new Field();
            target.DataField.FieldName = "Structure";
            target.DataField.FieldType = DbType.String;

            target.Val = new Value("	C1CCCCC1  ", DbType.String);

            target.CaseSensitive = false;
            target.TrimPosition = SearchCriteria.Positions.Both;
            //target.FullWordSearch = false;
            target.WildCardPosition = SearchCriteria.Positions.None;
            List<Value> values = new List<Value>(); // TODO: Initialize to an appropriate value
            List<Value> values_expected = new List<Value>(); // TODO: Initialize to an appropriate value
            values_expected.Add(new Value("c1ccccc1", target.DataField.FieldType));

            string expected = "(LOWER(TRIM(\"STRUCTURE\")) NOT LIKE :0)";
            string actual = target.Execute(DBMSType.ORACLE, values).Trim().ToUpper();

            Assert.IsTrue(CompareElements(values_expected, values), "values_GetDependantString_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLik" +
                    "e.GetDependantString did not return the expected value.");
        }

        [Test]
        public void GetDependantStringWhereClauseLike_FullWordTest()
        {
            WhereClauseNotLike target = new WhereClauseNotLike();
            target.DataField = new Field();
            target.DataField.FieldName = "Structure";
            target.DataField.FieldType = DbType.String;

            target.Val = new Value("	C1CCCCC1  ", DbType.String);

            target.CaseSensitive = false;
            target.TrimPosition = SearchCriteria.Positions.Both;
            target.FullWordSearch = true;
            target.WildCardPosition = SearchCriteria.Positions.Both;
            List<Value> values = new List<Value>(); // TODO: Initialize to an appropriate value
            List<Value> values_expected = new List<Value>(); // TODO: Initialize to an appropriate value
            values_expected.Add(new Value("c1ccccc1", target.DataField.FieldType));

            string expected = "(LOWER(TRIM(\"STRUCTURE\")) NOT LIKE '% ' || :0 OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE :1 || ' %' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE :2 || '.%' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE '% ' || :3 || ' %' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE '% ' || :4 || '.%' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE :5)";
            string actual = target.Execute(DBMSType.ORACLE, values).Trim().ToUpper();
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLik" +
                    "e.GetDependantString did not return the expected value.");
        }

        [Test]
        public void GetDependantStringWhereClauseLike_FullWordNormalizeChemicalNameTest()
        {
            WhereClauseNotLike target = new WhereClauseNotLike();
            target.DataField = new Field();
            target.DataField.FieldName = "Structure";
            target.DataField.FieldType = DbType.String;
            target.NormalizeChemicalName = true;
            target.Val = new Value("	C1CCCCC1  ", DbType.String);

            target.CaseSensitive = false;
            target.TrimPosition = SearchCriteria.Positions.Both;
            target.FullWordSearch = true;
            target.WildCardPosition = SearchCriteria.Positions.Both;
            List<Value> values = new List<Value>(); // TODO: Initialize to an appropriate value
            List<Value> values_expected = new List<Value>(); // TODO: Initialize to an appropriate value
            values_expected.Add(new Value("c1ccccc1", target.DataField.FieldType));

            string expected = "(LOWER(TRIM(\"STRUCTURE\")) NOT LIKE '% ' || COEDB.NORMALIZE(:0) OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE COEDB.NORMALIZE(:1) || ' %' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE COEDB.NORMALIZE(:2) || '.%' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE '% ' || COEDB.NORMALIZE(:3) || ' %' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE '% ' || COEDB.NORMALIZE(:4) || '.%' OR LOWER(TRIM(\"STRUCTURE\")) NOT LIKE COEDB.NORMALIZE(:5))";
            string actual = target.Execute(DBMSType.ORACLE, values).Trim().ToUpper();
            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems.WhereClauseLik" +
                    "e.GetDependantString did not return the expected value.");
        }


        private bool CompareElements(List<Value> values_expected, List<Value> values)
        {
            if (values_expected.Count != values.Count)
                return false;
            for (int i = 0; i < values.Count; i++)
            {

                if (values_expected[i] != values[i])
                    return false;
            }
            return true;
        }

    }
}
