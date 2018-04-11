using System;
using System.Text.RegularExpressions;
using Csla;
using Csla.Validation;
using Csla.Properties;

namespace CambridgeSoft.COE.Registration.Services.Common.Validation
{
    public static class COERules
    {


        #region CAS

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

        public static bool CASCheckSum(object target, RuleArgs e)
        {
            return true;
        }

        #endregion

        #region TextLength
        /// <summary>
        /// Rule that checks to make sure the lenght is ok.
        /// </summary>
        /// <param name="target">Object containing the data to validate</param>
        /// <param name="args">TextLengthRuleArgs parameter specifying the name of the 
        /// property to validate and the min and max length.</param>
        /// <returns>False if the rule is broken</returns>
        /// <remarks>
        /// This implementation uses late binding.
        /// </remarks>
        public static bool TextLength(object target, RuleArgs args) {
            TextLengthRuleArgs e = args as TextLengthRuleArgs;
            string value = (string) Csla.Utilities.CallByName(target, e.MethodName, CallType.Get, e.PropertyName);
            if(string.IsNullOrEmpty(value) || (value.Length <= e.MaxLength && value.Length >= e.MinLength))
                return true;
            else
                return false;
        }
        public class TextLengthRuleArgs : RuleArgs {
            private string _methodName;
            private int _minLength;
            private int _maxLength;

            public int MinLength {
                get {
                    return _minLength;
                }
                set {
                    _minLength = value;
                }
            }
            public int MaxLength {
                get {
                    return _maxLength;
                }
                set {
                    _maxLength = value;
                }
            }

            public string MethodName {
                get {
                    return _methodName;
                }
            }

            public TextLengthRuleArgs(string propertyName, int minLength, int maxLength, string methodName)
                : base(propertyName) {
                _methodName = methodName;
                _minLength = minLength;
                _maxLength = maxLength;
            }

            public override string ToString() {
                return base.ToString() + "!" + _methodName + "!" + _minLength.ToString() + "!" + _maxLength.ToString();
            }
        }
        #endregion

    }
}
