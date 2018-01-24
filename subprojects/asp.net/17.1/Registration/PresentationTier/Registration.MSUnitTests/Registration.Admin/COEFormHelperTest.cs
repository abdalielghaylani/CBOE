using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CambridgeSoft.COE.Framework.Common.IniParser;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;
using RegistrationAdmin.Services.MSUnitTests.Helpers;
using CambridgeSoft.COE.RegistrationAdmin.Access;
using CambridgeSoft.COE.Framework.COEFormService;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for COEFormHelperTest and is intended
    ///to contain all COEFormHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class COEFormHelperTest
    {
        XmlDocument configurationRegistryRecordXmlDoc;
        XmlDocument registrationAppSettingsXmlDoc;
        XmlDocument customTableListXmlDoc;
        XmlDocument coeFormGroupXmlDoc;

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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void COEFormHelperTestInitialize()
        {
            configurationRegistryRecordXmlDoc = XMLDataLoader.LoadXmlDocument("ConfigurationRegistryRecord.xml");
            if (configurationRegistryRecordXmlDoc == null)
            {
                Assert.Fail("ConfigurationRegistryRecord.xml document can not be loaded");
            }

            registrationAppSettingsXmlDoc = XMLDataLoader.LoadXmlDocument("RegAppSettings.xml");
            if (registrationAppSettingsXmlDoc == null)
            {
                Assert.Fail("RegAppSettings.xml document can not be loaded");
            }

            customTableListXmlDoc = XMLDataLoader.LoadXmlDocument("CustomTableList.xml");
            if (customTableListXmlDoc == null)
            {
                Assert.Fail("CustomTableList.xml document can not be loaded");
            }

            coeFormGroupXmlDoc = XMLDataLoader.LoadXmlDocument("FormGroup.xml");
            if (coeFormGroupXmlDoc == null)
            {
                Assert.Fail("FormGroup.xml document can not be loaded");
            }
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void COEFormHelperTestCleanup()
        {
        }

        #endregion

        /// <summary>
        ///A test for NewConfigurationRegistryRecord with default constructor
        ///</summary>
        private ConfigurationRegistryRecord CreateNewConfigurationRegistryRecord()
        {
            ConfigurationRegistryRecord target;
            //create RegAdminOracleDAL object to avoid exceptions while running default constructor
            RegAdminOracleDAL regDal = null;
            Helpers.Helpers.GetRegDal(ref regDal, "");
            target = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Assert.IsNotNull(target, string.Format("{0}.NewConfigurationRegistryRecord() failed to create object", Helpers.Helpers.GetFullTypeNameMessage<ConfigurationRegistryRecord>(target)));
            return target;
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_SendToRegistrationFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SendToRegistrationForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            //int formId = (int)obj.GetFieldOrProperty("SENDTOREGISTRATIONFORMID");
            //COEFormBO coeFormBo = COEFormBO.Get(formId);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load SendToRegistrationForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_ComponentDuplicatesFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.ComponentDuplicatesForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load ComponentDuplicatesForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_DataLoaderFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.DataLoaderForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load DataLoaderForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_DeleteLogFromTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.DeleteLogFrom; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load DeleteLogFrom", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_ELNSearchPermFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.ELNSearchPermForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load ELNSearchPermForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_ELNSearchTempFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.ELNSearchTempForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load ELNSearchTempForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_RegistryDuplicatesFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.RegistryDuplicatesForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load RegistryDuplicatesForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_ReviewRegisterMixtureTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.ReviewRegisterMixture; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load ReviewRegisterMixture", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_SearchComponentToAddFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SearchComponentToAddForm; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load SearchComponentToAddForm", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_SearchComponentToAddFormRRTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SearchComponentToAddFormRR; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load SearchComponentToAddFormRR", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_SearchPermanentFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SearchPermanent; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load SearchPermanent", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_SearchTemporaryFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SearchTemporary; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load SearchTemporary", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_SubmitMixtureFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SubmitMixture; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load SubmitMixture", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void Load_ViewMixtureFormTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.ViewMixture; // TODO: Initialize to an appropriate value
            target.Load(formGroupType);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO coeFormBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(coeFormBo, string.Format("{0}.Load(formGroupType) failed to load ViewMixture", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for SaveFormGroup
        ///</summary>
        [TestMethod()]
        public void SaveFormGroupTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            string newFormGroup = string.Empty; // TODO: Initialize to an appropriate value
            COEFormHelper.COEFormGroups formGroupType = COEFormHelper.COEFormGroups.SubmitMixture;
            target.Load(formGroupType);
            XmlNode formGroupNode = coeFormGroupXmlDoc.ChildNodes[1].ChildNodes[0];
            newFormGroup = formGroupNode.OuterXml;
            target.SaveFormGroup(newFormGroup);
            PrivateObject obj = new PrivateObject(target);
            COEFormBO formBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.AreEqual<int>(4010, formBo.COEFormGroup.Id);
        }

        /// <summary>
        ///A test for UpdateRegistrationFormGroups
        ///</summary>
        [TestMethod()]
        public void UpdateRegistrationFormGroupsTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            target.UpdateRegistrationFormGroups();
            PrivateObject obj = new PrivateObject(target);
            COEFormBO formBo = (COEFormBO)obj.GetFieldOrProperty("_formGroupBO");
            Assert.IsNotNull(formBo);
        }

        /// <summary>
        ///A test for COEFormHelper Constructor
        ///</summary>
        [TestMethod()]
        public void COEFormHelperConstructorTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord);
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
        }

        /// <summary>
        ///A test for ApplyFormEditing
        ///</summary>
        [TestMethod()]
        public void ApplyFormEditingTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            FormGroup.Form mixtureForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form compoundForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form batchForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form batchComponentForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            target.ApplyFormEditing(mixtureForm, compoundForm, batchForm, batchComponentForm);
        }

        /// <summary>
        ///A test for ApplyFormEditingToELNSearchPerm
        ///</summary>
        [TestMethod()]
        public void ApplyFormEditingToELNSearchPermTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));
            FormGroup.Form elnSearchPermMixtureForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchPermCompoundForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchPermBatchForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchPermBatchComponentForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchPermanentListForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form enlSearchPermanentMixtureDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form enlSearchPermanentCompoundDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form enlSearchPermanentBatchDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form enlSearchPermanentBatchComponentDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            target.ApplyFormEditingToELNSearchPerm(elnSearchPermMixtureForm, elnSearchPermCompoundForm, elnSearchPermBatchForm, elnSearchPermBatchComponentForm, elnSearchPermanentListForm, enlSearchPermanentMixtureDetailForm, enlSearchPermanentCompoundDetailForm, enlSearchPermanentBatchDetailForm, enlSearchPermanentBatchComponentDetailForm);
        }

        /// <summary>
        ///A test for ApplyFormEditingToELNSearchTemp
        ///</summary>
        [TestMethod()]
        public void ApplyFormEditingToELNSearchTempTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));

            FormGroup.Form elnSearchTempBatchForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchTempCompoundForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchTempDetailsBaseForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchTempDetailsChildForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form elnSearchTempListForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            target.ApplyFormEditingToELNSearchTemp(elnSearchTempBatchForm, elnSearchTempCompoundForm, elnSearchTempDetailsBaseForm, elnSearchTempDetailsChildForm, elnSearchTempListForm);
        }

        /// <summary>
        ///A test for ApplyFormEditingToSearchPerm
        ///</summary>
        [TestMethod()]
        public void ApplyFormEditingToSearchPermTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));

            FormGroup.Form searchPermMixtureForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermCompoundForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermBatchForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermBatchComponentForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermanentListForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermanentMixtureDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermanentCompoundDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermanentBatchDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchPermanentBatchComponentDetailForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            target.ApplyFormEditingToSearchPerm(searchPermMixtureForm, searchPermCompoundForm, searchPermBatchForm, searchPermBatchComponentForm, searchPermanentListForm, searchPermanentMixtureDetailForm, searchPermanentCompoundDetailForm, searchPermanentBatchDetailForm, searchPermanentBatchComponentDetailForm);
        }

        /// <summary>
        ///A test for ApplyFormEditingToSearchTemp
        ///</summary>
        [TestMethod()]
        public void ApplyFormEditingToSearchTempTest()
        {
            ConfigurationRegistryRecord configRecord = CreateNewConfigurationRegistryRecord(); // TODO: Initialize to an appropriate value
            COEFormHelper target = new COEFormHelper(configRecord); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("{0}.COEFormHelper(configRecord) failed to create object", Helpers.Helpers.GetFullTypeNameMessage<COEFormHelper>(target)));

            FormGroup.Form searchTempBatchForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchTempCompoundForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchTempDetailsBaseForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchTempDetailsChildForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            FormGroup.Form searchTempListForm = new FormGroup.Form(); // TODO: Initialize to an appropriate value
            target.ApplyFormEditingToSearchTemp(searchTempBatchForm, searchTempCompoundForm, searchTempDetailsBaseForm, searchTempDetailsChildForm, searchTempListForm);
        }
    }
}
