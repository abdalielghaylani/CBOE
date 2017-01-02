// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChemDrawInvoker.cs" company="PerkinElmer Inc.">
//   Copyright © 2005 - 2011 PerkinElmer Inc., 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StructureSearchSupport
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
   

   


    internal class ChemDrawInvoker
    {

        #region Constants and Fields

        /// <summary>
        /// Error prefix output from OOP process
        /// </summary>
        public const string StructureControlHostErrorPrefix = "StructureControlHostError ";

        /// <summary>
        /// Normal prefix ouput from OOP process
        /// </summary>
        public const string StructureControlHostOutputPrefix = "StructureControlHostOutput ";

        /// <summary>
        /// OOP process run out of Spotfire process
        /// </summary>
        private static Process oopProcess;

        private static object locker = new object();

        //OOP Commands for ChemDraw
        private readonly static string SET_STRUCTUREDATA_CMD = "set_StructureData {0}";
        private readonly static string GET_MOLFILE_CMD = "get_MolFileString";
        private readonly static string GET_SMILES_CMD = "get_SmilesString";
        private readonly static string GET_CDX_CMD = "get_CDXData";
        private readonly static string SET_CDX_CMD = "set_CDXData {0}";
        private readonly static string GET_CDXML_CMD = "get_CDXMLString";
        private readonly static string GET_IMG_CMD = "get_Image";
        private readonly static string GET_FORMULA_CMD = "get_FormulaName";
        private readonly static string COPY_TO_CLIPBOARD_CMD_1 = "CopyToClipboard {0}";
        private readonly static string INIT_CMD = "Init {0} {1}";

        #endregion

        #region Constructors and Destructors

        static ChemDrawInvoker()
        {
            Init();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Launch OOP process Spotfire.Dxp.LeadDiscovery.StructureControlHost.exe to host one structure control
        /// </summary>
        /// <returns></returns>
        private static bool LaunchProcess()
        {
            string structureControlHost = GetBundledExecutablePath("Spotfire.Dxp.LeadDiscovery.StructureControlHost.exe"); //@"C:\From DTP090-A\Sources\P4\Spotfire\Spotfire\Spotfire\Spotfire.Dxp.LeadDiscovery\build\AnyCPU\Spotfire.Dxp.LeadDiscovery.StructureControlHost.exe";
            ProcessStartInfo processStartInfo = new ProcessStartInfo(structureControlHost);
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            try
            {
                oopProcess = Process.Start(processStartInfo);
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

            return false;
        }

        internal static string GetBundledExecutablePath(string executableFile)
        {
            string structureControlHost =
                Path.Combine(
                    Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), executableFile);
            return structureControlHost;
        }


        /// <summary>
        /// Initialize OOP process
        /// host ChemDraw structure control for converter
        /// </summary>
        /// <returns></returns>
        internal static bool Init()
        {
            if (oopProcess == null)
            {
                if (!LaunchProcess())
                {
                    return false;
                }
            }


            // use ChemDraw structure control as converter control
            string initResult =
                CallOOPProcess(
                    string.Format(CultureInfo.CurrentCulture, INIT_CMD, ControlType.ChemDraw, true));
            if (initResult.Equals(bool.TrueString))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call OOP process with command
        /// </summary>
        /// <param name="call">command</param>
        /// <returns></returns>
        private static string CallOOPProcess(string call)
        {
            if (oopProcess == null)
            {
                throw new Exception("Outer process not started");
            }

            if (oopProcess.HasExited)
            {
                throw new InvalidOperationException("Outer process has exited.");
            }

            oopProcess.StandardInput.WriteLine(call);

            string result = string.Empty;
            while (!result.StartsWith(StructureControlHostOutputPrefix, StringComparison.OrdinalIgnoreCase))
            {
                result = oopProcess.StandardOutput.ReadLine();
            }

            result = result.Remove(0, StructureControlHostOutputPrefix.Length);

            string error = string.Empty;
            while (!error.StartsWith(StructureControlHostErrorPrefix, StringComparison.OrdinalIgnoreCase))
            {
                error = oopProcess.StandardError.ReadLine();
            }

            error = error.Remove(0, StructureControlHostErrorPrefix.Length);

            if (!string.IsNullOrEmpty(error))
            {
                try
                {
                    error = Encoding.UTF8.GetString(Convert.FromBase64String(error));
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }

                throw new Exception(error);
            }

            return result;
        }


        /// <summary>
        /// Convert structure data from one type to another types
        /// It supports convert from MolString, SMILES, CDXML string type to MolString, SMILES, CDXML and CDX data format
        /// support to return structure image and Formula name 
        /// </summary>
        /// <param name="originalStructureValue">original structure string, it can be MolString, SMILES and CDXML string</param>
        /// <param name="destinationMimeTypes">destination MIME types, Mol, SMILES, CDX, CDXML, img and formula
        /// available values for this parameter:
        ///   Mol: "mol" "chemical/x-mdl-molfile" "chemical/mdl-molfile"
        ///   SMILES: "smiles" "chemical/x-daylight-smiles" "chemical/daylight-smiles" "chemical/x-smiles" "chemical/smiles"
        ///   CDXML: "cdxml" "chemical/cdxml" "chemical/x-cdxml" "text/xml"
        ///   CDX: "cdx" "chemical/x-cdx" "chemical/cdx" "chemical/x-chemdraw-cdx" "chemical/chemdraw-cdx"
        ///   Structure Image: "img" "image"
        ///   Formula Name: "formula"</param>
        /// <returns>one Dictionary contains the converted values, and their key are the destination MIME type</returns>
        public static Dictionary<string, object> ConvertStructure(string originalStructureValue, string[] destinationMimeTypes)
        {
            lock (locker)
            {
                CallOOPProcess(string.Format(SET_STRUCTUREDATA_CMD, Convert.ToBase64String(UnicodeEncoding.UTF8.GetBytes(originalStructureValue))));

                return ConvertData(destinationMimeTypes);
            }
        }


        /// <summary>
        /// Convert CDX byte array data to other types
        /// It supports convert CDX data to MolString, SMILES, CDXML and CDX data format
        /// support to return structure image and Formula name 
        /// </summary>
        /// <param name="originalCDXData">CDX byte array data</param>
        /// <param name="destinationMimeTypes">destination MIME types, Mol, SMILES, CDX, CDXML, img and formula
        /// available values for this parameter:
        ///   Mol: "mol" "chemical/x-mdl-molfile" "chemical/mdl-molfile"
        ///   SMILES: "smiles" "chemical/x-daylight-smiles" "chemical/daylight-smiles" "chemical/x-smiles" "chemical/smiles"
        ///   CDXML: "cdxml" "chemical/cdxml" "chemical/x-cdxml" "text/xml"
        ///   CDX: "cdx" "chemical/x-cdx" "chemical/cdx" "chemical/x-chemdraw-cdx" "chemical/chemdraw-cdx"
        ///   Structure Image: "img" "image"
        ///   Formula Name: "formula"</param>
        /// <returns>one Dictionary contains the converted values, and their key are the destination MIME type</returns>
        public static Dictionary<string, object> ConvertCDXData(byte[] originalCDXData, string[] destinationMimeTypes)
        {
            lock (locker)
            {
                CallOOPProcess(string.Format(SET_CDX_CMD, Convert.ToBase64String(originalCDXData)));

                return ConvertData(destinationMimeTypes);
            }
        }


        /// <summary>
        /// convert and add the destination data into return dictionary object
        /// </summary>
        /// <param name="destinationMimeTypes"></param>
        /// <returns></returns>
        private static Dictionary<string, object> ConvertData(string[] destinationMimeTypes)
        {
            Dictionary<string, object> destinationObjects = new Dictionary<string, object>();

            foreach (string type in destinationMimeTypes)
            {
                switch (type.Trim().ToLower())
                {
                    case "mol":
                    case Identifiers.MolfileContentType:
                    case "chemical/mdl-molfile":
                        destinationObjects[type] = Encoding.UTF8.GetString(Convert.FromBase64String(CallOOPProcess(GET_MOLFILE_CMD)));
                        break;
                    case "smiles":
                    case Identifiers.SmilesContentType:
                    case "chemical/daylight-smiles":
                    case Identifiers.XSmilesContentType:
                    case "chemical/smiles":
                        destinationObjects[type] = Encoding.UTF8.GetString(Convert.FromBase64String(CallOOPProcess(GET_SMILES_CMD)));
                        break;
                    case "cdx":
                    case Identifiers.CDXContentType:
                    case "chemical/cdx":
                    case "chemical/x-chemdraw-cdx":
                    case "chemical/chemdraw-cdx":
                        destinationObjects[type] = Convert.FromBase64String(CallOOPProcess(GET_CDX_CMD));
                        break;
                    case "cdxml":
                    case "chemical/cdxml":
                    case "chemical/x-cdxml":
                    case Identifiers.CDXMLContentType:
                        destinationObjects[type] = Encoding.UTF8.GetString(Convert.FromBase64String(CallOOPProcess(GET_CDXML_CMD)));
                        break;
                    case "img":
                    case "image":
                        destinationObjects[type] = Convert.FromBase64String(CallOOPProcess(GET_IMG_CMD));
                        break;
                    case "formula":
                        destinationObjects[type] = Encoding.UTF8.GetString(Convert.FromBase64String(CallOOPProcess(GET_FORMULA_CMD)));
                        break;
                }
            }

            return destinationObjects;
        }


        /// <summary>
        /// Copy structure string to Clipboard
        /// </summary>
        /// <param name="originalStructureValue">Structure data to copy</param>
        /// <returns></returns>
        public static bool CoppyToClipboard(string originalStructureValue)
        {
            CallOOPProcess(string.Format(COPY_TO_CLIPBOARD_CMD_1, Convert.ToBase64String(UnicodeEncoding.ASCII.GetBytes(originalStructureValue))));            
            return true;
        }


        #endregion
    }
}
