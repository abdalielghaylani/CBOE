using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Reflection;

namespace PreCheck_Installation
{
    public class GenenerateOutPuttxt
    {
        # region Constant - Static Node Vaiables

        //------ PRE CHECK NODES
        //-------------------------------------------
        const string PRECHECKATTRNAME = "Name";
        const string PRECHECKNODENAME = "PreCheck";
        //-------------------------------------------

        const string FORMATSEP = ": ";
        const string GAPSEP = " ";
        const string NEWLINE = "\n";
        const string PRECHECKLISTNEWLINE = "\n\n";

        const string PRECHECKNEWLINE = "\n=========================================================================\n\n";

        //------ PRE CHECK SUBLIST NODES
        //-------------------------------------------
        const string PCSUBLISTNODENAME = "PreCheck SubList";
        const string PCSUBLISTATTRNAME = "Name";
        //-------------------------------------------

        //------ PRE CHECK SUBLIST NODES
        //-------------------------------------------
        const string EXPECTEDLISTNODENAME = "Expected Values";
        //-------------------------------------------


        //------ STATUS ,ACTUAL REQUIRMENT,  MESSAGE,
        //-------------------------------------------
        const string STATUSNODENAME = "Status";
        const string ACTUALREQNODENAME = "Actual Result";
        const string MESSAGENODENAME = "Message";
        //-------------------------------------------



        # endregion

        # region Vaiables
        string _preCheck = string.Empty;
        string _exception = string.Empty;
        List<object> _generateXMLList = new List<object>();
        Hashtable _hPreCheckSubList = new Hashtable();
        Hashtable _hExpectedList = new Hashtable();
        Hashtable _hActualReq = new Hashtable();
        Hashtable _hStatus = new Hashtable();
        Hashtable _hMessage = new Hashtable();
        string _precheckOutput = string.Empty;
        string _tPrecheckConcatenate = string.Empty;
        string _tPrecheckRootText = string.Empty;
        string _tPreCheckText = "";
        string _tPrecheckSublist = string.Empty;
        string _tStatus = string.Empty;
        string _tExpectedList = string.Empty;
        string _tMessage = string.Empty;
        string _tActualResult = string.Empty;
        string _rootDocument = string.Empty;
        # endregion

        #region Public Properties


        #region String Properties
        public string PreCheckName
        {
            get
            {
                return _preCheck;
            }
            set
            {
                _preCheck = value;
            }
        }
        
        # endregion

        #region Collection Properties
        public List<object> ObjectList
        {
            get
            {
                return _generateXMLList;
            }
            set
            {
                _generateXMLList = value;

                ProcessSystemCheck();
            }
        }
        public Hashtable ExpectedValueList
        {
            get
            {
                return _hExpectedList;
            }
            set
            {
                _hExpectedList = value;
            }
        }
        public Hashtable PreCheckSubList
        {
            get
            {
                return _hPreCheckSubList;
            }
            set
            {
                _hPreCheckSubList = value;

            }
        }
        public Hashtable Status
        {
            get
            {
                return _hStatus;
            }
            set
            {
                _hStatus = value;

            }
        }

        public Hashtable ActualValue
        {
            get
            {
                return _hActualReq;
            }
            set
            {
                _hActualReq = value;
            }
        }
        public Hashtable Message
        {
            get
            {
                return _hMessage;
            }
            set
            {
                _hMessage = value;
            }
        }
        # endregion

        #region XmlDocument Properties
        public string GetPreCheckXMLDoc
        {
            get
            {
                return _rootDocument;
            }

        }
        # endregion
        #endregion

