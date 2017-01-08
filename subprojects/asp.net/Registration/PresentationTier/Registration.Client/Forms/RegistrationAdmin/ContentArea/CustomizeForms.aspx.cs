using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Resources;

using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.GUIShell;

using CambridgeSoft.COE.Registration.Services.Types;

using CambridgeSoft.COE.RegistrationAdmin;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.RegistrationAdmin.Services.Common;

namespace PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.ContentArea
{
    public partial class CustomizeForms : GUIShellPage
    {
        #region Variables

        RegistrationMaster _masterPage = null;

        private const int MIXTURESUBFORMINDEX = 1000;
        private const int COMPOUNDSUBFORMINDEX = 1001;
        private const int STRUCTURESUBFORMINDEX = 1;
        private const int BATCHSUBFORMINDEX = 1002;
        private const int BATCHCOMPONENTSUBFORMINDEX = 1003;

        private const int TEMPORARYBASEFORM = 0;
        private const int TEMPORARYCHILDFORM = 1;

        private const int MIXTURESEARCHFORM = 0;
        private const int COMPOUNDSEARCHFORM = 1;
        private const int BATCHSEARCHFORM = 2;
        private const int BATCHCOMPONENTSEARCHFORM = 3;
        private const int STRUCTURESEARCHFORM = 4;
        private List<string> _requiredFields = new List<string>();
        private List<string> BatchFormIngoreFields = new List<string>(){"FORMULA_WEIGHT", "BATCH_FORMULA", "PERCENT_ACTIVE"}; //Avoiding the batch properties which should not affected by Customize form action
        private DataTable _table;
        #endregion

        #region Properties

        private ConfigurationRegistryRecord ConfigurationRegistryRecord
        {
            get
            {
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

        private DataTable Table
        {
            get
            {
                if (_table == null)
                    _table = new DataTable();
                return _table;
            }
            set
            {
                _table = value;
            }
        }

        #endregion

        #region Overriden Life Cycle Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetDataListsStyles();
                    this.FillGrids();
                }
                this._masterPage.SetDefaultAction(this.SaveButton.UniqueID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveAndContinue();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        void DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) && e.Item.DataItem != null)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int tableId = int.Parse(row.DataView.Table.TableName);
                string propertyName = row["name"].ToString();
                string propertyType = string.Empty;
                string friendlyName = string.Empty;
                string propertySubType = string.Empty;
                switch (tableId)
                {
                    case COEFormHelper.MIXTURESUBFORMINDEX:
                        foreach (Property prop in ConfigurationRegistryRecord.PropertyList)
                        {
                            if (prop.Name == propertyName)
                            {
                                propertyType = prop.Type;
                                friendlyName = prop.FriendlyName;
                                if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                            }
                        }
                        break;

                    case COEFormHelper.COMPOUNDSUBFORMINDEX:

                        foreach (Property prop in ConfigurationRegistryRecord.CompoundPropertyList)
                        {
                            if (prop.Name == propertyName)
                            {
                                propertyType = prop.Type;
                                friendlyName = prop.FriendlyName;
                                if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                            }
                        }
                        break;

                    case COEFormHelper.STRUCTURESUBFORMINDEX:

                        foreach (Property prop in ConfigurationRegistryRecord.StructurePropertyList)
                        {
                            if (prop.Name == propertyName)
                            {
                                propertyType = prop.Type;
                                friendlyName = prop.FriendlyName;
                                if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                            }
                        }
                        break;

                    case COEFormHelper.BATCHSUBFORMINDEX:

                        foreach (Property prop in ConfigurationRegistryRecord.BatchPropertyList)
                        {
                            if (prop.Name == propertyName)
                            {
                                propertyType = prop.Type;
                                friendlyName = prop.FriendlyName;
                                if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                            }
                        }
                        break;

                    case COEFormHelper.BATCHCOMPONENTSUBFORMINDEX:

                        foreach (Property prop in ConfigurationRegistryRecord.BatchComponentList)
                        {
                            if (prop.Name == propertyName)
                            {
                                propertyType = prop.Type;
                                friendlyName = prop.FriendlyName;
                                if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                            }
                        }
                        break;
                }


