using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using System.Configuration;
using Infragistics.WebUI.UltraWebTab;
using Infragistics.WebUI.WebCombo;
//using CambridgeSoft.COE.Framework.COEHitListService.BLL;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEHitlistManager", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEHitlistManager
{
    [ToolboxData("<{0}:COEHitlistManager runat=server Height='400px' Width='400px'></{0}:COEHitlistManager>")]
    class Constants
    {
        public const int ControlHeight = 400;
        public const int ControlWidth = 400;
    }
    /// <summary>
    /// Control Class, This contains the complete description for the Control.
    /// </summary>
    public class COEHitlistManager : CompositeControl, INamingContainer
    {
        #region Enumerations
        /// <summary>
        /// Custom Enumeration to keep track of selected Tab.
        /// </summary>
          public enum eMode { 
                Restore,
                Manage,
                Save
                }
        #endregion
        #region Class Variables
        
#endregion
        #region ControlProperties
        /// <summary>
        /// Currently Selected Main Tab .
        /// </summary>
        public int CurrentTab
        {
            get
            {
                if (ViewState["CurrentTab"] != null)
                {

                    return (int)ViewState["CurrentTab"];
                }
                else
                {
                    return 0;
                }
            }
            set {
                ViewState["CurrentTab"] = value;
                }
        }
        /// <summary>
        /// Maintain the selected Item from the Webcombo inside Manage Tab
        /// </summary>
        public int SelectedHitlistManage
        {
            get
            {
                if (ViewState["SelectedHitlistManage"] != null)
                {
                    return (int)ViewState["SelectedHitlistManage"];
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                ViewState["SelectedHitlistManage"] = value;
            }
        }
        /// <summary>
        /// Application Name that has been selected.
        /// </summary>
        public string AppName
        {
            get
            {
                //the appName should come from the ini file or set by the calling applicaton in csla globalcontext["AppName"]
                string appName = COEAppName.Get();
                if (ViewState["AppName"] != null)
                    appName = (string)ViewState["AppName"];
                return appName;
            }
            set
            {
                ViewState["AppName"] = value;
            }
        }
        /// <summary>
        /// CurrentHitlistID, that is currently control is working on.
        /// </summary>
        public int CurrentHitListID
        {
            get
            {
                if (ViewState["CurrentHitListID"] != null)
                {
                    if(((int)ViewState["CurrentHitListID"])!=0)
                        return (int)ViewState["CurrentHitListID"];
                }
                if ((!DesignMode) && AppName != string.Empty)
                {
                    COEHitListBO hitListBO= COEHitListBO.New(COEConfiguration.GetDatabaseNameFromAppName(AppName), HitListType.TEMP);
                    hitListBO.Save();
                    CurrentHitListType = HitListType.TEMP;
                    ViewState["CurrentHitListID"] = hitListBO.ID;
                    return hitListBO.ID;
                }    
                    return -1;
            }
            set
            {
                 if (ViewState["CurrentHitListID"] != null)
                 {
                     if (((int)ViewState["CurrentHitListID"]) != (int)value)
                     {
                         if (((int)ViewState["CurrentHitListID"]) != 0)
                             LastHitListID = (int)ViewState["CurrentHitListID"];
                     }
                 }
                ViewState["CurrentHitListID"] = value;
            }
        }
        /// <summary>
        /// CurrentHitlistID, that is currently control is working on.
        /// </summary>
        public HitListType CurrentHitListType
        {
            get
            {
                if (ViewState["CurrentHitListType"] != null)
                {
                    return (HitListType)ViewState["CurrentHitListType"];
                }
                else
                {
                        return HitListType.TEMP;
                }
            }
            set
            {
                if (ViewState["CurrentHitListType"] != null)
                    LastHitlistType = (HitListType)ViewState["CurrentHitListType"];
                ViewState["CurrentHitListType"] = value;
            }
        }
        /// <summary>
        /// ResolvedHitListID (Resolved Hitlist After operations), that is currently control is working on.
        /// </summary>
        public int ResolvedHitListID
        {
            get
            {
                if (ViewState["ResolvedHitListID"] != null)
                {
                        return (int)ViewState["ResolvedHitListID"];
                }
                return -1;
            }
            set
            {
                ViewState["ResolvedHitListID"] = value;
            }
        }
        /// <summary>
        /// ResolvedHitlistType (HitlistType of the Resolved Hitlist), that is currently control is working on.
        /// </summary>
        public HitListType ResolvedHitListType
        {
            get
            {
                if (ViewState["ResolvedHitListType"] != null)
                {
                    return (HitListType)ViewState["ResolvedHitListType"];
                }
                else
                {
                    return HitListType.TEMP;
                }
            }
            set
            {
                ViewState["ResolvedHitListType"] = value;
            }
        }
        /// <summary>
        /// LastHitListID (Hitlist that has been used before the current one), that is currently control is having.
        /// </summary>
        public int LastHitListID
        {
            get
            {
                if (ViewState["LastHitListID"] != null)
                {
                    return (int)ViewState["LastHitListID"];
                }
                return -1;
            }
            set
            {
                ViewState["LastHitListID"] = value;
            }
        }
        /// <summary>
        /// LastHitlistType (HitlistType of the Last Hitlist), that is currently control is having.
        /// </summary>
        public HitListType LastHitlistType
        {
            get
            {
                if (ViewState["LastHitlistType"] != null)
                {
                    return (HitListType)ViewState["LastHitlistType"];
                }
                else
                {
                    return HitListType.TEMP;
                }
            }
            set
            {
                ViewState["LastHitlistType"] = value;
            }
        }
        /// <summary>
        /// Manage tab can handle 2 type of records Either Saved (Can be selected from DropDown),
        /// or Current Record (select the CheckBox). This property keep track of that Record.
        /// </summary>
        private eRecordToManage RecordToManage
        {
            get {
                if (ViewState["RecordToManage"] != null)
                    return (eRecordToManage)ViewState["RecordToManage"];
                else
                    return eRecordToManage.Current;
                }
            set {
                ViewState["RecordToManage"] = value;
                }
        }
        enum eRecordToManage
        {
            Saved,
            Current
        }
        #endregion
        
        #region MemberControlsDefined
            private UltraWebTab mainTab;
            private UltraWebTab tabControl;
            private DropDownList DropDownName;
        #endregion
        #region Constructor
            /// <summary>
        /// Constructor for the Control
        /// </summary>
        public COEHitlistManager()
        {

        }
        #endregion

        #region Overrided Functions
        protected override void OnLoad(EventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["AppName"]= AppName;
            ConfigurationManager.AppSettings.Set("AppName", AppName);

            if (!Page.IsPostBack)
            {
                if (Page.Request.QueryString["CurrentHitListID"] != null)
                {
                    this.CurrentHitListID = Convert.ToInt32(Page.Request.QueryString["CurrentHitListID"].ToString());
                    this.CurrentHitListType = (HitListType)Enum.Parse(typeof(HitListType), Page.Request.QueryString["CurrentHitListType"].ToString());
                }
                this.RecordToManage = eRecordToManage.Current;
            }

            base.OnLoad(e);
        }
        protected override void OnInit(EventArgs e)
        {
           
            
            base.OnInit(e);
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            //To save the ClientID for the Hidden Field.
            Page.ClientScript.RegisterHiddenField("dropDownName",DropDownName.ClientID);
            
            //Hiding Unwanted Columns.
            Tab manageTab = (Tab)mainTab.Tabs.FromKey("Manage");
            WebCombo manageCombo = (WebCombo)manageTab.FindControl("WCCurrentRecord");
            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn col in manageCombo.Columns)
            {
                if (col.Key.Equals("ID") || col.Key.Equals("BrokenRulesCollection") || (col.Key.StartsWith("Is") && !col.Key.Equals("IsPublic")))
                {
                    col.Hidden = true;
                }
            }
            base.OnPreRender(e);
        }
        protected override void CreateChildControls()
        {
            if(mainTab!=null)
                CurrentTab = mainTab.SelectedTabIndex;

            Controls.Clear();
            mainTab = new UltraWebTab();
            mainTab.ID = "HitlistBar";
            mainTab.EnableTheming = true;
            mainTab.StylePreset = StylePreset.WindowsXP;
            mainTab.Height = Constants.ControlHeight;
            mainTab.Width = Constants.ControlWidth;
           
            //Defining ActionTabs {Manage,Restore,Save}
            Tab tabManage = new Tab("Manage");
            tabManage.Key = "Manage";
            Tab tabRestore = new Tab("Restore");
            tabRestore.Key = "Restore";
            Tab tabSave = new Tab("Save");
            tabSave.Key = "Save";
            //End Defining

            //Adding the Internal Contents of the Tabs.
            mainTab.Tabs.Add(tabManage);    
            CreateFormManage(tabManage);

            mainTab.Tabs.Add(tabRestore);
                CreateFormRestore(tabRestore);

            mainTab.Tabs.Add(tabSave);
                CreateFormSave(tabSave);
                PopulateSaveTab(tabSave);
           //Internal Contents Drawn.

            //Setting Currently Selected Tab
                mainTab.SelectedTabIndex = CurrentTab;
                
            this.Controls.Add(mainTab);
        }
        #endregion
        #region Internal Called Methods
        /// <summary>
        /// This method will draw the Restore Tab.
        /// </summary>
        /// <param name="tab">tabRestore tab would be passed as parameter</param>
        protected void CreateFormRestore(Tab tab)
        {
            DropDownList drpDownList = new DropDownList();
            drpDownList.ID = "RestoreDropDown";
            DropDownName = drpDownList;
            if (!DesignMode)
            {
                drpDownList.DataSource = COEHitListBOList.GetSavedHitListList(COEConfiguration.GetDatabaseNameFromAppName(AppName));
                drpDownList.DataValueField = "ID";
                drpDownList.DataTextField = "Name";
                drpDownList.DataBind();
            }

            Infragistics.WebUI.WebDataInput.WebImageButton btn = new Infragistics.WebUI.WebDataInput.WebImageButton();

            RadioButton chkSaved = new RadioButton();
            RadioButton chkLast = new RadioButton();
            
            chkSaved.GroupName = "HitList";
            chkLast.GroupName = "HitList";
            chkSaved.ID = "Saved";
            chkLast.ID = "Last";
            
            chkSaved.Checked = true;

            string DisableEnableDropDown=
              @"<script language='javascript' type='text/javascript'>
              function showDropDown(show)
              {
              var dropDownID = document.getElementById('dropDownName');
              var DropDown=document.getElementById(dropDownID.value);
              if(show){
              DropDown.style.visibility='visible';
              }else{
              DropDown.style.visibility='hidden';
              }
              }
              </script>";
            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "DisableEnableDropDown", DisableEnableDropDown);

            chkSaved.Attributes.Add("onclick", "showDropDown(true)");
            chkLast.Attributes.Add("onclick", "showDropDown(false)");
            btn.StylePreset= Infragistics.WebUI.WebDataInput.ButtonStylePreset.WindowsXPBlue;
            btn.Width=60;
            btn.Height=10;

            btn.Text = "Restore";

            btn.Click += new Infragistics.WebUI.WebDataInput.ClickHandler(btn_Click);

            chkLast.Text = "Last Selected";
            chkSaved.Text = "Saved";


            drpDownList.Width = 100;
            drpDownList.Items.Add("Saved Lists ");

            if (LastHitListID == CurrentHitListID)
                chkLast.Enabled = false;

            Table tbl = new Table();
            TableRow rowSaved=new TableRow();
            TableRow rowLast=new TableRow();
            TableRow rowBtn = new TableRow();
            TableCell cellChkSaved = new TableCell();
            TableCell cellChkLast = new TableCell();
            TableCell cellCmbSaved = new TableCell();
            TableCell cellBtn = new TableCell();

            rowSaved.Cells.Add(cellChkSaved);
            rowSaved.Cells.Add(cellCmbSaved);
            rowLast.Cells.Add(cellChkLast);
            rowBtn.Cells.Add(cellBtn);

            cellChkSaved.Controls.Add(chkSaved);
            cellCmbSaved.Controls.Add(drpDownList);
            cellChkLast.Controls.Add(chkLast);
            cellBtn.Controls.Add(btn);

            tbl.Rows.Add(rowSaved);
            tbl.Rows.Add(rowLast);
            tbl.Rows.Add(rowBtn);

            tbl.Height=Constants.ControlHeight;
            tbl.Width=Constants.ControlWidth;

            tab.ContentPane.Controls.Add(tbl);
        }
        /// <summary>
        /// Restores the selected Hitlist:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            //TODO : Restore:
            CheckBox chkSaved=(CheckBox)((Tab)mainTab.Tabs.FromKey("Restore")).FindControl("Saved");
            int hitlistID;
            if(chkSaved.Checked)
            {
                DropDownList RestoreDropDown = (DropDownList)((Tab)mainTab.Tabs.FromKey("Restore")).FindControl("RestoreDropDown");
                hitlistID= Convert.ToInt32(RestoreDropDown.SelectedItem.Value.ToString());
                CurrentHitListID = hitlistID;
                CurrentHitListType = HitListType.SAVED;
            }
            else
            {
                if (LastHitListID != -1)
                {
                    CurrentHitListID = LastHitListID;
                    CurrentHitListType = LastHitlistType;
                }
            }
            CreateChildControls();
        }
        /// <summary>
        /// This method would draw the Save Tab of the MainTab inside COEHitlistManager.
        /// </summary>
        /// <param name="tab">tabSave : as the input from CreateChildControls. </param>
        protected void CreateFormSave(Tab tab)
        {
            Table tbl = new Table();

            TableRow rowName = new TableRow();
            TableRow rowDescription = new TableRow();
            TableRow rowIsPublic = new TableRow();
            TableRow rowSave = new TableRow();

            TableCell cell = new TableCell();
            cell.Width = 30;
            rowName.Cells.Add(cell);

            cell = new TableCell();
            Label lbl=new Label();
            lbl.Text="Name";
            cell.Controls.Add(lbl);
            rowName.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            rowDescription.Cells.Add(cell);

            cell = new TableCell();
            lbl = new Label();
            lbl.Text = "Description";
            cell.Controls.Add(lbl);
            rowDescription.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            rowIsPublic.Cells.Add(cell);

            cell = new TableCell();
            lbl = new Label();
            lbl.Text = "IsPublic";
            cell.Controls.Add(lbl);
            rowIsPublic.Cells.Add(cell);

            cell = new TableCell();
            TextBox txtBox=new TextBox();
            txtBox.ID="Name";
            cell.Controls.Add(txtBox);
            rowName.Cells.Add(cell);
            
            cell = new TableCell();
            txtBox = new TextBox();
            txtBox.ID = "Description";
            cell.Controls.Add(txtBox);
            rowDescription.Cells.Add(cell);

            cell = new TableCell();
            CheckBox chkIsPublic = new CheckBox();
            chkIsPublic.ID = "IsPublic";
            cell.Controls.Add(chkIsPublic);
            rowIsPublic.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            rowSave.Cells.Add(cell);
            cell = new TableCell();
            Infragistics.WebUI.WebDataInput.WebImageButton btnSave = new Infragistics.WebUI.WebDataInput.WebImageButton();
            btnSave.Text = "Save";
            btnSave.ID = "Save";
            cell.Controls.Add(btnSave);
            rowSave.Cells.Add(cell);
            btnSave.Click += new Infragistics.WebUI.WebDataInput.ClickHandler(btnSave_Click);
            tbl.Rows.Add(rowDescription);
            tbl.Rows.Add(rowName);
            tbl.Rows.Add(rowIsPublic);
            tbl.Rows.Add(rowSave);

            tbl.Height = Constants.ControlHeight-50;
            tbl.Width = Constants.ControlWidth;
            tab.ContentPane.Controls.Add(tbl);
        }

        void btnSave_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            if(CurrentHitListType!= HitListType.SAVED)
            {
                COEHitListBO objSavedHitlist= COEHitListBO.Get(HitListType.TEMP, CurrentHitListID);
                CurrentHitListType = objSavedHitlist.HitListType;
                Tab tab=(Tab)mainTab.Tabs.FromKey("Save");
                objSavedHitlist.Name = ((TextBox)tab.FindControl("Name")).Text;
                objSavedHitlist.Description = ((TextBox)tab.FindControl("Description")).Text; ;
                objSavedHitlist.IsPublic = ((CheckBox)tab.FindControl("IsPublic")).Checked;
                objSavedHitlist = objSavedHitlist.Save();
                CurrentHitListID = objSavedHitlist.ID;
            }
            CreateChildControls();
        }
        /// <summary>
        /// This methods draws the Manage Tab control on tha tab.
        /// </summary>
        /// <param name="tab">tabManage: as the input from CreateChildControls</param>
        protected void CreateFormManage(Tab tab)
        {
            WebCombo wbCmbo = new WebCombo();
            wbCmbo.ID = "WCCurrentRecord";
            wbCmbo.Width = 200;
            if (!DesignMode)
            {
                COEHitListBOList savedHitList = COEHitListBOList.GetSavedHitListList(CambridgeSoft.COE.Framework.COEConfigurationService.COEConfiguration.GetDatabaseNameFromAppName(AppName));

                wbCmbo.DataSource = savedHitList;
                wbCmbo.DataValueField = "ID";
                wbCmbo.DataTextField = "Name";
                wbCmbo.DataBind();
            }
            
            wbCmbo.SelectedRowChanged += new SelectedRowChangedEventHandler(wbCmbo_SelectedRowChanged);
            UltraWebTab webTab = new UltraWebTab();
            webTab.ID = "ManageSubtab";
            webTab.Height = Constants.ControlHeight - 50;
            webTab.Width = Constants.ControlWidth;
            Tab tabEdit = new Tab("Edit");
            tabEdit.Key = "Edit";
            Tab tabOperations = new Tab("Operations");
            tabOperations.Key = "Operations";
            Tab tabDelete = new Tab("Delete");
            tabDelete.Key = "Delete";

            webTab.Tabs.Add(tabEdit);
            webTab.Tabs.Add(tabOperations);
            webTab.Tabs.Add(tabDelete);

            if (SelectedHitlistManage != -1 && wbCmbo.Rows.Count > 0)
                wbCmbo.SelectedIndex = SelectedHitlistManage;
            else
                wbCmbo.SelectedIndex = 0;
            //Drawing Manage Tab
            Table tbl = new Table();
            TableCell cell = new TableCell();
            
            TableRow row = new TableRow();
            Label lbl = new Label();
            lbl.Text = "Saved Record (Manage)";
            lbl.Font.Bold = true;
            lbl.Font.Size = 10;
            lbl.Font.Name = "Arial";
            cell.Controls.Add(lbl);
            cell.Controls.Add(wbCmbo);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            CheckBox chkCurrentRecord = new CheckBox();
            chkCurrentRecord.ID = "CurrentRecordManage";
            chkCurrentRecord.AutoPostBack = true;
            if (RecordToManage == eRecordToManage.Current)
                chkCurrentRecord.Checked = true;
            else
                chkCurrentRecord.Checked = false;

            //if (CurrentHitListID == LastHitListID)
            //    chkCurrentRecord.Enabled = false;

            

            string str = @"<script language='javascript' type='text/javascript'>
                          //Making the CheckBox ReadOnly:
                          function noChangeIfChecked(checkbox)
                          {
                            if(!checkbox.checked)
                            {
                                checkbox.checked=! checkbox.checked;
                                alert('Select Hitlist from the DropDown');
                                return false;
                            }
                            window.setTimeout('__doPostBack(\'\',\'\')', 1000);
                            return true;
                          }
                          </script>";

            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "CheckBoxChangeOnUncheckedOnly", str);
            chkCurrentRecord.Attributes.Add("onclick", "return noChangeIfChecked(this)");

            chkCurrentRecord.CheckedChanged += new EventHandler(chkCurrentRecord_CheckedChanged);

            Label lblRecord = new Label();
            lblRecord.Text = "Current HitList : ";

            row = new TableRow();
            cell = new TableCell();
            cell.Controls.Add(lblRecord);
            cell.Controls.Add(chkCurrentRecord);
            row.Cells.Add(cell);

            tbl.Rows.Add(row);

            cell = new TableCell();
            row = new TableRow();
            cell.Controls.Add(webTab);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            tab.ContentPane.Controls.Add(tbl);
            //Drawing Manage Tab End

            //Drawing Edit Tab:
            CreateEditTab(tabEdit);
            PopulateDataInManageSubTab(tabEdit);
            //Edit Tab Complete.

            //Delete Tab Draw

            CreateDeleteTab(tabDelete);
            PopulateDataInManageSubTab(tabDelete);
            //Delete Tab Completed

            //Operation Tab Draw
            
            CreateOperationTab(tabOperations);
        }

        void chkCurrentRecord_CheckedChanged(object sender, EventArgs e)
        {
            RecordToManage = eRecordToManage.Current;
            CreateChildControls();
        }

        void wbCmbo_SelectedRowChanged(object sender, SelectedRowChangedEventArgs e)
        {
            SelectedHitlistManage = e.Row.Index;
            RecordToManage = eRecordToManage.Saved;
            CreateChildControls();
        }
        private void CreateOperationTab(Tab tabOperations)
        {
            DropDownList drpDwnSvdList = new DropDownList();
            RadioButtonList radioButtonOperationList = new RadioButtonList();
            Table tblOperation = new Table();
            TableRow rowDrpDown = new TableRow();
            TableRow rowTemp = new TableRow();
            TableRow rowBtn = new TableRow();
            TableCell cell = new TableCell();

            // Coverity Fix CID : 11833 
            
            try
            {
                drpDwnSvdList.ID = "OperationsDropDown";
                drpDwnSvdList.Width = 150;

                //Saved Hitlists
                if (!DesignMode)
                {
                    drpDwnSvdList.DataSource = COEHitListBOList.GetSavedHitListList(COEConfiguration.GetDatabaseNameFromAppName(AppName));
                    drpDwnSvdList.DataTextField = "Name";
                    drpDwnSvdList.DataValueField = "ID";
                    drpDwnSvdList.DataBind();
                }

                radioButtonOperationList.ID = "OperationsOptions";
                for (int i = 0; i < 3; i++)
                {
                    radioButtonOperationList.Items.Add(new ListItem());
                }

                radioButtonOperationList.Items[0].Selected = true;

                radioButtonOperationList.Items[0].Text = "Intersect with Current List";
                radioButtonOperationList.Items[1].Text = "Subtract with Current List";
                radioButtonOperationList.Items[2].Text = "Union with Current List";



                cell.Width = 50;
                rowDrpDown.Cells.Add(cell);

                cell = new TableCell();

                cell.Controls.Add(drpDwnSvdList);
                rowDrpDown.Cells.Add(cell);

                cell = new TableCell();
                cell.Width = 50;
                rowBtn.Cells.Add(cell);
                cell = new TableCell();
                Infragistics.WebUI.WebDataInput.WebImageButton btnPerformOperation = new Infragistics.WebUI.WebDataInput.WebImageButton();
                btnPerformOperation.Click += new Infragistics.WebUI.WebDataInput.ClickHandler(btnPerformOperation_Click);
                btnPerformOperation.Text = "Perform Operation";
                btnPerformOperation.ID = "Perform Operation";
                btnPerformOperation.Width = 150;
                cell.Controls.Add(btnPerformOperation);
                rowBtn.Cells.Add(cell);

                rowTemp = new TableRow();
                rowTemp.Height = 50;
                tblOperation.Rows.Add(rowTemp);

                tblOperation.Rows.Add(rowDrpDown);

                rowTemp = new TableRow();
                rowTemp.Height = 50;
                tblOperation.Rows.Add(rowTemp);

                rowTemp = new TableRow();
                cell = new TableCell();
                cell.Width = 50;
                rowTemp.Cells.Add(cell);
                cell = new TableCell();
                cell.Controls.Add(radioButtonOperationList);
                rowTemp.Cells.Add(cell);
                tblOperation.Rows.Add(rowTemp);

                rowTemp = new TableRow();
                rowTemp.Height = 50;
                tblOperation.Rows.Add(rowTemp);//To make space before Button.
                tblOperation.Rows.Add(rowBtn);

                tabOperations.ContentPane.Controls.Add(tblOperation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                drpDwnSvdList.Dispose();
                radioButtonOperationList.Dispose();
                cell.Dispose();
                rowBtn.Dispose();
                rowTemp.Dispose();
                rowDrpDown.Dispose();
                tblOperation.Dispose();
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPerformOperation_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            RadioButtonList radioButtonOperationList = (RadioButtonList)((Tab)((UltraWebTab)((Tab)mainTab.Tabs.FromKey("Manage")).FindControl("ManageSubtab")).Tabs.FromKey("Operations")).FindControl("OperationsOptions");
            DropDownList OperationsDropDown = (DropDownList)((Tab)((UltraWebTab)((Tab)mainTab.Tabs.FromKey("Manage")).FindControl("ManageSubtab")).Tabs.FromKey("Operations")).FindControl("OperationsDropDown");
            COEHitListBO obj=null;

            HitListInfo infoObj1 = new HitListInfo();
            HitListInfo infoObj2 = new HitListInfo();

            infoObj1.Database = infoObj2.Database = COEConfiguration.GetDatabaseNameFromAppName(AppName);

            HitListType infoObj1Type;

            if (RecordToManage == eRecordToManage.Saved)
            {
                Tab manageTab = (Tab)mainTab.Tabs.FromKey("Manage");
                WebCombo manageCombo = (WebCombo)manageTab.FindControl("WCCurrentRecord");   
                infoObj1.HitListID = Convert.ToInt32(manageCombo.SelectedRow.Cells.FromKey("ID").Value.ToString());
                infoObj1.HitListType = HitListType.SAVED;
            }
            else
            {
                infoObj1.HitListID = CurrentHitListID;
                infoObj1Type=CurrentHitListType;
            }
                infoObj2.HitListID = Convert.ToInt32(OperationsDropDown.SelectedItem.Value.ToString());
                switch (radioButtonOperationList.SelectedIndex)
                {
                    case 0:
                        obj = COEHitListOperationManager.IntersectHitList(infoObj2, infoObj1);

                        break;
                    case 1:
                        obj = COEHitListOperationManager.SubtractHitLists(infoObj2, infoObj1);
                        break;
                    case 2:
                        obj = COEHitListOperationManager.UnionHitLists(infoObj2, infoObj1);
                        break;
                }
                // Coverity Fix CID - 10469 (from local server)
                if (obj != null)
                {
                    obj.Save();
                    ResolvedHitListID = obj.ID;
                    ResolvedHitListType = obj.HitListType;
                }
            
            CreateChildControls();
        }

        private void CreateEditTab(Tab tabEdit)
        {
            Table tblEdit = new Table();

            TableRow rowName = new TableRow();
            TableRow rowDescription = new TableRow();
            TableRow rowIsPublic = new TableRow();
            TableRow rowSave = new TableRow();

            TableCell cell = new TableCell();
           
            cell.Width = 50;
            rowName.Cells.Add(cell);
            cell = new TableCell();
            Label lbl = new Label();
            lbl.Text = "List Name";
            cell.Controls.Add(lbl);
            rowName.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 50;
            rowDescription.Cells.Add(cell);
            cell = new TableCell();
            lbl = new Label();
            lbl.Text = "Description";
            cell.Controls.Add(lbl);
            rowDescription.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 50;
            rowIsPublic.Cells.Add(cell);
            cell = new TableCell();
            lbl = new Label();
            lbl.Text = "List Public";
            cell.Controls.Add(lbl);
            rowIsPublic.Cells.Add(cell);
            
            cell = new TableCell();
            TextBox txtBox = new TextBox();
            txtBox.ID = "Name";
            cell.Controls.Add(txtBox);
            rowName.Cells.Add(cell);

            
            cell = new TableCell();
            txtBox = new TextBox();
            txtBox.ID = "Description";
            cell.Controls.Add(txtBox);
            rowDescription.Cells.Add(cell);

            cell = new TableCell();
            CheckBox chkIsPublic = new CheckBox();
            chkIsPublic.ID = "IsPublic";
            cell.Controls.Add(chkIsPublic);
            rowIsPublic.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 50;
            rowSave.Cells.Add(cell);

            cell = new TableCell();
            Infragistics.WebUI.WebDataInput.WebImageButton btnEdit= new Infragistics.WebUI.WebDataInput.WebImageButton();
            btnEdit.Text = "Edit";
            btnEdit.ID = "Edit";
            cell.Controls.Add(btnEdit);
            rowSave.Cells.Add(cell);
            btnEdit.Click += new Infragistics.WebUI.WebDataInput.ClickHandler(btnEdit_Click);
            

            TableRow rowTemp = new TableRow();
            rowTemp.Height = 20;
            tblEdit.Rows.Add(rowTemp);
            tblEdit.Rows.Add(rowDescription);
            rowTemp = new TableRow();
            rowTemp.Height = 20;
            tblEdit.Rows.Add(rowTemp);
            tblEdit.Rows.Add(rowName);
            rowTemp = new TableRow();
            rowTemp.Height = 20;
            tblEdit.Rows.Add(rowTemp);
            tblEdit.Rows.Add(rowIsPublic);
            rowTemp = new TableRow();
            rowTemp.Height = 20;
            tblEdit.Rows.Add(rowTemp);
            tblEdit.Rows.Add(rowSave);

            //tblEdit.Width = 400;
            //tblEdit.Height = 300;
            tabEdit.ContentPane.Controls.Add(tblEdit);
        }
        /// <summary>
        /// Saves the Edited Data inside the Selected Hitlist Object from the WebCombo inside Manage Tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEdit_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {

            Tab manageTab = (Tab)mainTab.Tabs.FromKey("Manage");
            int hitListID;
            HitListType hitListType;
            if(RecordToManage== eRecordToManage.Saved)
            {
                WebCombo manageCombo = (WebCombo)manageTab.FindControl("WCCurrentRecord");
                hitListID=Convert.ToInt32(manageCombo.SelectedRow.Cells.FromKey("ID").Value.ToString());
                hitListType=HitListType.SAVED;
            }
            else
            {
                hitListID=CurrentHitListID;
                hitListType=CurrentHitListType;
            }

            Tab tab = (Tab)((UltraWebTab)manageTab.FindControl("ManageSubtab")).Tabs.FromKey("Edit");

            COEHitListBO hitListObj = COEHitListBO.Get(hitListType,hitListID);
            
            hitListObj.Name=((TextBox)tab.FindControl("Name")).Text;
            hitListObj.Description=((TextBox)tab.FindControl("Description")).Text;
            hitListObj.IsPublic=((CheckBox)tab.FindControl("IsPublic")).Checked;

            hitListObj.Save();

            //Refresh the Control.
            CreateChildControls();
        }
        private void CreateDeleteTab(Tab tabDelete)
        {
            Table tblDelete = new Table();

            TableRow rowName = new TableRow();
            TableRow rowDescription = new TableRow();
            TableRow rowIsPublic = new TableRow();
            TableRow rowSave = new TableRow();

            TableCell cell = new TableCell();

            // Coverity Fix CID : 11832 
            try
            {


                cell = new TableCell();
                cell.Width = 50;
                rowName.Cells.Add(cell);

                cell = new TableCell();
                Label lbl = new Label();
                lbl.Text = "List Name";
                cell.Controls.Add(lbl);
                rowName.Cells.Add(cell);

                cell = new TableCell();
                cell.Width = 50;
                rowDescription.Cells.Add(cell);

                cell = new TableCell();
                lbl = new Label();
                lbl.Text = "Description";
                cell.Controls.Add(lbl);
                rowDescription.Cells.Add(cell);

                cell = new TableCell();
                cell.Width = 50;
                rowIsPublic.Cells.Add(cell);

                cell = new TableCell();
                lbl = new Label();
                lbl.Text = "List Public";
                cell.Controls.Add(lbl);
                rowIsPublic.Cells.Add(cell);

                cell = new TableCell();
                TextBox txtBox = new TextBox();
                txtBox.ReadOnly = true;
                txtBox.ID = "Name";
                cell.Controls.Add(txtBox);
                rowName.Cells.Add(cell);

                cell = new TableCell();
                txtBox = new TextBox();
                txtBox.ReadOnly = true;
                txtBox.ID = "Description";
                cell.Controls.Add(txtBox);
                rowDescription.Cells.Add(cell);

                cell = new TableCell();
                CheckBox chkIsPublic = new CheckBox();
                chkIsPublic.ID = "IsPublic";

                //Inorder to make it ReadOnly:
                string str = @"<script language='javascript' type='text/javascript'>
                          //Making the CheckBox ReadOnly:
                          function noChange(checkbox)
                          {
                            checkbox.checked=! checkbox.checked;
                          }
                          //To Confirm before Delete:
                           function confirm_delete(oButton, oEvent){
                            if (confirm('Are you sure you want to delete the Hitlist?')==true)
                              oEvent.cancel = false;
                            else
                              oEvent.cancel = true;
                            }
                          </script>";

                this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "CheckBoxDisplayOnly", str);
                chkIsPublic.Attributes.Add("onclick", "noChange(this)");

                cell.Controls.Add(chkIsPublic);
                rowIsPublic.Cells.Add(cell);

                cell = new TableCell();
                cell.Width = 50;
                rowSave.Cells.Add(cell);

                cell = new TableCell();
                Infragistics.WebUI.WebDataInput.WebImageButton btnDelete = new Infragistics.WebUI.WebDataInput.WebImageButton();
                btnDelete.StylePreset = Infragistics.WebUI.WebDataInput.ButtonStylePreset.WindowsXPBlue;
                btnDelete.Text = "Delete";
                btnDelete.ID = "Delete";
                cell.Controls.Add(btnDelete);
                rowSave.Cells.Add(cell);

                btnDelete.ClientSideEvents.Click = "confirm_delete";

                btnDelete.Click += new Infragistics.WebUI.WebDataInput.ClickHandler(btnDelete_Click);

                TableRow rowTemp = new TableRow();
                rowTemp.Height = 20;
                tblDelete.Rows.Add(rowTemp);
                tblDelete.Rows.Add(rowDescription);
                rowTemp = new TableRow();
                rowTemp.Height = 20;
                tblDelete.Rows.Add(rowTemp);
                tblDelete.Rows.Add(rowName);
                rowTemp = new TableRow();
                rowTemp.Height = 20;
                tblDelete.Rows.Add(rowTemp);
                tblDelete.Rows.Add(rowIsPublic);
                rowTemp = new TableRow();
                rowTemp.Height = 20;
                tblDelete.Rows.Add(rowTemp);
                tblDelete.Rows.Add(rowSave);

                tabDelete.ContentPane.Controls.Add(tblDelete);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cell.Dispose();
                rowName.Dispose();
                rowDescription.Dispose();
                rowIsPublic.Dispose();
                rowSave.Dispose();
                tblDelete.Dispose();
            }
        }

        void btnDelete_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            int hitListID;
            HitListType hitListType;
            if (RecordToManage == eRecordToManage.Saved)
            {
                Tab manageTab = (Tab)mainTab.Tabs.FromKey("Manage");
                WebCombo manageCombo = (WebCombo)manageTab.FindControl("WCCurrentRecord");
                hitListID = Convert.ToInt32(manageCombo.SelectedRow.Cells.FromKey("ID").Value.ToString());
                hitListType = HitListType.SAVED;
            }
            else
            {
                hitListID = CurrentHitListID;
                hitListType = CurrentHitListType;
            }
            
            COEHitListBO.Delete(hitListType,hitListID);
            //Refresh the Control.
            CreateChildControls();
        }
        /// <summary>
        /// Populate the Edit and Delete tabs value.
        /// </summary>
        /// <param name="tab">Either of the Tab inside Manage Tab, Edit or Delete</param>
        private void PopulateDataInManageSubTab(Tab tab)
        {
            COEHitListBO hitListObj;
            if (!DesignMode)
            {
                if (RecordToManage == eRecordToManage.Saved)
                {
                    Tab manageTab = (Tab)mainTab.Tabs.FromKey("Manage");
                    WebCombo manageCombo = (WebCombo)manageTab.FindControl("WCCurrentRecord");
                    hitListObj = COEHitListBO.Get( HitListType.SAVED, Convert.ToInt32(manageCombo.SelectedRow.Cells.FromKey("ID").Value.ToString()));
                }
                else
                {
                    hitListObj = COEHitListBO.Get(CurrentHitListType, CurrentHitListID);
                }

                ((TextBox)tab.FindControl("Name")).Text = hitListObj.Name;
                ((TextBox)tab.FindControl("Description")).Text = hitListObj.Description;
                ((CheckBox)tab.FindControl("IsPublic")).Checked = hitListObj.IsPublic;
            }
        }
        /// <summary>
        /// Populates the Save Tab Fields with the CurrentHitlist Data.
        /// </summary>
        /// <param name="tab"></param>
        private void PopulateSaveTab(Tab tab)
        {
            if (!DesignMode)
            {
                COEHitListBO hitListObj = COEHitListBO.Get( CurrentHitListType, CurrentHitListID);
                ((TextBox)tab.FindControl("Name")).Text = hitListObj.Name;
                ((TextBox)tab.FindControl("Description")).Text = hitListObj.Description;
                ((CheckBox)tab.FindControl("IsPublic")).Checked = hitListObj.IsPublic;
            }
        }
        #endregion
        #region Internal Classes

#endregion
    }
}