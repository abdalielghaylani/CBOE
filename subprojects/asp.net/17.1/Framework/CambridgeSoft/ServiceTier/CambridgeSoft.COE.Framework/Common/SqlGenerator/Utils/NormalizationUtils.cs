using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils
{
    /// <summary>
    /// Provides methods to normalize strings.
    /// </summary>
    /// 
    public class NormalizationUtils
    {
        private static string _decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator; 
        
        /// <summary>
        ///		  Examines the provided string, which represents a real number, possibly
        ///       in exponential notation, and determines the number of significant figures present.
        ///       In the conventional simple and scientific representations of real numbers, there is
        ///       one region within the string that represents the significant digits.  The position
        ///       of this region given by (start,finish).
        ///
        ///       To determine this region:
        ///           1)  Locate the decimal point.
        ///           2)  Remove a leading minus or plus sign.
        ///
        ///           3)  Remove leading zeroes.
        ///           4)  Strip off everything including and following an exponent (E).
        ///           5)  If a decimal point is not present, strip off trailing zeroes.
        ///           6)  Count the number of characters left, ignoring any decimal point.
        ///           7)  Every number is considered to have *at least* one significant figure.
        ///
        ///       Examples of "significant regions" are:
        ///               "-003.14E01"    -> "3.14",      3 sig figs.
        ///               "0.0300"        -> ".0300",     3 sig figs.
        ///               "3020"          -> "3020",      3 sig figs.
        ///               ".00E-7"        -> ".00",       1 sig figs.
        /// 
        ///		  Penned by HEH, 12/15/95.
        ///		  Ported by DGB, 7/2003.
        ///		  Ported by fgaldeano to C#, 9/2005.
        ///		  Fixed by llillo 10/2005. 
        /// </summary>
        /// <param name="src">The string containing the representation of the original value.</param>
        /// <param name="loVal">The lower value to set.</param>
        /// <param name="hiVal">The higher value to set.</param>
        public static void GetSearchRange(string src, ref double loVal, ref double hiVal)
        {
            double v = double.Parse(src);
            int figs = GetSigFigs(src);
            double tol = SetToleranceFromSigFig(v, figs);
            double delta = 0.5 * tol;
            if (delta > 1)
            {
                delta = 0.5;
            }

            loVal = v - delta;
            hiVal = v + delta;
        }

        private static int GetSigFigs(string src)
        {
            int numSigFigs;
            // 1) Locate the decimal point.
            int decPt = src.IndexOf(_decimalSeparator);
            
            // Removing this makes 3.14 have 3 sigfigs instead of 4
            // not sure why it was added but seems wrong
            //if (decPt >= 0)
            //{
            //    decPt = decPt -1;
            //}
            
            
            /* 
             * -1 if not present
             * 2-3) Remove leading zeroes, pluses, minuses (and periods for chuckles).
             */
            int start = 0;
            string plusMinusZeroDecimal = "+-0" + _decimalSeparator;
            while (start < src.Length && plusMinusZeroDecimal.Contains(src.Substring(start, 1)))
            {
                start++;
            }

            int finish = src.Length - 1;

            // 4) Strip off an exponent and everything after.
            int exponent = src.ToLower().IndexOf("e");
            if (exponent >= 0)
            {
                exponent = exponent - 1;
            }
            if (exponent > 0)
            {
                finish = exponent - 1;
            }
            // 5) If a decimal point is not present, strip off trailing zeroes.
            if (decPt == -1)
            {
                while (finish > start && src.Substring(finish, 1) == "0")
                {
                    finish = finish - 1;
                }
            }

            // 5.5) Handle the special case of ".00" (with or without a trailing exponential).
            if (finish < start)
            {
                start = 0;
                while (start + 1 < src.Length && "+-0".Contains(src.Substring(start + 1, 1)))
                {
                    start = start + 1;
                }
                numSigFigs = 1;
            }
            else
            {

                //   6)  Count the number of characters left, ignoring any decimal point.
                //       start and finish are the first and last digits in the significant region.

                numSigFigs = finish - start + 1;

                if (decPt > start && decPt < finish)
                {
                    numSigFigs = numSigFigs - 1;
                }
            }

            // 7) Every number has at least one significant figure.
            if (numSigFigs < 1)
            {
                numSigFigs = 1;
            }
            return numSigFigs;
        }

        private static double SetToleranceFromSigFig(double v, int numsigfig)
        {
            int exponente = (PowerOfMostSignificantFig(v) - numsigfig + 1);

            //clamp the power to -3 due to oracle precision issues with cscartridge.molweightcontains. Seems to be it has problem beyond the third decimal, as depicted by CSBR-117965
            if(exponente < -3)
                exponente = -3;

            return Math.Pow(10.0, exponente);
        }

        private static int PowerOfMostSignificantFig(double val)
        {
            return (int)Math.Floor(Math.Log10(Math.Abs(val)));
        }

        /// <summary>
        /// Clean name query to match the format of cleanfield in database.
        /// </summary>
        /// <param name="thesyns">Not cleaned thesyns.</param>
        /// <returns>Cleaned thesyns</returns>
        public static string CleanTheSyns(ref string thesyns)
        {
            int offset;
            string newsyns = string.Empty;
            string allnewsyns = string.Empty;

            /*
            if Left(thesyns, 1) = "=") {
                thesyns = Right(thesyns, Len(thesyns) - 1) ' get rid of "=" at the beginning
            }
            */

            thesyns = "(" + thesyns.ToLower();

            offset = 0;
            //Debug.Print thesyns
            while (offset < thesyns.Length)
            {
                //Debug.Print offset
                switch (SubString(thesyns, offset, 1))
                {
                    case ".":
                    case ")":
                    case "[":
                    case "]":
                    case "{":
                    case "}":
                    case "/":
                    case "_":
                    case ":":
                    case "\"\"":
                        offset++;
                        break;
                    case " ":
                        if (SubString(thesyns, offset, 10) == " potassium")
                        {
                            newsyns = "potassium" + newsyns;
                            offset = offset + 10;
                        }
                        else if (SubString(thesyns, offset, 7) == " sodium")
                        {
                            newsyns = "sobium" + newsyns;
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 5) == " and ")
                        {
                            newsyns = newsyns + "+";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 4) == " na+")
                        {
                            newsyns = "sobium" + newsyns;
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 3) == " k+")
                        {
                            newsyns = "potassium" + newsyns;
                            offset = offset + 3;
                        }
                        else
                        {
                            offset = offset + 1;
                        }
                        break;
                    case "`":
                        newsyns = newsyns + "'";
                        offset = offset + 1;
                        break;
                    case "&":
                        newsyns = newsyns + "+";
                        offset = offset + 1;
                        break;
                    // case "*":
                    //    newsyns = newsyns + "%";
                    //    offset = offset + 1;
                    case ",":
                        if (SubString(thesyns, offset, 3) == ",0'")
                        {
                            newsyns = newsyns + ",O'";
                            offset = offset + 3;
                        }
                        else
                        {
                            offset = offset + 1;
                        }
                        break;
                    case "-":
                        if (SubString(thesyns, offset, 3) == "-0-")
                        {
                            newsyns = newsyns + "-O-";
                            offset = offset + 3;
                        }
                        else
                        {
                            offset = offset + 1;
                        }
                        break;
                    case ";":
                        if (SubString(thesyns, offset, 2) == "; ")
                        {
                            if (allnewsyns != "")
                            {
                                allnewsyns = allnewsyns + "; ";
                            }
                            allnewsyns = allnewsyns + newsyns;
                            newsyns = "";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + ";";
                            offset = offset + 1;
                        }
                        break;
                    case "0":
                        if (SubString(thesyns, offset, 3) == "0,0")
                        {
                            newsyns = newsyns + "22";
                            offset = offset + 3;
                        }
                        else
                        {
                            newsyns = newsyns + "0";
                            offset = offset + 1;
                        }
                        break;
                    case "1":
                        if (SubString(thesyns, offset, 2) == "1+")
                        {
                            newsyns = newsyns + "i";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "1";
                            offset = offset + 1;
                        }
                        break;
                    case "2":
                        if (SubString(thesyns, offset, 4) == "2hcl")
                        {
                            newsyns = newsyns + "bihydrochlorid";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 2) == "2+")
                        {
                            newsyns = newsyns + "ii";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "2";
                            offset = offset + 1;
                        }
                        break;
                    case "3":
                        if (SubString(thesyns, offset, 2) == "3+")
                        {
                            newsyns = newsyns + "iii";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "3";
                            offset = offset + 1;
                        }
                        break;
                    case "4":
                        if (SubString(thesyns, offset, 2) == "4+")
                        {
                            newsyns = newsyns + "iv";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "4";
                            offset = offset + 1;
                        }
                        break;
                    case "5":
                        if (SubString(thesyns, offset, 2) == "5+")
                        {
                            newsyns = newsyns + "v";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "5";
                            offset = offset + 1;
                        }
                        break;
                    case "6":
                        if (SubString(thesyns, offset, 2) == "6+")
                        {
                            newsyns = newsyns + "vi";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "6";
                            offset = offset + 1;
                        }
                        break;
                    case "(":
                        if (SubString(thesyns, offset, 3) == "(-)")
                        {
                            newsyns = newsyns + "-";
                            offset = offset + 3;
                        }
                        else
                        {
                            offset = offset + 1;
                        }
                        break;
                    case "#":
                        newsyns = newsyns + "no";
                        offset = offset + 1;
                        break;
                    case "+":
                        if (SubString(thesyns, offset, 3) == "+/-" || SubString(thesyns, offset, 3) == "+,-")
                        {
                            newsyns = newsyns + "+-";
                            offset = offset + 3;
                        }
                        else if (SubString(thesyns, offset, 2) == "+-")
                        {
                            newsyns = newsyns + "+-";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "+";
                            offset = offset + 1;
                        }
                        break;
                    case "a":
                        if (SubString(thesyns, offset, 12) == "antimony tri")
                        {
                            newsyns = newsyns + "antimonyiii";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 11) == "antimonious")
                        {
                            newsyns = newsyns + "antimonyiii";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 10) == "antimonous" ||
                                SubString(thesyns, offset, 10) == "antimonius")
                        {
                            newsyns = newsyns + "antimonyiii";
                            offset = offset + 10;
                        }
                        else if (SubString(thesyns, offset, 9) == "aluminium")
                        {
                            newsyns = newsyns + "aluminum";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "anhydrous")
                        {
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "argentous")
                        {
                            newsyns = newsyns + "silveri";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "arsenious")
                        {
                            newsyns = newsyns + "arseniciii";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "anisidine")
                        {
                            newsyns = newsyns + "methoxybenzenamin";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 8) == "arsenous")
                        {
                            newsyns = newsyns + "arseniciii";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 7) == "aniline")
                        {
                            newsyns = newsyns + "benzenamin";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 6) == "ammine")
                        {
                            newsyns = newsyns + "amin";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 5) == "alpha")
                        {
                            newsyns = newsyns + "a";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 4) == "amyl")
                        {
                            newsyns = newsyns + "pent";
                            offset = offset + 2;
                        }
                        else if (SubString(thesyns, offset, 3) == "ate")
                        {
                            newsyns = FixIcAte(newsyns) + "a";
                            offset = offset + 1;
                        }
                        else
                        {
                            newsyns = newsyns + "a";
                            offset = offset + 1;
                        }
                        break;
                    case "b":
                        if (SubString(thesyns, offset, 12) == "benzeneamine")
                        {
                            newsyns = newsyns + "benzenamin";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 11) == "bismuth tri")
                        {
                            newsyns = newsyns + "bismuthiii";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 5) == "bromo")
                        {
                            newsyns = newsyns + "brom";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 4) == "beta")
                        {
                            newsyns = newsyns + "b";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 3) == "bis")
                        {
                            newsyns = newsyns + "bi";
                            offset = offset + 3;
                        }
                        else
                        {
                            newsyns = newsyns + "b";
                            offset = offset + 1;
                        }
                        break;
                    case "c":
                        if (SubString(thesyns, offset, 21) == "carbamodithioato-S,S'")
                        {
                            newsyns = newsyns + "bithiocarbamato";
                            offset = offset + 21;
                        }
                        else if (SubString(thesyns, offset, 9) == "columbium")
                        {
                            newsyns = newsyns + "niobium";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "cobaltous")
                        {
                            newsyns = newsyns + "cobaltii";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "carbamoyl")
                        {
                            newsyns = newsyns + "carbamyl";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 7) == "complex")
                        {
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 7) == "chromic")
                        {
                            newsyns = newsyns + "chromiumiii";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 7) == "caesium")
                        {
                            newsyns = newsyns + "cesium";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 7) == "cuprous")
                        {
                            newsyns = newsyns + "copperi";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 7) == "capryl ")
                        {
                            newsyns = newsyns + "octyl";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "chloro")
                        {
                            newsyns = newsyns + "chlor";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "capryl")
                        {
                            newsyns = newsyns + "octano";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "cresol")
                        {
                            newsyns = newsyns + "methylphenol";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "cupric")
                        {
                            newsyns = newsyns + "copperii";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "cerous")
                        {
                            newsyns = newsyns + "ceriumiii";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 5) == "choro")
                        {
                            newsyns = newsyns + "chlor";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 5) == "ceric")
                        {
                            if (SubString(thesyns, offset - 1, 1) == "y")
                            {
                                newsyns = newsyns + "cer";
                                offset = offset + 3;
                            }
                            else
                            {
                                newsyns = newsyns + "ceriumiv";
                                offset = offset + 5;
                            }
                        }
                        else
                        {
                            newsyns = newsyns + "c";
                            offset = offset + 1;
                        }
                        break;
                    case "d":
                        if (SubString(thesyns, offset, 13) == "dodecahydrate")
                        {
                            newsyns = newsyns + "12h2o";
                            offset = offset + 13;
                        }
                        else if (SubString(thesyns, offset, 11) == "decahydrate")
                        {
                            newsyns = newsyns + "10h2o";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 10) == "derivative")
                        {
                            offset = offset + 10;
                        }
                        else if (SubString(thesyns, offset, 9) == "dihydrate")
                        {
                            newsyns = newsyns + "2h2o";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 6) == "dextro")
                        {
                            newsyns = newsyns + "d";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 5) == "delta")
                        {
                            newsyns = newsyns + "d";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 3) == "des" && thesyns.Substring(offset) != "des")
                        {
                            newsyns = newsyns + "de";
                            offset = offset + 3;
                        }
                        else if (SubString(thesyns, offset, 2) == "di")
                        {
                            newsyns = newsyns + "bi";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "d";
                            offset = offset + 1;
                        }
                        break;
                    case "e":
                        if (SubString(thesyns, offset, 12) == "enneahydrate")
                        {
                            newsyns = newsyns + "9h2o";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 7) == "enathyl")
                        {
                            newsyns = newsyns + "heptyl";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 4) == "eico")
                        {
                            newsyns = newsyns + "ico";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 4) == "ehty")
                        {
                            newsyns = newsyns + "ethy";
                            offset = offset + 4;
                        }
                        if (thesyns.Length > 6)
                        {
                            if (SubString(thesyns, offset, 2) == "e")
                            {
                                if ("abcdefghijklmnopqrstuvwxyz".IndexOf(SubString(thesyns, offset - 1, 1)) > 0)
                                {
                                    offset = offset + 1;
                                }
                                else
                                {
                                    newsyns = newsyns + "e";
                                    offset = offset + 1;
                                }
                            }
                            else
                            {
                                if ("1234567890-()+[]}{':,./ *%".IndexOf(SubString(thesyns, offset + 1, 1)) > 0)
                                {
                                    if (offset == 1)
                                    {
                                        newsyns = newsyns + "e";
                                        offset = offset + 1;
                                    }
                                    else
                                    {
                                        if ("abcdefghijklmnopqrstuvwxyz".IndexOf(SubString(thesyns, offset - 1, 1)) > 0)
                                        {
                                            offset = offset + 1;
                                        }
                                        else
                                        {
                                            newsyns = newsyns + "e";
                                            offset = offset + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    newsyns = newsyns + "e";
                                    offset = offset + 1;
                                }
                            }
                        }
                        else
                        {
                            newsyns = newsyns + "e";
                            offset = offset + 1;
                        }
                        break;
                    case "f":
                        if (SubString(thesyns, offset, 7) == "ferrous")
                        {
                            newsyns = newsyns + "ironii";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 6) == "ferric")
                        {
                            newsyns = newsyns + "ironiii";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "fluoro")
                        {
                            newsyns = newsyns + "fluor";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 6) == "flouro")
                        {
                            newsyns = newsyns + "fluor";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 5) == "flour")
                        {
                            newsyns = newsyns + "fluor";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "f";
                            offset = offset + 1;
                        }
                        break;
                    case "g":
                        if (SubString(thesyns, offset, 5) == "gamma")
                        {
                            newsyns = newsyns + "y";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "g";
                            offset = offset + 1;
                        }
                        break;
                    case "h":
                        if (SubString(thesyns, offset, 16) == "hemipentahydrate")
                        {
                            newsyns = newsyns + "2.5h2o";
                            offset = offset + 16;
                        }
                        else if (SubString(thesyns, offset, 12) == "heptahydrate")
                        {
                            newsyns = newsyns + "7h2o";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 11) == "hemihydrate")
                        {
                            newsyns = newsyns + "1/2h2o";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 11) == "hexahydrate")
                        {
                            newsyns = newsyns + "6h2o";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 9) == "hexadecyl")
                        {
                            newsyns = newsyns + "cetyl";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 7) == "hydrate")
                        {
                            newsyns = newsyns + "h2o";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 4) == "hyro")
                        {
                            newsyns = newsyns + "hydro";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 3) == "hcl")
                        {
                            newsyns = newsyns + "hydrochlorid";
                            offset = offset + 3;
                        }
                        else if (SubString(thesyns, offset, 3) == "hbr")
                        {
                            newsyns = newsyns + "hydrobromid";
                            offset = offset + 3;
                        }
                        else
                        {
                            newsyns = newsyns + "h";
                            offset = offset + 1;
                        }
                        break;
                    case "i":
                        if (SubString(thesyns, offset, 7) == "ic acid")
                        {
                            if (newsyns.Contains("yl"))
                            {
                                newsyns = newsyns + "icacid";
                                offset = offset + 7;
                            }
                            else
                            {
                                newsyns = FixIcAte(newsyns) + "at";
                                offset = offset + 7;
                            }
                        }
                        else if (SubString(thesyns, offset, 6) == "icacid")
                        {
                            newsyns = FixIcAte(newsyns) + "at";
                            offset = offset + 6;
                        }
                        else
                        {
                            newsyns = newsyns + "i";
                            offset = offset + 1;
                        }
                        break;
                    case "l":
                        if (SubString(thesyns, offset, 4) == "levo")
                        {
                            newsyns = newsyns + "l";
                            offset = offset + 4;
                        }
                        else
                        {
                            newsyns = newsyns + "l";
                            offset = offset + 1;
                        }
                        break;
                    case "m":
                        if (SubString(thesyns, offset, 9) == "mercaptan")
                        {
                            newsyns = newsyns + "thiol";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "manganous")
                        {
                            newsyns = newsyns + "manganesii";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 9) == "mercurous")
                        {
                            newsyns = newsyns + "mercuryi";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 8) == "mercuric")
                        {
                            newsyns = newsyns + "mercuryii";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 8) == "methyoxy")
                        {
                            newsyns = newsyns + "methoxy";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 5) == "methy" && !(SubString(thesyns, offset, 6) == "methyl"))
                        {
                            newsyns = newsyns + "methyl";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 4) == "meta")
                        {
                            newsyns = newsyns + "3";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset + 1, 1).Length *
                      "- ,'".IndexOf(SubString(thesyns, offset + 1, 1)) *
                      "- ,'([{".IndexOf(SubString(thesyns, offset - 1, 1)) > 0)
                        {
                            newsyns = newsyns + "3";
                            offset = offset + 1;
                        }
                        else
                        {
                            newsyns = newsyns + "m";
                            offset = offset + 1;
                        }
                        break;
                    case "n":
                        if (SubString(thesyns, offset, 11) == "nonahydrate")
                        {
                            newsyns = newsyns + "9h2o";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 9) == "nickelous")
                        {
                            newsyns = newsyns + "nickelii";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 5) == "napth")
                        {
                            newsyns = newsyns + "naphth";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "n";
                            offset = offset + 1;
                        }
                        break;
                    case "o":
                        if (SubString(thesyns, offset, 15) == "octadecahydrate")
                        {
                            newsyns = newsyns + "18h2o";
                            offset = offset + 15;
                        }
                        else if (SubString(thesyns, offset, 11) == "octahydrate")
                        {
                            newsyns = newsyns + "8h2o";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 8) == "ous acid")
                        {
                            if (newsyns.Contains("yl"))
                            {
                                newsyns = newsyns + "ousacid";
                                offset = offset + 8;
                            }
                            else
                            {
                                newsyns = newsyns + "it";
                                offset = offset + 8;
                            }
                        }
                        else if (SubString(thesyns, offset, 5) == "ortho")
                        {
                            newsyns = newsyns + "2";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 5) == "omega")
                        {
                            newsyns = newsyns + "w";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 3) == "oyl")
                        {
                            newsyns = FixIcAte(newsyns) + "o";
                            if (newsyns.PadRight(1) != "o")
                            {
                                newsyns = newsyns + "o";
                            }
                            offset = offset + 1;
                        }
                        if (SubString(thesyns, offset + 1, 1).Length * "- ,'".IndexOf(SubString(thesyns, offset + 1, 1)) * "- ,'([{".IndexOf(SubString(thesyns, offset - 1, 1)) > 0)
                        {
                            newsyns = newsyns + "2";
                            offset = offset + 1;
                        }
                        else
                        {
                            newsyns = newsyns + "o";
                            offset = offset + 1;
                        }
                        break;
                    case "p":
                        if (SubString(thesyns, offset, 16) == "phosphorothioate")
                        {
                            newsyns = newsyns + "thiophosphat";
                            offset = offset + 16;
                        }
                        else if (SubString(thesyns, offset, 12) == "pentahydrate")
                        {
                            newsyns = newsyns + "5h2o";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 11) == "phosphorous")
                        {
                            newsyns = newsyns + "phosphorus";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 8) == "plumbous")
                        {
                            newsyns = newsyns + "leadi";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 6) == "pyrine")
                        {
                            newsyns = newsyns + "pyren";
                            offset = offset + 6;
                        }
                        else if (SubString(thesyns, offset, 4) == "para")
                        {
                            newsyns = newsyns + "4";
                            offset = offset + 4;
                        }
                        if ((SubString(thesyns, offset, 3) == "pht") && !(SubString(thesyns, offset, 4) == "phth"))
                        {
                            newsyns = newsyns + "phth";
                            offset = offset + 3;
                        }
                        if (SubString(thesyns, offset + 1, 1).Length * ("- ,'".IndexOf(SubString(thesyns, offset + 1, 1)) * "- ,'([{".IndexOf(SubString(thesyns, offset - 1, 1))) != 0)
                        {
                            newsyns = newsyns + "4";
                            offset = offset + 1;
                        }
                        else
                        {
                            newsyns = newsyns + "p";
                            offset = offset + 1;
                        }
                        break;
                    case "s":
                        if (SubString(thesyns, offset, 13) == "sesquihydrate")
                        {
                            newsyns = newsyns + "3/2h2o";
                            offset = offset + 13;
                        }
                        else if (SubString(thesyns, offset, 8) == "salicycl")
                        {
                            newsyns = newsyns + "salicyl";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 8) == "stannane")
                        {
                            newsyns = newsyns + "tin";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 8) == "stannous")
                        {
                            newsyns = newsyns + "tinii";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 7) == "stannic")
                        {
                            newsyns = newsyns + "tiniv";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 5) == "sulph")
                        {
                            newsyns = newsyns + "sulf";
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 5) == "salts")
                        {
                            offset = offset + 5;
                        }
                        else if (SubString(thesyns, offset, 4) == "sec-")
                        {
                            newsyns = newsyns + "s";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 4) == "salt")
                        {
                            offset = offset + 4;
                        }
                        else
                        {
                            newsyns = newsyns + "s";
                            offset = offset + 1;
                        }
                        break;
                    case "t":
                        if (SubString(thesyns, offset, 12) == "tetrahydrate")
                        {
                            newsyns = newsyns + "4h2o";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 10) == "trihydrate")
                        {
                            newsyns = newsyns + "3h2o";
                            offset = offset + 10;
                        }
                        else if (SubString(thesyns, offset, 9) == "toluidine")
                        {
                            newsyns = newsyns + "methylbenzenamin";
                            offset = offset + 9;
                        }
                        else if (SubString(thesyns, offset, 8) == "thallous")
                        {
                            newsyns = newsyns + "thalliumi";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 8) == "terthiop")
                        {
                            newsyns = newsyns + "terthiop";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 7) == "thallic")
                        {
                            newsyns = newsyns + "thalliumiii";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 4) == "tert")
                        {
                            newsyns = newsyns + "t";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 4) == "tris")
                        {
                            newsyns = newsyns + "tri";
                            offset = offset + 4;
                        }
                        else
                        {
                            newsyns = newsyns + "t";
                            offset = offset + 1;
                        }
                        break;
                    case "w":
                        if (SubString(thesyns, offset, 5) == "water")
                        {
                            newsyns = newsyns + "h2o";
                            offset = offset + 6;
                        }
                        else
                        {
                            newsyns = newsyns + "w";
                            offset = offset + 1;
                        }
                        break;
                    case "y":
                        if (SubString(thesyns, offset, 12) == "yl mercaptan")
                        {
                            newsyns = newsyns + "anethiol";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 11) == "ylmercaptan")
                        {
                            newsyns = newsyns + "anethiol";
                            offset = offset + 11;
                        }
                        else if (SubString(thesyns, offset, 10) == "yl alcohol")
                        {
                            newsyns = newsyns + "anol";
                            offset = offset + 10;
                        }
                        else if (SubString(thesyns, offset, 9) == "ylalcohol")
                        {
                            newsyns = newsyns + "anol";
                            offset = offset + 9;
                        }
                        else
                        {
                            newsyns = newsyns + "y";
                            offset = offset + 1;
                        }
                        break;
                    default:
                        newsyns = newsyns + SubString(thesyns, offset, 1);
                        offset = offset + 1;
                        break;
                }
            }

            if (allnewsyns != string.Empty)
            {
                allnewsyns = allnewsyns + "; ";
            }
            newsyns = allnewsyns + newsyns;

            //Debug.Print newsyns

            thesyns = SubString(thesyns, 2, thesyns.Length - 1 - 2);
            //Coverity Bug Fix :- CID : 19054  Jira Id :CBOE-194
            if (!string.IsNullOrEmpty(newsyns) && newsyns != "%" && newsyns != "%%")
            {
                return CleanTheSynsPart2(ref newsyns);
            }
            else
            {
                return thesyns.Replace('*', '%');
            }
            //Debug.Print newsyns
        }

        /// <summary>
        /// Clean name query to match the format of cleanfield in database.
        /// Called by CleanTheSyns.
        /// </summary>
        /// <param name="thesyns">Partially cleaned thesyns.</param>
        /// <returns>More cleaned thesyns.</returns>
        private static string CleanTheSynsPart2(ref string thesyns)
        {
            int offset;
            string newsyns = string.Empty;

            thesyns = "(" + thesyns;

            offset = 0;
            //Debug.Print thesyns
            while (offset < thesyns.Length)
            {
                switch (SubString(thesyns, offset, 1))
                {
                    case "(":
                        offset = offset + 1;
                        break;
                    case "1":
                        if (SubString(thesyns, offset, 15) == "11dimethylethyl")
                        {
                            newsyns = newsyns + "tbutyl";
                            offset = offset + 15;
                        }
                        else if (SubString(thesyns, offset, 13) == "1methylpropyl")
                        {
                            newsyns = newsyns + "sbutyl";
                            offset = offset + 13;
                        }
                        else if (SubString(thesyns, offset, 12) == "1methylethyl")
                        {
                            newsyns = newsyns + "isopropyl";
                            offset = offset + 12;
                        }
                        else if (SubString(thesyns, offset, 11) == "135triazine")
                        {
                            newsyns = newsyns + "striazin";
                            offset = offset + 11;
                        }
                        else
                        {
                            newsyns = newsyns + "1";
                            offset = offset + 1;
                        }
                        break;
                    case "a":
                        if (SubString(thesyns, offset, 7) == "arachid")
                        {
                            newsyns = newsyns + "eicosan";
                            offset = offset + 7;
                        }
                        else
                        {
                            newsyns = newsyns + "a";
                            offset = offset + 1;
                        }
                        break;
                    case "b":
                        if (SubString(thesyns, offset, 5) == "butyr")
                        {
                            newsyns = newsyns + "butan";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "b";
                            offset = offset + 1;
                        }
                        break;
                    case "c":
                        if (SubString(thesyns, offset, 5) == "capro")
                        {
                            newsyns = newsyns + "hexan";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "c";
                            offset = offset + 1;
                        }
                        break;
                    case "d":
                        if (SubString(thesyns, offset, 2) == "d+")
                        {
                            newsyns = newsyns + "d";
                            offset = offset + 2;
                        }
                        else if (SubString(thesyns, offset, 2) == "d-")
                        {
                            newsyns = newsyns + "d";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "d";
                            offset = offset + 1;
                        }
                        break;
                    case "e":
                        if (SubString(thesyns, offset, 32) == "ethylenedinitrilotetraaceticacid")
                        {
                            newsyns = newsyns + "edta";
                            offset = offset + 32;
                        }
                        else if (SubString(thesyns, offset, 30) == "ethylenediaminetetraaceticacid")
                        {
                            newsyns = newsyns + "edta";
                            offset = offset + 30;
                        }
                        else if (SubString(thesyns, offset, 27) == "ethylenediaminetetraacetate")
                        {
                            newsyns = newsyns + "edta";
                            offset = offset + 27;
                        }
                        else if (SubString(thesyns, offset, 7) == "edetate")
                        {
                            newsyns = newsyns + "edta";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 5) == "enath")
                        {
                            newsyns = newsyns + "heptan";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "e";
                            offset = offset + 1;
                        }
                        break;
                    case "l":
                        if (SubString(thesyns, offset, 4) == "laur")
                        {
                            newsyns = newsyns + "dodecan";
                            offset = offset + 4;
                        }
                        else if (SubString(thesyns, offset, 2) == "l+")
                        {
                            newsyns = newsyns + "l";
                            offset = offset + 2;
                        }
                        else if (SubString(thesyns, offset, 2) == "l-")
                        {
                            newsyns = newsyns + "l";
                            offset = offset + 2;
                        }
                        else
                        {
                            newsyns = newsyns + "l";
                            offset = offset + 1;
                        }
                        break;
                    case "m":
                        if (SubString(thesyns, offset, 6) == "myrist")
                        {
                            newsyns = newsyns + "tetradecan";
                            offset = offset + 6;
                        }
                        else
                        {
                            newsyns = newsyns + "m";
                            offset = offset + 1;
                        }
                        break;
                    case "n":
                        if (SubString(thesyns, offset, 12) == "naphthalenyl")
                        {
                            newsyns = newsyns + "naphthyl";
                            offset = offset + 6;
                        }
                        else
                        {
                            newsyns = newsyns + "n";
                            offset = offset + 1;
                        }
                        break;
                    case "p":
                        if (SubString(thesyns, offset, 8) == "pelargon")
                        {
                            newsyns = newsyns + "nonan";
                            offset = offset + 8;
                        }
                        else if (SubString(thesyns, offset, 7) == "propion")
                        {
                            newsyns = newsyns + "propan";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 6) == "palmit")
                        {
                            newsyns = newsyns + "hexadecan";
                            offset = offset + 6;
                        }
                        else
                        {
                            newsyns = newsyns + "p";
                            offset = offset + 1;
                        }
                        break;
                    case "s":
                        if (SubString(thesyns, offset, 7) == "silveri")
                        {
                            newsyns = newsyns + "silver";
                            offset = offset + 7;
                        }
                        else if (SubString(thesyns, offset, 5) == "stear")
                        {
                            newsyns = newsyns + "octadecan";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "s";
                            offset = offset + 1;
                        }
                        break;
                    case "v":
                        if (SubString(thesyns, offset, 5) == "valer")
                        {
                            newsyns = newsyns + "pentan";
                            offset = offset + 5;
                        }
                        else
                        {
                            newsyns = newsyns + "v";
                            offset = offset + 1;
                        }
                        break;
                    default:
                        newsyns = newsyns + SubString(thesyns, offset, 1);
                        offset = offset + 1;
                        break;
                }
            }

            thesyns = thesyns.Substring(2, thesyns.Length - 1 - 2);
            if (newsyns != string.Empty)
            {
                return newsyns;
            }
            else
            {
                return thesyns;
            }
        }

        /// <summary>
        /// Part of the chemical name normalization routine.
        /// Used primarily for acids, esters, and acid halides.
        /// Called by CleanTheSyns.
        /// </summary>
        /// <param name="thesyns">thesyns.</param>
        /// <returns>More cleaned thesyns.</returns>
        private static string FixIcAte(string thesyns)
        {
            if (thesyns.EndsWith("pelargon"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 8) + "nonano";
            }
            else if (thesyns.EndsWith("propion"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 7) + "propano";
            }
            else if (thesyns.EndsWith("arachid"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 7) + "eicosano";
            }
            else if (thesyns.EndsWith("methano"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 7) + "form";
            }
            else if (thesyns.EndsWith("myrist"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 6) + "tetradecano";
            }
            else if (thesyns.EndsWith("ethano"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 6) + "acet";
            }
            else if (thesyns.EndsWith("palmit"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 6) + "hexadecano";
            }
            else if (thesyns.EndsWith("stear"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 5) + "octadecano";
            }
            else if (thesyns.EndsWith("valer"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 5) + "pentano";
            }
            else if (thesyns.EndsWith("butyr"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 5) + "butano";
            }
            else if (thesyns.EndsWith("capro"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 5) + "hexano";
            }
            else if (thesyns.EndsWith("enath"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 5) + "heptano";
            }
            else if (thesyns.EndsWith("laur"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 4) + "dodecano";
            }
            else if (thesyns.EndsWith("capr"))
            {
                thesyns = thesyns.Substring(0, thesyns.Length - 4) + "decano";
            }

            return thesyns;
        }

        /// <summary>
        /// This function is just to avoid exceptions when trying to get a substring that exceeds the string length
        /// </summary>
        /// <param name="original">The original string.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length</param>
        /// <returns>If it is possible the substring, otherwise the original string.</returns>
        public static string SubString(string original, int offset, int length)
        {
            if (offset >= original.Length)
                return original;

            if (offset + length > original.Length)
                length = original.Length - offset;

            return original.Substring(offset, length);
        }

        /// <summary>
        /// Trims value taking into account whether it's text or number
        /// </summary>
        /// <param name="value">The value to trim.</param>
        /// <param name="type">Its DbType.</param>
        /// <param name="trimPosition">The position to trim.</param>
        /// <returns>A trimmed string.</returns>
        public static string TrimValue(string value, DbType type, SearchCriteria.Positions trimPosition)
        {
            char[] charactersToTrim = null;
            switch (TypesConversor.GetAbstractType(type))
            {
                case COEDataView.AbstractTypes.Text:
                    charactersToTrim = new char[4] { ' ', '\n', '\r', '\t' };
                    switch (trimPosition)
                    {
                        case SearchCriteria.Positions.Left:
                            return value.TrimStart(charactersToTrim);
                        case SearchCriteria.Positions.Right:
                            return value.TrimEnd(charactersToTrim);
                        case SearchCriteria.Positions.Both:
                            return value.Trim(charactersToTrim);
                    }
                    break;
                case COEDataView.AbstractTypes.Real:
                case COEDataView.AbstractTypes.Integer:
                    if (value.Trim() != "")
                    {
                        
                        string[] parts = value.Split(_decimalSeparator.ToCharArray());

                        switch (trimPosition)
                        {
                            case SearchCriteria.Positions.Left:
                                parts[0] = trimNumberLeft(parts[0]);
                                break;
                            case SearchCriteria.Positions.Right:
                                if (parts.Length >= 2)
                                    parts[1] = trimNumberRight(parts[1]);
                                break;
                            case SearchCriteria.Positions.Both:
                                parts[0] = trimNumberLeft(parts[0]);
                                if (parts.Length >= 2)
                                    parts[1] = trimNumberRight(parts[1]);
                                break;
                        }
                        return parts.Length >= 2 ? parts[0] + _decimalSeparator + parts[1] : parts[0];
                    }
                    break;
            }
            return value;
        }
        private static string trimNumberLeft(string number)
        {
            string result = number.TrimStart(new char[5] { ' ', '0', '\n', '\r', '\t' });
            result = result.TrimEnd(new char[4] { ' ', '\n', '\r', '\t' });
            if (result == "")
                result = "0";

            return result;
        }
        private static string trimNumberRight(string number)
        {
            string result = number.TrimEnd(new char[5] { ' ', '0', '\n', '\r', '\t' });
            if (result == "")
                result = "0";

            return result;
        }

        public static string[] ParseRange(string value)
        {
            value = value.Replace(" ", "");
            if (value.IndexOf("--") > 0)
                value = value.Replace("--", "/-");
            else
            {
                if (value.StartsWith("-"))
                {
                    value = value.Trim('-').Replace('-', '/');
                    value = '-' + value;
                }
                else
                    value = value.Trim('-').Replace('-', '/');
            }
            return value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
