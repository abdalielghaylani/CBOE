using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using CBVNStructureFilter;
using CBVNStructureFilter.Properties;
using CBVNStructureFilterSupport.Framework;

namespace CBVNStructureFilter
{
    class HistoricalDropDown
    {
        public const int ImageHeight = 50;
        public const int ImageWidth = 68;

        public static void DrawItem(DrawItemEventArgs e, ComboBox previousSearches, StructureControlBase structureControl)
        {
            e.DrawBackground();
            var settings = (SavedSearchSettings)previousSearches.Items[e.Index];
            if (e.Bounds.Height < ImageHeight)
            {
                DrawText(e, settings, true);
            }
            else
            {
                DrawStructureAndText(e, structureControl, settings);
            }
            
            e.DrawFocusRectangle();
        }

        private static void DrawText(DrawItemEventArgs e, SavedSearchSettings settings, bool singleLine)
        {
            var foreColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : SystemColors.WindowText;
            var backColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : SystemColors.Window;

            var dateBits = settings.Name.Split(' ');
            var info = singleLine
                           ? settings.Name
                           : string.Format("{0}\r\n{1} {2}", dateBits[0], dateBits[1],
                                           dateBits.Length >= 3 ? dateBits[2] : string.Empty);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var rect = new Rectangle(singleLine ? e.Bounds.Left : ImageWidth + 5, e.Bounds.Top + 1,
                                     singleLine ? e.Bounds.Width : e.Bounds.Width - ImageWidth,
                                     e.Bounds.Height - 2);
            TextRenderer.DrawText(e.Graphics, info, new Font("Microsoft Sans Serif", 8.25F), rect, foreColor, backColor,
                                  TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
        }

        private static void DrawStructureAndText(DrawItemEventArgs e, StructureControlBase structureControl, SavedSearchSettings settings)
        {
            var oldMolString = structureControl.MolFileString;
            var oldCdxData = structureControl.CDXData;
            if (settings.RGroupDecomposition)
            {
                if (settings.RGDCDXStructureData == null)
                {
                    structureControl.MolFileString = settings.RGDStructure;
                }
                else
                {
                    structureControl.CDXData = settings.RGDCDXStructureData;
                }
            }
            else
            {
                if (settings.CDXStructureData == null)
                {
                    structureControl.MolFileString = settings.FilterStructure;
                }
                else
                {
                    structureControl.CDXData = settings.CDXStructureData;
                }
            }

            e.Graphics.DrawImage(structureControl.Image, e.Bounds.Left, e.Bounds.Top + 1, ImageWidth, e.Bounds.Height - 2);
            structureControl.MolFileString = oldMolString;
            structureControl.CDXData = oldCdxData;

            DrawText(e, settings, false);
        }
    }
}
