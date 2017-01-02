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
	internal class GenericProcessor : SearchProcessor
	{
		#region Variables
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of Generic Processor with an xmlNode of the SearchCriteria XML.
		/// </summary>
		/// <param name="xmlNode">The node of a SearchCriteriaItem.</param>
        internal GenericProcessor(XmlNode xmlNode) : base(xmlNode) { }

        public GenericProcessor(SearchCriteria.SearchCriteriaItem item) : base(item) { }
		#endregion

		#region Methods
		/// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this command: Nothing in this case.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview) { }

		/// <summary>
        /// Template Method Process - in this case don't modify the original resultsCriteria item
		/// </summary>
		/// <param name="item"></param>
        public override void Process(Query query) { }

		/// <summary>
		/// Template method PostProcess - Performs the required steps for cleaning up the command execution - Nothing in this case.
		/// </summary>
		/// <param name="searchDAL"></param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) { }
		#endregion
	}
}
