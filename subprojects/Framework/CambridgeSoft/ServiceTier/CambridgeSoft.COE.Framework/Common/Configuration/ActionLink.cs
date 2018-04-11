using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    public class ActionLink : COEConfigurationElement
    {
        private const string id = "id";
        [ConfigurationProperty(id, IsRequired = true, IsKey=true)]
        public string Id
        {
            get { return (string) base[id]; }
            set { base[id] = value; }
        }

        private const string href = "href";
        [ConfigurationProperty(href, IsRequired = false)]
        public string HRef
        {
            get { return (string) base[href]; }
            set { base[href] = value; }
        }

        private const string text = "text";
        [ConfigurationProperty(text, IsRequired = false)]
        public string Text
        {
            get { return (string) base[text]; }
            set { base[text] = value; }
        }

        private const string target = "target";
        [ConfigurationProperty(target, IsRequired = false)]
        public string Target
        {
            get { return (string) base[target]; }
            set { base[target] = value; }
        }

        private const string tooltip = "tooltip";
        [ConfigurationProperty(tooltip, IsRequired = false)]
        public string Tooltip
        {
            get { return (string) base[tooltip]; }
            set { base[tooltip] = value; }
        }

        private const string cssClass = "cssClass";
        [ConfigurationProperty(cssClass, IsRequired = false)]
        public string CssClass
        {
            get { return (string) base[cssClass]; }
            set { base[cssClass] = value; }
        }

        private const string enabled = "enabled";
        [ConfigurationProperty(enabled, IsRequired = false, DefaultValue=true)]
        public bool Enabled
        {
            get { return (bool) base[enabled]; }
            set { base[enabled] = value; }
        }

        private const string privileges = "privileges";
        [ConfigurationProperty(privileges, IsRequired = false)]
        public string Privileges
        {
            get { return (string) base[privileges]; }
            set { base[privileges] = value; }
        }

        /// <summary>
        /// Sets the confirmation message to display when the action link is clicked
        /// </summary>
        private const string confirmationMessage = "confirmationMessage";
        [ConfigurationProperty(confirmationMessage, IsRequired = false)]
        public string ConfirmationMessage
        {
            get { return (string)base[confirmationMessage]; }
            set { base[confirmationMessage] = value; }
        }
    }

    public class ActionLinkCollection : COEConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new xml data element.
        /// </summary>
        /// <returns>The configuration element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ActionLink();
        }

        /// <summary>
        /// Gets an element by its id.
        /// </summary>
        /// <param name="element">The element with the key.</param>
        /// <returns>A string with the element's id</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ActionLink) element).Id;
        }

        /// <summary>
        /// Indexer for ActionLink by its index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The <see cref="ActionLink"/></returns>
        public ActionLink this[int index]
        {
            get { return (ActionLink) base.BaseGet(index); }
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
        public ActionLink this[string id]
        {
            get { return (ActionLink) base.BaseGet(id); }
        }
    }
}
