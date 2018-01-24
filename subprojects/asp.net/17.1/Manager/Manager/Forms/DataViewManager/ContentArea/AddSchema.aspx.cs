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
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common;

namespace Manager.Forms.DataViewManager.ContentArea
{
    public partial class AddSchema : GUIShellPage
    {
        #region Properties
        private COEDataViewManagerBO DataViewManager
        {
            get
            {
                //Coverity Fixes: CBOE-313 : CID-11765
                COEDataViewBO coeDataViewBO = this.GetDataViewBO();
                if (coeDataViewBO != null)
                    return coeDataViewBO.DataViewManager;
                return null;
            }
        }
        #endregion

        #region Variables
        List<string> _schemasInDataView = new List<string>();
        #endregion

        #region Page life cycle
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SubscribeToUCEvents();

            //Coverity Fixes: CBOE-313 :CID-11766
            COEDataViewBO coeDataViewBO = this.GetDataViewBO();
            if (coeDataViewBO != null)
                this.GetSchemasInDataView(coeDataViewBO);
            if (!Page.IsPostBack)
                this.AddSchemasUserControl.DataBind(this.GetSchemasToAddToDataView());
        }
        #endregion

        #region Event handlers
        void AddSchemaUC_SchemaCancelAdd(object sender, CommandEventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            //close current
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "cancel", "window.parent.CloseModal(false);", true);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void AddSchemasUserControl_ErrorOcurred(object sender, EventArgs e)
        {
            this.ErrorAreaUserControl.Text = this.AddSchemasUserControl.ErrorMessage;
            this.ErrorAreaUserControl.Visible = true;
        }

        void AddSchemaUC_SchemaAdded(object sender, CommandEventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string schema = string.Empty;
            if (sender is Forms_Public_UserControls_ImageButton)
            {
                schema = ((Forms_Public_UserControls_ImageButton)sender).CommandArgument;
                if (!string.IsNullOrEmpty(schema))
                    this.AddSelectedSchema(schema);
            }
            //close this and refresh parent
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "redirect", "window.parent.location.href = 'DataviewBoard.aspx?IsMaster=true&schemaSelected=" + schema + "';", true);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Private Methods
        public COEDataViewBO GetDataViewBO()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COEDataViewBO retVal = null;
            if (Session[Constants.COEDataViewBO] != null)
                retVal = ((COEDataViewBO)Session[Constants.COEDataViewBO]);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return retVal;
        }

        private void SubscribeToUCEvents()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (this.AddSchemasUserControl != null)
            {
                this.AddSchemasUserControl.SchemaAdded += new CommandEventHandler(AddSchemaUC_SchemaAdded);
                this.AddSchemasUserControl.Cancel += new CommandEventHandler(AddSchemaUC_SchemaCancelAdd);
                this.AddSchemasUserControl.ErrorOcurred += new EventHandler<EventArgs>(AddSchemasUserControl_ErrorOcurred);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        private COEDatabaseBOList GetSchemasToAddToDataView()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COEDatabaseBOList schemas = this.GetUnPublishedSchemas();
            foreach (COEDatabaseBO schema in this.GetPublishedToAdd())
                schemas.Add(schema);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return schemas;
        }

        private COEDatabaseBOList GetUnPublishedSchemas()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //Get all the schema in the Db instance.(Unpublish ones)
            COEDatabaseBOList unpublishedSchemas = COEDatabaseBOList.GetList(false);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return unpublishedSchemas;
        }

        private COEDatabaseBOList GetPublishedToAdd()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COEDatabaseBOList publishedSchemas = COEDatabaseBOList.GetList(true);
            foreach (string schema in _schemasInDataView)
                publishedSchemas.Remove(COEDatabaseBO.Get(schema)); //Do a directly remove
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return publishedSchemas;
        }

        private void GetSchemasInDataView(COEDataViewBO dataView)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            foreach (TableBO table in dataView.DataViewManager.Tables)
            {
                if (!_schemasInDataView.Contains(table.DataBase))
                    _schemasInDataView.Add(table.DataBase);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void AddSelectedSchema(string schemToAdd)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            //Coverity Fixes: CBOE-313
            COEDataViewManagerBO coeDataViewManagerBO = this.DataViewManager;
            if (coeDataViewManagerBO != null)
            {
                //Get DataView and Merge it to the current Master DataView.
                COEDatabaseBO databaseBO = COEDatabaseBO.Get(schemToAdd);
                int originalHighestID = coeDataViewManagerBO.Tables.HighestID;
                if (databaseBO != null)
                {
                    if (databaseBO.COEDataView != null)
                    {
                        foreach (COEDataView.DataViewTable table in databaseBO.COEDataView.Tables)
                        {
                            foreach (COEDataView.Field fld in table.Fields)
                            {
                                fld.Id += originalHighestID;
                                if (fld.LookupDisplayFieldId >= 0)
                                    fld.LookupDisplayFieldId += originalHighestID;
                                if (fld.LookupFieldId >= 0)
                                    fld.LookupFieldId += originalHighestID;
                                fld.ParentTableId += originalHighestID;
                            }
                            table.Id += originalHighestID;

                            int tblpk;
                            if (!string.IsNullOrEmpty(table.PrimaryKey) && int.TryParse(table.PrimaryKey, out tblpk))
                                table.PrimaryKey = (tblpk + originalHighestID).ToString();

                            TableBO newTable = TableBO.NewTable(table);
                            int i = 0;


                            while (coeDataViewManagerBO.Tables.Contains(newTable))
                            {
                                newTable.Alias = newTable.Name + "_" + (++i).ToString();
                            }
                            coeDataViewManagerBO.Tables.Add(newTable);

                        }
                        foreach (COEDataView.Relationship rel in databaseBO.COEDataView.Relationships)
                        {
                            rel.Child += originalHighestID;
                            rel.ChildKey += originalHighestID;
                            rel.Parent += originalHighestID;
                            rel.ParentKey += originalHighestID;
                            RelationshipBO relBO = RelationshipBO.NewRelationship(rel);
                            coeDataViewManagerBO.Relationships.Add(relBO);
                        }
                        if (databaseBO.COEDataView.Basetable >= 0)
                        {
                            databaseBO.COEDataView.Basetable += originalHighestID;
                            coeDataViewManagerBO.SetBaseTable(databaseBO.COEDataView.Basetable);
                        }
                    }
                }
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
    }
}