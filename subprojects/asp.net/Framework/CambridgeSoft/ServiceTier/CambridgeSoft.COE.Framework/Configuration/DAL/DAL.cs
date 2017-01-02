using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    public class DAL : DALBase
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEConfiguration");

        public virtual void AddSection(string description, string handlerClassName, string sectionXml)
        {
            return;
        }

        public virtual string GetSection(string description, ref string handlerClassName)
        {
            return string.Empty;
        }

        public virtual void RemoveSection(string sectionName)
        {
            return;
        }

        internal virtual string GetConfigurationSettingsXml(string appName)
        {
            string sql = "select CONFIGURATIONXML from " + Resources.SecurityDatabaseName + ".COECONFIGURATION where DESCRIPTION = '" + appName + "'";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            return (string)DALManager.ExecuteScalar(dbCommand);
        }

        internal virtual void SetConfigurationSettingsFromXml(string appName, string xml)
        {
            return;
        }

        internal string GetConfigurationValueFromTable(string schemaName, string tableName, string returnField, string filterField, string filterValue)
        {
            string sql = "select " + returnField + " from " + schemaName + "." + tableName + " where " + filterField + " = '" + filterValue + "'";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            return (string)DALManager.ExecuteScalar(dbCommand);
        }

        internal bool HasSchemaInstalled(string schema)
        {
            var sql = "SELECT COUNT(OBJECT_ID) FROM DBA_OBJECTS WHERE UPPER(OWNER)= '" + schema.ToUpper() + "'";
            var dbCommand = this.DALManager.Database.GetSqlStringCommand(sql);

            if (Convert.ToInt32(DALManager.ExecuteScalar(dbCommand)) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal virtual CambridgeSoft.COE.Framework.Caching.COECacheDependency GetDependency(string applicationName)
        {
            return null;
        }
    }
}
