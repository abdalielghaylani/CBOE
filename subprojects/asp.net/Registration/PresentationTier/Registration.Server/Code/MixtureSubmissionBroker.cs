using System;
using System.Collections.Generic;

using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;


namespace PerkinElmer.COE.Registration.Server.Code
{
    public class MixtureSubmissionBroker: ISubmissionBroker
    {
        #region ISubmissionBroker Members

        public IMatchResponse MakeDecision(RegistryRecord subject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Submit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Register()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