                switch (propertyType)
                {
                    case "NUMBER":
                    case "TEXT":
                        if (!string.IsNullOrEmpty(propertySubType))
                        {
                            if (propertySubType == "URL")
                            {
                                ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("URL", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink"));
                                ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Enabled = false;
                            }
                        }
                        else
                        {
                            if(propertyType == "NUMBER")
                                ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("NumericTextBox", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox"));
                            ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("TextBox", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox"));
                            ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("TextArea", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea"));
                        } break;
                    case "BOOLEAN":
                        ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("CheckBox", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBox"));
                        break;
                    case "DATE":
                        ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("DatePicker", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker"));
                        break;
                    case "PICKLISTDOMAIN":
                        ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Items.Add(new ListItem("unmodifiable", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList"));
                        ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).Enabled = false;
                        break;
                }

                ((DropDownList)e.Item.FindControl("CSSClassDropDown")).DataSource = RegAdminUtils.GetCustomPropertyStyles();
                ((DropDownList)e.Item.FindControl("CSSClassDropDown")).DataBind();
                ((DropDownList)e.Item.FindControl("CSSClassDropDown")).SelectedValue = row["cssclass"].ToString().Trim();                
                ((DropDownList)e.Item.FindControl("ControlTypeDropDownList")).SelectedValue = row["controltype"].ToString();
                ((Label)e.Item.FindControl("NameLabel")).Text = friendlyName;
                ((TextBox)e.Item.FindControl("LabelTextBox")).Text = row["label"].ToString();

                if (row["visible"] != null && (row["visible"].ToString() == "true" || row["visible"].ToString() == string.Empty))
                    ((CheckBox)e.Item.FindControl("VisibleCheckBox")).Checked = true;
                else
                    ((CheckBox)e.Item.FindControl("VisibleCheckBox")).Checked = false;
            }

        }

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

        #endregion

        protected enum COEFormType
        {
            ListForm,
            QueryForm,
            DetailForm,            
        }

        protected enum DisplayInfo
        {
            AddMode,
            EditMode,
            ViewMode,
            LayoutMode
        }

        #region Business Methods

        protected override void SetControlsAttributtes()
        {
           this.Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.CustomizeForms_Page_Title;
        }

        private void SetDataListsStyles()
        {
            this.MixturePropertiesDataList.RepeatColumns = 1;
            this.MixturePropertiesDataList.RepeatDirection = RepeatDirection.Vertical;
            this.MixturePropertiesDataList.RepeatLayout = RepeatLayout.Table;
            this.MixturePropertiesDataList.ItemDataBound += new DataListItemEventHandler(DataList_ItemDataBound);

            this.CompoundPropertiesDataList.RepeatColumns = 1;
            this.CompoundPropertiesDataList.RepeatDirection = RepeatDirection.Vertical;
            this.CompoundPropertiesDataList.RepeatLayout = RepeatLayout.Table;
            this.CompoundPropertiesDataList.ItemDataBound += new DataListItemEventHandler(DataList_ItemDataBound);

            this.StructurePropertiesDataList.RepeatColumns = 1;
            this.StructurePropertiesDataList.RepeatDirection = RepeatDirection.Vertical;
            this.StructurePropertiesDataList.RepeatLayout = RepeatLayout.Table;
            this.StructurePropertiesDataList.ItemDataBound += new DataListItemEventHandler(DataList_ItemDataBound);

            this.BatchPropertiesDataList.RepeatColumns = 1;
            this.BatchPropertiesDataList.RepeatDirection = RepeatDirection.Vertical;
            this.BatchPropertiesDataList.RepeatLayout = RepeatLayout.Table;
            this.BatchPropertiesDataList.ItemDataBound += new DataListItemEventHandler(DataList_ItemDataBound);

            this.BatchComponentPropertiesDataList.RepeatColumns = 1;
            this.BatchComponentPropertiesDataList.RepeatDirection = RepeatDirection.Vertical;
            this.BatchComponentPropertiesDataList.RepeatLayout = RepeatLayout.Table;
            this.BatchComponentPropertiesDataList.ItemDataBound += new DataListItemEventHandler(DataList_ItemDataBound);
        }

        private void FillGrids()
        {
            COEFormBO formBO = COEFormBO.Get(4010);
            this.MixturePropertiesDataList.DataSource = GetData(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, MIXTURESUBFORMINDEX), false);
            this.MixturePropertiesDataList.DataBind();
            this.CompoundPropertiesDataList.DataSource = GetData(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, COMPOUNDSUBFORMINDEX), false);
            this.CompoundPropertiesDataList.DataBind();
            this.StructurePropertiesDataList.DataSource = GetData(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, STRUCTURESUBFORMINDEX), false);
            this.StructurePropertiesDataList.DataBind();
            this.BatchPropertiesDataList.DataSource = GetData(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, BATCHSUBFORMINDEX), false);
            this.BatchPropertiesDataList.DataBind();
            this.BatchComponentPropertiesDataList.DataSource = GetData(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, BATCHCOMPONENTSUBFORMINDEX), true);
            this.BatchComponentPropertiesDataList.DataBind();
        }

