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
using CambridgeSoft.COE.RegistrationAdmin.Services;
using System.Xml;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.UserControls
{
    public partial class AddInList : System.Web.UI.UserControl
    {
        #region Variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Properties
        public string DataSourceID
        {
            get
            {
                if (ViewState["DataSourceID"] == null)
                    ViewState["DataSourceID"] = string.Empty;

                return (string)ViewState["DataSourceID"];
            }
            set
            {
                ViewState["DataSourceID"] = value;
            }
        }

        public ConfigurationRegistryRecord ConfigurationRegistryRecord
        {
            get
            {//Fix for CSBR-163697- Getting object reference error after session out
                if (Session["ConfigurationRegistryRecord"] == null)
                {
                    Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
                }
                else
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
            }
            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }

        public EventList EventList
        {
            get
            {
                if (ViewState["EventList"] == null)
                {
                    ViewState["EventList"] = EventList.NewEventList();
                }
                return (EventList)ViewState["EventList"];
            }
            set
            {
                ViewState["EventList"] = value;
            }
        }

        #endregion

        #region Page Load


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.SetControlsAttributes();
                    this.FillEventNames();
                    this.FillAddInList();
                }
                FillAssemblies();
                _masterPage.SetDefaultAction(this.Btn_Save.UniqueID);
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

        #endregion

        #region Methods

        private void EditAddIn()
        {
            if (!this.ConfigurationRegistryRecord.IsDirty)
            {
                if (AddInsUltraWebTree.SelectedNode != null && AddInsUltraWebTree.SelectedNode.Index != -1)
                {
                    Session["AddInID"] = AddInsUltraWebTree.SelectedNode.Index;
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/AddInsEdit.aspx");
                }
                else
                    _masterPage.DisplayErrorMessage( Resource.AddInNotSelected_MasterPage , false);
            }
            else
                _masterPage.DisplayErrorMessage( Resource.SaveConfiguraitionBeforeEditAddIn_MasterPage , false);
        }

        internal void SetControlsAttributes()
        {
            this.BtnDeleteAddIn.Text = Resource.Delete_AddIn_Button_Text;
            this.LblClass.Text = Resource.Class_Label_Text;
            this.LblHandler.Text = Resource.Handler_Label_Text;
            this.LblEvent.Text = Resource.Event_Label_Text;
            this.BtnAddEvent.Text = Resource.Add_Event_Button_Text;
            this.BtnDeleteEvent.Text = Resource.Delete_Event_Button_Text;
            this.BtnAddAddIn.Text = Resource.Add_AddIn_Button_Text;
            this.Btn_Save.Text = Resource.Save_Button_Text;   
            this.LblAssembly.Text = Resource.Assembly_Label_Text;            
            this.ButtonEditAddIn.Text = Resource.Edit_AddIn_Button_Text;
            this.LabelEventList.Text = Resource.EventList_Label_Text;
            this.LabelAddInConfiguration.Text = Resource.AddInConfiguration_Label_Text;
            this.LabelAddInList.Text = Resource.AddInList_Label_Text;
            this.BtnDeleteAddIn.Attributes.Add("onclick", "return confirm('" + Resource.ConfirmDeleteAddIn_Alert_Text + "');");
            this.BtnDeleteEvent.Attributes.Add("onclick", "return confirm('" + Resource.ConfirmDeleteEvent_Alert_Text + "');");
            this.LabelFrienlyName.Text = Resource.FriendlyName_Label_Text;
        }

        internal void FillAddInList()
        {
            try
            {
                AddInsUltraWebTree.ClearAll();
                string realName;
                string eVent;
                string friendlyName;
                if (ConfigurationRegistryRecord.AddInList.Count > 0)
                {
                    for (int i = 0; i < ConfigurationRegistryRecord.AddInList.Count; i++)
                    {
                        if (ConfigurationRegistryRecord.AddInList[i].IsNew)
                        {
                            realName = ConfigurationRegistryRecord.AddInList[i].ClassNameSpace + "." + ConfigurationRegistryRecord.AddInList[i].ClassName;
                        }
                        else
                        {
                            realName = ConfigurationRegistryRecord.AddInList[i].ClassName;
                        }
                        if (ConfigurationRegistryRecord.AddInList[i].FriendlyName != string.Empty)
                            friendlyName = ConfigurationRegistryRecord.AddInList[i].FriendlyName;
                        else
                            friendlyName = i.ToString();
                        AddInsUltraWebTree.Nodes.Add(friendlyName);
                        AddInsUltraWebTree.Nodes[i].Nodes.Add("AddIn = " + realName);
                        AddInsUltraWebTree.Nodes[i].Nodes.Add("Assembly = " + ConfigurationRegistryRecord.AddInList[i].Assembly);
                        AddInsUltraWebTree.Nodes[i].Nodes.Add("Event List:");
                        for (int j = 0; j < ConfigurationRegistryRecord.AddInList[i].EventList.Count; j++)
                        {
                            eVent = "Event Name = " + ConfigurationRegistryRecord.AddInList[i].EventList[j].EventName + " - Event Handler = " + ConfigurationRegistryRecord.AddInList[i].EventList[j].EventHandler;
                            AddInsUltraWebTree.Nodes[i].Nodes[2].Nodes.Add(eVent);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
                _masterPage.DisplayErrorMessage(ex.Message, true);
            }
        }

        private void FillAssemblies()
        {
            if (ConfigurationRegistryRecord.GetAssemblyList.Assemblies == null || ConfigurationRegistryRecord.GetAssemblyList.Assemblies.Count < 1)
            {
                this.NewAddInPanel.Visible = false;
            }
            else
            {
                if (IsPostBack)
                {
                    if (ConfigurationRegistryRecord.GetAssemblyList.Assemblies != null)
                    {
                        int selAss = this.DdlAssemblies.SelectedIndex;

                        int selClass = this.DdlClass.SelectedIndex;

                        int selHand = this.DdlHandler.SelectedIndex;

                        this.DdlAssemblies.Items.Clear();



                        for (int i = 0; i < ConfigurationRegistryRecord.GetAssemblyList.Assemblies.Count; i++)
                        {
                            this.DdlAssemblies.Items.Add(ConfigurationRegistryRecord.GetAssemblyList.Assemblies[i].Name);
                        }

                        this.DdlAssemblies.SelectedIndex = selAss;

                        this.FillClassList(selClass, selHand);
                    }
                }
                else
                {
                    if (ConfigurationRegistryRecord.GetAssemblyList.Assemblies != null)
                    {
                        for (int i = 0; i < ConfigurationRegistryRecord.GetAssemblyList.Assemblies.Count; i++)
                        {
                            this.DdlAssemblies.Items.Add(ConfigurationRegistryRecord.GetAssemblyList.Assemblies[i].Name);
                        }
                        this.FillClassList(0, 0);
                    }
                }            
            }
        }

        private void DeleteAddIn()
        {
            if (this.AddInsUltraWebTree.SelectedNode != null)
            {
                if (ConfigurationRegistryRecord.AddInList[AddInsUltraWebTree.SelectedNode.Index].IsRequired)
                {
                    _masterPage.DisplayErrorMessage(Resource.DeleteRequiredAddIns_MasterPage, false);
                }
                else
                {
                    if (this.AddInsUltraWebTree.SelectedNode.Index != -1)
                    {
                        ConfigurationRegistryRecord.AddInList.RemoveAt(AddInsUltraWebTree.SelectedNode.Index);
                        FillAddInList();
                    }
                    else
                    {
                        _masterPage.DisplayErrorMessage(Resource.AddInNotSelected_MasterPage, false);
                    }
                }
            }
            else
            {
                _masterPage.DisplayErrorMessage(Resource.AddInNotSelected_MasterPage, false);
            }                   
        }

        private void AddEvent()
        {
            Event evt = Event.NewEvent(this.DdlEvent.SelectedValue, this.DdlHandler.SelectedValue, true);

            if (!EventList.ExistEventCheck(evt))
            {
                EventList.Add(evt);
                this.DdlAssemblies.Enabled = false;
                this.DdlClass.Enabled = false;
                ShowAddedEvents();
            }
            else
            {
                ShowAddedEvents();
                _masterPage.DisplayErrorMessage(Resource.EventAlreadyExist_MasterPage, false);
            }
        }

        private void ShowAddedEvents()
        {
            this.EventsUltraWebTree.ClearAll();

            foreach (Event evt in EventList)
            {
                string name = evt.EventName;
                string handler = evt.EventHandler;
                this.EventsUltraWebTree.Nodes.Add("Event = " + name + " - Handler = " + handler);
            }
            if (EventsUltraWebTree.Nodes.Count == 0)
            {
                this.DdlAssemblies.Enabled = true;
                this.DdlClass.Enabled = true;
            }          
        }

        private void DeleteEvent()
        {
            if (EventsUltraWebTree.Nodes.Count > 0)
            {
                if (EventsUltraWebTree.SelectedNode != null)
                {
                    if (EventsUltraWebTree.SelectedNode.Index != -1)
                    {
                        this.EventList.RemoveAt(EventsUltraWebTree.SelectedNode.Index);
                    }
                    ShowAddedEvents();
                }
                else
                    _masterPage.DisplayErrorMessage( Resource.EventNotSelected_MasterPage , false);
            }
            else
                _masterPage.DisplayErrorMessage( Resource.NoAvailableEventToDelete_MasterPage , false);
        }

        private void FillEventNames()
        {
            foreach (string eventName in ConfigurationRegistryRecord.EventNames)
            {
                this.DdlEvent.Items.Add(eventName);
            }         
        }

        private void FillClassList(int selClass, int selHand)
        {
            this.DdlClass.Items.Clear();

            if (ConfigurationRegistryRecord.GetAssemblyList.Assemblies != null)
            {
                if (IsPostBack)
                {
                    int selAss = this.DdlAssemblies.SelectedIndex;


                    for (int i = 0; i < ConfigurationRegistryRecord.GetAssemblyList.Assemblies[selAss].ClassList.Count; i++)
                    {
                        this.DdlClass.Items.Add(ConfigurationRegistryRecord.GetAssemblyList.Assemblies[selAss].ClassList[i].Name);
                    }
                    this.DdlClass.SelectedIndex = selClass;
                    FillHandlers(selHand);
                }
                else
                {
                    for (int i = 0; i < ConfigurationRegistryRecord.GetAssemblyList.Assemblies[0].ClassList.Count; i++)
                    {
                        this.DdlClass.Items.Add(ConfigurationRegistryRecord.GetAssemblyList.Assemblies[0].ClassList[i].Name);
                    }
                    FillHandlers(0);
                }
            }
        }

        private void FillHandlers(int selHand)
        {
            this.DdlHandler.Items.Clear();
            if (IsPostBack)
            {
                int indexA = this.DdlAssemblies.SelectedIndex;
                int indexC = this.DdlClass.SelectedIndex;

                for (int i = 0; i < ConfigurationRegistryRecord.GetAssemblyList.Assemblies[indexA].ClassList[indexC].EventHandlerList.Count; i++)
                {
                    this.DdlHandler.Items.Add(ConfigurationRegistryRecord.GetAssemblyList.Assemblies[indexA].ClassList[indexC].EventHandlerList[i]);
                }

                this.DdlHandler.SelectedIndex = selHand;

            }
            else
            {
                for (int i = 0; i < ConfigurationRegistryRecord.GetAssemblyList.Assemblies[0].ClassList[0].EventHandlerList.Count; i++)
                {
                    this.DdlHandler.Items.Add(ConfigurationRegistryRecord.GetAssemblyList.Assemblies[0].ClassList[0].EventHandlerList[i]);
                }
            }           
        }

        private void AddAddIn()
        {
            string friendlyName = this.TexBoxFriendlyName.Text;
            string addInName = ConfigurationRegistryRecord.GetAssemblyList.Assemblies[DdlAssemblies.SelectedIndex].FullName;
            string className = ConfigurationRegistryRecord.GetAssemblyList.Assemblies[DdlAssemblies.SelectedIndex].ClassList[DdlClass.SelectedIndex].Name;
            string nameSpace = ConfigurationRegistryRecord.GetAssemblyList.Assemblies[DdlAssemblies.SelectedIndex].ClassList[DdlClass.SelectedIndex].NameSpace;
            if (friendlyName != string.Empty)
            {
                if (!ConfigurationRegistryRecord.AddInList.ExistAddInFriendlyName(friendlyName))
                {
                    if (this.CheckAddInConfXML())
                    {
                        AddIn addIn = AddIn.NewAddIn(addInName, className, friendlyName, EventList,
                            "<AddInConfiguration>" + this.TextBoxConf.Text + "</AddInConfiguration>", nameSpace, true, false);
                        //if (!ConfigurationRegistryRecord.AddInList.ExistAddIndCheck(addIn))
                        //{
                        ConfigurationRegistryRecord.AddInList.Add(addIn);
                        FillAddInList();
                        this.TextBoxConf.Text = string.Empty;
                        this.EventsUltraWebTree.ClearAll();
                        this.EventList = null;
                        this.DdlAssemblies.Enabled = true;
                        this.DdlClass.Enabled = true;
                    }
                    else
                    {
                        _masterPage.DisplayErrorMessage(Resource.InvalidAddInConfigurationFormat_MasterPage, false);
                    }
                }
                else
                {
                    _masterPage.DisplayErrorMessage(Resource.FriendlyNameAlreadyExist_MasterPage, false);
                }
            }
            else
            {
                _masterPage.DisplayErrorMessage(Resource.FriendlyNameRequired_MasterPage, false);
            }
        }

        private bool CheckAddInConfXML()
        {

            if (this.TextBoxConf.Text != string.Empty)
            {
                string conf = "<AddInConfiguration>" + this.TextBoxConf.Text + "</AddInConfiguration>";
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.LoadXml(conf);
                }
                catch
                {
                    return false;
                }
                if (xml.DocumentElement.FirstChild.Name == "AddInConfiguration")
                    throw new Exception(Resource.AddInConfigurationFormat_Exception);
            }         
            return true;

        }

        private void SaveConfigurationRegistryRecord()
        {
            //this.ConfigurationRegistryRecord.CreateCoeFormTemplates();
            this.ConfigurationRegistryRecord = this.ConfigurationRegistryRecord.Save();
            this.FillAddInList();
            ShowConfirmationMessage(Resource.ConfigurationSaved_MessagesArea);      
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.MessagesAreaUserControl.AreaText = messageToDisplay;
            this.MessagesAreaUserControl.Visible = true;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Event Handler

        protected void BtnDeleteAddIn_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteAddIn();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void BtnAddEvent_Click(object sender, EventArgs e)
        {
            try
            {
                AddEvent();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void BtnDeleteEvent_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteEvent();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void DdlAssemblies_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlClass_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void BtnAddAddIn_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddAddIn();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Default.aspx");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveConfigurationRegistryRecord();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        //protected void CheckBox_Click(object sender, EventArgs e)
        //{
        //    SetAddInState();
        //}

        //protected void Node_Click(object sender, EventArgs e)
        //{
        //    ShowAddInStates();
        //}

        protected void ButtonEditAddIn_Click(object sender, EventArgs e)
        {
            try
            {
                this.EditAddIn();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        #endregion

    }

}
