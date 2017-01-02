using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEDataViewService.UnitTests
{
    /// <summary>
    /// Summary All the below test cases are to increase code coverage.
    /// This class contains testcases for FieldBO, FieldListBO, RelationshipBO, RelationshipListBO, TableBO & TableListBO
    /// </summary>
    [TestClass]
    public class COEDataViewSupportingClassesTest : LoginBase
    {
        private TestContext testContextInstance;
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

        #region FieldBO
      
        #endregion

        

        #region RelationshipBO
        /// <summary>
        /// Test for NewRelationship passing by Relationship
        /// </summary>
        [TestMethod()]
        public void NewRelationship_Test()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;

            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;

            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            COEDataView.Relationship theRelationShip = new COEDataView.Relationship();
            theRelationShip.Child = Child;
            theRelationShip.ChildKey = ChildKey;
            theRelationShip.Parent = Parent;
            theRelationShip.ParentKey = ParentKey;
            theRelationShip.JoinType = COEDataView.JoinTypes.INNER;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(theRelationShip));
            int Expected = dvm.Relationships.Count;

            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(Actual + 1, Expected, "RelationshipBO.NewRelationship did notreturn the expected value.");
        }

        /// <summary>
        /// Test for NewRelationship passing by Relationship and IsClean True
        /// </summary>
        [TestMethod()]
        public void NewRelationship_IsCleanTrueTest()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;

            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;

            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            COEDataView.Relationship theRelationShip = new COEDataView.Relationship();
            theRelationShip.Child = Child;
            theRelationShip.ChildKey = ChildKey;
            theRelationShip.Parent = Parent;
            theRelationShip.ParentKey = ParentKey;
            theRelationShip.JoinType = COEDataView.JoinTypes.INNER;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(theRelationShip, true));
            int Expected = dvm.Relationships.Count;

            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(Actual + 1, Expected, "RelationshipBO.NewRelationship did notreturn the expected value.");
        }

        /// <summary>
        /// Test for NewRelationship passing by Relationship and IsClean False
        /// </summary>
        [TestMethod()]
        public void NewRelationship_IsCleanFalseTest()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;

            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;

            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            COEDataView.Relationship theRelationShip = new COEDataView.Relationship();
            theRelationShip.Child = Child;
            theRelationShip.ChildKey = ChildKey;
            theRelationShip.Parent = Parent;
            theRelationShip.ParentKey = ParentKey;
            theRelationShip.JoinType = COEDataView.JoinTypes.INNER;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(theRelationShip, true));
            int Expected = dvm.Relationships.Count;

            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(Actual + 1, Expected, "RelationshipBO.NewRelationship did notreturn the expected value.");
        }

        /// <summary>
        /// Test for NewRelationship passing by Parent And Child primary Key
        /// </summary>
        [TestMethod()]
        public void NewRelationship_WithParentAndChildKeyTest()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;

            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;

            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(ParentKey, ChildKey, COEDataView.JoinTypes.INNER));
            int Expected = dvm.Relationships.Count;

            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(Actual + 1, Expected, "RelationshipBO.NewRelationship did notreturn the expected value.");
        }
        #endregion

        #region RelationshipListBO
        
        /// <summary>
        /// Test for GetByChildId
        /// </summary>
        [TestMethod]
        public void GetByChildID_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm = dataViewBO.DataViewManager;

            RelationshipBO currentRelationship = dvm.Relationships.Get(dvm.Tables[0].PrimaryKey, dvm.Tables[1].PrimaryKey);
            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(dvm.Tables[0].ID, dvm.Tables[0].PrimaryKey, dvm.Tables[1].ID, dvm.Tables[1].PrimaryKey, COEDataView.JoinTypes.INNER));

            int Expected = dvm.Relationships.Count;
            List<RelationshipBO> theRelationshipList = null;
            theRelationshipList = dvm.Relationships.GetByChildId(dvm.Tables[1].ID);

            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(theRelationshipList.Count > 0, "RelationshipListBO.GetByChildId did notreturn the expected value.");
        }

        /// <summary>
        /// Test for GetByChildKey
        /// </summary>
        [TestMethod]
        public void GetByChildKey_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm = dataViewBO.DataViewManager;

            RelationshipBO currentRelationship = dvm.Relationships.Get(dvm.Tables[0].PrimaryKey, dvm.Tables[1].PrimaryKey);
            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(dvm.Tables[0].ID, dvm.Tables[0].PrimaryKey, dvm.Tables[1].ID, dvm.Tables[1].PrimaryKey, COEDataView.JoinTypes.INNER));

            int Expected = dvm.Relationships.Count;
            List<RelationshipBO> theRelationshipList = null;
            theRelationshipList = dvm.Relationships.GetByChildKey(dvm.Tables[1].PrimaryKey);

            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(theRelationshipList.Count > 0, "RelationshipListBO.GetByChildKey did notreturn the expected value.");
        }

        /// <summary>
        /// Test for GetByParentKeyOrChildKeyId
        /// </summary>
        [TestMethod]
        public void GetByParentKeyOrChildKeyId_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm = dataViewBO.DataViewManager;

            RelationshipBO currentRelationship = dvm.Relationships.Get(dvm.Tables[0].PrimaryKey, dvm.Tables[1].PrimaryKey);
            dvm.Relationships.Remove(currentRelationship);
            int Actual = dvm.Relationships.Count;

            dvm.Relationships.Add(RelationshipBO.NewRelationship(dvm.Tables[0].ID, dvm.Tables[0].PrimaryKey, dvm.Tables[1].ID, dvm.Tables[1].PrimaryKey, COEDataView.JoinTypes.INNER));
            COEDataViewBO.Delete(dataViewBO.ID);
            int Expected = dvm.Relationships.Count;
            List<RelationshipBO> theRelationshipList = null;
            //Get By Parent
            theRelationshipList = dvm.Relationships.GetByParentKeyOrChildKeyId(dvm.Tables[0].PrimaryKey);
            Assert.IsTrue(theRelationshipList.Count > 0, "RelationshipListBO.GetByParentKeyOrChildKeyId did notreturn the expected value.");

            theRelationshipList = null;
            //Get By Child
            theRelationshipList = dvm.Relationships.GetByParentKeyOrChildKeyId(dvm.Tables[1].PrimaryKey);
            Assert.IsTrue(theRelationshipList.Count > 0, "RelationshipListBO.GetByParentKeyOrChildKeyId did notreturn the expected value.");

        }

        #endregion

        #region TableBO

        /// <summary>
        /// Test for GetHighestIdField
        /// </summary>
        [TestMethod]
        public void GetHighestIdField_Test()
        {
            COEDataViewBOTest theDataViewBO = new COEDataViewBOTest();
            COEDataViewBO theDataView = theDataViewBO.CreateDataView(@"\DataView.xml", true, -1);

            TableBO theTableBO = TableBO.NewTable(theDataView.COEDataView.Tables[0]);
            Assert.IsTrue(theTableBO.GetHighestIdField() > 0, "TableBO.GetHighestIdField does not return expected value");
            COEDataViewBO.Delete(theDataView.ID);
        }

        /// <summary>
        /// Test for NewTableListBO
        /// </summary>
        [TestMethod]
        public void NewTableListBO_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            TableListBO theTableList = TableListBO.NewTableListBO();
            Assert.IsNotNull(theTableList, "TableListBO.NewTableListBO  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for NewTableListBO by Tables
        /// </summary>
        [TestMethod]
        public void NewTableListBO_TablesTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables);
            Assert.IsTrue(theTableList.Count == dataViewBO.COEDataView.Tables.Count, "TableListBO.NewTableListBO  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for NewTableListBO by Tables and IsClean true
        /// </summary>
        [TestMethod]
        public void NewTableListBO_CleanTablesTrueTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables, true);
            Assert.IsTrue(theTableList.Count == dataViewBO.COEDataView.Tables.Count, "TableListBO.NewTableListBO  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for NewTableListBO by Tables and IsClean false
        /// </summary>
        [TestMethod]
        public void NewTableListBO_CleanTablesFalseTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables, false);
            Assert.IsTrue(theTableList.Count == dataViewBO.COEDataView.Tables.Count, "TableListBO.NewTableListBO  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for Remove
        /// </summary>
        [TestMethod]
        public void Remove_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables);
            int actual = theTableList.Count;
            theTableList.Remove(dataViewBO.COEDataView.Tables[0].Id);
            int expected = theTableList.Count;
            Assert.IsTrue(actual > expected, "TableListBO.Remove  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for Remove by list
        /// </summary>
        [TestMethod]
        public void Remove_ListTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            List<int> theTableIds = new List<int>();
            foreach (COEDataView.DataViewTable table in dataViewBO.COEDataView.Tables)
                theTableIds.Add(table.Id);

            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables);
            int actual = theTableList.Count;
            theTableList.Remove(theTableIds);
            int expected = theTableList.Count;
            Assert.IsTrue(actual > expected, "TableListBO.Remove  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for GetDatabasesInTables
        /// </summary>
        [TestMethod]
        public void GetDatabasesInTables_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables);
            string[] theDatabases = theTableList.GetDatabasesInTables();

            Assert.IsNotNull(theDatabases, "TableListBO.GetDatabasesInTables  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        /// Test for GetTableByFieldId
        /// </summary>
        [TestMethod]
        public void GetTableByFieldId_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            
            int theFieldId = dataViewBO.COEDataView.Tables[dataViewBO.COEDataView.Tables.Count - 1].Fields[0].Id;
            
            TableListBO theTableList = TableListBO.NewTableListBO(dataViewBO.COEDataView.Tables);
            TableBO theTable = theTableList.GetTableByFieldId(theFieldId);

            int ExpectedFieldID = theTable.Fields[0].ID;

            Assert.AreEqual(theFieldId, ExpectedFieldID, "TableListBO.GetTableByFieldId  does not return expected value");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        #endregion

    }
}
