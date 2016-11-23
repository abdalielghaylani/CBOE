using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common
{
    //this class is for convenience since we are not clear where the user is coming from yet.
    [Serializable]
    internal class COELoggingSettings
    {
        [NonSerialized]
        private COECallingClientLoggingConfig localConfig = new COECallingClientLoggingConfig();
 
        internal COELoggingSettings()
        {
        }
  
        internal COECallingClientLoggingConfig GetSettings()
        {

            if (Csla.ApplicationContext.GlobalContext["COECConfigFilled"] == null)
            {
                try
                {
                    COEClientConfigurationBO _coeLoggingConfigurationBO = COEClientConfigurationBO.Get(COELoggingConfiguration.SectionName);
                    COELoggingConfiguration loggingSettings = (COELoggingConfiguration)_coeLoggingConfigurationBO.ConfigurationSection;

                    if(loggingSettings != null){
                        localConfig.Priority = System.Convert.ToInt16(loggingSettings.Priority.ToString());
                        localConfig.Categories = loggingSettings.Categories.ToString();
                        localConfig.Severity = loggingSettings.Severity.ToString();
                        localConfig.Enabled = System.Convert.ToBoolean(loggingSettings.Enabled.ToString());
                        localConfig.LogEntryIdentifier = loggingSettings.LogEntryIdentifier.ToString();
                        Csla.ApplicationContext.GlobalContext["COEClientIdentifier"] = localConfig.LogEntryIdentifier;
                        Csla.ApplicationContext.GlobalContext["COELoggingSettings"] = localConfig;
                        Csla.ApplicationContext.GlobalContext["COECConfigFilled"] = "FILLED";

                    }
                    else
                    {
                        localConfig.Enabled = false;

                    }
                }
                catch (System.Exception e)
                {
                    localConfig.Enabled = false;
                }

            }
            else
            {
                localConfig = (COECallingClientLoggingConfig)Csla.ApplicationContext.GlobalContext["COELoggingSettings"];
                
            }
            return localConfig;
        }

  
    }
}
