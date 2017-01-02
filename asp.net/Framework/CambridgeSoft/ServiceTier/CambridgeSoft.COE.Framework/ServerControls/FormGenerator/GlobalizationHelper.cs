using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using CambridgeSoft.COE.Framework.Common;
using System.Text.RegularExpressions;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    public abstract class GlobalizationHelper
    {
        public static string FormatData(string inputStr, COEDataView.AbstractTypes dataType, NumberFormatInfo inFormat, NumberFormatInfo outFormat)
        {
            if (string.IsNullOrEmpty(inputStr))
                return inputStr;

            if (dataType == COEDataView.AbstractTypes.Real)
            {
                double val;
                if (Double.TryParse(inputStr, NumberStyles.Float, inFormat, out val))
                    return val.ToString(outFormat);
                else
                {
                    //{RANGES}: (double-double), (-double-double), (double--double), (-double--double)
                    //{OPERATOR OVERRIDE}: (< double), (<= double), (> double), (>= double)
                    //{MULTIPLE OPERATOR OVERRIDE}: {OPERATOR OVERRIDE} AND {OPERATOR OVERRIDE}
                    //{BETWEEN}: BETWEEN double AND double
                    //TODO: Test IN clauses for double, -doble, ... 
                    string decimalPoint = inFormat.NumberDecimalSeparator == "." ? "\\." : inFormat.NumberDecimalSeparator;
                    Regex regex = new Regex(@"(^-?\d+" + decimalPoint + @"?\d*)|[^\d] *(-?\d+" + decimalPoint + @"?\d*)");

                    int i = 0;
                    MatchCollection matches = regex.Matches(inputStr);
                    object[] parameters = new object[matches.Count];
                    foreach (Match match in matches)
                    {
                        string valstr = string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[2].Value.Replace("--", "-") : match.Groups[1].Value.Replace("--", "-");
                        inputStr = inputStr.Replace(valstr, "{" + i + "}");
                        if (valstr.Length > 0)
                        {
                            if (Double.TryParse(valstr, NumberStyles.Float, inFormat, out val))
                            {
                                parameters[i] += val.ToString(outFormat);
                                i++;
                            }
                        }
                    }
                    return string.Format(inputStr, parameters);
                }
            }
            else
                return inputStr;
        }
    }
}
