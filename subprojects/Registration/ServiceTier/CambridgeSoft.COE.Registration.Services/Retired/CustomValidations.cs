//using System;
//using System.IO;
//using System.Xml.XPath;

//using Csla;
//using Csla.Validation;

//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Registration.Services.Types;
//using CambridgeSoft.COE.Registration.Services.Common;

//public class BrokenRuleDescription
//{
//    #region Attributes
//    private string[] _brokenRulesMessages;
//    private Csla.Core.BusinessBase _businessObject;
//    #endregion

//    #region Properties
//    public string[] BrokenRulesMessages
//    {
//        get
//        {
//            return _brokenRulesMessages;
//        }
//        set
//        {
//            _brokenRulesMessages = value;
//        }
//    }

//    public Csla.Core.BusinessBase BusinessObject
//    {
//        get
//        {
//            return _businessObject;
//        }
//        set
//        {
//            _businessObject = value;
//        }
//    }
//    #endregion

//    #region Constructors

//    public BrokenRuleDescription(Csla.Core.BusinessBase businessObject, string[] brokenRulesMessages)
//    {
//        _businessObject = businessObject;
//        _brokenRulesMessages = brokenRulesMessages;
//    }
//    #endregion
//}

//public class ValidationRulesFactory
//{
//    #region Singleton Pattern
//    private static ValidationRulesFactory _instance = null;

//    public static ValidationRulesFactory GetInstance()
//    {
//        if (_instance == null)
//            _instance = new ValidationRulesFactory();

//        return _instance;
//    }

//    private ValidationRulesFactory()
//    {
//    }
//    #endregion

//    [COEUserActionDescription("AddValidationRulesForProperty")]
//    public void AddInstanceRules(ValidationRules validationRules, string propertyName, string xml)
//    {
//        try
//        {
//            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
//            XPathNavigator xNavigator = xDocument.CreateNavigator();
//            XPathNodeIterator xIterator = xNavigator.Select("validationRuleList/validationRule");
//            if (xIterator.MoveNext())
//            {
//                do
//                {
//                    string ruleName = xIterator.Current.GetAttribute("validationRuleName", xIterator.Current.NamespaceURI);

//                    if (!string.IsNullOrEmpty(ruleName))
//                    {
//                        string message = xIterator.Current.GetAttribute("errorMessage", xIterator.Current.NamespaceURI);

//                        switch (ruleName)
//                        {
//                            case "positiveInteger":
//                                validationRules.AddInstanceRule(PositiveInteger, new PositiveIntegerRuleArgs(propertyName, message));
//                                break;
//                            case "regularExpressionMatch":
//                                string regExp = xIterator.Current.SelectSingleNode("params/param[@name=\"regExp\"]").GetAttribute("value", xIterator.Current.NamespaceURI);
//                                validationRules.AddInstanceRule(RegExpMatch, new RegExpMatchRuleArgs(propertyName, regExp, message));
//                                break;
//                            case "textLength":
//                                int max = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"max\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
//                                int min = int.Parse(xIterator.Current.SelectSingleNode("params/param[@name=\"min\"]").GetAttribute("value", xIterator.Current.NamespaceURI));
//                                validationRules.AddInstanceRule(StringRange, new StringRangeRuleArgs(propertyName, min, max, message));

//                                break;

//                            case "requiredField":
//                                validationRules.AddInstanceRule(StringRequired, new StringRequiredRuleArgs(propertyName, message));
//                                break;
//                        }
//                    }
//                } while (xIterator.Current.MoveToNext());
//            }
//        }
//        catch (Exception exception)
//        {
//            COEExceptionDispatcher.HandleBLLException(exception);
//        }
//    }


//    #region Custom Validations
//    public class PositiveIntegerRuleArgs : CommonRules.IntegerMinValueRuleArgs
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
//        public PositiveIntegerRuleArgs(string propertyName, string userMessage)
//            : base(propertyName, 0)
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

//    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
//    public static bool PositiveInteger(object target, RuleArgs e)
//    {
//        PositiveIntegerRuleArgs args = (PositiveIntegerRuleArgs)e;

//        string value = (string)Utilities.CallByName(
//          target, e.PropertyName, CallType.Get);
        
//        if (!string.IsNullOrEmpty(value))
//        {
//            int integer;

//            if (!int.TryParse(value, out integer))
//            {
//                args.Description = "Invalid integer";
//                return false;
//            }

//            if (integer < 0)
//            {
//                if (!string.IsNullOrEmpty(args.UserMessage))
//                    args.Description = string.Format(args.UserMessage, args.PropertyName);
//                else
//                    args.Description = string.Format("{0} Not Valid", args.PropertyName);
//                return false;
//            }
//        }

//        return true;
//    }

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

