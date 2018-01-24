using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// The types of mechanisms available when matching a compound or mixture against the repository.
    /// </summary>
    [Serializable()]
    public enum MatchMechanism
    {
        /// <summary>
        /// duplicate-detection is not configured
        /// </summary>
        [System.Xml.Serialization.XmlEnum("0")]
        None = 0,
        /// <summary>
        /// use the structure as a basis for comparison to other registrations
        /// </summary>
        [System.Xml.Serialization.XmlEnum("1")]
        Structure = 1,
        /// <summary>
        /// in lieu of a structure, use a component property as the basis for comparison
        /// </summary>
        [System.Xml.Serialization.XmlEnum("2")]
        ComponentProperty = 2,
        /// <summary>
        /// in lieu of a structure, use a component identifier as the basis for comparison
        /// </summary>
        [System.Xml.Serialization.XmlEnum("3")]
        ComponentIdentifier = 3,
        /// <summary>
        /// in lieu of a structure, use a structure property as the basis for comparison
        /// </summary>
        [System.Xml.Serialization.XmlEnum("4")]
        StructureProperty = 4,
        /// <summary>
        /// in lieu of a structure, use a structure identifier as the basis for comparison
        /// </summary>
        [System.Xml.Serialization.XmlEnum("5")]
        StructureIdentifier = 5,
        /// <summary>
        /// the components of a mixture are used to match existing mixtures
        /// </summary>
        [System.Xml.Serialization.XmlEnum("6")]
        Mixture = 6,
    }
}
