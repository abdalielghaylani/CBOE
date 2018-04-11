//using System;
//using System.Collections.Generic;
//using System.Text;
//using Csla.Validation;

//namespace CambridgeSoft.COE.Registration.Validation
//{
//    public class StringRequiredRuleArgs : RuleArgs
//    {
//        private string _userMessage;

//        public string UserMessage
//        {
//            get { return _userMessage; }
//        }

//        /// <summary>
//        /// Create a new object.
//        /// </summary>
//        /// <param name="propertyName">Name of the property to validate.</param>
//        /// <param name="maxLength">Max length of characters allowed.</param>
//        public StringRequiredRuleArgs(string propertyName, string userMessage)
//            : this(propertyName)
//        {
//            _userMessage = userMessage;
//        }

//        public StringRequiredRuleArgs(string propertyName)
//            : base(propertyName)
//        {
//            _userMessage = null;
//        }

//        /// <summary>
//        /// Return a string representation of the object.
//        /// </summary>
//        public override string ToString()
//        {
//            return base.ToString() + (_userMessage != null ? "?userMessage=" + _userMessage : string.Empty);
//        }
//    }
//}
