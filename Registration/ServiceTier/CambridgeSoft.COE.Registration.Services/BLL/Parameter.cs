using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.BLL
{
    [Serializable]
    public class Parameter: BusinessBase<Parameter>
    {
        #region Variables

        string _name;
        string _value;

        #endregion

        #region Properties

        public string Name
        {
            get 
            {
                return _name;
            }
        }

        public string Value 
        {
            get 
            {
                return _value;
            }
        }

        #endregion

        #region Constructors

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

        [COEUserActionDescription("CreateParameter")]
        public static Parameter NewParameter(string name, string value, bool isNew) 
        {
            try
            {
                return new Parameter(name, value, isNew);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        #region Business Methods

        protected override object GetIdValue()
        {
            return Guid.NewGuid();
        }

        [COEUserActionDescription("Get")]
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
