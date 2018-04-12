using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    class MyFunctions
    {
        private string _strMyString = "My string!";
        public string MyString() { return _strMyString; }
        public static bool b()
        {
            return true;
        }
        public static Double d(Double v1)
        {
            return v1 * 2;
        }
        public static Double d2(Double v1, Double v2)
        {
            return (v1 * 2) + v2;
        }
        public static String e()
        {
            return "empty!";
        }
        public static Int32 i(Int32 v1)
        {
            return v1 * 2;
        }
        public static Int32 i2(Int32 v1, Int32 v2)
        {
            return v1 * v2;
        }
        public static Double d(Object v1)
        {
            return 1024;
        }
    } // class MyFunctions
}
