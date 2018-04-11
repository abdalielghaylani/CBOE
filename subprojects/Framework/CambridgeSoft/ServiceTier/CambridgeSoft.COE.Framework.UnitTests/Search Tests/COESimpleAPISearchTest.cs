using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using CambridgeSoft.COE.Framework.COESearchService;
using System.Xml;
using CambridgeSoft.COE.Framework.COEHitListService;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;

namespace CambridgeSoft.COE.Framework.UnitTests.Search_Tests
{
    /// <summary>
    /// Summary description for COERegularAPISearch
    /// </summary>
    [TestClass]
    public class COESimpleAPISearchTest
    {
        #region Variables
        COESearch _searchService = null;
        static COEDataView _dataView = null;
        static DALFactory _df = null;
        static CambridgeSoft.COE.Framework.COESearchService.DAL _dal;
        static string _pathToXmls = string.Empty;
        static bool _checkQueryExec = false;
        static int _dataViewID = 0;
        static XmlDocument _doc;
        #endregion

        public COESimpleAPISearchTest()
        {
        }

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

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Login();
            _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._COESimpleAPISearchpathToXml);
            _dataView = GetDataView();
            _dataViewID = _dataView.DataViewID;
            LoadDAL();
            _checkQueryExec = AddColumn(); //Add column to Coetest.Moltable           
            try
            {
                _checkQueryExec = UpdateData(); //Update data in ISVAlID_STRUCT column in Coetest.Moltable
                _checkQueryExec = InsertDataview(); //Add data to Coedb.coedataview
            }
            finally
            {
                if (!_checkQueryExec)
                    DropColumn(); //If sql fails to insert data so removed the added column
            }

        }
        //Class Cleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            _checkQueryExec = DropColumn(); //Remove the added column from Coetest.Moltable
            _checkQueryExec = RemoveData(); //Remove the added data from Coedb.codedataview
        }

        #endregion

        //Test the boolean criteria
        [TestMethod]
        public void SearchUsingBooleanField()
        {
            _searchService = new COESearch(_dataView.DataViewID);
            string boolValue = "T";
            string boolField = "MOLTABLE.ISVALID_STRUCT";
            SearchInput input = new SearchInput(new string[] { boolField + "=" + boolValue }, null, null, new string[] { "FULL = NO" }, false);
            ResultPageInfo rpi = new ResultPageInfo();
            rpi.Start = 1;
            rpi.PageSize = 2000;
            string[] resultFields = new string[] { "MOLTABLE.ID", "MOLTABLE.MOL_ID", "MOLTABLE.BASE64_CDX", "MOLTABLE.MOLNAME", "SYNONYMS_R.ID", boolField };

            DataResult result = null;
            result = _searchService.DoSearch(input, resultFields, rpi, "YES");  //Searching using boolean criteria
            Assert.IsTrue(result.Status.Trim().ToUpper().Equals("SUCCESS", StringComparison.OrdinalIgnoreCase), "Boolean Search not working");
        }

        #region Static Methods
        static void Login()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
            string userName = "cssadmin";
            string password = "cssadmin";
            bool result = COEPrincipal.Login(userName, password);
        }

        //Load dataview in dataview object
        static COEDataView GetDataView()
        {
            _doc = new XmlDocument();
            _doc.Load(_pathToXmls + @"\DataView.xml");
            COEDataView dataview = new COEDataView(_doc);
            return dataview;
        }

        //Add boolean column into Moltable
        static bool AddColumn()
        {
            try
            {
                DbCommand addCol = _dal.DALManager.Database.GetSqlStringCommand("ALTER TABLE COETEST.MOLTABLE ADD ISVALID_STRUCT NCHAR(1) DEFAULT 'F'");
                _dal.DALManager.Database.ExecuteNonQuery(addCol);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Add dataview record into COEDATAVIEW
        static bool InsertDataview()
        {
            try
            {
                COEDataViewBO dv = COEDataViewBO.New(_dataView.Name, _dataView.Description, _dataView, null);
                dv.ID = _dataView.DataViewID;
                dv.IsPublic = true;
                dv = dv.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Update ISVALID_STRUCT column data from F to T for ID 1 and 5 only
        static bool UpdateData()
        {
            try
            {
                DbCommand addData = _dal.DALManager.Database.GetSqlStringCommand("UPDATE COETEST.MOLTABLE SET ISVALID_STRUCT='T' WHERE ID IN (1,5)");
                _dal.DALManager.Database.ExecuteNonQuery(addData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Delete dataview record from COEDATAVIEW
        static bool RemoveData()
        {
            try
            {
                DbCommand addData = _dal.DALManager.Database.GetSqlStringCommand("DELETE FROM COEDB.COEDATAVIEW WHERE ID=" + _dataViewID);
                _dal.DALManager.Database.ExecuteNonQuery(addData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Drop boolean column from Moltable
        static bool DropColumn()
        {
            try
            {
                DbCommand delCol = _dal.DALManager.Database.GetSqlStringCommand("ALTER TABLE COETEST.MOLTABLE DROP COLUMN ISVALID_STRUCT");
                _dal.DALManager.Database.ExecuteNonQuery(delCol);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void LoadDAL()
        {
            if (_df == null)
                _df = new DALFactory();
            if (_dal == null)
                _df.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref _dal, "COESearch", "COEDB", true);
        }
        #endregion
    }
}