        # region Functions
        private string CreatePrecheckNode(string Name)
        {
            if (Name.Trim().Length == 0)
            {
                return "";
            }
            return PRECHECKNODENAME + GAPSEP + PRECHECKATTRNAME + FORMATSEP + Name + PRECHECKLISTNEWLINE;
        }
        private string CreatePCSublistNode<T>(T Name)
        {
            if (Name.ToString().Trim().Length == 0)
            {
                return "";
            }
            return PCSUBLISTNODENAME + GAPSEP + PCSUBLISTATTRNAME + FORMATSEP + Name.ToString() + NEWLINE;
        }
        private string CreateStatusNode<T>(T Name)
        {
            try
            {
                if (Name.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return STATUSNODENAME + GAPSEP + FORMATSEP + Name.ToString() + NEWLINE;
            }
            catch (Exception Ex)
            {
                _exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private string CreateActualReqNode<T>(T Name)
        {
            try
            {
                if (Name.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return ACTUALREQNODENAME + GAPSEP + FORMATSEP + Name.ToString() + NEWLINE;
            }
            catch (Exception Ex)
            {
                _exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private string CreateMessageNode<T>(T Name)
        {
            try
            {
                if (Name.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return MESSAGENODENAME + GAPSEP + FORMATSEP + Name.ToString() + NEWLINE;
            }
            catch (Exception Ex)
            {
                _exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private string CreateExpectedListRootNode(Hashtable hExpectedList, string sDupHashnameExpectedList)
        {
            try
            {
                string expectedValues = string.Empty;
                string[] arrayExpectedList = { "" };

                int nExpectedCount = 0;
                foreach (DictionaryEntry Key_ExpList in hExpectedList)
                {
                    if (Key_ExpList.Key.ToString().StartsWith(sDupHashnameExpectedList.ToString()))
                    {
                        Array.Resize(ref arrayExpectedList, nExpectedCount + 1);
                        arrayExpectedList[nExpectedCount] = Key_ExpList.Value.ToString();
                        nExpectedCount = nExpectedCount + 1;
                    }
                    // create node Expected
                }
                expectedValues = string.Join(",", arrayExpectedList);
                if (expectedValues.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return EXPECTEDLISTNODENAME + GAPSEP + FORMATSEP + expectedValues + NEWLINE;
            }
            catch (Exception Ex)
            {
                _exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private void MakeEmpty()
        {
          
                _tPrecheckConcatenate = "";
                _tPrecheckRootText = "";
                _tPreCheckText = "";
                _tPrecheckSublist ="";
                _tStatus = "";
                _tExpectedList = "";
                _tMessage = "";
                _tActualResult = "";
                _preCheck = "";
           
        }
        # endregion

        # region Methods
        private void SetProperties(ref PropertyInfo[] propertyInfos, object myObject)
        {
            try
            {
                string propertyName = string.Empty;


                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    propertyName = propertyInfo.Name;
                    object oPropValue = propertyInfo.GetValue(myObject, null);

                    if (propertyName.ToUpper() == "PRECHECKNAME")
                    {
                        this.PreCheckName = Convert.ToString(oPropValue);
                    }
                    else if (propertyName.ToUpper() == "PRECHECKSUBLIST")
                    {
                        this.PreCheckSubList = ((Hashtable)oPropValue);
                    }
                    else if (propertyName.ToUpper() == "STATUS")
                    {
                        this.Status = ((Hashtable)oPropValue);
                    }
                    else if (propertyName.ToUpper() == "EXPECTEDVALUELIST")
                    {
                        this.ExpectedValueList = ((Hashtable)oPropValue);
                    }
                    else if (propertyName.ToUpper() == "ACTUALVALUE")
                    {
                        this.ActualValue = ((Hashtable)oPropValue);
                    }
                    else if (propertyName.ToUpper() == "MESSAGE")
                    {
                        this.Message = ((Hashtable)oPropValue);
                    }
                }

            }
            catch (Exception Ex)
            {
                _exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }


        private void ProcessSystemCheck()
        {
            try
            {
                foreach (object obj in _generateXMLList)
                {

                    Type t = (obj).GetType();
                    PropertyInfo[] propertyInfos = t.GetProperties();
                    SetProperties(ref propertyInfos, obj);

                    _preCheck = this.PreCheckName;  // retrieve precheck name
                    _tPreCheckText = CreatePrecheckNode(_preCheck);

                    _hPreCheckSubList = this.PreCheckSubList;
                    _hStatus = this.Status;
                    _hActualReq = this.ActualValue;
                    _hMessage = this.Message;
                    _hExpectedList = this.ExpectedValueList;


                    int n_StatusCount = 0;

                    if (!(_hPreCheckSubList.Count == 0))
                    {
                        for (int i = 0; i <= _hPreCheckSubList.Count - 1;i++ )
                        {
                            object Key_PCList=_hPreCheckSubList[i];

                            object sDupHashname_Status = n_StatusCount;
                            object sDupHashname_Requirement  = n_StatusCount;
                            object sDupHashname_Message = n_StatusCount;
                            object sDupHashname_ExpectedList = Convert.ToString(n_StatusCount + 1);

                            if (_hStatus.ContainsKey(sDupHashname_Status))
                            {
                                sDupHashname_Status = _hStatus[sDupHashname_Status];
                            }
                            else
                            {
                                sDupHashname_Status = string.Empty;
                            }
                            if (_hActualReq.ContainsKey(sDupHashname_Requirement ))
                            {
                                sDupHashname_Requirement  = _hActualReq[sDupHashname_Requirement ];
                            }
                            else
                            {
                                sDupHashname_Requirement  = string.Empty;
                            }
                            if (_hMessage.ContainsKey(sDupHashname_Message))
                            {
                                sDupHashname_Message = _hMessage[sDupHashname_Message];
                            }
                            else
                            {
                                sDupHashname_Message = string.Empty;
                            }

                            _tPrecheckSublist = CreatePCSublistNode(Key_PCList);
                            _tStatus = CreateStatusNode(sDupHashname_Status);
                            _tExpectedList = CreateExpectedListRootNode(_hExpectedList, sDupHashname_ExpectedList.ToString());
                            _tMessage = CreateMessageNode(sDupHashname_Message);
                            _tActualResult = CreateActualReqNode(sDupHashname_Requirement );
                            n_StatusCount = n_StatusCount + 1;


                            _tPrecheckConcatenate = _tPrecheckConcatenate + _tPrecheckSublist;
                            _tPrecheckConcatenate = _tPrecheckConcatenate + _tActualResult;
                            _tPrecheckConcatenate = _tPrecheckConcatenate + _tExpectedList;
                            _tPrecheckConcatenate = _tPrecheckConcatenate + _tStatus;
                            _tPrecheckConcatenate = _tPrecheckConcatenate + _tMessage;
                            _tPrecheckConcatenate = _tPrecheckConcatenate + PRECHECKLISTNEWLINE;
                        }
                    }

               _rootDocument = _rootDocument + _tPreCheckText + _tPrecheckConcatenate.Trim() + PRECHECKNEWLINE;
                    
                    MakeEmpty();
                }
            }
            catch (Exception Ex)
            {
                _exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        # endregion
    }
}
