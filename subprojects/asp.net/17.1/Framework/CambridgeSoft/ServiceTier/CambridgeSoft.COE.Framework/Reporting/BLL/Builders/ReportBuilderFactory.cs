using System;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

namespace CambridgeSoft.COE.Framework.COEReportingService.Builders
{
    /// <summary>
    /// Creates a report builder based on the information provided.
    /// </summary>
    public class ReportBuilderFactory
    {
        #region Methods
        /// <summary>
        /// Obtains a report builder based upon the information provided. This report builder can then be used for generating a report (definition)
        /// </summary>
        /// <param name="reportBuilderMetaData">Information describing the desired report builder</param>
        /// <returns>A report builder wich can be used for generating a report (definition)</returns>
        internal static ReportBuilderBase GetReportBuilder(ReportBuilderMeta reportBuilderMetaData)
        {
            if (reportBuilderMetaData.Class.Equals(typeof(DataBaseReportBuilder).AssemblyQualifiedName))
                return new DataBaseReportBuilder(reportBuilderMetaData.Id);

            Type builderType = Type.GetType(reportBuilderMetaData.Class);

            if (builderType == null)
                throw new Exception(string.Format("Could not find assembly: {0}", reportBuilderMetaData.Class));

            System.Reflection.ConstructorInfo defaultConstructor = builderType.GetConstructor(System.Type.EmptyTypes);
            if (defaultConstructor == null)
                throw new Exception("Missing default constructor");

            ReportBuilderBase templateBuilder = (ReportBuilderBase)defaultConstructor.Invoke(null);

            if (!string.IsNullOrEmpty(reportBuilderMetaData.Config) && templateBuilder != null)
                BindConfigInfo(reportBuilderMetaData.Config, templateBuilder);

            return templateBuilder;
        }

        /// <summary>
        /// Parses an xml snippet and binds the nodes to properties of the builder.
        /// </summary>
        /// <param name="configInfoXml">The xml snippet containing the configuration information. Note that node names should match properties names of the report builder for the binding to succeed.</param>
        /// <param name="reportBuilder">The report builder that is to be configured</param>
        private static void BindConfigInfo(string configInfoXml, ReportBuilderBase reportBuilder)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(configInfoXml);

            COEDataBinder dataBinder = new COEDataBinder(reportBuilder);

            foreach (XmlNode currentNode in document.DocumentElement.ChildNodes)
            {
                foreach (String propertyName in GetPropertyAliases(currentNode.Name))
                {
                    if (dataBinder.ContainsProperty(propertyName))
                    {
                        dataBinder.SetProperty(propertyName, currentNode.InnerXml);
                        break;
                    }
                }
            }
        }

        private static string[] GetPropertyAliases(string propertyName)
        {
            string [] result = new string[3];

            if (propertyName.Trim().StartsWith("_"))
                propertyName = propertyName.Trim().Substring(1);

            result[0] = char.ToLower(propertyName[0]) + propertyName.Substring(1);
            result[1] = "_" + propertyName;
            result[2] = char.ToUpper(propertyName[0]) + propertyName.Substring(1); 

            return result;
        }
        #endregion
    }
}
