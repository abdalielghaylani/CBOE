using System;


namespace CambridgeSoft.COE.Framework.GUIShell
{
    /// <summary>
    /// This interface muy be implemented for all the pages that are related with registry records (Add-Search-Save-Submit,etc)
    /// </summary>
    public interface ICOERegistryPage
    {
        #region Properties

        string RegistryID
        { get; }

        string RegistryBatchID
        { get; }

        #endregion
    }
}