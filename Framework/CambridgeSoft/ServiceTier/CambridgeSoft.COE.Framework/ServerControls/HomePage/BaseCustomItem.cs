using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls.WebParts
{
    public abstract class CustomItem : ICustomHomeItem
    {
        public COENamedElementCollection<CustomItemConfiguration> configData = null;

        public void SetConfiguration(COENamedElementCollection<CustomItemConfiguration> configuration)
        {
            configData = configuration;
        }

        public abstract string GetCustomItem();

            
    }
}
