using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;


namespace CambridgeSoft.COE.Framework.Services.Configuration
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// Retrieves a ConfigurationSection from the proper configuration location (text file or database).
        /// </summary>
        /// <param name="applicationName">The name of the application trying to retrieve configuration</param>
        /// <param name="sectionName">The section to be returned</param>
        /// <returns>returns a configuration management class that extends ConfiguraitonSection</returns>
        ConfigurationSection GetSection(string applicationName, string sectionName);

        /// <summary>
        /// Retrieves the xml representation of a configuration section from the proper configuration location (text file or database)
        /// </summary>
        /// <param name="applicationName">The name of the application trying to retrieve configuration</param>
        /// <param name="sectionName">The name of the section to be returned</param>
        /// <returns>The xml representation of the configuration section</returns>
        string GetSectionXml(string applicationName, string sectionName);
    }
}
