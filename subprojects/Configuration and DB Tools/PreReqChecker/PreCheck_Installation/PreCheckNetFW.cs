using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace PreCheck_Installation
{
    class PreCheckNetFW:IPreCheck       
    {
        #region IPreCheck Members

        #region private varibales
       
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
        public PreCheckNetFW()
        {


            PreCheckName = ".NET FrameWork Installation";
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
            PreCheckSubList.Add(0, ".NET FrameWork Installation");
            
        }

        public void AssignExpectedList()
        {
            ExpectedValueList = new Hashtable();
            ExpectedValueList.Add(1, "2.0");
           
        }

        public void GetActualResult()
        {
            try
            {
                string dotNetFWversion = GetDotNETFW();
                ActualValue.Add(0, dotNetFWversion);
               
                
            }
            catch (Exception ex)
            {
                Console.Write( ex.Message);
                
            }
        }
           
        public void CompareExpectedResult()
        {
            string DotNetFWstatus = CompareDotNetFWVersionExpectedvalue();
            Status.Add(0, DotNetFWstatus);
            ;
        }

        public void DisplayMessage()
        {
            string DotNetFWDisplayMessage = DotNetFWVersionDisplayMessage();
            Message.Add(0, DotNetFWDisplayMessage);
           
        }

        #endregion
       
        #region private methods

        #region private methods related to GetActualResult

        private string GetDotNETFW()
        {
           string dotNetFWVersion="";
           dotNetFWVersion = Environment.Version.ToString();
           return dotNetFWVersion;
        }

        #endregion

        #region private methods related to CompareExpectedResult

        private string CompareDotNetFWVersionExpectedvalue()
         {
                      string strStatus = "";
                      try
                        {
                                          
                            Hashtable actualResult = ActualValue;
                            string strActualresult = actualResult[0].ToString();
                            string strExpectedValue1 = ExpectedValueList[1].ToString();
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
        private string DotNetFWVersionDisplayMessage()
        {
            string displayResult;
            try
            {
                if (Status[0] ==PreCheckerGlobals.PreCheckerResults.Success.ToString())
                {
                    displayResult = "Expected version of .Net Framework is available";
                    
                    
                }
                else if (Status[0] == PreCheckerGlobals.PreCheckerResults.Fail.ToString())
                {
                    displayResult = "Expected version of .Net Framework is not available";

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
