using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace ChemBioVizExcelAddIn
{
    public partial class frmAbout : Form
    {
        Process lnkProcess = new Process();
        public frmAbout()
        {
            InitializeComponent();
            InitializeEventAndLinks();
        }

        private void InitializeEventAndLinks()
        {
            this.Load+=new EventHandler(frmAbout_Load);
            this.btnOK.Click+=new EventHandler(btnOK_Click);

            this.llblSupportServices.Links.Add(0, 30, "www.cambridgesoft.com/support/");
           
            this.llblSupportServices.LinkClicked+=new LinkLabelLinkClickedEventHandler(llblSupportServices_LinkClicked);

            //this.llblSupportServices2.Links.Add(0, 25, "mailto:support@cambridgesoft.com");

           // this.llblSupportServices2.LinkClicked+=new LinkLabelLinkClickedEventHandler(llblSupportServices2_LinkClicked);

            this.llblOrderInfo1.Links.Add(0, 21, "www.cambridgesoft.com");
            this.llblOrderInfo1.LinkClicked+=new LinkLabelLinkClickedEventHandler(llblOrderInfo1_LinkClicked);

            //this.llblOrderInfo2.Links.Add(0, 22, "mailto:info@cambridgesoft.com");
           // this.llblOrderInfo2.LinkClicked += new LinkLabelLinkClickedEventHandler(llblOrderInfo2_LinkClicked);


        }

        #region "Events"
        private void frmAbout_Load(object sender, EventArgs e)
        {
            LoadAssemblyInfo();
            this.btnOK.Focus();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();  
        }
        private void llblSupportServices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                llblSupportServices.Links[llblSupportServices.Links.IndexOf(e.Link)].Visited = true;

                Process.Start(e.Link.LinkData.ToString());
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

        }

        //private void llblSupportServices2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    try
        //    {
        //        llblSupportServices2.Links[llblSupportServices2.Links.IndexOf(e.Link)].Visited = true;

        //        Process.Start(e.Link.LinkData.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        CBVExcel.ErrorLogging(ex.Message);
        //    }

        //}


        private void llblOrderInfo1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                llblOrderInfo1.Links[llblOrderInfo1.Links.IndexOf(e.Link)].Visited = true;

                Process.Start(e.Link.LinkData.ToString());
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }
        //private void llblOrderInfo2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    try
        //    {
        //        llblOrderInfo2.Links[llblOrderInfo2.Links.IndexOf(e.Link)].Visited = true;

        //        Process.Start(e.Link.LinkData.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        CBVExcel.ErrorLogging(ex.Message);
        //    }
        //}
        #endregion "Events"

        #region "Methods"
        private void LoadAssemblyInfo()
        {
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            this.lblHeader.Text = fileInfo.ProductName;
            // 11.0.4 - Getting the Assembly File Version instead of Product version
            //this.lblVersion.Text = fileInfo.ProductVersion;
            this.lblVersion.Text = fileInfo.FileVersion;
            this.lblCopyRight.Text = fileInfo.LegalCopyright;
            this.lblCurrentDataTime.Text = System.DateTime.Now.ToString();

        }
        #endregion "Methods"
    }
}