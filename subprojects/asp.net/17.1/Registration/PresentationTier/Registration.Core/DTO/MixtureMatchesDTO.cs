using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CambridgeSoft.COE.Registration.Core
{
    /// <summary>
    /// This data-transfer object is used to convert an XML-formatted list of mixture matches
    /// from the Oracle data-store.
    /// </summary>
    [Serializable]
    [XmlRoot("matches")]
    public class MixtureMatchesDTO
    {
        //TODO: Document
        public static MixtureMatchesDTO GetResponse(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MixtureMatchesDTO));
            MixtureMatchesDTO dto = (MixtureMatchesDTO)xs.Deserialize(new StringReader(xml));
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

        private int _matchCount;
        [XmlAttribute("uniqueMixtures")]
        public int MatchCount
        {
            get { return this._matchCount; }
            set { this._matchCount = value; }
        }

        private int _mechanism;
        [XmlAttribute("mechanism")]
        public int Mechanism
        {
            get { return this._mechanism; }
            set { this._mechanism = value; }
        }

        private List<MixtureMatchDTO> _mixtures;
        [XmlElement("match")]
        public List<MixtureMatchDTO> Mixtures
        {
            get
            {
                if (this._mixtures == null)
                    this._mixtures = new List<MixtureMatchDTO>();
                return this._mixtures;
            }
            set
            {
                this._mixtures = value;
            }
        }

        #region [Nested classes]

        [Serializable]
        public partial class MixtureMatchDTO
        {
            private int _mixtureRegId;
            [XmlAttribute("mixtureRegId")]
            public int MixtureRegistrationId
            {
                get { return this._mixtureRegId; }
                set { this._mixtureRegId = value; }
            }

            private string _mixtureRegNum;
            [XmlAttribute("mixtureRegNum")]
            public string MixtureRegNumber
            {
                get { return this._mixtureRegNum; }
                set { this._mixtureRegNum = value; }
            }

            private List<MixtureCompoundDTO> _compounds;
            [XmlElement("compound")]
            public List<MixtureCompoundDTO> Compounds
            {
                get
                {
                    if (this._compounds == null)
                        this._compounds = new List<MixtureCompoundDTO>();
                    return this._compounds;
                }
                set
                {
                    this._compounds = value;
                }
            }
        }

        [Serializable]
        public class MixtureCompoundDTO
        {
            private int _compoundRegId;
            private string _compoundRegNum;
            private int _structureId;

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
