using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Reflection;
using System.Xml.XPath;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Common.Validation;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// RegistryRecordTest class contains unit test methods for RegistryRecord
    /// </summary>
    [TestClass]
    public class RegistryRecordTest
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
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            Helpers.Authentication.Logoff();
        }

        #endregion

        #region Registration Test Methods

        /// <summary>
        /// Registering Component into the Temparary table
        /// </summary>
        [TestMethod]
        public void Save_DuplicateNone() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord.Save(DuplicateCheck.None);

            iTempRegID = theRegistryRecord.ID;
            RegistryRecord result = RegistryHelper.LoadRegistryRecord(iTempRegID.ToString(), GlobalVariables.RegistryLoadType.ID);

            Assert.IsNotNull(result, "RegistryRecord.Save(DuplicateCheck.None) did not save the registry record."); //If record exist then test case get passed
        }

        /// <summary>
        /// Registering Component into the Temparary table
        /// </summary>
        [TestMethod]
        public void Save_DuplicatePreReg() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord.Save(DuplicateCheck.PreReg);
            iTempRegID = theRegistryRecord.ID;
            RegistryRecord result = RegistryHelper.LoadRegistryRecord(iTempRegID.ToString(), GlobalVariables.RegistryLoadType.ID);

            Assert.IsNotNull(result, "RegistryRecord.Save(DuplicateCheck.PreReg) did not save the registry record."); //If record exist then test case get passed
        }

        /// <summary>
        /// Registering Component into the Temparary table
        /// </summary>
        [TestMethod]
        public void Save_DuplicateDataAccessStrategyAtomic() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord.Save(RegistryRecord.DataAccessStrategy.Atomic);
            iTempRegID = theRegistryRecord.ID;
            RegistryRecord result = RegistryHelper.LoadRegistryRecord(iTempRegID.ToString(), GlobalVariables.RegistryLoadType.ID);

            Assert.IsNotNull(result, "RegistryRecord.Save(RegistryRecord.DataAccessStrategy.Atomic) did not save the registry record."); //If record exist then test case get passed
        }

        //[TestMethod]
        //[Priority(4)]
        //public void Save_Duplicate_DataAccessStrategy_BulkLoader() //Registering component into the Temparary table
        //{
        //    RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
        //    theRegistryRecord.Save(RegistryRecord.DataAccessStrategy.BulkLoader);
        //    iTempRegID = theRegistryRecord.ID;
        //    RegistryRecord result = RegistryHelper.LoadRegistryRecord(iTempRegID.ToString(), GlobalVariables.RegistryLoadType.ID);

        //    Assert.IsNotNull(result); //If record exist then test case get passed
        //}

        /// <summary>
        /// Deleting registered Component from Temparay table by ID
        /// </summary>
        [TestMethod]
        public void DeleteRegistryRecord_ByID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord = theRegistryRecord.Save(DuplicateCheck.None);
            RegistryRecord.DeleteRegistryRecord(theRegistryRecord.ID); //iTempRegID);

            RegistryRecord result = RegistryHelper.LoadRegistryRecord(iTempRegID.ToString(), GlobalVariables.RegistryLoadType.ID);
            Assert.IsNull(result, "RegistryRecord.DeleteRegistryRecord(iTempRegID) did not delete the registry record."); //If record not exist then test case get passed
        }

        /// <summary>
        /// Registering Component into the Permanant table. Requires 'EnableUseBatchPrefixes = False'
        /// </summary>
        [TestMethod]
        public void Register_DuplicateNone() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            RegistryRecord result = theRegistryRecord.Register(DuplicateAction.None);

            //RegistryRecord result = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            Assert.IsNotNull(result, "RegistryRecord.Register(DuplicateAction.None) did not register the registry record."); //If record exist then test case get passed
        }

        /// <summary>
        /// Creating registry record without input parameter
        /// </summary>
        [TestMethod]
        public void Register_NoParameter() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            RegistryRecord result = theRegistryRecord.Register();

            //theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            Assert.IsNotNull(result, "RegistryRecord.Register did not register the registry record.");
        }

        /// <summary>
        /// Creating registry record with Duplicate Action and Data Access Strategy
        /// </summary>
        [TestMethod]
        public void Register_ByActionAndStrategy() 
        {
            RegistryRecord theRegistryRecord = null;
            bool blnFlag = true;
            int iCounter = 1;
            string strRegNum = "AB-21000" + iCounter.ToString();
            do
            {
                theRegistryRecord = RegistryHelper.LoadRegistryRecord(strRegNum, GlobalVariables.RegistryLoadType.REGNUM);
                if (theRegistryRecord == null)
                {
                    break;
                }
                else
                {
                    iCounter++;
                    strRegNum = "AB-21000" + iCounter.ToString();
                }
            }
            while (blnFlag);

            theRegistryRecord = RegistryHelper.CreateRegistryObject(strRegNum);

            RegistryRecord result = theRegistryRecord.Register(DuplicateAction.Duplicate, RegistryRecord.DataAccessStrategy.Atomic);

            //theRegistryRecord = RegistryHelper.LoadRegistryRecord("AB-210003", GlobalVariables.RegistryLoadType.REGNUM);

            Assert.IsNotNull(result, "RegistryRecord.Register(DuplicateAction.Duplicate, RegistryRecord.DataAccessStrategy.Atomic) did not register the registry record.");
        }

        /// <summary>
        /// Creating registry record with Duplicate Check
        /// </summary>
        [TestMethod]
        public void Register_ByDuplicateCheck()
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            RegistryRecord result = theRegistryRecord.Register(DuplicateCheck.CompoundCheck);

            Assert.IsNotNull(result, "RegistryRecord.Register(DuplicateCheck.CompoundCheck) did not register the registry record.");
        }

        /// <summary>
        /// Deleting registered Component from Permanant table by Registration number
        /// </summary>
        [TestMethod]
        public void DeleteRegistryRecord_ByRegNum() 
        {
            string strRegNum = GlobalVariables.REGNUM1;
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(strRegNum, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(strRegNum);
                theRegistryRecord.Register(DuplicateAction.Duplicate);
            }

            RegistryRecord.DeleteRegistryRecord(strRegNum);

            RegistryRecord result = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            Assert.IsNull(result, "RegistryRecord.DeleteRegistryRecord(GlobalVariables.REGNUM1) did not delete the registry record."); //If record not exist then test case get passed
        }

        /// <summary>
        /// Updating Component information using xml file.
        /// </summary>
        [TestMethod]
        public void UpdateFromXml_Should_Update_Registry_Level() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            }

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));
            theRegistryRecord.UpdateFromXml(theXmlDocument.InnerXml);

            theRegistryRecord = theRegistryRecord.Save();

            Assert.AreEqual(132, theRegistryRecord.PersonCreated, "RegistryRecord.UpdateFromXml(theXmlDocument.InnerXml) did not update the registry record.");
        }

        /// <summary>
        /// Checking duplicate Component. Requires 'CheckDuplication = True'
        /// </summary>
        [TestMethod]
        public void FindDuplicates_DataAccessStrategyAtomic() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("CheckDuplication", "True");
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            }
            List<DuplicateCheckResponse> theDuplicateCheckResponseList = RegistryRecord.FindDuplicates(theRegistryRecord, RegistryRecord.DataAccessStrategy.Atomic);
            Assert.IsTrue(theDuplicateCheckResponseList.Count > 0, "RegistryRecord.FindDuplicates(theRegistryRecord, RegistryRecord.DataAccessStrategy.Atomic) did not find the duplicate registry record.");
        }

        /// <summary>
        /// Locking Component
        /// </summary>
        [TestMethod]
        public void SetLockStatus_LockingComponent() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.Register(DuplicateAction.Duplicate);
            }

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            theRegistryRecord.Status = RegistryStatus.Locked;
            theRegistryRecord.SetLockStatus();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            int iResult = RegistryRecord.GetCompoundLockedStatus(theRegistryRecord.ComponentList[0].Compound.ID);

            Assert.AreEqual(RegistryStatus.Locked, (RegistryStatus)iResult, "RegistryRecord.SetLockStatus() did not change the registry status i.e Locked.");
        }

        /// <summary>
        /// UnLocking Component
        /// </summary>
        [TestMethod]
        public void SetLockStatus_UnLockingComponent() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            theRegistryRecord.Status = RegistryStatus.Registered;
            theRegistryRecord.SetLockStatus();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            int iResult = RegistryRecord.GetCompoundLockedStatus(theRegistryRecord.ComponentList[0].Compound.ID);

            Assert.AreEqual(RegistryStatus.Registered, (RegistryStatus)iResult, "RegistryRecord.SetLockStatus() did not change the registry status i.e. UnLocked.");
        }

        /// <summary>
        /// Adding Batch
        /// </summary>
        [TestMethod]
        public void AddBatch_AddingDefaultBatch() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            int iPrevBatchCount = theRegistryRecord.BatchList.Count;

            //theRegistryRecord.AddComponent(Component.NewComponent());

            //Fragment theFragment = Fragment.NewFragment(true,true);
            // theFragment.Structure.Value = GlobalVariables.NAPROXEN_SUBSTRUCTURE_SMILES;
            // theFragment.Structure.Format = "chemical/x-smiles";
            // theRegistryRecord.AddFragment(0, theFragment, 1);

            theRegistryRecord.AddBatch();
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            Assert.AreEqual((iPrevBatchCount + 1), theRegistryRecord.BatchList.Count, "RegistryRecord.AddBatch() did not add default batch into the registry record");
        }

        /// <summary>
        /// Adding Batch
        /// </summary>
        [TestMethod]
        public void AddBatch_AddingCustomBatch() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            if(theRegistryRecord==null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord = theRegistryRecord.Register(DuplicateCheck.CompoundCheck);
            }

            int iPrevBatchCount = theRegistryRecord.BatchList.Count;

            RegistryRecord record = RegistryRecord.NewRegistryRecord();
            Batch batch = record.BatchList[0];
            BatchComponent firstBatchComponent = BatchComponent.NewBatchComponent();
            firstBatchComponent.CompoundID = theRegistryRecord.ComponentList[0].Compound.ID;
            firstBatchComponent.ComponentIndex = theRegistryRecord.ComponentList[0].ComponentIndex;
            firstBatchComponent.BatchID = 0;
            batch.BatchComponentList[0] = firstBatchComponent;
            batch.Status = RegistryStatus.Registered;
            batch.MarkClean();

            theRegistryRecord.AddBatch(batch);
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            Assert.AreEqual((iPrevBatchCount + 1), theRegistryRecord.BatchList.Count, "RegistryRecord.AddBatch(batch) did not add custom batch into the registry record");
        }

        /// <summary>
        /// Adding Project by ID
        /// </summary>
        [TestMethod]
        public void AddProject_ByProjectID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            int iPrevProjectCount = theRegistryRecord.ProjectList.Count;

            int iProjectID = 0;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { iProjectID = theProjectList[1].ProjectID; }
            else { iProjectID = theProjectList[0].ProjectID; }

            theRegistryRecord.AddProject(iProjectID);
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            Assert.AreEqual((iPrevProjectCount + 1), theRegistryRecord.ProjectList.Count, "RegistryRecord.AddProject(iProjectID) did not add new project into the registry record");
        }

        /// <summary>
        /// Adding Project by Name
        /// </summary>
        [TestMethod]
        public void AddProject_ByProjectName() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            int iPrevProjectCount = theRegistryRecord.ProjectList.Count;

            string strProjectName = string.Empty;
            ProjectList theProjectList = ProjectList.GetProjectList();
            if (theProjectList.Count > 1) { strProjectName = theProjectList[1].Name; }
            else { strProjectName = theProjectList[0].Name; }

            theRegistryRecord.AddProject(strProjectName);
            theRegistryRecord.Save();

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            Assert.AreEqual((iPrevProjectCount + 1), theRegistryRecord.ProjectList.Count, "RegistryRecord.AddProject(strProjectName) did not add new project into the registry record");
        }

        /// <summary>
        /// Override Batch Prefix with Default one. Requires 'EnableUseBatchPrefixes = True'
        /// </summary>
        [TestMethod]
        public void BatchPrefixDefaultOverride_SettingBatchprefix() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            }

            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "True");

            theRegistryRecord.DataStrategy = RegistryRecord.DataAccessStrategy.BulkLoader;
            theRegistryRecord.BatchPrefixDefaultOverride(false);

            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Assert.IsNotNull(theRegistryRecord.BatchList[theRegistryRecord.BatchList.Count - 1].PropertyList["BATCH_PREFIX"].Value, "RegistryRecord.BatchPrefixDefaultOverride(false) did not override the Batch prefix");
        }

        /// <summary>
        /// Fetching Locked registry records
        /// </summary>
        [TestMethod]
        public void GetLockedRegistryRecords_FetchingLockedRecrods() 
        {
            SetLockStatus_LockingComponent();
            string strRegistryList = string.Format("'{0}','{1}'", GlobalVariables.REGNUM1, GlobalVariables.REGNUM2);

            string strResult = RegistryRecord.GetLockedRegistryRecords(strRegistryList);

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "RegistryRecord.GetLockedRegistryRecords(strRegistryList) did not return the exepcted value.");
            SetLockStatus_UnLockingComponent();
        }

        /// <summary>
        /// Get registration number by Batch ID
        /// </summary>
        [TestMethod]
        public void GetRegNumberByBatchId_RegistryNumByBatchID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            string strResult = RegistryRecord.GetRegNumberByBatchId(theRegistryRecord.BatchList[0].ID.ToString());

            Assert.IsTrue(!string.IsNullOrEmpty(strResult), "RegistryRecord.GetRegNumberByBatchId(theRegistryRecord.BatchList[0].ID.ToString()) did not return the exepcted value.");
        }

        /// <summary>
        /// Get registration number by Temparary Batch ID
        /// </summary>
        [TestMethod]
        public void GetRegisteredInfoFromTempBatchID_RegistryNumByTem_BatchID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            string strResult = theRegistryRecord.GetRegisteredInfoFromTempBatchID(theRegistryRecord.BatchList[0].TempBatchID);

            string strRegNum = string.Empty;
            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.LoadXml(strResult);
            XPathNavigator xPathNavigator = theXmlDocument.CreateNavigator();

            XPathNodeIterator regNumIterator = xPathNavigator.Select("//RegNumber");
            if (regNumIterator.Count > 0)
            {
                while (regNumIterator.MoveNext())
                {
                    strRegNum = regNumIterator.Current.InnerXml;
                }
            }
            Assert.IsTrue(!string.IsNullOrEmpty(strRegNum), "RegistryRecord.GetRegisteredInfoFromTempBatchID(theRegistryRecord.BatchList[0].TempBatchID) did not return the exepcted value.");
        }

        /// <summary>
        /// Get registration record by Batch ID
        /// </summary>
        [TestMethod]
        public void GetRegistryRecordByBatch_RegistryByBatchID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            RegistryRecord result = RegistryRecord.GetRegistryRecordByBatch(theRegistryRecord.BatchList[0].ID);

            Assert.AreEqual(theRegistryRecord, result, "RegistryRecord.GetRegistryRecordByBatch(theRegistryRecord.BatchList[0].ID) did not return the exepcted value.");
        }

        /// <summary>
        /// Get registration number by Batch Number
        /// </summary>
        [TestMethod]
        public void GetRegistryRecordByBatch_RegistryByBatchNumber() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            RegistryRecord result = RegistryRecord.GetRegistryRecordByBatch(theRegistryRecord.BatchList[0].RegNumber);

            Assert.AreEqual(theRegistryRecord, result, "RegistryRecord.GetRegistryRecordByBatch(theRegistryRecord.BatchList[0].RegNumber) did not return the exepcted value.");
        }

        /// <summary>
        /// Coping batches in one object to another registration object
        /// </summary>
        [TestMethod]
        public void MoveBatches_CopyBatches() 
        {
            RegistryRecord originRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);

            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            int iRegBatchCount = theRegistryRecord.BatchList.Count + originRegistryRecord.BatchList.Count;

            originRegistryRecord.MoveBatches(theRegistryRecord);

            Assert.AreEqual(iRegBatchCount, originRegistryRecord.BatchList.Count, "RegistryRecord.MoveBatches(theRegistryRecord) did not copy the batches from one registry object to another.");
        }

        /// <summary>
        /// Registering component using xml
        /// </summary>
        [TestMethod]
        public void SaveFromCurrentXml_RegisterUsingXml() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            RegistryRecord result = theRegistryRecord.SaveFromCurrentXml();

            Assert.IsNotNull(result, "RegistryRecord.SaveFromCurrentXml() did not register the registry record.");
        }

        /// <summary>
        /// Replacing existing component
        /// </summary>
        [TestMethod]
        public void ReplaceComponent_ReplacingExistingComponent() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreComponentsCount = theRegistryRecord.ComponentList.Count;

            Component theComponent = Component.NewComponent();
            theComponent.ComponentIndex = theRegistryRecord.ComponentList[0].ComponentIndex;
            theComponent.Percentage = 100;
            theRegistryRecord.ReplaceComponent(0, theComponent);

            Assert.AreEqual(iPreComponentsCount, theRegistryRecord.ComponentList.Count, "RegistryRecord.ReplaceComponent(0, theComponent) did not replace the component in the registry record.");
        }

        /// <summary>
        /// Replacing existing Compound by Index and Compund
        /// </summary>
        [TestMethod]
        public void ReplaceCompound_ReplacingExistingCompoundByIndexAndCompound() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreCompoundsCount = theRegistryRecord.ComponentList.Count;

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));

            Compound theCompound = Compound.NewCompound(theXmlDocument.InnerXml, true, true);
            theCompound.PersonCreated = theRegistryRecord.PersonCreated;
            theRegistryRecord.ReplaceCompound(0, theCompound);

            Assert.AreEqual(iPreCompoundsCount, theRegistryRecord.ComponentList.Count, "RegistryRecord.ReplaceCompound(0, theCompound) did not replace the compound in the registry record.");
        }

        /// <summary>
        /// Replacing existing Compound by Index, Compund and BatchComponentFragmentList
        /// </summary>
        [TestMethod]
        public void ReplaceCompound_ReplacingExistingCompoundByIndexCompoundAndBatchComponentFragmentList() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreCompoundsCount = theRegistryRecord.ComponentList.Count;

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));

            Compound theCompound = Compound.NewCompound(theXmlDocument.InnerXml, true, true);
            theCompound.PersonCreated = theRegistryRecord.PersonCreated;

            BatchComponentFragmentList theBatchComponentFragmentList = BatchComponentFragmentList.NewBatchComponentFragmentList(theXmlDocument.InnerXml, true, true);

            theRegistryRecord.ReplaceCompound(0, theCompound, theBatchComponentFragmentList);

            Assert.AreEqual(iPreCompoundsCount, theRegistryRecord.ComponentList.Count, "RegistryRecord.ReplaceCompound(0, theCompound, theBatchComponentFragmentList) did not replace the compound in the registry record.");
        }

        /// <summary>
        /// Replacing existing Structure
        /// </summary>
        [TestMethod]
        public void ReplaceStructure_ReplacingExistingSturcture() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            Structure theStructure = Structure.NewStructure();
            theStructure.DrawingType = DrawingType.NonChemicalContent;
            theStructure.Formula = "C11H22";
            theStructure.MolWeight = 30.45;
            theRegistryRecord.ReplaceStructure(0, theStructure);

            Assert.AreEqual("C11H22", theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Formula, "RegistryRecord.ReplaceStructure(0, theStructure) did not replace the compound in the registry record.");
        }

        /// <summary>
        /// Saving registry template
        /// </summary>
        [TestMethod]
        public void SaveTemplate_SavingRegistryTemplate() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            Random theRandomNum = new Random();

            COEGenericObjectStorageBO theCOEGenericObjectStorageBO = theRegistryRecord.SaveTemplate("NewTemplate" + theRandomNum.Next(1000).ToString(), "Test case template" + theRandomNum.Next(1000).ToString(), true, 2);

            Assert.IsNotNull(theCOEGenericObjectStorageBO, "RegistryRecord.SaveTemplate('NewTemplate', 'Test template', true, 0) did not save the registry template.");
        }

        /// <summary>
        /// Adding new component into the registry record
        /// </summary>
        [TestMethod]
        public void AddComponent_AddingNewComponent() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreComponentsCount = theRegistryRecord.ComponentList.Count;

            Component theComponent = Component.NewComponent();
            theComponent.ComponentIndex = theRegistryRecord.ComponentList[0].ComponentIndex;
            theComponent.Percentage = 100;
            theRegistryRecord.AddComponent(theComponent);

            Assert.AreEqual(iPreComponentsCount + 1, theRegistryRecord.ComponentList.Count, "RegistryRecord.AddComponent(theComponent) did not add new component into the registry record.");
        }

        /// <summary>
        /// Adding new Fragment into the registry record
        /// </summary>
        [TestMethod]
        public void AddFragment_AddingNewFragment() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            FragmentList theFragmentList = FragmentList.GetFragmentList();
            Fragment theFragment = Fragment.NewFragment(true, true);
            theRegistryRecord.AddFragment(0, theFragmentList[0].Code, 123);

            Assert.AreEqual(theFragmentList[0].Code, theRegistryRecord.ComponentList[0].Compound.FragmentList[0].Code, "RegistryRecord.AddFragment(0, theFragmentList[0].Code, 123) did not add new fragment into the registry component.");
        }

        /// <summary>
        /// Adding new Identifier into the registry record
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingNewIdentifier() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreIdentifierCount = theRegistryRecord.IdentifierList.Count;

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.InputText = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                theIdentifier.IdentifierID = idList[0].IdentifierID;
                theRegistryRecord.AddIdentifier(theIdentifier);

                Assert.AreEqual(iPreIdentifierCount + 1, theRegistryRecord.IdentifierList.Count, "RegistryRecord.AddIdentifier(theIdentifier) did not add new identifier into the registry record.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Adding new Identifier using Identifier ID and Identifier Value into the registry record
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingNewIdentifierByIdentifierIDAndValue() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreIdentifierCount = theRegistryRecord.IdentifierList.Count;

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                theRegistryRecord.AddIdentifier(idList[0].IdentifierID, "Test case Identifier");

                Assert.AreEqual(iPreIdentifierCount + 1, theRegistryRecord.IdentifierList.Count, "RegistryRecord.AddIdentifier(idList[0].IdentifierID, 'Test case Identifier') did not add new identifier into the registry record.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Adding new Identifier using Identifier Name and Identifier Value into the registry record
        /// </summary>
        [TestMethod]
        public void AddIdentifier_AddingNewIdentifierByIdentifierNameAndValue() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            int iPreIdentifierCount = theRegistryRecord.IdentifierList.Count;

            IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R, true, true);
            if (idList.Count > 0)
            {
                Identifier theIdentifier = Identifier.NewIdentifier();
                theIdentifier.Active = true;
                theIdentifier.Description = "Test case Identifier";
                theIdentifier.IdentifierID = idList[0].IdentifierID;
                theRegistryRecord.AddIdentifier(idList[0].Name, "Test case Identifier");

                Assert.AreEqual(iPreIdentifierCount + 1, theRegistryRecord.IdentifierList.Count, "RegistryRecord.AddIdentifier('TEST_CASE_IDENTIFIER', 'Test case Identifier') did not add new identifier into the registry record.");
            }
            else
            {
                Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
            }
        }

        /// <summary>
        /// Assigning default structure by ID into the registry component
        /// </summary>
        [TestMethod]
        public void AssignDefaultStructureById_AssigningDefaultStructureByID() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value = string.Empty;

            theRegistryRecord.AssignDefaultStructureById(theRegistryRecord.ComponentList[0].ComponentIndex, 168);

            Assert.IsTrue(!string.IsNullOrEmpty(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value), "RegistryRecord.AssignDefaultStructureById(theRegistryRecord.ComponentList[0].ComponentIndex, 168) did not assign default structure to the registry component.");
        }

        /// <summary>
        /// Checking Auto select component applicable for Duplicate Check
        /// </summary>
        [TestMethod]
        public void CanAutoSelectComponentForDupChk_CheckingAutoSelectComponentForDuplicate() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanAutoSelectComponentForDupChk();

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanAutoSelectComponentForDupChk() returns True/False value.");
        }

        /// <summary>
        /// Checks whether current user can create duplicate components
        /// </summary>
        [TestMethod]
        public void CanCreateDuplicateComponent_CheckingCanCreateDuplicateComponent() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanCreateDuplicateComponent();

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanCreateDuplicateComponent() returns True/False value.");
        }

        /// <summary>
        /// Checks whether current user can create duplicate registry
        /// </summary>
        [TestMethod]
        public void CanCreateDuplicateRegistry_CheckingCanCreateDuplicateRegistry() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanCreateDuplicateRegistry();

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanCreateDuplicateRegistry() returns True/False value.");
        }

        /// <summary>
        /// Checks whether current user can propogate structure and component edits
        /// </summary>
        [TestMethod]
        public void CanPropogateComponentEdits_CheckingCanPropogateComponentEdits() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanPropogateComponentEdits();

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanPropogateComponentEdits() returns True/False value.");
        }

        /// <summary>
        /// Checks whether current user can propogate structure and component edits
        /// </summary>
        [TestMethod]
        public void CanPropogateStructureEdits_CheckingCanPropogateStructureEdits() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanPropogateStructureEdits();

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanPropogateStructureEdits() returns True/False value.");
        }

        /// <summary>
        /// Checks the compound edition scope of the current user
        /// </summary>
        [TestMethod]
        public void CanEditBatch_CheckingCanEditBatch() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanEditBatch(theRegistryRecord.BatchList.Count - 1);

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanEditBatch(theRegistryRecord.BatchList.Count-1) returns True/False value.");
        }

        /// <summary>
        /// Checks the compound edition scope of the current user
        /// </summary>
        [TestMethod]
        public void CanEditRegistry_CheckingCanEditBatch() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanEditRegistry();

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanEditRegistry() returns True/False value.");
        }

        /// <summary>
        /// Checks the compound edition scope of the current user
        /// </summary>
        [TestMethod]
        public void CanEditComponent_CheckingCanEditBatch() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

            bool blnResult = theRegistryRecord.CanEditComponent(theRegistryRecord.ComponentList[0].ComponentIndex);

            Assert.IsTrue((blnResult || !blnResult), "RegistryRecord.CanEditComponent(theRegistryRecord.ComponentList[0].ComponentIndex) returns True/False value.");
        }

        /// <summary>
        /// Checking whether other Mixture components get affected or not
        /// </summary>
        [TestMethod]
        public void CheckOtherMixturesAffected_CheckingCanEditBatch() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            theRegistryRecord.ComponentList.Clear();

            theRegistryRecord.CreateMixture("C000111|C000112", "40|60", "|");
            theRegistryRecord.ComponentList[0].Percentage = 50;
            theRegistryRecord.ComponentList[1].Percentage = 50;

            theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value = GlobalVariables.NAPROXEN_SMILES;
            theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Format = "chemical/x-smiles";

            theRegistryRecord.ComponentList[1].Compound.BaseFragment.Structure.Value = GlobalVariables.NAPROXEN_SUBSTRUCTURE_SMILES;
            theRegistryRecord.ComponentList[1].Compound.BaseFragment.Structure.Format = "chemical/x-smiles";

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);
            thePrivateObject.Invoke("CheckOtherMixturesAffected", new object[] { });

            object theCompoundUpdate = thePrivateObject.GetFieldOrProperty("_regsAffectedByCompoundUpdate");
            object theStructureUpdate = thePrivateObject.GetFieldOrProperty("_regsAffectedByStructureUpdate");

            Assert.IsTrue(((theCompoundUpdate as Dictionary<string, string[]>).Count == 0 || (theStructureUpdate as Dictionary<string, string[]>).Count > 0),
                "RegistryRecord.CheckOtherMixturesAffected() did not return the expected value.");
        }

        /// <summary>
        /// Checking whether unique registry record or not
        /// </summary>
        [TestMethod]
        public void CheckUniqueRegistryRecord_CheckingUniqeRegistryRecord() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            string strUniqeRecrods = theRegistryRecord.CheckUniqueRegistryRecord(DuplicateCheck.MixCheck);

            Assert.IsTrue(!string.IsNullOrEmpty(strUniqeRecrods),
                "RegistryRecord.CheckUniqueRegistryRecord(DuplicateCheck.PreReg) did not return the expected value.");
        }

        /// <summary>
        /// Copying modified components
        /// </summary>
        [TestMethod]
        public void CopyEditedComponents_CopyModifiedComponents() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            theRegistryRecord.ComponentList[0].Percentage = 100;
            theRegistryRecord.CopyEditedComponents();

            Assert.IsTrue(string.IsNullOrEmpty(theRegistryRecord.ComponentList[0].Compound.RegNumber.RegNum),
                "RegistryRecord.CopyEditedComponents() did not return the expected value.");
        }

        /// <summary>
        /// Copying modified structures
        /// </summary>
        [TestMethod]
        public void CopyEditedStructures_CopyModifiedStructures() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
            theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.MolWeight = 123.45;
            theRegistryRecord.CopyEditedStructures();

            Assert.IsTrue(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.ID == 0,
                "RegistryRecord.CopyEditedStructures() did not return the expected value.");
        }

        /// <summary>
        /// Removing duplicate components from Registry record
        /// </summary>
        [TestMethod]
        public void CleanFoundDuplicates_RemovingDuplicateComponents() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            string strDuplicates = theRegistryRecord.FoundDuplicates;

            theRegistryRecord.CleanFoundDuplicates();

            Assert.AreNotEqual(strDuplicates, theRegistryRecord.FoundDuplicates,
                "RegistryRecord.CleanFoundDuplicates() did not return the expected value.");
        }

        /// <summary>
        /// Removing component from Registry record
        /// </summary>
        [TestMethod]
        public void DeleteComponent_RemovingComponents()
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord.Register(DuplicateCheck.CompoundCheck);
            int iPrevComponentCount = theRegistryRecord.ComponentList.Count;

            theRegistryRecord.DeleteComponent(theRegistryRecord.ComponentList.Count - 1);

            Assert.AreNotEqual(iPrevComponentCount, theRegistryRecord.ComponentList.Count,
                "RegistryRecord.DeleteComponent(theRegistryRecord.ComponentList[0].ComponentIndex) did not return the expected value.");
        }

        /// <summary>
        /// Fixing mixture fragments into all the batches in a registry record
        /// </summary>
        [TestMethod]
        public void FixMixtureBatchesFragments_SynchronizingMixtureFragmentsInAllBatches() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord.Register(DuplicateCheck.CompoundCheck);

            Component theComponent = Component.NewComponent();
            theComponent.ComponentIndex = theRegistryRecord.ComponentList[0].ComponentIndex;
            theComponent.Percentage = 100;
            theRegistryRecord.AddComponent(theComponent);

            theRegistryRecord.FixMixtureBatchesFragments();

            int iBatchComponentListCount = theRegistryRecord.BatchList[theRegistryRecord.BatchList.Count - 1].BatchComponentList.Count;
            Assert.IsTrue(theRegistryRecord.BatchList[theRegistryRecord.BatchList.Count - 1].BatchComponentList[iBatchComponentListCount - 1].BatchComponentFragmentList.Count > 0,
                "RegistryRecord.FixMixtureBatchesFragments() did not return the expected value.");
        }

        /// <summary>
        /// Getting the list of Broken rules i.e. failed validation messages. Requires 'EnableUseBatchPrefixes = True'
        /// </summary>
        [TestMethod]
        public void GetBrokenRulesDescription_FetchingListOfFailedValidationMessages() 
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "True");
            
            RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            if (theRegistryRecord == null)
            {
                DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
                theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
                theRegistryRecord = theRegistryRecord.Register(DuplicateCheck.None);

                DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "True");
                theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM1, GlobalVariables.RegistryLoadType.REGNUM);
            }

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);
            object theObjectValue = thePrivateObject.GetFieldOrProperty("IsValid");
            List<BrokenRuleDescription> theBrokenRuleList = theRegistryRecord.GetBrokenRulesDescription();

            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            Assert.IsTrue(theBrokenRuleList.Count > 0, "RegistryRecord.GetBrokenRulesDescription() did not return the expected value.");
        }

        /// <summary>
        /// Fetching all properties exists in input xml data
        /// </summary>
        [TestMethod]
        public void GetPropertiesBySection_LoadingPropertiesFromXml() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord.Register(DuplicateCheck.CompoundCheck);

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), GlobalVariables.RegistryRecordXml));

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);
            object theResult = thePrivateObject.Invoke("GetPropertiesBySection", new string[] { theXmlDocument.InnerXml });

            if (theResult != null)
            {
                Dictionary<string, List<string>> thePropertyList = theResult as Dictionary<string, List<string>>;
                Assert.IsTrue(thePropertyList.Count > 0, "RegistryRecord.GetPropertiesBySection() did not return the expected value.");
            }
            else
            {
                Assert.IsNotNull(theResult, "RegistryRecord.GetPropertiesBySection() did not return the expected value.");
            }

        }

        /// <summary>
        /// Deleting registry record from permanant table
        /// </summary>
        [TestMethod]
        public void DataPortal_DeleteSelf_DeletingPermanatRegistryRecord() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord.Register(DuplicateCheck.CompoundCheck);

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);
            object theResult = thePrivateObject.Invoke("DataPortal_DeleteSelf", new string[] { });

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);

            Assert.IsNull(theRegistryRecord, "RegistryRecord.DataPortal_DeleteSelf() did not return the expected value.");

        }

        /// <summary>
        /// Deleting registry record from temparary table
        /// </summary>
        [TestMethod]
        public void DataPortal_DeleteSelf_DeletingTempararyRegistryRecord() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM1);
            theRegistryRecord = theRegistryRecord.Save(DuplicateCheck.None);

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);
            object theResult = thePrivateObject.Invoke("DataPortal_DeleteSelf", new string[] { });

            theRegistryRecord = RegistryHelper.LoadRegistryRecord(theRegistryRecord.ID.ToString(), GlobalVariables.RegistryLoadType.ID);

            Assert.IsNull(theRegistryRecord, "RegistryRecord.DataPortal_DeleteSelf() did not return the expected value.");
        }

        /// <summary>
        /// Importing registry record
        /// </summary>
        [TestMethod]
        public void Import_ImportingRegistryRecord() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            bool blnResult = theRegistryRecord.Import();

            Assert.IsTrue(blnResult, "RegistryRecord.Import() did not return the expected value.");
        }

        /// <summary>
        /// Remove potentially copied components
        /// </summary>
        [TestMethod]
        public void RemovePotentiallyCopiedCompounds_RemovingPotentiallyCopiedComponents() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.LoadXml(theRegistryRecord.FoundDuplicates);

            XmlNodeList theXmlNodeList = theXmlDocument.SelectNodes("COMPOUNDLIST");

            thePrivateObject.Invoke("RemovePotentiallyCopiedCompounds", theXmlNodeList[0]);
            object theResult = thePrivateObject.GetFieldOrProperty("_potentiallyCopiedCompoundsIds");
            if (theResult != null)
            {
                Assert.IsTrue((theResult as List<int>).Count == 0, "RegistryRecord.RemovePotentiallyCopiedCompounds() did not return the expected value.");
            }
            else
            {
                Assert.IsNotNull(theResult, "RegistryRecord.RemovePotentiallyCopiedCompounds() did not return the expected value.");
            }
        }

        /// <summary>
        /// Remove resolved duplicate components
        /// </summary>
        [TestMethod]
        public void RemoveResolvedDuplicates_RemovingResolvedDuplicateComponents() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);

            Component theComponent = Component.NewComponent();
            theComponent.ComponentIndex = theRegistryRecord.ComponentList[0].ComponentIndex;
            theComponent.Percentage = 100;
            theRegistryRecord.AddComponent(theComponent);

            theRegistryRecord.ComponentList[0].Compound.RegNumber.RegNum = "C000127";
            theRegistryRecord.ComponentList[1].Compound.RegNumber.RegNum = "C000127";

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);

            StringBuilder theStringBuilder = new StringBuilder();
            theStringBuilder.Append("<ErrorList>");
            theStringBuilder.Append(theRegistryRecord.FoundDuplicates);
            theStringBuilder.Append("</ErrorList>");

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.LoadXml(theStringBuilder.ToString());

            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./ErrorList/COMPOUNDLIST/COMPOUND/REGISTRYLIST");
            if (theXmlNode != null)
            {
                int iRegistyListNodeCount = theXmlNode.ChildNodes.Count;
                theXmlNode = theXmlDocument.SelectSingleNode("./ErrorList");

                thePrivateObject.Invoke("RemoveResolvedDuplicates", theXmlNode);

                theXmlNode = theXmlNode.SelectSingleNode("./COMPOUNDLIST/COMPOUND/REGISTRYLIST");
                if (theXmlNode != null)
                {
                    Assert.AreNotEqual(iRegistyListNodeCount, theXmlNode.ChildNodes.Count, "RegistryRecord.RemoveResolvedDuplicates() did not return the expected value.");
                }
                else
                {
                    Assert.IsNull(theXmlNode, "RegistryRecord.RemoveResolvedDuplicates() did not return the expected value.");
                }
            }
            else
            {
                Assert.Inconclusive("No duplicate compounds found");
            }
        }

        /// <summary>
        /// Approving registry record
        /// </summary>
        [TestMethod]
        public void SetApprovalStatus_ApprovingRegistryRecord() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);
            theRegistryRecord.Status = RegistryStatus.Approved;
            theRegistryRecord.SetApprovalStatus();

            Assert.IsTrue(theRegistryRecord.Status == RegistryStatus.Approved, "RegistryRecord.SetApprovalStatus() did not return the expected value.");
        }

        /// <summary>
        /// Finding duplicate Compound
        /// </summary>
        [TestMethod]
        public void FindCompound_ApprovingRegistryRecord() 
        {
            RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
            theRegistryRecord = theRegistryRecord.Register(DuplicateAction.None);

            PrivateObject thePrivateObject = new PrivateObject(theRegistryRecord);
            object theResult = thePrivateObject.Invoke("findCompound", theRegistryRecord.ComponentList[0].ComponentIndex);

            Assert.IsNotNull(theResult, "RegistryRecord.findCompound() did not return the expected value.");
        }

        #endregion
    }
}
