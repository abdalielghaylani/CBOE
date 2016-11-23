using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Types;
using Csla.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services
{
    /// <summary>
    /// JED: This is a poor implementation. If the user marks hundreds of records, the program
    /// must transport an immense amount of data for no other reason than to flip a flag in the
    /// corresponding table column.
    /// </summary>
    [Serializable()]
    public class BulkApprove : RegistrationCommandBase
    {
        int _hitListId;
        bool _result;

        public static bool Execute(int hitListId)
        {
            bool result = false;
            try
            {
                BulkApprove cmd = new BulkApprove(hitListId);
                cmd = DataPortal.Execute<BulkApprove>(cmd);
                result = cmd._result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        private BulkApprove(int hitListId)
        {
            this._hitListId = hitListId;
        }

        protected override void DataPortal_Execute()
        {
            try
            {
                string idString = this.RegDal.GetRegistryRecordTemporaryIdList(_hitListId);

                if (!string.IsNullOrEmpty(idString))
                {
                    foreach (string regNumber in idString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        this.RegDal.UpdateApprovedStatus(int.Parse(regNumber), RegistryStatus.Approved, COEUser.ID);
                    }

                    COEHitListBO.Get(CambridgeSoft.COE.Framework.HitListType.MARKED, _hitListId).UnMarkAllHits();

                    _result = true;
                }
            }
            catch (Exception ex)
            {
                _result = false;
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }
    }
}
