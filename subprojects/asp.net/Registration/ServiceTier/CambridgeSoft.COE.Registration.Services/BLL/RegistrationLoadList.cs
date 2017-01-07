using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Services.BLL.Command;
using System.Xml;
using System.IO;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Container class for multiple RegistryRecord instances. <br />
    /// Intended specifically for bulk-loading, instances of this class ferry an internal
    /// list of [RegistryRecord]n to the server where they process themselves. Instead of
    /// returning RegistryRecord objects, a light-weight property-bag object is used to
    /// carry requisite details to the caller.
    /// </summary>
    /// <remarks>
    /// Though serializing multiple records via CSLA creates a much larger packet to send,
    /// there should be significant savings due to:
    /// <list type="bullet">
    /// <item>fewer round trips</item>
    /// <item>smaller packet return size</item>
    /// </list>
    /// </remarks>
    [Serializable()]
    public class RegistrationLoadList : Csla.BusinessBase<RegistrationLoadList>
    {
        private List<RegistryRecord> _recordList = new List<RegistryRecord>();

        /// <summary>
        /// Gets the underlying list's count.
        /// </summary>
        public int Count
        {
            get { return this._recordList.Count; }
        }

        #region > Constructors <

        /// <summary>
        /// Factory-style constructor
        /// </summary>
        /// <returns>An instance of the containing class</returns>
        /// 
        public static RegistrationLoadList NewRegistrationLoadList()
        {
            return new RegistrationLoadList();
        }
        private RegistrationLoadList()
        {
            //nothing required
        }

        /// <summary>
        /// Factory-style constructor
        /// </summary>
        /// <returns>An instance of the containing class</returns>
        /// <param name="records">An existing list of new registration records</param>
        /// 
        [COEUserActionDescription("CreateRegistrationLoadList")]
        public static RegistrationLoadList NewRegistrationLoadList(List<RegistryRecord> records)
        {
            try
            {
                return new RegistrationLoadList(records);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
        private RegistrationLoadList(List<RegistryRecord> records)
        {
            this._recordList.AddRange(records);
        }

        #endregion

        /// <summary>
        /// Wrapper for clearing the underlying list.
        /// </summary>
        public void Clear()
        {
            this._recordList.Clear();
        }

        /// <summary>
        /// Wrapper method to add records to the underlying list.
        /// </summary>
        /// <param name="record"></param>
        public void AddRegistration(RegistryRecord record)
        {
            this._recordList.Add(record);
        }

        /// <summary>
        /// Uses a Csla command object to transfer the records to the server, wherer they will self-register.
        /// </summary>
        /// <param name="duplicateAction"></param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("RegistrationLoadList_Register")]
        public List<RegRecordSummaryInfo> Register(DuplicateAction duplicateAction)
        {
            try
            {
                List<RegRecordSummaryInfo> results =
                    RegisterRecordsCommand.RegisterAll(duplicateAction, _recordList);

                
                this.Clear();
                return results;
                //this class can be serialized and deserialized by a binary formatter
                //{
                //    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                //        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                //    System.IO.Stream binStream = new System.IO.MemoryStream();

                //    RegisterMultipleRecordsCommand x = new RegisterMultipleRecordsCommand();
                //    x._recordList.AddRange(this._recordList);
                //    bf.Serialize(binStream, x);

                //    binStream.Position = 0;

                //    System.IO.Stream outStream = new System.IO.MemoryStream();
                //    RegisterMultipleRecordsCommand y = (RegisterMultipleRecordsCommand)bf.Deserialize(binStream);
                //}
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        } 
                
        /// <summary>
        /// Uses a Csla command object to transfer the records to the server, wherer they will self-submit.
        /// </summary>
        /// <param name="duplicateAction"></param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("RegistrationLoadList_Register")]
        public List<RegRecordSummaryInfo> Submit()
        {
            try
            {
                List<RegRecordSummaryInfo> results =
                    SubmitRecordsCommand.SubmitAll(_recordList);
                this.Clear();
                return results;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Uses a Csla command object to transfer the records to the server, wherer they will self-check
        /// for structure (and no-structure_ duplicates..
        /// </summary>
        /// <returns></returns>
        [COEUserActionDescription("RegistrationLoadList_CheckDuplicates")]
        public List<DuplicateCheckResponse> CheckDuplicates()
        {
            try
            {
                List<DuplicateCheckResponse> results =
                    DuplicateCheckRecordsCommand.CheckAll(
                    _recordList, RegistryRecord.DataAccessStrategy.BulkLoader);
                this.Clear();
                return results;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Added to fulfill CSLA requirements.<br />
        /// Not necessary here due to being just a wrapper for a list of RegistryRecord instances.
        /// </summary>
        /// <returns>Zero (0)</returns>
        protected override object GetIdValue()
        {
            return 0;
        }

        /// <summary>
        /// Intended primarily for new registrations, this method will back-fill each Component.Compound's FragmentList
        /// from the correlated BatchComponentFragmentList
        /// </summary>
        /// <param name="record">a RegistryRecord instance</param>
        public static void BackFillCompoundFragmentsFromBatchInfos(RegistryRecord record)
        {
            foreach (Batch batch in record.BatchList)
            {
                foreach (BatchComponent batchComp in batch.BatchComponentList)
                {
                    int componentIndex = batchComp.ComponentIndex;
                    Component comp = record.ComponentList.GetComponentByIndex(componentIndex);
                    FragmentList frags = comp.Compound.FragmentList;

                    //Copy fragment information to the Component if not already present
                    foreach(BatchComponentFragment batchCompFrag in batchComp.BatchComponentFragmentList)
                    {
                        if (frags.GetByID(batchCompFrag.FragmentID) == null)
                            frags.Add(
                                Fragment.NewFragment(batchCompFrag.FragmentID, batchCompFrag.FragmentID.ToString(), null, 0, null, 0, null)
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Converts the results of a duplicate-check into a simpler message format for use in
        /// an overloaded constructor for <see cref="RegRecordSummaryInfo">a response 'message' object</see>
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        public static List<RegRecordSummaryInfo> ConvertFromDupCheckResponses(List<DuplicateCheckResponse> responses)
        {
            List<RegRecordSummaryInfo> results = new List<RegRecordSummaryInfo>();
            foreach (DuplicateCheckResponse response in responses)
            {
                string msg;
                EventLogEntryType msgType = EventLogEntryType.Information;
                if (response.UniqueRegistrations == -1)
                {
                    msg = "Error while checking duplicates";
                    msgType = EventLogEntryType.Warning;
                }
                else if (response.MatchedRegistrations.Count == 0)
                    msg = "No duplicates found";
                else
                {
                    string msgFormatter = "Matching registrations found (based on {0}): {1}";
                    string[] regsNumbers = new string[response.UniqueRegistrations];
                    for (int index = 0; index < regsNumbers.Length; index++)
                    {
                        regsNumbers[index] = response.MatchedRegistrations[index].RegistryNumber;
                    }
                    string matchingMechanism = response.Mechanism.ToString();
                    string concatenatedRegNumbers = string.Join(",", regsNumbers);
                    msg = string.Format(msgFormatter, matchingMechanism, concatenatedRegNumbers);
                    msgType = EventLogEntryType.Warning;
                }

                RegRecordSummaryInfo summary = new RegRecordSummaryInfo(msg, msgType);
                results.Add(summary);
            }

            return results;
        }

     
    }

    /// <summary>
    /// Property-container class which is used to deliver summary information about an
    /// interaction between a RegistryRecord and the datastore.
    /// </summary>
    /// <remarks>
    /// Intended to deliver a light-weight alternative to a complete RegistryRecord when
    /// only selected properties of that record are required (such as feedback during
    /// bulk-loading exercises or to act as querystring parameters for URLs.)
    /// </remarks>
    [Serializable()]
    public class RegRecordSummaryInfo
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegRecordSummaryInfo() { }

        /// <summary>
        /// Constructor: initialize properties using a RegistryRecord instance.
        /// </summary>
        /// <param name="record">A RegistryRecord instance.</param>
        public RegRecordSummaryInfo(RegistryRecord record, bool includeXml)
        {
            this._isRedBoxWarningExists = record.RegisterCheckRedBoxWarning;     // Jira ID - CBOE-1158
            this._redBoxWarningMessage = record.RedBoxWarning.Trim().Length > 0 ? GetWarning(record.RedBoxWarning) : string.Empty;    // Jira ID - CBOE-1158
            this._action =record.ActionDuplicates.ToString().Substring(0,1);
            this._diagnosticLevel = EventLogEntryType.Information;

            //TODO: Alter the DAL process so the number is valid!
            // If the RegistryRecord hasn't been re-populated with the XML from the database,
            //   then we cannot possibly know what batch number a 'new batch' represents.
            // That means that probably we should ALWAYS allow the DAL to return the new XML
            //   in the case of a CRUD event.
            this._batchCount = record.BatchList.Count;

            this._regnum = record.RegNumber.RegNum;
            if (string.IsNullOrEmpty(record.RegNumber.RegNum))
                this._tempid = record.ID.ToString();
            this._message = null;
            if (includeXml)
                this._registryXml = record.Xml;
            if (!string.IsNullOrEmpty(record.DalResponseMessage) && !string.IsNullOrEmpty(record.FoundDuplicates))
            {
                string msgFormatter = "Matching registrations found: {0}";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(record.FoundDuplicates);
                XmlNodeList nodeList = doc.GetElementsByTagName("REGNUMBER");
                if (nodeList.Count > 0)
                {
                    List<string> matchingRegistrations = new List<string>();
                    foreach (XmlNode node in nodeList)
                    {
                        matchingRegistrations.Add(node.InnerXml);
                    }
                    this._duplicateMessage = record.DalResponseMessage + string.Format(msgFormatter, string.Join(",", matchingRegistrations.ToArray()));
                }
            }
            if (!string.IsNullOrEmpty(record.CustomFieldsResponse))
            {
                this.ExtractCustomResponse(record.CustomFieldsResponse);
            }
        }

        // Jira ID - CBOE-1158
        /// <summary>
        /// Method returns the formatted warnign message.
        /// </summary>
        /// <param name="warning">actual warning</param>
        /// <returns>Formatted warning</returns>
        private string GetWarning(string warning) 
        {
            string newWarning = string.Empty;
            newWarning = warning.Replace("<br />","");
            return newWarning;
        }

        /// <summary>
        /// Constructor: initialize properties using an Exception object.
        /// </summary>
        /// <param name="ex">An Exception instance, or an instance that inherits from one.</param>
        public RegRecordSummaryInfo(Exception ex)
        {
            this._diagnosticLevel = EventLogEntryType.Error;
            this._action = null;
            this._batchCount = 0;
            this._regnum = null;
            this._tempid = null;
            this._message = ex.Message;
        }

        /// <summary>
        /// Constructor: initialize properties using a message string and an indicator of the message type.
        /// </summary>
        /// <param name="message">a string containing an informational, warning or error message.</param>
        /// <param name="diagnosticLevel">
        /// the type of message being processed (informational, warning or error)
        /// </param>
        public RegRecordSummaryInfo(string message, System.Diagnostics.EventLogEntryType diagnosticLevel)
        {
            this._diagnosticLevel = diagnosticLevel;
            this._action = null;
            this._batchCount = 0;
            this._regnum = null;
            this._tempid = null;
            this._message = message;
        }

        /// <summary>
        /// Extracts the Message, Error and Result properties from the raw DAL response.
        /// Called 'on-demand' by property accessors.
        /// </summary>
        [COEUserActionDescription("ExtractDalResponse")]
        private void ExtractCustomResponse(string customFieldResponse )
        {
            //Extract the rest from the output of the DAL procedure
            if (!string.IsNullOrEmpty(customFieldResponse))
            {
                try
                {
                    StringReader responseReader = new StringReader(customFieldResponse);
                    XmlTextReader xtReader = new XmlTextReader(responseReader);
                    xtReader.WhitespaceHandling = WhitespaceHandling.None;
                    while (xtReader.Read())
                    {
                        if (xtReader.LocalName == "RegId" && xtReader.NodeType == XmlNodeType.Element && xtReader.Read())
                            this.RegId = int.Parse(xtReader.Value);
                        if (xtReader.LocalName == "BatchNumber" && xtReader.NodeType == XmlNodeType.Element && xtReader.Read())
                             this.BatchCount =int.Parse(xtReader.Value);
                    }

                    xtReader.Close();
                    responseReader.Close();
                }
                catch
                {
                    //TODO: Determine course of action if message undecipherable.
                }
            }
        }
        private EventLogEntryType _diagnosticLevel;
        /// <summary>
        /// Represents the nature of the message returned from the DAL interaction.
        /// </summary>
        [XmlElement()]
        public EventLogEntryType DiagnosticLevel
        {
            get { return _diagnosticLevel; }
            set { _diagnosticLevel = value; }
        }

        private string _action;
        /// <summary>
        /// Describes the action taken during the interaction with the datastore. Generally
        /// this will describe any automated duplciate-resolution process that was invoked.
        /// </summary>
        /// <remarks>
        /// <b>NOTE: </b>this is not truly a reflection of the action taken becuase the RegistryRecord has
        /// been re-fetched and thus this data-point hs been lost. This is one of the weaknesses of CSLA;
        /// loss of contextual information due to the class also being the service!
        /// </remarks>
        [XmlElement()]
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        private string _regnum;
        /// <summary>
        /// The registration number assigned to the RegistryRecord. This may be null or empty
        /// if this registration is 'temporary'.
        /// </summary>
        [XmlElement()]
        public string RegNum
        {
            get { return _regnum; }
            set { _regnum = value; }
        }

        private string _tempid;
        /// <summary>
        /// The identifier of the RegistryRecord if it is a 'temporary' record. This will be
        /// null or empty if the RegistryRecord is 'permanent'.
        /// </summary>
        [XmlElement()]
        public string TempNum
        {
            get { return _tempid; }
            set { _tempid = value; }
        }

        private int _batchCount;
        /// <summary>
        /// The batch number that the record was saved against.
        /// </summary>
        [XmlElement()]
        public int BatchCount
        {
            get { return _batchCount; }
            set { _batchCount = value; }
        }


        private int _regId;
        /// <summary>
        /// The batch id that the record was saved against.
        /// </summary>
        [XmlElement()]
        public int RegId
        {
            get { return _regId; }
            set { _regId = value; }
        }

        private string _message;
        /// <summary>
        /// If the interaction between the RegistryRecord and the datastore resulted in an exception,
        /// the exception details will be placed here. This will normally be null or empty.
        /// </summary>
        /// <remarks>
        /// This is typically mutually exclusive of all other properties, except in the case where
        /// the interaction was an update of an existing RegistryRecord.
        /// </remarks>
        [XmlElement()]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _duplicateMessage;
        /// <summary>
        /// If the interaction between the RegistryRecord and the datastore resulted in an dalResponse and FoundDuplicates,
        /// those details will be formatted as message. This will normally be null or empty.
        /// </summary>
        /// <remarks>
        /// This is typically mutually exclusive of all other properties, except in the case where
        /// the interaction was an update of an existing RegistryRecord.
        /// </remarks>
        [XmlElement()]
        public string DuplicateMessage
        {
            get { return _duplicateMessage; }
            set { _duplicateMessage = value; }
        }

        private string _registryXml;
        /// <summary>
        /// The XML of the registry record, generally provided only when there have been errors.
        /// </summary>
        [XmlElement()]
        public string RegistryXml
        {
            get { return _registryXml; }
            set { _registryXml = value; }
        }

        // Jira ID - CBOE-1158
        private bool _isRedBoxWarningExists=false;
        /// <summary>
        /// Gets or sets the registry record has red box warning for structure. Default it is false.
        /// </summary>
        [XmlElement()]
        public bool IsRedBoxWarningExists
        {
            get { return _isRedBoxWarningExists; }
            set { _isRedBoxWarningExists = value; }
        }

        // Jira ID - CBOE-1158
        private string _redBoxWarningMessage = string.Empty;
        /// <summary>
        /// Gets or sets red box warning message.
        /// </summary>
        public string RedBoxWarningMessage
        {
            get { return _redBoxWarningMessage; }
            set { _redBoxWarningMessage = value; }
        }
    
    }
}
