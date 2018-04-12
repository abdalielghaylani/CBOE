using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Reflection;


namespace PreCheck_Installation
{
    public class GenerateXML
    {

        # region Constant - Static Node Variables

        //------ PRE CHECK NODES
        //-------------------------------------------
       const string PRECHECKMNODENAME = "PreChecksMain";
       const string PRECHECKATTRNAME = "Name";
       const string PRECHECKNODENAME = "PreCheck";
        //-------------------------------------------


        //------ PRE CHECK SUBLIST NODES
        //-------------------------------------------
       const string PCSUBLISTNODENAME = "PreCheckSubList";
       const string PCSUBLISTATTRNAME = "id";
        //-------------------------------------------

        //------ PRE CHECK SUBLIST NODES
        //-------------------------------------------
       const string EXPECTEDLISTNODENAME = "Expectedvalues";
       const string CEXPECTEDLISTNODENAME = "Expected";
        //-------------------------------------------


        //------ STATUS ,ACTUAL REQUIRMENT,  MESSAGE,
        //-------------------------------------------
       const string STATUSNODENAME = "Status";
       const string ACTUALREQNODENAME = "ActualRequirment";
       const string MESSAGENODENAME = "Message";
        //-------------------------------------------



        # endregion

        # region Vaiables
        List<object> _generateXMLList = new List<object>();
        string _preCheck = string.Empty;
        Hashtable _hPreCheckSubList = new Hashtable();
        Hashtable _hExpectedList = new Hashtable();
        Hashtable _hActualReq = new Hashtable();
        Hashtable _hStatus = new Hashtable();
        Hashtable _hMessage = new Hashtable();

        string _Exception = string.Empty;
        XmlDocument _xmlRootDocument = new XmlDocument();

        # endregion
      

        #region Public Properties

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
       
        #region Collection Properties
        public List<object> ObjectList
        {
            set
            {
                _generateXMLList = value;

                ProcessSystemCheck();
            }
        }

        public Hashtable ExpectedList
        {
            set
            {
                _hExpectedList.Add(_hExpectedList.Count, value);
            }
        }
        public Hashtable PreCheckSubList_H
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
        public Hashtable Status_H
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
        public Hashtable ExpectedList_H
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
        public Hashtable ActualRequirement_H
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
        public Hashtable Message_H
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
        public XmlDocument GetPreCheckXMLDoc
        {
            get
            {
                return _xmlRootDocument;
            }

        }
        # endregion
        #endregion

        # region Methods


        private XmlNode CreatePrecheckNode(string Name)
        {
            XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(PRECHECKNODENAME);
            XmlAttribute XmlRootAttribute = _xmlRootDocument.CreateAttribute(PRECHECKATTRNAME);
            XmlRootAttribute.Value = Name;
            XmlNodeRoot.Attributes.Append(XmlRootAttribute);
            return XmlNodeRoot;
        }

        private XmlNode CreatePCSublistNode<T>(T Name)
        {
            XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(PCSUBLISTNODENAME);
            XmlAttribute XmlRootAttribute = _xmlRootDocument.CreateAttribute(PCSUBLISTATTRNAME);
            XmlRootAttribute.Value = Name.ToString();
            XmlNodeRoot.Attributes.Append(XmlRootAttribute);
            return XmlNodeRoot;
        }

