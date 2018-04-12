using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Win32;

namespace PreCheck_Installation
{
    class PreCheckOS:IPreCheck 
    {
        #region IPreCheck Members

        #region private varibales
        private double _dblWinServiceVersion;
        private string _operatingSystem = "";
        private string _userAccountName="";
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
        public PreCheckOS()
        {
           
           
            PreCheckName = "Operating System";
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
            PreCheckSubList.Add(0, "OS Version");
            PreCheckSubList.Add(1, "Windows Install Service");
            //PreCheckSubList.Add(2, "OS Patch");
            PreCheckSubList.Add(2, "Verify Windows Administrator User Account");
            PreCheckSubList.Add(3, "Windows Administrator");
            PreCheckSubList.Add(4, "Check OS Hotfix KB925366 Installation");
            PreCheckSubList.Add(5, "Check OS Hotfix KB942288 Installation");
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add(1.0, "Windows Server 2003");
            ExpectedValueList.Add(1.1, "Windows Server 2008");
            ExpectedValueList.Add(2.0, "4.5");
            ExpectedValueList.Add(3.0, "Yes");
            ExpectedValueList.Add(4.0, "Yes");
            ExpectedValueList.Add(5.0, "Yes");
            ExpectedValueList.Add(6.0, "Yes");
        }

        public void GetActualResult()
        {
            try
            {
                string oSVersion = GetOSVersion();
                ActualValue.Add(0, oSVersion);
                string winService45 = GetWinService45();
                ActualValue.Add(1, winService45);
                string winAccount = GetWinUserAccount();
                ActualValue.Add(2, winAccount);
                string verDocSetting = VerifyDocSettings();
                ActualValue.Add(3, verDocSetting);

                if (_operatingSystem.Contains("Windows Server 2003"))
                {
                string strVerHotFixKB925366 = VerifyOSHotFix_KB925366();
                ActualValue.Add(4, strVerHotFixKB925366);
                string strVerHotFixKB942288 = VerifyOSHotFix_KB942288();
                ActualValue.Add(5, strVerHotFixKB942288);
                }
                else
                {
                    string strVerHotFixKB925366 = " This Hot fix is not required for other then Windows 2003";
                    ActualValue.Add(4, strVerHotFixKB925366);
                    string strVerHotFixKB942288 = " This Hot fix is not required for other then Windows 2003";
                    ActualValue.Add(5, strVerHotFixKB942288);
                }
            }
            catch (Exception ex)
            {
                Console.Write( ex.Message);
                
            }
        }
       
 
        public void CompareExpectedResult()
        {
            string oSStatus = CompareOSversionExpectedvalue();
            Status.Add(0, oSStatus);
            string win4Status = CompareWin45Expectedvalue();
            Status.Add(1, win4Status);
            string winUserAccountStatus = CompareWinAdminAccountExpectedvalue();
            Status.Add(2, winUserAccountStatus);
            string verDocAndSettingStatus = VerifyDocAndSettingsExpectedvalue();
            Status.Add(3, verDocAndSettingStatus);
            string oSHotFixKB925366Status = CompareOSHotFixKB925366Expectedvalue();
            Status.Add(4, oSHotFixKB925366Status);
            string oSHotFixKB942288Status = CompareOSHotFixKB942288Expectedvalue();
            Status.Add(5, oSHotFixKB942288Status);
        }

        public void DisplayMessage()
        {
            string osDisplayMessage = OSDisplayMessage();
            Message.Add(0, osDisplayMessage);
            string win45DisplayMessage = Win45DiaplayMessage();
            Message.Add(1, win45DisplayMessage);
            string winAdminAccountDisplayMessage = WinAdminAccountDiaplayMessage();
            Message.Add(2, winAdminAccountDisplayMessage);
            string verDocAndSesttingDisplayMessage = VerifyDocandSettingsDiaplayMessage();
            Message.Add(3, verDocAndSesttingDisplayMessage);
            string oSHotFixKB925366DisplayMessage = OSHotFixKB9253661DisplayMessage();
            Message.Add(4, oSHotFixKB925366DisplayMessage);
            string oSHotFixKB942288DisplayMessage = OSHotFixKB942288DisplayMessage();
            Message.Add(5, oSHotFixKB942288DisplayMessage);
        }
       

        #endregion
       
        #region private methods

        #region private methods related to GetActualResult

        private string GetOSVersion()
        {
            try
            {
                 
       RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
                _operatingSystem = parameters.GetValue("ProductName").ToString();
                //ActualIISVersion = MajorVersion;
                return _operatingSystem;
            }
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }

