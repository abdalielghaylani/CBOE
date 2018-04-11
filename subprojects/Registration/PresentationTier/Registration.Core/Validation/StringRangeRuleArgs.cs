using System;
using System.Collections.Generic;
using System.Text;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Validation
{
    public class StringRangeRuleArgs : RuleArgs
    {
        private int _maxLength;
        private int _minLength;
        private string _userMessage;

        /// <summary>
        /// Get the max length for the string.
        /// </summary>
        public int MaxLength
        {
            get { return _maxLength; }
        }

        public int MinLength
        {
            get { return _minLength; }
        }

        public string UserMessage
        {
            get { return _userMessage; }
        }

        /// <summary>
        /// Create a new object.
        /// </summary>
        /// <param name="propertyName">Name of the property to validate.</param>
        /// <param name="maxLength">Max length of characters allowed.</param>
        public StringRangeRuleArgs(string propertyName, int minLength, int maxLength, string userMessage)
            : this(propertyName, minLength, maxLength)
        {
            _userMessage = userMessage;
        }

        public StringRangeRuleArgs(string propertyName, int minLength, int maxLength)
            : base(propertyName)
        {
            _maxLength = maxLength;
            _minLength = minLength;
            _userMessage = null;
        }

        /// <summary>
        /// Return a string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + "?maxLength=" + _maxLength.ToString() + "&minLength=" + _minLength.ToString();
        }
    }

}
