//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Globalization;
//using Csla.Validation;
//using CambridgeSoft.COE.Registration.Services.Common;

//namespace CambridgeSoft.COE.Registration.Validation
//{
//    /// <summary>
//    /// Csla rule argument used for validating that a string is a valid date.
//    /// </summary>
//    public class IsDateFormatRuleArgs : RuleArgs
//    {
//        private string _dateFormat = Constants.DATE_FORMAT;
//        /// <summary>
//        /// the date format to apply to the string for date validation
//        /// </summary>
//        public string DateFormat
//        {
//            get { return _dateFormat; }
//        }

//        private CultureInfo _culture = new CultureInfo("en-US");
//        public CultureInfo Culture
//        {
//            get { return _culture; }
//        }

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="propertyName">the name of the target object's property to validate</param>
//        /// <param name="dateFormat">the expected format of the date string</param>
//        /// <param name="culture">the culture information used to validate the format</param>
//        public IsDateFormatRuleArgs(string propertyName, string dateFormat, CultureInfo culture)
//            : base(propertyName)
//        {
//            if (!string.IsNullOrEmpty(dateFormat))
//                _dateFormat = dateFormat;
//            if (culture != null)
//                _culture = culture;
//        }
//    }

//}
