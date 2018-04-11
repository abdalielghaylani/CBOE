using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ChemBioViz.NET
{
    public partial class RadioList : ListBox
    {
        #region Variables
        private StringFormat Align;
        private bool IsTransparent = false;
        private Brush BackBrush;
        #endregion

        #region Properties
        [Browsable(false)]
        public override SelectionMode SelectionMode
        {
            get
            {
                return base.SelectionMode;
            }
            set
            {
                if (value != SelectionMode.One)
                    throw new Exception("Invalid value for SelectionMode property");
                else
                    base.SelectionMode = value;
            }
        }

        // Hides these properties in the designer
        [Browsable(false)]
        public override DrawMode DrawMode
        {
            get
            {
                return base.DrawMode;
            }
            set
            {
                if (value != DrawMode.OwnerDrawFixed)
                    throw new Exception("Invalid value for DrawMode property");
                else
                    base.DrawMode = value;
            }
        }

        // Allows the BackColor to be transparent
        public override Color BackColor
        {
            get
            {
                if (IsTransparent)
                    return Color.Transparent;
                else
                    return base.BackColor;
            }
            set
            {
                if (value == Color.Transparent)
                {
                    IsTransparent = true;
                    base.BackColor = (this.Parent == null) ? SystemColors.Window : this.Parent.BackColor;
                }
                else
                {
                    IsTransparent = false;
                    base.BackColor = value;
                }

                if (this.BackBrush != null)
                    this.BackBrush.Dispose();
                BackBrush = new SolidBrush(base.BackColor);

                Invalidate();
            }
        }
        #endregion

        #region Constructors
        public RadioList()
        {
            this.SelectionMode = SelectionMode.One;

            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = this.FontHeight;
            this.Align = new StringFormat(StringFormat.GenericDefault);
            this.Align.LineAlignment = StringAlignment.Center;
            this.BackColor = this.BackColor;
        }
        #endregion

        #region Methods
        // Prevent background erasing
        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 0x0014)  // WM_ERASEBKGND
            {
                m.Result = (IntPtr)1; // avoid default background erasing
                return;
            }

            base.DefWndProc(ref m);
        }
        #endregion

        #region Events
        // Main paiting method
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            int maxItem = this.Items.Count - 1;

            if (e.Index < 0 || e.Index > maxItem)
            {
                // Erase all background if control has no items
                e.Graphics.FillRectangle(BackBrush, this.ClientRectangle);
                return;
            }

            int size = e.Font.Height; // button size depends on font height, not on item height

            // Calculate bounds for background, if last item paint up to bottom of control
            Rectangle backRect = e.Bounds;
            if (e.Index == maxItem)
                backRect.Height = this.ClientRectangle.Top + this.ClientRectangle.Height - e.Bounds.Top;
            e.Graphics.FillRectangle(BackBrush, backRect);

            // Determines text color/brush
            Brush textBrush;
            bool isChecked = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            RadioButtonState state = isChecked ? RadioButtonState.CheckedNormal : RadioButtonState.UncheckedNormal;
            if ((e.State & DrawItemState.Disabled) == DrawItemState.Disabled)
            {
                textBrush = SystemBrushes.GrayText;
                state = isChecked ? RadioButtonState.CheckedDisabled : RadioButtonState.UncheckedDisabled;
            }
            else if ((e.State & DrawItemState.Grayed) == DrawItemState.Grayed)
            {
                textBrush = SystemBrushes.GrayText;
                state = isChecked ? RadioButtonState.CheckedDisabled : RadioButtonState.UncheckedDisabled;
            }
            else
            {
                textBrush = SystemBrushes.FromSystemColor(this.ForeColor);
            }

            // Determines bounds for text and radio button
            Size glyphSize = RadioButtonRenderer.GetGlyphSize(e.Graphics, state);
            Point glyphLocation = new Point(e.Bounds.Location.X + 3, e.Bounds.Location.Y + 3);
            glyphLocation.Y += (e.Bounds.Height - glyphSize.Height) / 2;

            Rectangle bounds = new Rectangle(e.Bounds.X + glyphSize.Width, e.Bounds.Y, e.Bounds.Width - glyphSize.Width, e.Bounds.Height);

            // Draws the radio button
            RadioButtonRenderer.DrawRadioButton(e.Graphics, new Point(e.Bounds.Location.X, e.Bounds.Location.Y + 1), state);

            // Draws the text
            if (!string.IsNullOrEmpty(DisplayMember)) // Bound Datatable? Then show the column written in Displaymember
                e.Graphics.DrawString(((System.Data.DataRowView)this.Items[e.Index])[this.DisplayMember].ToString(),
                    e.Font, textBrush, bounds, this.Align);
            else
                e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, textBrush, bounds, this.Align);

            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }
        // Other event handlers
        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.FontHeight > this.ItemHeight)
                this.ItemHeight = this.FontHeight;

            base.OnHandleCreated(e);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            if (this.FontHeight > this.ItemHeight)
                this.ItemHeight = this.FontHeight;
            Update();
        }
        protected override void OnParentChanged(EventArgs e)
        {
            // Force to change backcolor
            this.BackColor = this.BackColor;
        }
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            // Force to change backcolor
            this.BackColor = this.BackColor;
        }
        #endregion

    }
}
