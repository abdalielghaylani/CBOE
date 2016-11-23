//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//using System.Xml;
//using System.Xml.XPath;
//using System.Data;
//using CambridgeSoft.COE.Registration;
//using CambridgeSoft.COE.Framework.Common;
//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Framework.COEConfigurationService;
//using Csla;
//using Csla.Data;
//using Csla.Validation;

//namespace CambridgeSoft.COE.Registration.Services.Types
//{
//    [Serializable()]
//    public class UtilizationList : BusinessListBase<UtilizationList, Utilization>
//    {
//        #region Business Methods


//        #endregion

//        #region Factory Methods

//        internal static UtilizationList NewUtilizationList()
//        {
//            return new UtilizationList();
//        }

//        [COEUserActionDescription("CreateUtilizationList")]
//        public static UtilizationList NewUtilizationList(string xml)
//        {
//            try
//            {
//                UtilizationList utilizationList = new UtilizationList();

//                XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));

//                XPathNavigator xNavigator = xDocument.CreateNavigator();

//                XPathNodeIterator xIterator = xNavigator.Select("UtilizationList/Utilization");
//                xIterator.MoveNext();

//                bool more = false;
//                do
//                {
//                    Utilization utilization = Utilization.NewUtilization(xIterator.Current.OuterXml);
//                    utilizationList.Add(utilization);
//                    more = xIterator.Current.MoveToNext();
//                } while (more);

//                return utilizationList;
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        /*internal static UtilizationList GetUtilizationList(SafeDataReader dr)
//        {
//            return new UtilizationList(dr);
//        }*/

//        private UtilizationList()
//        {
//            //MarkAsChild();
//        }

//        [COEUserActionDescription("GetUtilizationList")]
//        public static UtilizationList GetUtilizationList()
//        {
//            try
//            {
//                //if (!CanGetObject())
//                //{
//                //    throw new System.Security.SecurityException("User not authorized to view a UtilizationList");
//                //}
//                return DataPortal.Fetch<UtilizationList>(new Criteria());
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
//        protected class Criteria
//        {

//            public Criteria() { }

//        }

//        protected void DataPortal_Fetch(Criteria criteria)
//        {
//        }

//        private void Fetch(SafeDataReader reader)
//        {
//        }


//        #endregion

//        #region Xml

//        internal string UpdateSelf()
//        {
//            StringBuilder builder = new StringBuilder("");

//            builder.Append("<UtilizationList>");

//            for (int i = 0; i < this.Count; i++)
//            {
//                builder.Append(this[i].UpdateSelf());
//            }

//            builder.Append("</UtilizationList>");

//            return builder.ToString();
//        }

//        #endregion
//    }
//}
