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
	/// This class is responsible for handling MolWeight queries. This class follows the template pattern along with search service GetHitList Method
	/// </summary>
	internal class MolWeightProcessor : SearchProcessor
	{
		#region Variables
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of MolWeight Processor with a given SearchCriteria Item xml representation
		/// </summary>
		/// <param name="xmlNode">The SearchCriteriaItem xml represntation that is to be handled by this Processor</param>
        internal MolWeightProcessor(XmlNode xmlNode) : base(xmlNode) { }

        public MolWeightProcessor(SearchCriteria.SearchCriteriaItem item) : base(item) { }
		#endregion

		#region Methods
		/// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this processor.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview) { }

		/// <summary>
        /// Template Method Process - modify xml to be inserted in searchCriteria.xml
		/// </summary>
		/// <param name="item"></param>
        public override void Process(Query query) { }

		/// <summary>
		/// Template Method PostProcess - Performs the required steps for cleaning up the command execution.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) { }
		#endregion
	}
}
