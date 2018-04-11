using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Registration-specific implementation of ReadOnlyBase. Added primarily to eliminate
    /// repetitive concerns over the Csla 2.0 'ID' property.
    /// </summary>
    /// <typeparam name="T">the type of object being wrapped</typeparam>
    [Serializable()]
    public class RegistrationReadOnlyBase<T> : ReadOnlyBase<T> where T : ReadOnlyBase<T>
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
