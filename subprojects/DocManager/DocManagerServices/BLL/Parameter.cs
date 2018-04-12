using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    /// <summary>
    /// Domain object used to hold information about an 'instance' of a Parameter.
    /// It is used with in ValidationRule, It is used to validate based on name and value.
    /// </summary>
    [Serializable]
    public class Parameter : BusinessBase<Parameter>
    {
        #region Variables

        string _name;
        string _value;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Value of the parameter
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameter constructor used for initialize the properties
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isNew"></param>
        private Parameter(string name, string value, bool isNew)
        {
            _name = name;
            _value = value;
            if (!isNew)
            {
                MarkClean();
                MarkOld();
            }
        }

        #endregion

        #region Factory Methods
        
        /// <summary>
        /// call the constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isNew"></param>
        /// <returns>parameter object</returns>
        public static Parameter NewParameter(string name, string value, bool isNew)
        {
            return new Parameter(name, value, isNew);
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Get the Id value
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            throw new Exception("The method or operation is not implemented.");
        }            

        /// <summary>
        /// returns the parameter xml
        /// </summary>
        /// <param name="valRuleIsNew"></param>
        /// <returns>param xml</returns>
        public string UpdateSelfConfig(bool valRuleIsNew)
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_value))
                builder.Append("<param name=\"" + _name + "\" value=\"" + _value + "\"");

            if (!IsDeleted && IsNew && !valRuleIsNew)
            {
                builder.Append(" insert=\"yes\"/>");
                return builder.ToString();
            }
            else if (IsDeleted && !IsNew)
            {
                builder.Append(" delete=\"yes\"/>");
                return builder.ToString();
            }
            builder.Append("/>");
            return builder.ToString();
        }

        #endregion
    }
}
