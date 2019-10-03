using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Timers;

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
	public partial class AboutBox : Form
    {
        const String INSTITUTIONAL_LINK = "http://www.perkinelmer.com/";
        const String INSTITUTIONAL_SUPPORT_LINK = "http://www.perkinelmer.com/support/";

        private Assembly _assembly;

        /// <summary>
        /// Help popup for this applciation.
        /// </summary>
        public AboutBox()
		{
			InitializeComponent();
            SetDialogConfiguration();
        }

        private void SetDialogConfiguration()
        {
            this.CenterToParent();

            _assembly = System.Reflection.Assembly.GetExecutingAssembly();

            this.Text = this.GetAssemblyTitle();
            this.lblProductName.Text = Application.ProductName;
            this.lblBuildDate.Text = this.GetAssemblyVersionCreationTime();
            this.lblVersion.Text = this.GetAssemblyFileVersion();
            this.lblCopyright.Text = this.GetAssemblyCopyright();
            this.Icon = Properties.Resources.DL;
        }

        /// <summary>
        ///   Gets the executing assembly version number
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyFileVersion()
        {
            string retVal = "Unknown";
            object[] fileversionAttributte;
            if (_assembly != null)
            {
                fileversionAttributte = _assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
                if (fileversionAttributte != null && fileversionAttributte.Length > 0)
                    retVal = ((System.Reflection.AssemblyFileVersionAttribute)fileversionAttributte[0]).Version;
            }
            return retVal;
        }

        /// <summary>
        ///   Gets the executing assembly creation date
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyVersionCreationTime()
        {
            return File.GetCreationTime(_assembly.Location).ToString();
        }

        private string GetAssemblyCopyright()
        {
            AssemblyCopyrightAttribute attribute =
                (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(
                    _assembly, typeof(AssemblyCopyrightAttribute));
            return attribute.Copyright;
        }

        private string GetAssemblyTitle()
        {
            AssemblyTitleAttribute attribute =
                (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(
                    _assembly, typeof(AssemblyTitleAttribute));
            return attribute.Title;
        }

        /// <summary>
        /// Launch the CambridgeSoft URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void institutionalLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.institutionalLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start(INSTITUTIONAL_LINK);
        }

        /// <summary>
        /// Launch the CambridgeSoft product support URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void institutionalSupportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.institutionalLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start(INSTITUTIONAL_SUPPORT_LINK);
        }

        /// <summary>
        /// Close the form (DialogResult.Cancel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
