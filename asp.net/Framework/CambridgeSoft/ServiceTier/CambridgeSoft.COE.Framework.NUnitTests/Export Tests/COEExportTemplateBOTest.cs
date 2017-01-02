using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;

namespace CambridgeSoft.COE.Framework.COEExportService.UnitTests
{
    [TestFixture]
    public class COEExportTemplateBOTest : LoginBase
    {
        #region Variables
        private string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Export Tests\COEExportXML");

        #endregion

        #region Test Methods

        [Test]
        public void NewTest()
        {
            Csla.SmartDate currentDate = new Csla.SmartDate(DateTime.Now);
            ResultsCriteria rc = BuildResultsCriteriaFromXML("ResultsCriteria.xml");
            COEExportTemplateBO.New("testName", "testDesc", rc, true, currentDate, 3, 1, 1, "1");
        }

        [Test]
        public void SaveTest()
        {
            string actualName = string.Empty;
            string expectedName = string.Empty;

            COEExportTemplateBO cetb = CreateCOEExportTemplate();
            expectedName = cetb.Name;
            cetb = COEExportTemplateBO.Get(cetb.ID);
            COEExportTemplateBO.Delete(cetb.ID);
            Assert.AreEqual(expectedName, cetb.Name, "Failed to create Template.");
        }

        [Test]
        public void SaveOnlyMandatoryValuesTest()
        {
            string actualName = string.Empty;
            string expectedName = string.Empty;

            COEExportTemplateBO cetb = CreateCOEExportTemplateOnlyMandatory();
            expectedName = cetb.Name;
            cetb = COEExportTemplateBO.Get(cetb.ID);
            COEExportTemplateBO.Delete(cetb.ID);
            Assert.AreEqual(expectedName, cetb.Name, "Failed to create Template.");
        }

        [Test]
        public void UpdateTest()
        {
            string actualName = string.Empty;
            string expectedName = string.Empty;
            COEExportTemplateBO cetb = CreateCOEExportTemplate();
            expectedName = cetb.Name;
            cetb = UpdateCOEExportTemplate(cetb);
            cetb = COEExportTemplateBO.Get(cetb.ID);
            COEExportTemplateBO.Delete(cetb.ID);
            Assert.AreNotEqual(expectedName, cetb.Name, "Failed to create Template.");
        }

        [Test]
        public void GetTest()
        {
            string name = string.Empty;
            COEExportTemplateBO cetb = CreateCOEExportTemplate();

            name = cetb.Name;
            cetb = COEExportTemplateBO.Get(cetb.ID);
            COEExportTemplateBO.Delete(cetb.ID);
            Assert.AreEqual(name, cetb.Name, "Failed to get template");
        }

        [Test]
        public void DeleteTest()
        {
            int actualId = int.MinValue;
            int expectedId = int.MinValue;

            COEExportTemplateBO cetb = CreateCOEExportTemplate();
            expectedId = cetb.ID;

            COEExportTemplateBO.Delete(cetb.ID);
            cetb = COEExportTemplateBO.Get(cetb.ID);
            actualId = cetb.ID;

            Assert.AreNotEqual(expectedId, actualId, "Failed to delete template");
        }

        [Test]
        public void DeleteByNameTest()
        {
            int actualId = int.MinValue;
            int expectedId = int.MinValue;

            COEExportTemplateBO cetb = CreateCOEExportTemplate();
            expectedId = cetb.ID;

            COEExportTemplateBO.Delete(cetb.Name);
            cetb = COEExportTemplateBO.Get(cetb.ID);
            actualId = cetb.ID;

            Assert.AreNotEqual(expectedId, actualId, "Failed to delete template");
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// returns results criteria
        /// </summary>
        /// <param name="resultsCriteriaXML">xml document name</param>
        /// <returns>results criteria</returns>
        public ResultsCriteria BuildResultsCriteriaFromXML(string resultsCriteriaXML)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + resultsCriteriaXML);
            ResultsCriteria resultsCriteria = new ResultsCriteria();
            resultsCriteria.GetFromXML(doc);
            return resultsCriteria;
        }

        /// <summary>
        /// Generates Random number which will be added to Name of the template
        /// </summary>
        /// <returns>random number</returns>
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

        /// <summary>
        /// Creates template
        /// </summary>
        /// <returns></returns>
        public COEExportTemplateBO CreateCOEExportTemplate()
        {
            COEExportTemplateBO cetb = new COEExportTemplateBO();
            cetb.DatabaseName = "COEDB";
            cetb.Name = "Test new" + GenerateRandomNumber();
            cetb.Description = "Test Description ";
            cetb.ResultCriteria = BuildResultsCriteriaFromXML("ResultsCriteria.xml");
            cetb.IsPublic = false;
            cetb.UserName = string.Empty;
            cetb.DataViewId = 5;
            cetb.WebFormId = 5;
            cetb.WinFormId = 5;

            cetb = cetb.Save();
            return cetb;
        }

        /// <summary>
        /// Creates template
        /// </summary>
        /// <returns></returns>
        public COEExportTemplateBO CreateCOEExportTemplateOnlyMandatory()
        {
            COEExportTemplateBO cetb = new COEExportTemplateBO();
            cetb.DatabaseName = "COEDB";
            cetb.Name = "Test new" + GenerateRandomNumber();
            cetb.ResultCriteria = BuildResultsCriteriaFromXML("ResultsCriteria.xml");
            cetb.IsPublic = false;
            cetb.DataViewId = 5;
            cetb = cetb.Save();
            return cetb;
        }

        /// <summary>
        /// Updates template
        /// </summary>
        /// <returns></returns>
        public COEExportTemplateBO UpdateCOEExportTemplate(COEExportTemplateBO cetb)
        {
            COEExportTemplateBO Cetb = cetb;
            Cetb.DatabaseName = "COEDB";

            Cetb.Name = "TestUpdate" + GenerateRandomNumber();
            Cetb.Description = "Test DescriptionUpdate";
            Cetb.IsPublic = false;
            Cetb.UserName = "TestUserIdUpdate";
            Cetb.DataViewId = 2;
            Cetb.WebFormId = 2;
            Cetb.WinFormId = 2;

            Cetb = Cetb.Update();
            return Cetb;
        }

        #endregion
    }
}
