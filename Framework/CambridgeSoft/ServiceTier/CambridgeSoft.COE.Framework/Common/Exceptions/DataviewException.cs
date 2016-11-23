using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Types.Exceptions
{
    /// <summary>
    /// Exception raised when there was a problem related to DataviewService
    /// </summary>
    [Serializable]
    public class DataviewException : Exception
    {
        public DataviewException() : base(Resources.DataViewPermissions) { }
        public DataviewException(string message) : base(message) { }
        public DataviewException(string message, Exception innerException) : base(message, innerException) { }
        public DataviewException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }
    }
}
