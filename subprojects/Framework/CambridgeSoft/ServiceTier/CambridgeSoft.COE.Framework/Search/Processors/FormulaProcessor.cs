using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors
{
	/// <summary>
	/// This processor is in charge of handling a Formula type queries. This class follows the template pattern along with search service GetHitList Method
	/// </summary>
	internal class FormulaProcessor : SearchProcessor
	{
		#region Variables
		[NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");
        #endregion

		#region Constructors
		/// <summary>
		/// Constructor that fills the object with the SearchCriteriaItem received in xml form
		/// </summary>
		/// <param name="xmlNode">An xml snippet that represents the SearchCriteriaItem to be handled by this processor</param>
        internal FormulaProcessor(XmlNode xmlNode) : base(xmlNode) { }

        public FormulaProcessor(SearchCriteria.SearchCriteriaItem item) : base(item) { }
		#endregion

		#region Methods
		/// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this processor.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview)
        {
			switch (this.chemImpl) {
				case ChemImplementations.CsCartridge:
					break;
				case ChemImplementations.MolServer:
					break;
			}
		}

		/// <summary>
        /// Template Method Process - modify xml to be inserted in searchCriteria.xml
		/// </summary>
		/// <param name="item"></param>
        public override void Process(Query query)
        {
			switch (this.chemImpl) {
				case ChemImplementations.CsCartridge:
					break;
				case ChemImplementations.MolServer:
					break;
			}
		}

		/// <summary>
		/// Template Method PostProcess - Performs the required steps for cleaning up the command execution.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL)
        {
			switch (this.chemImpl) {
				case ChemImplementations.CsCartridge:
					break;
				case ChemImplementations.MolServer:
					break;
			}
		}
		#endregion
	}
}
