using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;


namespace CambridgeSoft.COE.Framework.Services.Configuration
{
    /// <summary>
    /// Public interface that needs to be implemented for each Configuration Manager.
    /// </summary>
    public interface ICOEConfigurationManager
    {
        /// <summary>
        /// Retrieves a ConfigurationSection from the proper configuration location (text file or database).
        /// </summary>
        /// <param name="applicationName">The name of the application trying to retrieve configuration</param>
        /// <param name="sectionName">The section to be returned</param>
        /// <returns>returns a configuration management class that extends ConfiguraitonSection</returns>
        ConfigurationSection GetSection(SecurityInfo securityInfo, string applicationName, string sectionName);

        /// <summary>
        /// Retrieves the xml representation of a configuration section from the proper configuration location (text file or database)
        /// </summary>
        /// <param name="applicationName">The name of the application trying to retrieve configuration</param>
        /// <param name="sectionName">The name of the section to be returned</param>
        /// <returns>The xml representation of the configuration section</returns>
        string GetSectionXml(SecurityInfo securityInfo, string applicationName, string sectionName);
    }
}
