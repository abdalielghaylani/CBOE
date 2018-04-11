using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.Framework.UnitTests
{
    /// <summary>
    /// Summary description for CachingTest
    /// </summary>
    [TestClass]
    [Serializable]
    public class CachingTest
    {
        public CachingTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
            bool result = COEPrincipal.Login("cssadmin", "cssadmin");
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ClientVsServerCaching()
        {
            COEDataViewBO dataViewTestObject = StoreDataView(true, -1);
            int id = dataViewTestObject.ID;

            dataViewTestObject = COEDataViewBO.Get(dataViewTestObject.ID);
            dataViewTestObject.Description = "Something different";
            dataViewTestObject = dataViewTestObject.Save();
            COEDataViewBO clientOBJ = LocalCache.Get(dataViewTestObject.ID.ToString(), dataViewTestObject.GetType()) as COEDataViewBO;
            COEDataViewBO serverOBJ = ServerCache.Get(dataViewTestObject.ID.ToString(), dataViewTestObject.GetType()) as COEDataViewBO;
            Assert.AreEqual(dataViewTestObject.Description, clientOBJ.Description);
            Assert.AreEqual(null, serverOBJ);
            COEDataViewBO.Delete(id);
            clientOBJ = LocalCache.Get(dataViewTestObject.ID.ToString(), dataViewTestObject.GetType()) as COEDataViewBO;
            Assert.AreEqual(null, clientOBJ);
        }

        [TestMethod]
        public void ChemDrawCtrlCaching()
        {
            ChemDrawControl12.ChemDrawCtl ctrl = new ChemDrawControl12.ChemDrawCtl();
            string uniqueID = ctrl.GetHashCode().ToString() + "_" + AppDomain.CurrentDomain.FriendlyName;

            LocalCache.Add(uniqueID, ctrl.GetType(), ctrl, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(50), COECacheItemPriority.Normal);
            ServerCache.Add(uniqueID, ctrl.GetType(), ctrl, ServerCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(50), COECacheItemPriority.Normal);

            ChemDrawControl12.ChemDrawCtl clientOBJ = LocalCache.Get(uniqueID, ctrl.GetType()) as ChemDrawControl12.ChemDrawCtl;
            ChemDrawControl12.ChemDrawCtl serverOBJ = ServerCache.Get(uniqueID, ctrl.GetType()) as ChemDrawControl12.ChemDrawCtl;
            
            Assert.AreEqual(ctrl.FullName, clientOBJ.FullName);
            Assert.AreEqual(ctrl.FullName, serverOBJ.FullName);

            LocalCache.Remove(uniqueID, ctrl.GetType());
            ServerCache.Remove(uniqueID, ctrl.GetType());

            clientOBJ = LocalCache.Get(uniqueID, ctrl.GetType()) as ChemDrawControl12.ChemDrawCtl;
            serverOBJ = ServerCache.Get(uniqueID, ctrl.GetType()) as ChemDrawControl12.ChemDrawCtl;
            Assert.AreEqual(clientOBJ, null);
            Assert.AreEqual(serverOBJ, null);
        }

        protected void RemovedFromServerCache(string key, object value, COECacheItemRemovedReason reason)
        {
            Console.WriteLine("Object: *** " + key + " *** was removed from server cache, because " + reason.ToString());
        }

        /// <summary>
        /// Stores a new dataview.
        /// </summary>
        /// <param name="isPublic">Is Public property</param>
        /// <param name="id">A given id. If it is &lt;= 0 a new ID is generated.</param>
        /// <returns>The saved dataview. If the provided is is grater than 0, that id is used, a new id is generated otherwise.</returns>
        private COEDataViewBO StoreDataView(bool isPublic, int id)
        {
            //this should create a new object
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.ID = id;
            dataViewObject.Name = "temp" + GenerateRandomNumber();
            dataViewObject.Description = "temp";
            dataViewObject.DatabaseName = "COEDB";
            //this really should come from the logged in user..
            dataViewObject.UserName = "CSSUSER";
            dataViewObject.COEDataView = BuildCOEDataViewFromXML();
            dataViewObject.IsPublic = isPublic;
            //this is where it get's persisted
            dataViewObject = dataViewObject.Save();
            return dataViewObject;
        }
        
        public COEDataView BuildCOEDataViewFromXML()
        {
            //Load DataViewSerialized.XML
            XmlDocument doc = new XmlDocument();
            doc.Load(Utilities.GetProjectBasePath("CambridgeSoft.COE.Framework.UnitTests") + @"\TestXML\COEDataViewForTests.xml");
            COEDataView coeDataView = new COEDataView();
            coeDataView.GetFromXML(doc);
            return coeDataView;
        }

        private string GenerateRandomNumber()
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
