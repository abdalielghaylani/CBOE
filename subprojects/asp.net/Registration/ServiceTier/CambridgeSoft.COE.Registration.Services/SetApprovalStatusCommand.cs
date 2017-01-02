using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services
{
    [Serializable]
    public class SetApprovalStatusCommand : RegistrationCommandBase
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
                SetApprovalStatusCommand cmd = new SetApprovalStatusCommand(tempId, status, personApproved);
                cmd = DataPortal.Execute<SetApprovalStatusCommand>(cmd);
                result = cmd._result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        private SetApprovalStatusCommand(int tempId, RegistryStatus status, int personApproved)
        {
            _regTempId = tempId;
            _status = status;
            _personApproved = personApproved;
        }

        protected override void DataPortal_Execute()
        {
            try
            {
                this.RegDal.UpdateApprovedStatus(_regTempId, _status, _personApproved);
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
