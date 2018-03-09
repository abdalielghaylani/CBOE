using System;
using System.Collections.Generic;
using System.Text;
using ChemDrawControlConst11;
using System.Reflection;
using CBVNStructureFilterSupport.Framework;

namespace CBVNStructureFilterSupport.ChemDraw
{
    /// <summary>
    /// ChemDrawRenderSettings
    /// </summary>
    [Serializable]
    public class ChemDrawRenderSettings : RenderSettings
    {
        ///// <summary>
        ///// AminoAcidTermini
        ///// </summary>
        //public CDAminoAcidTermini AminoAcidTermini { get; set; }

        /// <summary>
        /// BackgroundColor
        /// </summary>
        public uint BackgroundColor { get; set; }

        /// <summary>
        /// BoldWidth
        /// </summary>
        public double BoldWidth { get; set; }

        /// <summary>
        /// BondLength
        /// </summary>
        public double BondLength { get; set; }

        /// <summary>
        /// BondSpacing
        /// </summary>
        public double BondSpacing { get; set; }

        /// <summary>
        /// CaptionFace
        /// </summary>
        public short CaptionFace { get; set; }

        /// <summary>
        /// CaptionFont
        /// </summary>
        public string CaptionFont { get; set; }

        /// <summary>
        /// CaptionJustification
        /// </summary>
        public CDJustification CaptionJustification { get; set; }

        /// <summary>
        /// CaptionLineHeight
        /// </summary>
        public double CaptionLineHeight { get; set; }

        /// <summary>
        /// CaptionSize
        /// </summary>
        public double CaptionSize { get; set; }

        /// <summary>
        /// ChainAngle
        /// </summary>
        public short ChainAngle { get; set; }

        ///// <summary>
        ///// Color
        ///// </summary>
        //public uint Color { get; set; }

        /// <summary>
        /// HashSpacing
        /// </summary>
        public double HashSpacing { get; set; }

        /// <summary>
        /// HideImplicitHydrogens
        /// </summary>
        public bool HideImplicitHydrogens { get; set; }

        /// <summary>
        /// InterpretChemically
        /// </summary>
        public bool InterpretChemically { get; set; }

        /// <summary>
        /// LabelFace
        /// </summary>
        public short LabelFace { get; set; }

        /// <summary>
        /// LabelFont
        /// </summary>
        public string LabelFont { get; set; }

        /// <summary>
        /// LabelJustification
        /// </summary>
        public CDJustification LabelJustification { get; set; }

        /// <summary>
        /// LabelLineHeight
        /// </summary>
        public double LabelLineHeight { get; set; }

        /// <summary>
        /// LabelSize
        /// </summary>
        public double LabelSize { get; set; }

        /// <summary>
        /// LineWidth
        /// </summary>
        public double LineWidth { get; set; }

        /// <summary>
        /// MarginWidth
        /// </summary>
        public double MarginWidth { get; set; }

        ///// <summary>
        ///// NumberResidueBlocks
        ///// </summary>
        //public bool NumberResidueBlocks { get; set; }

        ///// <summary>
        ///// ResidueBlockCount
        ///// </summary>
        //public short ResidueBlockCount { get; set; }

        ///// <summary>
        ///// ResidueWrapCount
        ///// </summary>
        //public short ResidueWrapCount { get; set; }

        ///// <summary>
        ///// ResidueZigZag
        ///// </summary>
        //public bool ResidueZigZag { get; set; }

        /// <summary>
        /// ShowAtomEnhancedStereo
        /// </summary>
        public bool ShowAtomEnhancedStereo { get; set; }

        /// <summary>
        /// ShowAtomNumber
        /// </summary>
        public bool ShowAtomNumber { get; set; }

        /// <summary>
        /// ShowAtomQuery
        /// </summary>
        public bool ShowAtomQuery { get; set; }

        /// <summary>
        /// ShowAtomStereo
        /// </summary>
        public bool ShowAtomStereo { get; set; }

        /// <summary>
        /// ShowBondQuery
        /// </summary>
        public bool ShowBondQuery { get; set; }

        /// <summary>
        /// ShowBondRxn
        /// </summary>
        public bool ShowBondRxn { get; set; }

        /// <summary>
        /// ShowBondStereo
        /// </summary>
        public bool ShowBondStereo { get; set; }

        /// <summary>
        /// ShowNonTerminalCarbonLabels
        /// </summary>
        public bool ShowNonTerminalCarbonLabels { get; set; }

        /// <summary>
        /// ShowSequenceBonds
        /// </summary>
        public bool ShowSequenceBonds { get; set; }

        /// <summary>
        /// ShowSequenceTermini
        /// </summary>
        public bool ShowSequenceTermini { get; set; }

        /// <summary>
        /// ShowTerminalCarbonLabels
        /// </summary>
        public bool ShowTerminalCarbonLabels { get; set; }

        /// <summary>
        /// Get the default settings
        /// </summary>
        /// <returns>the default settings</returns>
        public static ChemDrawRenderSettings GetDefaultSettings()
        {
            ChemDrawRenderSettings settings = new ChemDrawRenderSettings();

            //settings.AminoAcidTermini = CDAminoAcidTermini.kCDAminoAcidNH2COOH;
            settings.BackgroundColor = 16777215;
            settings.BoldWidth = 2;
            settings.BondLength = 14.3999938964844;
            settings.BondSpacing = 18.5;
            settings.CaptionFace = 0;
            settings.CaptionFont = "Arial";
            settings.CaptionJustification = CDJustification.kCDJustificationLeft;
            settings.CaptionLineHeight = 0.05;
            settings.CaptionSize = 10;
            settings.ChainAngle = 120;
            //settings.Color = 0;
            settings.HashSpacing = 2.5;
            settings.HideImplicitHydrogens = false;
            settings.InterpretChemically = true;
            settings.LabelFace = 96;
            settings.LabelFont = "Arial";
            settings.LabelJustification = CDJustification.kCDJustificationAuto;
            settings.LabelLineHeight = 0;
            settings.LabelSize = 10;
            settings.LineWidth = 0.599990844726563;
            settings.MarginWidth = 1.59999084472656;
            settings.ShowAtomEnhancedStereo = true;
            settings.ShowAtomNumber = false;
            settings.ShowAtomQuery = true;
            settings.ShowAtomStereo = false;
            settings.ShowBondQuery = true;
            settings.ShowBondRxn = true;
            settings.ShowBondStereo = false;
            settings.ShowNonTerminalCarbonLabels = false;
            settings.ShowSequenceBonds = true;
            settings.ShowSequenceTermini = true;
            settings.ShowTerminalCarbonLabels = false;

            return settings;
        }

        /// <summary>
        /// the equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ChemDrawRenderSettings settings = obj as ChemDrawRenderSettings;
            if (settings == null)
            {
                return false;
            }
            PropertyInfo[] properties = typeof(ChemDrawRenderSettings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object objValue = property.GetValue(settings, null);
                object thisValue = property.GetValue(this, null);
                if (!objValue.Equals(thisValue))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
