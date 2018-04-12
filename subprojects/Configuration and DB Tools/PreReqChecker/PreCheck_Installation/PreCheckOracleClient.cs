using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Management;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;

namespace PreCheck_Installation
{
    class PreCheckOracleClient:IPreCheck    
    {
        #region private varibales
        private double _dblOracleVersion;
        private double _dblODPVersion;
        private double _dblODACVersion;
        private double _dblMTSVersion;
        private string _preCheckName;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _serviceName = string.Empty;
        private Hashtable _preCheckSubList = new Hashtable();
        private Hashtable _status = new Hashtable();
        private Hashtable _expectedList = new Hashtable();
        private Hashtable _actualValue = new Hashtable();
        public Hashtable _message = new Hashtable();
       
        #endregion

        #region Properties
        public string PreCheckName
        {
            get
            {
                return _preCheckName;
            }

            set
            {
                _preCheckName = value;
            }
        }

        public Hashtable PreCheckSubList
        {
            get
            {
                return _preCheckSubList;
            }

            set
            {
                _preCheckSubList = value;
            }
        }

        public Hashtable Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        public Hashtable ExpectedValueList
        {
            get
            {
                return _expectedList;
            }

            set
            {
                _expectedList = value;
            }
        }

        public Hashtable ActualValue
        {
            get
            {
                return _actualValue;
            }

            set
            {
                _actualValue = value;
            }
        }

