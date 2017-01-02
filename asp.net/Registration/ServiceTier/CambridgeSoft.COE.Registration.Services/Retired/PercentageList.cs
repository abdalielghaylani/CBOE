//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml.XPath;
//using Csla;
//using Csla.Validation;
//using System.IO;
//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Registration.Services.Properties;

//namespace CambridgeSoft.COE.Registration.Services.Types {
//    [Serializable()]
//    public class PercentageList : BusinessListBase<PercentageList, Percentage> {
//        #region Business Methods
//        public override bool IsValid {
//            get {
//                //CheckPercentages();
//                return base.IsValid;
//            }
//        }

//        private void CheckPercentages() {
//            double sum = 0;
//            try {
//                foreach(Percentage percent in this.Items) {
//                    sum += double.Parse(percent.Value);
//                }
//            } catch(Exception) {
//                throw new ValidationException(Resources.PercentagesInvalidValue);
//            }
//            if(sum != 100.00)
//                throw new ValidationException(Resources.PercentagesInvalidSum);
//        }

//        [COEUserActionDescription("GetPercentageListBrokenRules")]
//        public string[] GetAllBrokenRules()
//        {
//            try
//            {
//                List<string> brokenRulesList = new List<string>();

//                foreach (Percentage currentObject in this)
//                {
//                    brokenRulesList.AddRange(currentObject.GetAllBrokenRules());
//                }

//                return brokenRulesList.ToArray();
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
//        #endregion

//        #region Factory Methods

//        [COEUserActionDescription("CreatePercentageList")]
//        public static PercentageList NewPercentageList() {
//            try
//            {
//                return new PercentageList();
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        [COEUserActionDescription("CreatePercentageList")]
//        public static PercentageList NewPercentageList(string xml, bool isNew, bool isClean) {
//            try
//            {
//                return new PercentageList(xml, isNew, isClean);
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        private PercentageList() {
//            //MarkAsChild();
//        }

//        private PercentageList(string xml, bool isNew, bool isClean) {
//            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
//            XPathNavigator xNavigator = xDocument.CreateNavigator();
//            XPathNodeIterator xIterator = xNavigator.Select("PercentageList/Percentage");

//            if (xIterator.MoveNext())
//            {
//                do
//                {
//                    this.Add(Percentage.NewPercentage(xIterator.Current.OuterXml, isNew, isClean));
//                } while (xIterator.Current.MoveToNext());
//            }
//        }

//        [COEUserActionDescription("GetPercentageList")]
//        public static PercentageList GetPercentageList() {
//            try
//            {
//                //if (!CanGetObject())
//                //{
//                //    throw new System.Security.SecurityException("User not authorized to view a PropertyList");
//                //}
//                return DataPortal.Fetch<PercentageList>(new Criteria());
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
//        protected class Criteria {

//            public Criteria() { }

//        }

//        protected void DataPortal_Fetch(Criteria criteria) {
//        }

//        #endregion

//        #region Xml

//        internal string UpdateSelf() {
//            StringBuilder builder = new StringBuilder("");

//            builder.Append("<PercentageList>");

//            for(int i = 0; i < this.Count; i++) {
//                builder.Append(this[i].UpdateSelf());
//            }

//            builder.Append("</PercentageList>");

//            return builder.ToString();
//        }

//        #endregion

//    }
//}