using System;
using System.Web;
using System.Runtime.Serialization;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.ExceptionHandling
{
    /// <summary>
    /// The base class of all coe exception types.
    /// </summary>
    public class COEBaseException : ApplicationException
    {
        // TODO: Add ToString() override to provide custom string representation for each type of exception.
        private string _clientIP;
        private string _registrationUserID;
        private string _userActionDescription;

        /// <summary>
        /// Client machine's IP address
        /// </summary>
        public string ClientIP
        {
            get
            {
                return _clientIP;
            }
            set
            {
                _clientIP = value;
            }
        }

        /// <summary>
        /// User name of the logged in user
        /// </summary>
        public string RegistrationUserID
        {
            get
            {
                return _registrationUserID;
            }
            set
            {
                _registrationUserID = value;
            }
        }

        /// <summary>
        /// Describe behavior of user action causing this exception 
        /// </summary>
        public string UserActionDescription
        {
            get
            {
                return this._userActionDescription;
            }
            set
            {
                this._userActionDescription = value;
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public COEBaseException()
            : base()
        {
        }

        /// <summary>
        /// Initializes with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public COEBaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes with a specified error 
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.
        /// </param>
        /// <param name="exception">The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference, the current exception 
        /// is raised in a catch block that handles the inner exception.
        /// </param>
        public COEBaseException(string message, Exception exception)
            :base(message, exception)
        {
        }

        /// <summary>
        /// Initializes with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.
        /// </param>
        protected COEBaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Set information about the client machine and user that cause the exception
        /// </summary>
        private void SetClientInfo()
        {
            this.ClientIP = COEUser.CurrentIP;
            this.RegistrationUserID = COEUser.Name;
        }

        /// <summary>
        /// <para>To add additional information to further describe the exception</para>
        /// <para>
        /// Override this method in derived classes to add information specific to that type of exception
        /// </para>
        /// </summary>
        /// <param name="context">A <seealso cref="ExceptionContext"/> type
        /// argument containing additional information about the exception
        /// </param>
        public virtual void SetExceptionContext(ExceptionContext context)
        {
            SetClientInfo();
        }
    }
}
