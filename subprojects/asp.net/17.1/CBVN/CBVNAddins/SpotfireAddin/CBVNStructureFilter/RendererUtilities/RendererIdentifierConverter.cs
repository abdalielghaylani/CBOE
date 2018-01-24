// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererIdentifierConverter.cs" company="PerkinElmer Inc.">
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
    using Spotfire.Dxp.Framework.DocumentModel;
    using CBVNStructureFilterSupport.Framework;

    internal class RendererIdentifierConverter
    {
        #region Public Methods

        internal static ControlType ToControlType(TypeIdentifier rendererTypeIdentifier)
        {
            if (rendererTypeIdentifier.Equals(AccordRendererIdentifiers.AccordRenderer))
            {
                return ControlType.Accord;
            }

            if (rendererTypeIdentifier.Equals(ChemDrawRendererIdentifiers.ChemDrawRenderer))
            {
                return ControlType.ChemDraw;
            }

            if (rendererTypeIdentifier.Equals(MarvinRendererIdentifiers.MarvinRenderer))
            {
                return ControlType.Marvin;
            }

            if (rendererTypeIdentifier.Equals(MDLDrawRendererIdentifiers.MDLDrawRenderer))
            {
                return ControlType.MdlDraw;
            }

            if (rendererTypeIdentifier.Equals(ChemIQRendererIdentifiers.ChemIQRenderer))
            {
                return ControlType.ChemIQ;
            }

            return ControlType.MdlDraw;
        }

        public static CustomTypeIdentifier ToCustomTypeIdentifier(ControlType controlType)
        {
            if (controlType == ControlType.Accord)
            {
                return AccordRendererIdentifiers.AccordRenderer;
            }

            if (controlType == ControlType.ChemDraw)
            {
                return ChemDrawRendererIdentifiers.ChemDrawRenderer;
            }

            if (controlType == ControlType.Marvin)
            {
                return MarvinRendererIdentifiers.MarvinRenderer;
            }

            if (controlType == ControlType.MdlDraw)
            {
                return MDLDrawRendererIdentifiers.MDLDrawRenderer;
            }

            if (controlType == ControlType.ChemIQ)
            {
                return ChemIQRendererIdentifiers.ChemIQRenderer;
            }

            return MDLDrawRendererIdentifiers.MDLDrawRenderer;
        }

        #endregion
    }
}
