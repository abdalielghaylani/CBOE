using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Types.Exceptions {
    /// <summary>
    /// Exception raised when an error ocurred inside the Mol Server.
    /// </summary>
    /// 
    [Serializable]
    public class MolServerException : Exception {
        public MolServerException() : base() { }
        public MolServerException(string message) : base(message) { }
        public MolServerException(string message, Exception innerException) : base(message, innerException) { }
        public MolServerException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }
    }
}
