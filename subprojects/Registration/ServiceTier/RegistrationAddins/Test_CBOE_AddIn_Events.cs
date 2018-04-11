using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.AddIns;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{

    /// <summary>
    /// For testing purposes, these methods can be associated with their intended events,
    /// triggering an Event Log entry when the event is fired by some Registration process.
    /// </summary>
    [Serializable()]
    class Test_CBOE_AddIn_Events : IAddIn
    {
        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_Loaded_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_Loaded_Event");
        }
        
        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_Inserting_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_Inserting_Event");
        }

        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_Inserted_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_Inserted_Event");
        }

        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_Updating_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_Updating_Event");
        }

        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_Updated_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_Updated_Event");
        }

        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_Registering_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_Registering_Event");
        }

        /// <summary>
        /// Will write an audit record to a log to indicate the event was fired.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">this will be null</param>
        public void Test_UpdatingPerm_Event(object sender, EventArgs args)
        {
            WriteAudit("Test_UpdatingPerm_Event");
        }

        #region IAddIn Members

        private IRegistryRecord _registryRecord;
        /// <summary>
        /// The instance against which this Add-In can take action (read and/or modify).
        /// </summary>
        public IRegistryRecord RegistryRecord
        {
            get{ return _registryRecord; }
            set{ _registryRecord = value; }
        }

        /// <summary>
        /// The custom xml used to initialize variables, settings, etc. for the AddIn.
        /// </summary>
        /// <param name="xmlConfiguration"></param>
        public void Initialize(string xmlConfiguration)
        {
           //no custom initialization is required at this time 
        }

        #endregion

        private void WriteAudit(string callingMethodName)
        {
            LogEntry entry = new LogEntry();
            entry.Categories.Add("Event Log");
            entry.Title = callingMethodName;
            entry.Message = string.Format(
                "Successful invocation of '{0}' from the 'test' Registration Add-In", callingMethodName);
            entry.Severity = System.Diagnostics.TraceEventType.Information;

            try
            {
                if (Logger.IsLoggingEnabled() == true)
                    Logger.Write(entry);
            }
            catch
            {
                //TODO: Consider writing the message to a local text file as an alternative.
            }
        }
    }
}
