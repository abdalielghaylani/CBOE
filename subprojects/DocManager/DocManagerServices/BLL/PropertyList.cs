using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Csla;
using Csla.Data;
using Csla.Validation;

using System.Configuration;
using System.ComponentModel;


namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
    public class PropertyList : BusinessListBase<PropertyList, Property>
    {
        #region Properties

        #endregion

        #region Business Methods

        public int GetPropertyIndex(string name)
        {
            int index = 0;

            while (this[index].Name != name)
            {
                index++;
            }

            return index;
        }

        public void ChangeOrder(int toIndex, int fromIndex, string propertyName)
        {
            if (toIndex < 0 || toIndex >= this.Count)
            {
                throw new Exception("Out off range");
            }
            else
            {
                foreach (Property prop in this)
                {
                    if (prop.SortOrder == toIndex)
                        prop.SortOrder = fromIndex;
                }
                this[propertyName].SortOrder = toIndex;
            }
        }

        public void AddProperty(Property property)
        {
            if (property.Name == string.Empty || property.Type == string.Empty)
            {
                throw new Exception("You must insert Property Name and Type");
            }
            else
            {
                if (!this.CheckExistingNames(property))
                    if (property.SortOrder == -1)
                    {
                        if (this.Count == 0)
                            property.SortOrder = 0;
                        else
                            property.SortOrder = this.Count;
                    }
                this.Add(property);
            }
        }

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        public bool CheckExistingNames(Property property)
        {
            bool existence = false;
            foreach (Property prop in this)
            {
                if (prop.Name == property.Name)
                {
                    existence = true;
                }
            }

            return existence;
        }

        public Property this[string propertyName]
        {
            get
            {
                foreach (Property currentProperty in this)
                    if (currentProperty.Name == propertyName.Trim())
                        return currentProperty;

                return null;
            }
        }

        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            foreach (Property currentProperty in this)
                currentProperty.GetBrokenRulesDescription(brokenRules);
        }

        public List<Property> GetDeletedList()
        {
            return this.DeletedList;
        }

        public void ClearDeletedList()
        {
            this.DeletedList.Clear();
        }

        public PropertyList GetSortedPropertyList()
        {
            PropertyList tempPropertyList = PropertyList.NewPropertyList();

            for (int i = 0; i < this.Count; i++)
            {
                foreach (Property prop in this)
                {
                    if (prop.SortOrder == i)
                        tempPropertyList.AddProperty(prop);
                }
            }

            foreach (Property prop in this.DeletedList)
                tempPropertyList.DeletedList.Add(prop);

            return tempPropertyList;

        }

        #endregion

        #region Factory Methods

        public static PropertyList NewPropertyList()
        {
            return new PropertyList();
        }

        public static PropertyList NewPropertyList(string xml, bool isClean)
        {
            return new PropertyList(xml, isClean);
        }

        private PropertyList()
        {
            MarkAsChild();
        }

        protected override void RemoveItem(int index)
        {
            int deletedSortOrder = this[index].SortOrder;

            this[index].Delete();

            if (!this[index].IsNew)
            {
                DeletedList.Add(this[index]);
            }
            this.Items.RemoveAt(index);

            foreach (Property prop in this)
            {
                if (!(deletedSortOrder > this.Count))
                {
                    if (prop.SortOrder > deletedSortOrder)
                        prop.SortOrder--;
                }
            }


        }

        private PropertyList(string xml, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("Properties/Property");

            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Property.NewProperty(xIterator.Current.OuterXml, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        public static PropertyList GetPropertyList()
        {
            //if (!CanGetObject())
            //{
            //    throw new System.Security.SecurityException("User not authorized to view a PropertyList");
            //}
            return DataPortal.Fetch<PropertyList>(new Criteria());
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

            //        cmd.CommandText = "Select ID, IDENTIFIER_TYPE, IDENTIFIER_DESCRIPTOR from Propertys";

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
                //Property property = Property.NewProperty();
                //property.ID = reader.GetInt32("ID");
                //property.Type = reader.GetInt32("IDENTIFIER_TYPE");
                //property.Description = reader.GetString("IDENTIFIER_DESCRIPTOR");

                //this.Add(property);
            }
        }

        #endregion

        #region Xml

        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<Properties>");

            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i].UpdateSelf());
            }

			builder.Append("</Properties>");

            return builder.ToString();
        }

        public string UpdateSelfConfig()
        {
            StringBuilder builder = new StringBuilder("");
			builder.Append("<Properties>");
            foreach (Property prop in this)
            {
                builder.Append(prop.UpdateSelf());
            }
            foreach (Property delprop in DeletedList)
            {
                if (!delprop.IsNew)
                    builder.Append(delprop.UpdateSelf());
            }
			builder.Append("</Properties>");
            return builder.ToString();
        }

        #endregion


    }
}
