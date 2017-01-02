using System;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.ExceptionHandling
{
    [Serializable]
    public class COEDALException : COEBaseException
    {
        private bool _shouldDisplayMessageToUsers;
        private string _connectionString;
        private string _commandText;
        private CommandType _commandType;
        private DbParameterCollection _parameters=null;

        /// <summary>
        /// Indicates whether the exception message should be displayed to the end user
        /// </summary>
        public bool ShouldDisplayMessageToUsers
        {
            get
            {
                return _shouldDisplayMessageToUsers;
            }
            set
            {
                _shouldDisplayMessageToUsers = value;
            }
        }

        private bool _isDBException;

        public bool IsDBException
        {
            get { return _isDBException; }
            set { _isDBException = value; }
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public string CommandText
        {
            get
            {
                return _commandText;
            }
            set
            {
                _commandText = value;
            }
        }

        public CommandType CommandType
        {
            get
            {
                return _commandType;
            }
            set
            {
                _commandType = value;
            }
        }

        public DbParameterCollection Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }

        }

        /// <summary>
		/// Default constructor
		/// </summary>
		public COEDALException() : base()
		{
		}

		/// <summary>
		/// Initializes with a specified error message.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		public COEDALException(string message,bool shouldDisplayMessageToUsers)
            : base(message)
		{
            this.ShouldDisplayMessageToUsers = shouldDisplayMessageToUsers;
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
        public COEDALException(string message, Exception innerException, bool isDBException)
            : base(message, innerException)
        {
            this.IsDBException = isDBException;
        }

        /// <summary>
        /// Initializes with serilized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public COEDALException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

		/// <summary>
		/// Initializes with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.
		/// </param>
        protected COEDALException(SerializationInfo info, StreamingContext context, bool shouldDisplayMessageToUsers)
            : base(info, context) 
		{
            this.ShouldDisplayMessageToUsers = shouldDisplayMessageToUsers;
		}

        /// <summary>
        /// Override SetExceptionContext(ExceptionContext context) to add information about the Command that cause this exception
        /// </summary>
        /// <param name="context">A <seealso cref="ExceptionContext"/> type
        /// argument containing additional information about the exception</param>
        public override void SetExceptionContext(ExceptionContext context)
        {
            if (context != null)
            {
                if (context.Command != null)
                {
                    if (context.Command.Connection != null)
                    {
                        this.ConnectionString = context.Command.Connection.ConnectionString;
                    }
                    this.CommandText = context.Command.CommandText;
                    this.CommandType = context.Command.CommandType;
                    this.Parameters = context.Command.Parameters;
                }
            }
            base.SetExceptionContext(context);
        }
    }
}
