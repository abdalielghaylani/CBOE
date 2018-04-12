using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;

namespace PreCheck_Installation
{
    class PreCheckMSOffice : IPreCheck 
    {
         #region IPreCheck Members

        #region private variables
       
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
        public PreCheckMSOffice()
        {

          PreCheckName = "Check MS Office Version";
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
            PreCheckSubList.Add(0, "MS Office Version");
            
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add(1, "2003");
           
        }

        public void GetActualResult()
        {
            try
            {
                string strMSOfficeVersion = GetMSOfficeVersion();
                if (strMSOfficeVersion.Contains("2003"))
                {
                    ActualValue.Add(0, "2003");
                }
                else if (strMSOfficeVersion.Contains("2007"))
                {
                    ActualValue.Add(0, "2007");
                }
                else if (strMSOfficeVersion.Contains("2010"))
                {
                    ActualValue.Add(0, "2010");
                }
                else
                {
                    ActualValue.Add(0, PreCheckerGlobals.PreCheckerResults.Unknown.ToString());
                }
               
               
                
            }
            catch (Exception ex)
            {
                Console.Write( ex.Message);
                
            }
        }
           
        public void CompareExpectedResult()
        {
            string strMSOfficeVersionStatus = CompareMSOfficeVersionExpectedvalue();
            Status.Add(0, strMSOfficeVersionStatus);
            
        }

        public void DisplayMessage()
        {
            string strMSOfficeVersionDisplayMessage = MSOfficeVersionDisplayMessage();
            Message.Add(0, strMSOfficeVersionDisplayMessage);
           
        }

        #endregion
       
        #region private methods

        #region private methods related to GetActualResult

        private string GetMSOfficeVersion()
        {
            string strPathResult = "";        
            string strVersionResult = "";          
            string strKeyName = "Path";           
            object objResult = null;          
            Microsoft.Win32.RegistryValueKind regValueKind;          
            Microsoft.Win32.RegistryKey regKey = null;         
            Microsoft.Win32.RegistryKey regSubKey = null;          
            try           
            {               
                regKey = Microsoft.Win32.Registry.LocalMachine;

                if (regSubKey == null)
                {//Office2003                    
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\11.0\Common\InstallRoot", false);
                    strVersionResult = "office2003";
                    strKeyName = "Path";
                    objResult = regSubKey.GetValue(strKeyName);
                    if (objResult != null)
                    {
                        regValueKind = regSubKey.GetValueKind(strKeyName);
                        if (regValueKind == Microsoft.Win32.RegistryValueKind.String)
                        {
                            strPathResult = objResult.ToString();
                        }
                    }
                    else
                    {
                        regSubKey = null;
                    }
                }                
                if (regSubKey == null)            
                {//office2007                  
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Common\InstallRoot", false);  
                    strVersionResult = "office2007";               
                    strKeyName = "Path";
                    objResult = regSubKey.GetValue(strKeyName);
                    if (objResult != null)
                    {
                        regValueKind = regSubKey.GetValueKind(strKeyName);
                        if (regValueKind == Microsoft.Win32.RegistryValueKind.String)
                        {
                            strPathResult = objResult.ToString();
                        }
                    }
                    else
                    {
                        regSubKey = null;
                    }
                }
                if (regSubKey == null)
                {//office2010                 
                    regSubKey = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Common\InstallRoot", false);
                    strVersionResult = "office2010";
                    strKeyName = "Path";
                    objResult = regSubKey.GetValue(strKeyName);
                    if (objResult != null)
                    {
                        regValueKind = regSubKey.GetValueKind(strKeyName);
                        if (regValueKind == Microsoft.Win32.RegistryValueKind.String)
                        {
                            strPathResult = objResult.ToString();
                        }
                    }
                    else
                    {
                        regSubKey = null;
                    }
                }    
                       
            }           
            catch (System.Security.SecurityException ex)       
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString(); 
            }            
            catch (Exception ex)        
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();      
            }           
            finally        
            {              
                if (regKey != null)          
                {                  
                    regKey.Close();           
                    regKey = null;            
                }               
                if (regSubKey != null)            
                {                   
                    regSubKey.Close();          
                    regSubKey = null;         
                }         
            }           
            string Path = strPathResult;      
            string  Version = strVersionResult;     
            return Version;
        }       
       
                            
        

        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareMSOfficeVersionExpectedvalue()
         {
                      string strStatus = "";
                      try
                        {
                                          
                            Hashtable htActualResult = ActualValue;
                            int actualResult = Convert.ToInt32(htActualResult[0]);
                            int expectedValue = Convert.ToInt32(ExpectedValueList[1]);
                            if (actualResult >= expectedValue)
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
        private string MSOfficeVersionDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Version of MS Office Version is available";
                    
                    
                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected ersion of MS Office Version is not available";

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
