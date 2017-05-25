using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COETableManager;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;
using Newtonsoft.Json;
using System.Xml;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ConfigurationController : RegControllerBase
    {
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

        [HttpGet]
        [Route(Consts.apiPrefix + "addins")]
        [SwaggerOperation("GetAddins")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetAddins()
        {
            return await CallMethod(() =>
            {
                var addinList = new JArray();
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                for (int i = 0; i < configurationBO.AddInList.Count; i++)
                {
                    string realName;   
                    if (configurationBO.AddInList[i].IsNew)
                    {
                        realName = configurationBO.AddInList[i].ClassNameSpace + "." + configurationBO.AddInList[i].ClassName;
                    }
                    else
                    {
                        realName = configurationBO.AddInList[i].ClassName;
                    }

                    string friendlyName;
                    if (configurationBO.AddInList[i].FriendlyName != string.Empty)
                        friendlyName = configurationBO.AddInList[i].FriendlyName;
                    else
                        friendlyName = i.ToString();
                 
                    dynamic addin = new JObject();
                    addin.Name = friendlyName;
                    addin.AddIn = realName;
                    addin.Assembly = configurationBO.AddInList[i].Assembly;
                    addin.EventList = new JArray() as dynamic;

                    for (int j = 0; j < configurationBO.AddInList[i].EventList.Count; j++)
                    {
                        string eVent = configurationBO.AddInList[i].EventList[j].EventName + " - Event Handler = " + configurationBO.AddInList[i].EventList[j].EventHandler;
                        var eventItem = new JObject(new JProperty("Event Name", eVent));
                        addin.EventList.Add(eventItem);
                    }

                    addinList.Add(addin);
                }

                return addinList;
            });
        }

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

        [HttpGet]
        [Route(Consts.apiPrefix + "properties")]
        [SwaggerOperation("GetProperties")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetProperties()
        {
            return await CallMethod(() =>
            {
                var propertyArray = new JArray();
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
                        propertyArray.Add(new JObject(
                            new JProperty("typeName", propertyType.ToString()),
                            new JProperty("typeLabel", typeLabel),
                            new JProperty("name", property.Name)
                        ));
                    }
                }
                return propertyArray;
            });
        }

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
                    }
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("{0} was not found", settingInfo));
                if (!updated)
                    throw new IndexOutOfRangeException("No change is required");
                FrameworkUtils.SaveAppConfigSettings(currentApplicationName, appConfigSettings);
                return new ResponseData(null, null, string.Format("{0} was updated successfully", settingInfo), null);
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "xml-forms")]
        [SwaggerOperation("GetXmlForms")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetXmlForms()
        {
            return await CallMethod(() =>
            {
                var xmlFormList = new JArray();
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                foreach (COEFormHelper.COEFormGroups formGroupType in Enum.GetValues(typeof(COEFormHelper.COEFormGroups)))
                {
                    configRegRecord.COEFormHelper.Load(formGroupType);
                    xmlFormList.Add(new JObject(
                        new JProperty("name", formGroupType.ToString()),
                        new JProperty("data", configRegRecord.FormGroup.ToString())
                    ));
                }
                return xmlFormList;
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "xml-forms")]
        [SwaggerOperation("UpdateXmlForms")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateXmlForms(JObject data)
        {
            return await CallMethod(() =>
            {
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                XmlDocument datatoXml = JsonConvert.DeserializeXmlNode(data.ToString());
                configRegRecord.COEFormHelper.SaveFormGroup(datatoXml.InnerXml);
                return new ResponseData(null, null, "Selected FormGroup was saved successfuly", null);
            });
        }
    }
}
