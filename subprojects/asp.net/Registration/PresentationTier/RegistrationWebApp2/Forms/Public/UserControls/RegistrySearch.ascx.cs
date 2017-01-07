using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Registration.Services.Types;
using Resources;
using RegistrationWebApp.Code;

namespace RegistrationWebApp.Forms.Public.UserControls
{
    public partial class RegistrySearch : System.Web.UI.UserControl, ICOENavigationPanelControl
    {
        #region Variables
        private string _newRegistryType = string.Empty;
        #endregion
        #region Properties

        /// <summary>
        /// The identification of the UserControl.
        /// </summary>
        public override string ID
        {
            get { return ViewState[GUIShellTypes.ViewStateEntries.AccordionControlID.ToString()].ToString(); }
            set { ViewState[GUIShellTypes.ViewStateEntries.AccordionControlID.ToString()] = value; }
        }

        public string NewRegistryType
        {
            get
            {
                return _newRegistryType;
            }
            set
            {
                _newRegistryType = value;
            }
        }

        #endregion

        #region Variables

        #region ICOENavigationPanelControl Members

        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;

        #endregion

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SetControlAttributes();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods
        private void SetControlAttributes()
        {
            this.MessageLabel.Text = Resource.SearchComponent_Label_Text;
        }
        #endregion

        #region ICOENavigationPanelControl Members

        public void DataBind(RegistryRecord registryRecord)
        {
            return;
        }

        public void DataBind(RegistryRecord registryRecord, string nodeTextToDisplayAsSelected)
        {
            return;
        }

        public void DataBind(RegistryRecord registryRecord, bool mixtureDuplicates)
        {
            return;
        }

        public void DataBind(CompoundList compoundList)
        {
            return;
        }

        public void DataBind(BatchList batchList)
        {
            return;
        }

        public void SetTitle(string title)
        {
            return;
        }

        #endregion
    }
}