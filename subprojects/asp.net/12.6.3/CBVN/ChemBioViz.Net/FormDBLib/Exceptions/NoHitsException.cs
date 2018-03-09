using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormDBLib.Exceptions
{
    public class NoHitsException :  Exception
    {
        #region Constructors
        public NoHitsException(): base("No hits to return") { }
        public NoHitsException(string message) : base(message) { }
        public NoHitsException(string message, Exception ex) : base(message, ex) { }
        #endregion
    }
}
