using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;

/*
 * This class will be initialized by a RegistryRecord and a list of possible component
 * duplicates. For now, it is fully-automated.
 */

namespace CambridgeSoft.COE.Registration
{
    public class ComponentDuplicateResolver
    {
        public static RegistryRecord ResolveUsingAction(
            RegistryRecord registration
            , DuplicateAction action
            , List<DuplicateCheckResponse> duplicates
            )
        {
            RegistryRecord result = null;
            switch (action)
            {
                case DuplicateAction.Batch:
                    {
                        result = ResolveUsingNewBatch(registration, duplicates[0]);
                        break;
                    }
                //case RegistryRecord.DuplicateAction.Duplicate:
                //    {
                //        result = registration.Register(
                //            RegistryRecord.DuplicateAction.None
                //            , RegistryRecord.DataAccessStrategy.BulkLoader
                //            );
                //        break;
                //    }
                //case RegistryRecord.DuplicateAction.Temporary:
                //    {
                //        result = registration.Save(
                //            RegistryRecord.DuplicateCheck.None
                //            , RegistryRecord.DataAccessStrategy.BulkLoader
                //            );
                //        break;
                //    }
            }
            return result;
        }

        private static RegistryRecord ResolveUsingNewBatch(
            RegistryRecord currentRegistration
            , DuplicateCheckResponse existingRecordInfo
            )
        {
            DateTime before = DateTime.Now;
            //fetch the first matching pre-existing record (the 'duplicate')
            RegistryRecord existingRegistration = 
                RegistryRecord.GetRegistryRecord(existingRecordInfo.MatchedRegistrations[0].RegistryNumber);
            Trace.WriteLine(string.Format("{0} seconds for fetching first matching record", DateTime.Now.Subtract(before).TotalSeconds));

            before = DateTime.Now;
            Trace.Indent();
            //use the current batch as the source, as a new Batch as a target
            Batch currentBatch = currentRegistration.BatchList[0];
            currentBatch.BatchComponentList[0].ComponentIndex = existingRegistration.ComponentList[0].ComponentIndex;
            currentBatch.BatchComponentList[0].CompoundID = existingRegistration.ComponentList[0].Compound.ID;
            Batch newBatch = Batch.NewBatch(currentBatch.UpdateSelf(false), true, false); ;
            existingRegistration.BatchList.Add(newBatch);

            //add the batch and save the existing record
            RegistryRecord result = existingRegistration.Save(RegistryRecord.DataAccessStrategy.BulkLoader);
            Trace.Unindent();
            Trace.WriteLine(string.Format("{0} seconds for updating first matching record", DateTime.Now.Subtract(before).TotalSeconds));
            return result;
        }
    }
}
