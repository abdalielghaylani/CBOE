using System;

namespace CambridgeSoft.COE.Framework.Common.Validation
{
    /// <summary>
    /// Information about business objects which contain broken rules.
    /// </summary>
    public class BrokenRuleDescription
    {
        #region Attributes
        private string[] _brokenRulesMessages;
        private Object _businessObject;
        #endregion

        #region Properties

        /// <summary>
        /// Strings containing each broken rule message of the business object.
        /// </summary>
        public string[] BrokenRulesMessages
        {
            get
            {
                return _brokenRulesMessages;
            }
            set
            {
                _brokenRulesMessages = value;
            }
        }

        /// <summary>
        /// The business object that contains broken rules.
        /// </summary>
        public Object BusinessObject
        {
            get
            {
                return _businessObject;
            }
            set
            {
                _businessObject = value;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new BrokenRuleDescription object.
        /// </summary>
        /// <param name="businessObject">The business object that contains broken rules.</param>
        /// <param name="brokenRulesMessages">Strings containing each broken rule message of the business object.</param>
        public BrokenRuleDescription(Object businessObject, string[] brokenRulesMessages)
        {
            _businessObject = businessObject;
            _brokenRulesMessages = brokenRulesMessages;
        }

        #endregion
    }
}
