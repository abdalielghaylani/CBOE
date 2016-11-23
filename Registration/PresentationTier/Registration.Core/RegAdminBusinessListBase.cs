using System;
using System.Collections.Generic;
using System.Text;

using Csla;

using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.RegistrationAdmin.Access;

namespace CambridgeSoft.COE.Registration
{
    [Serializable]
    public abstract class RegAdminBusinessListBase<T, C>
        : BusinessListWithValidation<T, C>
        where T : BusinessListWithValidation<T, C>
        where C : Csla.Core.IEditableBusinessObject
    {

        [NonSerialized, NotUndoable]
        private RegAdminOracleDAL _regDal = null;
        /// <summary>
        /// Lazy-loading DAL implementation.
        /// </summary>
        protected RegAdminOracleDAL RegDal
        {
            get
            {
                if (_regDal == null)
                    AdminDalUtils.GetRegDal(ref _regDal, Constants.ADMINSERVICENAME);
                return _regDal;
            }
        }


    }
}
