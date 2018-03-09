using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Oracle.DataAccess.Client;
using System.Reflection;
using CambridgeSoft.COE.Patcher.Repository;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Controls the workflow given user input.
    /// </summary>
    public class PatchController
    {
        #region Private Variables
        private string _fwConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\PerkinElmer\ChemOfficeEnterprise\COEFrameworkConfig.xml";
        private string[] _csbrList = new string[0];
        private string _connectionString = string.Empty;
        private string _coeFormSQL = string.Format("select coeform from {0}.coeform", Resource.COEDBSchema);
        private string _coeDataviewsSQL = string.Format("select coedataview from {0}.coedataview", Resource.COEDBSchema);
        private string _coeConfigurationSQL = string.Format("select configurationxml from {0}.coeconfiguration", Resource.COEDBSchema);
        private string _coeObjectConfigSQL = string.Format("select xml from {0}.coeobjectconfig", Resource.REGDBSchema);
        private StringBuilder _log = new StringBuilder();
        private XmlDocument _coeFrameworkConfig = new XmlDocument();
        private List<XmlDocument> _coeForms = new List<XmlDocument>();
        private List<XmlDocument> _coeDataviews = new List<XmlDocument>();
        private List<XmlDocument> _coeConfigurations = new List<XmlDocument>();
        private XmlDocument _coeObjectConfig = new XmlDocument();
        private string _fileName = "PatchLog.txt";

        private IConfigurationRepository _oracleConfigurationRepository = null;
        private IConfigurationRepository _fileSystemConfigurationRepository = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Performed actions log
        /// </summary>
        public string Log
        {
            get { return _log.ToString(); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes its members given user input. The only optional parameter is listPath, which can be empty, meaning that all known csbrs would be run.
        /// </summary>
        /// <param name="orclInstance">Oracle instance name</param>
        /// <param name="userName">Oracle user name</param>
        /// <param name="password">Oracle password</param>
        /// <param name="listPath">Path to a text file containing the list of csbrs to run</param>
        public PatchController(string orclInstance, string userName, string password, string listPath)
        {
            _connectionString = string.Format("Data Source = {0};User Id = {1};Password = {2};", orclInstance, userName, password);
            if (!string.IsNullOrEmpty(listPath))
            {
                _fileName = Path.GetFileNameWithoutExtension(listPath) + "_PatchLog.txt";
                _csbrList = File.ReadAllLines(listPath);
            }
            _log.AppendLine(string.Format("User Input: ({0}), ({1}), ({2}), ({3})", orclInstance, userName, "***************", listPath));

            _oracleConfigurationRepository = new OracleConfigurationRepository(
                orclInstance,
                userName,
                password);
            ExecuteSql.OracleConfigurationRepository = (IDBCommunicator)_oracleConfigurationRepository;
            _fileSystemConfigurationRepository = new FileSystemConfigurationRepository(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\PerkinElmer\ChemOfficeEnterprise");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Iterates over the list of csbr to fix and calls its method FIX(), logging the messages returned by bugfixcommands as needed.
        /// </summary>
        public void Patch()
        {
            // Use Repository to retrieve all xmls.
            // FillOriginalXML();
            RetrieveOriginalXML();

            if(_csbrList.Length > 0)
            {
                foreach(string bug in _csbrList)
                {
                    if (!string.IsNullOrEmpty(bug) && !bug.StartsWith("//"))
                    {
                        BugFixBaseCommand command = Assembly.GetExecutingAssembly().CreateInstance("CambridgeSoft.COE.Patcher." + bug) as BugFixBaseCommand;
                        ExecuteFix(command);
                    }
                }
            }

            StreamWriter writer = File.CreateText(_fileName);
            writer.Write(Log);
            writer.Close();

            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\" + _fileName);
        }
        #endregion

        #region Private Methods
        private void ExecuteFix(BugFixBaseCommand command)
        {
            Console.WriteLine();
            Console.WriteLine("**************  PATCHING " + command.GetType().Name + " **************");
            Console.WriteLine();

            _log.AppendLine("");
            _log.AppendLine("**************  PATCHING " + command.GetType().Name + " **************");
            _log.AppendLine("");

            List<string> messages = new List<string>();

            try
            {
                messages = command.Fix(_coeForms, _coeDataviews, _coeConfigurations, _coeObjectConfig, _coeFrameworkConfig);
                this.Update();
                foreach(string msg in messages)
                {
                    Console.Write("\t");
                    Console.WriteLine(msg);

                    _log.Append("\t");
                    _log.AppendLine(msg);
                }
            }
            catch
            {
                Console.Write("\t");
                Console.WriteLine(command.GetType().Name + " was not applied due to errors saving the changes");

                _log.Append("\t");
                _log.AppendLine(command.GetType().Name + " was not applied due to errors saving the changes");
            }

        }

        private void Update()
        {
            OracleConnection conn = null;
            OracleCommand command = null;

            /* UPDATE COEFRAMEWORKCONFIG.XML */
            try
            {
                _coeFrameworkConfig.Save(_fwConfigPath);
            }
            catch 
            {
                _log.AppendLine("FrameworkConfig was not saved");
            }
            conn = new OracleConnection(_connectionString);

            /* UPDATE COEFORMS */
            foreach(XmlDocument doc in _coeForms)
            {
                string id = string.Empty;
                try
                {
                    id = doc.DocumentElement.Attributes["id"].Value;
                    conn.Open();
                    command = conn.CreateCommand();
                    command.CommandText = "update " + Resource.COEDBSchema + ".coeform set coeform=:xml where id=" + id;
                    command.Parameters.Add(new OracleParameter(":xml", doc.OuterXml));
                    command.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    _log.AppendLine(string.Format("COEForm ID='{0}' was not saved", id));
                }
                finally
                {
                    conn.Close();
                }
            }

            /* UPDATE COEDATAVIEWS */
            foreach(XmlDocument doc in _coeDataviews)
            {
                string id = string.Empty;
                try
                {
                    conn.Open();
                    command = conn.CreateCommand();
                    id = doc.DocumentElement.Attributes["dataviewid"] != null? doc.DocumentElement.Attributes["dataviewid"].Value:string.Empty;
                    if (string.IsNullOrEmpty(id) && doc.DocumentElement.Attributes["id"] != null)
                        id = doc.DocumentElement.Attributes["id"].Value;
                    if (!string.IsNullOrEmpty(id))
                    {
                        command.CommandText = "update " + Resource.COEDBSchema + ".coedataview set coedataview=:xml where id=" + id;
                        command.Parameters.Add(new OracleParameter(":xml", OracleDbType.XmlType, doc.OuterXml, System.Data.ParameterDirection.Input));
                        command.ExecuteNonQuery();
                    }
                }
                catch(Exception)
                {
                    _log.AppendLine(string.Format("COEDataview ID='{0}' was not saved", id));
                }
                finally
                {
                    conn.Close();
                }
            }

            /* UPDATE CONFIGURATION */
            foreach(XmlDocument doc in _coeConfigurations)
            {
                string configName = string.Empty;
                try
                {
                    configName = doc.DocumentElement.Name;
                    conn.Open();
                    command = conn.CreateCommand();
                    command.CommandText = "update " + Resource.COEDBSchema + ".coeconfiguration set configurationxml=:xml where upper(description)=upper('" + configName + "')";
                    command.Parameters.Add(new OracleParameter(":xml", doc.OuterXml));
                    command.Parameters[0].OracleDbType = OracleDbType.XmlType;
                    command.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    _log.AppendLine(string.Format("COEConfiguration with name='{0}' was not saved", configName));
                }
                finally
                {
                    conn.Close();
                }
            }

            /* UPDATE COEOBJECTCONFIG */
            try
            {
                conn.Open();
                command = conn.CreateCommand();
                command.CommandText = "update " + Resource.REGDBSchema + ".coeobjectconfig set xml=:xml where id=2";
                command.Parameters.Add(new OracleParameter(":xml", _coeObjectConfig.OuterXml));
                command.ExecuteNonQuery();
            }
            catch(Exception)
            {
                _log.Append("COEObjectConfig was not saved");
            }
            finally
            {
                conn.Close();
            }

        }

        private void RetrieveOriginalXML()
        {
            _coeForms = _oracleConfigurationRepository.GetAllCoeFormGroups() as List<XmlDocument>;
            _coeDataviews = _oracleConfigurationRepository.GetAllCoeDataViews() as List<XmlDocument>;
            _coeConfigurations = _oracleConfigurationRepository.GetAllConfigurationSettings() as List<XmlDocument>;
            _coeObjectConfig = _oracleConfigurationRepository.GetCoeObjectConfig();
            _coeFrameworkConfig = _fileSystemConfigurationRepository.GetFrameworkConfig();
        }
        #endregion
    }
}
