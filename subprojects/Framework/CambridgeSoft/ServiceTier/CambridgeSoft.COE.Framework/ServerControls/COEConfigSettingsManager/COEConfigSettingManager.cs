using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebTab;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Drawing;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Properties;
using System.Collections.Generic;
using System.Reflection;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEConfigSettingManager", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEConfigSettingManager
{
    /// <summary>
    /// A user defined control is used to set page's controls.
    /// </summary>
    [ToolboxData("<{0}:COEConfigSettingManager runat=server></{0}:COEConfigSettingManager>")]
    public class COEConfigSettingManager : UltraWebTab, INamingContainer
    {
        #region Constants
        const int PROCESSOR_PARAMETERS_OLD_VALUE_INDEX = 0;
        const int PROCESSOR_PARAMETERS_NEW_VALUE_INDEX = 1;
        const int PROCESSOR_PARAMETERS_PROCESSOR_CLASS_INDEX = 2;
        #endregion

        #region Variables

        private SettingsGroup _webGridDataSource;
        private AppSettingsData _appSettingsData;
        private string _currentApplicationName;
        private readonly string _nameKey = "Name";
        private readonly string _typeKey = "Type";
        private readonly string _valueKey = "Value";
        private readonly string _descKey = "Description";
        private readonly string _userValuesListKey = "UserValuesList";
        private readonly string _isNewKey = "IsNew";
        private readonly string _isAdminKey = "IsAdmin";
        private readonly string _isDeletedKey = "IsDeleted";
        private readonly string _processorClassKey = "Processor";
        
        private string _valueListString = "";
        private int _valueListIndex = 0;
        private bool _manageConfigurationSettings = false;
        private ValueList _appSettingTypes;
        private ValueList _isAdminTypes;
        private string _cancelButtonURL = "~/Forms/Public/ContentArea/Home.aspx";

        #endregion

        #region Properties

        public AppSettingsData AppSettings
        {
            get
            {
                return _appSettingsData;
            }
            set 
            { 
                _appSettingsData = value; 
            }
        }

        public string CurrentApplicationName
        {
            get
            {
                return _currentApplicationName;
            }
            set
            {
                _currentApplicationName = value;
            }
        }

        public string CancelButtonURL
        {
            get 
            { 
                return _cancelButtonURL; 
            }
            set 
            { 
                _cancelButtonURL = value; 
            }
        }

        #endregion

        #region Methods

        private void SaveChanges()
        {
            ((Table)this.SelectedTabObject.FindControl("messageTable")).Visible = false;
            Dictionary<string, List<string>> modifiedSettingsToProcess = new Dictionary<string, List<string>>();
            foreach (SettingsGroup settingGroup in this.AppSettings.SettingsGroup)
            {
                if (this.Tabs.FromKeyTab(settingGroup.Name) != null)
                {
                    Infragistics.WebUI.UltraWebGrid.UltraWebGrid grid = (UltraWebGrid)this.Tabs.FromKeyTab(settingGroup.Name).FindControl(settingGroup.Name + "WebGrid");
                    List<UltraGridRow> toDeleteRows = new List<UltraGridRow>();
                    if (grid != null)
                    {
                        foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in grid.Rows)
                        {
                            if (row.Cells[2].Text == null)
                                row.Cells[2].Text = string.Empty;

                            if (row.Cells[0].Text != null && row.Cells[7].Text != "1")
                            {
                                if (settingGroup.Settings.Get(row.Cells[0].Text) != null)
                                {
                                    settingGroup.Settings.Get(row.Cells[0].Text).ControlType = row.Cells[1].Text;
                                    if (settingGroup.Settings.Get(row.Cells[0].Text).Value != row.Cells[2].Text && !String.IsNullOrEmpty(row.Cells[8].Text))
                                    {
                                        List<string> processorParameters = new List<string>();
                                        processorParameters.Add(settingGroup.Settings.Get(row.Cells[0].Text).Value);
                                        processorParameters.Add(row.Cells[2].Text);
                                        processorParameters.Add(row.Cells[8].Text);
                                        modifiedSettingsToProcess.Add(row.Cells[0].Text, processorParameters);
                                    }
                                    settingGroup.Settings.Get(row.Cells[0].Text).Value = row.Cells[2].Text;
                                    settingGroup.Settings.Get(row.Cells[0].Text).Description = row.Cells[3].Text;
                                    settingGroup.Settings.Get(row.Cells[0].Text).AllowedValues = row.Cells[4].Text;
                                    settingGroup.Settings.Get(row.Cells[0].Text).IsAdmin = row.Cells[6].Text;
                                    settingGroup.Settings.Get(row.Cells[0].Text).ProcessorClass = row.Cells[8].Text;
                                }
                                else
                                {
                                    AppSetting newSetting = new AppSetting();
                                    newSetting.Name = row.Cells[0].Text;
                                    newSetting.ControlType = row.Cells[1].Text;
                                    newSetting.Value = row.Cells[2].Text;
                                    newSetting.AllowedValues = row.Cells[4].Text;
                                    newSetting.IsAdmin = row.Cells[6].Text;
                                    if (row.Cells[3].Text != null)
                                        newSetting.Description = row.Cells[3].Text;
                                    else
                                        newSetting.Description = Resources.COEConfigSettingManager_No_Description_Default_Text;
                                    settingGroup.Settings.Add(newSetting);
                                }
                            }
                            else if (row.Cells[7].Text == "1")
                            {
                                settingGroup.Settings.Remove(row.Cells[0].Text);
                                toDeleteRows.Add(row);
                            }
                        }
                        foreach (UltraGridRow toDeleteRow in toDeleteRows)
                        {
                            grid.Rows.Remove(toDeleteRow);
                        }
                    }
                }
            }

            FrameworkUtils.SaveAppConfigSettings(this.CurrentApplicationName, this.AppSettings);

            foreach (KeyValuePair<string, List<string>> currentEntry in modifiedSettingsToProcess)
            {
                string assemblyName = currentEntry.Value[PROCESSOR_PARAMETERS_PROCESSOR_CLASS_INDEX].Substring(0, currentEntry.Value[PROCESSOR_PARAMETERS_PROCESSOR_CLASS_INDEX].LastIndexOf('.'));
                Assembly processorAssembly = Assembly.Load(assemblyName);
                Type processorType = null;
                foreach (Type currentType in processorAssembly.GetTypes())
                {
                    if (currentType.Name == currentEntry.Value[PROCESSOR_PARAMETERS_PROCESSOR_CLASS_INDEX].Substring(currentEntry.Value[PROCESSOR_PARAMETERS_PROCESSOR_CLASS_INDEX].LastIndexOf('.') + 1))
                    {
                        processorType = currentType;
                        break;
                    }
                }
                ICOEConfigurationProcessor configurationProcessor = Activator.CreateInstance(processorType) as ICOEConfigurationProcessor;
                string key = currentEntry.Key;
                string oldValue = currentEntry.Value[PROCESSOR_PARAMETERS_OLD_VALUE_INDEX];
                string newValue = currentEntry.Value[PROCESSOR_PARAMETERS_NEW_VALUE_INDEX];
                configurationProcessor.Process(key, oldValue, newValue);
            }
            InitializeControls();
            //Success Message
            this.ShowConfirmationMessage(Resources.COEConfigSettingManager_Save_Success_Message);
        }

        private void BuildTabs()
        {
            Button saveButton, deleteSelectedButton, addNewButton;
            Table messageTable;
            Label messageLabel;
            int currentIndex = 0;
           // COESpotFireSettingsBO SpotfireSetting = ConfigurationUtilities.GetSpotFireSettings(false);
            foreach (SettingsGroup settingsGroup in this.AppSettings.SettingsGroup)
            {                
                //if ((SpotfireSetting.SpotfireUser != "") || (SpotfireSetting.SpotfireUser == "" && settingsGroup.Name != "DVManager"))
                //{
                if (settingsGroup.Name != "DVManager" && settingsGroup.Title != "DataViewManager settings")
                {
                    Infragistics.WebUI.UltraWebTab.Tab tab = new Infragistics.WebUI.UltraWebTab.Tab();
                    tab.Key = settingsGroup.Name;
                    tab.Text = settingsGroup.Title;
                    messageLabel = new Label();
                    messageLabel.ID = "messageLabel";
                    messageLabel.SkinID = "MessagesAreaTextLabel";
                    messageTable = new Table();
                    messageTable.ID = "messageTable";
                    messageTable.CssClass = "MessageAreaTable";
                    //messageTable.Style.Add(HtmlTextWriterStyle.Width,"100%");
                    TableRow messageRow = new TableRow();
                    TableCell messageCell = new TableCell();
                    messageCell.Attributes.Add("align", "center");
                    messageCell.Controls.Add(messageLabel);
                    messageRow.Controls.Add(messageCell);
                    messageTable.Controls.Add(messageRow);
                    messageTable.Visible = false;
                    tab.ContentPane.Controls.Add(messageTable);
                    UltraWebGrid webGridUC = new UltraWebGrid();
                    webGridUC.EnableViewState = false;
                    webGridUC.ID = settingsGroup.Name + "WebGrid";
                    webGridUC.SkinID = "AppSettingsUltraWebGridSkin";
                    BindWebGrid(ref webGridUC, settingsGroup);
                    webGridUC.DisplayLayout.ClientSideEvents.BeforeEnterEditModeHandler = "BeforeEnterEditMode";
                    webGridUC.DisplayLayout.ClientSideEvents.DblClickHandler = "DblClick";
                    webGridUC.DisplayLayout.ClientSideEvents.BeforeExitEditModeHandler = "BeforeExitEditMode";
                    webGridUC.DisplayLayout.ClientSideEvents.AfterRowInsertHandler = "AfterRowInsert";
                    webGridUC.DisplayLayout.ClientSideEvents.BeforeRowDeletedHandler = "BeforeDelete";
                    tab.ContentPane.Controls.Add(webGridUC);
                    Panel buttonsPanel = new Panel();
                    buttonsPanel.CssClass = "ConfigSettingManagerButtons";
                    if (this._manageConfigurationSettings == true)
                    {
                        addNewButton = new Button();
                        addNewButton.SkinID = "ButtonBig";
                        addNewButton.Text = Resources.COEConfigSettingManager_Add_Button_Text;
                        addNewButton.OnClientClick = "return AddNew('" + (string)(this.ClientID + this.ClientIDSeparator + this.ClientIDSeparator + "ctl" + currentIndex.ToString() + this.ClientIDSeparator + webGridUC.UniqueID).Replace('_', 'x') + "')";
                        buttonsPanel.Controls.Add(addNewButton);
                        deleteSelectedButton = new Button();
                        deleteSelectedButton.SkinID = "ButtonBig";
                        deleteSelectedButton.Text = Resources.COEConfigSettingManager_Delete_Button_Text;
                        deleteSelectedButton.ToolTip = Resources.COEConfigSettingManager_Delete_Button_Tooltip;
                        deleteSelectedButton.OnClientClick = "return DeleteSelected('" + (string)(this.ClientID + this.ClientIDSeparator + this.ClientIDSeparator + "ctl" + currentIndex.ToString() + this.ClientIDSeparator + webGridUC.UniqueID).Replace('_', 'x') + "')";
                        buttonsPanel.Controls.Add(deleteSelectedButton);
                    }
                    saveButton = new Button();
                    saveButton.SkinID = "ButtonBig";
                    saveButton.Text = Resources.COEConfigSettingManager_Save_Button_Text;
                    saveButton.Click += new EventHandler(SaveButton_Click);
                    buttonsPanel.Controls.Add(saveButton);
                    tab.ContentPane.Controls.Add(buttonsPanel);
                    Infragistics.WebUI.UltraWebTab.TabItem ti;
                    if (webGridUC.Rows.Count > 0)
                    {
                        this.Tabs.Add(tab);
                        currentIndex++;
                    }
                }
                //}
            }

        }

        private void InitializeControls()
        {
            this._manageConfigurationSettings = ConfigurationUtilities.GetManageConfigurationSettings();
            this.AppSettings = FrameworkUtils.GetAppConfigSettings(this.CurrentApplicationName,true);
            this._appSettingTypes = this.GetAppSettingTypesValueList();
            this._isAdminTypes = this.GetIsAdminTypesValueList();
            if (this.AppSettings != null)
            {
                // Clear tabs to refresh grids
                this.Tabs.Clear();
                BuildTabs();
            }
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            ((Table)this.SelectedTabObject.FindControl("messageTable")).Visible = true;
            ((Label)this.SelectedTabObject.FindControl("messageLabel")).Text = messageToDisplay;
        }

        private void BindWebGrid(ref UltraWebGrid ultraWebGrid, SettingsGroup currentSetting)
        {
            int currentIndex = 0;
            DataTable settingsDT = new DataTable(currentSetting.Name);
            DataColumn nameDC = new DataColumn(this._nameKey);
            DataColumn typeDC = new DataColumn(this._typeKey);
            DataColumn valueDC = new DataColumn(this._valueKey);
            DataColumn descriptionDC = new DataColumn(this._descKey);
            DataColumn usersValueListDC = new DataColumn(this._userValuesListKey);
            usersValueListDC.Caption = "List of Values";
            DataColumn isNewDC = new DataColumn(this._isNewKey);
            DataColumn isAdminDC = new DataColumn(this._isAdminKey);
            DataColumn isDeletedDC = new DataColumn(this._isDeletedKey);
            DataColumn processorClassDC = new DataColumn(this._processorClassKey);
            isAdminDC.Caption = "Is Admin";
            settingsDT.Columns.Add(nameDC);
            settingsDT.Columns.Add(typeDC);
            settingsDT.Columns.Add(valueDC);
            settingsDT.Columns.Add(descriptionDC);
            settingsDT.Columns.Add(usersValueListDC);
            settingsDT.Columns.Add(isNewDC);
            settingsDT.Columns.Add(isAdminDC);
            settingsDT.Columns.Add(isDeletedDC);
            settingsDT.Columns.Add(processorClassDC);
            foreach (AppSetting appSetting in currentSetting.Settings)
            {
                if (!(appSetting.IsHidden.ToLower() == bool.TrueString.ToLower()))
                {
                    if ((appSetting.IsAdmin == bool.TrueString && this._manageConfigurationSettings == true) || appSetting.IsAdmin != bool.TrueString)
                    {
                        DataRow row = settingsDT.NewRow();
                        if (appSetting.ControlType == AppSettingType.PICKLIST.ToString())
                        {
                            row[usersValueListDC] = appSetting.AllowedValues;
                        }
                        row[nameDC] = appSetting.Name;
                        row[typeDC] = String.IsNullOrEmpty(appSetting.ControlType) ? AppSettingType.TEXT.ToString() : appSetting.ControlType;
                        row[isAdminDC] = String.IsNullOrEmpty(appSetting.IsAdmin) ? bool.FalseString : appSetting.IsAdmin;
                        row[valueDC] = appSetting.Value;
                        row[descriptionDC] = appSetting.Description;
                        row[isNewDC] = 0;
                        row[processorClassDC] = appSetting.ProcessorClass;
                        settingsDT.Rows.Add(row);
                    }
                }
            }
            if (settingsDT.Rows.Count > 0)
            {
                ultraWebGrid.DataSource = settingsDT;
                ultraWebGrid.DataBind();
                ultraWebGrid.Bands[0].Columns[nameDC.Ordinal].Width = Unit.Percentage(15);
                ultraWebGrid.Bands[0].Columns[typeDC.Ordinal].Type = ColumnType.DropDownList;
                ultraWebGrid.Bands[0].Columns[typeDC.Ordinal].DataType = "System.String";
                ultraWebGrid.Bands[0].Columns[typeDC.Ordinal].ValueList = this._appSettingTypes;
                ultraWebGrid.Bands[0].Columns[typeDC.Ordinal].Width = Unit.Percentage(8);
                ultraWebGrid.Bands[0].Columns[valueDC.Ordinal].Type = ColumnType.DropDownList;
                ultraWebGrid.Bands[0].Columns[valueDC.Ordinal].DataType = "System.String";
                ultraWebGrid.Bands[0].Columns[valueDC.Ordinal].Width = Unit.Percentage(15);
                ultraWebGrid.Bands[0].Columns[valueDC.Ordinal].HTMLEncodeContent = false;
                ultraWebGrid.Bands[0].Columns[isNewDC.Ordinal].Hidden = true;
                ultraWebGrid.Bands[0].Columns[isAdminDC.Ordinal].Type = ColumnType.DropDownList;
                ultraWebGrid.Bands[0].Columns[isAdminDC.Ordinal].DataType = "System.String";
                ultraWebGrid.Bands[0].Columns[isAdminDC.Ordinal].ValueList = this._isAdminTypes;
                ultraWebGrid.Bands[0].Columns[isAdminDC.Ordinal].Width = Unit.Percentage(8);
                ultraWebGrid.Bands[0].Columns[isDeletedDC.Ordinal].Hidden = true;
                ultraWebGrid.Bands[0].Columns[descriptionDC.Ordinal].Width = Unit.Percentage(30);
                ultraWebGrid.Bands[0].Columns[valueDC.Ordinal].AllowUpdate = AllowUpdate.Yes;
                ultraWebGrid.Bands[0].Columns[usersValueListDC.Ordinal].Width = Unit.Percentage(15);
                ultraWebGrid.Bands[0].Columns[processorClassDC.Ordinal].Width = Unit.Percentage(14);
                if (this._manageConfigurationSettings == true)
                {
                    ultraWebGrid.Bands[0].Columns[typeDC.Ordinal].AllowUpdate = AllowUpdate.Yes;
                    ultraWebGrid.Bands[0].Columns[descriptionDC.Ordinal].AllowUpdate = AllowUpdate.Yes;
                    ultraWebGrid.Bands[0].Columns[usersValueListDC.Ordinal].AllowUpdate = AllowUpdate.Yes;
                    ultraWebGrid.Bands[0].Columns[isAdminDC.Ordinal].AllowUpdate = AllowUpdate.Yes;
                    ultraWebGrid.Bands[0].Columns[processorClassDC.Ordinal].AllowUpdate = AllowUpdate.Yes;
                    ultraWebGrid.DisplayLayout.AllowAddNewDefault = AllowAddNew.Yes;
                    ultraWebGrid.DisplayLayout.AllowDeleteDefault = AllowDelete.Yes;
                    ultraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
                }
                else
                {
                    ultraWebGrid.Bands[0].Columns[usersValueListDC.Ordinal].Hidden= true;
                    ultraWebGrid.Bands[0].Columns[isAdminDC.Ordinal].Hidden = true;
                }
            }
        }

        private int CreateValueListString(int currentIndex, DataColumn valueListDC, AppSetting appSetting, DataRow row)
        {
            if (!String.IsNullOrEmpty(appSetting.AllowedValues))
            {
                string[] allowedValues = appSetting.AllowedValues.Split('|');
                this._valueListString += "valueList[" + _valueListIndex.ToString() + "]=new Array();";
                currentIndex = 0;
                foreach (string currentValue in allowedValues)
                {
                    this._valueListString += "valueList[" + _valueListIndex.ToString() + "][" + currentIndex.ToString() + "]=new Array('" + currentValue + "','" + currentValue + "');";
                    currentIndex++;
                }
                row[valueListDC] = _valueListIndex;
                _valueListIndex++;
            }
            return currentIndex;
        }

        private ValueList GetAppSettingTypesValueList()
        {
            ValueList retValueList = new ValueList();
            foreach (string i in Enum.GetNames(typeof(AppSettingType)))
            {
                retValueList.ValueListItems.Add(i);
            }
            return retValueList;
        }

        private ValueList GetIsAdminTypesValueList()
        {
            ValueList retValueList = new ValueList();
            retValueList.ValueListItems.Add(bool.FalseString);
            retValueList.ValueListItems.Add(bool.TrueString);
            return retValueList;
        }

        #endregion

        #region Events
        
        protected override void OnLoad(EventArgs e)
        {
            InitializeControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Page on PreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!Page.IsClientScriptBlockRegistered("BeforeEnterEditMode"))
            {
                string strCode = @"
                        <script>
                            var valueList = new Array();"+this._valueListString+ @"          			                    
                            function BeforeEnterEditMode(tableName, cellName)
		                    {
			                    var col = igtbl_getColumnById(cellName);
			                    if (col.Key != '"+this._valueKey+@"')
                                {
                                    return;
                                }
			                    var row = igtbl_getRowById(cellName);
                                if (row.getCell(1).getValue()=='"+AppSettingType.PICKLIST.ToString()+@"')                                
                                {
                                    var allowedValues = row.getCell(4).getValue();                                
                                    if (allowedValues)
                                    {			                    
                                        dynamicValueList = new Array();                                    
                                        allowedValuesArr = allowedValues.split('|');                                                                      for(i = 0; i<allowedValuesArr.length; i++)
                                        {
                                            dynamicValueList[i] = new Array(allowedValuesArr[i],allowedValuesArr[i]);
                                        }
                                        col.ColumnType=5;                                        
                                        col.ValueList = dynamicValueList;
                                    }
                                    else
                                    {
                                        alert('Please enter a list of values for the picklist');
                                        return;
                                    }
                                }
                                else
                                {
                                    col.ColumnType=1;
                                    col.ValueList=null;
                                }
		                    }
		                </script>
                ";
                Page.RegisterClientScriptBlock("BeforeEnterEditMode", strCode);
            }
            if (!Page.IsClientScriptBlockRegistered("DblClick"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function AllowUpdate(col)
                {
                    col.AllowUpdate=1;
                }        
                function DblClick(tableName, itemName)
                {
        			var cell = igtbl_getElementById(itemName);
                    var col = igtbl_getColumnById(cell.Object.Column.Id);			        
                    var row = igtbl_getRowById(itemName);
				    var cell = row.getCell(5);   
                    row.select();
                    if(col.Key=='" + this._nameKey+ @"' && cell.getValue()!='1' && "+this._manageConfigurationSettings.ToString().ToLower()+ @")
			        {
                        var handleYes = function(columnObj) {
                            confirmDialog.destroy();
                            AllowUpdate(columnObj);                            
                        };

                        var handleNo = function() {
                            confirmDialog.destroy();
                         };      
                        confirmDialog = new YAHOO.widget.SimpleDialog('yuiConfirm',
                                                                    { width: '300px',
                                                                    fixedcenter: true,
                                                                    visible: false,
                                                                    draggable: false,
                                                                    close: false,
                                                                    modal: true,
                                                                    text: '" + Resources.COEConfigSettingManager_EditName + @"',
                                                                    icon: YAHOO.widget.SimpleDialog.ICON_HELP,
                                                                    constraintoviewport: true,
                                                                    buttons: [ { text:'Yes', handler: function(){handleYes(col);}},
                                                                            { text:'No', handler: handleNo  } ]
                                                                    } );
                        confirmDialog.setHeader('Confirmation');
                        confirmDialog.columnObject = col;
                        confirmDialog.render(document.body);
                        confirmDialog.show();
                        return false;
			        }
                    else if (col.Key=='" + this._nameKey + @"' && cell.getValue()=='1' && "+this._manageConfigurationSettings.ToString().ToLower()+@")
                    {
                        col.AllowUpdate=1;
                        return false;
                    }
                }
                </script>";
                Page.RegisterClientScriptBlock("DblClick", strCode);
            }
            if (!Page.IsClientScriptBlockRegistered("BeforeExitEditMode"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function BeforeExitEditMode(tableName, itemName)
                {
        			var cell = igtbl_getElementById(itemName);
                    var col = igtbl_getColumnById(cell.Object.Column.Id);			        
                    if(col.Key=='"+this._nameKey+@"')
			        {
                        col.AllowUpdate=0;
        				return false;
			        }
                }
                </script>";
                Page.RegisterClientScriptBlock("BeforeExitEditMode", strCode);
            }

            if (!Page.IsClientScriptBlockRegistered("AfterRowInsert"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function AfterRowInsert(tableName, itemName)
                {
                    var row = igtbl_getRowById(itemName);
				    var cell = row.getCell(5);                  			
                    cell.setValue('1');
                    var cell = row.getCell(7);                  			
                    cell.setValue('0');
                }
                </script>";
                Page.RegisterClientScriptBlock("AfterRowInsert", strCode);
            }
            if (!Page.IsClientScriptBlockRegistered("DeleteSelected"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function DeleteSelected(gridName)
                {
                    if (" + this._manageConfigurationSettings.ToString().ToLower() + @")                    
                    {
                        var handleYes = function(gridName) {
                            confirmDialog.destroy();
                            DeleteRows(gridName);                            
                        };
                        var handleNo = function() {
                            confirmDialog.destroy();
                         };      
                        confirmDialog = new YAHOO.widget.SimpleDialog('yuiConfirm',
                                                                    { width: '300px',
                                                                    fixedcenter: true,
                                                                    visible: false,
                                                                    draggable: false,
                                                                    close: false,
                                                                    modal: true,
                                                                    text: '" + Resources.COEConfigSettingManager_DeleteConfirmation + @"',
                                                                    icon: YAHOO.widget.SimpleDialog.ICON_HELP,
                                                                    constraintoviewport: true,
                                                                    buttons: [ { text:'Yes', handler: function(){handleYes(gridName);}},
                                                                            { text:'No', handler: handleNo  } ]
                                                                    } );
                        confirmDialog.setHeader('Confirmation');
                        confirmDialog.gridName = gridName;
                        confirmDialog.render(document.body);
                        confirmDialog.show();
                    }
                    return false;
                }

                function DeleteRows(gridName)
                {
                    var grid = igtbl_getGridById(gridName);                        
                    for(var rowId in grid.SelectedRows)
                    {
                        var row=igtbl_getRowById(rowId);
                        row.getCell(7).setValue('1');
                        row.setHidden(true);
                    }
                }
                </script>";
                Page.RegisterClientScriptBlock("DeleteSelected", strCode);
            }

            if (!Page.IsClientScriptBlockRegistered("AddNew"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function AddNew(gridName)
                {
                    if (" + this._manageConfigurationSettings.ToString().ToLower() + @")     
                    {
                        igtbl_addNew(gridName,0);
                    }
                    return false;
                }                            
                </script>";
                Page.RegisterClientScriptBlock("AddNew", strCode);
            }

        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                this.SaveChanges();
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            this.Page.Server.Transfer(this.CancelButtonURL, false);
        }

        #endregion
    }

}