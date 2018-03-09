using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormDBLib.Exceptions
{
    public class SearchException : Exception
    {
        #region Constructors
        public SearchException(): base() { }
        public SearchException(string message) : base(message) { }
        public SearchException(string message, Exception ex) : base(message, ex) { }
        #endregion
    }
}
