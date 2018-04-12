using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.DataLoader.Core.DataMapping
{
    /// <summary>
    /// Represents the mapping xml in the memory.
    /// </summary>
    [Serializable]
    [XmlRoot("mappings")]
    public class Mappings
    {

        #region fields

        private DestinationRecordTypeEnum _destinationRecordType;
        private List<Mapping> _mappingCollection;

        #endregion

        #region properties

        [XmlElement("destinationRecordType")]
        public DestinationRecordTypeEnum DestinationRecordType
        {
            get { return _destinationRecordType; }
            set { _destinationRecordType = value; }
        }

        [XmlElement("mapping")]
        public List<Mapping> MappingCollection
        {
            get
            {
                if (this._mappingCollection == null)
                {
                    this._mappingCollection = new List<Mapping>();
                }
                return this._mappingCollection;
            }
            set
            {
                this._mappingCollection = value;
            }
        }

        #endregion

        #region factroy methods

        /// <summary>
        /// Get mappings instance from xml string.
        /// </summary>
        /// <param name="xml">Xml sting</param>
        /// <returns>Mappings instance ref</returns>
        public static Mappings GetMappings(string xml)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Mappings));
                return (Mappings)xs.Deserialize(new StringReader(xml));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get mappings instance from specify full file path.
        /// </summary>
        /// <param name="fullFilePath">full file path sting</param>
        /// <returns>Mappings instance ref</returns>
        public static Mappings GetMappingsFromFile(string fullFilePath)
        {
            Mappings mappings = null;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fullFilePath);
                mappings = Mappings.GetMappings(doc.OuterXml);
                return mappings;
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convert DelimiterEnum to corresponding string for called method.
        /// </summary>
        /// <param name="delimiter">The delimiter need to be converted</param>
        /// <returns></returns>
        public static string ConvertDelimiterEnumToString(DelimiterEnum delimiter)
        {
            switch (delimiter)
            {
                case DelimiterEnum.Tab:
                    return "\u0009";
                    break;
                case DelimiterEnum.Pipeline:
                    return "\u007C";
                    break;
                case DelimiterEnum.Comma:
                    return "\u002C";
                    break;
                default:
                    return string.Empty;
                    break;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Returns the string representation of the current object as an xml fragmentmt,
        /// lacking a declaration, enabling the resulting string to be embedded into another
        /// xml document.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retVal = null;

            XmlSerializer xs = new XmlSerializer(typeof(Mappings));
                
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            writerSettings.Indent = true;
            StringWriter stringWriter = new StringWriter();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
            {
                xs.Serialize(xmlWriter, this, ns);
            }
            retVal = stringWriter.ToString();

            return retVal;
        }

        /// <summary>
        /// Save current mappings instance information to a specify file.
        /// </summary>
        /// <param name="fullFilePath">specify full file path</param>
        public void SaveToFile(string fullFilePath)
        {
            if (!this.IsMappingsEmpty())
            {
                string xml = this.ToString();
                StreamWriter writer = new StreamWriter(fullFilePath, false, Encoding.UTF8);
                writer.Write(xml);
                writer.Flush();
                writer.Close();
            }
        }

        private bool IsMappingsEmpty()
        {
            return this._mappingCollection.Count == 0;
        }

        #endregion

        #region additional classes
        //These classes represent all complex element(a element has attribute or child elements) of mapping xml

        /// <summary>
        /// Represent <mapping> element of mapping xml in memory. 
        /// <mapping> element is a child node of <mappings>.
        /// </summary>
        [Serializable]
        public class Mapping
        {
            #region fields

            private bool _enabledEnum = true;
            private string _objectBindingPath = string.Empty;
            private MemberInformation _memberInformation;

            #endregion

            #region properties

            [XmlElement("enabled")]
            public bool Enabled
            {
                get
                {
                    return this._enabledEnum;
                }
                set
                {
                    this._enabledEnum = value;
                }
            }

            [XmlElement("objectBindingPath")]
            public string ObjectBindingPath
            {
                get
                {
                    return this._objectBindingPath;
                }
                set
                {
                    this._objectBindingPath = value;
                }
            }

            [XmlElement("memberInformation")]
            public MemberInformation MemberInformation
            {
                get
                {
                    if (this._memberInformation == null)
                    {
                        this._memberInformation = new MemberInformation();
                    }
                    return this._memberInformation;
                }
                set
                {
                    this._memberInformation = value;
                }
            }

            #endregion
        }

        /// <summary>
        /// Represent <memberInformation> element of mapping xml in memory.
        /// <memberInformation> element is a child node of <mapping>.
        /// </summary>
        [Serializable]
        public class MemberInformation
        {
            #region fields

            private MemberTypeEnum _memberType;
            private TypeEnum _type;
            private string _name = string.Empty;
            private string _description;
            private List<Arg> _args;

            #endregion

            #region properties

            [XmlAttribute("memberType")]
            public MemberTypeEnum MemberType
            {
                get
                {
                    return this._memberType;
                }
                set
                {
                    this._memberType = value;
                }
            }

            [XmlElement("type")]
            public TypeEnum Type
            {
                get
                {
                    return this._type;
                }
                set
                {
                    this._type = value;
                }
            }

            [XmlElement("name")]
            public string Name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }

            [XmlElement("description")]
            public string Description
            {
                get
                {
                    return this._description;
                }
                set
                {
                    this._description = value;
                }
            }

            [XmlArray("args")]
            [XmlArrayItem("arg")]
            public List<Arg> Args
            {
                get
                {
                    if (this._args == null)
                    {
                        this._args = new List<Arg>();
                    }
                    return this._args;
                }
                set
                {
                    this._args = value;
                }
            }

            #endregion

        }

        /// <summary>
        /// Represent <arg> element of mapping xml in memory.
        /// <arg> element is a child of <args>.
        /// </summary>
        [Serializable]
        public class Arg
        {
            #region fields

            private int _index;
            private InputEnum _input;
            private string _type = string.Empty;
            private string _pickListCode;
            private string _value = string.Empty;
            private Resolver _resolver;

            #endregion

            #region properties

            [XmlAttribute("index")]
            public int Index
            {
                get
                {
                    return this._index;
                }
                set
                {
                    this._index = value;
                }
            }

            [XmlAttribute("input")]
            public InputEnum Input
            {
                get
                {
                    return this._input;
                }
                set
                {
                    this._input = value;
                }
            }

            [XmlAttribute("type")]
            public string Type
            {
                get
                {
                    return this._type;
                }
                set
                {
                    this._type = value;
                }
            }

            [XmlAttribute("pickListCode")]
            public string PickListCode
            {
                get
                {
                    return this._pickListCode;
                }
                set
                {
                    this._pickListCode = value;
                }
            }

            [XmlElement("value")]
            public string Value
            {
                get
                {
                    return this._value;
                }
                set
                {
                    this._value = value;
                }
            }

            [XmlElement("resolver")]
            public Resolver Resolver
            {
                get { return this._resolver; }
                set { this._resolver = value; }
            }

            #endregion
        }

        /// <summary>
        /// Represent <resolver> element of mapping xml in memory.
        /// <resolver> element is a child of <arg>.
        /// </summary>
        [Serializable]
        public class Resolver
        {
            #region fields

            private string _file = string.Empty;
            private DelimiterEnum _delimiter;
            private string _externalValueColumn = string.Empty;
            private string _internalValueColumn = string.Empty;

            #endregion

            #region properties

            [XmlElement("file")]
            public string File
            {
                get { return this._file; }
                set { this._file = value; }
            }

            [XmlElement("delimiter")]
            public DelimiterEnum Delimiter
            {
                get { return this._delimiter; }
                set { this._delimiter = value; }
            }

            [XmlElement("externalValueColumn")]
            public string ExternalValueColumn
            {
                get { return this._externalValueColumn; }
                set { this._externalValueColumn = value; }
            }

            [XmlElement("internalValueColumn")]
            public string InternalValueColumn
            {
                get { return this._internalValueColumn; }
                set { this._internalValueColumn = value; }
            }

            #endregion
        }

        #endregion

        #region enum

        /// <summary>
        /// Indicate the mapping type of one single <mapping> element.
        /// When the value is "property", we do SetProperty process in MappingService class.
        /// When the value is "method", we do InvokeMethod process in MappingService class.
        /// </summary>
        [Serializable]
        public enum MemberTypeEnum
        {
            [XmlEnum("property")]
            Property,
            [XmlEnum("method")]
            Method,
        }

        /// <summary>
        /// Indicate the meaning of the <value> element in one <arg> element.
        /// When this enum value is "Constant", the <value> element's text is from user input, 
        /// and we can directly use this value.
        /// When this enum value is "Derived", the <value> element's text just means a string key,
        /// we use this key to get the final value which represent an <arg> in our process.
        /// </summary>
        [Serializable]
        public enum InputEnum
        {
            [XmlEnum("constant")]
            Constant,
            [XmlEnum("derived")]
            Derived,
        }

        /// <summary>
        /// Indicate the destination recored type for a mapping xml.
        /// </summary>
        [Serializable]
        public enum DestinationRecordTypeEnum
        {
            /// <summary>
            /// Registration record
            /// </summary>
            [XmlEnum("RegistryRecord")]
            RegistryRecord,
            /// <summary>
            /// The mock registration record, for testing purpose only
            /// </summary>
            [XmlEnum("MockRegistryRecord")]
            MockRegistryRecord
        }

        /// <summary>
        /// Indicate the member type of destinationObject which represented by one single <mapping> element.
        /// </summary>
        [Serializable]
        public enum TypeEnum
        {
            [XmlEnum("instance")]
            Instance,
            [XmlEnum("static")]
            Static
        }

        /// <summary>
        /// Enumerate all kinds of delimiters that supporting current mapping process.
        /// </summary>
        [Serializable]
        public enum DelimiterEnum
        {
            [XmlEnum("tab")]
            Tab,
            [XmlEnum("pipeline")]
            Pipeline,
            [XmlEnum("comma")]
            Comma
        }

        #endregion

    }
}
