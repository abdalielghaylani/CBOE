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
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using Resources;

public partial class SchemaSummary : System.Web.UI.UserControl
{
    #region Properties
    /// <summary>
    /// Current COEDataViewBO
    /// </summary>
    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO)Session[Constants.COEDataViewBO] : null;
        }
    }

    public string DatabaseName
    {
        get
        {
            if (Session["DatabaseName"] == null)
                Session["DatabaseName"] = string.Empty;

            return (string)Session["DatabaseName"];
        }
        set
        {
            Session["DatabaseName"] = value;
        }
    }

    public string SchemaNameTextBoxClientID
    {
        get { return this.SchemaNameTextBox.ClientID; }
    }
    #endregion

    #region Page Life Cycle Events
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            if (!string.IsNullOrEmpty(Page.Request["schemaSelected"]))
                this.BindSchemaDetails(Page.Request["schemaSelected"]);
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set all the controls values.
    /// </summary>
    private void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.NameTitleLabel.Text = Resource.Name_Label_Text;
        this.SchemaDetailTitle.Text = Resource.SchemaSummary_Label_Text;
        this.PrimaryKeysLabel.Text = Resource.PrimaryKey_Label_Text;
        this.LookupsLabel.Text = Resource.Lookups_Label_Text;
        this.RelationshipsLabel.Text = Resource.Relationships_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public string GetPkDataSource(string database)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (this.DataViewBO.DataViewManager.Tables.Count > 0)
        {
            if (string.IsNullOrEmpty(database))
            {
                database = this.DatabaseName;
            }
            string result = "YAHOO.DataviewBoardNS.SchemaSummary.PkDataSource.liveData = [";
            foreach (TableBO currentTable in this.DataViewBO.DataViewManager.Tables)
            {
                if (currentTable.DataBase == database)
                {
                    FieldBO pkField = currentTable.Fields.GetField(currentTable.PrimaryKey);

                    if (pkField != null)
                    {
                        result += "{" + string.Format("fieldid: \"{0}\", tablealias: \"{1}\", fieldalias: \"{2}\", fieldtype: \"{3}\"", pkField.ID, System.Web.HttpUtility.HtmlEncode(currentTable.Alias), pkField.Alias, pkField.DataType.ToString()) + "},";
                    }
                }
            }
            
            if (result.Length > 0 && !result.EndsWith("["))
                result = result.Remove(result.Length - 1);
            result += @"];
                ";
            return result;
        }
        return string.Empty;
    }

    public string GetRelDataSource(string database)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (this.DataViewBO.DataViewManager.Tables.Count > 0)
        {
            if (string.IsNullOrEmpty(database))
            {
                database = this.DatabaseName;
            }
            string result = "YAHOO.DataviewBoardNS.SchemaSummary.RelDataSource.liveData = [";
            foreach (TableBO currentTable in this.DataViewBO.DataViewManager.Tables)
            {
                if (currentTable.DataBase == database)
                {
                    foreach (RelationshipBO relationship in this.DataViewBO.DataViewManager.Relationships.GetByChildId(currentTable.ID))
                    {
                        TableBO childTbl = this.DataViewBO.DataViewManager.Tables.GetTable(relationship.Child);
                        TableBO parentTbl = this.DataViewBO.DataViewManager.Tables.GetTable(relationship.Parent);
                        if (!relationship.FromMasterSchema && childTbl != null && !childTbl.FromMasterSchema && parentTbl != null && !parentTbl.FromMasterSchema && relationship.ParentKey >= 0 && relationship.ChildKey >= 0)
                        {
                            string parenttable = string.Format("{0} ({1})", System.Web.HttpUtility.HtmlEncode(parentTbl.Alias), parentTbl.Fields.GetField(relationship.ParentKey).Alias);
                            string childtable = string.Format("{0} ({1})", System.Web.HttpUtility.HtmlEncode(childTbl.Alias), childTbl.Fields.GetField(relationship.ChildKey).Alias);
                            string reltype = relationship.JoinType.ToString();
                            result += "{" + string.Format("parenttable: \"{0}\", childtable: \"{1}\", reltype: \"{2}\"", parenttable, childtable, reltype) + "},";
                        }
                    }
                }
            }

            if (result.Length > 0 && !result.EndsWith("["))
                result = result.Remove(result.Length - 1);
            result += @"];
                ";
            return result;
        }
        return string.Empty;
    }

    public string GetLookupDataSource(string database)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (this.DataViewBO.DataViewManager.Tables.Count > 0)
        {
            if (string.IsNullOrEmpty(database))
            {
                database = this.DatabaseName;
            }
            string result = "YAHOO.DataviewBoardNS.SchemaSummary.LookupDataSource.liveData = [";
            foreach (TableBO currentTable in this.DataViewBO.DataViewManager.Tables)
            {

                if (currentTable.DataBase == database)
                {
                    foreach (FieldBO field in currentTable.Fields.GetAllFieldsByLookup())
                    {
                        if (field.LookupFieldId != -1 && field.LookupFieldId != int.MinValue) //Fixed 162170 To add a valid lookup to result, A valid lookup will never have a negative value 
                        {
                            result += "{" + string.Format("table: \"{0}\", lookup: \"{1}\", type: \"{2}\"", currentTable.Alias, field.Alias, field.DataType.ToString()) + "},";
                        }
                    }
                }
            }

            if (result.Length > 0 && !result.EndsWith("["))
                result = result.Remove(result.Length - 1);
            result += @"];
                ";
            return result;
        }
        return string.Empty;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Binds the details table with the given datasource.
    /// </summary>
    /// <param name="dataKey">Datakey of the selected table to display</param>
    public void BindSchemaDetails(string databaseName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.SchemaNameTextBox.Text = this.DatabaseName = databaseName;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion
}
