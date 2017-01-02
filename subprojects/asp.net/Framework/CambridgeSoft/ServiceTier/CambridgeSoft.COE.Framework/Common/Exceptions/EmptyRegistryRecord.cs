using System;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.Types.Exceptions
{
    /// <summary>
    /// Exception raised for empties registry records
    /// </summary>
    [Serializable]
    public class EmptyRegistryRecord  : Exception
    {
        public EmptyRegistryRecord() : base("Empty Registry Record") { }
        public EmptyRegistryRecord(string message) : base(message.ToString()) { }
    }
}
