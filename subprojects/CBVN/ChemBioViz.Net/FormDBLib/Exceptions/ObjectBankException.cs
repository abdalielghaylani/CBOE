using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormDBLib.Exceptions
{
    /// <summary>
    ///   ObjectBankException handles the exceptions that occurred when using the ObjectBank class and its subclasses
    /// </summary>
    public class ObjectBankException : Exception
    {
        #region Constructors
        public ObjectBankException() : base() {}
        public ObjectBankException(string message) : base(message) { }
        public ObjectBankException(string message, Exception ex) : base(message, ex) { }
        #endregion
    }
}
