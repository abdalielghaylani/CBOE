using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEReportingService.Builders;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using DevExpress.XtraReports.UI;
using System;

namespace CambridgeSoft.COE.Framework.COEReportingService
{
    /// <summary>
    /// Main inteface of COE Reporting Service.
    /// </summary>
    public class COEReporting
    {
        #region Methods
        #region Inteface Methods
        #region Report Listing
        public static List<ReportBuilderMeta> GetReportByUser(string userId)
        {
            return GetReportBuilders(userId, null, -1);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders
        /// </summary>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetReportBuilders()
        {
            return GetReportBuilders(null, null, -1);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that match the parameters provided.
        /// </summary>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetReportBuilders(int dataviewId)
        {
            return GetReportBuilders(null, null, dataviewId);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that match the parameters provided.
        /// </summary>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetReportBuilders(string applicationName, int dataviewId)
        {
            return GetReportBuilders(null, applicationName, dataviewId);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that match the parameters provided.
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetReportBuilders(string user, string applicationName, int dataviewId)
        {
            
            return GetReportBuilders(user, applicationName, dataviewId, null);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that match the parameters provided.
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <param name="type">The type of the report. Ignored if null</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetReportBuilders(string user, string applicationName, int dataviewId, COEReportType? type)
        {
            return GetReportBuilders(user, applicationName, dataviewId, type, null);
        }
        
        /// <summary>
        /// Obtains a list containing information of available report builders that match the parameters provided.
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <param name="type">The type of the report. Ignored if null</param>
        /// <param name="category">The category related to the report. Ignored if null.</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetReportBuilders(string user, string applicationName, int dataviewId, COEReportType? type, string category)
        {
            List<ReportBuilderMeta> reportBuildersMeta = new List<ReportBuilderMeta>();

            reportBuildersMeta.AddRange(GetDatabaseReportBuilders(user, applicationName, dataviewId, type, category));
            reportBuildersMeta.AddRange(GetTemplateReportBuilders(user, applicationName, dataviewId, type, category));

            return reportBuildersMeta;
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that are stored on database and match the provided parameters
        /// </summary>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetDatabaseReportBuilders(string applicationName, int dataviewId)
        {
            return GetDatabaseReportBuilders(null, applicationName, dataviewId);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that are stored on database and match the provided parameters
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetDatabaseReportBuilders(string user, string applicationName, int dataviewId)
        {
            return GetDatabaseReportBuilders(user, applicationName, dataviewId, null);
        }
        
        /// <summary>
        /// Obtains a list containing information of available report builders that are stored on database and match the provided parameters
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <param name="type">The type of the report. Ignored if null</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetDatabaseReportBuilders(string user, string applicationName, int dataviewId, COEReportType? type)
        {
            return GetDatabaseReportBuilders(user, applicationName, dataviewId, type, null);
        }

        /// <summary>
        /// Obtains a list containing information of available report builders that are stored on database and match the provided parameters
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <param name="type">The type of the report. Ignored if null</param>
        /// <param name="category">The category related to the report. Ignored if null.</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetDatabaseReportBuilders(string user, string applicationName, int dataviewId, COEReportType? type, string category)
        {
            List<ReportBuilderMeta> reportbuildersMeta = new List<ReportBuilderMeta>();

            COEReportBOList availableDBReportBuilders = COEReportBOList.GetCOEReportBOList(
                                    user, /*(string.IsNullOrEmpty(user) ? COEUser.Name : user),*/
                                    null/*ConfigurationUtilities.GetApplicationData(COEAppName.Get()).Database*/,
                                    (string.IsNullOrEmpty(applicationName) ? COEAppName.Get() : applicationName),
                                    true,
                                    dataviewId,
                                    type,
                                    category);

            foreach (COEReportBO currentReportBuilder in availableDBReportBuilders)
            {
                ReportBuilderMeta currentBuilderMeta = new ReportBuilderMeta(currentReportBuilder.ID, currentReportBuilder.Name, currentReportBuilder.Description, typeof(COEReportBO).FullName);
                currentBuilderMeta.Class = typeof(DataBaseReportBuilder).AssemblyQualifiedName;
                reportbuildersMeta.Add(currentBuilderMeta);
            }

            return reportbuildersMeta;
        }

        /// <summary>
        /// Obtains a list containing information of configured report template builders that match the provided parameters
        /// </summary>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetTemplateReportBuilders(string applicationName, int dataviewId)
        {
            return GetTemplateReportBuilders(null, applicationName, dataviewId);
        }

        /// <summary>
        /// Obtains a list containing information of template builders that match the provided parameters
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetTemplateReportBuilders(string user, string applicationName, int dataviewId)
        {
            return GetTemplateReportBuilders(user, applicationName, dataviewId, null);
        }

        /// <summary>
        /// Obtains a list containing information of template builders that match the provided parameters
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <param name="type">The type of the report. Ignored if null</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetTemplateReportBuilders(string user, string applicationName, int dataviewId, COEReportType? type)
        {
            return GetTemplateReportBuilders(user, applicationName, dataviewId, type, null);
        }

        /// <summary>
        /// Obtains a list containing information of template builders that match the provided parameters
        /// </summary>
        /// <param name="user">The name of the user related to the report. Ignored if null</param>
        /// <param name="applicationName">The name of the aplication related with the report. Ignored if null</param>
        /// <param name="dataviewId">The id of the dataview related with the report. Ignored if less than zero</param>
        /// <param name="type">The type of the report. Ignored if null</param>
        /// <param name="category">The category related to the report. Ignored if null.</param>
        /// <returns></returns>
        public static List<ReportBuilderMeta> GetTemplateReportBuilders(string user, string applicationName, int dataviewId, COEReportType? type, string category)
        {
            List<ReportBuilderMeta> templatesInfo = new List<ReportBuilderMeta>();

            ApplicationData applicationData = ConfigurationUtilities.GetApplicationData(COEAppName.Get());

            foreach (CambridgeSoft.COE.Framework.Common.COEReportingConfiguration currentTemplateBuilderInfo in applicationData.Reporting)
                templatesInfo.Add(new ReportBuilderMeta(templatesInfo.Count, currentTemplateBuilderInfo.Name, currentTemplateBuilderInfo.Description, currentTemplateBuilderInfo.Class, true));

            return templatesInfo;
        }
        #endregion

        #region Report Generation
        /// <summary>
        /// Obtains a report instance from a report builder derived from the information provided
        /// </summary>
        /// <param name="reportBuilderMeta">Information about the builder to use for generating the report instance</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(ReportBuilderMeta reportBuilderMeta)
        {
            ReportBuilderBase reportBuilder = ReportBuilderFactory.GetReportBuilder(reportBuilderMeta);

            return GetReportInstance(reportBuilder);
        }
        
        /// <summary>
        /// Obtains a report instance from a report builder derived from the information provided, and binds it to the data specified by hitlistId.
        /// </summary>
        /// <param name="reportBuilderMeta">Information about the builder to use for generating the report instance</param>
        /// <param name="hitlistId">Id of a hitlist generated by interacting with search service</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(ReportBuilderMeta reportBuilderMeta, int hitlistId, PagingInfo pagingInfo)
        {
            ReportBuilderBase reportBuilder = ReportBuilderFactory.GetReportBuilder(reportBuilderMeta);

            return GetReportInstance(reportBuilder, hitlistId, pagingInfo);
        }
        
        /// <summary>
        /// Obtains a report instance from a report builder derived from the information provided, and binds it to the dataSource specified.
        /// </summary>
        /// <param name="reportBuilderMeta">Information about the builder to use for generating the report instance</param>
        /// <param name="dataSource">Datasource for the report instance. Note that the identifiers on the datasource (table names, column names, property names, etc.) should match the report binding expressions and datamembers.</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(ReportBuilderMeta reportBuilderMeta, object dataSource)
        {
            ReportBuilderBase reportBuilder = ReportBuilderFactory.GetReportBuilder(reportBuilderMeta);

            return GetReportInstance(reportBuilder, dataSource);
        }

        /// <summary>
        /// Obtains a report instance from the specified report builder.
        /// </summary>
        /// <param name="reportBuilder">The report builder from which the report (definition) is obtained</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(ReportBuilderBase reportBuilder)
        {
            COEReport reportDefinition = NormalizeReport(reportBuilder.GetReport());
            return BuildReportInstance(reportDefinition);
        }

        /// <summary>
        /// Obtains a report instance from the specified report builder and binds it to the data specified by hitlistId.
        /// </summary>
        /// <param name="reportBuilder">The report builder from which the report (definition) is obtained</param>
        /// <param name="hitlistId">Id of a hitlist generated by interacting with search service</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(ReportBuilderBase reportBuilder, int hitlistId, PagingInfo pagingInfo)
        {
            COEReport reportDefinition = NormalizeReport(reportBuilder.GetReport());
            return BuildReportInstance(reportDefinition, hitlistId, pagingInfo);
        }
        
        /// <summary>
        /// Obtains a report instance from the specified report builder and binds it to the data specified by dataSource.
        /// </summary>
        /// <param name="reportBuilder">The report builder from which the report (definition) is obtained</param>
        /// <param name="dataSource">Datasource for the report instance. Note that the identifiers on the datasource (table names, column names, property names, etc.) should match the report binding expressions and datamembers.</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(ReportBuilderBase reportBuilder, object dataSource)
        {
            COEReport reportDefinition = NormalizeReport(reportBuilder.GetReport());
            return BuildReportInstance(reportDefinition, dataSource);
        }

        /// <summary>
        /// Retrieves a report instance by using the report (definition) stored on database under the provided Id.
        /// </summary>
        /// <param name="reportId">Id of the report (definition) stored on database</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(int reportId)
        {
            ReportBuilderMeta reportBuilderInfo = new ReportBuilderMeta();
            reportBuilderInfo.Id = reportId;
            reportBuilderInfo.Class = typeof(DataBaseReportBuilder).AssemblyQualifiedName;

            return GetReportInstance(reportBuilderInfo);
        }

        /// <summary>
        /// Retrieves a report instance by using the report (definition) stored on database under the provided Id, and binds it to the data specified by the hitlistId
        /// </summary>
        /// <param name="reportId">Id of the report (definition) stored on database</param>
        /// <param name="hitlistId">Id of a hitlist generated by interacting with search service</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(int reportId, int hitlistId)
        {
            ReportBuilderMeta reportBuilderMeta = new ReportBuilderMeta();
            reportBuilderMeta.Id = reportId;
            reportBuilderMeta.Class = typeof(DataBaseReportBuilder).AssemblyQualifiedName;

            return GetReportInstance(reportBuilderMeta, hitlistId);
        }

        /// <summary>
        /// Retrieves a report instance by using the report (definition) stored on database under the provided Id and binds it to the provided dataSource
        /// </summary>
        /// <param name="reportId">Id of the report (definition) stored on database</param>
        /// <param name="dataSource">Datasource for the report instance. Note that the identifiers on the datasource (table names, column names, property names, etc.) should match the report binding expressions and datamembers.</param>
        /// <returns>A report instance (COEXtraReport)</returns>
        public static XtraReport GetReportInstance(int reportId, object dataSource)
        {
            ReportBuilderMeta reportBuilderInfo = new ReportBuilderMeta();
            reportBuilderInfo.Id = reportId;
            reportBuilderInfo.Class = typeof(DataBaseReportBuilder).AssemblyQualifiedName;

            return GetReportInstance(reportBuilderInfo, dataSource);
        }
        
        /// <summary>
        /// Retrieves a report (definition) through a report builder instanciated using the information provided.
        /// </summary>
        /// <param name="reportBuilderMeta">Information about the builder to use for generating the report instance</param>
        /// <returns>A report (definition)</returns>
        public static COEReport GetReportDefinition(ReportBuilderMeta reportBuilderMeta)
        {
            return GetReportDefinition(ReportBuilderFactory.GetReportBuilder(reportBuilderMeta));
        }

        /// <summary>
        /// Retrieves a report (definition) through the provided report builder.
        /// </summary>
        /// <param name="reportBuilder">The report builder to use</param>
        /// <returns>A report (definition)</returns>
        public static COEReport GetReportDefinition(ReportBuilderBase reportBuilder)
        {
            return NormalizeReport(reportBuilder.GetReport());
        }

        /// <summary>
        /// Removes the specified report definition from database
        /// </summary>
        /// <param name="reportId"></param>
        public static void DeleteReport(int reportId)
        {
            COEReportBO.Delete(reportId);
        }
        #endregion

        static int _temporaryTemplateId = 1000;
        static public int StoreTemporaryReport(COEReport template)
        {
            COEReportBO templateBO = COEReportBO.Get(_temporaryTemplateId);

            templateBO.ReportDefinition = template;
            templateBO.Save();

            return templateBO.ID;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Utility method for serializing an XtraReport.ReportLayout to a string.
        /// </summary>
        /// <param name="reportInstance"></param>
        /// <returns></returns>
        public static string GetReportLayout(XtraReport reportInstance)
        {
            MemoryStream outputStream = new MemoryStream();
            reportInstance.SaveLayout(outputStream);

            return UTF8Encoding.UTF8.GetString(outputStream.GetBuffer());
        }

        /// <summary>
        /// Utility Method for de-serializing an XtraReport.ReportLayout from a string.
        /// </summary>
        /// <param name="reportLayout"></param>
        public static XtraReport LoadReport(string reportLayout)
        {
            return COEXtraReport.FromStream(new MemoryStream(UTF8Encoding.UTF8.GetBytes(reportLayout)), true);
        }


        /// <summary>
        /// Normalizes a report instance. After this process, the dataviewId should be valid (only this for now).
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static COEReport NormalizeReport(COEReport report)
        {
            if (report.Dataview == null)
            {
                if (report.DataViewId <= 0 && report.HitListId > 0)
                    report.DataViewId = COEHitListBO.Get(HitListType.TEMP, report.HitListId).DataViewID;

                if(report.DataViewId > 0)
                    report.Dataview = COEDataViewBO.Get(report.DataViewId).COEDataView;

                if (report.Dataview != null)
                {
                    string errorMessage = IsValidDataView(report.Dataview);
                    if (!string.IsNullOrEmpty(errorMessage))
                        throw new Exception(string.Format("Invalid DataView {0}:\n{1}", report.DataViewId, errorMessage));
                }
            }

            if (report.Dataview != null)
                if (report.ResultsCriteria == null || report.ResultsCriteria.Tables.Count == 0)
                    report.ResultsCriteria = COEReporting.ExtractResultsCriteria(report.Dataview);

            return report;
        }


        /// <summary>
        /// Returns a resultsCriteria that contains all the dataview tables and fields.
        /// </summary>
        /// <param name="dataview"></param>
        /// <returns></returns>
        public static ResultsCriteria ExtractResultsCriteria(COEDataView dataview)
        {
            CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView metaDataDataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView(dataview.ToString());

            ResultsCriteria resultsCriteria = new ResultsCriteria();

            foreach (COEDataView.DataViewTable currentTable in dataview.Tables)
            {
                if (currentTable.Id == dataview.Basetable || metaDataDataView.GetRelations(dataview.Basetable, currentTable.Id).Count == 1)
                {
                    ResultsCriteria.ResultsCriteriaTable resultsCriteriaTable = new ResultsCriteria.ResultsCriteriaTable(currentTable.Id);

                    foreach (COEDataView.Field currentField in currentTable.Fields)
                    {
                        ResultsCriteria.Field resultsCriteriaField = new ResultsCriteria.Field(currentField.Id);
                        resultsCriteriaField.Alias = currentField.Alias;

                        resultsCriteriaTable.Criterias.Add(resultsCriteriaField);

                        if (currentField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                        {
                            ResultsCriteria.Formula resultsCriteriaFormula = new ResultsCriteria.Formula();
                            resultsCriteriaFormula.Id = currentField.Id;
                            resultsCriteriaFormula.Alias = currentField.Alias + "_FORMULA";
                            resultsCriteriaTable.Criterias.Add(resultsCriteriaFormula);

                            ResultsCriteria.MolWeight resultsCriteriaMolweight = new ResultsCriteria.MolWeight();
                            resultsCriteriaMolweight.Id = currentField.Id;
                            resultsCriteriaMolweight.Alias = currentField.Alias + "_MOLWEIGHT";
                            resultsCriteriaTable.Criterias.Add(resultsCriteriaMolweight);
                        }
                    }
                    resultsCriteria.Tables.Add(resultsCriteriaTable);
                }
            }
            return resultsCriteria;
        }

        private static string IsValidDataView(COEDataView coeDataView)
        {
            StringBuilder builder = new StringBuilder();

            try
            {
                foreach (COEDataView.Relationship currentRelationship in coeDataView.Relationships)
                {
                    COEDataView.DataViewTable table = coeDataView.Tables.getById(currentRelationship.Parent);
                    if (table == null)
                        builder.AppendFormat("TableId {0} not found\n", currentRelationship.Parent);
                    else if (table.Fields.getById(currentRelationship.ParentKey) == null)
                        builder.AppendFormat("Field {0} not found in table {1}-'{2}'\n", currentRelationship.ParentKey, table.Id, table.Alias);

                    table = coeDataView.Tables.getById(currentRelationship.Child);
                    if (table == null)
                        builder.AppendFormat("TableId {0} not found\n", currentRelationship.Child);
                    else if (table.Fields.getById(currentRelationship.ChildKey) == null)
                        builder.AppendFormat("Field {0} not found in table {1}-'{2}'\n", currentRelationship.ChildKey, table.Id, table.Alias);
                }
            }
            catch (Exception exception)
            {
                builder.AppendFormat(exception.Message);
            }

            return builder.ToString();
        }

        #endregion

        #region Private Methods
        private static XtraReport BuildReportInstance(COEReport report)
        {
            return BuildReportInstance(report, report.HitListId, null);
        }

        private static XtraReport BuildReportInstance(COEReport report, int hitlistId, PagingInfo pagingInfo)
        {
            return BuildReportInstance(report, DoSearch(hitlistId, report.Dataview, ConstrainResultsCriteria(report.ReportLayout, report.ResultsCriteria, report.Dataview), null, pagingInfo));
        }

        private static XtraReport BuildReportInstance(COEReport report, object dataSource)
        {
            XtraReport reportInstance = null;

            if (!string.IsNullOrEmpty(report.ReportLayout))
            {
                reportInstance = LoadReport(report.ReportLayout);

                // Coverity Fix CID - 11736
                if (reportInstance != null && dataSource != null)
                {
                    reportInstance.DataSource = dataSource;
                    ReBindDetailReports(report.ReportLayout, reportInstance.Bands);
                }
            }
            return reportInstance;
        }

        private static void ReBindDetailReports(string reportLayout, BandCollection bandCollection)
        {
            reportLayout = reportLayout.Replace(" ", "");
            foreach (Band currentBand in bandCollection)
            {
                if (currentBand is DetailReportBand/* && string.IsNullOrEmpty(((DetailReportBand)currentBand).DataMember)*/)
                {
                    string dataMemberAssignmentLine = string.Format("{0}.DataMember=", currentBand.Name);

                    if (reportLayout.Contains(dataMemberAssignmentLine))
                    {
                        int startIndex = reportLayout.IndexOf(dataMemberAssignmentLine) + dataMemberAssignmentLine.Length + 1;


                        string dataMember = reportLayout.Substring(startIndex, reportLayout.IndexOf("\"", startIndex) - startIndex);
                        ((DetailReportBand)currentBand).DataMember = dataMember;
                    }

                    ReBindDetailReports(reportLayout, ((DetailReportBand)currentBand).Bands);
                }
            }
        }

        private static DataSet DoSearch(int hitListId, COEDataView dataview, ResultsCriteria resultsCriteria, SearchCriteria searchCriteria, PagingInfo pagingInfo)
        {
            if (dataview != null && resultsCriteria.Tables.Count > 0)
            {
                COESearch searchService = new COESearch();

                if (pagingInfo == null)
                {
                    pagingInfo = new PagingInfo();
                    pagingInfo.RecordCount = int.MaxValue;
                }

                if (hitListId > 0)
                    pagingInfo.HitListID = hitListId;

                if (searchCriteria == null || hitListId > 0)
                {
                    searchCriteria = new SearchCriteria();
                    return searchService.GetData(resultsCriteria, pagingInfo, dataview, bool.TrueString);
                }
                return searchService.DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataview, bool.TrueString).ResultsDataSet;
            }

            return null;
        }

        private static ResultsCriteria ConstrainResultsCriteria(string reportLayout, ResultsCriteria resultsCriteria, COEDataView dataView)
        {
            ResultsCriteria responseResultsCriteria = new ResultsCriteria();

            foreach (ResultsCriteria.ResultsCriteriaTable currentTable in resultsCriteria.Tables)
            {
                //TODO: Move table and relation name generation to COEDataview, so that we don't have to guess search service results dataset table names.
                string tableDataSetName = dataView.Tables.getById(currentTable.Id).Alias; // "Table_" + currentTable.Id;
                string tableName = dataView.Tables.getById(currentTable.Id).Alias;

                if (reportLayout.Contains(tableDataSetName) || reportLayout.Contains(tableName))
                {
                    ResultsCriteria.ResultsCriteriaTable newTable = new ResultsCriteria.ResultsCriteriaTable(currentTable.Id);

                    foreach (ResultsCriteria.IResultsCriteriaBase currentCriterium in currentTable.Criterias)
                    {
                        string criteriumName = currentCriterium.Alias;

                        if (reportLayout.Contains(criteriumName))
                        {
                            newTable.Criterias.Add(currentCriterium);
                        }
                    }

                    if (newTable.Criterias.Count > 0)
                        responseResultsCriteria.Tables.Add(newTable);
                }
            }

            //There is no need to check if resultsCriteria contains required relationship tables and fields, as search manager does this.

            return responseResultsCriteria;
        }

        #endregion
        #endregion
    }

    public enum COEReportType
    { 
        List,
        Label,
    }
}
