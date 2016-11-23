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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.COEExportService.UnitTests
{
    [TestClass()]
    public class COEExportTemplateBOListTest : LoginBase
    {
        #region Variables

        private string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Export Tests\COEExportXML");
        private COEExportTemplateBOList exportTemplateList;
        private List<string> templateNameList;
        private int specificCount;
        private string privilegeduser = ConfigurationManager.AppSettings["LogonUserName"];
        private string underprivilegeduser = ConfigurationManager.AppSettings["underprivilegeduser"];

        #endregion

        #region Properties

        public List<string> TemplateNameList
        {
            get { return templateNameList; }
            set { templateNameList = value; }
        }

        public int SpecificCount
        {
            get { return specificCount; }
            set { specificCount = value; }
        }

        #endregion

        #region Constructor

        public COEExportTemplateBOListTest()
        {
            this.TemplateNameList = new List<string>();
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void GetAllTemplatesTest()
        {
            int actualCount = 0;
            int expectedCount = 0;


            try
            {
                expectedCount = this.CreateCOEExportTemplates(); // inserts multiple templates
                exportTemplateList = COEExportTemplateBOList.GetTemplates();
                actualCount = this.GetActualCount(exportTemplateList); // returns actual count
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetTemplatesTest()
        {
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = this.SpecificCount; // gives count where isPublic is true
                exportTemplateList = COEExportTemplateBOList.GetTemplates(true);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetTemplatesTest1()
        {
            int actualCount = 0;
            int expectedCount = 0;

            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = totalInsertions - this.SpecificCount; // gives count where isPublic is false
                exportTemplateList = COEExportTemplateBOList.GetTemplates(false);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByUserIdTest()
        {
            string userId = privilegeduser;
            int actualCount = 0;
            int expectedCount = 0;


            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = this.SpecificCount; // gives count where userId is CSSADMIN
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByUserId(userId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByUserIdTest1()
        {
            string userId = privilegeduser;
            int actualCount = 0;
            int expectedCount = 0;


            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = this.SpecificCount; // gives count where userId is cssadmin
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByUserId(userId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByUserIdTest2()
        {
            string userId = underprivilegeduser;
            int actualCount = 0;
            int expectedCount = 0;


            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = totalInsertions - this.SpecificCount; // gives count where userId is CSSUSER
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByUserId(userId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByWinFormIdTest()
        {
            int winFormId = 10;
            int actualCount = 0;
            int expectedCount = 0;


            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = this.SpecificCount; // gives counts where winFormId is 10
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByWinFormId(winFormId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByWinFormIdTest1()
        {
            int winFormId = 0;
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates                
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByWinFormId(winFormId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByWinFormIdTest2()
        {
            int winFormId = -5;
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates                
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByWinFormId(winFormId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByWebFormIdTest()
        {
            int webFormId = 10;
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = this.SpecificCount; // gives counts where webFormId is 10
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByWebFormId(webFormId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByWebFormIdTest1()
        {
            int webFormId = 0;
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates                
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByWebFormId(webFormId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByWebFormIdTest2()
        {
            int webFormId = -5;
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates                
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByWebFormId(webFormId);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByDataViewIdTest()
        {
            int dataViewId = 10;
            string userId = privilegeduser;
            bool isPublic = true;

            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = this.SpecificCount; // gives counts where dataView id is 10, isPublic is true, userId is CSSADMIN
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByDataViewId(dataViewId, userId, isPublic);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void GetUserTemplatesByDataViewIdTest1()
        {
            int dataViewId = 20;

            string userId = underprivilegeduser;//string.Empty;
            bool isPublic = false;
            int actualCount = 0;
            int expectedCount = 0;
            try
            {
                int totalInsertions = this.CreateCOEExportTemplates(); // inserts multiple templates
                expectedCount = totalInsertions - this.SpecificCount; // gives counts where dataView id is 20, isPublic is false, userId is empty
                // It is not possible to send empty userid (query will always return 0).
                exportTemplateList = COEExportTemplateBOList.GetUserTemplatesByDataViewId(dataViewId, userId, isPublic);
                actualCount = this.GetActualCount(exportTemplateList);
                this.DeleteCreatedTemplates(TemplateNameList);
                Assert.AreEqual(expectedCount, actualCount, "Did not return the expected value.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void NewList_Test()
        {
            Assert.IsNotNull(COEExportTemplateBOList.NewList());
            Assert.IsNotNull(COEExportTemplateBOList.NewList("COEDB"));
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
            int min = 100;
            int max = 100000;
            Random random = new Random();
            int randomNumber = random.Next(min, max);
            return randomNumber.ToString();

        }

        /// <summary>
        /// Creates template
        /// </summary>
        /// <returns></returns>
        public COEExportTemplateBO CreateCOEExportTemplate(int i)
        {
            COEExportTemplateBO cetb = new COEExportTemplateBO();
            cetb.DatabaseName = "COEDB";
            cetb.Name = "Test template" + GenerateRandomNumber();
            cetb.Description = "Test Description";
            cetb.ResultCriteria = BuildResultsCriteriaFromXML("ResultsCriteria.xml");
            if (i % 2 == 0)
            {
                cetb.IsPublic = true;
                cetb.DataViewId = 10;
                cetb.UserName = privilegeduser;
                this.SpecificCount += 1;
                cetb.WebFormId = 10;
                cetb.WinFormId = 10;
            }
            else
            {
                cetb.IsPublic = false;
                cetb.DataViewId = 20;
                cetb.UserName = underprivilegeduser;
                cetb.WebFormId = 20;
                cetb.WinFormId = 20;
            }

            this.TemplateNameList.Add(cetb.Name);
            cetb = cetb.Save();
            return cetb;
        }

        /// <summary>
        /// creates multiple insertions and returns count of insertions happened
        /// </summary>
        /// <returns>expected count</returns>
        public int CreateCOEExportTemplates()
        {
            int noOfInsertions = 5;
            int expectedCount = 0;
            for (int i = 0; i < noOfInsertions; i++)
            {
                COEExportTemplateBO templateObject = this.CreateCOEExportTemplate(i);
                expectedCount += 1;
            }
            return expectedCount;
        }

        /// <summary>
        /// returns actual count
        /// </summary>
        /// <param name="templateList">Created list of templates</param>
        /// <returns>actual count</returns>
        public int GetActualCount(COEExportTemplateBOList templateList)
        {
            int actualCount = 0;

            foreach (COEExportTemplateBO var in templateList)
            {
                foreach (string templateName in this.templateNameList)
                {
                    if (templateName == var.Name)
                        actualCount += 1;
                }
            }
            return actualCount;
        }

        /// <summary>
        /// deletes all the created templates
        /// </summary>
        /// <param name="templateNames">holds created template names as list</param>
        public void DeleteCreatedTemplates(List<string> templateNames)
        {
            foreach (string var in templateNames)
            {
                COEExportTemplateBO.Delete(var);
            }
        }

        #endregion

    }
}
