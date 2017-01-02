using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;

using System.Xml;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.NUnitTests.Helpers
{
    internal class SearchHelper
    {

        #region COEAggregationResultsTestsMembers
        public static int _COEAggdataViewID1 = 5003;
        public static int _COEAggdataViewID2 = 5103;
        public static string _COEAggpathToXml = @"\Search Tests\COEAggregationResultsTests XML";
        public static string _COEAggpathOfDV1 = @"Search Tests\DataViews\5003.xml";
        public static string _COEAggpathOfDV2 = @"Search Tests\DataViews\5103.xml";
        #endregion

        #region COESearchByAggregateTestMembers
        public static string _COESearchByAggpathToXml = @"\Search Tests\COESearchByAggregateTest XML";
        #endregion

        #region COESearchByHitlistTestsMembers
        public static string _COESearchByHitlistpathToXml = @"\Search Tests\COESearchByHitlistTests XML\";
        #endregion

        #region COESimpleAPISearchTestMembers
        public static string _COESimpleAPISearchpathToXml = @"\Search Tests\COESimpleAPISearch XML";
        #endregion

        #region COEStructureListTestsMembers
        public static string _COEStructureListTestspathToXml = @"\Search Tests\COEStructureListTests XML";
        public static int _COEStructureListTestDV1 = 5002;
        public static int _COEStructureListTestDV2 = 5010;
        public static string _COEStructureListTestDv1path = @"Export Tests\COEExportXML\COTESTDataview.xml";
        public static string _COEStructureListTestDv2path = @"Search Tests\DataViews\5010.xml";
        #endregion

        #region FilterChildDataTestMembers
        public static string _FilterChildDataTestDvPath = @"Search Tests\DataViews\5003.xml";
        public static int _FilterChildDataTestDv = 5003;
        public static string _FilterChildDataTestpathToXml = @"\Search Tests\FilterChildDataTest XML";
        #endregion

        #region LookupSearchTest
        public static string _LookupSearchTestDvPath = @"\Search Tests\LookupSearchTest XML\DataView.xml";
        public static string _LookupSearchTestpathToXml = @"\Search Tests\LookupSearchTest XML";
        public static int _LookupSearchTestDV = 10;

        #endregion

        public static string _selectClausexmlPath = @"\Common\SqlGenerator\Queries\SelectItems";
        public static string _COEExportToExcel = @"\COEExportToExcel";

        #region COESearchTest
        public static string _COESearchTestpathToXml = @"\Search Tests\COESearchTest XML\OrderedSearchTestXml";
        #endregion



        #region CommandMethodsforSearchModule
        public static string GetExecutingTestResultsBasePath(string strExtendedPath)
        {
            return AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("CambridgeSoft.COE.Framework.NUnitTests")) + @"CambridgeSoft.COE.Framework.NUnitTests\" + strExtendedPath;
                
                
              //  Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")) + @"CambridgeSoft.COE.Framework.NUnitTests\" + strExtendedPath;
        }
        /// <summary>
        /// Create dataview 
        /// </summary>
        /// <param name="dataViewXML"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static COEDataViewBO CreateDataView(string dataViewXML, int id)
        {
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.ID = id;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = "COEDB";
            dataViewObject.UserName = ConfigurationManager.AppSettings["LogonUserName"];
            dataViewObject.IsPublic = true;
            dataViewObject.DataViewManager = COEDataViewManagerBO.NewManager(dataViewObject.COEDataView);
            try
            {
                COEDataViewBO theCOEDataViewBO = COEDataViewBO.Get(id);
            }
            catch
            {
                dataViewObject = dataViewObject.Save();

            }
            return dataViewObject;
        }

        public static COEDataViewBO CreateDataView(string dataViewXML)
        {
            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = BuildCOEDataViewFromXML(dataViewXML);
            dataViewObject.ID = dataViewObject.COEDataView.DataViewID;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = "COEDB";
            dataViewObject.UserName = ConfigurationManager.AppSettings["LogonUserName"];
            dataViewObject.IsPublic = true;
            dataViewObject.DataViewManager = COEDataViewManagerBO.NewManager(dataViewObject.COEDataView);

            try
            {
                COEDataViewBO theCOEDataViewBO = COEDataViewBO.Get(dataViewObject.COEDataView.DataViewID);
            }
            catch
            {
                dataViewObject = dataViewObject.Save();

            }
            return dataViewObject;
        }
        public static COEDataView BuildCOEDataViewFromXML(string dataViewXML)
        {
            //Load DataViewSerialized.XML
            XmlDocument doc = new XmlDocument();
            doc.Load(GetExecutingTestResultsBasePath(string.Empty) + "\\" + dataViewXML);
            COEDataView coeDataView = new COEDataView();
            coeDataView.GetFromXML(doc);
            return coeDataView;
        }
        private static string GenerateRandomNumber()
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
        /// Deletes dataview from COEDATAVIEW
        /// </summary>
        /// <param name="_dataviewID"></param>
        public static void DeleteDataView(int _dataviewID)
        {
            try
            {
                COEDataViewBO.Delete(_dataviewID);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
