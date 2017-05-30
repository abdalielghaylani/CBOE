using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COETableManager;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;
using CambridgeSoft.COE.RegistrationAdmin.Services.Common;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ConfigurationController : RegControllerBase
    {
        #region Custom tables
        private static JObject GetTableConfig(string tableName)
        {
            // Returns the field configuration
            // COETableEditorUtilities.getColumnList returns the column lists
            // COETableEditorUtilities.GetAlias returns the column aliases (labels)
            // COETableEditorUtilities.getLookupLocation returns the look-up information
            // This should also return user authorization
            // Refer to routines like COETableEditorUtilities.HasDeletePrivileges and COETableEditorUtilities.HasEditPrivileges

            return new JObject();
        }

        private static int SaveColumnValues(string tableName, JObject data, bool creating)
        {
            COETableEditorBOList.NewList().TableName = tableName;
            var idField = COETableEditorUtilities.getIdFieldName(tableName);
            var idFieldValue = creating ? null : data[idField].ToString();
            var tableEditorBO = creating ? COETableEditorBO.New() : COETableEditorBO.Get(int.Parse(idFieldValue));
            var columns = tableEditorBO.Columns;

            foreach (var column in columns)
                UpdateColumnValue(tableName, column, data);

            tableEditorBO.Columns = columns;
            foreach (var column in columns)
            {
                if (!COETableEditorUtilities.GetIsUniqueProperty(tableName, column.FieldName)) continue;
                if (!tableEditorBO.IsUniqueCheck(tableName, column.FieldName, column.FieldValue.ToString(), idField, idFieldValue))
                    throw new RegistrationException(string.Format("{0} should be unique", column.FieldName));
            }
            // Validate if view [VW_PICKLISTDOMAIN] Note : Add more to the list if contains SQL filters for update
            if (tableName.Equals(COETableManager.ValidateSqlQuery.VW_PICKLISTDOMAIN.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                foreach (var column in columns)
                {
                    var columnName = column.FieldName;
                    if (columnName.Equals("EXT_TABLE", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("EXT_ID_COL", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("EXT_DISPLAY_COL", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("EXT_SQL_FILTER", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("EXT_SQL_SORTORDER", StringComparison.OrdinalIgnoreCase))
                    {
                        var columnValue = column.FieldValue.ToString();
                        if (!string.IsNullOrEmpty(columnValue))
                            COETableEditorBO.Get(tableName, columns);
                        break;
                    }
                }
            }
            try
            {
                tableEditorBO = tableEditorBO.Save();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-00001"))
                    throw new RegistrationException("The value provided already exists");
                else
                    throw;
            }
            return tableEditorBO.ID;
        }

        private static void UpdateColumnValue(string tableName, Column column, JObject data)
        {
            var columnName = column.FieldName;
            var columnValue = (string)data[columnName];
            if (columnValue == null)
            {
                var defaultValue = COETableEditorUtilities.getDefaultValue(tableName, column.FieldName);
                column.FieldValue = !string.IsNullOrEmpty(defaultValue) ? defaultValue : null;
            }
            else if (COETableEditorUtilities.getIsStructure(tableName, column.FieldName))
            {
                column.FieldValue = ChemistryHelper.ConvertToCdx(columnValue);
            }
            else if (column.FieldType == DbType.Double)
            {
                double value;
                if (!double.TryParse(columnValue, out value))
                    throw new RegistrationException(string.Format("{0} needs a valid number", columnName));
                else
                    column.FieldValue = value;
            }
            else if (column.FieldType == DbType.Boolean)
            {
                bool value;
                if (!bool.TryParse(columnValue, out value))
                    throw new RegistrationException(string.Format("{0} needs a valid boolean value", columnName));
                else
                    column.FieldValue = value;
            }
            else if (column.FieldType == DbType.DateTime)
            {
                DateTime value;
                if (!DateTime.TryParse(columnValue, out value))
                    throw new RegistrationException(string.Format("{0} needs a valid date-time value", columnName));
                else
                    column.FieldValue = value;
            }
            else
            {
                column.FieldValue = columnValue;
            }
        }

        private bool CheckCustomPropertyName(ConfigurationRegistryRecord configurationBO, int formId, string propertyId)
        {
            if (string.IsNullOrEmpty(propertyId)) return false;
            var propertyName = propertyId.Replace("Property", string.Empty);
            var batchFormIngoreFields = new string[] { "FORMULA_WEIGHT", "BATCH_FORMULA", "PERCENT_ACTIVE" };
            if (formId == COEFormHelper.BATCHSUBFORMINDEX && batchFormIngoreFields.Contains(propertyName)) return false;
            var propertyList = formId == COEFormHelper.MIXTURESUBFORMINDEX ? configurationBO.PropertyList :
                formId == COEFormHelper.COMPOUNDSUBFORMINDEX ? configurationBO.CompoundPropertyList :
                formId == COEFormHelper.STRUCTURESUBFORMINDEX ? configurationBO.StructurePropertyList :
                formId == COEFormHelper.BATCHSUBFORMINDEX ? configurationBO.BatchPropertyList :
                configurationBO.BatchComponentList;
            var propertyListEnumerable = (IEnumerable<Property>)propertyList;
            return propertyListEnumerable.FirstOrDefault(p => p.Name == propertyName) != null;
        }

        private void PutCustomFormData(ConfigurationRegistryRecord configurationBO, int formId, List<FormGroup.FormElement> formElement, FormElementData formElementData)
        {
            foreach (var element in formElement)
            {
                element.Name = formElementData.Name;
                element.DisplayInfo.Type = formElementData.ControlType;
                element.Label = formElementData.Label;
                element.DisplayInfo.CSSClass = formElementData.CssClass;
                element.DisplayInfo.Visible = formElementData.Visible;
            }
        }

        private JArray GetCustomFormData(ConfigurationRegistryRecord configurationBO, int formId, List<FormGroup.FormElement> formElement)
        {
            var data = new JArray();
            foreach (var element in formElement)
            {
                if (!CheckCustomPropertyName(configurationBO, formId, element.Id)) continue;
                var name = element.Id.Replace("Property", string.Empty);
                if (string.IsNullOrEmpty(element.DisplayInfo.Assembly))
                {
                    var prop = new JObject(
                        new JProperty("name", name),
                        new JProperty("controlType", element.DisplayInfo.Type),
                        new JProperty("label", element.Label),
                        new JProperty("cssClass", element.DisplayInfo.CSSClass),
                        new JProperty("visible", element.DisplayInfo.Visible)
                    );
                    data.Add(prop);
                }
            }
            return data;
        }

        private JArray GetCustomFormData(ConfigurationRegistryRecord configurationBO, FormGroup.Form form)
        {
            var data = new JArray();
            if (form.LayoutInfo.Count > 0)
                data = new JArray(data.Union(GetCustomFormData(configurationBO, form.Id, form.LayoutInfo)));
            if (form.AddMode.Count > 0)
                data = new JArray(data.Union(GetCustomFormData(configurationBO, form.Id, form.AddMode)));
            if (form.EditMode.Count > 0)
                data = new JArray(data.Union(GetCustomFormData(configurationBO, form.Id, form.EditMode)));
            if (form.ViewMode.Count > 0)
                data = new JArray(data.Union(GetCustomFormData(configurationBO, form.Id, form.ViewMode)));
            return data;
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables")]
        [SwaggerOperation("GetCustomTables")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTables()
        {
            return await CallMethod(() =>
            {
                var tableList = new JArray();
                var tables = COETableEditorUtilities.getTables();
                foreach (var key in tables.Keys)
                {
                    var table = new JObject(
                        new JProperty("tableName", key),
                        new JProperty("label", tables[key])
                    );
                    tableList.Add(table);
                }
                return tableList;
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}")]
        [SwaggerOperation("GetCustomTableRows")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTableRows(string tableName)
        {
            return await CallMethod(() =>
            {
                var config = GetTableConfig(tableName);
                var rows = new JArray();
                COETableEditorBOList.NewList().TableName = tableName;
                var dt = COETableEditorBOList.getTableEditorDataTable(tableName);
                foreach (DataRow dr in dt.Rows)
                {
                    var p = new JObject();
                    foreach (DataColumn dc in dt.Columns)
                        p.Add(new JProperty(dc.ColumnName, dc.ColumnName.Equals("structure", StringComparison.OrdinalIgnoreCase) ? "fragment/" + dr[0].ToString() : dr[dc]));
                    rows.Add(p);
                }
                return new JObject(
                    new JProperty("config", config),
                    new JProperty("rows", rows)
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}/{id}")]
        [SwaggerOperation("GetCustomTableRow")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTableRow(string tableName, int id)
        {
            return await CallMethod(() =>
            {
                COETableEditorBOList.NewList().TableName = tableName;
                var dt = COETableEditorBOList.getTableEditorDataTable(tableName);
                var idField = COETableEditorUtilities.getIdFieldName(tableName);
                var selected = dt.Select(string.Format("{0}={1}", idField, id));
                if (selected == null || selected.Count() == 0)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the entry ID, {0}, in {1}", id, tableName));

                var dr = selected[0];
                var data = new JObject();
                foreach (DataColumn dc in dt.Columns)
                    data.Add(new JProperty(dc.ColumnName, dc.ColumnName.Equals("structure", StringComparison.OrdinalIgnoreCase) ? "fragment/" + dr[0].ToString() : dr[dc]));
                return data;
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}")]
        [SwaggerOperation("CreateCustomTableRow")]
        [SwaggerResponse(201, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateCustomTableRow(string tableName, JObject data)
        {
            return await CallMethod(() =>
            {
                if (!COETableEditorUtilities.HasAddPrivileges(tableName))
                    throw new UnauthorizedAccessException(string.Format("Not allowed to add entries to {0}", tableName));
                var id = SaveColumnValues(tableName, data, true);
                return new ResponseData(id, null, null, null);
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}/{id}")]
        [SwaggerOperation("UpdateCustomTableRow")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateCustomTableRow(string tableName, int id, JObject data)
        {
            return await CallMethod(() =>
            {
                if (!COETableEditorUtilities.HasEditPrivileges(tableName))
                    throw new UnauthorizedAccessException(string.Format("Not allowed to edit entries from {0}", tableName));
                SaveColumnValues(tableName, data, false);
                return new ResponseData(id, null, null, null);
            });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}/{id}")]
        [SwaggerOperation("DeleteCustomTableRow")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteCustomTableRow(string tableName, int id)
        {
            return await CallMethod(() =>
            {
                if (!COETableEditorUtilities.HasDeletePrivileges(tableName))
                    throw new UnauthorizedAccessException(string.Format("Not allowed to delete entries from {0}", tableName));
                COETableEditorBOList.NewList().TableName = tableName;
                // TODO: Should check if id is present.
                // If not, throw error 404.
                COETableEditorBO.Delete(id);
                return new ResponseData(id, null, null, null);
            });
        }

        #endregion

        #region Addins

        [HttpGet]
        [Route(Consts.apiPrefix + "addins")]
        [SwaggerOperation("GetAddins")]
        [SwaggerResponse(200, type: typeof(List<AddinData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetAddins()
        {
            return await CallMethod(() =>
            {
                var addinList = new List<AddinData>();
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                int counter = 0;
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    AddinData addinData = new AddinData();
                    addinData.Name = string.IsNullOrEmpty(addin.FriendlyName) ? counter.ToString() : addin.FriendlyName;
                    addinData.AddIn = addin.IsNew ? addin.ClassNameSpace + "." + addin.ClassName : addin.ClassName;
                    addinData.ClassName = addin.ClassName;
                    addinData.ClassNamespace = addin.ClassNameSpace;
                    addinData.Assembly = addin.Assembly;
                    addinData.Enable = addin.IsEnable;
                    addinData.Required = addin.IsRequired;
                    addinData.Configuration = addin.AddInConfiguration;

                    addinData.Events = new List<EventData>();
                    foreach (Event evt in addin.EventList)
                        addinData.Events.Add(new EventData(evt.EventName, evt.EventHandler));

                    addinList.Add(addinData);
                    counter++;
                }

                return addinList;
            });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "addins/{name}")]
        [SwaggerOperation("DeleteAddin")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteAddin(string name)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(name))
                    throw new RegistrationException("Invalid addin name");
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                bool found = false;
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    if (!addin.FriendlyName.Equals(name)) continue;
                    found = true;
                    configurationBO.AddInList.Remove(addin);
                    configurationBO.Save();
                    break;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The addin, {0}, was not found", name));
                return new ResponseData(message: string.Format("The addin, {0}, was deleted successfully.", name));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "addins")]
        [SwaggerOperation("CreateAddin")]
        [SwaggerResponse(201, type: typeof(AddinData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateAddin(AddinData data)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(data.Name))
                    throw new RegistrationException("Invalid addin name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                // check addin with friendly name already exist
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    if (addin.FriendlyName.Equals(data.Name))
                    {
                        throw new RegistrationException(string.Format("The addin {0} with same name already exists.", data.Name));
                    }
                }

                // check addin configuration is valid
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.LoadXml(data.Configuration);
                }
                catch
                {
                    throw new RegistrationException(string.Format("The addin {0}'s configuration is not valid.", data.Name));
                }
                if (xml.DocumentElement.FirstChild.Name == "AddInConfiguration")
                    throw new RegistrationException(string.Format("The addin {0}'s configuration is not valid.", data.Name));

                // get all events
                EventList eventList = EventList.NewEventList();
                foreach (PerkinElmer.COE.Registration.Server.Models.EventData evtItem in data.Events)
                {
                    Event evt = Event.NewEvent(evtItem.EventName, evtItem.EventHandler, true);
                    eventList.Add(evt);
                }

                AddIn addIn = AddIn.NewAddIn(data.Assembly, data.ClassName, data.Name, eventList,
                            data.Configuration, data.ClassNamespace, true, false);

                configurationBO.AddInList.Add(addIn);
                configurationBO.Save();

                return new ResponseData(message: string.Format("The addin, {0}, was saved successfully.", data.Name));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "addins")]
        [SwaggerOperation("UpdateAddin")]
        [SwaggerResponse(200, type: typeof(AddinData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateAddin(AddinData data)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(data.Name))
                    throw new RegistrationException("Invalid addin name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                bool found = false;
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    if (!addin.FriendlyName.Equals(data.Name)) continue;
                    found = true;

                    addin.Assembly = data.Assembly;
                    addin.IsEnable = data.Enable;
                    addin.AddInConfiguration = data.Configuration;

                    // clear existing events, if any
                    List<Event> markedEventsForDeletion = new List<Event>();
                    if (addin.EventList.Count > 0)
                    {
                        foreach (Event evt in addin.EventList)
                            markedEventsForDeletion.Add(evt);

                        foreach (Event evt in markedEventsForDeletion)
                            addin.EventList.Remove(evt);
                    }

                    // add new events
                    foreach (EventData evtData in data.Events)
                    {
                        Event evt = Event.NewEvent(evtData.EventName, evtData.EventHandler, true);
                        addin.EventList.Add(evt);
                    }

                    addin.ApplyEdit();
                    configurationBO.Save();

                    break;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The addin, {0}, was not found", data.Name));

                return new ResponseData(message: string.Format("The addin, {0}, was updated successfully.", data.Name));
            });
        }

        #endregion

        #region Forms

        [HttpGet]
        [Route(Consts.apiPrefix + "forms")]
        [SwaggerOperation("GetForms")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetForms()
        {
            return await CallMethod(() =>
            {
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var formList = new JArray();
                var formBO = COEFormBO.Get(4010);
                var detailsForms = formBO.COEFormGroup.DetailsForms;
                formList = new JArray(formList.Union(GetCustomFormData(configurationBO, formBO.GetForm(detailsForms, 0, COEFormHelper.MIXTURESUBFORMINDEX))));
                formList = new JArray(formList.Union(GetCustomFormData(configurationBO, formBO.GetForm(detailsForms, 0, COEFormHelper.COMPOUNDSUBFORMINDEX))));
                formList = new JArray(formList.Union(GetCustomFormData(configurationBO, formBO.GetForm(detailsForms, 0, COEFormHelper.STRUCTURESUBFORMINDEX))));
                formList = new JArray(formList.Union(GetCustomFormData(configurationBO, formBO.GetForm(detailsForms, 0, COEFormHelper.BATCHSUBFORMINDEX))));
                formList = new JArray(formList.Union(GetCustomFormData(configurationBO, formBO.GetForm(detailsForms, 0, COEFormHelper.BATCHCOMPONENTSUBFORMINDEX))));
                return formList.GroupBy(d => d["name"]).Select(g => g.First());
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "forms")]
        [SwaggerOperation("UpdateCustomForm")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateCustomForm(FormElementData formElementData)
        {
            return await CallMethod(() =>
            {
                bool found = false;
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                string[] customFromGroupsIds = RegAdminUtils.GetRegCustomFormGroupsIds();
                foreach (string formGroupId in customFromGroupsIds)
                {
                    var formBO = COEFormBO.Get(Convert.ToInt32(formGroupId));
                    var detailsForms = formBO.COEFormGroup.DetailsForms;
                    var ListForms = formBO.COEFormGroup.ListForms;
                    var QueryForms = formBO.COEFormGroup.QueryForms;
                    if (formElementData.FormGroupName == "RegistryProperties")
                    {
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(detailsForms, 0, COEFormHelper.MIXTURESUBFORMINDEX).EditMode, formElementData); }
                        catch { }
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(ListForms, 0, COEFormHelper.MIXTURESEARCHFORM).EditMode, formElementData); }
                        catch { }
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.MIXTURESEARCHFORM).EditMode, formElementData); }
                        catch { }
                    }
                    else if (formElementData.FormGroupName == "CompoundProperties")
                    {
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(detailsForms, 0, COEFormHelper.COMPOUNDSUBFORMINDEX).EditMode, formElementData); }
                        catch { }
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.COMPOUNDSEARCHFORM).EditMode, formElementData); }
                        catch { }
                    }
                    else if (formElementData.FormGroupName == "BaseFragmentProperties")
                    {
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(detailsForms, 0, COEFormHelper.STRUCTURESUBFORMINDEX).EditMode, formElementData); }
                        catch { }
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.STRUCTURESEARCHFORM).EditMode, formElementData); }
                        catch { }
                    }
                    else if (formElementData.FormGroupName == "BatchProperties")
                    {
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(detailsForms, 0, COEFormHelper.BATCHSUBFORMINDEX).EditMode, formElementData); }
                        catch { }
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.BATCHSEARCHFORM).EditMode, formElementData); }
                        catch { }
                    }
                    else if (formElementData.FormGroupName == "BatchComponentProperties")
                    {
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(detailsForms, 0, COEFormHelper.BATCHCOMPONENTSUBFORMINDEX).EditMode, formElementData); }
                        catch { }
                        try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.BATCHCOMPONENTSEARCHFORM).EditMode, formElementData); }
                        catch { }
                    }
                    try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.TEMPORARYBASEFORM).EditMode, formElementData); }
                    catch { }
                    try { PutCustomFormData(configurationBO, formBO.ID, formBO.GetForm(QueryForms, 0, COEFormHelper.TEMPORARYCHILDFORM).EditMode, formElementData); }
                    catch { }
                    formBO.Save();
                    found = true;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The form-element, {0}, was not found", formElementData.Name));
                return new ResponseData(null, null, string.Format("The form-elements was updated successfully"), null);
            });
        }
        #endregion

        #region Properties
        [HttpGet]
        [Route(Consts.apiPrefix + "properties")]
        [SwaggerOperation("GetProperties")]
        [SwaggerResponse(200, type: typeof(List<PropertyData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetProperties()
        {
            return await CallMethod(() =>
            {
                var propertyArray = new List<PropertyData>();
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
                foreach (var propertyType in propertyTypes)
                {
                    configurationBO.SelectedPropertyList = propertyType;
                    var propertyList = configurationBO.GetSelectedPropertyList;
                    if (propertyList == null) continue;
                    var properties = (IEnumerable<Property>)propertyList;
                    foreach (var property in properties)
                    {
                        var typeLabel = propertyType == ConfigurationRegistryRecord.PropertyListType.AddIns ? "Add-in" :
                            propertyType == ConfigurationRegistryRecord.PropertyListType.Batch ? "Batch" :
                            propertyType == ConfigurationRegistryRecord.PropertyListType.BatchComponent ? "Batch Component" :
                            propertyType == ConfigurationRegistryRecord.PropertyListType.Compound ? "Compound" :
                            propertyType == ConfigurationRegistryRecord.PropertyListType.PropertyList ? "Registry" :
                            propertyType == ConfigurationRegistryRecord.PropertyListType.Structure ? "Base Fragment" : "Extra Properties";

                        PropertyData propertyData = new PropertyData();
                        propertyData.Name = property.Name;
                        propertyData.TypeName = propertyType.ToString();
                        propertyData.TypeLabel = typeLabel;
                        propertyData.PickListDisplayValue = string.IsNullOrEmpty(property.PickListDisplayValue) ? string.Empty : property.PickListDisplayValue;
                        propertyData.PickListDomainId = string.IsNullOrEmpty(property.PickListDomainId) ? string.Empty : property.PickListDomainId; 
                        propertyData.Value = property.Value;
                        propertyData.DefaultValue = property.DefaultValue;
                        propertyData.Precision = string.IsNullOrEmpty(property.Precision) ? string.Empty : property.Precision; 
                        propertyData.SortOrder = property.SortOrder;
                        propertyData.SubType = property.SubType;
                        propertyData.FriendlyName = property.FriendlyName;
                        propertyData.ValidationRules = new List<ValidationRuleData>();

                        foreach (CambridgeSoft.COE.Registration.Services.Types.ValidationRule rule in property.ValRuleList)
                        {
                            ValidationRuleData ruleData = new ValidationRuleData();                         
                            ruleData.Name = rule.Name;
                            ruleData.Min = rule.MIN;
                            ruleData.Max = rule.MAX;
                            ruleData.MaxLength = rule.MaxLength;
                            ruleData.Error = rule.Error;
                            ruleData.DefaultValue = rule.DefaultValue;
                            ruleData.Parameters = new List<ParameterData>();

                            foreach (CambridgeSoft.COE.Registration.Services.BLL.Parameter param in rule.Parameters)
                                ruleData.Parameters.Add(new ParameterData(param.Name, param.Value));

                            propertyData.ValidationRules.Add(ruleData);
                        }

                        propertyArray.Add(propertyData);                       
                    }
                }
                return propertyArray;
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "properties")]
        [SwaggerOperation("CreateProperties")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateProperties(PropertyData data)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(data.Name))
                    throw new RegistrationException("Invalid property name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                bool duplicateExists = false;
                if (configurationBO.PropertyList.CheckExistingNames(data.Name.ToUpper(), true) || configurationBO.PropertyColumnList.Contains(data.Name.ToUpper()))
                    duplicateExists = true;
                else if (configurationBO.BatchPropertyList.CheckExistingNames(data.Name.ToUpper(), true) || configurationBO.BatchPropertyColumnList.Contains(data.Name.ToUpper()))
                    duplicateExists = true;
                else if (configurationBO.BatchComponentList.CheckExistingNames(data.Name.ToUpper(), true) || configurationBO.BatchComponentColumnList.Contains(data.Name.ToUpper()))
                    duplicateExists = true;
                else if (configurationBO.CompoundPropertyList.CheckExistingNames(data.Name.ToUpper(), true) || configurationBO.CompoundPropertyColumnList.Contains(data.Name.ToUpper()))
                    duplicateExists = true;
                else if (configurationBO.StructurePropertyList.CheckExistingNames(data.Name.ToUpper(), true) || configurationBO.StructurePropertyColumnList.Contains(data.Name.ToUpper()))
                    duplicateExists = true;

                if (duplicateExists)
                    throw new RegistrationException(string.Format("The property '{0}' already exists.", data.Name));

                var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
                foreach (var propertyType in propertyTypes)
                {
                    if (!propertyType.ToString().Equals(data.TypeName)) continue;

                    configurationBO.SelectedPropertyList = propertyType;
                    var propertyList = configurationBO.GetSelectedPropertyList;
                    if (propertyList == null) continue;
                    var properties = (IEnumerable<Property>)propertyList;

                    string prefix = string.Empty;
                    switch (configurationBO.SelectedPropertyList)
                    {
                        case ConfigurationRegistryRecord.PropertyListType.PropertyList:
                            prefix = RegAdminUtils.GetRegistryPrefix();
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.Batch:
                            prefix = RegAdminUtils.GetBatchPrefix();
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.Compound:
                            prefix = RegAdminUtils.GetComponentPrefix();
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.BatchComponent:
                            prefix = RegAdminUtils.GetBatchComponentsPrefix();
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.Structure:
                            prefix = RegAdminUtils.GetStructurePrefix();
                            break;
                    }

                    ConfigurationProperty confProperty = ConfigurationProperty.NewConfigurationProperty(prefix + data.Name.ToUpper(),
                                  data.Name.ToUpper(), data.TypeName, data.Precision, true, data.SubType, data.PickListDomainId);
                    configurationBO.GetSelectedPropertyList.AddProperty(confProperty);

                    break;
                }

                configurationBO.Save();

                return new ResponseData(message: string.Format("The property, {0}, was saved successfully.", data.Name));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "properties")]
        [SwaggerOperation("UpdateProperties")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateProperties(PropertyData data)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(data.Name.Trim()))
                    throw new RegistrationException("Invalid property name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                ConfigurationProperty selectedProperty = (ConfigurationProperty)configurationBO.GetSelectedPropertyList[data.Name];
                if (selectedProperty == null)
                    throw new RegistrationException(string.Format("The property, {0}, was not found", data.Name));

                selectedProperty.BeginEdit();

                if (!selectedProperty.Precision.Equals(data.Precision))
                {
                    switch (selectedProperty.Type)
                    {
                        case "NUMBER":
                            if (data.Precision.Contains(".") || data.Precision.Contains(","))
                                selectedProperty.Precision = data.Precision;
                            else
                                selectedProperty.Precision = data.Precision + ".0";

                            selectedProperty.Precision = RegAdminUtils.ConvertPrecision(selectedProperty.Precision, true);
                            break;
                        case "TEXT":
                            selectedProperty.Precision = data.Precision;
                            break;
                    }
                }

                if (selectedProperty.PrecisionIsUpdate)
                {
                    ValidationRuleList valRulesToDelete = selectedProperty.ValRuleList.Clone();
                    foreach (CambridgeSoft.COE.Registration.Services.Types.ValidationRule valRule in valRulesToDelete)
                        selectedProperty.ValRuleList.RemoveValidationRule(valRule.ID);
                    ((ConfigurationProperty)selectedProperty).AddDefaultRule();
                }

                selectedProperty.ApplyEdit();
                configurationBO.Save();

                return new ResponseData(message: string.Format("The property, {0}, was updated successfully.", data.Name));
            });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "properties/{name}")]
        [SwaggerOperation("DeleteProperties")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteProperties(string name)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(name))
                    throw new RegistrationException("Invalid property name");
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
                bool found = false;
                foreach (var propertyType in propertyTypes)
                {
                    configurationBO.SelectedPropertyList = propertyType;
                    var propertyList = configurationBO.GetSelectedPropertyList;
                    if (propertyList == null) continue;
                    var properties = (IEnumerable<Property>)propertyList;
                    foreach (var property in properties)
                    {
                        if (!property.Name.Equals(name)) continue;

                        found = true;
                        int index = propertyList.GetPropertyIndex(name);
                        propertyList.RemoveAt(index);
                        configurationBO.Save();
                        break;
                    }
                }

                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The property, {0}, was not found", name));
                return new ResponseData(message: string.Format("The property, {0}, was deleted successfully.", name));
            });
        }

        #endregion

        #region Settings
        [HttpGet]
        [Route(Consts.apiPrefix + "settings")]
        [SwaggerOperation("GetSettings")]
        [SwaggerResponse(200, type: typeof(List<SettingData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetSettings()
        {
            return await CallMethod(() =>
            {
                var settingList = new List<SettingData>();
                var currentApplicationName = RegUtilities.GetApplicationName();
                var appConfigSettings = FrameworkUtils.GetAppConfigSettings(currentApplicationName, true);
                var groups = appConfigSettings.SettingsGroup;
                foreach (var group in groups)
                {
                    var settings = group.Settings;
                    foreach (var setting in settings)
                    {
                        bool isAdmin;
                        if (bool.TryParse(setting.IsAdmin, out isAdmin) && isAdmin) continue;
                        settingList.Add(new SettingData(group, setting));
                    }
                }
                return settingList;
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "settings")]
        [SwaggerOperation("UpdateSetting")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateSetting([FromBody] SettingData data)
        {
            return await CallMethod(() =>
            {
                var settingInfo = "The setting {0} in {1}";
                var currentApplicationName = RegUtilities.GetApplicationName();
                var appConfigSettings = FrameworkUtils.GetAppConfigSettings(currentApplicationName, true);
                var groupName = (string)data.GroupName;
                if (string.IsNullOrEmpty(groupName))
                    throw new RegistrationException("The setting group must be specified");
                var settingName = (string)data.Name;
                if (string.IsNullOrEmpty(settingName))
                    throw new RegistrationException("The setting name must be specified");
                var settingValue = (string)data.Value;
                if (settingValue == null)
                    throw new RegistrationException("The setting value must be specified");
                bool found = false, updated = false;
                var groups = appConfigSettings.SettingsGroup;
                foreach (var group in groups)
                {
                    if (!group.Name.Equals(groupName)) continue;
                    var settings = group.Settings;
                    foreach (var setting in settings)
                    {
                        bool isAdmin;
                        if (bool.TryParse(setting.IsAdmin, out isAdmin) && isAdmin) continue;
                        if (!setting.Name.Equals(settingName)) continue;
                        found = true;
                        if (!setting.Value.Equals(settingValue))
                        {
                            updated = true;
                            setting.Value = settingValue;
                        }
                        break;
                    }
                    if (found) break;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("{0} was not found", settingInfo));
                if (!updated)
                    throw new IndexOutOfRangeException("No change is required");
                FrameworkUtils.SaveAppConfigSettings(currentApplicationName, appConfigSettings);
                return new ResponseData(null, null, string.Format("{0} was updated successfully", settingInfo), null);
            });
        }

        #endregion

        #region XML forms

        [HttpGet]
        [Route(Consts.apiPrefix + "xml-forms")]
        [SwaggerOperation("GetXmlForms")]
        [SwaggerResponse(200, type: typeof(List<XmlFormData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetXmlForms()
        {
            return await CallMethod(() =>
            {
                var xmlFormList = new List<XmlFormData>();
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var formGroupTypes = Enum.GetValues(typeof(COEFormHelper.COEFormGroups));
                foreach (COEFormHelper.COEFormGroups formGroupType in formGroupTypes)
                {
                    configRegRecord.COEFormHelper.Load(formGroupType);
                    xmlFormList.Add(new XmlFormData(formGroupType.ToString(), configRegRecord.FormGroup.ToString()));
                }
                return xmlFormList;
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "xml-forms")]
        [SwaggerOperation("UpdateXmlForm")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateXmlForm(XmlFormData data)
        {
            return await CallMethod(() =>
            {
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var formGroupTypes = Enum.GetValues(typeof(COEFormHelper.COEFormGroups));
                if (string.IsNullOrEmpty(data.Name))
                    throw new RegistrationException("Invalid form-group name");
                bool found = false;
                foreach (COEFormHelper.COEFormGroups formGroupType in formGroupTypes)
                {
                    if (!data.Name.Equals(formGroupType.ToString())) continue;
                    found = true;
                    configRegRecord.COEFormHelper.Load(formGroupType);
                    configRegRecord.COEFormHelper.SaveFormGroup(data.Data);
                    break;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The form-group, {0}, was not found", data.Name));
                return new ResponseData(null, null, string.Format("The form-group, {0}, was updated successfully", data.Name), null);
            });
        }

        #endregion
    }
}
