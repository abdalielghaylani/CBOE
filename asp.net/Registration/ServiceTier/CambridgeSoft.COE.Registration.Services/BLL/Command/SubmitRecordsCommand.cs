using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Validation;

namespace CambridgeSoft.COE.Registration.Services.BLL.Command
{
    /// <summary>
    /// Csla Command class which performs the 'Submit' action on each record in the list.
    /// </summary>
    [Serializable()]
    class SubmitRecordsCommand : Csla.CommandBase
    {
        private List<RegistryRecord> _recordList = new List<RegistryRecord>();
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
        /// Provided a list of RegistryRecords, this will cause each of the records to be
        /// 'submitted' to 'temporary' status.
        /// </summary>
        /// <param name="records">List of RegistryRecord objects.</param>
        /// <returns>
        /// A collection of property-bag items containing pertinent information regarding the interaction
        /// between each record and the datastore, including either error information OR summary data
        /// points descriptive of the record itself.
        /// </returns>
        [COEUserActionDescription("RegistrationLoadList_Register")]
        public static List<RegRecordSummaryInfo> SubmitAll(List<RegistryRecord> records)
        {
            SubmitRecordsCommand parameter =
                new SubmitRecordsCommand(records);

            SubmitRecordsCommand resultWrapper =
                Csla.DataPortal.Execute<SubmitRecordsCommand>(parameter);

            List<RegRecordSummaryInfo> results = resultWrapper.SaveInfos;
            return results;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private SubmitRecordsCommand() { }

        /// <summary>
        /// Factory-style constructor
        /// </summary>
        /// <param name="records"></param>
        private SubmitRecordsCommand(List<RegistryRecord> records)
        {
            _recordList = records;
        }

        /// <summary>
        /// Cycles through the RegistryRecord instances and tries to Submit each one,
        /// collecting processing artefacts in a small, "property-bag"-like object.
        /// </summary>
        protected override void DataPortal_Execute()
        {
            if (_saveInfos == null)
                _saveInfos = new List<RegRecordSummaryInfo>();

            RegRecordSummaryInfo regInfo = null;
            List<RegRecordSummaryInfo> regInfoList = new List<RegRecordSummaryInfo>();
            RegistryRecord.DataAccessStrategy dataStretegy = RegistryRecord.DataAccessStrategy.BulkLoader;

            foreach (RegistryRecord record in _recordList)
            {
                try
                {
                    if (record.ID == 0)
                        RegistrationLoadList.BackFillCompoundFragmentsFromBatchInfos(record);

                    RegistryRecord result = record.Save(dataStretegy);

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
                    regInfo.RegistryXml = record.ExceptionXml; // Adds Exception Xml RegRecordSummaryInfo To List Which Allows Us To Write Xml File In Log
                }
                finally
                {
                    regInfoList.Add(regInfo);
                }
            }
            _saveInfos = regInfoList;
        }
    }
}
