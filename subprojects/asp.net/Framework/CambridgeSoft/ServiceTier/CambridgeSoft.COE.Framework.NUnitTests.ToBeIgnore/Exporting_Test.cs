using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
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

namespace NUnitTest
{
    [TestFixture]
    public class Exporting_Test : LoginBase
    {
        #region Variables
        private string _pathToXmls = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + @"\TestXML";


        #endregion
        #region Test Methods
        [Test]
        public void ExportSDFFlatCorrelated()
        {
            string exportResults = DoExport("SDFFlatFileCorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void ExportSDFFlatUNCorrelated()
        {
            string exportResults = DoExport("SDFFlatFileUncorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void ExportSDFNested()
        {
            string exportResults = DoExport("SDFNested");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void GrandChildTestSDFFlatCorrelated()
        {
            string exportResults = CSBR127380("SDFFlatFileCorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void GrandChildTestSDFFlatUNCorrelated()
        {
            string exportResults = CSBR127380("SDFFlatFileUncorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void GrandChildTestSDFNested()
        {
            string exportResults = CSBR127380("SDFNested");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void GrandChildTestMolForm()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated","resultcriteriamarMOLFORM.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriamarMOLFORM.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriamarMOLFORM.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void GrandChildTestStruct()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated","resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }



        [Test]
        public void GrandChildTestMolID()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated", "resultcriteriaMOLID.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriaMOLID.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriaMOLID.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }


        [Test]
        public void GrandChildTestSynID()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated", "resultcriteriaSYNID.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriaSYNID.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriaSYNID.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        [Test]
        public void GrandChildTestComplicatedStruct()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated", "resultcriteriaSTRUCTCOMP.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriaSTRUCTCOMP.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriaSTRUCTCOMP.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }
        #endregion

        #region Private Methos

        private string DoExport(string exportType)
        {
            ResultsCriteria resultsCriteria = new ResultsCriteria();


            int ActualId = int.MinValue;
            //try
            //{
            COEExport coeExport = new COEExport();

            //ResultsCriteria resultsCriteria = new ResultsCriteria();
            SearchCriteria searchCriteria = new SearchCriteria();
            COEDataView dataView = new COEDataView();


            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO CVBO = obj.CreateDataView("DataviewExport.xml", true, -1);
            COEDataViewBO CVBO = obj.CreateDataView("COTESTDataview.xml", true, -1);
            COEDataView dv = CVBO.COEDataView;
            ActualId = CVBO.ID;



            XmlDocument doc3 = new XmlDocument();
            doc3.Load(_pathToXmls + @"\COETESTResultCriteria.xml");
            //XmlNode resultsCriteriaNode = xmlResultsCriteria.SelectSingleNode("//" + xmlNamespace + ":resultsCriteria", this.manager);
            resultsCriteria.GetFromXML(doc3);

            XmlDocument doc4 = new XmlDocument();
            doc4.Load(_pathToXmls + @"\COTESTPaginginfo.xml");

            SearchCriteria.NumericalCriteria nc = new SearchCriteria.NumericalCriteria();
            nc.Operator = SearchCriteria.COEOperators.LTE;
            nc.Value = "50";

            SearchCriteria.SearchCriteriaItem sci = new SearchCriteria.SearchCriteriaItem();
            sci.ID = 1;
            sci.FieldId = 206;
            sci.TableId = 203;
            sci.Criterium = nc;

            searchCriteria.Items.Add(sci);

            COESearch coeSearch = new COESearch();
            HitListInfo hitListInfo = coeSearch.GetHitList(searchCriteria, ActualId);

            //create a paginginof object and populate it with info from the hitlistinfo object
            PagingInfo pagingInfo = new PagingInfo();
            //pagingInfo.GetFromXML(doc4);
            pagingInfo.HitListID = hitListInfo.HitListID;
            pagingInfo.Start = 1;
            pagingInfo.RecordCount = 500; //more than the last one

            string exportResults = coeExport.GetData(resultsCriteria, pagingInfo, ActualId, exportType);
            COEDataViewBO.Delete(ActualId);

            return exportResults;


        }

        private string CSBR127380(string exportType)
        {

            ResultsCriteria resultsCriteria = new ResultsCriteria();


            int ActualId = int.MinValue;
            //try
            //{
            COEExport coeExport = new COEExport();

            //ResultsCriteria resultsCriteria = new ResultsCriteria();
            SearchCriteria searchCriteria = new SearchCriteria();
            COEDataView dataView = new COEDataView();


            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO CVBO = obj.CreateDataView("DataviewExport.xml", true, -1);
            COEDataViewBO CVBO = obj.CreateDataView("COETESTGrandChildDataview.xml", true, -1);
            COEDataView dv = CVBO.COEDataView;
            ActualId = CVBO.ID;



            XmlDocument doc3 = new XmlDocument();
            doc3.Load(_pathToXmls + @"\COETESTResultCriteria.xml");
            //XmlNode resultsCriteriaNode = xmlResultsCriteria.SelectSingleNode("//" + xmlNamespace + ":resultsCriteria", this.manager);
            resultsCriteria.GetFromXML(doc3);

            XmlDocument doc4 = new XmlDocument();
            doc4.Load(_pathToXmls + @"\COTESTPaginginfo.xml");

            SearchCriteria.NumericalCriteria nc = new SearchCriteria.NumericalCriteria();
            nc.Operator = SearchCriteria.COEOperators.LTE;
            nc.Value = "50";

            SearchCriteria.SearchCriteriaItem sci = new SearchCriteria.SearchCriteriaItem();
            sci.ID = 1;
            sci.FieldId = 206;
            sci.TableId = 203;
            sci.Criterium = nc;

            searchCriteria.Items.Add(sci);

            COESearch coeSearch = new COESearch();
            HitListInfo hitListInfo = coeSearch.GetHitList(searchCriteria, ActualId);

            //create a paginginof object and populate it with info from the hitlistinfo object
            PagingInfo pagingInfo = new PagingInfo();
            //pagingInfo.GetFromXML(doc4);
            pagingInfo.HitListID = hitListInfo.HitListID;
            pagingInfo.Start = 1;
            pagingInfo.RecordCount = 500; //more than the last one

            string exportResults = coeExport.GetData(resultsCriteria, pagingInfo, ActualId, exportType);
            COEDataViewBO.Delete(ActualId);

            return exportResults;


        }

        private string CSBR127380_2(string exportType, string rescriteria)
        {
            ResultsCriteria resultsCriteria = new ResultsCriteria();


            int ActualId = int.MinValue;
            //try
            //{
            COEExport coeExport = new COEExport();

            //ResultsCriteria resultsCriteria = new ResultsCriteria();
            SearchCriteria searchCriteria = new SearchCriteria();
            COEDataView dataView = new COEDataView();


            COEDataViewBO_Test obj = new COEDataViewBO_Test();
            //COEDataViewBO CVBO = obj.CreateDataView("DataviewExport.xml", true, -1);
            COEDataViewBO CVBO = obj.CreateDataView("resultcriteria\\dataviewmar1.xml", true, -1);
            COEDataView dv = CVBO.COEDataView;
            ActualId = CVBO.ID;



            XmlDocument doc3 = new XmlDocument();
            //doc3.Load(_pathToXmls + @"\resultcriteria\resultcriteriamarMOLFORM.xml");
            doc3.Load(_pathToXmls + @"\resultcriteria\" + rescriteria);
            //XmlNode resultsCriteriaNode = xmlResultsCriteria.SelectSingleNode("//" + xmlNamespace + ":resultsCriteria", this.manager);
            resultsCriteria.GetFromXML(doc3);
            //resultsCriteria = ResultsCriteria.GetResultsCriteria(doc3.InnerXml.ToString());

            XmlDocument doc4 = new XmlDocument();
            doc4.Load(_pathToXmls + @"\COTESTPaginginfo.xml");

            SearchCriteria.NumericalCriteria nc = new SearchCriteria.NumericalCriteria();
            nc.Operator = SearchCriteria.COEOperators.LTE;
            nc.Value = "50";

            SearchCriteria.SearchCriteriaItem sci = new SearchCriteria.SearchCriteriaItem();
            sci.ID = 1;
            sci.FieldId = 206;
            sci.TableId = 203;
            sci.Criterium = nc;


            searchCriteria.Items.Add(sci);

            COESearch coeSearch = new COESearch();
            HitListInfo hitListInfo = coeSearch.GetHitList(searchCriteria, ActualId);

            //create a paginginof object and populate it with info from the hitlistinfo object
            PagingInfo pagingInfo = new PagingInfo();
            //pagingInfo.GetFromXML(doc4);
            pagingInfo.HitListID = hitListInfo.HitListID;
            pagingInfo.Start = 1;
            pagingInfo.RecordCount = 500; //more than the last one

            string exportResults = coeExport.GetData(resultsCriteria, pagingInfo, ActualId, exportType);
            COEDataViewBO.Delete(ActualId);

            return exportResults;

        }

        public void Resultcrit()
        {
            ResultsCriteria res = new ResultsCriteria();
             XmlDocument doc3 = new XmlDocument();
            //doc3.Load(_pathToXmls + @"\resultcriteria\resultcriteriamarMOLFORM.xml");
            doc3.Load(_pathToXmls + @"\resultcriteria\resultcriteriamarMOLFORM.xml");

            res = ResultsCriteria.GetResultsCriteria(doc3.InnerXml.ToString());

        }
        #endregion
    }
}
