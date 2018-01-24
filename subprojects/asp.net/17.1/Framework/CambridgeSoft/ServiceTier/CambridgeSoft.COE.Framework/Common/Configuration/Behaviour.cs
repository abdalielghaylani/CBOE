using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace CambridgeSoft.COE.Framework.Common
{
    public class Behaviour : COEConfigurationElement
    {
        private const string formId = "formId";
        [ConfigurationProperty(formId, IsRequired = false)]
        public string FormId
        {
            get { return (string) base[formId]; }
            set { base[formId] = value; }
        }

        private const string generalOptions = "generalOptions";
        [ConfigurationProperty(generalOptions, IsRequired = false)]
        public COENamedElementCollection<NameValueConfigurationElement> GeneralOptions
        {
            get { return (COENamedElementCollection<NameValueConfigurationElement>)base[generalOptions]; }
            set { base[generalOptions] = value; }
        }

        private const string chemDrawOptions = "chemDrawOptions";
        [ConfigurationProperty(chemDrawOptions, IsRequired = false)]
        public ChemDrawOptions ChemDrawOptions
        {
            get { return (ChemDrawOptions) base[chemDrawOptions]; }
            set { base[chemDrawOptions] = value; }
        }

        private const string leftPanelOptions = "leftPanelOptions";
        [ConfigurationProperty(leftPanelOptions, IsRequired = false)]
        public LeftPanelOptions LeftPanelOptions
        {
            get { return (LeftPanelOptions) base[leftPanelOptions]; }
            set { base[leftPanelOptions] = value; }
        }

        private const string menuOptions = "menuOptions";
        [ConfigurationProperty(menuOptions, IsRequired = false)]
        public MenuOptions MenuOptions
        {
            get { return (MenuOptions) base[menuOptions]; }
            set { base[menuOptions] = value; }
        }

        private const string actionLinks = "actionLinks";
        [ConfigurationProperty(actionLinks, IsRequired = false)]
        [ConfigurationCollection(typeof(ActionLink), AddItemName = "link")]
        public ActionLinkCollection ActionLinks
        {
            get { return (ActionLinkCollection) base[actionLinks]; }
            set { base[actionLinks] = value; }
        }
    }

    public class BehaviourCollection : COEConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new xml data element.
        /// </summary>
        /// <returns>The configuration element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Behaviour();
        }

        /// <summary>
        /// Gets an element by its id.
        /// </summary>
        /// <param name="element">The element with the key.</param>
        /// <returns>A string with the element's id</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Behaviour) element).FormId;
        }

        /// <summary>
        /// Indexer for ActionLink by its index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The <see cref="ActionLink"/></returns>
        public Behaviour this[int index]
        {
            get { return (Behaviour) base.BaseGet(index); }
            set
            {
                if(base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Indexer for ActionLink by its id
        /// </summary>
        /// <param name="index">The id</param>
        /// <returns>The <see cref="ActionLink"/></returns>
        public Behaviour this[string formId]
        {
            get { return (Behaviour) base.BaseGet(formId); }
        }
    }
    
}
