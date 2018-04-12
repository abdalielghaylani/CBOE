using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;
//using CambridgeSoft.COE.Registration;
//using CambridgeSoft.COE.Framework.Common;
//using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla;
using Csla.Data;
using Csla.Validation;


//this is temporary until all the commands are converted for using the DAL
//using Oracle.DataAccess.Types;
//using Oracle.DataAccess.Client;
using System.ComponentModel;


namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
    public class ValidationRuleList : BusinessListBase<ValidationRuleList, ValidationRule>
    {
        #region Business Methods

        private string _specialFolder = CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + @"SimulationFolder\Registration\";

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
            bool isDuplicated = false;
            foreach (ValidationRule valRule in this)
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

        public static ValidationRuleList NewValidationRuleList(XmlDocument valiadtionRulesXml)
        {
            ValidationRuleList validationRuleList = new ValidationRuleList();

            foreach (XmlNode node in valiadtionRulesXml)
            {
                ValidationRule valRule = ValidationRule.NewValidationRule(node.OuterXml);
                validationRuleList.Add(valRule);
            }
            return validationRuleList;
        }

        public static ValidationRuleList NewValidationRuleList(string propertyName)
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

        public static ValidationRuleList NewValidationRuleList(XmlNode xml)
        {

            return new ValidationRuleList(xml);

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


        public static ValidationRuleList GetValidationRuleList()
        {
            return DataPortal.Fetch<ValidationRuleList>(new Criteria());
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
            //this.RaiseListChangedEvents = false;
            //using (OracleConnection cn = new OracleConnection("Data Source=sunnyora;User ID=regdb;Password=Oracle;"))
            //{
            //    cn.Open();
            //    using (OracleCommand cmd = cn.CreateCommand())
            //    {

            //        cmd.CommandText = "Select ID, IDENTIFIER_TYPE, IDENTIFIER_DESCRIPTOR from ValidationRules";

            //        using (SafeDataReader reader = new SafeDataReader(cmd.ExecuteReader()))
            //        {
            //            Fetch(reader);
            //        }
            //    }
            //}
            //this.RaiseListChangedEvents = true;
        }

        private void Fetch(SafeDataReader reader)
        {
            while (reader.Read())
            {
                //ValidationRule validationRule = ValidationRule.NewValidationRule();
                //validationRule.ID = reader.GetInt32("ID");
                //validationRule.Type = reader.GetInt32("IDENTIFIER_TYPE");
                //validationRule.Description = reader.GetString("IDENTIFIER_DESCRIPTOR");

                //this.Add(validationRule);
            }
        }


        #endregion

        #region Xml

        public string UpdateSelf()
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

        public string UpdateSelfConfig(bool propertyIsNew)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<validationRuleList>");

            foreach (ValidationRule val in this)
            {

                builder.Append(val.UpdateSelfConfig(propertyIsNew));
            }
            foreach (ValidationRule delVal in DeletedList)
            {
                if (!delVal.IsNew)
                    builder.Append(delVal.UpdateSelfConfig(propertyIsNew));
            }

            builder.Append("</validationRuleList>");

            DeletedList.Clear();

            return builder.ToString();
        }


        #endregion
    }
}
