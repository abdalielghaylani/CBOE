using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Types.Exceptions {
    /// <summary>
    /// Exception raised when an error ocurred inside the SQL Generator sub system.
    /// </summary>
    [Serializable]
    public class SQLGeneratorException : Exception {
        public SQLGeneratorException() : base() { }
        public SQLGeneratorException(string message) : base(message) { }
        public SQLGeneratorException(string message, Exception innerException) : base(message, innerException) { }
        public SQLGeneratorException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }
    }
}
