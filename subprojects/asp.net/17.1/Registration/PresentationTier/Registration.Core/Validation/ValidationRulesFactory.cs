using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

using Csla;
using Csla.Validation;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Validation
{
    /// <summary>
    /// Generates validation rules from an XML configuration
    /// </summary>
    public class ValidationRulesFactory
    {
        #region Singleton Pattern
        private static ValidationRulesFactory _instance = null;

        public static ValidationRulesFactory GetInstance()
        {
            if (_instance == null)
                _instance = new ValidationRulesFactory();

            return _instance;
        }

        private ValidationRulesFactory()
        {
        }
        #endregion

        [COEUserActionDescription("AddValidationRulesForProperty")]
        public void AddInstanceRules(ValidationRules validationRules, string propertyName, string xml)
        {
            try
            {
                XPathDocument xDocument = new XPathDocument(new StringReader(xml));
                XPathNavigator xNavigator = xDocument.CreateNavigator();
                XPathNodeIterator xIterator = xNavigator.Select("validationRuleList/validationRule");
                if (xIterator.MoveNext())
                {
                    do
                    {
                        string ruleName = xIterator.Current.GetAttribute("validationRuleName", xIterator.Current.NamespaceURI);

                        if (!string.IsNullOrEmpty(ruleName))
                        {
                            string message = xIterator.Current.GetAttribute("errorMessage", xIterator.Current.NamespaceURI);

                            switch (ruleName)
                            {
                                case "positiveInteger":
                                    validationRules.AddInstanceRule(PositiveInteger, new PositiveIntegerRuleArgs(propertyName, message));
                                    break;
                                case "regularExpressionMatch":
                                    string regExp = xIterator.Current.SelectSingleNode("params/param[@name=\"regExp\"]").GetAttribute("value", xIterator.Current.NamespaceURI);
                                    validationRules.AddInstanceRule(RegExpMatch, new RegExpMatchRuleArgs(propertyName, regExp, message));
                                    break;
                                case "textLength":
                                    int max = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"max\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
                                    int min = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"min\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
                                    validationRules.AddInstanceRule(StringRange, new StringRangeRuleArgs(propertyName, min, max, message));

                                    break;
                                case "float":
                                    string regExpression = string.Empty;
                                    int intPrecison = 0;
                                    int decPrecison = 0;
                                    int startRange = 0;
                                    string intPartRegExp = string.Empty;
                                    string decPartRegExp = string.Empty;

                                    intPrecison = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"integerPart\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
                                    decPrecison = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"decimalPart\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
                                    intPartRegExp = string.Format(@"\d{{{0},{1}}}", startRange, intPrecison);
                                    decPartRegExp = (decPrecison == 0) ? "" : string.Format(@"([.]\d{{{0},{1}}})?", startRange, decPrecison);
                                    regExpression = string.Format(@"^{0}{1}$", intPartRegExp, decPartRegExp);
                                    validationRules.AddInstanceRule(RegExpMatch, new RegExpMatchRuleArgs(propertyName, regExpression, message));

                                    break;
                                case "numericRange":
                                    int minRange = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"min\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
                                    int maxRange = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"max\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
                                   validationRules.AddInstanceRule(NumericRange, new NumericRangeRuleArgs(propertyName, minRange, maxRange, message));
                                    break;
                                case "requiredField":
                                    validationRules.AddInstanceRule(StringRequired, new StringRequiredRuleArgs(propertyName, message));
                                    break;
                            }
                        }
                    } while (xIterator.Current.MoveToNext());
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        #region Custom Rule Methods

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool CoRequiredValues(object target, RuleArgs e)
        {
            CoRequiredRuleArgs args = (CoRequiredRuleArgs)e;

            object valueOne = (string)Utilities.CallByName(target, args.FirstProperty, CallType.Get);
            object valueTwo = (string)Utilities.CallByName(target, args.SecondProperty, CallType.Get);

            if (valueOne == null && valueTwo == null)
            {
                args.Description = string.Empty;
            }
            else if (valueOne != null && valueTwo != null)
            {
                args.Description = string.Empty;
            }
            else
            {
                args.Description = string.Format(
                    "{0} and {1} must both be empty or not empty.", args.FirstProperty, args.SecondProperty);
            }

            bool isValid = string.IsNullOrEmpty(args.Description);
            return isValid;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool PositiveInteger(object target, RuleArgs e)
        {
            PositiveIntegerRuleArgs args = (PositiveIntegerRuleArgs)e;

            string value = (string)Utilities.CallByName(
              target, e.PropertyName, CallType.Get);

            if (!string.IsNullOrEmpty(value))
            {
                int integer;

                if (!int.TryParse(value, out integer))
                {
                    args.Description = "Invalid integer";
                    return false;
                }

                if (integer < 0)
                {
                    if (!string.IsNullOrEmpty(args.UserMessage))
                        args.Description = string.Format(args.UserMessage, args.PropertyName);
                    else
                        args.Description = string.Format("{0} Not Valid", args.PropertyName);
                    return false;
                }
            }

            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool RegExpMatch(object target, RuleArgs e)
        {
            RegExpMatchRuleArgs args = (RegExpMatchRuleArgs)e;

            if (!CommonRules.RegExMatch(target, e))
            {
                if (!string.IsNullOrEmpty(args.UserMessage))
                    args.Description = string.Format(args.UserMessage, args.PropertyName);
                else
                    args.Description = string.Format("{0} Not Valid", args.PropertyName);
                return false;
            }

            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool StringRange(object target, RuleArgs e)
        {
            StringRangeRuleArgs args = (StringRangeRuleArgs)e;

            string value = (string)Csla.Utilities.CallByName(
              target, e.PropertyName, CallType.Get);

            if (!string.IsNullOrEmpty(value) && (args.MinLength > value.Length || value.Length > args.MaxLength))
            {
                if (!string.IsNullOrEmpty(args.UserMessage))
                    args.Description = string.Format(args.UserMessage, args.PropertyName, args.MinLength, args.MaxLength, value);
                else
                    args.Description = string.Format("{0} must be between {1} and {2}", args.PropertyName, args.MinLength, args.MaxLength);

                return false;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool IsPicklistValue(object target, RuleArgs e)
        {
            IsPicklistValueRuleArgs arg = (IsPicklistValueRuleArgs)e;
            string picklistCode = arg.PicklistCode;
            string value = (string)Csla.Utilities.CallByName(
              target, e.PropertyName, CallType.Get);

            if (value == string.Empty)
                return true;

            Picklist picklist = DalUtils.GetPicklistByCode(picklistCode);
            if (picklist == null)
            {
                arg.Description = string.Format("Picklist '{0}' does not exist", picklistCode);
                return false;
            }

            // It's possible the value is already a picklist item's Id value.
            int id = -1;
            if (Int32.TryParse(value, out id))
            {
                picklist.IncludeValue = id; // May be a picklist in Inactive state.
                if (picklist.PickList.ContainsKey(id))
                {
                    return true;
                }   
            }

            // Otherwise, assume the value as picklist item's value and do the validation.
            if (picklist.GetListItemIdByValue(value) == -1)
            {
                arg.Description = string.Format("Picklist item with value '{0}' does not exist", value);
                return false;
            }

            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool StringRequired(object target, RuleArgs e)
        {
            StringRequiredRuleArgs args = (StringRequiredRuleArgs)e;

            string value = (string)Utilities.CallByName(
              target, e.PropertyName, CallType.Get);
            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(args.UserMessage))
                    args.Description = string.Format(args.UserMessage, args.PropertyName);
                else
                    args.Description = string.Format("{0} Required", args.PropertyName);
                return false;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool IsDateFormat(object target, RuleArgs e)
        {
            bool isMatch = false;

            IsDateFormatRuleArgs arg = (IsDateFormatRuleArgs)e;

            string dateFormat = arg.DateFormat;
            System.Globalization.CultureInfo culture = arg.Culture;

            string value = (string)Csla.Utilities.CallByName(target, e.PropertyName, CallType.Get);

            if (value == string.Empty)
                return true;

            DateTime date;
            if (DateTime.TryParse(value, out date))
                isMatch = true;
            else
                arg.Description = string.Format("Date with value '{0}' is not in the correct format", value);

            return isMatch;
        }

        /// <summary>
        /// Rule that checks to make sure a value
        /// matches a given CAS pattern.
        /// </summary>
        /// <param name="target">Object containing the data to validate</param>
        /// <param name="e">RegExRuleArgs parameter specifying the name of the 
        /// property to validate and the regex pattern.</param>
        /// <returns>False if the rule is broken</returns>
        /// <remarks>
        /// This implementation uses late binding.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool CASFormatMatch(object target, RuleArgs e)
        {
            string value = (string)Csla.Utilities.CallByName(target, e.PropertyName, CallType.Get);

            Regex rx = new Regex(@"^\d{6}-\d{2}-\d{1}$"); //CAS format of xxxxxx-xx-x

            if (!rx.IsMatch(value))
            {
                e.Description = string.Format("", e.PropertyName);
                return false;
            }
            else
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool CASCheckSum(object target, RuleArgs e)
        {
            return true;
        }

        /// <summary>
        /// Rule that checks to make sure the length is ok.
        /// </summary>
        /// <param name="target">Object containing the data to validate</param>
        /// <param name="args">TextLengthRuleArgs parameter specifying the name of the 
        /// property to validate and the min and max length.</param>
        /// <returns>False if the rule is broken</returns>
        /// <remarks>
        /// This implementation uses late binding.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool TextLength(object target, RuleArgs args)
        {
            TextLengthRuleArgs e = args as TextLengthRuleArgs;
            if (e != null) // Coverity fix - CID 11690 
            {
                string value = (string)Csla.Utilities.CallByName(target, e.MethodName, CallType.Get, e.PropertyName);
                if (string.IsNullOrEmpty(value) || (value.Length <= e.MaxLength && value.Length >= e.MinLength))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Rule that checks to make sure value contains valid NumericRange [0 to 100(99 is valid)(101 is broken rule)]
        /// Validation with validation rules assigned to property.
        /// </summary>
        /// <param name="target">Object containing the data to validate</param>
        /// <param name="e">contains parameter specifying the name of the 
        /// property to validate and the NumericRange rules [RuleArgs] for validation.</param>
        /// <returns>False if the rule is broken</returns>
        /// <remarks>
        /// This implementation uses late binding.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static bool NumericRange(object target, RuleArgs e)
        {
            NumericRangeRuleArgs args = (NumericRangeRuleArgs)e;

            string value = (string)Utilities.CallByName(
              target, e.PropertyName, CallType.Get);

            if (string.IsNullOrEmpty(value))
                return true;

            double dOutput = 0;
            Int32 integer;

            if (Double.TryParse(value, out dOutput))
            {
                integer = (Int32)dOutput;
            }
            else
            {
                args.Description = "Invalid integer";
                return false;
            }

            if ((integer < args.MinRange) || (integer > args.MaxRange))
            {
                if (!string.IsNullOrEmpty(args.UserMessage))
                    args.Description = string.Format(args.UserMessage, args.PropertyName, args.MinRange, args.MaxRange, value);
                else
                    args.Description = string.Format("{0} must be between {1} and {2}", args.PropertyName, args.MinRange, args.MaxRange);
                return false;
            }

            return true;
        }
        #endregion

    }
}
