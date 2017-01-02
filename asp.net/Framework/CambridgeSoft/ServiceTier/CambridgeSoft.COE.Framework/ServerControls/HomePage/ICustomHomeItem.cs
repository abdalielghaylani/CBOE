using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls.WebParts
{
    public interface ICustomHomeItem 
    {

        string GetCustomItem();
        void SetConfiguration(COENamedElementCollection<CustomItemConfiguration> configuration);
    }
}
