using System;
using System.Collections.Generic;
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class BulkRegistrationHelper
    {
        public static int BulkRegisterRecords(List<string> records, DuplicateAction duplicateAction, string description, string username)
        {
            int logId = CriteriaLog.InsertCriteriaLog(duplicateAction, username, description);
            bool approvalEnabled = RegUtilities.GetApprovalsEnabled();
            try
            {
                foreach (string regId in records)
                {
                    RegistryRecord record = RegistryRecord.GetRegistryRecord(int.Parse(regId));
                    if (record == null) continue;

                    if (!record.IsEditable || !record.CanEditRegistry())
                        CriteriaBulkLog.InsertCriteriaBulkLog(logId, regId, duplicateAction, string.Empty, 0, Resource.NotAuthorizedToRegisterTempRecords);
                    else if (approvalEnabled && record.Status != RegistryStatus.Approved)
                        CriteriaBulkLog.InsertCriteriaBulkLog(logId, regId, duplicateAction, string.Empty, 0, Resource.NotApprovedCannotRegister);
                    else
                    {
                        record.DataStrategy = RegistryRecord.DataAccessStrategy.BulkLoader;
                        record.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;

                        RegistryRecord result = record.Register(duplicateAction, record.DataStrategy);

                        if (result != null && !string.IsNullOrEmpty(result.RegNumber.RegNum) && result.RegNumber.RegNum.ToLower() != "null")
                        {
                            if (duplicateAction != DuplicateAction.Temporary && duplicateAction != DuplicateAction.None)
                            {
                                // Next two lines are to make the record deletable
                                record.Status = RegistryStatus.Submitted;
                                record.SetApprovalStatus();
                                RegistryRecord.DeleteRegistryRecord(record.ID);
                                CriteriaBulkLog.InsertCriteriaBulkLog(logId, string.Empty, string.IsNullOrEmpty(record.FoundDuplicates) ? result.ActionDuplicates : duplicateAction, result.RegNumber.RegNum, 0, result.DalResponseMessage);
                            }
                            else if (string.IsNullOrEmpty(record.FoundDuplicates) && (duplicateAction == DuplicateAction.Temporary || duplicateAction == DuplicateAction.None))
                            {
                                // Next two lines are to make the record deletable
                                record.Status = RegistryStatus.Submitted;
                                record.SetApprovalStatus();
                                RegistryRecord.DeleteRegistryRecord(record.ID);
                                CriteriaBulkLog.InsertCriteriaBulkLog(logId, string.Empty, DuplicateAction.Compound, result.RegNumber.RegNum, 0, result.DalResponseMessage);
                            }
                            else
                            {
                                string message = result.DalResponseMessage;
                                if ((duplicateAction != DuplicateAction.Temporary || duplicateAction != DuplicateAction.None) && !string.IsNullOrEmpty(record.FoundDuplicates) && string.IsNullOrEmpty(message))
                                    message = "Duplicate found";
                                CriteriaBulkLog.InsertCriteriaBulkLog(logId, regId, duplicateAction, string.Empty, 0, message);
                            }
                        }
                        else
                        {
                            if (result != null)
                            {
                                if ((result.IsTemporal && result.SubmitCheckRedBoxWarning) || (!result.IsTemporal && result.RegisterCheckRedBoxWarning))
                                {
                                    // string  message = "Chem Draw warnings found";
                                    string message = Resource.RedBoxWarningMessage_BulkReg;
                                    CriteriaBulkLog.InsertCriteriaBulkLog(logId, regId, duplicateAction == DuplicateAction.None ? DuplicateAction.None : DuplicateAction.Temporary, string.Empty, 0, message);
                                }
                                else
                                {
                                    string message = result.DalResponseMessage;
                                    if (!string.IsNullOrEmpty(record.FoundDuplicates) && string.IsNullOrEmpty(message))
                                        message = "Duplicate found";
                                    CriteriaBulkLog.InsertCriteriaBulkLog(logId, regId, duplicateAction == DuplicateAction.None ? DuplicateAction.None : DuplicateAction.Temporary, string.Empty, 0, message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                COEExceptionDispatcher.HandleBLLException(e);
                throw e;
            }

            return logId;
        }
    }
}