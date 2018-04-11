using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEDataViewService.UnitTests;

namespace CambridgeSoft.COE.Framework.UnitTests.DataView_Tests
{
    /// <summary>
    /// Summary description for FieldListBOTest
    /// </summary>
    [TestClass]
    public class FieldListBOTest
    {
        private string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\DataView Tests\COEDataViewTestXML");
        private string _databaseName = "COEDB";
        private string _databasePassword = "ORACLE";
        private string _userName = "cssadmin";
        public FieldListBOTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Test Initialization

        /// <summary>
        /// Use TestInitialize to run code before running each test  
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
           // Authentication.Logon();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
           // Authentication.Logoff();
        }

        #endregion
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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void RemoveEntryFromLookUpTest()
        {
            int lookUpFldId=-1;
            int lookUpDisplyaId=-1;
            COEDataViewBO dv = CreateDataView("DvLookup.xml", false, -1);
            TableBO tbl = dv.DataViewManager.Tables.GetTable(dv.DataViewManager.Tables[0].ID);
            List<FieldBO> fldBo = tbl.Fields.GetAllFieldsByLookup();
            foreach (FieldBO item in fldBo)
            {
                lookUpFldId=item.LookupFieldId;
                lookUpDisplyaId=item.LookupDisplayFieldId;
                tbl.Fields.RemoveEntryFromLookUp(lookUpFldId);
                tbl.Fields.RemoveEntryFromLookUp(lookUpDisplyaId);
            }
            fldBo = tbl.Fields.GetAllFieldsByLookup();
            Assert.IsTrue(fldBo.Count == 0, "RemoveEntryFromLookUp did not return the expected value.");
        }

        #region FieldListBO
        /// <summary>
        /// Test for CloneAndAddField
        /// </summary>
        [TestMethod]
        public void CloneAndAddField_Test()
        {
            COEDataViewBOTest theCOEDataViewBOTest = new COEDataViewBOTest();
            COEDataViewBO theCOEDataViewBO = theCOEDataViewBOTest.CreateDataView(@"\DataView.xml", true, -1);
            TableBO table = theCOEDataViewBO.DataViewManager.Tables[0];
            int Actual = theCOEDataViewBO.DataViewManager.Tables.HighestID;
            FieldBO addedField = table.Fields.CloneAndAddField(1, "_Alias", ++theCOEDataViewBO.DataViewManager.Tables.HighestID);
            int Expected = theCOEDataViewBO.DataViewManager.Tables.HighestID;
            COEDataViewBO.Delete(theCOEDataViewBO.ID);
            Assert.IsTrue(Actual + 1 == Expected, "FieldListBO.CloneAndAddField does not return expected value");
        }

        /// <summary>
        /// Test for GetAllFieldsByLookup
        /// </summary>
        [TestMethod]
        public void GetAllFieldsByLookup_Test()
        {
            COEDataViewBOTest theCOEDataViewBOTest = new COEDataViewBOTest();
            COEDataViewBO theCOEDataViewBO = theCOEDataViewBOTest.CreateDataView(@"\DvWithLookupAndRelations.xml", true, -1);
            TableBO table = theCOEDataViewBO.DataViewManager.Tables[0];
            List<FieldBO> theFieldBOList = table.Fields.GetAllFieldsByLookup();
            COEDataViewBO.Delete(theCOEDataViewBO.ID);
            Assert.IsTrue(theFieldBOList.Count > 0, "FieldListBO.GetAllFieldsByLookup does not return expected value");
        }


        #endregion

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
        public COEDataView BuildCOEDataViewFromXML(string dataViewXML)
        {
            //Load DataViewSerialized.XML
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + "\\" + dataViewXML);
            COEDataView coeDataView = new COEDataView();
            coeDataView.GetFromXML(doc);
            return coeDataView;
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
    }
}
