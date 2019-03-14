using System;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;

namespace CambridgeSoft.COE.Patcher.Repository
{
    /// <summary>
    /// The Repository that has an Oracle database behind the scene as the persistence.
    /// </summary>
    public class OracleConfigurationRepository : IConfigurationRepository, IDBCommunicator
    {
        private readonly string _allCoeFormSQL = string.Format("select coeform from {0}.coeform", Resource.COEDBSchema);
        private readonly string _allCoeDataviewsSQL = string.Format("select coedataview from {0}.coedataview", Resource.COEDBSchema);
        private readonly string _allConfigurationSettingSQL = string.Format("select configurationxml from {0}.coeconfiguration", Resource.COEDBSchema);
        private readonly string _coeObjectConfigSQL = string.Format("select xml from {0}.coeobjectconfig", Resource.REGDBSchema);
        private readonly string _singleCoeFormSQL = "select coeform from {0}.coeform where id = {1}";
        private readonly string _singleCoeDataViewSQL = "select coedataview from {0}.coedataview where id = {1}";

        OracleConnection conn = null;
        OracleCommand command = null;
        OracleDataReader reader = null;

        public OracleConfigurationRepository(string orclInstance, string userName, string password)
        {
            conn = new OracleConnection(
                string.Format("Data Source = {0};User Id = {1};Password = {2};",
                orclInstance, userName, password)
            );
        }

        #region IConfigurationRepository Members

        public IEnumerable<XmlDocument> GetAllCoeFormGroups()
        {
            IList<XmlDocument> allCoeForms = new List<XmlDocument>();

            try
            {
                if(conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = _allCoeFormSQL;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    XmlDocument coeForm = new XmlDocument();
                    string formString = reader.GetString(0);
                    if (!string.IsNullOrEmpty(formString))
                    {
                        coeForm.LoadXml(reader.GetString(0));
                        allCoeForms.Add(coeForm);
                    }
                }
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            return allCoeForms;
        }

        public IEnumerable<XmlDocument> GetAllCoeDataViews()
        {
            IList<XmlDocument> allCoeDataViews = new List<XmlDocument>();

            try
            {
                if(conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = _allCoeDataviewsSQL;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    XmlDocument coeForm = new XmlDocument();
                    coeForm.LoadXml(reader.GetString(0));
                    allCoeDataViews.Add(coeForm);
                }
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            return allCoeDataViews;
        }

        public IEnumerable<XmlDocument> GetAllConfigurationSettings()
        {
            IList<XmlDocument> allConfigurationSettings = new List<XmlDocument>();

            try
            {
                if(conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = _allConfigurationSettingSQL;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    XmlDocument coeForm = new XmlDocument();
                    coeForm.LoadXml(reader.GetString(0));
                    allConfigurationSettings.Add(coeForm);
                }
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            return allConfigurationSettings;
        }

        public XmlDocument GetCoeObjectConfig()
        {
            XmlDocument coeObjectConfig = new XmlDocument();

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            command = conn.CreateCommand();
            command.CommandText = _coeObjectConfigSQL;
            string objConfigStr = command.ExecuteScalar() as string;
            coeObjectConfig.LoadXml(objConfigStr);

            return coeObjectConfig;
        }

        public XmlDocument GetFrameworkConfig()
        {
            throw new NotSupportedException(@"Currently we get framework configuration from file system.
        Use FileSystemConfigurationRepository class instead.");
        }

        public XmlDocument GetSingleCoeFormGroupById(string formGroupId)
        {
            XmlDocument coeFormGroup = new XmlDocument();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = string.Format(_singleCoeFormSQL, Resource.COEDBSchema, formGroupId);
                string coeFormGroupXml = command.ExecuteScalar() as string;
                coeFormGroup.LoadXml(coeFormGroupXml);

                return coeFormGroup;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public XmlDocument GetSingleCOEDataViewById(string dataViewId)
        {
            XmlDocument coeDataView = new XmlDocument();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = string.Format(_singleCoeDataViewSQL, Resource.COEDBSchema, dataViewId);
                string coeDataViewXml = command.ExecuteScalar() as string;
                coeDataView.LoadXml(coeDataViewXml);

                return coeDataView;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataSet ExecuteDataset(string sql)
        {
            DataSet odsData = new DataSet();
            OracleDataAdapter odadp = new OracleDataAdapter();
            try
            {
                //Coverity fix- CID 19412
                if (conn.State == ConnectionState.Closed)
                        conn.Open();
                command = conn.CreateCommand();
                command.CommandText = sql;
                odadp.SelectCommand = command;
                odadp.Fill(odsData);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
               if (conn != null)
                    conn.Close();
            }
            return odsData;
        }

        public string ExecuteScaler(string sql)
        {
            object retVal = null;
            try
            {
                //Coverity fix- CID 19413
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = sql;
                retVal = command.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (retVal == null)
                    retVal = string.Empty;
                if (conn != null)
                    conn.Close();
            }
            return retVal.ToString();
        }

        public int ExecuteNonQuery(string sql)
        {
            object retVal = null;
            try
            {
                //Coverity fix- CID 19414
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                command = conn.CreateCommand();
                command.CommandText = sql;
                retVal = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (retVal == null)
                    retVal = -1;
                if (conn != null)
                    conn.Close();
            }
            return Convert.ToInt32(retVal);
        }

        #endregion
    }
}