using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChemControls
{

    public class ChemFormulaBox : RichTextBox
    {
        EventHandler eventHandler;

        public ChemFormulaBox()
        {
            eventHandler = new EventHandler(Changed);
            TextChanged += eventHandler;
            FontChanged += eventHandler;

			this.BorderStyle = BorderStyle.FixedSingle;
			/* doesn't work; doc says "The derived class, RichTextBox, does not support the BorderStyle.FixedSingle style. 
			 * This style will cause the BorderStyle to use the BorderStyle.Fixed3D style instead."
				Other option is BorderStyle.None. You will get the look you want if you place the RichTextBox inside a 
			 * Panel that support BorderStyle.FixedSingle and set RTB.Dock=Fill + RTB.BorderStyle.None. */

            // prevent scrollbars
            this.ScrollBars = RichTextBoxScrollBars.None;
		}

        void Changed(object sender, EventArgs e)
        {
            Reformat();
        }

        public void Reformat()
        {
            TextChanged -= eventHandler;
            FontChanged -= eventHandler;
            int selStart = SelectionStart;
            int selLength = SelectionLength;

            string text = Text;
            Text = "";

            Font smallFont = new Font(Font.FontFamily, System.Convert.ToSingle(0.7 * Font.Size), Font.Style);

            int i, n = text.Length;

            //int subscriptOffset = -3;
            int superscriptOffset = System.Convert.ToInt32(Font.SizeInPoints) - 3;
            int charOffset = 0;

            for (i = 0; i < n; i++)
            {
                char c = text[i];

                if (c == '+' || c == '-')
                    charOffset = superscriptOffset;
                else if (Char.IsDigit(c) && i>0 && text[i-1] == ' ')    // supposed isotope
                    charOffset = superscriptOffset;
                else if (Char.IsDigit(c) && charOffset == 0)
                    charOffset = -3;
                else if (Char.IsLetter(c))
                    charOffset = 0;
                else
                {                                                       // do nothing
                }

                SelectionFont = charOffset == 0 ? Font : smallFont;
                SelectionCharOffset = charOffset;

                AppendText(new string(c, 1));
            }

            SelectionStart = selStart;
            SelectionLength = selLength;
            FontChanged += eventHandler;
            TextChanged += eventHandler;
        }

        public string SupportedCommands
        {
            get
            {
                string s;

                s = "Edit/Copy, EnableCopy, , Copy, ";
                s += "Edit/Paste, EnablePaste, , Paste, ";
                s += "Edit/Undo, EnableUndo, , Undo";
                return s;
            }
        }

        public bool EnableCopy { get { return SelectionLength > 0; } }
        public bool EnablePaste { get { return Clipboard.ContainsText(); } }
        public bool EnableUndo { get { return this.CanUndo; } }
    }
}
