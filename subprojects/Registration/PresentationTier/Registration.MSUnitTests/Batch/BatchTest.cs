using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Registration.Services.BLL.Command;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Xml;
using System.Reflection;
using System.Data;
using CambridgeSoft.COE.Framework.Common.Validation;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// BatchTest class contains unit test methods for Batch
    /// </summary>
    [TestClass]
    public class BatchTest
    {
        #region Variables

        private TestContext testContextInstance;

        public static int iTempRegID;

        #endregion

        #region Properties

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

        #endregion

        #region Test Initialization

        /// <summary>
        /// Use TestInitialize to run code before running each test  
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Helpers.Authentication.Logon();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run 
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Helpers.Authentication.Logoff();
        }

        #endregion

        #region Batch Test Methods

        /// <summary>
        /// Adding new project to Batch by Name
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToBatchByName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));

            string strProjectName = string.Empty;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { strProjectName = theProjectList[1].Name; }
            else { strProjectName = theProjectList[0].Name; }

            Batch theBatch = Batch.NewBatch(theXmlDocument.InnerXml, true, true);
            int iPrevProjectCount = theBatch.ProjectList.Count;
            theBatch.AddProject(strProjectName);

            Assert.AreEqual(iPrevProjectCount + 1, theBatch.ProjectList.Count, "Batch.AddProject(strProjectName) did not return the expected value.");
        }

        /// <summary>
        /// Adding new project to Batch by Name
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToBatchByName1()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            string strProjectName = string.Empty;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { strProjectName = theProjectList[1].Name; }
            else { strProjectName = theProjectList[0].Name; }

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);
            int iPrevProjectCount = theBatch.ProjectList.Count;
            theBatch.AddProject(strProjectName);

            Assert.AreEqual(iPrevProjectCount + 1, theBatch.ProjectList.Count, "Batch.AddProject(strProjectName) did not return the expected value.");
        }

        /// <summary>
        /// Adding new project to Batch by Name
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToBatchByName2ndConstructor()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            string strProjectName = string.Empty;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { strProjectName = theProjectList[1].Name; }
            else { strProjectName = theProjectList[0].Name; }

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true, true, GlobalVariables.REGNUM1);
            int iPrevProjectCount = theBatch.ProjectList.Count;
            theBatch.AddProject(strProjectName);

            Assert.AreEqual(iPrevProjectCount + 1, theBatch.ProjectList.Count, "Batch.AddProject(strProjectName) did not return the expected value.");
        }

        /// <summary>
        /// Adding new project to Batch by Name
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToBatchByName3rdConstructor()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            string strProjectName = string.Empty;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { strProjectName = theProjectList[1].Name; }
            else { strProjectName = theProjectList[0].Name; }

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true, true, GlobalVariables.REGNUM1, RLSStatus.BatchLevelProjects);
            int iPrevProjectCount = theBatch.ProjectList.Count;
            theBatch.AddProject(strProjectName);

            Assert.AreEqual(iPrevProjectCount + 1, theBatch.ProjectList.Count, "Batch.AddProject(strProjectName) did not return the expected value.");
        }

        /// <summary>
        /// Adding new project to Batch by ID
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToBatchByID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            int iProjectID = 0;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { iProjectID = theProjectList[1].ProjectID; }
            else { iProjectID = theProjectList[0].ProjectID; }

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);
            int iPrevProjectCount = theBatch.ProjectList.Count;
            theBatch.AddProject(iProjectID);

            Assert.AreEqual(iPrevProjectCount + 1, theBatch.ProjectList.Count, "Batch.AddProject(iProjectID) did not return the expected value.");
        }

        /// <summary>
        /// Adding new Identifier to Batch by Identifier object
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingNewIdentifierToBatchByIdentifierObject()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.B, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.InputText = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                theIdentifier.IdentifierID = idList[0].IdentifierID;

                Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);
                int iPrevIdentifierCount = theBatch.IdentifierList.Count;
                theBatch.AddIdentifier(theIdentifier);

                Assert.AreEqual(iPrevIdentifierCount + 1, theBatch.IdentifierList.Count, "Batch.AddIdentifier(theIdentifier) did not return the expected value.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Adding new Identifier to Batch by Identifier ID and Value
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingNewIdentifierToBatchByIdentifierIDAndValue()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.B, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";

                Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);
                int iPrevIdentifierCount = theBatch.IdentifierList.Count;
                theBatch.AddIdentifier(idList[0].IdentifierID, "Test case Identifier");

                Assert.AreEqual(iPrevIdentifierCount + 1, theBatch.IdentifierList.Count, "Batch.AddIdentifier(idList[0].IdentifierID, 'Test case Identifier') did not return the expected value.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Adding new Identifier to Batch by Identifier Name and Value
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingNewIdentifierToBatchByIdentifierNameAndValue()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.B, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";

                Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);
                int iPrevIdentifierCount = theBatch.IdentifierList.Count;
                theBatch.AddIdentifier(idList[0].Name, "Test case Identifier");

                Assert.AreEqual(iPrevIdentifierCount + 1, theBatch.IdentifierList.Count, "Batch.AddIdentifier(idList[0].IdentifierID, 'Test case Identifier') did not return the expected value.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Loading Batch using Batch ID
        /// </summary>
        [TestMethod]
        public void GetBatch_GettingBatchAsPerBatchID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("SELECT BATCH_INTERNAL_ID, BATCH_NUMBER FROM REGDB.BATCHES");

            DataTable dtBatchList = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtBatchList != null && dtBatchList.Rows.Count > 0)
            {
                Batch result = Batch.GetBatch(Convert.ToInt32(Convert.ToString(dtBatchList.Rows[0]["BATCH_INTERNAL_ID"])));

                Assert.IsNotNull(result, "Batch.GetBatch(theBatch.ID) did not return the expected value.");
            }
            else
            {
                Assert.IsNotNull(dtBatchList, "Batches are not exist");
            }
        }

        /// <summary>
        /// Loading Batch using Temp Batch ID
        /// </summary>
        [TestMethod]
        public void GetBatch_GettingBatchAsPerBatchTempID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("SELECT DISTINCT TEMPBATCHID FROM REGDB.VW_TEMPORARYBATCH");

            DataTable dtBatchList = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtBatchList != null && dtBatchList.Rows.Count > 0)
            {
                Batch result = Batch.GetBatch(Convert.ToInt32(Convert.ToString(dtBatchList.Rows[0]["TEMPBATCHID"])), true);

                Assert.IsNotNull(result, "Batch.GetBatch(Convert.ToInt32(Convert.ToString(dtBatchList.Rows[0]['TEMPBATCHID'])), true) did not return the expected value.");
            }
            else
            {
                Assert.IsNotNull(dtBatchList, "Batches are not exist");
            }
        }

        /// <summary>
        /// Moving Batch into registry record
        /// </summary>
        [TestMethod]
        public void MoveBatch_MovingBatchInRegistryRecord()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            DALHelper.SetRegistrationSettingsONOrOFF("CheckDuplication", "True");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord.Register(DuplicateAction.None);

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            theRegistryRecord.AddBatch();
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            int iBachID = theRegistryRecord.BatchList[0].ID;

            Batch.MoveBatch(theRegistryRecord.BatchList[0].ID, "AB-000049");

            theRegistryRecord = RegistryHelper.LoadRegistryRecord("AB-000049", GlobalVariables.RegistryLoadType.REGNUM);

            Assert.IsTrue(theRegistryRecord.BatchList[theRegistryRecord.BatchList.Count - 1].ID == iBachID, "Batch.MoveBatch(theRegistryRecord.BatchList[0].ID, 'AB-000049') did not return the expected value.");

        }

        /// <summary>
        /// Deleting Batch from registry record
        /// </summary>
        [TestMethod]
        public void DeleteBatch_DeletingBatchFromRegistryRecord()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            DALHelper.SetRegistrationSettingsONOrOFF("CheckDuplication", "True");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord.Register(DuplicateAction.None);

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            theRegistryRecord.AddBatch();
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);

            int iBachID = theRegistryRecord.BatchList[0].ID;
            Batch.DeleteBatch(iBachID);

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);

            Batch theBatchExist = theRegistryRecord.BatchList.GetBatchById(iBachID);

            Assert.IsNull(theBatchExist, "Batch.DeleteBatch(iBachID) did not return the expected value.");

        }

        /// <summary>
        /// Deleting Batch from registry record
        /// </summary>
        [TestMethod]
        public void DataPortal_DeleteSelf_DeletingBatchFromRegistryRecord()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            DALHelper.SetRegistrationSettingsONOrOFF("CheckDuplication", "True");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord.Register(DuplicateAction.None);

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            theRegistryRecord.AddBatch();
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);

            int iBachID = theRegistryRecord.BatchList[0].ID;
            theBatch.ID = iBachID;

            PrivateObject thePrivateObject = new PrivateObject(theBatch);
            thePrivateObject.Invoke("DataPortal_DeleteSelf", new object[] { });

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);

            Batch theBatchExist = theRegistryRecord.BatchList.GetBatchById(iBachID);

            Assert.IsNull(theBatchExist, "Batch.DataPortal_DeleteSelf() did not return the expected value.");

        }

        /// <summary>
        /// Updating Batch XML
        /// </summary>
        [TestMethod]
        public void UpdateXml_UpdatingBatchXML()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            theBatch.DateCreated = DateTime.Now;
            theBatch.PersonCreated = 1;
            theBatch.PersonRegistered = 1;
            theBatch.PersonApproved = 1;
            theBatch.DateLastModified = DateTime.Now;

            theBatch.UpdateXml();

            PrivateObject thePrivateObject = new PrivateObject(theBatch);
            object theResult = thePrivateObject.GetFieldOrProperty("_xml");

            Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "Batch.UpdateXml() did not return the expected value.");
        }

        /// <summary>
        /// Updating Batch from XML
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_UpdatingBatchFromXml()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            if (theBatch.ProjectList.Count > 0)
            {
                theBatch.ProjectList[0].Delete();
            }

            if (theBatch.IdentifierList.Count > 0)
            {
                theBatch.IdentifierList[0].Delete();
            }
            if (theBatch.PropertyList.Count > 0)
            {
                theBatch.PropertyList[0].Delete();
            }
            if (theBatch.BatchComponentList.Count > 0)
            {
                theBatch.BatchComponentList[0].RegNum = "1";
            }

            theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList/Batch");
            theBatch.UpdateFromXml(theXmlNode);

            if (theBatch.ProjectList.Count > 0)
            {
                Assert.IsTrue(theBatch.ProjectList[0].IsDirty, "Batch.UpdateFromXml(theXmlNode) did not return the expected value.");
            }
            else if (theBatch.IdentifierList.Count > 0)
            {
                Assert.IsTrue(theBatch.IdentifierList[0].IsDirty, "Batch.UpdateFromXml(theXmlNode) did not return the expected value.");
            }
            else if (theBatch.PropertyList.Count > 0)
            {
                Assert.IsTrue(theBatch.PropertyList[0].IsDirty, "Batch.UpdateFromXml(theXmlNode) did not return the expected value.");
            }
            else if (theBatch.BatchComponentList.Count > 0)
            {
                Assert.IsTrue(theBatch.BatchComponentList[0].IsDirty, "Batch.UpdateFromXml(theXmlNode) did not return the expected value.");
            }
        }

        /// <summary>
        /// Updating Batch from XML
        /// </summary>
        [TestMethod]
        public void DataPortal_Update_UpdatingBatchFromXml()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./MultiCompoundRegistryRecord/BatchList");

            Batch theBatch = Batch.NewBatch(theXmlNode.InnerXml, true, true);

            if (theBatch.ProjectList.Count > 0)
            {
                theBatch.ProjectList[0].Delete();
            }

            if (theBatch.IdentifierList.Count > 0)
            {
                theBatch.IdentifierList[0].Delete();
            }
            if (theBatch.PropertyList.Count > 0)
            {
                theBatch.PropertyList[0].Delete();
            }
            if (theBatch.BatchComponentList.Count > 0)
            {
                theBatch.BatchComponentList[0].RegNum = "1";
            }

            PrivateObject thePrivateObject = new PrivateObject(theBatch);
            thePrivateObject.Invoke("DataPortal_Update", new object[] { });

            if (theBatch.ProjectList.Count > 0)
            {
                Assert.IsTrue(theBatch.ProjectList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
            }
            else if (theBatch.IdentifierList.Count > 0)
            {
                Assert.IsTrue(theBatch.IdentifierList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
            }
            else if (theBatch.PropertyList.Count > 0)
            {
                Assert.IsTrue(theBatch.PropertyList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
            }
            else if (theBatch.BatchComponentList.Count > 0)
            {
                Assert.IsTrue(theBatch.BatchComponentList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
            }
        }

        /// <summary>
        /// Updating Temparary Batch from XML
        /// </summary>
        [TestMethod]
        public void DataPortal_Update_UpdatingTempararyBatchFromXml()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            StringBuilder cmdString = new StringBuilder();
            cmdString.Append("SELECT DISTINCT TEMPBATCHID FROM REGDB.VW_TEMPORARYBATCH");

            DataTable dtBatchList = DALHelper.ExecuteQuery(cmdString.ToString());

            if (dtBatchList != null && dtBatchList.Rows.Count > 0)
            {
                Batch theBatch = Batch.GetBatch(Convert.ToInt32(Convert.ToString(dtBatchList.Rows[0]["TEMPBATCHID"])), true);

                if (theBatch.ProjectList.Count > 0)
                {
                    theBatch.ProjectList[0].Delete();
                }

                if (theBatch.IdentifierList.Count > 0)
                {
                    theBatch.IdentifierList[0].Delete();
                }
                if (theBatch.PropertyList.Count > 0)
                {
                    theBatch.PropertyList[0].Delete();
                }
                if (theBatch.BatchComponentList.Count > 0)
                {
                    theBatch.BatchComponentList[0].RegNum = "1";
                }

                PrivateObject thePrivateObject = new PrivateObject(theBatch);
                thePrivateObject.Invoke("DataPortal_Update", new object[] { });

                if (theBatch.ProjectList.Count > 0)
                {
                    Assert.IsTrue(theBatch.ProjectList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
                }
                else if (theBatch.IdentifierList.Count > 0)
                {
                    Assert.IsTrue(theBatch.IdentifierList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
                }
                else if (theBatch.PropertyList.Count > 0)
                {
                    Assert.IsTrue(theBatch.PropertyList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
                }
                else if (theBatch.BatchComponentList.Count > 0)
                {
                    Assert.IsTrue(theBatch.BatchComponentList[0].IsDirty, "Batch.DataPortal_Update() did not return the expected value.");
                }
            }
            else
            {
                Assert.IsNotNull(dtBatchList, "Batches are not exist");
            }
        }

        #endregion
    }
}
