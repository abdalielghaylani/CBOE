using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FormDBLib
{
    public class NumericComparer : System.Collections.Generic.IComparer<String>
    {
        #region Constructors
        public NumericComparer()
        { }
        #endregion

        #region Methods 
        public int Compare(string x, string y)
        {
            return StringLogicalComparer.Compare((string)x, (string)y);
        }
        #endregion
    }
}
