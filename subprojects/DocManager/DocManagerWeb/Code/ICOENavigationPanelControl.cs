using System;
using CambridgeSoft.COE.DocumentManager.Services.Types;

namespace CambridgeSoft.COE.Framework.GUIShell
{
    public interface ICOENavigationPanelControl
    {
        #region Properties

        string ID
        { get; set;}

        #endregion

        #region Events

        event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;

        #endregion

        #region Methods
        void SetTitle(string title);
        void DataBind(Document documentRecord, string nodeTextToDisplayAsSelected);
        #endregion
    }
}

