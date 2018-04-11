using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using CambridgeSoft.COE.Framework.Properties;
using System.Data;

namespace CambridgeSoft.COE.Framework.Common
{   
    /// <summary>
    /// Base class for Data Access Logic Layer
    /// </summary>
    [Serializable]
    public  class DALBase : LoaderBase
    {
        [NonSerialized] 
        private DALManager dALManager;
        [NonSerialized] 
        private Database database;

        public DALBase()
        {

        }
        #region Public properties
       
        /// <summary>
        /// Data Access Manager
        /// </summary>
        public DALManager DALManager
        {
            get { return this.dALManager; }
            set { this.dALManager = value; }
        }

       
        /// <summary>
        /// Database object
        /// </summary>
        public Database Database
        {
            get { return this.database; }
            set { this.database = value; }
        }

        /// <summary>
        /// A general purpose method that you can override in a service that is called after a DAL is instantiated. Allows
        /// you to create a method that does general purposed variable settings taht require a fully constructed DAL
        /// </summary>
        public virtual void SetServiceSpecificVariables()
        {

        }

        /// <summary>
        /// Overloaded. A general purpose method that you can override in a service that is called after a DAL is instantiated. Allows
        /// you to create a method that does general purposed variable settings taht require a fully constructed DAL.
        /// </summary>
        /// <param name="param">The parameters</param>
        public virtual void SetServiceSpecificVariables(string param)//By Sumeet
        { 
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// <para>
        /// It builds the string sql to enforce permissions in your object. This string is meant to be appended to your sql statement.
        /// </para>
        /// <para>
        /// This sql relies in the fact that the table has a field named id that is unique, and generated a sql in the form: " alias.id IN(...)"
        /// </para>
        /// <para>
        /// It also assumes that you are going to add three parameter to your Command: pPersonID, pUserName and pUserName1. The value of pPersonID should be the id of the current user and the values for pUserName and pUserName1 should be the name of the current user. (the same value for both)
        /// </para>
        /// </summary>
        /// <example>
        /// Typically your get method would look like:
        /// <code language="C#">
        /// string sql = @"SELECT TBLALIAS.FIELD1, TBLALIAS.FIELD2 FROM MYTABLE TBLALIAS WHERE MYFILTER = 'MYVALUE' AND " + AccessPermissionsSQL("TBLALIAS", "MYTABLE");
        /// DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
        /// 
        /// DALManager.Database.AddInParameter(dbCommand, "pPersonID", DbType.Int32, COEUser.ID);
        /// DALManager.Database.AddParameter(dbCommand, "pUserName", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
        /// DALManager.Database.AddParameter(dbCommand, "pUserName1", DbType.AnsiString, 100, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, COEUser.Name.ToUpper());
        /// 
        /// //And execute the command.
        /// </code>
        /// </example>
        /// <param name="alias">The alias used in your SQL for the table where your object lives</param>
        /// <param name="table">The table where your object lives</param>
        /// <returns>A sql in the form: " alias.id IN(...)"</returns>
        public virtual string AccessPermissionsSQL(string alias, string table)
        {
            string sql = " " + alias + ".id IN(select obj.id from " + table + " obj, " + Resources.CentralizedStorageDB + ".coepermissions up, " + Resources.CentralizedStorageDB + ".coeobjecttype ot, " + Resources.CentralizedStorageDB + ".coeprincipaltype pt " +
                " where (obj.id = up.objectid " +
               " and ot.id = up.objectypeid " +
               " and up.principaltypeid = pt.id " +
               " and pt.name = 'USER'" +
               " and up.principalid = " + DALManager.BuildSqlStringParameterName("pPersonID") + ")" +
               " union all select obj.id" +
               " from " + table + " obj, " + Resources.CentralizedStorageDB + ".coepermissions rp, " + Resources.CentralizedStorageDB + ".coeobjecttype ot, " + Resources.CentralizedStorageDB + ".coeprincipaltype pt " +
               " where " +
               " obj.id = rp.objectid " +
               " and ot.id = rp.objectypeid " +
               " and  rp.principaltypeid = pt.id " +
               " and pt.name = 'ROLE' " +
               " and  rp.principalid " +
               " in (" +
               " select distinct s.role_id " +
               " from " + Resources.CentralizedStorageDB + ".security_roles s, privilege_tables p, dba_role_privs rp" +
               " where s.role_name = rp.granted_role " +
               " and rp.granted_role " +
               " in (select granted_role from dba_role_privs" +
               " where  grantee = " + DALManager.BuildSqlStringParameterName("pUserName") +
               " union select granted_role " +
               " from dba_role_privs WHERE grantee IN (select granted_role" +
               " from dba_role_privs " +
               " where  grantee = " + DALManager.BuildSqlStringParameterName("pUserName1") + "))))";
            return sql;

        }

        public virtual string GetApplicationFilter(string alias)
        {
           return " and " + alias + ".Application IS NULL";
        }

        public virtual string GetApplicationNotNullFilter(string alias)
        {
            return " and " + alias + ".Application IS NOT NULL";
        }

        #endregion
    }
}
