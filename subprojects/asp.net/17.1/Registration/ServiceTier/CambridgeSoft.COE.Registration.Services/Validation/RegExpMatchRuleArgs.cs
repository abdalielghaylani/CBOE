//using System;
//using System.Collections.Generic;
//using System.Text;
//using Csla.Validation;

//namespace CambridgeSoft.COE.Registration.Validation
//{
//    public class RegExpMatchRuleArgs : CommonRules.RegExRuleArgs
//    {
//        private string _userMessage = null;

//        public string UserMessage
//        {
//            get { return _userMessage; }
//        }

//        /// <summary>
//        /// Create a new object.
//        /// </summary>
//        /// <param name="propertyName">Name of the property to validate.</param>
//        /// <param name="maxLength">Max length of characters allowed.</param>
//        public RegExpMatchRuleArgs(string propertyName, string regExp, string userMessage)
//            : base(propertyName, regExp)
//        {
//            _userMessage = userMessage;
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
