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
using System.Reflection;
using System.Linq;

using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace Manager.Forms.DataViewManager.UserControls
{
    public partial class TablesTreeView : System.Web.UI.UserControl
    {
        #region Properties
        /// <summary>
        /// Id of the base table
        /// </summary>
        private int BaseTableID
        {
            get
            {
                if (ViewState["BaseTableID"] == null)
                    ViewState["BaseTableID"] = int.MinValue;

                return (int)ViewState["BaseTableID"];
            }
            set
            {
                ViewState["BaseTableID"] = value;
            }
        }
       
        /// <summary>
        /// List of tables
        /// </summary>
        private TableListBO Tables
        {
            get
            {
                return Session[Constants.TableListBO] != null ? (TableListBO)Session[Constants.TableListBO] : null;
            }
            set
            {
                if (Session[Constants.TableListBO] != value && value != null)
                    Session[Constants.TableListBO] = value;
            }
        }

        /// <summary>
        /// Node table id
        /// </summary>
        public string SelectedTableID
        {
            get
            {
                if (ViewState["SelectedTable"] == null)
                    ViewState["SelectedTable"] = string.Empty;

                return (string)ViewState["SelectedTable"];
            }
            set
            {
                //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
                //check null value
                if (value != null)
                {
                    ViewState["SelectedTable"] = value;
                    if (this.Tables != null && this.Tables.Count > 0)   //check null value
                    {
                        TableBO tbl = this.Tables.GetTable(int.Parse(value));
                        if (tbl != null)
                            this.SchemaDropDownList.SelectedValue = tbl.DataBase;
                    }
                }
            }
        }        

        public string SelectedDatabase
        {
            get
            {
                 return this.SchemaDropDownList.SelectedValue; 
            }
        }

        public string SchemaDropDownListClientID
        {
            get { return this.SchemaDropDownList.ClientID; }
        }

        public string TableTextBoxClientID
        {
            get { return this.TableTextBox.ClientID; }
        }
        //CSBR-162275: Disabling Refresh/Remove Schema buttons when there are no published schemas in Edit Master
        public LinkButton RemoveSchemaLink
        {
            get { return this.RemoveSchema; }
        }

        public LinkButton AddTableLink
        {
            get { return this.AddTable; }
        }

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void AddTable_Click(object sender, EventArgs e)
        {
            string selectedSchema = SchemaDropDownList.SelectedValue;
            Response.Redirect("../ContentArea/AddTable.aspx?schemaSelected=" + selectedSchema);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Binds a datasource to the control
        /// </summary>
        /// <param name="dataViewsNodes">List of tables to display</param>
        /// <remarks>Here we have to convert the datasource to a List<DataViewNode></remarks>
        public void DataBind(TableListBO tables, int basetableId)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Tables = tables;
            this.BaseTableID = basetableId;
            this.SchemaDropDownList.DataSource = GetSchemas();
            this.SchemaDropDownList.DataBind();

            if (!Page.IsPostBack && !string.IsNullOrEmpty(Page.Request["schemaSelected"]))
            {
                string schema = Page.Request["schemaSelected"];
                this.SchemaDropDownList.SelectedValue = schema;
            }
            else
            {
                this.SchemaDropDownList.SelectedValue = GetSchemasFromBaseTableID(basetableId);
            }           

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to get schema name from base table id.
        /// </summary>
        /// <param name="basetableId">Base table id</param>
        /// <returns>Returns base schema name </returns>
        public string GetSchemasFromBaseTableID(int basetableId)
        {
            try
            {
                if (this.Tables.Count > 0 && basetableId != -1)
                {
                    foreach (TableBO table in this.Tables)
                    {
                        if (table.ID == basetableId)
                            return table.DataBase;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;
        }

        public void ReBindData(List<string> dbs)
        {
            try
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                if (dbs != null)
                {
                    this.SchemaDropDownList.DataSource = null;
                    this.SchemaDropDownList.DataBind();
                    this.SchemaDropDownList.DataSource = dbs;
                    this.SchemaDropDownList.DataBind();
                }
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a table
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="removeItem"></param>
        public string DeleteTable(int tableId)
        {
            int tableHadRelationship = 0;
            int tableHadLookup = 0;

            tableHadRelationship += IsTableInRelationships(tableId) ? 1 : 0;
            tableHadLookup += IsTableInLookup(tableId) ? 1 : 0;
            return this.BuildConfirmationMessage(tableHadRelationship, tableHadLookup);
        }

        /// <summary>
        /// Checks if a table belongs to a relationship
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        private bool IsTableInRelationships(int tableId)
        {
            COEDataViewBO dvBO = ((Manager.Forms.Master.DataViewManager)this.Page.Master).GetDataViewBO();
            return dvBO.DataViewManager.Relationships.GetByParentOrChildId(tableId).Count > 0;
        }

        /// <summary>
        /// Checks if a table belongs to a lookup
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        private bool IsTableInLookup(int tableId)
        {
            COEDataViewBO dvBO = ((Manager.Forms.Master.DataViewManager)this.Page.Master).GetDataViewBO();
            foreach (FieldBO field in dvBO.DataViewManager.Tables.GetTable(tableId).Fields)
            {
                if (field.LookupDisplayFieldId >= 0)
                    return true;

                List<TableBO> tables = dvBO.DataViewManager.Tables.GetTablesByLookup(field.ID);
                if (tables != null && tables.Count > 0)
                    return true;

            }
            return false;
        }

        /// <summary>
        /// Builds a confirmation message
        /// </summary>
        /// <param name="tablesWithRelationships"></param>
        /// <param name="tablesWithLookups"></param>
        /// <returns></returns>
        private string BuildConfirmationMessage(int tablesWithRelationships, int tablesWithLookups)
        {
            string confirmationMessage = string.Empty;
            if (tablesWithLookups > 1)
            {
                if (tablesWithRelationships > 0)
                    confirmationMessage = Resources.Resource.TablesRemovedHaveRelationshipsAndLookups_Label_Text;
                else
                    confirmationMessage = Resources.Resource.TablesRemovedHaveLookups_Label_Text;
            }
            else if (tablesWithLookups > 0)
            {
                if (tablesWithRelationships > 1)
                    confirmationMessage = Resources.Resource.TablesRemovedHaveRelationshipsAndLookups_Label_Text;
                else if (tablesWithRelationships > 0)
                    confirmationMessage = Resources.Resource.TableRemovedHasRelationshipsAndLookups_Label_Text;
                else
                    confirmationMessage = Resources.Resource.TableRemovedHasLookups_Label_Text;
            }
            else
            {
                if (tablesWithRelationships > 1)
                    confirmationMessage = Resources.Resource.TablesRemovedHaveRelationships_Label_Text;
                else if (tablesWithRelationships > 0)
                    confirmationMessage = Resources.Resource.TableRemovedHasRelationships_Label_Text;
            }
            return confirmationMessage;
        }

        /// <summary>
        /// Shows a confirmation message
        /// </summary>
        /// <param name="message"></param>
        private void ShowConfirmationMessage(string message)
        {
            Forms_Public_UserControls_ConfirmationArea confirmationArea = null;
            confirmationArea = (Forms_Public_UserControls_ConfirmationArea)((Manager.Forms.Master.DataViewManager)this.Page.Master).FindControlInPage("ConfirmationAreaUserControl");
            if (confirmationArea != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    confirmationArea.Visible = true;
                    confirmationArea.Text = message;
                }
                else
                    confirmationArea.Visible = false;
            }
        }

        public string GetTablesDataSource(string schema)
        {
            if (this.Tables == null)
                return string.Empty;


            this.BaseTableID = ((Master.DataViewManager)this.Page.Master).GetDataViewBO().DataViewManager.BaseTableId;
            if (this.Tables.Count > 0)
            {
                if (string.IsNullOrEmpty(schema))
                {
                    if (!string.IsNullOrEmpty(this.SelectedDatabase))
                        schema = this.SelectedDatabase;
                    else
                        schema = this.Tables[0].DataBase;
                }

                string result = "YAHOO.DataviewBoardNS.LeftPanel.DataSource.liveData = [";
                foreach (TableBO table in this.Tables)
                {
                    if (table.DataBase == schema)
                        result += "{" + string.Format("tableschema: \"{0}\", tablealias: \"{1}\", tableid: \"{2}\", tablename: \"{3}\", isbasetable: \"{4}\"", table.DataBase, System.Web.HttpUtility.HtmlEncode(table.Alias.Trim()), table.ID, table.Name, (table.ID == this.BaseTableID)) + "},";
                }
                if (result.Length > 0 && !result.EndsWith("["))
                    result = result.Remove(result.Length - 1);
                result += @"];
                ";

                this.SchemaDropDownList.SelectedValue = schema;

                return result;
            }
            return string.Empty;
        }

        private List<string> GetSchemas()
        {
            List<string> schemas = new List<string>();
            if (this.Tables.Count > 0)
            {
                foreach (TableBO table in this.Tables)
                {
                    if (!schemas.Contains(table.DataBase))
                        schemas.Add(table.DataBase);
                }
            }

            try
            {
                COEDataViewBO theCOEDataViewBO = COEDataViewBO.New();
                if (theCOEDataViewBO != null)
                {
                    List<string> lstBAViews = theCOEDataViewBO.GetELNSchemaName();
                    if (lstBAViews != null && lstBAViews.Count > 0)
                    {
                        foreach (string item in lstBAViews)
                        {
                            if (schemas.Contains(item))
                            {
                                List<TableBO> lstTableBO = this.Tables.GetTablesByDB(item);
                                foreach (TableBO theTableBO in lstTableBO)
                                {
                                    FieldBO theFieldBO = theTableBO.Fields.GetField(item, "ROW_ID");
                                    if (theTableBO.PrimaryKey <= 0 && theFieldBO != null)
                                    {
                                        this.Tables.GetTable(theTableBO.ID).PrimaryKey = theFieldBO.ID;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return schemas;
            }
            ////CBOE-1021:: To set primary key of ENBIOASSAYVIEWS table if it is 0
            //if (schemas.Contains(COEDataView.SchemaName.ENBIOASSAYVIEWS.ToString()))
            //{
            //    List<TableBO> lstTableBO = this.Tables.GetTablesByDB(COEDataView.SchemaName.ENBIOASSAYVIEWS.ToString());
            //    foreach (TableBO theTableBO in lstTableBO)
            //    {
            //        FieldBO theFieldBO = theTableBO.Fields.GetField(COEDataView.SchemaName.ENBIOASSAYVIEWS.ToString(), "ROW_ID");
            //        if (theTableBO.PrimaryKey <= 0 && theFieldBO != null)
            //        {
            //            this.Tables.GetTable(theTableBO.ID).PrimaryKey = theFieldBO.ID;
            //        }
            //    }
            //}
            return schemas;
        }
        #endregion
    }
}
