using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;

namespace PreCheck_Installation
{
    class PreCheckMDAC:IPreCheck 
    {
         #region IPreCheck Members

        #region private varibales
        private double _dblOMDACVersion;
        private string _preCheckName;
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
        public PreCheckMDAC()
        {


            PreCheckName = "MDAC Installed Version";
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
            PreCheckSubList.Add(0, "Check MDAC Version");
            
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add(1, "2.8");
           
        }

        public void GetActualResult()
        {
            try
            {
                string strMDACVersion = GetMDACVersion();
                ActualValue.Add(0, strMDACVersion);
               
                
            }
            catch (Exception ex)
            {
                Console.Write( ex.Message);
                
            }
        }
           
        public void CompareExpectedResult()
        {
            string strMDACVersionstatus = CompareMDACVersionExpectedvalue();
            Status.Add(0, strMDACVersionstatus);
            ;
        }

        public void DisplayMessage()
        {
            string strMDACVersionDisplayMessage = MDACVersionDisplayMessage();
            Message.Add(0, strMDACVersionDisplayMessage);
           
        }

        #endregion
       
        #region private methods

        #region private methods related to GetActualResult

        private string GetMDACVersion()
        {
            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\DataAccess");
                if (parameters != null)
                {
                    string Version = (string)parameters.GetValue("Version");
                    _dblOMDACVersion = Convert.ToDouble(Version.Substring(0, 3));
                    return Version;
                }
                else
                {
                    _dblOMDACVersion = 0.0;
                    return PreCheckerGlobals.PreCheckerResults.Unknown.ToString(); 
                
                }
            }
            catch(Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareMDACVersionExpectedvalue()
         {
                      string strStatus = "";
                      try
                        {

                            double actualResult = _dblOMDACVersion;
                            string strExpectedValue1 = ExpectedValueList[1].ToString();
                            double dblExceptedValue = Convert.ToDouble(strExpectedValue1);
                            if (actualResult >= dblExceptedValue)
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
        private string MDACVersionDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Version of MDAC is available";
                    
                    
                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Version of MDAC is not available";

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
