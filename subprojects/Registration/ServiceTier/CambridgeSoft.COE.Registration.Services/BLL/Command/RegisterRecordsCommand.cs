using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Services.Properties;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Validation;

namespace CambridgeSoft.COE.Registration.Services.BLL.Command
{
    /// <summary>
    /// Csla Command class which perfoems the 'Register' action on each record in the list.
    /// </summary>
    [Serializable()]
    class RegisterRecordsCommand : Csla.CommandBase
    {
        private List<RegistryRecord> _recordList = new List<RegistryRecord>();
        private DuplicateAction _duplicateAction = DuplicateAction.None;

        private List<RegRecordSummaryInfo> _saveInfos = new List<RegRecordSummaryInfo>();
        /// <summary>
        /// The collection of property-bag objects summarizing each interaction of a RegistryRecord
        /// with the datasource. This collection's 'count' will match the number of records passed
        /// to the constructor, given that all records will be acted upon.
        /// </summary>
        public List<RegRecordSummaryInfo> SaveInfos
        {
            get { return _saveInfos; }
        }

        /// <summary>
        /// Provided a list of RegistryRecords and an automated dulication-resolution strategy,
        /// this will cause each of the records to be 'registered' to 'permanent' status.
        /// </summary>
        /// <param name="duplicateAction">The mechanism by which duplicates will be resolved.</param>
        /// <param name="records">List of RegistryRecord objects.</param>
        /// <returns>
        /// A collection of property-bag items containing pertinent information regarding the interaction
        /// between each record and the datastore, including either error information OR summary data
        /// points descriptive of the record itself.
        /// </returns>
        /// 
        [COEUserActionDescription("RegistrationLoadList_Register")]
        public static List<RegRecordSummaryInfo> RegisterAll(
            DuplicateAction duplicateAction
            , List<RegistryRecord> records
            )
        {
            try
            {
                RegisterRecordsCommand parameter =
                    new RegisterRecordsCommand(duplicateAction, records);

                RegisterRecordsCommand resultWrapper =
                    Csla.DataPortal.Execute<RegisterRecordsCommand>(parameter);

                List<RegRecordSummaryInfo> results = resultWrapper.SaveInfos;
                return results;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private RegisterRecordsCommand() { }

        /// <summary>
        /// Factory-style constructor
        /// </summary>
        /// <param name="duplicateAction"></param>
        /// <param name="records"></param>
        private RegisterRecordsCommand(
            DuplicateAction duplicateAction
            , List<RegistryRecord> records
            )
        {
            _duplicateAction = duplicateAction;
            _recordList = records;
        }

        /// <summary>
        /// Cycles through the RegistryRecord instances and tries to Register each one,
        /// collecting processing artefacts in a small, "property-bag"-like object.
        /// </summary>
        protected override void DataPortal_Execute()
        {
            if (_saveInfos == null)
                _saveInfos = new List<RegRecordSummaryInfo>();

            RegRecordSummaryInfo regInfo = null;
            List<RegRecordSummaryInfo> regInfoList = new List<RegRecordSummaryInfo>();
            DuplicateAction dupAction = _duplicateAction;
            RegistryRecord.DataAccessStrategy dataStretegy = RegistryRecord.DataAccessStrategy.BulkLoader;

            foreach (RegistryRecord record in _recordList)
            {
                DateTime before = DateTime.Now;
                Trace.Indent();
                try
                {
                    RegistryRecord result = record.Register(dupAction, dataStretegy);
                    if (result != null)
                        regInfo = new RegRecordSummaryInfo(result, false);
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        List<BrokenRuleDescription> brokenRulesDescriptions = record.GetBrokenRulesDescription();
                        foreach (BrokenRuleDescription brd in brokenRulesDescriptions)
                        {
                            Csla.Core.BusinessBase bb = brd.BusinessObject as Csla.Core.BusinessBase;
                            foreach (Csla.Validation.BrokenRule bRule in bb.BrokenRulesCollection)
                            {
                                string msg = string.Format("{0} - {1}", bRule.Property, bRule.Description);
                                sb.AppendLine(msg);
                            }
                        }
                        throw new Csla.Validation.ValidationException(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Exception baseEx = ex.GetBaseException();
                    regInfo = new RegRecordSummaryInfo(baseEx);
                }
                finally
                {
                    regInfoList.Add(regInfo);
                    Trace.Unindent();
                    Trace.WriteLine(string.Format("{0} seconds for registering a record", DateTime.Now.Subtract(before).TotalSeconds));
                }
            }

            _saveInfos.AddRange(regInfoList);
        }

    }
}
