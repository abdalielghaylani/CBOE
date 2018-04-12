using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.DataLoaderGUI.Common
{
    /// <summary>
    /// (Failed) attempt to fix DateTimePicker bug in not supporting BackColor and ForeColor
    /// And the fact the the downarrow does not initially appear
    /// </summary>
    public partial class COEDateTimePicker : DateTimePicker
    {
        #region constructors
        /// <summary>
        /// Constructor. Sets up for UserPaint.
        /// </summary>
        public COEDateTimePicker()
        {
            SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();
        } // COEDateTimePicker()
        #endregion

        #region event overrides
        /// <summary>
        /// Attempt to fix the fact that DateTimePicker does not support BackColor and ForeColor
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), 0, 2);
            ControlPaint.DrawScrollButton(e.Graphics, new Rectangle(new Point(ClientRectangle.Right - 16, ClientRectangle.Left), new Size(16, ClientRectangle.Height)), ScrollButton.Down, ButtonState.Normal);

        } // OnPaint()
        #endregion

    } // class COEDateTimePicker
}
