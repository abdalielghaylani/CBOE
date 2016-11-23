using System;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// The types of mechanisms available when duplicate-checking during a bulk-load operation.
    /// </summary>
    [Serializable()]
    public enum PreloadDupCheckMechanism
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
    }
}