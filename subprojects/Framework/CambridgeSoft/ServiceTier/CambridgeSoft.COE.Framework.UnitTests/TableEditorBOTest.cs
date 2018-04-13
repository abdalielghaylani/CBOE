﻿//// The following code was generated by Microsoft Visual Studio 2005.
//// The test owner should check each test for validity.
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Text;
//using System.Xml;
//using System.Collections.Generic;
//using CambridgeSoft.COE.Framework.COETableEditorService;
//using CambridgeSoft.COE.Framework.Common;
//using CambridgeSoft.COE.Framework.COESecurityService;
//namespace CambridgeSoft.COE.Framework.COETableEditorService.UnitTests
//{
//    /// <summary>
//    ///This is a test class for CambridgeSoft.COE.Framework.COETableEditorService.COETableEditorBO and is intended
//    ///to contain all CambridgeSoft.COE.Framework.COETableEditorService.COETableEditorBO Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class COETableEditorBOTest
//    {
//        private string pathToXmls = Utilities.GetProjectBasePath("CambridgeSoft.COE.Framework.UnitTests") + @"\TestXML";
//        private string _tableName;
//        private string _databaseName;
//        private string _service = "COETableEditor";
//        private CambridgeSoft.COE.Framework.COETableEditorService.DAL _coeDAL;
//        private DALFactory _dalFactory=new DALFactory();

//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }
//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //
//        //[ClassInitialize()]
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        //
//        [TestInitialize()]
//        public void MyTestInitialize()
//        {
//            _tableName = "COEForm";
//            _databaseName = "SAMPLE";
//            Csla.ApplicationContext.GlobalContext["AppName"] = "SAMPLE";
//            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COETableEditorService.DAL>(ref _coeDAL, _service, _databaseName, true);
//            COEPrincipal.Logout();
//            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;

//            string userName = "CSSADMIN";
//            string password = "CSSADMIN";
//            // bool result = COEPrincipal.Login(userName, password); //This need to be resolved.
//            // if (result ==true){
//            if (Csla.ApplicationContext.GlobalContext["TablePrep"] == null)
//            {
//                // TruncateTables(); //Uncomment this to clean the COEForm table in database
//                Csla.ApplicationContext.GlobalContext["TablePrep"] = "DONE";
//            }
//        }

//        //Use TestCleanup to run code after each test has run
//        //
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //
//        #endregion

//        /// <summary>
//        /// Test for get and insert Function in TableEditorBO
//        /// </summary>
//        [TestMethod()]
//        public void AddTableEditorObjectTest()
//        {
//            COETableEditorBOList objList = COETableEditorBOList.NewList();
//            objList.TableName = _tableName;
//            COETableEditorBO obj = COETableEditorBO.New();
//            List<Column> lstColumn = null;
//            lstColumn = COETableEditorBOList.getColumns();
//            lstColumn.Find(new GetColumn("Name").Match).FieldValue = "Sumeet";
//            lstColumn.Find(new GetColumn("Description").Match).FieldValue = "Sumeet";
//            lstColumn.Find(new GetColumn("User_Id").Match).FieldValue = "Sumeet";
//            lstColumn.Find(new GetColumn("FormGroup").Match).FieldValue = 3;
//            lstColumn.Find(new GetColumn("Date_Created").Match).FieldValue =DateTime.Now;
//            lstColumn.Find(new GetColumn("COEForm").Match).FieldValue = GetFormXMLStringForStoring();

//            obj.Columns = lstColumn;
//            obj = obj.Save();
//            int i = obj.ID;
//            COETableEditorBO objComp = null;
//            objComp = COETableEditorBO.Get(i);
//            Assert.AreEqual(obj, objComp, "Either not added Successfully or not returning correct Value");
//        }
//        /// <summary>
//        /// Unit Test function for Delete
//        /// </summary>
//        [TestMethod()]
//        public void DeleteTableEditorObject()
//        {
//            COETableEditorBOList objList = COETableEditorBOList.NewList();
//            objList.TableName = _tableName;
//            COETableEditorBO obj = COETableEditorBO.New();
//            List<Column> lstColumn = null;
//            lstColumn = COETableEditorBOList.getColumns();
//            lstColumn.Find(new GetColumn("Name").Match).FieldValue = "Sumeet";
//            lstColumn.Find(new GetColumn("Description").Match).FieldValue = "Sumeet";
//            lstColumn.Find(new GetColumn("User_Id").Match).FieldValue = "Sumeet";
//            lstColumn.Find(new GetColumn("FormGroup").Match).FieldValue = 3;
//            lstColumn.Find(new GetColumn("Date_Created").Match).FieldValue =DateTime.Now;
//            lstColumn.Find(new GetColumn("COEForm").Match).FieldValue = GetFormXMLStringForStoring();

//            obj.Columns = lstColumn;
//            obj= obj.Save();
//            int i = obj.ID;
//            COETableEditorBO.Delete(i);
//            obj = COETableEditorBO.Get(i);
//            Assert.IsNull(obj.Columns, "Couldn't delete the object Successfully");
//        }

//        public string GetFormXMLStringForStoring()
//        {
//            //this is not implemented yet, so we will use an alternate route
//            XmlDocument doc = new XmlDocument();
//            doc.Load(pathToXmls + "\\COEFormForTests.xml");
//            return doc.InnerXml;//That must be InnerXML instead of String.
//        }
//    }
//    public class GetColumn
//    {
//        private string _columnName;

//        // Initializes with suffix we want to match.
//        public GetColumn(string columnName)
//        {
//            _columnName = columnName;
//        }

//        // Sets a different suffix to match.
//        public string ColumnName
//        {
//            get { return _columnName; }
//            set { _columnName = value; }
//        }

//        // Gets the predicate.  Now it's possible to re-use this predicate with 
//        // various suffixes.
//        public Predicate<Column> Match
//        {
//            get { return IsMatch; }
//        }

//        private bool IsMatch(Column s)
//        {
//            if (s.FieldName.Equals(_columnName))
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//    }

//}