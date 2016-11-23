//using System;
//using System.Collections.Generic;
//using System.Text;
//using Csla;
//using Csla.Validation;
//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Registration.Validation;
//using System.Xml;
//using System.Text.RegularExpressions;
//using System.Globalization;
//using System.Threading;

//namespace CambridgeSoft.COE.Registration.Services.Types {
//    [Serializable()]
//    public class Percentage : BusinessBase<Percentage> {
//        #region Business Methods

//        private int _id;
//        private int _compoundId;
//        private int _componentIndex;
//        private string _regNum;
//        private string _value;
//        private ValidationRuleList _validationRuleList;

//        [System.ComponentModel.DataObjectField(true, true)]
//        public int ID
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _id;
//            }
//        }

//        protected override object GetIdValue()
//        {
//            return _id;
//        }
        
//        public int CompoundID
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _compoundId;
//            }
//            set
//            {
//                CanWriteProperty(true);
//                if(_compoundId != value)
//                {
//                    _compoundId = value;
//                    PropertyHasChanged();
//                }
//            }
//        }

//        public int ComponentIndex {
//            get {
//                CanReadProperty(true);
//                return _componentIndex;
//            }
//            set {
//                CanWriteProperty(true);
//                if(_componentIndex != value) {
//                    _componentIndex = value;
//                    //PropertyHasChanged();
//                }
//            }
//        }

//        public string RegNum
//        {
//            get {
//                CanReadProperty(true);
//                return _regNum;
//            }
//            set {
//                CanWriteProperty(true);
//                if (_regNum != value)
//                {
//                    _regNum = value;
//                    PropertyHasChanged();
//                }
//            }
//        }

//        public string DisplayKey
//        {
//            get {
//                CanReadProperty(true);

//                if (!string.IsNullOrEmpty(_regNum))
//                    return _regNum;

//                if (_compoundId > 0)
//                    return string.Format("{0:000}", _compoundId);

//                return string.Format("{0:000}", Math.Abs(_componentIndex + 1));
//            }
//        }

//        public string Value
//        {
//            get
//            {
//                return _value;
//            }
//            set
//            {
//                CanWriteProperty(true);
//                if (value == null) value = string.Empty;
//                if (_value != value.Trim())
//                {
//                    _value = value.Trim();
//                    PropertyHasChanged();
//                }
//            }
//        }

//        public override bool IsValid
//        {
//            [COEUserActionDescription("ValidatePercentage")]
//            get {
//                try
//                {
//                    return base.IsValid && this.ValidatePercentages(); 
//                }
//                catch (Exception exception)
//                {
//                    COEExceptionDispatcher.HandleBLLException(exception);
//                    return false;
//                }
//            }
//        }

//        private bool ValidatePercentages()
//        {
//            //double percentage;
//            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
//            Regex regEx = new Regex(@"^\d{0,3}([" + decimalSeparator + @"]\d{1,2})?$");

//            //if (_value.Length > 5 || !double.TryParse(_value, out percentage))
//            if (!regEx.IsMatch(_value))
//                throw new ValidationException("Invalid Percentage: Format '###" + decimalSeparator + "##'");

//            return true;
//        }

//        public override bool IsDirty
//        {
//            get { return base.IsDirty; }
//        }

//        public override string ToString()
//        {
//            return "Percentage " + this.DisplayKey;
//        }

//        [COEUserActionDescription("GetPercentageBrokenRules")]
//        public string[] GetAllBrokenRules()
//        {
//            try
//            {
//                this.ValidationRules.CheckRules();
//                List<string> brokenRulesList = new List<string>();

//                string[] brokenRulesArray = BrokenRulesCollection.ToArray();

//                for (int index = 0; index < brokenRulesArray.Length; index++ )
//                    brokenRulesArray[index] = this.ToString() + " - " + brokenRulesArray[index];


//                brokenRulesList.AddRange(brokenRulesArray);

//                return brokenRulesList.ToArray();
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        public override void Delete()
//        {
//            this.MarkDeleted();
//        }
//        #endregion

//        #region Validation Rules

//        protected override void AddBusinessRules()
//        {
//            //ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Value", 6));

//            if (_validationRuleList != null && _validationRuleList.Count > 0)
//            {
//                foreach (ValidationRule validationRule in _validationRuleList)
//                {
//                    if (validationRule.Name.ToUpper() == "REQUIRED")
//                    {
//                        ValidationRules.AddRule(CommonRules.StringRequired, this._value);
//                    }

//                    if (validationRule.Name.ToUpper() == "MAXLENGTH")
//                    {
//                        ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(this._value, validationRule.MaxLength));
//                    }

//                    if (validationRule.Name.ToUpper() == "RANGE")
//                    {
//                        ValidationRules.AddRule(CommonRules.MaxValue<double>, new CommonRules.MinValueRuleArgs<double>(this._value, double.Parse(validationRule.MAX)));
//                        ValidationRules.AddRule(CommonRules.MinValue<double>, new CommonRules.MinValueRuleArgs<double>(this._value, double.Parse(validationRule.MIN)));
//                    }

//                    if (validationRule.Name.ToUpper() == "CAS")
//                    {
//                        ValidationRules.AddRule(ValidationRulesFactory.CASFormatMatch, this._value);
//                        ValidationRules.AddRule(ValidationRulesFactory.CASCheckSum, this._value);
//                    }

//                    if (validationRule.Name.ToUpper() == "DATE")
//                    {
//                    }

//                    if (validationRule.Name.ToUpper() == "DATERANGE")
//                    {
//                    }


//                }
//            }
//        }

//        #endregion

//        #region Authorization Rules

//        protected override void AddAuthorizationRules()
//        {
//            AuthorizationRules.AllowWrite(
//              "Percentage", "ADD_IDENTIFIER");
//        }

