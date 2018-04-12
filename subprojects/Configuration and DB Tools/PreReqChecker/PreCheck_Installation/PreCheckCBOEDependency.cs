using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using System.Data.OracleClient;

namespace PreCheck_Installation
{
    class PreCheckCBOEDependency:IPreCheck
    {
         #region IPreCheck Members

        #region private variables
        private double _dblCartridgeVersion;
        private double _dblChemdrawVersion;
        private double _dblChemScriptVersion;
        private string _preCheckName;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _serviceName = string.Empty;
        private string _hostName = string.Empty;
        private Hashtable _preCheckSubList = new Hashtable();
        private Hashtable _status=new Hashtable();
        private Hashtable _expectedList = new Hashtable();
        private Hashtable _actualValue=new Hashtable();
        public Hashtable _message=new Hashtable();
        
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
        public PreCheckCBOEDependency(string userName, string password, string serviceName, string hostName)
        {


           PreCheckName = "CBOE Application Dependencies";

           this._userName = userName.ToLower();
           this._password = password.ToLower();
           this._serviceName = serviceName.ToLower();
           this._hostName = hostName;
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
            PreCheckSubList.Add(0, "Check CsCartridge Installation.");
            PreCheckSubList.Add(1, "Check ChemDraw Version.");
            PreCheckSubList.Add(2, "Check ChemScript Version.");
            
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add(1, "13.0.1.442");
            ExpectedValueList.Add(2, "12.0");
            ExpectedValueList.Add(3, "12.0");
           
        }

        public void GetActualResult()
        {
            try
            {
                string csCartridgeVersion = VerifyCsCartrigdeInstallation();
                ActualValue.Add(0, csCartridgeVersion);
                string checkDrawVersion = GetChemDrawVersion();
                ActualValue.Add(1, checkDrawVersion);
                string chemScriptVersion = GetChemScriptVersion();
                ActualValue.Add(2, chemScriptVersion);
                
            }
            catch (Exception ex)
            {
                Console.Write( ex.Message);
                
            }
        }
           
        public void CompareExpectedResult()
        {
            string csCartridgeStatus = CompareCsCartridgeVersionExpectedvalue();
            Status.Add(0, csCartridgeStatus);
            string chemDrawStatus = CompareChemDrawVersionExpectedvalue();
            Status.Add(1, chemDrawStatus);
            string chemScriptStatus = CompareChemScriptVersionExpectedvalue();
            Status.Add(2, chemScriptStatus);
        }

       

        public void DisplayMessage()
        {
            string csCartridgeVerDisplayMessage = CsCartridgeVersionDisplayMessage();
            Message.Add(0, csCartridgeVerDisplayMessage);
            string chemDrawDisplayMessage = ChemDrawVersionnDisplayMessage();
            Message.Add(1, chemDrawDisplayMessage);
            string chemScriptDisplayMessage = ChemScriptVersionnDisplayMessage();
            Message.Add(2, chemScriptDisplayMessage);
        }

        #endregion
       
        #region private methods

        #region private methods related to GetActualResult

        private string VerifyCsCartrigdeInstallation()
        {
            string cartridgeVersion = "";
            try
            {
             string connectionString = "Data Source=(DESCRIPTION="
             + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST="+_hostName+")(PORT=1521)))"
             + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME="+_serviceName+")));"
             + "User Id="+_userName+";Password="+_password+";";
              OracleConnection conn = new OracleConnection(connectionString);
          
              conn.Open();
              OracleCommand cmd = new OracleCommand("select value from cscartridge.globals where id='VERSION'", conn);
                
                OracleDataReader dr= cmd.ExecuteReader();
               
                

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                       cartridgeVersion= dr[0].ToString();
                      
                           
                    }
                }
                else
                {
                    return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                }
                
                dr.Close();
                conn.Close();
                return cartridgeVersion;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }
        private string GetChemDrawVersion()
        {
            string ChemDrawVersion = "";
            try
            {
              RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\CambridgeSoft\\ChemDraw\\12.0");
                if (parameters != null)
                {
                    _dblChemdrawVersion = 12.0;
                    ChemDrawVersion = "12.0";
                }
                else
                {
                    _dblChemdrawVersion = 0.0;
                    ChemDrawVersion = PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                }
                return ChemDrawVersion;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }
        private string GetChemScriptVersion()
        {
            string ChemScriptVersion = "";
            try
            {
        RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\CambridgeSoft\\ChemScript\\12.0");
                if (parameters != null)
                {
                    _dblChemScriptVersion = 12.0;
                    ChemScriptVersion = "12.0";
                }
                else
                {
                    _dblChemScriptVersion = 0.0;
                    ChemScriptVersion =  PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                }
                return ChemScriptVersion;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareCsCartridgeVersionExpectedvalue()
         {
             string strStatus = "";
             try
             {

                 Hashtable ActualResult = ActualValue;
                 double Actualresult = _dblCartridgeVersion;
                 string ExpectedValue = ExpectedValueList[1].ToString();
                 ExpectedValue = ExpectedValue.Substring(0,4);
                 
                 double dblExceptedValue = Convert.ToDouble(ExpectedValue);
                 if (Actualresult >= dblExceptedValue)
                 {
                     strStatus =PreCheckerGlobals.PreCheckerResults.Success.ToString();


                 }
                 else
                 {
                     strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString(); ;

                 }
                 return strStatus;

             }
             catch (Exception ex)
             {
                 Console.Write(ex.Message);
                 strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString(); ;
                 return strStatus;

             }
        }

        private string CompareChemDrawVersionExpectedvalue()
        {
            string strStatus = "";
            try
            {

                Hashtable ActualResult = ActualValue;
                double Actualresult = _dblChemdrawVersion;
                string ExpectedValue = ExpectedValueList[2].ToString();
                ExpectedValue = ExpectedValue.Substring(0, ExpectedValue.Length);

                double dblExceptedValue = Convert.ToDouble(ExpectedValue);
                if (Actualresult >= dblExceptedValue)
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString(); ;


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString(); ;

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString(); ;
                return strStatus;

            }
        }

        private string CompareChemScriptVersionExpectedvalue()
        {
            string strStatus = "";
            try
            {

                Hashtable ActualResult = ActualValue;
                double Actualresult = _dblChemScriptVersion;
                string ExpectedValue = ExpectedValueList[2].ToString();
                ExpectedValue = ExpectedValue.Substring(0, ExpectedValue.Length);

                double dblExceptedValue = Convert.ToDouble(ExpectedValue);
                if (Actualresult >= dblExceptedValue)
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Success.ToString(); 


                }
                else
                {
                    strStatus = PreCheckerGlobals.PreCheckerResults.Fail.ToString(); ;

                }
                return strStatus;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                strStatus = PreCheckerGlobals.PreCheckerResults.Unknown.ToString(); ;
                return strStatus;

            }
        }


        #region

        #endregion

        #endregion

        #region private methods related to DisplayMessage
        private string CsCartridgeVersionDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Version of CsCatridge is available";
                    
                    
                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Version of .CsCatridge is not available";

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
        private string ChemDrawVersionnDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[1] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Version of ChemDraw is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Version of ChemDraw is not available";

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
        private string ChemScriptVersionnDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[2] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Version of ChemScript is available";


                }
                else if (Status[2] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Version of ChemScript is not available";

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

        #endregion
    }
}
