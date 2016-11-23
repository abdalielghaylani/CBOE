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
using CambridgeSoft.COE.Registration.Services.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// RegSvcUtilitiesTest class contains unit test methods for RegSvcUtilities
    /// </summary>
    [TestClass]
    public class RegSvcUtilitiesTest
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

        #region RegSvcUtilities Test Methods

        /// <summary>
        /// Loading File version
        /// </summary>
        [TestMethod]
        public void GetFileVersion_LoadingFileVersion()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                string strFileVersion = RegSvcUtilities.GetFileVersion();

                Assert.IsTrue(!string.IsNullOrEmpty(strFileVersion), "RegSvcUtilities.GetFileVersion() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Refromatting XML to recognized by Oracle
        /// </summary>
        [TestMethod]
        public void ReformatXMLDate_ReformattingXMLToBeRecognizedByOracle()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                DataSet theNewDataSet = new DataSet("New_DataSet");
                DataTable dtNewTable = new DataTable("New_DataTable");

                DataColumn dcNewColumn = new DataColumn("REG_NUMBER", typeof(System.String));
                dtNewTable.Columns.Add(dcNewColumn);
                dcNewColumn = new DataColumn("CREATION_DATE", typeof(DateTime));
                dtNewTable.Columns.Add(dcNewColumn);
                dcNewColumn = new DataColumn("ENTRY_DATE", typeof(DateTime));
                dtNewTable.Columns.Add(dcNewColumn);
                dcNewColumn = new DataColumn("LAST_MOD_DATE", typeof(DateTime));
                dtNewTable.Columns.Add(dcNewColumn);

                DataRow newRow = dtNewTable.NewRow();
                newRow["REG_NUMBER"] = GlobalVariables.REGNUM1;
                newRow["CREATION_DATE"] = DateTime.Now;
                newRow["ENTRY_DATE"] = DateTime.Now;
                newRow["LAST_MOD_DATE"] = DateTime.Now;
                dtNewTable.Rows.Add(newRow);

                theNewDataSet.Tables.Add(dtNewTable);

                string strDataSetXML = theNewDataSet.GetXml();

                string strReformattedXML = RegSvcUtilities.ReformatXMLDate(strDataSetXML);

                Assert.AreNotEqual(strDataSetXML, strReformattedXML, "RegSvcUtilities.ReformatXMLDate(theNewDataSet.GetXml()) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Converting Structure to CDX
        /// </summary>
        [TestMethod]
        public void ConvertStructureFormatToCdx_ConvertingStructureToCDX()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                string strStructureValue = theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value;

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("ConvertStructureFormatToCdx", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure });

                Assert.AreNotEqual(strStructureValue, theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value, "RegSvcUtilities.ConvertStructureFormatToCdx(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading Property information from Property collection usign Property name
        /// </summary>
        [TestMethod]
        public void GetPropertyValue_LoadingPropertyInformationFromProeprtyCollection()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                theRegistryRecord.PropertyList[0].Value = "This is Reg Comment";

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("GetPropertyValue", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.PropertyList, theRegistryRecord.PropertyList[0].Name });

                Assert.AreEqual("This is Reg Comment", Convert.ToString(theResult), "RegSvcUtilities.GetPropertyValue(theRegistryRecord.PropertyList, theRegistryRecord.PropertyList[0].Name) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading Identifier Value From Identifier List Using Identifier Name
        /// </summary>
        [TestMethod]
        public void GetIdentifierValue_LoadingIdentifierValueFromIdentifierListUsingIdentifierName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

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
                }

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("GetIdentifierValue", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.IdentifierList, theRegistryRecord.IdentifierList[0].Name });

                Assert.IsTrue(!string.IsNullOrEmpty(Convert.ToString(theResult)), "RegSvcUtilities.GetIdentifierValue(theRegistryRecord.IdentifierList, theRegistryRecord.IdentifierList[0].ID) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading Identifier Value From Identifier List Using Identifier empty name
        /// </summary>
        [TestMethod]
        public void GetIdentifierValue_LoadingIdentifierValueFromIdentifierListUsingIdentifierEmptyName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.A, true, true);
                if (idList.Count > 0)
                {
                    Identifier theIdentifier = Identifier.NewIdentifier();
                    theIdentifier.Active = true;
                    theIdentifier.Description = "Test case Identifier";
                    theIdentifier.InputText = "Test case Identifier";
                    theIdentifier.Name = string.Empty;
                    theIdentifier.IdentifierID = idList[0].IdentifierID;
                    theRegistryRecord.AddIdentifier(theIdentifier);
                }

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("GetIdentifierValue", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.IdentifierList, theRegistryRecord.IdentifierList[0].Name });

                Assert.IsTrue(!string.IsNullOrEmpty(theRegistryRecord.IdentifierList[0].Name), "RegSvcUtilities.GetIdentifierValue(theRegistryRecord.IdentifierList, theRegistryRecord.IdentifierList[0].ID) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Adding Fragment into the Registry record
        /// </summary>
        [TestMethod]
        public void AddFragment_AddingFragmentInToRegistryRecord()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevBatchComponentFrgmentCount = theRegistryRecord.ComponentList[0].Compound.FragmentList.Count;

                FragmentList theFragmentList = FragmentList.GetFragmentList();
                Fragment theFragment = Fragment.NewFragment(true, true);

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddFragment", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, theFragment, 123 });

                Assert.AreNotEqual(iPrevBatchComponentFrgmentCount, theRegistryRecord.ComponentList[0].Compound.FragmentList.Count, "RegSvcUtilities.AddFragment(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, theFragment, 123) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Getting Invalid Component Index Exception
        /// </summary>
        [TestMethod]
        public void AddFragment_GettingInvalidComponentIndexException()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevBatchComponentFrgmentCount = theRegistryRecord.ComponentList[0].Compound.FragmentList.Count;

                FragmentList theFragmentList = FragmentList.GetFragmentList();
                Fragment theFragment = Fragment.NewFragment(true, true);

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddFragment", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, 15, theFragment, 123 });

                Assert.AreNotEqual(iPrevBatchComponentFrgmentCount, theRegistryRecord.ComponentList[0].Compound.FragmentList.Count, "RegSvcUtilities.AddFragment(theRegistryRecord, 15, theFragment, 123) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Getting Equivalent must be greater than zero Exception
        /// </summary>
        [TestMethod]
        public void AddFragment_GettingEquivalentZeroException()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevBatchComponentFrgmentCount = theRegistryRecord.ComponentList[0].Compound.FragmentList.Count;

                FragmentList theFragmentList = FragmentList.GetFragmentList();
                Fragment theFragment = Fragment.NewFragment(true, true);

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddFragment", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, theFragment, 0 });

                Assert.AreNotEqual(iPrevBatchComponentFrgmentCount, theRegistryRecord.ComponentList[0].Compound.FragmentList.Count, "RegSvcUtilities.AddFragment(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, theFragment, 0) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Getting Null fragment Exception
        /// </summary>
        [TestMethod]
        public void AddFragment_GettingNullFragmentException()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevBatchComponentFrgmentCount = theRegistryRecord.ComponentList[0].Compound.FragmentList.Count;

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddFragment", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, null, 123 });

                Assert.AreNotEqual(iPrevBatchComponentFrgmentCount, theRegistryRecord.ComponentList[0].Compound.FragmentList.Count, "RegSvcUtilities.AddFragment(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, null, 123) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding New Project  by name To Registry Record Project List
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToRegistryRecordProjectListByName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevProjectListCount = theRegistryRecord.ProjectList.Count;

                string strProjectName = string.Empty;
                ProjectList theProjectList = ProjectList.GetProjectList();
                if (theProjectList.Count > 1) { strProjectName = theProjectList[1].Name; }
                else { strProjectName = theProjectList[0].Name; }

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddProject", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(ProjectList), typeof(string), typeof(ProjectList.ProjectTypeEnum) }, null);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.ProjectList, strProjectName, ProjectList.ProjectTypeEnum.A });

                Assert.AreNotEqual(iPrevProjectListCount, theRegistryRecord.ProjectList.Count, "RegSvcUtilities.AddProject(theRegistryRecord.ProjectList, strProjectName, ProjectList.ProjectTypeEnum.A) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding New invalid Project by name To Registry Record Project List
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewInvalidProjectToRegistryRecordProjectListByName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevProjectListCount = theRegistryRecord.ProjectList.Count;

                string strProjectName = "Invalid Project name";

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddProject", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(ProjectList), typeof(string), typeof(ProjectList.ProjectTypeEnum) }, null);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.ProjectList, strProjectName, ProjectList.ProjectTypeEnum.A });

                Assert.AreNotEqual(iPrevProjectListCount, theRegistryRecord.ProjectList.Count, "RegSvcUtilities.AddProject(theRegistryRecord.ProjectList, strProjectName, ProjectList.ProjectTypeEnum.A) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding New Project  by ID To Registry Record Project List
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewProjectToRegistryRecordProjectListByID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevProjectListCount = theRegistryRecord.ProjectList.Count;

                int iProjectID = 0;
                ProjectList theProjectList = ProjectList.GetProjectList();
                if (theProjectList.Count > 1) { iProjectID = theProjectList[1].ProjectID; }
                else { iProjectID = theProjectList[0].ProjectID; }

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddProject", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(ProjectList), typeof(int), typeof(ProjectList.ProjectTypeEnum) }, null);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.ProjectList, iProjectID, ProjectList.ProjectTypeEnum.A });

                Assert.AreNotEqual(iPrevProjectListCount, theRegistryRecord.ProjectList.Count, "RegSvcUtilities.AddProject(theRegistryRecord.ProjectList, strProjectName, ProjectList.ProjectTypeEnum.A) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding New invalid Project by ID To Registry Record Project List
        /// </summary>
        [TestMethod]
        public void AddProject_AddingNewInvalidProjectToRegistryRecordProjectListByID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevProjectListCount = theRegistryRecord.ProjectList.Count;

                int iProjectID = 0;

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AddProject", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(ProjectList), typeof(int), typeof(ProjectList.ProjectTypeEnum) }, null);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.ProjectList, iProjectID, ProjectList.ProjectTypeEnum.A });

                Assert.AreNotEqual(iPrevProjectListCount, theRegistryRecord.ProjectList.Count, "RegSvcUtilities.AddProject(theRegistryRecord.ProjectList, strProjectName, ProjectList.ProjectTypeEnum.A) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding Structure by Structure ID
        /// </summary>
        [TestMethod]
        public void AssignDefaultStructureById_AddingStrcutureByStructureID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value = string.Empty;

                StringBuilder cmdString = new StringBuilder();
                cmdString.Append("SELECT STRUCTUREID, STRUCTUREFORMAT FROM REGDB.VW_STRUCTURE");

                DataTable dtStructureInfo = DALHelper.ExecuteQuery(cmdString.ToString());

                if (dtStructureInfo != null && dtStructureInfo.Rows.Count > 0)
                {
                    Type type = typeof(RegSvcUtilities);
                    MethodInfo theMethodInfo = type.GetMethod("AssignDefaultStructureById", BindingFlags.NonPublic | BindingFlags.Static);
                    theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, Convert.ToInt32(Convert.ToString(dtStructureInfo.Rows[0]["STRUCTUREID"])) });

                    Assert.IsTrue(!string.IsNullOrEmpty(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value), "RegSvcUtilities.AssignDefaultStructureById(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, Convert.ToInt32(Convert.ToString(dtStructureInfo.Rows[0]['STRUCTUREID']))) did not return the expected value.");
                }
                else
                {
                    Assert.IsTrue((dtStructureInfo == null && dtStructureInfo.Rows.Count == 0), "Structure information does not exist.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding Structure by Structure ID with Invalid Component Index
        /// </summary>
        [TestMethod]
        public void AssignDefaultStructureById_AddingStrcutureByStructureIDWithInvalidComponentIndex()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value = string.Empty;

                StringBuilder cmdString = new StringBuilder();
                cmdString.Append("SELECT STRUCTUREID, STRUCTUREFORMAT FROM REGDB.VW_STRUCTURE");

                DataTable dtStructureInfo = DALHelper.ExecuteQuery(cmdString.ToString());

                if (dtStructureInfo != null && dtStructureInfo.Rows.Count > 0)
                {
                    Type type = typeof(RegSvcUtilities);
                    MethodInfo theMethodInfo = type.GetMethod("AssignDefaultStructureById", BindingFlags.NonPublic | BindingFlags.Static);
                    theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, 15, Convert.ToInt32(Convert.ToString(dtStructureInfo.Rows[0]["STRUCTUREID"])) });

                    Assert.IsTrue(!string.IsNullOrEmpty(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value), "RegSvcUtilities.AssignDefaultStructureById(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, Convert.ToInt32(Convert.ToString(dtStructureInfo.Rows[0]['STRUCTUREID']))) did not return the expected value.");
                }
                else
                {
                    Assert.IsTrue((dtStructureInfo == null && dtStructureInfo.Rows.Count == 0), "Structure information does not exist.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding Structure by Negative Structure ID
        /// </summary>
        [TestMethod]
        public void AssignDefaultStructureById_AddingStrcutureByNegativeStructureID()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value = string.Empty;

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("AssignDefaultStructureById", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, -1 });

                Assert.IsTrue(theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value == DrawingType.Unknown.ToString(), "RegSvcUtilities.AssignDefaultStructureById(theRegistryRecord, theRegistryRecord.ComponentList[0].ComponentIndex, 1111) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checks whether Components are selected automatically for mixture duplicate checking
        /// </summary>
        [TestMethod]
        public void CanAutoSelectComponentForDupChk_CheckingComponentForMixtureDuplicate()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                SequenceList theSequenceList = SequenceList.GetSequenceList(1);
                bool blnResult = RegSvcUtilities.CanAutoSelectComponentForDupChk(theSequenceList[0]);

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanAutoSelectComponentForDupChk(theSequenceList[0]) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checking Whether User Can Create Duplicate Component
        /// </summary>
        [TestMethod]
        public void CanUserCreateDuplicateComponent_CheckingWhetherUserCanCreateDuplicateComponent()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                bool blnResult = RegSvcUtilities.CanUserCreateDuplicateComponent();

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanUserCreateDuplicateComponent() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checking Whether User Can Create Duplicate Registry
        /// </summary>
        [TestMethod]
        public void CanUserCreateDuplicateRegistry_CheckingWhetherUserCanCreateDuplicateRegistry()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                bool blnResult = RegSvcUtilities.CanUserCreateDuplicateRegistry();

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanUserCreateDuplicateRegistry() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checking Whether User Can edit Temparary registration
        /// </summary>
        [TestMethod]
        public void CanUserEditRegistration_CheckingWhetherUserCanEditTempararyRegistration()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                bool blnResult = RegSvcUtilities.CanUserEditRegistration(theRegistryRecord.PersonCreated, true);

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanUserEditRegistration(theRegistryRecord.PersonCreated, true) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checking Whether User Can edit Permanant registration
        /// </summary>
        [TestMethod]
        public void CanUserEditRegistration_CheckingWhetherUserCanEditPermanantRegistration()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                bool blnResult = RegSvcUtilities.CanUserEditRegistration(theRegistryRecord.PersonCreated, false);

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanUserEditRegistration(theRegistryRecord.PersonCreated, false) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checks whether current user can propogate component edits
        /// </summary>
        [TestMethod]
        public void CanUserPropogateComponentEdits_CheckingWhetherUserCanPropogateComponentEdits()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                bool blnResult = RegSvcUtilities.CanUserPropogateComponentEdits();

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanUserPropogateComponentEdits() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Checks whether current user can propogate structure edits
        /// </summary>
        [TestMethod]
        public void CanUserPropogateStructureEdits_CheckingWhetherUserCanPropogateSturctureEdits()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                bool blnResult = RegSvcUtilities.CanUserPropogateStructureEdits();

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.CanUserPropogateStructureEdits() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Creating mixture from registrerd components
        /// </summary>
        [TestMethod]
        public void CreateMixtureFromRegisteredComponents_CreatingMixtureFromRegistredComponents()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.LoadRegistryRecord(GlobalVariables.REGNUM2, GlobalVariables.RegistryLoadType.REGNUM);
                if (theRegistryRecord == null)
                {
                    theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                    theRegistryRecord = theRegistryRecord.Register(DuplicateAction.Duplicate);
                }

                string strRegNumList = string.Format("{0}|{1}", theRegistryRecord.ComponentList[0].Compound.RegNumber.RegNum, theRegistryRecord.ComponentList[0].Compound.RegNumber.RegNum);
                string strCompoundPercentageList = "50|50";

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("CreateMixtureFromRegisteredComponents", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord, strRegNumList, strCompoundPercentageList, "|" });

                Assert.IsTrue(theRegistryRecord.ComponentList.Count == 2, "RegSvcUtilities.CreateMixtureFromRegisteredComponents(theRegistryRecord, strRegNumList, strCompoundPercentageList, '|') did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Adding New Identifier into the Registry record by Identifier object
        /// </summary>
        [TestMethod]
        public void CreateNewIdentifier_AddingNewIdentifierIntoRegistryRecordByIdentifier()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevIdentifierListCount = theRegistryRecord.IdentifierList.Count;

                IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R, true, true);
                if (idList.Count > 0)
                {
                    Identifier theIdentifier = Identifier.NewIdentifier();
                    theIdentifier.Active = true;
                    theIdentifier.Description = "Test case Identifier";
                    theIdentifier.InputText = "Test case Identifier";
                    theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                    theIdentifier.IdentifierID = idList[0].IdentifierID;

                    Type type = typeof(RegSvcUtilities);
                    MethodInfo theMethodInfo = type.GetMethod("CreateNewIdentifier", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(IdentifierList), typeof(Identifier), typeof(IdentifierTypeEnum) }, null);
                    theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord.IdentifierList, theIdentifier, IdentifierTypeEnum.R });

                    Assert.AreEqual(iPrevIdentifierListCount + 1, theRegistryRecord.IdentifierList.Count, "RegSvcUtilities.CreateNewIdentifier(theRegistryRecord.IdentifierList, theIdentifier, IdentifierTypeEnum.R) did not add new identifier into the registry record.");
                }
                else
                {
                    Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Adding New Identifier into the Registry record using Identifier ID and Value
        /// </summary>
        [TestMethod]
        public void CreateNewIdentifier_AddingNewIdentifierIntoRegistryRecordByIdentifierIDAndValue()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevIdentifierListCount = theRegistryRecord.IdentifierList.Count;

                IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R, true, true);
                if (idList.Count > 0)
                {
                    Type type = typeof(RegSvcUtilities);
                    MethodInfo theMethodInfo = type.GetMethod("CreateNewIdentifier", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(int), typeof(string), typeof(IdentifierList), typeof(IdentifierTypeEnum) }, null);
                    theMethodInfo.Invoke(theRegistryRecord, new object[] { idList[0].IdentifierID, "Test case Identifier", theRegistryRecord.IdentifierList, IdentifierTypeEnum.R });

                    Assert.AreEqual(iPrevIdentifierListCount + 1, theRegistryRecord.IdentifierList.Count, "RegSvcUtilities.CreateNewIdentifier(idList[0].IdentifierID, 'Test case Identifier', theRegistryRecord.IdentifierList, IdentifierTypeEnum.R) did not add new identifier into the registry record.");
                }
                else
                {
                    Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Adding New Identifier into the Registry record using Identifier ID and Name
        /// </summary>
        [TestMethod]
        public void CreateNewIdentifier_AddingNewIdentifierIntoRegistryRecordByIdentifierIDAndName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                int iPrevIdentifierListCount = theRegistryRecord.IdentifierList.Count;

                IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R, true, true);
                if (idList.Count > 0)
                {
                    Type type = typeof(RegSvcUtilities);
                    MethodInfo theMethodInfo = type.GetMethod("CreateNewIdentifier", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(IdentifierList), typeof(IdentifierTypeEnum) }, null);
                    theMethodInfo.Invoke(theRegistryRecord, new object[] { idList[0].Name, "Test case Identifier", theRegistryRecord.IdentifierList, IdentifierTypeEnum.R });

                    Assert.AreEqual(iPrevIdentifierListCount + 1, theRegistryRecord.IdentifierList.Count, "RegSvcUtilities.CreateNewIdentifier(idList[0].Name, 'Test case Identifier', theRegistryRecord.IdentifierList, IdentifierTypeEnum.R) did not add new identifier into the registry record.");
                }
                else
                {
                    Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Getting Duplicate components from registry record using Component Identifier with chemical content
        /// </summary>
        [TestMethod]
        public void FindDuplicates_LoadingDuplicateComponentsAccordingtoRegistryRecord()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("FindDuplicates", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord });

                Assert.IsNotNull(theResult, "RegSvcUtilities.FindDuplicates(theRegistryRecord) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Getting Duplicate components from registry record using Component Identifier with Non chemical content
        /// </summary>
        [TestMethod]
        public void FindDuplicates_LoadingDuplicateComponentsAccordingtoRegistryRecordWithComponentIdentifierAndNonChemicalContent()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");
            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Identifier");
                theKeyValuePair.Add("Value", "TEST_CASE_IDENTIFIER");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.DrawingType = DrawingType.NonChemicalContent;

                IdentifierList idList = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C, true, true);
                if (idList.Count > 0)
                {
                    Identifier theIdentifier = Identifier.NewIdentifier();
                    theIdentifier.Active = true;
                    theIdentifier.Description = "Test case Identifier";
                    theIdentifier.InputText = "Test case Identifier";
                    theIdentifier.Name = "TEST_CASE_IDENTIFIER";
                    theIdentifier.IdentifierID = idList[0].IdentifierID;
                    theRegistryRecord.ComponentList[0].Compound.AddIdentifier(theIdentifier);
                }
                else
                {
                    Assert.AreEqual(idList.Count > 0, "Identifier list is empty.");
                }

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("FindDuplicates", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord });

                Assert.IsNotNull(theResult, "RegSvcUtilities.FindDuplicates(theRegistryRecord) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Getting Duplicate components from registry record using Component Property with Non chemical content
        /// </summary>
        [TestMethod]
        public void FindDuplicates_LoadingDuplicateComponentsAccordingtoRegistryRecordWithComponentPropertyAndNonChemicalContent()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                Dictionary<string, string> theKeyValuePair = new Dictionary<string, string>();
                theKeyValuePair.Add("Level", "Component");
                theKeyValuePair.Add("Type", "Property");
                theKeyValuePair.Add("Value", "CMP_COMMENTS");
                DALHelper.SetRegistrationSettingsONOrOFF(string.Empty, string.Empty, "ENHANCED_DUPLICATE_SCAN", theKeyValuePair);

                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.DrawingType = DrawingType.NonChemicalContent;
                theRegistryRecord.ComponentList[0].Compound.PropertyList["CMP_COMMENTS"].Value = "125";

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("FindDuplicates", BindingFlags.NonPublic | BindingFlags.Static);
                object theResult = theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord });

                Assert.IsNotNull(theResult, "RegSvcUtilities.FindDuplicates(theRegistryRecord) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Returns true if the unregistered components allowed in mixtures workflow was enabled by configuration
        /// </summary>
        [TestMethod]
        public void GetAllowUnregisteredComponents_CheckingWhetherUnRegistrerdComponentsAllowedInMixture()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                bool blnResult = RegSvcUtilities.GetAllowUnregisteredComponents();

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.GetAllowUnregisteredComponents() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Get the  no of components so that we can differentiate b/w a mixture and single component 
        /// </summary>
        [TestMethod]
        public void GetComponentCount_LoadingTotalNumberOfComponents()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                int iResult = RegSvcUtilities.GetComponentCount();

                Assert.IsTrue(iResult > 0, "RegSvcUtilities.GetComponentCount() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Returns true if EnableBatchPrefix is enabled 
        /// </summary>
        [TestMethod]
        public void GetEnableBatchPrefix_CheckingWhetherBatchPrefixIsAllowed()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                bool blnResult = RegSvcUtilities.GetEnableBatchPrefix();

                Assert.IsTrue(blnResult || !blnResult, "RegSvcUtilities.GetEnableBatchPrefix() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Returns true the default batch prefix to use for non-UI loading when EnableBatchPrefix is enabled 
        /// </summary>
        [TestMethod]
        public void GetDefaultBatchPrefix_LoadingDefaultBatchPrefixWhenBatchPrefixIsEnabled()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                int iResult = RegSvcUtilities.GetDefaultBatchPrefix();

                Assert.IsTrue(iResult > 0, "RegSvcUtilities.GetDefaultBatchPrefix() did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading System settings using Setion name
        /// </summary>
        [TestMethod]
        public void GetSystemSettings_LoadingSystemSettingdUsingSectionName()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                NamedElementCollection<AppSetting> theResult = RegSvcUtilities.GetSystemSettings("REGADMIN");

                Assert.IsTrue(theResult.Count > 0, "RegSvcUtilities.GetSystemSettings('REGADMIN') did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Loading System settings using Setion name and Key
        /// </summary>
        [TestMethod]
        public void GetSystemSettings_LoadingSystemSettingdUsingSectionNameAndKey()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                string strResult = RegSvcUtilities.GetSystemSettings("REGADMIN", "TableNameList");

                Assert.IsTrue(!string.IsNullOrEmpty(strResult), "RegSvcUtilities.GetSystemSettings('REGADMIN', 'TableNameList') did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Storing exception messages
        /// </summary>
        [TestMethod]
        public void LogError_LoggingException()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                try
                {
                    int i = 0;
                    int j = 1;
                    int k = j / i;
                }
                catch (Exception ex)
                {
                    RegSvcUtilities.LogError("Unit test error message :- ", 1, ex);
                    Assert.Inconclusive("RegSvcUtilities.LogError('Unit test error message :- ', 1, ex) could not validate the result");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Storing exception messages with Entry type as Error
        /// </summary>
        [TestMethod]
        public void LogOther_LoggingExceptionWithEntryTypeAsError()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                RegSvcUtilities.LogOther("Unit test error message :- ", 1, System.Diagnostics.EventLogEntryType.Error);
                Assert.Inconclusive("RegSvcUtilities.LogOther('Unit test error message :- ', 1, System.Diagnostics.EventLogEntryType.Error) could not validate the result");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Storing exception messages with Entry type as FailureAudit
        /// </summary>
        [TestMethod]
        public void LogOther_LoggingExceptionWithEntryTypeAsFailureAudit()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                RegSvcUtilities.LogOther("Unit test error message :- ", 1, System.Diagnostics.EventLogEntryType.FailureAudit);
                Assert.Inconclusive("RegSvcUtilities.LogOther('Unit test error message :- ', 1, System.Diagnostics.EventLogEntryType.FailureAudit) could not validate the result");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Storing exception messages with Entry type as Information
        /// </summary>
        [TestMethod]
        public void LogOther_LoggingExceptionWithEntryTypeAsInformation()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                RegSvcUtilities.LogOther("Unit test error message :- ", 1, System.Diagnostics.EventLogEntryType.Information);
                Assert.Inconclusive("RegSvcUtilities.LogOther('Unit test error message :- ', 1, System.Diagnostics.EventLogEntryType.Information) could not validate the result");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Storing exception messages with Entry type as SuccessAudit
        /// </summary>
        [TestMethod]
        public void LogOther_LoggingExceptionWithEntryTypeAsSuccessAudit()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                RegSvcUtilities.LogOther("Unit test error message :- ", 1, System.Diagnostics.EventLogEntryType.SuccessAudit);
                Assert.Inconclusive("RegSvcUtilities.LogOther('Unit test error message :- ', 1, System.Diagnostics.EventLogEntryType.SuccessAudit) could not validate the result");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Storing exception messages with Entry type as Warning
        /// </summary>
        [TestMethod]
        public void LogOther_LoggingExceptionWithEntryTypeAsWarning()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                RegSvcUtilities.LogOther("Unit test error message :- ", 1, System.Diagnostics.EventLogEntryType.Warning);
                Assert.Inconclusive("RegSvcUtilities.LogOther('Unit test error message :- ', 1, System.Diagnostics.EventLogEntryType.Warning) could not validate the result");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Updating Fragment from its Batch Component Fragments
        /// </summary>
        [TestMethod]
        public void UpdateFragments_UpdatingFragmentFromBatchComponentFragment()
        {
            DALHelper.SetRegistrationSettingsONOrOFF("EnableUseBatchPrefixes", "False");

            try
            {
                RegistryRecord theRegistryRecord = RegistryHelper.CreateRegistryObject(GlobalVariables.REGNUM2);

                theRegistryRecord.ComponentList[0].Compound.FragmentList.Clear();

                Type type = typeof(RegSvcUtilities);
                MethodInfo theMethodInfo = type.GetMethod("UpdateFragments", BindingFlags.NonPublic | BindingFlags.Static);
                theMethodInfo.Invoke(theRegistryRecord, new object[] { theRegistryRecord });

                Assert.IsNotNull(theRegistryRecord.ComponentList[0].Compound.FragmentList.Count > 0, "RegSvcUtilities.UpdateFragments(theRegistryRecord) did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
