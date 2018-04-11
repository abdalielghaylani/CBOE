using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.BLL;

using Csla;
using Csla.Data;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class ValidationRuleList : BusinessListBase<ValidationRuleList, ValidationRule>
    {
        #region Business Methods

        protected override void RemoveItem(int index)
        {

            this[index].Delete();
            if (!this[index].IsNew)
                DeletedList.Add(this[index]);


            this.Items.RemoveAt(index);
        }

        public void RemoveValidationRule(int index) 
        {
            this.RemoveItem(index);
        }


        public bool CheckDuplicated(string valRuleName) 
        {
            bool isDuplicated=false;
            foreach(ValidationRule valRule in this)
            {
                if (valRule.Name == valRuleName)
                    isDuplicated = true;
            }
            return isDuplicated;
        }
        #endregion

        #region Factory Methods

        public static ValidationRuleList NewValidationRuleList()
        {
            return new ValidationRuleList();
        }

        [COEUserActionDescription("CreateValidatioinRuleList")]
        public static ValidationRuleList NewValidationRuleList(XmlDocument valiadtionRulesXml)
        {
            try
            {
                ValidationRuleList validationRuleList = new ValidationRuleList();

                foreach (XmlNode node in valiadtionRulesXml)
                {
                    ValidationRule valRule = ValidationRule.NewValidationRule(node.OuterXml);
                    validationRuleList.Add(valRule);
                }
                return validationRuleList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateValidatioinRuleList")]
        public static ValidationRuleList NewValidationRuleList(string propertyName)
        {
            try
            {
                ValidationRuleList validationRuleList = new ValidationRuleList();

                XPathDocument xDocument = new XPathDocument(new StringReader(propertyName.ToString()));

                XPathNavigator xNavigator = xDocument.CreateNavigator();

                XPathNodeIterator xIterator = xNavigator.Select("ValidationRuleList/ValidationRule");
                if (xIterator.MoveNext())
                {

                    bool more = false;
                    do
                    {
                        ValidationRule validationRule = ValidationRule.NewValidationRule(xIterator.Current.OuterXml);
                        validationRuleList.Add(validationRule);
                        more = xIterator.Current.MoveToNext();
                    } while (more);
                }
                return validationRuleList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateValidatioinRuleList")]
        public static ValidationRuleList NewValidationRuleList(XmlNode xml)
        {
            try
            {
                return new ValidationRuleList(xml);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }


        /*internal static ValidationRuleList GetValidationRuleList(SafeDataReader dr)
        {
            return new ValidationRuleList(dr);
        }*/

        private ValidationRuleList()
        {
            MarkAsChild();
        }

        private ValidationRuleList(XmlNode xml)
        {
            FillValidationRules(xml);
        }

        private void FillValidationRules(XmlNode xml)
        {
            foreach (XmlNode valRule in xml)
            {
                this.Add(ValidationRule.NewValidationRule(valRule.OuterXml));       
            }
        }

        [COEUserActionDescription("GetValidationRuleList")]
        public static ValidationRuleList GetValidationRuleList()
        {
            try
            {
                return DataPortal.Fetch<ValidationRuleList>(new Criteria());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        #region Data Access
        [Serializable()]
        protected class Criteria
        {

            public Criteria() { }

        }

        protected void DataPortal_Fetch(Criteria criteria)
        {
        }

        #endregion

        #region Xml

        [COEUserActionDescription("GetValidationRuleListXml")]
        public string UpdateSelf()
        {
            try
            {
                StringBuilder builder = new StringBuilder("");

                builder.Append("<validationRuleList>");

                for (int i = 0; i < this.Count; i++)
                {
                    builder.Append(this[i].UpdateSelf());
                }

                builder.Append("</validationRuleList>");

                return builder.ToString();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetValidationRuleListXml")]
        public string UpdateSelfConfig(bool propertyIsNew)
        {
            try
            {
                StringBuilder builder = new StringBuilder("");
                builder.Append("<validationRuleList>");
                
                    foreach (ValidationRule val in this)
                    {                    
                        builder.Append(val.UpdateSelfConfig(propertyIsNew));
                    }
                    foreach (ValidationRule delVal in DeletedList)
                    {
                        if(!delVal.IsNew)
                            builder.Append(delVal.UpdateSelfConfig(propertyIsNew));
                    }
                
                builder.Append("</validationRuleList>");

                //DeletedList.Clear();

                return builder.ToString();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }


        #endregion
    }
}
