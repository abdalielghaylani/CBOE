using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;
using System.DirectoryServices;
namespace PreCheck_Installation
{
    class PreCheckIIS:IPreCheck
    {
        #region IPreCheck Members

        #region private varibales
        private int _actualIISVersion;
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
        public PreCheckIIS()
        {


           PreCheckName = "IIS Version";
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
            PreCheckSubList.Add(0, "IIS installed version");
            PreCheckSubList.Add(1, "Check Web Service Extensions for ASP.NET v.2.0.50727");
            PreCheckSubList.Add(2, "Check Web Service Extensions for Active Server Pages");
            PreCheckSubList.Add(3, "Check Web Service Extensions for Server Side Includes");
            PreCheckSubList.Add(4, "IIS Virtual Path");
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add(1.0, "IIS6");
            ExpectedValueList.Add(1.1, "IIS7");
            ExpectedValueList.Add(2.0, "Enabled");
            ExpectedValueList.Add(3.0, "Enabled");
            ExpectedValueList.Add(4.0, "Enabled");
            ExpectedValueList.Add(5.0, "\\Inetpub\\wwwroot");
        }

        public void GetActualResult()
        {
            try
            {
                string strIISVersion = GetIISVersion();
                ActualValue.Add(0, strIISVersion);
                string strWSASPNET = CheckWebServiceExtASPNETEnabled();
                ActualValue.Add(1, strWSASPNET);
                string strWSASP = CheckWebServiceExtASPEnabled();
                ActualValue.Add(2, strWSASP);
                string strWSSSI = CheckWebServiceExtSSIEnabled();
                ActualValue.Add(3, strWSSSI);
                string strVirtualPath = GetIISVirtualPath();
                ActualValue.Add(4, strVirtualPath);
                
            }
            catch (Exception ex)
            {
                Console.Write( ex.Message);
                
            }
        }
           
        public void CompareExpectedResult()
        {
            string strIISVersionStatus = CompareIISveVersionExpectedvalue();
            Status.Add(0, strIISVersionStatus);
            string strWebServiceExtASPNetStatus = CompareWebServiceExtASPNetExpectedvalue();
            Status.Add(1, strWebServiceExtASPNetStatus);
            string strWebServiceExtASPStatus = CompareWebServiceExtASPExpectedvalue();
            Status.Add(2, strWebServiceExtASPStatus);
            string strWebServiceExtSSIStatus = CompareWebServiceExtSSIExpectedvalue();
            Status.Add(3, strWebServiceExtSSIStatus);
            string strIISVirtualPathStatus = CompareVirtualPathExpectedvalue();
            Status.Add(4, strIISVirtualPathStatus);
        }

        public void DisplayMessage()
        {
            string strIISVersionDisplayMessage = IISVersionDisplayMessage();
            Message.Add(0, strIISVersionDisplayMessage);
            string strWebServiceExtASPNetDisplayMessage = CheckWebServiceExtASPNetDisplayMessage();
            Message.Add(1, strWebServiceExtASPNetDisplayMessage);
            string strWebServiceExtASPDisplayMessage = CheckWebServiceExtASPDisplayMessage();
            Message.Add(2, strWebServiceExtASPDisplayMessage);
            string strWebServiceExtSSIDisplayMessage = CheckWebServiceExtSSIDisplayMessage();
            Message.Add(3, strWebServiceExtSSIDisplayMessage);
            string strIISVirtualPathDisplayMessage = CheckVirtualPathDisplayMessage();
            Message.Add(4, strIISVirtualPathDisplayMessage);
        }

        #endregion
       
        #region private methods

        #region private methods related to GetActualResult

