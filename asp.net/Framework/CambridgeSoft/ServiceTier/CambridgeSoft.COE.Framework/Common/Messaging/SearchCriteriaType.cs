using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;



namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Contains information representing a Search Criteria. Allowed values are:
    /// <list type="bullet">
    ///   <item>Temp: Temp Search Criteria.</item>
    ///   <item>Saved: Search Criteria saved by an user.</item>   
    /// </list>
    /// </summary>
    [Serializable]
    public enum SearchCriteriaType
    {
        ///<remarks> represents a temporary search criteria /remarks>
        TEMP,
        ///<remarks> represents a saved  </remarks>
        SAVED
    }
}
