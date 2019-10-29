using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Properties;
using ChemDrawControl19;
using System.Runtime.InteropServices;

namespace CambridgeSoft.COE.Framework.COEChemDrawConverterService
{

    /// <summary>
    /// Contains methods that enable the conversion of molecular structures from one MIME type (format)
    /// to another. Some methods generate files in the file-system and provide URLs for retrieving
    /// those documents.
    /// </summary>
    /// <remarks>Currently relies on the ChemDraw control as the underlying converter class.</remarks>
    public class COEChemDrawConverterUtils
    {
        private const string cacheRelativePath = "TempResources";

        /// <summary>
        /// Uses a ChemDraw control instance (from ChemDrawControl12) to convert a structure from one formatted
        /// representation (MIME type) to another.
        /// </summary>
        /// <param name="originalStructureValue">The value of the original structure.</param>
        /// <param name="sourceMimeType">MIME type of the original structure</param>
        /// <param name="destimationMimeType">Desired MIME type for the reformatted structure</param>
        /// <returns>A MIME-specific representation of the original structure.</returns>
        public static string ConvertStructure(string originalStructureValue, string sourceMimeType, string destimationMimeType)
        {
            ChemDrawCtl ctrl = new ChemDrawCtl();
            string output = string.Empty;

            ctrl.Objects.Clear();
            ctrl.DataEncoded = true;
            ctrl.Objects.set_Data(
                sourceMimeType, null, null, null, UnicodeEncoding.ASCII.GetBytes(originalStructureValue)
            );
            output = ctrl.get_Data(destimationMimeType).ToString();

            return output;
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
        internal static string GetStructureResource(string structure, string sourceMimeType, string destinyMymeType, Unit height, Unit width)
        {
            // First we need to convert the string into bytes, which
            // means using a text encoder.
            string returnVal = string.Empty;
            ChemDrawCtl ctrl = new ChemDrawCtl();
            try
            {
                ctrl.Objects.Clear();
                ctrl.DataEncoded = true;

                ctrl.set_Data(sourceMimeType, structure);
                int finalHeight = (height == null) ? (int)ctrl.Objects.Height : (int)height.Value;
                int finalWidth = (width == null) ? (int)ctrl.Objects.Width : (int)width.Value;

                string base64 = ctrl.get_Data("chemical/x-cdx").ToString();
                returnVal = GetStructureResource(base64, destinyMymeType, height.Value, width.Value, "72");
                Marshal.ReleaseComObject(ctrl);
                ctrl = null;
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                Marshal.ReleaseComObject(ctrl);
                ctrl = null;
                GC.Collect();
            }
            return returnVal;
        }

        /// <summary>
        /// Given a structure representation in any of the chemical mime types, a new representation of the structure is created in the 
        /// destiny mime type desired, and then is stored and cached on a file system. Finally a relative URL pointing to that file is
        /// returned.
        /// </summary>
        /// <param name="structure">The input structure base64 encoded.</param>
        /// <param name="destinyMymeType">The desired mime type for the output.</param>
        /// <returns>A relative URL pointing to the translated structure.</returns>
        internal static string GetStructureResource(string base64, string destinyMymeType, double height, double width, string resolution)
        {
            string hash = GetStructureHash(base64);
            string path = System.Web.HttpContext.Current.Server.MapPath("/coecommonresources");
            DirectoryInfo dir = new DirectoryInfo(path + "\\" + cacheRelativePath);
            if (!dir.Exists)
                dir.Create();

            string fileExtension = ConvertMimeTypeToFileExtension(destinyMymeType);
            string fullPath = path + "\\" + cacheRelativePath + "\\" + hash + "-" + width + "X" + height + "." + fileExtension;


            if (!File.Exists(fullPath))
            {
                ChemDrawCtl ctrl = null;
                try
                {
                    ctrl = new ChemDrawCtl();
                    ctrl.Objects.Clear();
                    ctrl.DataEncoded = true;
                    ctrl.set_Data("chemical/x-cdx", base64);
                    object outResoulution = resolution;
                    object outPath = fullPath;
                    object outMymeType = destinyMymeType;
                    object outWidth = width;
                    object outHeight = height;
                    ctrl.SaveAs(ref outPath, ref outMymeType, ref outResoulution, ref outWidth, ref outHeight);
                    Marshal.ReleaseComObject(ctrl);
                    ctrl = null;
                    GC.Collect();
                }
                catch (System.Exception ex)
                {
                    Marshal.ReleaseComObject(ctrl);
                    ctrl = null;
                    GC.Collect();
                }
            }

            return "/coecommonresources/" + cacheRelativePath + "/" + hash + "-" + width + "X" + height + "." + fileExtension;
        }

        /// <summary>
        /// Given a base64 string representation of a structure, a unique hash is created for it. An standard MD5 hash algorithm is used.
        /// </summary>
        /// <param name="base64">The structure representation.</param>
        /// <returns>Its Hash.</returns>
        private static string GetStructureHash(string base64)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            // Create a buffer large enough to hold the string
            byte[] unicodeText = new byte[base64.Length * 2];
            enc.GetBytes(base64.ToCharArray(), 0, base64.Length, unicodeText, 0, true);

            // Build the final string by converting each byte
            // into hex and appending it to a StringBuilder
            StringBuilder sb = new StringBuilder();

            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] result = sha1.ComputeHash(unicodeText);
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method returns the file extension for a given mime type.
        /// </summary>
        /// <param name="destinyMymeType">The mime type.</param>
        /// <returns>The file extension.</returns>
        private static string ConvertMimeTypeToFileExtension(string destinyMymeType)
        {
            switch (destinyMymeType)
            {
                #region Chemical MimeTypes
                case "chemical/x-cdx":
                case "chemical/cdx":
                    return "cdx";
                case "text/xml":
                    return "cdxml";
                case "chemical/x-chemdraw":
                    return "chm";
                case "chemical/x-mdl-molfile":
                case "chemical/mdl-molfile":
                case "chemical/x-mdl-molfile-v3000":
                case "chemical/mdl-molfile-v3000":
                    return "mol";
                case "chemical/x-mdl-tgf":
                case "chemical/mdl-tgf":
                    return "tgf";
                case "chemical/x-mdl-rxn":
                case "chemical/mdl-rxn":
                case "chemical/x-mdl-rxn-v3000":
                case "chemical/mdl-rxn-v3000":
                    return "rxn";
                case "chemical/x-daylight-smiles":
                case "chemical/daylight-smiles":
                case "chemical/x-smiles":
                case "chemical/smiles":
                    return "smi";
                case "chemical/x-mdl-isis":
                case "chemical/mdl-isis":
                    return "skc";
                case "chemical/x-questel-f1":
                    return "fld";
                case "chemical/x-questel-f1-query":
                    return null; //TODO: Find out the file extension for chemical/x-questel-f1-query.
                case "chemical/x-msi-molfile":
                case "chemical/msi-molfile":
                    return "msm";
                case "chemical/x-smd":
                case "chemical/smd":
                    return "smd";
                case "chemical/x-ct":
                case "chemical/ct":
                    return "ct";
                case "chemical/x-cml":
                case "chemical/cml":
                    return "cml";
                case "chemical/x-name":
                    return string.Empty;
                case "chemical/x-inchi":
                case "chemical/inchi":
                    return "inchi";
                #endregion
                #region Images MimeTypes
                case "image/x-wmfs":
                case "image/x-wmf":
                case "image/wmf":
                    return "wmf";
                case "image/x-emf":
                case "image/emf":
                    return "emf";
                case "image/bmp":
                    return "bmp";
                case "image/gif":
                    return "gif";
                case "image/x-png":
                case "image/png":
                    return "png";
                case "image/tiff":
                    return "tif";
                #endregion
                #region Application MimeTypes
                case "application/postscript":
                    return "eps";
                #endregion
                default:
                    throw new Exception(Resources.MimeTypeNotSupported.Replace("{0}", destinyMymeType));
            }
        }

    }
}
