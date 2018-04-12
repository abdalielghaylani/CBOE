using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Calculation
{
    class BuiltinConversions
    {
        public static String ToString(Boolean b) { return b.ToString(); }
        public static String ToString(Char c) { return c.ToString(); }
        public static String ToString(DateTime dt) { return dt.ToString(); }
        public static String ToString(Double d) { return d.ToString(); }
        public static String ToString(Int32 i) { return i.ToString(); }
        
        public static Double ToDouble(Int32 i) { return (Double)i; }

        public static Boolean[] ToBooleanArray(Boolean b) { return new Boolean[] { b }; }
        public static Char[] ToCharArray(Char c) { return new Char[] { c }; }
        public static Double[] ToDoubleArray(Double d) { return new Double[] { d }; }
        public static Int32[] ToInt32Array(Int32 i) { return new Int32[] { i }; }
        public static String[] ToStringArray(String s) { return new String[] { s }; }
    } // class BuildinConversions
}
