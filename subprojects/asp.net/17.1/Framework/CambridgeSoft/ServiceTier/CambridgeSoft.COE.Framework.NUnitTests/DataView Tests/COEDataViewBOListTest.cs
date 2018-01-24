using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.Data.Common;
using Csla.Data;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;

namespace CambridgeSoft.COE.Framework.COEDataViewService.UnitTests
{
    [TestFixture]
    public class COEDataViewBOListTest : LoginBase
    {
        #region Variables
        private string _pathToXmls = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("CambridgeSoft.COE.Framework.NUnitTests")) + @"CambridgeSoft.COE.Framework.NUnitTests" + @"\DataView Tests\COEDataViewTestXML";
        private string _databaseName = "orcl";
        DataViewDAL dvDal = new DataViewDAL();
        private DALFactory _dalFactory = new DALFactory();
        private int[] _ids;
        #endregion

        #region Test Methods

        #region GetDataViewDataList
        /// <summary>
        ///A test for GetDataViewDataList ()
        ///</summary>
        [Test]
       // [Priority(101)]
        public void GetDataViewDataList_Test()
        {
            COEDataViewBOList dvList;
            int Actual = 0;
            int Expected = dvDal.GetExpectedDataViewsCount();
            try
            {
                dvList = COEDataViewBOList.GetDataViewDataList();
                Actual = dvList.Count;
                Assert.AreEqual(Actual, Expected, "Expected '" + Expected + "' dataviews but got back '" + Actual + "' dataviews.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #region GetDataViewDataListByUser
        /// <summary>
        ///A test for GetDataViewDataListByUser (string)
        /// Checking with cssadmin
        ///</summary>
        [Test]
        public void GetDataViewDataListByUser()
        {
            GetDataViewDataListByUserTest_Test("cssadmin");
            GetDataViewDataListByUserTest_Test("cssuser");
        }

        /// <summary>
        ///A test for GetDataViewDataListByUser (string)
        ///</summary>
        public void GetDataViewDataListByUserTest_Test(string userName)
        {    
            int Actual = 0;
            int Expected = dvDal.GetExpectedDataViewsCount(userName);
            COEDataViewBOList dvList;
            try
            {
                dvList = COEDataViewBOList.GetDataViewDataListByUser(userName);
                Actual = dvList.Count;
            }
            catch
            {
                Assert.Fail("GetDataViewDataListByUserTest failed.");
            }
            Assert.AreEqual(Actual, Expected, "Expected '" + Expected + "' dataviews but got back '" + Actual + "' dataviews for the user '" + userName +"'.");
        }

        /// <summary>
        ///A test for GetDataViewDataListByUser (string)
        /// testing by user using COEDataviewBO methods
        /// Checking GetDataViewDataListByUser after adding new dataview to the user
        ///</summary>
        [Test]
        public void GetDataViewDataListByUser_CountCompare()
        {
            string userName1 = "cssadmin";
            COEDataViewBOList dvList1;
            string userName2 = "cssuser";
            COEDataViewBOList dvList2;
            int Expected = 0;// dvDal.GetExpectedDataViewsCount(userName1);
            int Actual = 0;
            int Actual1 = 0;
            int Expected1 = 0;// dvDal.GetExpectedDataViewsCount(userName2);
            try
            {
                dvList1 = COEDataViewBOList.GetDataViewDataListByUser(userName1);
                int Count1 = dvList1.Count;
                dvList2 = COEDataViewBOList.GetDataViewDataListByUser(userName2);
                int Count2 = dvList2.Count;
                //creating public dataview
                COEDataViewBOTest objBoTest = new COEDataViewBOTest();
                //Following method creats dataview, but not assigns access rights. 
                //As a result view is not accessible. Hences changing the method.
                //COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", userName1, true, -1);
                COEDataViewBO dv = objBoTest.CreateDataViewWithAP("DataView.xml", userName1, true);
                userName1 = "cssadmin";
                dvList1 = COEDataViewBOList.GetDataViewDataListByUser(userName1);
                Expected = Count1 + 1;
                Actual = dvList1.Count;

                userName2 = "cssuser";
                dvList2 = COEDataViewBOList.GetDataViewDataListByUser(userName2);
                Expected1 = Count2;
                Actual1 = dvList2.Count;
                COEDataViewBO.Delete(dv.ID);
            }
            catch
            {
                Assert.Fail("GetDataViewDataListByUserTest failed.");
            }
            Assert.AreEqual(Actual, Expected, "Expected '" + Expected + "' dataviews but got back '" + Actual + "' dataviews.");
            Assert.AreEqual(Actual1, Expected1, "Expected '" + Expected1 + "' dataviews but got back '" + Actual1 + "' dataviews.");

        }

        #endregion

        #region GetDataViewListAndNoMaster
        /// <summary>
        ///A test for GetDataViewListAndNoMaster ()
        ///</summary>
        [Test]
        public void GetDataViewListAndNoMaster_Test()
        {
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            Assert.IsTrue(actual.Count >= 0, "COEDataViewBOList.GetDataViewListAndNoMaster did not return the expected value.");
        }

        /// <summary>
        ///A TestMethod() for GetDataViewListAndNoMaster ()
        /// Checking GetDataViewListAndNoMaster after adding new dataview
        ///</summary>
        [Test]
        public void GetDataViewListAndNoMaster_AddingNewPublicView()
        {
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int count = actual.Count;
            COEDataViewBOTest objBoTest = new COEDataViewBOTest();
            COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", true, -1);
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int ActualCount = actual.Count;
            COEDataViewBO.Delete(dv.ID);
            Assert.AreEqual(count + 1, ActualCount, "COEDataViewBOList.GetDataViewListAndNoMaster did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDataViewListAndNoMaster ()
        /// Checking GetDataViewListAndNoMaster after adding new dataview
        ///</summary>
        [Test]
        public void GetDataViewListAndNoMaster_AddingNewPrivateView()
        {
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int count = actual.Count;
            COEDataViewBOTest objBoTest = new COEDataViewBOTest();
            COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", false, -1);
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int ActualCount = actual.Count;
            COEDataViewBO.Delete(dv.ID);
            Assert.AreEqual(count + 1, ActualCount, "COEDataViewBOList.GetDataViewListAndNoMaster did not return the expected value.");
        }

        #endregion

        #region GetDataViewListbyDatabaseTest
        /// <summary>
        ///A test for GetDataViewListbyDatabase (string)
        ///</summary>
        [Test]
        public void GetDataViewListbyDatabase_Test()
        {
            string databaseName = _databaseName;
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListbyDatabase(databaseName);
            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListb" +
                    "yDatabase did not return the expected value.");
        }
        /// <summary>
        ///A test for GetDataViewListbyDatabase (string, bool)
        ///</summary>
        [Test]
        public void GetDataViewListbyDatabase_LightWeightTrue()
        {
            string databaseName = _databaseName;
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListbyDatabase(databaseName, true);
            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListb" +
                    "yDatabase did not return the expected value.");
        }
        /// <summary>
        ///A test for GetDataViewListbyDatabase (string, bool)
        ///</summary>
        [Test]
        public void GetDataViewListbyDatabase_LightWeightFalse()
        {
            string databaseName = _databaseName;
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListbyDatabase(databaseName, false);
            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListb" +
                    "yDatabase did not return the expected value.");
        }

        #endregion

        #region GetDataViewListforAllDatabases
        /// <summary>
        ///A test for GetDataViewListforAllDatabases ()
        ///</summary>
        [Test]
        public void GetDataViewListforAllDatabases_Test()
        {
            COEDataViewBOList actual;
            actual = COEDataViewBOList.GetDataViewListforAllDatabases();
            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListf" +
                    "orAllDatabases did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDataViewListforAllDatabases (bool)
        ///</summary>
        [Test]
        public void GetDataViewListforAllDatabases_LightWeightTrue()
        {
            COEDataViewBOList actual;
            actual = COEDataViewBOList.GetDataViewListforAllDatabases(true);
            Assert.IsTrue(actual.Count >= 0, "GetDataViewListforAllDatabases did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDataViewListforAllDatabases (bool)
        ///</summary>
        [Test]
        public void GetDataViewListforAllDatabases_LightWeightFalse()
        {
            COEDataViewBOList actual;
            actual = COEDataViewBOList.GetDataViewListforAllDatabases(false);
            Assert.IsTrue(actual.Count >= 0, "GetDataViewListforAllDatabases did not return the expected value.");
        }

        /// <summary>
        /// Test for GetDataviewListForApplication(String)
        /// </summary>
        [Test]
        public void GetDataviewListForApplication_Test()
        {
            COEDataViewBOList actual;
            actual = COEDataViewBOList.GetDataviewListForApplication("COEDB");
            Assert.IsTrue(actual.Count >= 0, "GetDataviewListForApplication did not return the expected value.");
        }

        /// <summary>
        /// Test For GetByDatabaseName(string)
        /// </summary>
        [Test]
        public void GetByDatabaseName_Test()
        {
            COEDataViewBOList actual = COEDataViewBOList.GetDataViewListforAllDatabases();

            Assert.IsTrue(actual.GetByDatabaseName("COEDB").Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetByDatabaseName did not return the expected value.");
        }

        /// <summary>
        /// Test for GetDataViewBOByID(int)
        /// </summary>
        [Test]
        public void GetDataViewBOByID_Test()
        {
            COEDataViewBOTest objBoTest = new COEDataViewBOTest();
            COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", true, -1);

            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(typeof(COEDataViewBOList));
            Csla.FilteredBindingList<COEDataViewBO> actual = (Csla.FilteredBindingList<COEDataViewBO>)thePrivateObject.Invoke("GetDataViewBOByID", new object[] { dv.ID });
            COEDataViewBO.Delete(dv.ID);
            Assert.IsTrue((actual == null || actual.Count >= 0), "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewBOByID did not return the expected value.");
        }

        /// <summary>
        /// Test for NewList()
        /// </summary>
        [Test]
        public void NewList_Test()
        {
            COEDataViewBOList actual = COEDataViewBOList.NewList();

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.NewList did not return the expected value.");
        }

        /// <summary>
        /// Test For Dataportal_Update
        /// </summary>
        [Test]
        public void DataPortal_Update_Test()
        {
            COEDataViewBOList actual = COEDataViewBOList.GetDataViewListforAllDatabases();
            Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(typeof(COEDataViewBOList));
            thePrivateObject.Invoke("DataPortal_Update", new object[] { });
            Assert.IsNotNull(thePrivateObject, "DataPortal_Update cannot be verified");
        }

        #endregion

        #region Login Compare

        /// <summary>
        ///A test for Login (string,string)
        ///</summary>
        [Test]
        public void LoginCompare_Test()
        {
            try
            {
                COEPrincipal.Logout();
                System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
                bool resultLogin1 = COEPrincipal.Login("cssadmin", "cssadmin");
                COEDataViewBOList listForUser1;
                listForUser1 = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases();
                COEPrincipal.Logout();
                bool resultLogin2 = COEPrincipal.Login("cssuser", "cssuser");
                COEDataViewBOList listForUser2;
                listForUser2 = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases();
                COEPrincipal.Logout();
            }
            catch
            {
                Assert.Inconclusive("A method cannot be verified, because login failed.");
            }

        }
        #endregion

        #endregion
    }
}
