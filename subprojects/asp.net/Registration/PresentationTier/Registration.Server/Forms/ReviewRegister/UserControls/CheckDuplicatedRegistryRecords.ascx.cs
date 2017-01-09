using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CambridgeSoft.COE.Framework.GUIShell;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Registration.Services.Types;
using Resources;
using PerkinElmer.COE.Registration.Server.Code;

namespace PerkinElmer.COE.Registration.Server.Forms.ReviewRegister.UserControls
{
    [Serializable]
    public partial class CheckDuplicatedRegistryRecords : System.Web.UI.UserControl, ICOENavigationPanelControl
    {
        #region Variables

        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised = null;
        private string _newRegistryType = string.Empty;

        #endregion

        #region Properties

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

        #region ICOENavigationPanelControl Members

        /// <summary>
        /// The identification of the UserControl.
        /// </summary>
        public override string ID
        {
            get { return ViewState[GUIShellTypes.ViewStateEntries.AccordionControlID.ToString()].ToString(); }
            set { ViewState[GUIShellTypes.ViewStateEntries.AccordionControlID.ToString()] = value; }
        }

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

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SetControlAttributes();
            }
        }

        #endregion

        #region Methods


        private void SetControlAttributes()
        {
            this.MessageLabel.Text = Resource.CheckDuplicates_Label_Text;
        }

        #endregion
    }
}