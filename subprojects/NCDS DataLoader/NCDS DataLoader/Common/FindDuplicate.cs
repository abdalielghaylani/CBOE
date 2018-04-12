using System;
using System.Diagnostics;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.NCDS_DataLoader.Common
{
    public class FindDuplicate
    {
        public const string DUPLICATE_CHECK_RESPONSE = "Duplicate_Check_Response";

        /// <summary>
        /// Find duplicated record of the destination records
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameters object.
        /// JobParameters.DestinationRecords is required
        /// </param>
        //public FindDuplicate() : base(jobParameters) { }

        /// <summary>
        /// // This service only applies when the destination object type is RegistryRecord
        /// </summary>
        /// <returns></returns>
        public JobResponse DoJobInternal(JobParameters jobParameters)
        {
            JobResponse jobResponse = new JobResponse(jobParameters);

            List<IDestinationRecord> destinationRecords = jobParameters.DestinationRecords;
            RegistrationLoadList registrationLoadList = RegistrationLoadList.NewRegistrationLoadList();
            RegistryRecord registryRecord = null;

            for (int j = 0; j < destinationRecords.Count; j++)
            {
                registryRecord = destinationRecords[j] as RegistryRecord;

                if (registryRecord != null)
                {
                    registrationLoadList.AddRegistration(registryRecord);
                }
            }

            jobResponse.ResponseContext[DUPLICATE_CHECK_RESPONSE] = registrationLoadList.CheckDuplicates();

            return jobResponse;
        }


    }
}
