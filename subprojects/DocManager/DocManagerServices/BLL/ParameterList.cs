using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    /// <summary>
    /// Domain object used to hold information about an 'instance' of ParameterList
    /// It is used with in ValidationRule, It is used to validate based on name and value.
    /// </summary>
    [Serializable]
    public class ParameterList : BusinessListBase<ParameterList, Parameter>
    {
        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        private ParameterList() { }

        /// <summary>
        /// Constructor for initialize properties
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="isNew"></param>
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

        /// <summary>
        /// call the default constructor
        /// </summary>
        /// <returns>parameterList object</returns>
        public static ParameterList NewParameterList()
        {
            return new ParameterList();
        }

        /// <summary>
        /// call the constructor
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns>ParameterList object</returns>
        public static ParameterList NewParameterList(XmlNode paramList)
        {
            return new ParameterList(paramList, false);
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Remove the parameter from the parameter list
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            this[index].Delete();
            if (!this[index].IsNew)
                DeletedList.Add(this[index]);
            this.Items.RemoveAt(index);
        }

        /// <summary>
        /// returns the parameter xml
        /// </summary>
        /// <param name="valRuleIsNew"></param>
        /// <returns>Params xml</returns>
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

