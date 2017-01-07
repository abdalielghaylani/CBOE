using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;

namespace Manager.Forms.DataViewManager.ContentArea
{
    public partial class AddSchema : GUIShellPage
    {
        #region Properties
        private COEDataViewManagerBO DataViewManager
        {
            get
            {
                // Coverity Fixes: CBOE-313 : CID-11765
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

            // CLear last error message.
            this.ErrorAreaUserControl.Text = string.Empty;
            this.ErrorAreaUserControl.Visible = false;
            
            //Coverity Fixes: CBOE-313 :CID-11766
            COEDataViewBO coeDataViewBO = this.GetDataViewBO();
            if (coeDataViewBO != null)
                this.GetSchemasInDataView(coeDataViewBO);
            if (!Page.IsPostBack)
            {
                this.AddSchemasUserControl.SetInstanceSchemas(this.GetInstanceToAddToDataView());
                this.AddSchemasUserControl.BindDropDownList();
            }
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

        void AddSchemaUC_InstanceSchemaAdded(object sender, CommandEventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string instanceSchema = string.Empty;
            if (sender is Forms_Public_UserControls_ImageButton)
            {
                instanceSchema = ((Forms_Public_UserControls_ImageButton)sender).CommandArgument;

                if (!string.IsNullOrEmpty(instanceSchema) && instanceSchema.Contains("."))
                {
                    // InstanceSchema format is "InstanceName.SchemaName"
                    var instanceName = instanceSchema.Split(new char[] { '.' })[0].Trim();
                    var schemaName = instanceSchema.Split(new char[] { '.' })[1].Trim();

                    if (!string.IsNullOrEmpty(instanceName) && !string.IsNullOrEmpty(schemaName))
                    {
                        this.AddSelectedSchema(instanceName, schemaName);
                    }
                }
            }
            //close this and refresh parent
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "redirect", "window.parent.location.href = 'DataviewBoard.aspx?IsMaster=true&schemaSelected=" + instanceSchema + "';", true);
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
                this.AddSchemasUserControl.InstanceSchemaAdded += new CommandEventHandler(AddSchemaUC_InstanceSchemaAdded);
                this.AddSchemasUserControl.Cancel += new CommandEventHandler(AddSchemaUC_SchemaCancelAdd);
                this.AddSchemasUserControl.ErrorOcurred += new EventHandler<EventArgs>(AddSchemasUserControl_ErrorOcurred);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private IList<string> GetInstanceToAddToDataView()
        {
            var instancesList = COEConfiguration.GetAllInstancesInConfig();
           
            return instancesList.Select(i=>i.Name).ToList();
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
            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            COEDatabaseBOList unpublishedSchemas = COEDatabaseBOList.GetList(false, mainInstance.Name);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return unpublishedSchemas;
        }

        private COEDatabaseBOList GetPublishedToAdd()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            COEDatabaseBOList publishedSchemas = COEDatabaseBOList.GetList(true, mainInstance.Name);
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

        private void AddSelectedSchema(string instanceName, string schemaName)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            // Coverity Fixes: CBOE-313
            COEDataViewManagerBO coeDataViewManagerBO = this.DataViewManager;
            if (coeDataViewManagerBO != null)
            {
                // Get DataView and Merge it to the current Master DataView.
                var qualifiedSchemaName = Utilities.GetQualifyInstaceSchemaName(instanceName, schemaName);

                //COEDatabaseBO databaseBO = COEDatabaseBO.Get(qualifiedSchemaName.ToUpper());
                var schemaOnPublishing = Session[Constants.COESchemasOnPublishing] as Dictionary<string, COEDatabaseBO>;

                COEDatabaseBO databaseBO = null;
                if (schemaOnPublishing != null && schemaOnPublishing.ContainsKey(qualifiedSchemaName.ToUpper()))
                {
                    var tempBO = schemaOnPublishing[qualifiedSchemaName.ToUpper()] as COEDatabaseBO;
                    databaseBO = tempBO.Clone();
                }
                else
                {
                    throw new Exception("Cannot find the databases on schema");
                }

                int originalHighestID = coeDataViewManagerBO.Tables.HighestID;

                if (databaseBO != null && databaseBO.COEDataView != null)
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
