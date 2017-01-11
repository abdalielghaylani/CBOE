using System;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Controls
{
    public interface ICOENavigationPanelControl
    {
        #region Properties

        string ID
        { get; set;}

        string NewRegistryType
        { get; set;}
        #endregion

        #region Events

        event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;

        #endregion

        #region Methods
        void SetTitle(string title);
        void DataBind(RegistryRecord multiCompoundRegistryRecord);
        void DataBind(RegistryRecord registryRecord, string nodeTextToDisplayAsSelected);
        void DataBind(RegistryRecord registryRecord, bool mixtureDuplicates);
        void DataBind(CompoundList compoundList);
        void DataBind(BatchList batchList);
        #endregion
    }
}

