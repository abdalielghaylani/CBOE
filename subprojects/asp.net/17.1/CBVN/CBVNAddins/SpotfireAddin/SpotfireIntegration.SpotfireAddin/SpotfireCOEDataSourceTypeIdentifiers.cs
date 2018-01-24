using Spotfire.Dxp.Application.Extension;

using SpotfireIntegration.SpotfireAddin.Properties;

namespace SpotfireIntegration.SpotfireAddin
{
    /// <summary>Type identifiers for custom file data sources. The type identifiers are both used
    /// to uniquely identify a file data source and to provide texts that are shown in user interfaces 
    /// where one can select file data sources.
    /// </summary>
    public sealed class SpotfireCOEDataSourceTypeIdentifiers : CustomTypeIdentifiers
    {
        #region Constants and Fields

        public static readonly CustomTypeIdentifier SpotfireCOETableDataSource =
            CreateTypeIdentifier(
                Resources.COETableDataSource_Name,
                Resources.COETableDataSource_DisplayName, 
                Resources.COETableDataSource_Description);

        public static readonly CustomTypeIdentifier COEMetadataTransformation =
            CreateTypeIdentifier(
                Resources.COEMetadataTransformation_Name,
                Resources.COEMetadataTransformation_DisplayName,
                Resources.COEMetadataTransformation_Description);

        public static readonly CustomTypeIdentifier COEColumnID_Property =
            CreateTypeIdentifier(
                Resources.COEColumnID_PropertyName,
                Resources.COEColumnID_DisplayName,
                Resources.COEColumnID_Description);

        public static readonly CustomTypeIdentifier COEDataView_Property =
            CreateTypeIdentifier(
            Resources.COEDataView_PropertyName,
            Resources.COEDataView_DisplayName,
            Resources.COEDataView_Description);

        public static readonly CustomTypeIdentifier COEDataViewID_Property =
            CreateTypeIdentifier(
            Resources.COEDataViewID_PropertyName,
            Resources.COEDataViewID_DisplayName,
            Resources.COEDataViewID_Description);

        public static readonly CustomTypeIdentifier COEHitListID_Property =
            CreateTypeIdentifier(
            Resources.COEHitListID_PropertyName,
            Resources.COEHitListID_DisplayName,
            Resources.COEHitListID_Description);

        public static readonly CustomTypeIdentifier COEHitListType_Property =
            CreateTypeIdentifier(
            Resources.COEHitListType_PropertyName,
            Resources.COEHitListType_DisplayName,
            Resources.COEHitListType_Description);

        public static readonly CustomTypeIdentifier COEResultsCriteria_Property =
            CreateTypeIdentifier(
                Resources.COEResultsCriteria_PropertyName,
                Resources.COEResultsCriteria_DisplayName,
                Resources.COEResultsCriteria_Description);

        public static readonly CustomTypeIdentifier COETableID_Property =
            CreateTypeIdentifier(
            Resources.COETableID_PropertyName,
            Resources.COETableID_DisplayName,
            Resources.COETableID_Description);

        public static readonly CustomTypeIdentifier COETablePK_Property =
            CreateTypeIdentifier(
            Resources.COETablePK_PropertyName,
            Resources.COETablePK_DisplayName,
            Resources.COETablePK_Description);

        /// <summary>
        /// Property for search criteria
        /// </summary>
        public static readonly CustomTypeIdentifier COESearchCriteria_Property =
           CreateTypeIdentifier(
               Resources.COESearchCriteria_PropertyName,
               Resources.COESearchCriteria_DisplayName,
               Resources.COESearchCriteria_Description);

        /// <summary>
        /// Property for search criteria fields order
        /// </summary>
        public static readonly CustomTypeIdentifier COESearchCriteriaFieldOrder_Property =
           CreateTypeIdentifier(
               Resources.COESearchCriteriaFieldOrder_PropertyName,
               Resources.COESearchCriteriaFieldOrder_DisplayName,
               Resources.COESearchCriteriaFieldOrder_Description);

        /// <summary>
        /// Property for filter child hits based on query
        /// </summary>
        public static readonly CustomTypeIdentifier FilterChildHits_Property =
           CreateTypeIdentifier(
               Resources.FilterChildHits_PropertyName,
               Resources.FilterChildHits_DisplayName,
               Resources.FilterChildHits_Description);


        /// <summary>
        /// Property for connection information details
        /// </summary>
        public static readonly CustomTypeIdentifier COEConnectionInfo_Property =
           CreateTypeIdentifier(
               Resources.COEConnectionInfo_PropertyName,
               Resources.COEConnectionInfo_DisplayName,
               Resources.COEConnectionInfo_Description);

        #endregion
    }
}