        private DataTable GetData(FormGroup.Form form, bool isGridForm)
        {
            Table = new DataTable();
            Table.Columns.Add("name", typeof(string));
            Table.Columns.Add("controltype", typeof(string));
            Table.Columns.Add("label", typeof(string));
            Table.Columns.Add("cssclass", typeof(string));
            Table.Columns.Add("visible", typeof(string));
            Table.PrimaryKey = new DataColumn[] { _table.Columns[0] };

            if (form.LayoutInfo.Count > 0)
                InitializeRows(isGridForm, DisplayInfo.LayoutMode, form);
            if (form.AddMode.Count > 0)
                InitializeRows(isGridForm, DisplayInfo.AddMode, form);
            if (form.EditMode.Count > 0)
                InitializeRows(isGridForm, DisplayInfo.EditMode, form);
            if (form.ViewMode.Count > 0)
                InitializeRows(isGridForm, DisplayInfo.ViewMode, form);

            Table.TableName = form.Id.ToString();
            return Table;
        }

        private void InitializeRows(bool isGridForm, DisplayInfo displayInfo, FormGroup.Form form)
        {
            string errorMsg;
            List<FormGroup.FormElement> formElement = form.AddMode;
            switch (displayInfo)
            {
                case DisplayInfo.AddMode:
                    formElement = form.AddMode;
                    break;
                case DisplayInfo.EditMode:
                    formElement = form.EditMode;
                    break;
                case DisplayInfo.ViewMode:
                    formElement = form.ViewMode;
                    break;
                case DisplayInfo.LayoutMode:
                    formElement = form.LayoutInfo;
                    break;
            } 
            switch (isGridForm)
            {
                case true :
                    COEGridView gridControl = (COEGridView)COEFormGenerator.GetCOEGenerableControl(formElement[0], out errorMsg);
                    XmlNode configInfo = gridControl.GetConfigInfo();
                    XmlNamespaceManager manager = new XmlNamespaceManager(configInfo.OwnerDocument.NameTable);
                    manager.AddNamespace("COE", configInfo.OwnerDocument.DocumentElement.NamespaceURI);
                    XmlNode tableNode = configInfo.SelectSingleNode("//COE:table", manager);
                    //Coverity Fix CID :13148
                    if (tableNode != null)
                    {
                        foreach (XmlNode colNode in tableNode.SelectNodes("./COE:Columns/COE:Column", manager))
                        {
                            FormGroup.FormElement
                                element = FormGroup.FormElement.GetFormElement(colNode.SelectSingleNode("./COE:formElement", manager).OuterXml);
                            if (!string.IsNullOrEmpty(element.Id) && !Table.Rows.Contains(element.Id.Replace("Property", string.Empty)) && CheckCustomPropertyName(element.Id, form.Id))
                            {
                                ICOEGenerableControl control = COEFormGenerator.GetCOEGenerableControl(element, out errorMsg);
                                DataRow row = Table.NewRow();
                                row["name"] = element.Id.Replace("Property", string.Empty);
                                row["controltype"] = element.DisplayInfo.Type;
                                row["cssclass"] = element.DisplayInfo.CSSClass;

                                if (colNode.SelectSingleNode("./COE:headerText", manager) != null)
                                    row["label"] = colNode.SelectSingleNode("./COE:headerText", manager).InnerText;

                                string visible = string.Empty;

                                if (colNode.SelectSingleNode("./COE:visible", manager) != null && !string.IsNullOrEmpty(colNode.SelectSingleNode("./COE:visible", manager).InnerText))
                                    visible = colNode.SelectSingleNode("./COE:visible", manager).InnerText;

                                if (colNode.Attributes["hidden"] != null && !string.IsNullOrEmpty(colNode.Attributes["hidden"].Value))
                                    visible = (colNode.Attributes["hidden"].Value.ToLower() == "true") ? "false" : "true";// Hidden will be always vice versa to visible..

                                row["visible"] = visible;
                                Table.Rows.Add(row);
                            }
                        }
                    }
                    break;
                case false:
                    foreach (FormGroup.FormElement element in formElement)
                    {
                        if (!string.IsNullOrEmpty(element.Id) && string.IsNullOrEmpty(element.DisplayInfo.Assembly) && !Table.Rows.Contains(element.Id.Replace("Property", string.Empty)) && CheckCustomPropertyName(element.Id, form.Id))
                        {
                            ICOEGenerableControl control = COEFormGenerator.GetCOEGenerableControl(element, out errorMsg);
                            DataRow row = Table.NewRow();
                            row["name"] = element.Id.Replace("Property", string.Empty);
                            row["controltype"] = element.DisplayInfo.Type;
                            row["label"] = element.Label;
                            row["cssclass"] = element.DisplayInfo.CSSClass;
                            row["visible"] = element.DisplayInfo.Visible.ToString().ToLower();
                            Table.Rows.Add(row);

                        }
                    }
                    break;
            }
        }
        
