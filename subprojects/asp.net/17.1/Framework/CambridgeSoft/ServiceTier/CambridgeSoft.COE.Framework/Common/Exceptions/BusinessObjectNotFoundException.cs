using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Security;
using System.Runtime.Serialization;
using CambridgeSoft.COE.Framework.Common.Exceptions;

namespace CambridgeSoft.COE.Framework.Common.Exceptions
{
    /// <summary>
    /// For Registration Web Services, provides an exception callers can use when requested
    /// objects were not retrievable from the object repository.
    /// </summary>
    [Serializable]
    public class BusinessObjectNotFoundException : Exception, ISerializable, ICOEException
    {
        private string _objectIdentifierSought = string.Empty;
        /// <summary>
        /// The identifier for the sought object.
        /// </summary>
        public string ObjectIdentifierSought
        {
            get { return _objectIdentifierSought; }
        }

        private Type _objectTypeSought;
        /// <summary>
        /// The System.Type of object that was being requested.
        /// </summary>
        private Type ObjectTypeSought
        {
            get { return _objectTypeSought; }
        }

        /// <summary>
        /// Provide a custom ToString() method for this business exception.
        /// </summary>
        /// <returns>String representing a business exception</returns>
        public string ToShortErrorString()
        {
            return ToErrorString(false);
        }

        /// <summary>
        /// Provides a custom exception representation
        /// </summary>
        /// <param name="includeInnerMessage"></param>
        /// <returns></returns>
        public string ToErrorString(bool includeInnerMessage)
        {
            if (string.IsNullOrEmpty(this._objectIdentifierSought))
            {
                if (includeInnerMessage && this.InnerException != null)
                    return this.InnerException.ToString();
                else
                    return base.ToString();
            }
            else
            {
                string buf = "Error retrieving type '{0}' using identifier '{1}'";
                buf = string.Format(buf, ObjectTypeSought, ObjectIdentifierSought);
                if (includeInnerMessage && this.InnerException != null)
                    buf += "\r\n" + this.InnerException.Message;
                return buf;
            }

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BusinessObjectNotFoundException()
            :base() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="objectIdentifier"></param>
        public BusinessObjectNotFoundException(string objectIdentifier)
            : base(string.Format("Attempt to retrieve object with ID '{0}'", objectIdentifier))
        {
            this._objectIdentifierSought = objectIdentifier;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="objectIdentifier">The ID for the object being requested</param>
        /// <param name="objectType">The Type of object being requested</param>
        public BusinessObjectNotFoundException(string objectIdentifier, Type objectType)
            : base(string.Format("Attempt to retrieve {0} with ID '{1}'", objectType.Name, objectIdentifier))
        {
            this._objectIdentifierSought = objectIdentifier;
            this._objectTypeSought = objectType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="objectIdentifier">The ID for the object being requested</param>
        /// <param name="objectType">The Type of object being requested</param>
        /// <param name="rootCause">Any exception leading to this object not being found.</param>
        public BusinessObjectNotFoundException(string objectIdentifier, Type objectType, Exception rootCause)
            : base(objectIdentifier, rootCause)
        {
            this._objectIdentifierSought = objectIdentifier;
            this._objectTypeSought = objectType;
        }

        /// <summary>
        /// Constructor enabling serialization.
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="context"></param>
        public BusinessObjectNotFoundException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) { }

    }


    /// <summary>
    /// Temporary fix for GUI's need to know about this SPECIFIC event
    /// </summary>
    public class EditAffectsOtherMixturesException : Exception, ISerializable
    {
        private Dictionary<string, string[]> _sharedComponentConflicts;
        /// <summary>
        /// A list of mixture registration numbers, keyed by component reg number.
        /// </summary>
        public Dictionary<string, string[]> SharedComponentConflicts
        {
            get { return _sharedComponentConflicts; }
        }

        /// <summary>
        /// Given a component registration number, get a listing of the mixture registration numbers
        /// that will be affected by a corresponding component edit.
        /// </summary>
        /// <param name="componentRegNumber"></param>
        /// <returns></returns>
        public string[] GetComponentConflicts(string componentRegNumber)
        {
            if (_sharedComponentConflicts != null)
            {
                string[] mixtureRegNums = null;
                if (_sharedComponentConflicts.TryGetValue(componentRegNumber, out mixtureRegNums))
                    return mixtureRegNums;
                else
                    return null;
            }
            return null;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditAffectsOtherMixturesException()
            : base() { }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public EditAffectsOtherMixturesException(string message)
            : base(message) { }

        /// <summary>
        /// Enhanced constructor, allows caller to examine the origin of the exception.
        /// </summary>
        public EditAffectsOtherMixturesException(string message, Dictionary<string, string[]> sharedComponentConflicts)
            : base(message) {
                this._sharedComponentConflicts = sharedComponentConflicts;
        }

        /// <summary>
        /// Constructor enabling serialization.
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="context"></param>
        public EditAffectsOtherMixturesException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) { }

    }
}
