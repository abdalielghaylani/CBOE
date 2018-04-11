// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChemDrawUtilities.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.ChemDraw
{
    using System;
    using System.Globalization;

    using AxChemDrawControlConst11;
    using ChemDrawControlConst11;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.Win32;

    /// <summary>
    /// ChemDraw utility methods and constants
    /// </summary>
    internal class ChemDrawUtilities
    {
        /// <summary>
        /// the xmlSerializer which is used for serialization and deserialization
        /// </summary>
        private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(ChemDrawRenderSettings));

        internal static readonly string DefaultRenderSettingsString = ChemDrawUtilities.Base64SerializeChemDrawRenderSettings(ChemDrawRenderSettings.GetDefaultSettings());

        #region Methods

        /// <summary>
        /// Invokes the CopyToClipboard method
        /// </summary>
        /// <param name="chemDrawCtl">The COM chemdraw control instance.</param>
        /// <remarks>This method is needed since CambridgeSoft introduced a
        /// bug in CD12 - the XXX interface suddenly changed IID, causing typed access to fail
        /// for code compiled against the CD10 typelib</remarks>
        internal static void CopyToClipboard(AxChemDrawCtl chemDrawCtl)
        {
            chemDrawCtl.Objects.Copy();
        }

        
        /// <summary>
        /// Tries the get class id from prog id.
        /// </summary>
        /// <param name="progId">The prog id.</param>
        /// <param name="classId">The class id. Set to <see cref="Guid.Empty"/>
        /// if the method returns false</param>
        /// <returns>True if successful, otherwise false</returns>
        public static bool TryGetClassIdFromProgId(string progId, out Guid classId)
        {
            classId = Guid.Empty;
            Type t = Type.GetTypeFromProgID(progId);
            bool result = false;
            if (t != null)
            {
                if (t.GUID != Guid.Empty)
                {
                    classId = t.GUID;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Deserialize a base64 settings string to a ChemDrawRenderSettings instance
        /// </summary>
        /// <param name="settings">the base64 settings string</param>
        /// <returns>the ChemDrawRenderSettings instance</returns>
        public static ChemDrawRenderSettings Base64DeserializeChemDrawRenderSettings(string settings)
        {
            if (string.IsNullOrEmpty(settings))
            {
                return null;
            }

            MemoryStream ms = new MemoryStream(Convert.FromBase64String(settings));
            ChemDrawRenderSettings renderSettings = (ChemDrawRenderSettings)xmlSerializer.Deserialize(ms);
            return renderSettings;
        }

        /// <summary>
        /// Serialize a ChemDrawRenderSettings instance to a base64 settings string
        /// </summary>
        /// <param name="settings">the ChemDrawRenderSettings instance</param>
        /// <returns>the base64 settings string</returns>
        public static string Base64SerializeChemDrawRenderSettings(ChemDrawRenderSettings settings)
        {
            if (settings == null)
            {
                return string.Empty;
            }

            MemoryStream ms = new MemoryStream();
            xmlSerializer.Serialize(ms, settings);
            string result = Convert.ToBase64String((ms.ToArray()));
            return result;
        }

        /// <summary>
        /// Get the version number of chemdraw Dll
        /// </summary>
        /// <returns>the version number</returns>
        public static int GetDllVersion()
        {
            int versionNumber = 12;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey("ChemDrawControlConst11.ChemDrawCtl\\CurVer");
                if (key != null)
                {
                    string valueString = key.GetValue("") as string;
                    if (!string.IsNullOrEmpty(valueString))
                    {
                        string[] array = valueString.Split(new char[] { '.' });
                        if (array.Length > 2)
                        {
                            int.TryParse(array[2], out versionNumber);
                        }
                    }
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
            
            return versionNumber;
        }

        #endregion
    }
}
