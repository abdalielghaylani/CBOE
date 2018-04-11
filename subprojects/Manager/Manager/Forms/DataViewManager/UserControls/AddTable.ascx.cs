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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using System.Text;

namespace Manager.Forms.DataViewManager.UserControls
{
    public partial class AddTable : System.Web.UI.UserControl, ICallbackEventHandler
    {
        #region Properties
        private DataTable SourceDVTables
        {
            get { return Session["SourceDVTables"] as DataTable; }
            set { Session["SourceDVTables"] = value; }
        }

        private COEDataViewBO CurrentDataview
        {
            get { return ((Master.DataViewManager)this.Page.Master).GetDataViewBO(); }
        }

        private COEDataViewBO SourceDataview
        {
            get { return Session["SourceDataview"] as COEDataViewBO; }
            set { Session["SourceDataview"] = value; }
        }

        private bool IsMasterDV
        {
            get { return this.CurrentDataview.ID == Constants.MasterSchemaDataViewID; }
        }

        private List<TableBO> PendingTables
        {
            get
            {
                if (Session["PendingTables"] == null)
                    Session["PendingTables"] = new List<TableBO>();
                return Session["PendingTables"] as List<TableBO>;
            }
            set { Session["PendingTables"] = value; }
        }

        private string LastAddedTable
        {
            get
            {
                TableBO tbl = this.PendingTables[PendingTables.Count - 1];
                return string.Format("{0}.{1}", tbl.DataBase, tbl.Alias);
            }
        }

        private string SchemaSelected
        {
            get
            {
                return ViewState["SchemaSelected"] as string;
            }
            set
            {
                ViewState["SchemaSelected"] = value;
            }
        }

        #endregion

        #region Page events
        protected override void OnInit(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            base.OnInit(e);
            this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
            this.OkImageButton.ButtonClicked += new EventHandler<EventArgs>(OkImageButton_ButtonClicked);
            if (!Page.IsPostBack && !string.IsNullOrEmpty(Page.Request["schemaSelected"]))
            {
                this.SchemaSelected = Page.Request["schemaSelected"];
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            ((GUIShellMaster)Page.Master).EnableYUI = true;
            ((GUIShellMaster)this.Page.Master).SetDefaultFocus(this.SchemaDropDownList.ClientID);
            this.SelectTableLabel.Text = Resources.Resource.TablesSelectedInDataView_Label_Text;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }        

        #endregion

        #region Event Handlers
        void CancelImageButton_ButtonClicked(object sender, EventArgs e)
        {
            COEDataViewBO dv = this.CurrentDataview;
            foreach (TableBO table in this.PendingTables)
            {
                dv.DataViewManager.Tables.Remove(table);
                foreach (RelationshipBO rel in dv.DataViewManager.Relationships.GetByChildId(table.ID))
                    dv.DataViewManager.Relationships.Remove(rel);
            }

            this.CleanSessionVariables();
            Server.Transfer(string.Format("DataviewBoard.aspx{0}", string.IsNullOrEmpty(this.SchemaSelected) ? string.Empty : "?schemaSelected=" + this.SchemaSelected));
        }

        void OkImageButton_ButtonClicked(object sender, EventArgs e)
        {
            this.CleanSessionVariables();
            Server.Transfer(string.Format("DataviewBoard.aspx{0}", string.IsNullOrEmpty(this.SchemaSelected) ? string.Empty : "?schemaSelected=" + this.SchemaSelected));
        }
        #endregion

        #region Methods

        private void CleanSessionVariables()
        {
            this.PendingTables = null;
            this.SourceDVTables = null;
            this.SourceDataview = null;
        }

        private List<string> GetSchemas()
        {            
            List<string> schemas = new List<string>();
            //Coverity Fixes: CBOE-313
            DataTable dataTable = this.SourceDVTables;
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (!schemas.Contains(row["TableDB"].ToString()))
                        schemas.Add(row["TableDB"].ToString());
                }
            }

            return schemas;
        }