        private string GetWinService45()
        {
            FileVersionInfo info;
            string fileName = Path.Combine(Environment.SystemDirectory, "msi.dll");
            try
            {
               info = FileVersionInfo.GetVersionInfo(fileName);
               string winServiceVersion = info.FileVersion;
               _dblWinServiceVersion = Convert.ToDouble(winServiceVersion.Substring(0, winServiceVersion.Length - 11));
               return winServiceVersion;
            }
            catch (FileNotFoundException)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
            
            //return (info.FileMajorPart > 4 || info.FileMajorPart == 4 && info.FileMinorPart >= 5); 
        }
        private string GetWinUserAccount()
        {
           WindowsIdentity identity = WindowsIdentity.GetCurrent();
           WindowsPrincipal principal = new WindowsPrincipal(identity);
            string role = "BUILTIN\\Administrators";
            bool isAdmin = principal.IsInRole(role);
            if (isAdmin)
            {
                _userAccountName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

                
                return "Yes";
            }
            else
            {
            return "No";
            }

        }
        private string VerifyDocSettings()
        {
            String docAndSettingPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            docAndSettingPath=docAndSettingPath.Substring(0,docAndSettingPath.Length-35);
            bool exists = System.IO.Directory.Exists(docAndSettingPath);
            if (exists)
            {
                
               return "Yes";
            }
            else
            {
                return "No";
            }

        }

        private string VerifyOSHotFix_KB925366()
        {
            string verfiyHotFix = "";

            try
            {
  RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\HotFix\\KB925336");

                if (parameters != null)
                {
                    verfiyHotFix = parameters.GetValue("Installed").ToString();
                    if (verfiyHotFix == "1")
                    { 
                     verfiyHotFix="Yes";
                    }
                    else
                        {
                    verfiyHotFix="NO";    
                        }
                    }
                else
                {
                
                verfiyHotFix="NO";
                }

                return verfiyHotFix;
                }
                //ActualIISVersion = MajorVersion;
                
            
            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }
        private string VerifyOSHotFix_KB942288()
        {
            string verfiyHotFix = "";

            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\HotFix\\KB942288-v3");

                if (parameters != null)
                {
                    verfiyHotFix = parameters.GetValue("Installed").ToString();
                    if (verfiyHotFix == "1")
                    {
                        verfiyHotFix = "Yes";
                    }
                    else
                    {
                        verfiyHotFix = "NO";
                    }
                }
                else
                {

                    verfiyHotFix = "NO";
                }

                return verfiyHotFix;
            }
            //ActualIISVersion = MajorVersion;


            catch (Exception ex)
            {

                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }
       
        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareOSversionExpectedvalue()
         {
                      string strStatus = "";
                      try
                        {
                                          
                            Hashtable actualResult = ActualValue;
                            string strActualresult = actualResult[0].ToString();
                            string strExpectedValue1 = ExpectedValueList[1.0].ToString();
                            string strExpectedValue2 = ExpectedValueList[1.1].ToString();
                    if (strActualresult.Contains(strExpectedValue1) || strActualresult.Contains(strExpectedValue2))
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

        private string CompareWin45Expectedvalue()
        {
            string strStatus = string.Empty;
            try
            {
                double actualResult = _dblWinServiceVersion;
                string expectedValue = ExpectedValueList[2.0].ToString();
                double dblExceptedValue = Convert.ToDouble(expectedValue);

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

        private string CompareWinAdminAccountExpectedvalue()
        {
            string strStatus = string.Empty;
            try
            {

                Hashtable actualResult = ActualValue;
                string strActualresult = actualResult[2].ToString();
                string strExpectedValue = ExpectedValueList[3.0].ToString();
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

        private string VerifyDocAndSettingsExpectedvalue()
        {
            string strStatus = string.Empty;
            try
            {

                Hashtable actualResult = ActualValue;
                string strActualresult = actualResult[3].ToString();
                string strExpectedValue = ExpectedValueList[4.0].ToString();
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

        private string CompareOSHotFixKB925366Expectedvalue()
        {
            string strStatus = "";
            try
            {

                Hashtable actualResult = ActualValue;
                string strActualresult = actualResult[4].ToString();
                string strExpectedValue1 = ExpectedValueList[5.0].ToString();
                
                if (strActualresult.Contains(strExpectedValue1))
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

        private string CompareOSHotFixKB942288Expectedvalue()
        {
            string strStatus = "";
            try
            {

                Hashtable actualResult = ActualValue;
                string strActualresult = actualResult[5].ToString();
                string strExpectedValue1 = ExpectedValueList[6.0].ToString();

                if (strActualresult.Contains(strExpectedValue1))
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

        private string OSDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected OS is available";
                    
                    
                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected OS is not available";

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

        private string Win45DiaplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "System has Windows service install 4.5 available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Windows service install 4.5 is not available";

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

        private string WinAdminAccountDiaplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[2] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Current user is a Administrator user account ";


                }
                else if (Status[2] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Current user is not a Administrator user account";

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

        private string VerifyDocandSettingsDiaplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[3] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "The Document and Settings folder match the username ";


                }
                else if (Status[3] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "The Document and Settings folder does not match the username ";

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
        private string OSHotFixKB9253661DisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[4] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Hot fix is available";


                }
                else if (Status[5] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Hot fix is not available";

                }
                else
                {
                    displayResult = "This Hot fixes are not required other then Windows 2003";

                }

                return displayResult;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ex.Message;
            }
        }
        private string OSHotFixKB942288DisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[6] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Hot fix is available";


                }
                else if (Status[6] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Hot fix is not available";

                }
                else
                {
                    displayResult = "This Hot fixes are not required other then Windows 2003";

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
