using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services
{
   public class SetLockedStatusCommand: RegistrationCommandBase
    {
        int _regTempId;
        bool _result;
        RegistryStatus _status;
        int _personApproved;

        public static bool Execute(int tempId, RegistryStatus status, int personApproved)
        {
            bool result = false;
            try
            {
                SetLockedStatusCommand cmd = new SetLockedStatusCommand(tempId, status, personApproved);
                cmd = DataPortal.Execute<SetLockedStatusCommand>(cmd);
                result = cmd._result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        private SetLockedStatusCommand(int tempId, RegistryStatus status, int personApproved)
        {
            _regTempId = tempId;
            _status = status;
            _personApproved = personApproved;
        }

        protected override void DataPortal_Execute()
        {
            try
            {
                this.RegDal.UpdateLockedStatus(_regTempId.ToString(),_status,_personApproved);
                _result = true;
            }
            catch (Exception ex)
            {
                _result = false;
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }
    }
}
