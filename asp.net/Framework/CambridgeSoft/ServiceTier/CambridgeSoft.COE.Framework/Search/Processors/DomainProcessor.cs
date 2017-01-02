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
	/// A dummy processor for Criteria that don't need specific work. This class follows the template pattern along with search service GetHitList Method
	/// </summary>
	internal class DomainProcessor : SearchProcessor
	{
		#region Variables
        private int _hitListID;
        private int _hitListTableID;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of Generic Processor with an xmlNode of the SearchCriteria XML.
		/// </summary>
		/// <param name="xmlNode">The node of a SearchCriteriaItem.</param>
		internal DomainProcessor(XmlNode xmlNode) : base(xmlNode) { }

        /// <summary>
		/// Creates a new instance of Generic Processor with a given SearchCriteria Item
		/// </summary>
		/// <param name="item">The SearchCriteriaItem that is to be handled by this Processor</param>
        internal DomainProcessor(SearchCriteria.SearchCriteriaItem item, int hitListID, int hitListTableID)
            : base(item)
        {
            _hitListID = hitListID;
            _hitListTableID = hitListTableID;
        }
		#endregion

		#region Methods
		/// <summary>
        /// Template method Preprocess - performs the required setup steps for executing this command: add a numerical criteria that specifies teh hitlist table, hitlist id field and hitlistid to search over.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview)
        {
            //here we get information for the DomainCriteria item 
            int currentSearchCriteriaID = _searchCriteriaItem.ID;  //id of the searchcriteria item
            SearchCriteria.DomainCriteria domainCriteria = (SearchCriteria.DomainCriteria) _searchCriteriaItem.Criterium;
            string domainCriteriaInnerText = domainCriteria.InnerText; //hit list id that defines the domain

            //here we are just going to add a  numerical search criteria for the domain criteria
            SearchCriteria.NumericalCriteria newNumericalCriteria = new SearchCriteria.NumericalCriteria();
            newNumericalCriteria.InnerText = domainCriteriaInnerText;
            newNumericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;



            //create a new searchCriteria item witht he field and table information for the hitlist table
            _searchCriteriaItem.FieldId = _hitListID;
            _searchCriteriaItem.TableId = _hitListTableID;
            _searchCriteriaItem.Criterium = newNumericalCriteria;
            _searchCriteriaItem.ID = currentSearchCriteriaID;//set the the same id that the originating domaincriteria item had 
		}

		/// <summary>
		/// No modification to query is needed
		/// </summary>
		/// <returns></returns>
        public override void Process(Query query) { }

		/// <summary>
		/// Template method PostProcess - Performs the required steps for cleaning up the command execution - Nothing in this case.
		/// </summary>
		/// <param name="searchDAL"></param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) { }
		#endregion
	}
}
