using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using CambridgeSoft.COE.Registration;
using Csla.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Core.Properties;

namespace CambridgeSoft.COE.Registration.Access
{
    /// <summary>
    /// DAL services for Registration.
    /// </summary>
    public class OracleDataAccessClientDAL : DALBase
    {
        #region > Data & constants <

        // method description attributes
        const string BulkSupportDescription = "Supports bulk data-access strategy";
        const string WrappableTransactionDescription = "Allows the caller to control the transaction";

        // in conjunction with single-connection (bulk) data-processing
        private string _persistentConnectionKey;
        private PersistentConnectionWrapper _persistentConenctionWrapper;

        //  accessory SPROC names
        const string SPROC_ROOT = "REGDB.CompoundRegistry.";
        const string SPROC_ROOT_DUPCHECK = "REGDB.RegistryDuplicateCheck.";
        const string CAN_CREATE_REGISTRATION = SPROC_ROOT + "CanCreateMultiCompoundRegistry ";
        const string GET_COMPONENT_DUPLICATES = SPROC_ROOT_DUPCHECK + "FindComponentDuplicates";

        //  crud SPROC names
        const string CREATE_REGISTRATION = SPROC_ROOT + "CreateMultiCompoundRegistry ";
        const string UPLOAD_REGISTRATION = SPROC_ROOT + "LoadMultiCompoundRegRecord ";
        const string READ_REGISTRATION = SPROC_ROOT + "RetrieveMultiCompoundRegistry ";
        const string READ_REGISTRATION_BY_BATCH = SPROC_ROOT + "GetRegistrationByBatch ";
        const string UPDATE_REGISTRATION = SPROC_ROOT + "UpdateMultiCompoundRegistry ";
        const string DELETE_REGISTRATION = SPROC_ROOT + "DeleteMultiCompoundRegistry ";

        //  SPROC parameter names
        const string UNIVERSAL_FUNCTION_RETURN_VALUE = "Universal_Function_Return_Value";
        const string AACTION = "AAction";
        const string ABATCHID = "ABatchID";
        const string ABATCHREGNUMBER = "ABatchRegNumber";
        const string ACARTRIDGEPARAMETERS = "ACartridgeParameters";
        const string ACONFIGURATIONID = "AConfigurationID";
        const string ACOUNT = "ACount";
        const string ADUPLICATECHECK = "ADuplicateCheck";
        const string AHITLISTID = "Ahitlistid";
        const string AID = "Aid";
        const string ADUPLICATEACTION = "ADuplicateAction";
        const string ADUPLICATETYPE = "p_type";
        const string AGENERICPARAMETER = "p_params";
        const string AGENERICVALUE = "p_value";
        const string ALOGDESCRIPTION = "ADescription";
        const string ALOGID = "ALogID";
        const string AMESSAGE = "AMessage";
        const string APERSONID = "APersonID";
        const string APROJECTTYPE = "AType";
        const string AIDENTIFIERTYPE = "AType";
        const string AREGISTRATION = "ARegistration";
        const string AREGISTRYXML = "ARegistryXml";
        const string AREGISTRYLISTXML = "ARegistryListXml";
        const string AREGNUMGENERATION = "ARegNumGeneration";
        const string AREGNUMBER = "ARegNumber";
        const string ASECTIONSLIST = "ASectionsList";
        const string ASEQTYPEID = "ASeqTypeID";
        const string ASTRUCTURE = "AStructure";
        const string ATEMPID = "ATempID";
        const string AUSERID = "AUserID";
        const string AXML = "AXml";
        const string AXMLREGNUMBERS = "AXmlRegNumbers";
        const string AXMLCOMPOUNDLIST = "AXmlCompoundList";
        const string AXMLREGIDS = "AXmlRegIDs";

        //other string constants
        const string NONE = "NONE";

        #endregion

        #region >DAL - Utility<

