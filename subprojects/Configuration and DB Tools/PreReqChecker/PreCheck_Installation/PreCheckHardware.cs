using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using Microsoft.Win32;
using System.Collections;

namespace PreCheck_Installation
{
    public class PreCheckHardware:IPreCheck
    {
        #region private variables
        private int _expectedRAM;
        private int _actualRAM;
        private int _exceptedHDSize;
        private int _actualHDSize;
        private string _preCheckName;
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
        public PreCheckHardware()
        {
           
           
           PreCheckName = "Hardware Requirements";
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
            PreCheckSubList.Add(0, "Check Processors");
            PreCheckSubList.Add(1, "Check RAM");
            PreCheckSubList.Add(2, "Check Hard Disk Type");
            PreCheckSubList.Add(3, "Check Hard Disk Size");
           
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add("1.0", "Intel x86 P4");
            ExpectedValueList.Add("1.1", "Xeon");
            ExpectedValueList.Add("2.0", "2GB");
            _expectedRAM = 2;
            ExpectedValueList.Add("3.0", "RAID");
            ExpectedValueList.Add("3.1", "External Disc");
            ExpectedValueList.Add("4.0", "Greater than or Equal to 4GB");
            _exceptedHDSize = 4;
        }

        public void GetActualResult()
        {
            try
            {
                string Processor = GetProcessor();
                ActualValue.Add(0, Processor);
                string RAM = GetRAM();
                ActualValue.Add(1, RAM);
                string HDType = GetHardDiskType();
                ActualValue.Add(2, HDType);
                string HDSize = GetHardDiskSize();
                ActualValue.Add(3, HDSize);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);

            }
        }

        public void CompareExpectedResult()
        {
            string OSstatus = CompareProcessorExpectedvalue();
            Status.Add(0, OSstatus);
            string RAMStatus = CompareRAMExpectedvalue();
            Status.Add(1, RAMStatus);
            string HDTypeStatus = CompareHDTypexpectedvalue();
            Status.Add(2, HDTypeStatus);
            string HDSizeStatus = CompareHDSizeExpectedvalue();
            Status.Add(3, HDSizeStatus);
        }

        public void DisplayMessage()
        {
            string ProcessorDisplayMessage = this.ProcessorDisplayMessage();
            Message.Add(0, ProcessorDisplayMessage);
            string RAMDisplayMessage = RAMDiaplayMessage();
            Message.Add(1, RAMDisplayMessage);
            string strHDTypeDisplayMessage = HDTypeDisplayMessage();
            Message.Add(2, strHDTypeDisplayMessage);
            string strHDSizeDisplayMessage = HDSizeDisplayMessage();
            Message.Add(3, strHDSizeDisplayMessage);
        }

        #endregion

        #region private methods

        #region private methods related to GetActualResult

        private string GetProcessor()
        {
            try
            {
                string sCpuInfo = string.Empty;
                RegistryKey regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
                Object cpuType = regKey.GetValue("ProcessorNameString");
                sCpuInfo = cpuType.ToString();
                return sCpuInfo;

            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
                
            }
        }

        private string GetRAM()
        {
            try
            {

                string strRAM = "";
                double dblRAMSizeinGB = 0.0;
                UInt32 SizeinGB = 0;
                string Query = "SELECT * FROM Win32_LogicalMemoryConfiguration";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);
                foreach (ManagementObject item in searcher.Get())
                {
                    UInt32 SizeinKB = Convert.ToUInt32(item["TotalPhysicalMemory"]);
                    UInt32 SizeinMB = SizeinKB / 1024;
                    dblRAMSizeinGB = (SizeinMB / 1024.0);

                }

                _actualRAM = (int)Math.Ceiling(dblRAMSizeinGB);
                strRAM = Math.Ceiling(dblRAMSizeinGB).ToString() + "GB";
                return strRAM;
            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
           
        }
        private string GetHardDiskType()
        {
            try
            {
                string Type = "";
                UInt32 HDType = 0;
                ManagementClass driveClass = new ManagementClass("Win32_LogicalDisk");
                ManagementObjectCollection drives = driveClass.GetInstances();
                foreach (ManagementObject drive in drives)
                {

                    HDType = Convert.ToUInt32(drive.Properties["DriveType"].Value);

                }
                switch (HDType)
                {
                    case 1:
                        Type = "No root directory. "
                            + "Drive type could not be "
                            + "determined.";
                        break;
                    case 2:
                        Type = "Removable drive.";
                        break;
                    case 3:
                        Type = "Local hard disk.";
                        break;
                    case 4:
                        Type = "Network disk.";
                        break;
                    case 5:
                        Type = "Compact disk.";
                        break;
                    case 6:
                        Type = "RAM disk.";
                        break;
                    default:
                        Type = "Drive type could not be"
                            + " determined.";
                        break;

                }

                return Type;
            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
      

        }
        private string GetHardDiskSize()
        {
            try
            {
            string size = "";
            double HDSize = 0.0;
            ManagementClass driveClass = new ManagementClass("Win32_LogicalDisk"); 
            ManagementObjectCollection drives = driveClass.GetInstances(); 
            foreach (ManagementObject drive in drives) 
            {

                UInt64 bytes = Convert.ToUInt64( drive.Properties["Size"].Value);
                UInt64 SizeinKB = bytes / 1024;
                UInt64 sizeinMB = SizeinKB / 1024;
                HDSize = (sizeinMB / 1024.0);
                
            }
            _actualHDSize = (int)Math.Ceiling(HDSize);
            size = Math.Ceiling(HDSize).ToString() + "GB";
            return size;
            }
            catch (Exception ex)
            {
                return PreCheckerGlobals.PreCheckerResults.Unknown.ToString();
            }
        }
    
        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareProcessorExpectedvalue()
        {
            string strStatus = "";
            try
            {

                Hashtable actualResult = ActualValue;
                string strActualresult = actualResult[0].ToString();
                string strExpectedValue1 = ExpectedValueList["1.0"].ToString();
                string strExpectedValue2 = ExpectedValueList["1.1"].ToString();
                if (strActualresult.Contains("Intel") || strActualresult.Contains("Pentium") || strActualresult.Contains("Xeon") || strActualresult.Contains("Core"))
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

        private string CompareRAMExpectedvalue()
        {
            string strStatus = string.Empty;
            try
            {
                                             
                if (_actualRAM>=_expectedRAM)
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

        private string CompareHDTypexpectedvalue()
        {
            string strStatus = "";
            try
            {
                
            Hashtable actualResult = ActualValue;
            string strActualresult = actualResult[2].ToString();
                string strExpectedValue1 = ExpectedValueList["3.0"].ToString();
                string strExpectedValue2 = ExpectedValueList["3.1"].ToString();
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

        private string CompareHDSizeExpectedvalue()
        {
            string strStatus = string.Empty;
            try
            {

                if (_actualHDSize >= _exceptedHDSize)
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
        private string ProcessorDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Processor is available";


                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Processor is not available";

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
        private string RAMDiaplayMessage()
        {
            string displayResult="";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "System has expected RAM";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "System doe not have expected RAM";

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

        private string HDTypeDisplayMessage()
        {
            string displayResult = "";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected type of Hard Disk is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected type of Hard Disk is not available";

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
        private string HDSizeDisplayMessage()
        {
            string displayResult = "";
            try
            {
                if (Status[1] == PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected Hard Disk Size is available";


                }
                else if (Status[1] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected Hard Disk Size is not available";

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
