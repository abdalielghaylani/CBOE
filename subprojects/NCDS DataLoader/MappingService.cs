using System;
using System.Reflection;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core.Caching;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Registration.Services.Types;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

namespace CambridgeSoft.COE.DataLoader.Core.DataMapping
{
    /// <summary>
    /// Serves as the service-oriented utility class, providing mapping service to map data from
    /// source record or user input to destination object's property values.
    /// </summary>
    public class MappingService : JobService
    {
        public const string DESTINATION_RECORDS = "DestinationRecords";
        public event EventHandler<RecordProcessingEventArgs> RecordProcessing;

        /// <summary>
        /// Transfer the source records to destination records
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameter object.
        /// JobParameter.SourceRecords and JobParameter.Mappings are required
        /// </param>
        public MappingService(JobParameters jobParameters)
            : base(jobParameters)
        { }

        #region Override members

        protected override JobResponse DoJobInternal()
        {
            int originalRecordsCount = JobParameters.SourceRecords.Count;
            List<ISourceRecord> sourceRecords = JobParameters.SourceRecords;
            List<IDestinationRecord> destinationRecords = new List<IDestinationRecord>();
            JobParameters.DestinationRecords = destinationRecords;

            Dictionary<ISourceRecord, string> invalidSourceRecordsDic = new Dictionary<ISourceRecord, string>();
            List<ISourceRecord> invalidSourceRecords = new List<ISourceRecord>();
            List<IDestinationRecord> invalidDestinationRecords = new List<IDestinationRecord>();

            string message = string.Empty;
            for (int i = 0; i < sourceRecords.Count; i++)
            {
                destinationRecords.Add(DestinationRecordFactory.CreateDestinationRecord(JobParameters.Mappings.DestinationRecordType));
                OnRecordProcessing(sourceRecords[i]);
                message = Map(sourceRecords[i], destinationRecords[i], JobParameters.Mappings);
                if (!string.IsNullOrEmpty(message))
                {
                    invalidSourceRecords.Add(sourceRecords[i]);
                    invalidDestinationRecords.Add(destinationRecords[i]);
                    invalidSourceRecordsDic.Add(sourceRecords[i], message);
                }
            }

            if (invalidSourceRecords.Count > 0)
            {
                JobUtility.HandleInvalidRecords(invalidSourceRecords, invalidDestinationRecords, JobParameters);
            }

            JobResponse jobResponse = new CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse(JobParameters);

            jobResponse.ResponseContext[DESTINATION_RECORDS] = destinationRecords;

            OnRecordsMapped(invalidSourceRecordsDic, originalRecordsCount);

            return jobResponse;
        }

        protected override bool AreJobParametersValid()
        {
            return JobParameters.SourceRecords != null;
        }

        #endregion

