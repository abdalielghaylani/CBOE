using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spotfire.Dxp.Framework.Persistence;
using System.Diagnostics.CodeAnalysis;

namespace StructureSearchSupport
{
    /// <summary>
    /// Supported control types.
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        None = -1,

        /// <summary>
        /// Accord renderer.
        /// </summary>
        Accord = 2,

        /// <summary>
        /// ChemDraw renderer.
        /// </summary>
        ChemDraw = 1,

        /// <summary>
        /// Marvin renderer.
        /// </summary>
        Marvin = 3,

        /// <summary>
        /// Symyx/MDL renderer.
        /// </summary>
        MdlDraw = 0,

        /// <summary>
        /// ChemIQ renderer.
        /// </summary>
        ChemIQ = 4
    }

    /// <summary>
    ///     Structure payload hint value.
    /// </summary>
    /// <remarks>Must be public since it's used in the XmlSerialized IdList class</remarks>
    [Serializable]
    [PersistenceVersion(9, 0)]
    [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member",
        Justification = "FBM: CodeCleanup for legacy code.")]
    public enum StructureStringType
    {
        /// <summary>
        ///     Unknown payload type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        ///     Molfile clob payload type.
        /// </summary>
        Molfile,

        /// <summary>
        ///     CHIME string payload type.
        /// </summary>
        CHIME,

        /// <summary>
        ///     SMILES string payload type.
        /// </summary>
        SMILES,

        /// <summary>
        ///     CDX binary payload type.
        /// </summary>
        CDX
    }

    /// <summary>
    /// This class holds the names of the parameters that we are sending to and receiving from an editor.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The structure format for the structure.
        /// </summary>
        public const string StructureFormat = "StructureFormat";

        /// <summary>
        /// The actual structure.
        /// </summary>
        public const string Structure = "Structure";

        /// <summary>
        /// The window handle (can be used to make a subprocess modal).
        /// </summary>
        public const string Handle = "Handle";

        /// <summary>
        /// Gets the dialog result.
        /// </summary>
        public const string DialogResult = "DialogResult";
    }

    /// <summary>
    /// Constants for use throughout the product.
    /// </summary>
    public sealed class Identifiers
    {
        #region Constants and Fields
        /// <summary>
        /// The MIME content type for EMF.
        /// </summary>
        public const string EMFContentType = "image/emf";
        /// <summary>
        /// The MIME content type for CDX.
        /// </summary>
        public const string CDXContentType = "chemical/x-cdx";
        /// <summary>
        /// The MIME content type for CDXML.
        /// </summary>
        public const string CDXMLContentType = "text/xml";
        /// <summary>
        /// The MIME content type for SKC (MDLSK
        /// </summary>
        public const string SKCContentType = "chemical/x-mdl-isis";
        /// <summary>
        /// The MIME content type for CHIME.
        /// </summary>
        public const string ChimeContentType = "chemical/x-mdl-chime";

        /// <summary>
        /// The template for a property name used to find the retrieve link.
        /// </summary>
        public const string DefaultRetrieveLinkIdName = "LeadDiscovery.RetrieveLinkId";

        /// <summary>
        /// The template for a property name used to find the structure data column.
        /// </summary>
        public const string DefaultStructureDataColumnName = "LeadDiscovery.StructureDataColumn";

        /// <summary>
        /// The template for a property name used to find the structure ID column.
        /// </summary>
        public const string DefaultStructureIdColumnName = "LeadDiscovery.StructureIdColumn";

        /// <summary>
        /// The part of the template property names to replace for each specific IL collection.
        /// </summary>
        public const string ILPropertyDefaultCommonPart = "LeadDiscovery";

        /// <summary>
        /// The IL parameter name used for similarity search (lower bound).
        /// </summary>
        public const string ILSimilarityLowerParameter = "similarity_lower";

        /// <summary>
        /// The IL parameter name used for similarity search (type of search).
        /// </summary>
        public const string ILSimilarityTypeParameter = "similarity_type";

        /// <summary>
        /// The IL parameter name used for similarity search (upper bound).
        /// </summary>
        public const string ILSimilarityUpperParameter = "similarity_upper";

        /// <summary>
        /// The IL parameter name used for structure search (MOLFILE data).
        /// </summary>
        public const string ILStructureParameter = "structure";

        /// <summary>
        /// The MIME content type for MOLFILE data.
        /// </summary>
        public const string MolfileContentType = "chemical/x-mdl-molfile";

        /// <summary>
        /// IL propertyname to get the information link to retrieve structures.
        /// </summary>
        public const string RetrieveILPropertyName = ".Retrieve";

        /// <summary>
        /// IL property value to get the information link to retrieve structures.
        /// </summary>
        public const string RetrieveILPropertyValue = "Structure";

        // <summary>
        // The identifier for XXXXX.
        // </summary>
        // private const string RetrieveLinkID = "RetrieveLinkID";

        /// <summary>
        /// IL propertyname to get the information link to search structures.
        /// </summary>
        public const string SearchILPropertyName = ".Search";

        // public const string SimilaritySearchILPropertyValue = "Similarity";
        /// <summary>
        /// The MIME content type for SMILES data. 
        /// this string type is used in original implementation for smiles string conversion, though spotfire uses another smile type 
        /// Not sure why LD used the different type. Just create another const string for reuse to avoid typo.
        /// </summary>
        public const string XSmilesContentType = "chemical/x-smiles";

        /// <summary>
        /// The MIME content type for SMILES data.
        /// </summary>
        public const string SmilesContentType = "chemical/x-daylight-smiles";

        /// <summary>
        /// The metadata table property name used to define the structure filter data column.
        /// </summary>
        public const string StructureFilterDataColumnName = "LeadDiscovery.StructureFilterDataColumn";

        /// <summary>
        /// The metadata table property name used to define the structure filter Id column.
        /// </summary>
        public const string StructureFilterIdColumnName = "LeadDiscovery.StructureFilterIdColumn";

        /// <summary>
        /// The metadata table property name used to define the structure filter search type.
        /// </summary>
        public const string StructureFilterSearchType = "LeadDiscovery.StructureFilterSearchType";

        /// <summary>
        /// The metadata table property name used to define the structure filter similarity search's percent similarity.
        /// </summary>
        public const string StructureFilterPercentSimilarity = "LeadDiscovery.StructureFilterPercentSimilarity";

        /// <summary>
        /// The metadata table property name used to define the structure filter similarity search mode.
        /// </summary>
        public const string StructureFilterSimilarityMode = "LeadDiscovery.StructureFilterSimilarityMode";

        /// <summary>
        /// The metadata table property name used to define the structure filter search's structure string.
        /// </summary>
        public const string StructureFilterStructureString = "LeadDiscovery.StructureFilterStructureString";

        /// <summary>
        /// Column with structure identifiers.
        /// </summary>
        public const string StructureIdentifierColumnPropertyName = ".Column";

        /// <summary>
        /// Value expected for the property on the Id column.
        /// </summary>
        public const string StructureIdentifierColumnPropertyValue = "ID";

        /// <summary>
        /// Value expected for the property on the similarty score result column.
        /// </summary>
        public const string StructureSimilarityScoreColumnPropertyValue = "SimilarityScore";

        /// <summary>
        /// String used to identify substructure search.
        /// </summary>
        public const string SubstructureSearchILPropertyValue = "Substructure";

        /// <summary>
        /// The name of the structure match tag column
        /// </summary>

        public const string StructureFilterRGroup = "LeadDiscovery.StructureFilterRGroup";
        /// <summary>
        /// Check box to check if R-Group deecomposition is ticked or not.
        /// </summary>
        public const string StructureMatchColumn = "StructureMatchCol";

        /// <summary>The name of the information model property that is used to look up
        /// the model for the structure search parameters for the given link.
        /// </summary>
        internal const string StructureSearchModelILPropertyKey = "LeadDiscovery.StructureSearch.ParameterType";

        /// <summary>
        /// String used to identify CDX structure data
        /// </summary>
        public const string CDXStructureData = "CDXStructureData";

        /// <summary>
        /// String used to identify R-Group structure data
        /// </summary>
        public const string RGroupStructure = "LeadDiscovery.RGroupStructure";

        /// <summary>
        /// String used to identify R-Group structure data
        /// </summary>
        public const string RGroupStructureType = "LeadDiscovery.RGroupStructureType";

        /// <summary>
        /// Number of R-Groups from decomposition
        /// </summary>
        public const string NumberRGroups = "LeadDiscovery.NumberRGroups";

        /// <summary>
        /// CoreChemistryId, a GUID
        /// </summary>
        public const string CoreChemistryId = "LeadDiscovery.CoreChemistryId";

        #endregion
    }

    class General
    {
         // Not in Use
    }
}
