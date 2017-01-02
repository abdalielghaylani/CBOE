using System;
using System.Collections.Generic;
using System.Text;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Validation
{
    public class TextLengthRuleArgs : RuleArgs
    {
        private string _methodName;
        private int _minLength;
        private int _maxLength;

        public int MinLength
        {
            get
            {
                return _minLength;
            }
            set
            {
                _minLength = value;
            }
        }
        public int MaxLength
        {
            get
            {
                return _maxLength;
            }
            set
            {
                _maxLength = value;
            }
        }

        public string MethodName
        {
            get
            {
                return _methodName;
            }
        }

        public TextLengthRuleArgs(string propertyName, int minLength, int maxLength, string methodName)
            : base(propertyName)
        {
            _methodName = methodName;
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public override string ToString()
        {
            return base.ToString() + "!" + _methodName + "!" + _minLength.ToString() + "!" + _maxLength.ToString();
        }
    }
}
