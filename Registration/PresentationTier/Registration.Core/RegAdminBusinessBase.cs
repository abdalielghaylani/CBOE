using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using CambridgeSoft.COE.RegistrationAdmin.Access;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Registration-specific implementation of BusinessBase. Added primarily to eliminate
    /// repetitive concerns over the Csla 2.0 'ID' property.
    /// </summary>
    /// <typeparam name="T">the type of object being wrapped</typeparam>
    [Serializable()]
    public abstract class RegAdminBusinessBase<T> : BusinessBase<T> where T : BusinessBase<T>
    {
        private int _id;
        /// <summary>
        /// Added to fulfill Csla 2 requirement.
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Added to fulfill Csla 2 requirement.
        /// </summary>
        protected override object GetIdValue()
        {
            return _id;
        }

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
