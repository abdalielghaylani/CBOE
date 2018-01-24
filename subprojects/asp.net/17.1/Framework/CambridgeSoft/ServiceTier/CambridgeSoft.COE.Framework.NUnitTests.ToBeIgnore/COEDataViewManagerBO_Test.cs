using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;

namespace NUnitTest
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewManagerBO and is intended
    ///to contain all CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewManagerBO Unit Tests
    ///</summary>
    [TestFixture]
    public class COEDataViewManagerBO_Test : LoginBase
    {
        #region Constants
        private const string _defaultAliasWord = "_Alias";
        #endregion

        #region Test Static Methods

        /// <summary>
        ///A test for Get (int)
        /// Getting the COEDataViewBO object from XML file
        ///</summary>
        [Test]
        public void GetTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            int dataviewId = dataViewBO.ID; // TODO: Initialize to an appropriate value

            COEDataViewManagerBO actual;
            try
            {
                //throw new NotSupportedException(Resources.FetchNotSupportedException);
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
        [Test]
        public void GetTest1()
        {
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);

            int dataviewId = dataViewBO.ID; // TODO: Initialize to an appropriate value

            COEDataViewManagerBO actual;
            try
            {
                //throw new NotSupportedException(Resources.FetchNotSupportedException);
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
        [Test]
        public void NewManagerTest()
        {
            COEDataViewManagerBO actual;
            actual = COEDataViewManagerBO.NewManager();

            Assert.IsTrue(actual.IsNew, "COEDataViewManagerBO.NewManager did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NewManager (COEDataView)
        ///</summary>
        [Test]
        public void NewManagerWithDataviewTest()
        {
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);
            COEDataView dataView = dataViewBO.COEDataView; // TODO: Initialize to an appropriate value
            COEDataViewManagerBO actual;
            actual = COEDataViewManagerBO.NewManager(dataView);

            Assert.IsTrue(actual.IsNew, "COEDataViewManagerBO.NewManager did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        /// <summary>
        ///A test for NewManager (COEDataView, int)
        ///</summary>
        [Test]
        public void NewManagerWithDataviewAndIdTest()
        {
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);
            COEDataView dataView = dataViewBO.COEDataView; // TODO: Initialize to an appropriate value

            int dataviewId = dataView.DataViewID; // TODO: Initialize to an appropriate value

            COEDataViewManagerBO actual;

            actual = COEDataViewManagerBO.NewManager(dataView, dataviewId);

            Assert.IsTrue(actual.DataViewId > 0, "COEDataViewManagerBO.NewManager did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
            COEDataViewBO.Delete(dataviewId);
        }

        /// <summary>
        ///A test for Delete (int)
        ///</summary>
        [Test]
        public void DeleteTest()
        {
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);

            int dataviewId = dataViewBO.ID; // TODO: Initialize to an appropriate value
            try
            {
                //throw new NotSupportedException(Resources.DeleteNotSupportedException);
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
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        #endregion

        #region Test Methods

        /// <summary>
        ///A test for CheckDataviewsRelationships (ref string, RelationshipRuleArgs)
        /// Checking the relationships with valid base table and valid relationships
        ///</summary>
        [Test]
        public void CheckDataviewsRelationshipsTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);

            string errors = null; // TODO: Initialize to an appropriate value
            string errors_expected = null; // TODO: Initialize to an appropriate value
            RelationshipRuleArgs ruleArgs = null; // TODO: Initialize to an appropriate value
            bool expected = true;
            bool actual;
            actual = dataViewBO.DataViewManager.CheckDataviewsRelationships(ref errors, ruleArgs);

            COEDataViewBO.Delete(dataViewBO.ID);

            Assert.AreEqual(errors_expected, errors, "errors_CheckDataviewsRelationships_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.CheckDataviewsRelationships did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CheckDataviewsRelationships (ref string, RelationshipRuleArgs)
        /// Checking the relationsships without base table 
        ///</summary>
        [Test]
        public void CheckDataviewsRelationshipsTest1()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);

            string errors = null; // TODO: Initialize to an appropriate value
            string errors_expected = null; // TODO: Initialize to an appropriate value
            RelationshipRuleArgs ruleArgs = null; // TODO: Initialize to an appropriate value
            bool expected = false;
            bool actual = false;
            try
            {
                //It Should not throw the error, if there are any errors it should update to errors ref paramer vaiable
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
            //Expecting errors without base table
            Assert.AreNotEqual(errors_expected, errors, "errors_CheckDataviewsRelationships_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.CheckDataviewsRelationships did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetMasterDataViewTablesStatus (TableListBO, TableListBO)
        ///</summary>
        [Test]
        public void GetMasterDataViewTablesStatusTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            TableListBO dataViewTablesBO = dataViewBO.DataViewManager.Tables; // TODO: Initialize to an appropriate value
            TableListBO masterDVTables = null; // TODO: Initialize to an appropriate value
            //System.Collections.Generic.List<TableStatus> expected = null;
            System.Collections.Generic.List<TableStatus> actual;
            //Result not unique (sometimes fail and sometimes successful). (need to trace out)
            actual = dataViewBO.DataViewManager.GetMasterDataViewTablesStatus(dataViewTablesBO, masterDVTables);
            COEDataViewBO.Delete(dataViewBO.ID);
            //Assert.AreEqual(expected, actual, "COEDataViewManagerBO.GetMasterDataViewTablesStatus did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsValidBaseTable (ref string)
        /// Checking the table with valid properties
        ///</summary>
        [Test]
        public void IsValidBaseTableTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            string errors = null; // TODO: Initialize to an appropriate value
            string errors_expected = null; // TODO: Initialize to an appropriate value

            bool expected = true;
            bool actual;

            actual = dataViewBO.DataViewManager.IsValidBaseTable(ref errors);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(errors_expected, errors, "errors_IsValidBaseTable_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.IsValidBaseTable did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsValidBaseTable (ref string)
        /// Checking the table with No base table
        ///</summary>
        [Test]
        public void IsValidBaseTableTest1()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);
            string errors = null; // TODO: Initialize to an appropriate value
            string errors_expected = null; // TODO: Initialize to an appropriate value

            bool expected = false;
            bool actual;

            actual = dataViewBO.DataViewManager.IsValidBaseTable(ref errors);
            COEDataViewBO.Delete(dataViewBO.ID);
            //Expecting errors, because no base table exists in the dataview.
            Assert.AreNotEqual(errors_expected, errors, "errors_IsValidBaseTable_expected was not set correctly.");
            Assert.AreEqual(expected, actual, "COEDataViewManagerBO.IsValidBaseTable did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Checking the merge with valid table data and valid relationships
        ///</summary>
        [Test]
        public void MergeTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            TableListBO tables = dataViewBO.DataViewManager.Tables; // TODO: Initialize to an appropriate value
            RelationshipListBO relationships = dataViewBO.DataViewManager.Relationships; // TODO: Initialize to an appropriate value
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
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Checking the merge with invalid relationships
        ///</summary>
        [Test]
        public void MergeTest1()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
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
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveOrphanRelationships ()
        /// Checkin OrphanRelationships with valid dataview
        ///</summary>
        [Test]
        public void RemoveOrphanRelationshipsTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
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

            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveOrphanRelationships ()
        /// Checking OrphanRelationships with no base table
        ///</summary>
        [Test]
        public void RemoveOrphanRelationshipsTest1()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
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

            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetBaseTable (int)
        /// Checking SetBaseTable with the dataview which is already having the base table
        ///</summary>
        [Test]
        public void SetBaseTableTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("COEDataViewForTests.xml", false, -1);
            int tableId = dataViewBO.DataViewManager.Tables[0].ID; // TODO: Initialize to an appropriate value

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

            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetBaseTable (int)
        /// Checking the SetBaseTable with dataview which is initially not having base table
        ///</summary>
        [Test]
        public void SetBaseTableTest1()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("DataViewNoBaseTable.xml", false, -1);
            int tableId = dataViewBO.DataViewManager.Tables[0].ID; // TODO: Initialize to an appropriate value

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

            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetBaseTable (int)
        /// Checking the SetBaseTable with table which is not having primary key.
        ///</summary>
        [Test]
        public void SetBaseTableTest2()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
            COEDataViewBO dataViewBO = obj.CreateDataView("DvNoBaseNoPK.xml", false, -1);
            int tableId = dataViewBO.DataViewManager.Tables[0].ID; // TODO: Initialize to an appropriate value
            int expected = int.MinValue;
            try
            {
                //Expecting errors, because there is no primary key in the selected table
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
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Save ()
        /// Checking the DataViewManager Save method with dataview object, which is getting from the database.
        ///</summary>
        [Test]
        public void SaveTest()
        {
            //Create dataview
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DataView.xml", false, -1);
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
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Save ()
        /// Checking the DataViewManager Save method with dataview object, which is getting from the XML.
        ///</summary>
        [Test]
        public void SaveTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("COEDataViewForTests.xml", false, -1);
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
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        #region Field Methods

        /// <summary>
        ///A test for GetField (int)
        /// 
        ///</summary>
        [Test]
        public void GetFieldTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
        [Test]
        public void GetFieldTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
        [Test]
        public void GetLookUpFieldsTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
        [Test]
        public void RemoveLookUpFieldsTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
        [Test]
        public void GetRelationShipsTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Get(int,int)
        /// Get RelationShips
        ///</summary>
        [Test]
        public void GetRelationShipsTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = dvm.Tables[0].PrimaryKey;
            int ChildKey = dvm.Tables[1].PrimaryKey;
            RelationshipBO currentRelationship = dvm.Relationships.Get(0, 0); // trying to get non-existing relation
            RelationshipBO Expected = null;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(currentRelationship, Expected, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Get(int,int,int,int)
        /// Get RelationShips
        ///</summary>
        [Test]
        public void GetRelationShipsWithParentAndChildTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }



        /// <summary>
        ///A test for Add RelationShips
        ///</summary>
        [Test]
        public void AddRelationShipsTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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


            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Add RelationShips
        ///</summary>
        [Test]
        public void AddRelationShipsTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int Parent = dvm.Tables[0].ID;
            int Child = dvm.Tables[1].ID;
            int ParentKey = 0; // assign invalid key
            int ChildKey = 0; //assign invalid key
            RelationshipBO currentRelationship = dvm.Relationships.Get(ParentKey, ChildKey);
            if (currentRelationship != null)
                currentRelationship.FromMasterSchema = true;
            //relation should unsuccessful.
            currentRelationship = RelationshipBO.NewRelationship(Parent, ParentKey, Child, ChildKey, COEDataView.JoinTypes.INNER);
            dataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            bool actual = false;
            bool expected = false;
            RelationshipBO rel = dvm.Relationships.Get(Parent, ParentKey, Child, ChildKey);
            actual = rel.FromMasterSchema;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, expected, "COEDataViewManagerBO.Save did notreturn the expected value.");


            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Update RelationShips
        ///</summary>
        [Test]
        public void UpdateRelationShipsTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Del RelationShips
        ///</summary>
        [Test]
        public void DeletedRelationShipsTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
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
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        #endregion

        #region Table Methods

        /// <summary>
        ///A test for GetTableIdByFieldId (int)
        /// 
        ///</summary>
        [Test]
        public void GetTableIdByFieldIdTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = dvm.Tables[0].Fields[0].ID;
            int tblId = dvm.Tables.GetTableIdByFieldId(fieldId);
            int actual = dvm.Tables[0].ID;
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.AreEqual(actual, tblId, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTableIdByFieldId (int)
        /// passing no-existing field id parameter
        ///</summary>
        [Test]
        public void GetTableIdByFieldIdTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = 222; //assigning no-existing field id
            int tblId = int.MinValue;
            tblId = dvm.Tables.GetTableIdByFieldId(fieldId);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(tblId == int.MinValue, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByLookup(int lookupField)
        /// 
        ///</summary>
        [Test]
        public void GetTablesByLookupTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            int fieldId = dvm.Tables[0].Fields[0].ID;
            List<TableBO> tables = dvm.Tables.GetTablesByLookup(fieldId);
            COEDataViewBO.Delete(dataViewBO.ID);
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDB(string database)
        ///</summary>
        [Test]
        public void GetTablesByDBTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDB(dvm.DataBase);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesFromMasterSchema(bool fromMasterSchema)
        /// master schema true
        ///</summary>
        [Test]
        public void GetTablesFromMasterSchemaTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesFromMasterSchema(true);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesFromMasterSchema(bool fromMasterSchema)
        /// master schema false
        ///</summary>
        [Test]
        public void GetTablesFromMasterSchemaTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesFromMasterSchema(false);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema(dvm.DataBase, true);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        /// test with masterSchema as false
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema(dvm.DataBase, false);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        /// test with masterSchema as true and unknown database name
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaTest2()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema("test", true);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchema(string database, bool fromMasterSchema)
        /// test with masterSchema as false and unknown database name
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaTest3()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchema("test", false);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as true
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaExcludingTablesTest()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = dvm.Tables[1].ID.ToString();
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, true, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as false
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaExcludingTablesTest1()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = dvm.Tables[1].ID.ToString();
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, false, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as true and unknown table Id
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaExcludingTablesTest2()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = "999";
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, true, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for GetTablesByDBAndFromMasterSchemaExcludingTables(string database, bool fromMasterSchema, string tableIdsToExclude)
        /// test with masterSchema as false and unknown table Id
        ///</summary>
        [Test]
        public void GetTablesByDBAndFromMasterSchemaExcludingTablesTest3()
        {
            //Get Dataview from XML
            COEDataViewBO_Test objBo = new COEDataViewBO_Test();
            COEDataViewBO dataViewBO = objBo.CreateDataView("DvWithLookupAndRelations.xml", false, -1);
            COEDataViewManagerBO dvm;
            dvm = dataViewBO.DataViewManager;
            List<TableBO> TblList = null;
            string tableIdsToExclude = "999";
            TblList = dvm.Tables.GetTablesByDBAndFromMasterSchemaExcludingTables(dvm.DataBase, false, tableIdsToExclude);
            COEDataViewBO.Delete(dataViewBO.ID);
            Assert.IsTrue(TblList.Count > 0, "COEDataViewManagerBO.Save did notreturn the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Also test for NewTableListBO(TableListBO tables, bool isClean)
        ///</summary>
        [Test]
        public void NewTableListBOTest()
        {
            foreach (COEDatabaseBO db in this.GetPublishedSchemas())
            {
                RelationshipListBO relationshipsBO = RelationshipListBO.NewRelationShipListBO(db.COEDataView.Relationships);
                TableListBO tableListBO = TableListBO.NewTableListBO(db.COEDataView.Tables, true);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Merge (TableListBO, RelationshipListBO)
        /// Also test for NewTableListBO(TableListBO tables)
        ///</summary>
        [Test]
        public void MergeDataViewTest()
        {
            COEDataViewBO masterDV = COEDataViewBO.GetMasterSchema();
            foreach (COEDatabaseBO db in this.GetPublishedSchemas())
            {
                this.MergeDataView(db.COEDataView, masterDV);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Test for Creates an Alias of a Table
        /// CloneAndAddTable(int tableID, string wordToAdd)
        /// </summary>
        [Test]
        public void DuplicateTableTest()
        {
            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO dataViewBO = obj.SaveFromDataViewManager();
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
        [Test]
        public void ApplySortTest()
        {
            Csla.SortedBindingList<COEDataViewBO> sortedList = new Csla.SortedBindingList<COEDataViewBO>(COEDataViewBOList.GetDataViewListAndNoMaster());
            sortedList.ApplySort("Name", System.ComponentModel.ListSortDirection.Ascending);
            //Assert.Inconclusive("Verify the correctness of this test method.");
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
            COEDatabaseBOList publishedDataBases = COEDatabaseBOList.GetList(true);
            return publishedDataBases;
        }

        #endregion


    }


}
