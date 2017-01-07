using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CambridgeSoft.COE.Framework.GUIShell;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Registration.Services.Types;
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using System.Reflection;
using RegistrationWebApp.Code;

namespace RegistrationWebApp.Forms.Public.UserControls
{
    /// <summary>
    /// This is an example of a User Control implementing the ICOENavigationPanelControl interface.
    /// </summary>
    /// <seealso cref="../AppCode/ICOENavigationPanelControl.cs"/>
    [Serializable]
    public partial class SearchCompound : System.Web.UI.UserControl, ICOENavigationPanelControl
    {
        #region Variables

        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised = null;
        private string _customControlID = String.Empty;
        private string _compoundID = String.Empty;
        CompoundTreeInfo compoundTreeInfoObject = null;
        RegistrationMaster _masterPage = null;
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

        /// <summary>
        /// After a "Compound Registration" this is the ID(encoded) of the saved compound.
        /// </summary>
        /// <remarks>The strings is encoded (just to don't show the compound as it is)</remarks>
        public string CompoundID
        {
            get { return GUIShellUtilities.Decode(this.CompoundIDHidden.Value.ToString()); }
        }

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    this.SetControlAttributes();
                    if (this.CompoundID != String.Empty)
                    {
                        this.LoadWebTreeInfo();
                    }
                }
                this.SetJScriptReference();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.Page.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Page.Master;
            }
            base.OnInit(e);
        }

        /// <summary>
        /// This method handles the Click in the SearchButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                /* If somebody has subscribe to this event, call to the subscriber */
                if (CommandRaised != null)
                {
                    EventHandler<COENavigationPanelControlEventArgs> currentEventRaised = CommandRaised;
                    currentEventRaised(this, new COENavigationPanelControlEventArgs(((Button)sender).ID, ((Button)sender).CommandName));
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        /// <summary>
        /// This method handles the Click in the UpdateButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                /* If somebody has subscribe to this event, call to the subscriber */
                if (CommandRaised != null)
                {
                    EventHandler<COENavigationPanelControlEventArgs> currentEventRaised = CommandRaised;
                    currentEventRaised(this, new COENavigationPanelControlEventArgs(((Button)sender).ID, ((Button)sender).CommandName));
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        /// <summary>
        /// This method handles the Click in the AddBatchButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddBatchButton_Click(object sender, EventArgs e)
        {
            try
            {
                /* If somebody has subscribe to this event, call to the subscriber */
                if (CommandRaised != null)
                {
                    EventHandler<COENavigationPanelControlEventArgs> currentEventRaised = CommandRaised;
                    currentEventRaised(this, new COENavigationPanelControlEventArgs(((Button)sender).ID, ((Button)sender).CommandName));
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        private void SetControlAttributes()
        {
            this.SearchTitleLabel.Text = Resource.Search_Label_Text;
            this.SearchButton.Text = Resource.Search_Button_Text;
            this.RegistrationNumberLabel.Text = Resource.RegistrationNumber_Label_Text;
            this.IDLabel.Text = Resource.ID_Label_Text;
            this.SecuenceNumberLabel.Text = Resource.SecuenceNumber_Label_Text;
            this.UpdateButton.Text = Resource.Edit_Button_Text;
            this.AddBatchButton.Text = Resource.AddBatch_Button_Text;

            this.DisplaySearchTableImage.Src = GUIShellTypes.UCImagesPath + "plus.gif";
            this.DisplaySearchTableImage.Alt = Resource.DisplaySearch_Img_Alt;

            this.DisplaySearchTableImage.Attributes.Add("onclick", "jscript: Toggle('" + this.SearchControlsTable.ClientID.ToString() + "',this.id);");
        }

        /// <summary>
        /// This method will load all the compound information to the Tree control.
        /// </summary>
        private void LoadWebTreeInfo()
        {
            this.GetCompoundTreeDetails();
            /* 
             * The data for showing the Tree will come from the SingleCompoundRecord (compoundTreeInfoObject)
             */
            for (int i = 0; i < compoundTreeInfoObject.Compounds.Rows.Count; i++)
            {
                Node currentCompoundNode = new Node();
                currentCompoundNode.Text = compoundTreeInfoObject.Compounds[i].Name.ToString();
                currentCompoundNode.Expanded = true;
                for (int j = 0; j < compoundTreeInfoObject.Batchs.Rows.Count; j++)
                {
                    Node currentBatchNode = new Node();
                    currentBatchNode.Text = compoundTreeInfoObject.Batchs[i].Name;
                    currentBatchNode.TargetUrl = "../Contents/BatchDetails.aspx?BatchID=" + compoundTreeInfoObject.Batchs[i].ID.ToString();

                    currentCompoundNode.Nodes.Add(currentBatchNode);
                }
                this.UltraWebTreeControl.Nodes.Add(currentCompoundNode);
            }
        }

        void UltraWebTreeControl_NodeClicked(object sender, WebTreeNodeEventArgs e)
        {
            Server.Transfer(e.Node.TargetUrl + "?", true);
        }

        /// <summary>
        /// Just for testing purpose we use a DataSet!
        /// This method will load all the New Compound Info in a DataSet
        /// This method must be deleted after using SingleCompundRecord.
        /// </summary>
        private void GetCompoundTreeDetails()
        {
            /* Load the compound Info by a CompoundID */
            compoundTreeInfoObject = new CompoundTreeInfo();
            CompoundTreeInfo.CompoundsRow compoundInfoRow = compoundTreeInfoObject.Compounds.NewCompoundsRow();
            compoundInfoRow.ID = this.CompoundID;
            compoundInfoRow.Name = this.CompoundID;
            compoundTreeInfoObject.Compounds.AddCompoundsRow(compoundInfoRow);

            CompoundTreeInfo.BatchsRow batchInfoRow = compoundTreeInfoObject.Batchs.NewBatchsRow();
            batchInfoRow.CompoundID = this.CompoundID;
            batchInfoRow.ID = "L-001";
            batchInfoRow.Name = "L-001";
            compoundTreeInfoObject.Batchs.AddBatchsRow(batchInfoRow);
        }

        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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