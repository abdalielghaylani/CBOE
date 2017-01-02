using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    public interface ICOEConfigurationProcessor
    {
        void Process(string settingName, string previousValue, string currentValue);
    }
}
