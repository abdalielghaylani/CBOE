using System;
using System.Xml;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// Creates the right subclass of a SelectClauseItem, based upon input parameters.
    /// </summary>
    public sealed class SelectClauseFactory
    {
        #region Methods
        

		/// <summary>
		/// Creates a SelectClause item, based on the results xml and the data view xml definitions.
		/// </summary>
		/// <param name="resultsXmlNode">The node from the search results xml that we are trying to instanciate.</param>
		/// <param name="dvnLookup">The data view object, needed in case of lookups of names.</param>
		/// <returns>A select clause item, instanced with the proper subclass.</returns>
		/// <remarks>It is necessary to determine how to load the items if they live in another assembly.</remarks>
		public static SelectClauseItem CreateSelectClauseItem(XmlNode resultsXmlNode, INamesLookup dvnLookup) {
			string xmlNamespace = "COE";
			SelectClauseItem item = null;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            XmlDocument mappings = new XmlDocument();

            if (currentDomain.GetData("mappingsXML") == null)
            {
                //JHS - This is very expensive.  The expense seems to be in the underlying configuration utility and not the loadxml.
                mappings.LoadXml(ConfigurationUtilities.GetMappingsXml());
                currentDomain.SetData("mappingsXML", mappings);
            }
            else
            {
                mappings = (XmlDocument)currentDomain.GetData("mappingsXML");
            }


			XmlNamespaceManager manager = new XmlNamespaceManager(mappings.NameTable);
			manager.AddNamespace(xmlNamespace, "COE.Mappings");

			try {
                XmlNode node;
                if (resultsXmlNode.Name.Trim().ToLower() != "custom")
                    node = mappings.SelectSingleNode("//" + xmlNamespace + ":selectClause[@name='" + resultsXmlNode.Name.Trim().ToLower() + "']", manager);
                else
                    node = mappings.SelectSingleNode("//" + xmlNamespace + ":selectClause[@name='" + resultsXmlNode.FirstChild.Name.Trim().ToLower() + "']", manager);

				if(node != null && node.NodeType != XmlNodeType.Comment) {
					ISelectClauseParser parser = null;
					if(node.Attributes["assemblyName"] != null && node.Attributes["assemblyName"].Value.Trim().ToLower() != "cambridgesoft.coe.core.common.sqlgenerator") {
						System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(node.Attributes["assemblyName"].Value.Trim());

						parser = (ISelectClauseParser)assembly.CreateInstance(node.Attributes["parserClassName"].Value.Trim());
					} else {
						Type parserClass = Type.GetType(node.Attributes["parserClassName"].Value.Trim());
						if(parserClass == null)
                            throw new SQLGeneratorException(Resources.ReflectionErrors.Replace("&clauseName",resultsXmlNode.Name.Trim()).Replace("&className",node.Attributes["parserClassName"].Value.Trim()));

						ConstructorInfo parserDefaultConstructor = parserClass.GetConstructor(System.Type.EmptyTypes);
						if(parserDefaultConstructor == null)
                            throw new SQLGeneratorException(Resources.SelectClauseWithoutDefaultConstructor.Replace("&className", resultsXmlNode.Name.Trim()));

						parser = (ISelectClauseParser)parserDefaultConstructor.Invoke(null);
					}

                    if(resultsXmlNode.Name.Trim().ToLower() != "custom")
                        item = ((ISelectClauseParser) parser).CreateInstance(resultsXmlNode, dvnLookup);
                        
                    else
                        item = ((ISelectClauseParser) parser).CreateInstance(resultsXmlNode.FirstChild, dvnLookup);

					return item;
				} else
					throw new SQLGeneratorException(Resources.SelectClauseNotFound + " " + resultsXmlNode.Name.Trim());
			} catch(System.Xml.XPath.XPathException) {
                throw new SQLGeneratorException(Resources.SelectClauseNotSupported + " " + resultsXmlNode.Name);
			} catch(InvalidCastException) {
                throw new SQLGeneratorException(Resources.ParserNotImplemented.Replace("&clauseName",resultsXmlNode.Name));
			}
		}

        #endregion
    }
}
