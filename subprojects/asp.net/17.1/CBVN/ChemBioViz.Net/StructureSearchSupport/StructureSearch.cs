using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace StructureSearchSupport
{
    public class StructureSearch
    {
        #region Methods

        /// <summary>
        /// Generating Renderer list from ControlType enumerator
        /// </summary>
        /// <returns>Renderer list</returns>
        public Dictionary<string, string> GetRenderersMenu()
        {
            Dictionary<string, string> _rendererList = new Dictionary<string, string>(); 
            foreach (ControlType controlType in Enum.GetValues(typeof(ControlType)))
            {
                if (controlType != ControlType.None)
                {
                    //if (IsControlTypeInstalled(controlType))
                    //{
                        _rendererList.Add(controlType.ToString(), RendererIdentifierConverter.ToCustomTypeIdentifier(controlType).DisplayName);
                    //}
                }
            }
            return _rendererList;
        }

        /// <summary>
        /// Generating Editors list from ControlType enumerator
        /// </summary>
        /// <returns>Editors list</returns>
        public Dictionary<string, string> GetEditorsMenu()
        {
            Dictionary<string, string> _rendererList = new Dictionary<string, string>();
            foreach (ControlType controlType in Enum.GetValues(typeof(ControlType)))
            {
                if (controlType != ControlType.None)
                {
                    //if (IsControlTypeInstalled(controlType))
                    //{
                    _rendererList.Add(controlType.ToString(), RendererIdentifierConverter.ToCustomTypeIdentifier(controlType).DisplayName);
                    //}
                }
            }
            return _rendererList;
        }

        #endregion

        #region Structure methods (Methods taken from Lead Discovery and are modified)

        public static ProcessHost GetEditingHost(ControlType controlType)
        {
            //if (!ControlTypeSupportsEditing(controlType))
            //{
            //    return null;
            //}

            string executableName;
            switch (controlType)
            {
                case ControlType.ChemDraw:
                    executableName = "Spotfire.Dxp.LeadDiscovery.ChemDrawEditor.exe";
                    break;
                case ControlType.MdlDraw:
                    executableName = "Spotfire.Dxp.LeadDiscovery.MdlDrawEditor.exe";
                    break;
                //case ControlType.Marvin:
                //    ProcessHost processHost = new MarvinSketchProcessHost();
                //    return processHost;

                default:
                    throw new ArgumentOutOfRangeException("controlType");
            }

            string fullPath = GetBundledExecutablePath(executableName); // @"C:\From DTP090-A\Sources\P4\Spotfire\Spotfire\Spotfire\Spotfire.Dxp.LeadDiscovery\build\AnyCPU\Spotfire.Dxp.LeadDiscovery.ChemDrawEditor.exe";// GetBundledExecutablePath(executableName);

            ProcessHost host = new NetProcessHost(fullPath);
            return host;
        }

        internal static string GetBundledExecutablePath(string executableFile)
        {
            string structureControlHost =
                Path.Combine(
                    Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), executableFile);
            return structureControlHost;
        }

        public string GetStructureTypeStringFromEnum(StructureStringType sStringType)
        {
            // TODO why are these handled differently?
            switch (sStringType)
            {
                case StructureStringType.Molfile:
                    return StructureStringType.Molfile.ToString();
                case StructureStringType.CDX:
                    return Identifiers.CDXContentType;
                default:
                    throw new InvalidOperationException("Invalid string type");
            }
        }

        public string ConvertCdxToMol(string cdx)
        {
            return (string)(ChemDrawInvoker.ConvertStructure(cdx,
                                new[] { Identifiers.MolfileContentType })
                      [Identifiers.MolfileContentType]);
        }

        #endregion
    }
}
