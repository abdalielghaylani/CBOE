using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CambridgeSoft.COEDataLoader.Core
{
    /// <summary>
    /// Instances of this class hold a string/Type pair. If the Type is not known
    /// at time of instantiation, it is loosely evaluated and assigned in a
    /// culture-dependent fashion.
    /// </summary>
    public class DataPair
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawValue">the raw string value to evaluate and store</param>
        public DataPair(string rawValue)
        {
            if (!string.IsNullOrEmpty(rawValue))
            {
                _rawValue = rawValue;
                _dataType = DataPair.GuessType(rawValue, CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Constructor with culture-specificity.
        /// </summary>
        /// <param name="rawValue">the raw string value to evaluate and store</param>
        /// <param name="culture">
        /// the System.Globalization.CultureInfo to use when evaluating the data-type
        /// of the rawValue parameter value.
        /// </param>
        public DataPair(string rawValue, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(rawValue))
            {
                _rawValue = rawValue;
                _dataType = DataPair.GuessType(rawValue, culture);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawValue">the raw string value to evaluate and store</param>
        /// <param name="knownType">the System.Type to assign to the rawValue parameter value</param>
        public DataPair(object rawValue, Type knownType)
        {
            _rawValue = rawValue;
            _dataType = knownType;
        }

        private object _rawValue;
        /// <summary>
        /// The value assigned by the constructor whose Type is to be evaulated.
        /// </summary>
        public object RawValue
        {
            get { return _rawValue; }
            set { _rawValue = value; }
        }

        private Type _dataType;
        /// <summary>
        /// The evaluated Type of the RawValue property value.
        /// </summary>
        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        #region > Static members <

        /// <summary>
        /// 
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

        #endregion

    }
}
