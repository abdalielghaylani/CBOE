using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using SingleSignOn.Properties;
using System.IO;
using System.Xml.Serialization;


namespace CambridgeSoft.COE.Security.Services
{


	public class SSOConfig
	{
		//C:\\Documents and Settings\\All Users\\Application Data
		//static readonly string configPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ChemOfficeEnterprise" + Resources.COEVersion + "\\" + Resources.SSOConfigFileName;
		//above changing to use values from web.config
		//also make it public so it can be used from CSSConnection or other future providers that need the path
		public static readonly string configPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ConfigurationManager.AppSettings["ConfigPath"].ToString() + "\\" + Resources.SSOConfigFileName;

		public static Configuration OpenConfig()
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap();
			map.ExeConfigFilename = configPath;
            if (File.Exists(configPath)){
			    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			    return config;
            }
            else{
                throw new ConfigurationErrorsException(string.Format("Invalid path in SingleSignOn web.config: {0}", configPath));
            }
		}
	}

	public class SSOConfigurationProvider : ConfigurationSection
	{
		public static SSOConfigurationProvider GetConfig()
		{
			return SSOConfig.OpenConfig().GetSection("SSOConfiguration/ProviderConfiguration") as SSOConfigurationProvider;

			//return ConfigurationManager.GetSection("SSOConfiguration/ProviderConfiguration") as SSOConfigurationProvider;
		}

		[ConfigurationProperty("Settings")]
		public SSOConfigurationSettingsCollection GetSettings
		{
			get
			{
				return this["Settings"] as SSOConfigurationSettingsCollection;
			}
		}

		[ConfigurationProperty("ExemptUsers")]
		public SSOConfigurationExemptUsersCollection ExemptUsers
		{
			get
			{
				return this["ExemptUsers"] as SSOConfigurationExemptUsersCollection;
			}
			set
			{
				this["ExemptUsers"] = value;
			}
		}
	}


	public class SSOConfigurationCSSecurity : ConfigurationSection
	{
		public static SSOConfigurationCSSecurity GetConfig()
		{
			return SSOConfig.OpenConfig().GetSection("SSOConfiguration/CSSecurity") as SSOConfigurationCSSecurity;
			//return ConfigurationManager.GetSection("SSOConfiguration/CSSecurity") as SSOConfigurationCSSecurity;
		}

		[ConfigurationProperty("CSSConnections")]
		public SSOConfigurationCSSConnectionsCollection CSSConnections
		{
			get
			{
				return this["CSSConnections"] as SSOConfigurationCSSConnectionsCollection;
			}
		}
	}

	public class SSOConfigurationLDAP : ConfigurationSection
	{
		public static SSOConfigurationLDAP GetConfig()
		{
			return SSOConfig.OpenConfig().GetSection("SSOConfiguration/LDAPConfiguration") as SSOConfigurationLDAP;
			//return ConfigurationManager.GetSection("SSOConfiguration/LDAPConfiguration") as SSOConfigurationLDAP;
		}

		[ConfigurationProperty("GetUserReturnInfo")]
		public SSOConfigurationReturnInfoCollection GetUserReturnInfo
		{
			get
			{
				return this["GetUserReturnInfo"] as SSOConfigurationReturnInfoCollection;
			}
		}

		[ConfigurationProperty("Settings")]
		public SSOConfigurationLDAPSettingsCollection GetLDAPSettings
		{
			get
			{
				return this["Settings"] as SSOConfigurationLDAPSettingsCollection;
			}
		}
	}

    //public class COELDAPConfiguration : IConfigurationSectionHandler 
    //{

    //    #region IConfigurationSectionHandler Members

    //    public object Create(object parent, object configContext, XmlNode section)
    //    {

    //        XmlSerializer ser = new XmlSerializer(typeof(string));
    //        return ser.Deserialize(new System.IO.StringReader(section.OuterXml));
    //    }

    //    #endregion
    //}

    public class COELDAPConfiguration : ConfigurationSection
    {

        private XmlNode data;

        public static COELDAPConfiguration GetConfig()
        {
            return SSOConfig.OpenConfig().GetSection("SSOConfiguration/COELDAPConfiguration") as COELDAPConfiguration;
            //return ConfigurationManager.GetSection("SSOConfiguration/LDAPConfiguration") as SSOConfigurationLDAP;
        }


        /// <summary>
        /// Create an instance of this object.
        /// </summary>
        public COELDAPConfiguration(): base()
        {
        }

        /// <summary>
        /// The contained object.
        /// </summary>
        public string Data
        {
            get
            {
                return data.OuterXml;
            }
            set
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(value);
                data = doc.DocumentElement;
            }
        }

        #region Overrides

        /// <summary>
        /// Retrieves the contained object from the section.
        /// </summary>
        /// <returns>The contained data object.</returns>
        protected override object GetRuntimeObject()
        {
            return data;
        }

        /// <summary>
        /// Deserializes the configuration section in the configuration file.
        /// </summary>
        /// <param name="reader">The reader containing the XML for the section.</param>
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            if (!reader.Read() || (reader.NodeType != XmlNodeType.Element))
            {
                throw new ConfigurationErrorsException("Configuration reader expected to find an element", reader);
            }
            this.DeserializeElement(reader, false);
        }

        /// <summary>
        /// Deserializes the configuration element in the configuration file.
        /// </summary>
        /// <param name="reader">The reader containing the XML for the section.</param>
        /// <param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false. 
        /// Ignored in this implementation. </param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            reader.MoveToContent();
            DeserializeData(reader);
            reader.ReadEndElement();
        }

        /// <summary>
        /// Serializes the configuration section to an XML string representation.
        /// </summary>
        /// <param name="parentElement">The parent element of this element.</param>
        /// <param name="name">The name of the section.</param>
        /// <param name="saveMode">The mode to use for saving.</param>
        /// <returns>The string representation of the section.</returns>
        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            //coverity Fix CID :18789
            string serializeSection = string.Empty;
            using (StringWriter sWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
            {
                using (XmlTextWriter xWriter = new XmlTextWriter(sWriter))
                {
                    xWriter.Formatting = Formatting.Indented;
                    xWriter.Indentation = 4;
                    xWriter.IndentChar = ' ';
                    this.SerializeToXmlElement(xWriter, name);
                    xWriter.Flush();
                    serializeSection = sWriter.ToString();
                }
            }
            return serializeSection;
        }

        /// <summary>
        /// Serializes the section into the configuration file.
        /// </summary>
        /// <param name="writer">The writer to use for serializing the class.</param>
        /// <param name="elementName">The name of the configuration section.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
        {
            if (writer == null)
                return false;
            writer.WriteStartElement(elementName);
            bool success = true;
            success = SerializeElement(writer, false);
            writer.WriteEndElement();
            return success;
        }

        /// <summary>
        /// Serilize the element to XML.
        /// </summary>
        /// <param name="writer">The XmlWriter to use for the serialization.</param>
        /// <param name="serializeCollectionKey">Flag whether to serialize the collection keys. Not used in this override.</param>
        /// <returns>True if the serialization was successful, false otherwise.</returns>
        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            if (writer == null)
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(string));
            serializer.Serialize(writer, data);

            return true;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Deserializes the data from the reader.
        /// </summary>
        /// <param name="reader">The XmlReader containing the serilized data.</param>
        private void DeserializeData(System.Xml.XmlReader reader)
        {
            reader.Read();
            reader.MoveToContent();
            XmlSerializer serializer = new XmlSerializer(typeof(XmlNode));
            this.data = serializer.Deserialize(reader) as XmlNode;
        }

        #endregion

        
    }


}