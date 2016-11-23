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
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace CambridgeSoft.COE.Framework.COEExportService
{

    public class COEAdvancedExport
    {

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEAdvancedExport");
        /// <summary>
        /// Service for converting dataset to alternate data format
        /// </summary>
        public COEAdvancedExport()
        {
        }

        /// <summary>
        /// Main method for COEExport 
        /// </summary>
        /// <param name="resultsCriteria">Results Criteria indicating fields and their types</param>
        /// <param name="pagingInfo">the part of the dataset to return</param>
        /// <param name="dataViewId">Dataview object describing the tables, fields and relationships to used identified by ID</param>
        /// <param name="exportType">Type of export format type</param>
        /// <param name="pageInfo">The paging information</param>
        /// <param name="userName">The user name</param>
        /// <param name="resultCriteria">The result list</param>
        /// <param name="SARSheetName">Name of the sheet</param>
        /// <param name="searchCriteriaItem">The filters</param>
        /// <returns></returns>
        public string GetData(int dataViewId, SearchCriteria searchCriteriaItem, ResultsCriteria resultCriteria, PagingInfo pageInfo, string userName, string SARSheetName, string exportType)
        {
            try
            {
                COEDataViewBO dvBO = COEDataViewBO.Get(dataViewId);

                return GetData(dvBO.COEDataView, searchCriteriaItem, resultCriteria, pageInfo, userName, SARSheetName, exportType, ConnStringType.OWNERPROXY);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Main method for COEExport 
        /// </summary>
        /// <param name="resultsCriteria">Results Criteria indicating fields and their types</param>
        /// <param name="pagingInfo">the part of the dataset to return</param>
        /// <param name="dataView">Dataview object describing the tables, fields and relationships to used identified by ID</param>
        /// <param name="exportType">Type of export format type</param>
        /// <returns></returns>
        public string GetData(COEDataView dataView, SearchCriteria searchCriteriaItem, ResultsCriteria resultCriteria, PagingInfo pageInfo, string userName, string SARSheetName, string exportType)
        {
            try
            {
                return GetData(dataView, searchCriteriaItem, resultCriteria, pageInfo, userName, SARSheetName, exportType, ConnStringType.OWNERPROXY);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string GetData(COEDataView dataView, SearchCriteria searchCriteriaItem, ResultsCriteria resultCriteria, PagingInfo pageInfo, string userName, string SARSheetName, string exportType, ConnStringType connStringType)
        {
            try
            {
                GetDataCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetDataCommand>(new GetDataCommand(dataView, searchCriteriaItem, resultCriteria, pageInfo, userName, SARSheetName, exportType, connStringType));
                return (string)result.FormattedResults;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //

        [Serializable]
        private class GetDataCommand : CommandBase
        {

            private SearchManager _searchManager;
            private string _serviceName = "COEAdvancedExport";

            //put private variables that will be sent by the factory method. don't need properties


            private ResultsCriteria _resultsCriteria;
            private SecurityInfo _securityInfo;
            private PagingInfo _pagingInfo;
            private COEDataView _dataView;
            private String _formattedResults;
            private string _exportType;
            private bool _searchResults;
            private ConnStringType _connStringType;

            //SG    

            private ResultPageInfo _resultPageInfo;
            private COESecurityService.COEIdentity _coeIdentity;
            private SearchInput _searchInput;
            private string _sarsheetName;
            private string _userName;
            private SearchCriteria _searchCriteriaItem;

            //SG
            public GetDataCommand(COEDataView dataView, SearchCriteria searchCriteriaItem, ResultsCriteria resultsCriteria, PagingInfo pageInfo, string userName, string sarsheetName, string exportType, ConnStringType connStringType)
            {
                _resultsCriteria = resultsCriteria;
                _dataView = dataView;
                _exportType = exportType;
                _connStringType = connStringType;
                _pagingInfo = pageInfo;
                _sarsheetName = sarsheetName;
                _userName = userName;
                _searchCriteriaItem = searchCriteriaItem;
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
                        //instantiate search service
                        SearchManager searchManager = new SearchManager();

                        //get Formatter section from COEFrameworkConfig  
                        ExportFormatterData exportFormatterData = ConfigurationUtilities.GetExportFormatterData(_exportType);


                        //load formatter using factory methods
                        //FormatterLoader formatterLoader = new FormatterLoader();
                        // IFormatterAdvanced formatterAdvanced = formatterLoader.InstantiateFormatterAdvanced(exportFormatterData.FormatterAssemblyName, exportFormatterData.FormatterTypeName);
                        //
                        FormatterLoader formatterLoader = new FormatterLoader();
                        IFormatterAdvanced formatterAdvanced = formatterLoader.InstantiateFormatterAdvanced(exportFormatterData.FormatterAssemblyName, exportFormatterData.FormatterTypeName);

                        //

                        FormatterBase formatterModifier = (FormatterBase)formatterAdvanced;
                        //modify resultsCriteria if necessary. The default method returns the originating resultscriteria, however
                        //the method in FormatterBase can be overriden in the formatter class.
                        _resultsCriteria = formatterModifier.ModifyResultsCriteria(_resultsCriteria, _dataView);

                        //get dataset from search service
                        DataSet _resultsDataSet = searchManager.GetData(_resultsCriteria, _pagingInfo, _dataView, _connStringType);


                        _formattedResults = formatterAdvanced.FormatData(_resultsDataSet, _dataView, _searchCriteriaItem, _resultsCriteria, _pagingInfo, _userName, _sarsheetName);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            

            /// <summary>
            /// Returns a list of available formmatters found in the COEFrameworkConfig file
            /// </summary>
            /// <returns>List of formats types as  string</returns>
            public List<string> GetFormatterTypesList()
            {
                try
                {
                    GetFormatterTypesListCommand result;
                    result = DataPortal.Execute<GetFormatterTypesListCommand>(new GetFormatterTypesListCommand());
                    return result.FormatTypesList;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }


            [Serializable]
            private class GetFormatterTypesListCommand : CommandBase
            {
                private string _serviceName = "COEExportXML";
                //put private variables that will be sent by the factory method. don't need properties
                List<string> _formatTypesList;

                public GetFormatterTypesListCommand()
                {

                }

                public List<string> FormatTypesList
                {
                    get { return _formatTypesList; }
                    set { _formatTypesList = value; }
                }


                protected override void DataPortal_Execute()
                {
                    try
                    {

                        _formatTypesList = ConfigurationUtilities.GetExportFormatterTypeNames();

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

            }

            #region dataView permissions

            /// <summary>
            /// gets the coedataview for a particular id  if the logged in user has permissions to the dataview
            /// </summary>
            /// <param name="dataViewID">valid id for stored dataview</param>
            /// <returns>coedataview</returns>
            private COEDataView GetDataViewFromID(int dataViewID)
            {

                COEDataViewBO coeDataViewBO = COEDataViewBO.Get(dataViewID);
                if (coeDataViewBO.COEDataView == null)
                {
                    throw new Exception(Resources.DataViewPermissions);
                }
                else
                {
                    return (COEDataView)coeDataViewBO.COEDataView;
                }
            }
            #endregion


        }
    }
}
