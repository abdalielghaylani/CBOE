using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spotfire.Dxp.Framework.DocumentModel;
using Spotfire.Dxp.Application.Extension;

namespace StructureSearchSupport
{
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
