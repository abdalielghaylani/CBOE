using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    public class InnerXmlData : COENamedConfigurationElement
    {
        private const string _name = "name";
        private const string _value = "value";
        //private const string _url = "url";
        private const string _display = "display";


        /// <summary>
        /// Initialize a new instance of the <see cref="LinkParameterData"/> class.
        /// </summary>
        public InnerXmlData()
        {
        }

        /// <summary>
        /// Name that uniquely identifies the configuration entry)
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("name that uniquely identifies the configuration entry")]
        [Category("activeCases")]
        [DisplayName("Name")]
        [ConfigurationProperty(_name, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_name]; }
            set { base[_name] = value; }
        }

        /// <summary>
        /// privilege required to show a link
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("activeCases's value")]
        [Category("activeCases")]
        [DisplayName("Value")]
        [ConfigurationProperty(_value, IsRequired = false)]
        public string Value
        {
            get { return (string)base[_value]; }
            set { base[_value] = value; }
        }

        ///// <summary>
        ///// URL for the link)
        ///// </summary>
        //[Browsable(true)]
        //[ReadOnly(false)]
        //[Description("url for the link")]
        //[Category("linkParameters")]
        //[DisplayName("url")]
        //[ConfigurationProperty(_url, IsRequired = false)]
        //public string URL
        //{
        //    get { return (string)base[_url]; }
        //    set { base[_url] = value; }
        //}

        /// <summary>
        /// text to search for link)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("display of the value in web page")]
        [Category("activeCases")]
        [DisplayName("Display")]
        [ConfigurationProperty(_display, IsRequired = true)]
        public string Display
        {
            get { return (string)base[_display]; }
            set { base[_display] = value; }
        }
    }
}
