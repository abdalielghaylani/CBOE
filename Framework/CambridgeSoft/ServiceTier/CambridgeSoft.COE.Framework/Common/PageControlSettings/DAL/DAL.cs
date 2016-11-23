using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using System.Data;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    public class DAL : DALBase
    {

        #region Variables

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEPageControlSettings");

        #endregion

        #region Methods

        /// <summary>
        /// Get a xml from DB by given application name and type.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <returns></returns>
        internal virtual string GetConfigurationXML(string appName,COEPageControlSettings.Type type)
        {
            return string.Empty;
        }

        /// <summary>
        /// Update the custom xml by given application name.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <param name="xml">Custom.xml</param>
        internal virtual void UpdateConfigurationXML(string appName, COEPageControlSettings.Type type, string xml)
        {
            return;
        }

        /// <summary>
        /// Get a xml from DB by given application name and type.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <returns></returns>
        /// <remarks>it will check the privileges table in the DB, not the xml</remarks>
        internal virtual string GetConfigurationXMLFromPrivsPage(string appName,COEPageControlSettings.Type type)
        {
            return string.Empty;
        }

        #endregion
    }
}