        private void AddSelectedTable(string database, string table)
        {
            DataTable dataTable = this.SourceDVTables;
            //List of FieldBO, which is used to store the field,on which lookup is present.
            List<FieldBO> lstFields = null;
            //Coverity Fixes: CBOE-313 : CID-11771
            if (dataTable != null)
            {
                DataRow[] row = dataTable.Select("[TableAlias] = '" + table + "' AND " + "[TableDB] = '" + database + "'");
                if (row != null && row.Length > 0)
                {
                    COEDataViewBO sourceDV = this.SourceDataview;

                    TableBO masterTable = sourceDV.DataViewManager.Tables.GetTable(int.Parse(row[0]["TableID"].ToString())).Clone();
                    List<RelationshipBO> rels = sourceDV.DataViewManager.Relationships.GetByChildId(masterTable.ID);

                    COEDataViewBO dv = this.CurrentDataview;

                    string baseTableName = string.Empty;
                    string parentFieldName = string.Empty;
                    string childFieldName = string.Empty;
                    string parentAlias = string.Empty;
                    string childAlias = string.Empty;
                    COEDataView.JoinTypes joinType = COEDataView.JoinTypes.INNER;
                    if (!this.IsMasterDV)
                    {
                        baseTableName = dv.DataViewManager.Tables.GetTable(dv.DataViewManager.BaseTableId).Name;
                        foreach (RelationshipBO rel in rels)
                        {
                            TableBO parentTbl = sourceDV.DataViewManager.Tables.GetTable(rel.Parent);

                            if (parentTbl.Name == baseTableName && parentTbl.DataBase == dv.DataViewManager.DataBase)
                            {
                                //Build the relationship for the dataview, having the table being added the child of the basetable.
                                parentFieldName = parentTbl.Fields.GetField(rel.ParentKey).Name;
                                parentAlias = parentTbl.Fields.GetField(rel.ParentKey).Alias;
                                childFieldName = masterTable.Fields.GetField(rel.ChildKey).Name;
                                childAlias = masterTable.Fields.GetField(rel.ChildKey).Alias;
                                joinType = rel.JoinType;
                                masterTable.Fields.GetField(rel.ChildKey).IsDefault = true;
                                break;
                            }
                        }
                    }

                    dv.DataViewManager.Tables.RegenerateIds(masterTable);
                    TableBO newTable = TableBO.NewTable(masterTable); //cleans dictionaries and garbage.
                    int i = 0;
                    while (dv.DataViewManager.Tables.Contains(newTable))
                    {
                        newTable.Alias = newTable.Name + "_" + (++i).ToString();
                    }
                    dv.DataViewManager.Tables.Add(newTable);
                    if (!string.IsNullOrEmpty(parentFieldName) && !string.IsNullOrEmpty(childFieldName))
                    {
                        TableBO baseTable = dv.DataViewManager.Tables.GetTable(dv.DataViewManager.BaseTableId);
                        // FieldBO childField = newTable.Fields.GetField(newTable.DataBase, childFieldName);
                        //FieldBO parentField = baseTable.Fields.GetField(dv.DataViewManager.DataBase, parentFieldName);
                        FieldBO childField = newTable.Fields.GetField(newTable.DataBase, childFieldName, childAlias);
                        FieldBO parentField = baseTable.Fields.GetField(dv.DataViewManager.DataBase, parentFieldName, parentAlias);
                        RelationshipBO relationship = RelationshipBO.NewRelationship(baseTable.ID, parentField.ID, newTable.ID, childField.ID, joinType);
                        dv.DataViewManager.Relationships.Add(relationship);
                    }

                    //If table we are adding has Lookup details.
                    lstFields = new List<FieldBO>();
                    //Get the Master dataview to find the Lookup fields.
                    COEDataViewBO masterDataViewBO = COEDataViewBO.GetMasterSchema();
                    //Loop through the tables of current dataview  so find if it has lookup present.
                    foreach (TableBO tables in dv.DataViewManager.Tables)
                    {
                        //Collect lookup fields of table.
                        lstFields = tables.Fields.GetAllFieldsByLookup();
                        //Check if  field list not empty.
                        if (lstFields != null && lstFields.Count > 0)
                        {
                            //Need to loop through the list to collect the details from master dataview.
                            foreach (FieldBO field in lstFields)
                            {
                                //Check if lookup field is present.
                                TableBO tbl = masterDataViewBO.DataViewManager.Tables.GetTableByFieldId(field.LookupFieldId);
                                //check if table got from master dataview is same as new table added.
                                if (tbl != null && tbl.DataBase.Equals(newTable.DataBase) && tbl.Name.Equals(newTable.Name) && tbl.Alias.Equals(newTable.Alias))
                                {
                                    //find the lookup field.
                                    FieldBO fld = tbl.Fields.GetField(field.LookupFieldId);
                                    FieldBO newfld = null;
                                    if (fld != null)
                                    {
                                        //Get table from current dataview to add lookup id.
                                        newfld = newTable.Fields.GetField(fld.Database, fld.Name, fld.Alias);
                                        if (newfld != null)
                                            (tables.Fields.GetField(field.ID)).LookupFieldId = newfld.ID;
                                    }
                                    //find the display field.
                                    fld = tbl.Fields.GetField(field.LookupDisplayFieldId);
                                    if (fld != null)
                                    {
                                        //Get table from current dataview to add display id.
                                        newfld = newTable.Fields.GetField(fld.Database, fld.Name, fld.Alias);
                                        if (newfld != null)
                                            (tables.Fields.GetField(field.ID)).LookupDisplayFieldId = newfld.ID;
                                    }
                                }
                            }
                        }
                    }
                    //If table we are adding has lookUp Fields,collect those fields.
                    lstFields = newTable.Fields.GetAllFieldsByLookup();
                    if (lstFields != null && lstFields.Count > 0)
                    {
                        //Loop through the fields
                        foreach (FieldBO field in lstFields)
                        {
                            //check if table got from master dataview is same as new table added.
                            TableBO tbl = masterDataViewBO.DataViewManager.Tables.GetTableByFieldId(field.LookupFieldId);
                            //find the lookup field.
                            int FieldId = GetFieldID(tbl, dv, field.LookupFieldId);
                            if (FieldId != -1)
                                (newTable.Fields.GetField(field.ID)).LookupFieldId = FieldId;
                            //find the display field.
                            FieldId = GetFieldID(tbl, dv, field.LookupDisplayFieldId);
                            if (FieldId != -1)
                                (newTable.Fields.GetField(field.ID)).LookupDisplayFieldId = FieldId;
                        }
                    }
                    this.PendingTables.Add(newTable);
                }
            }
        }
        