        private void SaveAndContinue()
        {
            try
            {
                CustomizeFormGroups();
                if (this._requiredFields.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (string field in _requiredFields)
                        builder.Append(field + ",");

                    builder.Remove(builder.Length - 1, 1);

                    ShowConfirmationMessage(Resource.FormElementsCustomize_SuccessMessage + string.Format(Resource.FormElementsCustomizeWarnings_Message, builder.ToString()));
                }
                else
                    ShowConfirmationMessage(Resource.FormElementsCustomize_SuccessMessage);
             }
            catch (Exception e)
            {
                _masterPage.DisplayErrorMessage(e.Message, false);
            }
        }

        private XmlNode CreateConfigInfoFromFieldConfig(XmlNode xmlNode)
        {
            XmlDocument xmlData = new XmlDocument();
            return CreateConfigInfoFromFieldConfig(xmlNode, xmlData, "COE", "COE.FormGroup");
        }

        private XmlNode CreateConfigInfoFromFieldConfig(XmlNode xmlNode, XmlDocument ownerDocument, string prefix, string namespaceuri)
        {
            XmlNode resultNode = ownerDocument.CreateNode(XmlNodeType.Element, prefix, "configInfo", namespaceuri);
            resultNode.AppendChild(ownerDocument.ImportNode(xmlNode, true));
            return resultNode;
        }

