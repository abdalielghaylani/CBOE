using System;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.Types.Exceptions
{
    /// <summary>
    /// Exception raised when there are no records to be saved.
    /// </summary>
    [Serializable]
    public class NoRecordToSaveException : SystemException
    {
        public NoRecordToSaveException() : base("There's no record to save!") { }
        public NoRecordToSaveException(string message) : base(message.ToString()) { }
    }
}
