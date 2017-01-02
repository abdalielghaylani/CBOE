using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using System.Drawing;

namespace ChemControls
{
    public class EditCommandsHandler
    {
        public void UpdateCut(UltraToolbarsManager manager, TextBox t)
        {
            manager.Tools["Cut"].SharedProps.Enabled = t.SelectionLength > 0;
        }
        public void UpdateCopy(UltraToolbarsManager manager, TextBox t)
        {
            manager.Tools["Copy"].SharedProps.Enabled = t.SelectionLength > 0;
        }
        public void UpdatePaste(UltraToolbarsManager manager, TextBox t)
        {
            manager.Tools["Paste"].SharedProps.Enabled = Clipboard.GetText() != null;
        }

        public void ExecuteCut(TextBox t) { t.Cut(); }
        public void ExecuteCopy(TextBox t) { t.Copy(); }
        public void ExecutePaste(TextBox t) { t.Paste(); }

        public void UpdateCut(UltraToolbarsManager manager, RichTextBox t)
        {
            manager.Tools["Cut"].SharedProps.Enabled = t.SelectionLength > 0;
        }
        public void UpdateCopy(UltraToolbarsManager manager, RichTextBox t)
        {
            manager.Tools["Copy"].SharedProps.Enabled = t.SelectionLength > 0;
        }
        public void UpdatePaste(UltraToolbarsManager manager, RichTextBox t)
        {
            manager.Tools["Paste"].SharedProps.Enabled = Clipboard.GetText() != null;
        }

        public void ExecuteCut(RichTextBox t) { t.Cut(); }
        public void ExecuteCopy(RichTextBox t) { t.Copy(); }
        public void ExecutePaste(RichTextBox t) { t.Paste(); }

        public void UpdateUndo(UltraToolbarsManager manager, TextBox t)
        {
            manager.Tools["Undo"].SharedProps.Enabled = t.CanUndo;
        }

        public void ExecuteUndo(TextBox t) { t.Undo(); }

        public void UpdateUndo(UltraToolbarsManager manager, RichTextBox t)
        {
            manager.Tools["Undo"].SharedProps.Enabled = t.CanUndo;
        }
        public void ExecuteUndo(RichTextBox t) { t.Undo(); }

        public void UpdateRedo(UltraToolbarsManager manager, RichTextBox t)
        {
            manager.Tools["Redo"].SharedProps.Enabled = t.CanRedo;
        }
        public void ExecuteRedo(RichTextBox t) { t.Redo(); }

        public void UpdateFontName(UltraToolbarsManager manager, TextBox t)
        {
            FontListTool tool = manager.Tools["FontName"] as FontListTool;

            if (tool == null)
                return;

            if (tool.Value.ToString() != t.Font.FontFamily.Name)   // prevent loop
                tool.Value = t.Font.FontFamily.Name;
        }

        public void ExecuteFontName(TextBox t, object o)
        {
            string name = o as string;
            Font oldFont = t.Font;
            if (oldFont.FontFamily.Name != name)
            {
                //Coverity Bug Fix CID 11802 
                using (FontFamily ff = new FontFamily(name))
                {
                    using (Font newFont = new Font(ff, oldFont.Size, oldFont.Style))
                    {
                        t.Font = newFont;
                    }
                }
            }
            t.Focus();
        }

        public void UpdateFontName(UltraToolbarsManager manager, RichTextBox t)
        {
            FontListTool tool = manager.Tools["FontName"] as FontListTool;

            if (tool == null)
                return;

            if (t.SelectionFont == null)    // do nothing
            {
            }
            else
            {
                if (tool.Value.ToString() != t.SelectionFont.FontFamily.Name)   // prevent loop
                    tool.Value = t.SelectionFont.FontFamily.Name;
            }
        }

        public void ExecuteFontName(RichTextBox t, object o)
        {
            string name = o as string;
            Font oldFont = t.SelectionFont;
            //Coverity Bug Fix CID 11803 
            using (FontFamily ff = new FontFamily(name))
            {
                if (oldFont == null)
                {
                    using (Font newFont = new Font(ff, t.Font.Size, t.Font.Style))
                    {
                        t.SelectionFont = newFont;
                    }
                }
                else if (oldFont.FontFamily.Name != name)
                {
                    using (Font newFont = new Font(ff, oldFont.Size, oldFont.Style))
                    {
                        t.SelectionFont = newFont;
                    }
                }
            }
            t.Focus();
        }

        public void UpdateBold(UltraToolbarsManager manager, RichTextBox t)
        {
            StateButtonTool tool = manager.Tools["Bold"] as StateButtonTool;
            //Coverity Bug Fix :CID 11705 /11422 
            Font oldFont = t.SelectionFont;
            if (tool != null && oldFont != null)
            {
                if (tool.Checked != oldFont.Bold)
                    tool.Checked = oldFont.Bold;
            }
        }

        public void ExecuteBold(RichTextBox t)
        {
            //Coverity Bug Fix CID 11801 
            Font oldFont = t.SelectionFont;
            if (oldFont == null)
            {
                using (FontFamily fontFamily = new FontFamily(t.Font.Name))
                {
                    Font newFont = new Font(fontFamily, t.Font.Size, t.Font.Style | FontStyle.Bold);
                    t.SelectionFont = newFont;
                }
            }
            else
            {
                using (FontFamily fontFamily = new FontFamily(oldFont.Name))
                {
                    Font newFont = new Font(fontFamily, oldFont.Size, oldFont.Style ^ FontStyle.Bold);
                    t.SelectionFont = newFont;
                }
            }

        }
    }
}
