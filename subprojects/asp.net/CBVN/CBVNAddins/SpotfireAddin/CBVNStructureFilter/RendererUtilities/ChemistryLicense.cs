// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChemistryLicense.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter
{
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Framework.License;
    using CBVNStructureFilter.Properties;
    using CBVNStructureFilterSupport.Framework;

    internal sealed class ChemistryLicense : CustomLicense
    {
        // MUST be public!!!
        #region Methods

        internal static bool IsControlTypeLicensed(LicenseManager licenseManager, ControlType controlType)
        {
            if (licenseManager != null)
            {
                switch (controlType)
                {
                    case ControlType.Accord:
                        return licenseManager.IsEnabled(Functions.AccordFunction);

                    case ControlType.ChemDraw:
                        return licenseManager.IsEnabled(Functions.ChemDrawFunction);

                    case ControlType.Marvin:
                        return licenseManager.IsEnabled(Functions.MarvinFunction);

                    case ControlType.MdlDraw:
                        return licenseManager.IsEnabled(Functions.MDLDrawFunction);

                    case ControlType.ChemIQ:
                        return licenseManager.IsEnabled(Functions.ChemIQFunction);
                }
            }

            return false;
        }

        #endregion

        public new sealed class Functions : CustomLicense.Functions
        {
            #region Constants and Fields

            public static readonly LicensedFunction AccordFunction = CreateLicensedFunction(
                "Accord", InvariantResources.Accord, string.Format(Resources.RendererDescription, InvariantResources.Accord));

            public static readonly LicensedFunction ChemDrawFunction = CreateLicensedFunction(
                "ChemDraw", InvariantResources.ChemDraw, string.Format(Resources.RendererDescription, InvariantResources.ChemDraw));

            public static readonly LicensedFunction ChemistryFunction =
                CreateLicensedFunction(
                    "ChemistryViewer", Resources.ChemistryFunction, Resources.ChemistryFunctionDescription);

            public static readonly LicensedFunction ExportToSDFFunction =
                CreateLicensedFunction(
                    "ExportToSDF", 
                    Resources.ExportToSDFFunction, 
                    Resources.ExportToSDFFunctionDescription);

            public static readonly LicensedFunction MarvinFunction = CreateLicensedFunction(
                "Marvin", InvariantResources.Marvin, string.Format(Resources.RendererDescription, InvariantResources.Marvin));

            public static readonly LicensedFunction MDLDrawFunction = CreateLicensedFunction(
                "MDL® Draw", InvariantResources.SymyxDraw, string.Format(Resources.RendererDescription, InvariantResources.SymyxDraw));

            public static readonly LicensedFunction SDFParserFunction = CreateLicensedFunction(
                "SDFParser", Resources.SDFParser, Resources.SDFParserDescription);

            public static readonly LicensedFunction StructureSearchFunction =
                CreateLicensedFunction(
                    "StructureSearch", 
                    Resources.StructureSearchFunction, 
                    Resources.StructureSearchFunctionDescription);

            public static readonly LicensedFunction ChemIQFunction = CreateLicensedFunction(
                "ChemIQ", InvariantResources.ChemIQ, string.Format(Resources.RendererDescription, InvariantResources.ChemIQ));


            public static readonly LicensedFunction ExportToCDExcelFunction =
                CreateLicensedFunction(
                    "ExportToCDExcel",
                    Resources.ExportToCDExcelFunction,
                    Resources.ExportToCDExcelFunctionDescription);

            public static readonly LicensedFunction StructureFilterFunction =
                CreateLicensedFunction(
                    "StructureFilter",
                    Resources.StructureFilterFunction,
                    Resources.StructureFilterFunctionDescription);

            #endregion
        }
    }
}
