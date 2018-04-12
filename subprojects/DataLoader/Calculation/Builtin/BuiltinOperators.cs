using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Calculation.Parser
{
    class BuiltinOperators
    {
        [CalculationParser.OperatorInfo("||", 4, true)] public static Boolean Bor(Boolean b1, Boolean b2) { return b1 || b2; }
        [CalculationParser.OperatorInfo("&&", 5, true)] public static Boolean Band(Boolean b1, Boolean b2) { return b1 && b2; }
        [CalculationParser.OperatorInfo("==", 8, true)] public static Boolean Beq(Boolean b1, Boolean b2) { return b1 == b2; }
        [CalculationParser.OperatorInfo("==", 8, true)] public static Boolean Deq(Double d1, Double d2) { return d1 == d2; }
        [CalculationParser.OperatorInfo("==", 8, true)] public static Boolean Ieq(Int32 i1, Int32 i2) { return i1 == i2; }
        [CalculationParser.OperatorInfo("==", 8, true)] public static Boolean Seq(String s1, String s2) { return s1.CompareTo(s2) == 0; }
        [CalculationParser.OperatorInfo("!=", 8, true)] public static Boolean Bne(Boolean b1, Boolean b2) { return b1 != b2; }
        [CalculationParser.OperatorInfo("!=", 8, true)] public static Boolean Dne(Double d1, Double d2) { return d1 != d2; }
        [CalculationParser.OperatorInfo("!=", 8, true)] public static Boolean Ine(Int32 i1, Int32 i2) { return i1 != i2; }
        [CalculationParser.OperatorInfo("!=", 8, true)] public static Boolean Sne(String s1, String s2) { return s1.CompareTo(s2) != 0; }
        [CalculationParser.OperatorInfo("<", 9, true)] public static Boolean Dlt(Double d1, Double d2) { return d1 < d2; }
        [CalculationParser.OperatorInfo("<", 9, true)] public static Boolean Ilt(Int32 i1, Int32 i2) { return i1 < i2; }
        [CalculationParser.OperatorInfo("<", 9, true)] public static Boolean Slt(String s1, String s2) { return s1.CompareTo(s2) < 0; }
        [CalculationParser.OperatorInfo("<=", 9, true)] public static Boolean Dle(Double d1, Double d2) { return d1 <= d2; }
        [CalculationParser.OperatorInfo("<=", 9, true)] public static Boolean Ile(Int32 i1, Int32 i2) { return i1 <= i2; }
        [CalculationParser.OperatorInfo("<=", 9, true)] public static Boolean Sle(String s1, String s2) { return s1.CompareTo(s2) <= 0; }
        [CalculationParser.OperatorInfo(">", 9, true)] public static Boolean Dgt(Double d1, Double d2) { return d1 > d2; }
        [CalculationParser.OperatorInfo(">", 9, true)] public static Boolean Igt(Int32 i1, Int32 i2) { return i1 > i2; }
        [CalculationParser.OperatorInfo(">", 9, true)] public static Boolean Sgt(String s1, String s2) { return s1.CompareTo(s2) > 0; }
        [CalculationParser.OperatorInfo(">=", 9, true)] public static Boolean Dge(Double d1, Double d2) { return d1 >= d2; }
        [CalculationParser.OperatorInfo(">=", 9, true)] public static Boolean Ige(Int32 i1, Int32 i2) { return i1 >= i2; }
        [CalculationParser.OperatorInfo(">=", 9, true)] public static Boolean Sge(String s1, String s2) { return s1.CompareTo(s2) >= 0; }
        [CalculationParser.OperatorInfo("+", 11, true)] public static Int32 Iplus(Int32 i1, Int32 i2) { return i1 + i2; }
        [CalculationParser.OperatorInfo("+", 11, true)] public static Double Dplus(Double d1, Double d2) { return d1 + d2; }
        [CalculationParser.OperatorInfo("-", 11, true)] public static Int32 Iminus(Int32 i1, Int32 i2) { return i1 - i2; }
        [CalculationParser.OperatorInfo("-", 11, true)] public static Double Dminus(Double d1, Double d2) { return d1 - d2; }
        [CalculationParser.OperatorInfo("*", 12, true)] public static Int32 Imul(Int32 i1, Int32 i2) { return i1 * i2; }
        [CalculationParser.OperatorInfo("*", 12, true)] public static Double Dmul(Double d1, Double d2) { return d1 * d2; }
        [CalculationParser.OperatorInfo("/", 12, true)] public static Int32 Idiv(Int32 i1, Int32 i2) { return i1 / i2; }
        [CalculationParser.OperatorInfo("/", 12, true)] public static Double Ddiv(Double d1, Double d2) { return d1 / d2; }
        [CalculationParser.OperatorInfo("%", 12, true)] public static Int32 Imod(Int32 i1, Int32 i2) { return i1 % i2; }
        [CalculationParser.OperatorInfo("U!", 13, false)] public static Boolean UBnot(Boolean b) { return !b; }
        [CalculationParser.OperatorInfo("U+", 13, false)] public static Double UDplus(Double d) { return d; }
        [CalculationParser.OperatorInfo("U+", 13, false)] public static Int32 UIplus(Int32 i) { return i; }
        [CalculationParser.OperatorInfo("U-", 13, false)] public static Double UDminus(Double d) { return -d; }
        [CalculationParser.OperatorInfo("U-", 13, false)] public static Int32 UIminus(Int32 i) { return -i; }
    } // class BuiltinOperators
}
