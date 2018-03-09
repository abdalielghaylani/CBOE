using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COESearchService;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.COEExportService
{

    #region COEFormInterchange
    /// <summary>
    /// Class for retreiving the form data from client applications to xml spreadsheet format
    /// </summary>
    public class COEFormInterchange
    {

        public enum formatterType
        {
            CBVExcel
        }

        public enum criteriaFields
        {
            IDENTITY,
            EXACT,
            SUBSTRUCTURE,
            SIMILARITY,
            FULL,
            MOLWEIGHT,
            FORMULA
        }
        
        public const string key = "ChemBioVizExcelAddIn";
        public const string value = "COECBVExcel";
        public const string separator = "--^@^--";
        public const string delimiter = "--~$~--";
        public const string fieldDelimiter = "--~#~--";
        public const string tier_2 = "2-Tier";
        public const string tier_3 = "3-Tier";
        public static string[] columnSeparator = { "--~$C@S$~--" };
      

        /// <summary>
        /// 
        /// </summary>
        public COEFormInterchange()
        {
        }

        public string GetForm(COEDataView dataView, SearchCriteria searchCriteria, ResultsCriteria resultCriteria, PagingInfo pageInfo, ServerInfo serverInfo, string userName, string formName, string exportType)
        {
            try
            {
                GetDataCommand result = DataPortal.Execute<GetDataCommand>(new GetDataCommand(dataView, searchCriteria, resultCriteria, pageInfo, serverInfo, userName, formName, exportType));
                return (string)result.FormattedResults;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        

        [Serializable]
        private class GetDataCommand : CommandBase
        {   
            

            private ResultsCriteria _resultsCriteria;            
            private PagingInfo _pagingInfo;
            private COEDataView _dataView;
            private String _formattedResults;
            private string _exportType;    
            private string _formName;
            private string _userName;
            private SearchCriteria _searchCriteria;
            private ServerInfo _serverInfo;


            public GetDataCommand(COEDataView dataView, SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pageInfo, ServerInfo serverInfo, string userName, string formName, string exportType)
            {
                _resultsCriteria = resultsCriteria;
                _dataView = dataView;
                _exportType = exportType;
                _pagingInfo = pageInfo;
                _formName = formName;
                _userName = userName;
                _searchCriteria = searchCriteria;
                _serverInfo = serverInfo;
            }

            public string FormattedResults
            {
                get { return _formattedResults; }
                set { _formattedResults = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //be sure that the export type is supported:
                    if (ConfigurationUtilities.ExportTypeExists(_exportType))
                    {                        
                        //get Formatter section from COEFrameworkConfig  
                        ExportFormatterData exportFormatterData = ConfigurationUtilities.GetExportFormatterData(_exportType);


                        //load formatter using factory methods
                        FormatterLoader formatterLoader = new FormatterLoader();
                        IFormInterchangeFormatter formInterchangeFormatter = formatterLoader.InstantiateFormInterchangeFormatter(exportFormatterData.FormatterAssemblyName, exportFormatterData.FormatterTypeName);

                                                
                        _formattedResults = formInterchangeFormatter.GetForm(_dataView, _searchCriteria, _resultsCriteria, _pagingInfo, _serverInfo, _userName, _formName);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
           

        }
    }

    #endregion COEFormInterchange

    #region ServerInfo Class

    // Class for Server Details
    [Serializable]
    public class ServerInfo
    {

        private string _serverName = string.Empty;
        private bool _is3TierServer = false;
        private bool _isSSL = false;

        public ServerInfo()
        {

        }

        public ServerInfo(string serverName, bool is3TierServer, bool isSSL)
        {
            _serverName = serverName;
            _is3TierServer = is3TierServer;
            _isSSL = isSSL;
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        public bool Is3TierServer
        {
            get { return _is3TierServer; }
            set { _is3TierServer = value; }
        }

        public bool IsSSL
        {
            get { return _isSSL; }
            set { _isSSL = value; }
        }


    }

    #endregion ServerInfo Class
}
