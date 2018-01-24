using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Types.Exceptions;
//using CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO;

namespace CambridgeSoft.COE.Framework.NUnitTests.Common.SqlGenerator.MetaData
{
    /// <summary>
    /// Summary description for DataViewTest
    /// </summary>
    [TestFixture]
    public class DataViewTest
    {

        CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView theDataView = null;
        COEDataView theCOEDataView = null;
        int _FieldId = 0;
        int _BaseTableId = 0;
        string _FieldName = string.Empty;
        string _PrimaryKey = string.Empty;
        string _BaseTableName = string.Empty;
        public DataViewTest()
        {
            theCOEDataView = new COEDataView();
            theDataView = new Framework.Common.SqlGenerator.MetaData.DataView();
            theCOEDataView = GetDataView();
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
        [SetUp]
        public void MyTestInitialize()
        {
            LoadDataViewFromXml();
            _BaseTableId = theCOEDataView.Basetable;
            _PrimaryKey = theCOEDataView.BaseTablePrimaryKey;
            _FieldId = theCOEDataView.Tables[theCOEDataView.BaseTableName].Fields[0].Id;
            _FieldName = theCOEDataView.Tables[theCOEDataView.BaseTableName].Fields[0].Name;
            _BaseTableName = theCOEDataView.BaseTableName;
            
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TearDown]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Gets Base Table ID.
        /// </summary>
        [Test]
        public void GetBaseTableIdTest()
        {
            try
            {
                int actual = theDataView.GetBaseTableId();
                Assert.AreEqual(_BaseTableId, actual, "DataView.GetBaseTableId didnot return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Gets primary key Field.
        /// </summary>
        [Test]
        public void GetBaseTablePKTest()
        {
            try
            {
                Field actual = theDataView.GetBaseTablePK();
                Assert.AreEqual(_PrimaryKey, Convert.ToString(actual.FieldId), "DataView.GetBaseTablePK didnot return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Gets the table from its id.
        /// </summary>
        [Test]
        public void GetTableTest()
        {
            try
            {
                Table theTable = theDataView.GetTable(_BaseTableId);
                Assert.IsNotNull(theTable, "DataView.GetTable didnot return the expected value");
            }
            catch
            {

                throw;
            }
        }
        /// <summary>
        /// Gets the field name from its id.
        /// </summary>
        [Test]
        public void GetFieldNameTest()
        {
            try
            {
                string actual = theDataView.GetFieldName(_FieldId);
                Assert.AreEqual(_FieldName, actual, "DataView.GetFieldName didnot return the expected value");
            }
            catch
            {
                throw;
            }
        }

        [Test]
        [ExpectedException(typeof(SQLGeneratorException))]
        public void GetParentTableName_ExceptionTest()
        {

            string actual = theDataView.GetParentTableName(-1);

        }
        /// <summary>
        /// Returns the name of the parent table of the Field whose id is fieldIndex.
        /// </summary>
        [Test]
        public void GetParentTableNameTest()
        {
            try
            {
                string actual = theDataView.GetParentTableName(_FieldId);
                Assert.AreEqual(_BaseTableName, actual, "GetParentTableName did not return the expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Returns the FieldType of the field whose id equals fieldIndex
        /// </summary>
        [Test]
        public void GetFieldTypeTest()
        {
            try
            {
                DbType expected = DbType.String;
                DbType actual = theDataView.GetFieldType(_FieldId);
                Assert.AreEqual(expected, actual, "GetFieldType did not return expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Gets all the relation needed to access a parent table from a child. It is the shortest path between them.
        /// </summary>
        [Test]
        public void GetRelationsTest()
        {
            try
            {
                List<Relation> actual = theDataView.GetRelations(_BaseTableId, theCOEDataView.Relationships[0].Child);
               // Assert.IsTrue(actual.Count > 0, "GetRelations did not return the expected value");
                Assert.AreEqual(theCOEDataView.Relationships.Count, actual.Count, "GetFieldType did not return expected value");
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        ///  Gets the table name from its id.
        /// </summary>
        [Test]
        public void GetParentTableTest()
        {
            try
            {
                Table expected = theDataView.GetTable(_BaseTableId);
                Table actual = theDataView.GetParentTable(105);
                Assert.AreEqual(expected, actual, "GetParentTable did not return the expected value");
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Returns a Column instance (Which can be a Lookup or a regular field) from the dataView, given the Identifier.
        /// </summary>
        [Test]
        public void GetColumnTest()
        {
            try
            {
                IColumn actual = theDataView.GetColumn(_FieldId);
                Assert.AreEqual(_FieldId, actual.FieldId, "GetColumn did not return the expected value");

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Gets the alias of a column from its id.
        /// </summary>
        [Test]
        public void GetColumnAliasTest()
        {
            try
            {
                string actual = theDataView.GetColumnAlias(_FieldId);
                Assert.AreEqual(_FieldName, actual, "GetColumnAlias did not return the expected value");

            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Loads the  values of this class from an xml string.
        /// </summary>
        private void LoadDataViewFromXml()
        {
            try
            {
                theDataView.LoadFromXML(theCOEDataView.ToString());
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get dataview.
        /// </summary>
        /// <returns></returns>
        private COEDataView GetDataView()
        {
            try
            {
                COEDataView theCOEDataView = SearchHelper.BuildCOEDataViewFromXML(SearchHelper._COEExportToExcel + @"\DataView.xml");
                return theCOEDataView;

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
