using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FormDBLib;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Timers;

using CBVUtilities;

namespace ChemBioViz.NET
{
	public partial class AboutBox : Form
    {
        #region Constructors
        public AboutBox()
		{
			InitializeComponent();
            SetDialogConfiguration();
            InitNameList();
        }
        #endregion

        #region Methods
        private void SetDialogConfiguration()
        {
            this.CenterToParent();

            versionCreationTimeLabel.Text = this.GetCBVNFAssemblyVersionCreationTime();
            version.Text = this.GetCBVNAssemblyFileVersion();
        }
        //------------------------------------------------------------------------------
        /// <summary>
        ///   Gets the executing assembly version number
        /// </summary>
        /// <returns></returns>
        private string GetCBVNAssemblyFileVersion()
        {
            string retVal = "Unknown";
            object[] fileversionAttributte;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                fileversionAttributte = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
                if (fileversionAttributte != null && fileversionAttributte.Length > 0)
                    retVal = ((System.Reflection.AssemblyFileVersionAttribute)fileversionAttributte[0]).Version;
            }
            return retVal;
        }
        //------------------------------------------------------------------------------
        /// <summary>
        ///   Gets the executing assembly creation date
        /// </summary>
        /// <returns></returns>
        private string GetCBVNFAssemblyVersionCreationTime()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return File.GetCreationTime(assembly.Location).ToString();
        }
        #endregion

        #region Events
        private void institutionalLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.institutionalLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start(CBVConstants.INSTITUTIONAL_LINK);
        }
        //------------------------------------------------------------------------------
        private void institutionalSupportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.institutionalLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start(CBVConstants.INSTITUTIONAL_SUPPORT_LINK);
        }
        //---------------------------------------------------------------------
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //---------------------------------------------------------------------
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            this.versionCreationTimeLabel.Text = this.m_nameGen.GetNextName();
        }
        #endregion

        #region Easter Egg
        //---------------------------------------------------------------------
        private System.Timers.Timer m_timer = null;
        private NameGenerator m_nameGen;
        private List<String> m_namelist;

        private void InitNameList()
        {
            m_namelist = new List<string> {
            #region TeamNames
                "David Gosalvez, director",
                "Megean Schoenberg, manager",
                "Jim Dill, developer",
                "Silvana Santos, developer",
                "Franco Fiorini, tester",
                "Vikas Bhatti, engineer",
                "Facundo Galdeano, engineer",
                "Andras Furst, contributor",
                "Julie Nelson, contributor"
            #endregion
            };
            m_nameGen = new NameGenerator(m_namelist);
        }
        //---------------------------------------------------------------------
        private void StartTimer()
        {
            double dInterval = 2500;    // ms
            m_timer = new System.Timers.Timer(dInterval);
            m_timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            m_timer.SynchronizingObject = this;
            m_timer.AutoReset = true;
            m_timer.Enabled = true;     // starts timer
        }
        //---------------------------------------------------------------------
        private void logoPictureBox_Click(object sender, EventArgs e)
        {
            // the easter egg
            this.version.Text = "The ChemBioViz Team";
            this.versionCreationTimeLabel.Text = "(in random order)";
            StartTimer();
        }
        #endregion
    }
}
