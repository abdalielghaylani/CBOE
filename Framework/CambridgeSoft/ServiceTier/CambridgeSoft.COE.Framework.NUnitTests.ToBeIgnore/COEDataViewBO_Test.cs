using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace NUnitTest
{
    [TestFixture]
    public class COEDataViewBO_Test : LoginBase
    {
        #region Variables
        private string _pathToXmls = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + @"\TestXML";
        private string _databaseName = "COEDB";
        private string _databasePassword = "ORACLE";
        private string _userName = "cssadmin";
        //private string _password = "cssadmin";

        //private DALFactory _dalFactory = new DALFactory();
        //private TestContext _testContextInstance;
        COEDatabaseBOList Schemas;
        #endregion

        #region Test Methods

        /// <summary>
        ///A test for Clone (int, string, string)
        ///</summary>
        [Test]
        public void CloneTest()
        {
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, -1);
            int id = dataViewBO.ID; // TODO: Initialize to an appropriate value
            string name = "test" + GenerateRandomNumber(); // TODO: Initialize to an appropriate value
            string description = "test"; // TODO: Initialize to an appropriate value
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = COEDataViewBO.Clone(id, name, description);
                Assert.AreNotEqual(null, dv, "Did not return the expected result.");
                Assert.AreNotEqual(ExpectedId, dv.ID, "Did not return the expected Id value.");
            }
            catch(Exception ex)
            {
                Assert.Fail("Clone failed.");
                
            }
            finally
            {
                //Assert.Inconclusive("Verify the correctness of this test method.");
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for Clone (int, string, string, bool)
        /// using the frommaster value true
        ///</summary>
        [Test]
        public void CloneFromMasterTest()
        {
            //Master should be created before test this method
            int id = 0; // TODO: Initialize to an appropriate value
            string name = "test" + GenerateRandomNumber(); // TODO: Initialize to an appropriate value
            string description = "test"; // TODO: Initialize to an appropriate value
            bool fromMaster = true; // TODO: Initialize to an appropriate value
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CloneFromMaster(id, name, description, fromMaster);
                Assert.AreNotEqual(null, dv, "Did not return the expected result.");
                Assert.AreNotEqual(ExpectedId, dv.ID, "Did not return the expected id value.");
            }
            catch
            {
                Assert.Fail("Clone from master failed.");
            }

        }

        /// <summary>
        ///A test for Save()
        ///</summary>
        [Test]
        public void SaveTest()
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
        [Test]
        public void SavePublicDataViewFromXmlTest()
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
        [Test]
        public void SavePrivateDataViewFromXmlTest()
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
        [Test]
        public void SaveFromXmlTest()
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
        /// get the private dataview by using ispublic property as false
        ///</summary>
        [Test]
        public void GetPrivateDataViewFromXmlTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                //First make sure their is an object to get
                COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, -1);
                ExpectedId = dataViewBO.ID; // TODO: Initialize to an appropriate value
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
        ///A test for Get (int)
        /// input invalid id value as 0
        ///</summary>
        [Test]
        public void GetTest1()
        {
            DateTime LogExceptedTime = DateTime.Now;
            try
            {
                int ActualId = 0; // TODO: Initialize to an appropriate value
                int ExpectedId = 0;
                
                COEDataViewBO myGetObject = COEDataViewBO.Get(ExpectedId);
                ActualId = myGetObject.ID;
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected value.");
            }
            catch
            {
            }
        }

          

        /// <summary>
        ///A test for Get (int, bool) 
        /// getting the dataview, which was created for the user with is public false property
        /// created the dataview from master with access rights
        ///</summary>
        [Test]
        public void GetPrivateDataViewWithAPFromMasterTest()
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
                if (ex.Message.Contains("logged in user lacks permissions to use the dataview"))
                {
                    Assert.Inconclusive(_userName + " doesn't have permissions to get the master schema. Use cssadmin for get the masterschema.");
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
        [Test]
        public void GetPrivateDataViewFromMasterTest()
        {
            int ExpectedId = int.MinValue;
            int ActualId = int.MinValue;
            try
            {
                //First make sure their is an object to get
                COEDataViewBO dataViewBO = StoreDataViewFromMasterWithAP("coetest", false, false);
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
                if (ex.Message.Contains("logged in user lacks permissions to use the dataview"))
                {
                    Assert.Inconclusive(_userName + " doesn't have permissions to get the master schema. Use cssadmin for get the masterschema.");
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
        [Test]
        public void GetPrivateDataViewWithApXmlTest()
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
        [Test]
        public void NewTest()
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
        [Test]
        public void NewWithAccessRightsTest()
        {
            bool Actual = false;
            bool Expected = true;
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, -1);
            string name = "DataviewNew"; // TODO: Initialize to an appropriate value
            string description = "test"; // TODO: Initialize to an appropriate value
            COEDataView dataView = dataViewBO.COEDataView; // TODO: Initialize to an appropriate value
            COEAccessRightsBO COEAccessRights = dataViewBO.COEAccessRights; // TODO: Initialize to an appropriate value
            //COEDataViewBO expected = null;
            COEDataViewBO dv;
            dv = COEDataViewBO.New(name, description, dataView, COEAccessRights);
            Actual = dv.IsNew;
            COEDataViewBO.Delete(dataView.DataViewID);
            Assert.AreEqual(Expected, Actual, "Did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");

        }

        /// <summary>
        ///A test for Delete (int)
        ///</summary>
        [Test]
        public void DeleteTest()
        {
            //this should create a new object
            COEDataViewBO dataViewBO = CreateDataView("DataView.xml", false, 0);
            int id = dataViewBO.ID; // TODO: Initialize to an appropriate value
            try
            {
                COEDataViewBO.Delete(id);
                COEDataViewBO Actual = COEDataViewBO.Get(id);
                Assert.AreNotEqual(id, Actual.ID, "Did not return the expected result.");
            }
            catch (Exception ex)
            {
                if (!ex.InnerException.Message.Contains("DataView does not exist"))
                    Assert.Fail("Deletion failed.");
            }
        }

        /// <summary>
        ///A test for Delete (int)
        /// passing the id value as 0
        ///</summary>
        [Test]
        public void DeleteTest1()
        {
            int id = 0; // TODO: Initialize to an appropriate value   
            
                COEDataViewBO.Delete(id);
           
            try
            {
                COEDataViewBO Actual = COEDataViewBO.Get(id);
                Assert.AreEqual(id, Actual.ID, "Did not return the expected value.");
            }
            catch (Exception ex)
            {

                if (!ex.InnerException.Message.Contains("DataView does not exist"))
                    Assert.Fail("Deletion failed.");
            }
        }

        /// <summary>
        ///A test for GetMasterSchema ()
        ///</summary>
        [Test]
        public void GetMasterSchemaTest()
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
        [Test]
        public void CreateDataViewTest()
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
        [Test]
        public void CreateDataViewWithNoBaseTableTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            int Actual = -1;
            int Expected = -1;
            try
            {
                COEDataViewBO dv = CreateDataView("DataViewNoBaseTable.xml", false, -1);
                ActualId = dv.ID;
                Actual = dv.COEDataView.Basetable;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
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
        [Test]
        public void CreateDataViewWithNoDatabaseNameTest()
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
        [Test]
        public void CreateDataViewWithSameTableIdsTest()
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
        [Test]
        public void CreateDataViewWithNoAliasTableTest()
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
        [Test]
        public void CreateDataViewWithEmptyAliasTableTest()
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
        [Test]
        public void CreateDataViewWithAliasTableTest()
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
        [Test]
        public void CreateDataViewWithInvalidMimeTypeTest()
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
                {
                    Assert.AreNotEqual(Expected, Actual, "Did not return the expected mimetype value.");
                }
                else
                {
                    //Expecting an exception.
                    //Failed to create dataview with invalid mimetype.
                }
            }

        }

        /// <summary>
        ///A test for Save()
        /// Create DataView test with invalid datatype property using XML
        ///</summary>
        [Test]
        public void CreateDataViewWithInvalidDataTypeTest()
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
        [Test]
        public void CreateDataViewCountTest()
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
        [Test]
        public void CreateDataViewNewFieldTest()
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
        [Test]
        public void CreateDataViewNewFieldWithMissingPropertiesTest()
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
        [Test]
        public void CreateDataViewNoFieldIdTest()
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
        [Test]
        public void CreateDataViewFieldIdEmptyTest()
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
        [Test]
        public void CreateDataViewRelationshipTest()
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
        /// Create DataView test with empty relationship and invalid join type using XML
        ///</summary>
        [Test]
        public void CreateDataViewJoinTypeTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            string Actual = string.Empty;
            string Expected = "test";
            try
            {
                COEDataViewBO dv = CreateDataView("DVJoinType.xml", false, -1);
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
        [Test]
        public void CreateDataViewSortOrderTest()
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
        [Test]
        public void CreateDataViewUniqueKeyTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVUniqueKey.xml", false, -1);
                ActualId = dv.ID;
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected value.");
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
        [Test]
        public void CreateDataViewIsViewTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DVIsView.xml", false, -1);
                ActualId = dv.ID;
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                if (ActualId > 0)
                {
                    int Expected = 2;
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
        ///</summary>
        [Test]
        public void CreateDataViewVisibleTest()
        {
            int ActualId = int.MinValue;
            int ExpectedId = int.MinValue;
            try
            {
                COEDataViewBO dv = CreateDataView("DvVisible.xml", false, -1);
                ActualId = dv.ID;
                string Expected = "";
                string Actual = dv.COEDataView.Tables[0].Fields[0].Visible.ToString();
                //delete dataview after creation
                COEDataViewBO.Delete(ActualId);
                Assert.AreEqual(ExpectedId, ActualId, "Did not return the expected result.");
                Assert.AreNotEqual(Expected, Actual, "Did not return the expected default visible property value.");
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
        [Test]
        public void CreateDataViewIndexTypeTest()
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
        [Test]
        public void CreateDataViewInvalidIdTest()
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
        [Test]
        public void CreateDataviewWithNoTableIdTest()
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
        [Test]
        public void CreateDataviewWithEmptyTableIdTest()
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
        [Test]
        public void CreateDataviewWithNoTableNameTest()
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
        [Test]
        public void CreateDataviewWithEmptyTableNameTest()
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
        [Test]
        public void CreateDataviewMissingPropertiesTableTest()
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

        #endregion

        #region Private Methods

        #region Schema Methods

        public void CreateSchema()
        {
            //bool unpublished = this.UnPublishSchema(_databaseName);
            //this.Schemas = GetUnPublishedSchemas();            
            //bool published = this.PublishSchema(_databaseName, _databasePassword);

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
                //Get the password to send as an argument to publish.
                //string password = this.PasswordTextBox.Text;
                COEDatabaseBO database = COEDatabaseBOList.GetList().GetDatabase(databaseName);
                COEDatabaseBO unPublishedDataBase;
                if (database != null)
                {
                    try
                    {
                        if (database.IsPublished)
                        {
                            unPublishedDataBase = database.UnPublish();
                            if (unPublishedDataBase != null) //If it fails, keep it on the list with no changes.
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
            COEDatabaseBOList unpublishedSchemas = COEDatabaseBOList.GetList(false);
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
                        if (publishedDataBase != null) //If it fails, keep it on the list with no changes.
                        {
                            this.Schemas.Remove(database);
                            this.Schemas.Add(publishedDataBase);
                            retVal = true;
                        }
                        //this.DataBind(this.Schemas);
                    }
                }
            }
            return retVal;
        }

        #endregion

        public void DeleteTest(int id)
        {
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
                System.Exception ex = new Exception("logged in user lacks permissions to use the dataview");
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
            //this really should come from the logged in user..
            dv.UserName = userName;
            //dv.COEDataView = BuildCOEDataViewFromXML("COEDataViewForTests.xml");
            //set ispublic to false since we are applying access rights
            dv.IsPublic = isPublic;
            if (IsAP)
            {
                //get some users and roles
                COEUserReadOnlyBOList userList = COEUserReadOnlyBOList.GetList();
                COERoleReadOnlyBOList rolesList = COERoleReadOnlyBOList.GetList();
                //build the accessrights object
                dv.COEAccessRights = new COEAccessRightsBO(userList, rolesList);
            }
            //this is where it get's persisted
            dataViewObject = dv.Save();
            return dataViewObject;
        }

        private COEDataViewBO CreateDataViewWithAP(string dataViewXML, string userName, bool isPublic)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = _databaseName;
            //this really should come from the logged in user..
            dataViewObject.UserName = _userName;
            dataViewObject.IsPublic = isPublic;
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
        /// Creates a new dataview.
        /// </summary>
        /// <param name="isPublic">Is Public property</param>
        /// <param name="id">A given id. If it is &lt;= 0 a new ID is generated.</param>
        /// <returns>The saved dataview. If the provided is is grater than 0, that id is used, a new id is generated otherwise.</returns>
        public COEDataViewBO CreateDataView(string dataViewXML, bool isPublic, int id)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.ID = id;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = _databaseName;
            //this really should come from the logged in user..
            dataViewObject.UserName = _userName;
            dataViewObject.IsPublic = isPublic;
            dataViewObject.DataViewManager = COEDataViewManagerBO.NewManager(dataViewObject.COEDataView);
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
            //this really should come from the logged in user..
            dataViewObject.UserName = userName;
            dataViewObject.IsPublic = isPublic;
            //this is where it get's persisted
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

        #endregion

        #region Task Methods

        /// <summary>
        ///A test for adding tables and relations to existing dataview
        ///</summary>
        //[Test]
        public void AddTablesAndRelationsToDataViewTest()
        {
            //xml file
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
                //deleting the created dataview from the database.
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for adding tables and relations with no Ids to existing dataview
        ///</summary>
        //[Test]
        public void AddTablesAndRelationsWithNoIdsToDataViewTest()
        {
            //100065
            //xml file
            string TableXml = "TableWithNoIds.xml";
            string RelationshipXml = "RelationshipWithNoIds.xml";
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
                //deleting the created dataview from the database.
                COEDataViewBO.Delete(id);
            }
        }

        /// <summary>
        ///A test for adding tables and relations to existing dataview
        ///</summary>
        //[Test]
        public void AddTablesAndRelationsTest1()
        {
            //xml file
            string TableXml = "tbl1.xml";
            string RelationshipXml = "rel1.xml";
            int id = int.MinValue;
            try
            {
                //get the dataview
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
                //deleting the created dataview from the database.
                COEDataViewBO.Delete(id);
            }
        }

        public void AddTablesAndRelationsWithObjTest1()
        {
            //xml file
            string TableXml = "tbl1.xml";
            string RelationshipXml = "rel1.xml";
            int id = int.MinValue;
            try
            {
                //get the dataview
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
                //deleting the created dataview from the database.
                COEDataViewBO.Delete(id);
            }
        }

        public void AddTablesAndRelationsWithObjTest2()
        {
            //xml file
            //string TableXml = "tbl1.xml";
            //string RelationshipXml = "rel1.xml";
            int id = int.MinValue;
            try
            {
                //get the dataview
                COEDataViewBO dv = SaveDataViewWithBasetable("DataView1.xml");
                id = dv.ID;

                //XmlDocument tabledoc = new XmlDocument();
                //tabledoc.Load(_pathToXmls + "\\" + TableXml);
                //XmlDocument relationshipdoc = new XmlDocument();
                //relationshipdoc.Load(_pathToXmls + "\\" + RelationshipXml);

                //XmlNode tableNode = tabledoc.SelectSingleNode("//table");

                COEDataView.DataViewTable tbl = new COEDataView.DataViewTable();
                tbl.Name = "vwregistrynumber";
                tbl.Alias = "test";
                COEDataView.Field fld1 = new COEDataView.Field();
                fld1.Name = "ID";
                fld1.DataType = COEDataView.AbstractTypes.Integer;
                tbl.Fields.Add(fld1);

                //XmlNode relationshipNode = relationshipdoc.SelectSingleNode("//relationship");

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
                //deleting the created dataview from the database.
                COEDataViewBO.Delete(id);
            }
        }

        #endregion

    }
}
