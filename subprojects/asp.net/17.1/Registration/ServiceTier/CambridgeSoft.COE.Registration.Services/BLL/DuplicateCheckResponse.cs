using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// The top-level container for duplicate-checking response objects.
    /// </summary>
    [Serializable]
    [XmlRoot("RegistryList")]
    public partial class DuplicateCheckResponse
    {
        /// <summary>
        /// Factory-style constructor; deserializes the raw XML from the DAL method
        /// to instantiate an instance.
        /// </summary>
        /// <param name="xml">the xml response returned from the DAL</param>
        /// <returns>an instance of the DuplicateCheckResponse class</returns>
        public static DuplicateCheckResponse GetResponse(string xml)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(DuplicateCheckResponse));
                return (DuplicateCheckResponse)xs.Deserialize(new StringReader(xml));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<MatchedRegistration> _matchedRegistrations;
        private int _uniqueRegistrations;
        private int _uniqueComponents;
        private int _uniqueStructures;
        private PreloadDupCheckMechanism _mechanism;

        /// <summary>
        /// List of registrations containing components matching the given duplicate-checking criteria.
        /// </summary>
        [XmlElement("Registration")]
        public List<MatchedRegistration> MatchedRegistrations
        {
            get
            {
                if (this._matchedRegistrations == null)
                    this._matchedRegistrations = new List<MatchedRegistration>();
                return this._matchedRegistrations;
            }
            set
            {
                this._matchedRegistrations = value;
            }
        }

        /// <summary>
        /// The total number of unique registrations found.
        /// </summary>
        [XmlAttribute("uniqueRegs")]
        public int UniqueRegistrations
        {
            get { return this._uniqueRegistrations; }
            set { this._uniqueRegistrations = value; }
        }

        /// <summary>
        /// The total number of unique Components found.
        /// </summary>
        [XmlAttribute("uniqueComps")]
        public int UniqueComponents
        {
            get { return this._uniqueComponents; }
            set { this._uniqueComponents = value; }
        }

        /// <summary>
        /// The total number of unique Structures found; for non-structural checking, this may be
        /// one (1) for 'No-Structure' indicators, but could also be greater than one if there
        /// are different structures in use.
        /// </summary>
        [XmlAttribute("uniqueStructs")]
        public int UniqueStructures
        {
            get { return this._uniqueStructures; }
            set { this._uniqueStructures = value; }
        }

        /// <summary>
        /// The mechanism used to match the component.
        /// </summary>
        [XmlAttribute("mechanism")]
        public PreloadDupCheckMechanism Mechanism
        {
            get { return this._mechanism; }
            set { this._mechanism = value; }
        }

        /// <summary>
        /// Returns the string representation of the current object as an xml fragmentmt,
        /// lacking a declaration, enabling the resulting string to be embedded into another
        /// xml document.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retVal = null;

            XmlSerializer xs = new XmlSerializer(typeof(DuplicateCheckResponse));

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

        #region > Nested classes <

        /// <summary>
        /// The IDs representing a registry record that contain a component matching the given criteria.
        /// </summary>
        [Serializable]
        public partial class MatchedRegistration
        {
            private List<MatchedComponent> _matchedComponents;
            private int _id;
            private string _regNum;

            [XmlElement("Component")]
            public List<MatchedComponent> MatchedComponents
            {
                get
                {
                    if (this._matchedComponents == null)
                        this._matchedComponents = new List<MatchedComponent>();
                    return this._matchedComponents;
                }
                set
                {
                    this._matchedComponents = value;
                }
            }

            [XmlAttribute("id")]
            public int Id
            {
                get { return this._id; }
                set { this._id = value; }
            }

            [XmlAttribute("regNum")]
            public string RegistryNumber
            {
                get { return this._regNum; }
                set { this._regNum = value; }
            }
        }

        /// <summary>
        /// The IDs representing a component matching the given criteria.
        /// </summary>
        [Serializable]
        public partial class MatchedComponent
        {

            private List<MatchedStructure> _matchedStructures;
            private int _id;
            private string _regNum;
            private string _fragmentlist;
            private string _samefragments;
            private string _sameEquivalents="True";

            [XmlElement("Structure")]
            public List<MatchedStructure> MatchedStructures
            {
                get
                {
                    if (this._matchedStructures == null)
                        this._matchedStructures = new List<MatchedStructure>();
                    return this._matchedStructures;
                }
                set
                {
                    this._matchedStructures = value;
                }
            }

            [XmlAttribute("id")]
            public int Id
            {
                get { return this._id; }
                set { this._id = value; }
            }

            [XmlAttribute("regNum")]
            public string RegistryNumber
            {
                get { return this._regNum; }
                set { this._regNum = value; }
            }

            [XmlAttribute("fragmentlist")]
            public string FragmentList
            {
                get { return this._fragmentlist; }
                set { this._fragmentlist = value; }
            }

            [XmlAttribute("samefragments")]
            public string SameFragments
            {
                get { return this._samefragments; }
                set { this._samefragments = value; }
            }
            [XmlAttribute("sameEquivalents")]
            public string SameEquivalents
            {
                get { return this._sameEquivalents; }
                set { this._sameEquivalents = value; }
            }

        }

        /// <summary>
        /// The IDs representing a structure matching the given criteria, for structure-based
        /// duplicate-checking.
        /// </summary>
        [Serializable]
        public partial class MatchedStructure
        {

            private int _id;

            [XmlAttribute("id")]
            public int Id
            {
                get { return this._id; }
                set { this._id = value; }
            }
        }

        #endregion
    }

}