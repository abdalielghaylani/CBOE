using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors {
    public abstract class BaseCustomProcessor : SearchProcessor
    {
        #region Constructors - Hidden
        // Hiding constructor by making it private. Required at least one constructor because base class does not have parameterless/default constructor.
        // Use Factory Method instead (CreateCustomProcessor)
        private BaseCustomProcessor(XmlNode node) : base(node) { }
        private BaseCustomProcessor(SearchCriteria.SearchCriteriaItem item) : base(item) { }
        #endregion

        #region Factory Method
        public static SearchProcessor CreateCustomProcessor(SearchCriteria.SearchCriteriaItem customCriteriaItem) {
            string xmlNamespace = "COE";
            SearchProcessor processor = new GenericProcessor(customCriteriaItem);
            XmlDocument mappings = new XmlDocument();
            mappings.LoadXml(ConfigurationUtilities.GetMappingsXml());
            XmlNamespaceManager manager = new XmlNamespaceManager(mappings.NameTable);
            manager.AddNamespace(xmlNamespace, "COE.Mappings");
            
            if(customCriteriaItem.Criterium is SearchCriteria.CustomCriteria) {
                SearchCriteria.CustomCriteria customCriteria = (SearchCriteria.CustomCriteria) customCriteriaItem.Criterium;

                try {
                    XmlNode node;
                    node = mappings.SelectSingleNode("//" + xmlNamespace + ":whereClause[@name='" + customCriteria.Custom.Name.Trim().ToLower() + "']", manager);

                    if(node != null && node.NodeType != XmlNodeType.Comment) {
                        string searchProcessorClassName = string.Empty;
                        if(node.Attributes["searchProcessorClassName"] != null && !string.IsNullOrEmpty(node.Attributes["searchProcessorClassName"].Value))
                            searchProcessorClassName = node.Attributes["searchProcessorClassName"].Value;
                        
                        string assemblyFullName = string.Empty;
                        if(node.Attributes["assemblyName"] != null && !string.IsNullOrEmpty(node.Attributes["assemblyName"].Value))
                            assemblyFullName = node.Attributes["assemblyName"].Value;

                        if(!string.IsNullOrEmpty(searchProcessorClassName)) {
                            if(!string.IsNullOrEmpty(assemblyFullName)) {
                                System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assemblyFullName);
                                processor = (SearchProcessor) assembly.CreateInstance(searchProcessorClassName);
                            } else {
                                Type processorType = Type.GetType(searchProcessorClassName);
                                if(processorType == null)
                                    throw new Exception();

                                System.Reflection.ConstructorInfo processorDefaultConstructor = processorType.GetConstructor(System.Type.EmptyTypes);
                                if(processorDefaultConstructor == null)
                                    throw new Exception();

                                processor = (SearchProcessor) processorDefaultConstructor.Invoke(null);
                            }
                        }
                    } else
                        throw new SQLGeneratorException(Resources.WhereClauseNotFound + " " + customCriteria.Custom.Name.Trim());
                } catch(System.Xml.XPath.XPathException) {
                    throw new SQLGeneratorException(Resources.WhereClauseNotSupported + " " + customCriteria.Custom.Name.Trim());
                } catch(InvalidCastException) {
                    throw new SQLGeneratorException(Resources.WhereParserNotImplemented.Replace("&clauseName", customCriteria.Custom.Name.Trim()));
                }
            }
            return processor;
        }
        #endregion
    }
}
