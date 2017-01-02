using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;


namespace CustomSelectClause {
    class Program {
        static void Main(string[] args) {
            COESearch search = new COESearch();
            
            XmlDocument dataviewXml = new XmlDocument();
            XmlDocument resultsCriteriaXml = new XmlDocument();
            XmlDocument searchCriteriaXml = new XmlDocument();
            // Change to point to your xmls
            dataviewXml.Load("SearchXml\\DataView.xml");
            resultsCriteriaXml.Load("SearchXml\\ResultsCriteria.xml");
            searchCriteriaXml.Load("SearchXml\\SearchCriteria.xml");
            CambridgeSoft.COE.Framework.Common.PagingInfo pagingInfo = new CambridgeSoft.COE.Framework.Common.PagingInfo();
            pagingInfo.Start = 1;
            pagingInfo.RecordCount = 1000;
            DoLogin();

            SearchResponse searchResponse = search.DoSearch(new CambridgeSoft.COE.Framework.Common.SearchCriteria(searchCriteriaXml),
                            new CambridgeSoft.COE.Framework.Common.ResultsCriteria(resultsCriteriaXml),
                            pagingInfo,
                            new CambridgeSoft.COE.Framework.Common.COEDataView(dataviewXml));


            Console.WriteLine("Resulting Tables Count: " + searchResponse.ResultsDataSet.Tables.Count);
            Console.WriteLine("Record count: " + searchResponse.HitListInfo.RecordCount);

            Console.WriteLine("Press any key to test the custom Molweight Criteria...");
            Console.ReadKey(true);

            dataviewXml.Load("SearchXml\\DataView-SAMPLE.xml");
            resultsCriteriaXml.Load("SearchXml\\ResultsCriteria-SAMPLE.xml");
            searchCriteriaXml.Load("SearchXml\\SearchCriteria-SAMPLE.xml");
            pagingInfo = new CambridgeSoft.COE.Framework.Common.PagingInfo();
            pagingInfo.Start = 1;
            pagingInfo.RecordCount = 1000;

            searchResponse = search.DoSearch(new CambridgeSoft.COE.Framework.Common.SearchCriteria(searchCriteriaXml),
                            new CambridgeSoft.COE.Framework.Common.ResultsCriteria(resultsCriteriaXml),
                            pagingInfo,
                            new CambridgeSoft.COE.Framework.Common.COEDataView(dataviewXml));

            Console.WriteLine("Resulting Tables Count: " + searchResponse.ResultsDataSet.Tables.Count);
            Console.WriteLine("Record count: " + searchResponse.HitListInfo.RecordCount);
            Console.WriteLine("Press any key to exit...");

            Console.ReadKey(true);
        }

        static void DoLogin() {
            string userIdentifier = System.Configuration.ConfigurationManager.AppSettings["LogInSimulationMode"];
            bool isTicket = false;
            COEPrincipal.Login(userIdentifier, isTicket);
        }
    }
}
