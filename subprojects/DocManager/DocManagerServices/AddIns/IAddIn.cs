using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DocumentManager.Services.AddIns
{
    public interface IAddIn
    {
        #region Properties
        CambridgeSoft.COE.DocumentManager.Services.Types.IDocument Document { get; set;}
        #endregion

        #region Methods
        void Initialize(string xmlConfiguration);
        #endregion
    }
}
