using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors

{
	/// <summary>
	/// Abstract super class for the search processors.
	/// </summary>
	public abstract class SelectProcessor
	{
		#region Variables
		/// <summary>
		/// Indicates the chemical subsystem that resolves chemical queries. Possible values: CSCartridge, MolServer
		/// </summary>
		protected ChemImplementations chemImpl;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");

		#endregion

		#region Methods
		/// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this processor.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public abstract void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL);
		
		/// <summary>
		/// Template Method Process - returns the modified xml to be inserted in searchCriteria.xml
		/// </summary>
		/// <returns>a new searchCriteriaItem that is used to rebuild the searchCriteria xml</returns>
		public abstract void Process(ResultsCriteria.IResultsCriteriaBase item);

		/// <summary>
		/// Template Method PostProcess - Performs the required steps for cleaning up the command execution.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public abstract void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL);
		#endregion
	}
}
