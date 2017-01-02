using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Web.UI.WebControls;

namespace CambridgeSoft.COE.Framework.COEChemDrawConverterService
{
    /// <summary>
    /// Acts as a wrapper class for the underlying class, COEChemDrawConverterUtils, which provides
    /// chemical structure conversion utilities.
    /// </summary>
    public class COEChemDrawConverter
    {
        /// <summary>
        /// Uses a ChemDraw control instance (from ChemDrawControl12) to convert a structure from one formatted
        /// representation (MIME type) to another.
        /// </summary>
        /// <remarks><seealso cref=""/>COEChemDrawConverterUtils.ConvertStructure</remarks>
        /// <param name="originalStructureValue">The value of the original structure.</param>
        /// <param name="sourceMimeType">MIME type of the original structure</param>
        /// <param name="destimationMimeType">Desired MIME type for the reformatted structure</param>
        /// <returns>A MIME-specific representation of the original structure.</returns>
        public static string ConvertStructure(string structure, string sourceMimeType, string destinationMymeType)
        {
            ConvertStructureCommand cmd = DataPortal.Execute<ConvertStructureCommand>(
                new ConvertStructureCommand(structure, sourceMimeType, destinationMymeType)
            );
            return cmd.NewStructure;
        }

        /// <summary>
        /// Given a structure representation in any of the chemical mime types, a new representation of the structure is created in the 
        /// destiny mime type desired, and then is stored and cached on a file system. Finally a relative URL pointing to that file is
        /// returned.
        /// </summary>
        /// <param name="structure">The input structure.</param>
        /// <param name="sourceMimeType">The original mime type of the structure.</param>
        /// <param name="destinyMymeType">The desired mime type for the output.</param>
        /// <returns>A relative URL pointing to the translated structure.</returns>
        public static string GetStructureResource(string structure, string sourceMimeType, string destinyMymeType)
        {
            try
            {
                GetStructureResourceCommand cmd = DataPortal.Execute<GetStructureResourceCommand>(new GetStructureResourceCommand(structure, sourceMimeType, destinyMymeType));
                return cmd.ResourceURL;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Given a structure representation in any of the chemical mime types, a new representation of the structure is created in the 
        /// destiny mime type desired, and then is stored and cached on a file system. Finally a relative URL pointing to that file is
        /// returned.
        /// </summary>
        /// <param name="structure">The input structure.</param>
        /// <param name="sourceMimeType">The original mime type of the structure.</param>
        /// <param name="destinyMymeType">The desired mime type for the output.</param>
        /// <returns>A relative URL pointing to the translated structure.</returns>
        public static string GetStructureResource(string structure, string sourceMimeType, string destinyMymeType, Unit height, Unit width)
        {
            try
            {
                GetStructureResourceCommand cmd = DataPortal.Execute<GetStructureResourceCommand>(new GetStructureResourceCommand(structure, sourceMimeType, destinyMymeType, height, width));
                return cmd.ResourceURL;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Given a structure representation in any of the chemical mime types, a new representation of the structure is created in the 
        /// destiny mime type desired, and then is stored and cached on a file system. Finally a relative URL pointing to that file is
        /// returned.
        /// </summary>
        /// <param name="structure">The input structure.</param>
        /// <param name="sourceMimeType">The original mime type of the structure.</param>
        /// <param name="destinyMymeType">The desired mime type for the output.</param>
        /// <returns>A relative URL pointing to the translated structure.</returns>
        public static string GetStructureResource(string base64, string destinyMymeType)
        {
            try
            {
                GetStructureResourceCommand cmd = DataPortal.Execute<GetStructureResourceCommand>(new GetStructureResourceCommand(base64, destinyMymeType));
                return cmd.ResourceURL;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Given a structure representation in any of the chemical mime types, a new representation of the structure is created in the 
        /// destiny mime type desired, and then is stored and cached on a file system. Finally a relative URL pointing to that file is
        /// returned.
        /// </summary>
        /// <param name="structure">The input structure.</param>
        /// <param name="sourceMimeType">The original mime type of the structure.</param>
        /// <param name="destinyMymeType">The desired mime type for the output.</param>
        /// <returns>A relative URL pointing to the translated structure.</returns>
        public static string GetStructureResource(string base64, string destinyMymeType, Unit height, Unit width)
        {
            try
            {
                GetStructureResourceCommand cmd = DataPortal.Execute<GetStructureResourceCommand>(new GetStructureResourceCommand(base64, destinyMymeType, height, width));
                return cmd.ResourceURL;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Commands

        [Serializable]
        private class ConvertStructureCommand : CommandBase
        {
            private string _sourceMimeType;
            private string _outputMimeType;
            private string _structure;
            private string _newStructure;

            /// <summary>
            /// Stores the result of the server-side structure conversion.
            /// </summary>
            public string NewStructure
            {
                get { return _newStructure; }
            }

            /// <summary>
            /// Client-side constructor
            /// </summary>
            /// <param name="originalStructureValue">Original structure prior to any conversion</param>
            /// <param name="sourceMimeType">The MIME type of the <paramref name="originalStructureValue"/> parameter</param>
            /// <param name="destimationMimeType">MIME type for the desired structure format to be returned</param>
            public ConvertStructureCommand(string originalStructureValue, string sourceMimeType, string destimationMimeType)
            {
                _structure = originalStructureValue;
                _sourceMimeType = sourceMimeType;
                _outputMimeType = destimationMimeType;
            }

            /// <summary>
            /// Server-side execution
            /// </summary>
            protected override void DataPortal_Execute()
            {
                _newStructure =
                    COEChemDrawConverterUtils.ConvertStructure(_structure, _sourceMimeType, _outputMimeType);
            }

        }

        [Serializable]
        private class GetStructureResourceCommand : CommandBase
        {
            private string _sourceMimeType;
            private string _outputMimeType;
            private string _structure;
            private string _resourceUrl;
            private Unit _height;
            private Unit _width;

            public string ResourceURL
            {
                get { return _resourceUrl; }
            }

            public GetStructureResourceCommand(string structure, string sourceMimeType, string outputMimeType)
            {
                _structure = structure;
                _sourceMimeType = sourceMimeType;
                _outputMimeType = outputMimeType;
            }

            public GetStructureResourceCommand(string structure, string sourceMimeType, string outputMimeType, Unit height, Unit width)
            {
                _structure = structure;
                _sourceMimeType = sourceMimeType;
                _outputMimeType = outputMimeType;
                _height = height;
                _width = width;
            }

            public GetStructureResourceCommand(string base64, string outputMimeType)
            {
                _structure = base64;
                _sourceMimeType = "chemical/x-cdx";
                _outputMimeType = outputMimeType;
            }

            public GetStructureResourceCommand(string base64, string outputMimeType, Unit height, Unit width)
            {
                _structure = base64;
                _sourceMimeType = "chemical/x-cdx";
                _outputMimeType = outputMimeType;
                _height = height;
                _width = width;
            }

            protected override void DataPortal_Execute()
            {
                _resourceUrl = COEChemDrawConverterUtils.GetStructureResource(_structure, _sourceMimeType, _outputMimeType, _height, _width);
            }
        }

        #endregion
    }
}
