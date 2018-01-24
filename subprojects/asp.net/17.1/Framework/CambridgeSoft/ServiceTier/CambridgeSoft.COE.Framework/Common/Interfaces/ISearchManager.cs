using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;



namespace CambridgeSoft.COE.Framework.Services.Search
{
    /// <summary>
    /// Search Service public Interface
    /// </summary>
	public interface ISearchManager
    {
        /// <summary>
        /// General purpose search method
        /// </summary>
        /// <param name="securityInfo">object cantaining user credentials</param>
        /// <param name="appName">name of application</param>
        /// <param name="searchCriteria">object containing fields and criteria for search</param>
        /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
        /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
		/// <param name="dataView">object containing database structure view required for search</param>
        /// <returns>SearchResponse object made up of hitlist object, paging info object and dataset object. One or more of these objects might be null
        /// depending on the initial search</returns>
        SearchResponse DoSearch(SecurityInfo securityInfo, string appName, SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, CambridgeSoft.COE.Framework.Common.COEDataView dataView);
        
        /// <summary>
        /// Search method for return dataset from preexisting search
        /// </summary>
        /// <param name="securityInfo">object cantaining user credentials</param>
        /// <param name="appName">name of application</param>
        /// <param name="resultsCriteria">object containing fields and criteria for returning results dataset</param>
        /// <param name="pagingInfo">object containing paging info for getting results from a cached search</param>
		/// <param name="dataView">object containing database structure view required for search</param>
        /// <returns>Dataset containing results</returns>
		DataSet GetData(SecurityInfo securityInfo, string appName, ResultsCriteria resultsCriteria, ref PagingInfo pagingInfo, CambridgeSoft.COE.Framework.Common.COEDataView dataView);

        /// <summary>
        /// Search method that performs a search and returns a hitlistInfo object
        /// </summary>
        /// <param name="securityInfo">object cantaining user credentials</param>
        /// <param name="appName">name of application</param>
		/// <param name="dataView">object containing database structure view required for search</param>
		/// <param name="searchCriteria">object containing fields and criteria for search</param>
		/// <returns>HitListInfo is an object containing hitlist info</returns>
		HitListInfo GetHitList(SecurityInfo securityInfo, string appName, SearchCriteria searchCriteria, CambridgeSoft.COE.Framework.Common.COEDataView dataView);
    }
}
