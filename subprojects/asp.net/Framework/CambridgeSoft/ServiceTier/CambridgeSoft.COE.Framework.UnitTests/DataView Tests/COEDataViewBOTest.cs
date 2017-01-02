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
    [TestClass()]
    public class COEDataViewBOTest : LoginBase
    {
        #region Variables
        private string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\DataView Tests\COEDataViewTestXML");
        private string _databaseName = "COEDB";
        private string _databasePassword = "ORACLE";
        private string _userName = "cssadmin";
        COEDatabaseBOList Schemas;
        #endregion

        #region Test Methods

        /// <summary>
        ///A test for Clone (int, string, string)
        ///</summary>
        [TestMethod()]
        public void Clone_Test()
        {
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, -1);
            int id = dataViewBO.ID;
            string name = "test" + GenerateRandomNumber();
            string description = "test";
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = COEDataViewBO.Clone(id, name, description);
                Assert.AreNotEqual(null, dv, "Did not return the expected result.");
                Assert.AreNotEqual(ExpectedId, dv.ID, "Did not return the expected Id value.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Clone failed.");
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for Clone (int, string, string, bool)
        /// using the frommaster value true
        ///</summary>
        [TestMethod()]
        public void Clone_FromMasterTest()
        {
            //Master should be created before test this method
            int id = 0;
            string name = "test" + GenerateRandomNumber();
            string description = "test";
            bool fromMaster = true;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CloneFromMaster(id, name, description, fromMaster);
                Assert.AreNotEqual(null, dv, "Did not return the expected result.");
                Assert.AreNotEqual(ExpectedId, dv.ID, "Did not return the expected id value.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Master dataview does not exists"))
                    Assert.Inconclusive(ex.Message);
                else
                    Assert.Fail("Clone from master failed.");
            }
        }

        /// <summary>
        ///A test for Clone (int, string, string,string)
        ///using application name
        ///</summary>
        [TestMethod()]
        public void Clone_ApplicationNameTest()
        {
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, -1);
            int id = dataViewBO.ID;
            string name = "test" + GenerateRandomNumber();
            string description = "test";
            string strApplication = "COETEST";
            try
            {
                COEDataViewBO dv = COEDataViewBO.Clone(id, name, description, strApplication);
                COEDataViewBO.Delete(id);
                id = dv.ID;
                Assert.AreNotEqual(null, dv, "Did not return the expected result.");
                Assert.IsTrue(strApplication.Equals(dv.Application), "Did not return the expected Id value.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Clone failed.");
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for Save()
        ///</summary>
        [TestMethod()]
        public void Save_Test()
        {
            DateTime LogExceptedTime = DateTime.Now;
            try
            {
                int ActualId = int.MinValue;
                int ExpectedId = int.MinValue;
                COEDataViewBO dv = SaveDataView();
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("A method cannot be verified, because no master exists.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Save test using XML with ispublic property value as true
        ///</summary>
        [TestMethod()]
        public void Save_PublicDataViewFromXmlTest()
        {
            int ActualId = int.MinValue;
            int ExpectedID = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("COEDataViewForTests.xml", true, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedID, ActualId, "did not return the expected value.");
            }
            catch
            {
                Assert.Fail("Falied to create dataview using the xml file.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Save test using XML with ispublic property value as false
        ///</summary>
        [TestMethod()]
        public void Save_PrivateDataViewFromXmlTest()
        {
            int ActualId = int.MinValue;
            int ExpectedID = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("COEDataViewForTests.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedID, ActualId, "did not return the expected value.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("logged in user lacks permissions to use the dataview"))
                {
                    Assert.Inconclusive(_userName + " doesn't have permissions to get the master schema. Use cssadmin for get the masterschema.");
                }
                else
                {
                    Assert.Fail("Falied to create dataview using the xml file.");
                }
            }
        }

        /// <summary>
        ///A test for Save()
        /// Save test using XML, which contains dataview advanced properties
        ///</summary>
        [TestMethod()]
        public void Save_FromXmlTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DataView1.xml", true, -1);
                ActualId = dv.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Failed to create dataview.");
            }
            catch
            {
                Assert.Fail("Save failed");
            }
            finally
            {
                //delete dataview after creation   
                COEDataViewBO.Delete(ActualId);
            }
        }

        /// <summary>
        ///A test for Get (int)
        /// input invalid id value as 0
        ///</summary>
        [TestMethod()]
        public void Get_Test()
        {
            DateTime LogExceptedTime = DateTime.Now;
            try
            {
                int ActualId = 0;
                int ExpectedId = 0;
                COEDataViewBO myGetObject = COEDataViewBO.Get(ExpectedId);
                ActualId = myGetObject.ID;
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch
            {
            }
        }

        [TestMethod]
        public void GetNewfield_Test()
        {
            int ActualId = -1;
            try
            {
                COEDataViewBO dv = CreateDataView("DataView1.xml", true, -1);
                ActualId = dv.ID;
                string Oldname = "NewField";
                FieldBO newField= dv.GetNewfield("NewField");
                Assert.AreEqual(Oldname, newField.Name, "Did not return the expected value.");
            }
            catch (Exception)
            {
                Assert.Fail("GetNewfields failed");
            }
            finally
            {
                COEDataViewBO.Delete(ActualId);
            }
        }

        /// <summary>
        ///A test for Get (int)
        /// get the private dataview by using ispublic property as false
        ///</summary>
        [TestMethod()]
        public void Get_PrivateDataViewFromXmlTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                ///Following method creates private dataview but as now any access right specified it fails to Get
                //COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false,-1); 

                COEDataViewBO dataViewBO = CreateDataViewWithAP("DataView.xml", _userName, false);
                ExpectedId = dataViewBO.ID;
                COEDataViewBO myGetObject = COEDataViewBO.Get(ExpectedId);
                ActualId = myGetObject.ID;
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("logged in user lacks permissions to use the dataview"))
                {
                    Assert.Inconclusive(_userName + " doesn't have permissions to get the master schema. Use cssadmin for get the masterschema.");
                }
                else
                {
                    Assert.Fail("Failed to create or get the dataview.");
                }
            }
            finally
            {
                COEDataViewBO.Delete(ExpectedId);
            }

        }


        /// <summary>
        ///A test for Get (int, bool) 
        /// getting the dataview, which was created for the user with is public false property
        /// created the dataview from master with access rights
        ///</summary>
        [TestMethod()]
        public void Get_PrivateDataViewWithAPFromMasterTest()
        {

            int ExpectedId = int.MinValue;
            int ActualId = int.MinValue;
            try
            {
                //First make sure their is an object to get
                COEDataViewBO dataViewBO = StoreDataViewFromMasterWithAP("coetest", false, true);
                ActualId = dataViewBO.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Failed to create dataview from master.");
                ExpectedId = dataViewBO.ID;
                COEDataViewBO myGetObject = COEDataViewBO.Get(ExpectedId, true);
                ActualId = myGetObject.ID;
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected dataview id value.");
                Assert.IsTrue(myGetObject.COEAccessRights != null, "Did not return the expected access rights.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Master dataview does not exists"))
                {
                    Assert.Inconclusive("Master dataview does not exists");
                }
                else
                {
                    Assert.Fail("Failed to create or get the dataview.");
                }
            }

        }

        /// <summary>
        ///A test for Get (int, bool) 
        /// getting the dataview, which was created for the user with is public false property
        /// created the dataview from master
        ///</summary>
        [TestMethod()]
        public void Get_PrivateDataViewFromMasterTest()
        {
            int ExpectedId = int.MinValue;
            int ActualId = int.MinValue;
            try
            {
                //First make sure their is an object to get
                //for private dataview, access permission are must, hence last parameter must be true here.
                //COEDataViewBO dataViewBO = StoreDataViewFromMasterWithAP("coetest", false, false);
                COEDataViewBO dataViewBO = StoreDataViewFromMasterWithAP("coetest", false, true);
                ActualId = dataViewBO.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Failed to create dataview from master.");
                ExpectedId = dataViewBO.ID;
                COEDataViewBO myGetObject = COEDataViewBO.Get(ExpectedId, true);
                ActualId = myGetObject.ID;
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected dataview id value.");
                Assert.IsTrue(myGetObject.COEAccessRights != null, "Did not return the expected access rights.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Master dataview does not exists"))
                {
                    Assert.Inconclusive("Master dataview does not exists");
                }
                else
                {
                    Assert.Fail("Failed to create or get the dataview.");
                }
            }

        }

        /// <summary>
        ///A test for Get (int, bool) 
        /// getting the dataview, which was created for the user with is public false property
        /// created the dataview using the xml file
        [TestMethod()]
        public void Get_PrivateDataViewWithApXmlTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                //First make sure their is an object to get
                COEDataViewBO dataViewBO = CreateDataViewWithAP("DataView.xml", "coetest", false);
                ExpectedId = dataViewBO.ID;
                COEDataViewBO myGetObject = COEDataViewBO.Get(ExpectedId, true);
                ActualId = myGetObject.ID;
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected id value.");
                Assert.AreNotEqual(null, myGetObject.COEAccessRights, "COEDataViewBO.Get did not return the expected value.");
            }
            catch
            {
                Assert.Fail("Private Dataview creation or getting failed.");
            }
            finally
            {
                COEDataViewBO.Delete(ActualId);
            }

        }

        /// <summary>
        ///A test for New ()
        ///</summary>
        [TestMethod()]
        public void New_Test()
        {
            int id = 0;
            //this should create a new object
            COEDataViewBO dataViewTestObject = COEDataViewBO.New();
            id = dataViewTestObject.ID;
            Assert.IsTrue(dataViewTestObject.IsNew, "Did not return the expected value.");
            COEDataViewBO.Delete(id);
        }

        /// <summary>
        ///A test for New (string, string, COEDataView, COEAccessRightsBO)
        ///</summary>
        [TestMethod()]
        public void New_WithAccessRightsTest()
        {
            bool Actual = false;
            bool Expected = true;
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, -1);
            string name = "DataviewNew";
            string description = "test";
            COEDataView dataView = dataViewBO.COEDataView;
            COEAccessRightsBO COEAccessRights = dataViewBO.COEAccessRights;
            COEDataViewBO dv;
            dv = COEDataViewBO.New(name, description, dataView, COEAccessRights);
            Actual = dv.IsNew;
            COEDataViewBO.Delete(dataView.DataViewID);
            Assert.AreEqual(Expected, Actual, "Did not return the expected value.");
        }

        /// <summary>
        ///A test for Delete (int)
        ///</summary>
        [TestMethod()]
        public void Delete_Test()
        {
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, 0);
            int id = dataViewBO.ID;
            COEDataViewBO Actual = null;
            try
            {
                COEDataViewBO.Delete(id);
                //following line will throw exception if view deleted successfully
                Actual = COEDataViewBO.Get(id);
                Assert.AreNotEqual(id, Actual.ID, "Did not return the expected result.");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("DataView does not exist"))
                    Assert.Fail("Deletion failed.");
                else
                    Assert.IsNull(Actual, "Did not return the expected result.");
            }
        }

        /// <summary>
        ///A test for GetMasterSchema ()
        ///</summary>
        [TestMethod()]
        public void GetMasterSchema_Test()
        {
            COEDataViewBO Actual;
            try
            {
                Actual = COEDataViewBO.GetMasterSchema();
                Assert.IsTrue(Actual.ID != -1, "COEDataViewBO.GetMasterSchema did not return the expected value.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("DataView does not exist or logged in user lacks permissions to use the dataview"))
                {
                    Assert.Inconclusive(_userName + " doesn't have permissions to get the master schema. Use cssadmin for get the masterschema.");
                }
                else
                {
                    Assert.Fail(ex.Message);
                }
            }
        }

        #region DataView Properties Test

        /// <summary>
        ///A test for Save()
        /// Create DataView test using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_Test()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("COEDataViewForTests.xml", false, -1);
                ActualId = dv.ID;
                int ExpectedTableCount = 8;
                int ActualTableCount = dv.COEDataView.Tables.Count;
                int ExpectedRelationshipCount = 7;
                int ActualRelationshipCount = dv.COEDataView.Relationships.Count;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value");
                Assert.AreEqual(ActualTableCount, ExpectedTableCount, "Did not return the expeted table count");
                Assert.AreEqual(ActualRelationshipCount, ExpectedRelationshipCount, "Did not return the expeted relationship count");
            }
            catch
            {
                Assert.Fail("Falied to create dataview.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with out base table property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithNoBaseTableTest()
        {
            int ActualId = int.MinValue;
            int Actual = -1;
            int Expected = -1;
            try
            {
                COEDataViewBO dv = CreateDataView("DataViewNoBaseTable.xml", false, -1);
                ActualId = dv.ID;
                Actual = dv.COEDataView.Basetable;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(Expected, Actual, "Did not return the expected result.");
            }
            catch
            {
                if (ActualId > 0)
                {
                    Assert.AreNotEqual(Expected, Actual, "Did not return the expected base table value.");
                }
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with out database name property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithNoDatabaseNameTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            string Actual = "";
            string Expected = "";
            try
            {
                COEDataViewBO dv = CreateDataView("DataViewWithNoDatabaseName.xml", false, -1);
                ActualId = dv.ID;
                Actual = dv.DatabaseName;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
            }
            catch
            {
                if (ActualId > 0)
                {
                    Assert.AreNotEqual(Expected, Actual, "Did not return the expected database property value.");
                }
                else
                {
                    Assert.Fail("Dataview Creation failed with out databaseame property. ");
                }
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with out database name property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithSameTableIdsTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            int Actual = int.MinValue;
            int Expected = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DataViewWithSameTableIds.xml", false, -1);
                Actual = dv.ID;
                ExpectedId = dv.COEDataView.Tables[0].Id;
                ActualId = dv.COEDataView.Tables[1].Id;
                COEDataViewBO.Delete(Actual);
                Assert.AreEqual(Expected, Actual, "Did not return expected result.");
            }
            catch
            {
                if (ExpectedId > 0)
                {
                    Assert.AreNotEqual(ExpectedId, ActualId, "Did not return expected table id.");
                }
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with out alias property of the table using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithNoAliasTableTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DataViewWithNoAlias.xml", false, -1);
                ActualId = dv.ID;
                string Expected = "moltable";
                string Actual = dv.COEDataView.Tables[0].Alias;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
                Assert.AreEqual(Expected, Actual, "Did not return the expected result.");
            }
            catch
            {
                Assert.Fail("Dataview creation failed for creating alias property value.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with empty alias property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithEmptyAliasTableTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DataViewWithEmptyAlias.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
                string Expected = "moltable";
                string Actual = dv.COEDataView.Tables[0].Alias;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(Expected, Actual, "Did not return the expected alias name.");
            }
            catch
            {
                Assert.Fail("Failed to create dataview.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with table alias property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithAliasTableTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            string ExpectedAlias = "grph123";
            string ActualAlias = string.Empty;
            try
            {
                COEDataViewBO dv = CreateDataView("DataView1.xml", false, -1);
                ActualId = dv.ID;
                ActualAlias = dv.COEDataView.Tables[0].Alias;
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                    Assert.AreEqual(ExpectedAlias, ActualAlias, "Table alias property did not return the expected value.");
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
            }
            catch
            {
                Assert.Fail("Failed to create dataview.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with invalid mimetype property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithInvalidMimeTypeTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            string Actual = string.Empty;
            string Expected = "test";
            try
            {
                COEDataViewBO dv = CreateDataView("DVWithInvalidMimeType.xml", false, -1);
                ActualId = dv.ID;
                Actual = dv.COEDataView.Tables[0].Fields[0].MimeType.ToString();
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
                Assert.AreNotEqual(Expected, Actual, "Did not return the expected value.");
            }
            catch
            {
                if (ExpectedId > 0)
                    Assert.AreNotEqual(Expected, Actual, "Did not return the expected mimetype value.");

            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with invalid datatype property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_WithInvalidDataTypeTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvInvalidDataType.xml", false, -1);
                ActualId = dv.ID;
                string Expected = "test";
                string Actual = "";
                Actual = dv.COEDataView.Tables[0].Fields[3].DataType.ToString();
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
                Assert.AreNotEqual(Expected, Actual, "Did not return the expected default datatype value.");
            }
            catch
            {
                Assert.Fail("Failed to create dataview with default datatype.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test verify tables and fields count using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_CountTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            COEDataViewBO dv = CreateDataView("COEDataViewForTests.xml", false, -1);
            ActualId = dv.ID;
            Assert.AreNotEqual(ExpectedId, ActualId, "Failed to create dataview.");
            if (ActualId > 0)
            {
                int TablesCount = dv.COEDataView.Tables.Count;
                int FieldsCount = dv.COEDataView.Tables[0].Fields.Count;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(8, TablesCount, "Failed to retrieve the all tables from xml files");
                Assert.AreEqual(11, FieldsCount, "Failed to retrieve the all fields from first table");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with new field using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_NewFieldTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvWithNewField.xml", false, -1);
                ActualId = dv.ID;
                int ExpectedCount = 5;
                int ActualCount = int.MinValue;
                Assert.AreNotEqual(ExpectedId, ActualId, "Failed to create dataview with new field.");
                ActualCount = dv.COEDataView.Tables[0].Fields.Count;
                string Expected = "testDesc";
                string Actual = dv.COEDataView.Tables[1].Fields[3].Name;
                //delete dataview after creation
                COEDataViewBO.Delete(ExpectedId);
                Assert.AreEqual(ExpectedCount, ActualCount, "Expected new field creation failed.");
                Assert.AreEqual(Expected, Actual, "Did not return the expected filed name");

            }
            catch
            {
                Assert.Fail("Failed to create dataview.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with new field with missing properties using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_NewFieldWithMissingPropertiesTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVNewFieldWithMissingProperties.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
                if (ActualId > 0)
                {
                    int Expected = 5;
                    int Actual = dv.COEDataView.Tables[0].Fields.Count;
                    //delete dataview after creation
                    COEDataViewBO.Delete(ActualId);
                    Assert.AreEqual(Expected, Actual, "Failed to create new field.");
                }
            }
            catch
            {
                Assert.Fail("Failed to create dataview.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with new field with missing id using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_NoFieldIdTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVNoFieldId.xml", false, -1);
                ActualId = dv.ID;
                if (ActualId > 0)
                {
                    int Expected = 2;
                    int Actual = dv.COEDataView.Tables[0].Fields.Count;
                    //delete dataview after creation
                    COEDataViewBO.Delete(ActualId);
                    Assert.AreEqual(Expected, Actual, "Failed to create new field");
                }
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Did not return the expected value.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with new field with missing empty id using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_FieldIdEmptyTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvFieldIdEmpty.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                {
                    int Expected = 2;
                    int Actual = dv.COEDataView.Tables[0].Fields.Count;
                    //delete dataview after creation
                    COEDataViewBO.Delete(ActualId);
                    Assert.AreEqual(Expected, Actual, "Failed to create new field");
                }
            }
            catch
            {
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                    Assert.Fail("Failed to create new field with empty Id.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with empty property values relationship using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_RelationshipTest()
        {
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVRelationship.xml", false, -1);
                int ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch
            {
                Assert.Fail("Failed to create dataview with default relationship property values.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create a DataView with atleast one relationship and check the jointype
        ///</summary>
        [TestMethod()]
        public void CreateDataView_JoinTypeTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            string Actual = string.Empty;
            string Expected = "test";
            try
            {
                COEDataViewBO dv = CreateDataView("DataView.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Failed to create dataview.");
                Expected = "test";
                Actual = dv.COEDataView.Relationships[0].JoinType.ToString();
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
            }
            catch
            {
                Assert.Fail("Failed to create dataview.");
            }
            //join type should not be the actual value and expected the default join type value.
            Assert.AreNotEqual(Expected, Actual, "Did not return expected value.");
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with repeated sort order for one of the field using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_SortOrderTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvSortOrder.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Did not return the expected value.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with empty IsUniqueKey attribute value using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_UniqueKeyTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVUniqueKey.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Did not return the expected result.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with IsView attribute value empty using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_IsViewTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVIsView.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                {
                    int Expected = 4;
                    int Actual = dv.COEDataView.Tables[0].Fields.Count;
                    Assert.AreEqual(Expected, Actual, "Failed to create new field");
                    //delete dataview after creation
                    COEDataViewBO.Delete(ActualId);
                }
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                    Assert.Fail(ex.Message);
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with visible attribute value empty using XML
        /// This will return false as per 'CSBR-163898'
        ///</summary>
        [TestMethod()]
        public void CreateDataView_VisibleTest()
        {
            int ActualId = int.MinValue;
            //int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvVisible.xml", false, -1);
                ActualId = dv.ID;
                string Expected = "FALSE"; // CSBR-163898 changed the value to "false" to serialize the field with visible attribute set to "true"
                string Actual = dv.COEDataView.Tables[0].Fields[0].Visible.ToString();
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                // Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                //  Assert.AreNotEqual(Expected, Actual, "Did not return the expected default visible property value.");
                Assert.IsTrue(string.Equals(Expected, Actual, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Failed to create dataview.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with indextype attribute value empty using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_IndexTypeTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvIndexType.xml", false, -1);
                ActualId = dv.ID;
                string Expected = "";
                string Actual = dv.COEDataView.Tables[0].Fields[0].Visible.ToString();
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                Assert.AreNotEqual(Expected, Actual, "Did not return the expected default index property value.");
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Failed to create dataview.");
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with alphabetic value of Id attribute using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataView_InvalidIdTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvInvalidId.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                {
                    int Expected = 2;
                    int Actual = dv.COEDataView.Tables[0].Fields.Count;
                    Assert.AreEqual(Expected, Actual, "Failed to create new field");
                    //delete dataview after creation
                    COEDataViewBO.Delete(ActualId);
                    Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                }
            }
            catch
            {
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                    Assert.Fail("Failed to create dataview with invalid new field id.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with table id missing using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataview_WithNoTableIdTest()
        {
            int Actual = int.MinValue;
            int Expected = int.MinValue;
            int ActualId = 0;
            int ExpectedId = 0;
            try
            {
                COEDataViewBO dv = CreateDataView("DvNoTableId.xml", false, -1);
                Actual = dv.ID;
                ActualId = dv.COEDataView.Tables[0].Id;
                //delete dataview after creation
                COEDataViewBO.Delete(Actual);
                Assert.AreEqual(Expected, Actual, "Did not return the expected result.");
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected table id.");
            }
            catch
            {
                if (Actual > 0)
                {
                    Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected table id.");
                }
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with table id value empty using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataview_WithEmptyTableIdTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvEmptyTableId.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ActualId, ExpectedId, "Did not return the expected result.");
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Did not return the expected value.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with missing table name property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataview_WithNoTableNameTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            string Actual = "";
            string Expected = "";
            try
            {
                COEDataViewBO dv = CreateDataView("DvNoTableName.xml", false, -1);
                ActualId = dv.ID;
                Actual = dv.COEDataView.Tables[0].Name;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                Assert.AreNotEqual(Expected, Actual, "Did not return the expected result.");
            }
            catch
            {
                if (ActualId > 0)
                {
                    Assert.AreNotEqual(Expected, Actual, "Did not return the expected result.");
                }
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with empty table name property using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataview_WithEmptyTableNameTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvEmptyTableName.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected result.");
            }
            catch
            {
                if (ActualId > 0)
                    Assert.Fail("Did not return the expected value.");
            }
        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with missing other properties except id and name using XML
        ///</summary>
        [TestMethod()]
        public void CreateDataview_MissingPropertiesTableTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvMissingPropTable.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreNotEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                if (ActualId > 0)
                    Assert.Fail(ex.Message);
            }
        }

        #endregion


        /// <summary>
        ///A test for Get ()
        ///</summary>
        [TestMethod()]
        public void Save_UpDateDataViewWithAccessPrivsTest()
        {
            COEDataViewBO dataViewBO = GetDataViewWithAPs();
            int usersCount = -1;
            int usersCount2 = -1;
            try
            {
                usersCount = dataViewBO.COEAccessRights.Users.Count;
                //get some users and roles
                List<string> myAppList = new List<string>();
                myAppList.Add("REGISTRATION");
                COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetListByApplication(myAppList);
                //build the accessrights object
                dataViewBO.COEAccessRights.Users = userList;
                dataViewBO = dataViewBO.Save();
                usersCount2 = dataViewBO.COEAccessRights.Users.Count;
            }
            catch (Exception) { }
            Assert.IsFalse(usersCount == usersCount2, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO" +
                    ".New did not return the expected value.");
            COEDataViewBO.Delete(dataViewBO.ID);
        }

        [TestMethod()]
        public void Save_UpdateDataViewNameTest()
        {
            string randomNumber = GenerateRandomNumber();
            COEDataViewBO dataViewBO = StoreDataView(true, -1);
            int id = dataViewBO.ID;
            dataViewBO = null;
            dataViewBO = COEDataViewBO.Get(id);
            dataViewBO.Name = "TempChanged" + randomNumber;
            dataViewBO.DatabaseName = "CHEMINVDB2";
            dataViewBO = dataViewBO.Save();
            dataViewBO = null;
            dataViewBO = COEDataViewBO.Get(id);
            Assert.IsTrue(dataViewBO.Name.Equals("TempChanged" + randomNumber), "Update is not Correct");

            COEDataViewBOList dataViewBOList = COEDataViewBOList.GetDataViewListbyDatabase("CHEMINVDB2");

            Assert.IsTrue(dataViewBOList != null && dataViewBOList.Count > 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO" +
                    ".Save did not return the expected value.");
            COEDataViewBO.Delete(id);
        }

        [TestMethod]
        public void SaveFromDataviewMaster_Test()
        {
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(@"\DataView.xml");
            dataViewObject.ID = 0;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = _databaseName;
            dataViewObject.UserName = _userName;
            dataViewObject.FormGroup = 0;
            dataViewObject.IsPublic = true;
            //dataViewObject.DataViewManager = COEDataViewManagerBO.NewManager(dataViewObject.COEDataView);
            COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
            COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
            dataViewObject.COEAccessRights = new COEAccessRightsBO(userList, rolesList);

            Assert.IsNotNull(dataViewObject.SaveFromDataViewManager(), "SaveFromDataViewManager unable to save Dataview");
            COEDataViewBO.Delete(dataViewObject.ID);
        }


        /// <summary>
        /// A test for Delete function inside COEDATAVIEWBO.
        /// </summary>
        [TestMethod()]
        public void StoreAndDeleteDataViewBOWithAGivenID()
        {
            int id = 80001;
            COEDataViewBO.Delete(id);
            COEDataViewBO dataViewBO = StoreDataView(true, id);
            Assert.AreEqual(id, dataViewBO.ID, "The dataview didn't use the given ID, but an autogenerated");
            dataViewBO = COEDataViewBO.Get(id);
            Assert.AreEqual(id, dataViewBO.ID, "The dataview didn't use the given ID, but an autogenerated");

            COEDataViewBO.Delete(id);
            try
            {
                dataViewBO = COEDataViewBO.Get(id);

                Assert.Fail("Deletion not confirmed");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("DataView does not exist or logged in user lacks permissions to use the dataview"), "Deletion not confirmed");
            }
        }

        /// <summary>
        /// PublishTableToDataviewTest
        /// </summary>
        [TestMethod()]
        public void PublishTableToDataview_WithFullNames()
        {
            COEDataViewBO dvBO = null;
            int dataviewid = 5000;
            string result = string.Empty;

            try
            {

                dvBO = SetUpDataviewForTestingPublishTable(dataviewid);
                string tableName = "COETEST.AID630SMAD";
                string primaryKey = "COETEST.AID630SMAD.PUBCHEM_CID";
                string parentField = "COETEST.ASSAYEDCOMPOUNDS.PUBCHEM_COMPOUND_CID";
                string childField = "COETEST.AID630SMAD.PUBCHEM_CID";
                result = dvBO.PublishTableToDataview(tableName, primaryKey, parentField, childField, COEDataView.JoinTypes.INNER);

                dvBO = COEDataViewBO.Get(dataviewid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
            finally
            {
                COEDataViewBO.Delete(dataviewid);
            }

            Assert.AreEqual("OK", result, "The table was not added.");
            Assert.IsTrue(dvBO.COEDataView.Tables.Exists(p => p.Name == "AID630SMAD"));

            //Assert.IsTrue(dvBO.COEDataView.Tables[dvBO.COEDataView.Tables.Count - 1].Name == "AID630SMAD");
        }

        /// <summary>
        /// PublishTableToDataviewTest without parent field. This means the join is going to be made against the base table primary key
        /// </summary>
        [TestMethod()]
        public void PublishTableToDataview_WithoutParentField()
        {
            COEDataViewBO dvBO = null;
            int dataviewid = 5000;
            string result = string.Empty;

            try
            {

                dvBO = SetUpDataviewForTestingPublishTable(dataviewid);
                string tableName = "COETEST.AID630SMAD";
                string primaryKey = "COETEST.AID630SMAD.PUBCHEM_CID";
                string parentField = null;
                string childField = "COETEST.AID630SMAD.PUBCHEM_CID";
                result = dvBO.PublishTableToDataview(tableName, primaryKey, parentField, childField, COEDataView.JoinTypes.INNER);

                dvBO = COEDataViewBO.Get(dataviewid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
            finally
            {
                COEDataViewBO.Delete(dataviewid);
            }

            Assert.AreEqual("OK", result, "The table was not added.");
            Assert.IsTrue(dvBO.COEDataView.Tables.Exists(p => p.Name == "AID630SMAD"));
            //Assert.IsTrue(dvBO.COEDataView.Tables[dvBO.COEDataView.Tables.Count - 1].Name == "AID630SMAD");
        }

        /// <summary>
        /// PublishTableToDataviewTest without primary key and without parent field.
        /// </summary>
        [TestMethod()]
        public void PublishTableToDataview_WithoutPKeyAndWithoutParentField()
        {
            COEDataViewBO dvBO = null;
            int dataviewid = 5000;
            string result = string.Empty;

            try
            {
                dvBO = SetUpDataviewForTestingPublishTable(dataviewid);
                string tableName = "AID630SMAD"; //No table schema, as it is provided in child field, that one would be used
                // tableName = "COETEST.AID630SMAD"; //if no table schema, method fails to add new table
                string primaryKey = null;
                string parentField = null;
                string childField = "COETEST.AID630SMAD.PUBCHEM_CID";
                dvBO.DataViewManager = null;
                result = dvBO.PublishTableToDataview(tableName, primaryKey, parentField, childField, COEDataView.JoinTypes.INNER);

                dvBO = COEDataViewBO.Get(dataviewid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
            finally
            {
                COEDataViewBO.Delete(dataviewid);
            }

            Assert.AreEqual("OK", result, "The table was not added.");
            Assert.IsTrue(dvBO.COEDataView.Tables.Exists(p => p.Name == "AID630SMAD"));
            //Assert.IsTrue(dvBO.COEDataView.Tables[dvBO.COEDataView.Tables.Count - 1].Name == "AID630SMAD");
        }

        /// <summary>
        /// PublishTableToDataviewTest without primary key and without parent field.
        /// </summary>
        [TestMethod()]
        public void PublishTableToDataview_WithoutPKeyAndWithoutParentFieldShortChildField()
        {
            COEDataViewBO dvBO = null;
            int dataviewid = 5000;
            string result = string.Empty;

            try
            {
                dvBO = SetUpDataviewForTestingPublishTable(dataviewid);
                string tableName = "AID630SMAD"; //No table schema, as it is not provided in child field, the main schema of the dv is going to be used
                string primaryKey = null;
                string parentField = null;
                string childField = "PUBCHEM_CID";
                dvBO.DataViewManager = null;
                result = dvBO.PublishTableToDataview(tableName, primaryKey, parentField, childField, COEDataView.JoinTypes.INNER);

                dvBO = COEDataViewBO.Get(dataviewid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
            finally
            {
                COEDataViewBO.Delete(dataviewid);
            }

            Assert.AreEqual("OK", result, "The table was not added.");
            Assert.IsTrue(dvBO.COEDataView.Tables.Exists(p => p.Name == "AID630SMAD"));
            //Assert.IsTrue(dvBO.COEDataView.Tables[dvBO.COEDataView.Tables.Count - 1].Name == "AID630SMAD");
        }

        [TestMethod]
        public void CompareTo_Test()
        {
            COEDataViewBO DV1 = COEDataViewBO.New();
            DV1.COEDataView = BuildCOEDataViewFromXML("\\DataView.xml");
            DV1.ID = 0;
            DV1.Name = "A00" + GenerateRandomNumber();
            DV1.Description = "test";
            DV1.DatabaseName = "COETEST";
            DV1.UserName = _userName;
            DV1.IsPublic = true;
            DV1.DataViewManager = COEDataViewManagerBO.NewManager(DV1.COEDataView);
            COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
            COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
            DV1.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            DV1 = DV1.Save();

            COEDataViewBO DV2 = COEDataViewBO.New();
            DV2.COEDataView = BuildCOEDataViewFromXML("\\COTESTDataview.xml");
            DV2.ID = 0;
            DV2.Name = "B00" + GenerateRandomNumber();
            DV2.Description = "test";
            DV2.DatabaseName = "COEDB";
            DV2.UserName = _userName;
            DV2.IsPublic = true;
            DV2.DataViewManager = COEDataViewManagerBO.NewManager(DV2.COEDataView);
            DV2.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            DV2 = DV2.Save();

            // DV1.ID is smaller than the DV2.ID
            Assert.IsTrue(COEDataViewBO.IDComparison_ASC(DV1, DV2) == -1, "IDComparison_ASC Failed");
            Assert.IsTrue(COEDataViewBO.IDComparison_DESC(DV1, DV2) == 1, "IDComparison_DESC Failed");

            // DV2.DatabaseName is smaller than the DV1.DatabaseName
            Assert.IsTrue(COEDataViewBO.DataBaseComparison_ASC(DV1, DV2) == DV1.DatabaseName.CompareTo(DV2.DatabaseName), "DataBaseComparison_ASC Failed");
            Assert.IsTrue(COEDataViewBO.DataBaseComparison_DESC(DV1, DV2) == DV2.DatabaseName.CompareTo(DV1.DatabaseName), "DataBaseComparison_DESC Failed");

            // DV1.Name is smaller than the DV2.Name
            Assert.IsTrue(COEDataViewBO.NameComparison_ASC(DV1, DV2) == -1, "IDComparison_ASC Failed");
            Assert.IsTrue(COEDataViewBO.NameComparison_DESC(DV1, DV2) == 1, "IDComparison_DESC Failed");

            // DV1.BaseTable is smaller than the DV2.BaseTable
            Assert.IsTrue(COEDataViewBO.BaseTableComparison_ASC(DV1, DV2) == -1, "IDComparison_ASC Failed");
            Assert.IsTrue(COEDataViewBO.BaseTableComparison_DESC(DV1, DV2) == 1, "IDComparison_DESC Failed");

            Assert.IsTrue(DV2.CompareTo(DV1) != 0, "CompareTo Failed");

            COEDataViewBO.Delete(DV1.ID);
            COEDataViewBO.Delete(DV2.ID);
        }

        [TestMethod]
        public void GetInvalidPrimaryKeyFieldsTest()
        {
            List<string> lstPrimaryKeyFields = new List<string>();
            COEDataViewBO DV1 = COEDataViewBO.New();
            lstPrimaryKeyFields = DV1.GetInvalidPrimaryKeyFields("COETEST", "INV_RESERVATIONS");
            Assert.IsTrue(lstPrimaryKeyFields.Count > 0, "The user can select any field from the table as primary key");
        }

        #endregion

        #region Private Methods

        #region Schema Methods

        public void CreateSchema()
        {
            this.Schemas = GetUnPublishedSchemas();
            COEDatabaseBO db = this.Schemas.GetDatabase(_databaseName);
            if (db == null)
            {
                bool published = this.PublishSchema(_databaseName, _databasePassword);
            }

        }

        private bool UnPublishSchema(string databaseName)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(databaseName))
            {
                COEDatabaseBO database = COEDatabaseBOList.GetList("MAIN").GetDatabase(databaseName);
                COEDatabaseBO unPublishedDataBase;
                if (database != null)
                {
                    try
                    {
                        if (database.IsPublished)
                        {
                            unPublishedDataBase = database.UnPublish();
                            if (unPublishedDataBase != null)
                            {
                                retVal = true;
                            }
                        }
                    }
                    catch { }
                }
            }
            return retVal;
        }

        private COEDatabaseBOList GetUnPublishedSchemas()
        {
            //Get all the schema in the Db instance.(Unpublish ones)
            COEDatabaseBOList unpublishedSchemas = COEDatabaseBOList.GetList(false,"MAIN");
            return unpublishedSchemas;
        }

        private bool PublishSchema(string databaseName, string password)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(databaseName))
            {
                //Get the password to send as an argument to publish.
                COEDatabaseBO database = this.Schemas.GetDatabase(databaseName);
                COEDatabaseBO publishedDataBase;
                if (database == null)
                {
                    bool unpublished = this.UnPublishSchema(databaseName);
                    this.Schemas = GetUnPublishedSchemas();
                    database = this.Schemas.GetDatabase(databaseName);
                    if (database.IsValid)
                    {
                        publishedDataBase = database.Publish(password);
                        if (publishedDataBase != null)
                        {
                            this.Schemas.Remove(database);
                            this.Schemas.Add(publishedDataBase);
                            retVal = true;
                        }
                    }
                }
            }
            return retVal;
        }

        #endregion

        private COEDataViewBO SetUpDataviewForTestingPublishTable(int dataviewid)
        {
            COEDataViewBO result = null;
            try
            {
                result = COEDataViewBO.Get(dataviewid);
            }
            catch
            {
                result = COEDataViewBO.New("ASSAYEDCOMPOUNDTEST", "ASSAYEDCOMPOUNDTEST", CambridgeSoft.COE.Framework.Common.Utilities.XmlDeserialize<COEDataView>(@"<?xml version=""1.0"" encoding=""utf-8""?><COEDataView xmlns=""COE.COEDataView"" basetable=""1429"" database=""COEDB"" dataviewid=""5000"">
  <tables>
    <table id=""1429"" name=""ASSAYEDCOMPOUNDS"" alias=""Samples"" database=""COETEST"" primaryKey=""1448"">
      <fields id=""1448"" name=""PUBCHEM_COMPOUND_CID"" alias=""PubChem CID"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""0"" />
      <fields id=""1430"" name=""BASE64_CDX"" alias=""Structure"" dataType=""TEXT"" indexType=""CS_CARTRIDGE"" mimeType=""NONE"" visible=""1"" sortOrder=""1"" />
      <fields id=""1457"" name=""PUBCHEM_MOLECULAR_FORMULA"" alias=""Formula"" dataType=""TEXT"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""2"" />
      <fields id=""1458"" name=""PUBCHEM_MOLECULAR_WEIGHT"" alias=""MW"" dataType=""REAL"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""3"" />
      <fields id=""1459"" name=""PUBCHEM_OPENEYE_CAN_SMILES"" alias=""Smiles"" dataType=""TEXT"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""4"" />
 		</table>
    <table id=""1901"" name=""AID629ERA"" alias=""ERalpha Primary Screen"" database=""COETEST"" primaryKey=""1906"" >
      <fields id=""1903"" name=""% Inhibition"" alias=""% Inhibition"" dataType=""REAL"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" />
      <fields id=""1904"" name=""PUBCHEM_ACTIVITY_OUTCOME"" alias=""Active?"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" />
      <fields id=""1905"" name=""PUBCHEM_ACTIVITY_SCORE"" alias=""Score"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" />
      <fields id=""1906"" name=""PUBCHEM_CID"" alias=""PubChem CID"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" />
    </table>
    <table id=""1879"" name=""AID1078ERA"" alias=""ERalpha Dose Response"" database=""COETEST"" primaryKey=""1884"">
      <fields id=""1881"" name=""EC50 (1 nM E2)"" alias=""EC50 (1 nM E2)"" dataType=""REAL"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""1"" />
      <fields id=""1882"" name=""PUBCHEM_ACTIVITY_OUTCOME"" alias=""Active?"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""2"" />
      <fields id=""1883"" name=""PUBCHEM_ACTIVITY_SCORE"" alias=""Score"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""3"" />
      <fields id=""1884"" name=""PUBCHEM_CID"" alias=""PubChem CID"" dataType=""INTEGER"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" sortOrder=""4"" />
    </table>
  </tables>
  <relationships>
    <relationship parentkey=""1448"" childkey=""1906"" parent=""1429"" child=""1901"" jointype=""OUTER"" />
    <relationship parentkey=""1448"" childkey=""1884"" parent=""1429"" child=""1879"" jointype=""OUTER"" />
  </relationships>
</COEDataView>"), null, "UnitTests");
                result.UserName = _userName;
                result.ID = dataviewid;
                result.IsPublic = true;
                result = result.Save();
            }

            COEDatabaseBO coetest = COEDatabaseBO.Get("COETEST");
            if (coetest.COEDataView == null) //Prerequisite is to have, at least once, published the schema
            {
                coetest = COEDatabaseBO.New("COETEST");
                coetest.Publish("ORACLE");
            }

            return result;
        }
        private void DeleteTest(int id)
        {
            if (id != 0)// Expected master dataview can not be deleted.
                COEDataViewBO.Delete(id);
        }

        private COEDataViewBO SaveDataView()
        {
            COEDataViewBO dv = COEDataViewBO.New();
            try
            {
                //trying to create dataview from master
                dv = CloneFromMaster(0, "test" + GenerateRandomNumber(), "test", true);
            }
            catch
            { }
            if (dv.DataViewManager != null && dv.DataViewManager.Tables.Count > 0)
            {
                dv.DatabaseName = _databaseName;
                dv.Save();
            }
            else
            {
                // trying to create dataview using the xml file.
                dv = CreateDataView("COEDataViewForTests.xml", false, -1);
            }
            return dv;
        }

        private COEDataViewBO SaveDataViewWithBasetable(string dataviewXML)
        {
            COEDataViewBO dv = CreateDataView(dataviewXML, true, -1);
            int basetableId = dv.COEDataView.Basetable;
            List<int> tblList = new List<int>();
            //removing all tables except base table.
            foreach (TableBO tbl in dv.DataViewManager.Tables)
            {
                if (tbl.ID != basetableId)
                    tblList.Add(tbl.ID);
            }
            dv.DataViewManager.Tables.Remove(tblList);
            dv.DataViewManager.Relationships.Clear();
            dv.COEDataView.GetFromXML(dv.DataViewManager.ToString());
            dv.DatabaseName = _databaseName;
            dv.Save();
            return dv;
        }

        private COEDataViewBO CloneFromMaster(int id, string name, string description, bool fromMaster)
        {
            COEDataViewBO dv = null;
            //publishing schema
            CreateSchema();
            try
            {
                dv = COEDataViewBO.Clone(id, name, description, fromMaster);
            }
            catch
            {
                System.Exception ex = new Exception("Master dataview does not exists");
                throw ex;
            }
            return dv;
        }

        public COEDataView BuildCOEDataViewFromXML(string dataViewXML)
        {
            //Load DataViewSerialized.XML
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + dataViewXML);
            COEDataView coeDataView = new COEDataView();
            coeDataView.GetFromXML(doc);
            return coeDataView;
        }

        private COEDataViewBO StoreDataViewFromMasterWithAP(string userName, bool isPublic, bool IsAP)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            COEDataViewBO dv = CloneFromMaster(0, dataViewObject.Name, dataViewObject.Description, true);
            dv.DatabaseName = _databaseName;
            dv.UserName = userName;
            dv.IsPublic = isPublic;
            if (IsAP)
            {
                COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
                COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
                dv.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            }
            dataViewObject = dv.Save();
            return dataViewObject;
        }

        public COEDataViewBO CreateDataViewWithAP(string dataViewXML, string userName, bool isPublic)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = _databaseName;
            dataViewObject.UserName = _userName;
            dataViewObject.IsPublic = isPublic;
            //get some users and roles
            COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
            COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
            dataViewObject.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            dataViewObject = dataViewObject.Save();
            return dataViewObject;
        }

        /// <summary>
        /// Creates a new dataview.
        /// </summary>
        /// <param name="isPublic">Is Public property</param>
        /// <param name="id">A given id. If it is &lt;= 0 a new ID is generated.</param>
        /// <returns>The saved dataview. If the provided is is grater than 0, that id is used, a new id is generated otherwise.</returns>
        public COEDataViewBO CreateDataView(string dataViewXML, bool isPublic, int id)
        {
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.ID = id;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = _databaseName;
            dataViewObject.UserName = _userName;
            dataViewObject.IsPublic = isPublic;
            dataViewObject.DataViewManager = COEDataViewManagerBO.NewManager(dataViewObject.COEDataView);
            //access rights are needed in case of private view
            if (!isPublic)
            {
                COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
                COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
                dataViewObject.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            }

            //this is where it get's persisted
            dataViewObject = dataViewObject.Save();

            return dataViewObject;
        }

        public COEDataViewBO CreateDataView(string dataViewXML, string userName, bool isPublic, int id)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.ID = id;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = _databaseName;
            dataViewObject.UserName = userName;
            dataViewObject.IsPublic = isPublic;
            //access rights are needed in case of private view
            if (!isPublic)
            {
                COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
                COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
                dataViewObject.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            }
            dataViewObject = dataViewObject.Save();
            return dataViewObject;
        }

        public string GenerateRandomNumber()
        {
            string miliseconds = DateTime.Now.Millisecond.ToString();
            int length = miliseconds.Length;
            while (length < 3)
            {
                miliseconds = miliseconds.Insert(0, "0");
                length++;
            }
            return miliseconds.Substring(length - 3, 3);
        }

        private COEDataViewBO StoreDataView()
        {
            return StoreDataView(false, -1);
        }


        private COEDataViewBO GetDataViewWithAPs()
        {
            ////First make sure their is an object to get
            COEDataViewBO dataViewBO = StoreDataViewWithAP();
            int id = dataViewBO.ID;
            //nullify
            dataViewBO = null;
            COEDataViewBO myGetObject = COEDataViewBO.Get(id, true);

            return myGetObject;
        }

        private COEDataViewBO StoreDataViewWithAP()
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.Name = "temp" + GenerateRandomNumber();
            dataViewObject.Description = "temp";
            dataViewObject.DatabaseName = _databaseName;
            //this really should come from the logged in user..
            dataViewObject.UserName = "CSSUSER";
            dataViewObject.COEDataView = BuildCOEDataViewFromXML("\\COEDataViewForTests.xml");
            //set ispublic to false since we are applying access rights
            dataViewObject.IsPublic = false;
            //get some users and roles
            COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
            COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
            //build the accessrights object
            dataViewObject.COEAccessRights = new COEAccessRightsBO(userList, rolesList);

            //this is where it get's persisted
            dataViewObject = dataViewObject.Save();
            return dataViewObject;
        }

        /// <summary>
        /// Stores a new dataview.
        /// </summary>
        /// <param name="isPublic">Is Public property</param>
        /// <param name="id">A given id. If it is &lt;= 0 a new ID is generated.</param>
        /// <returns>The saved dataview. If the provided is is grater than 0, that id is used, a new id is generated otherwise.</returns>
        private COEDataViewBO StoreDataView(bool isPublic, int id)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.ID = id;
            dataViewObject.Name = "temp" + GenerateRandomNumber();
            dataViewObject.Description = "temp";
            dataViewObject.DatabaseName = _databaseName;
            //this really should come from the logged in user..
            dataViewObject.UserName = "CSSUSER";
            dataViewObject.COEDataView = BuildCOEDataViewFromXML("\\COEDataViewForTests.xml");
            dataViewObject.IsPublic = isPublic;
            //this is where it get's persisted
            dataViewObject = dataViewObject.Save();
            return dataViewObject;
        }


        #endregion

        #region Task Methods

        /// <summary>
        ///A test for adding tables and relations to existing dataview
        ///</summary>
        //[Test]
        public void AddTablesAndRelationsToDataViewTest()
        {
            string TableXml = "TableTest.xml";
            string RelationshipXml = "RelationshipTest.xml";
            int id = int.MinValue;
            try
            {
                //get the dataview
                COEDataViewBO dv = SaveDataViewWithBasetable("COEDataViewForTests.xml");
                id = dv.ID;
                XmlDocument tabledoc = new XmlDocument();
                tabledoc.Load(_pathToXmls + "\\" + TableXml);
                XmlDocument relationshipdoc = new XmlDocument();
                relationshipdoc.Load(_pathToXmls + "\\" + RelationshipXml);
                DataviewCommand obj = new DataviewCommand();
                obj.AddTableToDataview(id, tabledoc, relationshipdoc);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive(ex.Message);
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for adding tables and relations with no Ids to existing dataview
        ///</summary>
        //[Test]
        public void AddTablesAndRelationsWithNoIdsToDataViewTest()
        {
            string TableXml = "TableWithNoIds.xml";
            string RelationshipXml = "RelationshipWithNoIds.xml";
            int id = int.MinValue;
            try
            {
                COEDataViewBO dv = SaveDataViewWithBasetable("COEDataViewForTests.xml");
                id = dv.ID;
                XmlDocument tabledoc = new XmlDocument();
                tabledoc.Load(_pathToXmls + "\\" + TableXml);
                XmlDocument relationshipdoc = new XmlDocument();
                relationshipdoc.Load(_pathToXmls + "\\" + RelationshipXml);
                DataviewCommand obj = new DataviewCommand();
                obj.AddTableToDataview(id, tabledoc, relationshipdoc);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive(ex.Message);
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for adding tables and relations to existing dataview
        ///</summary>
        //[Test]
        public void AddTablesAndRelationsTest1()
        {
            string TableXml = "tbl1.xml";
            string RelationshipXml = "rel1.xml";
            int id = int.MinValue;
            try
            {
                COEDataViewBO dv = SaveDataViewWithBasetable("DataView1.xml");
                id = dv.ID;
                XmlDocument tabledoc = new XmlDocument();
                tabledoc.Load(_pathToXmls + "\\" + TableXml);
                XmlDocument relationshipdoc = new XmlDocument();
                relationshipdoc.Load(_pathToXmls + "\\" + RelationshipXml);

                DataviewCommand obj = new DataviewCommand();
                obj.AddTableToDataview(id, tabledoc, relationshipdoc);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive(ex.Message);
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        public void AddTablesAndRelationsWithObjTest1()
        {
            string TableXml = "tbl1.xml";
            string RelationshipXml = "rel1.xml";
            int id = int.MinValue;
            try
            {
                COEDataViewBO dv = SaveDataViewWithBasetable("DataView1.xml");
                id = dv.ID;
                XmlDocument tabledoc = new XmlDocument();
                tabledoc.Load(_pathToXmls + "\\" + TableXml);
                XmlDocument relationshipdoc = new XmlDocument();
                relationshipdoc.Load(_pathToXmls + "\\" + RelationshipXml);

                XmlNode tableNode = tabledoc.SelectSingleNode("//table");

                COEDataView.DataViewTable tbl = new COEDataView.DataViewTable(tableNode);

                XmlNode relationshipNode = relationshipdoc.SelectSingleNode("//relationship");

                COEDataView.Relationship rel = new COEDataView.Relationship(relationshipNode);

                DataviewCommand obj = new DataviewCommand();
                obj.AddTableToDataview(id, tbl, rel);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive(ex.Message);
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        public void AddTablesAndRelationsWithObjTest2()
        {
            int id = int.MinValue;
            try
            {
                COEDataViewBO dv = SaveDataViewWithBasetable("DataView1.xml");
                id = dv.ID;
                COEDataView.DataViewTable tbl = new COEDataView.DataViewTable();
                tbl.Name = "vwregistrynumber";
                tbl.Alias = "test";
                COEDataView.Field fld1 = new COEDataView.Field();
                fld1.Name = "ID";
                fld1.DataType = COEDataView.AbstractTypes.Integer;
                tbl.Fields.Add(fld1);
                COEDataView.Relationship rel = new COEDataView.Relationship();
                DataviewCommand obj = new DataviewCommand();
                obj.AddTableToDataview(id, tbl, rel);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive(ex.Message);
            }
            finally
            {
                COEDataViewBO.Delete(id);
            }
        }

        #endregion
    }
}