        private bool CheckCustomPropertyName(string propertyName, int formId)
        {
            switch (formId)
            {
                case COEFormHelper.MIXTURESUBFORMINDEX:

                    foreach (Property prop in ConfigurationRegistryRecord.PropertyList)
                    {
                        if (prop.Name + "Property" == propertyName)
                            return true;
                    }
                    break;

                case COEFormHelper.COMPOUNDSUBFORMINDEX:

                    foreach (Property prop in ConfigurationRegistryRecord.CompoundPropertyList)
                    {
                        if (prop.Name + "Property" == propertyName)
                            return true;
                    }
                    break;

                case COEFormHelper.STRUCTURESUBFORMINDEX:

                    foreach (Property prop in ConfigurationRegistryRecord.StructurePropertyList)
                    {
                        if (prop.Name + "Property" == propertyName)
                            return true;
                    }
                    break;

                case COEFormHelper.BATCHSUBFORMINDEX:

                    foreach (Property prop in ConfigurationRegistryRecord.BatchPropertyList)
                    {
                        if (prop.Name + "Property" == propertyName && (!BatchFormIngoreFields.Contains(prop.Name)))
                            return true;
                    }
                    break;

                case COEFormHelper.BATCHCOMPONENTSUBFORMINDEX:

                    foreach (Property prop in ConfigurationRegistryRecord.BatchComponentList)
                    {
                        if (prop.Name + "Property" == propertyName)
                            return true;
                    }
                    break;
            }
            return false;
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

        private void CustomizeFormGroups()
        {
            string[] customFromGroupsIds = RegAdminUtils.GetRegCustomFormGroupsIds();

            foreach (string formGroupId in customFromGroupsIds)
            {
                COEFormBO formBO = COEFormBO.Get(int.Parse(formGroupId));

                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, MIXTURESUBFORMINDEX), COEFormType.DetailForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, COMPOUNDSUBFORMINDEX), COEFormType.DetailForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, STRUCTURESUBFORMINDEX), COEFormType.DetailForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, BATCHSUBFORMINDEX), COEFormType.DetailForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.DetailsForms, 0, BATCHCOMPONENTSUBFORMINDEX), COEFormType.DetailForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, TEMPORARYBASEFORM), COEFormType.QueryForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, TEMPORARYCHILDFORM), COEFormType.QueryForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.ListForms, 0, MIXTURESEARCHFORM),COEFormType.ListForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, MIXTURESEARCHFORM), COEFormType.QueryForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, COMPOUNDSEARCHFORM), COEFormType.QueryForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, BATCHSEARCHFORM), COEFormType.QueryForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, BATCHCOMPONENTSEARCHFORM), COEFormType.QueryForm); }
                catch { }
                try { ApplyChangesToForm(formBO.GetForm(formBO.COEFormGroup.QueryForms, 0, STRUCTURESEARCHFORM), COEFormType.QueryForm); }
                catch { }

                formBO.Save();
            }
        }

        private void ApplyChangesToForm(FormGroup.Form form, COEFormType type)
        {
       
            foreach (DataListItem item in this.MixturePropertiesDataList.Items)
            {
                this.UpdateFormElementInToForm(ref form, item);
                this.UpdateFormElementInToSearchForm(ref form, item);
            }

            foreach (DataListItem item in this.CompoundPropertiesDataList.Items)
            {
                this.UpdateFormElementInToForm(ref form, item); 
                if(type.ToString() != COEFormType.ListForm.ToString())
                    this.UpdateFormElementInToSearchForm(ref form, item);   // Not to allow Compound properities to Search forms for Listview [GRID].

            }

            foreach (DataListItem item in this.StructurePropertiesDataList.Items)
            {
                this.UpdateFormElementInToForm(ref form, item);
                if (type.ToString() != COEFormType.ListForm.ToString())
                    this.UpdateFormElementInToSearchForm(ref form, item);  // // Not to allow Structure properities to Search forms for Listview [GRID].

            }

            foreach (DataListItem item in this.BatchPropertiesDataList.Items)
            {
                this.UpdateFormElementInToForm(ref form, item);
                this.UpdateFormElementInToSearchForm(ref form, item);
            }

            foreach (DataListItem item in this.BatchComponentPropertiesDataList.Items)
            {
                this.UpdateFormElementInToForm(ref form, item);
                this.UpdateFormElementInToSearchForm(ref form, item);
            }
        }
        public string SetLabelValidationExpression()
        {
            string expression = @"^[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\) ][a-zA-Z0-9\s.\-_,;:\?!\[\]\{\}\(\)]{0,29}[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\) ]$";
            return expression;
        }
        public string SetErrorMessage()
        {
            string errorMessage = "Invalid label text: Label can have a maximum of 30 characters and may not contain (~,@,#,$,%,^,&,*,','\',<,>,=,+)";
            return errorMessage;
        }
        private void SetFormElementProperties(FormGroup.FormElement formElement, FormGroup.DisplayMode displayMode, DataListItem item, ref ICOEGenerableControl control)
        {
            string errorMsg;
            bool enabled = true;
            string formElementLabel = ((TextBox)item.FindControl("LabelTextBox")).Text;
            string formElementType = ((DropDownList)item.FindControl("ControlTypeDropDownList")).SelectedValue;
            string formElementCssClass = ((DropDownList)item.FindControl("CSSClassDropDown")).SelectedValue;
            bool visible = ((CheckBox)item.FindControl("VisibleCheckBox")).Checked;
            string controlStyle = RegAdminUtils.GetDefaultControlStyle(formElementType, displayMode);
            string defaultTexMode = string.Empty;
            if (formElementType.Contains("COETextArea"))
                defaultTexMode = "MultiLine";
            formElement.Label = formElementLabel;
            formElement.DisplayInfo.Type = formElementType;
            if (!visible)
            {
                if (this.CheckIfRequired(formElement.Id))
                {
                    if (!_requiredFields.Contains(formElement.Name))
                        this._requiredFields.Add(formElement.Name);

                    ((CheckBox)item.FindControl("VisibleCheckBox")).Checked = true;
                }
                else
                    formElement.DisplayInfo.Visible = visible;
            }
            else
                formElement.DisplayInfo.Visible = visible;

            formElement.DisplayInfo.CSSClass = formElementCssClass;

            if (displayMode == FormGroup.DisplayMode.View)
            {
                if (!formElement.DisplayInfo.Type.Contains("DropDownList") && !formElement.DisplayInfo.Type.Contains("DatePicker"))
                {
                    if (!formElement.DisplayInfo.Type.Contains("NumericTextBox"))
                        formElement.DisplayInfo.Type += "ReadOnly";
                    else
                    {
                        if (formElement.ConfigInfo["COE:fieldConfig"] != null && formElement.ConfigInfo["COE:fieldConfig"]["COE:ReadOnly"] != null)
                            formElement.ConfigInfo["COE:fieldConfig"]["COE:ReadOnly"].InnerText = bool.TrueString;
                        else
                            formElement.ConfigInfo.FirstChild.AppendChild(formElement.ConfigInfo.OwnerDocument.CreateElement("COE:ReadOnly", formElement.ConfigInfo.NamespaceURI)).InnerText = bool.TrueString;
                    }
                }
                else
                    enabled = false;
            }
            if (formElementType=="CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink")
            {
                if (displayMode == FormGroup.DisplayMode.Add || displayMode == FormGroup.DisplayMode.Edit || formElement.BindingExpression.Contains("SearchCriteria")) 
                    formElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox";
                else
                    formElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink";
            }

                if (formElement.ConfigInfo["COE:fieldConfig"] != null && formElement.ConfigInfo["COE:fieldConfig"]["COE:CSSClass"] != null && !string.IsNullOrEmpty(controlStyle))
                    formElement.ConfigInfo["COE:fieldConfig"]["COE:CSSClass"].InnerText = controlStyle;
                if (formElement.ConfigInfo["COE:fieldConfig"] != null && formElement.ConfigInfo["COE:fieldConfig"]["COE:TextMode"] != null)
                    formElement.ConfigInfo["COE:fieldConfig"]["COE:TextMode"].InnerText = defaultTexMode;
            
            control = COEFormGenerator.GetCOEGenerableControl(formElement, out errorMsg);
            ((WebControl)control).Enabled = enabled;
        }

        private FormGroup.FormElement UpdateFormElement(FormGroup.FormElement formElement, FormGroup.DisplayMode displayMode, DataListItem item)
        {
            try
            {
                string errorMsg;
                ICOEGenerableControl control = COEFormGenerator.GetCOEGenerableControl(formElement, out errorMsg);
                if (string.IsNullOrEmpty(formElement.DisplayInfo.Assembly))
                    this.SetFormElementProperties(formElement, displayMode, item, ref control);
                //formElement.ConfigInfo = CreateConfigInfoFromFieldConfig(((ICOEDesignable)control).GetConfigInfo());
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
            return formElement;
        }
        private void UpdateFormElementInToForm(ref FormGroup.Form form, DataListItem item)
        {
            string formElementId = ((Label)item.FindControl("NameLabel")).Text + "Property";

            foreach (FormGroup.FormElement formElement in form.AddMode)
            {
                if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView")
                    formElement.ConfigInfo = CreateConfigInfoFromFieldConfig(this.UpdateGridView(formElement, FormGroup.DisplayMode.Add, item).GetConfigInfo());
                else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid")
                    formElement.ConfigInfo = this.UpdateWebGrid(formElement, FormGroup.DisplayMode.Add, item);
                else if (formElement.Id == formElementId)
                    formElement.ConfigInfo = UpdateFormElement(formElement, FormGroup.DisplayMode.Add, item).ConfigInfo;
            }

            foreach (FormGroup.FormElement formElement in form.EditMode)
            {
                if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView")
                    formElement.ConfigInfo = CreateConfigInfoFromFieldConfig(this.UpdateGridView(formElement, FormGroup.DisplayMode.Edit, item).GetConfigInfo());
                else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid")
                    formElement.ConfigInfo = this.UpdateWebGrid(formElement, FormGroup.DisplayMode.Edit, item);
                else if (formElement.Id == formElementId)
                    formElement.ConfigInfo = UpdateFormElement(formElement, FormGroup.DisplayMode.Edit, item).ConfigInfo;
            }

            foreach (FormGroup.FormElement formElement in form.ViewMode)
            {
                if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView")
                    formElement.ConfigInfo = CreateConfigInfoFromFieldConfig(this.UpdateGridView(formElement, FormGroup.DisplayMode.View, item).GetConfigInfo());
                else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid")
                    formElement.ConfigInfo = this.UpdateWebGrid(formElement, FormGroup.DisplayMode.View, item);
                else if (formElement.Id == formElementId)
                    formElement.ConfigInfo = UpdateFormElement(formElement, FormGroup.DisplayMode.View, item).ConfigInfo;
            }

        }

        private void UpdateFormElementInToSearchForm(ref FormGroup.Form form, DataListItem item)
        {
            string formElementId = ((Label)item.FindControl("NameLabel")).Text + "Property";

            foreach (FormGroup.FormElement formElement in form.LayoutInfo)
            {
                if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView")
                    formElement.ConfigInfo = CreateConfigInfoFromFieldConfig(this.UpdateGridView(formElement, FormGroup.DisplayMode.All, item).GetConfigInfo());
                else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid")
                    formElement.ConfigInfo = this.UpdateWebGrid(formElement, FormGroup.DisplayMode.All, item);
                else if (formElement.Id == formElementId)
                    formElement.ConfigInfo = UpdateFormElement(formElement, FormGroup.DisplayMode.All, item).ConfigInfo;
            }
        }

        private COEGridView UpdateGridView(FormGroup.FormElement gridViewFormElement, FormGroup.DisplayMode displayMode, DataListItem item)
        {
            string errorMsg; 
            string formElementName = ((Label)item.FindControl("NameLabel")).Text;
            string formElementId = ((Label)item.FindControl("NameLabel")).Text + "Property";
            COEGridView gridView = (COEGridView)COEFormGenerator.GetCOEGenerableControl(gridViewFormElement, out errorMsg);
            XmlNamespaceManager nsManager = new XmlNamespaceManager(gridView.GetConfigInfo().OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode gridViewConfInfo = gridView.GetConfigInfo();
            XmlNodeList tablesNodeList = gridViewConfInfo.SelectNodes("//COE:table", nsManager);
            foreach (XmlNode table in tablesNodeList)
            {
                XmlNodeList columsNodeList = table.SelectNodes("//COE:Column", nsManager);

                foreach (XmlNode column in columsNodeList)
                {
                    if (column.SelectSingleNode("./COE:formElement", nsManager) != null)
                    {
                        FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(column.SelectSingleNode("./COE:formElement", nsManager).OuterXml);
                        if (formElement.Id == formElementId)
                        {
                            formElement.ConfigInfo = this.UpdateFormElement(formElement, displayMode, item).ConfigInfo;
                            column.SelectSingleNode("./COE:headerText", nsManager).InnerText = formElement.Label;
                            formElement.Label = string.Empty;
                            if (column.Attributes["hidden"] == null)
                                column.Attributes.Append(column.OwnerDocument.CreateAttribute("hidden"));
                            column.Attributes["hidden"].Value = (!formElement.DisplayInfo.Visible).ToString();
                        }
                        else if (formElement.Name == formElementName)
                        {
                            formElement.Label = string.Empty;
                            if (column.Attributes["hidden"] == null)
                                column.Attributes.Append(column.OwnerDocument.CreateAttribute("hidden"));
                            column.Attributes["hidden"].Value = (!((CheckBox)item.FindControl("VisibleCheckBox")).Checked).ToString();
                        }
                    }
                }
            }

            gridView.LoadFromXml(gridViewConfInfo.OuterXml);
            return gridView;
        }
        private XmlNode UpdateWebGrid(FormGroup.FormElement webGridFormElement, FormGroup.DisplayMode displayMode, DataListItem item)
        {
            string formElementName = ((Label)item.FindControl("NameLabel")).Text;
            string formElementId = ((Label)item.FindControl("NameLabel")).Text + "Property";
            XmlNamespaceManager nsManager = new XmlNamespaceManager(webGridFormElement.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode webGridConfInfo = webGridFormElement.ConfigInfo;

            try
            {
                XmlNodeList tablesNodeList = webGridConfInfo.SelectNodes("//COE:table", nsManager);
                foreach (XmlNode table in tablesNodeList)
                {
                    XmlNodeList columsNodeList = table.SelectNodes("//COE:Column", nsManager);

                    foreach (XmlNode column in columsNodeList)
                    {
                        if (column.SelectSingleNode("./COE:formElement", nsManager) != null)
                        {
                            FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(column.SelectSingleNode("./COE:formElement", nsManager).OuterXml);
                            if (formElement.Id == formElementId)
                            {
                                formElement.ConfigInfo = this.UpdateFormElement(formElement, displayMode, item).ConfigInfo;
                                column.SelectSingleNode("./COE:headerText", nsManager).InnerText = formElement.Label;
                                formElement.Label = string.Empty;
                                if (column.Attributes["hidden"] == null)
                                {
                                    column.Attributes.Append(column.OwnerDocument.CreateAttribute("hidden"));
                                    column.Attributes["hidden"].Value = (!formElement.DisplayInfo.Visible).ToString();
                                }
                                else
                                    column.Attributes["hidden"].Value = (!formElement.DisplayInfo.Visible).ToString();

                            }
                            else if (formElement.Name == formElementName)
                            {
                                //CBOE-2078 -- Fix is to update the form element properties as the ID and label properties are null 
                                //due to which the 4003.xml form element header text is null which was causing a same ID error and misalignment. 
                                formElement.ConfigInfo = this.UpdateFormElement(formElement, displayMode, item).ConfigInfo;
                                column.SelectSingleNode("./COE:headerText", nsManager).InnerText = formElement.Label;
                                formElement.Label = string.Empty;
                                if (column.Attributes["hidden"] == null)
                                {
                                    column.Attributes.Append(column.OwnerDocument.CreateAttribute("hidden"));
                                    column.Attributes["hidden"].Value = (!((CheckBox)item.FindControl("VisibleCheckBox")).Checked).ToString();
                                }
                                else
                                    column.Attributes["hidden"].Value = (!((CheckBox)item.FindControl("VisibleCheckBox")).Checked).ToString();

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
            return webGridConfInfo;
        }

        private bool CheckIfRequired(string formElementId)
        {
            foreach (CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationProperty prop in ConfigurationRegistryRecord.PropertyList)
            {
                if (prop.Name == formElementId.Replace("Property", string.Empty))
                {

                    foreach (ValidationRule valRule in prop.ValidationRuleList)
                    {
                        if (valRule.Name.ToLower() == "requiredfield")
                            return true;
                    }
                }
            }
            foreach (CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationProperty prop in ConfigurationRegistryRecord.CompoundPropertyList)
            {
                if (prop.Name == formElementId.Replace("Property", string.Empty))
                {
                    foreach (ValidationRule valRule in prop.ValidationRuleList)
                    {
                        if (valRule.Name.ToLower() == "requiredfield")
                            return true;
                    }
                }
            }
            foreach (CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationProperty prop in ConfigurationRegistryRecord.BatchPropertyList)
            {
                if (prop.Name == formElementId.Replace("Property", string.Empty))
                {
                    foreach (ValidationRule valRule in prop.ValidationRuleList)
                    {
                        if (valRule.Name.ToLower() == "requiredfield")
                            return true;
                    }
                }
            }
            foreach (CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationProperty prop in ConfigurationRegistryRecord.BatchComponentList)
            {
                if (prop.Name == formElementId.Replace("Property", string.Empty))
                {
                    foreach (ValidationRule valRule in prop.ValidationRuleList)
                    {
                        if (valRule.Name.ToLower() == "requiredfield")
                            return true;
                    }
                }
            }

            return false;
        }

        private void GoToHome()
        {
            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Default.aspx");
        }

        private void GoBack()
        {
            if (ConfigurationRegistryRecord.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.PropertyList)
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/RootPropertyList.aspx");
            else if (ConfigurationRegistryRecord.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.None ||
            ConfigurationRegistryRecord.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.AddIns)
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Default.aspx");
            else
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/" + ConfigurationRegistryRecord.SelectedPropertyList.ToString() + ".aspx");
        }

        #endregion

        #region Event Handlers

        protected void LinkButtonGoToMain_Click(object sender, EventArgs e)
        {
            try
            {
                this.GoToHome();
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
                this.GoBack();
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
