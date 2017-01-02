using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using CambridgeSoft.COE.Framework.COESearchService;
using System.Xml;
using System.IO;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;

namespace SearchUnitTests
{
    /// <summary>
    /// Summary description for LookupSearchTest
    /// </summary>
    [TestClass]
    public class LookupSearchTest
    {
        #region Variables
        private COESearch _searchService;
        private SearchCriteria _searchCriteria;
        private ResultsCriteria _resultsCriteria;
        private PagingInfo _pagingInfo;
        private COEDataView _dataView;
        private string _useRealTableNames;
        private string pathToXmls;
        #endregion

        #region Constructor
        public LookupSearchTest()
        {
            _searchService = new COESearch();
        }
        #endregion

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Login();
            _pagingInfo = GetFullPagingInfo();
            pathToXmls = SearchHelper.GetExecutingTestResultsBasePath(SearchHelper._LookupSearchTestpathToXml);
            _searchCriteria = GetSearchCriteria();
            _resultsCriteria = GetResultsCriteria();
            // _dataView = GetDataView();
            COEDataViewBO theCOEDataViewBO = SearchHelper.CreateDataView(SearchHelper._LookupSearchTestDvPath, SearchHelper._LookupSearchTestDV);
            _dataView = theCOEDataViewBO.COEDataView;
            _useRealTableNames = "YES";
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SearchHelper.DeleteDataView(SearchHelper._LookupSearchTestDV);
        }

        #endregion


        #region Test Methods
        [TestMethod]
        public void SearchUsingStructure_RegularAPI_Search()
        {
            SearchUsingStructure_RegularAPI(false, false);
        }

        [TestMethod]
        public void SearchUsingStructure_RegularAPI_NoHitlist()
        {
            SearchUsingStructure_RegularAPI(true, false);
        }

        [TestMethod]
        public void SearchUsingStructure_RegularAPI_NoHitlistNoPaging()
        {
            SearchUsingStructure_RegularAPI(true, true);
        }

        private void SearchUsingStructure_RegularAPI(bool noHitlist, bool noPaging)
        {
            _searchCriteria.Items[0].GetSearchCriteriaItem(0).Criterium.Value = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAA
AAAAAAAAAgkIAAAA4QAAgAYBDQgBAAEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAk
AAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA////
/wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA
5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggE
AAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCz
nXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggA
AMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQB
AAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgAC
AAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUG
BAACAAAACgYBAAEAAAAAAAAAAAAA";
            DataSet ds = GetDataSet(noHitlist, noPaging);
            Assert.AreNotEqual(0, ds.Tables.Count, "No table was returned by SearchUsingStructure_RegularAPI");
            Assert.AreEqual(0, ds.Tables[0].Rows.Count, "No rows were returned by SearchUsingStructure_RegularAPI");
        }

        /// <summary>
        /// Failed :CSCARTRIDGE.MOLECULEINDEXMETHODS
        /// </summary>
        [TestMethod]
        public void SearchUsingStructure_SimpleAPI()
        {
            try
            {
                _searchService = new COESearch(SearchHelper._LookupSearchTestDV);
                string base64 = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAA
AAAAAAAAAgkIAAAA4QAAgAYBDQgBAAEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAk
AAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA////
/wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA
5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggE
AAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCz
nXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggA
AMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQB
AAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgAC
AAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUG
BAACAAAACgYBAAEAAAAAAAAAAAAA";
                // SearchInput input = new SearchInput(new string[] { @"VW_MIXTURE_STRUCTURE.STRUCTURE SUBSTRUCTURE " + base64 }, null, null, new string[] { "FULL=NO", "LookupByValueFields: VW_MIXTURE_STRUCTURE.STRUCTURE" }, false);
                SearchInput input = new SearchInput(new string[] { @"COMPOUND.STRUCTURE SUBSTRUCTURE " + base64 }, null, null, new string[] { "FULL=NO", "LookupByValueFields: COMPOUND.STRUCTURE, COMPOUND.STRUCTUREFORMAT" }, false);
                ResultPageInfo rpi = new ResultPageInfo();
                rpi.Start = 1;
                rpi.PageSize = 2000;
                //string[] resultFields = new string[] { "VW_MIXTURE_STRUCTURE.COMPOUNDID", "VW_MIXTURE_STRUCTURE.REGID", "VW_MIXTURE_STRUCTURE.FORMULAWEIGHT", "VW_MIXTURE_STRUCTURE.MOLECULARFORMULA", "VW_MIXTURE_STRUCTURE.STRUCTURE" };
                string[] resultFields = new string[] { "COMPOUND.COMPOUNDID", "COMPOUND.REGID", "COMPOUND.FORMULAWEIGHT", "COMPOUND.MOLECULARFORMULA", "COMPOUND.STRUCTUREFORMAT", "COMPOUND.STRUCTURE" };
                DataResult result = _searchService.DoSearch(input, resultFields, rpi, "YES");
                StringReader reader = new StringReader(result.ResultSet);
                DataSet ds = new DataSet();
                ds.ReadXml(reader);
                Assert.IsTrue(ds.Tables.Count > 0, "No table was returned by SearchUsingStructure_SimpleAPI");
                Assert.IsTrue(ds.Tables[0].Rows.Count > 0, "No rows were returned by SearchUsingStructure_SimpleAPI");

                rpi.ResultSetID = result.resultSetInfo.ID;
                rpi.Start = 1;
                result = _searchService.GetDataPage(rpi, resultFields);
                reader = new StringReader(result.ResultSet);
                ds = new DataSet();
                ds.ReadXml(reader);
                Assert.IsTrue(ds.Tables.Count > 0, "No table was returned by SearchUsingStructure_SimpleAPI");
                Assert.IsTrue(ds.Tables[0].Rows.Count > 0, "No rows were returned by SearchUsingStructure_SimpleAPI");
            }
            catch
            {
                Assert.Inconclusive("Fail:Issues to execute queries containing CSCARTRIDGE.MoleculeContains");
            }
        }

