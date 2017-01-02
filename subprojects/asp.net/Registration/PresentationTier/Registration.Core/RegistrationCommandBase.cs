using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration
{
    [Serializable()]
    public abstract class RegistrationCommandBase: CommandBase
    {
        [NonSerialized, NotUndoable]
        private RegistrationOracleDAL _regDal = null;
        /// <summary>
        /// DAL implementation.
        /// </summary>
        protected RegistrationOracleDAL RegDal
        {
            get
            {
                if (_regDal == null)
                    DalUtils.GetRegistrationDAL(ref _regDal, Constants.SERVICENAME);
                return _regDal;
            }
        }
    }
}
