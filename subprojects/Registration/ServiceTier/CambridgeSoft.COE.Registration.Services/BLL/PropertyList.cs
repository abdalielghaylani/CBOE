using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Properties;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class PropertyList : RegistrationBusinessListBase<PropertyList, Property>
    {
        #region [ Administrative Only - NEEDS REFACTOR]

        /// <summary>
        /// Creates a new configuration property; for administrative use only.
        /// </summary>
        /// <remarks>
        /// This should be moved to an administrative version of the object specifically
        /// for use by the Administrative tools.
        /// </remarks>
        /// <param name="property"></param>
        [COEUserActionDescription("AddProperty")]
        public void AddProperty(Property property)
        {
            try
            {
                if (property.Name == string.Empty || property.Type == string.Empty)
                {
                    throw new ValidationException(Resources.PropertyNameAndTypeRequired);
                }
                else
                {
                    if (!this.CheckExistingNames(property.Name, false))
                    {
                        if (property.SortOrder != -1)
                        {
                            foreach (Property prop in this)
                            {
                                if (prop.SortOrder >= property.SortOrder)
                                    prop.SortOrder += 1;
                            }
                        }
                        else
                        {
                            if (this.Count != 0)
                                property.SortOrder = this.Count;
                            else
                                property.SortOrder = 0;
                        }

                        this.Add(property);
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// This is for administrative use only.
        /// </summary>
        /// <remarks>
        /// This is a data-container for public consumption, and this administrative method doesn't
        /// belong in this object. A separate, configuration-only version of PropertyList should
        /// be used for administrative use.
        /// </remarks>
        /// <param name="toIndex"></param>
        /// <param name="direction"></param>
        /// <param name="propertyName"></param>
        /// <returns>The name of the property whose sort order was affected as a consequence
        /// of the original order change request.</returns>
        [COEUserActionDescription("ReorderPropertyList")]
        public string ChangeOrder(int toIndex, bool direction, string propertyName)
        {
            string affectedPropertyName = string.Empty;

            try
            {
                if (toIndex < 0 || toIndex >= this.Count)
                {
                    throw new ValidationException("Index Out of range");
                }
                else
                {
                    if (direction && this[propertyName].SortOrder != 0)
                    {
                        foreach (Property prop in this)
                        {
                            if (prop.SortOrder == (this[propertyName].SortOrder - 1))
                            {
                                prop.SortOrder += 1;
                                affectedPropertyName = prop.Name;
                            }
                        }
                        this[propertyName].SortOrder = toIndex;
                    }
                    else if (!direction && this[propertyName].SortOrder != this.Count - 1)
                    {
                        foreach (Property prop in this)
                        {
                            if (prop.SortOrder == (this[propertyName].SortOrder + 1))
                            {
                                prop.SortOrder -= 1;
                                affectedPropertyName = prop.Name;
                            }
                        }
                        this[propertyName].SortOrder = toIndex;
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }

            return affectedPropertyName;
        }

        public bool CheckExistingNames(string propertyName, bool checkDeleted)
        {
            bool existence = false;

            foreach (Property prop in this)
            {
                if (prop.Name == propertyName)
                {
                    existence = true;
                }
            }
            if (checkDeleted)
            {
                foreach (Property delProp in this.DeletedList)
                {
                    if (delProp.Name == propertyName)
                    {
                        existence = true;
                    }
                }
            }

            return existence;
        }

        public void ClearDeletedList()
        {
            this.DeletedList.Clear();
        }

        public List<Property> GetDeletedList()
        {
            return this.DeletedList;
        }

        /// <summary>
        /// Get a copy of current instance, including DeletedList
        /// </summary>
        /// <returns></returns>
        [COEUserActionDescription("GetSortedPropertyList")]
        public PropertyList GetSortedPropertyList()
        {
            try
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
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }        

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        public string UpdateSelfConfig()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<PropertyList>");
            foreach (Property prop in this)
            {
                builder.Append(prop.UpdateSelf(true));
            }
            foreach (Property delprop in DeletedList)
            {
                if (!delprop.IsNew)
                    builder.Append(delprop.UpdateSelf(true));
            }
            builder.Append("</PropertyList>");
            return builder.ToString();
        }

        #endregion

        /// <summary>
        /// Returns the name-indexed Property object from the list
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Extracts explanatory text from Csla validation rules that are currently broken.
        /// </summary>
        /// <param name="brokenRules">the collection of broken rule objects</param>
        [COEUserActionDescription("GetPropertyListBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                foreach (Property currentProperty in this)
                    currentProperty.GetBrokenRulesDescription(brokenRules);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        public int GetPropertyIndex(string name)
        {
            int index = 0;

            foreach (Property prop in this)
            {
                if (prop.Name.ToLower() == name.ToLower())
                    return index;
                else
                    index++;
            }

            return -1;
        }

        [COEUserActionDescription("CreatePropertyList")]
        public static PropertyList NewPropertyList()
        {
            try
            {
                return new PropertyList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
        private PropertyList()
        {
            MarkAsChild();
        }

        [COEUserActionDescription("CreatePropertyList")]
        public static PropertyList NewPropertyList(string xml, bool isClean)
        {
            try
            {
                return new PropertyList(xml, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
        private PropertyList(string xml, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("PropertyList/Property");

            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Property.NewProperty(xIterator.Current.OuterXml, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        /// <summary>
        /// Moves an item from the public list to the private deleted list.
        /// </summary>
        /// <param name="index">the index of the property object to remove</param>
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

        /// <summary>
        /// Allows updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the PropertyList node</param>
        internal void UpdateFromXml(XmlNode parentNode)
        {
            if (parentNode != null)
            {
                foreach (Property p in this)
                {
                    XmlNode matchingChild =
                        parentNode.SelectSingleNode(string.Format("Property[@name='{0}']", p.Name));

                    if (matchingChild != null)
                    {
                        p.UpdateFromXml(matchingChild);
                    }
                }
            }
        }

        /// <summary>
        /// Updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the PropertyList node</param>
        internal void UpdateUserPreference(XmlNode parentNode)
        {
            if (parentNode != null)
            {
                foreach (Property p in this)
                {
                    XmlNode matchingChild =
                        parentNode.SelectSingleNode(string.Format("Property[@name='{0}']", p.Name));

                    if (matchingChild != null)
                    {
                        p.UpdateUserPreference(matchingChild);
                    }
                }
            }
        }

        /// <summary>
        /// Exposed for the purpose of 
        /// </summary>
        /// <param name="addCRUDattributes">
        /// use true to include state-change attributes in the xml
        /// </param>
        /// <returns>an xml string</returns>
        public string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<PropertyList>");

            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i].UpdateSelf(addCRUDattributes));
            }

            builder.Append("</PropertyList>");

            return builder.ToString();
        }

    }
}
