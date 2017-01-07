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
using System.Reflection;
using System.Collections.Generic;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Xml;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.ContentArea
{
    public partial class AddInsEdit : GUIShellPage
    {
        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ConfigurationBO.AddInList[AddInID].BeginEdit();
                    this.SetControlsAttributtes();

                    this.FillAddInData();

                    this.FillAddInConfiguration("edit");
                }

                this.ShowAddInStates();

                this._masterPage.SetDefaultAction(this.BtnSave.UniqueID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

        #region Variables

        RegistrationMaster _masterPage = null;
        private AddInAssembly _addInAssembly;
        private AddInClass _addInClass;

        #endregion

        #region Properties

        public ConfigurationRegistryRecord ConfigurationBO
        {
            get
            {
                if (Session["ConfigurationRegistryRecord"] == null)
                {
                    return ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                }
                else
                {
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
                }
            }

            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }

        public int AddInID
        {
            get
            {
                return (int)Session["AddInID"];
            }
        }

        #endregion

        #region Methods

        protected override void SetControlsAttributtes()
        {
            //this.AddInEditPanel.GroupingText = Resource.AddInEdit_Panel_Text;
            this.ButtonConf.Text = "Edit Config";
            this.ButtonCancelConfig.Text = "Cancel Edit";
            this.LblAddInAssembly.Text = Resource.Assembly_Label_Text;
            this.LblAddInClass.Text = Resource.Class_Label_Text;
            //this.AddInEvensPanel.GroupingText = Resource.AddInEvents_Panel_Text;
            this.LblEvent.Text = Resource.Event_Label_Text;
            this.LblHandler.Text = Resource.Handler_Label_Text;
            this.BtnAddEvent.Text = Resource.Add_Event_Button_Text;
            this.BtnDeleteEvent.Text = Resource.Delete_Event_Button_Text;
            this.BtnSave.Text = Resource.Save_Button_Text;
            //this.TextBoxConf.Text = ConfigurationBO.AddInList[AddInID].AddInConfiguration;
            this.LabelEventList.Text = Resource.EventList_Label_Text;
            this.BtnDeleteEvent.Attributes.Add("onclick", "return confirm('" + Resource.ConfirmDeleteEvent_Alert_Text + "');");
            this.BtnCancel.Text = Resource.Cancel_Button_Text;
            this.XmlControl.TransformSource = "/COECommonResources/XmlToHtml.xsl";
            this.LabelEnabled.Text = Resource.Enabled_Label_Text;
            this.LabelRequired.Text = Resource.Required_Label_Text;
            this.LblFriendlyName.Text = Resource.AddInFriendlyName_Label_Text;
        }

        private void FillAddInData()
        {
            LblAddInAssemblySelected.Text = ConfigurationBO.AddInList[AddInID].Assembly;

            LblAddInClassSelected.Text = ConfigurationBO.AddInList[AddInID].ClassName;

            LblFiendlyNameSelected.Text = ConfigurationBO.AddInList[AddInID].FriendlyName;

            this.FillEventNames();

            this.FillHandlers();

            this.ShowAddedEvents();

        }

        private void FillAddInConfiguration(string mode)
        {
            if (mode == "edit")
            {
                this.XmlControl.Visible = true;
                this.TextBoxConf.Visible = false;
                this.XmlControl.TransformSource = "/COECommonResources/XmlToHtml.xsl";
                this.XmlControl.DocumentContent = ConfigurationBO.AddInList[AddInID].AddInConfiguration;
                this.ButtonConf.Text = "Edit Config";
                this.ButtonCancelConfig.Visible = false;
            }
            else
            {
                this.XmlControl.Visible = false;
                this.TextBoxConf.Visible = true;
                XmlDocument addInXml = new XmlDocument();
                addInXml.LoadXml(ConfigurationBO.AddInList[AddInID].AddInConfiguration);
                this.TextBoxConf.Text = addInXml.DocumentElement.InnerXml;
                this.ButtonConf.Text = "Save Config";
                this.ButtonCancelConfig.Visible = true;
            }
        }

        private void EditConfig()
        {
            if (this.ButtonConf.Text == "Save Config")
            {
                try
                {
                    this.XmlControl.TransformSource = "/COECommonResources/XmlToHtml.xsl";
                    if (CheckAddInConfXML())
                        ConfigurationBO.AddInList[AddInID].AddInConfiguration = "<AddInConfiguration>" + this.TextBoxConf.Text + "</AddInConfiguration>";
                    else
                        _masterPage.DisplayErrorMessage( Resource.InvalidAddInConfiguration_MasterPage , false);
                }
                catch (Exception e)
                {
                    _masterPage.DisplayErrorMessage(e.Message, false);
                }
                FillAddInConfiguration("edit");
            }
            else
            {
                this.XmlControl.TransformSource = "/COECommonResources/XmlToHtml.xsl";
                FillAddInConfiguration("save");
            }
        }

        private void DeleteEvent()
        {
            if (UltraWebTreeEvents.Nodes.Count > 0)
            {
                if (UltraWebTreeEvents.SelectedNode != null)
                {
                    if (UltraWebTreeEvents.SelectedNode.Index != -1)
                    {
                        ConfigurationBO.AddInList[AddInID].EventList.RemoveAt(UltraWebTreeEvents.SelectedNode.Index);
                    }


                    ShowAddedEvents();

                }
                else
                    _masterPage.DisplayErrorMessage( Resource.EventNotSelected_MasterPage , false);
            }
            else
                _masterPage.DisplayErrorMessage(Resource.NoAvailableEventToDelete_MasterPage, false);
            FillAddInConfiguration("edit");

        }

        private void SaveAddIn()
        {
            ConfigurationBO.AddInList[AddInID].ApplyEdit();
            ConfigurationBO = ConfigurationBO.Save();
            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/AddIns.aspx");
        }

        private void CancelEdit()
        {
            ConfigurationBO.AddInList[AddInID].CancelEdit();
            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/AddIns.aspx");
        }

        private void FillEventNames()
        {
            foreach (string eventName in ConfigurationBO.EventNames)
            {
                this.DdlEvent.Items.Add(eventName);
            }
        }

        private void FillHandlers()
        {
            try
            {
                _addInAssembly = ConfigurationBO.GetAssemblyList.GetAseemblyByName(ConfigurationBO.AddInList[AddInID].Assembly);
                if (_addInAssembly != null) // Coverity fix - CID 11788 
                {
                    _addInClass = _addInAssembly.GetClassByName(ConfigurationBO.AddInList[AddInID].ClassName);

                    if (_addInClass != null)
                    {
                        foreach (string handler in _addInClass.EventHandlerList)
                        {
                            DdlHandler.Items.Add(handler);
                        }
                    }
                }
            }
            catch 
            {
                this.EventsPanel.Visible = false;
            }

        }

        private void ShowAddedEvents()
        {
            this.UltraWebTreeEvents.ClearAll();

            foreach (Event evt in ConfigurationBO.AddInList[AddInID].EventList)
            {
                string name = evt.EventName;
                string handler = evt.EventHandler;
                this.UltraWebTreeEvents.Nodes.Add("Event = " + name + " - Handler = " + handler);
            }
            FillAddInConfiguration("edit");

        }

        private void AddEvent()
        {

            try
            {
                Event evt = Event.NewEvent(this.DdlEvent.SelectedValue, this.DdlHandler.SelectedValue, true);

                if (!ConfigurationBO.AddInList[AddInID].EventList.ExistEventCheck(evt))
                {
                    ConfigurationBO.AddInList[AddInID].EventList.Add(evt);
                    ShowAddedEvents();
                }
                else
                {
                    ShowAddedEvents();
                    _masterPage.DisplayErrorMessage( Resource.EventAlreadyExist_MasterPage , false);
                }
            }
            catch (Exception e)
            {
                _masterPage.DisplayErrorMessage(e.Message, true);
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

        private void SetAddInState()
        {

            ConfigurationBO.AddInList[AddInID].IsEnable = !ConfigurationBO.AddInList[AddInID].IsEnable;
            ShowAddInStates();

        }

        private void ShowAddInStates()
        {

            if (!ConfigurationBO.AddInList[AddInID].IsRequired)
            {
                this.CheckBoxEnable.Checked = ConfigurationBO.AddInList[AddInID].IsEnable;
                this.CheckBoxRequired.Checked = ConfigurationBO.AddInList[AddInID].IsRequired;
                this.CheckBoxEnable.Enabled = true;
            }
            else
            {
                this.CheckBoxEnable.Checked = ConfigurationBO.AddInList[AddInID].IsEnable;
                this.CheckBoxRequired.Checked = ConfigurationBO.AddInList[AddInID].IsRequired;
                this.CheckBoxEnable.Enabled = false;
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion

            base.OnInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BtnAddEvent_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddEvent();
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
                this.DeleteEvent();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveAddIn();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.CancelEdit();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void ButtonConf_Click(object sender, EventArgs e)
        {
            try
            {
                this.EditConfig();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }
        protected void ButtonCancelConfig_Click(object sender, EventArgs e)
        {
            try
            {
                this.FillAddInConfiguration("edit");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void CheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                SetAddInState();
                this.FillAddInConfiguration("edit");
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
                this.CancelEdit();
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
