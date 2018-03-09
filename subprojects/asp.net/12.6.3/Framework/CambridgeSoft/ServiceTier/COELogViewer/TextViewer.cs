using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COELogViewer
{
    public partial class TextViewer : Form
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public TextViewer(string title, string message): this()
        {
            this.Text = title;

            this.DisplayTextBox.Lines = message.Split('\n');
        }
    }
}