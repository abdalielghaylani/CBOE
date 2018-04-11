using System;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;


namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
	/// <summary>
	/// Interface that is needed to implement when adding select clauses. Its responsibility is to 
	/// get an instance of a subclass of SelectClauseItem.
	/// Usually this interface is implemented by the same class that inherits SelectClauseItem.
	/// </summary>
	public interface ISelectClauseParser
	{
		/// <summary>
		/// Gets an instance of a provided subclass of SelectClauseItem.
		/// </summary>
		/// <param name="resultsXmlNode">The node of the searchResults.xml that the select clause represents</param>
		/// <param name="dvnLookup">This interface provides methods to lookup the names of fields and tables
		/// in the data view schema.</param>
		/// <returns>The SelectClauseItem</returns>
		SelectClauseItem CreateInstance(XmlNode resultsXmlNode, INamesLookup dvnLookup);
	}
}