        /// <summary>
        /// Conditionally closes and disposes of a connection object.
        /// </summary>
        /// <remarks>
        /// Leave this connection alive and open if we're using a persistent data-access strategy
        /// </remarks>
        /// <param name="connection">The connection to close and dispose of</param>
        private void DestroyConnection(DbConnection connection)
        {
            if (!connection.Equals(_persistentConenctionWrapper))
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Sets up a cached Oracle.DataAccess.Client.OracleConnection object, and configures
        /// a System.Timers.Timer object to provide a cache time-out mechanism.
        /// </summary>
        /// <param name="lifetimeInSeconds">The time-out duration, in integral seconds</param>
        internal void UseBulkLoadStrategy(int lifetimeInSeconds)
        {
            string session = CambridgeSoft.COE.Framework.Common.COEUser.SessionID.ToString();
            string cacheKeyTemplate = "{0}.{1}.RegDALConnection";
            _persistentConnectionKey = 
                string.Format(cacheKeyTemplate,Csla.ApplicationContext.User.Identity.Name,session);

            {
                try
                {
                    object existingPersistentConnection = AppDomain.CurrentDomain.GetData(_persistentConnectionKey);
                    if (existingPersistentConnection == null)
                    {
                        DbConnection oraConn;
                        TimeSpan lifetime = new TimeSpan(0, 0, lifetimeInSeconds);
                        if (DALManager.DbConnection != null)
                            oraConn = DALManager.DbConnection;
                        else
                            oraConn = DALManager.Database.CreateConnection();
                        _persistentConenctionWrapper = new PersistentConnectionWrapper(oraConn, _persistentConnectionKey, true, lifetime);
                        AppDomain.CurrentDomain.SetData(_persistentConnectionKey, _persistentConenctionWrapper);
                    }
                    else
                    {
                        _persistentConenctionWrapper = (PersistentConnectionWrapper)existingPersistentConnection;
                    }
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleDALException(ex, null);
                }
            }
        }

        /// <summary>
        /// Generates a System.Data.Common.DbCommand based on the connection being used,
        /// and opens the connection.
        /// </summary>
        /// <remarks>
        /// The OracleCommand object created binds SPROC parameters by name instead of parameter collection index.
        /// </remarks>
        /// <param name="commandText">The query text or procedure name for the command</param>
        /// <param name="commandType">A System.Data.CommandType to apply to the command object</param>
        /// <returns>A System.Data.Common.DbCommand object with an open connection.</returns>
        private DbCommand CreateCommand(string commandText, CommandType commandType)
        {
            DbCommand cmd = null;
            if (_persistentConenctionWrapper != null)
            {
                cmd = _persistentConenctionWrapper.Connection.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
            }
            else
            {
                if (commandType == CommandType.StoredProcedure)
                    cmd = DALManager.Database.GetStoredProcCommand(commandText);
                else
                    cmd = DALManager.Database.GetSqlStringCommand(commandText);
                cmd.Connection = DALManager.Database.CreateConnection();
            }

            OracleCommand oracleCmd = (OracleCommand)cmd;
            oracleCmd.BindByName = true;

            if (oracleCmd.Connection.State != ConnectionState.Open)
                oracleCmd.Connection.Open();
            return oracleCmd;
        }

        /// <summary>
        /// Fetches a connection object
        /// </summary>
        /// <returns></returns>
        private DbConnection GetCurrentConnection()
        {
            DbConnection conn = null;
            if (_persistentConenctionWrapper != null)
            {
                conn = _persistentConenctionWrapper.Connection;
            }
            else
            {
                if (DALManager.DbConnection == null)
                    conn = DALManager.Database.CreateConnection();
                else
                    conn = DALManager.DbConnection;
            }
            return conn;
        }

        /// <summary>
        /// Conditionally closes the command's connection object, then disposes of the command object.
        /// </summary>
        /// <param name="commandObject">The System.Data.DbCommand to dispose of.</param>
        private void DestroyCommand(DbCommand commandObject)
        {
            if (commandObject != null)
            {
                if (_persistentConenctionWrapper == null)
                {
                    if (commandObject.Connection != null)
                    {
                        DestroyConnection(commandObject.Connection);
                    }
                }
                commandObject.Dispose();
            }
        }

        string GetDuplicateCheckString(DuplicateCheck check)
        {
            switch (check)
            {
                case DuplicateCheck.CompoundCheck:
                    return "C";
                case DuplicateCheck.MixCheck:
                    return "M";
                case DuplicateCheck.None:
                    return "N";
                default:
                    return "";
            }
        }

        string GetDuplicateActionString(DuplicateAction action)
        {
            switch (action)
            {
                case DuplicateAction.Duplicate:
                    return "D";
                case DuplicateAction.Batch:
                    return "B";
                case DuplicateAction.Temporary:
                    return "T";
                case DuplicateAction.None:
                    return "N";
                default:
                    return "";
            }
        }

        #endregion

        #region >Utility<

        /// <summary>
        /// Helper method for converting an array of Registration Numbers into the filter XML
        /// required by the subsequent DAL method.
        /// </summary>
        /// <param name="regIds"></param>
        /// <returns></returns>
        public string CreateReglistFilterFromRegNums(string[] regNums)
        {
            StringBuilder builder = new StringBuilder("<REGISTRYLIST>");

            try
            {
                foreach (string regNum in regNums)
                {
                    builder.Append(string.Format("<REGNUMBER>{0}</REGNUMBER>", regNum.ToString()));
                }
                builder.Append("</REGISTRYLIST>");
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleDALException(ex, null);
            }

            return builder.ToString();
        }

        [Description(BulkSupportDescription)]
        public string CanInsertRegistryRecord(string xml, DuplicateCheck checkDuplicates)
        {
            string message = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(CAN_CREATE_REGISTRATION, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATECHECK, OracleDbType.Char, this.GetDuplicateCheckString(checkDuplicates), ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    if (((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull == false)
                    {
                        message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    }
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return message;
        }

        public int GetTempRegistriesCount()
        {
            DbCommand dbCommand = null;
            int result = 0;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETTEMPREGISTRIESCOUNT ");
                dbCommand.Parameters.Add(new OracleParameter(ACOUNT, OracleDbType.Int32, 0, ParameterDirection.Output));
                int count = DALManager.ExecuteNonQuery(dbCommand);
                result = int.Parse(dbCommand.Parameters[ACOUNT].Value.ToString());
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public int GetPermRegistriesCount()
        {
            DbCommand dbCommand = null;
            int result = 0;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETPERMREGISTRIESCOUNT ");
                dbCommand.Parameters.Add(new OracleParameter(ACOUNT, OracleDbType.Int32, 0, ParameterDirection.Output));
                int count = DALManager.ExecuteNonQuery(dbCommand);
                result = int.Parse(dbCommand.Parameters[ACOUNT].Value.ToString());
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public int GetApprovedRegistriesCount()
        {
            DbCommand dbCommand = null;
            int result = 0;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETAPPROVEDREGISTRIESCOUNT ");
                dbCommand.Parameters.Add(new OracleParameter(ACOUNT, OracleDbType.Int32, 0, ParameterDirection.Output));
                int count = DALManager.ExecuteNonQuery(dbCommand);
                result = int.Parse(dbCommand.Parameters[ACOUNT].Value.ToString());
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public int GetDuplicateComponentsCount()
        {
            DbCommand dbCommand = null;
            int result = 0;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETDUPLICATEDCOMPOUNDCOUNT");
                dbCommand.Parameters.Add(new OracleParameter(ACOUNT, OracleDbType.Int32, 0, ParameterDirection.Output));
                int count = DALManager.ExecuteNonQuery(dbCommand);
                result = int.Parse(dbCommand.Parameters[ACOUNT].Value.ToString());
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public string GetRegistryRecordTemporaryIdList(int hitListId)
        {
            string strIdList = string.Empty;
            DbCommand dbCommand = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand(SPROC_ROOT + "RetrieveTempIDList ");
                dbCommand.Parameters.Add(new OracleParameter(AHITLISTID, OracleDbType.Int32, hitListId, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter(AID, OracleDbType.Clob, strIdList, ParameterDirection.Output));
                dbCommand.Connection = DALManager.Database.CreateConnection();
                dbCommand.Connection.Open();

                //Not using DALManager.ExecuteNonQuery(dbCommand); because we need the connection opened.
                dbCommand.ExecuteNonQuery();

                strIdList = ((OracleClob)dbCommand.Parameters[AID].Value).IsNull ? string.Empty : ((OracleClob)dbCommand.Parameters[AID].Value).Value as string;
                dbCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return strIdList;
        }

        public string GetRegistryRecordsFromHitList(int hitListId)
        {
            List<string> results = new List<string>();
            string[] regNums = null;
            DbCommand dbCommand = null;
            string result = null;

            try
            {
                if (!string.IsNullOrEmpty(this.GetRegistryRecordTemporaryIdList(hitListId)))
                {
                    string sql = string.Format(@" SELECT RN.REGNUMBER
                            FROM REGDB.VW_MIXTURE M
                                    JOIN REGDB.VW_REGISTRYNUMBER RN ON M.REGID = RN.REGID
                            WHERE M.MIXTUREID IN ({0})", this.GetRegistryRecordTemporaryIdList(hitListId));
                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    IDataReader reader = DALManager.ExecuteReader(dbCommand);
                    while (reader.Read())
                    {
                        results.Add(reader.GetString(0));
                    }
                    reader.Close();
                }
                regNums = results.ToArray();
                result = CreateReglistFilterFromRegNums(regNums);
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public string GetRegIDListFromHitList(int hitlistID)
        {
            StringBuilder builder = new StringBuilder(string.Empty);
            DbCommand dbCommand = null;

            try
            {
                string mixIDList = this.GetRegistryRecordTemporaryIdList(hitlistID);
                if (!string.IsNullOrEmpty(mixIDList))
                {
                    string sql = string.Format(@" SELECT RN.REGID
                            FROM REGDB.VW_MIXTURE M
                                    JOIN REGDB.VW_REGISTRYNUMBER RN ON M.REGID = RN.REGID
                            WHERE M.MIXTUREID IN ({0})", mixIDList);

                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    IDataReader reader = DALManager.ExecuteReader(dbCommand);
                    while (reader.Read())
                    {
                        builder.Append(string.Format("{0},", reader.GetInt32(0)));
                    }
                    reader.Close();
                }
                if (builder.Length > 0 && builder[builder.Length - 1] == ',')
                    builder.Remove(builder.Length - 1, 1);
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Added on 2008/10/31 for DataLoader application bulk register compounds 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="actionDuplicates"></param>
        /// <returns></returns>
        [Description(BulkSupportDescription)]
        public string InsertRegistryRecordList(
            string xml
            , DuplicateAction actionDuplicates
            , Int32 ALogId
            )
        {
            string whyIsThisNotXmlResults = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "LoadMultiCompoundRegRecordList ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AREGISTRYLISTXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATEACTION, OracleDbType.Char, this.GetDuplicateActionString(actionDuplicates), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AREGISTRATION, OracleDbType.Char, 'Y', ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AREGNUMGENERATION, OracleDbType.Char, 'Y', ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ACONFIGURATIONID, OracleDbType.Int32, 1, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ALOGID, OracleDbType.Int32, ALogId, ParameterDirection.Input));
                    //cmd.Parameters.Add(new OracleParameter(ATEMPID, OracleDbType.Varchar2, temp_id, ParameterDirection.Input));
                    cmd.ExecuteNonQuery();

                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return whyIsThisNotXmlResults;
        }

        public string[] GetRecordsThatContainsCompound(int compoundId)
        {
            string sql = @" SELECT RN.REGNUMBER 
                            FROM VW_REGISTRYNUMBER RN, VW_COMPOUND C, VW_MIXTURE_COMPONENT MC, VW_MIXTURE M
                            WHERE RN.REGID = M.REGID
                                    AND MC.MIXTUREID = M.MIXTUREID
                                    AND MC.COMPOUNDID = C.COMPOUNDID
                                    AND NOT EXISTS (SELECT 1 FROM VW_COMPOUND WHERE RegID=RN.RegID )
                                    AND C.COMPOUNDID = " + DALManager.BuildSqlStringParameterName("pCompoundId");
            List<string> regNumbers = new List<string>();
            DbCommand dbCommand = null;
            string[] result = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pCompoundId", DbType.Int32, compoundId);
                IDataReader reader = DALManager.ExecuteReader(dbCommand);
                while (reader.Read())
                {
                    regNumbers.Add(reader.GetString(0));
                }
                reader.Close();
                result = regNumbers.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        /// <summary>
        /// Uses the reg database for matching fragments
        /// </summary>
        public SafeDataReader GetKnownFragment(string structureCDX)
        {
            //DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetKnownFragment ");
            //TODO - SQL. Fast SSS=SELECT ... FROM tab WHERE CsCartridge.MoleculeContains(fld, query4000, query8000, options)=1
            //The query string can be an encoded CDX document, a SMILES string, or a MolFile
            String query = "SELECT code, fragmenttypeid from REGDB.FRAGMENTS WHERE CsCARTRIDGE.MoleculeContains(BASE64_CDX, '" + structureCDX + "', null, 'IDENTITY=YES')=1";
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(query);
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty,
                    DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        /// <summary>
        /// Utility method which, given a repository structure ID value, will return a
        /// list of all Registrations that share this structure. Note that the 'special'
        /// structures can be conditionally excluded from consideration.
        /// </summary>
        /// <remarks>
        /// This is useful for gathering the list of 'other registrations affected' by a change
        /// to a structure already stored in the repository. In the case where this structure is
        /// shared across multiple registrations, this will enable the caller to re-create the
        /// aggregate structures. Thus, this is a cross-updating mechanism.
        /// </remarks>
        /// <param name="structureId">the repository identifier for the structure</param>
        /// <param name="excludeNonStructureValues">\
        /// The caller provides TRUE to avoid the 'special' structure identifiers (those with negative
        /// numbers as IDs) such as the 'No-Structure' identifier. Since users cannot edit these
        /// 'special' structures, they can conditionally be eliminated from consideration.
        /// </param>
        /// <returns>
        /// a list of RegNumbers corresponding to registrations that share this structure
        /// </returns>
        public string[] GetRecordsThatContainsStructure(int structureId, bool excludeNonStructureValues)
        {
            string paramName = "pStructureId";
            string sql = Resources.GetRecordsThatContainsStructure + " WHERE ";
            if (excludeNonStructureValues)
                sql += "c.structureid > 0 AND ";
            sql += "c.structureid = " + DALManager.BuildSqlStringParameterName(paramName);

            List<string> regNumbers = new List<string>();
            DbCommand dbCommand = null;
            string[] result = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, structureId);
                IDataReader reader = DALManager.ExecuteReader(dbCommand);
                while (reader.Read())
                {
                    regNumbers.Add(reader.GetString(0));
                }
                reader.Close();
                result = regNumbers.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public int[] GetAlreadyDuplicate(string compoundRegNum)
        {
            //TODO: Please move the SQL to the assembly's Resources file using the existing pattern.
            string sql = @" SELECT CompoundID FROM RegDB.VW_Duplicates D,RegDB.VW_RegistryNumber RN,RegDB.VW_Compound C
                                WHERE C.RegID=RN.RegID
                                AND RN.RegNumber=D.RegNumber 
                                AND D.RegNumberDuplicated= " + DALManager.BuildSqlStringParameterName("pCompoundRegNum");

            List<int> regNumbers = new List<int>();
            DbCommand dbCommand = null;
            int[] result = null;
            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pCompounRegNum", DbType.AnsiString, compoundRegNum);
                IDataReader reader = DALManager.ExecuteReader(dbCommand);
                while (reader.Read())
                {
                    regNumbers.Add(reader.GetInt32(0));
                }
                reader.Close();

                sql = @"SELECT CompoundID FROM RegDB.VW_Duplicates D,RegDB.VW_RegistryNumber RN,RegDB.VW_Compound C
                            WHERE C.RegID=RN.RegID 
                            AND RN.RegNumber= D.RegNumberDuplicated 
                            AND D.RegNumber= " + DALManager.BuildSqlStringParameterName("pCompoundRegNum");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pCompounRegNum", DbType.AnsiString, compoundRegNum);
                reader = DALManager.ExecuteReader(dbCommand);
                while (reader.Read())
                {
                    regNumbers.Add(reader.GetInt32(0));
                }
                reader.Close();

                result = regNumbers.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        public string FetchRegisteredInfoFromTempBatchID(int tempBatchID)
        {
            string sql =
@" Select 
mr.regnumber as RegNumber
, r.regnumber as ComponentRegNumber
, b.batchnumber
From vw_mixture m
, vw_registrynumber r
, vw_registrynumber mr
, vw_compound c
, vw_mixture_component mc
, vw_batch b
Where
m.regid = mr.regid 
and mc.mixtureid = m.mixtureid
and mc.compoundid = c.compoundid
and c.regid = r.regid 
and b.regid = m.regid" +
" and b.tempbatchid = " + DALManager.BuildSqlStringParameterName("tempBatchID");
            DbCommand dbCommand = null;
            string xml = "";

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "tempBatchID", DbType.Int32, tempBatchID);
                IDataReader reader = DALManager.ExecuteReader(dbCommand);
                string RegNumber = "";
                List<string> ComponentRegNumber = new List<string>();
                List<string> BatchNumber = new List<string>();
                while (reader.Read())
                {
                    RegNumber = reader.GetString(0);    // Same value will repeat for all rows
                    if (ComponentRegNumber.Contains(reader.GetString(1)) == false)
                    {
                        ComponentRegNumber.Add(reader.GetString(1));    // Accumulate component list
                    }
                    if (BatchNumber.Contains(reader.GetValue(2).ToString()) == false)
                    {
                        BatchNumber.Add(reader.GetValue(2).ToString()); // Accumulate batch list
                    }
                }
                reader.Close();
                xml += "<ReturnList>";
                xml += "<RegNumber>" + RegNumber + "</RegNumber>";
                xml += "<ComponentRegNumberList>";
                foreach (string s in ComponentRegNumber)
                {
                    xml += "<ComponentRegNumber>" + s + "</ComponentRegNumber>";
                }
                xml += "</ComponentRegNumberList>";
                xml += "<BatchNumberList>";
                foreach (string s in BatchNumber)
                {
                    xml += "<BatchNumber>" + s + "</BatchNumber>";
                }
                xml += "</BatchNumberList>";
                xml += "</ReturnList>";
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return xml;
        }

        internal int BulkDelete(string registryIdList, string logDescription)
        {
            int logId = 0;
            DbConnection conn = null;
            DbCommand dbCommand = null;

            try
            {
                conn = DALManager.Database.CreateConnection();
                conn.Open();
                using (DbTransaction trn = conn.BeginTransaction())
                {

                    string[] registryIds = registryIdList.Split(',');

                    dbCommand = DALManager.Database.GetSqlStringCommand(@"insert into regdb.vw_log_bulkregistration_ID(DESCRIPTION, DUPLICATE_ACTION) 
                                                                            values (" + DALManager.BuildSqlStringParameterName("pLogDescription") + @", 'R') returning log_id into :pNewId");

                    dbCommand.Parameters.Add(new OracleParameter("pLogDescription", OracleDbType.Varchar2, 250, logDescription, ParameterDirection.Input));
                    dbCommand.Parameters.Add(new OracleParameter("pNewId", OracleDbType.Int32, 0, logId, ParameterDirection.Output));
                    //DALManager.Database.AddOutParameter(dbCommand, "pNewId", DbType.Int32, logId);
                    DALManager.ExecuteNonQuery(dbCommand);

                    logId = ((OracleDecimal)dbCommand.Parameters[1].Value).ToInt32();

                    foreach (string registryIdString in registryIds)
                    {
                        int temporaryIdInt;
                        string sucessString = "Succeed";

                        DbCommand command = null;

                        try
                        {
                            if (int.TryParse(registryIdString, out temporaryIdInt))
                            {
                                command = DALManager.Database.GetSqlStringCommand(@"insert into Log_BulkRegistration(Log_Id, Temp_ID, Comments) values (" +
                                                                                DALManager.BuildSqlStringParameterName("pLogId") + ", " +
                                                                                DALManager.BuildSqlStringParameterName("pId") + ", " +
                                                                                DALManager.BuildSqlStringParameterName("pComments") + ")");

                                command.Parameters.Add(new OracleParameter("pLogId", OracleDbType.Int32, 0, logId, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter("pId", OracleDbType.Int32, 0, temporaryIdInt, ParameterDirection.Input));

                                this.DeleteRegistryRecordTemporary(temporaryIdInt);
                            }
                            else
                            {
                                command = DALManager.Database.GetSqlStringCommand(@"insert into Log_BulkRegistration(Log_Id, Reg_Number, Comments) values (" +
                                                                                DALManager.BuildSqlStringParameterName("pLogId") + ", " +
                                                                                DALManager.BuildSqlStringParameterName("pId") + ", " +
                                                                                DALManager.BuildSqlStringParameterName("pComments") + ")");

                                command.Parameters.Add(new OracleParameter("pLogId", OracleDbType.Int32, 0, logId, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter("pId", OracleDbType.Varchar2, 10, registryIdString.Trim(), ParameterDirection.Input));

                                this.DeleteRegistryRecord(registryIdString.Trim());
                            }
                            command.Parameters.Add(new OracleParameter("pComments", OracleDbType.Varchar2, 500, sucessString, ParameterDirection.Input));

                        }
                        catch (Exception)
                        {
                            command.Parameters.Add(new OracleParameter("pComments", OracleDbType.Varchar2, 500, "Failure", ParameterDirection.Input));
                        }
                        finally
                        {
                            DALManager.ExecuteNonQuery(command);
                        }
                    }

                    trn.Commit();

                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                conn.Dispose();
                DestroyCommand(dbCommand);
            }

            return logId;
        }

        public int BulkDelete(
            string registryIdList, string logDescription, bool recordsAreTemporary, out string[] failedDeletions
            )
        {
            int logId = 0;
            DbConnection conn = null;
            DbCommand dbCommand = null;
            failedDeletions = new string[0];
            try
            {
                conn = DALManager.Database.CreateConnection();
                conn.Open();
                using (DbTransaction trn = conn.BeginTransaction())
                {
                    
                    string[] registryIds = registryIdList.Split(',');

                    dbCommand = DALManager.Database.GetSqlStringCommand(@"insert into regdb.vw_log_bulkregistration_ID(DESCRIPTION, DUPLICATE_ACTION) 
                                                                            values (" + DALManager.BuildSqlStringParameterName("pLogDescription") + @", 'R') returning log_id into :pNewId");

                    dbCommand.Parameters.Add(new OracleParameter("pLogDescription", OracleDbType.Varchar2, 250, logDescription, ParameterDirection.Input));
                    dbCommand.Parameters.Add(new OracleParameter("pNewId", OracleDbType.Int32, 0, logId, ParameterDirection.Output));
                    //DALManager.Database.AddOutParameter(dbCommand, "pNewId", DbType.Int32, logId);
                    DALManager.ExecuteNonQuery(dbCommand);

                    logId = ((OracleDecimal)dbCommand.Parameters[1].Value).ToInt32();

                    foreach (string registryIdString in registryIds)
                    {
                        string sucessString = "Operation succeeded";

                        DbCommand command = null;
                        try
                        {
                            if (recordsAreTemporary == true)
                            {
                                int tempID = Convert.ToInt32(registryIdString);
                                command = DALManager.Database.GetSqlStringCommand(
                                    @"insert into Log_BulkRegistration(Log_Id, Temp_ID, Comments) values (" +
                                    DALManager.BuildSqlStringParameterName("pLogId") + ", " +
                                    DALManager.BuildSqlStringParameterName("pId") + ", " +
                                    DALManager.BuildSqlStringParameterName("pComments") + ")"
                                );

                                command.Parameters.Add(new OracleParameter("pLogId", OracleDbType.Int32, 0, logId, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter("pId", OracleDbType.Int32, 0, tempID, ParameterDirection.Input));

                                this.DeleteRegistryRecordTemporary(tempID);
                            }
                            else
                            {
                                command = DALManager.Database.GetSqlStringCommand(
                                    @"insert into Log_BulkRegistration(Log_Id, Reg_Number, Comments) values (" +
                                    DALManager.BuildSqlStringParameterName("pLogId") + ", " +
                                    DALManager.BuildSqlStringParameterName("pId") + ", " +
                                    DALManager.BuildSqlStringParameterName("pComments") + ")"
                                );

                                command.Parameters.Add(new OracleParameter("pLogId", OracleDbType.Int32, 0, logId, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter("pId", OracleDbType.Varchar2, 10, registryIdString.Trim(), ParameterDirection.Input));

                                this.DeleteRegistryRecord(registryIdString.Trim());
                            }
                            command.Parameters.Add(new OracleParameter("pComments", OracleDbType.Varchar2, 500, sucessString, ParameterDirection.Input));

                        }
                        catch (Exception ex)
                        {
                            //TODO: Determine why we're not logging the error!
                            Array.Resize<string>(ref failedDeletions, failedDeletions.Length + 1);
                            failedDeletions[failedDeletions.Length - 1] = registryIdString;
                            command.Parameters.Add(new OracleParameter("pComments", OracleDbType.Varchar2, 500, "Failure", ParameterDirection.Input));
                        }
                        finally
                        {
                            DALManager.ExecuteNonQuery(command);
                        }
                    }

                    trn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                conn.Dispose();
                DestroyCommand(dbCommand);
            }

            return logId;
        }

        /// <summary>
        /// Bulk-register a list of existing temporary IDs as a result of user-selection
        /// from a saved search result ('hit-list').
        /// </summary>
        /// <param name="actionDuplicates"></param>
        /// <param name="savedHitlistID"></param>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        public int PromoteTempRegistryRecordList(
            DuplicateAction actionDuplicates
            , int savedHitlistID
            , string userId
            , string logMessage
            )
        {
            int logId = 0;
            string action = this.GetDuplicateActionString(actionDuplicates);
            string message = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "ConvertHitlistTempsToPerm ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AHITLISTID, OracleDbType.Int32, savedHitlistID, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATEACTION, OracleDbType.Char, action, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AUSERID, OracleDbType.Varchar2, userId, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ALOGDESCRIPTION, OracleDbType.Varchar2, logMessage, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ALOGID, OracleDbType.Int32, logId, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, message, ParameterDirection.Output));

                    //bypass intrinsic, DB-side fetch of Registration XMLs during processing
                    cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));

                    cmd.ExecuteNonQuery();
                    logId = int.Parse(cmd.Parameters[ALOGID].Value.ToString());
                    message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return logId;
        }

        public string CheckForDuplicates(
            string duplicateCheckType
            , string parameterToCheck
            , string valueToCheck
            )
        {
            DbCommand cmd = null;
            string result = null;

            try
            {
                cmd = CreateCommand(GET_COMPONENT_DUPLICATES, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(
                        new OracleParameter(ADUPLICATETYPE, OracleDbType.Varchar2, duplicateCheckType, ParameterDirection.Input)
                    );
                    cmd.Parameters.Add(
                        new OracleParameter(AGENERICPARAMETER, OracleDbType.Varchar2, parameterToCheck, ParameterDirection.Input)
                    );
                    cmd.Parameters.Add(
                        new OracleParameter(AGENERICVALUE, OracleDbType.Clob, valueToCheck, ParameterDirection.Input)
                    );
                    cmd.Parameters.Add(
                        new OracleParameter(UNIVERSAL_FUNCTION_RETURN_VALUE, OracleDbType.Clob, ParameterDirection.ReturnValue)
                    );

                    cmd.ExecuteNonQuery();
                    Oracle.DataAccess.Types.OracleClob retval =
                        (Oracle.DataAccess.Types.OracleClob)cmd.Parameters[UNIVERSAL_FUNCTION_RETURN_VALUE].Value;
                    if (retval != null && !retval.IsEmpty && !retval.IsNull)
                        result = retval.Value;
                    
                    //txnScope.Complete();
                    //We do NOT want to commit anything
                    //txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return result;
        }

        #endregion

        #region >DAL - Lists<

        /// <summary>
        /// Returns a list of picklist 'domains' (lists) based on a picklist domain name.
        /// </summary>
        /// <param name="vDescription">the picklist domain name to match</param>
        /// <returns>a Csla.Data.SafeDataReader object</returns>
        public SafeDataReader GetPicklistDomains(string vDescription)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                string sql = @"Select * FROM REGDB.PICKLISTDOMAIN WHERE UPPER(DESCRIPTION) = " + DALManager.BuildSqlStringParameterName("description");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "description", DbType.AnsiString, vDescription.ToUpper());
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }

            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetPicklistDomains(int vId)
        {
            string sql = @"Select * FROM REGDB.PICKLISTDOMAIN WHERE ID = " + DALManager.BuildSqlStringParameterName("id");
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "id", DbType.Int32, vId);
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetPicklist(string vSql)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(vSql);
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            return safeReader;
        }

        /// <summary>
        /// Helper method which parameterizes a SQL statement, then executes it in order to retrieve
        /// a Picklist domain instance.
        /// </summary>
        /// <param name="vSql">the sql statement to use,including the correct parameter syntax</param>
        /// <param name="cmdParms">the dictionary of parameter names and their applciable values</param>
        /// <returns>a Csla data-reader</returns>
        public SafeDataReader GetPicklist(string vSql, Dictionary<string, object> cmdParms)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(vSql);
                foreach (KeyValuePair<string, object> pair in cmdParms)
                {
                    dbCommand.Parameters.Add(new OracleParameter(pair.Key, pair.Value));
                }

                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            return safeReader;
        }

        public SafeDataReader GetChemistNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetChemistNameValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetActiveChemistNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetActiveChemistNameValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetIdentifierNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetIdentifiedNameValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetFragmentNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetFragmentNameValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetFragmentsList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetFragmentsList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetFragmentsList(List<int> idList)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;
            int[] list = idList.ToArray(); 
            //"AIdList"
            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetFragmentListByIds ");
                OracleParameter pList = new OracleParameter("AIdList", OracleDbType.Int32, list.Length, list, ParameterDirection.Input);
                pList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                dbCommand.Parameters.Add(pList);
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetMatchedFragmentsList(string structureToMatch)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetMatchedFragmentsList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter(ASTRUCTURE, OracleDbType.Clob, structureToMatch, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetFragmentTypesValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetFragmentTypesValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));

            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetNoteBookNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetNoteBookNameValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetPrefixNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetPrefixNameValueList");
                //RefCursor parameter always goes last. I'm not sure if this is neccessray for the datareader or not.
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetProjectNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetProjectNameValueList ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetProjectList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetProjectList ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetActiveProjectListByPerson(int personID)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETACTIVEPROJECTLISTBYPERSONID ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                dbCommand.Parameters.Add(new OracleParameter(APERSONID, OracleDbType.Int32, personID, ParameterDirection.Input));                
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetActiveProjectListByPersonAndType(int personID, char type)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETACTIVEPROJECTLISTBYPERSONID ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));                
                dbCommand.Parameters.Add(new OracleParameter(APERSONID, OracleDbType.Int32, personID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter(APROJECTTYPE, OracleDbType.Char, type, ParameterDirection.Input));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetSequenceList(int sequenceTypeID)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetSequenceList ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                dbCommand.Parameters.Add(new OracleParameter(ASEQTYPEID, OracleDbType.Int32, sequenceTypeID, ParameterDirection.Input));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetSequenceListByPersonID(int sequenceTypeID, int personID)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetSequenceListByPersonID ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                dbCommand.Parameters.Add(new OracleParameter(ASEQTYPEID, OracleDbType.Int32, sequenceTypeID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter(APERSONID, OracleDbType.Int32, personID, ParameterDirection.Input));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetIdentifierList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETIDENTIFIERLIST ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetIdentifierListByType(char type)
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GETIDENTIFIERLIST ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                dbCommand.Parameters.Add(new OracleParameter(AIDENTIFIERTYPE, OracleDbType.Char, type, ParameterDirection.Input));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetSolventNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetSolventNameValueList ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public SafeDataReader GetUserNameValueList()
        {
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand("REGDB.GUI_UTIL.GetUserNameValueList ");
                dbCommand.Parameters.Add(new OracleParameter("O_RS", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Current, Convert.DBNull));
                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        public string GetCompoundList(string xml)
        {
            string resultXml = string.Empty;
            DbCommand dbCommand = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand(SPROC_ROOT + "RetrieveCompoundRegistryList ");
                dbCommand.Parameters.Add(new OracleParameter(AXMLREGNUMBERS, OracleDbType.Clob, xml, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter(AXMLCOMPOUNDLIST, OracleDbType.Clob, null, ParameterDirection.Output));

                dbCommand.Connection = DALManager.Database.CreateConnection();
                dbCommand.Connection.Open();
                dbCommand.ExecuteNonQuery();
                resultXml = ((OracleClob)dbCommand.Parameters[AXMLCOMPOUNDLIST].Value).Value as string;
                dbCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return resultXml;
        }

        public string GetRegistryRecordList(string xml)
        {
            string resultXml = string.Empty;
            DbCommand dbCommand = null;

            try
            {
                dbCommand = DALManager.Database.GetStoredProcCommand(SPROC_ROOT + "RetrieveMultiCompoundRegList ");
                dbCommand.Parameters.Add(new OracleParameter(AXMLREGIDS, OracleDbType.Clob, xml, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter(AXMLCOMPOUNDLIST, OracleDbType.Clob, null, ParameterDirection.Output));

                dbCommand.Connection = DALManager.Database.CreateConnection();
                dbCommand.Connection.Open();
                dbCommand.ExecuteNonQuery();
                resultXml = ((OracleClob)dbCommand.Parameters[AXMLCOMPOUNDLIST].Value).Value as string;
                dbCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return resultXml;
        }

        #endregion

        #region >DAL - Log<

        /// <summary>
        /// Added on 2008/11/25 for get log info in RegisterMarked webpage
        /// </summary>
        /// <param name="logId">log Id</param>
        /// <returns>data table</returns>
        public DataTable GetLogInfo(int logId)
        {
            string paramName = "logID";
            string sql = Resources.GetLogHeaderSql
                + " WHERE t.log_id = " + DALManager.BuildSqlStringParameterName(paramName);
            DbCommand dbCommand = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, logId);
                DALManager.DbConnection.Open();
                dataTable = DALManager.Database.ExecuteDataSet(dbCommand).Tables[0];
                DALManager.DbConnection.Close();
                DALManager.DbConnection = null;
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return dataTable;
        }

        /// <summary>
        /// Added on 2008/11/26 for insert logid info in RegisterMarked webpage
        /// </summary>
        /// <param name="actionDuplicates">The duplicate-resolution action to take.</param>
        /// <param name="userId">The user performing the registration.</param>
        /// <returns>datatable</returns>
        public Int32 InsertLogInfo(DuplicateAction actionDuplicates, string userId)
        {
            int logId = 0;
            string action = this.GetDuplicateActionString(actionDuplicates);
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "LogBULKREGISTRATIONID ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ALOGID, OracleDbType.Int32, logId, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATEACTION, OracleDbType.Varchar2, action, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AUSERID, OracleDbType.Varchar2, userId, ParameterDirection.Input));
                    //dbCommand.Parameters.Add(new OracleParameter(AREGISTRYXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.ExecuteNonQuery();

                    logId = int.Parse(cmd.Parameters[ALOGID].Value.ToString());
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return logId;
        }

        /// <summary>
        /// Added on 2008/11/25 for update log info in RegisterMarked webpage
        /// </summary>
        /// <param name="logId">log Id</param>
        /// <param name="strDescription">Description</param>
        [Description(BulkSupportDescription)]
        public void UpdateLogInfo(int logId, string strDescription)
        {
            string cmdText = "update REGDB.log_bulkregistration_id set Description = '"
                + strDescription + "'"
                + " where LOG_ID=" + logId;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(cmdText, CommandType.Text);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }
        }

        #endregion

        #region >DAL - Batch<

        [Description(BulkSupportDescription)]
        public string GetTemporalID()
        {
            string nextId = string.Empty;
            string cmdText = "select " + DALManager.DatabaseData.OriginalOwner
                + "." + Resources.Seq_Temporary_Batch
                + ".nextval as tempID from dual";
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(cmdText, CommandType.Text);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    nextId = cmd.ExecuteScalar().ToString();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return nextId;
        }

        [Description(BulkSupportDescription)]
        public string GetBatch(int id)
        {
            string xml = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "RetrieveBatch ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AID, OracleDbType.Int32, id, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    xml = ((OracleClob)cmd.Parameters[AXML].Value).Value as string;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return xml;
        }

        [Description(BulkSupportDescription)]
        public string GetBatchTemporary(int tempId)
        {
            string xml = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "RetrieveBatchTmp ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ATEMPID, OracleDbType.Int32, tempId, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    xml = ((OracleClob)cmd.Parameters[AXML].Value).Value as string;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return xml;
        }

        [Description(BulkSupportDescription)]
        public string UpdateBatch(string xml)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "UpdateBatch ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return recordsAffected.ToString();
        }

        [Description(BulkSupportDescription)]
        public string UpdateBatchTemporary(string xml)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "UpdateBatchTmp ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return recordsAffected.ToString();
        }

        [Description(BulkSupportDescription)]
        public void MoveBatch(int batchID, string toRegNum)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "MoveBatch ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ABATCHID, OracleDbType.Int32, batchID, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, toRegNum, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }
        }

        [Description(BulkSupportDescription)]
        public void DeleteBatch(int batchID)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "DeleteBatch ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ABATCHID, OracleDbType.Int32, batchID, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }
        }

        #endregion

        #region >DAL - Sequences<

        [Description(BulkSupportDescription)]
        public SafeDataReader GetSequence(int structureId)
        {
            string paramName = "sequenceID";
            string sql = Resources.GetSequenceSql
                + " WHERE s.sequenceid = " + DALManager.BuildSqlStringParameterName(paramName);
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, structureId);

                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        #endregion

        #region >DAL - Structure<

        /// <summary>
        /// Retrieves structural information for a chemical component.
        /// </summary>
        /// <param name="structureId">Internal identifier (key) for a chemical structure in the database.</param>
        /// <returns>The (partial) xml representation of a Structure object.</returns>
        [Description(BulkSupportDescription)]
        public SafeDataReader GetStructure(int structureId)
        {
            string paramName = "ID";
            string sql = Resources.GetStructureSql
                + " WHERE T.ID = " + DALManager.BuildSqlStringParameterName(paramName);
            DbCommand dbCommand = null;
            SafeDataReader safeReader = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, structureId);

                safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return safeReader;
        }

        #endregion

        #region >DAL - Registry Temp<

        [Description(BulkSupportDescription)]
        public RegistrationCrudResponse InsertRegistryRecordTemporary(string xml)
        {
            RegistrationCrudResponse result = null;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "CreateTemporaryRegistration ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ATEMPID, OracleDbType.Int32, 0, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, string.Empty, ParameterDirection.Output));
                    if (this._persistentConenctionWrapper != null)
                    {
                        cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));
                    }
                    cmd.ExecuteNonQuery();
                    int tempId = int.Parse(cmd.Parameters[ATEMPID].Value.ToString());
                    string message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    result = new RegistrationCrudResponse(message, tempId);
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return result;
        }

        [Description(BulkSupportDescription)]
        public string GetRegistryRecordTemporary(int tempId)
        {
            string xml = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "RetrieveTemporaryRegistration ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ATEMPID, OracleDbType.Int32, tempId, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    xml = ((OracleClob)cmd.Parameters[AXML].Value).Value as string;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return xml;
        }

        [Description(BulkSupportDescription)]
        public string UpdateRegistryRecordTemporary(string xml, out string message)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;
            message = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "UpdateTemporaryRegistration ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, message = string.Empty, ParameterDirection.Output));
                    if (this._persistentConenctionWrapper != null)
                    {
                        cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));
                    }
                    recordsAffected = cmd.ExecuteNonQuery();
                    message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);

            }

            return recordsAffected.ToString();
        }

        [Description(BulkSupportDescription)]
        public string DeleteRegistryRecordTemporary(int tempId)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(SPROC_ROOT + "DeleteTemporaryRegistration ", CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ATEMPID, OracleDbType.Int32, tempId, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return recordsAffected.ToString();
        }

        [Description(WrappableTransactionDescription)]
        private string DeleteRegistryRecordTemporary(int tempId, bool callerControlsTransaction)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;
            try
            {
                if (callerControlsTransaction == true)
                {
                    cmd = CreateCommand(SPROC_ROOT + "DeleteTemporaryRegistration ", CommandType.StoredProcedure);
                    cmd.Parameters.Add(new OracleParameter(ATEMPID, OracleDbType.Int32, tempId, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                }
                else
                {
                    recordsAffected = Convert.ToInt32(DeleteRegistryRecordTemporary(tempId));
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }

            return recordsAffected.ToString();
        }

        #endregion

        /// <summary>
        /// Retrieve the RegNum of the mixture housing a particular batch.
        /// </summary>
        /// <param name="batchId">the Batch identifier to search for</param>
        /// <returns>A registration number (RegNum) string</returns>
        public string GetRegNumberByBatchId(string batchId)
        {
            string paramName = "batchID";
            string sql = Resources.GetRegNumberByBatchIdSql
                + " WHERE vb.batchid = " + DALManager.BuildSqlStringParameterName(paramName);
            DbCommand dbCommand = null;
            string result = null;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, batchId);
                result = (string)DALManager.ExecuteScalar(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the number of temporary registration in the current queue.
        /// </summary>
        /// <param name="savedHitlistId">the identifier for a cached listing of search matches</param>
        /// <returns></returns>
        public int GetTempRecordCount(int savedHitlistId)
        {
            string paramName = "savedHitlistID";
            string sql = Resources.GetTempRecordCount
                + " WHERE hl.hitlistid = " + DALManager.BuildSqlStringParameterName(paramName);
            DbCommand dbCommand = null;
            int result = 0;

            try
            {
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, paramName, DbType.Int32, savedHitlistId);
                result = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = dbCommand;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(dbCommand);
            }

            return result;
        }

        /// <summary>
        /// Evaluate the 'Error' node of the message XML string
        /// </summary>
        private string EvaluateRegResponseForDuplicates(string xmlMessage)
        {
            string duplicates = string.Empty;

            XPathDocument xDocument = new XPathDocument(new StringReader(xmlMessage));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("Response/Error");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    duplicates = xIterator.Current.InnerXml;

            return duplicates;
        }

        /// <summary>
        /// Promotes an existing temporary registration to a permanent registration.
        /// </summary>
        /// <remarks>
        /// This method might better be called "PromoteTempToPerm"
        /// </remarks>
        /// <param name="temporaryId"></param>
        /// <param name="xml"></param>
        /// <param name="checkDuplicates"></param>
        /// <param name="actionDuplicates"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public RegistrationCrudResponse RegisterRegistryRecord(
            int temporaryId
            , string xml
            , DuplicateCheck checkDuplicates
            , DuplicateAction actionDuplicates
            )
        {
            RegistrationCrudResponse result = null;
            DbConnection conn = null;

            try
            {
                conn = GetCurrentConnection();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string duplicates = string.Empty;

                using (DbTransaction txn = conn.BeginTransaction())
                {
                    //In the user-interactive case, duplicate resolution has already occured
                    // and the object's XML will have already been updated
                    result = InsertRegistryRecord(xml, actionDuplicates, true);

                    //If we're promoting a temporary registration to a permanent one,
                    // eliminate the temporary one as soon as the permanent one has been handled
                    if (string.IsNullOrEmpty(result.Error) && temporaryId > 0)
                        DeleteRegistryRecordTemporary(temporaryId, true);

                    //transaction doesn't commit automatically; will be rolled back on exception
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyConnection(conn);
            }

            return result;
        }

        public RegistrationCrudResponse RegisterRegistryRecord(
            int temporaryId
            , string xml
            , DuplicateCheck checkDuplicates
            )
        {
            RegistrationCrudResponse result = null;
            DbConnection conn = null;

            try
            {
                conn = GetCurrentConnection();
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                string duplicates = string.Empty;
                using (DbTransaction txn = conn.BeginTransaction())
                {
                    //In the user-interactive case, duplicate resolution has already occured
                    // and the object's XML will have already been updated
                    result = InsertRegistryRecord(xml, checkDuplicates, true);

                    //Evaluate dismissal of the registration due to existing duplicates
                    duplicates = EvaluateRegResponseForDuplicates(result.RawDalMessage);
                    if (string.IsNullOrEmpty(duplicates) && temporaryId > 0)
                        DeleteRegistryRecordTemporary(temporaryId, true);

                    //The transaction doesn't commit automatically; will be rolled back on exception
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyConnection(conn);
            }

            return result;
        }

        [Description(BulkSupportDescription)]
        public RegistrationCrudResponse InsertRegistryRecord(
            string xml
            , DuplicateCheck checkDuplicates
            )
        {
            RegistrationCrudResponse result = null;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(CREATE_REGISTRATION, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, 50, string.Empty, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, string.Empty, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATECHECK, OracleDbType.Char, this.GetDuplicateCheckString(checkDuplicates), ParameterDirection.Input));
                    if (this._persistentConenctionWrapper != null)
                    {
                        //cmd.Parameters.Add(new OracleParameter(AREGNUMGENERATION, OracleDbType.Char, "Y", ParameterDirection.Input));
                        //cmd.Parameters.Add(new OracleParameter(ACONFIGURATIONID, OracleDbType.Int32, 1, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));
                    }
                    cmd.ExecuteNonQuery();
                    string message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    string regNum = cmd.Parameters[AREGNUMBER].Value.ToString();

                    result = new RegistrationCrudResponse(message, regNum, "N");
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return result;
        }

        [Description(WrappableTransactionDescription)]
        public RegistrationCrudResponse InsertRegistryRecord(
            string xml
            , DuplicateCheck checkDuplicates
            , bool callerControlsTransaction
            )
        {
            RegistrationCrudResponse result = null;

            if (callerControlsTransaction == true)
            {
                DbCommand cmd = CreateCommand(CREATE_REGISTRATION, CommandType.StoredProcedure);
                cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, 50, string.Empty, ParameterDirection.Output));
                cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, string.Empty, ParameterDirection.Output));
                cmd.Parameters.Add(new OracleParameter(ADUPLICATECHECK, OracleDbType.Char, this.GetDuplicateCheckString(checkDuplicates), ParameterDirection.Input));
                if (this._persistentConenctionWrapper != null)
                {
                    //cmd.Parameters.Add(new OracleParameter(AREGNUMGENERATION, OracleDbType.Char, "Y", ParameterDirection.Input));
                    //cmd.Parameters.Add(new OracleParameter(ACONFIGURATIONID, OracleDbType.Int32, 1, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));
                }
                cmd.ExecuteNonQuery();
                string message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                string regNum = cmd.Parameters[AREGNUMBER].Value.ToString();

                result = new RegistrationCrudResponse(message, regNum, "N");

            }
            else
            {
                result = InsertRegistryRecord(xml, checkDuplicates);
            }

            return result;
        }

        [Description(BulkSupportDescription)]
        public RegistrationCrudResponse InsertRegistryRecord(
            string xml
            , DuplicateAction actionDuplicates
            )
        {
            RegistrationCrudResponse result = null;
            char chAction = 'U';
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(UPLOAD_REGISTRATION, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AREGISTRYXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATEACTION, OracleDbType.Char, this.GetDuplicateActionString(actionDuplicates), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AACTION, OracleDbType.Char, 1, chAction, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, string.Empty, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, 50, string.Empty, ParameterDirection.Output));
                    if (this._persistentConenctionWrapper != null)
                    {
                        //cmd.Parameters.Add(new OracleParameter(AREGISTRATION, OracleDbType.Char, "Y", ParameterDirection.Input));
                        //cmd.Parameters.Add(new OracleParameter(AREGNUMGENERATION, OracleDbType.Char, "Y", ParameterDirection.Input));
                        //cmd.Parameters.Add(new OracleParameter(ACONFIGURATIONID, OracleDbType.Int32, 1, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));
                    }
                    cmd.ExecuteNonQuery();
                    string message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    string actionTaken = cmd.Parameters[AACTION].Value.ToString();
                    string redIdOrNumber = cmd.Parameters[AREGNUMBER].Value.ToString();

                    result = new RegistrationCrudResponse(message, redIdOrNumber, actionTaken);
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return result;
        }

        [Description(WrappableTransactionDescription)]
        private RegistrationCrudResponse InsertRegistryRecord(
            string xml
            , DuplicateAction actionDuplicates
            , bool callerControlsTransaction
            )
        {
            RegistrationCrudResponse result = null;
            char chAction = 'U';
            DbCommand cmd = null;

            try
            {
                if (callerControlsTransaction == true)
                {
                    cmd = CreateCommand(UPLOAD_REGISTRATION, CommandType.StoredProcedure);
                    cmd.Parameters.Add(new OracleParameter(AREGISTRYXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATEACTION, OracleDbType.Char, this.GetDuplicateActionString(actionDuplicates), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AACTION, OracleDbType.Char, 1, chAction, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, string.Empty, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, 50, string.Empty, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    string message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    string actionTaken = cmd.Parameters[AACTION].Value.ToString();
                    string redIdOrNumber = cmd.Parameters[AREGNUMBER].Value.ToString();

                    result = new RegistrationCrudResponse(message, redIdOrNumber, actionTaken);
                }
                else
                {
                    result = InsertRegistryRecord(xml, actionDuplicates);
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return result;
        }

        [Description(BulkSupportDescription)]
        public string GetRegistryRecord(string regNum)
        {
            string xml = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(READ_REGISTRATION, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, regNum, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    xml = ((OracleClob)cmd.Parameters[AXML].Value).Value as string;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return xml;
        }

        [Description(BulkSupportDescription)]
        public string GetRegistryRecordByBatch(string batchRegNum)
        {
            string xml = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(READ_REGISTRATION_BY_BATCH, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ABATCHREGNUMBER, OracleDbType.Varchar2, batchRegNum, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    xml = ((OracleClob)cmd.Parameters[AXML].Value).Value as string;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return xml;
        }

        [Description(BulkSupportDescription)]
        public string GetRegistryRecordByBatch(int batchId)
        {
            string xml = string.Empty;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(READ_REGISTRATION_BY_BATCH, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(ABATCHID, OracleDbType.Int32, batchId, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Output));
                    cmd.ExecuteNonQuery();
                    xml = ((OracleClob)cmd.Parameters[AXML].Value).Value as string;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return xml;
        }

        [Description(BulkSupportDescription)]
        public string UpdateRegistryRecord(
            string xml
            , DuplicateCheck checkDuplicates
            , out string message
            )
        {
            int recordsAffected = 0;
            DbCommand cmd = null;
            message = null;

            try
            {
                cmd = CreateCommand(UPDATE_REGISTRATION, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AXML, OracleDbType.Clob, xml, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter(AMESSAGE, OracleDbType.Clob, message = string.Empty, ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter(ADUPLICATECHECK, OracleDbType.Char, this.GetDuplicateCheckString(checkDuplicates), ParameterDirection.Input));
                    if (this._persistentConenctionWrapper != null)
                    {
                        //cmd.Parameters.Add(new OracleParameter(ACONFIGURATIONID, OracleDbType.Int32, 1, ParameterDirection.Input));
                        /* Allow the 'Update' to retrieve the new XML
                        cmd.Parameters.Add(new OracleParameter(ASECTIONSLIST, OracleDbType.Varchar2, NONE, ParameterDirection.Input));
                        */
                    }
                    recordsAffected = cmd.ExecuteNonQuery();
                    message = ((OracleClob)cmd.Parameters[AMESSAGE].Value).IsNull ? string.Empty : ((OracleClob)cmd.Parameters[AMESSAGE].Value).Value;
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                currentContext.RegistryRecordsXML = xml;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return recordsAffected.ToString();
        }

        [Description(BulkSupportDescription)]
        public string DeleteRegistryRecord(string regNum)
        {
            int recordsAffected = 0;
            DbCommand cmd = null;

            try
            {
                cmd = CreateCommand(DELETE_REGISTRATION, CommandType.StoredProcedure);
                //using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                using (DbTransaction txn = cmd.Connection.BeginTransaction())
                {
                    cmd.Parameters.Add(new OracleParameter(AREGNUMBER, OracleDbType.Varchar2, regNum, ParameterDirection.Input));
                    recordsAffected = cmd.ExecuteNonQuery();
                    //txnScope.Complete();
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionContext currentContext = new ExceptionContext();
                currentContext.Command = cmd;
                COEExceptionDispatcher.HandleDALException(ex, currentContext);
            }
            finally
            {
                DestroyCommand(cmd);
            }

            return recordsAffected.ToString();
        }

    }
}
