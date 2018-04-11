using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.BLL
{
    [Serializable]
    public class ParameterList : BusinessListBase<ParameterList, Parameter>
    {
        #region Properties
        #endregion

        #region Constructors

        private ParameterList() { }

        private ParameterList(XmlNode paramList, bool isNew) 
        {
            if (paramList != null)
            {
                foreach (XmlNode paramNode in paramList)
                {
                    string name = paramNode.Attributes["name"].Value.ToString();
                    string value = paramNode.Attributes["value"].Value.ToString();
                    Add(Parameter.NewParameter(name, value, isNew));
                }
            }            
        }

        #endregion

        #region Factory Methods

        [COEUserActionDescription("CreateParameterList")]
        public static ParameterList NewParameterList() 
        {
            try
            {
                return new ParameterList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateParameterList")]
        public static ParameterList NewParameterList(XmlNode paramList) 
        {
            try
            {
                return new ParameterList(paramList, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        #region Business Methods

        protected override void RemoveItem(int index)
        {
            this[index].Delete();
            if (!this[index].IsNew)
                DeletedList.Add(this[index]);
            this.Items.RemoveAt(index);
        }

        public string UpdateSelfConfig(bool valRuleIsNew) 
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<params>");

            foreach (Parameter parameter in this) 
            {
                builder.Append(parameter.UpdateSelfConfig(valRuleIsNew));
            }

            foreach (Parameter delParameter in this.DeletedList) 
            {
                builder.Append(delParameter.UpdateSelfConfig(valRuleIsNew));
            }

            builder.Append("</params>");

            return builder.ToString();
        }

        #endregion
       
    }
}