        /// <summary>
        /// The core method of the mapping service, parsing the mapping information and performing
        /// the actual mapping from source record to destination record.
        /// </summary>
        /// <param name="sourceRecord">The source record</param>
        /// <param name="destRecord">The destination record</param>
        /// <param name="mappings">The mapping information instructing how the mapping
        /// should be performed</param>
        private string Map(ISourceRecord sourceRecord, IDestinationRecord destRecord, Mappings mappings)
        {
            try
            {
                if (sourceRecord == null || destRecord == null)
                    return string.Empty;

                COEDataBinder dataBinder = new COEDataBinder(destRecord);

                foreach (Mappings.Mapping mapping in mappings.MappingCollection)
                {
                    if (mapping.Enabled == true)
                    {
                        switch (mapping.MemberInformation.MemberType)
                        {
                            case Mappings.MemberTypeEnum.Property:
                                SetProperty(sourceRecord, dataBinder, mapping);

                                break;
                            case Mappings.MemberTypeEnum.Method:
                                InvokeMethod(sourceRecord, dataBinder, mapping);

                                break;
                            default:
                                throw new NotSupportedException(string.Format("Specified member type {0} is not supported", mapping.MemberInformation.MemberType.ToString()));
                        }
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void SetProperty(ISourceRecord sourceRecord, COEDataBinder dataBinder, Mappings.Mapping mapping)
        {
            // For simple property mapping, we only need 1 argument.
            if (mapping.MemberInformation.Args.Count == 1)
            {
                // Get the transformed argument value for further check. If it's null, bypass the call to
                // SetProperty so that the destination object's value remains unchanged.
                object transformedArgumentValue = TransformArgumentValue(sourceRecord, mapping.MemberInformation.Args[0]);

                if (transformedArgumentValue != null)
                {
                    dataBinder.SetProperty(
                        mapping.ObjectBindingPath + (string.IsNullOrEmpty(mapping.MemberInformation.Name) ? string.Empty : "." + mapping.MemberInformation.Name),
                        transformedArgumentValue
                    );
                }
            }
        }

        private void InvokeMethod(ISourceRecord sourceRecord, COEDataBinder dataBinder, Mappings.Mapping mapping)
        {
            Type[] argumentTypes = RetrieveArgumentTypes(mapping);
            object[] argumentValues = RetrieveArgumentValues(sourceRecord, mapping);

            dataBinder.InvokeMethod(
                mapping.ObjectBindingPath,
                string.Compare(mapping.MemberInformation.Type.ToString().ToLower(), "static", false) == 0 ? true : false,
                mapping.MemberInformation.Name,
                argumentTypes,
                argumentValues);
        }

        private Type[] RetrieveArgumentTypes(Mappings.Mapping mapping)
        {
            List<Mappings.Arg> args = mapping.MemberInformation.Args;
            Type[] types = new Type[args.Count];

            for (int i = 0; i < args.Count; i++)
            {
                types[i] = GetTypeFromName(args[i].Type);
            }

            return types;
        }

        private object[] RetrieveArgumentValues(ISourceRecord sourceRecord, Mappings.Mapping mapping)
        {
            List<Mappings.Arg> args = mapping.MemberInformation.Args;
            object[] values = new object[args.Count];

            for (int i = 0; i < args.Count; i++)
            {
                values[i] = TransformArgumentValue(sourceRecord, args[i]);
            }

            return values;
        }

        /// <summary>
        /// Based on field mapping information, transforms a record's field original value to its desired value that is legitimate for saving to database.
        /// </summary>
        /// <param name="sourceRecord">The record being processed</param>
        /// <param name="arg">The argument information for a specific source field</param>
        /// <returns>The legitimate value for saving to database</returns>
        private object TransformArgumentValue(ISourceRecord sourceRecord, Mappings.Arg arg)
        {
            object sourceValue = null;

            // Step 1: Check 'input' and get the source value.
            sourceValue = GetSourceValue(sourceRecord, arg);
            if (sourceValue == null || sourceValue == DBNull.Value)
                return sourceValue;

            string stringRepValue = sourceValue.ToString();

            // Step 2: Resolve source value by translation file, if necessary.
            if (arg.Resolver != null)
                stringRepValue = ResolveValue(stringRepValue, arg.Resolver);

            // Step 3: Convert from value to Id by pickList, if neccary.
            if (!string.IsNullOrEmpty(arg.PickListCode))
                stringRepValue = ConvertPicklistValueToId(arg.PickListCode, stringRepValue);

            // Step 4: Convert to desired data type, specified by 'type'.
            return DataTypingUtility.ConvertToType(stringRepValue, arg.Type);
        }

        private object GetSourceValue(ISourceRecord sourceRecord, Mappings.Arg arg)
        {
            object sourceValue = null;

            if (arg.Input == Mappings.InputEnum.Constant)
            {
                sourceValue = arg.Value;
            }
            else
            {
                foreach (KeyValuePair<string, Type> kvp in SourceFieldTypes.TypeDefinitions)
                {
                    if (string.Compare(kvp.Key, arg.Value, true) == 0)
                    {
                        sourceRecord.FieldSet.TryGetValue(kvp.Key, out sourceValue);
                        break;
                    }
                }
            }

            return sourceValue;
        }

        private string ResolveValue(string valueToResolve, Mappings.Resolver resolver)
        {
            Dictionary<string, string> translationDictionary =
                CSVTranslator.DictionaryFromTranslationFile(
                resolver.File,
                Mappings.ConvertDelimiterEnumToString(resolver.Delimiter),
                resolver.ExternalValueColumn,
                resolver.InternalValueColumn
                );

            foreach (KeyValuePair<string, string> kvp in translationDictionary)
            {
                if (string.Compare(kvp.Key, valueToResolve, true) == 0)
                {
                    return kvp.Value;
                }
            }

            // If no translation information is found from translation file, simply return the
            // original value.
            return valueToResolve;
        }

        private string ConvertPicklistValueToId(string picklistCode, string value)
        {
            string id = string.Empty;
            Picklist picklist = JobUtility.GetPicklistByCode(picklistCode);

            if (picklist != null)
            {
                id = picklist.GetListItemIdByValue(value).ToString();
            }

            return id;
        }

        private Type GetTypeFromName(string typeName)
        {
            switch (typeName.ToUpper())
            {
                case "INT":
                    return typeof(int); break;
                case "DOUBLE":
                    return typeof(double); break;
                case "DATETIME":
                    return typeof(DateTime); break;
                case "FLOAT":
                    return typeof(float); break;
                default:
                    return typeof(string); break;
            }
        }

        /// <summary>
        /// Rise RecordMapping event.
        /// </summary>
        /// <param name="sourceRecord">Mapping source record</param>
        private void OnRecordProcessing(ISourceRecord sourceRecord)
        {
            if (RecordProcessing != null)
                RecordProcessing(this, new RecordProcessingEventArgs(sourceRecord));
        }

        /// <summary>
        /// Rise JobComplete event
        /// </summary>
        /// <param name="chunkSize">The count of source records that are mapped</param>
        private void OnRecordsMapped(Dictionary<ISourceRecord, string> unmappedSourceRecords, int count)
        {
            base.OnJobComplete(this, new RecordsProcessedEventArgs(unmappedSourceRecords, count));
        }

    }
}
