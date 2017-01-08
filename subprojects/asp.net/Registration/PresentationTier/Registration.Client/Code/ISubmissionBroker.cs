using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;

namespace PerkinElmer.CBOE.Registration.Client.Code
{
    // interface inherited by submission broker classes.  the class is used by the UI code behind to
    //simplifiy actions required for submission
    interface ISubmissionBroker
    {
        IMatchResponse MakeDecision(RegistryRecord subject);
        void Submit();
        void Register();

    }
}
