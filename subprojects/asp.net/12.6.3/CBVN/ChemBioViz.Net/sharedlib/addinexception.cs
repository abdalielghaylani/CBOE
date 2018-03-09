using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedLib
{
    public class AddinException : Exception
    {
        #region Constructors
        public AddinException() : base() { }
        public AddinException(string message) : base(message) { }
        public AddinException(string message, Exception ex) : base(message, ex) { }
        #endregion
    }
}