//    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
//    public static bool RegExpMatch(object target, RuleArgs e)
//    {
//        RegExpMatchRuleArgs args = (RegExpMatchRuleArgs)e;

//        if (!CommonRules.RegExMatch(target, e))
//        {
//            if (!string.IsNullOrEmpty(args.UserMessage))
//                args.Description = string.Format(args.UserMessage, args.PropertyName);
//            else
//                args.Description = string.Format("{0} Not Valid", args.PropertyName);
//            return false;
//        }

//        return true;
//    }
    
//    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
//    public static bool StringRequired(object target, RuleArgs e)
//    {
//        StringRequiredRuleArgs args = (StringRequiredRuleArgs)e;

//        string value = (string)Utilities.CallByName(
//          target, e.PropertyName, CallType.Get);
//        if (string.IsNullOrEmpty(value))
//        {
//            if(!string.IsNullOrEmpty(args.UserMessage))
//                args.Description = string.Format(args.UserMessage, args.PropertyName);
//            else
//                args.Description = string.Format("{0} Required", args.PropertyName);
//            return false;
//        }
//        return true;
//    }

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
//        public StringRequiredRuleArgs(string propertyName, string userMessage) : this(propertyName)
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

//    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
//    public static bool StringRange(object target, RuleArgs e)
//    {
//        StringRangeRuleArgs args = (StringRangeRuleArgs)e;

//        string value = (string)Csla.Utilities.CallByName(
//          target, e.PropertyName, CallType.Get);

//        if (!string.IsNullOrEmpty(value) && (args.MinLength > value.Length || value.Length > args.MaxLength))
//        {
//            if (!string.IsNullOrEmpty(args.UserMessage))
//                args.Description = string.Format(args.UserMessage, args.PropertyName, args.MinLength, args.MaxLength, value);
//            else
//                args.Description = string.Format("{0} must be between {1} and {2}", args.PropertyName, args.MinLength, args.MaxLength);

//            return false;
//        }
//        return true;
//    }

//    public class StringRangeRuleArgs : RuleArgs
//    {
//        private int _maxLength;
//        private int _minLength;
//        private string _userMessage;

//        /// <summary>
//        /// Get the max length for the string.
//        /// </summary>
//        public int MaxLength
//        {
//            get { return _maxLength; }
//        }

//        public int MinLength
//        {
//            get { return _minLength; }
//        }

//        public string UserMessage
//        {
//            get { return _userMessage; }
//        }

//        /// <summary>
//        /// Create a new object.
//        /// </summary>
//        /// <param name="propertyName">Name of the property to validate.</param>
//        /// <param name="maxLength">Max length of characters allowed.</param>
//        public StringRangeRuleArgs(string propertyName, int minLength, int maxLength, string userMessage)
//            : this(propertyName, minLength, maxLength)
//        {
//            _userMessage = userMessage;
//        }

//        public StringRangeRuleArgs(string propertyName, int minLength, int maxLength)
//            : base(propertyName)
//        {
//            _maxLength = maxLength;
//            _minLength = minLength;
//            _userMessage = null;
//        }

//        /// <summary>
//        /// Return a string representation of the object.
//        /// </summary>
//        public override string ToString()
//        {
//            return base.ToString() + "?maxLength=" + _maxLength.ToString() + "&minLength=" + _minLength.ToString();
//        }
//    }

//    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
//    public static bool IsPicklistValue(object target, RuleArgs e)
//    {
//        IsPicklistValueRuleArgs arg = (IsPicklistValueRuleArgs)e;
//        string picklistCode = arg.PicklistCode;
//        string value = (string)Csla.Utilities.CallByName(
//          target, e.PropertyName, CallType.Get);

//        if (value == string.Empty)
//            return true;

//        Picklist picklist = RegSvcUtilities.GetPicklistByCode(picklistCode);
//        if (picklist == null)
//        {
//            arg.Description = string.Format("Picklist '{0}' does not exist", picklistCode);
//            return false;
//        }
     
//        // It's possible the value is already a picklist item's Id value.
//        int id = -1;
//        if (Int32.TryParse(value, out id))
//        {
//            if (picklist.PickList.ContainsKey(id))
//            {
//                return true;
//            }
//        }

//        // Otherwise, assume the value as picklist item's value and do the validation.
//        if (picklist.GetListItemIdByValue(value) == -1)
//        {
//            arg.Description = string.Format("Picklist item with value '{0}' does not exist", value);
//            return false;
//        }

//        return true;
//    }

//    public class IsPicklistValueRuleArgs : RuleArgs
//    {
//        string _picklistCode = string.Empty;

//        public IsPicklistValueRuleArgs(string propertyName, string picklistCode)
//            : base(propertyName)
//        {
//            _picklistCode = picklistCode;
//        }

//        public string PicklistCode
//        {
//            get { return _picklistCode; }
//        }
//    }
//    #endregion
//}

