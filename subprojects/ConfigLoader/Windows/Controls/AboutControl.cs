using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Controls
{
    /// <summary>
    /// Intended to be the sole control placed on an About popup, this user control automatically derives
    /// the assembly (product) information from the AssemblyInfo of the containing application. Multiple
    /// constructors enable customization by the developer.
    /// 
    /// The parent form will automatically have its Text (caption-bar) value set using the assembly's title,
    /// and its icon can be set here as well.
    /// </summary>
    public partial class AboutControl : UserControl
    {
        const String INSTITUTION_LINK = "http://www.cambridgesoft.com/";
        const String SUPPORT_LINK = "http://www.cambridgesoft.com/support/";

        private Assembly _assembly;

        #region >Properties<

        PictureBox _logoBox;

        string _productName = string.Empty;
        string _buildDate = string.Empty;
        string _version = string.Empty;
        string _copyright = string.Empty;
        bool _dictatesToParentForm = true;

        private string _institutionLink = string.Empty;
        private string _supportLink = string.Empty;

        /// <summary>
        /// The picturebox is left publically available for the caller to provide
        /// a logo image, resize the control to fit a particular image, or hide the
        /// control altogether.
        /// </summary>
        public PictureBox LogoBox
        {
            get { return _logoBox; }
        }

        /// <summary>
        /// The product's name, as derived from Application.ProductName
        /// </summary>
        public string CustomProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        /// <summary>
        /// The creation date of the assembly file
        /// </summary>
        public string BuildDate
        {
            get { return _buildDate; }
            set { _buildDate = value; }
        }

        /// <summary>
        /// The product's version
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// The product's copyright information
        /// </summary>
        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

        /// <summary>
        /// If true, puts the control's OK button in chanrge of closing the parent form,
        /// as well as setting the parent form's title.
        /// </summary>
        public bool DictatesToParentForm
        {
            get { return _dictatesToParentForm; }
            set { _dictatesToParentForm = value; }
        }

        /// <summary>
        /// The link to the creating institution's website
        /// </summary>
        public string InstitutionLink
        {
            get
            {
                if (string.IsNullOrEmpty(_institutionLink))
                    return INSTITUTION_LINK;
                else
                    return _institutionLink;
            }
            set { _institutionLink = value; }
        }

        /// <summary>
        /// The link to the product's support website
        /// </summary>
        public string SupportLink
        {
            get
            {
                if (string.IsNullOrEmpty(_supportLink))
                    return SUPPORT_LINK;
                else
                    return _supportLink;
            }
            set { _supportLink = value; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutControl()
        {
            InitializeComponent();
            //Event-handlers
            this.lnkInstitution.LinkClicked += lnkInstitution_LinkClicked;
            this.lnkSupport.LinkClicked += lnkSupport_LinkClicked;
            this.okButton.Click += okButton_Click;
            //Other
            this._logoBox = this.picCSLogoPictureBox;
        }

        /// <summary>
        /// Fetches various data-points from the application's assembly metadata
        /// </summary>
        public void ExtractAndBindAssemblyData()
        {
            //data extraction
            _assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //conditional - this control is being used in a separate 'About' form
            if (_dictatesToParentForm)
            {
                this.ParentForm.Text = this.GetAssemblyTitle();
                this.ParentForm.AcceptButton = this.okButton;
                this.ParentForm.CancelButton = this.okButton;
            }

            _productName = Application.ProductName;
            _buildDate = this.GetAssemblyVersionCreationTime();
            _version = this.GetAssemblyFileVersion();
            _copyright = this.GetAssemblyCopyright();

            //databindings
            this.lblProductName.DataBindings.Add("Text", this, "CustomProductName");
            this.lblBuildDate.DataBindings.Add("Text", this, "BuildDate");
            this.lblVersion.DataBindings.Add("Text", this, "Version");
            this.lblCopyright.DataBindings.Add("Text", this, "Copyright");
            this.lnkInstitution.DataBindings.Add("Text", this, "InstitutionLink");
            this.lnkSupport.DataBindings.Add("Text", this, "SupportLink");
            this.okButton.DataBindings.Add("Visible", this, "DictatesToParentForm");
        }

        /// <summary>
        /// Gets the executing assembly's version number
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
        ///   Gets the executing assembly's creation date
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyVersionCreationTime()
        {
            return File.GetCreationTime(_assembly.Location).ToString();
        }

        /// <summary>
        /// Gets the executing assembly's Copyright information
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyCopyright()
        {
            AssemblyCopyrightAttribute attribute =
                (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(
                    _assembly, typeof(AssemblyCopyrightAttribute));
            return attribute.Copyright;
        }

        /// <summary>
        /// Get's the executing assembly's Title
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyTitle()
        {
            AssemblyTitleAttribute attribute =
                (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(
                    _assembly, typeof(AssemblyTitleAttribute));
            return attribute.Title;
        }

        #region >Event-handlers<

        /// <summary>
        /// Launch the CambridgeSoft URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkInstitution_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.lnkInstitution.LinkVisited = true;
            System.Diagnostics.Process.Start(this.InstitutionLink);
        }

        /// <summary>
        /// Launch the CambridgeSoft product support URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.lnkSupport.LinkVisited = true;
            System.Diagnostics.Process.Start(this.SupportLink);
        }

        /// <summary>
        /// Close the form (DialogResult.Cancel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (_dictatesToParentForm)
                this.ParentForm.Close();
        }

        #endregion

    }
}
