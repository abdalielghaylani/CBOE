using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Web;
using System.Globalization;
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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Registration.Services.BLL;
using RegServicesTypes = CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ConfigurationController : RegControllerBase
    {
        #region Util methods

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

                if (column.FieldValue != null)
                {
                    if (!tableEditorBO.IsUniqueCheck(tableName, column.FieldName, column.FieldValue.ToString(), idField, idFieldValue))
                        throw new RegistrationException(string.Format("{0} should be unique", column.FieldName));
                }
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
                        if ((column.FieldValue != null) && (!string.IsNullOrEmpty(column.FieldValue.ToString())))
                        {
                            COETableEditorBO.Get(tableName, columns);
                            break;
                        }
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
            if (string.IsNullOrEmpty(columnValue))
            {
                if (tableName.Equals(COETableManager.ValidateSqlQuery.VW_PICKLISTDOMAIN.ToString(), StringComparison.OrdinalIgnoreCase)
                    && (columnName.Equals("EXT_SQL_FILTER") || columnName.Equals("EXT_SQL_SORTORDER") ||
                    columnName.Equals("EXT_TABLE") || columnName.Equals("EXT_DISPLAY_COL") || columnName.Equals("EXT_ID_COL")))
                {
                    // Fix for CBOE-5862
                    column.FieldValue = string.Empty;
                }
                else
                {
                    var defaultValue = COETableEditorUtilities.getDefaultValue(tableName, column.FieldName);
                    column.FieldValue = string.IsNullOrEmpty(defaultValue) ? null : defaultValue;
                }
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
                if (tableName.Equals(COETableManager.ValidateSqlQuery.VW_PICKLISTDOMAIN.ToString(), StringComparison.OrdinalIgnoreCase)
                    && columnName.Equals("EXT_SQL_FILTER"))
                    column.FieldValue = HttpUtility.HtmlEncode(columnValue);
                else
                    column.FieldValue = columnValue;
            }
        }

        #endregion

        #region Custom tables
        private class Lookup
        {
            public string DataField { get; set; }

            public JArray DataSource { get; set; }

            public Dictionary<string, string>.KeyCollection Keys { get; set; }
        }

        private JArray GetTableConfig(string tableName)
        {
            // Returns the field configuration
            var config = new JArray();
            // COETableEditorUtilities.getColumnList returns the column lists
            var columns = COETableEditorUtilities.getColumnList(tableName);
            var idFieldName = COETableEditorUtilities.getIdFieldName(tableName);
            foreach (var column in columns)
            {
                var fieldName = column.FieldName;
                var lookupTableName = COETableEditorUtilities.getLookupTableName(tableName, fieldName);
                var lookupColumns = COETableEditorUtilities.getLookupColumnList(tableName, fieldName);
                var label = COETableEditorUtilities.GetAlias(tableName, fieldName);
                var lookupFieldName = string.IsNullOrEmpty(lookupTableName) ? fieldName : lookupColumns[1].FieldName;
                if (label == null) label = lookupFieldName;
                var dataType = column.FieldType.ToString();
                var columnObj = new JObject(
                    new JProperty("dataField", fieldName),
                    new JProperty("caption", label),
                    new JProperty("dataType", dataType)
                );
                if (COETableEditorUtilities.GetHiddenProperty(tableName, fieldName))
                    columnObj.Add("allowEditing", false);
                if (fieldName.Equals(idFieldName)) columnObj.Add(new JProperty("visible", false));
                if (!string.IsNullOrEmpty(lookupTableName))
                {
                    var lookupField = lookupColumns[0].FieldName;
                    var lookupData = ExtractData(string.Format("SELECT {0},{1} FROM {2} WHERE {1} IS NOT NULL", lookupField, lookupColumns[1].FieldName, lookupTableName));
                    if (dataType == "Double")
                    {
                        foreach (var lookupRow in lookupData)
                        {
                            int lookupKey;
                            if (int.TryParse(lookupRow[lookupField].ToString(), out lookupKey))
                                lookupRow[lookupField] = lookupKey;
                        }
                    }
                    columnObj.Add("lookup", new JObject(
                        new JProperty("dataSource", lookupData),
                        new JProperty("valueExpr", lookupColumns[0].FieldName),
                        new JProperty("displayExpr", lookupColumns[1].FieldName))
                    );
                }

                // lookup location property of the table field is inner XML 
                var lookupLocation = COETableEditorUtilities.getLookupLocation(tableName, column.FieldName);
                if (!string.IsNullOrEmpty(lookupLocation) && lookupLocation.ToLower().Contains("innerxml_") &&
                    !COETableEditorUtilities.getIsStructureLookupField(tableName, column.FieldName))
                {
                    // get all lookup data from XML file and append to the configuration data object 
                    var columnList = COETableEditorUtilities.getId_Column_List(tableName, column.FieldName);
                    var lookups = new Dictionary<string, Lookup>();
                    var lookupData = ExtractData(columnList, column.FieldName);
                    var lookupField = column.FieldName + "_value";
                    if (dataType == "Double")
                    {
                        foreach (var lookupRow in lookupData)
                        {
                            int lookupKey;
                            if (int.TryParse(lookupRow[lookupField].ToString(), out lookupKey))
                                lookupRow[lookupField] = lookupKey;
                        }
                    }
                    columnObj.Add("lookup", new JObject(
                         new JProperty("dataSource", lookupData),
                         new JProperty("valueExpr", lookupField),
                         new JProperty("displayExpr", column.FieldName + "_name"))
                         );
                }

                var validationRuleList = COETableEditorUtilities.getValidationRuleList(tableName, fieldName);
                if (validationRuleList != null)
                {
                    columnObj.Add("validationRules", new JArray(
                        validationRuleList.Select(vr =>
                            {
                                var min = vr.Parameter.Where(p => p.Name == "min");
                                var max = vr.Parameter.Where(p => p.Name == "max");
                                return JObject.FromObject(
                                    new ValidationRuleData(
                                        vr.Name,
                                        min.Count() == 0 ? null : min.First().Value.ToString(),
                                        max.Count() == 0 ? null : max.First().Value.ToString(),
                                        0,
                                        vr.ErrorMessage,
                                        null,
                                        vr.Parameter.Select(p => new ValidationParameter(p.Name, p.Value)).ToList<ValidationParameter>()
                                    )
                                );
                            }
                        ).ToList<JObject>()
                    ));

                }
                config.Add(columnObj);
            }
            return config;
        }

        private bool CheckCustomPropertyName(ConfigurationRegistryRecord configurationBO, int formId, string propertyId)
        {
            if (string.IsNullOrEmpty(propertyId)) return false;
            var propertyName = propertyId.Replace("Property", string.Empty);
            var batchFormIngoreFields = new string[] { "FORMULA_WEIGHT", "BATCH_FORMULA", "PERCENT_ACTIVE" };
            if (formId == Consts.BATCHSUBFORMINDEX && batchFormIngoreFields.Contains(propertyName)) return false;
            var propertyList = formId == Consts.MIXTURESUBFORMINDEX ? configurationBO.PropertyList :
                formId == Consts.COMPOUNDSUBFORMINDEX ? configurationBO.CompoundPropertyList :
                formId == Consts.STRUCTURESUBFORMINDEX ? configurationBO.StructurePropertyList :
                formId == Consts.BATCHSUBFORMINDEX ? configurationBO.BatchPropertyList :
                configurationBO.BatchComponentList;
            var propertyListEnumerable = (IEnumerable<Property>)propertyList;
            return propertyListEnumerable.FirstOrDefault(p => p.Name == propertyName) != null;
        }

        private bool PutCustomFormData(ConfigurationRegistryRecord configurationBO, int formId, FormGroup.Form form, FormElementData formElementData)
        {
            bool found = false;
            if (form != null)
            {
                string controlStyle = RegAdminUtils.GetDefaultControlStyle(formElementData.ControlType, FormGroup.DisplayMode.Edit);
                var formElements = form.EditMode;
                foreach (var element in formElements)
                {
                    if (!element.Name.Equals(formElementData.Name)) continue;
                    found = true;

                    element.DisplayInfo.Type = formElementData.ControlType;
                    element.Label = formElementData.Label;
                    element.DisplayInfo.CSSClass = formElementData.CssClass;
                    element.DisplayInfo.Visible = formElementData.Visible == null || formElementData.Visible.Value;
                    if (element.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink")
                    {
                        if (element.BindingExpression.Contains("SearchCriteria"))
                            element.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox";
                    }

                    if (element.ConfigInfo["COE:fieldConfig"] != null && element.ConfigInfo["COE:fieldConfig"]["COE:CSSClass"] != null && !string.IsNullOrEmpty(controlStyle))
                        element.ConfigInfo["COE:fieldConfig"]["COE:CSSClass"].InnerText = controlStyle;

                    string defaultTextMode = formElementData.ControlType.Contains("COETextArea") ? "MultiLine" : string.Empty;
                    if (element.ConfigInfo["COE:fieldConfig"] != null && element.ConfigInfo["COE:fieldConfig"]["COE:TextMode"] != null)
                        element.ConfigInfo["COE:fieldConfig"]["COE:TextMode"].InnerText = defaultTextMode;
                    break;
                }

                formElements = form.AddMode;
                foreach (var element in formElements)
                {
                    if (!element.Name.Equals(formElementData.Name)) continue;
                    found = true;
                    element.DisplayInfo.Type = formElementData.ControlType;
                    element.Label = formElementData.Label;
                    element.DisplayInfo.CSSClass = formElementData.CssClass;
                    element.DisplayInfo.Visible = formElementData.Visible == null || formElementData.Visible.Value;
                    if (element.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink")
                    {
                        if (element.BindingExpression.Contains("SearchCriteria"))
                            element.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox";
                    }
                    break;
                }

                formElements = form.ViewMode;
                foreach (var element in formElements)
                {
                    if (!element.Name.Equals(formElementData.Name)) continue;
                    found = true;
                    element.DisplayInfo.Type = formElementData.ControlType;
                    element.Label = formElementData.Label;
                    element.DisplayInfo.CSSClass = formElementData.CssClass;
                    element.DisplayInfo.Visible = formElementData.Visible == null || formElementData.Visible.Value;
                    if (!element.DisplayInfo.Type.Contains("DropDownList") && !element.DisplayInfo.Type.Contains("DatePicker"))
                    {
                        if (!element.DisplayInfo.Type.Contains("NumericTextBox"))
                            element.DisplayInfo.Type += "ReadOnly";
                        else
                        {
                            if (element.ConfigInfo["COE:fieldConfig"] != null && element.ConfigInfo["COE:fieldConfig"]["COE:ReadOnly"] != null)
                                element.ConfigInfo["COE:fieldConfig"]["COE:ReadOnly"].InnerText = bool.TrueString;
                            else
                                element.ConfigInfo.FirstChild.AppendChild(element.ConfigInfo.OwnerDocument.CreateElement("COE:ReadOnly", element.ConfigInfo.NamespaceURI)).InnerText = bool.TrueString;
                        }
                    }
                    break;
                }

                formElements = form.LayoutInfo;
                foreach (var element in formElements)
                {
                    if (!string.IsNullOrEmpty(element.DisplayInfo.Assembly))
                        break;

                    if (element.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid")
                    {
                        XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(element.ConfigInfo.OwnerDocument.NameTable);
                        nameSpaceManager.AddNamespace("COE", "COE.FormGroup");
                        XmlNodeList tablesNodeList = element.ConfigInfo.SelectNodes("//COE:table", nameSpaceManager);
                        foreach (XmlNode table in tablesNodeList)
                        {
                            XmlNodeList columsNodeList = table.SelectNodes("//COE:Column", nameSpaceManager);

                            foreach (XmlNode column in columsNodeList)
                            {
                                if (column.SelectSingleNode("./COE:formElement", nameSpaceManager) == null) continue;

                                if (column.Attributes["name"] != null && column.Attributes["name"].Value != formElementData.Name) continue;

                                found = true;
                                column.SelectSingleNode("./COE:headerText", nameSpaceManager).InnerText = formElementData.Label;
                                var visible = formElementData.Visible.HasValue ? (bool)formElementData.Visible : false;
                                if (column.Attributes["hidden"] == null)
                                {
                                    column.Attributes.Append(column.OwnerDocument.CreateAttribute("hidden"));
                                    column.Attributes["hidden"].Value = (!visible).ToString();
                                }
                                else
                                {
                                    column.Attributes["hidden"].Value = (!visible).ToString();
                                }

                                FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(column.SelectSingleNode("./COE:formElement", nameSpaceManager).OuterXml);
                                if (formElement != null)
                                {
                                    formElement.DisplayInfo.CSSClass = formElementData.CssClass;
                                    formElement.DisplayInfo.Type = formElementData.Type;
                                    formElement.DisplayInfo.Visible = visible;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return found;
        }

        private List<FormElementData> GetCustomFormData(ConfigurationRegistryRecord configurationBO, string group, FormGroup.Form form, List<FormGroup.FormElement> formElement, int groupIndex)
        {
            var data = new List<FormElementData>();
            foreach (var element in formElement)
            {
                if (!CheckCustomPropertyName(configurationBO, form.Id, element.Id)) continue;
                var name = element.Id.Replace("Property", string.Empty);
                if (string.IsNullOrEmpty(element.DisplayInfo.Assembly))
                {
                    FormElementData formElementData = new FormElementData(group, element);
                    UpdateFormElementDetails(formElementData, element.Name, configurationBO, groupIndex);
                    data.Add(formElementData);
                }
            }
            return data;
        }

        private void UpdateFormElementDetails(FormElementData formElementData, string propertyName, ConfigurationRegistryRecord configurationBO, int groupIndex)
        {
            string propertyType = string.Empty;
            string propertySubType = string.Empty;

            switch (groupIndex)
            {
                case Consts.MIXTURESUBFORMINDEX:
                    foreach (Property prop in configurationBO.PropertyList)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case Consts.COMPOUNDSUBFORMINDEX:

                    foreach (Property prop in configurationBO.CompoundPropertyList)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case Consts.STRUCTURESUBFORMINDEX:

                    foreach (Property prop in configurationBO.StructurePropertyList)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case Consts.BATCHSUBFORMINDEX:

                    foreach (Property prop in configurationBO.BatchPropertyList)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case Consts.BATCHCOMPONENTSUBFORMINDEX:
                case Consts.BATCHCOMPONENTSEARCHFORM:
                    foreach (Property prop in configurationBO.BatchComponentList)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;
            }

            formElementData.Type = propertyType;
            formElementData.ControlTypeOptions = new List<KeyValuePair<string, string>>();
            formElementData.ControlEnabled = true;
            switch (propertyType)
            {
                case "NUMBER":
                case "TEXT":
                    if (!string.IsNullOrEmpty(propertySubType))
                    {
                        if (propertySubType == "URL")
                        {
                            formElementData.ControlEnabled = false;
                            formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("URL", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink"));
                        }
                    }
                    else
                    {
                        if (propertyType == "NUMBER")
                        {
                            formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("NumericTextBox", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox"));
                        }

                        formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("TextBox", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox"));
                        formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("TextArea", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea"));

                    } break;
                case "BOOLEAN":
                    formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("CheckBox", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBox"));
                    break;
                case "DATE":
                    formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("DatePicker", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker"));
                    break;
                case "PICKLISTDOMAIN":
                    formElementData.ControlTypeOptions.Add(new KeyValuePair<string, string>("unmodifiable", "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList"));
                    formElementData.ControlEnabled = false;
                    break;
            }
        }

        private List<FormElementData> GetCustomFormData(ConfigurationRegistryRecord configurationBO, string group, FormGroup.Form form, int groupIndex)
        {
            var data = new List<FormElementData>();

            if (form != null)
            {
                if (form.LayoutInfo.Count > 0)
                    data.AddRange(GetCustomFormData(configurationBO, group, form, form.LayoutInfo, groupIndex));
                if (form.AddMode.Count > 0)
                    data.AddRange(GetCustomFormData(configurationBO, group, form, form.AddMode, groupIndex));
                if (form.EditMode.Count > 0)
                    data.AddRange(GetCustomFormData(configurationBO, group, form, form.EditMode, groupIndex));
                if (form.ViewMode.Count > 0)
                    data.AddRange(GetCustomFormData(configurationBO, group, form, form.ViewMode, groupIndex));
            }
            return data;
        }

        private bool CheckCustomTablePropertyIsUsed(string tableName, string id)
        {
            var tablEditor = COETableEditorBO.New();
            foreach (var curCol in COETableEditorUtilities.getColumnList(tableName))
            {
                string isUsedCheckValue = COETableEditorUtilities.GetIsUsedCheckProperty(tableName, curCol.FieldName);
                if (!string.IsNullOrEmpty(isUsedCheckValue) && tablEditor.IsUsedCheck(isUsedCheckValue, id))
                    return true;
            }
            return false;
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
            }, new string[] { Consts.PrivilegeConfigReg });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}")]
        [SwaggerOperation("GetCustomTableRows")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTableRows(string tableName, bool? configOnly = false)
        {
            return await CallMethod(() =>
            {
                var config = GetTableConfig(tableName);
                var rows = new JArray();
                if (configOnly != null && !configOnly.Value)
                {
                    COETableEditorBOList.NewList().TableName = tableName;
                    var dt = COETableEditorBOList.getTableEditorDataTable(tableName);
                    // Find dc.ColumnName in config.lookup.displayExpr
                    var lookups = new Dictionary<string, Lookup>();
                    foreach (var item in config.Where(r => r["lookup"] != null))
                    {
                        lookups.Add(item["lookup"]["displayExpr"].ToString(),
                            new Lookup()
                            {
                                DataField = item["dataField"].ToString(),
                                DataSource = (JArray)item["lookup"]["dataSource"],
                                Keys = item["lookup"]["dataSource"].First().ToObject<Dictionary<string, string>>().Keys
                            });
                    }

                    var columnCount = config.Count();
                    if (dt.Columns.Count != columnCount)
                        throw new RegistrationException("Invalid column configuration");
                    foreach (DataRow dr in dt.Rows)
                    {
                        var row = new JObject();
                        for (var columnIndex = 0; columnIndex < columnCount; ++columnIndex)
                        {
                            var dc = dt.Columns[columnIndex];
                            var columnName = config[columnIndex]["dataField"].ToString();
                            var columnValue = dr[dc];
                            // Lookup column values need to be converted to keys
                            if (lookups.ContainsKey(dc.ColumnName))
                            {
                                var lookupData = lookups[dc.ColumnName];
                                if (columnValue != null)
                                {
                                    var match = lookupData.DataSource.Where(r => r[lookupData.Keys.ElementAt(1)].ToString().Equals(columnValue));
                                    if (match.Count() == 1)
                                    {
                                        columnValue = match.First()[lookupData.Keys.ElementAt(0)];
                                    }
                                }
                            }
                            if (columnName.Equals("structure", StringComparison.OrdinalIgnoreCase))
                            {
                                string structure = columnValue == null ? string.Empty : columnValue.ToString();
                                row.Add(new JProperty(columnName, "fragment/" + dr[0].ToString()));
                                row.Add(new JProperty("STRUCTURE_XML", structure));
                            }
                            else if (tableName.Equals(COETableManager.ValidateSqlQuery.VW_PICKLISTDOMAIN.ToString(), StringComparison.OrdinalIgnoreCase)
                                && columnName.Equals("EXT_SQL_FILTER") && columnValue != null)
                            {
                                row.Add(new JProperty(columnName, HttpUtility.HtmlDecode(columnValue.ToString())));
                            }
                            else
                            {
                                row.Add(new JProperty(columnName, columnValue));
                            }
                        }

                        rows.Add(row);
                    }
                }
                return new JObject(
                    new JProperty("config", config),
                    new JProperty("rows", rows)
                );
            }, new string[] { Consts.PrivilegeConfigReg, Consts.PRIVILEGEMANAGETABLES });
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
                {
                    if (dc.ColumnName.Equals("structure", StringComparison.OrdinalIgnoreCase))
                    {
                        string structure = dr[dc] == null ? string.Empty : dr[dc].ToString();
                        data.Add(new JProperty(dc.ColumnName, structure));
                    }
                    else
                    {
                        data.Add(new JProperty(dc.ColumnName, dr[dc]));
                    }
                }

                return data;
            }, new string[] { Consts.PrivilegeConfigReg });
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
            }, new string[] { Consts.PrivilegeConfigReg });
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
            }, new string[] { Consts.PrivilegeConfigReg });
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

                if (CheckCustomTablePropertyIsUsed(tableName, id.ToString()))
                    throw new InvalidOperationException("The entry is being used and therefore should not be deleted.");

                COETableEditorBOList.NewList().TableName = tableName;
                // TODO: Should check if id is present.
                // If not, throw error 404.
                COETableEditorBO.Delete(id);
                return new ResponseData(id, null, null, null);
            }, new string[] { Consts.PrivilegeConfigReg });
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
                    addinData.ClassName = addin.ClassName.Substring(addin.ClassName.LastIndexOf(".") + 1);
                    addinData.ClassNamespace = string.IsNullOrEmpty(addin.ClassNameSpace) ? string.Empty : addin.ClassNameSpace;
                    addinData.Assembly = addin.Assembly;
                    addinData.Enable = addin.IsEnable;
                    addinData.Required = addin.IsRequired;

                    // get innter text of <AddInConfiguration>
                    if (!string.IsNullOrWhiteSpace(addin.AddInConfiguration))
                    {
                        addinData.Configuration = RegAppHelper.TransformToPrettyPrintXML(addin.AddInConfiguration);
                    }

                    addinData.Events = new List<AddinEvent>();
                    foreach (Event evt in addin.EventList)
                        addinData.Events.Add(new AddinEvent(evt.EventName, evt.EventHandler));

                    addinList.Add(addinData);
                    counter++;
                }

                return addinList;
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_ADDINS" });
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
                if (string.IsNullOrWhiteSpace(name))
                    throw new RegistrationException("Invalid addin name");
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                bool found = false;
                int index = 0;
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    if (!addin.FriendlyName.Equals(name))
                    {
                        index++;
                        continue;
                    }
                    found = true;
                    configurationBO.AddInList.RemoveAt(index);
                    configurationBO.Save();
                    break;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The addin, {0}, was not found", name));
                return new ResponseData(message: string.Format("The addin, {0}, was deleted successfully!", name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_ADDINS" });
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
                    throw new RegistrationException("Invalid name");

                if (string.IsNullOrEmpty(data.Assembly))
                    throw new RegistrationException("Invalid assembly name");

                if (string.IsNullOrEmpty(data.ClassName))
                    throw new RegistrationException("Invalid class name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                // check addin with friendly name already exist
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    if (addin.FriendlyName.Equals(data.Name))
                    {
                        throw new RegistrationException(string.Format("The addin {0} with same name already exists.", data.Name));
                    }
                }

                if (data.Configuration != null)
                {
                    if (!data.Configuration.ToLower().StartsWith("<addinconfiguration>"))
                    {
                        // user input may not contain root xml node <AddInConfiguration>, in this base we insert the root note in the input data
                        data.Configuration = string.Format("<AddInConfiguration>{0}</AddInConfiguration>", data.Configuration);
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
                    if ((xml.DocumentElement.FirstChild != null) && (xml.DocumentElement.FirstChild.Name == "AddInConfiguration"))
                        throw new RegistrationException(string.Format("The addin {0}'s configuration is not valid.", data.Name));
                }
                // get all events
                EventList eventList = EventList.NewEventList();
                foreach (PerkinElmer.COE.Registration.Server.Models.AddinEvent evtItem in data.Events)
                {
                    Event evt = Event.NewEvent(evtItem.EventName, evtItem.EventHandler, true);
                    eventList.Add(evt);
                }

                string assembly = string.Empty;
                string nameSpace = string.Empty;
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                foreach (AddInAssembly assemblyItem in configRegRecord.GetAssemblyList.Assemblies)
                {
                    if (data.Assembly.Equals(assemblyItem.Name))
                    {
                        assembly = assemblyItem.FullName;
                        nameSpace = assemblyItem.ClassList.Find(r => r.Name == data.ClassName).NameSpace;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(nameSpace))
                {
                    throw new RegistrationException(string.Format("The addin assembly '{0}' is not valid", data.AddIn));
                }

                AddIn addIn = AddIn.NewAddIn(assembly, data.ClassName, data.Name, eventList, data.Configuration, nameSpace, true, false);

                configurationBO.AddInList.Add(addIn);
                configurationBO.Save();

                return new ResponseData(message: string.Format("The addin, {0}, was saved successfully!", data.Name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_ADDINS" });
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
                if (string.IsNullOrWhiteSpace(data.Name))
                    throw new RegistrationException("Invalid addin name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                bool found = false;

                int index = -1;
                foreach (AddIn addin in configurationBO.AddInList)
                {
                    index++;
                    if (!addin.FriendlyName.Equals(data.Name)) continue;

                    found = true;
                    addin.BeginEdit();
                    addin.IsEnable = data.Enable;
                    addin.ApplyEdit();
                    configurationBO.Save();
                    break;
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The addin, {0}, was not found", data.Name));
                else
                {
                    // Notes: updating configuration and events seperately because these fields are not updating with IsEnable field
                    var addinSelected = configurationBO.AddInList[index];

                    if (!string.IsNullOrWhiteSpace(data.Configuration))
                    {
                        if (!data.Configuration.ToLower().StartsWith("<addinconfiguration>"))
                        {
                            // user input may not contain root xml node <AddInConfiguration>, in this base we insert the root note in the input data
                            data.Configuration = string.Format("<AddInConfiguration>{0}</AddInConfiguration>", data.Configuration);
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
                        if ((xml.DocumentElement.FirstChild != null) && (xml.DocumentElement.FirstChild.Name == "AddInConfiguration"))
                            throw new RegistrationException(string.Format("The addin {0}'s configuration is not valid.", data.Name));

                    }
                    else
                    {
                        data.Configuration = "<AddInConfiguration></AddInConfiguration>";
                    }

                    addinSelected.AddInConfiguration = data.Configuration;

                    // clear existing events, if any
                    List<Event> markedEventsForDeletion = new List<Event>();
                    if (addinSelected.EventList.Count > 0)
                    {
                        foreach (Event evt in addinSelected.EventList)
                            markedEventsForDeletion.Add(evt);

                        foreach (Event evt in markedEventsForDeletion)
                            addinSelected.EventList.Remove(evt);
                    }

                    // add new events
                    foreach (AddinEvent evtData in data.Events)
                    {
                        Event evt = Event.NewEvent(evtData.EventName, evtData.EventHandler, true);
                        addinSelected.EventList.Add(evt);
                    }

                    addinSelected.ApplyEdit();
                    configurationBO.Save();
                }

                return new ResponseData(message: string.Format("The addin, {0}, was updated successfully!", data.Name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_ADDINS" });
        }

        #endregion

        #region Forms

        [HttpGet]
        [Route(Consts.apiPrefix + "forms")]
        [SwaggerOperation("GetForms")]
        [SwaggerResponse(200, type: typeof(List<FormElementData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetForms()
        {
            return await CallMethod(() =>
            {
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                string[] customFromGroupsIds = RegAdminUtils.GetRegCustomFormGroupsIds();
                var formList = new List<FormElementData>();
                foreach (string formGroupId in customFromGroupsIds)
                {
                    var formBO = COEFormBO.Get(Convert.ToInt32(formGroupId));
                    var detailsForms = formBO.COEFormGroup.DetailsForms;
                    var listForms = formBO.COEFormGroup.ListForms;
                    var queryForms = formBO.COEFormGroup.QueryForms;
                    var group = GetPropertyTypeLabel(ConfigurationRegistryRecord.PropertyListType.PropertyList);
                    formList.AddRange(GetCustomFormData(configurationBO, group, formBO.TryGetForm(detailsForms, 0, Consts.MIXTURESUBFORMINDEX), Consts.MIXTURESUBFORMINDEX));
                    group = GetPropertyTypeLabel(ConfigurationRegistryRecord.PropertyListType.Compound);
                    formList.AddRange(GetCustomFormData(configurationBO, group, formBO.TryGetForm(detailsForms, 0, Consts.COMPOUNDSUBFORMINDEX), Consts.COMPOUNDSUBFORMINDEX));
                    group = GetPropertyTypeLabel(ConfigurationRegistryRecord.PropertyListType.Structure);
                    formList.AddRange(GetCustomFormData(configurationBO, group, formBO.TryGetForm(detailsForms, 0, Consts.STRUCTURESUBFORMINDEX), Consts.STRUCTURESUBFORMINDEX));
                    group = GetPropertyTypeLabel(ConfigurationRegistryRecord.PropertyListType.Batch);
                    formList.AddRange(GetCustomFormData(configurationBO, group, formBO.TryGetForm(detailsForms, 0, Consts.BATCHSUBFORMINDEX), Consts.BATCHSUBFORMINDEX));
                    group = GetPropertyTypeLabel(ConfigurationRegistryRecord.PropertyListType.BatchComponent);
                    formList.AddRange(GetCustomFormData(configurationBO, group, formBO.TryGetForm(detailsForms, 0, Consts.BATCHCOMPONENTSUBFORMINDEX), Consts.BATCHCOMPONENTSUBFORMINDEX));
                    formList.AddRange(GetCustomFormData(configurationBO, group, formBO.TryGetForm(queryForms, 0, Consts.BATCHCOMPONENTSEARCHFORM), Consts.BATCHCOMPONENTSEARCHFORM));

                }
                return formList.GroupBy(d => d.Name).Select(g => g.First());
            }, new string[] { Consts.PrivilegeConfigReg, "CUSTOMIZE_FORMS" });
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
                    bool formGroupUpdated = false;
                    var formBO = COEFormBO.Get(Convert.ToInt32(formGroupId));
                    var detailsForms = formBO.COEFormGroup.DetailsForms;
                    var listForms = formBO.COEFormGroup.ListForms;
                    var queryForms = formBO.COEFormGroup.QueryForms;

                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(detailsForms, 0, Consts.MIXTURESUBFORMINDEX), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(detailsForms, 0, Consts.COMPOUNDSUBFORMINDEX), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(detailsForms, 0, Consts.STRUCTURESUBFORMINDEX), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(detailsForms, 0, Consts.BATCHSUBFORMINDEX), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(detailsForms, 0, Consts.BATCHCOMPONENTSUBFORMINDEX), formElementData))
                        formGroupUpdated = true;

                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.TEMPORARYBASEFORM), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.TEMPORARYCHILDFORM), formElementData))
                        formGroupUpdated = true;

                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(listForms, 0, Consts.MIXTURESEARCHFORM), formElementData))
                        formGroupUpdated = true;

                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.MIXTURESEARCHFORM), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.COMPOUNDSEARCHFORM), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.BATCHSEARCHFORM), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.BATCHCOMPONENTSEARCHFORM), formElementData))
                        formGroupUpdated = true;
                    if (PutCustomFormData(configurationBO, formBO.ID, formBO.TryGetForm(queryForms, 0, Consts.STRUCTURESEARCHFORM), formElementData))
                        formGroupUpdated = true;

                    found = found || formGroupUpdated;
                    if (formGroupUpdated)
                        formBO.Save();
                }
                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The form-element, {0}, was not found", formElementData.Name));
                return new ResponseData(null, null, string.Format("The form-elements was updated successfully!"), null);
            }, new string[] { Consts.PrivilegeConfigReg, "CUSTOMIZE_FORMS" });
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
                    var properties = ((IEnumerable<Property>)propertyList).OrderBy(x => x.SortOrder);

                    foreach (var property in properties)
                    {
                        PropertyData propertyData = new PropertyData();
                        propertyData.Name = property.Name;
                        propertyData.GroupName = propertyType.ToString();
                        propertyData.GroupLabel = GetPropertyTypeLabel(propertyType);
                        propertyData.Type = property.Type;
                        propertyData.PickListDisplayValue = string.IsNullOrEmpty(property.PickListDisplayValue) ? string.Empty : property.PickListDisplayValue;
                        propertyData.PickListDomainId = string.IsNullOrEmpty(property.PickListDomainId) ? string.Empty : property.PickListDomainId;
                        propertyData.Value = property.Value;
                        propertyData.DefaultValue = property.DefaultValue;

                        // set precision to empty for display purpose
                        propertyData.Precision = string.IsNullOrEmpty(property.Precision) ? string.Empty : property.Precision;
                        // set precision to empty for Boolean, Date, Picklist, url type. In DB, default precision for these types are 1
                        switch (propertyData.Type.ToUpper())
                        {
                            case "BOOLEAN":
                            case "DATE":
                            case "PICKLISTDOMAIN":
                            case "URL":
                                propertyData.Precision = string.Empty;
                                break;
                            case "NUMBER":
                                if (!string.IsNullOrEmpty(propertyData.Precision) && propertyData.Precision.Contains("."))
                                {
                                    propertyData.Precision = RegAdminUtils.ConvertPrecision(propertyData.Precision, false);
                                }
                                break;
                        }
                        propertyData.SortOrder = property.SortOrder;
                        propertyData.SubType = property.SubType;
                        propertyData.FriendlyName = property.FriendlyName;
                        propertyData.Editable = (property.Type == "NUMBER" || property.Type == "TEXT") ? true : false;
                        propertyData.ValidationRules = new List<ValidationRuleData>();

                        foreach (CambridgeSoft.COE.Registration.Services.Types.ValidationRule rule in property.ValRuleList)
                        {
                            ValidationRuleData ruleData = new ValidationRuleData();
                            ruleData.Name = rule.Name;
                            ruleData.Min = string.IsNullOrEmpty(rule.MIN) ? string.Empty : rule.MIN;
                            ruleData.Max = string.IsNullOrEmpty(rule.MAX) ? string.Empty : rule.MAX;
                            ruleData.MaxLength = rule.MaxLength;
                            ruleData.Error = string.IsNullOrEmpty(rule.Error) ? string.Empty : rule.Error;
                            ruleData.DefaultValue = rule.DefaultValue;
                            ruleData.Parameters = new List<ValidationParameter>();

                            foreach (CambridgeSoft.COE.Registration.Services.BLL.Parameter param in rule.Parameters)
                                ruleData.Parameters.Add(new ValidationParameter(param.Name, param.Value));

                            propertyData.ValidationRules.Add(ruleData);
                        }

                        propertyArray.Add(propertyData);
                    }
                }
                return propertyArray;
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_PROPERTIES" });
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
                if (string.IsNullOrWhiteSpace(data.Name))
                    throw new RegistrationException("Invalid property name");
                if (string.IsNullOrWhiteSpace(data.FriendlyName))
                    throw new RegistrationException("Invalid property FriendlyName");
                if (string.IsNullOrWhiteSpace(data.Type))
                    throw new RegistrationException("Invalid property type");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                string propertyName = data.Name.ToUpper();
                bool reserved = propertyName.Equals("DATE") ? true : propertyName.Equals("NUMBER") ? true :
                    propertyName.Equals("TEXT") ? true : propertyName.Equals("BOOLEAN") ? true :
                    propertyName.Equals("PICKLISTDOMAIN") ? true : configurationBO.DatabaseReservedWords.Contains(propertyName) ? true : false;
                if (reserved)
                    throw new RegistrationException(string.Format("Property name '{0}' is a reserved keyword.", data.Name));

                bool duplicateExists = false;
                if (configurationBO.PropertyList.CheckExistingNames(propertyName, true) || configurationBO.PropertyColumnList.Contains(propertyName))
                    duplicateExists = true;
                else if (configurationBO.BatchPropertyList.CheckExistingNames(propertyName, true) || configurationBO.BatchPropertyColumnList.Contains(propertyName))
                    duplicateExists = true;
                else if (configurationBO.BatchComponentList.CheckExistingNames(propertyName, true) || configurationBO.BatchComponentColumnList.Contains(propertyName))
                    duplicateExists = true;
                else if (configurationBO.CompoundPropertyList.CheckExistingNames(propertyName, true) || configurationBO.CompoundPropertyColumnList.Contains(propertyName))
                    duplicateExists = true;
                else if (configurationBO.StructurePropertyList.CheckExistingNames(propertyName, true) || configurationBO.StructurePropertyColumnList.Contains(propertyName))
                    duplicateExists = true;

                if (duplicateExists)
                    throw new RegistrationException(string.Format("The property '{0}' already exists.", data.Name));

                // set defaut values (These default values are hard coded in the UI in old Reg app)
                switch (data.Type.ToUpper())
                {
                    case "INTEGER":
                        data.Type = ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper();
                        data.Precision = string.IsNullOrEmpty(data.Precision) ? "9.0" : data.Precision;
                        break;
                    case "FLOAT":
                        data.Type = ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper();
                        data.Precision = string.IsNullOrEmpty(data.Precision) ? "8.6" : data.Precision;
                        break;
                    case "URL":
                        data.Type = ConfigurationRegistryRecord.PropertyTypeEnum.Text.ToString().ToUpper();
                        data.SubType = "URL";
                        data.Precision = string.IsNullOrEmpty(data.Precision) ? "200" : data.Precision;
                        break;
                    case "NUMBER":
                        data.Precision = string.IsNullOrEmpty(data.Precision) ? "8.0" : data.Precision;
                        break;
                    case "TEXT":
                        data.Precision = string.IsNullOrEmpty(data.Precision) ? "200" : data.Precision;
                        break;
                }

                // set default precision for other types like BOOLEAN, PICKLIST DOMAIN
                data.Precision = string.IsNullOrWhiteSpace(data.Precision) ? "1" : data.Precision;

                // if comma is given as a decimal separator, replce it with .
                data.Precision = data.Precision.Replace(",", ".");

                // below code will make sure that the input is a valid decimal numbe
                // example , if 10 is given as input, it will return 10.0
                decimal precision;
                if (!decimal.TryParse(data.Precision, NumberStyles.Number, CultureInfo.InvariantCulture, out precision))
                    throw new RegistrationException("Property precision is not a valid input.");
                data.Precision = precision.ToString();

                var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
                bool found = false;
                foreach (var propertyType in propertyTypes)
                {
                    if (!propertyType.ToString().Equals(data.GroupName)) continue;

                    configurationBO.SelectedPropertyList = propertyType;
                    var propertyList = configurationBO.GetSelectedPropertyList;
                    if (propertyList == null) continue;
                    found = true;

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

                    ConfigurationProperty confProperty = ConfigurationProperty.NewConfigurationProperty(
                        prefix + propertyName,
                        propertyName,
                        data.Type,
                        string.IsNullOrEmpty(data.Precision) ? "1" : data.Precision,
                        true,
                        string.IsNullOrEmpty(data.SubType) ? string.Empty : data.SubType,
                        data.PickListDomainId);
                    configurationBO.GetSelectedPropertyList.AddProperty(confProperty);

                    switch (configurationBO.SelectedPropertyList)
                    {
                        case ConfigurationRegistryRecord.PropertyListType.PropertyList:
                            if (string.IsNullOrEmpty(data.FriendlyName))
                                configurationBO.PropertiesLabels[0].Add(prefix + propertyName, data.Name);
                            else
                                configurationBO.PropertiesLabels[0].Add(prefix + propertyName, data.FriendlyName);
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.Compound:
                            if (string.IsNullOrEmpty(data.FriendlyName))
                                configurationBO.PropertiesLabels[1].Add(prefix + propertyName, data.Name);
                            else
                                configurationBO.PropertiesLabels[1].Add(prefix + propertyName, data.FriendlyName);
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.Batch:
                            if (string.IsNullOrEmpty(data.FriendlyName))
                                configurationBO.PropertiesLabels[2].Add(prefix + propertyName, data.Name);
                            else
                                configurationBO.PropertiesLabels[2].Add(prefix + propertyName, data.FriendlyName);
                            break;

                        case ConfigurationRegistryRecord.PropertyListType.BatchComponent:
                            if (string.IsNullOrEmpty(data.FriendlyName))
                                configurationBO.PropertiesLabels[3].Add(prefix + propertyName, data.Name);
                            else
                                configurationBO.PropertiesLabels[3].Add(prefix + propertyName, data.FriendlyName);
                            break;
                        case ConfigurationRegistryRecord.PropertyListType.Structure:
                            if (string.IsNullOrEmpty(data.FriendlyName))
                                configurationBO.PropertiesLabels[4].Add(prefix + propertyName, data.Name);
                            else
                                configurationBO.PropertiesLabels[4].Add(prefix + propertyName, data.FriendlyName);
                            break;
                    }

                    break;
                }

                if (found)
                    configurationBO.Save();
                else
                    throw new RegistrationException(string.Format("The property '{0}' not saved.", data.Name));

                return new ResponseData(message: string.Format("The property, {0}, was saved successfully!", data.Name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_PROPERTIES" });
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
                if (string.IsNullOrWhiteSpace(data.Name))
                    throw new RegistrationException("Invalid property name");

                foreach (ValidationRuleData ruleData in data.ValidationRules)
                {
                    switch (ruleData.Name.ToUpper())
                    {
                        case "NUMERICRANGE":
                        case "TEXTLENGTH":
                            if ((ruleData.Parameters == null) || (ruleData.Parameters.Count == 0))
                                throw new RegistrationException("This Validation Rule type must have min and max parameters.");

                            if (ruleData.Parameters.Count < 2)
                                throw new RegistrationException("This Validation Rule type must have min and max parameters.");

                            if (int.Parse(ruleData.Parameters[0].Value) > int.Parse(ruleData.Parameters[1].Value))
                                throw new RegistrationException("Max parameter must be greater than min parameter.");
                            break;
                        case "WORDLISTENUMERATION":
                            if ((ruleData.Parameters == null) || (ruleData.Parameters.Count < 1))
                                throw new RegistrationException("Please insert one parameter at least.");
                            break;
                        case "REQUIREDFIELD":
                            // no validation
                            break;
                        case "CUSTOM":
                            if ((ruleData.Parameters == null) || (ruleData.Parameters.Count == 0))
                                throw new RegistrationException("This Validation Rule type must have clientscript parameters.");

                            if (string.IsNullOrWhiteSpace(ruleData.Parameters[0].Value))
                                throw new RegistrationException("This Validation Rule type must have clientscript parameters.");
                            break;
                    }
                }

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
                ConfigurationProperty selectedProperty = null;
                foreach (var propertyType in propertyTypes)
                {
                    configurationBO.SelectedPropertyList = propertyType;
                    var propertyList = configurationBO.GetSelectedPropertyList;
                    if (propertyList == null) continue;
                    var properties = (IEnumerable<Property>)propertyList;
                    foreach (var property in properties)
                    {
                        if (!property.Name.Equals(data.Name)) continue;
                        selectedProperty = (ConfigurationProperty)property;
                        break;
                    }

                    if (selectedProperty != null)
                        break;
                }

                if (selectedProperty == null)
                    throw new RegistrationException(string.Format("The property, {0}, was not found", data.Name));

                var currenPrecision = selectedProperty.Precision;
                switch (selectedProperty.Type.ToUpper())
                {
                    case "NUMBER":
                        if (!string.IsNullOrEmpty(selectedProperty.Precision) && selectedProperty.Precision.Contains("."))
                        {
                            currenPrecision = RegAdminUtils.ConvertPrecision(selectedProperty.Precision, false);
                        }
                        break;
                }

                selectedProperty.BeginEdit();

                if ((!string.IsNullOrWhiteSpace(currenPrecision)) && (!currenPrecision.Equals(data.Precision)))
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
                    // when precision is updated all existing rules will be cleared and default rule is created
                    ValidationRuleList valRulesToDelete = selectedProperty.ValRuleList.Clone();
                    foreach (CambridgeSoft.COE.Registration.Services.Types.ValidationRule valRule in valRulesToDelete)
                        selectedProperty.ValRuleList.RemoveValidationRule(valRule.ID);
                    ((ConfigurationProperty)selectedProperty).AddDefaultRule();
                }
                else
                {
                    // there will not be a scenario like both precistion and rules will be updated at the same time
                    // remove all existing validation rules, and create new rules using the new rule data
                    ValidationRuleList valRulesToDelete = selectedProperty.ValRuleList.Clone();
                    foreach (CambridgeSoft.COE.Registration.Services.Types.ValidationRule valRule in valRulesToDelete)
                        selectedProperty.ValRuleList.RemoveValidationRule(valRule.ID);

                    foreach (ValidationRuleData ruleData in data.ValidationRules)
                    {
                        switch (ruleData.Name.ToUpper())
                        {
                            case "NUMERICRANGE":
                            case "TEXTLENGTH":
                                if (string.IsNullOrWhiteSpace(ruleData.Error))
                                {
                                    ruleData.Error = ruleData.Name.ToUpper() == "TEXTLENGTH" ?
                                        ruleData.Error = "The property value can have between {0} and {1} characters." :
                                        ruleData.Error = "This property can have at most {0} integer and {1} decimal digits.";
                                }
                                break;
                            case "WORDLISTENUMERATION":
                                if (string.IsNullOrWhiteSpace(ruleData.Error))
                                    ruleData.Error = "The word is not valid.";
                                break;
                            case "REQUIREDFIELD":
                                if (string.IsNullOrWhiteSpace(ruleData.Error))
                                    ruleData.Error = "The property value is required.";
                                selectedProperty.DefaultValue = ruleData.DefaultValue;
                                break;
                            case "CUSTOM":
                                if (string.IsNullOrWhiteSpace(ruleData.Error))
                                    ruleData.Error = "The Validation failed for this property.";
                                break;
                        }

                        ParameterList paramList = ParameterList.NewParameterList();
                        foreach (ValidationParameter param in ruleData.Parameters)
                            paramList.Add(CambridgeSoft.COE.Registration.Services.BLL.Parameter.NewParameter(param.Name, param.Value, true));

                        RegServicesTypes.ValidationRule validationRule = RegServicesTypes.ValidationRule.NewValidationRule(ruleData.Name, ruleData.Error, paramList, false);
                        validationRule.MIN = ruleData.Min;
                        validationRule.MAX = ruleData.Max;
                        validationRule.MaxLength = ruleData.MaxLength;
                        validationRule.DefaultValue = ruleData.DefaultValue;
                        configurationBO.SelectedPropertyName = selectedProperty.Name;
                        if (ruleData.Name.ToUpper() == "REQUIREDFIELD" && !configurationBO.IsPropValidToAddValidator)
                        {
                            configurationBO.DefalutValue = ruleData.DefaultValue;
                        }
                        selectedProperty.ValRuleList.Add(validationRule);
                    }
                }

                selectedProperty.ApplyEdit();
                configurationBO.Save();

                return new ResponseData(message: string.Format("The property, {0}, was updated successfully!", data.Name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_PROPERTIES" });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "properties/sortOrder")]
        [SwaggerOperation("UpdatePropertySortOrder")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdatePropertySortOrder(PropertyData data)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrWhiteSpace(data.Name))
                    throw new RegistrationException("Invalid property name");

                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
                ConfigurationProperty selectedProperty = null;
                foreach (var propertyType in propertyTypes)
                {
                    configurationBO.SelectedPropertyList = propertyType;
                    var propertyList = configurationBO.GetSelectedPropertyList;
                    if (propertyList == null) continue;
                    var properties = (IEnumerable<Property>)propertyList;
                    foreach (var property in properties)
                    {
                        if (!property.Name.Equals(data.Name)) continue;
                        selectedProperty = (ConfigurationProperty)property;
                        break;
                    }

                    if (selectedProperty != null)
                        break;
                }

                if (selectedProperty == null)
                    throw new RegistrationException(string.Format("The property, {0}, was not found", data.Name));

                // update sort order
                if (selectedProperty.SortOrder != data.SortOrder)
                {
                    selectedProperty.BeginEdit();
                    var selectedPropertyList = configurationBO.GetSelectedPropertyList;

                    bool moveUp = selectedProperty.SortOrder > data.SortOrder ? true : false;
                    int sortOrder = moveUp ? selectedProperty.SortOrder - 1 : selectedProperty.SortOrder + 1;
                    string affectedPropertyName = selectedPropertyList.ChangeOrder(sortOrder, moveUp, selectedProperty.Name);
                    Dictionary<string, string> propertyLabels = GetPropertyLabelsByContainedPropertyName(configurationBO, selectedProperty.Name);
                    if (propertyLabels != null)
                    {
                        propertyLabels.Add(selectedPropertyList[affectedPropertyName].Name,
                            selectedPropertyList[affectedPropertyName].FriendlyName);
                    }

                    selectedProperty.ApplyEdit();
                    configurationBO.Save();
                }

                return new ResponseData(message: string.Format("The property, {0}, was updated successfully!", data.Name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_PROPERTIES" });
        }

        private Dictionary<string, string> GetPropertyLabelsByContainedPropertyName(ConfigurationRegistryRecord configBO, string propertyName)
        {
            foreach (Dictionary<string, string> propertyLabels in configBO.PropertiesLabels)
            {
                if (propertyLabels.ContainsKey(propertyName))
                {
                    return propertyLabels;
                }
            }

            return null;
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

                        switch (configurationBO.SelectedPropertyList)
                        {
                            case ConfigurationRegistryRecord.PropertyListType.PropertyList:
                                configurationBO.PropertiesLabels[0].Remove(name);
                                break;
                            case ConfigurationRegistryRecord.PropertyListType.Compound:
                                configurationBO.PropertiesLabels[1].Remove(name);
                                break;
                            case ConfigurationRegistryRecord.PropertyListType.Batch:
                                configurationBO.PropertiesLabels[2].Remove(name);
                                break;

                            case ConfigurationRegistryRecord.PropertyListType.BatchComponent:
                                configurationBO.PropertiesLabels[3].Remove(name);
                                break;
                            case ConfigurationRegistryRecord.PropertyListType.Structure:
                                configurationBO.PropertiesLabels[4].Remove(name);
                                break;
                        }

                        configurationBO.Save();
                        if (!string.IsNullOrEmpty(configurationBO.GetSaveErrorMessage))
                        {
                            const string replaceString = ", ";
                            throw new RegistrationException(configurationBO.GetSaveErrorMessage.Replace(" <br/>", replaceString).Trim(replaceString.ToCharArray()));
                        }
                        break;
                    }
                }

                if (!found)
                    throw new IndexOutOfRangeException(string.Format("The property, {0}, was not found", name));
                return new ResponseData(message: string.Format("The property, {0}, was deleted successfully!", name));
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_PROPERTIES" });
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
                return RegAppHelper.RetrieveSettings();
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_SYSTEM_SETTINGS" });
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
                    throw new RegistrationException("No change is required");
                FrameworkUtils.SaveAppConfigSettings(currentApplicationName, appConfigSettings);
                const string enableAudtingSettingName = "EnableAuditing";
                if (settingName.Equals(enableAudtingSettingName))
                {
                    AuditingConfigurationProcessor processor = new AuditingConfigurationProcessor();
                    processor.Process(enableAudtingSettingName, string.Empty, settingValue);
                }
                return new ResponseData(null, null, string.Format("{0} was updated successfully!", settingInfo), null);
            }, new string[] { Consts.PrivilegeConfigReg, "MANAGE_SYSTEM_SETTINGS" });
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
            }, new string[] { Consts.PrivilegeConfigReg, "EDIT_FORM_XML" });
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
                return new ResponseData(null, null, string.Format("The form-group, {0}, was updated successfully!", data.Name), null);
            }, new string[] { Consts.PrivilegeConfigReg, "EDIT_FORM_XML" });
        }

        #endregion

        #region Configuration

        private void ExportConfigurationSettings(string currentExportDir)
        {
            XmlDocument confSettingsXml = new XmlDocument();
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            confSettingsXml.AppendChild(confSettingsXml.CreateElement("configurationSettings"));
            confSettingsXml.FirstChild.InnerXml = configRegRecord.GetConfigurationSettingsXml();
            WriteFile(currentExportDir, Consts.CONFIGSETTINGSFILENAME, true, confSettingsXml.OuterXml);
        }

        private void ExportCustomProperties(string currentExportDir)
        {
            var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            WriteFile(currentExportDir, Consts.COEOBJECTCONFIGFILENAME, true, configurationBO.ExportCustomizedProperties());
        }

        private void ExportForms(string currentExportDir)
        {
            string formsDir = currentExportDir + "\\" + Consts.COEFORMSFOLDERNAME;
            Directory.CreateDirectory(formsDir);
            foreach (COEFormBO coeFormBO in COEFormBOList.GetCOEFormBOList(null, null, COEAppName.Get(), null, true))
            {
                COEFormBO toExport = COEFormBO.Get(coeFormBO.ID);
                WriteFile(formsDir, coeFormBO.ID.ToString(), true, toExport.COEFormGroup.ToString());
            }
        }

        private void ExportDataViews(string currentExportDir)
        {
            string dataViewDir = currentExportDir + "\\" + Consts.COEDATAVIEWSFOLDERNAME;
            Directory.CreateDirectory(dataViewDir);
            foreach (COEDataViewBO coeDV in COEDataViewBOList.GetDataviewListForApplication(COEAppName.Get()))
            {
                WriteFile(dataViewDir, coeDV.ID.ToString(), true, coeDV.COEDataView.ToString());
            }
        }

        private void ExportTables(List<string> tableNames, string currentExportDir)
        {
            string tablesDir = currentExportDir + "\\" + Consts.COETABLESFORLDERNAME;
            var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            Directory.CreateDirectory(tablesDir);
            int tableNamePrefix = 1;
            foreach (string tableName in tableNames)
            {
                WriteFile(tablesDir, string.Format("{0:000}", tableNamePrefix++) + " - " + tableName, true, configurationBO.GetTable(tableName));
            }
        }

        private void WriteFile(string dir, string fileName, bool outputFormatted, string content)
        {
            XmlDocument document = new XmlDocument();
            if (fileName.Contains(".xml"))
            {
                fileName = fileName.Replace(".xml", string.Empty);
            }
            using (XmlTextWriter tw = new XmlTextWriter(dir + "\\" + fileName + ".xml", Encoding.UTF8))
            {
                tw.Formatting = outputFormatted ? Formatting.Indented : Formatting.None;
                document.LoadXml(content);
                document.Save(tw);
            }
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "configuration-paths")]
        [SwaggerOperation("GetConfigurationPath")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetConfigurationPath()
        {
            return await CallMethod(() =>
            {
                string FixedInstallPath = "Registration";
                Page page = new Page();
                string AppRootInstallPath = page.Server.MapPath(string.Empty).Remove(page.Server.MapPath(string.Empty).IndexOf(FixedInstallPath) + FixedInstallPath.Length);
                string AppDrive = HttpContext.Current.Server.MapPath(string.Empty).Remove(2);
                string CurrentDate = DateTime.Now.ToString("yy-MM-dd HH_mm_ss");
                var exportPath = AppDrive + Consts.EXPORTFILESPATH + CurrentDate;
                var importPath = AppRootInstallPath + Consts.IMPORTFILESPATH;
                return new JObject(
                        new JProperty("ExportPath", exportPath),
                        new JProperty("ImportPath", importPath));
            }, new string[] { Consts.PrivilegeConfigReg });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "configuration-export")]
        [SwaggerOperation("ExportConfiguration")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> ExportConfiguration(ExportConfigurationData data)
        {
            return await CallMethod(() =>
            {
                Directory.CreateDirectory(data.ExportDir);
                ExportConfigurationSettings(data.ExportDir);
                ExportCustomProperties(data.ExportDir);
                ExportForms(data.ExportDir);
                ExportDataViews(data.ExportDir);
                if (data.SelectNone != true)
                    ExportTables(data.TableNames, data.ExportDir);
                return new ResponseData(message: string.Format("The configuration was exported successfully!"));
            }, new string[] { Consts.PrivilegeConfigReg });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "configuration-import")]
        [SwaggerOperation("ImportConfiguration")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> ImportConfiguration(ImportConfigurationData data)
        {
            return await CallMethod(() =>
            {
                string FixedInstallPath = "Registration";
                Page page = new Page();
                var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                string AppRootInstallPath = page.Server.MapPath(string.Empty).Remove(page.Server.MapPath(string.Empty).IndexOf(FixedInstallPath) + FixedInstallPath.Length);
                configurationBO.ImportCustomization(AppRootInstallPath, data.ServerPath, data.ForceImport);
                return new ResponseData(message: string.Format("The configuration was imported successfully!"));
            }, new string[] { Consts.PrivilegeConfigReg });
        }
        #endregion
    }
}
