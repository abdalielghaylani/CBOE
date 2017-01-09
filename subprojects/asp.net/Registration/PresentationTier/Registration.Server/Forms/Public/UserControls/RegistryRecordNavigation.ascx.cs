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
using System.Text;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Resources;
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services.Common;
using PerkinElmer.COE.Registration.Server.Code;


namespace PerkinElmer.COE.Registration.Server.Forms.Public.UserControls
{
    [Serializable]
    public partial class RegistryRecordNavigation : System.Web.UI.UserControl, ICOENavigationPanelControl
    {
        #region Variables

        RegistrationMaster _masterPage = null;
        private string _compoundID = String.Empty;
        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised = null;
        private const string newCompoundID = "New Compound";
        private string _selectedNodeDataKey = String.Empty;
        private string _newRegistryType = string.Empty;
       

        private enum NodeCSSStyles
        {
            TextNode,
            SelectedTextNode,
            HighlightedTextNode,
            HighlightedSelectedTextNode
        }
      

        private enum NodeKeywords
        {
            FirstBatch,
            LastBatch,
            FirstComponent,
            LastComponent,
            Root,
        }

        #endregion

        #region Properties

        private string SelectedNode
        {
            get
            {
                return ViewState["SelectedNode"] != null ? ViewState["SelectedNode"].ToString() : String.Empty;
            }
            set
            {
                if (ViewState["SelectedNode"] != null)
                {
                    if (ViewState["SelectedNode"].ToString() != value)
                        ViewState["SelectedNode"] = value;
                }
                else
                    ViewState["SelectedNode"] = value;
            }
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


        private int LevelToHighLight
        {
            get
            {
                if (ViewState["LevelToHighLight"] != null)
                    return int.Parse(ViewState["LevelToHighLight"].ToString());
                else
                    return 0;
            }
            set
            {
                int retVal = 0;
                int tempRetVal = 0;
                if (int.TryParse(value.ToString(), out tempRetVal))
                    retVal = tempRetVal;
                ViewState["LevelToHighLight"] = retVal;
            }
        }

        private int SubLevelToHighLight
        {
            get
            {
                if (ViewState["SubLevelToHighLight"] != null)
                    return int.Parse(ViewState["SubLevelToHighLight"].ToString());
                else
                    return 0;
            }
            set
            {
                int retVal = 0;
                int tempRetVal = 0;
                if (int.TryParse(value.ToString(), out tempRetVal))
                    retVal = tempRetVal;
                ViewState["SubLevelToHighLight"] = retVal;
            }
        }
        //private string SelectedNode
        //{
        //    get
        //    {
        //        return _selectedNodeDataKey;
        //    }
        //    set
        //    {
        //        _selectedNodeDataKey = value;
        //    }
        //}

        /// <summary>
        /// The identification of the UserControl.
        /// </summary>
        public override string ID
        {
            get { return ViewState[GUIShellTypes.ViewStateEntries.AccordionControlID.ToString()].ToString(); }
            set { ViewState[GUIShellTypes.ViewStateEntries.AccordionControlID.ToString()] = value; }
        }

        private string ThemeImagesFolderPath
        {
            get { return "~/App_Themes/" + this.Page.StyleSheetTheme + "/Images/"; }
        }

        /// <summary>
        /// After a "Compound Registration" this is the Compound ID(encoded) of the saved compound.
        /// </summary>
        /// <remarks>The integer is encoded (just to don't show the compound as it is)</remarks>
        public string CompoundID
        {
            get
            {
                return GUIShellUtilities.Decode(this.CompoundIDHidden.Value);
            }
        }

        /// <summary>
        /// After a "Compound Registration" this is the Batch ID (encoded) of the saved compound.
        /// </summary>
        /// <remarks>The integer is encoded (just to don't show the compound as it is)</remarks>
        //public string BatchID {
        //    get {
        //        return CambridgeSoft.COE.Framework.GUIShell.Utilities.Decode(this.BatchIDHidden.Value);
        //    }
        //}

        public bool ShowButtons
        {
            get
            {
                return bool.Parse(this.ShowButtonsHidden.Value);
            }
        }
        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SetControlAttributes();
            }
            this.SetJScriptReference();
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
        /// This method handles the Click in the DoneButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DoneButton_Click(object sender, EventArgs e)
        {
            try
            {
                /* If somebody has subscribe to this event, call to the subscriber */
                if (CommandRaised != null)
                {
                    // Copy to a temporary variable to be thread-safe.
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
                    // Copy to a temporary variable to be thread-safe.
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
        /// This method handles the Click in the AddBacthButton
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
                    // Copy to a temporary variable to be thread-safe.
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
        /// This method handles the Click in a Node and launches a new Custom Event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UltraWebTreeControl_NodeClicked(object sender, WebTreeNodeEventArgs e)
        {
            try
            {
                /* If somebody has subscribe to this event, call to the subscriber */
                if (CommandRaised != null)
                {
                    //Set selected node cssclass
                    this.LevelToHighLight = e.Node.Level;
                    this.SubLevelToHighLight = e.Node.Index;
                    //this.SetSelectedNodeCSS(e.Node.DataKey.ToString());
                    // Copy to a temporary variable to be thread-safe.
                    EventHandler<COENavigationPanelControlEventArgs> currentEventRaised = CommandRaised;
                    currentEventRaised(this, new COENavigationPanelControlEventArgs(e.Node.DataKey.ToString(), GUIShellTypes.WebTreeActions.NodeClicked.ToString()));
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

        private void SetSelectedNodeCSS(string selectedNode)
        {
            foreach (Node currentNode in this.UltraWebTreeControl.Nodes)
            {
                //Root Node
                if (currentNode.DataKey.ToString() == selectedNode)
                {
                    currentNode.Style.CssClass = NodeCSSStyles.SelectedTextNode.ToString();
                    this.SelectedNode = currentNode.Text;
                }
                else
                    currentNode.Style.CssClass = NodeCSSStyles.TextNode.ToString();
                //Check childs nodes
                if (currentNode.Nodes.Count > 0)
                {
                    foreach (Node currentInnerNode in currentNode.Nodes)
                    {
                        if (currentInnerNode.DataKey.ToString() == selectedNode)
                        {
                            currentInnerNode.Style.CssClass = NodeCSSStyles.SelectedTextNode.ToString();
                            this.SelectedNode = currentInnerNode.Text;
                        }
                        else
                            currentInnerNode.Style.CssClass = NodeCSSStyles.TextNode.ToString();
                    }
                }
            }
        }
        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        private void SetControlAttributes()
        {
            this.UpdateButton.Text = Resource.Edit_Button_Text;
            this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
            this.UpdateButton.Visible = ShowButtons;
            this.AddBatchButton.Visible = ShowButtons;
            this.UltraWebTreeControl.Images.CollapseImage.Url = GUIShellTypes.UCImagesPath + "collapse.gif";
            this.UltraWebTreeControl.Images.ExpandImage.Url = GUIShellTypes.UCImagesPath + "expand.gif";
            if (this.CompoundID == newCompoundID)
                this.MessageLabel.Text = Resource.SubmitforReview_Label_Text;
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

        private void SetSelectedNodeDataKey()
        {
            foreach (Node currentNode in this.UltraWebTreeControl.Nodes)
            {
                if (currentNode.Selected)
                {
                    this.SelectedNode = currentNode.ToString();
                    break;
                }
                if (currentNode.Nodes.Count > 0)
                {
                    foreach (Node currentInnerNode in currentNode.Nodes)
                    {
                        if (currentInnerNode.Selected)
                        {
                            this.SelectedNode = currentInnerNode.ToString();
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region ICOENavigationPanelControl Members

        public void DataBind(RegistryRecord registryRecord, string nodeToShowSelected)
        {
            try
            {
                this.SetSelectedNodeIndexes(nodeToShowSelected, registryRecord);
                this.DataBind(registryRecord);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

       
       
        public void DataBind(RegistryRecord registryRecord)
        {
            try
            {
                this.UltraWebTreeControl.Nodes.Clear();
                if (registryRecord == null || (registryRecord.IsTemporal == false && registryRecord.ComponentList.Count == 1 && String.IsNullOrEmpty(registryRecord.ComponentList[0].Compound.RegNumber.RegNum.ToString())))
                {
                    Node mixtureNode = new Node();
                    mixtureNode.DataKey = "001";

                    switch (this.NewRegistryType.ToLower())
                    {
                        case "mixture":
                            mixtureNode.Text = Resource.NewMixture_Prefix_Text;
                            mixtureNode.CssClass = "TitleTreeNode";
                            break;
                        case "both":
                            mixtureNode.Text = Resource.NewRegistry_Prefix_Text;
                            mixtureNode.CssClass = "TitleTreeNode";
                            break;
                        case "component":
                            mixtureNode.Text = Resource.NewComponent_PrefixTitle_Text;
                            mixtureNode.CssClass = "TitleTreeNode";
                            break;
                    }
                   
                    mixtureNode.Expanded = true;
                    mixtureNode.HiliteClass = NodeCSSStyles.SelectedTextNode.ToString();
                    mixtureNode.Style.CssClass = NodeCSSStyles.SelectedTextNode.ToString();
                    mixtureNode.Enabled = false;
                    //Empty Tree.

                        Node compoundNode1 = new Node();
                        compoundNode1.DataKey = "C0";
                    compoundNode1.HiliteClass = NodeCSSStyles.SelectedTextNode.ToString();
                        compoundNode1.Style.CssClass = NodeCSSStyles.TextNode.ToString();
                        switch (this.NewRegistryType.ToLower())
                        {
                            case "mixture":
                                compoundNode1.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewMixtureComponentNode_Text, 1);
                                compoundNode1.ToolTip = Resource.NewMixtureComponentNode_ToolTip;
                                break;
                            case "both":
                                compoundNode1.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewComponentNode_Text, 1);
                                break;
                            case "component":
                                compoundNode1.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewComponentNode_Text, 1);
                                break;
                        }
                        
                        compoundNode1.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "compound.gif";
                        compoundNode1.Style.CssClass = NodeCSSStyles.SelectedTextNode.ToString();
                        mixtureNode.Nodes.Add(compoundNode1);
                        this.LevelToHighLight = compoundNode1.Level;
                        this.SubLevelToHighLight = compoundNode1.Index;

                        Node currentBatchNode = new Node();
                        currentBatchNode.DataKey = "B0";
                    currentBatchNode.HiliteClass = NodeCSSStyles.SelectedTextNode.ToString();
                        currentBatchNode.Style.CssClass = NodeCSSStyles.TextNode.ToString();
                        currentBatchNode.Text = Resource.NewBatch_Prefix_Text;
                        currentBatchNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "batch.gif";
                        mixtureNode.Nodes.Add(currentBatchNode);
                    
                    this.UltraWebTreeControl.Nodes.Add(mixtureNode);

                }
                else
                {
                    Node mixtureNode = new Node();

                    if (registryRecord.IsNew)
                    {
                        switch (this.NewRegistryType.ToLower())
                        {
                            case "mixture":
                                mixtureNode.Text = Resource.NewMixture_Prefix_Text;
                                mixtureNode.CssClass = "TitleTreeNode";
                                break;
                            case "both":
                                mixtureNode.Text = Resource.NewRegistry_Prefix_Text;
                                mixtureNode.CssClass = "TitleTreeNode";
                                break;
                            case "component":
                                mixtureNode.Text = Resource.NewComponent_PrefixTitle_Text;
                                mixtureNode.CssClass = "TitleTreeNode";
                                break;
                        }
                        
                    }
                    else if (registryRecord.IsTemporal)
                        switch (this.NewRegistryType.ToLower())
                        {
                            case "mixture":
                                mixtureNode.Text = Resource.TemporalMixture_Prefix_Text + " - " + registryRecord.ID.ToString();
                                break;
                            case "both":
                                mixtureNode.Text = Resource.TemporalRegistry_Prefix_Text + " - " + registryRecord.ID.ToString();
                                break;
                            case "component":
                                mixtureNode.Text = Resource.TemporalComponent_Prefix_Text + " - " + registryRecord.ID.ToString();
                                break;
                        }
                    else
                        switch (this.NewRegistryType.ToLower())
                        {
                            case "mixture":
                                mixtureNode.Text = Resource.Mixture_Prefix_Text + " - " + registryRecord.RegNumber.RegNum;
                                break;
                            case "both":
                                mixtureNode.Text = Resource.Registry_Prefix_Text + " - " + registryRecord.RegNumber.RegNum;
                                break;
                            case "component":
                                mixtureNode.Text = Resource.Component_Prefix_Text + " - " + registryRecord.RegNumber.RegNum;
                                break;
                        }
                      

                    mixtureNode.HiliteClass = NodeCSSStyles.SelectedTextNode.ToString();

                    mixtureNode.DataKey = "0" + registryRecord.ID.ToString();
                    mixtureNode.Expanded = true;
                    for (int index = 0; index < registryRecord.ComponentList.Count; index++)
                    {
                        Component currentComponent = registryRecord.ComponentList[index];
                        Node currentCompoundNode = new Node();
                        currentCompoundNode.HiliteClass = NodeCSSStyles.SelectedTextNode.ToString();
                        if (currentComponent.IsNew)
                        {
                            switch (this.NewRegistryType.ToLower())
                            {
                                case "mixture":
                                    currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewMixtureComponentNode_Text, index + 1);
                                    currentCompoundNode.ToolTip = Resource.NewMixtureComponentNode_ToolTip;
                                    break;
                                case "both":
                                    currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewComponentNode_Text, index + 1);
                                    break;
                                case "component":
                                    currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewComponentNode_Text, index + 1);
                                    break;
                            }
                            //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                            //confusing mouse over
                            // This code Disable the Tooltip: ‘pick an existing component’ once we add at least one component.
                            if (currentComponent.Compound.RegNumber.ID > 0)
                                if (!string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum))
                                {
                                    currentCompoundNode.Text = currentComponent.Compound.RegNumber.RegNum;
                                    currentCompoundNode.ToolTip = "";
                                }
                        }
                        else if (registryRecord.IsTemporal)
                        {
                            if (currentComponent.Compound.RegNumber.ID > 0)
                            {
                                if (!string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum))
                                    currentCompoundNode.Text = currentComponent.Compound.RegNumber.RegNum;
                                else
                                    currentCompoundNode.Text = ("Structure " + currentComponent.Compound.BaseFragment.Structure.ID);
                            }
                            else
                                currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.TemporalComponent_Prefix_Text, index + 1);
                        }
                        else
                            currentCompoundNode.Text = currentComponent.Compound.RegNumber.RegNum;

                        currentCompoundNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "compound.gif";
                        currentCompoundNode.DataKey = "C" + index.ToString();
                        mixtureNode.Nodes.Add(currentCompoundNode);
                    }
                    for (int index = 0; index < registryRecord.BatchList.Count; index++)
                    {
                        Batch currentBatch = registryRecord.BatchList[index];
                        Node currentBatchNode = new Node();
                        currentBatchNode.HiliteClass = NodeCSSStyles.SelectedTextNode.ToString();

                        if (currentBatch.IsNew)
                            currentBatchNode.Text = string.Format(Resource.NewBatch_Prefix_Text);
                        else if (registryRecord.IsTemporal)
                            currentBatchNode.Text = string.Format(Resource.TemporalBatch_Prefix_Text);
                        else
                        {
                            currentBatchNode.Text = Resource.Batch_Prefix_Text + " - " + currentBatch.FullRegNumber;
                            if (HighlightBatch(currentBatch))
                            {
                                currentBatchNode.Style.CssClass = NodeCSSStyles.HighlightedTextNode.ToString();
                                currentBatchNode.HiliteClass = NodeCSSStyles.HighlightedSelectedTextNode.ToString();
                                currentBatchNode.ToolTip = GetHighlightTooltip();
                            }
                        }
                        currentBatchNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "batch.gif";
                        currentBatchNode.DataKey = "B" + index.ToString();
                        mixtureNode.Nodes.Add(currentBatchNode);
                    }
                    this.UltraWebTreeControl.Nodes.Add(mixtureNode);
                }

                //Find the node to show as selected.
                Node nodeToHighLight = this.GetNodeByLevelAndIndex(this.LevelToHighLight, this.SubLevelToHighLight);
                if (nodeToHighLight != null)
                    nodeToHighLight.Style.CssClass = nodeToHighLight.HiliteClass;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        public void DataBind(RegistryRecord registryRecord, bool MixtureDuplicates)
        {
            if ((_newRegistryType.ToLower() != "mixture") || (MixtureDuplicates == false))
            {
                DataBind( registryRecord);
            }
            else
            {
                    try
                        {       
                        this.SetTitle("Mixture Duplicate");
                        Node mixtureNode = new Node();
                        this.UltraWebTreeControl.Nodes.Clear();
                        mixtureNode.DataKey = "0" + registryRecord.ID.ToString();
                        mixtureNode.Expanded = true;
                        for (int index = 0; index < registryRecord.ComponentList.Count; index++)
                        {
                            Component currentComponent = registryRecord.ComponentList[index];
                            Node currentCompoundNode = new Node();
                            if (currentComponent.IsNew)
                            {
                                currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.NewComponent_Prefix_Text, index + 1);
                                if (currentComponent.Compound.RegNumber.ID > 0)
                                    if (!string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum))
                                        currentCompoundNode.Text = currentComponent.Compound.RegNumber.RegNum;
                            }
                            else if (registryRecord.IsTemporal)
                            {
                                if (currentComponent.Compound.RegNumber.ID > 0)
                                {
                                    if (!string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum))
                                        currentCompoundNode.Text = currentComponent.Compound.RegNumber.RegNum;
                                    else
                                        currentCompoundNode.Text = ("Structure " + currentComponent.Compound.BaseFragment.Structure.ID);
                                }
                                else
                                    currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.TemporalComponent_Prefix_Text, index + 1);
                            }
                            else
                                currentCompoundNode.Text = currentComponent.Compound.RegNumber.RegNum;

                            currentCompoundNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "compound.gif";
                            currentCompoundNode.DataKey = "C" + index.ToString();
                            mixtureNode.Nodes.Add(currentCompoundNode);
                        }
                        for (int index = 0; index < registryRecord.BatchList.Count; index++)
                        {
                            Batch currentBatch = registryRecord.BatchList[index];
                            Node currentBatchNode = new Node();

                            if (currentBatch.IsNew)
                                currentBatchNode.Text = string.Format(Resource.NewBatch_Prefix_Text);
                            else if (registryRecord.IsTemporal)
                                currentBatchNode.Text = string.Format(Resource.TemporalBatch_Prefix_Text);
                            else
                                currentBatchNode.Text = Resource.Batch_Prefix_Text + " - " + currentBatch.FullRegNumber;

                            currentBatchNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "batch.gif";
                            currentBatchNode.DataKey = "B" + index.ToString();
                            mixtureNode.Nodes.Add(currentBatchNode);
                        }
                        this.UltraWebTreeControl.Nodes.Add(mixtureNode);
                    

                    //Find the node to show as selected.
                    Node nodeToHighLight = this.GetNodeByLevelAndIndex(this.LevelToHighLight, this.SubLevelToHighLight);
                    if (nodeToHighLight != null)
                        nodeToHighLight.Style.CssClass = NodeCSSStyles.SelectedTextNode.ToString();
                   
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                    _masterPage.DisplayErrorMessage(exception, false);
                }
         }
        }

        /// <summary>
        /// Method to find a node inside the tree given the level and index
        /// </summary>
        /// <param name="level">In which level to search</param>
        /// <param name="index">In which index inside a level we have to find</param>
        /// <returns>The found node or null</returns>
        /// <remarks>Infragistics doesn't provide a funtion to do this. You can search by node or text</remarks>
        private Node GetNodeByLevelAndIndex(int level, int index)
        {
            Node retNode = null;
            if (level == 0)
            {
                if (this.UltraWebTreeControl.Nodes[0] != null)
                    retNode = this.UltraWebTreeControl.Nodes[0];
            }
            else if (level == 1)
            {
                if (this.UltraWebTreeControl.Nodes[0].Nodes[index] != null)
                    retNode = this.UltraWebTreeControl.Nodes[0].Nodes[index];
            }
            return retNode;
        }

        /// <summary>
        /// Set node indexes (Level and Index) to later search for a node
        /// </summary>
        /// <param name="nodeKey">Node to search</param>
        /// <param name="registry">Mixture object in case to use keywords as parameter</param>
        private void SetSelectedNodeIndexes(string nodeKey, RegistryRecord registry)
        {
            string separetor = "|";
            if (nodeKey == null)
            {
                //Set root node as selected
                this.LevelToHighLight = 0;
                this.SubLevelToHighLight = 0;
            }
            else if (nodeKey.IndexOf(separetor, 0) > 0)
            {
                this.LevelToHighLight = int.Parse(nodeKey.Split(separetor.ToCharArray())[0].ToString());
                this.SubLevelToHighLight = int.Parse(nodeKey.Split(separetor.ToCharArray())[1].ToString());
            }
            else
            {
                this.LevelToHighLight = 1;
                if (nodeKey == NodeKeywords.FirstComponent.ToString())
                    this.SubLevelToHighLight = 0;
                else if (nodeKey == NodeKeywords.LastComponent.ToString())
                    this.SubLevelToHighLight = registry.ComponentList.Count - 1;
                else if (nodeKey == NodeKeywords.FirstBatch.ToString())
                    this.SubLevelToHighLight = registry.ComponentList.Count;
                else if (nodeKey == NodeKeywords.LastBatch.ToString())
                    this.SubLevelToHighLight = registry.ComponentList.Count + registry.BatchList.Count - 1;
                else if (nodeKey == NodeKeywords.Root.ToString())
                {
                    this.LevelToHighLight = 0;
                    this.SubLevelToHighLight = 0;
                }
            }
            
        }

        public void DataBind(CompoundList compoundList)
        {
            
            try
            {
                this.UltraWebTreeControl.Nodes.Clear();

                if (compoundList != null)
                {
                    Node duplicateNode = new Node();
                    this.SetTitle("Duplicates");
                    duplicateNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "duplicates.gif";
                    duplicateNode.Expanded = true;

                    for (int index = 0; index < compoundList.Count; index++)
                    {
                        Compound currentCompound = compoundList[index];
                        Node currentCompoundNode = new Node();

                        currentCompoundNode.Text = string.Format(GUIShellTypes.IndexNumbersFormat + Resource.Component_Prefix_Text, Math.Abs(currentCompound.ID));

                        currentCompoundNode.DataKey = currentCompound.ID;
                        currentCompoundNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "compound.gif";
                        duplicateNode.Nodes.Add(currentCompoundNode);
                    }
                    this.UltraWebTreeControl.Nodes.Add(duplicateNode);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        public void DataBind(BatchList batchList)
        {
            try
            {
                this.UltraWebTreeControl.Nodes.Clear();

                if (batchList != null && batchList.Count > 0)
                {
                    Node rootNode = new Node();
                    switch (this.NewRegistryType.ToLower())
                    {
                        case "both":
                            this.SetTitle("Registry Duplicates");

                            break;
                        case "mixture":
                            this.SetTitle("Mixture Duplicates");

                            break;
                        case "component":
                            this.SetTitle("Component Duplicates");

                            break;
                    }
                    
                    rootNode.Expanded = true;

                    for (int index = 0; index < batchList.Count; index++)
                    {
                        Batch currentBatch = batchList[index];
                        Node currentChildNode = new Node();

                        if (currentBatch.IsNew)
                            //brand new batch, never been in the database
                            currentChildNode.Text = string.Format(Resource.NewBatch_Prefix_Text);
                        else if (string.IsNullOrEmpty(currentBatch.FullRegNumber))
                            //batch is from a TEMP registration
                            currentChildNode.Text = string.Format(Resource.TemporalBatch_Prefix_Text);
                        else
                        {
                            //registered batch, has a FullRegNum value
                            currentChildNode.Text = Resource.Batch_Prefix_Text + " - " + currentBatch.FullRegNumber;
                            if (HighlightBatch(currentBatch))
                            {
                                currentChildNode.Style.CssClass = NodeCSSStyles.HighlightedTextNode.ToString();
                                currentChildNode.HiliteClass = NodeCSSStyles.HighlightedSelectedTextNode.ToString();
                                currentChildNode.ToolTip = GetHighlightTooltip();
                            }
                        }
                        currentChildNode.Images.DefaultImage.Url = GUIShellTypes.UCImagesPath + "batch.gif";
                        currentChildNode.DataKey = "B" + index.ToString();
                        rootNode.Nodes.Add(currentChildNode);
                    }

                    this.UltraWebTreeControl.Nodes.Add(rootNode);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        private string GetHighlightTooltip()
        {
            return HightlightToolTip;
        }
        private string _hightlightToolTip;

        public string HightlightToolTip
        {
            get { return _hightlightToolTip; }
           
        }
	

        private bool HighlightBatch(Batch currentBatch)
        {   bool highlight = false;
            string highlightingField = RegUtilities.GetHighlightingField().ToUpper();
          
            if (!string.IsNullOrEmpty(highlightingField))
            {
                if (currentBatch.PropertyList[highlightingField] == null)
                    return false;

                string highlightingValue = RegUtilities.GetHighlightingValue();
                if(currentBatch.PropertyList[highlightingField] != null){

                    switch (highlightingValue)
                    {
                        case "is_not_empty":
                            highlight = !string.IsNullOrEmpty(currentBatch.PropertyList[highlightingField].Value);
                            break;
                        case "is_empty":
                            highlight = string.IsNullOrEmpty(currentBatch.PropertyList[highlightingField].Value);
                            break;
                        default:
                            highlight = currentBatch.PropertyList[highlightingField].Value == highlightingValue;
                            break;
                    }
                  
                }
                _hightlightToolTip = string.Format(RegUtilities.GetHighlightingTooltip().ToString(), currentBatch.PropertyList[highlightingField].Value);
                return highlight;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ICOENavigationPanelControl Members


        public void SetTitle(string title)
        {
            if(this.UltraWebTreeControl.Nodes.Count > 0)
                this.UltraWebTreeControl.Nodes[0].DataKey = this.UltraWebTreeControl.Nodes[0].Text = title;
           
        }

        #endregion
    }
}