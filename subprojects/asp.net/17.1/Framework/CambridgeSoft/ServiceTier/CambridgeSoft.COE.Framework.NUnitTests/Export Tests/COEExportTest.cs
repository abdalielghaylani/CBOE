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
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COEDataViewService.UnitTests;

namespace CambridgeSoft.COE.Framework.COEExportService.UnitTests
{
    [TestFixture]
    public class COEExportTest : LoginBase
    {
        #region Variables
        private string _pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(@"\Export Tests\COEExportXML");
        ResultsCriteria _resultsCriteria = null;
        SearchCriteria _searchCriteria = null;
        COEDataView _dataView = null;
        PagingInfo _pagingInfo = new PagingInfo();
        #endregion

        #region Test Methods

        /// <summary>
        /// A test for SDFFlatCorrelated based on (ResultsCriteria, PagingInfo, COEDataView, string)
        /// which will be created in DoExport method using the COTESTDataview.xml, 
        /// COETESTResultCriteria.xml and COTESTPaginginfo.xml
        /// </summary>
        [Test]
        public void ExportSDFFlatCorrelated()
        {
            string exportResults = DoExport("SDFFlatFileCorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        /// A test for SDFFlatUNCorrelated based on (ResultsCriteria, PagingInfo, COEDataView, string)
        /// which will be created in DoExport method using the COTESTDataview.xml, 
        /// COETESTResultCriteria.xml and COTESTPaginginfo.xml
        /// </summary>
        [Test]
        public void ExportSDFFlatUNCorrelated()
        {
            string exportResults = DoExport("SDFFlatFileUncorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        /// A test for ExportSDFNested based on (ResultsCriteria, PagingInfo, COEDataView, string)
        /// which will be created in DoExport method using the COTESTDataview.xml, 
        /// COETESTResultCriteria.xml and COTESTPaginginfo.xml
        /// </summary>
        [Test]
        public void ExportSDFNested()
        {
            string exportResults = DoExport("SDFNested");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        /// A test for SDFFlatCorrelated with GrandChild 
        /// which will be created in CSBR127380 method using the COETESTGrandChildDataview.xml
        /// </summary>
        [Test]
        public void GrandChildTestSDFFlatCorrelated()
        {
            string exportResults = CSBR127380("SDFFlatFileCorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        /// A test for SDFFlatUNCorrelated with GrandChild 
        /// which will be created in CSBR127380 method using the COETESTGrandChildDataview.xml
        /// </summary>
        [Test]
        public void GrandChildTestSDFFlatUNCorrelated()
        {
            string exportResults = CSBR127380("SDFFlatFileUncorrelated");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        ///  A test for SDFNested with GrandChild 
        /// which will be created in DoExport method using the COETESTGrandChildDataview.xml
        /// </summary>
        [Test]
        public void GrandChildTestSDFNested()
        {
            string exportResults = CSBR127380("SDFNested");
            Assert.IsTrue(exportResults != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        ///  A test for GrandChildSDFNested
        /// which will be created in DoExport method using the COETESTGrandChildDataview.xml
        /// </summary>
        [Test]
        public void GrandChildTestMolForm()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated", "resultcriteriamarMOLFORM.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriamarMOLFORM.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriamarMOLFORM.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        /// A test for all three export types i.e SDFFlatFileCorrelated,SDFFlatFileUncorrelated and SDFNested with Structure.
        /// </summary>
        [Test]
        public void GrandChildTestStruct()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        /// A test for all three export types i.e SDFFlatFileCorrelated,SDFFlatFileUncorrelated and SDFNested with Mol id.
        /// </summary>
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

        /// <summary>
        /// A test for all three export types i.e SDFFlatFileCorrelated,SDFFlatFileUncorrelated and SDFNested with Syn Id.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void GrandChildTestComplicatedStruct()
        {
            string exportSDFFlatFileCorrelated = CSBR127380_2("SDFFlatFileCorrelated", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFFlatFileCorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFFlatFileUncorrelated = CSBR127380_2("SDFFlatFileUncorrelated", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFFlatFileUncorrelated != string.Empty, "COEExport.GetData did not return the expected value.");

            string exportSDFNested = CSBR127380_2("SDFNested", "resultcriteriamarSTRUCT.xml");
            Assert.IsTrue(exportSDFNested != string.Empty, "COEExport.GetData did not return the expected value.");
        }

        /// <summary>
        ///A test for ExportSDFFlatFileUncorrelated (ResultsCriteria, PagingInfo, COEDataView, string)
        ///</summary>
        [Test]
        public void ExportSDFFlatFileUncorrelated()
        {

            ResultsCriteria.ResultsCriteriaTable rctSynoyms = new ResultsCriteria.ResultsCriteriaTable();
            rctSynoyms.Id = 2;

            ResultsCriteria.Field rcSynonyms = new ResultsCriteria.Field();
            rcSynonyms.Id = 6;
            rcSynonyms.Alias = "Synonym";

            rctSynoyms.Criterias.Add(rcSynonyms);
            XmlDocument doc3 = new XmlDocument();
            doc3.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\ResultsCriteria.xml");
            _resultsCriteria = new ResultsCriteria(doc3);

            XmlDocument doc2 = new XmlDocument();
            doc2.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\SearchCriteria.xml");
            _searchCriteria = new SearchCriteria(doc2);

            ResultsCriteria rc = _resultsCriteria;
            rc.Tables.Add(rctSynoyms);

            ResultsCriteria.ResultsCriteriaTable rctProps = new ResultsCriteria.ResultsCriteriaTable();
            rctProps.Id = 10;

            ResultsCriteria.Field rcProps = new ResultsCriteria.Field();
            rcProps.Id = 12;
            rcProps.Alias = "Properties";

            rctProps.Criterias.Add(rcProps);

            rc.Tables.Add(rctProps);

            XmlDocument doc = new XmlDocument();

            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\DataView.xml");
            _dataView = new COEDataView(doc);

            string exportResults = DoExport("SDFFlatFileUncorrelated", rc);
            Assert.IsTrue(exportResults != string.Empty, "CambridgeSoft.COE.Framework.COEExportService.COEExport.GetData did not return the" +
                     " expected value.");

            int recordCount = exportResults.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length;

            Assert.IsTrue((recordCount == 119), "119 results were expected but " + recordCount + " were returned.");
        }


        /// <summary>
        ///A test for ExportSDFFlatFileCorrelated (ResultsCriteria, PagingInfo, COEDataView, string)
        ///</summary>
        [Test]
        public void ExportSDFFlatFileCorrelated()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\DataView.xml");
            _dataView = new COEDataView(doc);

            doc = new XmlDocument();
            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\SearchCriteria.xml");
            _searchCriteria = new SearchCriteria(doc);

            doc = new XmlDocument();
            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\ResultsCriteria.xml");
            _resultsCriteria = new ResultsCriteria(doc);

            ResultsCriteria.ResultsCriteriaTable rctSynoyms = new ResultsCriteria.ResultsCriteriaTable();
            rctSynoyms.Id = 2;

            ResultsCriteria.Field rcSynonyms = new ResultsCriteria.Field();
            rcSynonyms.Id = 6;
            rcSynonyms.Alias = "Synonym";

            rctSynoyms.Criterias.Add(rcSynonyms);

            ResultsCriteria rc = _resultsCriteria;
            rc.Tables.Add(rctSynoyms);


            ResultsCriteria.ResultsCriteriaTable rctProps = new ResultsCriteria.ResultsCriteriaTable();
            rctProps.Id = 10;

            ResultsCriteria.Field rcProps = new ResultsCriteria.Field();
            rcProps.Id = 12;
            rcProps.Alias = "Properties";

            rctProps.Criterias.Add(rcProps);

            rc.Tables.Add(rctProps);

            string exportResults = DoExport("SDFFlatFileCorrelated", rc);
            Assert.IsTrue(exportResults != string.Empty, "CambridgeSoft.COE.Framework.COEExportService.COEExport.GetData did not return the" +
                     " expected value.");
        }

        /// <summary>
        ///A test for exporting to SDF Flat when there is no child data.  We need to make sure that the base table records are
        ///still output. Benzonaphthene and 1,2,3-Triazole will have no synonyms
        ///</summary>
        [Test]
        public void ExportSDFFlatFileUncorrelatedRecordsWithNochildData()
        {

            ResultsCriteria.ResultsCriteriaTable rctSynoyms = new ResultsCriteria.ResultsCriteriaTable();
            rctSynoyms.Id = 2;

            ResultsCriteria.Field rcSynonyms = new ResultsCriteria.Field();
            rcSynonyms.Id = 6;
            rcSynonyms.Alias = "Synonym";

            rctSynoyms.Criterias.Add(rcSynonyms);
            InitializeInputs();
            ResultsCriteria rc = _resultsCriteria;
            rc.Tables.Add(rctSynoyms);

            string exportResults = DoExport("SDFFlatFileUncorrelated", rc);
            Assert.IsTrue(exportResults != string.Empty, "CambridgeSoft.COE.Framework.COEExportService.COEExport.GetData did not return the" +
                     " expected value.");


            Assert.IsTrue((exportResults.IndexOf("1,2,3-Triazole") > 0), "1,2,3-Triazole which has no synyonyms was not found in the export but should be.");
            Assert.IsTrue((exportResults.IndexOf("Benzonaphthene") > 0), "Benzonaphthene which has no synyonyms was not found in the export but should be.");

            int recordCount = exportResults.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length;

            Assert.IsTrue((recordCount == 89), "89 results were expected but " + recordCount + " were returned.");

        }

        /// <summary>
        /// Test for Highlighted structures
        /// </summary>
        [Test]
        public void ExportStructureAndHighlightedStructure()
        {
            ResultsCriteria.HighlightedStructure hlStructure = new ResultsCriteria.HighlightedStructure();
            hlStructure.Id = 3;
            hlStructure.Alias = "HighlightedStructure";

            int Expected = 30;

            XmlDocument doc = new XmlDocument();
            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\DataView.xml");
            _dataView = new COEDataView(doc);

            doc = new XmlDocument();
            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\SearchCriteria.xml");
            _searchCriteria = new SearchCriteria(doc);

            doc = new XmlDocument();
            doc.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\ResultsCriteria.xml");
            _resultsCriteria = new ResultsCriteria(doc);

            _resultsCriteria.Tables[0].Criterias.Add(hlStructure);

            string sdfFlat = DoExport("SDFFlatFileUncorrelated");
            string[] molFiles = sdfFlat.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(Expected, molFiles.Length, "SDFFlatFileUncorrelated export did not return the right amount of hits");
            int i = 0;
            foreach (string molFile in molFiles)
            {
                Assert.IsTrue(molFile.StartsWith("\r\n  CsCart "), "SDFFlatFileUncorrelated exported an structure in a wrong format at pos " + i);
                //Assert.IsTrue(molFile.Contains(hlStructure.Alias), "SDFFlatFileUncorrelated did not export the highlightedstructure at pos " + i);
                i = i + 1;
            }

            InitializeInputs();
            _resultsCriteria.Tables[0].Criterias.Add(hlStructure);

            string sdfFlatCorrelated = DoExport("SDFFlatFileCorrelated");
            molFiles = sdfFlatCorrelated.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(Expected, molFiles.Length, "SDFFlatFileCorrelated export did not return the right amount of hits");
            i = 0;
            foreach (string molFile in molFiles)
            {
                Assert.IsTrue(molFile.StartsWith("\r\n  CsCart "), "SDFFlatFileCorrelated exported an structure in a wrong format at pos " + i);
                //Assert.IsTrue(molFile.Contains(hlStructure.Alias), "SDFFlatFileCorrelated did not export the highlightedstructure at pos " + i);
                i = i + 1;
            }

            InitializeInputs();
            _resultsCriteria.Tables[0].Criterias.Add(hlStructure);

            string sdfNested = DoExport("SDFNested");
            molFiles = sdfNested.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(Expected, molFiles.Length, "SDFNested export did not return the right amount of hits");
            i = 0;
            foreach (string molFile in molFiles)
            {
                Assert.IsTrue(molFile.StartsWith("\r\n  CsCart "), "SDFNested exported an structure in a wrong format at pos " + i);
                //Assert.IsTrue(molFile.Contains(hlStructure.Alias), "SDFNested did not export the highlightedstructure at pos " + i);
                i = i + 1;
            }



            InitializeInputs();
            _resultsCriteria.Tables[0].Criterias.RemoveAt(2); // Remove the structure, to leave only the highlighted
            _resultsCriteria.Tables[0].Criterias.Add(hlStructure);

            sdfFlat = DoExport("SDFFlatFileUncorrelated");
            molFiles = sdfFlat.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(Expected, molFiles.Length, "SDFFlatFileUncorrelated export did not return the right amount of hits");
            i = 0;
            foreach (string molFile in molFiles)
            {
                Assert.IsTrue(molFile.StartsWith("\r\n  CsCart "), "SDFFlatFileUncorrelated exported an structure in a wrong format at pos " + i);
                Assert.IsFalse(molFile.Contains(hlStructure.Alias), "SDFFlatFileUncorrelated did not remove the highlightedstructure at pos " + i);
                i = i + 1;
            }

            InitializeInputs();
            _resultsCriteria.Tables[0].Criterias.RemoveAt(2); // Remove the structure, to leave only the highlighted
            _resultsCriteria.Tables[0].Criterias.Add(hlStructure);

            sdfFlatCorrelated = DoExport("SDFFlatFileCorrelated");
            molFiles = sdfFlatCorrelated.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(Expected, molFiles.Length, "SDFFlatFileCorrelated export did not return the right amount of hits");
            i = 0;
            foreach (string molFile in molFiles)
            {
                Assert.IsTrue(molFile.StartsWith("\r\n  CsCart "), "SDFFlatFileCorrelated exported an structure in a wrong format at pos " + i);
                Assert.IsFalse(molFile.Contains(hlStructure.Alias), "SDFFlatFileCorrelated did not remove the highlightedstructure at pos " + i);
                i = i + 1;
            }

            InitializeInputs();
            _resultsCriteria.Tables[0].Criterias.RemoveAt(2); // Remove the structure, to leave only the highlighted
            _resultsCriteria.Tables[0].Criterias.Add(hlStructure);

            sdfNested = DoExport("SDFNested");
            molFiles = sdfNested.Split(new string[1] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(Expected, molFiles.Length, "SDFNested export did not return the right amount of hits");
            i = 0;
            foreach (string molFile in molFiles)
            {
                Assert.IsTrue(molFile.StartsWith("\r\n  CsCart "), "SDFNested exported an structure in a wrong format at pos " + i);
                Assert.IsFalse(molFile.Contains(hlStructure.Alias), "SDFNested did not remove the highlightedstructure at pos " + i);
                i = i + 1;
            }
        }

        /// <summary>
        /// Test for GetFormatterTypesList
        /// </summary>
        [Test]
        public void GetFormatterTypesList_Test()
        {
            COEExport coeExport = new COEExport();
            List<string> theList = coeExport.GetFormatterTypesList();
            Assert.IsNotNull(theList);
        }
        #endregion

        #region Test Cases for AdvanceExport

        [Test]
        public void COEAdvancedExport_GetData_Test()
        {
            InitializeInputs();
            COEAdvancedExport theCOEAdvancedExport = new COEAdvancedExport();
            try
            {
                string Result = theCOEAdvancedExport.GetData(_dataView, _searchCriteria, _resultsCriteria, _pagingInfo, "cssadmin", "", "SDFNested");
            }
            catch
            {


            }
            Assert.Inconclusive("Unable to verify method");
        }

        #endregion

        #region Test caces for COEFormInterchange

        [Test]
        public void GetForm_Test()
        {
            InitializeInputs();
            COEFormInterchange theCOEFormInterchange = new COEFormInterchange();
            try
            {
                string Result = theCOEFormInterchange.GetForm(_dataView, _searchCriteria, _resultsCriteria, _pagingInfo, new ServerInfo(), "cssadmin", "", "SDFNested");
            }
            catch
            {

            }
            Assert.Inconclusive("Unable to verify method");
        }


        #endregion

        #region Private Methos

        /// <summary>
        /// This method prepare's the SearchCriteria,resultsCriteria,dataView and pagingInfo 
        /// using COTESTDataview.xml,COETESTResultCriteria.xml and COTESTPaginginfo.xml.
        /// </summary>
        /// <param name="exportType">Gets the Export Results based on exportType </param>
        /// <returns></returns>
        private string DoExport(string exportType)
        {
            ResultsCriteria resultsCriteria = new ResultsCriteria();
            int ActualId = int.MinValue;

            COEExport coeExport = new COEExport();

            SearchCriteria searchCriteria = new SearchCriteria();
            COEDataView dataView = new COEDataView();
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO CVBO = CreateDataView(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\DataView.xml", true, -1);
            COEDataView dv = CVBO.COEDataView;
            ActualId = CVBO.ID;


            XmlDocument doc3 = new XmlDocument();
            doc3.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\ResultsCriteria.xml");
            resultsCriteria.GetFromXML(doc3);


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

            doc3 = new XmlDocument();
            doc3.Load(SearchHelper.GetExecutingTestResultsBasePath(@"\TestXML") + @"\SearchCriteria.xml");
            _searchCriteria = new SearchCriteria(doc3);

            HitListInfo hitListInfo = coeSearch.GetHitList(_searchCriteria, ActualId);

            //create a paginginof object and populate it with info from the hitlistinfo object
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.HitListID = hitListInfo.HitListID;
            pagingInfo.Start = 1;
            pagingInfo.RecordCount = 500; //more than the last one

            string exportResults = coeExport.GetData(resultsCriteria, pagingInfo, ActualId, exportType);
            COEDataViewBO.Delete(ActualId);

            return exportResults;


        }

        private string DoExport(string exportType, ResultsCriteria rc)
        {
            return DoExport(exportType, rc, _searchCriteria);
        }

        private string DoExport(string exportType, ResultsCriteria rc, SearchCriteria sc)
        {
            COEExport coeExport = new COEExport();


            //do the search so we can generate a Hitlist to pass to the Export service which is what we are testing
            //the database to be searched is a property of the dataview
            COESearch coeSearch = new COESearch();
            HitListInfo hitListInfo = coeSearch.GetHitList(_searchCriteria, _dataView);
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = hitListInfo.RecordCount;
            _pagingInfo.HitListID = hitListInfo.HitListID;
            _pagingInfo.HitListType = hitListInfo.HitListType;


            string exportResults = coeExport.GetData(rc, _pagingInfo, _dataView, exportType);
            return exportResults;
        }

        private void InitializeInputs()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_pathToXmls + @"\DataView.xml");
            _dataView = new COEDataView(doc);

            XmlDocument doc2 = new XmlDocument();
            doc2.Load(_pathToXmls + @"\SearchCriteria.xml");
            _searchCriteria = new SearchCriteria(doc2);

            XmlDocument doc3 = new XmlDocument();
            doc3.Load(_pathToXmls + @"\ResultsCriteria.xml");
            _resultsCriteria = new ResultsCriteria(doc3);

            //create a paginginof object and populate it with info from the hitlistinfo object
            _pagingInfo = new PagingInfo();
            _pagingInfo.Start = 1;
            _pagingInfo.RecordCount = 10;
        }

        private string CSBR127380(string exportType)
        {

            ResultsCriteria resultsCriteria = new ResultsCriteria();
            int ActualId = int.MinValue;
            COEExport coeExport = new COEExport();

            SearchCriteria searchCriteria = new SearchCriteria();
            COEDataView dataView = new COEDataView();
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO CVBO = obj.CreateDataView("COETESTGrandChildDataview.xml", true, -1);
            COEDataView dv = CVBO.COEDataView;
            ActualId = CVBO.ID;



            XmlDocument doc3 = new XmlDocument();
            doc3.Load(_pathToXmls + @"\COETESTResultCriteria.xml");
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
            PagingInfo pagingInfo = new PagingInfo();
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
            COEExport coeExport = new COEExport();
            SearchCriteria searchCriteria = new SearchCriteria();
            COEDataView dataView = new COEDataView();
            COEDataViewBOTest obj = new COEDataViewBOTest();
            COEDataViewBO CVBO = obj.CreateDataView("resultcriteria\\dataviewmar1.xml", true, -1);
            COEDataView dv = CVBO.COEDataView;
            ActualId = CVBO.ID;

            XmlDocument doc3 = new XmlDocument();
            doc3.Load(_pathToXmls + @"\resultcriteria\" + rescriteria);
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

        public void Resultcrit()
        {
            ResultsCriteria res = new ResultsCriteria();
            XmlDocument doc3 = new XmlDocument();
            doc3.Load(_pathToXmls + @"\resultcriteria\resultcriteriamarMOLFORM.xml");
            res = ResultsCriteria.GetResultsCriteria(doc3.InnerXml.ToString());
        }

        public COEDataViewBO CreateDataView(string dataViewXML, bool isPublic, int id)
        {
            //Load DataViewSerialized.XML
            XmlDocument doc = new XmlDocument();
            doc.Load(dataViewXML);
            COEDataView coeDataView = new COEDataView();
            coeDataView.GetFromXML(doc);

            COEDataViewBO dataViewObject = COEDataViewBO.New();
            dataViewObject.COEDataView = coeDataView;
            dataViewObject.ID = id;
            dataViewObject.Name = "test" + GenerateRandomNumber();
            dataViewObject.Description = "test";
            dataViewObject.DatabaseName = "COEDB";
            dataViewObject.UserName = "cssadmin";
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

        #endregion
    }

}