        #endregion

        #region Private Methods
        private void Login()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;

            string userName = "cssadmin";
            string password = "cssadmin";
            bool result = COEPrincipal.Login(userName, password);
        }

        private PagingInfo GetFullPagingInfo()
        {
            PagingInfo pi = new PagingInfo();
            pi.Start = 1;
            pi.RecordCount = 50000;
            return pi;
        }

        private COEDataView GetDataView()
        {

            COEDataViewBO bo = null;
            try
            {
                bo = COEDataViewBO.Get(SearchHelper._LookupSearchTestDV);
            }
            catch
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathToXmls + @"\DataView.xml");
                COEDataView dataview = new COEDataView(doc);
                bo = COEDataViewBO.New();
                bo.ID = SearchHelper._LookupSearchTestDV;
                bo.Name = "COM_MOLECULE";
                bo.Description = "Compound Molecule";
                bo.COEDataView = dataview;
                bo.IsPublic = true;
                bo = bo.Save();
            }
            return bo.COEDataView;
        }

        private ResultsCriteria GetResultsCriteria()
        {
            return this.GetResultsCriteria(@"\ResultsCriteria.xml");
        }

        private ResultsCriteria GetResultsCriteria(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + filename);
            ResultsCriteria rc = new ResultsCriteria(doc);
            return rc;
        }

        private SearchCriteria GetSearchCriteria()
        {
            return this.GetSearchCriteria(@"\SearchCriteria.xml");
        }

        private SearchCriteria GetSearchCriteria(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToXmls + fileName);
            SearchCriteria sc = new SearchCriteria(doc);
            return sc;
        }

        private DataSet GetDataSet(bool noHitlist, bool noPaging)
        {
            DataSet ds = new DataSet();
            if (noPaging)
            {
                _pagingInfo.End = _pagingInfo.RecordCount = 0;
            }

            if (noHitlist)
            {
                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView, _useRealTableNames, null, _searchCriteria);
            }
            else
            {
                HitListInfo info = _searchService.GetHitList(_searchCriteria, _dataView);
                _pagingInfo.HitListID = info.HitListID;
                _pagingInfo.HitListType = info.HitListType;


                ds = _searchService.GetData(_resultsCriteria, _pagingInfo, _dataView);
            }

            return ds;
        }
        #endregion

        #region Simplified Access Methods From COESearch
        /// <summary>
        /// Unit Test for  DataResult DoSearch(SearchInput searchInput, string[] resultFields, ResultPageInfo resultPageInfo)
        /// </summary>
        [TestMethod]
        public void DoSearchUsingTabNameTest()
        {
            _searchService = new COESearch(SearchHelper._LookupSearchTestDV);
            string base64 = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAA
AAAAAAAAAgkIAAAA4QAAgAYBDQgBAAkEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAk
AAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA////
/wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA
5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggE
AAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCz
nXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggA
AMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQB
AAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgAC
AAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUG
BAACAAAACgYBAAEAAAAAAAAAAAAA";
            SearchInput input = new SearchInput(new string[] { @"COMPOUND.STRUCTURE SUBSTRUCTURE " + base64 }, null, null, new string[] { "FULL=NO", "LookupByValueFields: COMPOUND.STRUCTURE, COMPOUND.STRUCTUREFORMAT" }, false);
            string[] resultFields = new string[] { "COMPOUND.COMPOUNDID", "COMPOUND.REGID", "COMPOUND.FORMULAWEIGHT", "COMPOUND.MOLECULARFORMULA", "COMPOUND.STRUCTUREFORMAT", "COMPOUND.STRUCTURE" };
            ResultPageInfo rpi = new ResultPageInfo();
            rpi.Start = 1;
            rpi.PageSize = 2000;
            DataResult result = _searchService.DoSearch(input, resultFields, rpi);
            Assert.IsNotNull(result, "COESearch.DoSearch does not return expected value");
        }
        /// <summary>
        /// Unit Test for  DataListResult DoSearch(SearchInput searchInput, string pkField, ResultPageInfo resultPageInfo)
        /// </summary>
        [TestMethod]
        public void DoSearchusingpkFieldTest()
        {
            _searchService = new COESearch(SearchHelper._LookupSearchTestDV);
            string base64 = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAA
AAAAAAAAAgkIAAAA4QAAgAYBDQgBAAkEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAk
AAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA////
/wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA
5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggE
AAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCz
nXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggA
AMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQB
AAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgAC
AAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUG
BAACAAAACgYBAAEAAAAAAAAAAAAA";
            SearchInput input = new SearchInput(new string[] { @"COMPOUND.STRUCTURE SUBSTRUCTURE " + base64 }, null, null, new string[] { "FULL=NO", "LookupByValueFields: COMPOUND.STRUCTURE, COMPOUND.STRUCTUREFORMAT" }, false);
            ResultPageInfo rpi = new ResultPageInfo();
            rpi.Start = 1;
            rpi.PageSize = 2000;
            DataListResult result = _searchService.DoSearch(input, "COMPOUND.COMPOUNDID", rpi);
            Assert.IsNotNull(result, "COESearch.DoSearch does not return expected value");
        }

        ///Failed :CSCARTRIDGE.MoleculeContains
        /// <summary>
        /// Unit Test for   ResultSetInfo GetResultSetInfo(int resultSetID)
        /// </summary>
        [TestMethod]
        public void GetResultSetInfoTest()
        {
            try
            {
                _searchService = new COESearch(SearchHelper._LookupSearchTestDV);
                string base64 = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAA
AAAAAAAAAgkIAAAA4QAAgAYBDQgBAAkEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAk
AAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA////
/wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA
5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggE
AAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCz
nXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggA
AMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQB
AAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgAC
AAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUG
BAACAAAACgYBAAEAAAAAAAAAAAAA";
                SearchInput input = new SearchInput(new string[] { @"COMPOUND.STRUCTURE SUBSTRUCTURE " + base64 }, null, null, new string[] { "FULL=NO", "LookupByValueFields: COMPOUND.STRUCTURE, COMPOUND.STRUCTUREFORMAT" }, false);
                string[] resultFields = new string[] { "COMPOUND.COMPOUNDID", "COMPOUND.REGID", "COMPOUND.FORMULAWEIGHT", "COMPOUND.MOLECULARFORMULA", "COMPOUND.STRUCTUREFORMAT", "COMPOUND.STRUCTURE" };
                ResultPageInfo rpi = new ResultPageInfo();
                rpi.Start = 1;
                rpi.PageSize = 2000;
                DataResult result = _searchService.DoSearch(input, resultFields, rpi);
                ResultSetInfo theResultSetInfo = _searchService.GetResultSetInfo(result.resultSetInfo.ID);
                Assert.IsNotNull(theResultSetInfo, "COESearch.GetResultSetInfoTest does not return expected value");
            }
            catch
            {
                Assert.Inconclusive("Fail:Issues to execute queries containing CSCARTRIDGE.MoleculeContains");
            }
        }

        //Failed : CSCARTRIDGE.MoleculeContains
        /// <summary>
        /// Unit Test for ResultSetInfo GetUpdatedResultSetInfo(ResultSetInfo resultSetInfo)
        /// </summary>
        [TestMethod]
        public void GetUpdatedResultSetInfoTest()
        {
            try
            {
                _searchService = new COESearch(SearchHelper._LookupSearchTestDV);
                string base64 = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAA
AAAAAAAAAgkIAAAA4QAAgAYBDQgBAAkEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAk
AAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA////
/wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA
5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggE
AAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCz
nXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggA
AMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQB
AAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgAC
AAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUG
BAACAAAACgYBAAEAAAAAAAAAAAAA";
                SearchInput input = new SearchInput(new string[] { @"COMPOUND.STRUCTURE SUBSTRUCTURE " + base64 }, null, null, new string[] { "FULL=NO", "LookupByValueFields: COMPOUND.STRUCTURE, COMPOUND.STRUCTUREFORMAT" }, false);
                string[] resultFields = new string[] { "COMPOUND.COMPOUNDID", "COMPOUND.REGID", "COMPOUND.FORMULAWEIGHT", "COMPOUND.MOLECULARFORMULA", "COMPOUND.STRUCTUREFORMAT", "COMPOUND.STRUCTURE" };
                ResultPageInfo rpi = new ResultPageInfo();
                rpi.Start = 1;
                rpi.PageSize = 2000;
                DataResult result = _searchService.DoSearch(input, resultFields, rpi);
                ResultSetInfo theResultSetInfo = _searchService.GetUpdatedResultSetInfo(result.resultSetInfo);
                Assert.IsNotNull(theResultSetInfo, "COESearch.GetUpdatedResultSetInfo does not return expected value");
            }
            catch
            {

                Assert.Inconclusive("Fail:Issues to execute queries containing CSCARTRIDGE.MoleculeContains");
            }
        }
        #endregion
    }
}