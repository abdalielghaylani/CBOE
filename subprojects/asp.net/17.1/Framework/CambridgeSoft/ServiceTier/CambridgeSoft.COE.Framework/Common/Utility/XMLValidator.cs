using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace CambridgeSoft.COE.Framework.Common
{
	/// <summary>
	/// Provides methods to validate XMLs.searchResults
	/// </summary>
	public class XMLValidator
	{
		private bool isValid;
		private string errorMsg;
		private string path = Utilities.GetProjectBasePath("CambridgeSoft.COE.Framework") + @"\CambridgeSoft.COE.Framework\Common\MessagingTypes\xsd\";
		/// <summary>
		/// Validates the dataview xml.
		/// </summary>
		/// <param name="dataView">The xml to validate.</param>
		/// <returns>True if it is ok, false otherwise.</returns>
		public bool ValidateDataView(XmlDocument dataView) {
			return this.ValidateXmlWithSchema(dataView, "COE.COEDataView", path + "COEDataView.xsd");
		}

		/// <summary>
		/// Validates the searchCriteria xml.
		/// </summary>
		/// <param name="searchCriteria"></param>
		/// <returns>True if it is ok, false otherwise.</returns>
		public bool ValidateSearchCriteria(XmlDocument searchCriteria) {
			return this.ValidateXmlWithSchema(searchCriteria, "COE.SearchCriteria", path + "SearchCriteria.xsd");
		}

		/// <summary>
		/// Validates the searchResults xml.
		/// </summary>
		/// <param name="searchResults">The xml to validate.</param>
		/// <returns>True if it is ok, false otherwise.</returns>
		public bool ValidateSearchResults(XmlDocument searchResults) {
			return this.ValidateXmlWithSchema(searchResults, "COE.SearchResults", path + "SearchResults.xsd");
		}

		/// <summary>
		/// Validates the mappings xml.
		/// </summary>
		/// <param name="mappings">The xml to validate.</param>
		/// <returns>True if it is ok, false otherwise.</returns>
		public bool ValidateSelectClauseMappings(XmlDocument mappings) {
			return this.ValidateXmlWithSchema(mappings, "COE.Mappings", path + "SelectClauseClassMappings.xsd");
		}

		/// <summary>
		/// Validates the dataview xml.
		/// </summary>
		/// <param name="xml">The xml to validate.</param>
		/// <param name="targetNamespace">The namespace to validate.</param>
		/// <param name="schemaUri">URI where the schema is located.</param>
		/// <returns>True if it is ok, false otherwise.</returns>
		public bool ValidateXmlWithSchema(XmlDocument xml, string targetNamespace, string schemaUri) {
			this.isValid = true;

			XmlSchemaSet schemaSet = new XmlSchemaSet();
			schemaSet.Add(targetNamespace, schemaUri);
			xml.Schemas = schemaSet;

			ValidationEventHandler validation = new ValidationEventHandler(OnValidationError);
			xml.Validate(validation);
			return this.isValid;
		}

		private void OnValidationError(object sender, ValidationEventArgs args) {
			this.isValid = false;
			this.errorMsg = args.Message;
		}
	}
}