        /// <summary>
        /// Get GetFieldID method return Id of the field.
        /// </summary>
        /// <param name="tbl">Table object</param>
        /// <param name="dv">Current DataviewBO Object.</param>
        /// <param name="id">Id of the field</param>
        /// <returns>return Id of the Field</returns>
        private int GetFieldID(TableBO tbl, COEDataViewBO dv, int id)
        {
            int result = -1;
            try
            {
                if (dv != null && tbl != null)
                {
                    FieldBO Fld = tbl.Fields.GetField(id);
                    foreach (TableBO table in dv.DataViewManager.Tables)
                    {
                        if (table != null && Fld != null && table.DataBase.Equals(tbl.DataBase) && table.Name.Equals(tbl.Name) && table.Alias.Equals(tbl.Alias))
                        {
                            FieldBO newfield = table.Fields.GetField(Fld.Database, Fld.Name, Fld.Alias);
                            result = newfield.ID;
                            break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        private string RemoveSelectedTable(string database, string alias)
        {
            bool succeed = false;
            TableBO tblBO = null;
            foreach (TableBO tbl in this.PendingTables)
            {
                if (tbl.DataBase == database && tbl.Alias == alias)
                {
                    tblBO = tbl;
                    COEDataViewManagerBO dvManager = this.CurrentDataview.DataViewManager;
                    dvManager.Relationships.Remove(new List<int>(new int[] { tbl.ID }));
                    dvManager.Tables.Remove(tbl);
                    succeed = true;
                    break;
                }
            }
            if (succeed)
                this.PendingTables.Remove(tblBO);
            return succeed.ToString();
        }

        internal void Bind()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COEDataViewBO dv = this.CurrentDataview;
            this.InitializeSourceData(string.Empty);
            this.SchemaDropDownList.DataSource = this.GetSchemas();
            this.SchemaDropDownList.DataBind();
            this.SchemaDropDownList.SelectedValue = string.IsNullOrEmpty(this.SchemaSelected) ? dv.DataViewManager.DataBase : this.SchemaSelected;
                        
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }       

        public string GetMasterTablesDataSource(string schema)
        {
            InitializeSourceData(schema);
            //Coverity Fixes: CBOE-313
            DataTable dataTable = this.SourceDVTables;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                string result = "AddTableNS.dataSource.liveData = [";
                foreach (DataRow table in dataTable.Rows)
                {
                    string dbName = dataTable.Columns.Contains("TableDB") ? table["TableDB"].ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(dbName) && dbName == schema)
                    {
                        string tableAlias = dataTable.Columns.Contains("TableAlias") ? table["TableAlias"].ToString() : string.Empty;

                        if (IsTableContains(table["TableName"].ToString(), tableAlias, dbName))
                            result += "{" + string.Format("tableschema: \"{0}\", tablealias: \"{1}\", addbutton: \"{2}\",IsAdded: \"{3}\"", dbName, tableAlias, Resources.Resource.Add_Button_Text, true) + "},";
                        else
                            result += "{" + string.Format("tableschema: \"{0}\", tablealias: \"{1}\", addbutton: \"{2}\",IsAdded: \"{3}\"", dbName, tableAlias, Resources.Resource.Add_Button_Text, false) + "},";
                    }
                }
                if (result.Length > 0)
                    result = result.Remove(result.Length - 1);
                result += @"];
                ";

                return result;
            }
            else
                return string.Empty;
        }

        //Jira Bug CBOE -240

        /// <summary>
        /// Methods checks wheather the table is present in currentdataview. If present returns true else returns false.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="alias"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private bool IsTableContains(string tableName, string alias, string dbName)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    foreach (TableBO item in this.CurrentDataview.DataViewManager.Tables)
                    {
                        if (item.Name.Equals(tableName) && item.Alias.Equals(alias) && item.DataBase.Equals(dbName))
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        public string GetPendingTablesDataSource()
        {
            if (this.PendingTables != null && this.PendingTables.Count > 0)
            {
                string result = "AddTableNS.pendingTablesDataSource.liveData = [";
                foreach (TableBO table in this.PendingTables)
                {
                    result += "{" + string.Format("table: \"{0}\", removebutton: \"{1}\"", table.DataBase + "." + table.Alias, Resources.Resource.Remove_Label_Text) + "},";
                }
                if (result.Length > 0)
                    result = result.Remove(result.Length - 1);
                result += @"];
                ";
                return result;
            }
            return string.Empty;
        }

        private void InitializeSourceData(string schema)
        {
            if (this.IsMasterDV)
            {
                if (string.IsNullOrEmpty(schema))
                {
                    if (this.CurrentDataview.DataViewManager.Tables.Count > 0)
                        schema = this.CurrentDataview.DataViewManager.Tables[0].DataBase;
                    else
                    {
                        COEDatabaseBOList list = COEDatabaseBOList.GetList(true);
                        if (list != null && list.Count > 0)
                        {
                            schema = list[0].Name;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(schema))
                {
                    COEDataView dv = COEDatabaseBO.Get(schema).COEDataView;
                    this.SourceDataview = COEDataViewBO.New(dv.Name, dv.Description, dv, null);
                    this.SourceDVTables = COEDataViewAsDataSet.GetPublishedTables().Tables[0];
                }
            }
            else
            {
                this.SourceDataview = COEDataViewBO.GetMasterSchema();
                this.SourceDVTables = COEDataViewAsDataSet.GetMasterTables().Tables[0];
            }
        }
        #endregion

        #region ICallbackEventHandler Members
        string result = string.Empty;
        string ICallbackEventHandler.GetCallbackResult()
        {
            return result;
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("AddTable: "))
            {
                string[] dbAndTable = eventArgument.Replace("AddTable: ", string.Empty).Split('.');

                this.AddSelectedTable(dbAndTable[0], dbAndTable[1]);
                this.result = "AddTable: " + this.LastAddedTable;
            }
            else if (eventArgument.StartsWith("RemoveTable: "))
            {
                string[] args = eventArgument.Replace("RemoveTable: ", string.Empty).Split('.');
                this.result = "RemoveTable: " + this.RemoveSelectedTable(args[0], args[1]);
            }
            else if (eventArgument.StartsWith("FilterSchema: "))
            {
                result = "FilterSchema: " + this.GetMasterTablesDataSource(eventArgument.Replace("FilterSchema: ", string.Empty)); ;
            }
        }       

        #endregion
    }
}
