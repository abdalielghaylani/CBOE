using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormWizard
{
    public partial class WaitForm : Form
    {
        public WaitForm()
        {
            InitializeComponent();
        }

        public void SetMessage(string messageText)
        {
            this.Text = FormWizard.Properties.Resources.FORM_TITLE;
            dispalyLabel.Text = messageText;
        }
    }
}
