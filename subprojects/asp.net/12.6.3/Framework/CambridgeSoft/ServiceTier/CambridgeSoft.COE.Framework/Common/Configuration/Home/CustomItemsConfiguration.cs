using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
   
     [Serializable]
    public class CustomItemConfiguration : NameValueConfigurationElement
    {
        private const string _name = "name";
        private const string _value = "value";
        

       

        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditorData"/> class.
        /// </summary>
        public CustomItemConfiguration()
        {
        }

        /// <summary>
        /// Name that uniquely identifies the configuration entry)
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("name that uniquely identifies the configuration entry")]
        [Category("Dash")]
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
        [Description("privilege required to show a dashboard item")]
        [Category("Dash")]
        [DisplayName("Value")]
        [ConfigurationProperty(_value, IsRequired = false)]
        public string Value
        {
            get { return (string)base[_value]; }
            set { base[_value] = value; }
        }

      
}
}
