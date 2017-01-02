using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the configuration data for the TableEditor's each Element that has been added inside each application
    /// properties can be added as needed. Each propery will be added to the add element in the TableEditor parent element
    /// <code lang="Xml">
   
    ///     &lt;LinkPermission&gt;
    ///         &lt;add name="Registration"&gt;
    ///             &lt;linkParameter&gt;
    ///                     &lt;add name"ID" privilege="" URL="xx.com" display="linktext"&gt linkIcon="icon_32.png"&gt linkIconCSS="icon_32"&gt linkIconBckCSS="blue_32"&gt;
    ///             &lt;/linkParameter&gt;
    ///         &lt;/add&gt;
    ///     &lt;/LinkPermission&gt;
    /// </code>
    /// </summary>
    /// 
     [Serializable]
    public class linkParameters : COENamedConfigurationElement
    {
        private const string _name = "name";
        private const string _formGroupId = "formGroupId";
        //private const string _url = "url";
        private const string _searchCriteriaId = "searchCriteriaId";
       

        /// <summary>
        /// Initialize a new instance of the <see cref="LinkParameterData"/> class.
        /// </summary>
        public linkParameters()
        {
        }

        /// <summary>
        /// Name that uniquely identifies the configuration entry)
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("name that uniquely identifies the configuration entry")]
        [Category("linkParameters")]
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
        [Description("formGroupId required to show a link")]
        [Category("linkParameters")]
        [DisplayName("FormGroupId")]
        [ConfigurationProperty(_formGroupId, IsRequired = false)]
        public string FormGroupId
        {
            get { return (string)base[_formGroupId]; }
            set { base[_formGroupId] = value; }
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
        [Description("text to searchCriteria for link")]
        [Category("linkParameters")]
        [DisplayName("Search CriteriaId")]
        [ConfigurationProperty(_searchCriteriaId, IsRequired = true)]
        public string SearchCriteriaId
        {
            get { return (string)base[_searchCriteriaId]; }
            set { base[_searchCriteriaId] = value; }
        }
}
}
