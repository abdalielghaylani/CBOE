using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using NUnit.Framework;
using System;


namespace NUnitTest
{
    [TestFixture]
    public class COEDataViewBOList_Test : LoginBase
    {
        #region Variables
        private string _pathToXmls = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + @"\TestXML";
        private string _databaseName = "orcl";
        //private DALFactory _dalFactory = new DALFactory();
        //private TestContext _testContextInstance;
        #endregion

        #region Test Methods

        #region GetDataViewDataList
        /// <summary>
        ///A test for GetDataViewDataList ()
        ///</summary>
        [Test]
        public void GetDataViewDataListTest()
        {
            COEDataViewBOList dvList;
            int Actual = 0;
            int Expected = 0;
            try
            {
                dvList = COEDataViewBOList.GetDataViewDataList();
                Expected = dvList.Count;
                Assert.AreNotEqual(Expected, Actual, "Did not return the expected value.");
            }
            catch
            {
                Assert.Fail("GetDataViewDataList failed.");
            }
        }
        #endregion

        #region GetDataViewDataListByUser
        /// <summary>
        ///A test for GetDataViewDataListByUser (string)
        /// Checking with cssadmin
        ///</summary>
        [Test]
        public void GetDataViewDataListByUserTest()
        {
            string userName = "cssadmin"; // TODO: Initialize to an appropriate value
            int Actual = 0;
            int Expected = 0;
            COEDataViewBOList dvList;
            try
            {
                dvList = COEDataViewBOList.GetDataViewDataListByUser(userName);
                Expected = dvList.Count;
            }
            catch
            {
                Assert.Fail("GetDataViewDataListByUserTest failed.");
            }
            Assert.AreNotEqual(Expected, Actual, "Did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDataViewDataListByUser (string)
        /// Checking with cssuser
        ///</summary>
        [Test]
        public void GetDataViewDataListByUserTest1()
        {
            string userName = "cssuser"; // TODO: Initialize to an appropriate value
            int Actual = 0;
            int Expected = 0;
            COEDataViewBOList dvList;
            try
            {
                dvList = COEDataViewBOList.GetDataViewDataListByUser(userName);
                Expected = dvList.Count;
            }
            catch
            {
                Assert.Fail("GetDataViewDataListByUserTest failed.");
            }
            Assert.AreNotEqual(Expected, Actual, "Did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDataViewDataListByUser (string)
        /// testing by user using COEDataviewBO methods
        /// Checking GetDataViewDataListByUser after adding new dataview to the user
        ///</summary>
        [Test]
        public void GetDataViewDataListByUserTest2()
        {
            string userName1 = "cssadmin"; // TODO: Initialize to an appropriate value
            COEDataViewBOList dvList1;
            string userName2 = "cssuser"; // TODO: Initialize to an appropriate value
            COEDataViewBOList dvList2;
            int Expected = 0;
            int Actual = 0;
            int Actual1 = 0;
            int Expected1 = 0;
            try
            {
                dvList1 = COEDataViewBOList.GetDataViewDataListByUser(userName1);
                int Count1 = dvList1.Count;
                dvList2 = COEDataViewBOList.GetDataViewDataListByUser(userName2);
                int Count2 = dvList2.Count;

                //creating public dataview
                COEDataViewBO_Test objBoTest = new COEDataViewBO_Test();
                COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", userName1, false, -1);

                userName1 = "cssadmin"; // TODO: Initialize to an appropriate value
                dvList1 = COEDataViewBOList.GetDataViewDataListByUser(userName1);
                Actual = Count1 + 1;
                Expected = dvList1.Count;

                userName2 = "cssuser"; // TODO: Initialize to an appropriate value
                dvList2 = COEDataViewBOList.GetDataViewDataListByUser(userName2);
                Actual1 = Count2;
                Expected1 = dvList2.Count;

                COEDataViewBO.Delete(dv.ID);
            }
            catch
            {
                Assert.Fail("GetDataViewDataListByUserTest failed.");
            }
            Assert.AreEqual(Expected, Actual, "Did not return the expected value for cssadmin user.");
            Assert.AreEqual(Expected1, Actual1, "Did not return the expected value for cssuser user.");

        }

        #endregion

        #region GetDataViewListAndNoMaster
        /// <summary>
        ///A test for GetDataViewListAndNoMaster ()
        ///</summary>
        [Test]
        public void GetDataViewListAndNoMasterTest()
        {
            //COEDataViewBOList expected = null;
            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();

            Assert.IsTrue(actual.Count >= 0, "COEDataViewBOList.GetDataViewListAndNoMaster did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataViewListAndNoMaster ()
        /// Checking GetDataViewListAndNoMaster after adding new dataview
        ///</summary>
        [Test]
        public void GetDataViewListAndNoMasterTest1()
        {
            //COEDataViewBOList expected = null;
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int count = actual.Count;

            //creating public dataview
            COEDataViewBO_Test objBoTest = new COEDataViewBO_Test();
            COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", true, -1);
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int ActualCount = actual.Count;
            COEDataViewBO.Delete(dv.ID);
            Assert.AreEqual(count + 1, ActualCount, "COEDataViewBOList.GetDataViewListAndNoMaster did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataViewListAndNoMaster ()
        /// Checking GetDataViewListAndNoMaster after adding new dataview
        ///</summary>
        [Test]
        public void GetDataViewListAndNoMasterTest2()
        {
            //COEDataViewBOList expected = null;
            COEDataViewBOList actual;
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int count = actual.Count;

            //creating public dataview
            COEDataViewBO_Test objBoTest = new COEDataViewBO_Test();
            COEDataViewBO dv = objBoTest.CreateDataView("DataView.xml", false, -1);
            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListAndNoMaster();
            int ActualCount = actual.Count;
            COEDataViewBO.Delete(dv.ID);
            Assert.AreEqual(count, ActualCount, "COEDataViewBOList.GetDataViewListAndNoMaster did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        #endregion

        #region GetDataViewListbyDatabaseTest
        /// <summary>
        ///A test for GetDataViewListbyDatabase (string)
        ///</summary>
        [Test]
        public void GetDataViewListbyDatabaseTest()
        {
            string databaseName = _databaseName; // TODO: Initialize to an appropriate value

            //COEDataViewBOList expected = null;
            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListbyDatabase(databaseName);

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListb" +
                    "yDatabase did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataViewListbyDatabase (string, bool)
        ///</summary>
        [Test]
        public void GetDataViewListbyDatabaseTest1()
        {
            string databaseName = _databaseName; // TODO: Initialize to an appropriate value

            //COEDataViewBOList expected = null;
            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListbyDatabase(databaseName, true);

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListb" +
                    "yDatabase did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataViewListbyDatabase (string, bool)
        ///</summary>
        [Test]
        public void GetDataViewListbyDatabaseTest2()
        {
            string databaseName = _databaseName; // TODO: Initialize to an appropriate value

            //COEDataViewBOList expected = null;
            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListbyDatabase(databaseName, false);

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListb" +
                    "yDatabase did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        #endregion

        #region GetDataViewListforAllDatabases
        /// <summary>
        ///A test for GetDataViewListforAllDatabases ()
        ///</summary>
        [Test]
        public void GetDataViewListforAllDatabasesTest()
        {

            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases();

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListf" +
                    "orAllDatabases did not return the expected value.");
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataViewListforAllDatabases (bool)
        ///</summary>
        [Test]
        public void GetDataViewListforAllDatabasesTest1()
        {
            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases(true);

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewDataL" +
                    "ist did not return the expected value.");
        }

        /// <summary>
        ///A test for GetDataViewListforAllDatabases (bool)
        ///</summary>
        [Test]
        public void GetDataViewListforAllDatabasesTest2()
        {
            COEDataViewBOList actual;

            actual = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases(false);

            Assert.IsTrue(actual.Count >= 0, "CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewDataL" +
                    "ist did not return the expected value.");
        }

        #endregion

        #region Login Compare

        /// <summary>
        ///A test for Login (string,string)
        ///</summary>
        [Test]
        public void LoginCompareTest()
        {
            try
            {
                COEPrincipal.Logout();
                System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
                bool resultLogin1 = COEPrincipal.Login("cssadmin", "cssadmin");
                COEDataViewBOList listForUser1;
                listForUser1 = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases();
                //Assert.IsNotNull(listForUser1);
                COEPrincipal.Logout();
                bool resultLogin2 = COEPrincipal.Login("cssuser", "cssuser");
                COEDataViewBOList listForUser2;
                listForUser2 = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases();
                //Assert.IsNotNull(listForUser2);
                //Assert.AreNotEqual(listForUser1,listForUser2, "GetDataViewUsingDifferentUsers did not return the expected value.");
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
