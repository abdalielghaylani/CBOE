using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChemBioViz.NET;
using FormDBLib;

namespace CBVNEmbed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            loginTextBox.Text = "cssadmin,cssadmin";
            serverTextBox.Text = "2-Tier / cbvdev"; //  "cbvdev:123";
            formTextBox.Text = "CS Demo Props";    // public form
            queryTextBox.Text = "molname:benz";
            this.CenterToParent();
        }
        private ChemBioVizForm m_cbvForm;
        //---------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            List<String> args = new List<String>();
            if (!String.IsNullOrEmpty(formTextBox.Text))
                args.Add(formTextBox.Text);
            if (!String.IsNullOrEmpty(loginTextBox.Text))
                args.Add(String.Concat("/login=", loginTextBox.Text));
            if (!String.IsNullOrEmpty(serverTextBox.Text))
                args.Add(String.Concat("/server=", serverTextBox.Text));

            bool bHasQuery = !String.IsNullOrEmpty(queryTextBox.Text) || !this.chemDraw1.IsEmpty();
            if (!String.IsNullOrEmpty(queryTextBox.Text))
                args.Add(String.Concat("/search=", queryTextBox.Text));
            if (!this.chemDraw1.IsEmpty())
                args.Add(String.Concat("/search=structure:", chemDraw1.Base64));

            String[] sargs = new String[args.Count];
            args.CopyTo(sargs);
            CommandLine cmdLine = new CommandLine(sargs);
            cmdLine.Parse();

            m_cbvForm = new ChemBioVizForm(cmdLine);
            m_cbvForm.FormClosing += new FormClosingEventHandler(cbvForm_FormClosing);
            m_cbvForm.ActionButtonClicked += new ChemBioVizForm.ActionButtonClickedEventHandler(m_cbvForm_ActionButtonClicked);

            if (!bHasQuery)
            {
                m_cbvForm.Show();
            }
            else
            {
                m_cbvForm.OnLoadForm();
                m_cbvForm.InitForm();
            }
            ChemBioViz.NET.Properties.Settings.Default.PageSize = 50;
        }
        //---------------------------------------------------------------------
        void m_cbvForm_ActionButtonClicked(object sender, ActionButtonEventArgs e)
        {
            // look for click on menu item named "Return List"
            // get 3-column result set
            if (CBVUtilities.CBVUtil.Eqstrs(e.buttonLabel, "Return List"))
            {
                DataSet dset =  e.resultSet.GetDataSet(CBVResultSet.CBVResultFilterType.ByField, "Mol_ID,Formula,Molname");
                this.chemDataGrid1.DataSource = dset;
            }
        }
        //---------------------------------------------------------------------
        void m_cbvForm_SearchCompleted(object sender, EventArgs e)
        {
            // not currently used
            DataSet dset = m_cbvForm.Pager.CurrDataSet;
            this.chemDataGrid1.DataSource = dset;
        }
        //---------------------------------------------------------------------
        void cbvForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        //---------------------------------------------------------------------
        public static String FileToString(String sFilename, Encoding encoding)
        {
            StreamReader reader = new StreamReader(sFilename, encoding);
            String s = reader.ReadToEnd();
            reader.Close();
            return s;
        }
        //---------------------------------------------------------------------
        private void button2_Click(object sender, EventArgs e)  // Cancel
        {
            Application.Exit();
        }
        //---------------------------------------------------------------------
    }
}
