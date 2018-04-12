using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Calculation
{
    class BuiltinFunctions
    {
        public static Boolean Iif(Boolean b, Boolean t, Boolean f) { return (b) ? t : f; }
        public static Char Iif(Boolean b, Char t, Char f) { return (b) ? t : f; }
        public static DateTime Iif(Boolean b, DateTime t, DateTime f) { return (b) ? t : f; }
        public static Double Iif(Boolean b, Double t, Double f) { return (b) ? t : f; }
        public static Int32 Iif(Boolean b, Int32 t, Int32 f) { return (b) ? t : f; }
        public static String Iif(Boolean b, String t, String f) { return (b) ? t : f; }
        public static Boolean IsNull(Boolean b) { return false; }
        public static Boolean IsNull(Char c) { return false; }
        public static Boolean IsNull(DateTime dt) { return false; }
        public static Boolean IsNull(Double b) { return false; }
        public static Boolean IsNull(Int32 b) { return false; }
        public static Boolean IsNull(String b) { return false; }
        public static Boolean IsNull(Object x) { return true; }
    } // class BuiltinFunctions
}
