using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;

using Csla;
using Csla.Data;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Services.Types
{

    /// <summary>
    /// This is NOT really a 'business object'.
    /// </summary>
    [Serializable()]
    public class CompoundList : RegistrationBusinessListBase<CompoundList, Compound>
    {
        public new void Remove(Compound compound){
            this.Items.Remove(compound);
        }

        [COEUserActionDescription("CreateCompoundList")]
        public static CompoundList NewCompoundList()
        {
            try
            {
                return new CompoundList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateCompoundList")]
        public static CompoundList NewCompoundList(string xml, bool isNew, bool isClean)
        {
            try
            {
                CompoundList compoundList = new CompoundList(xml, isNew, isClean);
                return compoundList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
        
        private CompoundList()
        {
        }

        private CompoundList(string xml, bool isNew, bool isClean) {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("CompoundList/Compound");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Compound.NewCompound(xIterator.Current.OuterXml, isNew, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        /// <summary>
        /// Method to compare two objects and know the index inside the collection.
        /// IndexOf was not overloaded because is used for the removeAt call.
        /// </summary>
        /// <param name="component">Component to compare</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("GetCompoundIndex")]
        public int GetIndex(Compound compound)
        {
            int retVal = 0;
            try
            {
                if (this.Contains(compound))
                {
                    foreach (Compound currentCompound in this)
                    {
                        if (currentCompound.UniqueID == compound.UniqueID)
                            break;
                        retVal++;
                    }
                }
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return retVal;
            }
        }

        [COEUserActionDescription("GetCompoundList")]
        public static CompoundList GetCompoundList(string filter) {
            try
            {
                Criteria criteria = new Criteria(filter);
                return DataPortal.Fetch<CompoundList>(criteria);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [Serializable()]
        protected class Criteria
        {
            private string _filter;
            public string Filter {
                get { return _filter; }
                set { _filter = value; }
            }
            public Criteria(string filter) {
                _filter = filter;
            }
        }

        protected void DataPortal_Fetch(Criteria criteria)
        {
            RaiseListChangedEvents = false;
            //this.IsReadOnly = false;

            string xml = this.RegDal.GetCompoundList(criteria.Filter);

            this.LoadFromXml(xml);

            XmlDocument document = new XmlDocument(); 
            document.LoadXml(criteria.Filter);
            foreach (Compound currentCompound in this)
            {
                if(!string.IsNullOrEmpty(currentCompound.RegNumber.RegNum))
                    currentCompound.BaseFragment.Structure.IsTemporary = false;

                XmlNode currentCompoundNode = document.SelectSingleNode(string.Format("//REGNUMBER[text()='{0}']", currentCompound.RegNumber.RegNum));
                if(currentCompoundNode.Attributes["count"] != null)
                    currentCompound.SingleCompoundCount = int.Parse(currentCompoundNode.Attributes["count"].Value);
            }
            //IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        private void LoadFromXml(string xml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            foreach(XmlNode node in doc.SelectNodes("CompoundList/Compound")){
                //Get registry 0 (template) instead of newregistryrecord so that it's marked as old.
                Compound compound = Compound.NewCompound(node.OuterXml, false, false);
                this.Add(compound);
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<CompoundList>");

            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i].UpdateSelf(addCRUDattributes));
            }

            builder.Append("</CompoundList>");

            return builder.ToString();
        }

    }
}
