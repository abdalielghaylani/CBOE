using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Types.Exceptions {
    /// <summary>
    /// Exception raised when trying to use an unsupported data type. Used in several contexts.
    /// </summary>
    /// 
    [Serializable]
    public class UnsupportedDataTypeException : Exception {
        public UnsupportedDataTypeException() : base() { }
        public UnsupportedDataTypeException(string message) : base(message) { }
        public UnsupportedDataTypeException(string message, Exception innerException) : base(message, innerException) { }
        public UnsupportedDataTypeException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }
    }
}
