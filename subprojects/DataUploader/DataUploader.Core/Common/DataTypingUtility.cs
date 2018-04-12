using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Utility class for determining the potential Type of a string. The Type is
    /// loosely evaluated and assigned in a culture-dependent fashion.
    /// </summary>
    public static class DataTypingUtility
    {
        /// <summary>
        /// Overloaded evaluation of the possible alternatives for a 'raw' string value.
        /// </summary>
        /// <param name="rawValue">the string value to evaluate</param>
        /// <returns>the evaluated Type</returns>
        public static Type EvaluateType(string rawValue)
        {
            //Type dataType = DataTypingUtility.GuessType(rawValue, CultureInfo.CurrentCulture);
            Type dataType = DataTypingUtility.GuessType(rawValue, CultureInfo.InvariantCulture);
            return dataType;
        }

        /// <summary>
        /// Overloaded evaluation of the possible alternatives for a 'raw' string value;
        /// takes into account a specific culture setting for the purposes of properly
        /// interpreting number formats (decimals, thousands separators, etc) as well as
        /// date formats.
        /// </summary>
        /// <param name="rawValue">the string value to evaluate</param>
        /// <param name="culture">the culture to apply when performint type-evaluation</param>
        /// <returns>the evaluated Type</returns>
        public static Type EvaluateType(string rawValue, CultureInfo culture)
        {
            Type dataType = DataTypingUtility.GuessType(rawValue, culture);
            return dataType;
        }

        /// <summary>
        /// Overloaded evaluation of the possible alternatives for a 'raw' string value;
        /// special-case scenario where the data-type has pre-determined (such as from
        /// the schema from an MS Access table). The type is 
        /// </summary>
        /// <param name="rawValue">the string value to evaluate</param>
        /// <param name="knownType">the DbType derived from the caller</param>
        /// <returns>the evaluated Type</returns>
        public static Type EvaluateType(string rawValue, System.Data.DbType knownType)
        {
            Type evaluatedType = null;

            switch (knownType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Binary:
                case DbType.String:
                case DbType.Xml:
                    {
                        evaluatedType = typeof(String);
                        break;
                    }
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.Time:
                    {
                        evaluatedType = typeof(DateTime);
                        break;
                    }
                case DbType.Double:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Single:
                    {
                        evaluatedType = typeof(Int32);
                        break;
                    }
            }

            // provide a fall-through mechanism when the type remains matched
            if (!string.IsNullOrEmpty(rawValue) && evaluatedType == null)
                evaluatedType = EvaluateType(rawValue);

            return evaluatedType;
        }

        /// <summary>
        /// The worker method for some of the EvaluateType overloads. This method calls
        /// the low-level type-converters as tests of infividual data-types.
        /// </summary>
        /// <param name="rawValue">the raw string value to evaluate and store</param>
        /// <param name="culture">
        /// the System.Globalization.CultureInfo to use when evaluating the data-type
        /// </param>
        /// <returns>the possible System.Type of the given raw string value</returns>
        public static Type GuessType(string rawValue, CultureInfo culture)
        {
            if (IsInteger(rawValue, culture))
                return typeof(int);
            else if (IsDouble(rawValue, culture))
                return typeof(double);
            else if (IsDate(rawValue, culture))
                return typeof(DateTime);
            else if (IsBool(rawValue, culture))
                return typeof(bool);
            else if (!string.IsNullOrEmpty(rawValue))
                return typeof(string);
            else
                return null;
        }

        /// <summary>
        /// Determines the convertibility into the specified Type.
        /// </summary>
        /// <param name="s">the raw string value to evaluate and store</param>
        /// <param name="culture">
        /// the System.Globalization.CultureInfo to use when evaluating the data-type
        /// </param>
        /// <returns>true if <paramref name="s"/> can be converted into an int</returns>
        public static bool IsInteger(string s, CultureInfo culture)
        {
            bool allowSign = false;
            if (s.StartsWith("-") || s.StartsWith("+"))
                allowSign = true;
            foreach (char c in s)
                if (!Char.IsDigit(c))
                    if (!allowSign)
                        return false;
            return true;
        }

        /// <summary>
        /// Determines the convertibility into the specified Type.
        /// </summary>
        /// <param name="s">the raw string value to evaluate and store</param>
        /// <param name="culture">
        /// the System.Globalization.CultureInfo to use when evaluating the data-type
        /// </param>
        /// <returns>true if <paramref name="s"/> can be converted into a double</returns>
        public static bool IsDouble(string s, CultureInfo culture)
        {
            if (!s.Contains(culture.NumberFormat.NumberDecimalSeparator))
                return false;
            char separator = Convert.ToChar(culture.NumberFormat.NumberDecimalSeparator);

            bool allowSign = false;
            if (s.StartsWith("-") || s.StartsWith("+"))
                allowSign = true;
            foreach (char c in s)
            {
                if (!(c == separator || Char.IsDigit(c)))
                    if (!allowSign)
                        return false;
            }
            return true;
        }

        /// <summary>
        /// Determines the convertibility into the specified Type.
        /// </summary>
        /// <param name="s">the raw string value to evaluate and store</param>
        /// <param name="culture">
        /// the System.Globalization.CultureInfo to use when evaluating the data-type
        /// </param>
        /// <returns>true if <paramref name="s"/> can be converted into a DateTime</returns>
        public static bool IsDate(string s, CultureInfo culture)
        {
            DateTime dt;
            if (DateTime.TryParse(s, culture, DateTimeStyles.AdjustToUniversal, out dt))
                return true;
            return false;
        }

        /// <summary>
        /// Determines the convertibility into the specified Type.
        /// </summary>
        /// <param name="s">the raw string value to evaluate and store</param>
        /// <param name="culture">
        /// the System.Globalization.CultureInfo to use when evaluating the data-type
        /// </param>
        /// <returns>true if <paramref name="s"/> can be converted into bool</returns>
        public static bool IsBool(string s, CultureInfo culture)
        {
            bool isValid = false;
            switch (s.ToLower())
            {
                case "true":
                case "false":
                case "1":
                case "0":
                    isValid = true;
                    break;
            }
            return isValid;
        }

        /// <summary>
        /// Converts a string to a specific type, and returns it as an object.
        /// </summary>
        /// <remarks>
        /// Intended to be used for argument conversion when performing method invocations
        /// via reflection.
        /// </remarks>
        /// <param name="s">the string to convert</param>
        /// <param name="t">the intended Type name</param>
        /// <returns>an object representation of the converted string</returns>
        public static object ConvertToType(string s, string t)
        {
            Type targetType = typeof(string);
            switch (t.ToUpper())
            {
                case "INT": targetType = typeof(int); break;
                case "DOUBLE": targetType = typeof(double); break;
                case "FLOAT": targetType = typeof(float); break;
                case "DATE": targetType = typeof(DateTime); break;
                default: break;
            }
            return ConvertToType(s, targetType);
        }

        /// <summary>
        /// Converts a string to a specific type, and returns it as an object.
        /// </summary>
        /// <remarks>
        /// Intended to be used for argument conversion when performing method invocations
        /// via reflection.
        /// </remarks>
        /// <param name="s">the string to convert</param>
        /// <param name="t">the intended Type</param>
        /// <returns>an object representation of the converted string</returns>
        public static object ConvertToType(string s, Type t)
        {
            object o = null;
            bool flag = false;

            if (t.Equals(typeof(int)))
            {
                int i;
                flag = int.TryParse(s, out i);
                o = i;
            }
            else if (t.Equals(typeof(double)))
            {
                double d;
                flag = double.TryParse(s, out d);
                o = d;
            }
            else if (t.Equals(typeof(float)))
            {
                float f;
                flag = float.TryParse(s, out f);
                o = f;
            }
            else if (t.Equals(typeof(DateTime)))
            {
                DateTime time;
                flag = DateTime.TryParse(s, out time);
                o = time;
            }
            //for current implementation, all kinds of return type is box into an object type, so when
            //outside use this method's return value, the value must be casted to a specific type. So, I
            //enumerate all cases of types, do not mix the type parsing process.
            //else
            //{
            //    o = s.ToString();
            //    flag = true;
            //}
            else if (t.Equals(typeof(string)))
            {
                o = s;
                flag = true;
            }

            if (flag == true)
                return o;
            else
            {
                //ToDo: We will add new handling process in the future.
                throw new Exception("There are some error occured when parsing the value of the record~!");
            }

            
        }
    }
}
