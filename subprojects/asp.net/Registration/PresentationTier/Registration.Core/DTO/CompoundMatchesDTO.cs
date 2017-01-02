using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Registration.Core
{
    /// <summary>
    /// This data-transfer object is used to convert an XML-formatted list of compound matches
    /// from the Oracle data-store.
    /// </summary>
    [Serializable]
    [XmlRoot("matches")]
    public class CompoundMatchesDTO
    {
        /// <summary>
        /// Static constructor, accepting an xml string derived ostensibly from a database stored procedure
        /// or some other mechanism.
        /// </summary>
        /// <param name="xml">the serialized format of an object instance</param>
        /// <returns>The data-transfer object for Compound-specific matches</returns>
        public static CompoundMatchesDTO GetResponse(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CompoundMatchesDTO));
            CompoundMatchesDTO dto = (CompoundMatchesDTO)xs.Deserialize(new StringReader(xml));
            return dto;
        }

        /// <summary>
        /// Generates the serialization XML for this instance.
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            string buf = BaseUtilities.ToXml(this);
            return buf;
        }

        private List<CompoundMatchDTO> _compounds;
        private int _uniqueComponents;
        private int _mechanism;

        /// <summary>
        /// A list of value-only objects representing matched compounds
        /// </summary>
        [XmlElement("match")]
        public List<CompoundMatchDTO> Compounds
        {
            get
            {
                if (this._compounds == null)
                    this._compounds = new List<CompoundMatchDTO>();
                return this._compounds;
            }
            set
            {
                this._compounds = value;
            }
        }

        /// <summary>
        /// The number of unique compounds found to match the target compound.
        /// </summary>
        [XmlAttribute("uniqueComps")]
        public int UniqueComponents
        {
            get { return this._uniqueComponents; }
            set { this._uniqueComponents = value; }
        }

        /// <summary>
        /// For individual compounds, the specific mechanism that was used to perform compound-matching. The
        /// value correspoinds to the <typeparamref name="MatchMechanism"/> enumeration.
        /// </summary>
        [XmlAttribute("mechanism")]
        public int Mechanism
        {
            get { return this._mechanism; }
            set { this._mechanism = value; }
        }

        #region [Nested classes]

        [Serializable]
        public partial class CompoundMatchDTO
        {
            private int _mixtureRegId;
            private string _mixtureRegNum;
            private int _compoundRegId;
            private string _compoundRegNum;
            private int _structureId;

            [XmlAttribute("mixtureRegId")]
            public int MixtureRegistrationId
            {
                get { return this._mixtureRegId; }
                set { this._mixtureRegId = value; }
            }

            [XmlAttribute("mixtureRegNum")]
            public string MixtureRegNumber
            {
                get { return this._mixtureRegNum; }
                set { this._mixtureRegNum = value; }
            }

            [XmlAttribute("compoundRegId")]
            public int CompoundRegistrationId
            {
                get { return this._compoundRegId; }
                set { this._compoundRegId = value; }
            }

            [XmlAttribute("compoundRegNum")]
            public string CompoundRegNumber
            {
                get { return this._compoundRegNum; }
                set { this._compoundRegNum = value; }
            }

            [XmlAttribute("structureId")]
            public int StructureId
            {
                get { return this._structureId; }
                set { this._structureId = value; }
            }
        }

        #endregion
    }
}
