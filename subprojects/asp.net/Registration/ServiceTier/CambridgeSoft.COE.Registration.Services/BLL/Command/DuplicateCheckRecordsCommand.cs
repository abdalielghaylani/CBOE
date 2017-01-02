using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.Services.BLL.Command
{
    /// <summary>
    /// Csla command which performs a duplicate-check on each of the listed records.
    /// </summary>
    [Serializable()]
    class DuplicateCheckRecordsCommand : Csla.CommandBase
    {
        private List<RegistryRecord> _recordList = new List<RegistryRecord>();
        private RegistryRecord.DataAccessStrategy _strategy = RegistryRecord.DataAccessStrategy.Atomic;

        private List<DuplicateCheckResponse> _responses = new List<DuplicateCheckResponse>();
        /// <summary>
        /// The collection of property-bag objects summarizing each interaction of a RegistryRecord
        /// with the datasource. This collection's 'count' will match the number of records passed
        /// to the constructor, given that all records will be acted upon.
        /// </summary>
        public List<DuplicateCheckResponse> Responses
        {
            get { return _responses; }
        }

        /// <summary>
        /// Provided a list of RegistryRecords, this will cause each of the records to be
        /// 'checked' for structure or non-structure duplicates.
        /// </summary>
        /// <param name="records">List of RegistryRecord objects.</param>
        /// <returns>
        /// A collection of property-bag items containing pertinent information regarding the interaction
        /// between each record and the datastore, including either error information OR summary data
        /// points descriptive of the record itself.
        /// </returns>
        [COEUserActionDescription("RegistrationLoadList_CheckDuplicates")]
        public static List<DuplicateCheckResponse> CheckAll(
            List<RegistryRecord> records, RegistryRecord.DataAccessStrategy accessStrategy)
        {
            DuplicateCheckRecordsCommand parameter =
                new DuplicateCheckRecordsCommand(records, accessStrategy);

            DuplicateCheckRecordsCommand resultWrapper =
                Csla.DataPortal.Execute<DuplicateCheckRecordsCommand>(parameter);

            List<DuplicateCheckResponse> results = resultWrapper.Responses;
            return results;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private DuplicateCheckRecordsCommand() { }

        /// <summary>
        /// Factory-style constructor
        /// </summary>
        /// <param name="duplicateAction"></param>
        /// <param name="records"></param>
        private DuplicateCheckRecordsCommand(
            List<RegistryRecord> records, RegistryRecord.DataAccessStrategy accessStrategy)
        {
            _recordList = records;
            _strategy = accessStrategy;
        }

        /// <summary>
        /// Cycles through the RegistryRecord instances and tries to Register each one,
        /// collecting processing artefacts in a small, "property-bag"-like object.
        /// </summary>
        protected override void DataPortal_Execute()
        {
            if (_responses == null)
                _responses = new List<DuplicateCheckResponse>();

            List<DuplicateCheckResponse> result = new List<DuplicateCheckResponse>();

            foreach (RegistryRecord record in _recordList)
            {
                try
                {
                    result = record.FindDuplicates(_strategy);
                }
                catch (Exception ex)
                {
                    if (record != null && record.DataStrategy != Services.Types.RegistryRecord.DataAccessStrategy.Atomic)
                        COEExceptionDispatcher.HandleBLLException(ex, COEExceptionDispatcher.Policy.LOG_ONLY);
                    else
                    COEExceptionDispatcher.HandleBLLException(ex);

                    result = new List<DuplicateCheckResponse>();
                    result.Add(new DuplicateCheckResponse());
                    result[0].UniqueRegistrations = -1;
                }
                finally
                {
                    _responses.AddRange(result);
                }
            }
        }
    }
}
