using System;
using System.Runtime.Serialization;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COESecurityService
{
    /// <summary>
    /// Exception raised for empties registry records
    /// </summary>
    [Serializable]
    public class InvalidConnection : Exception
    {
        public InvalidConnection()  { }
        public InvalidConnection(string message) : base(message.ToString()) { }
        public InvalidConnection(string message, Exception innerException) : base(message, innerException) { }
        public InvalidConnection(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }

        
    }
}