        private XmlNode CreateStatusNode<T>(T Name)
        {
            try
            {
                XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(STATUSNODENAME);
                XmlNodeRoot.AppendChild(_xmlRootDocument.CreateTextNode(Name.ToString()));
                return XmlNodeRoot;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }

        private XmlNode CreateActualReqNode<T>(T Name)
        {
            try
            {
                XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(ACTUALREQNODENAME);
                XmlNodeRoot.AppendChild(_xmlRootDocument.CreateTextNode(Name.ToString()));
                return XmlNodeRoot;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }

        private XmlNode CreateMessageNode<T>(T Name)
        {
            try
            {
                XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(MESSAGENODENAME);
                XmlNodeRoot.AppendChild(_xmlRootDocument.CreateTextNode(Name.ToString()));
                return XmlNodeRoot;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private XmlNode CreateExpectedListRootNode()
        {
            try
            {

                XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(EXPECTEDLISTNODENAME);
                return XmlNodeRoot;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }

        private XmlNode CreateExpectedListRootNode(Hashtable _hExpectedList, string sDupHashname_ExpectedList)
        {
            try
            {
                XmlNode XmlRootNode_Expectedlist = _xmlRootDocument.CreateElement(EXPECTEDLISTNODENAME);
                
                foreach (DictionaryEntry Key_ExpList in _hExpectedList)
                {
                    if (Key_ExpList.Key.ToString().StartsWith(sDupHashname_ExpectedList.ToString()))
                    {
                        XmlRootNode_Expectedlist.AppendChild(CreateExpectedNode(Key_ExpList.Value)); 
                    }
                    
                }
                return XmlRootNode_Expectedlist;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }

        private XmlNode CreateExpectedNode<T>(T Name)
        {
            try
            {
                XmlNode XmlNodeRoot = _xmlRootDocument.CreateElement(CEXPECTEDLISTNODENAME);
                XmlNodeRoot.AppendChild(_xmlRootDocument.CreateTextNode(Name.ToString()));
                return XmlNodeRoot;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }

        void SetPropertys(ref PropertyInfo[] propertyInfos, object myObject)
        {
            try
            {
                string PropertyName = string.Empty;


                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    PropertyName = propertyInfo.Name;
                    object o = propertyInfo.GetValue(myObject, null);

                    if (PropertyName.ToUpper() == "PRECHECKNAME")
                    {
                        this.PreCheckName = Convert.ToString(o);
                    }
                    if (PropertyName.ToUpper() == "PRECHECKSUBLIST")
                    {
                        this.PreCheckSubList_H = ((Hashtable)o);
                    }
                    if (PropertyName.ToUpper() == "STATUS")
                    {
                        this.Status_H = ((Hashtable)o);
                    }
                    if (PropertyName.ToUpper() == "EXPECTEDVALUELIST")
                    {
                        this.ExpectedList_H = ((Hashtable)o);
                    }
                    if (PropertyName.ToUpper() == "EXPECTEDVALUELIST")
                    {
                        this.ExpectedList_H = ((Hashtable)o);
                    }
                    if (PropertyName.ToUpper() == "ACTUALVALUE")
                    {
                        this.ActualRequirement_H = ((Hashtable)o);
                    }
                    if (PropertyName.ToUpper() == "MESSAGE")
                    {
                        this.Message_H = ((Hashtable)o);
                    }
                }

            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void ProcessSystemCheck()
        {
            try
            {

                XmlNode xn_Type = _xmlRootDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                _xmlRootDocument.AppendChild(xn_Type);
                XmlNode xn_Prechecks = _xmlRootDocument.CreateElement(PRECHECKMNODENAME);
                _xmlRootDocument.AppendChild(xn_Prechecks);
                foreach (object obj in _generateXMLList)
                {

                    Type t = (obj).GetType();
                    PropertyInfo[] propertyInfos = t.GetProperties();
                    SetPropertys(ref propertyInfos, obj);




                    _preCheck = this.PreCheckName;  // retrieve precheck name
                    xn_Prechecks.AppendChild(CreatePrecheckNode(_preCheck)); // create node precheck
                    // create node prechecklist
                    XmlNode XmlRootNode_precheck = xn_Prechecks.SelectSingleNode(PRECHECKNODENAME + "[@" + PRECHECKATTRNAME + "='" + _preCheck + "']"); 

                    _hPreCheckSubList = this.PreCheckSubList_H;
                    _hStatus = this.Status_H;
                    _hActualReq = this.ActualRequirement_H;
                    _hMessage = this.Message_H;
                    _hExpectedList = this.ExpectedList_H;
                    

                    int n_StatusCount = 0;

                    if (!(_hPreCheckSubList.Count == 0))
                    {

                         for (int i = 0; i <= _hPreCheckSubList.Count - 1;i++ )
                        {
                            object Key_PCList=_hPreCheckSubList[i];


                            object sDupHashname_Status = n_StatusCount;
                            object sDupHashname_Requirement = n_StatusCount;
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
                            if (_hActualReq.ContainsKey(sDupHashname_Requirement))
                            {
                                sDupHashname_Requirement = _hActualReq[sDupHashname_Requirement];

                            }
                            else
                            {
                                sDupHashname_Requirement = string.Empty;
                            }
                            if (_hMessage.ContainsKey(sDupHashname_Message))
                            {
                                sDupHashname_Message = _hMessage[sDupHashname_Message];

                            }
                            else
                            {
                                sDupHashname_Message = string.Empty;
                            }




                            XmlNode XmlRootNode_pcSublist = CreatePCSublistNode(Key_PCList);
                            XmlRootNode_precheck.AppendChild(XmlRootNode_pcSublist); // create node prechecksublist
                            XmlRootNode_pcSublist.AppendChild(CreateStatusNode(sDupHashname_Status)); // create node status

                             //create node Expectedlist
                             //-----------------------------------------------------------
                           // XmlNode XmlRootNode_Expectedlist = CreateExpectedListRootNode();

                             XmlNode XmlRootNode_Expectedlist =CreateExpectedListRootNode(_hExpectedList, sDupHashname_ExpectedList.ToString());
                             
                            if (_hExpectedList.ContainsKey(sDupHashname_ExpectedList))
                            {
                                Hashtable _hExpected = new Hashtable();
                               
                                _hExpected = ((Hashtable)_hExpectedList[sDupHashname_ExpectedList]);
                                foreach (DictionaryEntry Key_ExpList in _hExpected)
                                {
                                   XmlRootNode_Expectedlist.AppendChild(CreateExpectedNode(Key_ExpList.Value));                                   // create node Expected
                                }
                            }
                            XmlRootNode_pcSublist.AppendChild(XmlRootNode_Expectedlist);
                             //-----------------------------------------------------------

                            // create node Actual Requirement
                            XmlRootNode_pcSublist.AppendChild(CreateActualReqNode(sDupHashname_Requirement));

                            // create node Message
                            XmlRootNode_pcSublist.AppendChild(CreateMessageNode(sDupHashname_Message)); 
                            
                            n_StatusCount = n_StatusCount + 1;

                        }

                    }
                }
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }

       

        # endregion

    }




}
