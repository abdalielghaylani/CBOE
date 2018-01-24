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
    /// Class to perform bulk lock scenario.
    /// </summary>
    [Serializable()]
    public class BulkLock : RegistrationCommandBase
    {
          int _hitListId;
        bool _result;

        public static bool Execute(int hitListId)
        {
            bool result = false;
            try
            {
                BulkLock cmd = new BulkLock(hitListId);
                cmd = DataPortal.Execute<BulkLock>(cmd);
                result = cmd._result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }
        private BulkLock(int hitListId)
        {
            this._hitListId = hitListId;
        }

        protected override void DataPortal_Execute()
        {
            try
            {
               string idString=string.Empty;
               idString = this.RegDal.GetRegIDListFromHitList(_hitListId);

                if (!string.IsNullOrEmpty(idString))
                {
                    this.RegDal.UpdateLockedStatus(idString, RegistryStatus.Locked, COEUser.ID);
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
