using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COETableEditorService
{
    /// <summary>
    /// Implements DAL base functions.
    /// </summary>
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COETableEditorService.DAL
    {
        #region Variables

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COETableEditor");

        #endregion

        #region Methods

        /// <summary>
        /// Overide the base function for getting a new ID
        /// </summary>
        /// <returns>The new id generated</returns>
        public override int GetNewID()
        {

            string tableIdFieldName = COETableEditorUtilities.getIdFieldName(_COETableEditorTableName);
            string owner = COETableEditorUtilities.GetAppDataBase();
            string tableName = owner + "." + _COETableEditorTableName;
            string sequenceName = COETableEditorUtilities.getSequenceName(_COETableEditorTableName);
            string seqNameWithOwner = string.Concat(owner, ".", sequenceName);

            string anonymous_block = "DECLARE " + "\n" +
                                     "exist NUMBER := 0; " + "\n" +
                                     "maxId NUMBER := 0; " + "\n" +
                                     "lastNumber NUMBER := 0; " + "\n" +
                                     "BEGIN" + "\n" +
                                     "SELECT COUNT(1) INTO exist FROM ALL_SEQUENCES WHERE SEQUENCE_OWNER = '" + owner + "' AND SEQUENCE_NAME = '" + sequenceName + "'; " + "\n" +
                                     "SELECT NVL(MAX(" + tableIdFieldName + "),0)INTO maxId FROM " + tableName + "; " + "\n" +
                                     "IF exist = 0 THEN " + "\n" +
                                        "maxId := maxId + 1; " + "\n" +
                                        "EXECUTE IMMEDIATE 'CREATE SEQUENCE " + seqNameWithOwner + " INCREMENT BY 1 START WITH '||maxId||' MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE NOCACHE NOORDER'; " + "\n" +                                        
                                     "ELSE " + "\n" +
                                        "SELECT LAST_NUMBER INTO lastNumber FROM ALL_SEQUENCES WHERE SEQUENCE_OWNER = '" + owner + "' AND SEQUENCE_NAME = '" + sequenceName + "'; " + "\n" +
                                        "IF lastNumber < maxId THEN " + "\n" +
                                            "maxId := maxId + 1; " + "\n" +
                                            "EXECUTE IMMEDIATE 'DROP SEQUENCE " + seqNameWithOwner + "'; " + "\n" +
                                            "EXECUTE IMMEDIATE 'CREATE SEQUENCE " + seqNameWithOwner + " INCREMENT BY 1 START WITH '||maxId||' MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE NOCACHE NOORDER'; " + "\n" +
                                        "END IF; " + "\n" +
                                     "END IF;" + "\n" +
                                     "EXECUTE IMMEDIATE 'SELECT " + seqNameWithOwner + ".NEXTVAL FROM DUAL' INTO " + DALManager.BuildSqlStringParameterName("pNewId") + ";" + "\n" +
                                     "END;";

            OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(anonymous_block);

            dbCommand.Parameters.Add(new OracleParameter("pNewId", OracleDbType.Int32, 0, ParameterDirection.Output));
            
            DALManager.ExecuteNonQuery(dbCommand);            
            
            return int.Parse(dbCommand.Parameters["pNewId"].Value.ToString());
        }

        /// <summary>
        /// Ensure there is a sequence that can be used by the table.
        /// </summary>
        public void EnsureSequenceExist()
        {
            string sql = "CREATE SEQUENCE " + COETableEditorUtilities.getSequenceName(_COETableEditorTableName) + " INCREMENT BY 1 START WITH 1 MAXVALUE " +
                           " 1.0E27 MINVALUE 1 NOCYCLE NOCACHE NOORDER";
            DbCommand dbCommandCreate = DALManager.Database.GetSqlStringCommand(sql);

            try
            {
                DALManager.ExecuteNonQuery(dbCommandCreate);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Sequence already there");
            }
        }

        #endregion
    }
}
