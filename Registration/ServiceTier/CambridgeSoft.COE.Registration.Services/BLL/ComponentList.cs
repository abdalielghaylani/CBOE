using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration.Services.Properties;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class ComponentList : BusinessListBase<ComponentList, Component>
    {

        #region [ Properties and members ]

        [NonSerialized]
        private FilteredBindingList<Component> _filteredList;

        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.CheckComponents();
            }
        }

        #endregion

        #region [ Factory Methods ]

        [COEUserActionDescription("CreateComponentList")]
        public static ComponentList NewComponentList(string xml, bool isNew, bool isClean)
        {
            try
            {
                ComponentList componentList = new ComponentList(xml, isNew, isClean);
                return componentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private ComponentList(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("ComponentList/Component");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Component.NewComponent(xIterator.Current.OuterXml, isNew, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        private ComponentList() { }

        #endregion

        [COEUserActionDescription("GetComponentListBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                foreach (Component currentComponent in this)
                    currentComponent.GetBrokenRulesDescription(brokenRules);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        private bool CheckComponents()
        {
            for (int index = 0; index < this.Count - 1; index++)
            {
                Component currentComponent = this[index];

                if (currentComponent.Compound.RegNumber.ID > 0)
                {
                    for (int indexToCheck = index + 1; indexToCheck < this.Count; indexToCheck++)
                    {
                        Component currentCompoundToCheck = this[indexToCheck];
                        if (currentComponent.Compound.RegNumber.ID == currentCompoundToCheck.Compound.RegNumber.ID)
                            throw new ValidationException(Resources.RepeatedComponent_ErrorMessage);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Since the individual components are all being managed via the master XML document
        /// sent to the database, this isn't necessary.
        /// </summary>
        internal void ForgetDeleted()
        {
            this.DeletedList.Clear();
        }

        [COEUserActionDescription("GetComponentFromList")]
        public Component GetComponentByRegNumber(string regNumber)
        {
            try
            {
                foreach (Component comp in this)
                {
                    if (comp.Compound.RegNumber.RegNum.ToUpper() == regNumber.ToUpper())
                        return comp;
                }
                return null;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// List-item replacement
        /// </summary>
        /// <param name="index">the index at which to perform the replacement</param>
        /// <param name="component">the item to use as the replacement</param>
        public void ReplaceAt(int index, Component component)
        {
            this.Items[index] = component;
        }

        /// <summary>
        /// Custom xml-based 'serialization' of this object's current state
        /// </summary>
        /// <param name="addCRUDattributes">true to record state-changes</param>
        /// <returns>an xml string representing this object and its children</returns>
        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<ComponentList>");

            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i].UpdateSelf(addCRUDattributes));
            }

            for (int i = 0; i < this.DeletedList.Count; i++)
            {
                if (!this.DeletedList[i].IsNew)
                    builder.Append(this.DeletedList[i].UpdateSelf(true));
            }

            builder.Append("</ComponentList>");

            return builder.ToString();
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            XmlNodeList componentNodes = parentNode.SelectNodes("//ComponentList/Component");
            XmlNodeList componentIndexNodes = parentNode.SelectNodes("//ComponentList/Component/ComponentIndex");

            //Add and update
            foreach (XmlNode componentNode in componentNodes)
            {

                XmlNode componentIndexNode = componentNode.SelectSingleNode("//Component/ComponentIndex");

                //The inner text of id node should be empty if it's retrieved from db.
                //so in this case, we should check the <componentIdex> element.
                //1.make sure this componentIdex exists.
                foreach (Component comp in this)
                {
                    if (comp.ComponentIndex == int.Parse(componentIndexNode.InnerText))
                    {
                        comp.UpdateFromXml(componentNode);
                    }
                }

            }
        }

        internal void UpdateUserPreference(XmlNode parentNode)
        {
            XmlNodeList componentNodes = parentNode.SelectNodes("//ComponentList/Component");
            XmlNodeList componentIndexNodes = parentNode.SelectNodes("//ComponentList/Component/ComponentIndex");

            //Add and update
            foreach (XmlNode componentNode in componentNodes)
            {

                XmlNode componentIndexNode = componentNode.SelectSingleNode("//Component/ComponentIndex");

                //The inner text of id node should be empty if it's retrieved from db.
                //so in this case, we should check the <componentIdex> element.
                //1.make sure this componentIdex exists.
                foreach (Component comp in this)
                {
                    if (comp.ComponentIndex == int.Parse(componentIndexNode.InnerText))
                    {
                        comp.UpdateUserPreference(componentNode);
                    }
                }

            }
        }

        /// <summary>
        /// Utility function to enable the retrieval of all BatchComponent instances that are
        /// related to the Component in question.
        /// </summary>
        /// <param name="record">a RegistryRecord instance</param>
        /// <param name="componentIndex">the Component.ComponentIndex property of the desired BatchComponents</param>
        /// <returns>a simple list of BatchComponent instances that may belong to seaperate Batch instances</returns>
        public static List<BatchComponent> GetBatchComponentsForComponent(RegistryRecord record, int componentIndex)
        {
            List<BatchComponent> matchingBatchComponents = new List<BatchComponent>();

            foreach (Batch batch in record.BatchList)
            {
                foreach (BatchComponent batchComponent in batch.BatchComponentList)
                {
                    if (batchComponent.ComponentIndex == componentIndex)
                        matchingBatchComponents.Add(batchComponent);
                }
            }

            return matchingBatchComponents;
        }


        //TODO: Document what these do and why we need them.
        //  Is there some reason to do CSLA-style list filtering? It's pretty cumbersome
        //  and I don't see much benefit.
        #region Filters required classes

        /// <summary>
        /// Classes to allow filtering over CSLA collections.
        /// </summary>
        private class ComponentFilters
        {
            public static bool GetByComponentIndex(object item, object filter)
            {
                int index = 0;
                Component field = null;
                //Filter is a int
                if (filter is int)
                    index = Convert.ToInt32((string)filter.ToString());
                //assume item is the object it self. (See string.empty parameter)
                if (item is Component)
                {
                    field = ((Component)item);
                    if (field != null && field.ComponentIndex == index) //Coverity Fix - CID 11697
                        return true;
                }
                return false;
            }
        }

        public Compound GetCompoundByID(int compoundID)
        {
            foreach (Component component in this)
            {
                if (component.Compound.ID == compoundID)
                    return component.Compound;
            }
            return null;
        }

        internal Component GetComponentByIndex(int index)
        {
            Component retVal = null;
            if (_filteredList == null) this.InstantiateFilters();
            _filteredList.RemoveFilter();//Just in case a previous filter has been applied
            _filteredList.FilterProvider = new Csla.FilterProvider(ComponentFilters.GetByComponentIndex);
            _filteredList.ApplyFilter(String.Empty, index);
            if (_filteredList.Count == 1)
                retVal = _filteredList[0];
            return retVal;
        }

        private void InstantiateFilters()
        {
            _filteredList = new FilteredBindingList<Component>(this);
        }

        #endregion

    }
}
