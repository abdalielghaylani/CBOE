using System;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.Types.Exceptions
{
    /// <summary>
    /// Exception raised when a compound id is not valid
    /// </summary>
    [Serializable]
    public class InvalidCompoundIDType : Exception
    {
        public InvalidCompoundIDType() : base("Invalid Compound ID type") { }
        public InvalidCompoundIDType(string message) : base(message.ToString()) { }
    }
}
