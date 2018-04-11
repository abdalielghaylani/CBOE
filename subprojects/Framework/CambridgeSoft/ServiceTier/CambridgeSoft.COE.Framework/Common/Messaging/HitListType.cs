using System;
using System.Xml.Serialization;



namespace CambridgeSoft.COE.Framework
{
    /// <summary>
    /// Contains information representing a hitlist operation. Allowed values are:
    /// <list type="bullet">
    ///   <item>Temp: Temporary hitlist.</item>
    ///   <item>Saved: Hitlist saved by an user.</item>
    ///   <item>Marked: Marked hitlist.</item>
    ///   <item>All: All above.</item>
    /// </list>
    /// </summary>
    [Serializable]
    public enum  HitListType
    {
        ///<remarks> represents a temporary hitlist /remarks>
            TEMP,
            ///<remarks> represents a saved hitlist </remarks>
            SAVED,
            ///<remarks> represents a marked hitlist</remarks>
            MARKED,
            ///<remarks> represents all hitlist types</remarks>
            ALL
    }

    [Serializable]
    public enum HitListQueryType
    {
        ///<remarks> represents other hitlist types</remarks>
        OTHER,         
        //<remarks> represents a searhover hitlist </remark
        SEARCHOVER,
        //<remarks> represents a merged hitlist /remarks> 
        MERGED,        
        ///<remarks> represents all hitlist types</remarks>
        ALL
    }
}
