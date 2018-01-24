using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;

namespace CambridgeSoft.COE.Framework.COEDataViewService.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewManagerBO and is intended
    ///to contain all CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewManagerBO Unit Tests
    ///</summary>
    [TestClass]
    public class COEDataViewManagerBOTest : LoginBase
    {
        #region Constants
        private const string _defaultAliasWord = "_Alias";
        #endregion

        #region Test Static Methods

        /// <summary>
        ///A test for Get (int)
        /// Getting the COEDataViewBO object from XML file
        ///</summary>
        [TestMethod()]
        public void Get_UsingXMLTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            int dataviewId = dataViewBO.ID;
            COEDataViewManagerBO actual;
            try
            {
                actual = COEDataViewManagerBO.Get(dataviewId);
                Assert.IsTrue(dataviewId == actual.DataViewId, "COEDataViewManagerBO.Get did not return the expected value.");
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message == "The Service COEDataViewManager was not found in the config file")
                {
                    COEDataViewBO.Delete(dataviewId);
                    Assert.Inconclusive("Verify the correctness of this test method: Not implemented");
                }
                else
                {
                    COEDataViewBO.Delete(dataviewId);
                    Assert.Fail(ex.Message);
                }
            }
        }

        
        /// <summary>
        ///A test for Get (int)
        /// Getting the COEDataViewBO object from the database
        ///</summary>
        [TestMethod()]
        public void Get_UsingDBTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);
            int dataviewId = dataViewBO.ID;
            COEDataViewManagerBO actual;
            try
            {
                actual = COEDataViewManagerBO.Get(dataviewId);
                Assert.IsTrue(dataviewId == actual.DataViewId, "COEDataViewManagerBO.Get did not return the expected value.");
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message == "The Service COEDataViewManager was not found in the config file")
                {
                    COEDataViewBO.Delete(dataviewId);
                    Assert.Inconclusive("Verify the correctness of this test method: Not implemented");
                }
                else
                {
                    COEDataViewBO.Delete(dataviewId);
                    Assert.Fail(ex.Message);
                }
            }
        }

        /// <summary>
        ///A test for NewManager ()
        ///</summary>
        [TestMethod()]
        public void NewManager_Test()
        {
            COEDataViewManagerBO actual;
            actual = COEDataViewManagerBO.NewManager();
            Assert.IsTrue(actual.IsNew, "COEDataViewManagerBO.NewManager did not return the expected value.");
           
        }

        /// <summary>
        ///A test for NewManager (COEDataView)
        ///</summary>
        [TestMethod()]
        public void NewManager_WithDataviewTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);
            COEDataView dataView = dataViewBO.COEDataView; 
            COEDataViewManagerBO actual;
            actual = COEDataViewManagerBO.NewManager(dataView);
            Assert.IsTrue(actual.IsNew, "COEDataViewManagerBO.NewManager did not return the expected value.");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        ///A test for NewManager (COEDataView, int)
        ///</summary>
        [TestMethod()]
        public void NewManager_WithDataviewAndIdTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);
            COEDataView dataView = dataViewBO.COEDataView;
            int dataviewId = dataView.DataViewID; 
            COEDataViewManagerBO actual;
            actual = COEDataViewManagerBO.NewManager(dataView, dataviewId);
            Assert.IsTrue(actual.DataViewId > 0, "COEDataViewManagerBO.NewManager did not return the expected value.");
            COEDataViewBO.Delete(dataviewId);
        }

        /// <summary>
        ///A test for Delete (int)
        ///</summary>
        [TestMethod()]
        public void Delete_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", true, -1);
            int dataviewId = dataViewBO.ID; 
            try
            {
                COEDataViewManagerBO.Delete(dataviewId);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message == "Invalid operation - delete not allowed")
                {
                    COEDataViewBO.Delete(dataviewId);
                    Assert.Inconclusive("Verify the correctness of this test method: Not implemented");
                }
                else
                {
                    COEDataViewBO.Delete(dataviewId);
                    Assert.Fail(ex.Message);
                }
            }
        }

        #endregion

        #region Test Methods

        /// <summary>
        ///A test for CheckDataviewsRelationships (ref string, RelationshipRuleArgs)
        /// Checking the relationships with valid base table and valid relationships
        ///</summary>
        [TestMethod()]
        public void CheckDataviewsRelationships_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            string errors = null; 
            string errors_expected = null; 
            RelationshipRuleArgs ruleArgs = null; 
            bool expected = true;
            bool actual;
            actual = dataViewBO.DataViewManager.CheckDataviewsRelationships(ref errors, ruleArgs);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(errors_expected, errors, "errors_CheckDataviewsRelationships_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.CheckDataviewsRelationships did not return the expected value.");
        }

        /// <summary>
        ///A test for CheckDataviewsRelationships (ref string, RelationshipRuleArgs)
        /// Checking the relationsships without base table 
        ///</summary>
        [TestMethod()]
        public void CheckDataviewsRelationships_WithoutBaseTableTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);
            string errors = null; 
            string errors_expected = null; 
            RelationshipRuleArgs ruleArgs = new RelationshipRuleArgs("table"); 
            bool expected = false;
            bool actual = false;
            try
            {
                actual = dataViewBO.DataViewManager.CheckDataviewsRelationships(ref errors, ruleArgs);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
            Assert.AreNotEqual(errors_expected, errors, "errors_CheckDataviewsRelationships_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.CheckDataviewsRelationships did not return the expected value.");
         }

        /// <summary>
        ///A test for GetMasterDataViewTablesStatus (TableListBO, TableListBO)
        ///</summary>
        [TestMethod()]
        public void GetMasterDataViewTablesStatus_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            TableListBO dataViewTablesBO = dataViewBO.DataViewManager.Tables; // TODO: Initialize to an appropriate value
            TableListBO masterDVTables = null; // TODO: Initialize to an appropriate value
            System.Collections.Generic.List<TableStatus> actual;
            actual = dataViewBO.DataViewManager.GetMasterDataViewTablesStatus(dataViewTablesBO, masterDVTables);
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        ///A test for IsValidBaseTable (ref string)
        /// Checking the table with valid properties
        ///</summary>
        [TestMethod()]
        public void IsValidBaseTable_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            string errors = null; 
            string errors_expected = null; 
            bool expected = true;
            bool actual;
            actual = dataViewBO.DataViewManager.IsValidBaseTable(ref errors);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(errors_expected, errors, "errors_IsValidBaseTable_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.IsValidBaseTable did not return the expected value.");
         }

        /// <summary>
        ///A test for IsValidBaseTable (ref string)
        /// Checking the table with No base table
        ///</summary>
        [TestMethod()]
        public void IsValidBaseTable_WithoutBaseTableTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);
            string errors = null; 
            string errors_expected = null; 
            bool expected = false;
            bool actual;
            actual = dataViewBO.DataViewManager.IsValidBaseTable(ref errors);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreNotEqual(errors_expected, errors, "errors_IsValidBaseTable_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.IsValidBaseTable did not return the expected value.");
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Checking the merge with valid table data and valid relationships
        ///</summary>
        [TestMethod()]
        public void Merge_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            TableListBO tables = dataViewBO.DataViewManager.Tables; 
            RelationshipListBO relationships = dataViewBO.DataViewManager.Relationships; 
            try
            {
                dataViewBO.DataViewManager.Merge(tables, relationships);
            }
            catch
            {
                Assert.Fail("Merge failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
        }

        /// <summary>
        ///A test for Merge (TableListBO, TableBO, RelationshipListBO)
        /// Checking the merge with valid table data and valid relationships (if table's schema is not present in Master)
        ///</summary>
        [TestMethod()]
        public void MergeTable_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            TableListBO tables = dataViewBO.DataViewManager.Tables;
            TableBO currentTable = tables[0].Clone();            
            PrivateObject thePrivateObject = new PrivateObject(currentTable);
            thePrivateObject.SetField("_database", "REGDB"); // Database should not be present in Master           

            RelationshipListBO relationships = dataViewBO.DataViewManager.Relationships;
            try
            {
                int intTableCount = tables.Count;
                dataViewBO.DataViewManager.MergeTable(tables, currentTable, relationships);
                Assert.AreEqual(intTableCount,tables.Count);
            }
            catch
            {
                Assert.Fail("Merge failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
        }

        /// <summary>
        /// Checks whether current table schema is present in Master
        /// </summary>
        [TestMethod()]
        public void IsDatabasePresentInMaterTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            COEDataViewBO masterDataViewBO = COEDataViewBO.GetMasterSchema();
            TableListBO tables = masterDataViewBO.DataViewManager.Tables;

            try
            {            
                bool blnPresent = dataViewBO.DataViewManager.IsDatabasePresentInMater(tables[0].DataBase);
                Assert.AreEqual(true, blnPresent);
            }
            catch
            {
                Assert.Fail("IsDatabasePresentInMaterTest failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Checking the merge with invalid relationships
        ///</summary>
        [TestMethod()]
        public void Merge_InvalidRelationshipTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            COEDataViewBO dataViewBO1 = obj.CreateDataView("DataView.xml", false, -1);
            try
            {
                TableListBO tables = dataViewBO1.DataViewManager.Tables;
                RelationshipListBO relationships = dataViewBO1.DataViewManager.Relationships;
                int TblCount = dataViewBO.DataViewManager.Tables.Count;
                int RelCount = dataViewBO.DataViewManager.Relationships.Count;
                dataViewBO.DataViewManager.Merge(tables, relationships);
                TblCount = dataViewBO.DataViewManager.Tables.Count;
                RelCount = dataViewBO.DataViewManager.Relationships.Count;
            }
            catch
            {
                Assert.Fail("Merge failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
                COEDataViewBO.Delete(dataViewBO1.ID);
            }
         }

        /// <summary>
        ///A test for RemoveOrphanRelationships ()
        /// Checkin OrphanRelationships with valid dataview
        ///</summary>
        [TestMethod()]
        public void RemoveOrphanRelationships_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            try
            {
                dataViewBO.DataViewManager.RemoveOrphanRelationships();
            }
            catch
            {
                Assert.Fail("RemoveOrphanRelationships failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
       }

        /// <summary>
        ///A test for RemoveOrphanRelationships ()
        /// Checking OrphanRelationships with no base table
        ///</summary>
        [TestMethod()]
        public void RemoveOrphanRelationships_WithoutBaseTableTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);
            try
            {
                dataViewBO.DataViewManager.RemoveOrphanRelationships();
            }
            catch
            {
                Assert.Fail("RemoveOrphanRelationships failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
        }

        /// <summary>
        ///A test for SetBaseTable (int)
        /// Checking SetBaseTable with the dataview which is already having the base table
        ///</summary>
        [TestMethod()]
        public void SetBaseTable_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            int tableId = dataViewBO.DataViewManager.Tables[0].ID; 
            try
            {
                dataViewBO.DataViewManager.SetBaseTable(tableId);
            }
            catch
            {
                Assert.Fail("SetBaseTable failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
        }

        /// <summary>
        ///A test for SetBaseTable (int)
        /// Checking the SetBaseTable with dataview which is initially not having base table
        ///</summary>
        [TestMethod()]
        public void SetBaseTable_WithoutBaseTableTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);
            int tableId = dataViewBO.DataViewManager.Tables[0].ID; 
            try
            {
                dataViewBO.DataViewManager.SetBaseTable(tableId);
            }
            catch
            {
                Assert.Fail("SetBaseTable failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
        }

        /// <summary>
        ///A test for SetBaseTable (int)
        /// Checking the SetBaseTable with table which is not having primary key.
        ///</summary>
        [TestMethod()]
        public void SetBaseTable_WithoutPKTest()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("DvNoBaseNoPK.xml", false, -1);
            int tableId = dataViewBO.DataViewManager.Tables[0].ID;
            int expected = int.MinValue;
            try
            {
                dataViewBO.DataViewManager.SetBaseTable(tableId);
                expected = dataViewBO.DataViewManager.BaseTableId;
            }
            catch
            {
                Assert.Fail("SetBaseTable failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
            Assert.AreEqual(tableId, expected, "COEDataViewManagerBO.SetBaseTable should not set as base table, which is not having primary key.");
         }

        /// <summary>
        ///A test for Save ()
        /// Checking the DataViewManager Save method with dataview object, which is getting from the database.
        ///</summary>
        [TestMethod()]
        public void Save_UsingDatabaseTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", true, -1);
            COEDataViewManagerBO actual;
            int tableId = dataViewBO.DataViewManager.Tables[1].ID;
            actual = dataViewBO.DataViewManager;
            actual.SetBaseTable(tableId);
            try
            {
                actual.Save();
            }
            catch
            {
                Assert.Fail("Save failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
            Assert.AreNotEqual(actual.BaseTableId, tableId, "COEDataViewManagerBO.Save did notreturn the expected value.");
         }

        /// <summary>
        ///A test for Save ()
        /// Checking the DataViewManager Save method with dataview object, which is getting from the XML.
        ///</summary>
        [TestMethod()]
        public void Save_UsingXMLTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("COEDataViewForTests.xml", true, -1);
            COEDataViewManagerBO actual;
            int tableId = dataViewBO.DataViewManager.Tables[0].ID;
            actual = dataViewBO.DataViewManager;
            actual.SetBaseTable(tableId);
            try
            {
                actual.Save();
            }
            catch
            {
                Assert.Fail("Save failed");
            }
            finally
            {
                COEDataViewBO.Delete(dataViewBO.ID);
            }
            Assert.AreNotEqual(actual.BaseTableId, tableId, "COEDataViewManagerBO.Save did notreturn the expected value.");
         }

        #region Field Methods

        /// <summary>
        ///A test for GetField (int)
        /// 
        ///</summary>
        [TestMethod()]
        public void GetField_Test()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = dvm.Tables[0].Fields[0].ID;
            FieldBO fld = dvm.Tables.GetField(fieldId);
            int expected = fld.ID;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(fieldId, expected, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetField (int)
        /// 
        ///</summary>
        [TestMethod()]
        public void GetField_WithnonExistingFieldTest()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = 222; // assigning non-existing field id
            FieldBO fld = dvm.Tables.GetField(fieldId);
            FieldBO expected = null;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(fld, expected, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Lookup Fields
        /// 
        ///</summary>
        [TestMethod()]
        public void LookUpFields_Test()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO actual;
            actual = dataViewBO.DataViewManager;
            FieldBO fld = actual.Tables[0].Fields[0];
            int LookupFieldId = fld.LookupFieldId;
            int LookupDisplayFieldId = fld.LookupDisplayFieldId;
            COEDataView.SortDirection LookupSortOrder = fld.LookupSortOrder;
            COEDataViewBO.Delete(dataViewBO.ID);
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Lookup Fields
        /// 
        ///</summary>
        [TestMethod()]
        public void RemoveLookUpFieldsTest()
        {
            //Get Dataview from XML
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO actual;
            actual = dataViewBO.DataViewManager;
            FieldBO fld = actual.Tables[0].Fields[0];
            fld.LookupFieldId = int.MinValue;
            fld.LookupDisplayFieldId = int.MinValue;
            COEDataViewBO.Delete(dataViewBO.ID);
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        #endregion

        #region Relationship methods

        /// <summary>
        ///A test for Get(int,int)
        /// Get RelationShips
        ///</summary>
        [TestMethod()]
        public void Get_RelationShipsTest()
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
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            RelationshipBO rel = dvm.Relationships.Get(ParentKey, ChildKey);
            bool actual = rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, false, "COEDataViewManagerBO.Save did notreturn the expected value.");
         }

        /// <summary>
        ///A test for Get(int,int)
        /// Get RelationShips
        ///</summary>
        [TestMethod()]
        public void Get_RelationShipsWithParentAndChild0Test()
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
            RelationshipBO currentRelationship = dvm.Relationships.Get(0, 0); 
            RelationshipBO Expected = null;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(currentRelationship, Expected, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for Get(int,int,int,int)
        /// Get RelationShips
        ///</summary>
        [TestMethod()]
        public void Get_RelationShipsWithParentAndChildTest()
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
            RelationshipBO currentRelationship = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            RelationshipBO rel = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            bool actual = rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, false, "COEDataViewManagerBO.Save did notreturn the expected value.");
         }

        /// <summary>
        ///A test for Add RelationShips
        ///</summary>
        [TestMethod()]
        public void Add_RelationShipsTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", true, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            bool actual = false;
            bool expected = false;
            RelationshipBO rel = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            actual = rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, expected, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for Add RelationShips
        ///</summary>
        [TestMethod()]
        public void Add_RelationShipsTest1()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = 0;
            int ChildKey = 0;
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            bool actual = false;
            bool expected = false;
            RelationshipBO rel = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            actual = rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, expected, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for Update RelationShips
        ///</summary>
        [TestMethod()]
        public void UpdateRelationShipsTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            RelationshipBO rel = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            rel.Parent = Parent;
            rel.ParentKey = ParentKey;
            rel.Child = Child;
            rel.ChildKey = ChildKey;
            rel.FromMasterSchema = false;
            RelationshipBO Actual_Rel = dvm.Relationships.Get(ParentKey, ChildKey);
            bool actual = Actual_Rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, false, "COEDataViewManagerBO.Save did notreturn the expected value.");
         }

        /// <summary>
        ///A test for Del RelationShips
        ///</summary>
        [TestMethod()]
        public void DeletedRelationShipsTest()
        {
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
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            RelationshipBO rel = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            rel.FromMasterSchema = true;
            bool actual = rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, true, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        #endregion

        #region Table Methods

        /// <summary>
        ///A test for GetTableIdByFieldId (int)
        /// 
        ///</summary>
        [TestMethod()]
        public void GetTableIdByFieldId_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = dvm.Tables[0].Fields[0].ID;
            int tblId = dvm.Tables.GetTableIdByFieldId(fieldId);
            int actual = dvm.Tables[0].ID;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, tblId, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTableIdByFieldId (int)
        /// passing no-existing field id parameter
        ///</summary>
        [TestMethod()]
        public void GetTableIdByFieldId_InvalidFieldIDTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = 222; 
            int tblId = int.MinValue;
            tblId = dvm.Tables.GetTableIdByFieldId(fieldId);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(tblId == int.MinValue, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByLookup(int lookupField)
        /// 
        ///</summary>
        [TestMethod()]
        public void GetTablesByLookup_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = dvm.Tables[0].Fields[0].ID;
            List<TableBO> tables = dvm.Tables.GetTablesByLookup(fieldId);
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        ///A test for GetTablesByDB(string database)
        ///</summary>
        [TestMethod()]
        public void GetTablesByDB_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDB(dataViewBO.DatabaseName);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesFromMasterSchema(bool fromMasterSchema)
        /// master schema true
        ///</summary>
        [TestMethod()]
        public void GetTablesFromMasterSchema_TrueTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesFromMasterSchema(true);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count >= 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesFromMasterSchema(bool fromMasterSchema)
        /// master schema false
        ///</summary>
        [TestMethod()]
        public void GetTablesFromMasterSchema_FalseTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesFromMasterSchema(false);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count >= 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchema_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema(dvm.DataBase, true);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count >= 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        /// test with masterSchema as false
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchema_FalseMasterSchemaTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema(dvm.DataBase, false);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count >= 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        /// test with masterSchema as true and unknown database name
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchema_UnknownDBTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema("test", true);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count >= 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        /// test with masterSchema as false and unknown database name
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchema_FalseMasterSchemaAndUnknownDBTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema("test", false);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count >= 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as true
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchemaExcludingTables_Test()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = dvm.Tables[1].ID.ToString();
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, true, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }
        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as false
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchemaExcludingTables_FalseMasterSchemaTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = dvm.Tables[1].ID.ToString();
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, false, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as true and unknown table Id
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchemaExcludingTables_UnknownDBTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = "999";
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, true, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as false and unknown table Id
        ///</summary>
        [TestMethod()]
        public void GetTablesByDBAndFromMasterSchemaExcludingTables_FalseMasterSchemaAndUnknownDBTest()
        {
            COEDataViewBOTest objBo = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = "999";
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, false, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Also test for NewTableListBO(TableListBO tables, bool isClean)
        ///</summary>
        [TestMethod()]
        public void NewTableListBO_Test()
        {
            foreach (COEDatabaseBO db in this.GetPublishedSchemas())
            {
                RelationshipListBO relationshipsBO = RelationshipListBO.NewRelationShipListBO(db.COEDataView.Relationships);
                TableListBO tableListBO = TableListBO.NewTableListBO(db.COEDataView.Tables, true);
            }
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Also test for NewTableListBO(TableListBO tables)
        ///</summary>
        [TestMethod()]
        public void MergeDataView_Test()
        {
            COEDataViewBO masterDV = COEDataViewBO.GetMasterSchema();
            foreach (COEDatabaseBO db in this.GetPublishedSchemas())
            {
                this.MergeDataView(db.COEDataView, masterDV);
            }
        }

        /// <summary>
        /// Test for Creates an Alias of a Table
        /// CloneAndAddTable(int tableID, string wordToAdd)
        /// </summary>
        [TestMethod()]
        public void DuplicateTable_Test()
        {
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);                      
            int TableID = dataViewBO.COEDataView.Tables[0].Id;
            int newTableId = int.MinValue;
            if (TableID > 0)
            {
                newTableId = dataViewBO.DataViewManager.Tables.CloneAndAddTable(TableID, _defaultAliasWord);
            }
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(newTableId != int.MinValue, "COEDataViewManagerBO.Save did notreturn the expected value.");
        }

        /// <summary>
        /// Test for ApplySort(string property, ListSortDirection direction)
        /// </summary>
        [TestMethod()]
        public void ApplySort_Test()
        {
            Csla.SortedBindingList<COEDataViewBO> sortedList = new Csla.SortedBindingList<COEDataViewBO>(COEDataViewBOList.GetDataViewListAndNoMaster());
            sortedList.ApplySort("Name", System.ComponentModel.ListSortDirection.Ascending);
        }

        #endregion

        #endregion

        #region Private Methods

        private void MergeDataView(COEDataView dataView, COEDataViewBO dvBO)
        {
            RelationshipListBO relationshipsBO = RelationshipListBO.NewRelationShipListBO(dataView.Relationships);
            TableListBO tableListBO = TableListBO.NewTableListBO(dataView.Tables);
            dvBO.DataViewManager.Merge(tableListBO, relationshipsBO);
        }

        private COEDatabaseBOList GetPublishedSchemas()
        {
            COEDatabaseBOList publishedDataBases = COEDatabaseBOList.GetList(true, "Main");
            return publishedDataBases;
        }

        #endregion


    }


}
