using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration.Services.AddIns {
    public interface IAddIn
    {
        #region Properties
        IRegistryRecord RegistryRecord { get; set;}
        #endregion

        #region Methods
        void Initialize(string xmlConfiguration);
        #endregion
    }
}
