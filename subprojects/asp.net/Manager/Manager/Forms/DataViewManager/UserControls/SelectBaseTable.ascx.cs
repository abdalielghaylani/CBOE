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
using System.Collections.Generic;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDataViewService;

public partial class SelectBaseTable : System.Web.UI.UserControl
{
    #region Properties
    private List<string> SchemaList
    {
        get
        {
            if (ViewState["SchemaList"] == null)
                ViewState["SchemaList"] = new List<string>();
            return (List<string>)ViewState["SchemaList"];
        }
        set
        {
            ViewState["SchemaList"] = value;
        }
    }

    private COEDataViewBO SelectedDataView
    {
        get
        {
            COEDataViewBO currentDV = null;
            if (Session[Constants.COEDataViewBO] != null)
                currentDV = ((COEDataViewBO)Session[Constants.COEDataViewBO]);
            return currentDV;
        }
        set { }
    }

    private int SelectedTableID
    {
        get
        {
            if (ViewState["SelectedTableID"] == null)
                return int.MinValue;
            else
                return (int)ViewState["SelectedTableID"];
        }
        set
        {
            ViewState["SelectedTableID"] = value;
        }
    }
    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        ScriptManager scMan = ScriptManager.GetCurrent(this.Page);
        if (scMan != null)
            scMan.RegisterAsyncPostBackControl(this.SchemasDropDownList);

        this.SchemasDropDownList.SelectedIndexChanged += new EventHandler(SchemasDropDownList_SelectedIndexChanged);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }

        //Jira ID: CBOE-1161 : Veritcal scroll bar 
        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "afterpostback();", true);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void SchemasDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        TableBO tbl = this.Unbind();

        if (tbl != null)
            this.SelectedTableID = tbl.ID;

        this.BindTables(SelectedDataView.DataViewManager.Tables, SelectedDataView.DataViewManager.BaseTableId);
    }

    

    #endregion

    #region Methods

    /// <summary>
    /// Method to set all the controls attributtes as Text, tooltip, etc...
    /// </summary>
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RadioListrReqFieldValidator.Text = "*";
        this.RadioListrReqFieldValidator.ToolTip = this.RadioListrReqFieldValidator.ErrorMessage = Resources.Resource.SelectBaseTable_Validator_Text;
        this.SelectBaseTableLabel.Text = Resources.Resource.SelectBaseTable_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public override void DataBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        //CBOE-706 : User should not be able to change Base table in Create from Existing. ASV 12042013
        if (this.SelectedDataView.ID > 0) //check if the view is saved and not a master dataview
        {
            this.SchemasDropDownList.Enabled = false;
            this.RadioButtonList.Enabled = false;
            this.SelectBaseTableLabel.Enabled = false;

             if ((Session["IsBaseEnable"] != null && Convert.ToBoolean(Session["IsBaseEnable"]).Equals(true)))
            {
                this.SchemasDropDownList.Enabled = true;
                this.RadioButtonList.Enabled = true;
                this.SelectBaseTableLabel.Enabled = true;
            }
        }
        //END CBOE-706
        Session["MasterSchema"] = SelectedDataView;
        
        TableListBO tablesList = SelectedDataView.DataViewManager.Tables;
        int baseTableId = SelectedDataView.DataViewManager.BaseTableId;
        #region Bind Schemas
        List<string> schemas = new List<string>();
        string selectedSchema = string.Empty;        

        foreach (TableBO currentTable in tablesList)
        {
            if (!SchemaList.Contains(currentTable.DataBase))
            {
                SchemaList.Add(currentTable.DataBase);
                if (currentTable.ID == baseTableId)
                {
                    selectedSchema = currentTable.DataBase;
                }
            }
        }

        
       this.SchemasDropDownList.DataSource = SchemaList;
        this.SchemasDropDownList.DataBind();

        if (!string.IsNullOrEmpty(selectedSchema))
        this.SchemasDropDownList.SelectedValue = selectedSchema;
        #endregion
        this.BindTables(tablesList, baseTableId);

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void BindTables(TableListBO tablesList, int baseTableId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RadioButtonList.Items.Clear();
        COEDataViewBO masterSchema = Session["MasterSchema"] as COEDataViewBO;
        if (masterSchema != null)
        {
            tablesList = masterSchema.DataViewManager.Tables;
            SelectedDataView = masterSchema;
        }
        
        foreach (TableBO currentTable in tablesList)
        {
            if (currentTable.DataBase == this.SchemasDropDownList.SelectedValue)
            {
                if (this.SelectedDataView.ID < 0 || !currentTable.FromMasterSchema)
                {
                    ListItem currentItem = new ListItem();
                    currentItem.Text = currentTable.DataBase.ToUpper() + "." + Utilities.FormatTableText(currentTable.Name, currentTable.Alias);
                    currentItem.Value = currentTable.ID.ToString();
                    if (currentTable.ID == baseTableId)
                        currentItem.Selected = true;
                    this.RadioButtonList.Items.Add(currentItem);
                }
            }
        }
        #region Empty tables template
        if (this.RadioButtonList.Items.Count == 0)
        {
            //TODO: Display some text
        }
        #endregion
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public TableBO Unbind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int selectedTableid = int.MinValue;

        if (!int.TryParse(this.RadioButtonList.SelectedValue, out selectedTableid) && this.SelectedTableID > 0)
            selectedTableid = this.SelectedTableID;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        COEDataViewBO masterSchema = Session["MasterSchema"] as COEDataViewBO;
        if (masterSchema != null)
        {
            return masterSchema.DataViewManager.Tables.GetTable(selectedTableid);
        }
        else
        {
            return this.SelectedDataView.DataViewManager.Tables.GetTable(selectedTableid);
        }
    }

    public bool IsValidPrimaryKey()
    {
        if (!IsValidPrimaryUniqueKeyInfo())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsValidPrimaryUniqueKeyInfo()   //CBOE-767
    {
        try
        {
            int tableID = 0;
            if (!string.IsNullOrEmpty(RadioButtonList.SelectedValue) && Convert.ToInt32(RadioButtonList.SelectedValue) > 0)
            {
                tableID = Convert.ToInt32(RadioButtonList.SelectedValue);
            }

            COEDataViewBO theCOEDataViewBO = null;
            COEDataViewBO masterSchema = Session["MasterSchema"] as COEDataViewBO;
            if (masterSchema != null)
            {
                theCOEDataViewBO = masterSchema;
            }
            else
            {
                theCOEDataViewBO = this.SelectedDataView;
            }

            //COEDataViewBO theCOEDataViewBO = this.SelectedDataView;
            if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables.Count > 0 && tableID != -1)
            {
                TableBO theTableBO = theCOEDataViewBO.DataViewManager.Tables.GetTable(tableID);

                if (theTableBO != null)
                {
                    DataTable dtPkUkList = theCOEDataViewBO.DataViewManager.GetPrimaryUniqueKeyConstraintInfo(theTableBO.DataBase, theTableBO.Name);
                    //JiraID: CBOE-767 DVM Improvement
                    DataTable dtPkUkNotNullColsList = theCOEDataViewBO.DataViewManager.GetPrimaryKeyFieldNotNullCols(theTableBO.DataBase, theTableBO.Name);

                    if ((dtPkUkList != null && dtPkUkList.Rows.Count > 0) || (dtPkUkNotNullColsList != null && dtPkUkNotNullColsList.Rows.Count > 0))
                    {
                        if (dtPkUkList != null)
                            dtPkUkList.Dispose();
                        if (dtPkUkNotNullColsList != null)
                            dtPkUkNotNullColsList.Dispose();
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return false;
    }
    
    #endregion

}
