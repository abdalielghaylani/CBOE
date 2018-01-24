using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{


    /// <summary>
    /// Overall configuration settings for ChemOffice Enterprise Services
    /// </summary>    
    /// 
    [Serializable]
    public class ApplicationHome : COEConfigurationElement
    {

        private const string _groups = "groups";
        private const string _gridColumns = "gridColumns";

        public ApplicationHome()
        {
        }


        /// <summary>
        /// Defines the base type used for loading services
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("number of columns to display")]
        [Category("Display")]
        [DisplayName("Columns")]
        [DefaultValue("4")]
        [ConfigurationProperty(_gridColumns, IsRequired = false)]
        public string GridColumns
        {
            get { return (string)base[_gridColumns]; }
            set { base[_gridColumns] = value; }
        }



        /// <summary>
        /// Defines the base type used for loading groups
        /// </summary>

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("enties containing cboe application data")]
        [Category("Items")]
        [DisplayName("Applications")]
        [ConfigurationProperty(_groups, IsRequired = false)]
        public COENamedElementCollection<Group> Groups
        {
            get { return (COENamedElementCollection<Group>)base[_groups]; }
        }


     
    }
}
