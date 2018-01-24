using System;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// &lt;reportTemplateGenerators&gt;
    ///   &lt;reportTemplateGenerator&gt;
    ///     &lt;name&gt;List View&lt;/name&gt;
    ///     &lt;class&gt;CambridgeSoft.COE.Framework.COEReportingService.Builders.ListViewTemplateBuilder&lt;/class&gt;
    ///   &lt;/reportTemplateGenerator&gt;
    /// &lt;/reportTemplateGenerators&gt;
    /// </summary>
    [Serializable]
    public class COEReportingConfiguration : COENamedConfigurationElement
    {
        private const string ReportBuilderMetaName = "name";
        private const string ReportBuilderMetaDescription = "description";
        private const string ReportBuilderMetaClass = "class";

        [ConfigurationProperty(ReportBuilderMetaName, IsRequired = true)]
        public string Name
        {
            get { return (string)base[ReportBuilderMetaName]; }
            set { base[ReportBuilderMetaName] = value; }
        }

        [ConfigurationProperty(ReportBuilderMetaDescription, IsRequired = true)]
        public string Description
        {
            get { return (string)base[ReportBuilderMetaDescription]; }
            set { base[ReportBuilderMetaDescription] = value; }
        }

        [ConfigurationProperty(ReportBuilderMetaClass, IsRequired = true)]
        public string Class
        {
            get { return (string)base[ReportBuilderMetaClass]; }
            set { base[ReportBuilderMetaClass] = value; }
        }
    }
}