//        public static bool CanAddObject()
//        {
//            return true;
//        }

//        public static bool CanGetObject()
//        {
//            return true;
//        }

//        public static bool CanDeleteObject()
//        {
//            return true;
//        }

//        public static bool CanEditObject()
//        {
//            return true;
//        }

//        #endregion

//        #region Factory Methods

//        public static Percentage NewPercentage()
//        {
//            Percentage percentage = new Percentage();
//            percentage.MarkNew();
//            percentage.MarkClean();
//            return percentage;
//        }

//        public static Percentage NewPercentage(int componentIndex, string regNumber, string percentage)
//        {
//            Percentage percentageItem = new Percentage();
//            percentageItem.ComponentIndex = componentIndex;
//            percentageItem.Value = percentage;
//            percentageItem._regNum = regNumber;
//            percentageItem.MarkNew();
//            percentageItem.MarkClean();
//            return percentageItem;
//        }
        
//        public static Percentage NewPercentage(Component component)
//        {
//            Percentage percentageItem = new Percentage();
//            percentageItem.ComponentIndex = component.ComponentIndex;
//            percentageItem.Value = "0";
//            percentageItem._regNum = component.Compound.RegNumber.RegNum;
//            percentageItem.MarkNew();
//            percentageItem.MarkClean();
//            return percentageItem;
//        }

//        public static Percentage NewPercentage(string xml, bool isNew, bool isClean)
//        {
//            return new Percentage(xml, isNew, isClean);
//        }
        
//        private Percentage()
//        {
//            //MarkAsChild(); 
//            this._value = string.Empty;
//            this._compoundId = -1;
//            this._componentIndex = -1;

//            this.MarkAsChild();
//        }

//        private Percentage(string xml, bool isNew, bool isClean) : this() {
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(xml);
//            XmlNode node = doc.SelectSingleNode("Percentage");
//            _value = node.InnerText != null ? node.InnerText : string.Empty;
//            _compoundId = ((node.Attributes["CompoundID"] != null) && 
//                            (node.Attributes["CompoundID"].Value != string.Empty))? int.Parse(node.Attributes["CompoundID"].Value) : -1;

//            _componentIndex = ((node.Attributes["ComponentIndex"] != null) &&
//                            (node.Attributes["ComponentIndex"].Value != string.Empty)) ? int.Parse(node.Attributes["ComponentIndex"].Value) : -1;

//            if (node.Attributes["ID"] != null && node.Attributes["ID"].Value.Trim() != string.Empty)
//                _id = int.Parse(node.Attributes["ID"].Value);

//            string propList = node.SelectSingleNode("ValidationRuleList") != null ? node.SelectSingleNode("ValidationRuleList").OuterXml : string.Empty;
//            if(!string.IsNullOrEmpty(propList)) {
//                _validationRuleList = ValidationRuleList.NewValidationRuleList(propList);
//            }

//            if (!isNew)
//                MarkOld();

//            if (isClean)
//                MarkClean();
//        }

//        [COEUserActionDescription("GetPercentage")]
//        public static Percentage GetPercentage(int id)
//        {
//            try
//            {
//                if (!CanGetObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to view a Property");
//                }
//                return DataPortal.Fetch<Percentage>(new Criteria(id));
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        protected static void DeletePercentage(int id)
//        {
//            if (!CanDeleteObject())
//            {
//                throw new System.Security.SecurityException("User not authorized to remove a Property");
//            }
//            DataPortal.Delete(new Criteria(id));
//        }

//        [COEUserActionDescription("SavePercentage")]
//        public override Percentage Save()
//        {
//            try
//            {
//                if (IsDeleted && !CanDeleteObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to remove a Property");
//                }
//                else if (IsNew && !CanAddObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to add a Property");
//                }
//                else if (!CanEditObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to update a Property");
//                }
//                return base.Save();
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
//        #endregion

//        #region Data Access

//        [Serializable()]
//        private class Criteria
//        {
//            private int _id;
//            public int Id
//            {
//                get { return _id; }
//            }

//            public Criteria(int id)
//            { _id = id; }
//        }

//        [RunLocal()]
//        private void DataPortal_Create(Criteria criteria)
//        {
//            _id = 1;
//            ValidationRules.CheckRules();
//        }

//        private void DataPortal_Fetch(Criteria criteria)
//        {
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        protected override void DataPortal_Insert()
//        {
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        protected override void DataPortal_Update()
//        {
//        }


//        [Transactional(TransactionalTypes.TransactionScope)]
//        protected override void DataPortal_DeleteSelf()
//        {
//            DataPortal_Delete(new Criteria(_id));
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        private void DataPortal_Delete(Criteria criteria)
//        {
//        }

//        #endregion

//        #region Xml

//        internal string UpdateSelf()
//        {
//            StringBuilder builder = new StringBuilder("");
//            builder.Append("<Percentage");
//            builder.AppendFormat(" ID='{0}'", _id);
//            if(_compoundId >= 0) 
//                builder.Append(" CompoundID=\"" + _compoundId + "\"");
//            builder.Append(" ComponentIndex=\"" + _componentIndex + "\"");
//            if(this.IsNew)
//                builder.Append(" insert=\"yes\"");
//            else if(this.IsDirty)
//                builder.Append(" update=\"yes\"");
//            builder.Append(">");
//            /*CultureInfo defaultCulture = CultureInfo.CreateSpecificCulture("en");*/
//            builder.Append(_value == string.Empty ? "0" : double.Parse(_value).ToString(NumberFormatInfo.InvariantInfo));
//            builder.Append("</Percentage>");

//            return builder.ToString();
//        }

//        #endregion
//    }
//}
