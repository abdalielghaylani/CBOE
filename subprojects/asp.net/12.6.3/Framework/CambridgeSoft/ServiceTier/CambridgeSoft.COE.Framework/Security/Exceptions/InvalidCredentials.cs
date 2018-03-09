using System;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.COESecurityService
{
    /// <summary>
    /// Exception raised for empties registry records
    /// </summary>
    [Serializable]
    public class InvalidCredentials  : Exception
    {
        public InvalidCredentials()  { }
        public InvalidCredentials(string message) : base(message.ToString()) { }
        public InvalidCredentials(string message, Exception innerException) : base(message, innerException) { }
        public InvalidCredentials(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }

    }
}
