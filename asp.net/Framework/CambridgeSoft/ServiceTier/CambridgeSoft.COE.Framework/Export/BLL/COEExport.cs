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

    public class COEExport
    {

        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEExport");
        /// <summary>
        /// Service for converting dataset to alternate data format
        /// </summary>
        public COEExport()
        {
        }
        /// <summary>
        /// Main method for COEExport 
        /// </summary>
        /// <param name="resultsCriteria">Results Criteria indicating fields and their types</param>
        /// <param name="pagingInfo">the part of the dataset to return</param>
        /// <param name="dataView">Dataview object describing the tables, fields and relationships to used identified by ID</param>
        /// <param name="exportType">Type of export format type</param>
        /// <returns></returns>
        public string GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int dataViewID, string exportType)
        {
            try
            {
                COEDataView dataView = GetDataViewFromID(dataViewID);
                return GetData(resultsCriteria, pagingInfo, dataView, exportType, ConnStringType.OWNERPROXY);

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
        /// <param name="dataView">Dataview object describing the tables, fields and relationships to used.</param>
        /// <param name="exportType">Type of export format type</param>
        /// <returns></returns>
        public string GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string exportType)
        {
            try
            {
                return GetData(resultsCriteria, pagingInfo, dataView, exportType,ConnStringType.OWNERPROXY);
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private string GetData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string exportType, ConnStringType connStringType)
        {
            try
            {
                GetDataCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetDataCommand>(new GetDataCommand(resultsCriteria, pagingInfo, dataView, exportType, connStringType));
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

            private SearchManager _searchManager;
            private string _serviceName = "COEExport";

            //put private variables that will be sent by the factory method. don't need properties


            private ResultsCriteria _resultsCriteria;
            private SecurityInfo _securityInfo;
            private PagingInfo _pagingInfo;
            private COEDataView _dataView;
            private String _formattedResults;
            private string _exportType;
            private bool _searchResults;
            private ConnStringType _connStringType;

            public GetDataCommand(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, COEDataView dataView, string exportType, ConnStringType connStringType)
            {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _dataView = dataView;
                _exportType = exportType;
                _connStringType = connStringType;
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
                        FormatterLoader formatterLoader = new FormatterLoader();
                        IFormatter formatter = formatterLoader.InstantiateFormatter(exportFormatterData.FormatterAssemblyName, exportFormatterData.FormatterTypeName);
                        FormatterBase formatterModifier = (FormatterBase)formatter;
                        //modify resultsCriteria if necessary. The default method returns the originating resultscriteria, however
                        //the method in FormatterBase can be overriden in the formatter class.
                        _resultsCriteria = formatterModifier.ModifyResultsCriteria(_resultsCriteria, _dataView);

                        //get dataset from search service
                        DataSet _resultsDataSet = searchManager.GetData(_resultsCriteria, _pagingInfo, _dataView, _connStringType);

                        //call the formatter to reshape the dataset using the loaded formaater.
                         _formattedResults = formatter.FormatDataSet(_resultsDataSet, _dataView, _resultsCriteria);
                    }


                }
                catch (Exception ex)
                {

                    throw;
                }
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


            private string _serviceName = "COEExport";

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
