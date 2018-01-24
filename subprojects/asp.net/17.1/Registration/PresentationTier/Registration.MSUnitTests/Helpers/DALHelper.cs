using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Reflection;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Configuration;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// Helper class for database operation
    /// </summary>
    public static class DALHelper
    {
        #region Properties

        /// <summary>
        /// Get Database connection to execute Query
        /// </summary>
        public static Database TheDataBase
        {
            get { return new OracleDatabase(ConfigurationManager.ConnectionStrings["coedbConn"].ConnectionString); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to update settings value 
        /// </summary>
        /// <param name="Key">Setting Key</param>
        /// <param name="Value">Setting Value</param>
        /// <param name="ParentKey">Optional parent settings key</param>
        /// <param name="MultiKeyValuePair">Optional Multi setting key value pair</param>
        public static void SetRegistrationSettingsONOrOFF(string Key, string Value, string ParentKey = "REGADMIN", Dictionary<string, string> MultiKeyValuePair = null)
        {
            UpdateXmlFile(ParentKey, Key, Value, MultiKeyValuePair);
        }

        /// <summary>
        /// Method to execute input Query and returning DataTable as output
        /// </summary>
        /// <param name="strQuery">SQL query to be executed</param>
        /// <returns>Returning datatable</returns>
        public static DataTable ExecuteQuery(string strQuery)
        {
            try
            {
                DataSet ds = TheDataBase.ExecuteDataSet(CommandType.Text, strQuery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updating settings in Registration XML and finally updating registration xml into the DataBase. 
        /// </summary>
        /// <param name="parentKey">Parent setting key</param>
        /// <param name="settingKey">Setting key</param>
        /// <param name="Value">Setting value</param>
        /// <param name="multiKeyValuePair">If more that one settings requrie to update then user this parameter</param>
        private static void UpdateXmlFile(string parentKey, string settingKey, string Value, Dictionary<string, string> multiKeyValuePair)
        {
            #region Loading Registration xml and Traversing through it

            XmlDocument theXmlDocument = new XmlDocument();
            theXmlDocument.Load(string.Format(@"{0}\{1}", Assembly.GetExecutingAssembly().Location.Substring(0,
                Assembly.GetExecutingAssembly().Location.IndexOf("TestResults")), @"Registration.MSUnitTests\Xml_Files\ConfigurationSettings.xml"));

            XmlNode theXmlNode = theXmlDocument.SelectSingleNode("./Registration/applicationSettings/groups/add[@name='" + parentKey + "']/settings");

            if (multiKeyValuePair != null && multiKeyValuePair.Count > 0)
            {
                foreach (KeyValuePair<string, string> settingKeyValuePair in multiKeyValuePair)
                {
                    foreach (XmlNode theChildNode in theXmlNode.ChildNodes)
                    {
                        if (theChildNode.Attributes["name"].Value.Equals(settingKeyValuePair.Key))
                        {
                            theChildNode.Attributes["value"].Value = settingKeyValuePair.Value;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode theChildNode in theXmlNode.ChildNodes)
                {
                    if (theChildNode.Attributes["name"].Value.Equals(settingKey))
                    {
                        theChildNode.Attributes["value"].Value = Value;
                        break;
                    }
                }
            }

            #endregion

            #region Deleting Registration configuration

            DbCommand dbCommand = TheDataBase.GetStoredProcCommand("COEDB.ConfigurationManager.DeleteConfiguration");
            OracleParameter descriptionParam = new OracleParameter("ADESCRIPTION", OracleDbType.Varchar2, "Registration", ParameterDirection.Input);
            dbCommand.Parameters.Add(descriptionParam);

            try
            {
                dbCommand.Connection = TheDataBase.CreateConnection();
                dbCommand.Connection.Open();
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }

            #endregion

            #region Adding Registration configuration

            dbCommand = TheDataBase.GetStoredProcCommand("COEDB.ConfigurationManager.InsertConfiguration");

            descriptionParam = new OracleParameter("ADESCRIPTION", OracleDbType.Varchar2, "Registration", ParameterDirection.Input);
            OracleParameter handlerClassNameParam = new OracleParameter("ACLASSNAME", OracleDbType.Varchar2, "CambridgeSoft.COE.Framework.Common.ApplicationDataConfigurationSection, CambridgeSoft.COE.Framework, Version=12.1.0.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf", ParameterDirection.Input);
            OracleParameter sectionParam = new OracleParameter("ACONFIGURATION", OracleDbType.Clob, theXmlDocument.InnerXml, ParameterDirection.Input);

            dbCommand.Parameters.Add(descriptionParam);
            dbCommand.Parameters.Add(handlerClassNameParam);
            dbCommand.Parameters.Add(sectionParam);
            try
            {
                dbCommand.Connection = TheDataBase.CreateConnection();
                dbCommand.Connection.Open();
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DALUtils.DestroyCommandAndConnection(dbCommand);
            }

            #endregion
        }

        #endregion
    }
}
