using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Validation
{
    public class NumericRangeRuleArgs : RuleArgs
    {
        private int _minRange;
        private int _maxRange;

        private string _userMessage;

        /// <summary>
        /// Get the max range for the number.
        /// </summary>
        public int MaxRange
        {
            get { return _maxRange; }
        }

        /// <summary>
        /// Get the min range for the number.
        /// </summary>
        public int MinRange
        {
            get { return _minRange; }
        }

        /// <summary>
        /// Get the broken message to show.
        /// </summary>
        public string UserMessage
        {
            get { return _userMessage; }
        }

        /// <summary>
        /// Create a new object.
        /// </summary>
        /// <param name="propertyName">Name of the property to validate.</param>
        /// <param name="propertyName">Min range of numbers allowed.</param>
        /// <param name="maxLength">Max range of numbers allowed.</param>
        public NumericRangeRuleArgs(string propertyName, int minLength, int maxLength, string userMessage)
            : this(propertyName, minLength, maxLength)
        {
            _userMessage = userMessage;
        }

        public NumericRangeRuleArgs(string propertyName, int minLength, int maxLength)
            : base(propertyName)
        {
            _maxRange = maxLength;
            _minRange = minLength;
            _userMessage = null;
        }

        /// <summary>
        /// Return a string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + "?maxRange=" + _maxRange.ToString() + "&minRange=" + _minRange.ToString();
        }
    }
}
