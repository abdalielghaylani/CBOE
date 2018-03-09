// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Consts.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace COEConfigurationPublishTool
{
    /// <summary>
    /// Constant value for configuration publish tool
    /// </summary>
    internal class Consts
    {
        /// <summary>
        /// The default log file name
        /// </summary>
        internal const string DefaultLogFileName = "COEConfigurationPublishTool.log";

        /// <summary>
        /// The default main instance name
        /// </summary>
        internal const string DefaultInstanceName = "MAIN";

        /// <summary>
        /// The default global user of main instance.
        /// </summary>
        internal const string DefaultMainInstanceGlobalDb = "COEUSER";

        /// <summary>
        /// The Oracle type name.
        /// </summary>
        internal const string OracleType = "ORACLE";

        /// <summary>
        /// The parameter slash char
        /// </summary>
        internal const string Slash = "/";

        /// <summary>
        /// The parameter value separation char
        /// </summary>
        internal const char SwitchKeyValueSeperatorChar = ':';

        /// <summary>
        /// The log parameter char
        /// </summary>
        internal const string SwitchBagKeyLogFile = "log";

        /// <summary>
        /// The name parameter char
        /// </summary>
        internal const string SwitchBagKeyInstanceName = "name";

        /// <summary>
        /// The database type parameter char
        /// </summary>
        internal const string SwitchBagKeyDBMSType = "dbmsType";

        /// <summary>
        /// The database global user parameter char
        /// </summary>
        internal const string SwitchBagKeyDatabaseGlobalUser = "databaseGlobalUser";

        /// <summary>
        /// The database global user password parameter char
        /// </summary>
        internal const string SwitchBagKeyDatabaseGlobalUserPassword = "databaseGlobalUserPassword";

        /// <summary>
        /// The use proxy parameter char
        /// </summary>
        internal const string SwitchBagKeyUseProxy = "useProxy";

        /// <summary>
        /// The host name parameter char
        /// </summary>
        internal const string SwitchBagKeyHostName = "hostName";

        /// <summary>
        /// The port parameter char
        /// </summary>
        internal const string SwitchBagKeyPort = "port";

        /// <summary>
        /// The SID parameter char
        /// </summary>
        internal const string SwitchBagKeySID = "sid";

        /// <summary>
        /// The network alias parameter char
        /// </summary>
        internal const string SwitchBagKeyNetworkAlias = "networkAlias";

        /// <summary>
        /// The database sysUser parameter.
        /// </summary>
        internal const string SwitchBagKeyDatabaseSysUser = "databaseSysUser";

        /// <summary>
        /// The database sys user password parameter.
        /// </summary>
        internal const string SwitchBagKeyDatabaseSysUserPassword = "databaseSysUserPassword";

        /// <summary>
        /// The is CBOE instance parameter char
        /// </summary>
        internal const string SwitchBagKeyIsCBOEInstance = "isCBOEInstance";

        /// <summary>
        /// The publish data view parameter char
        /// </summary>
        internal const string SwitchBagKeyRepublishAllDataviews = "RepublishAllDataviews";

        /// <summary>
        /// The publish data sources parameter char
        /// </summary>
        internal const string SwitchBagKeyRepublishAllDatasources = "RepublishAllDataSources";

        /// <summary>
        /// The usage for the tool
        /// </summary>
        internal const string SwitchBagKeyUsage = "Usage";

        /// <summary>
        /// The password for encrypt or decrypt
        /// </summary>
        internal const string SwitchBagKeyPassword = "Password";

        /// <summary>
        /// The output path for encrypt or decrypt
        /// </summary>
        internal const string SwitchBagKeyOutputPath = "Outputpath";

        /// <summary>
        /// The default value of usage switch.
        /// </summary>
        internal const string SwitchBagKeyUsageDefaultValue = "Publish";

        /// <summary>
        /// The port parameter char
        /// </summary>
        internal const string SwitchBagKeyDriverType = "driverType";

        /// <summary>
        /// The instance node xml format
        /// </summary>
        internal const string InstanceNodeXmlFormat = "<add id=\"{0}\" name=\"{1}\" dbmsType=\"{2}\" databaseGlobalUser=\"{3}\" useProxy=\"{4}\" hostName=\"{5}\" port=\"{6}\" sid=\"{7}\" isCBOEInstance=\"{8}\" dataSource=\"{9}\" driverType=\"{10}\">"
                                    + "<sqlGeneratorData>"
                                    + "<add name=\"CSORACLECARTRIDGE\" schema=\"CSCARTRIDGE\" tempQueriesTableName=\"TempQueries\" molFileFormat=\"V2000\"/>"
                                    + "</sqlGeneratorData>"
                                    + "</add>";

        /// <summary>
        /// The database node xml format
        /// </summary>
        internal const string DatabaseNodeXmlFormat = "<add name=\"{0}\" instanceId=\"{1}\" dbmsType=\"{2}\" dataSource=\"{3}\" owner=\"{4}\" password=\"{5}\" providerName=\"Oracle.DataAccess.Client\" providerOptions=\"\" oracleTracing=\"{6}\" tracing=\"{7}\"/>";
    }
}