        public Hashtable Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
            }
        }

        #endregion 

        #region Constructor
        public PreCheckOracleClient(string UserName,string Password, string ServiceName)
        {

           PreCheckName = "Check Oracle Installation";
           this._userName = UserName.ToLower();
           this._password = Password.ToLower();
           this._serviceName = ServiceName.ToLower();
           this.AssignPreCheckSubList();
           this.AssignExpectedList();
           this.GetActualResult();
           this.CompareExpectedResult();
           this.DisplayMessage();
          
          
                       
                   
        }
        #endregion

        #region public abstract methods

        public void AssignPreCheckSubList()
        {
            PreCheckSubList = new Hashtable();
            PreCheckSubList.Add(0, "Check Oracle Client version");
            PreCheckSubList.Add(1, "Check ODP .Net  version");
            PreCheckSubList.Add(2, "Check Oracle MTS Services ");
            PreCheckSubList.Add(3, "Check SQLPLUS installed..");
            PreCheckSubList.Add(4, "Check Import utility installed..");
            PreCheckSubList.Add(5, "Check TNSPing..");
            PreCheckSubList.Add(6, "Check -	Oracle ODBC Driver version");
           
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add("1.0", "10.2.0.3");
            ExpectedValueList.Add("1.1", "10.2.0.4");
            ExpectedValueList.Add("2.0", "2.0");
            ExpectedValueList.Add("3.0", "11.1");
            ExpectedValueList.Add("4.0", "Yes");
            ExpectedValueList.Add("5.0", "Yes");
            ExpectedValueList.Add("6.0", "Yes");
            ExpectedValueList.Add("7.0", "10.2.0.3");
        }

        public void GetActualResult()
        {
            try
            {
                string strOracleClientVer = GetOracleClientVersion();
                ActualValue.Add(0, strOracleClientVer);
                string strODPNET = GetOracleODPVersion();
                ActualValue.Add(1, strODPNET);
                string strOracleServiceVersion = GetOracleMTSServiceVersion();
                ActualValue.Add(2, strOracleServiceVersion);
                string strCheckIsSQlPLUS = CheckIsSQlPLUS();
                ActualValue.Add(3, strCheckIsSQlPLUS);
                string strCheckIsImp = CheckIsIMP();
                ActualValue.Add(4, strCheckIsImp);
                string strCheckTNSPing = CheckTNSPing();
                ActualValue.Add(5, strCheckTNSPing);
                string strODACVersion = GetODACVersion();
                ActualValue.Add(6, strODACVersion);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);

            }
        }

        public void CompareExpectedResult()
        {
            string strOracleClientVerStatus = CompareOracleClientVerExpectedvalue();
            Status.Add(0, strOracleClientVerStatus);
            string strODPNETStatus = CompareODPNETvalue();
            Status.Add(1, strODPNETStatus);
            string strOracleMTSStatus = CompareOracleMTSVersion();
            Status.Add(2, strOracleMTSStatus);
            string strISSQlPlusStatus = CompareISSQlplus();
            Status.Add(3, strISSQlPlusStatus);
            string strIsImpStatus = CompareISImp();
            Status.Add(4, strIsImpStatus);
            string strISTNSPingStatus = CompareTNSPing();
            Status.Add(5, strISTNSPingStatus);
            string strODACStatus = CompareODACVerExpectedvalue();
            Status.Add(6, strODACStatus);
        }

        public void DisplayMessage()
        {
            string OracleClientVerDisplayMessage = this.OracleClientVersionDisplayMessage();
            Message.Add(0, OracleClientVerDisplayMessage);
            string ODPNETDisplayMessage = ODPNETDiaplayMessage();
            Message.Add(1, ODPNETDisplayMessage);
            string OracleMTSDisplayMessage = OracleMTSDiaplayMessage();
            Message.Add(2, OracleMTSDisplayMessage);
            string ISSqlPlusDisplayMessage = ISSQLPlusDiaplayMessage();
            Message.Add(3, ISSqlPlusDisplayMessage);
            string ISSImpDisplayMessage = ISImpDiaplayMessage();
            Message.Add(4, ISSImpDisplayMessage);
            string ISTNSPingDisplayMessage = ISTNSPingDiaplayMessage();
            Message.Add(5, ISTNSPingDisplayMessage);
            string ODACDisplayMessage = OracleODACVersionDisplayMessage();
            Message.Add(6, ODACDisplayMessage);
        }

        #endregion

        #region private methods

        #region private methods related to GetActualResult

        private string GetOracleClientVersion()
        {
            StringBuilder SBResult = new StringBuilder();
            try
            {
                string result;

           System.Diagnostics.ProcessStartInfo proces = new System.Diagnostics.ProcessStartInfo("tnsping.exe");
                proces.RedirectStandardOutput = true;
                proces.CreateNoWindow = true;
                proces.UseShellExecute = false;
                System.Diagnostics.Process bufor;
                bufor = System.Diagnostics.Process.Start(proces);
                System.IO.StreamReader Output = bufor.StandardOutput;
                bufor.WaitForExit(2000);
                if (bufor.HasExited)
                {
                    result = Output.ReadToEnd();
             if (result.ToUpper().Contains("32-BIT"))
                    {
                        SBResult.Append("Oracle Client: 32-bit");
                        
                    }
                    else
                    {
                        SBResult.Append("Oracle Client: 64-bit");
                    }

                    int verINT = result.IndexOf("Version", 0, result.Length);
                    
                    if (verINT != null)
                    {
                      _dblOracleVersion = Convert.ToDouble(result.Substring(verINT + "Version".Length + 1, 4));
                      SBResult.Append(result.Substring(verINT + "Version".Length + 1, 10));
                       
                    }


                }
                
                return SBResult.ToString();

            }
            catch (System.ComponentModel.Win32Exception w32e)
            {
                SBResult.Append("Missing Oracle Client. \n" +
                "Please install OracleDataBase or OracleClient \n" +
                "http://www.oracle.com/downloads");
                              
                return SBResult.ToString();
            }
            catch (Exception e)
            {
                               
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
            finally
            {
               
            }
        }

        private string GetOracleODPVersion()
        {

            string ODPVersion = "";
            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ORACLE\\KEY_OraDb11g_home1");
                if (parameters != null)
                {
                    string OracleHomePath = parameters.GetValue("ORACLE_HOME").ToString();
                    OracleHomePath = OracleHomePath + "\\ODP.NET\\bin\\2.x\\Oracle.DataAccess.dll";
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(OracleHomePath);

                    ODPVersion = myFileVersionInfo.FileVersion;

                    _dblODPVersion = Convert.ToDouble(ODPVersion.Substring(0, 4));
                }
                else
                {
                    _dblODPVersion = 0.0;
                    ODPVersion = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                }
                return ODPVersion;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }

        }

        private string GetOracleMTSServiceVersion()
        {
            string strPathResult = "";
            string strKeyName = "Path";
            object objResult = null;
            Microsoft.Win32.RegistryValueKind regValueKind;
            Microsoft.Win32.RegistryKey regKey = null;
            Microsoft.Win32.RegistryKey regSubKey = null;
            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine;

                if (regSubKey == null)
                {                    
            regSubKey = regKey.OpenSubKey(@"SOFTWARE\ORACLE\OracleMTSRecoveryService\Setup\Current Version", false);
                    strKeyName = "Version";
                    objResult = regSubKey.GetValue(strKeyName);
                    if (objResult != null)
                    {
                        regValueKind = regSubKey.GetValueKind(strKeyName);
                        if (regValueKind == Microsoft.Win32.RegistryValueKind.String)
                        {
                            strPathResult = objResult.ToString();
                            _dblMTSVersion = Convert.ToDouble(strPathResult.Substring(0, 4));
                        }
                    }
                    else
                    {
                        _dblMTSVersion = 0.0;
                        regSubKey = null;
                    }
                }
                return strPathResult;
            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
            
        }

        private string CheckIsSQlPLUS()
        {
         
            try
            {
               
                string result;
                string SQlPlusCheck="";
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                process.StartInfo.FileName = "sqlplus.exe";
                process.StartInfo.Arguments = _userName+"/" +_password+"@"+_serviceName;
                process.StartInfo.WorkingDirectory = "";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
              
                 bool isSqlplus= process.Start();
                 if (isSqlplus)
                 {
                     SQlPlusCheck = "Yes";
                 }
                 else
                 {
                     SQlPlusCheck = "NO";
                 }

                               
                return SQlPlusCheck;
            
             
            }
           
            catch (Exception e)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
            finally
            {
                
            }
        }

        private string CheckIsIMP()
        {

            try
            {
               
                string result;
                string SQlPlusCheck = "";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "imp.exe";
                process.StartInfo.Arguments = "system/manager2@orcl";

                process.StartInfo.WorkingDirectory = "";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;


                bool isSqlplus = process.Start();
                if (isSqlplus)
                {
                    SQlPlusCheck = "Yes";
                }
                else
                {
                    SQlPlusCheck = "NO";
                }


                return SQlPlusCheck;



            }

            catch (Exception e)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
            finally
            {
                //Console.ReadKey();
            }
        }

        private string CheckTNSPing()
        {

            try
            {

                string result;
                string SQlPlusCheck = "";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "tnsping.exe";
                process.StartInfo.Arguments = "orcl";

                process.StartInfo.WorkingDirectory = "";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;


                bool isSqlplus = process.Start();
                if (isSqlplus)
                {
                    SQlPlusCheck = "Yes";
                }
                else
                {
                    SQlPlusCheck = "NO";
                }


                return SQlPlusCheck;



            }

            catch (Exception e)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
            finally
            {
                //Console.ReadKey();
            }
        }

        private string GetODACVersion()
        {
            string ODACVersion = "";
            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ODBC\\ODBCINST.INI\\Oracle in OraDb11g_home1");
                if (parameters != null)
                {
                    string ODACPath = parameters.GetValue("Driver").ToString();
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(ODACPath);
                    
                    ODACVersion = myFileVersionInfo.FileVersion;
                    _dblODACVersion = Convert.ToDouble(ODACVersion.Substring(0, ODACVersion.Length -6));
                }
                else
                {
                    _dblODACVersion = 0.0;
                    ODACVersion = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                }
                return ODACVersion;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }
        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareOracleClientVerExpectedvalue()
        {
            string strStatus = "";
            try
            {

                Hashtable ActualResult = ActualValue;
                double Actualresult = _dblOracleVersion;
                string ExpectedValue =ExpectedValueList["1.0"].ToString();
                ExpectedValue = ExpectedValue.Substring(0, ExpectedValue.Length-4);
               // string strExpectedValue2 = ExpectedValueList["1.1"].ToString();
                double dblExceptedValue = Convert.ToDouble(ExpectedValue);
                if (Actualresult >= dblExceptedValue)
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                return strStatus;

            }
        }

        private string CompareODPNETvalue()
        {
            string strStatus = string.Empty;
            try
            {
                double Actualresult = _dblODPVersion;
                string strExpectedValue = ExpectedValueList["2.0"].ToString();
                double dblExceptedValue = Convert.ToDouble(strExpectedValue);
                if (Actualresult >= dblExceptedValue)
                {
                
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
               return strStatus;

            }
        }

        private string CompareOracleMTSVersion()
        {
            string strStatus = string.Empty;
            try
            {

                double Actualresult = _dblMTSVersion;
                string strExpectedValue = ExpectedValueList["3.0"].ToString();
                double dblExceptedValue = Convert.ToDouble(strExpectedValue);
                if (Actualresult >= dblExceptedValue)
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                return strStatus;

            }
        }

        private string CompareISSQlplus()
        {
            string strStatus = string.Empty;
            try
            {

                Hashtable ActualResult = ActualValue;
                string strActualresult = ActualResult[3].ToString();
                string strExpectedValue = ExpectedValueList["4.0"].ToString();
                if (strActualresult.Contains(strExpectedValue))
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                return strStatus;

            }
        }

        private string CompareISImp()
        {
            string strStatus = string.Empty;
            try
            {

                Hashtable ActualResult = ActualValue;
                string strActualresult = ActualResult[4].ToString();
                string strExpectedValue = ExpectedValueList["5.0"].ToString();
                if (strActualresult.Contains(strExpectedValue))
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                return strStatus;

            }
        }

        private string CompareTNSPing()
        {
            string strStatus = string.Empty;
            try
            {

                Hashtable ActualResult = ActualValue;
                string strActualresult = ActualResult[5].ToString();
                string strExpectedValue = ExpectedValueList["6.0"].ToString();
                if (strActualresult.Contains(strExpectedValue))
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                return strStatus;

            }
        }

        private string CompareODACVerExpectedvalue()
        {
            string strStatus = "";
            try
            {
                             
                double Actualresult = _dblODACVersion;
                string ExpectedValue = ExpectedValueList["7.0"].ToString();
                ExpectedValue = ExpectedValue.Substring(0, ExpectedValue.Length - 4);
                double dblExceptedValue = Convert.ToDouble(ExpectedValue);
                if (Actualresult >= dblExceptedValue)
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString();


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString();

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                return strStatus;

            }
        }
     

        #region

        #endregion

        #endregion

        #region private methods related to DisplayMessage
        private string OracleClientVersionDisplayMessage()
        {
          
            string displayResult;
            try
            {
                if (Status[0] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Oracle Client Version is available";


                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Oracle Client Version is not available";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }

        private string ODPNETDiaplayMessage()
        {
            string displayResult="";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Excepted ODPNET is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Excepted ODPNET is not available";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        private string OracleMTSDiaplayMessage()
        {
            string displayResult = "";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Oracle MTS Service is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Oracle MTS Service is not available";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        private string ISSQLPlusDiaplayMessage()
        {
            string displayResult = "";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "SqlPlus  is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "SqlPlus is not available";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        private string ISImpDiaplayMessage()
        {
            string displayResult = "";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Oracle import utility is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Oracle import utility is not available";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        private string ISTNSPingDiaplayMessage()
        {
            string displayResult = "";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "TNS Ping utility displays the required message ";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "TNS Ping utility does not displays the required message";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        private string OracleODACVersionDisplayMessage()
        {

            string displayResult;
            try
            {
                if (Status[6] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Oracle ODBC Driver version is available";


                }
                else if (Status[6] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Oracle ODBC Driver version is not available";

                }
                else
                {
                    displayResult = "Contact system admin...";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        #endregion
        #endregion


   

        
    }
}
