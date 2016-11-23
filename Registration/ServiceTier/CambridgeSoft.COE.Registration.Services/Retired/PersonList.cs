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
//    public class PersonList : BusinessListBase<PersonList, Person>
//    {
//        #region Business Methods


//        #endregion

//        #region Factory Methods

//        internal static PersonList NewPersonList()
//        {
//            return new PersonList();
//        }

//        [COEUserActionDescription("CreatePersonList")]
//        public static PersonList NewPersonList(string xml, bool isClean)
//        {
//            try
//            {
//                return new PersonList(xml, isClean);
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
        
//        private PersonList()
//        {
//            //MarkAsChild();
//        }

//        private PersonList(string xml, bool isClean) {
//            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
//            XPathNavigator xNavigator = xDocument.CreateNavigator();
//            XPathNodeIterator xIterator = xNavigator.Select("PersonList/Person");
//            xIterator.MoveNext();

//            bool more = false;
//            do {
//                this.Add(Person.NewPerson(xIterator.Current.OuterXml, isClean));
//                more = xIterator.Current.MoveToNext();
//            } while(more);

//        }
//        #endregion

//        #region Xml

//        internal string UpdateSelf()
//        {
//            StringBuilder builder = new StringBuilder("");

//            builder.Append("<PersonList>");

//            for (int i = 0; i < this.Count; i++)
//            {
//                builder.Append(this[i].UpdateSelf());
//            }

//            builder.Append("</PersonList>");

//            return builder.ToString();
//        }

//        #endregion

//    }
//}
