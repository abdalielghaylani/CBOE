using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChemBioViz.NET
{
    public partial class AddinsDialog : Form
    {
        //---------------------------------------------------------------------
        public AddinsDialog(CBVAddinsManager mgr)
        {
            InitializeComponent();
            FillListBox(mgr);
        }
        //---------------------------------------------------------------------
        private void FillListBox(CBVAddinsManager mgr)
        {
            // columns are: Name, Method, Description
            foreach (CBVAddin addin in mgr.Addins)
            {
                Type t = addin.Type;
                Object[] vals = new Object[3];
                vals[0] = t.Name;
                vals[1] = "Default";    // later enumerate methods?
                vals[2] = addin.GetDescription();
                this.dataGridView1.Rows.Add(vals);
            }
        }
        //---------------------------------------------------------------------
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        private String StringFromRow(DataGridViewRow row)
        {
            String ans = row.Cells["NameCol"].Value.ToString();
            // later encode method name into string also
            return ans;
        }
        //---------------------------------------------------------------------
        public String GetSelectedItem()
        {
            String ans = String.Empty;
            DataGridViewSelectedRowCollection selrows = this.dataGridView1.SelectedRows;
            if (selrows.Count > 0)
                ans = StringFromRow(selrows[0]);
            return ans;
        }
    }
    //---------------------------------------------------------------------
}
