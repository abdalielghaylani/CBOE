using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.IO;
using Csla.Data;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using System.Xml;

namespace CambridgeSoft.COE.Framework.UnitTests.Search_Tests
{
    [TestClass]
    public class MultipleInstancesTest
    {
        //INtegration test that insert data in database. Should be use to test during development. Not for CI

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    SearchManager manager = new SearchManager();

        //    var dataView = GetDataView(101492);

        //    XmlDocument doc1 = new XmlDocument();
        //    doc1.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\Multiple instances XML") + @"\searchCriteria.xml");

        //    SearchCriteria sc = new SearchCriteria();
        //    sc.GetFromXML(doc1);

        //    XmlDocument doc2 = new XmlDocument();
        //    doc2.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\Search Tests\Multiple instances XML") + @"\resultCriteria.xml");

        //    ResultsCriteria resultsCriteria = new ResultsCriteria();
        //    resultsCriteria.GetFromXML(doc2);

        //    Dictionary<int, DataSet> multipleDataSet = CrossInstanceHitListHelper.GenerateJoinedTable(sc, resultsCriteria, dataView, manager);

        //    int maxNum = 0;

        //    if (multipleDataSet != null)
        //    {
        //        maxNum = HitListDataViewHelper.InsertHitList(multipleDataSet, resultsCriteria, dataView, 999, manager);
        //    }

        //    Assert.AreEqual(maxNum, 73);

        //}

        static COEDataView GetDataView(int dataViewId)
        {
            DALFactory dalFactory = new DALFactory();
            CambridgeSoft.COE.Framework.COEDataViewService.DAL coeDAL = null;
            dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEDataViewService.DAL>(ref coeDAL, "COEDataView", "COEDB", true);
            using (SafeDataReader dr = coeDAL.Get(dataViewId))
            {
                if (dr.Read())
                {
                    return (COEDataView)COEDataViewUtilities.DeserializeCOEDataView(dr.GetString("COEDATAVIEW"));
                }
            }

            return null;
        }
    }
}