        private string GetIISVersion()
        {
            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters");
                int MajorVersion = (int)parameters.GetValue("MajorVersion");
                _actualIISVersion = MajorVersion;
                return "IIS" + MajorVersion.ToString();
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        private string CheckWebServiceExtASPNETEnabled()
        {
            try
            {
                string strASPNETWebServiceStatus = "Disabled";
                using (DirectoryEntry de = new DirectoryEntry("IIS://localhost/W3SVC"))
                {
                    foreach (string ext in de.Properties["WebSvcExtRestrictionList"])
                    {

                        if (ext.StartsWith("1,") && ext.IndexOf("ASP.NET v2.0") != -1)
                        {
                            strASPNETWebServiceStatus = "Enabled";
                        }
                    }
                }
                return strASPNETWebServiceStatus;

            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        private string CheckWebServiceExtASPEnabled()
        {
            try
            {
                string strASPNETWebServiceStatus = "Disabled";
                using (DirectoryEntry de = new DirectoryEntry("IIS://localhost/W3SVC"))
                {
                    foreach (string ext in de.Properties["WebSvcExtRestrictionList"])
                    {

                        if (ext.StartsWith("1,") && ext.IndexOf("Active Server Pages") != -1)
                        {
                            strASPNETWebServiceStatus = "Enabled";
                        }
                    }
                }
                return strASPNETWebServiceStatus;

            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        private string CheckWebServiceExtSSIEnabled()
        {
            try
            {
                string strASPNETWebServiceStatus = "Disabled";
                using (DirectoryEntry de = new DirectoryEntry("IIS://localhost/W3SVC"))
                {
                    foreach (string ext in de.Properties["WebSvcExtRestrictionList"])
                    {

                        if (ext.StartsWith("1,") && ext.IndexOf("Server Side Includes") != -1)
                        {
                            strASPNETWebServiceStatus = "Enabled";
                        }
                    }
                }
                return strASPNETWebServiceStatus;

            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        private string GetIISVirtualPath()
        {
            try
            {
   RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters\\Virtual Roots");
                string virtualPath = parameters.GetValue("/").ToString();
                virtualPath = virtualPath.Substring(0, virtualPath.Length - 5);
                return virtualPath;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareIISveVersionExpectedvalue()
         {
                      string strStatus = "";
                      try
                        {
                               
                            
                            int actualValue = _actualIISVersion;
                            int expectedValue = 6;

                            if (actualValue >= expectedValue)
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
        private string CompareWebServiceExtASPNetExpectedvalue()
        {
            string strStatus = "";
            try
            {


                string strActualValue = ActualValue[1].ToString();

                string strExpectedValue = ExpectedValueList[2.0].ToString();
                if (strActualValue.Contains(strExpectedValue))
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

        private string CompareWebServiceExtASPExpectedvalue()
        {
            string strStatus = "";
            try
            {


                string strActualValue = ActualValue[2].ToString();

                string strExpectedValue = ExpectedValueList[3.0].ToString();
                if (strActualValue.Contains(strExpectedValue))
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
        private string CompareWebServiceExtSSIExpectedvalue()
        {
            string strStatus = "";
            try
            {


                string strActualValue = ActualValue[3].ToString();

                string strExpectedValue = ExpectedValueList[4.0].ToString();
                if (strActualValue.Contains(strExpectedValue))
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
        private string CompareVirtualPathExpectedvalue()
        {
            string strStatus = "";
            try
            {


                string strActualValue = ActualValue[4].ToString();

                string strExpectedValue = ExpectedValueList[5.0].ToString();
                if (strActualValue.Contains(strExpectedValue))
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
        private string IISVersionDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Version of IIS is available";
                    
                    
                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Version of IIS is not available";

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
        private string CheckWebServiceExtASPNetDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Web Service Extensions for ASP.NET v.2.0.50727 is Enabled";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Web Service Extensions for ASP.NET v.2.0.50727 is not Enabled";

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
        private string CheckWebServiceExtASPDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[2] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Web Service Extensions for Active Server Pages is Enabled";


                }
                else if (Status[2] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Web Service Extensions for Active Server Pages is not Enabled";

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
        private string CheckWebServiceExtSSIDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[3] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Web Service Extensions for Server Side Includes is Enabled";


                }
                else if (Status[3] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Web Service Extensions for Server Side Includes is not Enabled";

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
        private string CheckVirtualPathDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[4] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "IIS virtual path is at expected path";


                }
                else if (Status[4] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "IIS virtual path is not at expected path";

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
