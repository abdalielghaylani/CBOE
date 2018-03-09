// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureControlFactory.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilterSupport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using log4net;

    using CBVNStructureFilterSupport.MDLDraw;

    using StructureControl = CBVNStructureFilterSupport.OOP.StructureControl;
    using CBVNStructureFilterSupport.Framework;
    using CBVNStructureFilterSupport.ExternalProcess;

    internal static class StructureControlFactory
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureControlFactory));

        internal static readonly Dictionary<ControlType, bool> InstalledControlTypes =
            new Dictionary<ControlType, bool>();

        #endregion

        #region Properties

        public static bool IsAnyControlTypeInstalled
        {
            get
            {
                foreach (ControlType controlType in Enum.GetValues(typeof(ControlType)))
                {
                    if (IsControlTypeInstalled(controlType))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Set to true by addin registration if running 64-bit or explicitly forced to run out-of-process
        /// through the for-test environment variable
        /// </summary>
        public static bool OutOfProcessRendering { get { return IntPtr.Size == 8; } }

        #endregion

        #region Public Methods

        public static bool ControlTypeSupportsConcurrentThreads(ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Accord:
                case ControlType.Marvin:
                case ControlType.ChemIQ:
                    return true;

                case ControlType.ChemDraw:
                case ControlType.MdlDraw:
                    return OutOfProcessRendering;
            }

            return false;
        }

        public static bool ControlTypeSupportsEditing(ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.ChemDraw:
                case ControlType.MdlDraw:
                case ControlType.Marvin:
                    return true;
            }

            return false;
        }

        public static bool ControlTypeUnderstandsContentType(ControlType controlType, string contentType)
        {
            string contentTypeUpper = contentType.ToUpperInvariant();

            switch (controlType)
            {
                case ControlType.MdlDraw:
                    return contentTypeUpper.Contains("MOLFILE") || contentTypeUpper.Contains("SMILE") ||
                           contentTypeUpper.Contains("CHIME");

                case ControlType.Accord:
                case ControlType.Marvin:
                case ControlType.ChemIQ:
                    return contentTypeUpper.Contains("MOLFILE") || contentTypeUpper.Contains("SMILE");

                case ControlType.ChemDraw:
                    return contentTypeUpper.Contains("MOLFILE") || contentTypeUpper.Contains("SMILE") ||
                           contentTypeUpper.Contains("CDX"); // Add CDX content-type for ChemDraw renderer
            }

            return false;
        }

        public static StructureControlBase Create(ControlType controlType, bool viewOnly)
        {
            switch (controlType)
            {
                case ControlType.Accord:
                    if (OutOfProcessRendering)
                    {
                        return new StructureControl(controlType, viewOnly);
                    }

                    return new Accord.StructureControl(viewOnly);

                case ControlType.ChemDraw:
                    if (OutOfProcessRendering)
                    {
                        return new StructureControl(controlType, viewOnly);
                    }

                    return new ChemDraw.StructureControl(viewOnly);

                case ControlType.Marvin:
                    return new Marvin.StructureControl(viewOnly);

                case ControlType.MdlDraw:
                    if (OutOfProcessRendering)
                    {
                        return new StructureControl(controlType, viewOnly);
                    }

                    return CBVNStructureFilterSupport.MDLDraw.SymyxMdlUtilities.CreateControl(viewOnly);

                case ControlType.ChemIQ:
                    if (OutOfProcessRendering)
                    {
                        return new StructureControl(controlType, viewOnly);
                    }

                    return new ChemIQ.StructureControl(viewOnly);

                default:
                    return null;
            }
        }

        public static ProcessHost GetEditingHost(ControlType controlType)
        {
            if (!ControlTypeSupportsEditing(controlType))
            {
                return null;
            }

            string executableName;
            switch (controlType)
            {
                case ControlType.ChemDraw:
                    executableName = Path.Combine(GeneralClass.LDInstallationPath, "Spotfire.Dxp.LeadDiscovery.ChemDrawEditor.exe");
                    break;
                case ControlType.MdlDraw:
                    executableName = Path.Combine(GeneralClass.LDInstallationPath, "Spotfire.Dxp.LeadDiscovery.MdlDrawEditor.exe");
                    break;
                case ControlType.Marvin:
                    ProcessHost processHost = new MarvinSketchProcessHost();
                    return processHost;

                default:
                    throw new ArgumentOutOfRangeException("controlType");
            }

            string fullPath = executableName; // GetBundledExecutablePath(executableName);

            ProcessHost host = new NetProcessHost(fullPath);
            return host;
        }

        public static bool IsControlTypeInstalled(ControlType controlType)
        {
            lock (InstalledControlTypes)
            {
                if (!InstalledControlTypes.ContainsKey(controlType))
                {
                    Log.DebugFormat("Checking if control type '{0}' is installed, OutOfProcess={1}", controlType, OutOfProcessRendering);

                    switch (controlType)
                    {
                        case ControlType.Accord:
                            if (OutOfProcessRendering)
                            {
                                InstalledControlTypes[controlType] = IsControlTypeInstalledOop(controlType);
                            }
                            else
                            {
                                InstalledControlTypes[controlType] = Accord.StructureControl.IsInstalled;
                            }

                            break;

                        case ControlType.ChemDraw:
                            if (OutOfProcessRendering)
                            {
                                InstalledControlTypes[controlType] = IsControlTypeInstalledOop(controlType);
                            }
                            else
                            {
                                InstalledControlTypes[controlType] = ChemDraw.StructureControl.IsInstalled;
                            }

                            break;

                        case ControlType.Marvin:
                            InstalledControlTypes[controlType] = Marvin.StructureControl.IsInstalled;
                            break;

                        case ControlType.MdlDraw:
                            if (OutOfProcessRendering)
                            {
                                InstalledControlTypes[controlType] = IsControlTypeInstalledOop(controlType);
                            }
                            else
                            {
                                InstalledControlTypes[controlType] = CBVNStructureFilterSupport.MDLDraw.SymyxMdlUtilities.IsSymyxOrMdlInstalled;
                            }

                            break;

                        case ControlType.ChemIQ:
                            if (OutOfProcessRendering)
                            {
                                InstalledControlTypes[controlType] = IsControlTypeInstalledOop(controlType);
                            }
                            else
                            {
                                InstalledControlTypes[controlType] = ChemIQ.StructureControl.IsInstalled;
                            }

                            break;
                    }

                    Log.DebugFormat(
                        "Control type '{0}' installed={2}, OutOfProcess={1}", 
                        controlType, 
                        OutOfProcessRendering, 
                        InstalledControlTypes.ContainsKey(controlType) && InstalledControlTypes[controlType]);
                }

                if (InstalledControlTypes.ContainsKey(controlType))
                {
                    return InstalledControlTypes[controlType];
                }
            }

            return false;
        }

        #endregion

        #region Methods

        internal static string GetBundledExecutablePath(string executableFile)
        {
            string structureControlHost =
                Path.Combine(
                    Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), executableFile);
            return structureControlHost;
        }

        private static bool IsControlTypeInstalledOop(ControlType controlType)
        {
            using (StructureControl structureControl = new StructureControl(controlType, true))
            {
                return structureControl.IsInstalled;
            }
        }

        #endregion
    }
}
