using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Registration;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class AuditingConfigurationProcessor : RegAdminBusinessBase<AuditingConfigurationProcessor>, ICOEConfigurationProcessor
    {
        #region ICOEConfigurationProcessor Members

        public void Process(string settingName, string previousValue, string currentValue)
        {
            RegDal.ToggleAuditing(currentValue);
        }

        #endregion
    }
}
