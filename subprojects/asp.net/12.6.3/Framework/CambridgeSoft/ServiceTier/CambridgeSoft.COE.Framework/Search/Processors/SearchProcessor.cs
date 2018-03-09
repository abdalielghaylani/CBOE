using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors

{
	/// <summary>
	/// Abstract super class for the search processors.
	/// </summary>
	public abstract class SearchProcessor
	{
		#region Variables
		/// <summary>
		/// Indicates the chemical subsystem that resolves chemical queries. Possible values: CSCartridge, MolServer
		/// </summary>
		protected ChemImplementations chemImpl;
        protected XmlNode xmlNode;
        
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");
        protected SearchCriteria.SearchCriteriaItem _searchCriteriaItem;
		#endregion

        #region Constructors
        public SearchProcessor(SearchCriteria.SearchCriteriaItem item)
        {
            _searchCriteriaItem = item;
        }

        public SearchProcessor(XmlNode xmlNode)
        {
            this.xmlNode = xmlNode;
        }
        #endregion

        #region Methods
        /// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this processor.
        /// This executes right before building a Query object.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public abstract void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview);
		
		/// <summary>
		/// Template Method Process - returns the modified xml to be inserted in searchCriteria.xml
        /// This executes right after building a Query object giving a chance to do modifications to it.
		/// </summary>
        /// <param name="item">The search criteria item</param>
        /// <param name="query">The Query object</param>
		/// <returns>a new searchCriteriaItem that is used to rebuild the searchCriteria xml</returns>
		public abstract void Process(Query query);

		/// <summary>
		/// Template Method PostProcess - Performs the required steps for cleaning up the command execution.
        /// This is executed at the end of the GetHitlist method and should be used for cleanup of the changed made by the other two methods.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public abstract void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL);
		#endregion
	}
}
